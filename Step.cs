using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
