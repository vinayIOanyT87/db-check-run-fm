using System;
using System.Collections.Generic;

namespace FMBusinessObjects.Constants
{
	public static class Guids
	{
		// General Guids
		public static readonly Guid AllFilterGuid = new Guid("10000000-0000-0000-0000-000000000000");

		// Site Guids
		public static readonly Guid SiteAdminGuid = new Guid("00000000-0000-0000-0000-000000000001");
		public static readonly Guid SiteDefaultGuid = new Guid("00000000-0000-0000-0000-000000000004"); // Default Site Guid for single site key

		// Group Guids
		public static readonly Guid GroupAdminGuid = new Guid("00000000-0000-0000-0000-000000000003");

		// User Guids
		public static readonly Guid UserAdminGuid = new Guid("00000000-0000-0000-0000-000000000002");

		// System Settings Guids
		public static readonly Guid SystemSettingsGuid = new Guid("00000000-0000-0000-0000-000000000005");

		// Accuload Guids
		public static readonly Guid AcculoadGuid = new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}");
		public static readonly Guid AcculoadPortGuid = new Guid("{2070F4BA-651D-4268-9F5A-1EBE0A137141}");
		public static readonly Guid AcculoadCardReaderGuid = new Guid("{0AB8E5B2-986C-4B03-A0C7-243FC6963328}");

		// Contrec Guids
		public static readonly Guid ContrecGuid = new Guid("{59DB8E98-D175-49A8-997B-8D342154B9D7}");
		public static readonly Guid ContrecPortGuid = new Guid("{2B2CCFD9-9EF7-48BB-BEF4-C58C0C43409D}");

		// Daniel Guids
		public static readonly Guid DanielGuid = new Guid("{54F57ECB-6111-4A9A-AFA6-ABC5B3C4FF59}");
		public static readonly Guid DanielPortGuid = new Guid("{265331A0-40D0-4DEC-B614-1A21CDC5CC1F}");

		// OptomuxController Guids
		public static readonly Guid OptomuxControllerGuid = new Guid("{DD940B4F-C212-4361-8FDE-D4061584E4D0}");
		public static readonly Guid OptomuxControllerPortGuid = new Guid("{D1CAA238-8AB9-4E70-A628-49AB61EC5BD1}");

		// SCADA Guids
		public static readonly Guid SCADAGraphicGuid = new Guid("{9E79E49B-5765-4793-AD41-F3EEB156E5D2}");

		// Tank Guids
		public static readonly Guid TankGuid = new Guid("{F075F7A6-0D97-4C94-B8FA-E3F9EB149833}");

		// WeightScale Guids
		public static readonly Guid WeightScaleGuid = new Guid("{FB4C3029-D5C9-4BB8-AC5A-1914858D79D5}");

		// RequestParser Guids
		public static readonly Guid UninitializedSiteGuid = new Guid("{00000000-0000-0000-0000-000000000005}");
		public static readonly Guid UninitializedLoginSiteGuid = new Guid("{00000000-0000-0000-0000-000000000006}");
		public static readonly Guid UninitializedUserGuid = new Guid("{00000000-0000-0000-0000-000000000007}");

		// Enterprise Master Synchronization Server (There can only be one that represents this.  Subsequent ones are configured for each child/sibling instance)
		// public static readonly Guid EnterpriseServerGuid = new Guid("{10000000-1000-1000-1000-100000000000}");

		// Managed Queries Import/Export Guid
		public static readonly Guid ManagedQueriesImportExportGuid = new Guid("{A8F9B57C-2AC8-4D10-9DC8-1D6198D0EFD2}");

