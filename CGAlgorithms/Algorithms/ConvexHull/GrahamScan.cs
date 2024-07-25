using CGUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Stack<Point> pst = new Stack<Point>();

            int countpoint = points.Count;

            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }



            //find lowest point in Y
            /////points = points.OrderBy(point => point.Y).ToList();
            Point p = points[0];
            for (int i = 0; i < countpoint; i++)
            {
                if (points[i].Y < p.Y)
                    p = points[i];
            }



            points.Remove(p);
            pst.Push(p);

            //// sort points with the right most angle
            points = points.OrderBy(point => Math.Atan2(point.Y - p.Y, point.X - p.X)).ToList();

            countpoint--;
            pst.Push(points[0]);
            Point p1, p2, p3;
            for (int i = 1; i < countpoint; i++)
            {

                p3 = points[i];
                p2 = pst.Pop();
                p1 = pst.Peek();

                Line p1p2line = new Line(p1, p2);



                while (HelperMethods.CheckTurn(new Line(p1, p2), p3) != Enums.TurnType.Left)
                {
                    if (pst.Count == 1)
                        break;
                    // b = a
                    p2 = pst.Pop();
                    p1 = pst.Peek();
                }

                if (!pst.Contains(p2))
                    pst.Push(p2);

                if (!pst.Contains(p3))
                    pst.Push(p3);
            }

            outPoints = pst.ToList();


        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }


    }

}