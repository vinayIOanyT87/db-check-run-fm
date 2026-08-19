<%@ Page language="c#" Codebehind="FMReportDynamicSelectionPage.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMReportWebMain.FMReportDynamicSelectionPage" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
			    <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			    <asp:label id="ReportPageTitle" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 8px"
				    runat="server" Width="640px" BackColor="Transparent" CssClass="headline">Reports</asp:label>
			    <asp:Panel id="MainPanel" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 40px" runat="server"
				    Width="984px" Height="672px"></asp:Panel>
            </div>
		</form>
	</body>
</HTML>
