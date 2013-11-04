
namespace HelpOverlay
{
    public class Step
    {
        private string _targetElementName;
        public string TargetElementName
        {
            get { return _targetElementName; }
            set { _targetElementName = value; }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private Placement _messagePlacement = Placement.Right;
        public Placement MessagePlacement
        {
            get { return _messagePlacement; }
            set { _messagePlacement = value; }
        }

    }

    public enum Placement
    {
        Above, Below, Left, Right
    }
}
