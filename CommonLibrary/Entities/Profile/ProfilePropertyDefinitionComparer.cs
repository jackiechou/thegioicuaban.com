using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Profile
{
    public class ProfilePropertyDefinitionComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return ((ProfilePropertyDefinition)x).ViewOrder.CompareTo(((ProfilePropertyDefinition)y).ViewOrder);
        }
    }
}
