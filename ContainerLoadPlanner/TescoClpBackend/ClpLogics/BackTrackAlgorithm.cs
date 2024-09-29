using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class BackTrackAlgorithm:IContainerPacker
    {

        static List<List<LotItem>> FindMaxSubsetsWithPruning(LotItem[] lotItems, double x, double y)
        {
            // Sort LotItems by TotalCbm in descending order for better pruning
            Array.Sort(lotItems, (a, b) => b.TotalCbm.CompareTo(a.TotalCbm));

            // List to store the final set of subsets
            List<List<LotItem>> result = new List<List<LotItem>>();

            // Backtracking to find the subsets with pruning
            Backtrack(lotItems, x, y, new List<LotItem>(), 0.0, 0, new bool[lotItems.Length], result);

            return result;
            
        }

        // Backtracking helper method with pruning
        static void Backtrack(LotItem[] lotItems, double x, double y, List<LotItem> currentSubset, double currentSum, int start, bool[] used, List<List<LotItem>> result)
        {
            // If the current sum is within the range, save this subset
            if (currentSum > x && currentSum < y)
            {
                result.Add(new List<LotItem>(currentSubset));
            }

            // Try to include more elements in the current subset
            for (int i = start; i < lotItems.Length; i++)
            {
                // Skip the element if it is already used in a previous subset
                if (used[i]) continue;

                // If adding this item makes the sum exceed 'y', prune the branch
                if (currentSum + lotItems[i].TotalCbm > y)
                {
                    continue; // Prune this branch
                }

                // Add the current LotItem's TotalCbm to the subset
                currentSubset.Add(lotItems[i]);
                currentSum += lotItems[i].TotalCbm;
                used[i] = true;

                // Recursively try to add more elements with pruning
                Backtrack(lotItems, x, y, currentSubset, currentSum, i + 1, used, result);

                // Backtrack: remove the current element and try the next
                currentSubset.RemoveAt(currentSubset.Count - 1);
                currentSum -= lotItems[i].TotalCbm;
                used[i] = false;

                // Prune if adding any more elements is impossible
                if (currentSum >= y) break;
            }
        }

     

        public List<Container<ClpItem>> PackItems(List<LotItem> lotItems, double minCapacity, double maxCapacity)
        {
            var exceedingLots= lotItems.Where(a=>a.TotalCbm>maxCapacity).ToList();
            lotItems.RemoveAll(a=>exceedingLots.Contains(a));
            foreach (var exceedingLot
                in exceedingLots)
            {
                var poItems= exceedingLot.Items.GroupBy(a => a.CfsReportItem.PO).Select(a => new PoItem(a)).ToList();

               var exceedingPos=poItems.Where(a=>a.TotalCbm > maxCapacity).ToList();
               var fittingPos = poItems.Where(a => a.TotalCbm <= maxCapacity).ToList();
                foreach (var exceedingPo in exceedingPos)
                {
                    var skuItems=exceedingPo.Items.GroupBy(a=>a.CfsReportItem.TpnLc).Select(a => new SkuItem (a)).ToList();

                    var exceedingSkus = skuItems.Where(a => a.TotalCbm > maxCapacity).ToList();
                    var fittingSkus=skuItems.Where(a=>a.TotalCbm <= maxCapacity).ToList();
                    foreach (var exceedingSku in exceedingSkus)
                    {
                     var splitSkus=   exceedingSku.Split(maxCapacity);
                        foreach (var splitItem in splitSkus)
                        {
                            var splitSkuLot = LotItem.ToLotItem(splitItem);
                            lotItems.Add(splitSkuLot);
                        }
                    }
                    foreach (var fittingSku in fittingSkus)
                    {
                        var fittingSkuLotItem=LotItem.ToLotItem(fittingSku);
                        lotItems.Add(fittingSkuLotItem);
                    }
                }
                foreach (var fittingPo in fittingPos)
                {
                    var fittingPoLotItem = LotItem.ToLotItem(fittingPo);
                    lotItems.Add(fittingPoLotItem);
                }
            }



            List<Container<ClpItem>>containers=new List<Container<ClpItem>>();
         var set=  FindMaxSubsetsWithPruning(lotItems.ToArray(), minCapacity, maxCapacity);
            foreach (var subset in set)
            {
                Container<ClpItem> c = new Container<ClpItem>("40HI");
                foreach (var item in subset)
                {
                    c.Items.AddRange(item.Items);
                }
            containers.Add(c);
            }
            return containers;
        }
    }
}

