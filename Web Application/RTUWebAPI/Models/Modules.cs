using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{

    [Serializable]
    public class RTUInterfaceModuleDO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public Dictionary<UInt32, Parameter> moduleConfiguration { get; set; }
        public RTUChannelDO channel1 { get; set; }
        public RTUChannelDO channel2 { get; set; }
        public RTUChannelDO channel3 { get; set; }
        public RTUChannelDO channel4 { get; set; }
        public RTUChannelDO channel5 { get; set; }
        public RTUChannelDO channel6 { get; set; }
        public RTUChannelDO channel7 { get; set; }
        public RTUChannelDO channel8 { get; set; }

        public RTUInterfaceModuleDO()
        {
            this.id = "0";
            this.name = "Empty";
            this.img = "emptymodule.png";
            this.moduleConfiguration = new Dictionary<UInt32, Parameter>();
            this.channel1 = new RTUChannelDO();
            this.channel2 = new RTUChannelDO();
            this.channel3 = new RTUChannelDO();
            this.channel4 = new RTUChannelDO();
            this.channel5 = new RTUChannelDO();
            this.channel6 = new RTUChannelDO();
            this.channel7 = new RTUChannelDO();
            this.channel8 = new RTUChannelDO();
        }
    }

    [Serializable]
    public class RTUCPUModuleDO
    {
        public string name { get; set; }
        public string img { get; set; }
        public Dictionary<UInt32, Parameter> moduleConfiguration { get; set; }
        public RTUChannelDO channel1 { get; set; }
        public RTUChannelDO channel2 { get; set; }
        public RTUChannelDO channel3 { get; set; }
        public RTUChannelDO channel4 { get; set; }
        public RTUChannelDO channel5 { get; set; }
        public RTUChannelDO channel6 { get; set; }
        public RTUChannelDO channel7 { get; set; }
        public RTUChannelDO channel8 { get; set; }

        public RTUCPUModuleDO()
        {
            this.name = "CPU";
            this.img = "cpu.png";
            this.moduleConfiguration = new Dictionary<UInt32, Parameter>();
            this.channel1 = new RTUChannelDO { type = ChannelType.Physical };
            this.channel2 = new RTUChannelDO { type = ChannelType.Physical };
            this.channel3 = new RTUChannelDO { type = ChannelType.Physical };
            this.channel4 = new RTUChannelDO();
            this.channel5 = new RTUChannelDO();
            this.channel6 = new RTUChannelDO();
            this.channel7 = new RTUChannelDO();
            this.channel8 = new RTUChannelDO();
        }

    }
}
