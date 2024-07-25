using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {

        // This method checks whether two points are approximately equal within a small epsilon value.
        private bool ArePointsEqual(Point a, Point b)
        {
            // Check if the absolute difference between X and Y coordinates is within a small epsilon value.
            return Math.Abs(a.X - b.X) <= Constants.Epsilon && Math.Abs(a.Y - b.Y) <= Constants.Epsilon;
        }

        // This method compares two points based on their X and Y coordinates.
        // If X coordinates are equal, it compares Y coordinates.
        private int ComparePoints(Point a, Point b)
        {
            // If X coordinates are equal, compare Y coordinates.
            if (a.X == b.X)
                return a.Y.CompareTo(b.Y);

            // If X coordinates are different, compare X coordinates.
            return a.X.CompareTo(b.X);
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // handle special cases 
            if (points.Count < 2)
            {
                outPoints.AddRange(points);
                return;
            }

            points.Sort(ComparePoints);

            // Initialize arrays to store the next and previous indices for each point in the 'points' list.
            int[] next = new int[points.Count];
            int[] prev = new int[points.Count];

            // Initialize an index 'ind' to traverse the 'points' list.
            int ind = 1;

            // Find the first index in 'points' that is not equal to the initial point (points[0]).
            while (ind < points.Count && ArePointsEqual(points[0], points[ind]))
                ind++;

            // Check if all points in the 'points' list are equal to the initial point.
            if (ind == points.Count)
            {
                // All points are equal, add the single point to the result and return.
                outPoints.Add(points[0]);
                return;
            }
            else
            {
                // Set up the initial next and previous indices for the initial point (points[0]).
                next[0] = ind;
                next[ind] = 0;
                prev[0] = ind;
                prev[ind] = 0;
            }


            int mostRightIndex = ind;

            // Iterate through the points starting from the previously determined index.
            for (ind = ind + 1; ind < points.Count; ind++)
            {
                // Get the current point in the iteration.
                Point newPoint = points[ind];

                // Check if the current point is above or on the rightmost point.
                if (newPoint.Y >= points[mostRightIndex].Y)
                {
                    // Set the next and previous indices accordingly.
                    next[ind] = next[mostRightIndex];
                    prev[ind] = mostRightIndex;
                }
                else
                {
                    // Set the next and previous indices accordingly.
                    next[ind] = mostRightIndex;
                    prev[ind] = prev[mostRightIndex];
                }

                // Check if the current point is equal to the rightmost point, and continue to the next iteration if true.
                if (ArePointsEqual(newPoint, points[mostRightIndex]))

                    continue;

                // Update the next and previous indices to maintain the convex hull.
                next[prev[ind]] = ind;
                prev[next[ind]] = ind;

                // Find the upper tangent by adjusting the previous index.
                for (; ; )
                {
                    Line seg = new Line(newPoint, points[prev[ind]]);
                    Point nextPoint = points[prev[prev[ind]]];
                    Enums.TurnType turn = HelperMethods.CheckTurn(seg, nextPoint);

                    if (turn != Enums.TurnType.Right)
                    {
                        // Adjust the previous index and update the next index.
                        prev[ind] = prev[prev[ind]];
                        next[prev[ind]] = ind;

                        // Break if the points are colinear.
                        if (turn == Enums.TurnType.Colinear)
                            break;
                    }
                    else
                        break;
                }

                // Find the lower tangent by adjusting the next index.
                for (; ; )
                {
                    Line seg = new Line(newPoint, points[next[ind]]);
                    Point nextPoint = points[next[next[ind]]];
                    Enums.TurnType turn = HelperMethods.CheckTurn(seg, nextPoint);

                    if (turn != Enums.TurnType.Left)
                    {
                        // Adjust the next index and update the previous index.
                        next[ind] = next[next[ind]];
                        prev[next[ind]] = ind;

                        // Break if the points are colinear.
                        if (turn == Enums.TurnType.Colinear)
                            break;
                    }
                    else
                        break;
                }

                // Update the rightmost point index.
                mostRightIndex = ind;
            }

            ind = 0;
            for (; ; )
            {
                //add the final points into outpoints 
                outPoints.Add(points[ind]);
                // Move to the next point in the convex hull using the next index.

                ind = next[ind];
                // Break the loop if we reach the starting point (index 0).

                if (ind == 0)
                {
                    break;
                }
            }

        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}