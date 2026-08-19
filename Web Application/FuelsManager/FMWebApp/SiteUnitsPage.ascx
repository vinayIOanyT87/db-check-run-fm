<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="SiteUnitsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteUnitsPage" %>
<html>
<head>
</head>
<body>
    <table style="Z-INDEX: 103; width: 66%; LEFT: 5px; POSITION: absolute; TOP: 5px; height: 300px;" role="presentation" aria-label="layout">
        <tr>
            <td></td>
            <td>
                <FMControls:FMLabel ID="Label18" runat="server"
                    CssClass="formfieldtitle" Width="39px" BackColor="Transparent">Units:</FMControls:FMLabel>
            </td>
            <td>
                <FMControls:FMLabel ID="Label17" runat="server"
                    CssClass="formfieldtitle" Width="92px" BackColor="Transparent">Decimal Places:</FMControls:FMLabel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Label1" AssociatedControlID="LevelUnitsDropDownList" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Level:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="LevelUnitsDropDownList" TabIndex="1" runat="server" Width="88px" CssClass="formfield"></asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="LevelDecimalPlacesTextbox" ToolTip="Level decimal places" TabIndex="2" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:TextBox>
            </td>
            <td>
                <FMControls:FMLabel ID="Fmlabel10" AssociatedControlID="QuantityDisplayDefaultDropDown"
                    runat="server" CssClass="formfieldtitle" Text="Quantity Display Default:"></FMControls:FMLabel>
            </td>
            <td>
                <FMControls:FMDropDownList ID="QuantityDisplayDefaultDropDown"
                    TabIndex="21" runat="server" CssClass="formfield" 
                    OnSelectedIndexChanged="QuantityDisplayDefaultDropDownSelectedIndexChanged">
                </FMControls:FMDropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Label8" runat="server" AssociatedControlID="VolumeUnitsDropDownList"
                    CssClass="formfieldtitle" BackColor="Transparent">Volume:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="VolumeUnitsDropDownList"
                    TabIndex="3" runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="VolumeDecimalPlacesTextbox" ToolTip="Volume decimal places"
                    TabIndex="4" runat="server" CssClass="formfield" Width="32px" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Label6" runat="server" AssociatedControlID="AdditiveVolumeUnitsDropDownList"
                    CssClass="formfieldtitle" BackColor="Transparent">Additive Volume:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="AdditiveVolumeUnitsDropDownList"
                    TabIndex="5" runat="server" Width="88px" CssClass="formfield">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="AdditiveVolumeDecimalPlacesTextbox" ToolTip="Additive Volume decimal places"
                    TabIndex="6" runat="server" CssClass="formfield" Width="32px" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="TemperatureUnitsLabel" AssociatedControlID="TemperatureUnitsDropDownList"
                    runat="server" CssClass="formfieldtitle" BackColor="Transparent">Temperature:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="TemperatureUnitsDropDownList"
                    TabIndex="7" runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="TemperatureDecimalPlacesTextbox" ToolTip="Temperature decimal places"
                    TabIndex="8" runat="server" CssClass="formfield" Width="32px" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Label5" runat="server" AssociatedControlID="DensityUnitsDropDownList"
                    CssClass="formfieldtitle" BackColor="Transparent">Density:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="DensityUnitsDropDownList"
                    TabIndex="9" runat="server" CssClass="formfield" Width="88px">
                </asp:DropDownList>
            </td>
            <td>

                <asp:TextBox ID="DensityDecimalPlacesTextbox" ToolTip="Density decimal places"
                    TabIndex="10" runat="server" CssClass="formfield" Width="32px" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Label2" runat="server" AssociatedControlID="MassUnitsDropDownList"
                    CssClass="formfieldtitle" BackColor="Transparent">Mass:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="MassUnitsDropDownList"
                    TabIndex="11" runat="server" Width="88px" CssClass="formfield" AutoPostBack="True">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="MassDecimalPlacesTextbox" ToolTip="Mass decimal places"
                    TabIndex="12" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Label3" runat="server" AssociatedControlID="FlowUnitsDropDownList"
                    CssClass="formfieldtitle" BackColor="Transparent">Flow:</FMControls:FMLabel>

            </td>
            <td>
                <asp:DropDownList ID="FlowUnitsDropDownList"
                    TabIndex="13" runat="server" Width="88px" CssClass="formfield">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="FlowDecimalPlacesTextbox" ToolTip="Flow decimal places"
                    TabIndex="14" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Label4" runat="server" AssociatedControlID="PressureUnitsDropDownList"
                    CssClass="formfieldtitle" BackColor="Transparent">Pressure:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="PressureUnitsDropDownList"
                    TabIndex="15" runat="server" Width="88px" CssClass="formfield">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="PressureDecimalPlacesTextbox" ToolTip="Pressure decimal places"
                    TabIndex="16" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Fmlabel1" runat="server" AssociatedControlID="AdditiveProfileCycleAmountUnitsDropDownList"
                    CssClass="formfieldtitle" BackColor="Transparent">Additive Profile Cycle Amount:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="AdditiveProfileCycleAmountUnitsDropDownList"
                    TabIndex="17" runat="server" Width="88px" CssClass="formfield">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="AdditiveProfileCycleAmountDecimalPlacesTextbox" ToolTip="Additive profile cycle amount decimal places"
                    TabIndex="18" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="Fmlabel2" runat="server" AssociatedControlID="AdditiveProfileRateUnitsDropDownList"
                    CssClass="formfieldtitle" BackColor="Transparent">Additive Profile Rate:</FMControls:FMLabel>

            </td>
            <td>
                <asp:DropDownList ID="AdditiveProfileRateUnitsDropDownList"
                    TabIndex="19" runat="server" Width="88px" CssClass="formfield">
                </asp:DropDownList>

            </td>
            <td>
                <asp:TextBox ID="AdditiveProfileRateDecimalPlacesTextbox" ToolTip="Additive profile rate decimal places"
                    TabIndex="20" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:TextBox>
            </td>
        </tr>
    </table>
</body>
</html>
