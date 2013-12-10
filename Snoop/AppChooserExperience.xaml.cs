using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections;
using System.Linq;
using System.Windows;

namespace Snoop
{
    /// <summary>
    /// Interaction logic for AppChooserExperience.xaml
    /// </summary>
    public partial class AppChooserExperience : Window
    {
        static AppChooserExperience()
        {
            AppChooserExperience.RefreshCommand.InputGestures.Add(new KeyGesture(Key.F5));
        }
        public AppChooserExperience()
        {
            this.windowsView = CollectionViewSource.GetDefaultView(this.windows);

            this.InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(AppChooserExperience.RefreshCommand, this.HandleRefreshCommand));
            this.CommandBindings.Add(new CommandBinding(AppChooserExperience.InspectCommand, this.HandleInspectCommand));
            this.CommandBindings.Add(new CommandBinding(AppChooserExperience.MinimizeCommand, this.HandleMinimizeCommand));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, this.HandleCloseCommand));

            this.Refresh();
        }

        public static readonly RoutedCommand InspectCommand = new RoutedCommand();
        public static readonly RoutedCommand RefreshCommand = new RoutedCommand();
        public static readonly RoutedCommand MinimizeCommand = new RoutedCommand();

        public ICollectionView Windows
        {
            get { return this.windowsView; }
        }
        private ICollectionView windowsView;
        private ObservableCollection<TMWindowInfo> windows = new ObservableCollection<TMWindowInfo>();

        public void Refresh()
        {
            this.windows.Clear();

            Dispatcher.BeginInvoke
            (
                System.Windows.Threading.DispatcherPriority.Loaded,
                (DispatcherOperationCallback)delegate
                {
                    try
                    {
                        Mouse.OverrideCursor = Cursors.Wait;

                        foreach (IntPtr windowHandle in NativeMethods.ToplevelWindows)
                        {
                            TMWindowInfo window = new TMWindowInfo(windowHandle);
                            if (window.IsValidProcess && !this.HasProcess(window.OwningProcess))
                            {
                                new TMAttachFailedHandler(window, this);
                                this.windows.Add(window);
                            }
                        }

                        if (this.windows.Count > 0)
                            this.windowsView.MoveCurrentTo(this.windows[0]);
                    }
                    finally
                    {
                        Mouse.OverrideCursor = null;
                    }
                    return null;
                },
                null
            );
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            try
            {
                // load the window placement details from the user settings.
                WINDOWPLACEMENT wp = (WINDOWPLACEMENT)Properties.Settings.Default.AppChooserWindowPlacement;
                wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == Win32.SW_SHOWMINIMIZED ? Win32.SW_SHOWNORMAL : wp.showCmd);
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                Win32.SetWindowPlacement(hwnd, ref wp);
            }
            catch
            {
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // persist the window placement details to the user settings.
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            Win32.GetWindowPlacement(hwnd, out wp);
            Properties.Settings.Default.AppChooserWindowPlacement = wp;
            Properties.Settings.Default.Save();
        }


        private bool HasProcess(Process process)
        {
            foreach (TMWindowInfo window in this.windows)
                if (window.OwningProcess.Id == process.Id)
                    return true;
            return false;
        }

        private void HandleInspectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            TMWindowInfo window = (TMWindowInfo)this.windowsView.CurrentItem;
            if (window != null)
                window.Snoop();
        }

        private void HandleRefreshCommand(object sender, ExecutedRoutedEventArgs e)
        {
            // clear out cached process info to make the force refresh do the process check over again.
            TMWindowInfo.ClearCachedProcessInfo();
            this.Refresh();
        }
        private void HandleMinimizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private void HandleCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void HandleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }

    public class TMWindowInfo
    {
        public TMWindowInfo(IntPtr hwnd)
        {
            this.hwnd = hwnd;
        }

        public static void ClearCachedProcessInfo()
        {
            TMWindowInfo.processIDToValidityMap.Clear();
        }

        public event EventHandler<TMAttachFailedEventArgs> AttachFailed;

        public IEnumerable<NativeMethods.MODULEENTRY32> Modules
        {
            get
            {
                if (_modules == null)
                    _modules = GetModules().ToArray();
                return _modules;
            }
        }
        /// <summary>
        /// Similar to System.Diagnostics.WinProcessManager.GetModuleInfos,
        /// except that we include 32 bit modules when Snoop runs in 64 bit mode.
        /// See http://blogs.msdn.com/b/jasonz/archive/2007/05/11/code-sample-is-your-process-using-the-silverlight-clr.aspx
        /// </summary>
        private IEnumerable<NativeMethods.MODULEENTRY32> GetModules()
        {
            int processId;
            NativeMethods.GetWindowThreadProcessId(hwnd, out processId);

            var me32 = new NativeMethods.MODULEENTRY32();
            var hModuleSnap = NativeMethods.CreateToolhelp32Snapshot(NativeMethods.SnapshotFlags.Module | NativeMethods.SnapshotFlags.Module32, processId);
            if (!hModuleSnap.IsInvalid)
            {
                using (hModuleSnap)
                {
                    me32.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(me32);
                    if (NativeMethods.Module32First(hModuleSnap, ref me32))
                    {
                        do
                        {
                            yield return me32;
                        } while (NativeMethods.Module32Next(hModuleSnap, ref me32));
                    }
                }
            }
        }
        private IEnumerable<NativeMethods.MODULEENTRY32> _modules;

        public bool IsValidProcess
        {
            get
            {
                bool isValid = false;
                try
                {
                    if (this.hwnd == IntPtr.Zero)
                        return false;

                    Process process = this.OwningProcess;
                    if (process == null)
                        return false;

                    // see if we have cached the process validity previously, if so, return it.
                    if (TMWindowInfo.processIDToValidityMap.TryGetValue(process.Id, out isValid))
                        return isValid;

                    // else determine the process validity and cache it.
                    if (process.Id == Process.GetCurrentProcess().Id)
                    {
                        isValid = false;

                        // the above line stops the user from snooping on snoop, since we assume that ... that isn't their goal.
                        // to get around this, the user can bring up two snoops and use the second snoop ... to snoop the first snoop.
                        // well, that let's you snoop the app chooser. in order to snoop the main snoop ui, you have to bring up three snoops.
                        // in this case, bring up two snoops, as before, and then bring up the third snoop, using it to snoop the first snoop.
                        // since the second snoop inserted itself into the first snoop's process, you can now spy the main snoop ui from the
                        // second snoop (bring up another main snoop ui to do so). pretty tricky, huh! and useful!
                    }
                    else
                    {
                        // a process is valid to snoop if it contains a dependency on PresentationFramework, PresentationCore, or milcore (wpfgfx).
                        // this includes the files:
                        // PresentationFramework.dll, PresentationFramework.ni.dll
                        // PresentationCore.dll, PresentationCore.ni.dll
                        // wpfgfx_v0300.dll (WPF 3.0/3.5)
                        // wpfgrx_v0400.dll (WPF 4.0)

                        // note: sometimes PresentationFramework.dll doesn't show up in the list of modules.
                        // so, it makes sense to also check for the unmanaged milcore component (wpfgfx_vxxxx.dll).
                        // see for more info: http://snoopwpf.codeplex.com/Thread/View.aspx?ThreadId=236335

                        // sometimes the module names aren't always the same case. compare case insensitive.
                        // see for more info: http://snoopwpf.codeplex.com/workitem/6090

                        foreach (var module in Modules)
                        {
                            if
                            (
                                module.szModule.StartsWith("PresentationFramework", StringComparison.OrdinalIgnoreCase) ||
                                module.szModule.StartsWith("PresentationCore", StringComparison.OrdinalIgnoreCase) ||
                                module.szModule.StartsWith("wpfgfx", StringComparison.OrdinalIgnoreCase)
                            )
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }

                    TMWindowInfo.processIDToValidityMap[process.Id] = isValid;
                }
                catch (Exception) { }
                return isValid;
            }
        }
        public Process OwningProcess
        {
            get { return NativeMethods.GetWindowThreadProcess(this.hwnd); }
        }
        public IntPtr HWnd
        {
            get { return this.hwnd; }
        }
        private IntPtr hwnd;
        public string Description
        {
            get
            {
                Process process = this.OwningProcess;
                return process.MainWindowTitle + " - " + process.ProcessName + " [" + process.Id.ToString() + "]";
            }
        }
        public override string ToString()
        {
            return this.Description;
        }

        public void Snoop()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Injector.Launch(this.HWnd, typeof(HelpOverlay.ClickyRecordingWindow).Assembly, typeof(HelpOverlay.ClickyRecordingWindow).FullName, "GoBabyGo");
            }
            catch (Exception e)
            {
                OnFailedToAttach(e);
            }
            Mouse.OverrideCursor = null;
        }

        private void OnFailedToAttach(Exception e)
        {
            var handler = AttachFailed;
            if (handler != null)
            {
                handler(this, new TMAttachFailedEventArgs(e, this.Description));
            }
        }

        private static Dictionary<int, bool> processIDToValidityMap = new Dictionary<int, bool>();
    }

    public class TMAttachFailedEventArgs : EventArgs
    {
        public Exception AttachException { get; private set; }
        public string WindowName { get; private set; }

        public TMAttachFailedEventArgs(Exception attachException, string windowName)
        {
            AttachException = attachException;
            WindowName = windowName;
        }
    }

    public class TMAttachFailedHandler
    {
        public TMAttachFailedHandler(TMWindowInfo window, AppChooserExperience appChooserExperience = null)
        {
            window.AttachFailed += OnSnoopAttachFailed;
            _appChooserExperience = appChooserExperience;
        }

        private void OnSnoopAttachFailed(object sender, TMAttachFailedEventArgs e)
        {
            System.Windows.MessageBox.Show
            (
                string.Format
                (
                    "Failed to attach to {0}. Exception occured:{1}{2}",
                    e.WindowName,
                    Environment.NewLine,
                    e.AttachException.ToString()
                ),
                "Can't attach to the process!"
            );
            if (_appChooserExperience != null)
            {
                // TODO This should be implmemented through the event broker, not like this.
                _appChooserExperience.Refresh();
            }
        }

        private AppChooserExperience _appChooserExperience;
    }
}
