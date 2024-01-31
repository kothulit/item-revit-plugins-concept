#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using ITEMS_PIKFillRoomFinishingParams.Model;
#endregion

namespace ITEMS_PIKFillRoomFinishingParams
{
    [Transaction(TransactionMode.Manual)]
    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            List<Room> rooms = new FilteredElementCollector(document).
                //WherePasses(new SelectableInViewFilter(document, document.ActiveView.Id, false)).
                WherePasses(new RoomFilter()).
                ToElements().Cast<Room>().ToList();

            if (rooms.Count() == 0)
            {
                TaskDialog.Show("Ошибка", "В проекте нет помещений");
                return Result.Failed;
            }
            else
            {
                Room room = rooms.First() as Room;
                ElementSeeker seeker = new ElementSeeker(document, room);
            }
            //using (Transaction t = new Transaction(document, "Замена значений этажа"))
            //{
            //    t.Start();
                
            //    t.Commit();
            //}

            return Result.Succeeded;
        }
    }
}