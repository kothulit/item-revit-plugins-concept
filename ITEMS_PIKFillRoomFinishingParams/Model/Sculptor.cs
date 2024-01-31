using System;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;
using Microsoft.SqlServer.Server;
using System.Windows.Media;

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
            Plane floorPlane = GetPlaneFromPlanarElement(floor);
            List<Line> roomLines = VerticalEdgesFromRoom(room);
            foreach (Line line in roomLines)
            {
                if (IsLineAndPlaneIntersect(line, floorPlane, offset))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Получение верхней горизонтальной плоскости из элемента.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Plane GetPlaneFromPlanarElement(Element element)
        {
            Plane plane;
            GeometryElement geometry = element.get_Geometry(new Options());
            GeometryObject geometryObject = geometry.First();
            Solid solid = geometryObject as Solid;
            foreach (PlanarFace planarFace in solid.Faces)
            {
                plane = (Plane)planarFace.GetSurface();
                if (plane.Normal.IsAlmostEqualTo(new XYZ(0, 0, 1))) return plane;
            }
            return null;
        }

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
                if (line.Direction.IsAlmostEqualTo(new XYZ(0, 0, 1)) || line.Direction.IsAlmostEqualTo(new XYZ(0, 0, -1))) lineList.Add(line); 
            }
            return lineList;
        }

        /// <summary>
        /// Определение пересекает ли горизонтальную плоскость данная вертикальная линия,
        /// удленненая с обоих концов на zAcceptability футов,
        /// </summary>
        /// <param name="line"></param>
        /// <param name="plane"></param>
        /// <param name="zAcceptability"></param>
        /// <returns></returns>
        private static bool IsLineAndPlaneIntersect(Line line, Plane plane, double zAcceptability)
        {
            if ((Math.Abs(plane.Origin.Z - line.Tessellate()[0].Z) < zAcceptability) ||
                (Math.Abs(plane.Origin.Z - line.Tessellate()[1].Z) < zAcceptability))
            {
                return true;
            }
            return false;
        } 
    }
}
