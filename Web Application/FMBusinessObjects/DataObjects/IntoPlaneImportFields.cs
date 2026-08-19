


namespace FMBusinessObjects.DataObjects
{
    using Constants;
    using FMBusinessObjects.Exceptions;
    using Microsoft.VisualBasic.FileIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Varec.CommonComponents.EngineeringUnitsLibrary;


    public class IntoPlaneImportFields
    {
        #region Static Members
        public enum TransTypes
        {
            Defuel_Primary = 3,
            Defuel_Secondary = 4,
            Issue_Primary = 5,
            Issue_Secondary = 6,
            Load_Rack = 7,
            Notation_Rotation = 12,
        }
        #endregion

        #region Private Attributes
        private readonly SortedList<string, int> headers;
        private readonly List<string> requiredFields = new List<string>
                {
                     IntoPlaneImportFieldNames.TransactionNumber,
                     IntoPlaneImportFieldNames.TransactionAlias,
                     IntoPlaneImportFieldNames.TransactionDate,
                     IntoPlaneImportFieldNames.Temperature,
                     IntoPlaneImportFieldNames.Gravity,
                     IntoPlaneImportFieldNames.VCF,
                     IntoPlaneImportFieldNames.Product,
                     IntoPlaneImportFieldNames.Manager,
                     IntoPlaneImportFieldNames.Owner,
                     IntoPlaneImportFieldNames.Vendor,
                     IntoPlaneImportFieldNames.Customer,
                     IntoPlaneImportFieldNames.Notes,
                     IntoPlaneImportFieldNames.TransactionSubtypeCode,
                     IntoPlaneImportFieldNames.TicketNumber,
                     IntoPlaneImportFieldNames.CartRegistrationID,
                     IntoPlaneImportFieldNames.MeterStart,
                     IntoPlaneImportFieldNames.MeterStop,
                     IntoPlaneImportFieldNames.GrossVolume,
                     IntoPlaneImportFieldNames.DestinationRegistrationID,
                     IntoPlaneImportFieldNames.SerialNumber_FlightNumber,
                     IntoPlaneImportFieldNames.TransactionSubtypeCode2,
                     IntoPlaneImportFieldNames.TransactionSubtypeCode3,
                     IntoPlaneImportFieldNames.UserData1_DestinationID,
                     IntoPlaneImportFieldNames.UserData2_GateID,
                     IntoPlaneImportFieldNames.UserData3_Operator,
                     IntoPlaneImportFieldNames.UserData4,
                     IntoPlaneImportFieldNames.UserData5,
                     IntoPlaneImportFieldNames.UserData6,
                     IntoPlaneImportFieldNames.UserData7,
                     IntoPlaneImportFieldNames.UserData8,
                     IntoPlaneImportFieldNames.UserData9,
                     IntoPlaneImportFieldNames.UserData10,
                     IntoPlaneImportFieldNames.UserData11,
                     IntoPlaneImportFieldNames.UserData12,
                     IntoPlaneImportFieldNames.UserData13,
                     IntoPlaneImportFieldNames.UserData14,
                     IntoPlaneImportFieldNames.UserData15,
                     IntoPlaneImportFieldNames.UserData16,
                     IntoPlaneImportFieldNames.UserData17,
                     IntoPlaneImportFieldNames.UserData18,
                     IntoPlaneImportFieldNames.UserData19,
                     IntoPlaneImportFieldNames.UserData20,
                     IntoPlaneImportFieldNames.UserData21,
                     IntoPlaneImportFieldNames.UserData22_OriginID,
                     IntoPlaneImportFieldNames.UserData23,
                     IntoPlaneImportFieldNames.UserData24,
                     IntoPlaneImportFieldNames.NetVolume,
                     IntoPlaneImportFieldNames.MeterFactor,
                     IntoPlaneImportFieldNames.FuelCp,
                     IntoPlaneImportFieldNames.NetVolumeIndicator
                };
        private readonly Dictionary<string, Type> fieldTypes= new Dictionary<string, Type>() {

                {IntoPlaneImportFieldNames.TransactionNumber, typeof(short)},
                {IntoPlaneImportFieldNames.TransactionAlias, typeof(string)},
                {IntoPlaneImportFieldNames.TransactionDate, typeof(string)},
                {IntoPlaneImportFieldNames.Temperature, typeof(double)},
                {IntoPlaneImportFieldNames.Gravity, typeof(double)},
                {IntoPlaneImportFieldNames.VCF, typeof(double)},
                {IntoPlaneImportFieldNames.Product, typeof(string)},
                {IntoPlaneImportFieldNames.Manager, typeof(string)},
                {IntoPlaneImportFieldNames.Owner, typeof(string)},
                {IntoPlaneImportFieldNames.Vendor, typeof(string)},
                {IntoPlaneImportFieldNames.Customer, typeof(string)},
                {IntoPlaneImportFieldNames.Notes, typeof(string)},
                {IntoPlaneImportFieldNames.TransactionSubtypeCode, typeof(string)},
                {IntoPlaneImportFieldNames.TicketNumber, typeof(string)},
                {IntoPlaneImportFieldNames.CartRegistrationID, typeof(string)},
                {IntoPlaneImportFieldNames.MeterStart, typeof(double)},
                {IntoPlaneImportFieldNames.MeterStop, typeof(double)},
                {IntoPlaneImportFieldNames.GrossVolume, typeof(double)},
                {IntoPlaneImportFieldNames.DestinationRegistrationID, typeof(string)},
                {IntoPlaneImportFieldNames.SerialNumber_FlightNumber, typeof(string)},
                {IntoPlaneImportFieldNames.TransactionSubtypeCode2, typeof(string)},
                {IntoPlaneImportFieldNames.TransactionSubtypeCode3, typeof(string)},
                {IntoPlaneImportFieldNames.UserData1_DestinationID, typeof(string)},
                {IntoPlaneImportFieldNames.UserData2_GateID, typeof(string)},
                {IntoPlaneImportFieldNames.UserData3_Operator, typeof(string)},
                {IntoPlaneImportFieldNames.UserData4, typeof(string)},
                {IntoPlaneImportFieldNames.UserData5, typeof(string)},
                {IntoPlaneImportFieldNames.UserData6, typeof(string)},
                {IntoPlaneImportFieldNames.UserData7, typeof(string)},
                {IntoPlaneImportFieldNames.UserData8, typeof(string)},
                {IntoPlaneImportFieldNames.UserData9, typeof(string)},
                {IntoPlaneImportFieldNames.UserData10, typeof(string)},
                {IntoPlaneImportFieldNames.UserData11, typeof(string)},
                {IntoPlaneImportFieldNames.UserData12, typeof(string)},
                {IntoPlaneImportFieldNames.UserData13, typeof(string)},
                {IntoPlaneImportFieldNames.UserData14, typeof(string)},
                {IntoPlaneImportFieldNames.UserData15, typeof(string)},
                {IntoPlaneImportFieldNames.UserData16, typeof(string)},
                {IntoPlaneImportFieldNames.UserData17, typeof(string)},
                {IntoPlaneImportFieldNames.UserData18, typeof(string)},
                {IntoPlaneImportFieldNames.UserData19, typeof(string)},
                {IntoPlaneImportFieldNames.UserData20, typeof(string)},
                {IntoPlaneImportFieldNames.UserData21, typeof(string)},
                {IntoPlaneImportFieldNames.UserData22_OriginID, typeof(string)},
                {IntoPlaneImportFieldNames.UserData23, typeof(string)},
                {IntoPlaneImportFieldNames.UserData24, typeof(string)},
                {IntoPlaneImportFieldNames.NetVolume, typeof(double)},
                {IntoPlaneImportFieldNames.MeterFactor, typeof(double)},
                {IntoPlaneImportFieldNames.FuelCp, typeof(double)},
                {IntoPlaneImportFieldNames.NetVolumeIndicator, typeof(short)},
                {IntoPlaneImportFieldNames.DeviceID, typeof(string)},
                {IntoPlaneImportFieldNames.OriginID, typeof(string)},
                {IntoPlaneImportFieldNames.OriginTime, typeof(DateTime)},
                {IntoPlaneImportFieldNames.OperatorName, typeof(string)},
                {IntoPlaneImportFieldNames.AckTime, typeof(DateTime)},
                {IntoPlaneImportFieldNames.LeaveTime, typeof(DateTime)},
                {IntoPlaneImportFieldNames.ArrivalFlightID, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTime, typeof(DateTime)},
                {IntoPlaneImportFieldNames.DensityUnits, typeof(int)},
                {IntoPlaneImportFieldNames.DensityTemp, typeof(double)},
                {IntoPlaneImportFieldNames.MassUnits, typeof(int)},
                {IntoPlaneImportFieldNames.TemperatureUnits, typeof(int)},
                {IntoPlaneImportFieldNames.VolumeUnits, typeof(int)},
                {IntoPlaneImportFieldNames.FuelDensity, typeof(double)},
                {IntoPlaneImportFieldNames.ObsDensity, typeof(double)},
                {IntoPlaneImportFieldNames.ObsDensityInd, typeof(short)},
                {IntoPlaneImportFieldNames.StdDensity, typeof(double)},
                {IntoPlaneImportFieldNames.StdDensityInd, typeof(short)},
                {IntoPlaneImportFieldNames.HydrantPressure, typeof(double)},
                {IntoPlaneImportFieldNames.NozzlePressure, typeof(double)},
                {IntoPlaneImportFieldNames.DiffPressure, typeof(double)},
                {IntoPlaneImportFieldNames.DualFueling, typeof(short)},
                {IntoPlaneImportFieldNames.EngRunTime, typeof(double)},
                {IntoPlaneImportFieldNames.ETA, typeof(DateTime)},
                {IntoPlaneImportFieldNames.ETD, typeof(DateTime)},
                {IntoPlaneImportFieldNames.SFT, typeof(DateTime)},
                {IntoPlaneImportFieldNames.STA, typeof(DateTime)},
                {IntoPlaneImportFieldNames.STD, typeof(DateTime)},
                {IntoPlaneImportFieldNames.FlowRate, typeof(double)},
                {IntoPlaneImportFieldNames.FreezePoint, typeof(double)},
                {IntoPlaneImportFieldNames.FromLDU, typeof(short)},
                {IntoPlaneImportFieldNames.FSRType, typeof(string)},
                {IntoPlaneImportFieldNames.PrimaryFSR, typeof(short)},
                {IntoPlaneImportFieldNames.FuelingStartTime, typeof(DateTime)},
                {IntoPlaneImportFieldNames.FuelingStopTime, typeof(DateTime)},
                {IntoPlaneImportFieldNames.PartialFill, typeof(short)},
                {IntoPlaneImportFieldNames.FuelTemp, typeof(double)},
                {IntoPlaneImportFieldNames.TempQualityStatus, typeof(string)},
                {IntoPlaneImportFieldNames.GseCategoryID, typeof(string)},
                {IntoPlaneImportFieldNames.GseID, typeof(string)},
                {IntoPlaneImportFieldNames.IPRemarks, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData1, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData2, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData3, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData4, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData5, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData6, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData7, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData8, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData9, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData10, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData11, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData12, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData13, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData14, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData15, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData16, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData17, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData18, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData19, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData20, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData21, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData22, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData23, typeof(string)},
                {IntoPlaneImportFieldNames.IPUserData24, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName1, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName2, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName3, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName4, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName5, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName6, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName7, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName8, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName9, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankName10, typeof(string)},
                {IntoPlaneImportFieldNames.DesiredTankValue1, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue2, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue3, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue4, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue5, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue6, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue7, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue8, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue9, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankValue10, typeof(double)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd1, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd2, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd3, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd4, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd5, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd6, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd7, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd8, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd9, typeof(short)},
                {IntoPlaneImportFieldNames.DesiredTankShutoffInd10, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankName1, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName2, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName3, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName4, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName5, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName6, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName7, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName8, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName9, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankName10, typeof(string)},
                {IntoPlaneImportFieldNames.ArrivalTankValue1, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue2, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue3, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue4, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue5, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue6, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue7, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue8, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue9, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankValue10, typeof(double)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd1, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd2, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd3, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd4, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd5, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd6, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd7, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd8, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd9, typeof(short)},
                {IntoPlaneImportFieldNames.ArrivalTankShutoffInd10, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankName1, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName2, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName3, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName4, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName5, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName6, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName7, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName8, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName9, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankName10, typeof(string)},
                {IntoPlaneImportFieldNames.FinalTankValue1, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue2, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue3, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue4, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue5, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue6, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue7, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue8, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue9, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankValue10, typeof(double)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd1, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd2, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd3, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd4, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd5, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd6, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd7, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd8, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd9, typeof(short)},
                {IntoPlaneImportFieldNames.FinalTankShutoffInd10, typeof(short)},
                {IntoPlaneImportFieldNames.MeterStartReadFromDCU, typeof(short)},
                {IntoPlaneImportFieldNames.MeterStopReadFromDCU, typeof(short)}
            };
        private string[] fields;
        private SiteClass site;
        private  readonly DateTime DEFAULT_DATE_TIME = new DateTime(1899, 12, 31);
        #endregion

