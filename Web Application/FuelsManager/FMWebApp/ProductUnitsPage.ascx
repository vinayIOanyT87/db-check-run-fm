<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="true" CodeBehind="ProductUnitsPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProductUnitsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<html>
<head>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
    <FMControls:FMLabel ID="Label18" Style="z-index: 110; left: 176px; position: absolute; top: 48px" runat="server"
        CssClass="formfieldtitle" Width="39px" BackColor="Transparent">Units:</FMControls:FMLabel>
    <FMControls:FMLabel ID="Label17" Style="z-index: 108; left: 280px; position: absolute; top: 48px" runat="server"
        CssClass="formfieldtitle" Width="92px" BackColor="Transparent">Decimal Places:</FMControls:FMLabel>

    <FMControls:FMLabel ID="FMLabel1" Style="z-index: 105; left: 0px; position: absolute; top: 80px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Level:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="LevelUnitsDropDownList" ToolTip="Level units" Style="z-index: 106; left: 176px; position: absolute; top: 80px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
    </FMControls:FMDropDownList>
    <asp:TextBox ID="LevelDecimalPlacesTextbox" ToolTip="Level decimal places" Style="z-index: 111; left: 280px; position: absolute; top: 80px"
        runat="server" CssClass="formfield" Width="32px" AutoPostBack="True" MaxLength="1"></asp:TextBox>

    <FMControls:FMLabel ID="Label8" Style="z-index: 105; left: 0px; position: absolute; top: 112px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Volume:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="VolumeUnitsDropDownList" ToolTip="Volume units" Style="z-index: 106; left: 176px; position: absolute; top: 112px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
    </FMControls:FMDropDownList>
    <asp:TextBox ID="VolumeDecimalPlacesTextbox" ToolTip="Volume decimal places" Style="z-index: 111; left: 280px; position: absolute; top: 112px"
        runat="server" CssClass="formfield" Width="32px" AutoPostBack="True"
        OnTextChanged="VolumeDecimalPlacesTextChanged" MaxLength="1"></asp:TextBox>

    <FMControls:FMLabel ID="FMLabel2" Style="z-index: 105; left: 0px; position: absolute; top: 144px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Temperature:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="TemperatureUnitsDropDownList" ToolTip="Temperature units" Style="z-index: 106; left: 176px; position: absolute; top: 144px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
    </FMControls:FMDropDownList>
    <asp:TextBox ID="TemperatureDecimalPlacesTextbox" ToolTip="Temperature decimal places" Style="z-index: 111; left: 280px; position: absolute; top: 144px"
        runat="server" CssClass="formfield" Width="32px" AutoPostBack="True" MaxLength="1"></asp:TextBox>

    <FMControls:FMLabel ID="FMLabel3" Style="z-index: 105; left: 0px; position: absolute; top: 176px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Density:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="DensityUnitsDropDownList" ToolTip="Density units" Style="z-index: 106; left: 176px; position: absolute; top: 176px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
    </FMControls:FMDropDownList>
    <asp:TextBox ID="DensityDecimalPlacesTextbox" ToolTip="Density decimal places" Style="z-index: 111; left: 280px; position: absolute; top: 176px;"
        runat="server" CssClass="formfield" Width="32px" AutoPostBack="True" MaxLength="1"></asp:TextBox>

    <FMControls:FMLabel ID="FMLabel4" Style="z-index: 105; left: 0px; position: absolute; top: 208px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Mass:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="MassUnitsDropDownList" ToolTip="Mass units" Style="z-index: 106; left: 176px; position: absolute; top: 208px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
    </FMControls:FMDropDownList>
    <asp:TextBox ID="MassDecimalPlacesTextbox" ToolTip="Mass decimal places" Style="z-index: 111; left: 280px; position: absolute; top: 208px;"
        runat="server" CssClass="formfield" Width="32px" AutoPostBack="True"
        OnTextChanged="MassDecimalPlacesTextChanged" MaxLength="1"></asp:TextBox>

    <FMControls:FMLabel ID="FMLabel5" Style="z-index: 105; left: 0px; position: absolute; top: 240px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Flow:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="FlowUnitsDropDownList" ToolTip="Flow units" Style="z-index: 106; left: 176px; position: absolute; top: 240px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
    </FMControls:FMDropDownList>
    <asp:TextBox ID="FlowDecimalPlacesTextbox" ToolTip="Flow decimal places" Style="z-index: 111; left: 280px; position: absolute; top: 240px"
        runat="server" CssClass="formfield" Width="32px" AutoPostBack="True" MaxLength="1"></asp:TextBox>

    <FMControls:FMLabel ID="FMLabel6" Style="z-index: 105; left: 0px; position: absolute; top: 272px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Pressure:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="PressureUnitsDropDownList" ToolTip="Pressure units" Style="z-index: 106; left: 176px; position: absolute; top: 272px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
    </FMControls:FMDropDownList>
    <asp:TextBox ID="PressureDecimalPlacesTextbox" ToolTip="Pressure decimal places" Style="z-index: 111; left: 280px; position: absolute; top: 272px"
        runat="server" CssClass="formfield" Width="32px" AutoPostBack="True" MaxLength="1"></asp:TextBox>

    <FMControls:FMLabel ID="FMLabel7" Style="z-index: 105; left: 0px; position: absolute; top: 304px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Packaged Size:</FMControls:FMLabel>

    <FMControls:FMLabel ID="FMLabel8" AssociatedControlID="VolumePackageSizeTextbox" Style="z-index: 105; left: 0px; position: absolute; top: 336px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Volume:</FMControls:FMLabel>
    <asp:TextBox ID="VolumePackageSizeTextbox" Style="z-index: 106; left: 176px; position: absolute; top: 336px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True"
        OnTextChanged="VolumePackageSizeTextChanged"></asp:TextBox>
    <FMControls:FMLabel ID="VolumePackageSizelbl" Style="z-index: 111; left: 280px; position: absolute; top: 336px"
        runat="server" CssClass="formfield" Width="320px"></FMControls:FMLabel>

    <FMControls:FMLabel ID="FMLabel10" AssociatedControlID="MassPackageSizeTextbox" Style="z-index: 105; left: 0px; position: absolute; top: 368px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Mass:</FMControls:FMLabel>
    <asp:TextBox ID="MassPackageSizeTextbox" Style="z-index: 106; left: 176px; position: absolute; top: 368px"
        runat="server" CssClass="formfield" Width="88px" AutoPostBack="True"
        OnTextChanged="MassPackageSizeTextChanged"></asp:TextBox>
    <FMControls:FMLabel ID="MassPackageSizelbl" Style="z-index: 111; left: 280px; position: absolute; top: 368px"
        runat="server" CssClass="formfield" Width="320px"></FMControls:FMLabel>

</body>
</html>
