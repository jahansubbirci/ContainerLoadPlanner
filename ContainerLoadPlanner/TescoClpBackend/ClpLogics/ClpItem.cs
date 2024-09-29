using ClpEngine;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class ClpItem : ContainerItem,ICloneable
    {
        private CfsReport cfsReportItem;

        public CfsReport CfsReportItem
        {
            get { return cfsReportItem; }
            set
            {
                cfsReportItem = value;
                this.Cbm = cfsReportItem.Cbm;
                this.Pkgs=cfsReportItem.Pkgs;
                this.CWeight = cfsReportItem.CWeight;
                this.Destination= cfsReportItem.Destination;
            }
        }

        //        public CfsReport CfsReportItem { get; set; }
        public PoUploadReportItem PoUploadReportItem { get; set; }
        public double UnitCbm => Pkgs > 0 ? Cbm / Pkgs : 0;

        public object Clone()
        {
            return this.MemberwiseClone() as ClpItem;
        }

        public List<ClpItem> Split(double maxCbmPerBox)
        {
            var result = new List<ClpItem>();

            if (Cbm <= maxCbmPerBox)
            {
                // No need to split, just return the original item
                result.Add(this);
            }
            else
            {
                double remainingCbm = Cbm;
                int remainingPkgs = Pkgs;

                while (remainingCbm > 0 && remainingPkgs>0)
                {
                    // Calculate the number of packages that fit within maxCbmPerBox
                    int pkgsForThisBox = Math.Min(remainingPkgs, (int)(maxCbmPerBox / UnitCbm));
                    double cbmForThisBox = pkgsForThisBox * UnitCbm;

                    // Create a new ContainerItem for this split
                    var newItem = new ClpItem
                    {
                        Pkgs = pkgsForThisBox,
                        Cbm = cbmForThisBox,
                        CWeight = (CWeight / Pkgs) * pkgsForThisBox, // Proportionate weight
                        Destination = this.Destination,
                        PoUploadReportItem=this.PoUploadReportItem,
                        CfsReportItem=this.cfsReportItem.Clone() as CfsReport
                        
                       
                    };
                    
                    newItem.Pkgs = pkgsForThisBox;
                    newItem.Cbm=cbmForThisBox;
                    newItem.CWeight = (CWeight / Pkgs) * pkgsForThisBox; // Proportionate weight
                    
                    newItem.cfsReportItem.Pkgs = newItem.Pkgs;
                    newItem.cfsReportItem.Cbm = newItem.Cbm;
                    result.Add(newItem);

                    // Update remaining CBM and packages
                    remainingCbm -= cbmForThisBox;
                    remainingPkgs -= pkgsForThisBox;
                }
            }

            return result;
        }
    }
}
