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

        //Коллекторы стен и перекрытий в проекте
        private FilteredElementCollector _wallCoolector;
        private FilteredElementCollector _floorCoolector;

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

        public ElementSeeker(Document document, Room room, double geometryTolerance = 0.5)
        {
            Document = document;
            AnalyzedRoom = room;
            GeometryTolerance = geometryTolerance;
        }

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
        /// Returns all walls from boundaryes of the room
        /// </summary>
        /// <returns></returns>

        //Create method to check is two bounding boxes intersected
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
