using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITEMS_PIKFillRoomFinishingParams.Model
{
    internal class ElementsComparer : IEqualityComparer<Element>
    {
        public ElementsComparer() { }

        public bool Equals(Element x, Element y)
        {
            return x.Id.IntegerValue == y.Id.IntegerValue;
        }

        public int GetHashCode(Element obj)
        {
            return obj.Id.IntegerValue.GetHashCode();
        }
    }
}
