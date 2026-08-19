using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects.Message
{
    public class CommandStatusMsg : EdgeData
    {
        public byte CmdStatus { get; set; }
        public uint CmdSchedule { get; set; }


        public override void Load(MemoryStream memoryStream)
        {
            base.Load(memoryStream);

            byte[] cmdStatus = new byte[1];
            byte[] cmdSchedule = new byte[4];

            memoryStream.Read(cmdStatus, 0, 1);
            memoryStream.Read(cmdSchedule, 0, 4);

            this.CmdStatus = cmdStatus[0];
            this.CmdSchedule = BitConverter.ToUInt32(cmdSchedule.Reverse().ToArray(), 0); 
        }
    }
}
