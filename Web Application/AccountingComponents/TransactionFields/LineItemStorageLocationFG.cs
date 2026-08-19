//*****************************************************************************************************************
//  FILE NAME:		LineItemStorageLocationFG.cs
//	PURPOSE:		This class inherits from the TankTextButtonGenerator, ILineItemField, and ISublineItemField 
//					classes. It is used to contain the line item product field information.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Richard Panachida
//	VERSION:		1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		   By:					Reason:
//		----------	-----------------	-------------------------------------------
//		4-10-2008	V. THOMPSON			CSI 5708 Added code to automatically populate temperature and density
//										      for a line item/subline item when a tank is selected
//    2008-12-24  Richard Panachida Defect 959: Changed the Require property to return the base require data member
//                                  instead of a hard coded True.
//		06-02-2009	W.Gray				7.4.6.0 - Changed to populate with standard density (CSI 3474)
//*****************************************************************************************************************

namespace TransactionFields
{
	using System;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
    using System.Collections.Generic;

    public class LineItemStorageLocationFG : TankTextButtonGenerator, ILineItemField, ISublineItemField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the tank field class.
		/// </summary>
		public LineItemStorageLocationFG ( )
		{
			this.autoPostBack = false;
		}
		#endregion

		#region Override properties
		/// <summary>
		/// This property return true if the editable.
		/// </summary>
		override public bool Editable
		{
			get { return true; }
		}

		/// <summary>
		/// This property will return the field ID for the tank object.
		/// </summary>
		public override string FieldID
		{
			get { return "LineItem StorageLocationID"; }
		}

