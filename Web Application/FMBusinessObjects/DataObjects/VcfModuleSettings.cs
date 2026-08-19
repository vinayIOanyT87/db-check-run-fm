

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using CodedVariables;
	using System.Collections.Generic;
	using FMBusinessObjects.Attributes;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
   using Varec.CommonComponents.VolumeCorrection;

	[DataContract(Namespace = "")]
	[Serializable()]
	public class VcfModuleSettings
	{

		[DataMember(Order = 0)]
		public PointPropertyUnitTypedDouble DensityPressure { get; set; }

		[DataMember(Order = 1)]
		public PointPropertyUnitTypedDouble AlternateTemperature { get; set; }
      
		[FMExposedSetting("Temperature Standard", ModifyDisabled = true)]
      [DataMember(Order = 2)]
		public PointPropertyUnitTypedDouble BaseTemperature { get; set; }

		[DataMember(Order = 3)]
		public PointPropertyUnitTypedDouble AlternateBasePressure { get; set; }

		[DataMember(Order = 4)]
		public double[] K { get; set; }

		[DataMember(Order = 5)]
		public double Alpha { get; set; }

		[DataMember(Order = 6)]
		public bool UseProductObservedDensity { get; set; }

		[DataMember(Order = 7)]
		public bool UseHydrometerCorrection { get; set; }

		[DataMember(Order = 8)]
		public bool ForceVcfTo4Digits { get; set; }

		[DataMember(Order = 9)]
		public ECorrectionTypeMajor CorrectionMethodType { get; set; }

		[DataMember(Order = 10)]
		public ECorrectionTypeMinor CorrectionMethodSpecific { get; set; }

		[FMExposedSetting("Correction Standard/Organization", ModifyDisabled = true)]
		public string CorrectionStandardOrOrganization
		{
			get
			{
				return GetStandardsOrganization(CorrectionMethodType);
			}
		}

		[FMExposedSetting("Correction Revision", ModifyDisabled = true)]
		public string CorrectionStandardRevision
		{
			get
			{
				return GetStandardRevision(CorrectionMethodType, CorrectionMethodSpecific);
			}
		}


		[FMExposedSetting("Correction Commodity/Table", ModifyDisabled = true)]
		public string CorrectionCommodityOrTable
		{
			get
			{
				return GetCommodityOrTable(CorrectionMethodSpecific);
			}
		}




		public VcfModuleSettings()
		{
			CorrectionMethodType = ECorrectionTypeMajor.CORR_ASTM_COMM_2004;
			CorrectionMethodSpecific = ECorrectionTypeMinor.CORR_REFINED_PRODUCTS;

			DensityPressure = new PointPropertyUnitTypedDouble(0.00, EngineeringUnitType.FmuPressure);
			AlternateTemperature = new PointPropertyUnitTypedDouble(0.00, EngineeringUnitType.FmuTemp);
			BaseTemperature = new PointPropertyUnitTypedDouble(60.00, EngineeringUnitType.FmuTemp);
			AlternateBasePressure = new PointPropertyUnitTypedDouble(0.00, EngineeringUnitType.FmuPressure);
			K = new double[] { 0.00, 0.00, 0.00, 0.00, 0.00 };
			Alpha = 0;
			UseProductObservedDensity = false;
			UseHydrometerCorrection = false;
			ForceVcfTo4Digits = false;
		}

		public static string GetStandardsOrganization(ECorrectionTypeMajor correctionTypeMajor)
		{
			switch (correctionTypeMajor)
			{
				case ECorrectionTypeMajor.CORR_NONE:
				case ECorrectionTypeMajor.CORR_NONE_1980:
					return "None";

				case ECorrectionTypeMajor.CORR_API_C:
				case ECorrectionTypeMajor.CORR_API_C_1980:
				case ECorrectionTypeMajor.CORR_API_F:
				case ECorrectionTypeMajor.CORR_API_F_1980:
				case ECorrectionTypeMajor.CORR_ASTM_COMM_2004:
					return "API";


				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F:
				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F_1980:
					return "Custom";

				case ECorrectionTypeMajor.CORR_LPG_C:
				case ECorrectionTypeMajor.CORR_LPG_C_1980:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2004:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_1980:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980:
				case ECorrectionTypeMajor.CORR_ASPHALT:
				case ECorrectionTypeMajor.CORR_ASTM_D1250_1952:
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009:
					return "ASTM";

				case ECorrectionTypeMajor.CORR_JAPAN_NONE:
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249:
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2250:
				case ECorrectionTypeMajor.CORR_JAPAN_CHEMICAL:
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE:
				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1250:
				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1555:
					return "JIS";

				case ECorrectionTypeMajor.CORR_GBT:
					return "GB/T";

				case ECorrectionTypeMajor.CORR_GOST:
					return "GOST";


				default:
					return "None";
			}
		}

		public static ECorrectionTypeMajor GetCorrectionTypeMajor(string standardsOrganization, string standardAndRevision, string standardTemperature)
		{
			switch (standardsOrganization)
			{
				case "None":
					switch (standardAndRevision)
					{
						case "1952":
							return ECorrectionTypeMajor.CORR_NONE;
						case "1980":
							return ECorrectionTypeMajor.CORR_NONE_1980;
						default:
							break;
					}
					break;

				case "API":
					switch (standardAndRevision)
					{
						case "1952":
							if (standardTemperature.Contains("°C"))
							{
								return ECorrectionTypeMajor.CORR_API_C;
							}
							else
							{
								return ECorrectionTypeMajor.CORR_API_F;
							}

						case "1980":
							if (standardTemperature.Contains("°C"))
							{
								return ECorrectionTypeMajor.CORR_API_C_1980;
							}
							else
							{
								return ECorrectionTypeMajor.CORR_API_F_1980;
							}

						case "Commodity (2004)":
							return ECorrectionTypeMajor.CORR_ASTM_COMM_2004;


						default:
							break;
					}
					break;

				case "ASTM":
					switch (standardAndRevision)
					{
						case "D1250 (1952)":
							return ECorrectionTypeMajor.CORR_ASTM_D1250_1952;
						case "D1250 (1980)":
							return ECorrectionTypeMajor.CORR_LPG_C_1980;
						case "D1555 (1980)":
							if (standardTemperature.Contains("°C"))
							{
								return ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980;
							}
							else
							{
								return ECorrectionTypeMajor.CORR_ASTM_D1555_F_1980;
							}
						case "D1555 (2004)":
							if (standardTemperature.Contains("°C"))
							{
								return ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004;
							}
							else
							{
								return ECorrectionTypeMajor.CORR_ASTM_D1555_F_1980;
							}
						case "D1555 (2009)":
							return ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009;
						case "D4311 (2004)":
							return ECorrectionTypeMajor.CORR_ASPHALT;
						case "D4311 (2009)":
							return ECorrectionTypeMajor.CORR_ASPHALT;
						case "IP":
							return ECorrectionTypeMajor.CORR_ASPHALT;
						default:
							break;
					}
					break;

				case "Custom":
					if (standardAndRevision == "Polynomial (1952)")
					{
						return ECorrectionTypeMajor.CORR_POLYNOMIAL_F;
					}
					else
					{
						return ECorrectionTypeMajor.CORR_POLYNOMIAL_F_1980;
					}

				case "JIS":
					switch (standardAndRevision)
					{
						case "None":
							return ECorrectionTypeMajor.CORR_JAPAN_NONE;
						case "D1555":
							return ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1555;
						case "D1250":
							return ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1250;
						case "2249 (1980)":
							return ECorrectionTypeMajor.CORR_JAPAN_JIS_2249;
						case "2250 (1967)":
							return ECorrectionTypeMajor.CORR_JAPAN_JIS_2250;
						case "Chemical":
							return ECorrectionTypeMajor.CORR_JAPAN_CHEMICAL;
						case "2249 (1980) Table":
							return ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE;
						default:
							break;
					}
					break;

				case "GB/T":
					return ECorrectionTypeMajor.CORR_GBT;

				case "GOST":
					return ECorrectionTypeMajor.CORR_GOST;

				default:
					break;
			}

			return ECorrectionTypeMajor.CORR_NONE;
		}

		public static ECorrectionTypeMinor GetCorrectionTypeMinor(string standardsOrganization, string standardAndRevision, string commodityOrTable, string standardTemperature)
		{
			switch (standardsOrganization)
			{
				case "None":
					return ECorrectionTypeMinor.CORR_NONE;

				case "API":
					switch (standardAndRevision)
					{
						case "1952":
						case "1980":
						case "2004":

							switch (commodityOrTable)
							{
								case "54A/53A":
									if (standardTemperature.Contains("15 °C"))
									{
										return ECorrectionTypeMinor.CORR_API54A;
									}
									else
									{
										return ECorrectionTypeMinor.CORR_API54A_30;
									}
								case "54B/53B":
									if (standardTemperature.Contains("15 °C"))
									{
										return ECorrectionTypeMinor.CORR_API54B;
									}
									else
									{
										return ECorrectionTypeMinor.CORR_API54B_30;
									}
								case "54C":
									if (standardTemperature.Contains("15 °C"))
									{
										return ECorrectionTypeMinor.CORR_API54C;
									}
									else
									{
										return ECorrectionTypeMinor.CORR_API54C_30;
									}
								case "54D":
									if (standardTemperature.Contains("15 °C"))
									{
										return ECorrectionTypeMinor.CORR_API54D;
									}
									else
									{
										return ECorrectionTypeMinor.CORR_API54D_30;
									}

								case "60A/59A":
									return ECorrectionTypeMinor.CORR_API60A;
								case "60B/59B":
									return ECorrectionTypeMinor.CORR_API60B;
								case "60D/59D":
									return ECorrectionTypeMinor.CORR_API60D;
								case "6A/5A":
									return ECorrectionTypeMinor.CORR_API6A;
								case "6B/5B":
									return ECorrectionTypeMinor.CORR_API6B;
								case "6C":
									return ECorrectionTypeMinor.CORR_API6C;
								case "6D":
									return ECorrectionTypeMinor.CORR_API6D;
								case "24E/23E":
									return ECorrectionTypeMinor.CORR_API24E;
								default:
									break;
							}
							break;

						case "Commodity (2004)":
							switch (commodityOrTable)
							{
								case "Alpha 60 Supplied":
									return ECorrectionTypeMinor.CORR_ALPHA60_SUPPLIED;
								case "Crude Oils":
									return ECorrectionTypeMinor.CORR_CRUDE_OIL;
								case "Lubrication Oils":
									return ECorrectionTypeMinor.CORR_LUBRICATION_OIL;
								case "Refined Products":
									return ECorrectionTypeMinor.CORR_REFINED_PRODUCTS;
								default:
									break;
							}
							break;

						default:
							break;
					}
					break;

				case "ASTM":
					switch (standardAndRevision)
					{
						case "D1250 (1952)":
							return ECorrectionTypeMinor.CORR_D125020DEGC;

						case "D1250 (1980)":
							return ECorrectionTypeMinor.CORR_LPG;

						case "D1555 (1980)":
						case "D1555 (2004)":
						case "D1555 (2009)":
							switch (commodityOrTable)
							{
								case "Benzene":
									return ECorrectionTypeMinor.CORR_BENZENE;
								case "Toluene":
									return ECorrectionTypeMinor.CORR_TOLUENE;
								case "Mixed Xylene":
									return ECorrectionTypeMinor.CORR_M_XYLENE;
								case "Styrene":
									return ECorrectionTypeMinor.CORR_STYRENE;
								case "o-Xylene":
									return ECorrectionTypeMinor.CORR_O_XYLENE;
								case "p-Xylene":
									return ECorrectionTypeMinor.CORR_P_XYLENE;
								case "Cylco-Hexane":
									return ECorrectionTypeMinor.CORR_CYCLO_HEXANE;
								case "Ethyl-Benzene":
									return ECorrectionTypeMinor.CORR_ETHYL_BENZENE;
								case "Cumene":
									return ECorrectionTypeMinor.CORR_CUMENE;
								case "300 °F/148.9 °C Aromatic":
									return ECorrectionTypeMinor.CORR_300_AROMATIC;
								case "350 °F/176.7 °C Aromatic":
									return ECorrectionTypeMinor.CORR_350_AROMATIC;
								default:
									break;
							}
							break;

						case "D4311 (2004)":
							if (standardTemperature.Contains("°C"))
							{
								return ECorrectionTypeMinor.CORR_D4311DEGC_2004;
							}
							else
							{
								return ECorrectionTypeMinor.CORR_D4311DEGC_2009;
							}
						case "D4311 (2009)":
							if (standardTemperature.Contains("°C"))
							{
								return ECorrectionTypeMinor.CORR_D4311DEGC_2009;
							}
							else
							{
								return ECorrectionTypeMinor.CORR_D4311DEGF_2009;
							}
						case "IP":
							return ECorrectionTypeMinor.CORR_TABLE7;
						default:
							break;
					}
					break;

				case "Custom":
					return ECorrectionTypeMinor.CORR_POLYNOMIAL;

				case "JIS":
					switch (standardAndRevision)
					{
						case "None":
							return ECorrectionTypeMinor.CORR_NONE;
						case "D1555":
							switch (commodityOrTable)
							{
								case "Benzene":
									return ECorrectionTypeMinor.CORR_BENZENE;
								case "Toluene":
									return ECorrectionTypeMinor.CORR_TOLUENE;
								case "Mixed Xylene":
									return ECorrectionTypeMinor.CORR_M_XYLENE;
								case "Styrene":
									return ECorrectionTypeMinor.CORR_STYRENE;
								case "o-Xylene":
									return ECorrectionTypeMinor.CORR_O_XYLENE;
								case "p-Xylene":
									return ECorrectionTypeMinor.CORR_P_XYLENE;
								case "Cylco-Hexane":
									return ECorrectionTypeMinor.CORR_CYCLO_HEXANE;
								case "Ethyl-Benzene":
									return ECorrectionTypeMinor.CORR_ETHYL_BENZENE;
								case "Cumene":
									return ECorrectionTypeMinor.CORR_CUMENE;
								case "300 °F/148.9 °C Aromatic":
									return ECorrectionTypeMinor.CORR_300_AROMATIC;
								case "350 °F/176.7 °C Aromatic":
									return ECorrectionTypeMinor.CORR_350_AROMATIC;
								default:
									break;
							}
							break;
						case "D1250":
							switch (commodityOrTable)
							{
								case "2 (54)":
									return ECorrectionTypeMinor.CORR_JIS_TABLE2;
								case "54A (6X)":
									return ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54A;
								case "54B (6X)":
									return ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54B;
								case "55":
									return ECorrectionTypeMinor.CORR_ASTM_TABLE55;
								default:
									break;
							}
							break;
						case "2249 (1980)":
							switch (commodityOrTable)
							{

								case "54A/53A":
									if (standardTemperature.Contains("15 °C"))
									{
										return ECorrectionTypeMinor.CORR_API54A;
									}
									else
									{
										return ECorrectionTypeMinor.CORR_API54A_30;
									}
								case "54B/53B":
									if (standardTemperature.Contains("15 °C"))
									{
										return ECorrectionTypeMinor.CORR_API54B;
									}
									else
									{
										return ECorrectionTypeMinor.CORR_API54B_30;
									}
								case "54C":
									if (standardTemperature.Contains("15 °C"))
									{
										return ECorrectionTypeMinor.CORR_API54C;
									}
									else
									{
										return ECorrectionTypeMinor.CORR_API54C_30;
									}
								case "54D":
									if (standardTemperature.Contains("15 °C"))
									{
										return ECorrectionTypeMinor.CORR_API54D;
									}
									else
									{
										return ECorrectionTypeMinor.CORR_API54D_30;
									}
								default:
									break;
							}
							break;

						case "2250 (1967)":
							return ECorrectionTypeMinor.CORR_JIS_TABLE2;
						case "Chemical":
							if (standardTemperature.Contains("15 °C"))
							{
								return ECorrectionTypeMinor.CORR_JIS_CHEMICAL1;
							}
							else
							{
								return ECorrectionTypeMinor.CORR_JIS_CHEMICAL2;
							}

						case "2249 (1980) Table":
							{
								switch (commodityOrTable)
								{

									case "54A/53A":
										if (standardTemperature.Contains("15 °C"))
										{
											return ECorrectionTypeMinor.CORR_API54A;
										}
										else
										{
											return ECorrectionTypeMinor.CORR_API54A_30;
										}
									case "54B/53B":
										if (standardTemperature.Contains("15 °C"))
										{
											return ECorrectionTypeMinor.CORR_API54B;
										}
										else
										{
											return ECorrectionTypeMinor.CORR_API54B_30;
										}
									case "54C":
										if (standardTemperature.Contains("15 °C"))
										{
											return ECorrectionTypeMinor.CORR_API54C;
										}
										else
										{
											return ECorrectionTypeMinor.CORR_API54C_30;
										}
									case "54D":
										if (standardTemperature.Contains("15 °C"))
										{
											return ECorrectionTypeMinor.CORR_API54D;
										}
										else
										{
											return ECorrectionTypeMinor.CORR_API54D_30;
										}

									default:
										break;
								}

								break;
							}

						default:
							break;
					}
					break;

				case "GB/T":
					switch (commodityOrTable)
					{
						case "60A/59A":
							return ECorrectionTypeMinor.CORR_API60A;
						case "60B/59B":
							return ECorrectionTypeMinor.CORR_API60B;
						case "60D/59D":
							return ECorrectionTypeMinor.CORR_API60D;
						default:
							break;
					}
					break;

				case "GOST":
					return ECorrectionTypeMinor.CORR_3900_85_20C;

				default:
					break;
			}

			return ECorrectionTypeMinor.CORR_NONE;
		}



		public static string GetStandardRevision(ECorrectionTypeMajor correctionTypeMajor, ECorrectionTypeMinor correctionTypeMinor)
		{
			switch (correctionTypeMajor)
			{
				case ECorrectionTypeMajor.CORR_NONE:
					return "1952";
				case ECorrectionTypeMajor.CORR_NONE_1980:
					return "1980";
				case ECorrectionTypeMajor.CORR_API_C:
					return "1952";
				case ECorrectionTypeMajor.CORR_API_F:
					return "1952";
				case ECorrectionTypeMajor.CORR_API_C_1980:
					return "1980";
				case ECorrectionTypeMajor.CORR_API_F_1980:
					return "1980";
				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F:
					return "Polynomial (1952)";
				case ECorrectionTypeMajor.CORR_POLYNOMIAL_F_1980:
					return "Polynomial (1980)";
				case ECorrectionTypeMajor.CORR_LPG_C:
					return "D1250 (1952)";
				case ECorrectionTypeMajor.CORR_LPG_C_1980:
					return "D1250 (1980)";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2004:
					return "D1555 (2004)";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_1980:
					return "D1555 (1980)";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004:
					return "D1555 (2004)";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980:
					return "D1555 (1980)";
				case ECorrectionTypeMajor.CORR_GBT:
					return "1980";
				case ECorrectionTypeMajor.CORR_GOST:
					return "1987";
				case ECorrectionTypeMajor.CORR_ASPHALT:
					switch (correctionTypeMinor)
					{
						case ECorrectionTypeMinor.CORR_D4311DEGC_2004:
							return "D4311 (2004)";
						case ECorrectionTypeMinor.CORR_D4311DEGF_2004:
							return "D4311 (2004)";
						case ECorrectionTypeMinor.CORR_D4311DEGC_2009:
							return "D4311 (2009)";
						case ECorrectionTypeMinor.CORR_D4311DEGF_2009:
							return "D4311 (2009)";
						case ECorrectionTypeMinor.CORR_TABLE7:
							return "IP";
						default:
							;
							return "";
					}
				case ECorrectionTypeMajor.CORR_ASTM_D1250_1952:
					return "D1250 (1952)";
				case ECorrectionTypeMajor.CORR_ASTM_COMM_2004:
					return "Commodity (2004)";
				case ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009:
					return "D1555 (2009)";
				case ECorrectionTypeMajor.CORR_JAPAN_NONE:
					return "1980";
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249:
					return "2249 (1980)";
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2250:
					return "2250 (1967)";
				case ECorrectionTypeMajor.CORR_JAPAN_CHEMICAL:
					return "Chemical";
				case ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE:
					return "2249 (1980) Table";
				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1250:
					return "D1250";
				case ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1555:
					return "D1555";

				default:
					return "None";
			}

		}



		public static string GetCommodityOrTable(ECorrectionTypeMinor correctionTypeMinor)
		{
			switch (correctionTypeMinor)
			{
				case ECorrectionTypeMinor.CORR_NONE:
					return "None";
				case ECorrectionTypeMinor.CORR_API54A:
					return "54A/53A";
				case ECorrectionTypeMinor.CORR_API54B:
					return "54B/53B";
				case ECorrectionTypeMinor.CORR_API54C:
					return "54C";
				case ECorrectionTypeMinor.CORR_API54D:
					return "54D";
				case ECorrectionTypeMinor.CORR_API54A_30:
					return "54A/53A";
				case ECorrectionTypeMinor.CORR_API54B_30:
					return "54B/53B";
				case ECorrectionTypeMinor.CORR_API54C_30:
					return "54C";
				case ECorrectionTypeMinor.CORR_API54D_30:
					return "54D";
				case ECorrectionTypeMinor.CORR_API60A:
					return "60A/59A";
				case ECorrectionTypeMinor.CORR_API60B:
					return "60B/59B";
				case ECorrectionTypeMinor.CORR_API60D:
					return "60D/59D";
				case ECorrectionTypeMinor.CORR_API6A:
					return "6A/5A";
				case ECorrectionTypeMinor.CORR_API6B:
					return "6B/5B";
				case ECorrectionTypeMinor.CORR_API6C:
					return "6C";
				case ECorrectionTypeMinor.CORR_API6D:
					return "6D";
				case ECorrectionTypeMinor.CORR_API24E:
					return "24E/23E";
				case ECorrectionTypeMinor.CORR_POLYNOMIAL:
					return "K Factors";
				case ECorrectionTypeMinor.CORR_LPG:
					return "LPG";
				case ECorrectionTypeMinor.CORR_BENZENE:
					return "Benzene";
				case ECorrectionTypeMinor.CORR_TOLUENE:
					return "Toluene";
				case ECorrectionTypeMinor.CORR_M_XYLENE:
					return "Mixed Xylene";
				case ECorrectionTypeMinor.CORR_STYRENE:
					return "Styrene";
				case ECorrectionTypeMinor.CORR_O_XYLENE:
					return "o-Xylene";
				case ECorrectionTypeMinor.CORR_P_XYLENE:
					return "p-Xylene";
				case ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
					return "Cylco-Hexane";
				case ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
					return "Ethyl-Benzene";
				case ECorrectionTypeMinor.CORR_CUMENE:
					return "Cumene";
				case ECorrectionTypeMinor.CORR_300_AROMATIC:
					return "300 °F/148.9 °C Aromatic";
				case ECorrectionTypeMinor.CORR_350_AROMATIC:
					return "350 °F/176.7 °C Aromatic";
				case ECorrectionTypeMinor.CORR_JIS_TABLE2:
					return "2 (54)";
				case ECorrectionTypeMinor.CORR_ASTM_TABLE55:
					return "55";
				case ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54A:
					return "54A (6X)";
				case ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54B:
					return "54B (6X)";
				case ECorrectionTypeMinor.CORR_ASTM_TABLE2:
					return "2 (54)";
				case ECorrectionTypeMinor.CORR_JIS_CHEMICAL1:
					return "Chemical 1";
				case ECorrectionTypeMinor.CORR_JIS_CHEMICAL2:
					return "Chemical 2";
				case ECorrectionTypeMinor.CORR_API54A_TABLE:
					return "54A/53A";
				case ECorrectionTypeMinor.CORR_API54B_TABLE:
					return "54B/53B";
				case ECorrectionTypeMinor.CORR_API54D_TABLE:
					return "54D/53D";
				case ECorrectionTypeMinor.CORR_APIGBT60A:
					return "60A/59A";
				case ECorrectionTypeMinor.CORR_APIGBT60B:
					return "60B/59B";
				case ECorrectionTypeMinor.CORR_APIGBT60D:
					return "60D/59D";
				case ECorrectionTypeMinor.CORR_3900_85_20C:
					return "3900-85";
				case ECorrectionTypeMinor.CORR_D4311DEGC_2004:
					return "Asphalt";
				case ECorrectionTypeMinor.CORR_D4311DEGF_2004:
					return "Asphalt";
				case ECorrectionTypeMinor.CORR_TABLE7:
					return "Asphalt";
				case ECorrectionTypeMinor.CORR_D4311DEGC_2009:
					return "Asphalt";
				case ECorrectionTypeMinor.CORR_D4311DEGF_2009:
					return "Asphalt";
				case ECorrectionTypeMinor.CORR_D125020DEGC:
					return "LPG";
				case ECorrectionTypeMinor.CORR_ALPHA60_SUPPLIED:
					return "Alpha 60 Supplied";
				case ECorrectionTypeMinor.CORR_CRUDE_OIL:
					return "Crude Oils";
				case ECorrectionTypeMinor.CORR_REFINED_PRODUCTS:
					return "Refined Products";
				case ECorrectionTypeMinor.CORR_LUBRICATION_OIL:
					return "Lubrication Oils";
				default:
					return "None";
			}
		}

		public static string GetStandardTemperature(ECorrectionTypeMajor correctionTypeMajor, ECorrectionTypeMinor correctionTypeMinor)
		{
			switch (correctionTypeMinor)
			{
				case ECorrectionTypeMinor.CORR_API54A:
				case ECorrectionTypeMinor.CORR_API54B:
				case ECorrectionTypeMinor.CORR_API54C:
				case ECorrectionTypeMinor.CORR_API54D:
				case ECorrectionTypeMinor.CORR_API60A:
				case ECorrectionTypeMinor.CORR_API60B:
				case ECorrectionTypeMinor.CORR_API60D:
					return "15 °C";

				case ECorrectionTypeMinor.CORR_API54A_30:
				case ECorrectionTypeMinor.CORR_API54B_30:
				case ECorrectionTypeMinor.CORR_API54C_30:
				case ECorrectionTypeMinor.CORR_API54D_30:
					return "30 °C";

				case ECorrectionTypeMinor.CORR_API6A:
				case ECorrectionTypeMinor.CORR_API6B:
				case ECorrectionTypeMinor.CORR_API6C:
				case ECorrectionTypeMinor.CORR_API6D:
				case ECorrectionTypeMinor.CORR_API24E:
				case ECorrectionTypeMinor.CORR_POLYNOMIAL:
					return "60 °F";

				case ECorrectionTypeMinor.CORR_LPG:
					return "15 °C";

				case ECorrectionTypeMinor.CORR_BENZENE:
				case ECorrectionTypeMinor.CORR_TOLUENE:
				case ECorrectionTypeMinor.CORR_M_XYLENE:
				case ECorrectionTypeMinor.CORR_STYRENE:
				case ECorrectionTypeMinor.CORR_O_XYLENE:
				case ECorrectionTypeMinor.CORR_P_XYLENE:
				case ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
				case ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
				case ECorrectionTypeMinor.CORR_CUMENE:
				case ECorrectionTypeMinor.CORR_300_AROMATIC:
				case ECorrectionTypeMinor.CORR_350_AROMATIC:
					if (correctionTypeMajor == ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980
					|| correctionTypeMajor == ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004
					|| correctionTypeMajor == ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1555)
					{
						return "15 °C";
					}
					else
					{
						return "60 °F";
					}


				case ECorrectionTypeMinor.CORR_JIS_TABLE2:
					return "15 °C";
				case ECorrectionTypeMinor.CORR_ASTM_TABLE55:
					return "15 °C";
				case ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54A:
					return "°C";
				case ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54B:
					return "°C";
				case ECorrectionTypeMinor.CORR_ASTM_TABLE2:
					return "15 °C";
				case ECorrectionTypeMinor.CORR_JIS_CHEMICAL1:
					return "15 °C";
				case ECorrectionTypeMinor.CORR_JIS_CHEMICAL2:
					return "20 °C";

				case ECorrectionTypeMinor.CORR_API54A_TABLE:
				case ECorrectionTypeMinor.CORR_API54B_TABLE:
				case ECorrectionTypeMinor.CORR_API54D_TABLE:
					return "20 °C";

				case ECorrectionTypeMinor.CORR_APIGBT60A:
				case ECorrectionTypeMinor.CORR_APIGBT60B:
				case ECorrectionTypeMinor.CORR_APIGBT60D:
					return "20 °C";

				case ECorrectionTypeMinor.CORR_3900_85_20C:
					return "20 °C";

				case ECorrectionTypeMinor.CORR_D4311DEGC_2004:
					return "15 °C";
				case ECorrectionTypeMinor.CORR_D4311DEGF_2004:
					return "60 °F";
				case ECorrectionTypeMinor.CORR_TABLE7:
					return "15 °C";
				case ECorrectionTypeMinor.CORR_D4311DEGC_2009:
					return "15 °C";
				case ECorrectionTypeMinor.CORR_D4311DEGF_2009:
					return "60 °F";

				case ECorrectionTypeMinor.CORR_D125020DEGC:
					return "20 °C";

				case ECorrectionTypeMinor.CORR_ALPHA60_SUPPLIED:
					return "°F";

				case ECorrectionTypeMinor.CORR_CRUDE_OIL:
				case ECorrectionTypeMinor.CORR_REFINED_PRODUCTS:
				case ECorrectionTypeMinor.CORR_LUBRICATION_OIL:
					return "60 °F";

				default:
					return "";
			}
		}

		public static List<KeyValuePair<string, string>> GetStandardsOrganizations()
		{
			var standardOrganaizationList = new List<KeyValuePair<string, string>>();


			standardOrganaizationList.Add(new KeyValuePair<string, string>("None", "None"));
			standardOrganaizationList.Add(new KeyValuePair<string, string>("API", "API"));
			standardOrganaizationList.Add(new KeyValuePair<string, string>("ASTM", "ASTM"));
			standardOrganaizationList.Add(new KeyValuePair<string, string>("Custom", "Custom"));
			standardOrganaizationList.Add(new KeyValuePair<string, string>("JIS", "JIS"));
			standardOrganaizationList.Add(new KeyValuePair<string, string>("GB/T", "GB/T"));
			standardOrganaizationList.Add(new KeyValuePair<string, string>("GOST", "GOST"));

			return standardOrganaizationList;
		}

		public static List<KeyValuePair<string, string>> GetStandardsAndRevisions(string standardsOrganization)
		{
			var standardAndRevisionList = new List<KeyValuePair<string, string>>();

			switch (standardsOrganization)
			{
				case "None":
					standardAndRevisionList.Add(new KeyValuePair<string, string>("1952", "1952"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("1980", "1980"));
					break;

				case "API":
					standardAndRevisionList.Add(new KeyValuePair<string, string>("1952", "1952"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("1980", "1980"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("Commodity (2004)", "Commodity (2004)"));
					break;

				case "ASTM":
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D1250 (1952)", "D1250 (1952)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D1250 (1980)", "D1250 (1980)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D1555 (1980)", "D1555 (1980)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D1555 (2004)", "D1555 (2004)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D1555 (2009)", "D1555 (2009)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D4311 (2004)", "D4311 (2004)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D4311 (2009)", "D4311 (2009)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("IP", "IP"));
					break;

				case "Custom":
					standardAndRevisionList.Add(new KeyValuePair<string, string>("Polynomial (1952)", "Polynomial (1952)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("Polynomial (1980)", "Polynomial (1980)"));
					break;

				case "JIS":
					standardAndRevisionList.Add(new KeyValuePair<string, string>("None", "None"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D1555", "D1555"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("D1250", "D1250"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("2249 (1980)", "2249 (1980)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("2250 (1967)", "2250 (1967)"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("Chemical", "Chemical"));
					standardAndRevisionList.Add(new KeyValuePair<string, string>("2249 (1980) Table", "2249 (1980) Table"));
					break;

				case "GB/T":
					standardAndRevisionList.Add(new KeyValuePair<string, string>("1980", "1980"));
					break;

				case "GOST":
					standardAndRevisionList.Add(new KeyValuePair<string, string>("1987", "1987"));
					break;


				default:
					break;
			}

			return standardAndRevisionList;
		}

		public static List<KeyValuePair<string, string>> GetCommoditiesOrTables(string standardsOrganization, string standardAndRevision)
		{
			var commodityOrTable = new List<KeyValuePair<string, string>>();

			switch (standardsOrganization)
			{
				case "None":
					commodityOrTable.Add(new KeyValuePair<string, string>("None", "None"));
					break;

				case "API":
					switch (standardAndRevision)
					{
						case "1952":
						case "1980":
						case "2004":
							commodityOrTable.Add(new KeyValuePair<string, string>("54A/53A", "54A/53A"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54B/53B", "54B/53B"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54C", "54C"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54D", "54D"));
							commodityOrTable.Add(new KeyValuePair<string, string>("60A/59A", "60A/59A"));
							commodityOrTable.Add(new KeyValuePair<string, string>("60B/59B", "60B/59B"));
							commodityOrTable.Add(new KeyValuePair<string, string>("60D/59D", "60D/59D"));
							commodityOrTable.Add(new KeyValuePair<string, string>("6A/5A", "6A/5A"));
							commodityOrTable.Add(new KeyValuePair<string, string>("6B/5B", "6B/5B"));
							commodityOrTable.Add(new KeyValuePair<string, string>("6C", "6C"));
							commodityOrTable.Add(new KeyValuePair<string, string>("6D", "6D"));
							if (standardAndRevision == "1980"
								|| standardAndRevision == "2004")
							{
								commodityOrTable.Add(new KeyValuePair<string, string>("24E/23E", "24E/23E"));
							}
							break;
						case "Commodity (2004)":
							commodityOrTable.Add(new KeyValuePair<string, string>("Alpha 60 Supplied", "Alpha 60 Supplied"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Crude Oils", "Crude Oils"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Lubrication Oils", "Lubrication Oils"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Refined Products", "Refined Products"));
							break;

						default:
							break;
					}
					break;

				case "ASTM":
					switch (standardAndRevision)
					{
						case "D1250 (1952)":
						case "D1250 (1980)":
							commodityOrTable.Add(new KeyValuePair<string, string>("LPG", "LPG"));
							break;


						case "D4311 (2004)":
						case "D4311 (2009)":
						case "IP":
							commodityOrTable.Add(new KeyValuePair<string, string>("Asphalt", "Asphalt"));
							break;

						case "D1555 (1980)":
						case "D1555 (2004)":
						case "D1555 (2009)":
							commodityOrTable.Add(new KeyValuePair<string, string>("Benzene", "Benzene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Toluene", "Toluene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Mixed Xylene", "Mixed Xylene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Styrene", "Styrene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("o-Xylene", "o-Xylene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("p-Xylene", "p-Xylene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Cylco-Hexane", "Cylco-Hexane"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Ethyl-Benzene", "Ethyl-Benzene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Cumene", "Cumene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("300 °F/148.9 °C Aromatic", "300 °F/148.9 °C Aromatic"));
							commodityOrTable.Add(new KeyValuePair<string, string>("350 °F /176.7 °C Aromatic", "350 °F /176.7 °C Aromatic"));
							break;

						default:
							break;
					}
					break;

				case "Custom":
					commodityOrTable.Add(new KeyValuePair<string, string>("K-Factors", "K-Factors"));
					break;


				case "JIS":
					switch (standardAndRevision)
					{
						case "None":
							commodityOrTable.Add(new KeyValuePair<string, string>("None", "None"));
							break;

						case "D1555":
							commodityOrTable.Add(new KeyValuePair<string, string>("Benzene", "Benzene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Toluene", "Toluene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Mixed Xylene", "Mixed Xylene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Styrene", "Styrene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("o-Xylene", "o-Xylene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("p-Xylene", "p-Xylene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Cylco-Hexane", "Cylco-Hexane"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Ethyl-Benzene", "Ethyl-Benzene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Cumene", "Cumene"));
							commodityOrTable.Add(new KeyValuePair<string, string>("300 °F/148.9 °C Aromatic", "300 °F/148.9 °C Aromatic"));
							commodityOrTable.Add(new KeyValuePair<string, string>("350 °F/176.7 °C Aromatic", "350 °F/176.7 °C Aromatic"));
							break;

						case "D1250":
							commodityOrTable.Add(new KeyValuePair<string, string>("2 (54)", "2 (54)"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54A (6X)", "54A (6X)"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54B (6X)", "54B (6X)"));
							commodityOrTable.Add(new KeyValuePair<string, string>("55", "55"));
							break;

						case "2249 (1980)":
							commodityOrTable.Add(new KeyValuePair<string, string>("54A/53A", "54A/53A"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54B/53B", "54B/53B"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54C", "54C"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54D", "54D"));
							break;

						case "2250 (1967)":
							commodityOrTable.Add(new KeyValuePair<string, string>("2 (54)", "2 (54)"));
							break;

						case "Chemical":
							commodityOrTable.Add(new KeyValuePair<string, string>("Chemical 1", "Chemical 1"));
							commodityOrTable.Add(new KeyValuePair<string, string>("Chemical 2", "Chemical 2"));
							break;

						case "2249 (1980) Table":
							commodityOrTable.Add(new KeyValuePair<string, string>("54A/53A", "54A/53A"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54B/53B", "54B/53B"));
							commodityOrTable.Add(new KeyValuePair<string, string>("54D", "54D"));
							break;


						default:
							break;
					}
					break;

				case "GB/T":
					commodityOrTable.Add(new KeyValuePair<string, string>("60A/59A", "60A/59A"));
					commodityOrTable.Add(new KeyValuePair<string, string>("60B/59B", "60B/59B"));
					commodityOrTable.Add(new KeyValuePair<string, string>("60D/59D", "60D/59D"));
					break;

				case "GOST":
					commodityOrTable.Add(new KeyValuePair<string, string>("3900-85", "3900-85"));
					break;

				default:
					break;
			}

			return commodityOrTable;
		}

		public static List<KeyValuePair<string, string>> GetStandardTemperatures(string organization, string standardAndRevision, string commodityOrTable)
		{
			var standardTemperatureList = new List<KeyValuePair<string, string>>();

			switch ( commodityOrTable )
			{
				case "None":
					if (organization == "JIS")
					{
						standardTemperatureList.Add(new KeyValuePair<string, string>("15 °C", "15 °C"));
					}
					else
					{
						standardTemperatureList.Add(new KeyValuePair<string, string>("15 °C", "15 °C"));
						standardTemperatureList.Add(new KeyValuePair<string, string>("60 °F", "60 °F"));
					}
					break;

				case "54A/53A":
				case "54B/53B":
				case "54C":
				case "54D":
					if ( standardAndRevision == "2249Z (1980)" )
					{
						standardTemperatureList.Add(new KeyValuePair<string, string>( "20 °C", "20 °C"));
					}
					else
					{
						standardTemperatureList.Add(new KeyValuePair<string, string>( "15 °C", "15 °C"));
						standardTemperatureList.Add(new KeyValuePair<string, string>( "30 °C", "30 °C"));
					}
					break;

				case "60A/59A":
				case "60B/59B":
				case "60D/59D":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "20 °C", "20 °C"));
					break;


				case "6A/5A":
				case "6B/5B":
				case "6C":
				case "6D":
				case "24E/23E":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "60 °F", "60 °F"));
					break;

				case "Alpha 60 Supplied":
				case "Crude Oils":
				case "Lubrication Oils":
				case "Refined Products":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "60 °F", "60 °F"));
					break;


				case "LPG":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "15 °C", "15 °C"));
					break;

				case "Asphalt":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "15 °C", "15 °C"));
					standardTemperatureList.Add(new KeyValuePair<string, string>( "60 °F", "60 °F"));
					break;

				case "Benzene":
				case "Toluene":
				case "Mixed Xylene":
				case "Styrene":
				case "o-Xylene":
				case "p-Xylene":
				case "Cylco-Hexane":
				case "Ethyl-Benzene":
				case "Cumene":
				case "300 °F/148.9 °C Aromatic":
				case "350 °F/176.7 °C Aromatic":
					if (standardAndRevision == "D1555 (2009)")
					{
						standardTemperatureList.Add(new KeyValuePair<string, string>("60 °F", "60 °F"));
					}
					else if(standardAndRevision == "D1555")
					{
						standardTemperatureList.Add(new KeyValuePair<string, string>("15 °C", "15 °C"));
					}
					else
					{
						standardTemperatureList.Add(new KeyValuePair<string, string>("15 °C", "15 °C"));
						standardTemperatureList.Add(new KeyValuePair<string, string>("60 °F", "60 °F"));
					}
					break;

				case "K-Factors":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "°F", "°F"));
					break;

				case "2 (54)":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "15 °C", "15 °C"));
					break;

				case "54A (6X)":
				case "54B (6X)":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "°C", "°C"));
					break;

				case "55":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "15 °C", "15 °C"));
					break;

				case "Chemical 1":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "15 °C", "15 °C"));
					break;

				case "Chemical 2":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "20 °C", "20 °C"));
					break;

				case "3900-85":
					standardTemperatureList.Add(new KeyValuePair<string, string>( "20 °C", "20 °C"));
					break;

				default:
					break;
			}

			return standardTemperatureList;
		}

		public Varec.CommonComponents.VolumeCorrection.VcfModuleSettings GetCommonComponentVcfModuleSettings(EngineeringUnit pressureUnit)
		{
			Varec.CommonComponents.VolumeCorrection.VcfModuleSettings vcfModuleSettings = new Varec.CommonComponents.VolumeCorrection.VcfModuleSettings();

			// The Base Temperature dictates the Temperature Units
			EngineeringUnit temperatureUnit = EngineeringUnit.FmtDegC;
			if(this.BaseTemperature.Value == 60.0)
			{
				temperatureUnit = EngineeringUnit.FmtDegF;
			}


			vcfModuleSettings.Alpha = this.Alpha;
			vcfModuleSettings.AlternateBasePressure = this.AlternateBasePressure.Value;
			vcfModuleSettings.AlternateBasePressureUnit = pressureUnit;
			vcfModuleSettings.AlternateTemperature = this.AlternateTemperature.Value;
			vcfModuleSettings.AlternateTemperatureUnit = temperatureUnit;
			vcfModuleSettings.BaseTemperature = this.BaseTemperature.Value;
			vcfModuleSettings.BaseTemperatureUnit = temperatureUnit;
			vcfModuleSettings.CorrectionMethodSpecific = this.CorrectionMethodSpecific;
			vcfModuleSettings.CorrectionMethodType = this.CorrectionMethodType;
			vcfModuleSettings.DensityPressure = this.DensityPressure.Value;
			vcfModuleSettings.DensityPressureUnit = pressureUnit;
			vcfModuleSettings.ForceVcfTo4Digits = this.ForceVcfTo4Digits;
			vcfModuleSettings.K[0] = this.K[0];
			vcfModuleSettings.K[1] = this.K[1];
			vcfModuleSettings.K[2] = this.K[2];
			vcfModuleSettings.K[3] = this.K[3];
			vcfModuleSettings.K[4] = this.K[4];
			vcfModuleSettings.UseHydrometerCorrection = this.UseHydrometerCorrection;
			vcfModuleSettings.UseProductObservedDensity = this.UseProductObservedDensity;

			return vcfModuleSettings;
		}
	}
}
