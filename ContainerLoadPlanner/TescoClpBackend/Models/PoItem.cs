using ClpEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;

namespace TescoClpBackend.Models
{
    public class PoItem : ContainerItem, ICloneable
    {
        public IGrouping<string, ClpItem> Items { get; set; }
        public double TotalCbm { get; set; }
        public int Priority { get; set; }
        public PoItem(IGrouping<string, ClpItem> items)
        {

            Items = items;
            TotalCbm = items.Sum(a => a.Cbm);
            Cbm = TotalCbm;
            Pkgs = items.Sum(a => a.Pkgs);
        }
        public PoItem()
        {
        }
        public object Clone()
        {
            return this.MemberwiseClone();// throw new NotImplementedException();
        }

        public static List<List<PoItem>> SplitPoItems(List<PoItem> poItems, double maxCapacity)
        {
            List<List<PoItem>> result = new List<List<PoItem>>();
            List<PoItem> currentGroup = new List<PoItem>();
            double currentCbm = 0;

            foreach (var poItem in poItems)
            {
                if (currentCbm + poItem.TotalCbm <= maxCapacity)
                {
                    currentGroup.Add(poItem);
                    currentCbm += poItem.TotalCbm;
                }
                else
                {
                    // If adding the current PoItem would exceed the maxCapacity, start a new group
                    result.Add(currentGroup);
                    currentGroup = new List<PoItem> { poItem };
                    currentCbm = poItem.TotalCbm;
                }
            }

            // Add the remaining group
            if (currentGroup.Count > 0)
            {
                result.Add(currentGroup);
            }

            return result;
        }

    }

    public class SkuItem : ContainerItem, ICloneable
    {
        public IGrouping<string, ClpItem> Items { get; set; }
        public double TotalCbm { get; set; }
        public int Priority { get; set; }
        public SkuItem(IGrouping<string, ClpItem> items)
        {

            Items = items;
            TotalCbm = items.Sum(a => a.Cbm);
            Cbm = TotalCbm;
            Pkgs = items.Sum(a => a.Pkgs);
        }

        public SkuItem()
        {
        }
        public object Clone()
        {
            return this.MemberwiseClone();// throw new NotImplementedException();
        }

        public List<SkuItem> Split(double maxCbmPerBox)
        {
            var result = new List<SkuItem>();

            if (Cbm <= maxCbmPerBox)
            {
                // No need to split, just return the original item
                result.Add(this);
            }
            else
            {
                double remainingCbm = Cbm;
                int remainingPkgs = Pkgs;

                while (remainingCbm > 0)
                {
                    // Calculate the number of packages that fit within maxCbmPerBox
                    int pkgsForThisBox = Math.Min(remainingPkgs, (int)(maxCbmPerBox / UnitCbm));
                    double cbmForThisBox = pkgsForThisBox * UnitCbm;

                    // Create a new ContainerItem for this split
                    var newItem = new SkuItem
                    {
                        Items = this.Items,
                        Pkgs = pkgsForThisBox,
                        Cbm = cbmForThisBox,
                        CWeight = (CWeight / Pkgs) * pkgsForThisBox, // Proportionate weight
                        Destination = this.Destination
                    };

                    result.Add(newItem);

                    // Update remaining CBM and packages
                    remainingCbm -= cbmForThisBox;
                    remainingPkgs -= pkgsForThisBox;
                }
            }

            return result;
        }

        public static List<List<SkuItem>> SplitSkuItems(List<SkuItem> skuItems, double maxCapacity)
        {

            List<List<SkuItem>> result = new List<List<SkuItem>>();
            List<SkuItem> currentGroup = new List<SkuItem>();
            double currentCbm = 0;

            foreach (var skuItem in skuItems)
            {
                if (currentCbm + skuItem.TotalCbm <= maxCapacity)
                {
                    currentGroup.Add(skuItem);
                    currentCbm += skuItem.TotalCbm;
                }
                else
                {
                    // If adding the current PoItem would exceed the maxCapacity, start a new group
                    result.Add(currentGroup);
                    currentGroup = new List<SkuItem> { skuItem };
                    currentCbm = skuItem.TotalCbm;
                }
            }

            // Add the remaining group
            if (currentGroup.Count > 0)
            {
                result.Add(currentGroup);
            }

            return result;

        }
    }
    public class SplitSkuItem
    {

    }
}
