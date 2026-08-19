using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RTUWebAPI.Models.AvailableChannel;

namespace RTUWebAPI.Models
{

    [Serializable]
    public enum ConfigurationClass { DYNAMIC = 32, CONFIG = 64, CONSTANT = 96, SCRATCH = 128, COMMAND = 160, SYSTEM = 192 }

    [Serializable]
    public class AvailableModules
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public Dictionary<UInt32, Parameter> moduleConfiguration  { get; set; }
        public ConfigurableChannelDO Channel1 { get; set; }
        public ConfigurableChannelDO Channel2 { get; set; }
        public ConfigurableChannelDO Channel3 { get; set; }
        public ConfigurableChannelDO Channel4 { get; set; }
        public ConfigurableChannelDO Channel5 { get; set; }
        public ConfigurableChannelDO Channel6 { get; set; }
        public ConfigurableChannelDO Channel7 { get; set; }
        public ConfigurableChannelDO Channel8 { get; set; }

        public AvailableModules()
        {
            this.moduleConfiguration = new Dictionary<UInt32,Parameter>();
            this.Channel1 = new ConfigurableChannelDO();
            this.Channel2 = new ConfigurableChannelDO();
            this.Channel3 = new ConfigurableChannelDO();
            this.Channel4 = new ConfigurableChannelDO();
            this.Channel5 = new ConfigurableChannelDO();
            this.Channel6 = new ConfigurableChannelDO();
            this.Channel7 = new ConfigurableChannelDO();
            this.Channel8 = new ConfigurableChannelDO();
        }

    }
}
