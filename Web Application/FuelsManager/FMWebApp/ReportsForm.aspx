<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="ReportsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ReportsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout" tabindex=-1>
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<asp:Image id="Image1" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px"
				runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMLabel id="Label6" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="96px" BackColor="Transparent">Reports</FMControls:FMLabel>
		</div>
</form>
	</body>
</HTML>
