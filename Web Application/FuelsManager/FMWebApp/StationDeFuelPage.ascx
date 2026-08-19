<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="StationDeFuelPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.StationDeFuelPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<html>
<head>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body>
	<FMControls:FMLabel ID="Label2" AssociatedControlID="SwingArmPositionDropDownList" Style="z-index: 106; left: 0px; position: absolute; top: 16px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Swing Arm Position:</FMControls:FMLabel>
	<asp:DropDownList ID="SwingArmPositionDropDownList" Style="z-index: 107; left: 152px; position: absolute; top: 16px"
		runat="server" Width="48px" CssClass="formfield" TabIndex="1">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Label1" AssociatedControlID="BOLPrinterDropDownList" Style="z-index: 120; left: 296px; position: absolute; top: 16px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="80px">BOL Printer:</FMControls:FMLabel>
	<asp:DropDownList ID="BOLPrinterDropDownList" Style="z-index: 121; left: 448px; position: absolute; top: 16px"
		runat="server" CssClass="formfield" Width="240px" TabIndex="5">
	</asp:DropDownList>
	<FMControls:FMCheckBox ID="OffLoafbyOffLoadingID" Style="z-index: 103; left: 0px; position: absolute; top: 48px"
		runat="server" Text="Off-Load by Off-Load ID" CssClass="formfieldtitle"
		TabIndex="2"></FMControls:FMCheckBox>
	<FMControls:FMLabel ID="NumberOfCopiesLabel" AssociatedControlID="NumberOfCopiesTextBox" Style="z-index: 120; left: 296px; position: absolute; top: 48px"
		runat="server" CssClass="formfieldtitle" Width="112px" BackColor="Transparent">Number of copies:</FMControls:FMLabel>
	<asp:TextBox ID="NumberOfCopiesTextBox" Style="z-index: 120; left: 448px; position: absolute; top: 48px"
		runat="server" Width="80px" Columns="2" MaxLength="2" CssClass="formfield"
		TabIndex="6"></asp:TextBox>
	<FMControls:FMCheckBox ID="SynchronizeReferenceDensityCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 80px"
		runat="server" Text="Synchronize Reference Density" CssClass="formfieldtitle"
		TabIndex="3"></FMControls:FMCheckBox>
	<FMControls:FMLabel ID="Label16" AssociatedControlID="ReceiptTransactionDropDownList" Style="z-index: 109; left: 296px; position: absolute; top: 80px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="128px">Receipt Transaction:</FMControls:FMLabel>
	<asp:DropDownList ID="ReceiptTransactionDropDownList" Style="z-index: 102; left: 448px; position: absolute; top: 80px"
		TabIndex="7" runat="server" CssClass="formfield" Width="240px">
	</asp:DropDownList>
	<FMControls:FMCheckBox ID="UseManualMeterData" Style="z-index: 103; left: 0px; position: absolute; top: 112px"
		runat="server" Text="Use Manual Meter Data" CssClass="formfieldtitle" TabIndex="4"></FMControls:FMCheckBox>
	<input class="formfieldtitle" id="StationPermissivesButton" onclick="PermissivesButton_Click('StationPermissives', '0')"
		type="button" value="Station Permissives" runat="server" name="StationPermissivesButton"
		style="z-index: 105; left: 448px; width: 240px; position: absolute; top: 112px; height: 22px"
		tabindex="8">
	<FMControls:FMCheckBox ID="PromptForBOLNumber" Style="z-index: 103; left: 0px; position: absolute; top: 144px"
		runat="server" Text="Prompt for BOL Number" CssClass="formfieldtitle" TabIndex="4"></FMControls:FMCheckBox>
	<FMControls:FMCheckbox id="PromptForGravity" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 176px"
		runat="server" Text="Prompt For Gravity" CssClass="formfieldtitle" tabIndex="4"></FMControls:FMCheckbox>
	<FMControls:FMCheckbox id="PromptForTemperature" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 208px"
		runat="server" Text="Prompt For Temperature" CssClass="formfieldtitle" tabIndex="4"></FMControls:FMCheckbox>
	<FMControls:FMLabel ID="Label17" AssociatedControlID="MeterRecircCardNumber"
		Style="z-index: 109; left: 296px; position: absolute; top: 147px; width: 146px;" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle">Meter Recirc Card No:</FMControls:FMLabel>
	<asp:TextBox ID="MeterRecircCardNumber" Style="z-index: 120; left: 448px; position: absolute; top: 145px"
		runat="server" Width="240px" Columns="2" MaxLength="30" CssClass="formfield"
		TabIndex="10"></asp:TextBox>
	<FMControls:FMLabel ID="Label18" AssociatedControlID="RecircTransactionDropDownList"
		Style="z-index: 109; left: 296px; position: absolute; top: 178px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="128px">Recirc Transaction:</FMControls:FMLabel>
	<asp:DropDownList ID="RecircTransactionDropDownList" Style="z-index: 102; left: 448px; position: absolute; top: 177px"
		TabIndex="8" runat="server" CssClass="formfield" Width="240px">
	</asp:DropDownList>
</body>
</html>
