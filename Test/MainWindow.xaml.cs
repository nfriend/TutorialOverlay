using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static MainWindow tempWindow;

        public static UIElement adornedElement;

        public static void GoBabyGo()
        {
            tempWindow = new MainWindow();
            tempWindow.Show();

            Application.Current.MainWindow.AddHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler((o, e) =>
                {
                    FrameworkElement element = e.OriginalSource as FrameworkElement;

                    if (element != null)
                    {
                        //Label l = new Label() { Content = PathToString(GenerateTypePath(GetNearestLogicalParent(element))) };
                        Label l = new Label() { Content = PathToString(GenerateTypePath(element)) };
                        DockPanel.SetDock(l, Dock.Top);
                        tempWindow.MainStackPanel.Children.Add(l);
                    }
                }), true);

            Application.Current.MainWindow.AddHandler(UIElement.PreviewMouseMoveEvent, new MouseEventHandler((o, e) =>
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

    public class TypeIndexAssociation
    {
        public Type ElementType { get; set; }
        public int Index { get; set; }
    }
}


