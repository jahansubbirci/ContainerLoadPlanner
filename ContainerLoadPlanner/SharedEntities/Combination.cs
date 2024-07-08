using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities
{
    public class Combination
    {
        public int FortyHICount { get; set; }
        public int FortySTDCount { get; set; }
        public double FortyHITolerance { get; set; }
        public double FortySTDTolerance { get; set; }

        public int TwentySTDCount { get; set; }
        public double TwentySTDTolerance { get; set; }
        public double Cbm { get; set; }
        public double Cost { get; set; }
    }
}
