using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
	public class RTUConnection
	{
		public string url { get; set; }
        public string securityMode { get; set; }
        public string securityPolicy { get; set; }
        public string userIdentity { get; set; }
        public string loginId { get; set; }
		public string loginPassword { get; set; }
		public bool returnPoints { get; set; }
		public string filename { get; set; }
        public string certificateFilename { get; set; }
	}
}
