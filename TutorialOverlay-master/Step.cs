
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
namespace HelpOverlay
{
    [Serializable]
    public class Step
    {
        public Step() 
        {
            Path = new List<TypeIndexAssociation>();
        }

        public Step(List<TypeIndexAssociation> stepList)
        {
            Message = String.Empty;
            Path = stepList;
        }

        private List<TypeIndexAssociation> _path;
        [XmlIgnore]
        public List<TypeIndexAssociation> Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private List<string> _stringPath;
        public List<string> StringPath
        {
            get 
            {
                if (_stringPath == null)
                    _stringPath = new List<string>();

                foreach(TypeIndexAssociation tia in Path)
                {
                    _stringPath.Add(tia.ToString());
                }

                return _stringPath;
            }
            set { _stringPath = value; }
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Path.Count; i++)
            {
                sb.Append(Path[i].ElementType.ToString());
                if (Path[i].Index != 0)
                {
                    sb.Append(" at index ");
                    sb.Append(Path[i].Index);
                }

                if (i != Path.Count - 1)
                {
                    sb.Append(" -> ");
                }
            }

            return sb.ToString();
        }
    }

    public enum Placement
    {
        Above, Below, Left, Right
    }
}
