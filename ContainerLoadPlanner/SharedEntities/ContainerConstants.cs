using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities
{
    public static class ContainerConstants
    {

        public static double FORTY_HI_DEFAULT_CAPACITY { get; set; } = 65;
        public static double FORTY_HI_MIN_ACCEPTABLE_VOLUME { get; set; } = 64;
        public static double FORTY_STD_DEFAULT_CAPACITY { get; set; } = 55;
        public static double FORTY_STD_MIN_ACCEPTABLE_VOLUME { get; set; } = 54;
        public static double TWENTY_STD_DEFAULT_CAPACITY { get; set; } = 25;
        public static double TWENTY_STD_MIN_ACCEPTABLE_VOLUME { get; set; } = 24;
        public static double FORTY_HI_TOLERANCE { get; set; } = 2;
        public static double FORTY_STD_TOLERANCE { get; set; } = 2;
        public static double TWENTY_STD_TOLERANCE { get; set; }= 2;
    }

}
