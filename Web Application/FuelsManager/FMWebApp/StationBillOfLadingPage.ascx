<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="StationBillOfLadingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.StationBillOfLadingPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<html>
<head>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body>
	<FMControls:FMLabel ID="Label1" AssociatedControlID="BOLPrinterDropDownList" Style="z-index: 120; left: 0px; position: absolute; top: 16px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="80px">BOL Printer:</FMControls:FMLabel>
	<asp:DropDownList ID="BOLPrinterDropDownList" Style="z-index: 121; left: 200px; position: absolute; top: 16px"
		runat="server" CssClass="formfield" Width="240px">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Fmlabel1" AssociatedControlID="BOLAgeInMinutesTextBox" Style="z-index: 120; left: 0px; position: absolute; top: 48px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="112px">Print BOL's within:</FMControls:FMLabel>
	<asp:TextBox ID="BOLAgeInMinutesTextBox" Style="z-index: 103; left: 200px; position: absolute; top: 48px"
		runat="server" CssClass="formfield" BackColor="White" Width="80px" MaxLength="256"></asp:TextBox>
	<FMControls:FMLabel ID="Fmlabel2" Style="z-index: 120; left: 296px; position: absolute; top: 48px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="56px">Minutes</FMControls:FMLabel>
	<FMControls:FMLabel ID="NumberOfCopiesLabel" AssociatedControlID="NumberOfCopiesTextBox" runat="server" Width="112px" CssClass="formfieldtitle"
		BackColor="Transparent" Style="z-index: 120; left: 0px; position: absolute; top: 85px">Number of copies:</FMControls:FMLabel>
	<asp:TextBox ID="NumberOfCopiesTextBox" Style="z-index: 120; left: 200px; position: absolute; top: 80px"
		runat="server" Width="80px" MaxLength="2" Columns="2" CssClass="formfield"></asp:TextBox>
	<FMControls:FMLabel ID="SignatureCaptureLabel" AssociatedControlID="SignatureDeviceTextBox" runat="server" Width="112px" CssClass="formfieldtitle"
		BackColor="Transparent" Style="z-index: 120; left: 0px; position: absolute; top: 120px">Signature Device:</FMControls:FMLabel>
	<asp:TextBox ID="SignatureDeviceTextBox" Style="z-index: 120; left: 200px; position: absolute; top: 120px"
		runat="server" Width="80px" MaxLength="2" Columns="2" CssClass="formfield"></asp:TextBox>
	<FMControls:FMLabel ID="Fmlabel3" AssociatedControlID="SignatureCapturePort" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="160px"
		Style="z-index: 120; left: 0px; position: absolute; top: 160px">Signature Device Serial Port</FMControls:FMLabel>
	<asp:TextBox ID="SignatureCapturePort" Style="z-index: 120; left: 200px; position: absolute; top: 160px"
		runat="server" Width="80px" MaxLength="2" Columns="2" CssClass="formfield"></asp:TextBox>
	<FMControls:FMLabel ID="Fmlabel4" AssociatedControlID="SignatureCaptureBaudRate" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="160px"
		Style="z-index: 120; left: 0px; position: absolute; top: 200px">Signature Device Baud Rate</FMControls:FMLabel>
	<asp:TextBox ID="SignatureCaptureBaudRate" Style="z-index: 120; left: 200px; position: absolute; top: 200px"
		runat="server" Width="80px" MaxLength="6" Columns="2" CssClass="formfield"></asp:TextBox>
</body>
</html>
