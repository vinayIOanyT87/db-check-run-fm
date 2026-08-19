<%@ Control language="c#" Codebehind="ProductVolumeCorrectionPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductVolumeCorrectionPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<FMCONTROLS:FMLABEL id="ForceVcfTo4DigitsLabel" style="Z-INDEX: 121; LEFT: 1px; POSITION: absolute; TOP: 152px; width: 152px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">Force VCF to 4 digits:</FMCONTROLS:FMLABEL>
	<FMControls:FMRadioButton TabIndex="5" ID="ForceVcfTo4DigitsYesRadioButton" runat="server" Text="Yes" GroupName="ForceVcfTo4Digits" 
	    style="Z-INDEX: 107; LEFT: 196px; POSITION: absolute; TOP: 151px; width: 74px; right: 877px;" CssClass="formfieldtitle" AutoPostBack="False" />
	<FMControls:FMRadioButton TabIndex="6" ID="ForceVcfTo4DigitsNoRadioButton" runat="server" Text="No" GroupName="ForceVcfTo4Digits" 
		 style="Z-INDEX: 107; LEFT: 283px; POSITION: absolute; TOP: 151px; width: 54px;" CssClass="formfieldtitle" AutoPostBack="False" />
	<FMCONTROLS:FMLABEL id="UseHydrometerCorrectionLabel" style="Z-INDEX: 121; LEFT: 0px; POSITION: absolute; TOP: 186px; width: 159px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">Use Hydrometer Correction:</FMCONTROLS:FMLABEL>
	<FMControls:FMRadioButton TabIndex="7" ID="UseHydrometerCorrectionYesRadioButton" runat="server" Text="Yes" GroupName="UseHydrometerCorrection" 
	    style="Z-INDEX: 107; LEFT: 196px; POSITION: absolute; TOP: 182px; width: 74px; right: 877px;" CssClass="formfieldtitle" AutoPostBack="False" />
	<FMControls:FMRadioButton TabIndex="8" ID="UseHydrometerCorrectionNoRadioButton" runat="server" Text="No" GroupName="UseHydrometerCorrection" 
		 style="Z-INDEX: 107; LEFT: 283px; POSITION: absolute; TOP: 182px; width: 54px;" CssClass="formfieldtitle" AutoPostBack="False" />
	<FMCONTROLS:FMLABEL id="UseProductObservedDensityLabel" style="Z-INDEX: 121; LEFT: 0px; POSITION: absolute; TOP: 220px; width: 176px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">Use Product Observed Density:</FMCONTROLS:FMLABEL>
	<FMControls:FMRadioButton TabIndex="9" ID="UseProductObservedDensityYesRadioButton" runat="server" Text="Yes" GroupName="UseProductObservedDensity" 
	    style="Z-INDEX: 107; LEFT: 196px; POSITION: absolute; TOP: 213px; width: 74px; right: 877px;" CssClass="formfieldtitle" AutoPostBack="False" />
	<FMControls:FMRadioButton TabIndex="10" ID="UseProductObservedDensityNoRadioButton" runat="server" Text="No" GroupName="UseProductObservedDensity" 
		 style="Z-INDEX: 107; LEFT: 283px; POSITION: absolute; TOP: 213px; width: 54px;" CssClass="formfieldtitle" AutoPostBack="False" />
	<FMCONTROLS:FMLABEL id="StandardTemperatureUnitsLabel" style="Z-INDEX: 121; LEFT: 382px; POSITION: absolute; TOP: 254px"
		runat="server" Width="56px" CssClass="formfieldtitle" BackColor="Transparent">Units</FMCONTROLS:FMLABEL>
	<asp:textbox id="StandardTemperatureTextbox" style="Z-INDEX: 120; LEFT: 190px; POSITION: absolute; TOP: 254px; width: 166px;"
		tabIndex="11" runat="server" CssClass="formfield"></asp:textbox>
	<FMCONTROLS:FMLABEL id="Label4" style="Z-INDEX: 119; LEFT: 0px; POSITION: absolute; TOP: 254px" runat="server"
		Width="136px" CssClass="formfieldtitle" BackColor="Transparent">Standard Temperature:</FMCONTROLS:FMLABEL>
	<FMCONTROLS:FMLABEL id="StandardDensityUnitsLabel" style="Z-INDEX: 118; LEFT: 806px; POSITION: absolute; TOP: 220px"
		runat="server" Width="56px" CssClass="formfieldtitle" BackColor="Transparent">Units</FMCONTROLS:FMLABEL>
	<FMCONTROLS:FMLABEL id="StandarddDensityLabel" style="Z-INDEX: 115; LEFT: 470px; POSITION: absolute; TOP: 220px" runat="server"
		Width="136px" CssClass="formfieldtitle" BackColor="Transparent">Standard Density:</FMCONTROLS:FMLABEL>
	<asp:textbox id="K4TextBox" style="Z-INDEX: 114; LEFT: 614px; POSITION: absolute; TOP: 152px"
		tabIndex="18" runat="server" Width="168px" CssClass="formfield"></asp:textbox>
	<FMCONTROLS:FMLABEL id="AlphaLabel" style="Z-INDEX: 113; LEFT: 470px; POSITION: absolute; TOP: 186px; width: 30px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">Alpha:</FMCONTROLS:FMLABEL>
	<asp:textbox id="AlphaTextBox" style="Z-INDEX: 112; LEFT: 615px; POSITION: absolute; TOP: 186px"
		tabIndex="19" runat="server" Width="168px" CssClass="formfield"></asp:textbox>
	<FMCONTROLS:FMLABEL id="K4Label" style="Z-INDEX: 113; LEFT: 470px; POSITION: absolute; TOP: 152px; width: 30px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">K4:</FMCONTROLS:FMLABEL>
	<asp:textbox id="K3TextBox" style="Z-INDEX: 112; LEFT: 615px; POSITION: absolute; TOP: 118px"
		tabIndex="17" runat="server" Width="168px" CssClass="formfield"></asp:textbox>
	<FMCONTROLS:FMLABEL id="K3Label" style="Z-INDEX: 111; LEFT: 468px; POSITION: absolute; TOP: 118px; width: 30px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">K3:</FMCONTROLS:FMLABEL>
	<asp:textbox id="K2TextBox" style="Z-INDEX: 110; LEFT: 612px; POSITION: absolute; TOP: 85px"
		tabIndex="16" runat="server" Width="168px" CssClass="formfield"></asp:textbox>
	<FMCONTROLS:FMLABEL id="K2Label" style="Z-INDEX: 109; LEFT: 468px; POSITION: absolute; TOP: 84px; width: 30px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">K2:</FMCONTROLS:FMLABEL>
	<asp:textbox id="K1TextBox" style="Z-INDEX: 108; LEFT: 612px; POSITION: absolute; TOP: 50px"
		tabIndex="15" runat="server" Width="168px" CssClass="formfield"></asp:textbox>
	<FMCONTROLS:FMLABEL id="K1Label" style="Z-INDEX: 107; LEFT: 468px; POSITION: absolute; TOP: 50px; width: 35px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">K1:</FMCONTROLS:FMLABEL>
	<FMCONTROLS:FMLABEL id="K0Label" style="Z-INDEX: 105; LEFT: 468px; POSITION: absolute; TOP: 18px; width: 32px;"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">K0:</FMCONTROLS:FMLABEL>
	<asp:dropdownlist id="TemperatureStandardDropdownlist" style="Z-INDEX: 104; LEFT: 190px; POSITION: absolute; TOP: 118px"
		tabIndex="4" runat="server" Width="168px" CssClass="formfield" AutoPostBack="True" onselectedindexchanged="TemperatureStandardDropDownList_SelectedIndexChanged"></asp:dropdownlist>
	<FMCONTROLS:FMLABEL id="TemperatureStandardLabel" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 118px" runat="server"
		Width="136px" CssClass="formfieldtitle" BackColor="Transparent">Temperature Standard:</FMCONTROLS:FMLABEL>
	<asp:dropdownlist id="CommodityTableDropdownlist" style="Z-INDEX: 104; LEFT: 190px; POSITION: absolute; TOP: 84px"
		tabIndex="3" runat="server" Width="168px" CssClass="formfield" AutoPostBack="True" onselectedindexchanged="CommodityTableDropDownList_SelectedIndexChanged"></asp:dropdownlist>
	<FMCONTROLS:FMLABEL id="CommodityTableLabel" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 84px" runat="server"
		Width="136px" CssClass="formfieldtitle" BackColor="Transparent">Commodity - Table:</FMCONTROLS:FMLABEL>
	<asp:dropdownlist id="StandardRevisionDropDownList" style="Z-INDEX: 104; LEFT: 190px; POSITION: absolute; TOP: 50px;"
		tabIndex="2" runat="server" Width="168px" CssClass="formfield" AutoPostBack="True" onselectedindexchanged="StandardRevisionDropDownList_SelectedIndexChanged"></asp:dropdownlist>
	<FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 50px;" runat="server"
		Width="136px" CssClass="formfieldtitle" BackColor="Transparent">Standard - Revision:</FMCONTROLS:FMLABEL>
	<FMCONTROLS:FMLABEL id="Label1" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 16px" runat="server"
		Width="136px" CssClass="formfieldtitle" BackColor="Transparent">Standards Organization:</FMCONTROLS:FMLABEL>
	<asp:dropdownlist id="StandardsOrganizationDropDownList" style="Z-INDEX: 101; LEFT: 190px; POSITION: absolute; TOP: 16px;"
		tabIndex="1" runat="server" Width="168px" CssClass="formfield" AutoPostBack="True" onselectedindexchanged="StandardsOrganizationDropDownList_SelectedIndexChanged"></asp:dropdownlist>
	<asp:textbox id="K0TextBox" style="Z-INDEX: 106; LEFT: 612px; POSITION: absolute; TOP: 16px"
		tabIndex="14" runat="server" Width="168px" CssClass="formfield"></asp:textbox>
	<FMControls:FMLabel id="AlternateTemperatureUnitsLabel" style="Z-INDEX: 121; LEFT: 382px; POSITION: absolute; TOP: 288px"
		runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="56px">Units</FMControls:FMLabel>
	<asp:textbox id="AlternateTemperatureTextbox" style="Z-INDEX: 120; LEFT: 190px; POSITION: absolute; TOP: 288px"
		tabIndex="12" runat="server" CssClass="formfield" Width="168px"></asp:textbox>
	<FMControls:FMLabel id="Fmlabel2" style="Z-INDEX: 119; LEFT: 0px; POSITION: absolute; TOP: 288px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="136px">Alternate Temperature:</FMControls:FMLabel>
	<FMControls:FMLabel id="AlternatePressureUnitsLabel" style="Z-INDEX: 121; LEFT: 383px; POSITION: absolute; TOP: 322px"
		runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="56px">Units</FMControls:FMLabel>
	<asp:textbox id="AlternatePressureTextbox" style="Z-INDEX: 120; LEFT: 190px; POSITION: absolute; TOP: 322px"
		tabIndex="13" runat="server" CssClass="formfield" Width="168px"></asp:textbox>
	<FMControls:FMLabel id="Fmlabel3" style="Z-INDEX: 119; LEFT: 0px; POSITION: absolute; TOP: 322px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="136px">Alternate Pressure:</FMControls:FMLabel>
    <FMControls:FMCheckBox tabIndex="21" id=ApplyStandardDensityCheckBox style="Z-INDEX: 111; LEFT: 470px; POSITION: absolute; TOP: 250px; width: 151px;" runat="server" CssClass="formfieldtitle" TextAlign="Left" Text="Apply Standard Density:"></FMControls:FMCheckBox>
    <FMControls:FMCheckBox tabIndex="22" id=ApplyVolumeCorrectionCheckBox style="Z-INDEX: 111; LEFT: 470px; POSITION: absolute; TOP: 284px" runat="server" CssClass="formfieldtitle" Width="230px" TextAlign="Left" Text="Enforce use of Volume Correction settings:"></FMControls:FMCheckBox>
	<asp:textbox id="StandardDensityTextbox" style="Z-INDEX: 116; LEFT: 616px; POSITION: absolute; TOP: 220px"
		tabIndex="20" runat="server" Width="168px" CssClass="formfield"></asp:textbox>


