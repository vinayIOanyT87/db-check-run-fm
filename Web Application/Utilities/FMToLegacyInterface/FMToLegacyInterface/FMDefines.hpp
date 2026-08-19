// contains the defines for FuelsManager. Add to the list as required just comment it.
#pragma once

// Define FuelsManager Database Point Types

#define	FM_POINT_NONE				0				// Undefined
#define	FM_POINT_IN 				1				// Standard Input Point
#define	FM_POINT_OUT				2				// Standard Output Point
#define	FM_POINT_TANK				3				// Special Tank Point
#define	FM_POINT_SCANNED 			4				// Special Scanned Output Point
#define	FM_POINT_FLOWMETER 		5				// flow meter point
#define	FM_POINT_PIPELINE 		6				// Pipeline point
#define	FM_POINT_LOGIC	 			7				// Pipeline point
#define	FM_POINT_TIMER	 			8				// Pipeline point
#define	FM_POINT_LASTPOINTTYPE		8				// Last fm point type

// define the variables for a tank point in legacy

#define	FMTANK_VARIABLES				6 	// Start of Tank Variables
// added for FM v4.3 

// added for v4.3 SP7 6 - 11
#define	FMTANK_MIN_LEAK					6		// minimum configured leak rate
#define	FMTANK_MAX_LEAK					7		// maximum configured leak rate
#define	FMTANK_LEAK_UNIT				8 		// Leak Rate Engineering Units
#define	FMTANK_MIN_HYDROPRESSURE		9		// minimum configured hydrostatic pressure
#define	FMTANK_MAX_HYDROPRESSURE		10		// maximum configured hydrostatic pressure
#define	FMTANK_HYDRO_PRESSURE_UNIT		11 	// Hydrostatic Pressure Engineering Units

