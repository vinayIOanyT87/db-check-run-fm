using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.SiteOmatObjects
{
	using System.Data;
	using System.Xml.Serialization;

	using FMBusinessObjects.DataObjects;

	public struct LoginRespond
	{
		public Int32 rc;

		public String rc_desc;

		public String SessionID;

		public String Version;
	}

	public struct GetRespond
	{
		public Int32 rc;

		public String rc_desc;
	}

	public struct AuthorizeRequestRespond
	{
		public Int32 rc;

		public String rc_desc;

		public Int32 auth_result;

		public String auth_result_msg;

		public Int32 limit_type;

		public Double limit;

		public Double credit;

		public Int32 any_product;

		public Int32 fuel_type;

		public Int32 num_products;

		[XmlArrayItem("productid")]
		public Int32[] aProducts;

		public Int32 num_dry_prod;

		[XmlArrayItem("dryproductid")]
		public Int32[] aDryProducts;

		public Int32 driver_type_req;

		public Int32 drivers_type;

		public Int32 num_drivers;

		[XmlArrayItem("driverid")]
		public Int32[] aDrivers;

		public Int32 mean_type;

		public Int32 fleet_code;

		public String fleet_name;

		public Int32 dept_id;

		public Int32 plate;

		public Int32 ref_num;

		public Int32 pressure_level;

		public Int32 fleet_id;

		public Int32 mean_id;

		public string mean_name;

		public Int32 price_list_id;

		public Int32 prompt_odo;

		public Int32 prompt_ho;

		public Int32 prompt_plate;

		public Int32 use_pin_code;

		public Int32 pin_code;

		public string ext_bank_rc;

		public string ext_bank_desc;

		public double volume_limit;

		public Int32 route_prompt;
	}


	public struct AuthRequestRespond
	{
		public Int32 rc;

		public String rc_desc;

		public Int32 auth_result;

		public Int32 limit_type;

		public double limit;

		public Int32 ref_num;

		public String fleet_name;

		public Int32 fleet_code;

		public String plate;

		public int use_pin_code;

		public string pin_code;

		public Int32 any_product;

		public Int32 product_list_type;

		public Int32 num_products;

		public string[] aProducts;
	}
}
