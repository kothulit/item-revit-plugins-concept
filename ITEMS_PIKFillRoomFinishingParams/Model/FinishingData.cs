using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITEMS_PIKFillRoomFinishingParams.Model
{
    internal class FinishingData
    {
        public string NameParameter { get; set; }
        public string Name { get; set; }
        public string MarkParameter { get; set; }
        public string Mark { get; set; }
        public string ValueParameter { get; set; }
        public double Value { get; set; }
        public FinishingData(string nameParameter, string markParameter, string valueParameter)
        {
            NameParameter = nameParameter;
            Name = "";
            MarkParameter = markParameter;
            Mark = "";
            ValueParameter = valueParameter;
            Value = 0;
        }
    }
}
