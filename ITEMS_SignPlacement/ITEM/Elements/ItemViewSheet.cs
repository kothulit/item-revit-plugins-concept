using Autodesk.Revit.DB.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITEMS_SignPlacement.ITEM.Elements
{
    internal class ItemViewSheet
    {
        public int RevitId { get; private set; }
        public Dictionary<string, string> Parameters { get; set; }
        public List<TitleBlock> TitleBlocks { get; set; }
        public ItemViewSheet(int id)
        {
            RevitId = id;
        }
    }
}
