using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using ITEMS_SignPlacement.ITEM.Elements;
using ITEMS_SignPlacement.Model;

namespace ITEMS_SignPlacement
{
    [Transaction(TransactionMode.Manual)]
    internal class Command : IExternalCommand
    {
        DateTime _startTime;
        int _numberOfSheetsPrщcessed = 0;
        SheetDataTransfer _sheetParameters = new SheetDataTransfer();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Записываем время начала работы плагина для вывода отчета
            _startTime = DateTime.Now;

            Document document = commandData.Application.ActiveUIDocument.Document;
            UIDocument uidoc = new UIDocument(document);

            // Список DTO листов для размещения подписей
            List<ItemViewSheet> itemViewSheets = new List<ItemViewSheet>();

            // Получение списка листов для обработки плагином
            try
            {
                List<ViewSheet> sheetList = new List<ViewSheet>();
                // Получаем выбранные в активном документе элементы
                List<ElementId> selectedIds = uidoc.Selection.GetElementIds().ToList();
                if (selectedIds.Count > 0)
                {
                    // Добавляем в список только листы
                    foreach (ElementId id in selectedIds)
                    {
                        Element element = document.GetElement(id);
                        if (element != null)
                        {
                            if (element.Category.Id.IntegerValue == ((int)BuiltInCategory.OST_Sheets))
                            {
                                sheetList.Add((ViewSheet)element);
                            }
                        }
                    }
                }
                else
                {
                    // Получаем все листы из активного документа
                    sheetList = new FilteredElementCollector(document).OfClass(typeof(ViewSheet)).
                        OfCategory(BuiltInCategory.OST_Sheets).ToElements().Cast<ViewSheet>().ToList();
                }

                // Если в проекте нет листов заканчиваем работу плагина
                if (sheetList.Count == 0)
                {
                    TaskDialog.Show("Предупреждение", "В проекте нет листов");
                    return Result.Cancelled;
                }
                else
                {
                    // Записываем сколько листов обрабатывается для вывода отчета
                    _numberOfSheetsPrщcessed = sheetList.Count;
                }

                
                foreach (ViewSheet viewSheet in sheetList)
                {

                }
            }
            catch (NullReferenceException ex)
            {
                TaskDialog.Show("Ошибка", "Ссылка на null при получении коллекции листов");
            }

            // Читаем названия список названий семейств из файла
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); //DEBUG!!! проверить работает ли путь к сборке

            // Раставляем подписи
            using (Transaction t = new Transaction(document, "Расстановка подписей"))
            {
                t.Start();
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}