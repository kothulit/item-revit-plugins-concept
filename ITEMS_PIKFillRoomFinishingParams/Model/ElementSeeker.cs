using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<ElementId> ElementIdListOfRoomElements
        {
            get
            {
                return _ElementIdListOfRoomElements;
            }
        }
        //Список стен помещения
        private List<Element> _walls = new List<Element>();
        //Список перекрытий помещения
        private List<Element> _floors = new List<Element>();

        //Коллекторы стен и перекрытий в проекте. Используются при определении ближайших к помещению элементов
        //через фильтр по пересечению баундинг боксов. Далее эти элементы анализируются на принадлежность к помещению
        private FilteredElementCollector _wallCoolector;
        private FilteredElementCollector _floorCoolector;

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
                IspectGeometry();
            }

        }

        //Конструктор при инициализации анализирует переданное помещение.
        public ElementSeeker(Document document, Room room, double geometryTolerance = 0.5)
        {
            Document = document;
            AnalyzedRoom = room;
            GeometryTolerance = geometryTolerance;
        }

        /// <summary>
        /// Поиск элементов принадлежащих помещению и обновновление данных о них в свойствах объекта класса
        /// </summary>
        private void IspectGeometry()
        {
            _ElementIdListOfRoomElements = new List<ElementId>();
            _walls = new List<Element>();
            _floors = new List<Element>();

            _wallCoolector = new FilteredElementCollector(Document).
                    WhereElementIsNotElementType().
                    OfCategory(BuiltInCategory.OST_Walls).
                    OfClass(typeof(Wall));

            _floorCoolector = new FilteredElementCollector(Document).
                WhereElementIsNotElementType().
                OfCategory(BuiltInCategory.OST_Floors).
                OfClass(typeof(Floor));
            IntersectedWalls();
            IntersectedFloors(_GeometryTolerance);

            foreach (Element element in _walls)
            {
                _ElementIdListOfRoomElements.Add(element.Id);
            }
            foreach (Element element in _floors)
            {
                _ElementIdListOfRoomElements.Add(element.Id);
            }
        }

        /// <summary>
        /// Поиск элементов стен (из списка BoundarySegment помещения) и запись данных о них в свойства объекта класса
        /// </summary>
        /// <returns></returns>
        private void IntersectedWalls()
        {
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
        }

        /// <summary>
        /// Поиск элементов перекрытий и запись данных о них в свойства объекта класса
        /// </summary>
        /// <param name="offset"></param>
        private void IntersectedFloors(double offset)
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

            //Вывод проверочных данных
            TaskDialog.Show("Intersected walls and floors count: ", _walls.Count().ToString() + "\n" + _floors.Count().ToString());
        }

    }
}
