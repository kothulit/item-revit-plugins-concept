using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;
using System.Reflection;

namespace ITEMS_RevitLvlRoomParamChanger
{
    public class RoomHandler
    {
        /*
         *  Проблемы:
            не задаются свойства связанные с параметрами
            нет универсальности или шаблона для перезаписи значений параметров
         */

        //Список параметров, которые нужно изменить. Пока задается по умолчанию для Брусники
        private List<string> _lvlParameterList = new List<string>() {   "ADSK_Номер квартиры",
                                                                        "ADSK_Номер помещения квартиры",
                                                                        "ADSK_Этаж",
                                                                        "BRU_Номер помещения",
                                                                        "BRU_Номер части помещения",
                                                                        "BRU_Этаж_Число" };
        //Параметр с исходными данными этажа и отметки в формате: "01;-3,900"
        private string _sourceParameter = "Примечание_Этаж";
        private List<Room> _RevitRooms;
        public List<Room> RevitRooms
        {
            get
            {
                return _RevitRooms;
            }
        }
        private Document _Document;
        public Document Document
        {
            get
            {
                return _Document;
            }
            set
            {
                _Document = value;
            }
        }
        public RoomHandler(Document document)
        {
            _Document = document;

            _RevitRooms = new FilteredElementCollector(_Document)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Rooms)
                .OfClass(typeof(SpatialElement))
                .ToElements()
                .Cast<Room>()
                .ToList();
        }

        public void FilterRevitRoomsByParamValue(string parameterName, string parameterValue)
        {
            List<Room> newRevitRooms = new List<Room>();
            if (!string.IsNullOrEmpty(parameterName) && (parameterValue != null))
            {
                foreach (Room room in _RevitRooms)
                {
                    if (GetParameterValue(room, parameterName) == parameterValue)
                    {
                        newRevitRooms.Add(room);
                    }
                }
            }
            _RevitRooms = newRevitRooms;
        }

        public void ChangeRoomLvlParamsValue()
        {
            foreach (Room room in _RevitRooms)
            {
                foreach (string paramName in _lvlParameterList)
                {
                    Parameter param = GetParameter(room, paramName);
                    SetNewParameterValue(room, param);
                }
            }
        }

        public Parameter GetParameter(Element element, string parameterName)
        {
            return element.LookupParameter(parameterName);
        }

        public string GetParameterValue(Element element, string parameterName)
        {
            return element.LookupParameter(parameterName).AsString();
        }

        private void SetNewParameterValue(Room room, Parameter parameter)
        {
            if ((room != null) && (parameter != null))
            {

                string[] notesData = GetParameter(room, "Примечание_Этаж").AsString().Trim().Split(';');
                int newLvl = int.Parse(notesData[0]);
                string newFloorMark = notesData[1].Trim();


                switch (parameter.Definition.Name)
                {
                    case "BRU_Этаж_Число":
                        parameter.Set(newLvl);
                        break;
                    case "ADSK_Номер квартиры":
                        CangeFloorIndex(parameter, newLvl);
                        break;
                    case "ADSK_Номер помещения квартиры":
                        CangeFloorIndex(parameter, newLvl);
                        break;
                    case "BRU_Номер помещения":
                        CangeFloorIndex(parameter, newLvl);
                        break;
                    case "BRU_Номер части помещения":
                        CangeFloorIndex(parameter, newLvl);
                        break;
                    case "ADSK_Этаж":
                        parameter.Set("Этаж " + string.Format("{0:D2}", newLvl) + " (отм. " + notesData[1] + ")");
                        break;
                }
            }
        }

        private void CangeFloorIndex(Parameter parameter, int newLvl)
        {
            string parameterValue = parameter.AsString();
            string newParameterValue = "";
            string[] splitedValue = parameterValue.Trim().Split('.');
            splitedValue[1] = newLvl.ToString();
            for (int i = 0; i < splitedValue.Length; i++)
            {
                newParameterValue += splitedValue[i];
                if (i < splitedValue.Length - 1)
                {
                    newParameterValue += ".";
                }
            }
            parameter.Set(newParameterValue);
        }
    }
}