		// Well Know Tag Identity Guids
		// If adding well know tag Guids to standard tank template also add them to the PointTemplateTag.EnumerateWellKnownTags list
		public static readonly Guid CreatedByGuid = new Guid("{5327ED53-7967-44E4-8A43-75AED4144469}");
		public static readonly Guid DensityProductHighGuid = new Guid("{93A68748-F403-4DF6-8C22-849EFB0A5CAE}");
		public static readonly Guid DensityProductInAirGuid = new Guid("{3C13EC14-13E7-4DB0-80D0-E7BF260C6169}");
		public static readonly Guid DensityProductLowGuid = new Guid("{27CE1BA8-5127-4715-9588-B1B39F782887}");
		public static readonly Guid DensityProductObservedGuid = new Guid("{CF9FCF9B-3B81-4EBF-85ED-2FCCD92F8EC1}");
		public static readonly Guid DensityProductStandardGuid = new Guid("{A8998003-ACED-4500-9D63-0B5A83942880}");
		public static readonly Guid DensityProductStandardInAirGuid = new Guid("{238F1B4A-05AE-442F-AB05-DA8FA6819887}");
		public static readonly Guid InitiateIdentifierGuid = new Guid("{A4E398DB-CEB3-4A1D-9C2B-C29F3AA18302}");
		public static readonly Guid InitiationCountGuid = new Guid("{78E86DFC-4E56-4670-B95E-9DC4D7B5C081}");
		public static readonly Guid LeakDetectionLastRunGuid = new Guid("B8DC44D4-C565-46A5-A256-10E72C942EA4");
		public static readonly Guid LeakRateGuid = new Guid("91F98328-AF76-4229-B701-4600B2B645EE");
      public static readonly Guid LevelProductMaxOpLimitWellKnownGuid = new Guid("{EA779F86-9D5A-4935-BDC2-5E1197AEFA08}");
      public static readonly Guid LevelProductMinOpLimitWellKnownGuid = new Guid("{3989FB28-8143-44AF-92BD-786B5AEFD636}");
      public static readonly Guid LevelProductGuid = new Guid("{5512A8AA-E049-47AA-95A3-810E82AF8B32}");
      public static readonly Guid LevelWaterGuid = new Guid("{FE06575F-F390-43CC-B173-536F785F1893}");
		public static readonly Guid MassLiquidGuid = new Guid("{B15227EA-1BEC-4107-9B69-558085BF1C88}");
		public static readonly Guid MovementCommandGuid = new Guid("{46532C05-B232-4309-BB92-F5414B9A4327}");
		public static readonly Guid MovementControlIdentifierWellKnownGuid = new Guid("{93E58A86-A296-42BC-B2A4-5D84C8032344}");
		public static readonly Guid MovementDeviationGuid = new Guid("{99E17476-4B04-4D89-A536-ADCC1BCF761F}");
		public static readonly Guid MovementPercentDeviationGuid = new Guid("{A6B239C7-9DB3-431A-8EF2-C50891476C93}");
		public static readonly Guid MovementHistoryWrittenTimeGuid = new Guid("8F77AAF1-946D-4BE8-8892-75CC7EF3587A");
		public static readonly Guid MovementStatusGuid = new Guid("{0BC90D94-A42B-4C6F-8C99-60A18A5546AB}");
		public static readonly Guid OperationalModeGuid = new Guid("{ED3CC2F3-EA9A-411C-A547-714D683D18EE}");
      public static readonly Guid PercentBSWGuid = new Guid("{6E609A88-45EC-4C6C-9DF1-A008BD65511A}");
      public static readonly Guid PointIdGuid = new Guid("{F1248A03-4E5B-4670-AC64-FA31FCB764E7}");
		public static readonly Guid PointProductGuid = new Guid("{5E2B6E97-3E50-4F16-900B-1D86DE9537F3}");
		public static readonly Guid PressureBottomGuid = new Guid("{4d6138b7-dbf5-41f9-8cc6-a2aaa84dba21}");
		public static readonly Guid PressureVaporGuid = new Guid("{91FD7E65-A53B-49AB-9D2D-69AB01180ABF}");
		public static readonly Guid PulseMeterNumberOfRollOversWellKnownGuid = new Guid("{0B42D1AB-CD4B-4A10-BCC8-7FCB1C3BEDFF}");
		public static readonly Guid PulseMeterLastValueWellKnownGuid = new Guid("{77A911A5-CF4B-4C3A-870A-7037D2D3D8E8}");
		public static readonly Guid PulseMeterCurrentValueWellKnownGuid = new Guid("{AAE9349F-518C-47FA-B7BE-E507F4F87BCA}");
		public static readonly Guid PulseMeterVolumePerPulseWellKnownGuid = new Guid("{15CD54AE-C8BA-4E49-BE9E-E7616CF1315B}");
		public static readonly Guid PulseMeterRollOverAmountWellKnownGuid = new Guid("{433C6CA9-8212-41D5-B233-8824C13D0FA2}");
		public static readonly Guid PulseMeterLastReadWasRollOverWellKnownGuid = new Guid("{DE575E24-4F5E-40D8-BFEB-1ABB75D4D3D2}");
		public static readonly Guid StopIdentifierGuid = new Guid("{64BF4616-4E1B-44BE-87E2-815996E972A0}");
		public static readonly Guid TankShellCorrectionGuid = new Guid("{D95F42D8-26F2-4294-AB0E-6DF30E8291F0}");
		public static readonly Guid TankStatusGuid = new Guid("{834B9D8A-C17A-48f6-97FC-1B18EB562866}");
		public static readonly Guid TemperatureAmbientGuid = new Guid("{B214BC73-9676-499D-AA0A-BA302A4257F6}");
		public static readonly Guid TemperatureDensityGuid = new Guid("{1A6FC43E-096D-4996-B1E0-BF4F477369D7}");
		public static readonly Guid TemperatureProductGuid = new Guid("{FD6C562A-15EC-4521-A3A6-31719ED3D9F9}");
		public static readonly Guid TemperatureProductHighGuid = new Guid("{8EBAFD8C-48C6-4750-A758-6EB398961BA0}");
		public static readonly Guid TemperatureProductHiHiGuid = new Guid("{AA14DBAE-EDD9-4DA9-8549-4FB5F6C21BAF}");
		public static readonly Guid TemperatureProductLoLoGuid = new Guid("{18721E39-22E0-418F-895E-294568D452BC}");
		public static readonly Guid TemperatureProductLowGuid = new Guid("{26253C9C-90D0-403F-80A8-738DEE390A21}");
		public static readonly Guid TransferModeGuid = new Guid("{763B7918-B1FB-4DD1-8AE8-A742F6ED2CAE}");
		public static readonly Guid TransferredGOVGuid = new Guid("{E8C62FC9-7781-471C-8385-C12435BF2959}");
		public static readonly Guid TransferredNSVGuid = new Guid("{7076A195-FB85-4690-A6F7-BA6C91D473A7}");
		public static readonly Guid TransferredVolumeGuid = new Guid("{32179DE8-9568-4091-912B-D2A834C7641A}");
		public static readonly Guid TransferredWaterVolumeGuid = new Guid("{4E845EB7-7D61-421B-A4DF-49735B0C758E}");
		public static readonly Guid TransferStartGOVGuid = new Guid("{DEFCDF34-4614-4FA0-98A7-B8D3EA06F689}");
		public static readonly Guid TransferStartLevelGuid = new Guid("{8E2B7CD7-EB09-4C1B-BBD1-460CAA5E44EC}");
		public static readonly Guid TransferStartNSVGuid = new Guid("{DCC09494-6833-4AC9-8240-E3CEB5CF231F}");
		public static readonly Guid TransferStartTimeGuid = new Guid("{CB388EE8-5A84-4B8C-8D0D-F95ED0F269D2}");
		public static readonly Guid TransferStartVolumeGuid = new Guid("{904D7A76-5AD2-4B9E-AD2E-87C3733200A0}");
		public static readonly Guid TransferStartWaterVolumeGuid = new Guid("{93D58C80-F8A8-4CF4-840E-ACE89AB5D750}");
		public static readonly Guid TransferStatusGuid = new Guid("{089BC64A-2C69-4B52-8BAC-C573BF547D7B}");
		public static readonly Guid TransferStopTimeGuid = new Guid("{950A300F-A02D-4314-A7B5-F62156EFE27F}");
		public static readonly Guid TransferTargetGuid = new Guid("{15117345-8C6B-45B0-8F4E-EFD82C5F0F10}");
		public static readonly Guid TransferLevelTargetGuid = new Guid("{659BDE3D-E776-45AD-B5B9-9FA4A12CBD53}");
		public static readonly Guid TransferVolumeTargetGuid = new Guid("{74CE4476-BBE3-4B3B-AFB5-F3B496023845}");
		public static readonly Guid TransferTimeCompletionGuid = new Guid("{B9AA15C9-3955-474C-9E66-E0FB06430FE4}");
		public static readonly Guid TransferTimeRemainingGuid = new Guid("{6CDD6625-E939-4AE0-A9E3-D098FA520FD1}");
		public static readonly Guid UpdatedByGuid = new Guid("{C0C85360-EDF9-4279-B9C9-F38F23A13C26}");
		public static readonly Guid UserData01WellKnownGuid = new Guid("{1DE65B74-1F49-4971-A311-F59FE1B8BA40}");
		public static readonly Guid UserData02WellKnownGuid = new Guid("{DC25F1E3-91C1-44CD-BB07-37046A45D51E}");
		public static readonly Guid UserData03WellKnownGuid = new Guid("{9FBFD5A7-1909-4E2B-BC42-A10BDC18A5D5}");
		public static readonly Guid UserData04WellKnownGuid = new Guid("{11175F8D-D78F-4390-B650-6A985B4F2F29}");
		public static readonly Guid UserData05WellKnownGuid = new Guid("{239FEB64-4484-4B79-84DB-FF5F7C490218}");
		public static readonly Guid UserData06WellKnownGuid = new Guid("{CC1C3E08-20D0-4A01-8F55-8064C3179374}");
		public static readonly Guid UserData07WellKnownGuid = new Guid("{BA010DE2-80D5-441F-B6F8-620E30563A12}");
		public static readonly Guid UserData08WellKnownGuid = new Guid("{58B04969-65FD-45DC-8893-19BDDD84EC76}");
		public static readonly Guid UserData09WellKnownGuid = new Guid("{C4C98608-2C88-44B0-A4C5-4C292566807C}");
		public static readonly Guid UserData10WellKnownGuid = new Guid("{D31CCE95-0EF2-4A66-B223-5ED003151EA4}");
      public static readonly Guid VolumeBSWGuid = new Guid("{22103D14-8718-46B2-9C03-A51A3533CEBE}");
      public static readonly Guid VolumeCorrectionFactorGuid = new Guid("{CB92A6EE-D07A-408E-80CF-DD808D70B5F2}");
      public static readonly Guid VolumeCorrectionFactorUnroundedGuid = new Guid("53F2BF73-8954-4A0A-A374-E8EE5AD75F40");
		public static readonly Guid VolumeGrossObservedAvailableGuid = new Guid("{90DC36CD-40E2-43E3-9429-9E9640F02354}");
		public static readonly Guid VolumeGrossObservedGuid = new Guid("{AD4CE065-D362-4AF5-A1A3-12D94E31B4C5}");
		public static readonly Guid VolumeGrossObservedRateGuid = new Guid("{2538CB70-4214-4FE8-A34B-6E6C74F3210C}");
		public static readonly Guid VolumeGrossObservedRemainingGuid = new Guid("{647E05AF-0B32-4B73-A872-339B9D16CFE1}");

