using ClpEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    //public class GenericBackTrackingAlgorithm<T> 
    //{
    //    protected GenericBackTrackingAlgorithm() { }

    //    static List<List<T>> FindMaxSubsets(T[] lotItems, double x, double y)
    //    {
    //        // List to store the final set of subsets
    //        List<List<T>> result = new List<List<T>>();

    //        // Backtracking to find the subsets
    //        Backtrack(lotItems, x, y, new List<T>(), 0.0, 0, new bool[lotItems.Length], result);

    //        // Print the result
    //        Console.WriteLine($"Number of valid subsets: {result.Count}");

    //        return result;
    //    }

    //    // Backtracking helper method
    //    static void Backtrack(T[] lotItems, double x, double y, List<T> currentSubset, double currentSum, int start, bool[] used, List<List<T>> result)
    //    {
    //        // If the current sum is within the range, save this subset
    //        if (currentSum > x && currentSum < y)
    //        {
    //            result.Add(new List<T>(currentSubset));
    //        }

    //        // Try to include more elements in the current subset
    //        for (int i = start; i < lotItems.Length; i++)
    //        {
    //            // Skip the element if it is already used in a previous subset
    //            if (used[i]) continue;

    //            // Add the current LotItem's TotalCbm to the subset
    //            currentSubset.Add(lotItems[i]);
    //            currentSum += lotItems[i].Cbm;
    //            used[i] = true;

    //            // Recursively try to add more elements
    //            Backtrack(lotItems, x, y, currentSubset, currentSum, i + 1, used, result);

    //            // Backtrack: remove the current element and try the next
    //            currentSubset.RemoveAt(currentSubset.Count - 1);
    //            currentSum -= lotItems[i].Cbm;
    //            used[i] = false;
    //        }
    //    }
    //}
}
