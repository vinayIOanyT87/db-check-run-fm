<%@ Control Language="c#" AutoEventWireup="True" Codebehind="StationWeightScalePage.ascx.cs" Inherits="FuelsManager.FMWebApp.StationWeightScalePage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
<head>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body>
	<FMControls:FMLabel ID="Fmlabel1" AssociatedContolID="SystemDropDownList" Style="z-index: 102; left: 0px; position: absolute; top: 16px" runat="server"
		Width="80px" CssClass="formfieldtitle">System:</FMControls:FMLabel>
	<FMControls:FMDropDownList ID="SelectSystemModeDropDownList" Style="z-index: 106; left: 128px; position: absolute; top: 16px"
		TabIndex="1" runat="server" Width="58px" CssClass="formfield" Height="24px" AutoPostBack="True" OnSelectedIndexChanged="SelectSystemModeDropDownList_SelectedIndexChanged">
	</FMControls:FMDropDownList>
	<asp:DropDownList ID="SystemDropDownList" Style="z-index: 106; left: 198px; position: absolute; top: 16px"
		runat="server" Width="170px" CssClass="formfield" AutoPostBack="True" TabIndex="2" OnSelectedIndexChanged="SystemDropDownList_SelectedIndexChanged">
	</asp:DropDownList>
	<asp:TextBox ID="SystemTextBox" ToolTip="System Textbox" Style="z-index: 106; left: 198px; position: absolute; top: 16px"
		TabIndex="2" runat="server" Width="170px" CssClass="formfield" AutoPostBack="True" MaxLength="80" OnTextChanged="SystemTextBox_TextChanged"></asp:TextBox>
	<FMControls:FMLabel ID="Label4" AssociatedContolID="OPCServerDropDownList" Style="z-index: 102; left: 0px; position: absolute; top: 48px" runat="server"
		Width="80px" CssClass="formfieldtitle">OPC Server:</FMControls:FMLabel>
	<asp:DropDownList ID="OPCServerDropDownList" Style="z-index: 102; left: 128px; position: absolute; top: 48px"
		TabIndex="3" runat="server" Width="240px" CssClass="formfield">
	</asp:DropDownList>
	<FMControls:FMCheckBox ID="InhibitOperatingModePromptCheckBox" Style="z-index: 103; left: 400px; position: absolute; top: 16px"
		TabIndex="10" runat="server" CssClass="formfieldtitle" Text="Inhibit Operating Mode Prompt"></FMControls:FMCheckBox>
	<FMControls:FMCheckBox ID="SetPreloadToZeroCheckBox" Style="z-index: 103; left: 400px; position: absolute; top: 36px"
		TabIndex="11" runat="server" CssClass="formfieldtitle" Text="Set Default Preset To Zero"></FMControls:FMCheckBox>
	<FMControls:FMCheckBox ID="InhibitLoadingByLoadIDCheckBox" Style="z-index: 103; left: 400px; position: absolute; top: 56px"
		TabIndex="2" runat="server" CssClass="formfieldtitle" Text="Inhibit Loading By LoadID"></FMControls:FMCheckBox>
	<FMControls:FMLabel ID="Label5" AssociatedContolID="OPCItemPathTextBox" Style="z-index: 102; left: 0px; position: absolute; top: 80px; width: 103px;" runat="server" CssClass="formfieldtitle">OPC Item Path:</FMControls:FMLabel>
	<asp:TextBox ID="OPCItemPathTextBox" Style="z-index: 103; left: 128px; position: absolute; top: 80px"
		runat="server" Width="240px" CssClass="formfield" MaxLength="256" BackColor="White"></asp:TextBox>
	<FMControls:FMLabel ID="Label1" AssociatedContolID="BOLPrinterDropDownList" Style="z-index: 120; left: 0px; position: absolute; top: 112px" runat="server"
		Width="80px" CssClass="formfieldtitle" BackColor="Transparent">BOL Printer:</FMControls:FMLabel>
	<asp:DropDownList ID="BOLPrinterDropDownList" Style="z-index: 121; left: 128px; position: absolute; top: 112px"
		runat="server" Width="240px" CssClass="formfield" TabIndex="4">
	</asp:DropDownList>
	<FMControls:FMLabel ID="NumberOfCopiesLabel" AssociatedContolID="NumberOfCopiesTextBox" Style="z-index: 120; left: 400px; position: absolute; top: 112px"
		runat="server" Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Number of copies:</FMControls:FMLabel>
	<asp:TextBox ID="NumberOfCopiesTextBox" Style="z-index: 120; left: 512px; position: absolute; top: 112px"
		runat="server" Width="56px" CssClass="formfield" MaxLength="2" Columns="2" TabIndex="12"></asp:TextBox>
	<FMControls:FMLabel ID="PreloadLabel" AssociatedContolID="PreloadPrinterDropDownlist" Style="z-index: 103; left: 0px; position: absolute; top: 144px"
		runat="server" Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Preload Printer:</FMControls:FMLabel>
	<asp:DropDownList ID="PreloadPrinterDropDownlist" Style="z-index: 103; left: 128px; position: absolute; top: 144px"
		runat="server" Width="240px" CssClass="formfield" TabIndex="5">
	</asp:DropDownList>
	<FMControls:FMLabel ID="PreloadNumberOfCopiesLabel" AssociatedContolID="PreloadNumberOfCopiesTextBox" Style="z-index: 103; left: 400px; position: absolute; top: 144px"
		runat="server" Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Number of copies:</FMControls:FMLabel>
	<asp:TextBox ID="PreloadNumberOfCopiesTextBox" Style="z-index: 103; left: 512px; position: absolute; top: 144px"
		runat="server" Width="56px" CssClass="formfield" MaxLength="2" Columns="2" TabIndex="13"></asp:TextBox>
	<FMControls:FMLabel ID="Label3" Style="z-index: 109; left: 0px; position: absolute; top: 176px" runat="server"
		Width="80px" CssClass="formfieldtitle" BackColor="Transparent">Transactions:</FMControls:FMLabel>
	<FMControls:FMLabel ID="Label16" AssociatedContolID="IssueByVolumeTransactionDropDownList" Style="z-index: 109; left: 0px; position: absolute; top: 208px" runat="server"
		Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Issue By Volume:</FMControls:FMLabel>
	<asp:DropDownList ID="IssueByVolumeTransactionDropDownList" Style="z-index: 102; left: 128px; position: absolute; top: 208px"
		TabIndex="6" runat="server" Width="240px" CssClass="formfield">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Fmlabel2" AssociatedContolID="IssueByWeightTransactionDropDownList" Style="z-index: 109; left: 0px; position: absolute; top: 240px" runat="server"
		Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Issue By Weight:</FMControls:FMLabel>
	<asp:DropDownList ID="IssueByWeightTransactionDropDownList" Style="z-index: 102; left: 128px; position: absolute; top: 240px"
		TabIndex="7" runat="server" Width="240px" CssClass="formfield">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Fmlabel3" AssociatedContolID="ReceiptByVolumeTransactionDropDownList" Style="z-index: 109; left: 0px; position: absolute; top: 272px; width: 121px;" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Receipt By Volume:</FMControls:FMLabel>
	<asp:DropDownList ID="ReceiptByVolumeTransactionDropDownList" Style="z-index: 102; left: 128px; position: absolute; top: 272px"
		TabIndex="8" runat="server" Width="240px" CssClass="formfield">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Fmlabel4" AssociatedContolID="ReceiptByWeightTransactionDropDownList" Style="z-index: 109; left: 0px; position: absolute; top: 304px; width: 120px;" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Receipt By Weight:</FMControls:FMLabel>
	<asp:DropDownList ID="ReceiptByWeightTransactionDropDownList" Style="z-index: 102; left: 128px; position: absolute; top: 304px"
		TabIndex="9" runat="server" Width="240px" CssClass="formfield">
	</asp:DropDownList>
</body>
</html>