#define	FMTANK_MOVEMENT_ID				12		// ID of Movement Controlling Tank
#define	FMTANK_HYDROMETER_READING		13		// Roof Landed Height
#define	FMTANK_ROOF_LANDED_HEIGHT		14		// Roof Landed Height
#define	FMTANK_LEVEL_ALARM_DATA			15		// Level Alarm & Ack Data
#define	FMTANK_TEMP_ALARM_DATA			16		// Temp Alarm & Ack Data
#define	FMTANK_DENSITY_ALARM_DATA		17		// Density Alarm & Ack Data
#define	FMTANK_FLOW_ALARM_DATA			18		// Flow Alarm & Ack Data
#define	FMTANK_LEVELRATE_ALARM_DATA	19		// Level Rate Alarm & Ack Data
#define	FMTANK_PRESSURE_ALARM_DATA		20		// Pressure Alarm & Ack Data
#define	FMTANK_DIGITAL_ALARM_DATA		21		// Digital Alarm & Ack Data
#define	FMTANK_GAUGE_ALARM_DATA			22		// Gauge Alarm & Ack Data
#define	FMTANK_MODE_ALARM_DATA			23		// Mode Alarm & Ack Data
#define	FMTANK_REM_VOL_NET_TIME			24		// Remaining Volume Net Time
#define	FMTANK_AVAIL_VOL_NET_TIME		25		// Available Volume Net Time
#define	FMTANK_WATER_VOL_TIME			26		// Water Volume Time
#define	FMTANK_SOLID_VOL_TIME			27		// Solids Volume Time
#define	FMTANK_XFRMODE_TIME				28		// Transfer Mode Time
#define	FMTANK_COMMAND_TIME				29		// Tank Mode Command Time
#define	FMTANK_STATUS_TIME				30		// Tank Status Time
#define	FMTANK_XFR_STATUS_TIME			31		// Transfer Status Time
#define	FMTANK_VCF_TIME					32		// VCF Time
#define	FMTANK_STRAP_VOL_TIME			33		// Strap Volume Time
#define	FMTANK_ROOF_VOL_TIME				34		// Roof Volume Time
#define	FMTANK_REM_VOL_GROSS_TIME		35		// Remaining Volume Gross Time
#define	FMTANK_AVAIL_VOL_GROSS_TIME	36		// Available Volume Gross Time
#define	FMTANK_AVAIL_MASS_TIME			37		// Available Mass Time
#define	FMTANK_REM_MASS_TIME				38		// Remaining Mass Time
#define	FMTANK_STRAPVOL_WTRVOL_TIME	39		// Strap Volume minus Water Volume Time
#define	FMTANK_CTSH_VOL_TIME				40		// Strap Volume corrected for Shell Correction Time
#define	FMTANK_NETVOL_CSW_TIME			41		// Net Vol minus correction for BSW Time
#define	FMTANK_CTSH_TIME					42		// Shell Correction Factor Time
#define	FMTANK_CSW_TIME					43		// BSW Correction Factor Time
#define	FMTANK_BSW_VOL_TIME				44		// BSW Volume Time
#define	FMTANK_BOTTOMS_VOL_TIME			45		// Bottoms Volume Time
#define	FMTANK_GAUGE_CMD_TIME			46		// Gauge Command Time
#define	FMTANK_GAUGE_STATUS_TIME		47		// Gauge Status Time
#define	FMTANK_GAUGEALARMS_TIME			48		// Gauge Alarms Time
#define	FMTANK_AMBIENT_TEMP_TIME		49		// Ambient Temperature Time
// original start of Tank Variables 
#define	FMTANK_XFR_ADV 					50 	// Transfer Advisory SetPoint
#define	FMTANK_XFR_SD						51 	// Transfer Shutdown SetPoint	(Based On XFR Mode)
#define	FMTANK_LEVEL						52 	// Level Value
#define	FMTANK_TEMP 						53 	// Temperature
#define	FMTANK_DENSITY 					54 	// Density
#define	FMTANK_LVL_H2O 					55 	// Water Level
#define	FMTANK_BSW							56 	// Bottom Sediment & Water
#define	FMTANK_GR_VOL						57 	// Gross Volume
#define	FMTANK_NET_VOL 					58 	// Net Standard Volume
#define	FMTANK_REM_VOL_NET 				59 	// Remaining Volume (Net)
#define	FMTANK_AVAIL_VOL_NET				60 	// Available Volume (Net)
#define	FMTANK_STD_DENS					61 	// Standard Density
#define	FMTANK_MASS 						62 	// Mass
#define	FMTANK_SOLID_LEVEL				63 	// Solids Level
#define	FMTANK_FLOW 						64 	// Flow rate
#define	FMTANK_VOL_H2O 					65 	// Water Volume
#define	FMTANK_GAUGEPOS					66 	// Tank Gauge Position
#define	FMTANK_DENSITYTEMP				67 	// Tank Gauge Temp for Density
#define	FMTANK_SOLID_VOL					68 	// Solids Volume
#define	FMTANK_XFR_MODE					69 	// Transfer Mode	(Coded Variable)
#define	FMTANK_COMMAND 					70 	// Tank Mode Command
#define	FMTANK_MODE 						71 	// Tank Status
#define	FMTANK_GAUGESTATUS				72 	// Tank Gauge Status (Coded Variable)
#define	FMTANK_GAUGECMND					73 	// Tank Gauge Command (Coded Variable)
#define	FMTANK_LVL_ALARM					74 	// Level Alarm Status (Composite)
#define	FMTANK_TEMP_ALARM 				75 	// Temperature Alarm Status (Composite)
#define	FMTANK_DENSITY_ALARM 			76 	// Density Alarm Status (Composite)
#define	FMTANK_FLOW_ALARM 				77 	// Flow Alarm Status (Composite)
#define	FMTANK_XFR_START					78 	// Transfer Start Time
#define	FMTANK_XFR_TIME					79 	// Transfer Time to Fill or Empty
#define	FMTANK_XFR_TOTAL_VOLUME 		80 	// Current Transferred Volume
#define	FMTANK_LEVEL_TIME 				81 	// Level Value
#define	FMTANK_TEMP_TIME					82 	// Temperature
#define	FMTANK_GR_VOLTIME 				83 	// Gross Volume
#define	FMTANK_NETVOLTIME 				84 	// Net Standard Volume
#define	FMTANK_DENS_TIME					85 	// Density
#define	FMTANK_STDDENTIME 				86 	// Standard Density
#define	FMTANK_MASS_TIME					87 	// Mass
#define	FMTANK_FLOW_TIME					88 	// Flow rate
#define	FMTANK_LVLH2OTIME 				89 	// Water Level
#define	FMTANK_BSW_TIME					90 	// Bottom Sediment & Water
#define	FMTANK_LEVEL_UNIT 				91 	// Level Engineering Units
#define	FMTANK_TEMP_UNIT					92 	// Temperature Engineering Units
#define	FMTANK_VOLUME_UNIT				93 	// Volume Engineering Units
#define	FMTANK_DENSITY_UNIT				94 	// Density Engineering Units
#define	FMTANK_STD_DENS_UNIT 			95 	// Standard Density Engineering Units
#define	FMTANK_MASS_UNIT					96 	// Mass Engineering Units
#define	FMTANK_FLOW_UNIT					97 	// Flow Engineering Units
#define	FMTANK_DIG_ALARM					98 	// Tank Digital Alarms WORD (Composite)
#define	FMTANK_GAUGETYPE					99 	// Tank Gauge Type
#define	FMTANK_XFR_STATUS					100	//	Tank Transfer Status
#define	FMTANK_LEVEL_SP					101	//	Transfer Setpoint Level			
#define	FMTANK_VOLUME_SP					102	//	Transfer Setpoint Final Volume
#define	FMTANK_DIFF_VOL_SP				103	//	Transfer Setpoint Differential Volume
#define	FMTANK_XFR_STOP_TIME				104	//	Transfer Complete Time (time_t)
#define	FMTANK_MODE_ALARM					105   //	Tank Mode Alarm Status
#define	FMTANK_BSW_VOLUME					106   // Gross Volume Due to BSW
#define	FMTANK_BOTTOM_VOL					107   // Combined Bottom Volume (H20 + Solids)
#define	FMTANK_LVL_HIHI					108	//	Current High High Level Limit Setpoint
#define	FMTANK_LVL_HIGH					109	//	Current High Level Limit Setpoint
#define	FMTANK_LVL_LOW						110	//	Current Low Level Limit Setpoint
#define	FMTANK_LVL_LOLO					111	//	Current Low Low Level Limit Setpoint
#define	FMTANK_TEMP_HIHI					112	//	Current High High Temperature Limit Setpoint
#define	FMTANK_TEMP_HIGH					113	//	Current High Temperature Limit Setpoint
#define	FMTANK_TEMP_LOW					114	//	Current Low Temperature Limit Setpoint
#define	FMTANK_TEMP_LOLO					115	//	Current Low Low Temperature Limit Setpoint
#define	FMTANK_DENS_HIGH					116	//	Current High Density Limit Setpoint
#define	FMTANK_DENS_LOW					117	//	Current Low Density Limit Setpoint
#define	FMTANK_FLOW_HIGH					118	//	Current High Flow Limit Setpoint
#define	FMTANK_FLOW_LOW					119	//	Current Low Flow Limit Setpoint
#define	FMTANK_MAX_FILL					120	//	Maximum Allowed Fill Level
#define	FMTANK_MIN_EMPTY					121	//	Minimum Allowed Empty Level
#define	FMTANK_XFR_UNIT					122	// Transfer Shutdown Engineering Units
#define	FMTANK_ADV_UNIT					123	// Transfer Advisory Engineering Units
#define	FMTANK_ETA_HIHI_LVL				124	// Estimated Time of Arrival
#define	FMTANK_ETA_HIGH_LVL				125	// Estimated Time of Arrival
#define	FMTANK_ETA_LOW_LVL				126	// Estimated Time of Arrival
#define	FMTANK_ETA_LOLO_LVL				127	// Estimated Time of Arrival
#define	FMTANK_GAUGE_ALARM				128	// Gauge Alarms WORD (Composite)
#define	FMTANK_DBCONFERROR_ALARM		129	// Database Configuration Error			
#define	FMTANK_TANKCALCERROR_ALARM		130	//	Calculation Error						
#define	FMTANK_STRAPERROR_ALARM			131	//	Strap Table Error						
#define	FMTANK_APICORRERROR_ALARM		132	//	API Correction Error					
#define	FMTANK_CRITICALZONE_ALARM		133	//	Level in Critical Zone				
#define	FMTANK_TANKMOVEMENT_ALARM		134	//	Unauthorized Flow (Level Change)	
#define	FMTANK_REVERSEFLOW_ALARM		135	//	Reverse Product Flow	 				
#define	FMTANK_NOFLOW_ALARM				136	//	No Product Flow						
#define	FMTANK_HIHILEVELETA_ALARM		137	//	HiHi Level ETA							
#define	FMTANK_HIGHLEVELETA_ALARM		138	//	High Level ETA							
#define	FMTANK_LOWLEVELETA_ALARM		139	//	Low Level ETA							
#define	FMTANK_LOLOLEVELETA_ALARM		140	// LoLo Level ETA							
#define	FMTANK_UNUSED12_ALARM 			141	// Bit 12	-								
#define	FMTANK_TANKINTESTMODE_ALARM	142	//	Test									
#define	FMTANK_TRANSFERADVSP_ALARM		143	// Transfer Advisory Alarm				
#define	FMTANK_TRANSFERSDSP_ALARM		144	// Transfer Complete Alarm				
#define	FMTANK_GAUGE00_ALARM				145   // Gauge Alarm 0
#define	FMTANK_GAUGE01_ALARM				146   // Gauge Alarm 1
#define	FMTANK_GAUGE02_ALARM				147   // Gauge Alarm 2
#define	FMTANK_GAUGE03_ALARM				148   // Gauge Alarm 3
#define	FMTANK_GAUGE04_ALARM				149   // Gauge Alarm 4
#define	FMTANK_GAUGE05_ALARM				150   // Gauge Alarm 5
#define	FMTANK_GAUGE06_ALARM				151   // Gauge Alarm 6
#define	FMTANK_GAUGE07_ALARM				152   // Gauge Alarm 7
#define	FMTANK_GAUGE08_ALARM				153   // Gauge Alarm 8
#define	FMTANK_GAUGE09_ALARM				154   // Gauge Alarm 9
#define	FMTANK_GAUGE10_ALARM				155   // Gauge Alarm 10
#define	FMTANK_GAUGE11_ALARM				156   // Gauge Alarm 11
#define	FMTANK_GAUGE12_ALARM				157   // Gauge Alarm 12
#define	FMTANK_GAUGE13_ALARM				158   // Gauge Alarm 13
#define	FMTANK_GAUGE14_ALARM				159   // Gauge Alarm 14
#define	FMTANK_GAUGE15_ALARM				160   // Gauge Alarm 15
#define	FMTANK_VCF							161	// Volume Correction Factor
#define	FMTANK_STRAP_VOLUME				162	// Strap Volume
#define	FMTANK_ROOF_VOLUME				163	// Roof Correction
#define	FMTANK_SOLID_LEVEL_TIME			164	// Solids Level Level Time
#define	FMTANK_GAUGEPOS_TIME				165	// Gauge Position Time
#define	FMTANK_DENSITYTEMP_TIME			166	// Density Temp Time
// added for v4.0	167 thro 196
#define	FMTANK_AVAIL_MASS					167	// Available Mass
#define	FMTANK_REM_MASS					168	// Remaining Mass
#define	FMTANK_AVAIL_VOL_GROSS			169	// Available Vol (Gross)
#define	FMTANK_REM_VOL_GROSS				170	// Remaining Vol (Gross)
#define	FMTANK_STRAPVOL_WTRVOL			171	// Strapping Vol minus Water Vol
#define	FMTANK_CTSHVOL						172	// Strapping Vol corrected for Tank Shell expansion
#define	FMTANK_NET_CSW						173	// Net Volume minus C&W Correction
#define	FMTANK_CTSH							174	// Tank Shell Correction factor
#define	FMTANK_CSW							175	// S&W Correction factor
#define	FMTANK_VAPOR_TEMP					176	// Vapor Temperature
#define	FMTANK_VAPOR_PRESS				177	// Vapor pressure
#define	FMTANK_LEVEL_RATE					178	// Rate of Change of Level
#define	FMTANK_LEVEL_RATE_UNITS			179	// Units for Rate of change of level
#define	FMTANK_VAPOR_PRESS_UNITS		180	// Units for Vapor Pressure
#define	FMTANK_PRESS_HIGH					181	// Current High pressure Setpoint
#define	FMTANK_PRESS_LOW					182	// Current Low pressure Setpoint
#define	FMTANK_LEVELRATE_HIGH			183	// Current Level Rate High Setpoint
#define	FMTANK_LEVELRATE_LOW				184	// Current Level Rate Low Setpoint
#define	FMTANK_VAPORTEMP_TIME			185	// Vapor Temperature Time
#define	FMTANK_VAPORPRESS_TIME			186	// Vapor Pressure Time
#define	FMTANK_LEVELRATE_TIME			187	// Level Rate Time
#define	FMTANK_VAPORPRESS_ALARM			188	// Vapor Pressure Alarm
#define	FMTANK_LEVELRATE_ALARM			189	// Level Rate Alarm
#define	FMTANK_PRODUCTCODE				190	// Tank Product Code
#define	FMTANK_AMBIENT_TEMP				191	// Ambient temp for ctsh
#define	FMTANK_MINLEVEL					192	// minimum configured level
#define	FMTANK_MAXLEVEL					193	// maximum configured level
#define	FMTANK_MINTEMPERATURE			194	// minimum configured temperature
#define	FMTANK_MAXTEMPERATURE			195	// maximum configured temperature
#define	FMTANK_MINDENSITY					196	// minimum configured density
#define	FMTANK_MAXDENSITY					197	// maximum configured density
#define	FMTANK_MINSTDDENSITY				198	// minimum configured standard density
#define	FMTANK_MAXSTDDENSITY				199	// maximum configured standard density
#define	FMTANK_MINVOLUME					200	// minimum configured volume
#define	FMTANK_MAXVOLUME					201	// maximum configured volume
#define	FMTANK_MINFLOW						202	// minimum configured flow
#define	FMTANK_MAXFLOW						203	// maximum configured flow
#define	FMTANK_MINMASS						204	// minimum configured mass
#define	FMTANK_MAXMASS						205	// maximum configured mass
#define	FMTANK_MINLEVELRATE				206	// minimum configured level rate
#define	FMTANK_MAXLEVELRATE				207	// maximum configured level rate
#define	FMTANK_MINPRESSURE				208	// minimum configured pressure
#define	FMTANK_MAXPRESSURE				209	// maximum configured pressure
#define	FMTANK_XFR_REMAINING_VOLUME	210	// transfer remaining volume
#define	FMTANK_NOFLOWHOLDOFF				211	// No flow hold off time in minutes
// added for v4.2 212 - 
#define	FMTANK_CORRECTION_VOLUME		212	// Correction Volume
#define	FMTANK_GAS_DENSITY				213	// Gas Density
#define	FMTANK_MASS_FLOW					214	// Mass Flow
#define	FMTANK_NET_FLOW					215	// Net Flow
#define	FMTANK_PRODUCT_DESC				216	// Product Description
#define	FMTANK_PRODUCT_ALL				217	// Product Name and description
#define	FMTANK_MOVEMENTHOLDOFF			218	// Movement Alarm Hold off time in seconds	
#define	FMTANK_REVERSEFLOWHOLDOFF		219   // Reverse Flow Alarm Hold Off time in seconds
#define	FMTANK_MASSFLOW_UNITS			220	// Units for mass flow
#define	FMTANK_MASSFLOW_TIME				221	// Time for mass flow
#define	FMTANK_MINMASSFLOW				222	// minimum configured mass flow
#define	FMTANK_MAXMASSFLOW				223	// maximum configured mass flow
#define	FMTANK_CORRECTION_VOLUME_TIME	224	// Correction Volume Time
#define	FMTANK_GAS_DENSITY_UNITS		225	// Gas Density Units
#define	FMTANK_GAS_DENSITY_TIME			226	// Gas Density Time
#define	FMTANK_MINGASDENSITY				227	// minimum configured gas density
#define	FMTANK_MAXGASDENSITY				228	// maximum configured gas density
#define	FMTANK_NETFLOW_TIME				229	// net rate time
#define	FMTANK_MASS_SP						230	//	Transfer Setpoint Final Mass
#define	FMTANK_DIFF_MASS_SP				231	//	Transfer Setpoint Differential Mass
#define	FMTANK_XFR_TOTAL_MASS			232	// Current Transferred Mass
#define	FMTANK_XFR_REMAINING_MASS		233	// Transfer Remaining Mass
#define	FMTANK_XFR_TOTAL					234	// Current Transfer Total (Based On Mode)
#define	FMTANK_XFR_REMAINING				235	// Transfer Remaining (Based On Mode)
// added for v4.3 236 - 245
#define	FMTANK_VAPOR_MASS					236	// Vapor Mass
#define	FMTANK_VAPOR_MASS_TIME			237	// Vapor Mass Time
#define	FMTANK_VAPOR_NET_VOLUME			238	// Vapor Net Volume
#define	FMTANK_VAPOR_NET_VOLUME_TIME	239	// Vapor Net Volume Time
#define	FMTANK_LIQUID_MASS				240	// Liquid Mass
#define	FMTANK_LIQUID_MASS_TIME			241	// Liquid Mass Time
#define	FMTANK_TOTAL_NET_VOLUME			242	// Total Net Volume
#define	FMTANK_TOTAL_NET_VOLUME_TIME	243	// Total Net Volume Time
#define	FMTANK_FASTSCAN_COMMAND			244	// FastScan Command
#define	FMTANK_FASTSCAN_COMMAND_TIME	245	// FastScan Command Time
#define	FMTANK_FASTSCAN_STATUS			246	// FastScan Status
#define	FMTANK_FASTSCAN_STATUS_TIME	247	// FastScan Status Time
#define	FMTANK_TANK_GEOMETRY				248	// Tank Geometry
#define	FMTANK_MAJOR_CORRECTION_TYPE	249	// Major CorrectionType
#define	FMTANK_MINOR_CORRECTION_TYPE	250	// Minor Correction Type
#define	FMTANK_STANDARD_TEMP				251	// Standard Temperature
#define	FMTANK_STANDARD_TEMP_UNITS		252	// Standard Temperature Units
#define	FMTANK_ROOF_TYPE					253	// Roof Type
#define	FMTANK_ROOF_MASS					254	// Roof Mass
#define	FMTANK_ROOF_FLOATING_HEIGHT	255	// Roof Floating Height
// added for v4.3 SP7 256 - 257
#define	FMTANK_LEAK_RATE					256	// Leak Rate
#define	FMTANK_HYDRO_PRESSURE			257	// Hydrostatic Pressure (bottom)
// added for v4.3 SP7 258 - 262	(IGO 15-Oct-03)
#define	FMTANK_LINING_MATERIAL			258	// Lining Material
#define	FMTANK_DATE_INSTALLED			259	// Date Installed
#define	FMTANK_CATHODIC_PROTECTION		260	// Cathodic Protection
#define	FMTANK_OVERFILL_PROTECTION		261	// Overfill Protection
#define	FMTANK_SPILL_PROTECTION			262	// Spill Protection
// added for v4.3 SP7 263 - 267	(IGO 23-Oct-03)
#define	FMTANK_TANK_VOLUME				263	// Tank Volume (Tank Characteristics)
#define	FMTANK_TANK_HEIGHT				264	// Tank Height (Tank Characteristics)
#define	FMTANK_TANK_RADIUS				265	// Tank Radius (Tank Characteristics)
#define	FMTANK_TANK_MATERIAL				266	// Tank Material (Tank Characteristics)
#define	FMTANK_HYDRO_PRESSURE_VOL		267	// Volume calculated from Hydrostatic Pressure
// added for v4.3 SP7 268 - 272	(IGO 06-Nov-03)
#define	FMTANK_LEAK_ANALYSIS_METHOD	268	// Leak Analysis Method
#define	FMTANK_LEAK_ANALYSIS_TYPE		269	// Leak Analysis Type
#define	FMTANK_LEAK_AUTO_PRINT			270	// Leak Auto Print
#define	FMTANK_LEAK_PRINT_TIME			271	// Leak Print Time
#define	FMTANK_LEAK_PRINT_DAYS_BEOM	272	// Leak Print Days Before End of Month
// added for v4.3 SP7 273 (IGO 02-Dec-03)
#define	FMTANK_UNROUNDED_NET_VOL		273	// Unrounded Net Volume (Leak Detection)
// added for v4.3 SP7 274 (IGO 04-Dec-03)
#define	FMTANK_LEAK_DATA_ALARM			274	// Leak Data Alarm
#define	FMTANK_HYDRO_PRESSURE_MID		275	// Hydrostatic Pressure (middle)
#define	FMTANK_AIR_DENSITY				276	// Air Density
// added for 7.1 sp1 2004 api calculations
#define	FMTANK_DENSITY_PRESSURE			277	// DENSITY PRESSURE FOR 2004 API CALCS
#define	FMTANK_ALT_TEMPERATURE			278	// 2004 API alternate temperature
// added for tank command scheduler capability
#define	FMTANK_SCH1_COMMAND				279	// scheduler command 1
#define	FMTANK_SCH2_COMMAND				280	// scheduler command 2
#define	FMTANK_SCH3_COMMAND				281	// scheduler command 3
#define	FMTANK_SCH4_COMMAND				282	// scheduler command 4
#define	FMTANK_SCH5_COMMAND				283	// scheduler command 5
#define	FMTANK_SCH6_COMMAND				284	// scheduler command 6
#define	FMTANK_SCH7_COMMAND				285	// scheduler command 7
#define	FMTANK_SCH1_TYPE					286	// scheduler type 1
#define	FMTANK_SCH2_TYPE					287	// scheduler type 2
#define	FMTANK_SCH3_TYPE					288	// scheduler type 3
#define	FMTANK_SCH4_TYPE					289	// scheduler type 4
#define	FMTANK_SCH5_TYPE					290	// scheduler type 5
#define	FMTANK_SCH6_TYPE					291	// scheduler type 6
#define	FMTANK_SCH7_TYPE					292	// scheduler type 7
#define	FMTANK_SCH1_DAY					293	// scheduler day 1
#define	FMTANK_SCH2_DAY					294	// scheduler day 2
#define	FMTANK_SCH3_DAY					295	// scheduler day 3
#define	FMTANK_SCH4_DAY					296	// scheduler day 4
#define	FMTANK_SCH5_DAY					297	// scheduler day 5
#define	FMTANK_SCH6_DAY					298	// scheduler day 6
#define	FMTANK_SCH7_DAY					299	// scheduler day 7
#define	FMTANK_SCH1_TIME					300	// scheduler time 1
#define	FMTANK_SCH2_TIME					301	// scheduler time 2
#define	FMTANK_SCH3_TIME					302	// scheduler time 3
#define	FMTANK_SCH4_TIME					303	// scheduler time 4
#define	FMTANK_SCH5_TIME					304	// scheduler time 5
#define	FMTANK_SCH6_TIME					305	// scheduler time 6
#define	FMTANK_SCH7_TIME					306	// scheduler time 7
#define	FMTANK_SCH1_QUIETTIME			307	// scheduler quiet time 1
#define	FMTANK_SCH2_QUIETTIME			308	// scheduler quiet time 2
#define	FMTANK_SCH3_QUIETTIME			309	// scheduler quiet time 3
#define	FMTANK_SCH4_QUIETTIME			310	// scheduler quiet time 4
#define	FMTANK_SCH5_QUIETTIME			311	// scheduler quiet time 5
#define	FMTANK_SCH6_QUIETTIME			312	// scheduler quiet time 6
#define	FMTANK_SCH7_QUIETTIME			313	// scheduler quiet time 7
#define	FMTANK_COMMENT						314	// Tank Comment Field
#define	FMTANK_OPER_ALRM_ENABLE			315	// Operator Alarm Enable
#define	FMTANK_OPER_ALRM_SETPT			316	// Operator Alarm Set Point
#define	FMTANK_GAUGE_DENSITY				317	// Std density from servo gauge after a profile
#define	FMTANK_GAUGE_DENSITY_TIME		318	// Std density from servo gauge after a profile
#define	FMTANK_OPERATIONAL_MODE			319	// Tanks current operational mode

