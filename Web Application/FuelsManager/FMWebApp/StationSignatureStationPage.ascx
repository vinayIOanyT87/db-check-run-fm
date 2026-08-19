<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="StationSignatureStationPage.ascx.cs" Inherits="FuelsManager.FMWebApp.StationSignatureStationPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
<FMControls:FMLabel id="SignatureCaptureLabel" AssociatedControlID="SignatureDeviceTextBox" runat="server" Width="112px" CssClass="formfieldtitle"
	BackColor="Transparent" style="Z-INDEX: 120; LEFT: 0px; POSITION: absolute; TOP: 8px">Signature Device:</FMControls:FMLabel>
<asp:TextBox id="SignatureDeviceTextBox" style="Z-INDEX: 120; LEFT: 200px; POSITION: absolute; TOP: 8px"
	runat="server" Width="80px" MaxLength="2" Columns="2" CssClass="formfield"></asp:TextBox>
<FMControls:FMLabel id="FMLabel1" AssociatedControlID="SignatureCapturePort" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="160px"
	style="Z-INDEX: 120; LEFT: 0px; POSITION: absolute; TOP: 40px">Signature Device Serial Port</FMControls:FMLabel>
<asp:TextBox id="SignatureCapturePort" style="Z-INDEX: 120; LEFT: 200px; POSITION: absolute; TOP: 40px"
	runat="server" Width="80px" MaxLength="2" Columns="2" CssClass="formfield"></asp:TextBox>
<FMControls:FMLabel id="Fmlabel2" AssociatedControlID="SignatureCaptureBaudRate" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="160px"
	style="Z-INDEX: 120; LEFT: 0px; POSITION: absolute; TOP: 72px">Signature Device Baud Rate</FMControls:FMLabel>
<asp:TextBox id="SignatureCaptureBaudRate" style="Z-INDEX: 120; LEFT: 200px; POSITION: absolute; TOP: 72px"
	runat="server" Width="80px" MaxLength="6" Columns="2" CssClass="formfield"></asp:TextBox>
	</body>
</HTML>
