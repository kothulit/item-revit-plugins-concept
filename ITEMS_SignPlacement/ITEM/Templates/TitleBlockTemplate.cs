using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ITEMS_SignPlacement.ITEM.Templates
{
    internal static class TitleBlockTemplate
    {
        private Dictionary<string, Transform> Form3 = new Dictionary<string, string>()
        {
            { "MainSurname1", new Transform() },    // -150, +30
            { "MainSurname2", "" },                 // -150, +25
            { "MainSurname3", "" },                 // -150, +20
            { "MainSurname4", "" },                 // -150, +15
            { "MainSurname5", "" },                 // -150, +10
            { "MainSurname6", "" },                 // -150, +5

            { "SideSurname1", new Transform },      // -405, +130, -90' ???
            { "SideSurname2", "" },                 // -400, +130, -90' ???


            { "RevisionSurname1", "" },                 // -150, +40
            { "RevisionSurname2", "" },                 // -150, +45
            { "RevisionSurname3", "" },                 // -150, +50
            { "RevisionSurname4", "" },                 // -150, +55
        };

        private Dictionary<string, Transform> Form3 = new Dictionary<string, string>()
        {
            { "MainSurname1", new Transform() },    // -150, +30
            { "MainSurname2", "" },                 // -150, +25
            { "MainSurname3", "" },                 // -150, +20
            { "MainSurname4", "" },                 // -150, +15
            { "MainSurname5", "" },                 // -150, +10
            { "MainSurname6", "" },                 // -150, +5

            { "SideSurname1", new Transform },      // -405, +130, -90' ???
            { "SideSurname2", "" },                 // -400, +130, -90' ???


            { "RevisionSurname1", "" },                 // -150, +40
            { "RevisionSurname2", "" },                 // -150, +45
            { "RevisionSurname3", "" },                 // -150, +50
            { "RevisionSurname4", "" },                 // -150, +55
        };

        private Dictionary<string, Transform> Form5 = new Dictionary<string, string>()
        {
            { "MainSurname1", new Transform() },    // -150, +25
            { "MainSurname2", "" },                 // -150, +20
            { "MainSurname3", "" },                 // -150, +15
            { "MainSurname4", "" },                 // -150, +10
            { "MainSurname5", "" },                 // -150, +5

            { "SideSurname1", new Transform },      // -405, +130, ???
            { "SideSurname2", "" },                 // -400, +130, ???


            { "RevisionSurname1", "" },                 // -150, +40
            { "RevisionSurname2", "" },                 // -150, +45
        };

        private Dictionary<string, Transform> Form6 = new Dictionary<string, string>()
        {
            { "SideSurname1", new Transform },      // -405, +130, ???
            { "SideSurname2", "" },                 // -400, +130, ???


            { "RevisionSurname1", "" },                 // -150, +40
            { "RevisionSurname2", "" },                 // -150, +45
        };

        private Dictionary<string, Transform> Form9 = new Dictionary<string, string>()
        {
            { "MainSurname1", new Transform() },    // -150, +20
            { "MainSurname2", "" },                 // -150, +15
            { "MainSurname3", "" },                 // -150, +10
            { "MainSurname4", "" },                 // -150, +5

        };


    }
}
