using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HelpOverlay
{
    public class TutorialManager
    {
        private static Dictionary<string, Tutorial> _tutorials;
        public static Dictionary<string, Tutorial> Tutorials
        {
            get 
            {
                if (_tutorials == null)
                {
                    _tutorials = new Dictionary<string, Tutorial>();
                }
                return _tutorials; 
            }
            set 
            { _tutorials = value; }
        }

        private static Tutorial _currentTutorial;
        public static Tutorial CurrentTutorial
        {
            get { return _currentTutorial; }
            set { _currentTutorial = value; }
        }

        private static HelpOverlayControl _overlay;
        public static HelpOverlayControl Overlay
        {
            get
            {
                return _overlay;
            }
            private set
            {
                _overlay = value;
            }
        }

        private static bool _overlayHasBeenInjected;
        public static bool OverlayHasBeenInjected
        {
            get { return _overlayHasBeenInjected; }
            set { _overlayHasBeenInjected = value; }
        }
        

        //public static readonly DependencyProperty IsTutorialOverlayCompatibleProperty = DependencyProperty.RegisterAttached("IsTutorialOverlayCompatible", typeof(Boolean), typeof(TutorialManager), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, IsTutorialOverlayCompatibleChanged));
        //public static void SetIsTutorialOverlayCompatible(UIElement element, Boolean value)
        //{
        //    element.SetValue(IsTutorialOverlayCompatibleProperty, value);
        //}
        //public static Boolean GetIsTutorialOverlayCompatible(UIElement element)
        //{
        //    return (Boolean)element.GetValue(IsTutorialOverlayCompatibleProperty);
        //}

        public static void InjectOverlayIntoMainWindow()
        {
            if (OverlayHasBeenInjected)
            {
                return;
            }
            
            Window window = Application.Current.MainWindow;

            Grid newRootElement = new Grid();
            newRootElement.Name = "HelpOverlayRoot";
            if (window.Content as UIElement != null)
            {
                UIElement currentContent = (UIElement)window.Content;
                window.Content = null;
                newRootElement.Children.Add(currentContent);
                newRootElement.Children.Add(new HelpOverlayControl());
                window.Content = newRootElement;

                Overlay = (HelpOverlayControl)HelpOverlyHelper.FindChild(newRootElement, "PART_HelpOverlayUserControl");

                OverlayHasBeenInjected = true;
            }
            else
            {
                Console.WriteLine("HelpOverlay cannot inject its overlay control into a window that defines its root as a DataTemplate.  In order to use HelpOverlay, ensure that the current Window's Content property is a UIElement.");
            }
        }

        //public static void IsTutorialOverlayCompatibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if ((Boolean)e.NewValue == true)
        //    {
        //        if (sender as Window != null)
        //        {
        //            Window window = (Window)sender;
        //            window.Loaded += new RoutedEventHandler((o, eargs) =>
        //                {
        //                    Grid newRootElement = new Grid();
        //                    newRootElement.Name = "HelpOverlayRoot";
        //                    if (window.Content as UIElement != null)
        //                    {
        //                        UIElement currentContent = (UIElement)window.Content;
        //                        window.Content = null;
        //                        newRootElement.Children.Add(currentContent);
        //                        newRootElement.Children.Add(new HelpOverlayControl());
        //                        window.Content = newRootElement;

        //                        Overlay = (HelpOverlayControl)HelpOverlyHelper.FindChild(newRootElement, "PART_HelpOverlayUserControl");
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("HelpOverlay cannot inject its overlay control into a window that defines its root as a DataTemplate.  In order to use HelpOverlay, ensure that the current Window's Content property is a UIElement.");
        //                    }
        //                });
        //        }
        //    }
        //}
    }
}
