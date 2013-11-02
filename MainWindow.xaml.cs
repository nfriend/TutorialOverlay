using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelpOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double CUTOUT_MARGIN = 20;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Overlay.Visibility != Visibility.Visible)
            {
                Overlay.Visibility = Visibility.Visible;
            }
            else
            {
                Overlay.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RectangleGeometry hole = this.Resources["Hole"] as RectangleGeometry;
            FrameworkElement myButton = FindChild(Application.Current.MainWindow, "myButton");
            
            if (hole != null && myButton != null)
            {
                Point topLeft = myButton.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);
                hole.Rect = new Rect(topLeft.X - CUTOUT_MARGIN, topLeft.Y - CUTOUT_MARGIN, myButton.ActualWidth + CUTOUT_MARGIN * 2, myButton.ActualHeight + CUTOUT_MARGIN * 2);
            }
        }

        public static FrameworkElement FindChild(DependencyObject rootElement, string childName)
        {
            if (rootElement == null) return null;

            FrameworkElement rootElementAsFrameworkElement = rootElement as FrameworkElement;
            if (rootElementAsFrameworkElement != null && rootElementAsFrameworkElement.Name == childName)
            {
                return rootElementAsFrameworkElement;
            }
            else
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(rootElement);
                for (int i = 0; i < childrenCount; i++)
                {
                    FrameworkElement fe = FindChild(VisualTreeHelper.GetChild(rootElement, i), childName);
                    if (fe != null)
                        return fe;
                }
            }

            return null;
        }
    }
}
