
namespace FMPointCommon
{
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Opc.Ua;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using System.Text.RegularExpressions;

	public class TSTFileOperations
	{
		protected string RemoveComments(string line)
		{
				try
				{
					int commentStartIndex = line.IndexOf('(');
					if (commentStartIndex >= 0)
					{
						int commentStopIndex = line.IndexOf(')');
						if (commentStopIndex >= 0 && commentStopIndex > commentStartIndex)
						{
								var ret = line.Remove(commentStartIndex, commentStopIndex - commentStartIndex + 1);
								return ret;
						}
					}
				}
				catch (Exception e)
				{
					System.Console.WriteLine(e.Message);
					return line;
				}
				return line;
		}

		protected string Trim(string line)
		{
				char[] charsToTrim = { ' ', '\t', '\n', '\r' };
				return line.Trim(charsToTrim);
		}

		protected string[] Split(string s, string separator)
		{
				return s.Split(new string[] { separator }, StringSplitOptions.None);
		}

		//public static List<EngineeringUnit> GetUnitsByType(EngineeringUnitType unitType)
		public bool IsUnits(EngineeringUnit units, EngineeringUnitType unitType)
		{
				var listUnits = EngineeringUnits.GetUnitsByType(unitType);
				foreach (var unit in listUnits)
				{
					if (unit == units)
					{
						return true;
					}
				}
				return false;
		}

		protected void GetStrapTableUnits(string line)
		{
				var l = Trim(line);
				var vals = Split(l, Delimeter);
				int iLevelUnits = Convert.ToInt32(Trim(vals[0]));
				int iVolumeUnits = Convert.ToInt32(Trim(vals[1]));
				int iMassUnits = Convert.ToInt32(Trim(vals[2]));
				LevelUnits = (EngineeringUnit)iLevelUnits;
				VolumeUnits = (EngineeringUnit)iVolumeUnits;
				MassUnits = (EngineeringUnit)iMassUnits;
		}

		protected void VerifyStrapTableUnits(string fileName)
		{
			string errorMessage = string.Empty;

			PressureTank = false;
			if (IsUnits(LevelUnits, EngineeringUnitType.FmuLength) == false)
			{
				errorMessage = "Invalid Level Units.\n\rFile: " + fileName;
				// the following is necessary for CST's when they are impleneted for now just comment out
				//if (IsUnits(LevelUnits, ENGINEERING_UNIT_TYPE.FMU_PRESSURE) == false)
				//{
				//	throw new Exception("Invalid Units!");
				//}
				//else
				//{
				//	PressureTank = true;
				//}
			}
			if (IsUnits(VolumeUnits, EngineeringUnitType.FmuVolume) == false)
			{
				errorMessage = "Invalid Volume Units.\n\rFile: " + fileName;
			}
			if (IsUnits(MassUnits, EngineeringUnitType.FmuMass) == false)
			{
				errorMessage = "Invalid Mass Units.\n\rFile: " + fileName;
			}

			if (errorMessage.Length > 0)
			{
				throw new Exception(errorMessage);
			}
		}
		protected string GetDescription(string line)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			return Trim(vals[0]);
		}

		protected double GetRoofMass(string line,int MassDecimalPlaces,ref bool precisionIsLessThanFile,ref int dataPrecision)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			string stTemp = vals[0];
			NumberFormatInfo nfi = new NumberFormatInfo();
			nfi.NumberDecimalDigits = NumberOfDecimalDigits;
			nfi.NumberDecimalSeparator = DetermineDecimalSeparator(ref stTemp);

