<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FCRC_UserDataPage.ascx.cs" Inherits="FuelsManager.FuelCardWebApp.FCRC_UserDataPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
	<asp:table id="UserDataTable" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 16px"
		runat="server" ForeColor="Transparent" BackColor="Transparent" Width="600px" CellSpacing="0"
		CellPadding="0" EnableViewState="False"></asp:table>
	</body>
</HTML>
