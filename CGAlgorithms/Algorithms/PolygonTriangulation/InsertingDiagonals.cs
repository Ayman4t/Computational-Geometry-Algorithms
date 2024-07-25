using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Check if there are polygons to process
            if (polygons.Count == 0)
                return;

            // Extract the points from the first polygon
            List<Point> polygonPoints = new List<Point>();
            for (int i = 0; i < polygons[0].lines.Count; ++i)
                polygonPoints.Add(polygons[0].lines[i].Start);

            // Ensure the polygon is clockwise
            EnsurePolygonClockwise(polygonPoints);

            // Insert diagonals into the polygon
            outLines = InsertDiagonals(polygonPoints);
        }

        // Insert diagonals into a convex polygon
        private List<Line> InsertDiagonals(List<Point> polygonPoints)
        {
            if (polygonPoints.Count > 3)
            {
                List<Line> result = new List<Line>();
                int convexPointIndex = GetConvexPointIndex(polygonPoints);

                // If no convex point found, return an empty list
                if (convexPointIndex == -1)
                    return new List<Line>();

                int previousIndex = (convexPointIndex - 1 + polygonPoints.Count) % polygonPoints.Count;
                int nextIndex = (convexPointIndex + 1) % polygonPoints.Count;

                int maxPointIndex = FindMaxPoint(polygonPoints, previousIndex, nextIndex, convexPointIndex);

                if (maxPointIndex == -1)
                    result.Add(new Line(polygonPoints[previousIndex], polygonPoints[nextIndex]));
                else
                    result.Add(new Line(polygonPoints[convexPointIndex], polygonPoints[maxPointIndex]));

                List<Point> firstPartition = new List<Point>();
                List<Point> secondPartition = new List<Point>();

                int start = maxPointIndex == -1 ? nextIndex : convexPointIndex;
                int end = maxPointIndex == -1 ? previousIndex : maxPointIndex;

                for (int i = end; i != start; i = (i + 1) % polygonPoints.Count)
                    firstPartition.Add(polygonPoints[i]);

                for (int i = start; i != end; i = (i + 1) % polygonPoints.Count)
                    secondPartition.Add(polygonPoints[i]);

                firstPartition.Add(polygonPoints[start]);
                secondPartition.Add(polygonPoints[end]);

                result.AddRange(InsertDiagonals(firstPartition));
                result.AddRange(InsertDiagonals(secondPartition));

                return result;
            }

            return new List<Line>();
        }

        // Find the index of the point with the maximum distance within the triangle
        private int FindMaxPoint(List<Point> polygonPoints, int previousIndex, int nextIndex, int convexPointIndex)
        {
            double maxDistance = -1e6;
            int maxIndex = -1;

            for (int i = 0; i < polygonPoints.Count; ++i)
            {
                // Check if the point is inside the triangle
                if (HelperMethods.PointInTriangle(polygonPoints[i], polygonPoints[convexPointIndex], polygonPoints[previousIndex], polygonPoints[nextIndex]) == Enums.PointInPolygon.Inside)
                {
                    double distance = LinePointDistance(new Line(polygonPoints[previousIndex], polygonPoints[nextIndex]), polygonPoints[i]);

                    // Update the max distance and index if a greater distance is found
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxIndex = i;
                    }
                }
            }

            return maxIndex;
        }

        // Find the index of the first convex point in the polygon
        private int GetConvexPointIndex(List<Point> polygonPoints)
        {
            for (int i = 0; i < polygonPoints.Count; ++i)
            {
                if (IsConvex(polygonPoints, i))
                    return i;
            }

            return -1;
        }

        // Calculate the distance between a line and a point
        public static double LinePointDistance(Line line, Point point)
        {
            Point vector1 = line.Start.Vector(line.End);
            Point vector2 = line.End.Vector(point);
            double projDistance = Math.Abs(DotProduct(vector2, vector1) / vector1.Magnitude());
            double diagonal = line.End.Vector(point).Magnitude();
            return Math.Sqrt(diagonal * diagonal - projDistance * projDistance);
        }

        // Calculate the dot product of two vectors
        public static double DotProduct(Point vector1, Point vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

        // Check if a point in the polygon is convex
        private bool IsConvex(List<Point> polygonPoints, int index)
        {
            int previousIndex = (index - 1 + polygonPoints.Count) % polygonPoints.Count;
            int nextIndex = (index + 1) % polygonPoints.Count;

            // Check if the turn is right, indicating a convex point
            if (HelperMethods.CheckTurn(new Line(polygonPoints[previousIndex], polygonPoints[nextIndex]), polygonPoints[index]) == Enums.TurnType.Right)
                return true;

            return false;
        }

        // Ensure the polygon is clockwise oriented
        public void EnsurePolygonClockwise(List<Point> polygonPoints)
        {
            int minIndex = 0;

            // Find the minimum X-coordinate point to determine orientation
            for (int i = 0; i < polygonPoints.Count; ++i)
            {
                if (polygonPoints[i].X < polygonPoints[minIndex].X)
                    minIndex = i;
            }

            int previousIndex = (minIndex - 1 + polygonPoints.Count) % polygonPoints.Count;
            int nextIndex = (minIndex + 1 + polygonPoints.Count) % polygonPoints.Count;

            // Reverse the polygon if it is counterclockwise
            if (HelperMethods.CheckTurn(new Line(polygonPoints[previousIndex], polygonPoints[nextIndex]), polygonPoints[minIndex]) == Enums.TurnType.Left)
                polygonPoints.Reverse();
        }

        // Display the algorithm name
        public override string ToString()
        {
            return "Inserting Diagonals";
        }
    }
}