			GetStrapValuePrecision(vals[0], MassDecimalPlaces, nfi, ref precisionIsLessThanFile, ref dataPrecision);
			var returnValue = Double.Parse(Trim(vals[0]));
			returnValue = EngineeringUnits.Convert(returnValue, MassUnits, PointMassUnits, 0.00);
			return returnValue;
		}

		protected RoofTypeEnum GetRoofType(string line)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			if (Enum.TryParse<RoofTypeEnum>(Trim(vals[0]), out RoofTypeEnum roofType))
			{
				return roofType;
			}
			// If an error then return FixedRoof -- Need to confirm with Warren/Jim
			return RoofTypeEnum.FixedRoof;
		}

		protected double GetStrapTemperature(string line, int TemperatureDecimalPlaces, ref bool precisionIsLessThanFile, ref int dataPrecision)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			string stTemp = vals[0];
			NumberFormatInfo nfi = new NumberFormatInfo();
			nfi.NumberDecimalDigits = NumberOfDecimalDigits;
			nfi.NumberDecimalSeparator = DetermineDecimalSeparator(ref stTemp);
			GetStrapValuePrecision(vals[0], TemperatureDecimalPlaces, nfi, ref precisionIsLessThanFile, ref dataPrecision);
			return Double.Parse(Trim(vals[0]));
		}

		protected double GetStrapDensity(string line, int DensityDecimalPlaces, ref bool precisionIsLessThanFile, ref int dataPrecision)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			string stTemp = vals[0];
			NumberFormatInfo nfi = new NumberFormatInfo();
			nfi.NumberDecimalDigits = NumberOfDecimalDigits;
			nfi.NumberDecimalSeparator = DetermineDecimalSeparator(ref stTemp);
			GetStrapValuePrecision(vals[0], DensityDecimalPlaces, nfi, ref precisionIsLessThanFile, ref dataPrecision);
			var returnValue = Double.Parse(Trim(vals[0]));
			return returnValue;
		}

		protected double GetCriticalZone(string line, int LevelDecimalPlaces, ref bool precisionIsLessThanFile, ref int dataPrecision)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			var val0 = Trim(vals[0]);
			string stTemp = val0;

			// Pin Height may be stored as decimal value in the strap table file instead of ft-in-16th or ft-in-8th
			var fileLevelUnit = LevelUnits;
			var fileLevelDecimalPlaces = LevelDecimalPlaces;
			if (fileLevelUnit == EngineeringUnit.FmlFtIn16Th || fileLevelUnit == EngineeringUnit.FmlFtIn8Th)
			{
				if (stTemp.IndexOf("-", StringComparison.InvariantCulture) < 0)
				{
					fileLevelUnit = EngineeringUnit.FmlFeet;
					fileLevelDecimalPlaces = NumberOfDecimalDigits;
				}

			}
				// no need to check for precision if the unit is ft-in-16th or ft-in-8th
			if (fileLevelUnit != EngineeringUnit.FmlFtIn16Th && fileLevelUnit != EngineeringUnit.FmlFtIn8Th)
			{
				NumberFormatInfo nfi = new NumberFormatInfo();
				nfi.NumberDecimalDigits = NumberOfDecimalDigits;
				nfi.NumberDecimalSeparator = DetermineDecimalSeparator(ref stTemp);
				GetStrapValuePrecision(vals[0], fileLevelDecimalPlaces, nfi, ref precisionIsLessThanFile, ref dataPrecision);
			}
			var returnValue = (double)PointManager.ParseValue(typeof(double), fileLevelUnit, LevelNfi, val0);
			returnValue = EngineeringUnits.Convert(returnValue, fileLevelUnit, PointLevelUnits, 0.00);
			return returnValue;
		}

		protected double GetPinHeightZone(string line, int LevelDecimalPlaces, ref bool precisionIsLessThanFile, ref int dataPrecision)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			var val0 = Trim(vals[0]);
			string stTemp = val0;


			// Pin Height may be stored as decimal value in the strap table file instead of ft-in-16th or ft-in-8th
			var fileLevelUnit = LevelUnits;
			var fileLevelDecimalPlaces = LevelDecimalPlaces;
			if (fileLevelUnit == EngineeringUnit.FmlFtIn16Th || fileLevelUnit == EngineeringUnit.FmlFtIn8Th)
			{
				if (stTemp.IndexOf("-", StringComparison.InvariantCulture) < 0)
				{
					fileLevelUnit = EngineeringUnit.FmlFeet;
					fileLevelDecimalPlaces = NumberOfDecimalDigits;
				}
			}

			// no need to check for precision if the unit is ft-in-16th or ft-in-8th
			if (fileLevelUnit != EngineeringUnit.FmlFtIn16Th && fileLevelUnit != EngineeringUnit.FmlFtIn8Th)
			{
				NumberFormatInfo nfi = new NumberFormatInfo();
				nfi.NumberDecimalDigits = NumberOfDecimalDigits;
				nfi.NumberDecimalSeparator = DetermineDecimalSeparator(ref stTemp);
				GetStrapValuePrecision(vals[0], fileLevelDecimalPlaces, nfi, ref precisionIsLessThanFile, ref dataPrecision);
			}

			var returnValue = (double)PointManager.ParseValue(typeof(double), fileLevelUnit, LevelNfi, val0);
			returnValue = EngineeringUnits.Convert(returnValue, fileLevelUnit, PointLevelUnits, 0.00);
			return returnValue;
		}

		protected int GetNumberOfEntries(string line)
		{
				var l = Trim(line);
				var vals = Split(l, Delimeter);
				return (int)Double.Parse(Trim(vals[0]));
		}

		protected DateTimeOffset GetTimestamp(string line)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			double secs = (double)Convert.ToInt64(Trim(vals[0]));

			var ts = epoch.AddSeconds(secs);
			return ts;
		}
		protected bool GetVersionInformation(string line)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			if(vals.Length >= 2)
			{
				string vNum = Regex.Replace(Trim(vals[1]), @"[^\d]", String.Empty);
				return Int32.TryParse(vNum, out int x);
			}

			return false;
		}

		protected int IndexOfLastNonNumericCharacter(char[] valArr, int len)
		{
			const int zero = '0';
			const int nine = '9';
			for (int i = len - 1; i >= 0; i--)
			{
				int asciiCode = valArr[i];
				if (asciiCode >= zero && asciiCode <= nine)
				{
					continue;
				}
				return i;
			}
			return -1;
		}

		protected int IndexOfFirstNumericCharacterBeforeIndex(char[] valArr, int index)
		{
			const int zero = '0';
			const int nine = '9';
			for (int i = index - 1; i >= 0; i--)
			{
				int asciiCode = valArr[i];
				if (asciiCode >= zero && asciiCode <= nine)
				{
					return i;
				}
			}
			return -1;
		}

		protected string DetermineDecimalSeparator(ref string value)
		{
			int len = value.Length;
			char[] valArr = value.ToCharArray();
			int endIndex = IndexOfLastNonNumericCharacter(valArr, len);
			if (endIndex < 0)
			{
				// if we get here it is most likely a whole number without a decimal point so default to a '.'
				return ".";
			}
			int startIndex = IndexOfFirstNumericCharacterBeforeIndex(valArr, endIndex);
			if (startIndex < 0)
			{
				throw new Exception("Can't Determine Decimal Separator!");
			}

			var ret = value.Substring(startIndex + 1, endIndex - startIndex);
			value = value.Remove(startIndex + 1, endIndex - startIndex);
			return ret;
		}

		protected string DetermineGroupSeparator(string value)
		{
			int len = value.Length;
			char[] valArr = value.ToCharArray();
			int endIndex = IndexOfLastNonNumericCharacter(valArr, len);
			if (endIndex < 0)
			{
				return "";
			}
			int startIndex = IndexOfFirstNumericCharacterBeforeIndex(valArr, endIndex);
			if (startIndex < 0)
			{
				throw new Exception("Can't Determine Group Separator!");
			}
			return value.Substring(startIndex + 1, endIndex - startIndex);
		}

		protected string DetermineDelimeterFromFirstLine(string line)
		{
			var l = Trim(line);
			char[] lCharArr = l.ToCharArray();
			const int zero = '0';
			const int nine = '9';
			for (int i = 0; i < lCharArr.Length; i++)
			{
				int asciiCode = (int)lCharArr[i];
				if (asciiCode >= zero && asciiCode <= nine)
				{
					continue;
				}
				return l.Substring(i, 1);
			}
			throw new Exception("Delimeter Could Not Be Determined!");
		}

		protected string StreamToStr(Stream stream)
		{
				stream.Position = 0;
				using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
				{
					return streamReader.ReadToEnd();
				}
		}

		protected NumberFormatInfo DetermineNumberFormatInfo(string value)
		{
				NumberFormatInfo nfi = new NumberFormatInfo();
				nfi.NumberDecimalDigits = NumberOfDecimalDigits;
				nfi.NumberDecimalSeparator = DetermineDecimalSeparator(ref value);
				//nfi.NumberGroupSizes = new int[] { 3 };
				nfi.NumberGroupSeparator = DetermineGroupSeparator(value);
				//nfi.NegativeSign = "-";
				return nfi;

		}

		public NumberFormatInfo LevelNfi = null;
		public NumberFormatInfo VolumeNfi = null;
		public NumberFormatInfo MassNfi = null;
		public NumberFormatInfo TemperatureNfi = null;
		public NumberFormatInfo TimestampNfi = null;
		public NumberFormatInfo DensityNfi = null;
		public NumberFormatInfo SiteNfi = null;
		public string Delimeter = ",";
		protected DateTimeOffset epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		public DateTimeOffset Timestamp;
		public string Description;
		public bool PressureTank = false;
		public EngineeringUnit LevelUnits = EngineeringUnit.Fml16Th;
		public EngineeringUnit VolumeUnits = EngineeringUnit.FmvImpGal;
		public EngineeringUnit MassUnits = EngineeringUnit.FmmLb;
		public EngineeringUnit TemperatureUnits = EngineeringUnit.FmtDegF;
		public EngineeringUnit DensityUnits = EngineeringUnit.FmdDegApi;

		public EngineeringUnit PointLevelUnits = EngineeringUnit.Fml16Th;
		public EngineeringUnit PointVolumeUnits = EngineeringUnit.FmvImpGal;
		public EngineeringUnit PointMassUnits = EngineeringUnit.FmmLb;

		public double RoofMass = 0;
		public double StrapTemperature = 0;
		public double StrapDensity = 0;
		public double PinHeight = 0;
		public double CriticalZone = 0;
		public double TankShellReferenceTemperature = 0;
		public double DatumHeight = 0;
		public RoofTypeEnum RoofType = RoofTypeEnum.FixedRoof;

		public int NumberOfEntries = -1;

		public double StrapDensity2 = 0;
		public bool IsFM12OrHigher = false;	//true - FuelsManager 12 and above

		protected StrapTableEntry GetStrapTableEntry(string line,
													string fileName,
													int LevelDecimalPlaces,
													int VolumeDecimalPlaces,
													ref int dataPrecision,
													ref bool precisionIsLessThanFile)
		{
			var l = Trim(line);
			var vals = Split(l, Delimeter);
			var val0 = Trim(vals[0]);
			string errorMessage = string.Empty;

			if (l.Length == 0)
			{
				errorMessage = "Invalid Entry in the Strap List.\n\rEntry line is empty.\n\rFile: " + fileName;
				throw new Exception(errorMessage);
			}

			LevelNfi = DetermineNumberFormatInfo(val0);
			// if the level units are ft-in-xx then check the format else just ignore
			if (LevelUnits == EngineeringUnit.FmlFtIn16Th ||
				LevelUnits == EngineeringUnit.FmlFtIn8Th)
			{
				// check for two '-'
				string tempString = string.Empty;
				int indexposition = 0;
				indexposition = vals[0].IndexOf("-");
				if(indexposition == -1)
				{
					errorMessage = "Invalid Level Entry for Level (" + vals[0] + ").\n\rLevel Entry Does Not Match Level Units.\n\rFile: " + fileName;
					throw new Exception(errorMessage);
				}
				indexposition++;
				indexposition = vals[0].IndexOf("-", indexposition);
				if (indexposition == -1)
				{
					errorMessage = "Invalid Level Entry for Level (" + vals[0] + ").\n\rLevel Entry Does Not Match Level Units.\n\rFile: " + fileName;
					throw new Exception(errorMessage);
				}
			}
			else
			{
				GetStrapValuePrecision(val0, LevelDecimalPlaces, LevelNfi,ref precisionIsLessThanFile,ref dataPrecision);
			}
			var level = (double)PointManager.ParseValue(typeof(double), LevelUnits, LevelNfi, val0);
			level = EngineeringUnits.Convert(level, LevelUnits, PointLevelUnits, 0.00);

			// check volume		
			var val1 = Trim(vals[1]);
			VolumeNfi = DetermineNumberFormatInfo(val1);
			GetStrapValuePrecision(val1, VolumeDecimalPlaces, VolumeNfi,ref precisionIsLessThanFile, ref dataPrecision);
			var volume = (double)PointManager.ParseValue(typeof(double), VolumeUnits, VolumeNfi, val1);
			volume = EngineeringUnits.Convert(volume, VolumeUnits, PointVolumeUnits, 0.00);
			return new StrapTableEntry(level, volume);
		}

		protected void GetStrapValuePrecision(string stValue, 
											int decimalPoints, 
											NumberFormatInfo formatInfo, 
											ref bool precisionIsLessThanFile,
											ref int dataPrecision)
		{
			int returnValue = 0;
			int iLoop = 0;
			int iposition = stValue.IndexOf(formatInfo.NumberDecimalSeparator[0]);

			if (iposition < 0)
				return;

			++iposition;

			string newValue = stValue.Right(stValue.Length - iposition);

			iLoop = 0;
			foreach (var charec in newValue)
			{
				++iLoop;
				// only deal with non zero numbers
				if (charec == '9' ||
					charec == '8' ||
					charec == '7' ||
					charec == '6' ||
					charec == '5' ||
					charec == '4' ||
					charec == '3' ||
					charec == '2' ||
					charec == '1')
				{
					returnValue = iLoop;
				}
			}

			// we do not want to create an error if precion in the file is greater than configured precision
			//if (returnValue > decimalPoints)
				//precisionIsLessThanFile = true;

			if (dataPrecision < returnValue)
				dataPrecision = returnValue;

			return;
		}

		protected void CheckForIncreasingStrapTable(IndividualStrapTable strap, string fileName)
		{
			double prevLevel = 0;
			double prevVolume = 0;
			string errorMessage = string.Empty;
			for (int i = 0; i < strap.table.Count; i++)
			{
				if (i == 0)
				{
					prevLevel = strap.table[i].Level;
					prevVolume = strap.table[i].Volume;
				}
				else
				{
					if (strap.table[i].Level <= prevLevel)
					{
						string levelValue = PointManager.FormatValue(typeof(double), LevelUnits, LevelNfi, prevLevel);
						errorMessage = "Invalid Level Entry for Level (" + levelValue + ").\n\rLevel Entries out of Sequence.\n\r" + fileName;
						throw new Exception(errorMessage);
					}
					prevLevel = strap.table[i].Level;

					// check the volume
					if (strap.table[i].Volume <= prevVolume)
					{
						errorMessage = "Invalid Volume Entry for Volume (" + prevVolume.ToString() + ").\n\rVolume Entries out of Sequence.\n\r" + fileName;
						throw new Exception(errorMessage);
					}
					prevVolume = strap.table[i].Volume;
				}
			}
		}

		protected IndividualStrapTable ImportStrapFile(StreamReader file, 
														int strapTableIndex,
														string fileName,
														int LevelDecimalPlaces,
														int VolumeDecimalPlaces,
														int DensityDecimalPlaces,
														int TemperatureDecimalPlaces,
														int MassDecimalPlaces,
														ref int dataPrecision,
														ref bool precisionIsLessThanFile)
		{
			string line;
			int counter = 0;
			var strap = new IndividualStrapTable();
			string errorMessage = string.Empty;

			while ((line = file.ReadLine()) != null)
			{
				++counter;

				line = RemoveComments(line);

				if (counter == 1)
				{
					Delimeter = DetermineDelimeterFromFirstLine(line);
					Timestamp = GetTimestamp(line);
					IsFM12OrHigher = GetVersionInformation(line);
				}
				else if (counter == 2)
				{
					strap.StrapTableDescription = GetDescription(line);
					if (strap.StrapTableDescription.Length == 0)
					{
						strap.StrapTableDescription = "Strap Table " + (strapTableIndex + 1).ToString();
					}
				}
				else if (counter == 3)
				{
					GetStrapTableUnits(line);
					VerifyStrapTableUnits(fileName);
				}
				else if (counter == 4)
				{
					strap.RoofMass.Value = GetRoofMass(line, MassDecimalPlaces, ref precisionIsLessThanFile, ref dataPrecision);
				}
				else if (counter == 5)
				{
					strap.StrapTemperature.Value = GetStrapTemperature(line, TemperatureDecimalPlaces, ref precisionIsLessThanFile, ref dataPrecision);
				}
				else if (counter == 6)
				{
					strap.StrapDensity.Value = GetStrapDensity(line, DensityDecimalPlaces, ref precisionIsLessThanFile, ref dataPrecision);
				}
				else if (counter == 7)
				{
					strap.RoofLandingHeight.Value = GetPinHeightZone(line, LevelDecimalPlaces, ref precisionIsLessThanFile, ref dataPrecision);
				}
				else if (counter == 8)
				{
					strap.RoofFloatingHeight.Value = GetCriticalZone(line, LevelDecimalPlaces, ref precisionIsLessThanFile, ref dataPrecision);
				}
				else if (counter >= 9)
				{
					if (NumberOfEntries < 0)
					{
						if (IsFM12OrHigher)
						{
							if (counter == 9)
							{
								strap.TankShellReferenceTemperature.Value = GetStrapTemperature(line, TemperatureDecimalPlaces, ref precisionIsLessThanFile, ref dataPrecision);
							}
							else if (counter == 10)
							{
								strap.DatumHeight.Value = GetPinHeightZone(line, LevelDecimalPlaces, ref precisionIsLessThanFile, ref dataPrecision);
							}
							else if (counter == 11)
							{
								strap.RoofType = GetRoofType(line);
							}
							else if (counter == 12)
							{
								NumberOfEntries = GetNumberOfEntries(line);
							}
						}
						else
						{
							NumberOfEntries = GetNumberOfEntries(line);
						}
					}
					else
					{
							strap.table.Add(GetStrapTableEntry(line,
																fileName,
																LevelDecimalPlaces,
																VolumeDecimalPlaces,
																ref dataPrecision,
																ref precisionIsLessThanFile));
					}
				}
			}

			if (strap.table.Count < 4)
			{
				errorMessage = "Strap Tables must have a minimum of 4 entries.\n\rFile: " + fileName;
				throw new Exception(errorMessage);
			}

			CheckForIncreasingStrapTable(strap, fileName);
			return strap;
		}

		public bool ReadStrapFile(	Stream stream,
									EngineeringUnit levelUnit,
									EngineeringUnit volumeUnit,
									EngineeringUnit MassUnit,
									StrapTable strapTable,
									int strapTableIndex,
									string fileName,
									int LevelDecimalPlaces,
									int VolumeDecimalPlaces,
									int DensityDecimalPlaces,
									int TemperatureDecimalPlaces,
									int MassDecimalPlaces,
									ref int numberFoundInFile,
									ref int dataPrecision,
									ref bool precisionIsLessThanFile)
		{
			PointLevelUnits = levelUnit;
			PointVolumeUnits = volumeUnit;
			PointMassUnits = MassUnit;

			IndividualStrapTable individualStrapTable;

			using (StreamReader file = new StreamReader(stream))
			{
				individualStrapTable = ImportStrapFile(file, 
														strapTableIndex,
														fileName,
														LevelDecimalPlaces,
														VolumeDecimalPlaces,
														DensityDecimalPlaces,
														TemperatureDecimalPlaces,
														MassDecimalPlaces,
														ref dataPrecision,
														ref precisionIsLessThanFile);
			}
			numberFoundInFile = NumberOfEntries;
			strapTable.StrapTables[strapTableIndex] = individualStrapTable;
			return IsFM12OrHigher;
		}

		public void WriteStrapFile(string strapFileName, StrapTable strap, BasePoint basePoint, SiteClass site, int strapTableIndex)
		{
				using (StreamWriter file = new StreamWriter(strapFileName))
				{
					ExportStrapFile(file, strap, basePoint, site, strapTableIndex);
					file.Close();
				}
		}

		public string WriteStrapFile(StrapTable strap, BasePoint basePoint, SiteClass site, int strapTableIndex)
		{
				using (MemoryStream stream = new MemoryStream())
				{
					WriteStrapFile(stream, strap, basePoint, site, strapTableIndex);
					return StreamToStr(stream);
				}
		}

		public void WriteStrapFile(Stream stream, StrapTable strap, BasePoint basePoint, SiteClass site, int strapTableIndex)
		{
				using (StreamWriter file = new StreamWriter(stream, Encoding.UTF8, 512, true))
				{
					ExportStrapFile(file, strap, basePoint, site, strapTableIndex);
				}
		}

		protected void WriteTimestamp(StreamWriter file, string longDatePattern)
		{
			var currentTime = DateTimeOffset.Now;
			var dateDiff = currentTime.Subtract(epoch).TotalSeconds;
			var dateDiffSeconds = (int)dateDiff;
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							dateDiffSeconds + Delimeter + "V12" + Delimeter,
							"(" + currentTime.ToString(longDatePattern) + ", FM Version)");
			file.WriteLine(formattedString);
			//file.WriteLine(dateDiffSeconds + Delimeter + "\t\t\t(" + currentTime.ToString(longDatePattern) + ")");
		}

		protected void WriteDescription(StreamWriter file)
		{
				file.WriteLine(Description + Delimeter);
		}

		protected void WriteUnits(StreamWriter file)
		{
				int vu = (int)VolumeUnits;
				int lu = (int)LevelUnits;
				int mu = (int)MassUnits;
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							lu.ToString(LevelNfi) + Delimeter + vu.ToString(VolumeNfi) + Delimeter + mu.ToString(MassNfi) + Delimeter,
							"(Level Units = " + EngineeringUnits.GetUnitString(LevelUnits) + ", Volume Units = " + EngineeringUnits.GetUnitString(VolumeUnits) + ", Mass Units = " + EngineeringUnits.GetUnitString(MassUnits) + ")");
			file.WriteLine(formattedString);
			//file.WriteLine(lu.ToString(LevelNfi) + Delimeter + vu.ToString(VolumeNfi) + Delimeter + mu.ToString(MassNfi) + Delimeter + "\t\t\t(Level Units = " + EngineeringUnits.GetUnitString(LevelUnits) + ", Volume Units = " + EngineeringUnits.GetUnitString(VolumeUnits) + ", Mass Units = " + EngineeringUnits.GetUnitString(MassUnits) + ")");
		}

		protected void WriteRoofMass(StreamWriter file)
		{
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString, RoofMass.ToString(MassNfi) + Delimeter, "(Roof Mass)");
			file.WriteLine(formattedString);
			//file.WriteLine(RoofMass.ToString(MassNfi) + Delimeter + "\t\t\t\t(Roof Mass)");
		}

		protected void WriteStrapTemperature(StreamWriter file)
		{
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							StrapTemperature.ToString(TemperatureNfi) + Delimeter,
							"(Strap Temperature)");
			file.WriteLine(formattedString);
			//file.WriteLine(StrapTemperature.ToString(TemperatureNfi) + Delimeter + "\t\t\t\t(Strap Temperature)");
		}

		protected void WriteStrapDensity(StreamWriter file)
		{
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							StrapDensity.ToString(DensityNfi) + Delimeter,
							"(Strap Density)");
			file.WriteLine(formattedString);
			//file.WriteLine(StrapDensity.ToString(DensityNfi) + Delimeter + "\t\t\t\t(Strap Density)");
		}

		protected void WritePinHeight(StreamWriter file)
		{
			string pinHeight = PointManager.FormatValue(typeof(double), LevelUnits, LevelNfi, PinHeight);
			StripTrailingZeros(ref pinHeight, LevelNfi.CurrencyDecimalSeparator);
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							pinHeight + Delimeter,
							"(Pin Height aka Critical Low)");
			file.WriteLine(formattedString);
			//file.WriteLine(pinHeight + Delimeter + "\t\t\t(Pin Height aka Critical Low)");
		}

		protected void WriteCriticalZone(StreamWriter file)
		{
			string criticalZone = PointManager.FormatValue(typeof(double), LevelUnits, LevelNfi, CriticalZone);
			StripTrailingZeros(ref criticalZone, LevelNfi.CurrencyDecimalSeparator);
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							criticalZone + Delimeter,
							"(Critical Zone aka Critical High)");
			file.WriteLine(formattedString);
			//file.WriteLine(criticalZone + Delimeter + "\t\t\t(Critical Zone aka Critical High)");
		}

		protected void WriteShellReferenceTemperature(StreamWriter file)
		{
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							TankShellReferenceTemperature.ToString(TemperatureNfi) + Delimeter,
							"(Tank Shell Reference Temperature)");
			file.WriteLine(formattedString);
			//file.WriteLine(StrapTemperature.ToString(TemperatureNfi) + Delimeter + "\t\t\t\t(Strap Temperature)");
		}

		protected void WriteDatumHeight(StreamWriter file)
		{
			string datumHeight = PointManager.FormatValue(typeof(double), LevelUnits, LevelNfi, DatumHeight);
			StripTrailingZeros(ref datumHeight, LevelNfi.CurrencyDecimalSeparator);
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							datumHeight + Delimeter,
							"(Datum Height)");
			file.WriteLine(formattedString);
			//file.WriteLine(pinHeight + Delimeter + "\t\t\t(Pin Height aka Critical Low)");
		}
		protected void WriteRoofType(StreamWriter file)
		{
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							RoofType.ToString() + Delimeter,
							"(Roof Type)");
			file.WriteLine(formattedString + Delimeter);
		}

		protected void WriteNumberOfEntries(StreamWriter file, int numEntries)
		{
			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							numEntries.ToString(SiteNfi) + Delimeter,
							"(" + numEntries.ToString(SiteNfi) + " of strapping entries)");
			file.WriteLine(formattedString);
			//file.WriteLine(numEntries.ToString(SiteNfi) + Delimeter + "\t\t\t\t(" + numEntries.ToString(SiteNfi) + " of strapping entries)");
		}

		protected void WriteStrapEntry(StreamWriter file, StrapTableEntry entry, int count)
		{
			string level = PointManager.FormatValue(typeof(double), LevelUnits, LevelNfi, entry.Level);
			StripTrailingZeros(ref level, LevelNfi.NumberDecimalSeparator);
			string volume = PointManager.FormatValue(typeof(double), VolumeUnits, VolumeNfi, entry.Volume);
			StripTrailingZeros(ref volume, VolumeNfi.NumberDecimalSeparator);
			string levelvolumeformattedString = string.Empty;
			FormatLevelVolumeFileOutput(ref levelvolumeformattedString,
							level + Delimeter,
							volume + Delimeter);

			string formattedString = string.Empty;
			FormatFileOutput(ref formattedString,
							levelvolumeformattedString,
							"(Entry #" + count.ToString(SiteNfi) + ")");
			file.WriteLine(formattedString);
			//file.WriteLine(level + Delimeter + "\t" + volume + Delimeter + "\t(Entry #" + count.ToString(SiteNfi) + ")");
		}

		public const int NumberOfDecimalDigits = 13;

		protected void ExportStrapFile(StreamWriter file, StrapTable strap, BasePoint basePoint, SiteClass site, int strapTableIndex)
		{
			Delimeter = site.ListSeparator;
			if (Delimeter == null || Delimeter == "")
			{
				throw new Exception("ListSeparator not defined for Site!");
			}

			if(strapTableIndex < 0
			|| strapTableIndex >= strap.StrapTables.Length)
			{
				throw new Exception("Strap Table Index out of range!");
			}

			var individualStrapTable = strap.StrapTables[strapTableIndex];			

			LevelNfi = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.LENGTH);
			LevelNfi.NumberDecimalDigits = NumberOfDecimalDigits; // point.LevelDecimalPlaces;
			VolumeNfi = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			VolumeNfi.NumberDecimalDigits = NumberOfDecimalDigits; // point.VolumeDecimalPlaces;
			MassNfi = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS);
			MassNfi.NumberDecimalDigits = NumberOfDecimalDigits; // point.MassDecimalPlaces;
			TemperatureNfi = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE);
			TemperatureNfi.NumberDecimalDigits = NumberOfDecimalDigits; // point.TemperatureDecimalPlaces;
			DensityNfi = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY);
			DensityNfi.NumberDecimalDigits = NumberOfDecimalDigits; // point.DensityDecimalPlaces;
			SiteNfi = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
			TimestampNfi = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
			Description = individualStrapTable.StrapTableDescription;
			LevelUnits = basePoint.LevelUnit;
			VolumeUnits = basePoint.VolumeUnit;
			MassUnits = basePoint.MassUnit;
			TemperatureUnits = basePoint.TemperatureUnit;
			DensityUnits = basePoint.DensityUnit;

			// need to specify selected strap table here
			individualStrapTable.SortByLevel();

			RoofMass = individualStrapTable.RoofMass.Value;
			StrapTemperature = individualStrapTable.StrapTemperature.Value;
			StrapDensity = individualStrapTable.StrapDensity.Value;
			PinHeight = individualStrapTable.RoofLandingHeight.Value;
			CriticalZone = individualStrapTable.RoofFloatingHeight.Value;
			TankShellReferenceTemperature = individualStrapTable.TankShellReferenceTemperature.Value;
			DatumHeight = individualStrapTable.DatumHeight.Value;
			RoofType = individualStrapTable.RoofType;

			WriteTimestamp(file, site.LongDatePattern);
			WriteDescription(file);
			WriteUnits(file);
			WriteRoofMass(file);
			WriteStrapTemperature(file);
			WriteStrapDensity(file);
			WritePinHeight(file);
			WriteCriticalZone(file);
			WriteShellReferenceTemperature(file);
			WriteDatumHeight(file);
			WriteRoofType(file);
			WriteNumberOfEntries(file, individualStrapTable.table.Count);
			int counter = 0;
			foreach (var entry in individualStrapTable.table)
			{
				counter++;
				WriteStrapEntry(file, entry, counter);
			}
		}

		protected void StripTrailingZeros(ref string passedString,string decimalSeperator)
		{
			string stTemp = string.Empty;
			passedString.Trim();
			
			if (passedString.Contains(decimalSeperator))
			{
				while (passedString.Right(1) == "0")
				{
					stTemp = passedString.Left(passedString.Length - 1);
					passedString = stTemp;
				}
				if(passedString.Right(1) == decimalSeperator)
				{
					passedString += "00";
				}
			}
			else if (!passedString.Contains("-"))
			{
				// non fractional and no decimal points
				passedString += decimalSeperator;
				passedString += "00";
			}
		}
		protected void FormatFileOutput(ref string formattedString, string beginning, string end)
		{
			int numberofSpaces = 35;
			int loopPosition = 0;
			formattedString = string.Empty;

			formattedString = beginning;
			for(loopPosition = formattedString.Length; loopPosition < numberofSpaces; loopPosition++)
			{
				formattedString += " ";
			}
			formattedString += end;
		}

		protected void FormatLevelVolumeFileOutput(ref string levelvolumeformattedString,string level, string volume)
		{
			int numberofSpaces = 15;
			int loopPosition = 0;
			levelvolumeformattedString = string.Empty;

			levelvolumeformattedString = level;
			for (loopPosition = levelvolumeformattedString.Length; loopPosition < numberofSpaces; loopPosition++)
			{
				levelvolumeformattedString += " ";
			}
			levelvolumeformattedString += volume;
		}


	}
}
