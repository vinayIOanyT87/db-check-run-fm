<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="true" Codebehind="TransactionAliasUnitsPage.ascx.cs" Inherits="FuelsManager.FMWebApp.TransactionAliasUnitsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

		<asp:textbox id="PressureDecimalPlacesTextbox" ToolTip="Pressure Decimal Places" style="Z-INDEX: 123; LEFT: 280px; POSITION: absolute; TOP: 272px"
			tabIndex="16" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:textbox>
		<asp:textbox id="FlowDecimalPlacesTextbox" ToolTip="Flow Decimal Places" style="Z-INDEX: 122; LEFT: 280px; POSITION: absolute; TOP: 240px"
			tabIndex="14" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:textbox>
		<asp:textbox id="MassDecimalPlacesTextbox" ToolTip="Mass Decimal Places" style="Z-INDEX: 121; LEFT: 280px; POSITION: absolute; TOP: 208px"
			tabIndex="12" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:textbox>
		<asp:textbox id="LevelDecimalPlacesTextbox" ToolTip="Level Decimal Places" style="Z-INDEX: 120; LEFT: 280px; POSITION: absolute; TOP: 48px"
			tabIndex="2" runat="server" Width="32px" CssClass="formfield" MaxLength="1"></asp:textbox>
		<FMControls:FMDropDownList id="PressureUnitsDropDownList" ToolTip="Pressure Units" style="Z-INDEX: 119; LEFT: 176px; POSITION: absolute; TOP: 272px"
			tabIndex="15" runat="server" Width="88px" CssClass="formfield" AutoPostBack="True" Translate="false"></FMControls:FMDropDownList>
		<FMControls:FMDropDownList id="FlowUnitsDropDownList" ToolTip="Flow Units" style="Z-INDEX: 118; LEFT: 176px; POSITION: absolute; TOP: 240px"
			tabIndex="13" runat="server" Width="88px" CssClass="formfield" AutoPostBack="True" Translate="false"></FMControls:FMDropDownList>
		<FMControls:FMDropDownList id="LevelUnitsDropDownList" ToolTip="Level Units" style="Z-INDEX: 117; LEFT: 176px; POSITION: absolute; TOP: 48px"
			tabIndex="1" runat="server" Width="88px" CssClass="formfield" AutoPostBack="True" Translate="false"></FMControls:FMDropDownList>
		<FMControls:FMDropDownList id="MassUnitsDropDownList" ToolTip="Mass Units" style="Z-INDEX: 116; LEFT: 176px; POSITION: absolute; TOP: 208px"
			tabIndex="11" runat="server" Width="88px" CssClass="formfield" AutoPostBack="True" Translate="false"></FMControls:FMDropDownList>
		<FMControls:FMDropDownList id="AdditiveVolumeUnitsDropDownList" ToolTip="Additive Volume Units" style="Z-INDEX: 116; LEFT: 176px; POSITION: absolute; TOP: 112px"
			tabIndex="5" runat="server" Width="88px" CssClass="formfield" AutoPostBack="True" Translate="false"></FMControls:FMDropDownList>
		<FMControls:FMLabel id="Label4" style="Z-INDEX: 115; LEFT: 0px; POSITION: absolute; TOP: 272px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Pressure:</FMControls:FMLabel>
		<FMControls:FMLabel id="Label3" style="Z-INDEX: 114; LEFT: 0px; POSITION: absolute; TOP: 240px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Flow:</FMControls:FMLabel>
		<FMControls:FMLabel id="Label2" style="Z-INDEX: 113; LEFT: 0px; POSITION: absolute; TOP: 208px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Mass:</FMControls:FMLabel>
		<FMControls:FMLabel id="Label6" style="Z-INDEX: 113; LEFT: 0px; POSITION: absolute; TOP: 112px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Additive Volume:</FMControls:FMLabel>
		<FMControls:FMLabel id="Label1" style="Z-INDEX: 112; LEFT: 0px; POSITION: absolute; TOP: 48px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Level:</FMControls:FMLabel>
		<asp:textbox id="VolumeDecimalPlacesTextbox" ToolTip="Volume Decimal Places" style="Z-INDEX: 111; LEFT: 280px; POSITION: absolute; TOP: 80px"
			tabIndex="4" runat="server" CssClass="formfield" Width="32px" AutoPostBack="True" MaxLength="1"></asp:textbox>
		<FMControls:FMLabel id="Label18" style="Z-INDEX: 110; LEFT: 176px; POSITION: absolute; TOP: 16px" runat="server"
			CssClass="formfieldtitle" Width="39px" BackColor="Transparent">Units:</FMControls:FMLabel>
		<asp:textbox id="TemperatureDecimalPlacesTextbox" ToolTip="Temperature Decimal Places" style="Z-INDEX: 109; LEFT: 280px; POSITION: absolute; TOP: 144px"
			tabIndex="8" runat="server" CssClass="formfield" Width="32px" MaxLength="1"></asp:textbox>
		<asp:textbox id="AdditiveVolumeDecimalPlacesTextbox" ToolTip="Additive Volume Decimal Places" style="Z-INDEX: 109; LEFT: 280px; POSITION: absolute; TOP: 112px"
			tabIndex="6" runat="server" CssClass="formfield" Width="32px" MaxLength="1"></asp:textbox>
		<FMControls:FMLabel id="Label17" style="Z-INDEX: 108; LEFT: 280px; POSITION: absolute; TOP: 16px" runat="server"
			CssClass="formfieldtitle" Width="92px" BackColor="Transparent">Decimal Places:</FMControls:FMLabel>
		<asp:textbox id="DensityDecimalPlacesTextbox" ToolTip="Density Decimal Places" style="Z-INDEX: 107; LEFT: 280px; POSITION: absolute; TOP: 176px"
			tabIndex="10" runat="server" CssClass="formfield" Width="32px" MaxLength="1"></asp:textbox>
		<FMControls:FMDropDownList id="VolumeUnitsDropDownList" ToolTip="Volum eUnits" style="Z-INDEX: 106; LEFT: 176px; POSITION: absolute; TOP: 80px"
			tabIndex="3" runat="server" CssClass="formfield" Width="88px" AutoPostBack="True" Translate="false"></FMControls:FMDropDownList>
		<FMControls:FMLabel id="Label8" style="Z-INDEX: 105; LEFT: 0px; POSITION: absolute; TOP: 80px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Volume:</FMControls:FMLabel>
		<FMControls:FMDropDownList id="TemperatureUnitsDropDownList" ToolTip="Temperature Units" style="Z-INDEX: 104; LEFT: 176px; POSITION: absolute; TOP: 144px"
			tabIndex="7" runat="server" CssClass="formfield" Width="88px" AutoPostBack="True" Translate="false"></FMControls:FMDropDownList>
		<FMControls:FMLabel id="TemperatureUnitsLabel" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 144px"
			runat="server" CssClass="formfieldtitle" BackColor="Transparent">Temperature:</FMControls:FMLabel>
		<FMControls:FMDropDownList id="DensityUnitsDropDownList" ToolTip="Density Units" style="Z-INDEX: 102; LEFT: 176px; POSITION: absolute; TOP: 176px"
			tabIndex="9" runat="server" CssClass="formfield" Width="88px" AutoPostBack="True" Translate="false"></FMControls:FMDropDownList>
		<FMControls:FMLabel id="Label5" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 176px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Density:</FMControls:FMLabel>

