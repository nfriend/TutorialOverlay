using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Test
{
    public class BorderAdorner : Adorner
    {
        public BorderAdorner(UIElement targetElement) : base(targetElement) { }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            if (this.AdornedElement is FrameworkElement)
            {
                adornedElementRect.Width = ((FrameworkElement)this.AdornedElement).ActualWidth;
                adornedElementRect.Height = ((FrameworkElement)this.AdornedElement).ActualHeight;
            }

            drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 1), adornedElementRect);
        }
    }
}
