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
#endregion

namespace ITEMS_RevitLvlRoomParamChanger
{
    [Transaction(TransactionMode.Manual)]
    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            RoomHandler roomHandler = new RoomHandler(document);
            roomHandler.FilterRevitRoomsByParamValue("ADSK_Номер секции", "Секция 5");
            using (Transaction t = new Transaction(document, "Замена значений этажа"))
            {
                t.Start();
                roomHandler.ChangeRoomLvlParamsValue();
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}