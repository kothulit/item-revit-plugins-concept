using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using ITEMS_SignPlacement.ITEM.Elements;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITEMS_SignPlacement.Model
{
    internal class SheetDataTransfer : Templates.Singleton<SheetDataTransfer>
    {
        // Значения параметров по умолчанию
        private Dictionary<string, string> _parameters = new Dictionary<string, string>()
        {
            { "MainSurname1", "ADSK_Штамп Строка 1 фамилия" },
            { "MainSurname2", "ADSK_Штамп Строка 2 фамилия" },
            { "MainSurname3", "ADSK_Штамп Строка 3 фамилия" },
            { "MainSurname4", "ADSK_Штамп Строка 4 фамилия" },
            { "MainSurname5", "ADSK_Штамп Строка 5 фамилия" },
            { "MainSurname6", "ADSK_Штамп Строка 6 фамилия" },

            { "SideSurname1", "ADSK_Штамп Строка 1 фамилия" },
            { "SideSurname2", "ADSK_Штамп Строка 2 фамилия" },
            { "SideSurname3", "ADSK_Штамп Строка 3 фамилия" },
            { "SideSurname4", "ADSK_Штамп Строка 4 фамилия" },
            { "SideSurname5", "ADSK_Штамп Строка 5 фамилия" },
            { "SideSurname6", "ADSK_Штамп Строка 6 фамилия" },

            { "RevisionSurname1", "ITEM_Изм1_Фамилия" },
            { "RevisionSurname2", "ITEM_Изм2_Фамилия" },
            { "RevisionSurname3", "ITEM_Изм3_Фамилия" },
            { "RevisionSurname4", "ITEM_Изм4_Фамилия" },
        };
        public Dictionary<string, string> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public void ReadDataFromFile(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создает объект с данными из элемента листа
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ItemViewSheet CreateDTO(Element element)
        {
            // Создаем объекты DTO для листов
            ItemViewSheet itemViewSheet = new ItemViewSheet(element.Id.IntegerValue);

            // Записываем параметры jcyj в объект DTO


            // Создаем коллекцию основных надписей находящихся на листе
            foreach (KeyValuePair<string, string> keyValue in _parameters)
            {
                string familyName = element.LookupParameter(keyValue.Value).AsString();
                /itemViewSheet.Parameters.Add(keyValue.Key, familyName);
            }

            TitleBlock itemTitleBlock = new TitleBlock();
            // Передаем Id листа
            itemTitleBlock.Id
            
            throw new NotImplementedException();
        }
    }
}

// Создаем объекты DTO для листов
ItemViewSheet itemViewSheet = new ItemViewSheet(viewSheet.Id.IntegerValue);



// Ищем основные надписи на листе
List<FamilyInstance> titleBlockCollector = new FilteredElementCollector(document).OfClass(typeof(FamilyInstance)).
    OfCategory(BuiltInCategory.OST_TitleBlocks).OwnedByView(viewSheet.Id).ToElements().Cast<FamilyInstance>().ToList();

foreach (FamilyInstance familyInstance in titleBlockCollector)
{
    XYZ xyz = ((Instance)familyInstance).GetTransform().Origin;
}