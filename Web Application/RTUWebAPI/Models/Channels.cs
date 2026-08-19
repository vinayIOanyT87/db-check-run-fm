using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
    [Serializable]
    public class RTUChannelDO
    {
        public string protocol { get; set; }
        public ChannelType type { get; set; }
        public Dictionary<UInt32,Parameter> channelConfiguration { get; set; }
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public RTUChannelDO()
        {
            this.protocol = "unknown";
            this.type = ChannelType.Virtual;
            this.channelConfiguration = new Dictionary<UInt32, Parameter>();
            this.top = 0;
            this.left = 0;
            this.width = 0;
            this.height = 0;
        }

    }
}