		public static readonly Guid VolumeGrossObservedHiHiWellKnownGuid = new Guid("{23E8758F-ABCC-43A9-83B0-06A048F86866}");
		public static readonly Guid VolumeGrossObservedHighWellKnownGuid = new Guid("{84170A0D-796A-4FD4-9DB6-68FE6E0F7A5F}");
		public static readonly Guid VolumeGrossObservedLowWellKnownGuid = new Guid("{071B343F-0DBA-4EE7-A2A3-A7B31AD1F4CC}");
		public static readonly Guid VolumeGrossObservedLoLoWellKnownGuid = new Guid("{D33A6834-3B4E-468A-9587-8ABAC4B75F32}");

		public static readonly Guid VolumeGrossStandardGuid = new Guid("{C0D21AB8-CF24-45B9-A9B7-48C732CFCCB2}");
		public static readonly Guid VolumeNetStandardAvailableGuid = new Guid("{E8817675-02E8-40D0-B0D9-4535E662F061}");
		public static readonly Guid VolumeNetStandardGuid = new Guid("{783A5FE8-7147-4D0B-BD44-5D46DC5B98C0}");
		public static readonly Guid VolumeNetStandardRateGuid = new Guid("{FD718EDD-A3E2-4876-912C-B552F0B2A39C}");
		public static readonly Guid VolumeNetStandardRemainingGuid = new Guid("{821B2F2E-1FF2-417C-8B4D-E66C826DA153}");
		public static readonly Guid VolumeNetStandardUnroundedGuid = new Guid("C7EB776B-A7B3-4212-B5EC-BB2873EEE2D4");
		public static readonly Guid VolumeRoofCorrectionGuid = new Guid("{DD947512-B3A0-42A1-88B5-EC2F00114483}");
		public static readonly Guid VolumeTotalObservedGuid = new Guid("{4D2FECC4-727F-43C5-B141-14ACE87D4E63}");
		public static readonly Guid VolumeTotalObservedRateGuid = new Guid("{C9FE307F-4015-45BB-80BF-E016431FD313}");
		public static readonly Guid VolumeWaterGuid = new Guid("{A1C95A16-B2E7-462D-AD23-4FBD2D5EA56C}");
		public static readonly Guid MovementInitiateIdentityGuid = new Guid("{A4E398DB-CEB3-4A1D-9C2B-C29F3AA18302}");
		public static readonly Guid MovementStopIdentityGuid = new Guid("{64BF4616-4E1B-44BE-87E2-815996E972A0}");
		public static readonly Guid MovementControlIdentifier = new Guid("{93E58A86-A296-42BC-B2A4-5D84C8032344}");
		public static readonly Guid VolumeTotalizerGrossGuid = new Guid("{BCD57442-9536-42D2-9008-F79F06A6E954}");
		public static readonly Guid VolumeTotalizerNetGuid = new Guid("{E909FF6F-90DF-4F1A-AF89-B8E860E4836D}");
		public static readonly Guid VolumeTotalizerGrossTagGuidWellKnownGuid = new Guid("{BCD57442-9536-42D2-9008-F79F06A6E954}");
		public static readonly Guid VolumeTotalizerNetTagGuidWellKnownGuid = new Guid("{E909FF6F-90DF-4F1A-AF89-B8E860E4836D}");

