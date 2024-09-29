using ClpEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;

namespace TescoClpBackend.Models
{

    public class LotItem :/*ItemUnit<ClpItem>*/ContainerItem, ICloneable
    {
        public Guid Id { get; set; }
        public IGrouping<int, ClpItem> Items { get; set; }
        public double TotalCbm { get; set; }
        public int Priority { get; set; }
        public LotItem(IGrouping<int, ClpItem> items)
        {
            
            Id=Guid.NewGuid();
            Items = items;
            TotalCbm = items.Sum(a => a.Cbm);

            Cbm = TotalCbm;
            Pkgs = items.Sum(a => a.Pkgs);
        }
      
        public LotItem()
        {
            Id = Guid.NewGuid();
        }
        public object Clone()
        {
            return this.MemberwiseClone();// throw new NotImplementedException();
        }

        //internal object Clone()
        //{
        //    return this;
        //}
        public static LotItem ToLotItem(SkuItem skuItem)
        {
            var items=skuItem.Items.ToList();
            var group=items.GroupBy(a => a.CfsReportItem.Lot);
            LotItem item = new LotItem()
            {
                Items = group.FirstOrDefault(),
               
                Pkgs=skuItem.Pkgs,
                Destination=skuItem.Destination,
                TotalCbm = skuItem.UnitCbm*skuItem.Pkgs,
                Cbm = skuItem.UnitCbm*skuItem.Pkgs,

            };
            return item;
        }

        public static LotItem ToLotItem(PoItem poItem)
        {
            var items = poItem.Items.ToList();
            var group = items.GroupBy(a => a.CfsReportItem.Lot);
            LotItem item = new LotItem()
            {
                Items = group.FirstOrDefault(),

                Pkgs = poItem.Pkgs,
                Destination = poItem.Destination,
                TotalCbm = poItem.UnitCbm * poItem.Pkgs,
                Cbm = poItem.UnitCbm * poItem.Pkgs,

            };
            return item;
        }


        public static LotItem ToLotItem(List<PoItem> poItems)
        {
            var itemGroups = poItems.Select(a => a.Items);
            var items = itemGroups.SelectMany(groups => groups);
            var lotGroup = items.GroupBy(a => a.CfsReportItem.Lot).Select(i => new LotItem(i));
            return lotGroup.First();
        }

        //public List<List<PoItem>> SplitLotItemByPo(LotItem lotItem, double maxCapacity)
        //{
        //    var poItems = lotItem
        //                 .Items.GroupBy(a => a.CfsReportItem.PO)
        //                 .Select(po => new PoItem(po))
        //                 .ToList();
        //    List<List<PoItem>> result = new List<List<PoItem>>();
        //    List<PoItem> currentSubset = new List<PoItem>();
        //    double currentCbm = 0;

        //    foreach (var poItem in poItems.Where(a=>a.TotalCbm<maxCapacity))
        //    {
        //        if (currentCbm + poItem.TotalCbm <= maxCapacity)
        //        {
        //            currentSubset.Add(poItem);
        //            currentCbm += poItem.TotalCbm;
        //        }
        //        else
        //        {
        //            // If adding this PoItem exceeds maxCapacity, save the current subset and start a new one
        //            result.Add(currentSubset);
        //            currentSubset = new List<PoItem> { poItem };
        //            currentCbm = poItem.TotalCbm;
        //        }
        //    }

        //    // Add the remaining subset
        //    if (currentSubset.Count > 0)
        //    {
        //        result.Add(currentSubset);
        //    }

            

        //    foreach (var poItem in poItems.Where(a=>a.TotalCbm>maxCapacity))
        //    {
        //        var skuItems=SplitPoItemBySku(poItem, maxCapacity);

        //       LotItem.ToLotItem(skuItem)
        //    }


        //}


        public List<List<SkuItem>> SplitPoItemBySku(PoItem poItem, double maxCapacity)
        {
            var skuItems = poItem
                         .Items.GroupBy(a => a.CfsReportItem.TpnLc)
                         .Select(sku => new SkuItem(sku))
                         .ToList();
            List<List<SkuItem>> result = new List<List<SkuItem>>();
            List<SkuItem> currentSubset = new List<SkuItem>();
            double currentCbm = 0;

            foreach (var skuItem in skuItems.Where(a => a.TotalCbm < maxCapacity))
            {
                if (currentCbm + skuItem.TotalCbm <= maxCapacity)
                {
                    currentSubset.Add(skuItem);
                    currentCbm += skuItem.TotalCbm;
                }
                else
                {
                    // If adding this PoItem exceeds maxCapacity, save the current subset and start a new one
                    result.Add(currentSubset);
                    currentSubset = new List<SkuItem> { skuItem };
                    currentCbm = skuItem.TotalCbm;
                }
            }

            // Add the remaining subset
            if (currentSubset.Count > 0)
            {
                result.Add(currentSubset);
            }

            return result;
        }

        internal static LotItem ToLotItem(List<SkuItem> combinedSku)
        {
            var itemGroups = combinedSku.Select(a => a.Items);
            var items = itemGroups.SelectMany(groups => groups);
            var lotGroup = items.GroupBy(a => a.CfsReportItem.Lot).Select(i => new LotItem(i));
            return lotGroup.First();
        }




        public static LotItem ToLotItem(ClpItem item)
        {
            var list = new List<ClpItem>() { item};
            return list.GroupBy(a => a.CfsReportItem.Lot).Select(a=>new LotItem(a)).First();
        }
    }
}
