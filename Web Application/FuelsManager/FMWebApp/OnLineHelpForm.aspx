<%@ Page language="c#" Codebehind="OnLineHelpForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.OnLineHelpForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
    <title></title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
  </HEAD>
  <body MS_POSITIONING="GridLayout" tabindex=-1>
	
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
<asp:Label id=Label1 style="Z-INDEX: 101; LEFT: 32px; POSITION: absolute; TOP: 32px" runat="server" CssClass="formfieldtitle" Width="448px" Height="24px" Font-Size="X-Large">Help is Under Construction</asp:Label>

     </div>
</form>
	
  </body>
</HTML>
