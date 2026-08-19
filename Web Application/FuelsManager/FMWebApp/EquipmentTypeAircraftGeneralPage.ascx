<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EquipmentTypeAircraftGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.EquipmentTypeAircraftGeneralPage" %>
<%@ Register Assembly="FMControls" Namespace="FMControls" TagPrefix="FMControls" %>

<style type="text/css">
    .formfield {
        font-family: Arial, Helvetica, sans-serif;
        font-size: 12px;
        font-style: normal;
        line-height: normal;
        font-weight: normal;
        font-variant: normal;
        text-transform: none;
        color: #000000;
    }

    .formfieldtitle {
        font-family: Arial, Helvetica, sans-serif;
        font-size: 12px;
        font-style: normal;
        line-height: normal;
        font-weight: bold;
        font-variant: normal;
        text-transform: none;
        color: #333333;
        white-space: nowrap;
    }
</style>
<table style="width: 600px">
    <tr>
        <td>
            <FMControls:FMLabel ID="TypeClassLabel"
                Style="z-index: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Type:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <FMControls:FMDropDownList ID="AttributeDropDownList" runat="server"
                Style="z-index: 101; width: 200px"
                BackColor="Transparent" CssClass="formfield" TabIndex="1" AutoPostBack="True"
                OnSelectedIndexChanged="AttributeDropDownListSelectedIndexChanged">
            </FMControls:FMDropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="EquipmentTypeIdLabel" AssociatedControlID="EquipmentTypeIDTextbox"
                Style="z-index: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Equipment Type ID:</FMControls:FMLabel>
        </td>
        <td style="padding-bottom: 5px;">
            <FMControls:FMLabel ID="AsteriskLabel"
                Style="z-index: 106; width: 12px;" runat="server"
                BackColor="Transparent" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
            <asp:TextBox ID="EquipmentTypeIDTextbox" Style="z-index: 109;" aria-required="true"
                runat="server" CssClass="formfield" Width="264px" MaxLength="50" TabIndex="2"
                OnTextChanged="EquipmentTypeIDTextboxTextChanged"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="DescriptionLabel"
                Style="z-index: 106; height: 15px;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Description:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <asp:TextBox ID="DescriptionTextbox" Style="z-index: 109; width: 263px;"
                runat="server" CssClass="formfield" MaxLength="50" TabIndex="3"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="ModelLabel"
                Style="z-index: 106; height: 15px;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Model:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <asp:TextBox ID="ModelTextbox" Style="z-index: 109; width: 150px;"
                runat="server" CssClass="formfield" MaxLength="20" TabIndex="3"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="MakeLabel"
                Style="z-index: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Make:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <asp:TextBox ID="MakeTextbox" Style="z-index: 109; width: 150px;"
                runat="server" CssClass="formfield" MaxLength="32" TabIndex="4"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="YearLabel"
                Style="z-index: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Year:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <asp:TextBox ID="YearTextbox" Style="z-index: 109; width: 70px;"
                runat="server" CssClass="formfield" MaxLength="4" TabIndex="5"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="CustomerDesignatorLabel"
                Style="z-index: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Customer Designator:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <asp:TextBox ID="CustomerDesignatorTextBox" Style="z-index: 109; width: 350px;"
                runat="server" CssClass="formfield" MaxLength="255" TabIndex="6"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="ServiceTimeLabel"
                Style="z-index: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Service Time:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <asp:TextBox ID="ServiceTimeTextbox" Style="z-index: 109; width: 150px;"
                runat="server" CssClass="formfield" MaxLength="32" TabIndex="7"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="ProductLabel"
                Style="z-index: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Product:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <FMControls:FMDropDownList ID="ProductDropDownList" runat="server"
                Style="z-index: 101; width: 175px"
                BackColor="Transparent" CssClass="formfieldtitle" TabIndex="8"
                AutoPostBack="True"
                OnSelectedIndexChanged="VolumeUnitsDownListSelectedIndexChanged">
            </FMControls:FMDropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="CompanyRoleConstraintLabel" AssociatedControlID="CompanyRoleDropDownList"
                Style="z-index: 101; width: 140px;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Company Role Constraint:</FMControls:FMLabel>
        </td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <FMControls:FMDropDownList ID="CompanyRoleDropDownList" runat="server"
                Style="z-index: 101; width: 200px"
                BackColor="Transparent" CssClass="formfield" TabIndex="8">
            </FMControls:FMDropDownList>
        </td>
    </tr>
    <tr>
        <td></td>
        <td style="padding-left: 9px; padding-bottom: 5px;">
            <FMControls:FMCheckBox ID="AllowFuelingByWeightCheckbox" Style="z-index: 118; "
                TabIndex="9" runat="server" CssClass="formfieldtitle" Text="Allow Fueling By Weight"
                Width="232px" AutoPostBack="True"
                OnCheckedChanged="AllowFuelingByWeightCheckboxCheckedChanged"></FMControls:FMCheckBox>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table style="width: 100%">
                <tr>
                    <td></td>
                    <td style="padding-left: 129px">
                        <FMControls:FMLabel ID="UnitsLabel"
                            Style="z-index: 106; " runat="server"
                            BackColor="Transparent" CssClass="formfieldtitle">Units:</FMControls:FMLabel>
                    </td>
                    <td style="padding-right: 150px">
                        <FMControls:FMLabel ID="DecimalPlacesLabel"
                            Style="z-index: 106; " runat="server"
                            BackColor="Transparent" CssClass="formfieldtitle">Decimal Places:</FMControls:FMLabel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="MassLabel"
                            Style="z-index: 106; " runat="server"
                            BackColor="Transparent" CssClass="formfieldtitle">Mass:</FMControls:FMLabel>
                    </td>
                    <td style="padding-left: 129px;">
                        <FMControls:FMDropDownList ID="MassUnitsDropDownList" runat="server"
                            Style="z-index: 101; width: 100px;"
                            BackColor="Transparent" CssClass="formfieldtitle" TabIndex="10"
                            AutoPostBack="True"
                            OnSelectedIndexChanged="MassUnitsDropDownListSelectedIndexChanged">
                        </FMControls:FMDropDownList>
                    </td>
                    <td>
                        <asp:TextBox ID="MassDecimalPlacesTextBox" Style="z-index: 109; width: 50px;"
                            runat="server" CssClass="formfield" MaxLength="2" TabIndex="11"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="VolumeLabel"
                            Style="z-index: 106; " runat="server"
                            BackColor="Transparent" CssClass="formfieldtitle">Volume:</FMControls:FMLabel>
                    </td>
                    <td style="padding-left: 129px;">
                        <FMControls:FMDropDownList ID="VolumeUnitsDownList" runat="server"
                            Style="z-index: 101; width: 100px"
                            BackColor="Transparent" CssClass="formfieldtitle" TabIndex="12"
                            AutoPostBack="True"
                            OnSelectedIndexChanged="VolumeUnitsDownListSelectedIndexChanged">
                        </FMControls:FMDropDownList>
                    </td>
                    <td>
                        <asp:TextBox ID="VolumeDecimalPlacesTextBox" Style="z-index: 109; width: 50px;"
                            runat="server" CssClass="formfield" MaxLength="2" TabIndex="13"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>


