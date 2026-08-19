using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.Constants
{
    public enum ConsortiumTypes
    {
        [Description("Non-Consortium")]
        NonConsortium = 0,
        [Description("Consortium")]
        Consortium = 1,
        [Description("Itinerant")]
        Itinerant = 2
    }
}
