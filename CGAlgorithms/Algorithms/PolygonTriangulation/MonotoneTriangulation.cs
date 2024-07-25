using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;



namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Polygon pol = new Polygon(lines);



            for (int i = 0; i < pol.lines.Count; i++)
                points.Add(pol.lines[i].Start);

            //sort the points on max Y and max X 
            List<Point> E = new List<Point>();
            for (int i = 0; i < pol.lines.Count; i++)
                E.Add(pol.lines[i].Start);
            E.Sort(point_sort);

            Stack<Point> S = new Stack<Point>();
            S.Push(E[0]);
            S.Push(E[1]);
            int indx = 2;

            while (indx != pol.lines.Count)
            {


                Point p = E[indx];
                Point last = S.Peek(); ///3

                bool same_side = false;
                if (p.X < E[0].X && last.X < E[0].X)
                    same_side = true;
                else if (p.X > E[0].X && last.X > E[0].X)
                    same_side = true;


                // P and Top on the same side 
                if (same_side == true)
                {
                    S.Pop();   /// pop top 3
                    Point Blast = S.Peek();  //// peek 2 

                    //Check the top point is convex 
                    int index = points.IndexOf(last);
                    if (CheckConvex(pol, index) == true)
                    {
                        outLines.Add(new Line(p, Blast));
                        if (S.Count == 1)
                        {
                            S.Push(p);
                            indx++;
                        }
                    }
                    else
                    {
                        S.Push(last);
                        S.Push(p);
                        indx++;
                    }
                }
                //P and Top on different side 
                else
                {
                    while (S.Count != 1)
                    {
                        Point Blast = S.Pop();
                        outLines.Add(new Line(p, Blast));
                    }
                    S.Pop();
                    S.Push(last);
                    S.Push(p);
                }
            }
        }



        //Check Convex  vertex  angel <180 
        public bool CheckConvex(Polygon p, int Curr)
        {
            int prev = ((Curr - 1) + p.lines.Count) % p.lines.Count;
            int next = (Curr + 1) % p.lines.Count;

            Point p1 = p.lines[prev].Start;
            Point p2 = p.lines[Curr].Start;
            Point p3 = p.lines[next].Start;
            Line l = new Line(p1, p2);
            if (HelperMethods.CheckTurn(l, p3) == Enums.TurnType.Left)
                return true;
            return false;
        }

        //sort the points on max Y and max X   
        public static int point_sort(Point p1, Point p2)
        {
            if (p1.Y == p2.Y)
                return -p1.X.CompareTo(p2.X);
            else
                return -p1.Y.CompareTo(p2.Y);
        }


        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}