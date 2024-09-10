using SharedEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Models;

namespace TescoClpBackend.ContainerLoaders
{
    public abstract class ContainerLoader : IContainerLoader
    {
        public List<Container<ClpItem>> Load(Combination combination, ref List<ClpItem> data)
        {
            List<Container<ClpItem>> loadedContainers = new List<Container<ClpItem>>();

            var containers = InitiateContainers(combination);
            var containerCount = containers.Count;

            for (int i = 0; i < containerCount; i++)
            {
                Container<ClpItem> container = containers.Pop();

                double previousCapacity = container.RemainingCapacity;

                var lotGroup = data
                .GroupBy(a => a.CfsReportItem.Lot)
                .Select(lot => new LotItem(lot))

                //.OrderByDescending(a => a.TotalCbm)
                .ToList();

                //If Lot Contains one high priority PO, all PO should be high priority
                lotGroup.ForEach(a =>
                {
                    if (a.Items.Any(item => item.PoUploadReportItem.Priority>0))
                    {
                        a.Priority = Int32.MaxValue;
                    }
                });
                lotGroup = lotGroup
                    //.OrderByDescending(a => a.Priority)
                    .OrderByDescending(a => a.TotalCbm)
                    .ToList();

                LoadUntilCap(ref lotGroup, ref container, previousCapacity);

                data.RemoveAll(a => container.Items.Contains(a));
                if (container.RemainingCapacity < 0.5)
                {
                    //   loadedContainer.Add(container);
                    if (!loadedContainers.Contains(container))
                        loadedContainers.Add(container);

                    //   break;
                }
                if (!loadedContainers.Contains(container))

                    loadedContainers.Add(container);
                Debug.WriteLine($"Container:{container.ContainerId};-CBM:{container.Items.Sum(a => a.CfsReportItem.Cbm)}");
            }
            return loadedContainers;
        }


        protected virtual double LoadQn(ref List<LotItem> lotList, ref Container<ClpItem> container)
        {
            lotList.OrderByDescending(a => a.TotalCbm).ToList();


            var capacity = container.RemainingCapacity;
            var viableLot = lotList.Where(a => (capacity - a.TotalCbm) > 0).ToList();
            if (viableLot.Count() > 0)
            {
                //   var viableLots = lotList.Where(a => (capacity - a.TotalCbm) >= 0);
                var closest = viableLot

                    .Aggregate((x, y) =>
                     (capacity - x.TotalCbm) >= 0
                    && (capacity - y.TotalCbm) >= 0
                    &&
                     (capacity - x.TotalCbm) < (capacity - y.TotalCbm)
                    ? x : y);
                var cloned = closest.Clone() as LotItem;
                if (container.Items.Sum(a => a.CfsReportItem.Cbm) + closest.TotalCbm
                    <= container.MaxCapacity)
                {
                    container.Items.AddRange(closest.Items);
                    lotList.Remove(closest);
                    container.UsedCbm+= closest.TotalCbm;
                   // container.RemainingCapacity -= closest.TotalCbm;
                }


            }
            return container.RemainingCapacity;

            //var loaded=   MaximizeSackUsage( lotList, container.RemainingCapacity);
            //   var loadedItems = loaded.Select(a => a.Item).ToList();
            //   foreach (var item in loadedItems)
            //   {
            //       var x=item.ToList();
            //       container.Items.AddRange(x);
            //       lotList.RemoveAll(a=>a==item);
            //       //container.RemainingCapacity -= x.Sum(a => a.CfsReportItem.Cbm);
            //   }

            //return container.RemainingCapacity;
        }


        public static List<LotItem> MaximizeSackUsage( List<LotItem> lotItems,double sackCapacity)
        {
            
            // Convert TotalCbm to integer values by scaling to avoid floating-point precision issues
            int scaleFactor = 10000;
            int scaledCapacity = (int)(sackCapacity * scaleFactor);
            List<int> scaledCbm = lotItems.Select(lot => (int)(lot.TotalCbm * scaleFactor)).ToList();

            // DP array to store the maximum weight possible without exceeding the scaled capacity
            int[] dp = new int[scaledCapacity + 1];
            bool[] isPossible = new bool[scaledCapacity + 1];
            isPossible[0] = true;

            // Track which LotItems are used to achieve the best weight
            int[] lotUsed = new int[scaledCapacity + 1];

            // Dynamic Programming to find the closest weight to the sack capacity
            for (int i = 0; i < scaledCbm.Count; i++)
            {
                for (int j = scaledCapacity; j >= scaledCbm[i]; j--)
                {
                    if (isPossible[j - scaledCbm[i]])
                    {
                        int newWeight = dp[j - scaledCbm[i]] + scaledCbm[i];
                        if (newWeight > dp[j])
                        {
                            dp[j] = newWeight;
                            isPossible[j] = true;
                            lotUsed[j] = i;
                        }
                    }
                }
            }

            // Find the best weight close to the sack capacity
            int bestWeight = 0;
            for (int i = 0; i <= scaledCapacity; i++)
            {
                if (isPossible[i] && dp[i] > bestWeight)
                {
                    bestWeight = dp[i];
                }
            }

            // Backtrack to find the LotItems used
            List<LotItem> selectedLotItems = new List<LotItem>();
            while (bestWeight > 0)
            {
                int usedLotIndex = lotUsed[bestWeight];
                selectedLotItems.Add(lotItems[usedLotIndex]);
                bestWeight -= scaledCbm[usedLotIndex];
            }
            

            return selectedLotItems;
        }




        private void LoadUntilCap(ref List<LotItem> lotGroup, ref Container<ClpItem> container, double previousCapacity)
        {
            
            while (container.RemainingCapacity > 0 && lotGroup.Count() > 0)
            {
                previousCapacity = container.RemainingCapacity;
                /*(dcList,container)=*/
                var capacity = LoadQn(ref lotGroup, ref container);
                if (previousCapacity == capacity)
                {
                    break;
                }


            }
        }

        protected abstract Stack<Container<ClpItem>> InitiateContainers(Combination combination);


        public void FillUpUnderUtilizedContainer(Container<ClpItem> container, ref List<ClpItem> nonPriorityGroup)
        {
            var lg = nonPriorityGroup
                .GroupBy(a => a.CfsReportItem.Lot)
                .Select(lot => new LotItem(lot))
                .ToList();
            
            LoadUntilCap(ref lg, ref container,container.RemainingCapacity);
            nonPriorityGroup.RemoveAll(a => container.Items.Contains(a));
            //var sum = 0d;
            //var itemsToTake = lg.TakeWhile(c => (sum + c.TotalCbm) < container.RemainingCapacity).ToList();
            ////container.Items.AddRange(itemsToTake);
            //foreach (var item in itemsToTake)
            //{
            //    container.Items.AddRange(item.Item.ToList());
            //}
            //foreach (var item in itemsToTake)
            //{
            //    nonPriorityGroup.RemoveAll(a => item.Item.Select(i => i).Contains(a));
            //}
        }
    }

}
