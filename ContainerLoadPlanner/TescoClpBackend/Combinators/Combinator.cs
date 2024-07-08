using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TescoClpBackend.Combinators
{
    public enum COMBINATOR_TYPE
    {
        REGULAR, CUTOFF
    }
    public class FortyHICombinator : ICombinator
    {
        public Combination GetCombination(double cbm, bool allowLeftover)
        {
            var quotient = cbm / ContainerConstants.FORTY_HI_DEFAULT_CAPACITY;
            var remainder = cbm % ContainerConstants.FORTY_HI_DEFAULT_CAPACITY;
            HashSet<Combination> combinations = new HashSet<Combination>();
            Combination c1 = new Combination()
            {
                FortyHICount = (int)quotient,
                FortyHITolerance = 0,
                Cbm = (int)quotient * ContainerConstants.FORTY_HI_DEFAULT_CAPACITY,



            };

            combinations.Add(c1);
            Combination c2 = new Combination()
            {
                FortyHICount = (int)quotient,
                FortyHITolerance = ContainerConstants.FORTY_HI_TOLERANCE,
                Cbm = (int)quotient * (ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE),



            };
            combinations.Add(c2);

            Combination c3 = new Combination()
            {
                FortyHICount = (int)(quotient+1),
                FortyHITolerance = ContainerConstants.FORTY_HI_TOLERANCE,
                Cbm = (int)(quotient+1) * (ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE),



            };
            Combination c4 = new Combination()
            {
                FortyHICount = (int)(quotient + 1),
                FortyHITolerance = 0,
                Cbm = (int)(quotient + 1) * ContainerConstants.FORTY_HI_DEFAULT_CAPACITY,



            };
            combinations.Add(c2);
            combinations.Add(c3);
            combinations.Add(c4);

            if (!allowLeftover)
            {
                return combinations
                    .Where(a => a.Cbm > cbm)
                    .Aggregate((x, y) =>
                    Math.Abs(x.Cbm - cbm) < Math.Abs(y.Cbm - cbm) ? x : y);
            }
            return combinations.Aggregate((x, y) => Math.Abs(x.Cbm - cbm) < Math.Abs(y.Cbm - cbm) ? x : y);


        }
    }
    public class MixContainerCombinator : ICombinator
    {
        public Combination GetCombination(double cbm, bool allowLeftover)
        {
            int quotient = (int)(cbm / ContainerConstants.FORTY_HI_DEFAULT_CAPACITY);
            HashSet<Combination> combinations = new HashSet<Combination>();
            for (int i = quotient; i >= quotient / 2; i--)
            {

                Combination c1 = new Combination()
                {
                    FortyHICount = i,
                    FortySTDCount = quotient - i,
                    Cbm =
                    ContainerConstants.FORTY_HI_DEFAULT_CAPACITY * i +
                    ContainerConstants.FORTY_STD_DEFAULT_CAPACITY * (quotient - i),

                };
                var c2 = new Combination()
                {
                    FortyHICount = i,
                    FortySTDCount = quotient - i,
                    FortyHITolerance = ContainerConstants.FORTY_HI_TOLERANCE,

                    Cbm =
                    (ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE) * i +
                    ContainerConstants.FORTY_STD_DEFAULT_CAPACITY * (quotient - i),
                };
                var c3 = new Combination()
                {
                    FortyHICount = i,
                    FortySTDCount = quotient - i,

                    FortySTDTolerance = ContainerConstants.FORTY_STD_TOLERANCE,

                    Cbm =
                    (ContainerConstants.FORTY_HI_DEFAULT_CAPACITY) * i +
                    ContainerConstants.FORTY_STD_DEFAULT_CAPACITY + ContainerConstants.FORTY_STD_TOLERANCE * (quotient - i),
                };
                var c4 = new Combination()
                {
                    FortyHICount = i,
                    FortySTDCount = quotient - i,
                    FortyHITolerance = ContainerConstants.FORTY_HI_TOLERANCE,
                    FortySTDTolerance = ContainerConstants.FORTY_STD_TOLERANCE,

                    Cbm =
                    (ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE) * i +
                    ContainerConstants.FORTY_STD_DEFAULT_CAPACITY + ContainerConstants.FORTY_STD_TOLERANCE * (quotient - i),
                };
                combinations.Add(c1);
                combinations.Add(c2);
                combinations.Add(c3);
                combinations.Add(c4);
            }
            if (!allowLeftover)
            {

                return combinations
                    .Where(a => a.Cbm > cbm)
                    .Aggregate((x, y) => x.Cbm <= y.Cbm ? x : y);
            }
            return combinations
                   //.Where(a => a.Cbm > cbm)
                   .Aggregate((x, y) => Math.Abs(x.Cbm - cbm) <= Math.Abs(y.Cbm - cbm) ? x : y);

        }
    }
}
