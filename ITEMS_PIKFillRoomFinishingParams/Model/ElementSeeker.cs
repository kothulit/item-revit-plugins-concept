using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace ITEMS_PIKFillRoomFinishingParams.Model
{
    /// <summary>
    /// Класс для анализа объекта помещения, хранения полученных данных и актуализации параметров помещения
    /// </summary>
    class ElementSeeker
    {
        //Документ, из которого взято помещение
        public Document Document { get; }
        //Объект исследуемого помещения
        public Room AnalyzedRoom { get; }
        //Список Id элементов, которые относятся к анализируемому помещению
        private List<ElementId> _ElementIdListOfRoomElements;
        //Список стен помещения
        private List<Element> _walls = new List<Element>();
        public List<Element> Walls
        {
            get
            {
                return _walls;
            }
        }
        //Список перекрытий помещения
        private List<Element> _floors = new List<Element>();
        public List<Element> Floors
        {
            get
            {
                return _floors;
            }
        }
        //Словарь Id стены помещения - дверь
        private Dictionary<int, List<Element>> _DoorsInWalls = new Dictionary<int, List<Element>>();
        public Dictionary<int, List<Element>> DoorsInWalls
        {
            get
            {
                return _DoorsInWalls;
            }
        }
        //Коллекторы стен и перекрытий в проекте. Используются при определении ближайших к помещению элементов
        //через фильтр по пересечению баундинг боксов. Далее эти элементы анализируются на принадлежность к помещению
        private FilteredElementCollector _wallCoolector;
        private FilteredElementCollector _doorCoolector; //!!!Переделать на поиск дверей через свойства FromRoom и ToRoom
        private FilteredElementCollector _floorCoolector;

        //Параметр группы модели для определения относится елемент к отделке или нет
        private string _isFinishingParamName = "Группа модели";
        private List<string> _isFinishingParamWallValue = new List<string>() { "Отделка" };
        private List<string> _isFinishingParamFloorValue = new List<string>() { "перекрытие архитектурное", "Перекрытие архитектурное", "Потолок", "Пол" };


        //Допуск при определении пересечения геометрии, для поиска немного отстоящих от помещения элементов.
        public double _GeometryTolerance;
        public double GeometryTolerance
        {
            get
            {
                return _GeometryTolerance;
            }
            set
            {
                _GeometryTolerance = value; 
            }

        }

        //Конструктор при инициализации анализирует переданное помещение.
        public ElementSeeker(Document document, Room room, double geometryTolerance = 0.3)
        {
            Document = document;
            AnalyzedRoom = room;
            int roomId = room.Id.IntegerValue; //DEBUG!!!
            GeometryTolerance = geometryTolerance;
            InspectGeometry();
        }

        /// <summary>
        /// Поиск элементов принадлежащих помещению и обновновление данных о них в свойствах объекта класса
        /// </summary>
        private void InspectGeometry()
        {
            _walls = new List<Element>();
            _floors = new List<Element>();
            _DoorsInWalls = new Dictionary<int, List<Element>>();

            SetWallCollector();
            SetFloorCollector();
            SetDoorCollector();

            SetIntersectedWalls();
            SetIntersectedFloors(_GeometryTolerance);

            List<Element> wallsResualt = new List<Element>();
            List<Element> floorsResualt = new List<Element>();

            foreach (Element element in _walls)
            {
                bool isFinishing = _isFinishingParamWallValue.Contains(Document.GetElement(element.GetTypeId()).LookupParameter(_isFinishingParamName).AsString());
                if (isFinishing)
                {
                    wallsResualt.Add(element);
                }
            }
            foreach (Element element in _floors)
            {
                bool isFinishing = _isFinishingParamFloorValue.Contains(Document.GetElement(element.GetTypeId()).LookupParameter(_isFinishingParamName).AsString());
                if (isFinishing)
                {
                    floorsResualt.Add(element);
                }
            }

            _walls = wallsResualt;
            _floors = floorsResualt;
        }

        /// <summary>
        /// Поиск элементов стен (из списка BoundarySegment помещения) и запись данных о них в свойства объекта класса
        /// </summary>
        /// <returns></returns>
        private void SetIntersectedWalls()
        {
            //Добавление в список стен ограничивающиех помещение
            List<int> intIds = new List<int>();
            foreach (List<BoundarySegment> boundarySegmentList in AnalyzedRoom.GetBoundarySegments(new SpatialElementBoundaryOptions()))
            {
                foreach (BoundarySegment boundarySegment in boundarySegmentList)
                {
                    Element boundaryElement = Document.GetElement(boundarySegment.ElementId);
                    if (boundaryElement != null)
                    {
                        if (boundaryElement.Category.Id.IntegerValue == ((int)BuiltInCategory.OST_Walls))
                        {

                            if (!intIds.Contains(boundaryElement.Id.IntegerValue))
                            {
                                intIds.Add(boundaryElement.Id.IntegerValue);
                                _walls.Add(boundaryElement);
                            }
                        }
                    }
                }
            }

            ElementsComparer wallsComparer = new ElementsComparer();
            //Проверка на геометрическое пересечение и добавление в список таких стен
            foreach (Element wall in _wallCoolector.ToElements())
            {
                if(!_walls.Contains(wall, wallsComparer))
                    _walls.Add(wall);
            }



            //Поиск двере в стенах
            List<Element> doorInRoom = _doorCoolector.ToElements().ToList();
            foreach (Element wall in _walls)
            {
                if (doorInRoom.Count() > 0)
                {
                    //Фильтруем коллектор через пересецение BBox
                    Outline outline = new Outline(wall.get_BoundingBox(null).Min, wall.get_BoundingBox(null).Max);
                    BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 0.15);

                    List<Element> doors = new List<Element>(doorInRoom);
                    foreach (Element door in doors)
                    {
                        if (IsIntersectedWithTolerance(door.get_BoundingBox(null), wall.get_BoundingBox(null), 0.1))
                        {
                            if (!_DoorsInWalls.Keys.Contains(wall.Id.IntegerValue))
                            {
                                _DoorsInWalls.Add(wall.Id.IntegerValue, new List<Element>());
                            }
                            _DoorsInWalls[wall.Id.IntegerValue].Add(door); //DEBUG!!!
                            doorInRoom.Remove(door);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Поиск элементов перекрытий и запись данных о них в свойства объекта класса
        /// </summary>
        /// <param name="offset"></param>
        private void SetIntersectedFloors(double offset)
        {
            BoundingBoxXYZ roomBBox = AnalyzedRoom.get_BoundingBox(null);
            roomBBox.Max += new XYZ(0, 0, offset);
            roomBBox.Min -= new XYZ(0, 0, offset);

            Outline outline = new Outline(roomBBox.Min, roomBBox.Max);
            BoundingBoxIntersectsFilter bbfilter = new BoundingBoxIntersectsFilter(outline);
            List<Element> floorsToCheck = _floorCoolector.WherePasses(bbfilter).ToElements().ToList();

            foreach (Element floor in floorsToCheck)
            {
                if (Sculptor.IsFloorRelateToRoom(floor, AnalyzedRoom, offset))
                {
                    _floors.Add(floor);
                }
            }

            //DEBUG!!! Вывод проверочных данных
            //TaskDialog.Show("Intersected walls and floors count: ", _walls.Count().ToString() + "\n" + _floors.Count().ToString());
        }

        /// <summary>
        /// Метод для записи в поле класса коллекции стен
        /// </summary>
        private void SetWallCollector()
        {
            Outline outline = new Outline(AnalyzedRoom.get_BoundingBox(null).Min, AnalyzedRoom.get_BoundingBox(null).Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 0.35);
            Solid solid = AnalyzedRoom.get_Geometry(new Options()).First() as Solid;
            ElementIntersectsSolidFilter solidFilter = new ElementIntersectsSolidFilter(solid);

            _wallCoolector = new FilteredElementCollector(Document).
                WhereElementIsNotElementType().
                OfCategory(BuiltInCategory.OST_Walls).
                OfClass(typeof(Wall)).
                WherePasses(boundingBoxIntersectsFilter).
                WherePasses(solidFilter);
        }

        /// <summary>
        /// Метод для записи в поле класса коллекции потолков и перекрытий
        /// </summary>
        private void SetFloorCollector()
        {
            LogicalOrFilter classFilter = new LogicalOrFilter(new ElementClassFilter(typeof(Floor)), new ElementClassFilter(typeof(Ceiling)));
            LogicalOrFilter categoryFilter = new LogicalOrFilter(new ElementCategoryFilter(BuiltInCategory.OST_Floors), new ElementCategoryFilter(BuiltInCategory.OST_Ceilings));
            _floorCoolector = new FilteredElementCollector(Document).
                WhereElementIsNotElementType().
                WherePasses(categoryFilter).
                WherePasses(classFilter);
        }

        private void SetDoorCollector()
        {
            Outline outline = new Outline(AnalyzedRoom.get_BoundingBox(null).Min, AnalyzedRoom.get_BoundingBox(null).Max);
            BoundingBoxIsInsideFilter boundingBoxIsInsideFilter = new BoundingBoxIsInsideFilter(outline, 0.1); //DEBUG!!! отступ вручную задается, не хорошо

            _doorCoolector = new FilteredElementCollector(Document).
                WhereElementIsNotElementType().
                OfCategory(BuiltInCategory.OST_Doors).
                OfClass(typeof(FamilyInstance)).
                WherePasses(boundingBoxIsInsideFilter);
        }

        //Craete method to check are two bbox intersected with tolerance
        public static bool IsIntersectedWithTolerance(BoundingBoxXYZ bbox1, BoundingBoxXYZ bbox2, double tolerance)
        {
            if (bbox1.Max.X + tolerance < bbox2.Min.X || bbox1.Min.X - tolerance > bbox2.Max.X) return false;
            if (bbox1.Max.Y + tolerance < bbox2.Min.Y || bbox1.Min.Y - tolerance > bbox2.Max.Y) return false;
            if (bbox1.Max.Z + tolerance < bbox2.Min.Z || bbox1.Min.Z - tolerance > bbox2.Max.Z) return false;
            return true;
        }

    }
}
