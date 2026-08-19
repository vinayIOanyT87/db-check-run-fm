<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchTotalAndAveragePage.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.DispatchTotalAndAveragePage" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
	<form id="form1" runat="server">
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<div id="content" style="position: absolute">
		<asp:Image ID="fadeImage" runat="server" Style="z-index: 100; left: 0px; top: 0px;
			position: absolute" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
		<FMControls:FMLabel ID="titleLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 8px; top: 8px; width: 800px" BackColor="Transparent" CssClass="headline">Dispatch Total And Average Calculations</FMControls:FMLabel>
		<FMControls:FMLabel ID="transactionsToIncludeLabel" Style="z-index: 118; position: absolute; left: 40px;
			top: 65px" runat="server" CssClass="formfieldtitle">Transactions to Include</FMControls:FMLabel>
		<asp:CheckBoxList ID="transactionAliasCheckBoxList" runat="server" Style="z-index: 118; left: 32px; position: absolute;
			top: 95px; width: 250px;" BorderColor="LightSteelBlue" BorderStyle="Solid"
			BorderWidth="1px" CssClass="formfieldtitle" CellPadding="3" CellSpacing="2" 
			RepeatColumns="1" AutoPostBack="True" OnSelectedIndexChanged="OnSelectionValueChanged"/>
		<FMControls:FMLabel ID="dataFieldToCalculateLabel" Style="z-index: 118; position: absolute; left: 340px;
			top: 65px" runat="server" CssClass="formfieldtitle">Data Field to Calculate</FMControls:FMLabel>
		<asp:Panel ID="fieldTypePanel" runat="server" Style="z-index: 118; left: 332px; position: absolute;
			top: 95px; width: 158px; height: 145px;" BorderColor="LightSteelBlue" BorderStyle="Solid"
			BorderWidth="1px" />
		<FMControls:FMRadioButton ID="rbQuantity" TabIndex="3" runat="server" Style="z-index: 118;
			left: 345px; position: absolute; top: 110px" GroupName="MeasurementTypeGroup"
			Text="Quantity" CssClass="formfieldtitle" AutoPostBack="True" OnCheckedChanged="OnSelectionValueChanged" />
		<FMControls:FMRadioButton ID="rbResponseTime" TabIndex="4" runat="server" Style="z-index: 118;
			left: 345px; position: absolute; top: 140px" GroupName="MeasurementTypeGroup" Text="Response Time"
			CssClass="formfieldtitle" AutoPostBack="True" OnCheckedChanged="OnSelectionValueChanged" />
		<FMControls:FMRadioButton ID="rbVariance" TabIndex="5" runat="server" Style="z-index: 118;
			left: 345px; position: absolute; top: 170px" GroupName="MeasurementTypeGroup" Text="Variance"
			CssClass="formfieldtitle" AutoPostBack="True" OnCheckedChanged="OnSelectionValueChanged" />
		<FMControls:FMRadioButton ID="rbFuelTime" TabIndex="6" runat="server" Style="z-index: 118;
			left: 345px; position: absolute; top: 200px" GroupName="MeasurementTypeGroup" Text="Fuel Time"
			CssClass="formfieldtitle" AutoPostBack="True" OnCheckedChanged="OnSelectionValueChanged" />
		<FMControls:FMLabel ID="totalAndAverageLabel" Style="z-index: 118; position: absolute; left: 548px;
			top: 65px" runat="server" CssClass="formfieldtitle">Total and Average</FMControls:FMLabel>
		<asp:Panel ID="averagesPanel" runat="server" Style="z-index: 118; left: 540px; position: absolute;
			top: 95px; width: 250px; height: 145px;" BorderColor="LightSteelBlue" BorderStyle="Solid"
			BorderWidth="1px" />
		<FMControls:FMLabel ID="averageLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 553px; top: 110px" Text="Average:" CssClass="formfieldtitle" />
		<asp:TextBox ID="averageTextBox" TabIndex="7" runat="server" Height="20" Style="z-index: 118;
			position: absolute; left: 553px; top: 130px; width: 124px;" CssClass="formfieldNoWrap" />
		<FMControls:FMLabel ID="averageUnitsLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 690px; top: 135px; height: 15px;" Text="Average Units" CssClass="formfieldNoWrap" />
		<FMControls:FMLabel ID="totalLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 553px; top: 170px" Text="Total:" CssClass="formfieldtitle" />
		<asp:TextBox ID="totalTextBox" TabIndex="8" runat="server" Height="20" Style="z-index: 118;
			position: absolute; left: 553px; top: 190px; width: 124px;" CssClass="formfieldNoWrap" />
		<FMControls:FMLabel ID="totalUnitsLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 690px; top: 195px; height: 15px;" Text="Total Units" CssClass="formfieldNoWrap" />
		<FMControls:FMButton ID="closeButton" runat="server" Style="z-index: 118; position: absolute;
			left: 710px; top: 255px" TabIndex="9" CssClass="formfieldtitle" Text="Close"
			Width="72px" OnClick="CloseButtonOnClick"></FMControls:FMButton>
	</div>
		</div>
	</form>
</body>
</html>
