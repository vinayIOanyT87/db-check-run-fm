<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="CompanyShipperPage.ascx.cs" AutoEventWireup="True" Inherits="FMWebApp.CompanyShipperPage" %>
<HTML>
	<HEAD>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
	<FMControls:FMLabel id="Label4" AssociatedControlID="TypeDropDownList" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 16px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="104px">Type:</FMControls:FMLabel>
	<asp:dropdownlist id="TypeDropDownList" style="Z-INDEX: 104; LEFT: 128px; POSITION: absolute; TOP: 16px"
		runat="server" CssClass="formfield" Width="182px" tabIndex="1"></asp:dropdownlist>
	<FMControls:FMCheckBox id="AdditiveAccountingCheckBox" style="Z-INDEX: 105; LEFT: 0px; POSITION: absolute; TOP: 48px"
		runat="server" CssClass="formfieldtitle" Width="152px" TextAlign="Left" Text="Additive Accounting"
		Height="27px" tabIndex="2"></FMControls:FMCheckBox>
	</body>
</HTML>
