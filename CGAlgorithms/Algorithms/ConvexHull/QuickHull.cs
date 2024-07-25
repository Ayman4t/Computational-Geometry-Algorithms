using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;
            }
            else
            {

                Point minX = points[0];
                Point maxX = points[0];

                foreach (Point point in points)
                {
                    if (point.X > maxX.X)
                    {
                        maxX = point;
                    }
                    if (point.X < minX.X)
                    {
                        minX = point;
                    }
                    
                }

                Line line = new Line(minX, maxX);
                Line lineRev = new Line(maxX, minX);

                List<Point> leftPoints = new List<Point>();
                List<Point> rightPoints = new List<Point>();

                foreach (Point point in points)
                {
                    Enums.TurnType type = HelperMethods.CheckTurn(line, point);
                    if (type == Enums.TurnType.Left)
                    {
                        leftPoints.Add(point);
                    }
                    else if (type == Enums.TurnType.Right)
                    {
                        rightPoints.Add(point);
                    }
                }

                List<Point> extremeLeft = QuickHullRec(leftPoints, line);
                List<Point> extremeRight = QuickHullRec(rightPoints, lineRev);

                extremeLeft.Add(minX);
                extremeLeft.Add(maxX);
                extremeLeft.AddRange(extremeRight);

                foreach (Point point in extremeLeft)
                {
                    if (!outPoints.Contains(point))
                    {
                        outPoints.Add(point);
                    }
                }
            }
        }

        public List<Point> QuickHullRec(List<Point> points, Line line)
        {
            if (points.Count == 0)
            {
                return new List<Point>();
            }

            Point Pmax = GetMaxPoint(points, line);

            Line line1 = new Line(line.Start, Pmax);
            List<Point> leftPoints1 = GetLeftPoints(points, line1);
            List<Point> R1 = QuickHullRec(leftPoints1, line1);

            Line line2 = new Line(Pmax, line.End);
            List<Point> leftPoints2 = GetLeftPoints(points, line2);
            List<Point> R2 = QuickHullRec(leftPoints2, line2);

            R1.Add(Pmax);
            R1.AddRange(R2);

            List<Point> result = new List<Point>();
            foreach (Point point in R1)
            {
                if (!result.Contains(point))
                {
                    result.Add(point);
                }
            }

            return result;
        }

        public Point GetMaxPoint(List<Point> points, Line line)
        {
            Point maxDistancePoint = points[0];
            double maxDistance = -10000000.0;

            foreach (Point point in points)
            {
                double distance = CalculateDistanceToLine(point, line);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistancePoint = point;
                }
            }

            return maxDistancePoint;
        }

        public double CalculateDistanceToLine(Point point, Line line)
        {
            double x1 = line.Start.X;
            double y1 = line.Start.Y;
            double x2 = line.End.X;
            double y2 = line.End.Y;
            double x0 = point.X;
            double y0 = point.Y;

            double xx = x2 - x1;
            double yy = y2 - y1;
            double h1 = Math.Abs(((x2 - x1) * (y1 - y0)) - ((x1 - x0) * (y2 - y1))) / Math.Sqrt((xx * xx) + (yy * yy));

            return h1;
        }

        public List<Point> GetLeftPoints(List<Point> points, Line line)
        {
            List<Point> leftPoints = new List<Point>();

            foreach (Point point in points)
            {
                Enums.TurnType type = HelperMethods.CheckTurn(line, point);
                if (type == Enums.TurnType.Left)
                {
                    leftPoints.Add(point);
                }
            }

            return leftPoints;
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