// alarm comment variables
#define	FMTANK_ALARM_COMMENT_LVL_HIGHHIGH	320
#define	FMTANK_ALARM_COMMENT_LVL_HIGH			321
#define	FMTANK_ALARM_COMMENT_LVL_LOW			322
#define	FMTANK_ALARM_COMMENT_LVL_LOWLOW		323
#define	FMTANK_ALARM_COMMENT_ADV_HIGH			324
#define	FMTANK_ALARM_COMMENT_ADV_LOW			325
#define	FMTANK_ALARM_COMMENT_OPERERATOR		326
#define	FMTANK_ALARM_COMMENT_MODE				327
#define	FMTANK_ALARM_COMMENT_TMP_HIGHHIGH	328
#define	FMTANK_ALARM_COMMENT_TMP_HIGH			329
#define	FMTANK_ALARM_COMMENT_TMP_LOW			330
#define	FMTANK_ALARM_COMMENT_TMP_LOWLOW		331
#define	FMTANK_ALARM_COMMENT_DENSITY_HIGH	332
#define	FMTANK_ALARM_COMMENT_DENSITY_LOW		333
#define	FMTANK_ALARM_COMMENT_FLOW_HIGH		334
#define	FMTANK_ALARM_COMMENT_FLOW_LOW			335
#define	FMTANK_ALARM_COMMENT_LVLRATE_HIGH	336
#define	FMTANK_ALARM_COMMENT_LVLRATE_LOW		337
#define	FMTANK_ALARM_COMMENT_VPRESS_HIGH		338
#define	FMTANK_ALARM_COMMENT_VPRESS_LOW		339
#define FMTANK_ALARM_LVL_DISABLE					340
#define FMTANK_ALARM_LVL_DISABLE_TIMELEFT		341

