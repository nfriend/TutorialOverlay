using System.Windows;
using System.Windows.Media;

namespace HelpOverlay
{
    public class HelpOverlyHelper
    {
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
