namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;

	using FMBusinessObjects.DataObjects;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class RateModuleSettingsEditorModel
	{
		#region Private data members
		private string rateUnitAbbr;
		#endregion

		#region Propoerties
		public string PointId { get; set; }
		public string PointPropertyId { get; set; }
		public Guid PointGuid { get; set; }
		public Guid PointPropertyGuid { get; set; }
		public RateModuleSettings RateModuleSettings { get; set; }
		public EngineeringUnit RateUnit { get; set; }
		public EngineeringUnitType RateUnitType { get; set; }
		public bool IsTemplatePoint { get; set; }
		public bool Readonly { get; set; }
		public string RateUnitAbbr
		{
			get
			{
				if (string.IsNullOrEmpty(this.rateUnitAbbr))
				{
					return string.Empty;
				}

				return " (" + this.rateUnitAbbr + ")";
			}
			set
			{
				this.rateUnitAbbr = value;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the rate module settings editor model.
		/// </summary>
		public RateModuleSettingsEditorModel()
		{
			this.Init();
		}

		/// <summary>
		/// This constructor will set the property in the model.
		/// </summary>
		/// <param name="isTemplatePoint"></param>
		/// <param name="pointId"></param>
		/// <param name="pointGuid"></param>
		/// <param name="pointPropertyId"></param>
		/// <param name="pointPropertyGuid"></param>
		/// <param name="rateModuleSettings"></param>
		/// <param name="rateUnitAbbr"></param>
		/// <param name="rateUnit"></param>
		/// <param name="rateUnitType"></param>
		/// <param name="readOnly"></param>
		public RateModuleSettingsEditorModel(	bool isTemplatePoint, 
															string pointId, 
															Guid pointGuid, 
															string pointPropertyId,
															Guid pointPropertyGuid, 
															RateModuleSettings rateModuleSettings, 
															string rateUnitAbbr, 
															EngineeringUnit rateUnit,
															EngineeringUnitType rateUnitType, 
															bool readOnly)
		{
			this.IsTemplatePoint = IsTemplatePoint;
			this.PointId			= pointId;
			this.PointGuid			= pointGuid;
			this.PointPropertyId		= pointPropertyId;
			this.PointPropertyGuid	= pointPropertyGuid;
			this.RateModuleSettings = rateModuleSettings;
			this.Readonly			= readOnly;
			this.rateUnitAbbr		= rateUnitAbbr;
			this.RateUnit			= rateUnit;
			this.RateUnitType		= rateUnitType;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.IsTemplatePoint = false;
			this.PointId			= string.Empty;
			this.PointGuid			= Guid.Empty;
			this.PointPropertyGuid	= Guid.Empty;
			this.RateModuleSettings = new RateModuleSettings();
			this.Readonly			= false;
			this.rateUnitAbbr		= string.Empty;
			this.RateUnit			= EngineeringUnit.FmuNone;
			this.RateUnitType		= EngineeringUnitType.FmuVelocity;
			this.PointPropertyId = string.Empty;
		}
		#endregion
	}
}