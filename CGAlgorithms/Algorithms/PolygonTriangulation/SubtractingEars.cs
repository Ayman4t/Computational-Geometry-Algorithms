using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        LinkedList<Point> point = new LinkedList<Point>();
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (polygons.Count == 0)
                return;

            for (int i = 0; i < polygons[0].lines.Count; ++i)
                point.AddLast(polygons[0].lines[i].Start);

            checkPolygon();
            Queue<LinkedListNode<Point>> E = new Queue<LinkedListNode<Point>>();

            for (LinkedListNode<Point> curr = point.First; curr != point.Last.Next; curr = curr.Next)
                if (isEar(curr))
                    E.Enqueue(curr);



            while (E.Count > 0)
                subtractEar(E, outLines);
        }

        private void subtractEar(Queue<LinkedListNode<Point>> Ears, List<Line> outLines)
        {
            LinkedListNode<Point> cur = Ears.Dequeue();

            if (cur == null || !isEar(cur) || (cur.Next == null && cur.Previous == null))
                return;
            if (point.Count == 3)
            { Ears.Clear(); return; }

            LinkedListNode<Point> next = cur.Next == null ? point.First : cur.Next;
            LinkedListNode<Point> prev = cur.Previous == null ? point.Last : cur.Previous;
            point.Remove(cur);
            outLines.Add(new Line(prev.Value, next.Value));

            Ears.Enqueue(prev);
            Ears.Enqueue(next);

        }



        private bool isEar(LinkedListNode<Point> p)
        {
            if (!isConvex(p))
                return false;
            LinkedListNode<Point> next = p.Next == null ? point.First : p.Next;
            LinkedListNode<Point> prev = p.Previous == null ? point.Last : p.Previous;

            for (LinkedListNode<Point> cur = point.First; cur != point.Last.Next; cur = cur.Next)
            {

                if (HelperMethods.PointInTriangle(cur.Value, prev.Value, next.Value, p.Value) == Enums.PointInPolygon.Inside)
                    return false;
            }
            return true;
        }


        private bool isConvex(LinkedListNode<Point> p)
        {
            LinkedListNode<Point> next = p.Next == null ? point.First : p.Next;
            LinkedListNode<Point> prev = p.Previous == null ? point.Last : p.Previous;
            if (HelperMethods.CheckTurn(new Line(prev.Value, p.Value), next.Value) == Enums.TurnType.Left)
                return true;
            return false;
        }
        /// check Counterclock
        public void checkPolygon()
        {
            LinkedListNode<Point> minP = new LinkedListNode<Point>(new Point(1e8, 0));
            for (LinkedListNode<Point> cur = point.First; cur != point.Last.Next; cur = cur.Next)
                if (cur.Value.X < minP.Value.X)
                    minP = cur;
            LinkedListNode<Point> next = minP.Next == null ? point.First : minP.Next;
            LinkedListNode<Point> prev = minP.Previous == null ? point.Last : minP.Previous;
            if (HelperMethods.CheckTurn(new Line(prev.Value, next.Value), minP.Value) == Enums.TurnType.Left)
                Reverse();
        }
        private void Reverse()
        {
            LinkedList<Point> newPP = new LinkedList<Point>();
            for (LinkedListNode<Point> cur = point.Last; cur != point.First.Previous; cur = cur.Previous)
                newPP.AddLast(cur.Value);
            point = newPP;
        }
        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }
}