using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClpEngine
{
    public class BackTrackAlgorithm
    {

        static List<int> FindMaxSubset(int[] A, int x, int y)
        {
            List<int> result = new List<int>();

            // Helper function for backtracking
            void Backtrack(int start, List<int> currentSet, int currentSum)
            {
                // Check if the current sum is within the bounds
                if (currentSum > x && currentSum < y)
                {
                    // Update the result if the current set is larger
                    if (currentSet.Count > result.Count)
                    {
                        result = new List<int>(currentSet);
                    }
                }

                // Explore further combinations
                for (int i = start; i < A.Length; i++)
                {
                    int newSum = currentSum + A[i];

                    // Stop if the new sum exceeds the upper bound
                    if (newSum >= y)
                        continue;

                    // Include A[i] and proceed recursively
                    currentSet.Add(A[i]);
                    Backtrack(i + 1, currentSet, newSum);

                    // Backtrack by removing the last element
                    currentSet.RemoveAt(currentSet.Count - 1);
                }
            }

            // Start backtracking from the first element
            Backtrack(0, new List<int>(), 0);
            return result;
        }

    }
}
