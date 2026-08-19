using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class PointGroupFilterRules
	{
		#region Constructors and Destructors

		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroupFilterRules()
		{
			this.Init();
		}
		#endregion

		#region properties
		[DataMember]
		public string type { get; set; }
		[DataMember]
		public List<string> point_type { get; set; }
		[DataMember]
		public List<string> point_category { get; set; }
		[DataMember]
		public string point_name { get; set; }
		[DataMember]
		public List<string> product_group { get; set; }
		[DataMember]
		public string product_name { get; set; }
		[DataMember]
		public string description { get; set; } 
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.type = string.Empty;
			this.point_name = string.Empty;
			this.product_name = string.Empty;
			this.description = string.Empty;
			this.point_type = new List<string>();
			this.point_category = new List<string>();
			this.product_group = new List<string>();
		}

		#endregion
	}
}
