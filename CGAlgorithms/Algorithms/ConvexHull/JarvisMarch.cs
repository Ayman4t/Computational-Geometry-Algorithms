using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //points length
            int len_points = points.Count;
            //less than three points
            if (len_points <= 3)
            {
                outPoints = new List<Point>(points);
            }
            else
            {
                double y_min = points[0].Y;
                Point smallest_point = new Point(points[0].X, points[0].Y);
                Point p=points[0];
                for (int i=1;i<len_points; i++)
                {
                    if (p.Y < y_min)
                    {
                        y_min = p.Y;
                        smallest_point = p;
                    }
                    p = points[i];
                }
                double x = smallest_point.X;
                double y = smallest_point.Y;
                Point start_point = new Point(x, y);
                int at_end = 0;
                outPoints.Add(smallest_point);
                do
                {
                    at_end = outPoints.Count;
                    smallest_point = outPoints[at_end - 1];
                    Point coliner = null;
                    foreach (Point i in points)
                    {
                        if (smallest_point == i)
                            continue;
                        int left = 0, right = 0;
                        foreach (Point j in points)
                        {
                            if (smallest_point != j && i != j)
                            {
                                Enums.TurnType type = HelperMethods.CheckTurn(new Line(smallest_point, i), j);
                                if (type == Enums.TurnType.Left)
                                {
                                    left++;
                                }
                                else if (type == Enums.TurnType.Right)
                                {
                                    right++;
                                    break;
                                }
                            }
                        }
                        if (left == len_points - 2)
                        {
                            outPoints.Add(i);
                            break;
                        }
                        else if (right == 0)
                        {
                            if (coliner == null)
                            {
                                coliner = i;
                            }
                            else
                            {
                                //
                                double dis1 = CalculateDistance(smallest_point, coliner);
                                double dis2 = CalculateDistance(smallest_point, i);
                                if (dis2 > dis1)
                                {
                                    coliner = i;
                                }
                            }
                        }

                    }
                    if (coliner != null)
                    {
                        outPoints.Add(coliner);
                    }
                    at_end = outPoints.Count;
                    if (outPoints[at_end - 1].Y == start_point.Y && outPoints[at_end - 1].X == start_point.X)
                    {
                        outPoints.Remove(outPoints[at_end - 1]);
                        break;
                    }
                } while (true);
            }
        }
        private double CalculateDistance(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
