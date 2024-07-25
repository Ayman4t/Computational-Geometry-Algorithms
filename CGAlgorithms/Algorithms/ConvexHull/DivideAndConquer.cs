using CGUtilities;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        // Run the Divide and Conquer Convex Hull algorithm
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Sort points lexicographically
            points = points.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
            // Get the convex hull
            outPoints = GetConvexHull(points);
        }

        // Find the index of the point with the minimum X-coordinate
        public static int FindMinXIndex(List<Point> points)
        {
            int index = points
                .Select((point, i) => new { Point = point, Index = i })
                .OrderBy(item => item.Point.X)
                .ThenBy(item => item.Point.Y)
                .First()
                .Index;

            return index;
        }

        // Find the index of the point with the maximum X-coordinate
        public static int FindMaxXIndex(List<Point> points)
        {
            int index = points
                .Select((point, i) => new { Point = point, Index = i })
                .OrderByDescending(item => item.Point.X)
                .ThenByDescending(item => item.Point.Y)
                .First()
                .Index;

            return index;
        }

        // Merge two convex hulls into one
        public List<Point> MergeConvexHulls(List<Point> leftHull, List<Point> rightHull)
        {
            int maxLeftIndex = FindMaxXIndex(leftHull);
            int minRightIndex = FindMinXIndex(rightHull);
            int leftCount = leftHull.Count;
            int rightCount = rightHull.Count;
            bool found;

            // Upper tangent
            int upperLeftIndex = maxLeftIndex;
            int upperRightIndex = minRightIndex;
            int nextLeftIndex = (upperLeftIndex + 1) % leftCount;
            int prevRightIndex = (rightCount + upperRightIndex - 1) % rightCount;
            // Lower tangent
            int lowerLeftIndex = maxLeftIndex;
            int lowerRightIndex = minRightIndex;
            int prevLeftIndex = (leftCount + lowerLeftIndex - 1) % leftCount;
            int nextRightIndex = (lowerRightIndex + 1) % rightCount;
            //iterate on  the lower tangent and upper from left hull
            do
            {
                found = true;

                // Update the lower tangent 
                while (HelperMethods.CheckTurn(new Line(rightHull[lowerRightIndex], leftHull[lowerLeftIndex]), leftHull[prevLeftIndex]) == Enums.TurnType.Left)
                {
                    lowerLeftIndex = prevLeftIndex;
                    prevLeftIndex = (leftCount + lowerLeftIndex - 1) % leftCount;
                    found = false;
                }

                if (found && (HelperMethods.CheckTurn(new Line(rightHull[lowerRightIndex], leftHull[lowerLeftIndex]), leftHull[prevLeftIndex]) == Enums.TurnType.Colinear))
                    lowerLeftIndex = prevLeftIndex;
                prevLeftIndex = (leftCount + lowerLeftIndex - 1) % leftCount;

                // Update the upper tangent
                while (HelperMethods.CheckTurn(new Line(leftHull[lowerLeftIndex], rightHull[lowerRightIndex]), rightHull[nextRightIndex]) == Enums.TurnType.Right)
                {
                    lowerRightIndex = nextRightIndex;
                    nextRightIndex = (lowerRightIndex + 1) % rightCount;
                    found = false;
                }

                if (found && (HelperMethods.CheckTurn(new Line(leftHull[lowerLeftIndex], rightHull[lowerRightIndex]), rightHull[nextRightIndex]) == Enums.TurnType.Colinear))
                    lowerRightIndex = nextRightIndex;
                nextRightIndex = (lowerRightIndex + 1) % rightCount;

            } while (!found);
            // ititerate on  the lower tangent and upper from right hull
            do
            {
                found = true;

                // Update the upper tangent
                while (HelperMethods.CheckTurn(new Line(rightHull[upperRightIndex], leftHull[upperLeftIndex]), leftHull[nextLeftIndex]) == Enums.TurnType.Right)
                {
                    upperLeftIndex = nextLeftIndex;
                    nextLeftIndex = (upperLeftIndex + 1) % leftCount;
                    found = false;
                }

                if (found && (HelperMethods.CheckTurn(new Line(rightHull[upperRightIndex], leftHull[upperLeftIndex]), leftHull[nextLeftIndex]) == Enums.TurnType.Colinear))
                    upperLeftIndex = nextLeftIndex;
                nextLeftIndex = (upperLeftIndex + 1) % leftCount;

                // Update the lower tangent
                while (HelperMethods.CheckTurn(new Line(leftHull[upperLeftIndex], rightHull[upperRightIndex]), rightHull[prevRightIndex]) == Enums.TurnType.Left)
                {
                    upperRightIndex = prevRightIndex;
                    prevRightIndex = (rightCount + upperRightIndex - 1) % rightCount;
                    found = false;
                }

                if (found && (HelperMethods.CheckTurn(new Line(leftHull[upperLeftIndex], rightHull[upperRightIndex]), rightHull[prevRightIndex]) == Enums.TurnType.Colinear))
                    upperRightIndex = prevRightIndex;
                prevRightIndex = (rightCount + upperRightIndex - 1) % rightCount;

            } while (!found);
            //merge part
            // out put of the points
            List<Point> mergedHull = new List<Point>();

            int index = upperLeftIndex;
            if (!mergedHull.Contains(leftHull[upperLeftIndex]))
                mergedHull.Add(leftHull[upperLeftIndex]);

            while (index != lowerLeftIndex)
            {
                index = (index + 1) % leftCount;
                if (!mergedHull.Contains(leftHull[index]))
                    mergedHull.Add(leftHull[index]);
            }

            index = lowerRightIndex;
            if (!mergedHull.Contains(rightHull[lowerRightIndex]))
                mergedHull.Add(rightHull[lowerRightIndex]);

            while (index != upperRightIndex)
            {
                index = (index + 1) % rightCount;
                if (!mergedHull.Contains(rightHull[index]))
                    mergedHull.Add(rightHull[index]);
            }

            return mergedHull;
        }

        // Get the convex hull using the Divide and Conquer approach
        public List<Point> GetConvexHull(List<Point> pointList)
        {
            // Base case: if there's only one point, it's already the convex hull
            if (pointList.Count == 1)
            {
                return pointList;
            }

            // Split the points into two halves
            List<Point> leftPoints = new List<Point>();
            List<Point> rightPoints = new List<Point>();

            for (int i = 0; i < pointList.Count / 2; i++)
                leftPoints.Add(pointList[i]);

            for (int i = pointList.Count / 2; i < pointList.Count; i++)
                rightPoints.Add(pointList[i]);

            // Recursively get the convex hulls of the two halves
            List<Point> leftHull = GetConvexHull(leftPoints);
            List<Point> rightHull = GetConvexHull(rightPoints);

            // Merge the convex hulls
            return MergeConvexHulls(leftHull, rightHull);
        }

        // Display the algorithm name
        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }
    }
}
