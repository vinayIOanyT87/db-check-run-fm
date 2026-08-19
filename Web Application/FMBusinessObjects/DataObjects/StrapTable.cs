namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;
	using CodedVariables;

	using FMBusinessObjects.Attributes;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class StrapTableLevelComparer : IComparer<StrapTableEntry>
	{
		public int Compare(StrapTableEntry x, StrapTableEntry y)
		{
			if (x == null)
			{
				if (y == null)
				{
					// If x is null and y is null, they're
					// equal. 
					return 0;
				}
				else
				{
					// If x is null and y is not null, y
					// is greater. 
					return -1;
				}
			}
			else
			{
				// If x is not null...
				//
				if (y == null)
				// ...and y is null, x is greater.
				{
					return 1;
				}
				else
				{
					// ...and y is not null, compare the 
					// levels.
					//
					if (x.Level < y.Level)
					{
						return -1;
					}
					else if (x.Level > y.Level)
					{
						return 1;
					}
					return 0;
				}
			}
		}
	}

	public class StrapTableVolumeComparer : IComparer<StrapTableEntry>
	{
		public int Compare(StrapTableEntry x, StrapTableEntry y)
		{
			if (x == null)
			{
				if (y == null)
				{
					// If x is null and y is null, they're
					// equal. 
					return 0;
				}
				else
				{
					// If x is null and y is not null, y
					// is greater. 
					return -1;
				}
			}
			else
			{
				// If x is not null...
				//
				if (y == null)
				// ...and y is null, x is greater.
				{
					return 1;
				}
				else
				{
					// ...and y is not null, compare the 
					// volumes.
					//
					if (x.Volume < y.Volume)
					{
						return -1;
					}
					else if (x.Volume > y.Volume)
					{
						return 1;
					}
					return 0;
				}
			}
		}
	}



	[DataContract(Namespace = "")]
	[Serializable()]
	public class StrapTableEntry
	{
		public StrapTableEntry()
		{
			this.Level = 0;
			this.Volume = 0;
		}

		public StrapTableEntry(double level, double volume)
		{
			this.Level = level;
			this.Volume = volume;
		}

		[DataMember]
		public double Level { get; set; }

		[DataMember]
		public double Volume { get; set; }
	}

	[DataContract(Namespace = "")]
	[Serializable()]
	public class IndividualStrapTable
	{
		[DataMember(Order = 0)]
		public List<StrapTableEntry> table { get; set; }

		[DataMember(Order = 1)]
		public string StrapTableDescription { get; set; }

		[DataMember(Order = 2)]
		public PointPropertyUnitTypedDouble StrapDensity { get; set; }

		[DataMember(Order = 3)]
		public PointPropertyUnitTypedDouble StrapTemperature { get; set; }

		[DataMember(Order = 4)]
		public PointPropertyUnitTypedDouble TankShellReferenceTemperature { get; set; }
		
		[DataMember(Order = 5)]
		public PointPropertyUnitTypedDouble RoofMass { get; set; }

		[DataMember(Order = 6)]
		public RoofTypeEnum RoofType { get; set; }

		[DataMember(Order = 7)]
		public PointPropertyUnitTypedDouble RoofLandingHeight { get; set; }

		[DataMember(Order = 8)]
		public PointPropertyUnitTypedDouble RoofFloatingHeight { get; set; }

		[DataMember(Order = 9)]
		public PointPropertyUnitTypedDouble DatumHeight { get; set; }

		public void SortByLevel()
		{
			this.table.Sort((x, y) => x.Level.CompareTo(y.Level));
		}


		public IndividualStrapTable()
		{
			this.RoofMass = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuMass);
			this.RoofType = RoofTypeEnum.NoRoof;
			this.RoofFloatingHeight = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuLength);
			this.RoofLandingHeight = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuLength);
			this.DatumHeight = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuLength);
			this.table = new List<StrapTableEntry>();
			this.StrapDensity = new PointPropertyUnitTypedDouble(60.0, EngineeringUnitType.FmuDensity);
			this.StrapTemperature = new PointPropertyUnitTypedDouble(60.0, EngineeringUnitType.FmuTemp);
			this.TankShellReferenceTemperature = new PointPropertyUnitTypedDouble(60, EngineeringUnitType.FmuTemp);
		}
    }

	/// <summary>
	/// ClassName: StrapTable
	/// Purpose: Store and Manage the Tank Strap Table Information
	/// 
	/// Modifications:
	/// 
	/// 5-27-2016: Eric Simmons
	/// Modified Class Definition from inheriting from List<> collection and added List<> collection as DataMember
	/// attribute called Table.  This is because if StrapTable inherits from List<> then no other attributes can be added
	/// to StrapTable class which must be serialized.
	/// 
	/// See the following link for details:
	/// http://stackoverflow.com/questions/666054/c-sharp-inheriting-generic-collection-and-serialization
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable()]
	public class StrapTable
	{
		private List<string> GetAllTables()
		{
			var tableList = new List<string>();

			foreach(var table in StrapTables)
			{
				tableList.Add(table.StrapTableDescription);
			}

			return tableList;
		}

		[FMExposedSetting("Product Table")]
		[XmlIgnore]
		public string ProductTable
		{
			get
			{
				return StrapTables[SelectedTableForStrap].StrapTableDescription;
			}
			set
			{
				for (int index = 0; index < StrapTables.Length; index++)
				{
					if (value == StrapTables[index].StrapTableDescription)
					{
						SelectedTableForStrap = index;
						return;
					}
				}

				throw new Exception("Invalid Product Table");
			}
		}

		[FMExposedSetting("Bottoms Table")]
		[XmlIgnore]
		public string BottomsTable
		{
			get
			{
				return StrapTables[SelectedTableForWaterVolume].StrapTableDescription;
			}
			set
			{
				for (int index = 0; index < StrapTables.Length; index++)
				{
					if (value == StrapTables[index].StrapTableDescription)
					{
						SelectedTableForWaterVolume = index;
						return;
					}
				}

				throw new Exception("Invalid Bottoms Table");
			}
		}

		[FMExposedSetting("Solids Table")]
		[XmlIgnore]
		public string SolidsTable {
			get
			{
				return StrapTables[SelectedTableForSolidsVolume].StrapTableDescription;
			}
			set
			{
				for (int index = 0; index < StrapTables.Length; index++)
				{
					if (value == StrapTables[index].StrapTableDescription)
					{
						SelectedTableForSolidsVolume = index;
						return;
					}
				}

				throw new Exception("Invalid Solids Table");
			}
		}


		[DataMember(Order = 0)]
		public int SelectedTableForStrap { get; set; }
		[DataMember(Order = 1)]
		public int SelectedTableForWaterVolume { get; set; }
		[DataMember(Order = 2)]
		public int SelectedTableForSolidsVolume { get; set; }
		[DataMember(Order = 3)]
		public IndividualStrapTable[] StrapTables { get; set; }

		[XmlIgnore]
		public bool StrapInRange
		{
			get
			{
				return (SelectedTableForStrap >= 0 && SelectedTableForStrap < 6 && SelectedTableForStrap < StrapTables.Length) ? true : false;
			}
		}

		[FMExposedSetting("Strap Density")]
		[XmlIgnore]
		public PointPropertyUnitTypedDouble StrapDensity
		{
			get
			{
				return (StrapInRange) ? StrapTables[SelectedTableForStrap].StrapDensity : null;
			}
			set
			{
				if(StrapInRange)
				{
					StrapTables[SelectedTableForStrap].StrapDensity = value;
				}
			}
		}

		[FMExposedSetting("Strap Temperature")]
		[XmlIgnore]
		public PointPropertyUnitTypedDouble StrapTemperature
		{
			get
			{
				return (StrapInRange) ? StrapTables[SelectedTableForStrap].StrapTemperature : null;
			}
			set
			{
				if (StrapInRange)
				{
					StrapTables[SelectedTableForStrap].StrapTemperature = value;
				}
			}
		}

		[XmlIgnore]
		public PointPropertyUnitTypedDouble TankShellReferenceTemperature
		{
			get
			{
				return (StrapInRange) ? StrapTables[SelectedTableForStrap].TankShellReferenceTemperature : null;
			}
			set
			{
				if (StrapInRange)
				{
					StrapTables[SelectedTableForStrap].TankShellReferenceTemperature = value;
				}
			}
		}

		[FMExposedSetting("Roof Type")]
		[XmlIgnore]
		public RoofTypeEnum RoofType
		{
			get
			{
				return (StrapInRange) ? StrapTables[SelectedTableForStrap].RoofType : RoofTypeEnum.NoRoof;
			}
			set
			{
				if (StrapInRange)
				{
					StrapTables[SelectedTableForStrap].RoofType = value;
				}
			}
		}



      [FMExposedSetting("Roof Mass", ModifyDisabled = true)]
      [XmlIgnore]
		public PointPropertyUnitTypedDouble RoofMass
		{
			get
			{
				return (StrapInRange) ? StrapTables[SelectedTableForStrap].RoofMass : null;
			}
			set
			{
				if (StrapInRange)
				{
					StrapTables[SelectedTableForStrap].RoofMass = value;
				}
			}
		}


		[FMExposedSetting("Roof Landing Height")]
		[XmlIgnore]
		public PointPropertyUnitTypedDouble RoofLandingHeight
		{
			get
			{
				return (StrapInRange) ? StrapTables[SelectedTableForStrap].RoofLandingHeight : null;
			}
			set
			{
				if (StrapInRange)
				{
					StrapTables[SelectedTableForStrap].RoofLandingHeight = value;
				}
			}
		}

		[FMExposedSetting("Roof Floating Height")]
		[XmlIgnore]
		public PointPropertyUnitTypedDouble RoofFloatingHeight
		{
			get
			{
				return (StrapInRange) ? StrapTables[SelectedTableForStrap].RoofFloatingHeight : null;
			}
			set
			{
				if (StrapInRange)
				{
					StrapTables[SelectedTableForStrap].RoofFloatingHeight = value;
				}
			}
		}

		[FMExposedSetting("Datum Height")]
		[XmlIgnore]
		public PointPropertyUnitTypedDouble DatumHeight
		{
			get
			{
				return (StrapInRange) ? StrapTables[SelectedTableForStrap].DatumHeight : null;
			}
			set
			{
				if (StrapInRange)
				{
					StrapTables[SelectedTableForStrap].DatumHeight = value;
				}
			}
		}

		public StrapTable()
		{
			var strapTableList = new List<IndividualStrapTable>();
			strapTableList.Add(new IndividualStrapTable() { StrapTableDescription = "Strap Table 1" });
			strapTableList[0].table.Add(new StrapTableEntry(0.0, 0.0));
			strapTableList[0].table.Add(new StrapTableEntry(10.0, 2500.0));
			strapTableList[0].table.Add(new StrapTableEntry(20.0, 5000.0));
			strapTableList[0].table.Add(new StrapTableEntry(40.0, 10000.0));

			this.StrapTables = strapTableList.ToArray();
		}

		public double? GetVolumeFromLevel(double level, int SelectedStrapTable,TankGeometryEnum TankGeometry)
		{
			double? ReturnedValue = 0.0;

			if (SelectedStrapTable < 0 || 
				SelectedStrapTable > 5 ||
				this.StrapTables[SelectedStrapTable].table.Count == 0)
			{
				return null;
			}
			// get the volume from the strap based on the tank geometry
			switch(TankGeometry)
			{
				case TankGeometryEnum.HorizontalCylinderWithEndCaps:
				case TankGeometryEnum.HorizontalCylinderWithFlatEnds:
				case TankGeometryEnum.UnderGroundWithEndCaps:
				case TankGeometryEnum.UnderGroundWithFlatEnds:
					{
						// these require a minimum of 3 strap table entries
						if (this.StrapTables[SelectedStrapTable].table.Count < 3)
						{
							return null;
						}
						ReturnedValue = GetVolumeFromSelectedStrapHorizontalTank(level, this.StrapTables[SelectedStrapTable].table);

						break;
					}
				case TankGeometryEnum.StandardSphere:
					{
						// this requires a minimum of 4 strap table entries
						if (this.StrapTables[SelectedStrapTable].table.Count < 4)
						{
							return null;
						}
						ReturnedValue = GetVolumeFromSelectedStrapStandardSphere(level, this.StrapTables[SelectedStrapTable].table);

						break;
					}
				default:
					ReturnedValue = GetVolumeFromSelectedStrap(level, this.StrapTables[SelectedStrapTable].table);
					break;
			}

			return ReturnedValue;

		}

		public double? GetLevelFromVolume(double volume, int SelectedStrapTable, TankGeometryEnum TankGeometry)
		{
			double ReturnedValue = 0.0;

			if (SelectedStrapTable < 0 ||
				SelectedStrapTable > 5 ||
				this.StrapTables[SelectedStrapTable].table.Count == 0)
			{
				return null;
			}

			switch (TankGeometry)
			{
				case TankGeometryEnum.HorizontalCylinderWithEndCaps:
				case TankGeometryEnum.HorizontalCylinderWithFlatEnds:
				case TankGeometryEnum.UnderGroundWithEndCaps:
				case TankGeometryEnum.UnderGroundWithFlatEnds:
					{
						// these require a minimum of 3 strap table entries
						if (this.StrapTables[SelectedStrapTable].table.Count < 3)
						{
							return null;
						}
						ReturnedValue = GetLevelFromSelectedStrapHorizontalTank(volume, this.StrapTables[SelectedStrapTable].table);
						break;
					}
				case TankGeometryEnum.StandardSphere:
					{
						// this requires a minimum of 4 strap table entries
						if (this.StrapTables[SelectedStrapTable].table.Count < 4)
						{
							return null;
						}
						ReturnedValue = GetLevelFromSelectedStrapStandardSphere(volume, this.StrapTables[SelectedStrapTable].table);
						break;
					}
				default:
					ReturnedValue = GetLevelFromSelectedStrap(volume, this.StrapTables[SelectedStrapTable].table);
					break;
			}

			return ReturnedValue;

		}


		public double? GetVolumeFromSelectedStrap(double level, List<StrapTableEntry> StrapTable)
		{
			var comparer = new StrapTableLevelComparer();

			var index = StrapTable.BinarySearch(new StrapTableEntry(level, -999), comparer);

			if (index >= 0) // if we find a match
			{
				return StrapTable[index].Volume;
			}
			else if (index == -1) // if below the minimun value
			{
				return null;
			}
			else if (-index - 1 == StrapTable.Count) //if above the maximum value
			{
				return null;
			}

			var firstEntry = StrapTable[-index - 2];
			var nextEntry = StrapTable[-index - 1];

			var percentage = (level - firstEntry.Level) / (nextEntry.Level - firstEntry.Level);
			return firstEntry.Volume + (nextEntry.Volume - firstEntry.Volume) * percentage;

		}

		public double GetLevelFromSelectedStrapStandardSphere(double volume, List<StrapTableEntry> StrapTable)
		{
			double dReturnValue = 0.0;
			double returnedVolume = 0.0;
			double level = 0.0;
			int maximumValue = StrapTable.Count - 1;
			int minimumValue = 0;
			bool strapFound = false;
			bool incrementLevel = false;
			int selectedPoint = 0;
			double testValue = 0.0;
/*			double HighestVolume = 0.0;
			double HighestLevel = 0.0;
			double HighVolume = 0.0;
			double HighLevel = 0.0;
			double LowVolume = 0.0;
			double LowLevel = 0.0;
			double LowestVolume = 0.0;
			double LowestLevel = 0.0;
			double Top1 = 0.0;
			double Top2 = 0.0;
			double Top3 = 0.0;
			double Top4 = 0.0;
			double Bottom1 = 0.0;
			double Bottom2 = 0.0;
			double Bottom3 = 0.0;
			double Bottom4 = 0.0;
*/			StrapTableEntry localStrapEntry = null;

			if (volume < StrapTable[0].Volume)
			{
				return StrapTable[0].Level;
			}
			if (volume > StrapTable[maximumValue].Volume)
			{
				return (StrapTable[maximumValue].Level);
			}

			minimumValue = 0;
			strapFound = false;
			while (minimumValue <= maximumValue)
			{
				selectedPoint = (minimumValue + maximumValue) >> 1;

				testValue = StrapTable[selectedPoint].Volume;

				if (testValue == volume)
				{
					strapFound = true;
					break;
				}

				if (volume < testValue)
				{
					if (selectedPoint == 0)
						break;

					maximumValue = selectedPoint - 1;
				}
				else
				{
					minimumValue = selectedPoint + 1;
					if (minimumValue <= 0)
						break;
				}
			}

			if (strapFound)
			{
				return StrapTable[selectedPoint].Level;
			}

			localStrapEntry = StrapTable[selectedPoint];

			if (selectedPoint == (int)(StrapTable.Count - 1))
			{
				return localStrapEntry.Level;
			}

			// determine which way the volume needs to go
			incrementLevel = false;

			if(volume > localStrapEntry.Volume)
			{
				incrementLevel = true;
			}

			strapFound = false;

			level = localStrapEntry.Level;

			while (!strapFound)
			{
				if (incrementLevel)
					level += .01;
				else
					level -= .01;
				returnedVolume = GetVolumeFromSelectedStrapStandardSphere(level, StrapTable);

				if(incrementLevel)
				{
					if (returnedVolume >= volume)
						return level;
				}
				else
				{
					if (returnedVolume <= volume)
						return level;
				}

			}



			/*
			if (selectedPoint < (int)(StrapTable.Count - 1))
			{
				selectedPoint += 1;
				localStrapEntry = StrapTable[selectedPoint];

				// if we are at the bottom of the tank then move 
				// up another place
				if (selectedPoint < 3)
				{
					selectedPoint += 1;
					localStrapEntry = StrapTable[selectedPoint];
				}
			}

			HighestVolume = localStrapEntry.Volume;
			HighestLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			if (selectedPoint < 0)
				selectedPoint = StrapTable.Count - 1;
			localStrapEntry = StrapTable[selectedPoint];
			HighVolume = localStrapEntry.Volume;
			HighLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			if (selectedPoint < 0)
				selectedPoint = StrapTable.Count - 1;
			localStrapEntry = StrapTable[selectedPoint];
			LowVolume = localStrapEntry.Volume;
			LowLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			if (selectedPoint < 0)
				selectedPoint = StrapTable.Count - 1;
			localStrapEntry = StrapTable[selectedPoint];
			LowestVolume = localStrapEntry.Volume;
			LowestLevel = localStrapEntry.Level;

			Top1 = LowestLevel * ((volume - LowVolume) / 100.0f)
				 * ((volume - HighVolume) / 100.0f) * ((volume - HighestVolume) / 100.0f);
			Top2 = LowLevel * ((volume - LowestVolume) / 100.0f)
				 * ((volume - HighVolume) / 100.0f) * ((volume - HighestVolume) / 100.0f);
			Top3 = HighLevel * ((volume - LowestVolume) / 100.0f)
				 * ((volume - LowVolume) / 100.0f) * ((volume - HighestVolume) / 100.0f);
			Top4 = HighestLevel * ((volume - LowestVolume) / 100.0f)
				  * ((volume - LowVolume) / 100.0f) * ((volume - HighVolume) / 100.0f);
			Bottom1 = ((LowestVolume - LowVolume) / 100.0f)
				  * ((LowestVolume - HighVolume) / 100.0f)
				 * ((LowestVolume - HighestVolume) / 100.0f);
			Bottom2 = ((LowVolume - LowestVolume) / 100.0f)
				 * ((LowVolume - HighVolume) / 100.0f)
				 * ((LowVolume - HighestVolume) / 100.0f);
			Bottom3 = ((HighVolume - LowestVolume) / 100.0f)
				 * ((HighVolume - LowVolume) / 100.0f)
				 * ((HighVolume - HighestVolume) / 100.0f);
			Bottom4 = ((HighestVolume - LowestVolume) / 100.0f)
				 * ((HighestVolume - LowVolume) / 100.0f)
				 * ((HighestVolume - HighVolume) / 100.0f);

			dReturnValue = (double)(Top1 / Bottom1 + Top2 / Bottom2 +
					   Top3 / Bottom3 + Top4 / Bottom4);

	*/
			return dReturnValue;
		}

		public double GetLevelFromSelectedStrapHorizontalTank(double volume, List<StrapTableEntry> StrapTable)
		{
			double dReturnValue = 0.0;
			double returnedVolume = 0.0;
			double level = 0.0;
			int maximumValue = StrapTable.Count - 1;
			int minimumValue = 0;
			bool strapFound = false;
			bool incrementLevel = false;
			int selectedPoint = 0;
			double testValue = 0.0;
/*			double HighestVolume = 0.0;
			double HighestLevel = 0.0;
			double HighVolume = 0.0;
			double HighLevel = 0.0;
			double LowVolume = 0.0;
			double LowLevel = 0.0;
			double VolumeRatio = 0.0;
			double CalcLevel1 = 0.0;
			double CalcLevel2 = 0.0;
			double CalcLevel3 = 0.0;
*/			StrapTableEntry localStrapEntry = null;

			if (volume < StrapTable[0].Volume)
			{
				return StrapTable[0].Level;
			}
			if (volume > StrapTable[maximumValue].Volume)
			{
				return (StrapTable[maximumValue].Level);
			}

			minimumValue = 0;
			strapFound = false;
			while (minimumValue <= maximumValue)
			{
				selectedPoint = (minimumValue + maximumValue) >> 1;

				testValue = StrapTable[selectedPoint].Volume;

				if (testValue == volume)
				{
					strapFound = true;
					break;
				}

				if (volume < testValue)
				{
					if (selectedPoint == 0)
						break;

					maximumValue = selectedPoint - 1;
				}
				else
				{
					minimumValue = selectedPoint + 1;
					if (minimumValue <= 0)
						break;
				}
			}

			if (strapFound)
			{
				return StrapTable[selectedPoint].Level;
			}
			localStrapEntry = StrapTable[selectedPoint];

			if (selectedPoint == (int)(StrapTable.Count - 1))
			{
				return localStrapEntry.Level;
			}

			// determine which way the volume needs to go
			incrementLevel = false;

			if (volume > localStrapEntry.Volume)
			{
				incrementLevel = true;
			}

			strapFound = false;

			level = localStrapEntry.Level;

			while (!strapFound)
			{
				if (incrementLevel)
					level += .01;
				else
					level -= .01;
				returnedVolume = GetVolumeFromSelectedStrapHorizontalTank(level, StrapTable);

				if (incrementLevel)
				{
					if (returnedVolume >= volume)
						return level;
				}
				else
				{
					if (returnedVolume <= volume)
						return level;
				}

			}





			/*
			localStrapEntry = StrapTable[selectedPoint];
			if (volume > testValue)
			{
				selectedPoint += 1;
				localStrapEntry = StrapTable[selectedPoint];
			}
			if (selectedPoint < (int)(StrapTable.Count - 1))
			{
				selectedPoint += 1;
				if(selectedPoint > (StrapTable.Count - 1))
				{
					selectedPoint = 0;
				}
				localStrapEntry = StrapTable[selectedPoint];
			}
			HighestVolume = localStrapEntry.Volume;
			HighestLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			if (selectedPoint < 0)
				selectedPoint = StrapTable.Count - 1;
			localStrapEntry = StrapTable[selectedPoint];

			HighVolume = localStrapEntry.Volume;
			HighLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			if (selectedPoint < 0)
				selectedPoint = StrapTable.Count - 1;
			localStrapEntry = StrapTable[selectedPoint];

			LowVolume = localStrapEntry.Volume;
			LowLevel = localStrapEntry.Level;

			VolumeRatio = (((volume - HighVolume) * (volume - HighestVolume)) /
			((LowVolume - HighVolume) * (LowVolume - HighestVolume)));

			CalcLevel1 = (LowLevel * VolumeRatio);

			VolumeRatio = (((volume - LowVolume) * (volume - HighestVolume)) /
					  ((HighVolume - LowVolume) * (HighVolume - HighestVolume)));

			CalcLevel2 = (HighLevel * VolumeRatio);

			VolumeRatio = (((volume - LowVolume) * (volume - HighVolume)) /
					  ((HighestVolume - LowVolume) * (HighestVolume - HighVolume)));

			CalcLevel3 = (HighestLevel * VolumeRatio);

			dReturnValue = (double)(CalcLevel1 + CalcLevel2 + CalcLevel3);
			*/
			return dReturnValue;
		}

		public double GetLevelFromSelectedStrap(double volume, List<StrapTableEntry> StrapTable)
		{
			var comparer = new StrapTableVolumeComparer();

			var index = StrapTable.BinarySearch(new StrapTableEntry(-9999, volume), comparer);

			if (index >= 0) // if we find a match
			{
				return StrapTable[index].Level;
			}
			else if (index == -1) // if below the minimun value
			{
				return StrapTable[0].Level;
			}
			else if (-index - 1 == StrapTable.Count) //if above the maximum value
			{
				return StrapTable[StrapTable.Count - 1].Level;
			}

			var firstEntry = StrapTable[-index - 2];
			var nextEntry = StrapTable[-index - 1];

			var percentage = (volume - firstEntry.Volume) / (nextEntry.Volume - firstEntry.Volume);
			return firstEntry.Level + (nextEntry.Level - firstEntry.Level) * percentage;
		}

		public void GetMinStrapTableLevelFromSelectedStrap(int SelectedStrapTable, ref double? minLevel, ref double? maxLevel)
		{
			minLevel = null;
			maxLevel = null;
			if (SelectedStrapTable < 0 ||
				SelectedStrapTable > 5 ||
				this.StrapTables[SelectedStrapTable].table.Count == 0)
			{
				return;
			}
			List<StrapTableEntry> StrapTable = this.StrapTables[SelectedStrapTable].table;
			minLevel = StrapTable[0].Level;
			maxLevel = StrapTable[StrapTable.Count - 1].Level;
		}

		public double GetVolumeFromSelectedStrapHorizontalTank(double level, List<StrapTableEntry> StrapTable)
		{
			double dReturnValue = 0.0;
			int maximumValue = StrapTable.Count - 1;
			int minimumValue = 0;
			int selectedPoint = 0;
			bool entryFound = false;
			double testValue = 0.0;
			double HighestVolume = 0.0;
			double HighestLevel = 0.0;
			double HighVolume = 0.0;
			double HighLevel = 0.0;
			double LowVolume = 0.0;
			double LowLevel = 0.0;
			double LevelRatio = 0.0;
			double CalcVolume1 = 0.0;
			double CalcVolume2 = 0.0;
			double CalcVolume3 = 0.0;
			StrapTableEntry localStrapEntry = null;

			// below minimum entry
			if (level < StrapTable[0].Level)
			{
				return(StrapTable[0].Volume);
			}
			// above maximum entry
			if (level > StrapTable[maximumValue].Level)
			{
				return (StrapTable[maximumValue].Volume);
			}

			while (minimumValue <= maximumValue)
			{
				selectedPoint = (minimumValue + maximumValue) >> 1;
				testValue = StrapTable[selectedPoint].Level;

				if (testValue == level)
				{
					entryFound = true;
					break;
				}
				// if below the selected point
				if (level < testValue)
				{
					// give up if at the bottom
					if (selectedPoint == 0)
						break;
					// Check Lower Entries
					maximumValue = selectedPoint - 1;
				}
				else
				{
					minimumValue = selectedPoint - 1;
					if (minimumValue <= 0)
						break;
				}

				// prevent an infinite loop, the resultant miinimumValue or maximumValue will not alter the selected point
				if (selectedPoint == (minimumValue + maximumValue) >> 1)
				{
					break;
				}
			}

			if (entryFound)
			{
				return(StrapTable[selectedPoint].Volume);
			}

			localStrapEntry = StrapTable[selectedPoint];
			if(level != testValue)
			{
				selectedPoint += 1;
				localStrapEntry = StrapTable[selectedPoint];
			}

			if (selectedPoint < (int)(StrapTable.Count - 1))
			{
				selectedPoint += 1;
				localStrapEntry = StrapTable[selectedPoint];
			}

			HighestVolume = localStrapEntry.Volume;   //	Get Highest Values
			HighestLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			localStrapEntry = StrapTable[selectedPoint];

			HighVolume = localStrapEntry.Volume;
			HighLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			localStrapEntry = StrapTable[selectedPoint];
			LowVolume = localStrapEntry.Volume;
			LowLevel = localStrapEntry.Level;

			// carry out quadratic equations for horizontal cyclinder
			LevelRatio = (((level - HighLevel) * (level - HighestLevel)) /
						((LowLevel - HighLevel) * (LowLevel - HighestLevel)));

			CalcVolume1 = (LowVolume * LevelRatio);

			LevelRatio = (((level - LowLevel) * (level - HighestLevel)) /
						((HighLevel - LowLevel) * (HighLevel - HighestLevel)));

			CalcVolume2 = (HighVolume * LevelRatio);

			LevelRatio = (((level - LowLevel) * (level - HighLevel)) /
						((HighestLevel - LowLevel) * (HighestLevel - HighLevel)));

			CalcVolume3 = (HighestVolume * LevelRatio);

			dReturnValue = (CalcVolume1 + CalcVolume2 + CalcVolume3);
			return dReturnValue;
		}

		public double GetVolumeFromSelectedStrapStandardSphere(double level, List<StrapTableEntry> StrapTable)
		{
			double dReturnValue = 0.0;
			int maximumValue = StrapTable.Count - 1;
			int minimumValue = 0;
			int selectedPoint = 0;
			bool entryFound = false;
			double testValue = 0.0;
			double HighestVolume = 0.0;
			double HighestLevel = 0.0;
			double HighVolume = 0.0;
			double HighLevel = 0.0;
			double LowVolume = 0.0;
			double LowLevel = 0.0;
			double LowestVolume = 0.0;
			double LowestLevel = 0.0;
			double Top1 = 0.0;
			double Top2 = 0.0;
			double Top3 = 0.0;
			double Top4 = 0.0;
			double Bottom1 = 0.0;
			double Bottom2 = 0.0;
			double Bottom3 = 0.0;
			double Bottom4 = 0.0;
			StrapTableEntry localStrapEntry = null;

			// below minimum entry
			if (level < StrapTable[0].Level)
			{
				return (StrapTable[0].Volume);
			}
			// above maximum entry
			if (level > StrapTable[maximumValue].Level)
			{
				return (StrapTable[maximumValue].Volume);
			}

			while (minimumValue <= maximumValue)
			{
				selectedPoint = (minimumValue + maximumValue) >> 1;
				testValue = StrapTable[selectedPoint].Level;

				if (testValue == level)
				{
					entryFound = true;
					break;
				}
				// if below the selected point
				if (level < testValue)
				{
					// give up if at the bottom
					if (selectedPoint == 0)
						break;
					// Check Lower Entries
					maximumValue = selectedPoint - 1;
				}
				else
				{
					minimumValue = selectedPoint - 1;
					if (minimumValue <= 0)
						break;
				}
			}

			if (entryFound)
			{
				return (StrapTable[selectedPoint].Volume);
			}

			localStrapEntry = StrapTable[selectedPoint];
			if (level != testValue)
			{
				selectedPoint += 1;
				localStrapEntry = StrapTable[selectedPoint];
			}
			if (selectedPoint < (int)(StrapTable.Count - 1))
			{
				selectedPoint += 1;
				localStrapEntry = StrapTable[selectedPoint];

				// if we are at the bottom of the tank then move 
				// up another place
				if (selectedPoint < 2)
				{
					selectedPoint += 1;
					localStrapEntry = StrapTable[selectedPoint];
				}
			}
			HighestVolume = localStrapEntry.Volume;
			HighestLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			localStrapEntry = StrapTable[selectedPoint];
			HighVolume = localStrapEntry.Volume;
			HighLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			if (selectedPoint < 0)
			{
				selectedPoint = StrapTable.Count - 1;
			}
			localStrapEntry = StrapTable[selectedPoint];
			LowVolume = localStrapEntry.Volume;
			LowLevel = localStrapEntry.Level;

			selectedPoint -= 1;
			if(selectedPoint < 0)
			{
				selectedPoint = StrapTable.Count - 1;
			}
			localStrapEntry = StrapTable[selectedPoint];
			LowestVolume = localStrapEntry.Volume;
			LowestLevel = localStrapEntry.Level;

			Top1 = LowestVolume * ((level - LowLevel) / 100.0)
					* ((level - HighLevel) / 100.0f) * ((level - HighestLevel) / 100.0);
			Top2 = LowVolume * ((level - LowestLevel) / 100.0)
					* ((level - HighLevel) / 100.0f) * ((level - HighestLevel) / 100.0);
			Top3 = HighVolume * ((level - LowestLevel) / 100.0)
					* ((level - LowLevel) / 100.0f) * ((level - HighestLevel) / 100.0);
			Top4 = HighestVolume * ((level - LowestLevel) / 100.0)
					* ((level - LowLevel) / 100.0f) * ((level - HighLevel) / 100.0);
			Bottom1 = ((LowestLevel - LowLevel) / 100.0)
				 	* ((LowestLevel - HighLevel) / 100.0)
					* ((LowestLevel - HighestLevel) / 100.0);
			Bottom2 = ((LowLevel - LowestLevel) / 100.0)
					* ((LowLevel - HighLevel) / 100.0)
					* ((LowLevel - HighestLevel) / 100.0);
			Bottom3 = ((HighLevel - LowestLevel) / 100.0)
					* ((HighLevel - LowLevel) / 100.0)
					* ((HighLevel - HighestLevel) / 100.0);
			Bottom4 = ((HighestLevel - LowestLevel) / 100.0)
					* ((HighestLevel - LowLevel) / 100.0)
					* ((HighestLevel - HighLevel) / 100.0);

			dReturnValue = (Top1 / Bottom1 + Top2 / Bottom2 +
						Top3 / Bottom3 + Top4 / Bottom4);


			return dReturnValue;
		}

	}
}
