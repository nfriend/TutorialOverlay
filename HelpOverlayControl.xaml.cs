using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public void UpdateOverlay()
        {
            if (TutorialManager.CurrentTutorial != null && TutorialManager.CurrentTutorial.CurrentStep != null)
            {
                RectangleGeometry hole = this.Resources["Hole"] as RectangleGeometry;
                FrameworkElement target = HelpOverlyHelper.FindChild(Application.Current.MainWindow, TutorialManager.CurrentTutorial.CurrentStep.TargetElementName);

                if (hole != null && target != null)
                {
                    Point targetTopLeft = target.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);
                    hole.Rect = new Rect(targetTopLeft.X - CUTOUT_MARGIN, targetTopLeft.Y - CUTOUT_MARGIN, target.ActualWidth + CUTOUT_MARGIN * 2, target.ActualHeight + CUTOUT_MARGIN * 2);

                    MessageTextBox.Text = TutorialManager.CurrentTutorial.CurrentStep.Message;

                    if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Right)
                    {
                        double leftPlacement = targetTopLeft.X + target.ActualWidth + CUTOUT_MARGIN;
                        Canvas.SetLeft(MessageContainer, leftPlacement);
                        Canvas.SetRight(MessageContainer, 0);
                        Canvas.SetTop(MessageContainer, 0);
                        Canvas.SetBottom(MessageContainer, 0);
                        MessageContainer.Width = Application.Current.MainWindow.ActualWidth - leftPlacement;
                        MessageContainer.Height = Application.Current.MainWindow.ActualHeight;
                    }
                    else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Left)
                    {
                        double rightPlacement = targetTopLeft.X - CUTOUT_MARGIN;
                        Canvas.SetRight(MessageContainer, rightPlacement);
                        Canvas.SetLeft(MessageContainer, 0);
                        Canvas.SetTop(MessageContainer, 0);
                        Canvas.SetBottom(MessageContainer, 0);
                        MessageContainer.Width = rightPlacement;
                        MessageContainer.Height = Application.Current.MainWindow.ActualHeight;
                    }
                    else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Above)
                    {
                        double bottomPlacement = targetTopLeft.Y - CUTOUT_MARGIN;
                        Canvas.SetBottom(MessageContainer, bottomPlacement);
                        Canvas.SetLeft(MessageContainer, 0);
                        Canvas.SetRight(MessageContainer, 0);
                        Canvas.SetTop(MessageContainer, 0);
                        MessageContainer.Width = Application.Current.MainWindow.ActualWidth;
                        MessageContainer.Height = bottomPlacement;
                    }
                    else if (TutorialManager.CurrentTutorial.CurrentStep.MessagePlacement == Placement.Below)
                    {
                        double topPlacement = targetTopLeft.Y + target.ActualHeight + CUTOUT_MARGIN;
                        Canvas.SetTop(MessageContainer, topPlacement);
                        Canvas.SetLeft(MessageContainer, 0);
                        Canvas.SetRight(MessageContainer, 0);
                        Canvas.SetBottom(MessageContainer, 0);
                        MessageContainer.Width = Application.Current.MainWindow.ActualWidth;
                        MessageContainer.Height = Application.Current.MainWindow.ActualHeight - topPlacement;
                    }
                }
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
                FrameworkElement target = HelpOverlyHelper.FindChild(Application.Current.MainWindow, TutorialManager.CurrentTutorial.CurrentStep.TargetElementName);

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
    }
}
