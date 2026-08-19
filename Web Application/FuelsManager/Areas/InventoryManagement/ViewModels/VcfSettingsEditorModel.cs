
namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using FMBusinessObjects.DataObjects;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class VcfSettingsEditorModel
	{
		public string PointId { get; set; }

		public Guid PointGuid { get; set; }

		public string PointPropertyId { get; set; }

		public Guid PointPropertyGuid { get; set; }

		public VcfModuleSettings VcfSettings { get; set; }

		public string StandardsOrganization { get; set; }

		public string StandardAndRevision { get; set; }

		public string CommodityOrTable { get; set; }

		public string StandardTemperature { get; set; }

		public bool IsTemplatePoint { get; set; }

		public Boolean Readonly { get; set; }
		public double K0
		{
			get
			{
				return this.VcfSettings.K[0];
			}
			set
			{
				VcfSettings.K[0] = value;
			}
		}

		public double K1
		{
			get
			{
				return this.VcfSettings.K[1];
			}
			set
			{
				VcfSettings.K[1] = value;
			}
		}

		public double K2
		{
			get
			{
				return this.VcfSettings.K[2];
			}
			set
			{
				VcfSettings.K[2] = value;
			}
		}

		public double K3
		{
			get
			{
				return this.VcfSettings.K[3];
			}
			set
			{
				VcfSettings.K[3] = value;
			}
		}

		public double K4
		{
			get
			{
				return this.VcfSettings.K[4];
			}
			set
			{
				VcfSettings.K[4] = value;
			}
		}

		public EngineeringUnit PressureUnit { get; set; }


		public VcfSettingsEditorModel()
		{
			this.VcfSettings = new VcfModuleSettings();
		}


		public VcfSettingsEditorModel(bool isTemplatePoint, string pointId, Guid pointGuid, string pointPropertyId, Guid pointPropertyGuid, VcfModuleSettings vcfSettings, EngineeringUnit pressureUnit, Boolean readOnly)
		{
			this.IsTemplatePoint = isTemplatePoint;
			this.PointId = pointId;
			this.PointGuid = pointGuid;
			this.PointPropertyId = pointPropertyId;
			this.PointPropertyGuid = pointPropertyGuid;
			this.VcfSettings = vcfSettings;
			this.StandardsOrganization = VcfModuleSettings.GetStandardsOrganization(this.VcfSettings.CorrectionMethodType);
			this.StandardAndRevision = VcfModuleSettings.GetStandardRevision(this.VcfSettings.CorrectionMethodType, this.VcfSettings.CorrectionMethodSpecific);
			this.CommodityOrTable = VcfModuleSettings.GetCommodityOrTable(this.VcfSettings.CorrectionMethodSpecific);
			if ( vcfSettings.BaseTemperature.Value != 0 ) {
				this.StandardTemperature = vcfSettings.BaseTemperature.Value == 60 ? "60 °F" : vcfSettings.BaseTemperature.Value.ToString() + " °C";
			} else {
				this.StandardTemperature = VcfModuleSettings.GetStandardTemperature(this.VcfSettings.CorrectionMethodType, this.VcfSettings.CorrectionMethodSpecific);
			}
			this.PressureUnit = pressureUnit;
			this.Readonly = readOnly;
		}
	}
}