//added for tacfuels
#define	FMTANK_GROSSVOL_ALARM_DATA				342		// Alarm & Ack Data
#define	FMTANK_GROSSVOL_HIHI						343	//	Current High High Limit Setpoint
#define	FMTANK_GROSSVOL_HIGH						344	//	Current High Limit Setpoint
#define	FMTANK_GROSSVOL_LOW						345	//	Current Low Limit Setpoint
#define	FMTANK_GROSSVOL_LOLO						346	//	Current Low Low Limit Setpoint
#define	FMTANK_NETVOL_ALARM_DATA				347		// Alarm & Ack Data
#define	FMTANK_NETVOL_HIHI						348	//	Current High High Limit Setpoint
#define	FMTANK_NETVOL_HIGH						349	//	Current High Limit Setpoint
#define	FMTANK_NETVOL_LOW							350	//	Current Low Limit Setpoint
#define	FMTANK_NETVOL_LOLO						351	//	Current Low Low Limit Setpoint
#define	FMTANK_MASS_ALARM_DATA					352		// Alarm & Ack Data
#define	FMTANK_MASS_HIHI							353	//	Current High High Limit Setpoint
#define	FMTANK_MASS_HIGH							354	//	Current High Limit Setpoint
#define	FMTANK_MASS_LOW							355	//	Current Low Limit Setpoint
#define	FMTANK_MASS_LOLO							356	//	Current Low Low Limit Setpoint
#define	FMTANK_GROSSVOL_ALARM					357	//  Alarm Status (Composite)
#define	FMTANK_NETVOL_ALARM						358	//  Alarm Status (Composite)
#define	FMTANK_MASS_ALARM							359	//  Alarm Status (Composite)

