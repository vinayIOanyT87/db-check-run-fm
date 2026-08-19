<%@ Control Language="c#" CodeBehind="ProductAlarmsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductAlarmsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    </HEAD>
	<body>
        <FMControls:FMLabel ID="Label3" Style="z-index: 105; left: 0px; position: absolute; top: 16px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Density High Limit:</FMControls:FMLabel>
        <asp:TextBox ID="DensityHighLimitTextbox" AutoPostBack="true" Style="z-index: 106; left: 175px; position: absolute; top: 16px" runat="server"
            Width="88px" CssClass="formfield" OnTextChanged="DensityHighLimitTextboxTextChanged"></asp:TextBox>
        <FMControls:FMLabel ID="Label4" Style="z-index: 107; left: 0px; position: absolute; top: 48px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Density Low Limit:</FMControls:FMLabel>
        <asp:TextBox ID="DensityLowLimitTextbox" AutoPostBack="true" Style="z-index: 108; left: 175px; position: absolute; top: 47px;"
            runat="server" Width="88px" CssClass="formfield" OnTextChanged="DensityLowLimitTextboxTextChanged"></asp:TextBox>
        <FMControls:FMLabel ID="Label5" Style="z-index: 109; left: 0px; position: absolute; top: 80px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Density High Minus Deadband:</FMControls:FMLabel>
        <asp:TextBox ID="DensityHighMinusDeadbandTextbox" AutoPostBack="true" Style="z-index: 110; left: 175px; position: absolute; top: 80px" runat="server"
            Width="88px" CssClass="formfield" OnTextChanged="DensityHighMinusDeadbandTextboxTextChanged"></asp:TextBox>
        <FMControls:FMLabel ID="FMLabel1" Style="z-index: 109; left: 0px; position: absolute; top: 112px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Density Low Plus Deadband:</FMControls:FMLabel>
        <asp:TextBox ID="DensityLowPlusDeadbandTextbox" AutoPostBack="true" Style="z-index: 110; left: 175px; position: absolute; top: 112px"
            runat="server" Width="88px" CssClass="formfield" OnTextChanged="DensityLowPlusDeadbandTextboxTextChanged"></asp:TextBox>
        <FMControls:FMCheckBox ID="ApplyDensityLimitsCheckBox" Style="z-index: 111; left: 0px; position: absolute; top: 144px" runat="server" CssClass="formfieldtitle" Width="144px" TextAlign="Left" Text="Apply Density Limits:"></FMControls:FMCheckBox>
        <FMControls:FMLabel ID="Label8" Style="z-index: 117; left: 312px; position: absolute; top: 16px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Temperature HiHi Limit:</FMControls:FMLabel>
        <asp:TextBox ID="TemperatureHiHiLimitTextbox" Style="z-index: 118; left: 496px; position: absolute; top: 16px" runat="server" Width="88px" CssClass="formfield"></asp:TextBox>
        <FMControls:FMLabel ID="Label9" Style="z-index: 119; left: 312px; position: absolute; top: 48px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Temperature High Limit:</FMControls:FMLabel>
        <asp:TextBox ID="TemperatureHighLimitTextbox" Style="z-index: 120; left: 496px; position: absolute; top: 48px" runat="server" Width="88px" CssClass="formfield"></asp:TextBox>
        <FMControls:FMLabel ID="Label10" Style="z-index: 121; left: 312px; position: absolute; top: 80px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Temperature Low Limit:</FMControls:FMLabel>
        <asp:TextBox ID="TemperatureLowLimitTextbox" Style="z-index: 122; left: 496px; position: absolute; top: 80px" runat="server" Width="88px" CssClass="formfield"></asp:TextBox>
        <FMControls:FMLabel ID="Label11" Style="z-index: 123; left: 312px; position: absolute; top: 112px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Temperature LoLo Limit:</FMControls:FMLabel>
        <asp:TextBox ID="TemperatureLoLoLimitTextbox" Style="z-index: 124; left: 496px; position: absolute; top: 112px" runat="server" Width="88px" CssClass="formfield"></asp:TextBox>
        <FMControls:FMLabel ID="Label12" Style="z-index: 125; left: 312px; position: absolute; top: 144px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Temperature Deadband:</FMControls:FMLabel>
        <asp:TextBox ID="TemperatureDeadbandTextbox" Style="z-index: 126; left: 496px; position: absolute; top: 144px" runat="server" Width="88px" CssClass="formfield"></asp:TextBox>
        <FMControls:FMCheckBox ID="ApplyTemperatureLimitsCheckBox" Style="z-index: 127; left: 312px; position: absolute; top: 176px" runat="server" CssClass="formfieldtitle" Width="176px" TextAlign="Left" Text="Apply Temperature Limits:"></FMControls:FMCheckBox>
	</body>
</HTML>
