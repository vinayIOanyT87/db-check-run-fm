<%@ Page language="c#" Codebehind="ReportSplashPage.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMReportWebMain.ReportSplashPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<asp:Label id="ReportTitleLabel" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" CssClass="headline" Width="96px" BackColor="Transparent">Reporting</asp:Label>
		</form>
	</body>
</HTML>
