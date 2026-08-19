<%@ Page language="c#" Codebehind="Import.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.Import" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:DropDownList id="ImportDropDown" style="Z-INDEX: 101; LEFT: 208px; POSITION: absolute; TOP: 168px"
				runat="server"></asp:DropDownList>
			<asp:Button id="GoButton" style="Z-INDEX: 102; LEFT: 216px; POSITION: absolute; TOP: 264px"
				runat="server" Text="Go" onclick="GoButtonClick"></asp:Button>
		</form>
	</body>
</HTML>
