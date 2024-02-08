using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ITEMS_PIKFillRoomFinishingParams.Model
{
    internal class Writer
    {
        //Статус объекта
        private bool _IsOk = true;
        public bool IsOk { get { return _IsOk; } }

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

        public Writer(ElementSeeker elementSeeker)
        {
            _ElementSeeker = elementSeeker;
            if (_ElementSeeker.Walls.Count > 0)
            {
                if (!CheckWallParametersIsOk(_ElementSeeker.Walls.First()))
                {
                    _IsOk = false;
                    return;
                }
            }
            if (_ElementSeeker.Floors.Count > 0)
            {
                foreach (Element floor in _ElementSeeker.Floors)
                {
                    if (!CheckFloorParametersIsOk(floor))
                    {
                        _IsOk = false;
                        return;
                    }
                }

            }
            if (!CheckRoomParametersIsOk(_ElementSeeker.AnalyzedRoom))
            {
                _IsOk = false;
                return;
            }

            WtiteFinishingData();
        }

        public void SetRoomFinishingParams()
        {
            Element room = _ElementSeeker.AnalyzedRoom;
            ClearRoomParams(room);
            foreach (KeyValuePair<string, FinishingData> keyValuePair in _roomFinishingData)
            {
                if (keyValuePair.Value.Name != "")
                {
                    room.LookupParameter(keyValuePair.Value.NameParameter)?.Set(keyValuePair.Value.Name);
                    room.LookupParameter(keyValuePair.Value.MarkParameter)?.Set(keyValuePair.Value.Mark);
                    room.LookupParameter(keyValuePair.Value.ValueParameter)?.Set(keyValuePair.Value.Value);
                }
            }
        }

        private void WtiteFinishingData()
        {
            WriteFinishingParamsFromWalls();
            WriteFinishingParamsFromFloors();
        }

        private void ClearRoomParams(Element room)
        {
            foreach (KeyValuePair<string, FinishingData> keyValuePair in _roomFinishingData)
            {
                if (keyValuePair.Value.NameParameter != "") room.LookupParameter(keyValuePair.Value.NameParameter)?.Set("");
                if (keyValuePair.Value.MarkParameter != "") room.LookupParameter(keyValuePair.Value.MarkParameter)?.Set("");
                if (keyValuePair.Value.ValueParameter != "") room.LookupParameter(keyValuePair.Value.ValueParameter)?.Set(0);
            }
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
                Element floorType = _ElementSeeker.Document.GetElement(floor.GetTypeId());

                string finishingGroupe = floorType.LookupParameter(_elemParamNameFinishingGroupe)?.AsString();


                //Проверка параметра группы отделки, если не ок то заканчиваем работу
                if (!_roomFinishingData.Keys.Contains(finishingGroupe))
                {
                    ShowParameterValueErrorDialog(floor, _elemParamNameFinishingGroupe, (finishingGroupe != null) ? finishingGroupe : "пустое значение");
                    _IsOk = false;
                    return;
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
        private void ShowParameterErrorDialog(Element element, string parameterName)
        {
            string elementText = "ID элемента: " + element.Id.ToString();
            string parameterText = element.Category.Name.ToString() + ": " + parameterName;
            TaskDialog.Show("ОШИБКА! Отсутствует параметр", parameterText + "\n" + elementText);
        }
        private void ShowParameterValueErrorDialog(Element element, string parameterName, string parameterValue)
        {
            string elementText = "ID элемента: " + element.Id.ToString();
            string parameterText = parameterName + ": " + parameterValue;
            TaskDialog.Show("ОШИБКА! Неправильно заполнен параметр", parameterText + "\n" + elementText);
        }
    }
}
