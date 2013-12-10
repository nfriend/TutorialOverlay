using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HelpOverlay
{
    /// <summary>
    /// Interaction logic for HelpOverlay.xaml
    /// </summary>
    public partial class HelpOverlayControl : UserControl
    {
        public HelpOverlayControl()
        {
            InitializeComponent();

            Message.LayoutUpdated += new System.EventHandler((o, e) =>
            {
                UpdateArrow();
            });

            HolePath.LayoutUpdated += new System.EventHandler((o, e) =>
            {
                UpdateArrow();
            });

        }

        private const double CUTOUT_MARGIN = 20;
        Duration duration = new Duration(TimeSpan.FromMilliseconds(300));
        Duration halfDuration = new Duration(TimeSpan.FromMilliseconds(150));
        private string lastTarget = string.Empty;

        public void UpdateOverlay()
        {
            if (TutorialManager.CurrentTutorial != null && TutorialManager.CurrentTutorial.CurrentStep != null)
            {
                RectangleGeometry hole = this.Resources["Hole"] as RectangleGeometry;
                //FrameworkElement target = HelpOverlyHelper.FindChild(Application.Current.MainWindow, TutorialManager.CurrentTutorial.CurrentStep.TargetElementName);
                FrameworkElement target = GetElement(null, Application.Current.MainWindow, TutorialManager.CurrentTutorial.CurrentStep.Path, 0);

                if (target != null)
                {
                    target.PreviewMouseDown -= previewMouseDown;
                    target.PreviewMouseDown += previewMouseDown;
                }

                if (hole != null && target != null)
                {
                    Point targetTopLeft = target.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);
                    Rect newRect = new Rect(targetTopLeft.X - CUTOUT_MARGIN, targetTopLeft.Y - CUTOUT_MARGIN, target.ActualWidth + CUTOUT_MARGIN * 2, target.ActualHeight + CUTOUT_MARGIN * 2);

                    double topDistance = newRect.Top;
                    double bottomDistance = Application.Current.MainWindow.ActualHeight - newRect.Bottom;
                    double leftDistance = newRect.Left;
                    double rightDistance = Application.Current.MainWindow.ActualWidth - newRect.Right;

                    if (topDistance > bottomDistance && topDistance > leftDistance && topDistance > rightDistance)
                        TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement = Placement.Above;
                    if (bottomDistance > topDistance && bottomDistance > leftDistance && bottomDistance > rightDistance)
                        TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement = Placement.Below;
                    if (leftDistance > bottomDistance && leftDistance > topDistance && leftDistance > rightDistance)
                        TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement = Placement.Left;
                    if (rightDistance > bottomDistance && rightDistance > leftDistance && rightDistance > topDistance)
                        TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement = Placement.Right;

                    if (hole.Rect != Rect.Empty || TutorialManager.CurrentTutorial.CurrentStep.ToString() == lastTarget)
                    {
                        RectAnimation rectAnimation = new RectAnimation(newRect, duration);
                        rectAnimation.EasingFunction = new CubicEase() { EasingMode = System.Windows.Media.Animation.EasingMode.EaseInOut };

                        DoubleAnimation fadeInWaitAnimation = new DoubleAnimation(0, 0, duration);

                        fadeInWaitAnimation.Completed += new EventHandler((o, e) =>
                        {
                            DoubleAnimation fadeInFinalAnimation = new DoubleAnimation(0, 1, duration);
                            Message.BeginAnimation(Border.OpacityProperty, fadeInFinalAnimation);
                            Arrow.BeginAnimation(Arrow.OpacityProperty, fadeInFinalAnimation);
                        });
                        //fadeInAnimation.BeginTime = halfDuration.TimeSpan;

                        hole.BeginAnimation(RectangleGeometry.RectProperty, rectAnimation);
                        Arrow.BeginAnimation(Arrow.OpacityProperty, fadeInWaitAnimation);
                        Message.BeginAnimation(Border.OpacityProperty, fadeInWaitAnimation);
                    }
                    else
                    {
                        hole.Rect = newRect;
                    }

                    MessageTextBox.Text = TutorialManager.CurrentTutorial.CurrentStep.Message;

                    if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Right)
                    {
                        double leftPlacement = targetTopLeft.X + target.ActualWidth + CUTOUT_MARGIN;
                        Canvas.SetLeft(MessageContainer, leftPlacement);
                        Canvas.SetRight(MessageContainer, 0);
                        Canvas.SetTop(MessageContainer, 0);
                        Canvas.SetBottom(MessageContainer, 0);
                        var size = Application.Current.MainWindow.ActualWidth - leftPlacement;
                        if (size < 0)
                        {
                            size *= -1;
                        }
                        MessageContainer.Width = size;
                        MessageContainer.Height = Application.Current.MainWindow.ActualHeight;
                    }
                    else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Left)
                    {
                        double rightPlacement = targetTopLeft.X - CUTOUT_MARGIN;
                        Canvas.SetRight(MessageContainer, rightPlacement);
                        Canvas.SetLeft(MessageContainer, 0);
                        Canvas.SetTop(MessageContainer, 0);
                        Canvas.SetBottom(MessageContainer, 0);
                        var size = rightPlacement;
                        if (size < 0)
                        {
                            size *= -1;
                        }
                        MessageContainer.Width = size;
                        MessageContainer.Height = Application.Current.MainWindow.ActualHeight;
                    }
                    else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Above)
                    {
                        double bottomPlacement = targetTopLeft.Y - CUTOUT_MARGIN;
                        Canvas.SetBottom(MessageContainer, bottomPlacement);
                        Canvas.SetLeft(MessageContainer, 0);
                        Canvas.SetRight(MessageContainer, 0);
                        Canvas.SetTop(MessageContainer, 0);
                        var size = bottomPlacement;
                        if (size < 0)
                        {
                            size *= -1;
                        }
                        MessageContainer.Width = Application.Current.MainWindow.ActualWidth;
                        MessageContainer.Height = size;
                    }
                    else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Below)
                    {
                        double topPlacement = targetTopLeft.Y + target.ActualHeight + CUTOUT_MARGIN;
                        Canvas.SetTop(MessageContainer, topPlacement);
                        Canvas.SetLeft(MessageContainer, 0);
                        Canvas.SetRight(MessageContainer, 0);
                        Canvas.SetBottom(MessageContainer, 0);
                        var size = Application.Current.MainWindow.ActualHeight - topPlacement;
                        if (size < 0)
                        {
                            size *= -1;
                        }
                        MessageContainer.Width = Application.Current.MainWindow.ActualWidth;
                        MessageContainer.Height = size;
                    }

                    lastTarget = TutorialManager.CurrentTutorial.CurrentStep.ToString();
                }
            }
        }

        private void previewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TutorialManager.CurrentTutorial != null)
            {
                TutorialManager.CurrentTutorial.GoToNextStep.Execute(null);
            }

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOverlay();
        }

        public void UpdateArrow()
        {
            if (TutorialManager.CurrentTutorial != null && TutorialManager.CurrentTutorial.CurrentStep != null)
            {
                //FrameworkElement target = HelpOverlyHelper.FindChild(Application.Current.MainWindow, TutorialManager.CurrentTutorial.CurrentStep.TargetElementName);
                FrameworkElement target = GetElement(null, Application.Current.MainWindow, TutorialManager.CurrentTutorial.CurrentStep.Path, 0);

                if (target != null)
                {
                    Point targetTopLeft = target.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);

                    if (TutorialManager.CurrentTutorial != null && TutorialManager.CurrentTutorial.CurrentStep != null)
                    {
                        if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Right)
                        {
                            double leftPlacement = targetTopLeft.X + target.ActualWidth + CUTOUT_MARGIN;
                            Point messageTopLeft = Message.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);

                            Arrow.X1 = leftPlacement + 15;
                            Arrow.Y1 = targetTopLeft.Y + (target.ActualHeight / 2);
                            Arrow.X2 = messageTopLeft.X - 15;
                            Arrow.Y2 = messageTopLeft.Y + (Message.ActualHeight / 2);

                            if (targetTopLeft.Y + (target.ActualHeight / 2) > Application.Current.MainWindow.ActualHeight / 2)
                                Arrow.CurveDirection = CurveDirection.Concave;
                            else
                                Arrow.CurveDirection = CurveDirection.Convex;
                        }
                        else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Left)
                        {
                            double rightPlacement = targetTopLeft.X - CUTOUT_MARGIN;
                            Point messageTopLeft = Message.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);

                            Arrow.X1 = rightPlacement - 15;
                            Arrow.Y1 = targetTopLeft.Y + (target.ActualHeight / 2);
                            Arrow.X2 = messageTopLeft.X + Message.ActualWidth + 15;
                            Arrow.Y2 = messageTopLeft.Y + (Message.ActualHeight / 2);

                            if (targetTopLeft.Y + (target.ActualHeight / 2) > Application.Current.MainWindow.ActualHeight / 2)
                                Arrow.CurveDirection = CurveDirection.Concave;
                            else
                                Arrow.CurveDirection = CurveDirection.Convex;
                        }
                        else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Above)
                        {
                            double bottomPlacement = targetTopLeft.Y - CUTOUT_MARGIN;
                            Point messageTopLeft = Message.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);

                            Arrow.X1 = targetTopLeft.X + (target.ActualWidth / 2);
                            Arrow.Y1 = bottomPlacement - 15;
                            Arrow.X2 = messageTopLeft.X + (Message.ActualWidth / 2);
                            Arrow.Y2 = messageTopLeft.Y + Message.ActualHeight + 15;

                            if (targetTopLeft.X + (target.ActualWidth / 2) > Application.Current.MainWindow.ActualWidth / 2)
                                Arrow.CurveDirection = CurveDirection.Concave;
                            else
                                Arrow.CurveDirection = CurveDirection.Convex;
                        }
                        else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Below)
                        {
                            double topPlacement = targetTopLeft.Y + target.ActualHeight + CUTOUT_MARGIN;
                            Point messageTopLeft = Message.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);

                            Arrow.X1 = targetTopLeft.X + (target.ActualWidth / 2);
                            Arrow.Y1 = topPlacement + 15;
                            Arrow.X2 = messageTopLeft.X + (Message.ActualWidth / 2);
                            Arrow.Y2 = messageTopLeft.Y - 15;

                            if (targetTopLeft.X + (target.ActualWidth / 2) > Application.Current.MainWindow.ActualWidth / 2)
                                Arrow.CurveDirection = CurveDirection.Concave;
                            else
                                Arrow.CurveDirection = CurveDirection.Convex;
                        }
                    }
                }
            }
        }

        public FrameworkElement GetElement(DependencyObject startParent, DependencyObject start, List<TypeIndexAssociation> path, int currentIndexInPath)
        {
            if (start == null)
            {
                return null;
            }
            FrameworkElement startAsFrameworkElement = start as FrameworkElement;

            //parent is root
            if (startParent == null)
            {
                if (startAsFrameworkElement.GetType() == path[currentIndexInPath].ElementType)
                {
                    if (currentIndexInPath == (path.Count - 1))
                    {
                        return startAsFrameworkElement;
                    }
                    else
                    {
                        int childrenCount = VisualTreeHelper.GetChildrenCount(start);
                        //for (int i = 0; i < childrenCount; i++)
                        {
                            DependencyObject nextDependencyObject = VisualTreeHelper.GetChild(start, path[currentIndexInPath + 1].Index);
                            if (start is Panel)
                            {
                                nextDependencyObject = ((Panel)start).Children[path[currentIndexInPath + 1].Index];
                            }
                            FrameworkElement fe = GetElement(null, nextDependencyObject, path, currentIndexInPath + 1);
                            if (fe != null)
                            {
                                return fe;
                            }
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {

                //FrameworkElement startParentAsFrameworkElement = startParent as FrameworkElement;
                //int childrenCount = VisualTreeHelper.GetChildrenCount(startParent);
                //for (int i = 0; i < childrenCount; i++)
                //{
                //    if (i == path[currentIndexInPath].Index)
                //    {
                //        FrameworkElement fe = VisualTreeHelper.GetChild(startParent, i) as FrameworkElement;
                //        if (fe.GetType() == path[currentIndexInPath].ElementType && i == path[currentIndexInPath - 1].Index)
                //        {
                //            if (currentIndexInPath == (path.Count - 1))
                //            {
                //                return fe;
                //            }
                //            else
                //            {
                //                int startChildrenCount = VisualTreeHelper.GetChildrenCount(start);
                //                for (int j = 0; j < startChildrenCount; j++)
                //                {
                //                    if (j == path[currentIndexInPath + 1].Index)
                //                    {
                //                        FrameworkElement fe2 = GetElement(start, VisualTreeHelper.GetChild(start, j), path, currentIndexInPath + 1);
                //                        if (fe2 != null)
                //                        {
                //                            return fe2;
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                //return null;
            }
            return null;
        }
    }
}
