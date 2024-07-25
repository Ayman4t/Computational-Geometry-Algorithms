using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{

    public class ExtremePoints : Algorithm
    {


        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 3)
            {
                // Convex hull is not possible with less than 3 distinct points
                outPoints.AddRange(points);
                return;
            }

            int i, j, k, l;

            for (i = 0; i < points.Count; i++)
            {
                for (j = i + 1; j < points.Count; j++)
                {
                    for (k = j + 1; k < points.Count; k++)
                    {
                        for (l = 0; l < points.Count; l++)
                        {
                            //find interior point 
                            if (points[l] != points[i] && points[l] != points[j] && points[l] != points[k])
                            {
                                if (HelperMethods.PointInTriangle(points[l], points[i], points[j], points[k]) != Enums.PointInPolygon.Outside)
                                {
                                    //remove point l in a triangle ijk
                                    points.RemoveAt(l);
                                    //updateIndices
                                    if (i > l) { i--; }

                                    if (j > l) { j--; }

                                    if (k > l) { k--; }

                                }
                            }
                        }
                    }
                }
            }
            outPoints = points;

        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}