// new variables for black friars
#define	FMTANK_CST_MANUFACTURER					360	//  
#define	FMTANK_CST_MFG_DATE						361	//  
#define	FMTANK_CST_WET_DATE						362	//  
#define	FMTANK_CST_MFG_VOLUME					363	//  
#define	FMTANK_CST_SERIAL_NUMBER				364	//  
#define	FMTANK_CST_LOCATION						365	//  
#define	FMTANK_CST_LONGITUDE						366	//  
#define	FMTANK_CST_LATITUDE						367	//

//new variable for API 2012 Standard			
#define	FMTANK_GROSS_STANDARD_VOL				368 //	GROSS STANDARD VOLUME (GOV * VCF)
#define	FMTANK_TOTAL_CALCULATED_VOL			369 //  TOTAL CALCULATED VOLUME (GSV + FW)
#define	FMTANK_GROSS_STANDARD_VOLTIME			370 //
#define	FMTANK_TOTAL_CALCULATED_VOLTIME		371 //
#define	FMTANK_DENSITY_PROD_INAIR				372
#define	FMTANK_DENSITY_PROD_INAIR_TIME		373
#define	FMTANK_GROSS_STD_WEIGHT					374
#define	FMTANK_GROSS_STD_WEIGHT_TIME			375
#define	FMTANK_NET_STD_WEIGHT					376
#define	FMTANK_NET_STD_WEIGHT_TIME				377
#define	FMTANK_STDDENSITY_PROD_INAIR			378
#define	FMTANK_STDDENSITY_PROD_INAIR_TIME	379