        #region Private Functions
        private void InitializeFieldHeadersAndTypes()
        {
           
        }
        private object GetRequiredFieldValue(string columnName)
        {
            object value = GetValue(columnName);
            if( value == null)
            {
                throw new IntoPlaneImportGeneralException("Invalid value for column "+ columnName);
            }

            Type type = this.fieldTypes[columnName];
            if (type.FullName == "System.String" && string.IsNullOrWhiteSpace((string)value))
            {
                throw new IntoPlaneImportGeneralException(columnName + " has no value");
            }
            else if (type.FullName == "System.DateTime" && DEFAULT_DATE_TIME.Equals((DateTime)value))
            {
                throw new IntoPlaneImportGeneralException(columnName + " has no value");
            }
            return value;
        }
        private object GetValue(string columnName)
        {
            object oValue = null;

            if (!this.headers.ContainsKey(columnName) || !this.fieldTypes.ContainsKey(columnName))
            {
                return oValue;
            }

            //If this column is not included in the record, return null
            if(this.headers.Where(x => x.Key == columnName ).First().Value + 1 > this.fields.Length)
            {
                return oValue;
            }

            Type type = this.fieldTypes[columnName];
            /*
             *  System.String
                System.Int16
                System.Double
                System.DateTime
                System.Int32
             */
            string field = this.fields[this.headers[columnName]];
            double dValue = 0.0;
            DateTime dt = DEFAULT_DATE_TIME;
            short sValue = 0;
            int iValue = 0;

