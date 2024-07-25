using System;
using CGUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class EventPoint
    {
        public CGUtilities.Point P;
        public string EventType; //  StartPoint , EndPoint 
        public int index;
    }

    class SweepLine : Algorithm
    {
        public static List<Point> ConvertToPoints(List<Line> pol)
        {
            List<Point> outPoints = new List<Point>();
            for (int i = 0; i < pol.Count; i++)
            {
                if (!outPoints.Contains(pol[i].Start))
                    outPoints.Add(pol[i].Start);
                if (!outPoints.Contains(pol[i].End))
                    outPoints.Add(pol[i].End);
            }
            return outPoints;
        }

        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            List<Line> res = new List<Line>();
            List<Point> intersectionPoints = new List<Point>();

            List<EventPoint> PtQueue = new List<EventPoint>();

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Start.X > lines[i].End.X)
                {
                    Point temp;
                    temp = lines[i].Start;
                    lines[i].Start = lines[i].End;
                    lines[i].End = temp;
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                EventPoint temppoint = new EventPoint();
                EventPoint temppoint2 = new EventPoint();
                temppoint.EventType = "StartPoint";
                temppoint.index = i;
                temppoint.P = lines[i].Start;
                temppoint2.EventType = "EndPoint";
                temppoint2.index = i;
                temppoint2.P = lines[i].End;
                PtQueue.Add(temppoint);
                PtQueue.Add(temppoint2);
            }

            int count = PtQueue.Count;
            EventPoint point_temp;
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    if (PtQueue[i].P.X < PtQueue[j].P.X)
                    {
                        point_temp = PtQueue[i];
                        PtQueue[i] = PtQueue[j];
                        PtQueue[j] = point_temp;
                    }
                }
            }
            PtQueue.Reverse();

            List<EventPoint> ActivitySet = new List<EventPoint>();
            List<EventPoint> AllBelow = new List<EventPoint>();
            List<EventPoint> AllAbove = new List<EventPoint>();
            EventPoint Above, Below;

            for (int i = 0; i < PtQueue.Count; i++)
            {
                AllBelow.Clear();
                AllAbove.Clear();

                if (PtQueue[i].EventType == "StartPoint")
                {
                    ActivitySet.Add(PtQueue[i]);

                    for (int j = 0; j < ActivitySet.Count - 1; j++)
                    {
                        if (ActivitySet[j].P.Y > PtQueue[i].P.Y)
                            AllAbove.Add(ActivitySet[j]);
                        else
                            AllBelow.Add(ActivitySet[j]);
                    }

                    if (AllAbove.Count != 0)
                    {
                        Above = MinY(AllAbove);
                        if (CheckIntersection(lines[PtQueue[i].index], lines[Above.index]) == true)
                        {
                            if (!res.Contains(lines[Above.index]))
                                res.Add(lines[Above.index]);
                            if (!res.Contains(lines[PtQueue[i].index]))
                                res.Add(lines[PtQueue[i].index]);

                            intersectionPoints.Add(CalculateIntersectionPoint(lines[PtQueue[i].index], lines[Above.index]));
                        }
                    }

                    if (AllBelow.Count != 0)
                    {
                        Below = MaxY(AllBelow);
                        if (CheckIntersection(lines[PtQueue[i].index], lines[Below.index]) == true)
                        {
                            if (!res.Contains(lines[Below.index]))
                                res.Add(lines[Below.index]);
                            if (!res.Contains(lines[PtQueue[i].index]))
                                res.Add(lines[PtQueue[i].index]);

                            intersectionPoints.Add(CalculateIntersectionPoint(lines[PtQueue[i].index], lines[Below.index]));
                        }
                    }
                }
                else
                {
                    int StartIndex = 0;
                    for (int j = 0; j < ActivitySet.Count; j++)
                    {
                        if (ActivitySet[j].index == PtQueue[i].index && ActivitySet[j].EventType != PtQueue[i].EventType)
                        {
                            StartIndex = j;
                            break;
                        }
                    }
                    ActivitySet.RemoveAt(StartIndex);

                    for (int j = 0; j < ActivitySet.Count; j++)
                    {
                        if (ActivitySet[j].P.Y > PtQueue[i].P.Y)
                            AllAbove.Add(ActivitySet[j]);
                        else
                            AllBelow.Add(ActivitySet[j]);
                    }

                    if (AllAbove.Count != 0)
                    {
                        Above = MinY(AllAbove);
                        if (CheckIntersection(lines[PtQueue[i].index], lines[Above.index]) == true)
                        {
                            if (!res.Contains(lines[Above.index]))
                                res.Add(lines[Above.index]);
                            if (!res.Contains(lines[PtQueue[i].index]))
                                res.Add(lines[PtQueue[i].index]);

                            intersectionPoints.Add(CalculateIntersectionPoint(lines[PtQueue[i].index], lines[Above.index]));
                        }
                    }

                    if (AllBelow.Count != 0)
                    {
                        Below = MaxY(AllBelow);
                        if (CheckIntersection(lines[PtQueue[i].index], lines[Below.index]) == true)
                        {
                            if (!res.Contains(lines[Below.index]))
                                res.Add(lines[Below.index]);
                            if (!res.Contains(lines[PtQueue[i].index]))
                                res.Add(lines[PtQueue[i].index]);

                            intersectionPoints.Add(CalculateIntersectionPoint(lines[PtQueue[i].index], lines[Below.index]));
                        }
                    }
                }
            }

            //outPoints = ConvertToPoints(res);
            outPoints.AddRange(intersectionPoints);
        }

        public static EventPoint MinY(List<EventPoint> li)
        {
            EventPoint A = li[0];
            for (int i = 1; i < li.Count; i++)
            {
                if (A.P.Y > li[i].P.Y)
                    A = li[i];
            }
            return A;
        }

        public static EventPoint MaxY(List<EventPoint> li)
        {
            EventPoint A = li[0];
            for (int i = 1; i < li.Count; i++)
            {
                if (A.P.Y < li[i].P.Y)
                    A = li[i];
            }
            return A;
        }

        public static int orientation(Point p, Point q, Point r)
        {
            int val = (int)((q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y));

            if (val == 0) return 0;

            return (val > 0) ? 1 : 2;
        }

        public static Boolean CheckIntersection(Line l1, Line l2)
        {
            Point a1, a2, b1, b2;
            a1 = l1.Start;
            a2 = l1.End;
            b1 = l2.Start;
            b2 = l2.End;
            int o1 = orientation(a1, a2, b1);
            int o2 = orientation(a1, a2, b2);
            int o3 = orientation(b1, b2, a1);
            int o4 = orientation(b1, b2, a2);

            if (o1 != o2 && o3 != o4)
                return true;
            else
                return false;
        }

        // Add this method to calculate the intersection point of two lines
        public static Point CalculateIntersectionPoint(Line l1, Line l2)
        {
            double x1 = l1.Start.X;
            double y1 = l1.Start.Y;
            double x2 = l1.End.X;
            double y2 = l1.End.Y;
            double x3 = l2.Start.X;
            double y3 = l2.Start.Y;
            double x4 = l2.End.X;
            double y4 = l2.End.Y;

            double determinant = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (determinant == 0)
            {
                // Lines are parallel, handle this case based on your requirements
                return null; // or throw an exception
            }

            double intersectionX = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / determinant;
            double intersectionY = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / determinant;

            return new Point(intersectionX, intersectionY);
        }
        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
