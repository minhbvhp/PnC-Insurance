using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.Model
{
    public partial class PropertyPolicy
    {
        public long TotalNetPremium 
        {
            get
            {
                return this.Arpremium + this.FnEpremium;
            }
        }
    }
}
