using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities
{
    public interface ICombinator
    {
        /// <summary>
        /// Get combination of Container Based pon cbm
        /// </summary>
        /// <param name="cbm"></param>
        /// <param name="allowLeftover">if true client allows few  cartons to be in the floor</param>
        /// <returns></returns>
        Combination GetCombination(double cbm,bool allowLeftover);
    }
}
