using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.Constants
{
    public class PIDXConstants
    {
        public const string CREDIT_AUTHORIZATION      = "CA";
        public const string LOAD_AUTHORIZATION			= "LA";
        public const string CHECK_ORDER_AUTHORIZATION = "CO";
        public const string BOL_BB                    = "BB";
        public const string BOL_BL							= "BL";
        public const string COMPLETED_BOL             = "CB";
        public const string RE_TRANSMIT               = "RT";
        public const string FINISHED_PROCESSING       = "FP";

        public const int CARRIER_ID_LENGTH   = 8;
        public const int CONSIGNEE_ID_LENGTH = 14;
        public const int PRODUCT_CODE_LENGTH = 6;
        public const int MAX_VERSION_1_BOL_PRODUCTS = 8;
		public const int MAX_VERSION_4_BOL_PRODUCTS = 99;

		public const string AUTH_GRANT	= "AUTH";
		public const string AUTH_DENY	= "DENY";

        public const string ERR_MSG_001 = "SPLC Code must be populated";
        public const string ERR_MSG_002 = "Terminal ID must be populated";
        public const string ERR_MSG_003 = "Seller ID must be populated";
        public const string ERR_MSG_004 = "Truck Number must be populated";
        public const string ERR_MSG_005 = "Consignee ID must be populated";
        public const string ERR_MSG_006 = "Final Shipper ID must be populated";
        public const string ERR_MSG_007 = "Carrier ID must be populated";
        public const string ERR_MSG_008 = "Consignee ID must be populated";
        public const string ERR_MSG_009 = "Order Number must be populated";
        public const string ERR_MSG_010 = "BOL Number must be populated";
        public const string ERR_MSG_011 = "Ship Day must be populated";
        public const string ERR_MSG_012 = "Blend ID must be populated";
        public const string ERR_MSG_013 = "Gross quantity must be populated";
        public const string ERR_MSG_014 = "Net Temperature quantity must be populated";
        public const string ERR_MSG_015 = "Net Temperature Flag must be populated";
        public const string ERR_MSG_016 = "Product Code must be populated";
        public const string ERR_MSG_017 = "Authorization Number must be populated";
        public const string ERR_MSG_018 = "Credit Indicator must be populated";
        public const string ERR_MSG_019 = "Re-transmit Last must be populated or have 2 charachers";
        public const string ERR_MSG_020 = "Finished Processing must be populated or have 2 charachers";
        public const string ERR_MSG_021 = "Authorization header is invalid due to length";
        public const string ERR_MSG_022 = "Authorization response invalid";
        public const string ERR_MSG_023 = "Invalid product lift amount";
        public const string ERR_MSG_024 = "Invalid deny record";
        public const string ERR_MSG_025 = "Transaction could not be decoded";
        public const string ERR_MSG_026 = "No products associated with the BOL";
        public const string ERR_MSG_027 = "Host Name must be populated";        
        public const string ERR_MSG_028 = "Port must be populated";
        public const string ERR_MSG_029 = "PIDX Record type not supported";
		public const string ERR_MSG_030 = "Host returned an empty response";
		public const string ERR_MSG_031 = "Shipped Date must be populated";
		public const string ERR_MSG_032 = "Validation Fault";
		public const string ERR_MSG_033 = "Invalid Stream";
		public const string ERR_MSG_034 = "Invalid Total PIDX Products in Authorization";
		public const string ERR_MSG_035 = "Invalid Credit Sign";
		public const string ERR_MSG_036 = "Invalid Temperature Measurement Type";
		public const string ERR_MSG_037 = "Invalid Measurement Type";
	 }
}