            switch (type.FullName)
            {
                case "System.String":
                    oValue = (field == null) ? "" : field;
                    break;
                case "System.Int16":
                    if (short.TryParse(field, out sValue))
                    {
                        oValue = sValue;
                    }
                    break;
                case "System.Double":
                    if (double.TryParse(field, out dValue))
                    {
                        oValue = dValue;
                    }
                    break;
                case "System.DateTime":
                    if (DateTime.TryParse(field, out dt))
                    {
                        oValue = dt;
                    }
                    break;
                case "System.Int32":
                    if (int.TryParse(field, out iValue))
                    {
                        oValue = iValue;
                    }
                    break;
                default:
                    oValue = null;
                    break;
            }
            return oValue;
        }
        static string GetSHA1Hash(SHA1 sha1Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        private string CreateTransID()
        {
            string line = "";
            string hash = "";
            foreach(string s in this.fields)
            {
                line += s;
            }
            using (SHA1 sha1hash = SHA1.Create())
            {
               hash = GetSHA1Hash(sha1hash, line);
            }
            return hash;
        }
        #endregion

        #region Construction
        public IntoPlaneImportFields(SortedList<string, int> headers):this(null, headers)
        {
        }

        public IntoPlaneImportFields(SiteClass site,SortedList<string, int> headers)
        {
            this.headers = headers;
            this.site = site;
        }
        #endregion

        #region Public Functions
        public void ParseValues(string line)
        {
            //use smarter parsing since Daily Fuel Transaction Files have quoted notes that may contain commas.
            using (var sr = new StringReader(line))
            {
                using (var tfp = new TextFieldParser(sr))
                {
                    tfp.TextFieldType = FieldType.Delimited;
                    tfp.SetDelimiters(",");
                    tfp.HasFieldsEnclosedInQuotes = true;
                    this.fields = tfp.ReadFields();
                }
            }

            if (this.fields != null)
            {
                for (int i = 0; i < this.fields.Length; i++)
                {
                    this.fields[i] = this.fields[i].Trim();
                }
            }
        }
        public bool ValidateHeader(out string message, out SortedList<string, int> headers )
        {
            bool result = true;
            headers = this.headers;
            message = string.Empty;
            List<string> missingColumns = new List<string>();

            if (null == this.fields)
            {
                    message = "No fields found";
                    return false;
            }
            int colNum = 0;
            foreach(string colName in this.fields)
            {
                if(!headers.Keys.Contains(colName))
                {
                    headers.Add(colName, colNum);
                }else
                {
                    message += "Duplicate column in header " + colName + Environment.NewLine;
                    result = false;
                }

                colNum++;
            }
            foreach (string requiredField in this.requiredFields)
            {
                if (!headers.ContainsKey(requiredField))
                {
                    missingColumns.Add(requiredField);
                }
            }

            
            if (missingColumns.Count() > 0)
            {
                message += "Header Missing column" + (missingColumns.Count() > 1 ? "s: " : ": ") + String.Join(", ", missingColumns) + Environment.NewLine;
                result = false;
            }
            return result;
        }
        #endregion

        #region Public Properties

        public bool IsGSE
        {
            get
            {
                return (this.TransactionSubtypeCode2.ToUpper() == "GSE");
            }
        }

        public bool ValidFieldCount
        {
            get
            {
                return (this.FieldCount >= this.headers.Count);
            }
        }

        public int FieldCount
        {
            get { return (this.fields != null) ? this.fields.Length : 0; }
        }

        public string Destination
        {
            get { return this.UserData1_DestinationID; }
        }
        
        public string Operator
        {
            get { return this.UserData22_OriginID; }
        }
        
        public string Gate
        {
            get { return this.UserData2_GateID; }
        }
        
        public bool FTZ => (this.TransactionSubtypeCode.ToUpper().Contains("FTZ"));

        public string ID => this.CreateTransID();

        public DateTime?     AckTime => (DateTime?)this.GetValue(IntoPlaneImportFieldNames.AckTime);

        public string       ArrivalFlightID => (string)(this.GetValue(IntoPlaneImportFieldNames.ArrivalFlightID) ?? string.Empty);

        public string       ArrivalTankName(int iIndex)
        {
            return (string)(this.GetValue("Arrival Tank Name " + iIndex) ?? string.Empty);
        }

        public bool         ArrivalTankShutoffInd(int iIndex)
        {
            object oValue = this.GetValue("Arrival Tank Shutoff Ind " + iIndex.ToString());
            return (oValue != null && (short)oValue == 1);
        }

        public double?       ArrivalTankValue(int iIndex)
        {
            return (double?)this.GetValue("Arrival Tank Value " + iIndex.ToString());
        }

        public DateTime?     ArrivalTime => (DateTime?)this.GetValue(IntoPlaneImportFieldNames.ArrivalTime);

        public string       CartRegistrationID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.CartRegistrationID) ?? string.Empty); }
        }

        public string       Customer
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.Customer) ?? string.Empty); }
        }

        public double       DensityTemp
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.DensityTemp); }
        }

        public EngineeringUnit          DensityUnits
        {
            get
            {
                var ret = this.GetValue(IntoPlaneImportFieldNames.DensityUnits);
                if (ret == null)
                {
                    return this.site.DensityUnits;
                }
                else
                {
                    return (EngineeringUnit)ret;
                }
            }
        }

        public string       DesiredTankName(int iIndex)
        {
            return (string)(this.GetValue("Desired Tank Name " + iIndex.ToString()) ?? string.Empty);
        }

        public bool         DesiredTankShutoffInd(int iIndex)
        {
            object oValue = this.GetValue("Desired Tank Shutoff Ind " + iIndex.ToString());
            return (oValue != null && (short)oValue == 1);
        }

        public double?       DesiredTankValue(int iIndex)
        {
            return (double?)this.GetValue("Desired Tank Value " + iIndex.ToString());
        }

        public string       DestinationRegistrationID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.DestinationRegistrationID) ?? string.Empty); }
        }

        public string       DeviceID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.DeviceID) ?? string.Empty); }
        }

        public double?       DiffPressure
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.DiffPressure); }
        }

        public bool?       DualFueling
        {
            get 
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.DualFueling);
                return (oValue != null && (short)oValue == 1);
            }
        }

        public double       EngRunTime
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.EngRunTime); }
        }

        public DateTime?     ETA
        {
            get { return (DateTime?)this.GetValue(IntoPlaneImportFieldNames.ETA); }
        }

        public DateTime?     ETD
        {
            get { return (DateTime?)this.GetValue(IntoPlaneImportFieldNames.ETD); }
        }

        public string       FinalTankName(int iIndex)
        {
            return (string)(this.GetValue("Final Tank Name " + iIndex.ToString()) ?? string.Empty);
        }

        public bool?         FinalTankShutoffInd(int iIndex)
        {
            object oValue = this.GetValue("Final Tank Shutoff Ind " + iIndex.ToString());
            return (oValue != null && (short)oValue == 1);

        }

        public double?       FinalTankValue(int iIndex)
        {
            return (double?)this.GetValue("Final Tank Value " + iIndex.ToString());
        }

        public double?       FlowRate
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.FlowRate); }
        }

        public double?       FreezePoint
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.FreezePoint); }
        }

        public bool         FromLDU
        {
            get
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.FromLDU);
                return (oValue != null && (short)oValue == 1);
            }
        }

        public string       FSRType
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.FSRType) ?? string.Empty); }
        }

        public double?       FuelCp
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.FuelCp); }
        }

        public double       FuelDensity
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.FuelDensity); }
        }

        public double       FuelTemp
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.FuelTemp); }
        }

        public DateTime?     FuelingStartTime
        {
            get { return (DateTime?)this.GetValue(IntoPlaneImportFieldNames.FuelingStartTime); }
        }

        public DateTime?     FuelingStopTime
        {
            get { return (DateTime?)this.GetValue(IntoPlaneImportFieldNames.FuelingStopTime); }
        }

        public double?       Gravity
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.Gravity); }
        }

        public double       GrossVolume
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.GrossVolume); }
        }

        public string        GseCategoryID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.GseCategoryID) ?? string.Empty); }
        }

        public string       GseID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.GseID) ?? string.Empty); }
        }

        public double?       HydrantPressure
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.HydrantPressure); }
        }

        public string       IPRemarks
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPRemarks) ?? string.Empty); }
        }

        public string       IPUserData1
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData1) ?? string.Empty); }
        }

        public string       IPUserData2
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData2) ?? string.Empty); }
        }

        public string       IPUserData3
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData3) ?? string.Empty); }
        }

        public string       IPUserData4
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData4) ?? string.Empty); }
        }

        public string       IPUserData5
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData5) ?? string.Empty); }
        }

        public string       IPUserData6
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData6) ?? string.Empty); }
        }

        public string       IPUserData7
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData7) ?? string.Empty); }
        }

        public string       IPUserData8
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData8) ?? string.Empty); }
        }

        public string       IPUserData9
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData9) ?? string.Empty); }
        }

        public string       IPUserData10
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData10) ?? string.Empty); }
        }

        public string       IPUserData11
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData11) ?? string.Empty); }
        }

        public string       IPUserData12
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData12) ?? string.Empty); }
        }

        public string       IPUserData13
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData13) ?? string.Empty); }
        }

        public string       IPUserData14
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData14) ?? string.Empty); }
        }

        public string       IPUserData15
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData15) ?? string.Empty); }
        }

        public string       IPUserData16
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData16) ?? string.Empty); }
        }

        public string       IPUserData17
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData17) ?? string.Empty); }
        }

        public string       IPUserData18
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData18) ?? string.Empty); }
        }

        public string       IPUserData19
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData19) ?? string.Empty); }
        }

        public string       IPUserData20
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData20) ?? string.Empty); }
        }

        public string       IPUserData21
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData21) ?? string.Empty); }
        }

        public string       IPUserData22
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData22) ?? string.Empty); }
        }

        public string       IPUserData23
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData23) ?? string.Empty); }
        }

        public string       IPUserData24
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.IPUserData24) ?? string.Empty); }
        }

        public DateTime     LeaveTime
        {
            get { return (DateTime)this.GetValue(IntoPlaneImportFieldNames.LeaveTime); }
        }

        public string       Manager
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.Manager) ?? string.Empty); }
        }

        public EngineeringUnit MassUnits
        {
            get {
                var ret = this.GetValue(IntoPlaneImportFieldNames.MassUnits);
                if (ret == null)
                {
                    return this.site.MassUnits;
                }
                else
                {
                    return (EngineeringUnit)ret;
                }
            }
        }

        public double?       MeterFactor
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.MeterFactor); }
        }

        public double       MeterStart
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.MeterStart); }
        }

        public bool?         MeterStartReadFromDCU
        {
            get
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.MeterStartReadFromDCU);
                return (oValue != null && (short)oValue == 1);
            }
        }

        public double       MeterStop
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.MeterStop); }
        }

        public bool?         MeterStopReadFromDCU
        {
            get
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.MeterStopReadFromDCU);
                return (oValue != null && (short)oValue == 1);
            }
        }

        public double?        NetVolume
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.NetVolume); }
        }

        public bool         NetVolumeIndicator
        {
            get
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.NetVolumeIndicator);
                return (oValue != null && (short)oValue == 1);
            }

        }

        public string       Notes
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.Notes) ?? string.Empty); }
        }

        public double       NozzlePressure
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.NozzlePressure); }
        }

        public double       ObsDensity
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.ObsDensity); }
        }

        public bool         ObsDensityInd
        {
            get
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.ObsDensityInd);
                return (oValue != null && (short)oValue == 1);
            }
        }

        public string       OperatorName
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.OperatorName) ?? string.Empty); }
        }

        public string       OriginID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.OriginID) ?? string.Empty); }
        }

        public DateTime?     OriginTime
        {
            get { return (DateTime?)this.GetValue(IntoPlaneImportFieldNames.OriginTime); }
        }

        public string       Owner
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.Owner) ?? string.Empty); }
        }

        public bool         PartialFill
        {
            get
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.PartialFill);
                return (oValue != null && (short)oValue == 1);
            }

        }

        public bool         PrimaryFSR
        {
            get
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.PrimaryFSR);
                return (oValue != null && (short)oValue == 1);
            }
        }

        public string       Product
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.Product) ?? string.Empty); }
        }

        public string       SerialNumber_FlightNumber
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.SerialNumber_FlightNumber) ?? string.Empty); }
        }

        public DateTime?     SFT
        {
            get { return (DateTime?)this.GetValue(IntoPlaneImportFieldNames.SFT); }
        }

        public DateTime?     STA
        {
            get { return (DateTime?)this.GetValue(IntoPlaneImportFieldNames.STA); }
        }

        public DateTime?     STD
        {
            get { return (DateTime?)this.GetValue(IntoPlaneImportFieldNames.STD); }
        }

        public double       StdDensity
        {
            get { return (double)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.StdDensity); }
        }

        public bool         StdDensityInd
        {
            get
            {
                object oValue = this.GetValue(IntoPlaneImportFieldNames.StdDensityInd);
                return (oValue != null && (short)oValue == 1);
            }
        }

        public int          TankCount
        {
            get
            {
                int i = 0;
                for (i = 0; i < 10; i++)
                {
                    if (this.DesiredTankName(i + 1) == "")
                        break;
                }
                return i;
            }
        }

        public string       TempQualityStatus
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.TempQualityStatus) ?? string.Empty); }
        }

        public double?       Temperature
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.Temperature); }
        }

        public EngineeringUnit TemperatureUnits
        {
            get {
                var ret = this.GetValue(IntoPlaneImportFieldNames.TemperatureUnits);
                if (ret == null)
                {
                    return this.site.TemperatureUnits;
                }
                else
                {
                    return (EngineeringUnit)ret;
                }
            }
        }

        public string       TicketNumber
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.TicketNumber) ?? string.Empty); }
        }

        public string       TransactionAlias
        {
            get { return (string)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.TransactionAlias); }
        }

        public DateTime     TransactionDate
        {

            get {
                var dateSting = this.GetValue(IntoPlaneImportFieldNames.TransactionDate);
                if(DateTime.TryParse((string)dateSting, out DateTime datetime))
                {
                    return datetime;
                }
                return DEFAULT_DATE_TIME;
            }
        }

        // Allow for validation of Transaction Date before all other usages
        public string TransactionDateString
        {
            get
            {
                return (string)this.GetValue(IntoPlaneImportFieldNames.TransactionDate);
            }
        }

        public short        TransactionNumber
        {
            get { return (short)this.GetRequiredFieldValue(IntoPlaneImportFieldNames.TransactionNumber); }
        }

        public string       TransactionSubtypeCode
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.TransactionSubtypeCode) ?? string.Empty); }
        }

        public string       TransactionSubtypeCode2
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.TransactionSubtypeCode2) ?? string.Empty); }
        }

        public string       TransactionSubtypeCode3
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.TransactionSubtypeCode3) ?? string.Empty); }
        }

        public string       UserData1_DestinationID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData1_DestinationID) ?? string.Empty); }
        }

        public string       UserData2_GateID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData2_GateID) ?? string.Empty); }
        }

        public string       UserData3_Operator
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData3_Operator) ?? string.Empty); }
        }

        public string       UserData4
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData4) ?? string.Empty); }
        }

        public string       UserData5
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData5) ?? string.Empty); }
        }

        public string       UserData6
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData6) ?? string.Empty); }
        }

        public string       UserData7
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData7) ?? string.Empty); }
        }

        public string       UserData8
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData8) ?? string.Empty); }
        }

        public string       UserData9
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData9) ?? string.Empty); }
        }

        public string       UserData10
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData10) ?? string.Empty); }
        }

        public string       UserData11
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData11) ?? string.Empty); }
        }

        public string       UserData12
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData12) ?? string.Empty); }
        }

        public string       UserData13
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData13) ?? string.Empty); }
        }

        public string       UserData14
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData14) ?? string.Empty); }
        }

        public string       UserData15
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData15) ?? string.Empty); }
        }

        public string       UserData16
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData16) ?? string.Empty); }
        }

        public string       UserData17
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData17) ?? string.Empty); }
        }

        public string       UserData18
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData18) ?? string.Empty); }
        }

        public string       UserData19
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData19) ?? string.Empty); }
        }

        public string       UserData20
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData20) ?? string.Empty); }
        }

        public string       UserData21
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData21) ?? string.Empty); }
        }

        public string       UserData22_OriginID
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData22_OriginID) ?? string.Empty); }
        }

        public string       UserData23
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData23) ?? string.Empty); }
        }

        public string       UserData24
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.UserData24) ?? string.Empty); }
        }

        public double?       VCF
        {
            get { return (double?)this.GetValue(IntoPlaneImportFieldNames.VCF); }
        }

        public string       Vendor
        {
            get { return (string)(this.GetValue(IntoPlaneImportFieldNames.Vendor) ?? string.Empty); }
        }

        public EngineeringUnit VolumeUnits
        {
            get {
                var ret = this.GetValue(IntoPlaneImportFieldNames.VolumeUnits);
                if (ret == null)
                {
                    return this.site.VolumeUnits;
                }
                else
                {
                    return (EngineeringUnit)ret;
                }
            }
        }

        #endregion

    }
}
