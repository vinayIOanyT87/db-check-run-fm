<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="ProductUserDataPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductUserDataPage" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
	<asp:table id="UserDataTable" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 16px"
		runat="server" ForeColor="Transparent" BackColor="Transparent" Width="600px" CellSpacing="0"
		CellPadding="0" EnableViewState="False" role="presentation" aria-label="layout"></asp:table>
	</body>
</HTML>
