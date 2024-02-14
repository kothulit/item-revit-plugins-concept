#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
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
        private string _parameterNameOfRoomNymber = "Плагин_Номер помещения";
        private string _errorParam = "Плагин_Ошибки в отделке";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var startTime = DateTime.Now;

            Document document = commandData.Application.ActiveUIDocument.Document;


            //Список помещений для анализа
            List<Room> rooms = new FilteredElementCollector(document).
                WherePasses(new SelectableInViewFilter(document, document.ActiveView.Id, false)). //Добавляется если нужно применять только для видимых (выделяемых) на виде помещений
                WherePasses(new RoomFilter()).
                ToElements().Cast<Room>().ToList();

            ElementCategoryFilter doorCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            ElementCategoryFilter wallCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            ElementCategoryFilter floorCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            ElementCategoryFilter ceilingCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Ceilings);

            LogicalOrFilter categoryFilter = new LogicalOrFilter(new List<ElementFilter>() { doorCategoryFilter,
                wallCategoryFilter,
                floorCategoryFilter,
                ceilingCategoryFilter });

            List<Element> elments = new FilteredElementCollector(document).
                WhereElementIsNotElementType().
                WherePasses(categoryFilter).
                ToElements().ToList();

            using (Transaction t = new Transaction(document, "Заполнение  параметров отделки  помещениях"))
            {
                t.Start();

                if (rooms.Count() == 0)
                {
                    TaskDialog.Show("Ошибка", "На виде нет помещений");
                    return Result.Failed;
                }
                else
                {
                    //Очистка принадлежности и ошибок
                    foreach (Element element in elments)
                    {
                        element.LookupParameter(_parameterNameOfRoomNymber)?.Set("");
                        element.LookupParameter(_errorParam)?.Set("");
                    }
                    foreach (Element element in rooms)
                    {
                        element.LookupParameter(_errorParam)?.Set("");
                    }
                    bool isOk = true;
                    foreach (Room room in rooms)
                    {
                        Writer writer = new Writer(document, room);
                        writer.SetRoomFinishingParams();
                        if (isOk) isOk = writer.IsOk;
                    }

                    if (!isOk)
                    {
                        TaskDialog.Show("ОШИБКА!", "Плагин закончил работу с ошибками.\r\n" +
                            "Проверьте наличие и значения параметра \"Плагин_Ошибки в отделке\" в экземплярах Помещений, Перекрытий, Потолков, Дверей и Стен \r\n" +
                            "Время работы плагина: " + ((int)(DateTime.Now - startTime).TotalSeconds).ToString() + " секунд");
                    }
                    else
                    {
                        TaskDialog.Show("ВЫПОЛНЕНО!", "Плагин закончил работу без ошибок.\r\n" +
                            "Время работы плагина: " + ((int)(DateTime.Now - startTime).TotalSeconds).ToString() + " секунд");
                    }
                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}