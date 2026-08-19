using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.SiteOmatObjects
{
	using System.Data;
	using System.Security.Policy;
	using System.Xml.Serialization;

	using FMBusinessObjects.DataObjects;

	public struct SOHOSendNewUpdatedTransactionsRespond
	{
		public Int32 rc;

		public string rc_desc;

	}

	public struct soTransaction
	{
		public string car_plate;

		public Int32 copy_num;

		public long counter2;

		public long counter3;

		public double discount_price;

		public double quantity;

		public string driver_code;

		public long engine_hours;

		public string expiration;

		public string external_reference_code;

		public string externtal_pre_receipt_code;

		public string fleet;

		public string fleet_code;

		public string hose;

		public string nozzle;

		public long odometer;

		public string odometer_code;

		public string pay_mean_company_code;

		public string pay_mean_identifier;

		public string presentation;

		public double ppv;

		public long product_id;

		public string product_desc;

		public long product_code;

		public long pump;

		public string pymean_code;

		public string receipt_code;

		public string receipt_date;

		public long receipt_id;

		public string receipt_time;

		public double received_amount;

		public long reference_receipt_id;

		public string site_code;

		//public string string;

		public string tank_external_code;

		public long timer2;

		public long timer3;

		public double sale;

		public string track1;
		public string track2;
		public string track3;
		public string track4;
		public string track5;
		public string track6;

		public string timestamp;

		public string date;

		public long transaction_id;

		public string time;

		public string trnsts_code;

		public string type;

		public string vehicle_plate;

		public long tank_number;

		public string tank_description;

		public string proxy_card_number;

		public double last_running_volume;

		public string external_tran_id;

		public string tax_invoice_no;

		public double base_price;

		public int cash_customer_id;

		public int coupon_id;

		public double density;

		public int driver_mean_id;

		public string driver_mean_plate;

		public string driver_mean_tag;

		public string driver_name;

		public string entry_type;

		public string ext_auth_number;

		public string ext_tran_id;

		public int fleet_id;

		public int hose_number;

		public int id;

		public int mean_id;

		public string mean_name;

		public int nozzle_id;

		public string phone_number;

		public string plate;

		public int price_list_id;

		public string product_name;

		public int proxy_id;

		public int pump_id;

		public int shift_id;

		public string start_flow;

		public string tag;

		public string tank_name;

		public double temperature;

		public double total_price;

		public double total_price_after_discount;

		public double base_product_percent;

		public int reject_status;

		public string reject_text;

		public int route_id;

		public string route_number;

		public int stn_id;

		public double totalizer_vol;


	}

	public struct SOGetNewUpdatedFleetsRespond
	{
		public Int32 rc;

		public string rc_desc;

		public int num_fleets;

		public string qids;

		public soFleet[] a_soFleet;

	}

	public struct soFleet
	{
		public int id;

		public string name;

		public int status;

		public int code;

		public int default_rule;

		public string address;

		public string phone;

		public string fax;

		public string email;

		public string contact;

		public int acctyp;

		public double available_amount;

		public double min_allowed;

		public int use_pin_code;

		public int auth_pin_from;

		public int nr_pin_retries;

		public int block_if_pin_retries_fail;

		public int opos_prompt_for_plate;

		public int opos_prompt_for_odometer;

		public int do_odo_reasonability_check;

		public int max_odo_delta_allowed;

		public int nr_odo_retries;

		public int price_list_id;

		public int use_rule_limit;

		public int max_rules;

		public int max_group_rules;

		public string contact2;

		public string contact3;

		public string city;

		public string state;

		public string zip;

		public string sales_person;

		public int eft_id;

		public double wex_renewal_fee;

		public double wex_billing_fee_56;

		public double on_line_fee_68;

		public double line_of_credit;

		public int opos_prompt_for_engine_hours;

		public string address2;

		public string user_data1;

		public string user_data2;

		public string user_data3;

		public string user_data4;

		public string user_data5;

		public int prompt_always_for_viu;

		public int do_eh_reasonability_check;

		public int max_eh_delta_allowed;

		public int nr_eh_retries;

		public int reject_if_eh_check_fails;

		public string company_name;

		public int reject_if_odm_check_fails;

		public double single_fuel;

	}

	public struct SOGetNewUpdatedDeptsRespond
	{
		public Int32 rc;

		public string rc_desc;

		public int num_dept;

		public string qids;

		public soDept[] a_soDept;
	}

	public struct soDept
	{
		public int id;

		public string name;

		public int fleet_id;

		public int status;

		public int code;

		public int default_rule;

		public string address;

		public string phone;

		public string fax;

		public string email;

		public string contact;

		public int use_pin_code;

		public int auth_pin_from;

		public int nr_pin_retries;

		public int block_if_pin_retries_fail;

		public int opos_prompt_for_plate;

		public int opos_prompt_for_odometer;

		public int do_odo_reasonability_check;

		public int max_odo_delta_allowed;

		public int nr_odo_retries;

		public int price_list_id;

		public int black_white_type;

		public int opos_prompt_for_engine_hours ;

		public string address2;

		public string city;

		public string state;

		public string zip;

		public string user_data1;

		public string user_data2;

		public string user_data3;

		private string user_data4;

		public string user_data5;

		public int prompt_always_for_viu;

		public int do_eh_reasonability_check;

		public int max_eh_delta_allowed;

		public int nr_eh_retries;

		public int reject_if_eh_check_fails;

		public int reject_if_odm_check_fails;

	}


	public struct SOGetNewUpdatedMeansRespond
	{
		public Int32 rc;

		public string rc_desc;

		public int num_of_means;

		public string qids;

		public soMean[] a_soMean;

	}

	public struct soMean
	{
		public long id;

		public string address;

		public double capacity;

		public double consumption;

		public double consumption2;

		public int cust_id;

		public int dept_id;

		public int employee_type;

		public double available_amount;

		public int fleet_id;

		public int hardware_type;

		public int auttyp;

		public int model_id;

		public string name;

		public double odometer;

		public string plate;

		public int pump;

		public int rule;

		public int status;

		[XmlElement("string")]
		public string string1;

		public string string2;

		public string string3;

		public string string4;

		public string string5;

		public int type;

		public int year;

		public string pin_code;

		public int auth_pin_from;

		public int nr_pin_retries;

		public int delta;

		public int nr_odo_retries;

		public int use_pin_code;

		public int block;

public int opos_prompt_for_plate;

		public int opos_plate_check_type;

		public int nr_plate_retries;

		public int block_if_plate_retries_fail;

		public int opos_prompt_for_odometer;

		public int reasonability;

		public int driver_required;

		public int price_list_id;

		public int opos_prompt_for_engine_hours ;

		public string update_timestamp;

		public string address2;

		public string city;

		public string state;

		public string zip;

		public string phone;

		public string user_data1;

		public string user_data2;

		public string user_data3;

		public string user_data4;

		public string user_data5;

		public int allow_id_replacement;

		public int num_of_strings;

		public int is_burned;

		public double start_odometer;

		public string chassis_number;

		public int prompt_always_for_viu ;

		public int disable_viu_two_stage;

		public int do_eh_reasonability_check;

		public int max_eh_delta_allowed;

		public int nr_eh_retries;

		public int reject_if_eh_check_fails;

		public int nr_2stage_elements;
	}

	public struct SOResponse
	{
		public int rc;

		public string rc_desc;
	}

	public struct soEventLog
	{
		public int id;

		public int errcls_code;

		public int error_code;

		public string error_timestamp ;

		public string error_date;

		public string error_time;

		public int object_id;

		public int object_type;

		public string device_name;

		public string field1;

		public string field2;

		public string field3;

		public string field4;

		public string field5;

		public string field6;

		public string field7;

		public string field8;

		public int fleet_id;

		public int status;

		public string end_timestamp;

	}

	public struct SOHOClockSynchRespond
	{
		public int rc;

		public string rc_desc;

		public string GMTDateTimeString;
	}

}
