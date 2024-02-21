using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace ITEMS_SignPlacement.ITEM.Elements
{
    internal class TitleBlock
    {
        public string FamilySymbolName { get; private set; }
        public string FamilyName { get; private set; }
        public string Style { get; private set; }
        public int HostSimbolId { get; private set; }
        public XYZ Position { get; set; }
        // Значения параметров по умолчанию
        private Dictionary<string, Transform> Lines = new Dictionary<string, string>()
        {
            { "MainSurname1", new Transform },
            { "MainSurname2", "" },
            { "MainSurname3", "" },
            { "MainSurname4", "" },
            { "MainSurname5", "" },
            { "MainSurname6", "" },

            { "MainSurname1", new Transform },
            { "MainSurname2", "" },
            { "MainSurname3", "" },
            { "MainSurname4", "" },
            { "MainSurname5", "" },

            { "RevisionSurname1", "" },
            { "RevisionSurname2", "" },
            { "RevisionSurname3", "" },
            { "RevisionSurname4", "" },
        };

        public TitleBlock()
        {

        }

        

        public Dictionary<string, string>  Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public void AddLine(XYZ offset, string text)
        {

        }

    }
}
