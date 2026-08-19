<%@ Page language="c#" Codebehind="GraphicsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.GraphicsForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title></title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
  </HEAD>
  <body MS_POSITIONING="GridLayout" tabindex=-1>
	
    <form id="Form1" method="post" runat="server">
			<asp:Image id="Image1" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px"
				runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<asp:Label id="Label6" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="136px" BackColor="Transparent">Graphics</asp:Label>

     </form>
	
  </body>
</HTML>