<asp:Panel ID="WingToWingTolerancePanel" runat="server" Style="z-index: 109; left: 780px; position: absolute; top: 20px; width: 325px; height: 75px;"
    BorderColor="LightSteelBlue" BorderStyle="Solid"
    BorderWidth="1px" />
<FMControls:FMRadioButton ID="rbWingToWingMassAlias" runat="server" Style="z-index: 109; left: 790px; position: absolute; top: 30px; right: 433px;"
    GroupName="WingToWingToleranceGroup" Text="Mass"
    CssClass="formfieldNoWrap" AutoPostBack="False" TabIndex="14" />
<FMControls:FMRadioButton ID="rbWingToWingVolumeAlias" runat="server" Style="z-index: 109; left: 890px; position: absolute; top: 30px"
    GroupName="WingToWingToleranceGroup" Text="Volume"
    CssClass="formfieldNoWrap" AutoPostBack="False" TabIndex="15" />
<FMControls:FMRadioButton ID="rbWingToWingPercentageAlias" runat="server" Style="z-index: 109; left: 990px; position: absolute; top: 30px"
    GroupName="WingToWingToleranceGroup" Text="Percentage"
    CssClass="formfieldNoWrap" AutoPostBack="False" TabIndex="16" />
<asp:TextBox ID="WingToWingValueTextBox" Style="z-index: 109; left: 790px; position: absolute; top: 60px; width: 200px; "
    runat="server" CssClass="formfield" MaxLength="12" TabIndex="17"></asp:TextBox>
<asp:Panel ID="TankToTankTolerancePanel" runat="server" Style="z-index: 109; left: 780px; position: absolute; top: 100px; width: 325px; height: 75px;"
    BorderColor="LightSteelBlue" BorderStyle="Solid"
    BorderWidth="1px" />
<FMControls:FMRadioButton ID="rbTankToTankMassAlias" runat="server" Style="z-index: 109; left: 790px; position: absolute; top: 110px;"
    GroupName="TankToTankToleranceGroup" Text="Mass"
    CssClass="formfieldNoWrap" AutoPostBack="False" TabIndex="19" />
<FMControls:FMRadioButton ID="rbTankToTankVoluemAlias" runat="server" Style="z-index: 109; left: 890px; position: absolute; top: 110px"
    GroupName="TankToTankToleranceGroup" Text="Volume"
    CssClass="formfieldNoWrap" AutoPostBack="False" TabIndex="20" />
<FMControls:FMRadioButton ID="rbTankToTankPercentageAlias" runat="server" Style="z-index: 109; left: 990px; position: absolute; top: 110px"
    GroupName="TankToTankToleranceGroup" Text="Percentage"
    CssClass="formfieldNoWrap" AutoPostBack="False" TabIndex="21" />