#define	FMTANK_LAST_TANK_VARIABLE				379	// Last Tank Variable Name

// Define Database Data Formats

#define		DSA_NONE							0	// Not Defined
#define		DSA_CHAR							1	// Signed Char 8 bit (-128 -> 127)
#define		DSA_BYTE							2	// Unsign Char 8 bit (0 -> 255)
#define		DSA_SHORT						3	// signed 16 bit (-32768 -> 32767 )
#define		DSA_WORD							4	// unsign 16 bit ( 0 -> 65535)
#define		DSA_CODED						5	// Coded Variable unsigned 16 bit
#define		DSA_LONG							6	// signed 32 bit
#define		DSA_DWORD						7	// unsign 32 bit
#define		DSA_FLOAT						8	// Floating Point (  ) 4 bytes
#define		DSA_DOUBLE						9	// Floating Point Double - 8 bytes
#define		DSA_TDATE						10	// Time & Date - FMTIMEDATA - 6 bytes
#define		DSA_DATE							11	// Date Only - same
#define		DSA_TIME							12	// Time Only - same
#define		DSA_ALARM						20	// Composite Alarm Status - ALARMSTAT Struct
#define		DSA_UNITS						21	// Variable Engineering Units -	WORD index to Units
#define		DSA_DESC							22	// Point Description String
#define		DSA_POINT_TAG					23	// PointName Structure
#define		DSA_PRODUCT_ALL				24	// Product name
#define		DSA_PRODUCT_DESC				25	// Product Description
#define		DSA_TIME_T						26	// Standard time_t format
#define		DSA_DIFFTIME					27	// Elapsed Time in Seconds
#define		DSA_PRODUCTCODE				28	// Product Code - WORD
#define		DSA_POINT						29	// Database Point reference \\System\Tag\Variable
#define		DSA_USERDEF0					30	// User Defined String
#define		DSA_USERDEF1					31	// User Defined String
#define		DSA_USERDEF2					32	// User Defined String
#define		DSA_USERDEF3					33	// User Defined String
#define		DSA_USERDEF4					34	// User Defined String
#define		DSA_USERDEF5					35	// User Defined String
#define		DSA_USERDEF6					36	// User Defined String
#define		DSA_USERDEF7					37	// User Defined String
#define		DSA_USERDEF8					38	// User Defined String
#define		DSA_USERDEF9					39	// User Defined String
#define		DSA_USERDEF10					40	// User Defined String
#define		DSA_USERDEF11					41	// User Defined String
#define		DSA_USERDEF12					42	// User Defined String
#define		DSA_USERDEF13					43	// User Defined String
#define		DSA_USERDEF14					44	// User Defined String
#define		DSA_USERDEF15					45	// User Defined String
#define		DSA_GAUGETYPE					46	// String Object for Gauge Type Strings
#define		DSA_MAJOR_CORRECTION_TYPE		47	// Byte converted to String 
#define		DSA_MINOR_CORRECTION_TYPE		48	// Word Upper/Lower Byte Major/Minor Type converted to String
#define		DSA_TANK_GEOMETRY				49 // Word Value converted to String
#define		DSA_ROOF_TYPE					50	// Byte Value converted to String
#define		DSA_LINING_MATERIAL				51	// Lining Material String
#define		DSA_SCHEDULE_STRING				52	// Scheduling Parameter String
#define		DSA_STRING						53	// String data type used for tank comment

#define		FM_ERROR_NONE					0x20000000	// No Error
#define		FM_ERROR_CANCEL					0xE0000005	// User Canceled
#define		MAX_USERNAME_LENGTH				20
#define		FM_ERROR_UNAVAIL				0xE0000015	// Not Available
#define		FM_ERROR_RPC					0xE0000004	// RPC Function Error
#define		FM_ERROR_NOTFOUND				0xE0000029	// Item Not Found
