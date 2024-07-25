using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }
            int len_points = points.Count;

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count;)
                {
                    if (points[i].X == points[j].X && points[i].Y == points[j].Y)//x, y
                    {
                        points.Remove(points[j]);
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            len_points = points.Count;
            //   List<Line> extremesegment = new List<Line>();
            // List<Point> outputpoints = new List<Point>(points);
            List<Point> pointsonsegment = new List<Point>();

            for (int p1 = 0; p1 < points.Count; p1++)
            {
                for (int p2 = 0; p2 < points.Count; p2++)
                {
                    // point i isnt equal point j
                    if (points[p1].Equals(points[p2])) continue;
                    int right = 0, left = 0, colinear = 0;


                    for (int point = 0; point < points.Count; point++)
                    {
                        if (point == p1 || point  == p2) continue;
                        Line p1p2line = new Line(points[p1], points[p2]);

                        if (points[point] != points[p1] && points[point] != points[p2])
                        {
                            if (HelperMethods.CheckTurn(p1p2line, points[point]) == Enums.TurnType.Left)
                                left++;
                            else if (HelperMethods.CheckTurn(p1p2line, points[point]) == Enums.TurnType.Right)
                                right++;
                            else colinear++;

                            if (HelperMethods.PointOnSegment(points[point], points[p1], points[p2]))
                            {
                                pointsonsegment.Add(points[point]);
                            }

                        }


                    }
                    if (left == (points.Count - colinear - 2) || (right == points.Count - colinear - 2))
                    {
                        //outputpoints.Add(points[p1]);
                        //outputpoints.Add(points[p2]);
                        ////extremesegment.Add(p1p2line);
                        if (!outPoints.Contains(points[p1]))
                            outPoints.Add(points[p1]);
                        if (!outPoints.Contains(points[p2]))
                            outPoints.Add(points[p2]);

                    }

                    /* left = 0;
                     right = 0;
                     colinear = 0;*/

                }

            }

            for (int i = 0; i < pointsonsegment.Count; i++)
            {
                outPoints.Remove(pointsonsegment[i]);
            }
        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}