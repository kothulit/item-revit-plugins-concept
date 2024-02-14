using System;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;
using Microsoft.SqlServer.Server;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Threading;

namespace ITEMS_PIKFillRoomFinishingParams.Model
{
    /// <summary>
    /// Класс статических методов для анализа геометрии и дополнительных построений
    /// </summary>
    internal class Sculptor
    {

        /// <summary>
        /// Определение принадлежности перекрытия к помещению с допуском. Допуском является расстояние
        /// от помещения до верхней грани перекрытия
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="room"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static bool IsFloorRelateToRoom(Element floor, Element room, double offset)
        {
            List<Line> roomLines = VerticalEdgesFromRoom(room);
            foreach (Line line in roomLines)
            {
                if (IsLineAndElementIntersect(line, floor, offset)) return true;
            }

            return false;
        }

        /// <summary>
        /// Получение верхней горизонтальной плоскости из элемента.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        //private static Plane GetPlaneFromPlanarElement(Element element)
        //{
        //    Plane plane;
        //    GeometryElement geometry = element.get_Geometry(new Options());
        //    GeometryObject geometryObject = geometry.First();
        //    Solid solid = geometryObject as Solid;
        //    foreach (PlanarFace planarFace in solid.Faces)
        //    {
        //        plane = (Plane)planarFace.GetSurface();
        //        if (plane.Normal.IsAlmostEqualTo(new XYZ(0, 0, 1))) return plane;
        //    }
        //    return null;
        //}

        /// <summary>
        /// Получение вертикальных отрезков на каждом углу помещения.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private static List<Line> VerticalEdgesFromRoom(Element room)
        {
            List<Line> lineList = new List<Line>();
            Solid solid = (Solid)room.get_Geometry(new Options()).First();
            foreach (Edge edge in solid.Edges)
            {
                Line line = (Line)edge.AsCurve();
                if (line.Direction.IsAlmostEqualTo(new XYZ(0, 0, 1)) || line.Direction.IsAlmostEqualTo(new XYZ(0, 0, -1)))
                {
                    lineList.Add(line);
                }
            }
            return lineList;
        }

        /// <summary>
        /// Определение пересекает ли элемент удлененная линия 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="plane"></param>
        /// <param name="zAcceptability"></param>
        /// <returns></returns>
        private static bool IsLineAndElementIntersect(Line line, Element element, double zAcceptability = 0) //DEBUG!!! смещение по z по не учитывается
        {
            XYZ topPoint;
            XYZ bottomPoint;
            if (line.Direction.IsAlmostEqualTo(new XYZ(0, 0, 1)))
            {
                bottomPoint = line.GetEndPoint(0);
                topPoint = line.GetEndPoint(1);
            }
            else
            {
                bottomPoint = line.GetEndPoint(1);
                topPoint = line.GetEndPoint(0);
            }
            bottomPoint -= new XYZ(0, 0, zAcceptability);
            topPoint += new XYZ(0, 0, zAcceptability);

            Line newLine = Line.CreateBound(bottomPoint, topPoint);

            GeometryElement solidList = element.get_Geometry(new Options());
            if (solidList != null)
            {
                foreach (GeometryObject geometry in solidList)
                {
                    if (geometry.GetType() == typeof(Solid))
                    {
                        try
                        {
                            SolidCurveIntersection result = ((Solid)geometry).IntersectWithCurve(newLine, new SolidCurveIntersectionOptions());
                            int segments = result.SegmentCount;
                            if (segments > 0) return true;
                        }
                        catch { } //DEBUG!!! Плохо...
                    }
                }
            }
            return false;
        }
    }
}
