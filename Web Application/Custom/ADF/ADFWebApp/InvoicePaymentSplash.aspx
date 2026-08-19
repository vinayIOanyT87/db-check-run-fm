<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="InvoicePaymentSplash.aspx.cs" AutoEventWireup="True" Inherits="ADFWebApp.InvoicePaymentSplashForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Invoice Payment Splash</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link href="../FuelsManager.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="InvoiceSplashForm" method="post" runat="server">
		    <asp:Image id="Image1" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
			    ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMLabel id="Label1" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="400px" BackColor="Transparent">Invoices and Payments</FMControls:FMLabel>
		</form>
	</body>
</HTML>
