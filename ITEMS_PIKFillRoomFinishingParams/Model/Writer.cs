using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ITEMS_PIKFillRoomFinishingParams.Model
{
    internal class Writer
    {
        //Статус объекта
        private bool _IsOk = true;
        public bool IsOk { get { return _IsOk; } }
        //Название параметра элемента, куда записывает номер помещения. Для анализа работы плагина
        private string _parameterNameOfRoomNymber = "Плагин_Номер помещения";
        private string _errorParam = "Плагин_Ошибки в отделке";

        //Параметры группы эелементов отделки
        private string _elemParamNameFinishingGroupe = "ITEM_Группа отделки";

        //Исходные параметры стен
        private string _wallParamNameFinishingTypeName = "СЭ_Отделка наименование";
        private string _wallParamNameDecorationTypeName = "ITEM_Отделка_Чистовая наименование";
        private string _wallParamNameDecorationTypeMark = "ITEM_Отделка_Чистовая марка";
        private string _wallParamNamePlinthTypeName = "ITEM_Отделка_Плинтус наименование";
        private string _wallParamNamePlinthTypeMark = "ITEM_Отделка_Плинтус марка";

        //Исходные параметры полов
        private string _wallParamNameFloorTypeName = "СЭ_Отделка наименование";
        private string _wallParamNameFloorTypeMark = "ITEM_Отделка_Марка";

        private ElementSeeker _ElementSeeker;
        public ElementSeeker ElementSeeker
        {
            get
            {
                return _ElementSeeker;
            }
        }

        private Dictionary<string, FinishingData> _roomFinishingData = new Dictionary<string, FinishingData>()
        {
            {"ОТД_Стены_Отделка_Железобетон", new FinishingData(        "ОТД_Стены_Отделка_Железобетон",        "",                         "ОТД_Стены_Площ_Железобетон")},
            {"ОТД_Стены_Отделка_Силикатные блоки", new FinishingData(   "ОТД_Стены_Отделка_Силикатные блоки",   "",                         "ОТД_Стены_Площ_Силикатные блоки")},
            {"ОТД_Стены_Отделка_ПГП", new FinishingData(                "ОТД_Стены_Отделка_ПГП",                "",                         "ОТД_Стены_Площ_ПГП")},
            {"ОТД_Стены_Отделка_ГСП", new FinishingData(                "ОТД_Стены_Отделка_ГСП",                "",                         "ОТД_Стены_Площ_ГСП")},
            {"ОТД_Стены_Отделка_Гипсокартон", new FinishingData(        "ОТД_Стены_Отделка_Гипсокартон",        "",                         "ОТД_Стены_Площ_Гипсокартон")},
            {"ОТД_Стены_Отделка_Навесные панели", new FinishingData(    "ОТД_Стены_Отделка_Навесные панели",    "",                         "ОТД_Стены_Площ_Навесные панели")},
            {"ОТД_Стены_Отделка_Кирпич", new FinishingData(             "ОТД_Стены_Отделка_Кирпич",             "",                         "ОТД_Стены_Площ_Кирпич")},
            {"ОТД_Стены_Отделка_Газобетон", new FinishingData(          "ОТД_Стены_Отделка_Газобетон",          "",                         "ОТД_Стены_Площ_Газобетон")},
            {"ОТД_Стены_Отделка_Газобетон_ТИП2", new FinishingData(     "ОТД_Стены_Отделка_Газобетон_ТИП2",     "",                         "ОТД_Стены_Площ_Газобетон_ТИП2")},
            {"ОТД_Стены_Отделка_Минвата", new FinishingData(            "ОТД_Стены_Отделка_Минвата",            "",                         "ОТД_Стены_Площ_Минвата")},
            {"ОТД_Стены_Отделка_Акотэк", new FinishingData(             "ОТД_Стены_Отделка_Акотэк",             "",                         "ОТД_Стены_Площ_Акотэк")},
            {"ОТД_Стены_Отделка_Чистовая", new FinishingData(           "ОТД_Стены_Отделка_Чистовая",           "ОТД_Стены_Марка_Чистовая", "ОТД_Стены_Площ_Чистовая")},
            {"ОТД_Стены_Отделка_Фартук", new FinishingData(             "ОТД_Стены_Отделка_Фартук",             "ОТД_Стены_Марка_Фартук",   "ОТД_Стены_Площ_Фартук")},
            //{"ОТД_Лестницы_Отделка_Низ марша", new FinishingData(       "ОТД_Лестницы_Отделка_Низ марша",       "",                         "ОТД_Лестницы_Площ_Низ марша")},
            //{"ОТД_Лестницы_Отделка_Торец марша", new FinishingData(     "ОТД_Лестницы_Отделка_Торец марша",     "",                         "ОТД_Лестницы_Площ_Торец марша")},
            //{"ОТД_Лестницы_Отделка_Подступенок", new FinishingData(     "ОТД_Лестницы_Отделка_Подступенок",     "",                         "ОТД_Лестницы_Площ_Подступенок")},
            //{"ОТД_Лестницы_Отделка_Проступь", new FinishingData(        "ОТД_Лестницы_Отделка_Проступь",        "",                         "ОТД_Лестницы_Площ_Проступь")},
            //{"ОТД_Лестницы_Отделка_Низ площадки", new FinishingData(    "ОТД_Лестницы_Отделка_Низ площадки",    "",                         "ОТД_Лестницы_Площ_Низ площадки")},
            //{"ОТД_Лестницы_Отделка_Торец площадки", new FinishingData(  "ОТД_Лестницы_Отделка_Торец площадки",  "",                         "ОТД_Лестницы_Площ_Торец площадки")},
            {"ОТД_Потолки_Отделка_ТИП1", new FinishingData(             "ОТД_Потолки_Отделка_ТИП1",             "ОТД_Потолки_Марка_ТИП1",   "ОТД_Потолки_Площ_ТИП1")},
            {"ОТД_Потолки_Отделка_ТИП2", new FinishingData(             "ОТД_Потолки_Отделка_ТИП2",             "ОТД_Потолки_Марка_ТИП2",   "ОТД_Потолки_Площ_ТИП2")},
            {"ОТД_Потолки_Отделка_ТИП3", new FinishingData(             "ОТД_Потолки_Отделка_ТИП3",             "ОТД_Потолки_Марка_ТИП3",   "ОТД_Потолки_Площ_ТИП3")},
            {"ОТД_Потолки_Отделка_Молдинг", new FinishingData(          "ОТД_Потолки_Отделка_Молдинг",          "ОТД_Потолки_Марка_Молдинг","ОТД_Потолки_Длина_Молдинг")},
            {"ОТД_Полы_Отделка_ТИП1", new FinishingData(                "ОТД_Полы_Отделка_ТИП1",                "ОТД_Полы_Марка_ТИП1",      "ОТД_Полы_Площ_ТИП1")},
            {"ОТД_Полы_Отделка_ТИП2", new FinishingData(                "ОТД_Полы_Отделка_ТИП2",                "ОТД_Полы_Марка_ТИП2",      "ОТД_Полы_Площ_ТИП2")},
            {"ОТД_Полы_Отделка_ТИП3", new FinishingData(                "ОТД_Полы_Отделка_ТИП3",                "ОТД_Полы_Марка_ТИП3",      "ОТД_Полы_Площ_ТИП3")},
            {"ОТД_Полы_Отделка_Плинтус", new FinishingData(             "ОТД_Полы_Отделка_Плинтус",             "ОТД_Полы_Марка_Плинтус",   "ОТД_Полы_Длина_Плинтус")},
        };

        public Writer(Document document, Room room, double geometryTolerance = 0.3)
        {
            try
            {
                _ElementSeeker = new ElementSeeker(document, room, geometryTolerance);

                //Очистка записей в элементах отделки и дверях
                foreach (Element element in _ElementSeeker.Walls) ClearFinishingElementParams(element);
                foreach (Element element in _ElementSeeker.Floors) ClearFinishingElementParams(element);
                foreach (var pair in _ElementSeeker.DoorsInWalls)
                {
                    foreach (Element element in pair.Value) ClearFinishingElementParams(element);
                }

                //Очистка парметров помещения
                ClearRoomParams(_ElementSeeker.AnalyzedRoom);

                if (_ElementSeeker == null)
                {
                    _IsOk = false;
                    return;
                }
                if (_ElementSeeker.Walls.Count > 0)
                {
                    if (!CheckWallParametersIsOk(_ElementSeeker.Walls.First()))
                    {
                        return;
                    }
                }
                if (_ElementSeeker.Floors.Count > 0)
                {
                    foreach (Element floor in _ElementSeeker.Floors)
                    {
                        if (!CheckFloorParametersIsOk(floor))
                        {
                            return;
                        }
                    }
                }
                if (!CheckRoomParametersIsOk(_ElementSeeker.AnalyzedRoom))
                {
                    return;
                }

                WtiteFinishingData();
            }
            catch (Exception ex)
            {
                ShowParameterValueErrorDialog(room, ex);
            }
        }

        public void SetRoomFinishingParams()
        {
            try
            {
                Element room = _ElementSeeker.AnalyzedRoom;
                foreach (KeyValuePair<string, FinishingData> keyValuePair in _roomFinishingData)
                {
                    if (keyValuePair.Value.Name != "")
                    {
                        room.LookupParameter(keyValuePair.Value.NameParameter)?.Set(keyValuePair.Value.Name);
                        room.LookupParameter(keyValuePair.Value.MarkParameter)?.Set(keyValuePair.Value.Mark);
                        room.LookupParameter(keyValuePair.Value.ValueParameter)?.Set(keyValuePair.Value.Value);
                    }
                }

                //Назначение принадлежности
                WriteRoomInElements(); //DEBUG!!! для проверки правильности определения элемента в помищении
            }
            catch (Exception ex)
            {
                ShowParameterValueErrorDialog(_ElementSeeker.AnalyzedRoom, ex);
            }
        }

        private void WtiteFinishingData()
        {
            WriteFinishingParamsFromWalls();
            WriteFinishingParamsFromFloors();
        }

        private void ClearFinishingElementParams(Element element)
        {
            element.LookupParameter(_parameterNameOfRoomNymber)?.Set("");
            element.LookupParameter(_errorParam)?.Set("");
        }
        private void ClearRoomParams(Element room)
        {
            foreach (KeyValuePair<string, FinishingData> keyValuePair in _roomFinishingData)
            {
                if (keyValuePair.Value.NameParameter != "") room.LookupParameter(keyValuePair.Value.NameParameter)?.Set("");
                if (keyValuePair.Value.MarkParameter != "") room.LookupParameter(keyValuePair.Value.MarkParameter)?.Set("");
                if (keyValuePair.Value.ValueParameter != "") room.LookupParameter(keyValuePair.Value.ValueParameter)?.Set(0);
            }
            //Удаление старых ошибок
            room.LookupParameter(_errorParam)?.Set("");
        }
        private void WriteFinishingParamsFromWalls()
        {
            List<Element> wallList = _ElementSeeker.Walls;
            foreach (Element wall in wallList)
            {
                Element wallType = _ElementSeeker.Document.GetElement(wall.GetTypeId());
                string finishingGroupe = wallType.LookupParameter(_elemParamNameFinishingGroupe)?.AsString();

                //Проверка параметра группы отделки, если не ок то заканчиваем работу
                if (!_roomFinishingData.Keys.Contains(finishingGroupe))
                {
                    ShowParameterValueErrorDialog(wall, _elemParamNameFinishingGroupe, (finishingGroupe != null) ? finishingGroupe : "пустое значение");
                    _IsOk = false;
                    return;
                }

                if (finishingGroupe != null)
                {
                    string finishingType = wallType.LookupParameter(_wallParamNameFinishingTypeName)?.AsString();
                    double finishingValue = wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();


                    string decorationType = wallType.LookupParameter(_wallParamNameDecorationTypeName).AsString();
                    string decorationMark = wallType.LookupParameter(_wallParamNameDecorationTypeMark)?.AsString();
                    double decorationValue = wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();

                    string plinthType = wallType.LookupParameter(_wallParamNamePlinthTypeName)?.AsString();
                    string plinthMark = wallType.LookupParameter(_wallParamNamePlinthTypeMark)?.AsString();
                    double plinthValue = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble() * 304.8; //Перевод в миллиметры
                    //Вычитатние дверей
                    if (_ElementSeeker.DoorsInWalls.Keys.Contains(wall.Id.IntegerValue))
                    {
                        foreach (Element door in _ElementSeeker.DoorsInWalls[wall.Id.IntegerValue])
                        {
                            double doorWidth = _ElementSeeker.Document.GetElement(door.GetTypeId()).
                                get_Parameter(BuiltInParameter.FURNITURE_WIDTH).AsDouble() * 304.8; //_ElementSeeker.Document.GetElement(door.GetTypeId())
                            plinthValue -= doorWidth;
                        }
                    }

                    if (finishingType != null)
                        if (!_roomFinishingData[finishingGroupe].Name.Contains(finishingType)) _roomFinishingData[finishingGroupe].Name += finishingType;
                    _roomFinishingData[finishingGroupe].Value += finishingValue;

                    if (decorationType != null)
                        if (!_roomFinishingData["ОТД_Стены_Отделка_Чистовая"].Name.Contains(decorationType))
                            _roomFinishingData["ОТД_Стены_Отделка_Чистовая"].Name += " " + decorationType;
                    if (decorationMark != null)
                        if (!_roomFinishingData["ОТД_Стены_Отделка_Чистовая"].Mark.Contains(decorationMark))
                            _roomFinishingData["ОТД_Стены_Отделка_Чистовая"].Mark += " " + decorationMark;

                    _roomFinishingData["ОТД_Стены_Отделка_Чистовая"].Value += decorationValue;

                    if (plinthType != null)
                        if (!_roomFinishingData["ОТД_Полы_Отделка_Плинтус"].Name.Contains(plinthType))
                            _roomFinishingData["ОТД_Полы_Отделка_Плинтус"].Name += " " + plinthType;
                    if (plinthMark != null)
                        if (!_roomFinishingData["ОТД_Полы_Отделка_Плинтус"].Mark.Contains(plinthMark))
                            _roomFinishingData["ОТД_Полы_Отделка_Плинтус"].Mark += " " + plinthMark;

                    _roomFinishingData["ОТД_Полы_Отделка_Плинтус"].Value += plinthValue;
                }
            }
        }

        private void WriteFinishingParamsFromFloors()
        {
            List<Element> floors = _ElementSeeker.Floors;
            foreach (Element floor in floors)
            {
                try
                {
                    Element floorType = _ElementSeeker.Document.GetElement(floor.GetTypeId());

                    string finishingGroupe = floorType.LookupParameter(_elemParamNameFinishingGroupe)?.AsString();


                    //Проверка параметра группы отделки, если не ок то заканчиваем работу
                    if (!_roomFinishingData.Keys.Contains(finishingGroupe))
                    {
                        ShowParameterValueErrorDialog(floor, _elemParamNameFinishingGroupe, (finishingGroupe != null) ? finishingGroupe : "пустое значение");

                    }

                    if (finishingGroupe != null)
                    {
                        string finishingType = floorType.LookupParameter(_wallParamNameFloorTypeName)?.AsString();
                        string finishingMark = floorType.LookupParameter(_wallParamNameFloorTypeMark)?.AsString();
                        double finishingValue = floor.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();

                        if (finishingType != null)
                        {
                            if (!_roomFinishingData[finishingGroupe].Name.Contains(finishingType))
                                _roomFinishingData[finishingGroupe].Name += " " + finishingType;
                        }
                        if (finishingMark != null)
                            if (!_roomFinishingData[finishingGroupe].Mark.Contains(finishingMark))
                            {
                                _roomFinishingData[finishingGroupe].Mark += " " + finishingMark;
                            }
                        _roomFinishingData[finishingGroupe].Value += finishingValue;
                    }
                }
                catch (Exception ex)
                {
                    ShowParameterValueErrorDialog(floor, ex);
                }
            }
        }

        private bool CheckWallParametersIsOk(Element element)
        {
            Element wall = _ElementSeeker.Document.GetElement(element.GetTypeId());

            if (wall.LookupParameter(_elemParamNameFinishingGroupe) == null)
            {
                ShowParameterErrorDialog(wall, _elemParamNameFinishingGroupe);
                return false;
            }
            if (wall.LookupParameter(_wallParamNameFinishingTypeName) == null)
            {
                ShowParameterErrorDialog(wall, _wallParamNameFinishingTypeName);
                return false;
            }
            if (wall.LookupParameter(_wallParamNameDecorationTypeName) == null)
            {
                ShowParameterErrorDialog(wall, _wallParamNameDecorationTypeName);
                return false;
            }
            if (wall.LookupParameter(_wallParamNameDecorationTypeMark) == null)
            {
                ShowParameterErrorDialog(wall, _wallParamNameDecorationTypeMark);
                return false;
            }
            if (wall.LookupParameter(_wallParamNamePlinthTypeName) == null)
            {
                ShowParameterErrorDialog(wall, _wallParamNamePlinthTypeName);
                return false;
            }
            if (wall.LookupParameter(_wallParamNamePlinthTypeMark) == null)
            {
                ShowParameterErrorDialog(wall, _wallParamNamePlinthTypeMark);
                return false;
            }
            return true;
        }

        private bool CheckFloorParametersIsOk(Element element)
        {
            Element floor = _ElementSeeker.Document.GetElement(element.GetTypeId());

            if (floor.LookupParameter(_elemParamNameFinishingGroupe) == null)
            {
                ShowParameterErrorDialog(floor, _elemParamNameFinishingGroupe);
                return false;
            }
            if (floor.LookupParameter(_wallParamNameFloorTypeName) == null)
            {
                ShowParameterErrorDialog(floor, _wallParamNameFloorTypeName);
                return false;
            }
            if (floor.LookupParameter(_wallParamNameFloorTypeMark) == null)
            {
                ShowParameterErrorDialog(floor, _wallParamNameFloorTypeMark);
                return false;
            }
            return true;
        }

        private bool CheckRoomParametersIsOk(Element room)
        {
            foreach (KeyValuePair<string, FinishingData> keyValuePair in _roomFinishingData)
            {
                if (room.LookupParameter(keyValuePair.Key) == null)
                {
                    ShowParameterErrorDialog(room, keyValuePair.Key);
                    return false;
                }
            }
            return true;
        }
        private void ShowParameterErrorDialog(Element element, string parameterName) //Привести к одному методу
        {
            string elementText = "ID элемента: " + element.Id.ToString();
            string parameterText = element.Category.Name.ToString() + ": " + parameterName;
            //TaskDialog.Show("ОШИБКА! Отсутствует параметр", parameterText + "\n" + elementText); //!!! Пока  не вызывать окно ошибки
            //Clipboard.SetText(element.Id.ToString()); //!!! Пока  не вызывать окно ошибки

            string value = "";
            if (element.LookupParameter(_errorParam) != null) value = element.LookupParameter(_errorParam).AsString();
            element.LookupParameter(_errorParam)?.Set(value + elementText + parameterText);
            _IsOk = false;
        }
        private void ShowParameterValueErrorDialog(Element element, string parameterName, string parameterValue) //!!! Привести к одному методу
        {
            string elementText = "ID элемента: " + element.Id.ToString();
            string parameterText = parameterName + ": " + parameterValue;
            //TaskDialog.Show("ОШИБКА! Неправильно заполнен параметр", parameterText + "\n" + elementText); //!!! Пока  не вызывать окно ошибки
            //Clipboard.SetText(element.Id.ToString()); //!!! Пока  не вызывать окно ошибки
            string value = "";
            if (element.LookupParameter(_errorParam) != null) value = element.LookupParameter(_errorParam).AsString();
            element.LookupParameter(_errorParam)?.Set(value + elementText + parameterText);
            _IsOk = false;
        }

        private void ShowParameterValueErrorDialog(Element element, Exception exception) //!!! Привести к одному методу
        {
            string elementText = "ID элемента: " + element.Id.ToString();
            string exceptionText = " Источник: " + exception.StackTrace + " Ошибка: " + exception.Message;
            //TaskDialog.Show("ОШИБКА! Исключение", exceptionText + "\n" + elementText); //!!! Пока  не вызывать окно ошибки
            //Clipboard.SetText(element.Id.ToString()); //!!! Пока  не вызывать окно ошибки
            string value = "";
            if (element.LookupParameter(_errorParam) != null) value = element.LookupParameter(_errorParam).AsString();
            element.LookupParameter(_errorParam)?.Set(value + elementText + exceptionText);
            _IsOk = false;
        }

        private void WriteRoomInElements() ///DEBUG!!! Переделать в номальный метод, без повторений
        {
            string roomData = String.Format("Помещение: Имя {0} Номер {1} ID {2}",
                _ElementSeeker.AnalyzedRoom.Name.ToString(),
                _ElementSeeker.AnalyzedRoom.Number.ToString(),
                _ElementSeeker.AnalyzedRoom.Id.ToString());
            foreach (Element element in _ElementSeeker.Walls)
            {
                string value = "";
                if (element.LookupParameter(_parameterNameOfRoomNymber) != null)
                {
                    value = element.LookupParameter(_parameterNameOfRoomNymber).AsString();
                }
                element.LookupParameter(_parameterNameOfRoomNymber)?.Set(value + roomData);
            }
            foreach (Element element in _ElementSeeker.Floors)
            {
                string value = "";
                if (element.LookupParameter(_parameterNameOfRoomNymber) != null)
                {
                    value = element.LookupParameter(_parameterNameOfRoomNymber).AsString();
                }
                element.LookupParameter(_parameterNameOfRoomNymber)?.Set(value + roomData);
            }
            foreach (var pair in _ElementSeeker.DoorsInWalls)
            {
                foreach (Element element in pair.Value)
                {
                    string value = "";
                    if (element.LookupParameter(_parameterNameOfRoomNymber) != null)
                    {
                        value = element.LookupParameter(_parameterNameOfRoomNymber).AsString();
                    }
                    element.LookupParameter(_parameterNameOfRoomNymber)?.Set(value + roomData);
                }
            }

        }
    }
}
