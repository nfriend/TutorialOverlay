using System.Windows;

namespace HelpOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string lorem = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            string doubleLorem = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. \n\n Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            Tutorial t = new Tutorial();
            t.Steps.Add(new Step() { Message = lorem, TargetElementName = "myOtherButton", MessagePlacement = Placement.Left });
            t.Steps.Add(new Step() { Message = doubleLorem, TargetElementName = "button3", MessagePlacement = Placement.Right });
            t.Steps.Add(new Step() { Message = "The final step.", TargetElementName = "button4", MessagePlacement = Placement.Above });
            t.Steps.Add(new Step() { Message = "The final final step.", TargetElementName = "myOtherButton", MessagePlacement = Placement.Below });
            TutorialManager.Tutorials.Add("First tutorial", t);
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            TutorialManager.Tutorials["First tutorial"].Begin();
        }

        private void myOtherButton_Click(object sender, RoutedEventArgs e)
        {
            if (TutorialManager.CurrentTutorial != null)
                TutorialManager.CurrentTutorial.GoToNextStep.Execute(null);
        }
    }
}
