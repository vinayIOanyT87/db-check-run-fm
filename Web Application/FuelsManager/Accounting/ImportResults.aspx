<%@ Page language="c#" Codebehind="ImportResults.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.ImportResults" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css"%>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:Label id="Label1" style="Z-INDEX: 101; LEFT: 32px; POSITION: absolute; TOP: 32px" runat="server">Import Results</asp:Label>
		</form>
	</body>
</HTML>
