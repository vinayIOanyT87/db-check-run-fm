using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{

    public class AvailableChannel
    {
        [Serializable]
        public class ConfigurableChannelDO
        {
            public ChannelType type { get; set; }
            public List<string> channelProtocols { get; set; }
            public int top { get; set; }
            public int left { get; set; }
            public int width { get; set; }
            public int height { get; set; }

            public ConfigurableChannelDO()
            {
                this.type = ChannelType.Virtual;
                this.channelProtocols = new List<string>();
                this.top = 0;
                this.left = 0;
                this.width = 0;
                this.height = 0;
            }

        }
    }
}
