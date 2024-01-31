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
    internal class Sculptor
    {
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

        private static Plane GetPlaneFromPlanarElement(Element element)
        {
            Plane plane = null;
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
