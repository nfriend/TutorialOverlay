using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HelpOverlay.Model;
using HelpOverlay.ViewModel;
using HelpOverlays;

namespace HelpOverlay
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ClickyRecordingWindow : Window
    {
        public ClickyRecordingWindow()
        {
            InitializeComponent();

            ClickyRecordingViewModel vm = new ClickyRecordingViewModel();
            this.DataContext = vm;

            TutorialManager.InjectOverlayIntoMainWindow();
        }

        public static ClickyRecordingWindow crWindow;

        public static UIElement adornedElement;

        public static void GoBabyGo()
        {
            crWindow = new ClickyRecordingWindow();
            crWindow.Show();

            Application.Current.MainWindow.AddHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler((o, e) =>
                {
                    if (TutorialManager.CurrentTutorial == null)
                    {
                        FrameworkElement element = e.OriginalSource as FrameworkElement;

                        if (element != null)
                        {
                            //Label l = new Label() { Content = PathToString(GenerateTypePath(GetNearestLogicalParent(element))) };
                            //Label l = new Label() { Content = PathToString(GenerateTypePath(element)) };
                            //DockPanel.SetDock(l, Dock.Top);
                            ((ClickyRecordingViewModel)crWindow.DataContext).RecordedClicks.Add(new Step(GenerateTypePath(element)));
                        }
                    }
                }), true);

            Application.Current.MainWindow.AddHandler(UIElement.PreviewMouseMoveEvent, new MouseEventHandler((o, e) =>
            {
                if (TutorialManager.CurrentTutorial == null)
                {
                    FrameworkElement element = e.OriginalSource as FrameworkElement;

                    if (element != null)
                    {
                        //element = GetNearestLogicalParent(element);

                        if (element != adornedElement)
                        {
                            if (adornedElement != null)
                            {
                                AdornerLayer oldElementLayer = AdornerLayer.GetAdornerLayer(adornedElement);

                                if (oldElementLayer != null)
                                {
                                    Adorner[] toRemoveArray = oldElementLayer.GetAdorners(adornedElement);
                                    if (toRemoveArray != null)
                                    {
                                        foreach (Adorner a in toRemoveArray)
                                        {
                                            if (a is BorderAdorner)
                                                oldElementLayer.Remove(a);
                                        }
                                    }
                                }
                            }

                            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
                            if (layer != null)
                            {
                                BorderAdorner ba = new BorderAdorner(element);
                                ba.IsHitTestVisible = false;
                                layer.Add(ba);
                            }

                            adornedElement = element;
                        }
                    }
                }
            }), true);
        }

        public static List<TypeIndexAssociation> GenerateTypePath(FrameworkElement element)
        {
            List<TypeIndexAssociation> typePath = new List<TypeIndexAssociation>();
            while (element != null)
            {
                int index = 0;

                if (VisualTreeHelper.GetParent(element) != null)
                {
                    DependencyObject parent = VisualTreeHelper.GetParent(element);

                    if (parent is Panel)
                    {
                        index = ((Panel)parent).Children.IndexOf(element);
                    }
                }

                typePath.Add(new TypeIndexAssociation() { ElementType = element.GetType(), Index = index });

                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }

            typePath.Reverse();
            return typePath;
        }

        public static string PathToString(List<TypeIndexAssociation> path)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < path.Count; i++)
            {
                sb.Append(path[i].ElementType.ToString());
                if (path[i].Index != 0)
                {
                    sb.Append(" at index ");
                    sb.Append(path[i].Index);
                }

                if (i != path.Count - 1)
                {
                    sb.Append(" -> ");
                }
            }

            return sb.ToString();
        }

        public static FrameworkElement GetNearestLogicalParent(FrameworkElement element)
        {
            DependencyObject parent = element;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);

                if (parent is FrameworkElement)
                {
                    if (((FrameworkElement)parent).Parent != null && ((FrameworkElement)parent).Parent is FrameworkElement)
                    {
                        return (FrameworkElement)((FrameworkElement)parent).Parent;
                    }
                }
            }

            return element;
        }
    }
}
