using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
    //[Serializable]
    //public enum ConfigurationClass { DYNAMIC = 32, CONFIG = 64, CONSTANT = 96, SCRATCH = 128, COMMAND = 160, SYSTEM = 192 }
    [Serializable]
    public class AvailablePoints
    {
        public string Name { get; set; }
        public int maximumAllowed { get; set; }
        public Dictionary<UInt32, Parameter> pointConfiguration { get; set; }
    }
}