		/// <summary>
		/// This property will return true if the tank is required.
		/// </summary>
		public override bool Required
		{
			get { return base.Required; }
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength ( FieldID, FIELD_LENGTH ); }
		}
		#endregion


		#region ILineItemField Members
		/// <summary>
		/// This method is used to retrieve the tank ID from the Line Item
		/// DO. It is used for the grid mode.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		virtual public object GetDataValue ( LineItemDO inLineItem )
		{
			return inLineItem.StorageLocationID;
		}

		/// <summary>
		/// This method is used to retrieve the product ID from the Line Item
		/// DO. It is used for the grid mode.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		virtual public string GetDataText(LineItemDO inLineItem)
		{
			string tankID = string.Empty;

			if (inLineItem != null)
			{
				tankID = inLineItem.StorageLocationID;
			}

			return tankID;
		}

		/// <summary>
		/// This method will set the tank information in the line item data object.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <param name="newValue"></param>
		virtual public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			var tankID = newValue as string;

			// vthompson - CSI 5708
			bool tankIdChanged = (inLineItem.StorageLocationID != tankID);

			if (string.IsNullOrEmpty(tankID))
			{
				inLineItem.StorageLocationID = string.Empty;
				inLineItem.StorageLocationTankGuid = Guid.Empty;
			}
			else
			{
				TankClass tank = this.GetTankObject ( tankID );

				if (tank == null)
				{
					inLineItem.StorageLocationID = string.Empty;
					inLineItem.StorageLocationTankGuid = Guid.Empty;
				}
				else
				{
					inLineItem.StorageLocationID = tank.ID;
					inLineItem.StorageLocationTankGuid = tank.IdentityGuid;

					// vthompson - CSI 5708
					// If the tank selected has changed set the temperature and density
					if (tankIdChanged)
					{
						bool TemperatureGood = true, DensityGood = true;
                  LineItemDensityFG densityFG = base.fieldGenerator.GetFieldGenerator("LineItem Density") as LineItemDensityFG;
                  LineItemTemperatureFG temperatureFG = base.fieldGenerator.GetFieldGenerator("LineItem Temperature") as LineItemTemperatureFG;


                  var pointValueIdentifierList = new List<PointValueIdentifier>(2);

						// With the new IM integration, we now want to get the values from the tank points, not the tank entity process variable values
						//object tempValue = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.TEMPERATURE_PV, tank);
						//object densityValue = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV, tank);

						// added try catch processing since the change below would throw if it is not configured.
						// this resulted in an exit from the application and a failure in configuaring the transaction.
						try
						{
							pointValueIdentifierList.Add(new PointValueIdentifier(new Guid(tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.TEMPERATURE_PV].OPCItemID), PointValueType.Tag, string.Empty));
						}
						catch
						{
							TemperatureGood = false;
						}

						try
						{
							pointValueIdentifierList.Add(new PointValueIdentifier(new Guid(tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV].OPCItemID), PointValueType.Tag, string.Empty));
						}
						catch
						{
							DensityGood = false;
						}

						if (TemperatureGood == true || DensityGood == true)
						{
							try
							{
								var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(
																							x =>
																							x.GetPointValueData(transContext.security, pointValueIdentifierList, false)
																					);
								if (TemperatureGood == true)
								{
									object tempValue = (Double)pointValueList[0].Value;
									if (tempValue is double)
									{
										if (temperatureFG == null)
											inLineItem.Temperature = (double)tempValue;
										else
											temperatureFG.SetDataValue(inLineItem, tempValue);
									}
								}
								if (DensityGood == true)
								{
									object densityValue = (Double)pointValueList[1].Value;
									if (densityValue is double)
									{
										if (densityFG == null)
											inLineItem.Density = (double)densityValue;
										else
											densityFG.SetDataValue(inLineItem, densityValue);
									}
								}
							}
							catch
							{
								// do nothing. Point service is most likely not running
							}
						}
               }
				}
			}

			this.SetTank ( );
			OnFieldChanged ( );
		}
		#endregion

		#region ISublineItemField Members
		/// <summary>
		/// This method will return a tank object back to the requestor.
		/// </summary>
		/// <param name="inSublineItem"></param>
		/// <returns></returns>
		object ISublineItemField.GetDataValue ( SubLineItemDO inSublineItem )
		{
			return inSublineItem.StorageLocationID;
		}

		/// <summary>
		/// This method will return a tank ID for the subline item.  It will return an empty string
		/// if the sublineItem is not defined.
		/// </summary>
		/// <param name="inSublineItem"></param>
		/// <returns></returns>
		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			string tankID = string.Empty;

			if (inSublineItem != null)
			{
				tankID = inSublineItem.StorageLocationID;
			}

			return tankID;
		}

		/// <summary>
		/// This method will set the tank information in the subline item data object.
		/// </summary>
		/// <param name="inSublineItem"></param>
		/// <param name="newValue"></param>
		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			var tankID = newValue as string;
			
			bool tankIdChanged = (inSublineItem.StorageLocationID != tankID);

			if (string.IsNullOrEmpty(tankID))
			{
				inSublineItem.StorageLocationID = string.Empty;
				inSublineItem.StorageLocationTankGuid = Guid.Empty;
			}
			else
			{
				TankClass tank = this.GetTankObject ( tankID );

				if (tank == null)
				{
					inSublineItem.StorageLocationID = string.Empty;
					inSublineItem.StorageLocationTankGuid = Guid.Empty;
				}
				else
				{
					inSublineItem.StorageLocationID = tank.ID;
					inSublineItem.StorageLocationTankGuid = tank.IdentityGuid;

					// If the tank selected has changed set the temperature and density
					if (tankIdChanged)
					{
						object tempValue = this.GetProcessVariableValue ( PROCESS_VARIABLE_TYPE.TEMPERATURE_PV, tank );
						object densityValue = this.GetProcessVariableValue ( PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV, tank );

						// If the values are not doubles then don't bother populating
						if (tempValue is double)
						{
							inSublineItem.Temperature = (double) tempValue;
						}

						if (densityValue is double)
						{
							inSublineItem.Density = (double) densityValue;
						}
					}
				}
			}

			this.SetTank ( );
			OnFieldChanged ( );
		}

		/// <summary>
		/// Returns the value of tanks Process Variable
		/// </summary>
		/// <param name="variableType">The Process Variable type for which the value should be returned</param>
		/// <param name="tank">The tank that owns the process variable</param>
		/// <returns>An object representing the Process Variable's value</returns>
		/// <remarks>The original version of this function returned only values for
		/// temperature and density.  If other variables are to be returned code for
		/// returning those variables must be added</remarks>
		private object GetProcessVariableValue ( PROCESS_VARIABLE_TYPE variableType, TankClass tank )
		{
			ProcessVariableClass processVariable = tank.ProcessVariableCollection[variableType];

			// The site is needed so that temperature units, density units, temperature
			// decimal places and density decimal places can be used to get the
			// values of the Process Variables
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(transContext.security, trans.SiteGuid, false, true, false)
																);


			object returnValue = null;

			// Density
			if (variableType == PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV)
			{
				returnValue = processVariable.GetValue ( site.DensityUnits, site._DensityDecimalPlaces );
			}
			// Temperature
			else if (variableType == PROCESS_VARIABLE_TYPE.TEMPERATURE_PV)
			{
				returnValue = processVariable.GetValue ( site.TemperatureUnits, site._TemperatureDecimalPlaces );
			}

			return returnValue;
		}
		#endregion
	}
}