		//WKGuids for TDU Template
		public static readonly Guid TduTemplateWellKnownGuid = new Guid("{186348C4-C81F-4BC0-8A9E-5ABB9579885A}");
		public static readonly Guid TduNotepadWellKnownGuid = new Guid("{870ebdf0-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrainCommWellKnownGuid = new Guid("{870ebdf1-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrCurrIndexWellKnownGuid = new Guid("{870ebdf2-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrDirectionWellKnownGuid = new Guid("{870ebdf3-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrEndVolWellKnownGuid = new Guid("{870ebdf4-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrInitialVolWellKnownGuid = new Guid("{870ebdf5-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrNextVolWellKnownGuid = new Guid("{870ebdf6-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrRelaxVolWellKnownGuid = new Guid("{870ebdf7-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrStatusWellKnownGuid = new Guid("{870ebdf8-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrTempWellKnownGuid = new Guid("{870ebdf9-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrTkNumberWellKnownGuid = new Guid("{870ebdfa-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrVolumeWellKnownGuid = new Guid("{870ebdfb-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduSearchHartCommandWellKnownGuid = new Guid("{870ebdfc-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduSearchHartTkNumWellKnownGuid = new Guid("{870ebdfd-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTduFirmwareWellKnownGuid = new Guid("{870ebdfe-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTduStatusWellKnownGuid = new Guid("{870ebdff-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTduTempWellKnownGuid = new Guid("{870ebe00-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTduVoltageWellKnownGuid = new Guid("{870ebe01-7a6e-11ee-b864-103d1cbd9c45}");
		public static readonly Guid TduTrPressureWellKnownGuid = new Guid("{870ebe02-7a6e-11ee-b864-103d1cbd9c45}");


		// If adding well know tag Guids to standard tank template also add them to the PointTemplateTag.EnumerateWellKnownTags list

		// Standard Point Template Guids
		public static readonly Guid VerticalTankTemplateGuid = new Guid("{0ADB4947-1CC4-4A44-91F8-E76F281EA718}");
		public static readonly Guid MovementTemplateGuid = new Guid("{0FE444B2-920F-4572-AC60-31171C1F4763}");
		public static readonly Guid StandardVolumeTemplateGuid = new Guid("{3C7895BF-8A90-40CB-AC3B-04FD089B438B}");
		public static readonly Guid StandardMovementControlTemplateGuid = new Guid("{03E2911F-3195-4BEF-98AB-E7292D4B5B7F}");
		public static readonly Guid StandardNodeTemplateGuid = new Guid("{42EDBDBD-C8FC-4B66-BB36-7EC0C969E378}");


		public static readonly List<Guid> StandardPointTemplatesGuids = new List<Guid>() {
			VerticalTankTemplateGuid,
			MovementTemplateGuid,
			StandardVolumeTemplateGuid,
			StandardMovementControlTemplateGuid,
            StandardNodeTemplateGuid,
        };

		// Standard Module Guids
		public static readonly Guid AvailableAndRemainingVolumeModuleGuid = new Guid("{9FA81AF0-451C-47C6-A0A1-86B4377EC79B}");
		public static readonly Guid LeakDetectionModuleGuid = new Guid("{0802B471-540B-4128-8535-D597E9328BEC}");
		public static readonly Guid MovementModuleGuid = new Guid("{E0024C94-0725-4423-9261-EDE9D84A6ACC}");
		public static readonly Guid MovementNodeModuleGuid = new Guid("{F769E8AF-1F5F-4EC7-A2E5-58759EF79186}");
		public static readonly Guid QuantityModuleGuid = new Guid("{923120A7-1A76-4a72-94BF-75B84265E503}");
		public static readonly Guid RateModuleGuid = new Guid("{1DA35F8B-1DB1-4D72-95FB-42B15D03FF5B}");
		public static readonly Guid RoofCorrectionModuleGuid = new Guid("{7AAC4E9D-46A4-4aec-AC8E-D1543BE71532}");
		public static readonly Guid ShellCorrectionModuleGuid = new Guid("{07B4584F-7B0B-436B-80A2-A9D5E89FE4F2}");
		public static readonly Guid StandardTankCalculatorModuleGuid = new Guid("{06B43B26-8383-4aa5-A9FF-DB46E4F3578A}");
		public static readonly Guid StrapTableModuleGuid = new Guid("{A109B4D4-CB54-4361-A331-A585344C01D9}");
		public static readonly Guid TankCommandModuleGuid = new Guid("{9F0BEAE3-FFC2-47ED-9C01-0E724D6813F8}");
		public static readonly Guid TankTransferModuleGuid = new Guid("{26DE3166-5417-415C-9801-BB2E363D2447}");
		public static readonly Guid VCFModuleGuid = new Guid("{66120ecb-547a-4a48-b9c2-2056068746e6}");


		// Standard Point Type Guids
		public static readonly Guid PointTypeTankGuid = new Guid("{E78CD406-4C19-4978-8940-FA4E404E3E53}");
		public static readonly Guid PointTypeValveGuid = new Guid("{E33A769F-3EFC-46C6-A50F-A103454BFE97}");
		public static readonly Guid PointTypePumpGuid = new Guid("{1135AA41-525B-4024-BF3D-6BF2D55A034B}");
		public static readonly Guid PointTypeMeterGuid = new Guid("{9403A36F-33F6-4DCC-857D-F53C8DC66196}");
		public static readonly Guid PointTypePresetGuid = new Guid("{7EA082F3-6FBF-4136-A2D7-8A3670E9A9EF}");
		public static readonly Guid PointTypePipeGuid = new Guid("{55F0E8B8-3A74-40D0-8B8C-675A4B6A478C}");
		public static readonly Guid PointTypeSystemGuid = new Guid("{2DDEB3E0-545C-444B-B1BF-9CAB048F21B7}");

		public static readonly List<Guid> StandardPointTypes = new List<Guid>(){
			PointTypeTankGuid,
			PointTypeValveGuid,
			PointTypePumpGuid,
			PointTypeMeterGuid,
			PointTypePresetGuid,
			PointTypePipeGuid,
			PointTypeSystemGuid
		};

		// Standard Alarm Priority Guids
		public static readonly Guid NormalUnacknowledgedAlarmPriorityGuid = new Guid("5B7D7344-7D3C-4CDE-A834-B5E2C8BFE11F");
		public static readonly Guid HiHiLoLoAlarmPriorityGuid = new Guid("AA9E557C-A652-4CAF-9BCA-2BCB9AB5B104");
		public static readonly Guid HighLowAlarmPriorityGuid = new Guid("BA35E686-5CCE-402D-982B-18D45958CCB6");
		public static readonly Guid MaxMinOperatingAlarmPriorityGuid = new Guid("402A7722-062B-42F6-B6A5-E6180E2BA2B8");
		public static readonly Guid NotifyPriorityGuid = new Guid("A3094EE3-314D-4834-B498-71D6B8075283");

		public static readonly List<Guid> StandardAlarmPriorityGuids = new List<Guid>()
		{
			NormalUnacknowledgedAlarmPriorityGuid,
			HiHiLoLoAlarmPriorityGuid,
			HighLowAlarmPriorityGuid,
			MaxMinOperatingAlarmPriorityGuid,
			NotifyPriorityGuid
		};

		// Standard Alarm Category Guids - "Alarm Group"
		public static readonly Guid AlarmGroupCategoryApplicationStringGuid = new Guid("{512ab266-b3b8-4a29-b8d9-594795cf63ed}");

		// Standard Email Group Guids
		public static readonly Guid LicenseExpirationNotificationEmailGroupGuid = new Guid("A1D606A5-BF39-436D-9FC5-A9E7F62C5D0B");
		public static readonly Guid AlarmNotNormalNotificationEmailGroupGuid = new Guid("91D267A7-5D80-483D-8958-B053163EB3EE");

		public static readonly List<Guid> StandardEmailGroupGuids = new List<Guid>()
		{
			LicenseExpirationNotificationEmailGroupGuid,
			AlarmNotNormalNotificationEmailGroupGuid
		};
	}
}
