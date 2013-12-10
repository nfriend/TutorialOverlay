using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpOverlay
{
    [Serializable]
    public class TypeIndexAssociation
    {
        public Type ElementType { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return ElementType.AssemblyQualifiedName + "|" + Index;
        }
    }
}
