using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
	public class Point
	{
		public string name { get; set; }
		public Dictionary<UInt32, Parameter> pointConfiguration { get; set; }

		public Point(string name)
		{
			this.name = name;
			this.pointConfiguration = new Dictionary<UInt32, Parameter>();
		}
	}
}
