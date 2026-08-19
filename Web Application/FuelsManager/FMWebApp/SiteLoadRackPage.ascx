<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="SiteLoadRackPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteLoadRackPage" %>
<html>
<head>
    <style>
			.column {
				float: left;
			}
			.box {
				border: 1px; 
				border-color: darkgray; 
				border-style: solid;
				width:320px;
				padding: 10px 20px 10px 20px;
				
				margin: 10PX;
			}
			.box span:first-child {
				font-size:15px;
			}
			.formfieldtitle {
				min-width:200px;
				margin-bottom: 1px;
			}

			input + .formfieldtitle {
				width:50px;
			}
			input.formfield {
				width:50px;
				margin-bottom: 2px;
			}
			input[type='checkbox'] + label {
				min-width:150px;
			}
			input[type='checkbox']
			{
				vertical-align: bottom;
			}
			.formfieldtitle label {
				margin-bottom:1px;
			}
		</style>
</head>
<body>
    <div>
        <div class="column">
            <!-- this div sets off the Station Prompt Configuration section -->
            <div class="box">
                <FMControls:FMLabel ID="StationPromptConfigurationLabel" runat="server" CssClass="formfieldtitle">Station Prompt Configuration</FMControls:FMLabel><br />
                <FMControls:FMCheckBox ID="PromptForReturnCheckBox" TabIndex="12" runat="server" CssClass="formfieldtitle" Text="Prompt For Returns"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForShipmentNumberCheckBox" TabIndex="13" runat="server" CssClass="formfieldtitle" Text="Prompt For Shipment Number"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForCustomerCardCheckBox" TabIndex="14" runat="server" CssClass="formfieldtitle" Text="Prompt For Customer Card"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="ListEquipmentCheckBox" TabIndex="15" runat="server" CssClass="formfieldtitle" Text="List Equipment"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForTruckCardCheckBox" TabIndex="16" runat="server" CssClass="formfieldtitle" Text="Prompt For Truck Card"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="UseCompanyEquipmentIdentifiersCheckBox" TabIndex="17" runat="server" CssClass="formfieldtitle" Text="Use Company Equipment Identifiers"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForCompartmentCheckBox" TabIndex="18" runat="server" CssClass="formfieldtitle" Text="Prompt For Compartment"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForTractorOrTankerCheckBox" TabIndex="19" runat="server" CssClass="formfieldtitle" Text="Prompt For Tractor or Tanker" AutoPostBack="True"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForFirstTrailerCheckBox" TabIndex="20" runat="server" CssClass="formfieldtitle" Text="Prompt For First Trailer"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForSecondTrailerCheckBox" TabIndex="21" runat="server" CssClass="formfieldtitle" Text="Prompt For Second Trailer"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForThirdTrailerCheckBox" TabIndex="22" runat="server" CssClass="formfieldtitle" Text="Prompt For Third Trailer"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="PromptForTransactionCompletionCheckBox" TabIndex="23" runat="server" CssClass="formfieldtitle" Text="Prompt For Transaction Completion"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="InhibitCustomerConfirmationPromptCheckBox" TabIndex="24" runat="server" Text="Inhibit Customer Confirmation Prompt" CssClass="formfieldtitle"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="RequireTrailerScullyCheckBox" TabIndex="25" runat="server" Text="Require Trailer Scully" CssClass="formfieldtitle"></FMControls:FMCheckBox>
                <FMControls:FMLabel ID="Label4" AssociatedControlID="MaximumPromptsTextBox" runat="server" CssClass="formfieldtitle">Maximum Prompts:</FMControls:FMLabel>
                <asp:TextBox ID="MaximumPromptsTextBox" TabIndex="26" runat="server" CssClass="formfield"></asp:TextBox>
            </div>
            <!-- this div sets off the Short Card Number Settings section -->
            <div class="box">
                <FMControls:FMLabel ID="ShortCardNumberSettingsLabel" runat="server" CssClass="formfieldtitle">Short Card Number Settings</FMControls:FMLabel><br />
                <FMControls:FMCheckBox ID="UseShortCardNumberCheckBox" TabIndex="27" runat="server" Text="Use Short Card Number" CssClass="formfieldtitle"></FMControls:FMCheckBox>
                <div>
                    <FMControls:FMLabel ID="Fmlabel2" AssociatedControlID="StartingShortCardNumberTextBox" runat="server" CssClass="formfieldtitle">Starting Short Card Number:</FMControls:FMLabel>
                    <asp:TextBox ID="StartingShortCardNumberTextBox" TabIndex="28" runat="server" CssClass="formfield"></asp:TextBox>
                </div>
            </div>


            <!-- this div sets off the Driver Lockout and Warning Period section -->
            <div class="box">
                <FMControls:FMLabel ID="DriverLockoutAndWarningPeriodLabel" runat="server" CssClass="formfieldtitle">Driver Lockout and Warning Period</FMControls:FMLabel><br />
                <div>
                    <FMControls:FMLabel ID="Label1" AssociatedControlID="DriverTimeoutPeriodTextBox" runat="server" CssClass="formfieldtitle">Inactivity Period before Lockout:</FMControls:FMLabel>
                    <asp:TextBox ID="DriverTimeoutPeriodTextBox" TabIndex="29" runat="server" CssClass="formfield"></asp:TextBox>
                    <FMControls:FMLabel ID="DriverTimeoutPeriodUnitLabel" runat="server" CssClass="formfieldtitle">days</FMControls:FMLabel>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label2" AssociatedControlID="DriverWarningPeriodTextBox" runat="server" CssClass="formfieldtitle">Driver Warning Period:</FMControls:FMLabel>
                    <asp:TextBox ID="DriverWarningPeriodTextBox" TabIndex="30" runat="server" CssClass="formfield"></asp:TextBox>
                    <FMControls:FMLabel ID="DriverWarningPeriodUnitsLabel" runat="server" CssClass="formfieldtitle">days</FMControls:FMLabel>
                </div>
            </div>
        </div>
        <div class="column">
             <!-- this div sets off the Station Validation section -->
            <div class="box">
                <FMControls:FMLabel ID="FMLabel3" runat="server" CssClass="formfieldtitle">Station Validation</FMControls:FMLabel><br />
                <FMControls:FMCheckBox ID="AccessCardinRequiredCheckBox" TabIndex="32" runat="server" CssClass="formfieldtitle" Text="Access Card In Required"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="EnforceDriverEquipmentMatchCheckBox" TabIndex="33" runat="server" CssClass="formfieldtitle" Text="Enforce Driver Equipment Match"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="InhibitAccessAfterHoursCheckBox" TabIndex="34" runat="server" CssClass="formfieldtitle" Text="Inhibit Access After Hours"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="InhibitMultipleCardInsCheckBox" TabIndex="35" runat="server" CssClass="formfieldtitle" Text="Inhibit Multiple Entry Card In's"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="InhibitLoadOffLoadMultipleCardIns" TabIndex="36" runat="server" CssClass="formfieldtitle" Text="Inhibit Multiple Loadrack Card In's"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="EnforceSalesOrderLimit" TabIndex="37" runat="server" CssClass="formfieldtitle" Text="Enforce Sales Order Limit"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="CheckSiteNumberCheckBox" TabIndex="38" runat="server" CssClass="formfieldtitle" Text="Check Site Number"></FMControls:FMCheckBox>
                <div>
                    <FMControls:FMLabel ID="FMLABEL4" AssociatedControlID="CardInTimeoutTextBox" runat="server" CssClass="formfieldtitle">Card Timeout (Min):</FMControls:FMLabel>
                    <asp:TextBox ID="CardInTimeoutTextBox" TabIndex="39" runat="server" CssClass="formfield"></asp:TextBox>
                </div>
            </div>
            <!-- this div sets off the Load Settings section -->
            <div class="box">
                <FMControls:FMLabel ID="LoadSettingsLabel" runat="server" CssClass="formfieldtitle">Load Settings</FMControls:FMLabel><br />
                <FMControls:FMCheckBox ID="LoadByNetCheckBox" TabIndex="40" runat="server" CssClass="formfieldtitle" Text="Load By Net"></FMControls:FMCheckBox>
                <div>
                    <FMControls:FMLabel ID="Fmlabel1" AssociatedControlID="MaximumVehicleWeightTextBox" runat="server" CssClass="formfieldtitle">Maximum Vehicle Weight:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumVehicleWeightTextBox" TabIndex="41" runat="server" CssClass="formfield"></asp:TextBox>
                    <FMControls:FMLabel ID="MaxVehicleWeightUnitsLabel" runat="server" CssClass="formfieldtitle">lbs</FMControls:FMLabel>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label6" AssociatedControlID="MaximumLoadAmountTextBox" runat="server" CssClass="formfieldtitle">Maximum Load Amount:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumLoadAmountTextBox" TabIndex="42" runat="server" CssClass="formfield"></asp:TextBox>
                    <asp:Label ID="MaxLoadAmountUnitsLabel" runat="server" CssClass="formfieldtitle">gal (US)</asp:Label>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label12" AssociatedControlID="MaximumNumberOfActiveArmsTextBox" runat="server" CssClass="formfieldtitle">Maximum Number of Active Arms:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumNumberOfActiveArmsTextBox" TabIndex="43" runat="server" CssClass="formfield"></asp:TextBox>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label8" AssociatedControlID="MaximumLoadTimeTextBox" runat="server" CssClass="formfieldtitle">Maximum Load Time:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumLoadTimeTextBox" TabIndex="44" runat="server" CssClass="formfield"></asp:TextBox>
                    <FMControls:FMLabel ID="MaxLoadTimeUnitsLabel" runat="server" CssClass="formfieldtitle">minutes</FMControls:FMLabel>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label9" AssociatedControlID="MaximumIdleTimeTextBox" runat="server" CssClass="formfieldtitle">Maximum Idle Time:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumIdleTimeTextBox" TabIndex="45" runat="server" CssClass="formfield"></asp:TextBox>
                    <FMControls:FMLabel ID="MaxIdleTimeUnitsLabel" runat="server" CssClass="formfieldtitle">minutes</FMControls:FMLabel>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label5" AssociatedControlID="MaximumFlushAmountTextBox" runat="server" CssClass="formfieldtitle">Maximum Flush Amount:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumFlushAmountTextBox" TabIndex="46" runat="server" CssClass="formfield"></asp:TextBox>
                    <asp:Label ID="MaxFlushAmountUnitsLabel" runat="server" CssClass="formfieldtitle">gal (US)</asp:Label>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label10" AssociatedControlID="MaximumMeterProvingAmountTextBox" runat="server" CssClass="formfieldtitle">Maximum Meter Proving Amount:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumMeterProvingAmountTextBox" TabIndex="47" runat="server" CssClass="formfield"></asp:TextBox>
                    <asp:Label ID="MaxMeterProvingAmountUnitsLabel" runat="server" CssClass="formfieldtitle">gal (US)</asp:Label>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label11" AssociatedControlID="MaximumReturnsAmountTextBox" runat="server" CssClass="formfieldtitle">Maximum Returns Amount:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumReturnsAmountTextBox" TabIndex="48" runat="server" CssClass="formfield"></asp:TextBox>
                    <asp:Label ID="MaxReturnsAmountUnitsLabel" runat="server" CssClass="formfieldtitle">gal (US)</asp:Label>
                </div>
                <div>
                    <FMControls:FMLabel ID="MaximumProductTemperatureLabel" AssociatedControlID="MaximumProductTemperatureTextBox" runat="server" CssClass="formfieldtitle">Maximum Product Temperature:</FMControls:FMLabel>
                    <asp:TextBox ID="MaximumProductTemperatureTextBox" TabIndex="49" runat="server" CssClass="formfield"></asp:TextBox>
                    <FMControls:FMLabel ID="MaxProductTempUnitsLabel" runat="server" CssClass="formfieldtitle">F</FMControls:FMLabel>
                </div>
                <div>
                    <FMControls:FMLabel ID="Label17" AssociatedControlID="VarianceCountTextBox" runat="server" CssClass="formfieldtitle">Variance Count:</FMControls:FMLabel>
                    <asp:TextBox ID="VarianceCountTextBox" TabIndex="50" runat="server" CssClass="formfield"></asp:TextBox>

                </div>
                <div>
                    <FMControls:FMLabel ID="Label18" AssociatedControlID="VarianceTolaranceTextBox" runat="server" CssClass="formfieldtitle">Variance Tolerance:</FMControls:FMLabel>
                    <asp:TextBox ID="VarianceTolaranceTextBox" TabIndex="51" runat="server" CssClass="formfield"></asp:TextBox>
                    <FMControls:FMLabel ID="Label20" runat="server" CssClass="formfieldtitle">%</FMControls:FMLabel>
                </div>
                <div>
                    <FMControls:FMLabel ID="FMLABEL5" AssociatedControlID="FillMethodDropDownList" runat="server" CssClass="formfieldtitle">Fill Method:</FMControls:FMLabel>
                    <FMControls:FMDropDownList ID="FillMethodDropDownList" TabIndex="52" runat="server" CssClass="formfield"></FMControls:FMDropDownList>
                </div>
            </div>

        </div>
    </div>
</body>
</html>