<asp:TextBox ID="TankToTankValueTextBox" Style="z-index: 109; left: 790px; position: absolute; top: 140px; width: 200px;"
    runat="server" CssClass="formfield" MaxLength="12" TabIndex="22"></asp:TextBox>
<asp:Panel ID="FuelServicePanel" runat="server" Style="z-index: 109; left: 780px; position: absolute; top: 180px; width: 425px; height: 135px;"
    BorderColor="LightSteelBlue" BorderStyle="Solid"
    BorderWidth="1px" />
<FMControls:FMLabel ID="EquipmentClassLabel"
    Style="z-index: 106; left: 790px; position: absolute; top: 190px" runat="server"
    BackColor="Transparent" CssClass="formfieldtitle">Type:</FMControls:FMLabel>
<FMControls:FMRadioButton ID="rbFuelServiceToleranceMassAlias" runat="server" Style="z-index: 109; left: 890px; position: absolute; top: 190px;"
    GroupName="FuelServiceToleranceGroup" Text="Mass"
    CssClass="formfieldNoWrap" AutoPostBack="True" OnCheckedChanged="RbFuelServiceToleranceMassAliasCheckedChanged" TabIndex="23" />
<FMControls:FMRadioButton ID="rbFuelServiceToleranceVolumeAlias" runat="server" Style="z-index: 109; left: 990px; position: absolute; top: 190px"
    GroupName="FuelServiceToleranceGroup" Text="Volume"
    CssClass="formfieldNoWrap" AutoPostBack="True" OnCheckedChanged="RbFuelServiceToleranceVolumeAliasCheckedChanged" TabIndex="24" />
<FMControls:FMRadioButton ID="rbFuelServiceTolerancePercentageAlias" runat="server" Style="z-index: 109; left: 1090px; position: absolute; top: 190px"
    GroupName="FuelServiceToleranceGroup" Text="Percentage"
    CssClass="formfieldNoWrap" AutoPostBack="True" OnCheckedChanged="RbFuelServiceTolerancePercentageAliasCheckedChanged" TabIndex="25" />
<FMControls:FMLabel ID="ToleranceLabel"
    Style="z-index: 106; left: 790px; position: absolute; top: 220px" runat="server"
    BackColor="Transparent" CssClass="formfieldtitle">Tolerance:</FMControls:FMLabel>
<asp:TextBox ID="FuelServiceToleranceTextBox" Style="z-index: 109; left: 890px; position: absolute; top: 220px; width: 200px;"
    runat="server" CssClass="formfield" MaxLength="12" TabIndex="26"></asp:TextBox>

<FMControls:FMLabel ID="MaxTypeLabel"
    Style="z-index: 106; left: 790px; position: absolute; top: 250px" runat="server"
    BackColor="Transparent" CssClass="formfieldtitle">Max Type:</FMControls:FMLabel>
<FMControls:FMRadioButton ID="rbFuelServiceMaxMassAlias" runat="server" Style="z-index: 109; left: 890px; position: absolute; top: 250px;"
    GroupName="FuelServiceMaxGroup" Text="Mass"
    CssClass="formfieldNoWrap" AutoPostBack="False" TabIndex="27" />
<FMControls:FMRadioButton ID="rbFuelServiceMaxVolueAlias" runat="server" Style="z-index: 109; left: 990px; position: absolute; top: 250px"
    GroupName="FuelServiceMaxGroup" Text="Volume"
    CssClass="formfieldNoWrap" AutoPostBack="False" TabIndex="28" />
<FMControls:FMLabel ID="MaxLabel"
    Style="z-index: 106; left: 790px; position: absolute; top: 280px" runat="server"
    BackColor="Transparent" CssClass="formfieldtitle">Max:</FMControls:FMLabel>
<asp:TextBox ID="FuelServiceMaxTextBox" Style="z-index: 109; left: 890px; position: absolute; top: 280px; width: 200px;"
    runat="server" CssClass="formfield" MaxLength="12" TabIndex="29"></asp:TextBox>


        <FMCONTROLS:FMLABEL id="WingToWingToleranceLabel" 
            style="Z-INDEX: 106; LEFT: 600px; POSITION: absolute; TOP: 25px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Wing-To-Wing Tolerance:</FMCONTROLS:FMLABEL>
        <FMCONTROLS:FMLABEL id="TankToTankToleranceLabel" 
            style="Z-INDEX: 106; LEFT: 600px; POSITION: absolute; TOP: 105px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Tank-To-Tank Tolerance:</FMCONTROLS:FMLABEL>
        <FMCONTROLS:FMLABEL id="FuelServiceToleranceLabel" 
            style="Z-INDEX: 106; LEFT: 600px; POSITION: absolute; TOP: 185px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Fuel Service Tolerance:</FMCONTROLS:FMLABEL>
		<p>
            &nbsp;</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>




