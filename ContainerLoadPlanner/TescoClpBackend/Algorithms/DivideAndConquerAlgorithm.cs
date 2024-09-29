using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Models;

namespace TescoClpBackend.Algorithms
{
    public class DivideAndConquerAlgorithm : IContainerPacker
    {
        static List<List<LotItem>> FindPartitions(List<LotItem> lotItems, double x, double y)
        {
            // Sort LotItems by TotalCbm in descending order (greedy approach)
            lotItems = lotItems.OrderByDescending(item => item.TotalCbm).ToList();

            List<List<LotItem>> result = new List<List<LotItem>>();
            bool[] used = new bool[lotItems.Count];

            // Greedily find valid subsets and mark used items
            for (int i = 0; i < lotItems.Count; i++)
            {
                if (!used[i])
                {
                    List<LotItem> subset = new List<LotItem>();
                    double sum = 0;

                    for (int j = i; j < lotItems.Count; j++)
                    {
                        if (!used[j] && sum + lotItems[j].TotalCbm <= y)
                        {
                            sum += lotItems[j].TotalCbm;
                            subset.Add(lotItems[j]);
                            used[j] = true;

                            if (sum >= x && sum <= y)
                            {
                                result.Add(subset);
                                break; // Found a valid subset
                            }
                        }

                        // Prune if sum exceeds y
                        if (sum > y)
                        {
                            break;
                        }
                    }
                }
            }
            return result;
        }



            public List<Container<ClpItem>> PackItems(List<LotItem> lotItems, double minCapacity, double maxCapacity)
        {
            List<Container<ClpItem>>containers=new List<Container<ClpItem>>();

            lotItems = lotItems.OrderByDescending(a => a.TotalCbm).ToList();
            var overflowLots = lotItems.Where(a => a.TotalCbm > maxCapacity).ToList();
            lotItems.RemoveAll(a => overflowLots.Contains(a));
            foreach (var lot in overflowLots)
            {
                if (lot.TotalCbm < maxCapacity)
                    continue;
                else
                {
                    var poItems = lot
                        .Items.GroupBy(a => a.CfsReportItem.PO)
                        .Select(po => new PoItem(po))
                        .ToList();
                    var overflowPos = poItems.Where(a => a.TotalCbm > maxCapacity).ToList();
                    poItems.RemoveAll(a => overflowPos.Contains(a));
                    var combinedPoItems = PoItem.SplitPoItems(poItems, maxCapacity).ToList();
                    foreach (var combined in combinedPoItems)
                    {
                        var lotItem = LotItem.ToLotItem(combined);
                        lotItems.Insert(0,lotItem);

                    }
                    foreach (var overFlowPoItem in overflowPos)
                    {
                        if (overFlowPoItem.TotalCbm < maxCapacity)
                            continue;
                        else
                        {
                            var skuItems = overFlowPoItem
                                .Items.GroupBy(a => a.CfsReportItem.TpnLc)
                                .Select(sku => new SkuItem(sku))
                                .ToList();

                            var overflowSkus = skuItems.Where(a => a.TotalCbm > maxCapacity).ToList();

                            skuItems.RemoveAll(a => overflowSkus.Contains(a));

                            var combinedSkuItems = SkuItem.SplitSkuItems(skuItems, maxCapacity).ToList();
                            foreach (var combinedSku in combinedSkuItems)
                            {
                                LotItem lotItem = LotItem.ToLotItem(combinedSku);
                              //  lotItems.Add(lotItem);
                                lotItems.Insert(0, lotItem);
                            }
                            foreach (var overFlowSku in overflowSkus)
                            {
                                if (overFlowSku.TotalCbm < maxCapacity)
                                    continue;
                                else
                                {
                                    var line = overFlowSku.Items.FirstOrDefault();
                                    var splitItems = line.Split(maxCapacity);
                                    foreach (var item in splitItems)
                                    {
                                        var lotItem = LotItem.ToLotItem(item);
                                        //lotItems.Add(lotItem);
                                        lotItems.Insert(0, lotItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var subsets = FindPartitions(lotItems, minCapacity, maxCapacity);
            foreach (var subset in subsets.OrderByDescending(a=>a.Sum(c=>c.TotalCbm)))
            {
                Container<ClpItem> c = new Container<ClpItem>("40HI");
                foreach (var item in subset)
                {
                    c.AddItems(item.Items);
                    
                }
                containers.Add(c);
            }
            return containers;
        }
    }
}
