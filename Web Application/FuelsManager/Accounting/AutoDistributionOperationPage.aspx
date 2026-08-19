<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoDistributionOperationPage.aspx.cs"
	Inherits="FuelsManager.Accounting.AutoDistributionOperationPage" %>

<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Import Namespace="FMBusinessObjects.DataObjects" %>
<%@ Import Namespace="FuelsManager.Accounting" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<link type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<style type="text/css">
		
		html
		{
			overflow-y: hidden;
			overflow-x: auto;
			background-color: transparent;
			height:100%;
			width:100%;
		}
		
		body
		{
			height:100%;
		}
		
		/*--------------- Common/Shared ---------------*/
		.standardTableWidth
		{
			width: 990px;
		}
		.standardDivWidth
		{
			width: 1020px;
		}
		.nowrap
		{
			white-space: nowrap;
		}
		
		/*--------------- info ---------------*/
		/* columns here don't have much meaning, just how the table layout to space out the fields */
		#infoCol1, #infoCol3, #infoCol5, #infoCol6, #infoCol8, #infoCol9
		{
			width: 100px;
		}
		#infoCol2
		{
			width: 50px;
		}
		
		#infoCol4, #infoCol7
		{
			width: 10px;
		}
		.dropDownList /* rules, managers, products and reason codes */
		{
			width: 168px;
		}
		.descriptionTextBox
		{
			width: 300px;
		}
		.standardTextBox
		{
			width: 180px;
		}
		.quantityTextBox
		{
			width: 108px;
		}
		/*--------------- grid ---------------*/
		.gridHeader, .gridFooter
		{
			table-layout: fixed;
		}
		.gridShared
		{
			table-layout: fixed;
			border: 0px solid transparent;
		}
		.tableHeader
		{
			color: White;
			background-color: #0D246A;
			font-weight: bold;
		}
		.captionRowCell /* this is the row with the word distributions */
		{
			border-bottom: 1px solid transparent;
			border-left: 1px solid transparent;
			text-align: center;
		}
		.headerRow1Cell /* this is the row has the words gross, net, mass */
		{
			border-bottom: 1px solid white;
			border-left: 1px solid white;
			text-align: center;
		}
		
		.footerBorder
		{
			border-left: 1px solid white;
		}
		.ownerColumn
		{
			text-align: left;
			width: 150px;
			border-left: 1px solid white;
		}
		.grossThruputColumn, .netThruputColumn, .massThruputColumn
		{
			text-align: right;
			width: 80px;
			border-left: 1px solid white;
		}
		.grossQuantityColumn, .netQuantityColumn, .massQuantityColumn
		{
			text-align: right;
			width: 70px;
			border-left: 1px solid white;
		}
		.grossThruputPercentColumn, .grossQuantityPercentColumn, .netThruputPercentColumn, .netQuantityPercentColumn, .massThruputPercentColumn, .massQuantityPercentColumn
		{
			text-align: right;
			width: 52px;
			border-left: 1px solid white;
			padding: 2px;
		}
		.grossQuantityCell, .netQuantityCell, .massQuantityCell
		{
			text-align: right;
			width: 94%;
			border-left: 1px solid white;
		}
		/*--------------- divs ---------------*/
		#floatingDiv /* this the standard background and page title */
		{
			position: absolute;
			top: 0px;
			left: 0px;
		}
		#topOuterDiv
		{
			position: absolute;
			left: 0px;
			bottom: 56px;
			width: 100%;
			height: auto;
			overflow-y: auto;
			overflow-x: hidden;
			background-color: transparent;
		}
		#topDiv
		{
			position: absolute;
			margin-left: 12px;
			background-color: transparent;
			margin-right: 17px;
			height:auto;
		}
		#bottomOuterDiv
		{
			position: absolute;
			left: 12px;
			bottom: 0px;
			height: 56px;
		}
		#bottomDiv
		{
			position: absolute;
			left: 0px;
			bottom: 0px;
			height: 56px;
			background-color: white;
		}
	</style>
	<%------------------------------------ Scripts ------------------------------------%>
	<script type='text/javascript'>
		// When the page resizes, we need to move the footer of the grid and 
		// the create butotn according to the size of the top div
		function pageResize() {


			if (window.bottomOuterDiv)
			{
				// initialize variabes
				var menuDiv = document.getElementById("menuDiv");
				var bottomDiv = document.getElementById("bottomOuterDiv");
				var topOuterDiv = document.getElementById("topOuterDiv");
				var topDiv = document.getElementById("topDiv");

				var parentHeight = document.body.clientHeight;
				var hasScrollBar = topOuterDiv.scrollHeight > topOuterDiv.offsetHeight + 1;
				
				// topOuterDiv is the scrollable div
				// we need to fix the top and height

				// top is right below menu's bottom
				var newTopDivTop = menuDiv.clientTop + menuDiv.clientHeight;
				if (newTopDivTop != topOuterDiv.offsetTop)
				{
					topOuterDiv.style.top = newTopDivTop  + "px";
				}

				var newTopDivHeight = (parentHeight - menuDiv.clientHeight - bottomDiv.clientHeight);
				if (!hasScrollBar && newTopDivHeight > topDiv.clientHeight)
				{
					newTopDivHeight = topDiv.clientHeight;
				}
				if (newTopDivHeight != topOuterDiv.clientHeight)
				{
					topOuterDiv.style.height = newTopDivHeight  + "px";
					hasScrollBar = topOuterDiv.scrollHeight > topOuterDiv.offsetHeight + 1;
				}
				var newBottomDivTop = topOuterDiv.offsetTop + topOuterDiv.scrollHeight;
				if (hasScrollBar && 
					(topOuterDiv.offsetHeight > 0)) {
					// make the bottomDiv stays at the bottom of the page.
					bottomDiv.style.top = "";
					bottomDiv.style.bottom = "0px";
				}
				else {
					// make the bottomDiv right after the top div.
					bottomDiv.style.bottom = "";
					bottomDiv.style.top = newBottomDivTop + "px";
				}
				syncDivs();
			}
		}

		// This is trying to restore the focus of the page after postback
		function restoreFocus() {
			try {
				var control = document.getElementById('restoreFocusControlID');

				if (control.value != undefined &&
					control.value != '') {

					var targetControl = document.getElementById(control.value);

					var start = document.getElementById('lastFocusControlSelectionStart').value;
					var end = document.getElementById('lastFocusControlSelectionEnd').value;

					if (targetControl != undefined ) {
						targetControl.focus();

						var oRange = targetControl.createTextRange();
						oRange.moveStart("character", start);
						oRange.moveEnd("character", end);
						oRange.select();
					}
				}
			}
			catch (error) {
			}
		}

		// This is to save the current control before postback
		function SaveFocus()
		{
			try {
				var control = document.activeElement;
				document.getElementById('lastFocusControlID').value = control.id;

				var selectedText = document.selection;
				var start = 0;
				var end = 0;

				if (selectedText != undefined) {

					var selectedRange = document.selection.createRange();

					if (selectedRange != undefined) {

						var tempRange = document.selection.createRange().duplicate();

						start = control.value.length;
			            while (tempRange.parentElement()== control && tempRange.move("character",1)==1)
						{
					        --start;
						}
						end = start + selectedRange.text.length;
					}
				}

				document.getElementById('lastFocusControlSelectionStart').value = start;
				document.getElementById('lastFocusControlSelectionEnd').value = end;
			}
			catch (error)
			{
			}
		}

		function onSubmitHandler() {
			SaveFocus();
		}

		function confirmCreate() {
		    return confirm('Are you sure you want to distribute adjustment amounts for each owner?');
		}

		function fmdate_onChangeDelayed(textboxID)
		{
			SaveFocus();
			<%= Page.ClientScript.GetPostBackEventReference(this, PostBackDateControlChangedArgument) %>
		}

		function fmdate_textbox_onChange(textboxID) {
			if ( (textboxID == "thruputStartDateControl") ||
				(textboxID == "thruputEndDateControl")) {
				window.setTimeout('fmdate_onChangeDelayed("+ textboxID + ")', 1);
			}
		}

		// This is to scroll the top div when the whole page scrolls horizontally
		function syncDivs(me)
		{
			var scrollLeft = document.documentElement.scrollLeft;
			document.getElementById('topOuterDiv').style.left  = scrollLeft + "px";
			document.getElementById('topDiv').style.left  = -scrollLeft + "px";
		}

		function onLoadHandler()
		{
			window.onscroll = syncDivs;
			window.onresize=pageResize;
			window.setTimeout('restoreFocus()', 1); 
			window.setTimeout('pageResize()', 100); // Need to delay initial page resize 1/10th second to allow page to render first.  
		}
		
	</script>
</head>
<body onload="onLoadHandler()" onresize="pageResize();">
	<form id="mainForm" runat="server" onsubmit="onSubmitHandler()">
		<div id="menuDiv" onresize="pageResize();">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		</div>
		<div id="pageContent">
	<input runat="server" type="hidden" name="lastFocusControlID" id="lastFocusControlID"
		value="" />
	<input runat="server" type="hidden" name="lastFocusControlSelectionStart" id="lastFocusControlSelectionStart"
		value="" />
	<input runat="server" type="hidden" name="lastFocusControlSelectionEnd" id="lastFocusControlSelectionEnd"
		value="" />
	<input runat="server" type="hidden" name="restoreFocusControlID" id="restoreFocusControlID"
		value="" />
	<div id="floatingDiv">
	</div>
	<div id="topOuterDiv" onload="onLoadTemp();">
		<div id="topDiv" class="standardDivWidth">
		<%------------------------------------ background image ------------------------------------%>
		<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
		<%------------------------------------ title ------------------------------------%>
		<fmcontrols:FMLabel ID="titleLabel" Style="z-index: 103; position: relative; top: 0px;
			left: 0px;" runat="server" CssClass="headline" Width="500px" BackColor="Transparent">
		<%=GetTranslatedText(PageTitle)%></fmcontrols:FMLabel>
			<%------------------------------------ InfoDiv ------------------------------------%>
			<table class="standardTableWidth" role="presentation" aria-label="layout">
				<%-- total column = 9, row 5 have all 9 columns --%>
				<%------------------------------------ Row1 ID/Name ------------------------------------%>
				<tr>
					<td>
						<fmcontrols:FMLabel ID="ruleIDLabel" AssociatedControlID="ruleIDDropDown" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(RuleIdLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td  colspan="8">
						<fmcontrols:FMDropDownList ID="ruleIDDropDown" runat="server" CssClass="formfield dropDownList"
							TabIndex="111" AutoPostBack="True" OnSelectedIndexChanged="RuleIdDropDownSelectedIndexChanged" />
					</td>
				</tr>
				<%------------------------------------ Row2 Description ------------------------------------%>
				<tr>
					<td>
						<fmcontrols:FMLabel ID="descriptionLabel" AssociatedControlID="descriptionTextBox" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(DescriptionLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td  colspan="8">
						<fmcontrols:FMTextBox ID="descriptionTextBox" runat="server" CssClass="formfield descriptionTextBox"
							Enabled="False" TabIndex="121" />
					</td>
				</tr>
				<%------------------------------------ Row3 Manager, Product, Trx Alias ------------------------------------%>
				<tr>
					<td>
						<fmcontrols:FMLabel ID="managerLabel" AssociatedControlID="managerDropDown" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(ManagerLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td  colspan="2">
						<fmcontrols:FMDropDownList ID="managerDropDown" runat="server" AutoPostBack="True"
							CssClass="formfield dropDownList" TabIndex="131" OnSelectedIndexChanged="ManagerDropDownSelectedIndexChanged" />
					</td>
					<td>
					</td>
					<td>
						<fmcontrols:FMLabel ID="productLabel" AssociatedControlID="productDropDown" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(ProductLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td>
						<fmcontrols:FMDropDownList ID="productDropDown" runat="server" AutoPostBack="True"
							CssClass="formfield dropDownList" TabIndex="132" OnSelectedIndexChanged="ProductDropDownSelectedIndexChanged" />
					</td>
					<td>
					</td>
					<td>
						<fmcontrols:FMLabel ID="transactionAliasLabel" AssociatedControlID="transactionAliasTextBox" runat="server" CssClass="formfieldtitle nowrap"><%=GetTranslatedText(TrxAliasLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td>
						<fmcontrols:FMTextBox ID="transactionAliasTextBox" runat="server" CssClass="formfield standardTextBox"
							Enabled="False" TabIndex="133" />
					</td>
				</tr>
				<%------------------------------------Row4a Thruput StartDate, Thruput End Date, Inv. Date  ------------------------------------%>
				<tr id="manualRow" runat="server" style="overflow: auto; height: auto">
					<td>
						<fmcontrols:FMLabel ID="thruputStartDateLabel" runat="server" CssClass="formfieldtitle nowrap"><%=GetTranslatedText(ThruputStartDateLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td colspan="2">
						<fmcontrols:FMDate ID="thruputStartDateControl" ToolTip="Thruput Start Date" runat="server" Width="160px" CssClass="formfield"
							IsStandard="false" />
					</td>
					<td>
					</td>
					<td>
						<fmcontrols:FMLabel ID="thruputEndDateLabel" runat="server" CssClass="formfieldtitle nowrap"><%=GetTranslatedText(ThruputEndDateLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td>
						<fmcontrols:FMDate ID="thruputEndDateControl" ToolTip="Thruput End Date" runat="server" Width="160px" CssClass="formfield"
							IsStandard="false" />
					</td>
					<td>
					</td>
					<td>
						<fmcontrols:FMLabel ID="inventoryDateLabelManual" runat="server" CssClass="formfieldtitle nowrap"><%=GetTranslatedText(InvDateLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td>
						<fmcontrols:FMDate ID="inventoryDateControlManual" ToolTip="Inventory Date" runat="server" Width="160px" CssClass="formfield"
							IsStandard="false" />
					</td>
				</tr>
				<%------------------------------------ or Row4b Inv. Month, Inv. Date ------------------------------------%>
				<tr id="inventoryReconRow" runat="server">
					<td class="nowrap">
						<fmcontrols:FMLabel ID="inventoryMonthLabel" AssociatedControlID="InventoryMonthTextBox" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(InventoryMonthLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td colspan="2">
						<fmcontrols:FMTextBox ID="inventoryMonthTextBox" runat="server" CssClass="formfield standardTextBox"
							TabIndex="144" Enabled="false" />
					</td>
					<td>
					</td>
					<td>
						<fmcontrols:FMLabel ID="inventoryDateLabelInvRecon" runat="server" CssClass="formfieldtitle nowrap"><%=GetTranslatedText(InvDateLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td>
						<fmcontrols:FMDate ID="inventoryDateControlInvRecon" ToolTip="Inventory Date Reconciliation" runat="server" Width="160px"
							CssClass="formfield" IsStandard="false" />
					</td>
				</tr>
				<%------------------------------------ Row5 Quantities/Variances ------------------------------------%>
				<tr>
					<td id="infoCol1">
						<fmcontrols:FMLabel ID="quantityLabel" runat="server" CssClass="formfieldtitle nowrap"><%=GetTranslatedText(this.operationType == AutoDistributionOperationTypes.Manual?QuantitiesManualLabelText: QuantitiesInvReconLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td id="infoCol2">
						<fmcontrols:FMLabel ID="expectedGrossLabel" AssociatedControlID="expectedGrossTextBox" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(ExpectedGrossLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td id="infoCol3">
						<fmcontrols:FMTextBox ID="expectedGrossTextBox" runat="server" CssClass="formfield quantityTextBox"
							TabIndex="151" AutoPostBack="true" OnTextChanged="ExpectedQuantityChanged" />
					</td>
					<td id="infoCol4">
					</td>
					<td id="infoCol5">
						<fmcontrols:FMLabel ID="expectedNetLabel" AssociatedControlID="expectedNetTextBox" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(ExpectedNetLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td id="infoCol6">
						<fmcontrols:FMTextBox ID="expectedNetTextBox" runat="server" CssClass="formfield quantityTextBox"
							TabIndex="152" AutoPostBack="true" OnTextChanged="ExpectedQuantityChanged" />
					</td>
					<td id="infoCol7">
					</td>
					<td id="infoCol8">
						<fmcontrols:FMLabel ID="expectedMassLabel" AssociatedControlID="expectedMassTextBox" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(ExpectedMassLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td id="infoCol9">
						<fmcontrols:FMTextBox ID="expectedMassTextBox" runat="server" CssClass="formfield quantityTextBox"
							TabIndex="153" AutoPostBack="true" OnTextChanged="ExpectedQuantityChanged" />
					</td>
				</tr>
				<%------------------------------------ Row6 Reason ------------------------------------%>
				<tr>
					<td>
						<fmcontrols:FMLabel ID="reasonCodeLabel" AssociatedControlID="reasonCodeDropDown" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(ReasonCodeLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td colspan="8">
						<fmcontrols:FMDropDownList ID="reasonCodeDropDown" runat="server" CssClass="formfield dropDownList"
							TabIndex="161" AutoPostBack="True" />
					</td>
				</tr>
				<%------------------------------------ Row7 Notes ------------------------------------%>
				<tr>
					<td style="vertical-align: top">
						<fmcontrols:FMLabel ID="notesLabel" AssociatedControlID="notesTextBox" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(NotesLabelText)%>:</fmcontrols:FMLabel>
					</td>
					<td colspan="8">
						<fmcontrols:FMTextBox ID="notesTextBox" runat="server" CssClass="formfield" TabIndex="171"
							TextMode="MultiLine" Height="60px" MaxLength="1000" Width="100%" wrap="true"/>
					</td>
				</tr>
				<%------------------------------------ Row8 Calculate button ------------------------------------%>
				<tr>
					<td colspan="9" style="text-align: right">
						<fmcontrols:FMButton ID="calculateButton" runat="server" Width="70px" CssClass="formfieldtitle"
							Text="Calculate" OnClick="CalculateButtonClick" />
					</td>
				</tr>
			</table>
			<fmcontrols:FMLabel ID="distributionLabel" runat="server" CssClass="formfieldtitle"><%=GetTranslatedText(DistributionsLabelText)%>:</fmcontrols:FMLabel>
			<%------------------------------------ header table------------------------------------%>
			<table class="tabletext gridHeader" id="gridHeaderTable2" cellspacing="0" cellpadding="3"
				rules="cols" border="0px" style="border-width: 1px; border-style: transparent;
				border-collapse: collapse;" role="presentation" aria-label="Header layout">
				<%------- We need to define the columns at the first row in order for the page to scale the table correctly. -------%>
				<tr class="tabletext tableHeader" style="background: transparent; color: transparent; height: 0px">
					<td class="ownerColumn">
					</td>
					<td class="grossThruputColumn captionRowCell">
					</td>
					<td class="grossThruputPercentColumn captionRowCell">
					</td>
					<td class="grossQuantityColumn captionRowCell">
					</td>
					<td class="grossQuantityPercentColumn captionRowCell">
					</td>
					<td class="netThruputColumn captionRowCell">
					</td>
					<td class="netThruputPercentColumn captionRowCell">
					</td>
					<td class="netQuantityColumn captionRowCell">
					</td>
					<td class="netQuantityPercentColumn captionRowCell">
					</td>
					<td class="massThruputColumn captionRowCell">
					</td>
					<td class="massThruputPercentColumn captionRowCell">
					</td>
					<td class="massQuantityColumn captionRowCell">
					</td>
					<td class="massQuantityPercentColumn captionRowCell">
					</td>
				</tr>
				<tr class="tabletext tableHeader">
					<th class="ownerColumn" rowspan="2">
					</th>
					<th class="headerRow1Cell" colspan="4">
						<fmcontrols:FMLabel ID="grossColumnLabel" runat="server" CssClass="tableHeader"><%=GetTranslatedText(GrossColumnLabelText)%></fmcontrols:FMLabel>
					</th>
					<th class="headerRow1Cell" colspan="4">
						<fmcontrols:FMLabel ID="netColumnLabel" runat="server" CssClass="tableHeader"><%=GetTranslatedText(NetColumnLabelText)%></fmcontrols:FMLabel>
					</th>
					<th class="headerRow1Cell" colspan="4">
						<fmcontrols:FMLabel ID="massColumnLabel" runat="server" CssClass="tableHeader"><%=GetTranslatedText(MassColumnLabelText)%></fmcontrols:FMLabel>
					</th>
				</tr>
			</table>
			<%------------------------------------ Body Table ------------------------------------%>
			<fmcontrols:FMBulkEditGridView ID="ApplicationGrid" runat="server" DataKeyNames="OwnerGuid"
				AutoGenerateColumns="False" AllowSorting="True" CssClass="tabletext gridShared" BorderWidth="0px"
				ShowHeader="true" ShowFooter="false" FixedHeaders="false" EmptyDataText="When you are ready, please click on Calculate button to generate the distributions."
				GroupColumnOffset="0" GroupingDepth="0" ShowFooterWhenEmpty="False" AllowPaging="false"
				ShowHeaderWhenEmpty="true" UseAccessibleHeader="False" OnRowDataBound="GridRowDataBound"
				OnRowCreated="GridRowCreated" OnSorting="GridSorting" aria-label="Application">
				<EditRowStyle BackColor="#DCDCDC" CssClass="tabletext" />
				<RowStyle BackColor="#EEEEEE" ForeColor="Black"></RowStyle>
				<SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
				<AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
				<Columns>
					<asp:TemplateField Visible="false">
						<ItemTemplate>
							<asp:Literal ID="EntityGuidText" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.OwnerGuidColumnName, null) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Owner" HeaderStyle-CssClass="ownerColumn" SortExpression="OwnerID">
						<ItemStyle CssClass="ownerColumn" />
						<ItemTemplate>
							<asp:Literal ID="ownerID" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.OwnerIDColumnName, null) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Throughput" HeaderStyle-CssClass="grossThruputColumn"
						SortExpression="GrossThruput">
						<ItemStyle CssClass="grossThruputColumn" />
						<ItemTemplate>
							<asp:Literal ID="grossThruput" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.GrossThruputColumnName, myOperationHelper.VolumeProductNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Percent" HeaderStyle-CssClass="grossThruputPercentColumn"
						SortExpression="GrossThruputPercent">
						<ItemStyle CssClass="grossThruputPercentColumn" />
						<ItemTemplate>
							<asp:Literal ID="grossThruputPercent" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.GrossThruputPercentColumnName, myOperationHelper.PercentNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Quantity" HeaderStyle-CssClass="grossQuantityColumn"
						SortExpression="GrossQuantity">
						<ItemStyle CssClass="grossQuantityColumn" />
						<EditItemTemplate>
							<fmcontrols:FMTextBox ID="grossQuantityTextBox" alt="Gross Quantity" runat="server" CssClass="tabletext grossQuantityCell"
								Text='<%# BindColumn(Container, AutoDistributionOperationHelper.GrossQuantityColumnName, myOperationHelper.VolumeTrxNumberFormat) %>'
								AutoPostBack="true" OnTextChanged="GrossQuantityColumnTextChanged" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Percent" HeaderStyle-CssClass="grossQuantityPercentColumn"
						SortExpression="GrossQuantityPercent">
						<ItemStyle CssClass="grossQuantityPercentColumn" />
						<ItemTemplate>
							<asp:Literal ID="grossQuantityPercent" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.GrossQuantityPercentColumnName, myOperationHelper.PercentNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Throughput" HeaderStyle-CssClass="netThruputColumn"
						SortExpression="NetThruput">
						<ItemStyle CssClass="netThruputColumn" />
						<ItemTemplate>
							<asp:Literal ID="netThruput" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.NetThruputColumnName, myOperationHelper.VolumeProductNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Percent" HeaderStyle-CssClass="netThruputPercentColumn"
						SortExpression="NetThruputPercent">
						<ItemStyle CssClass="netThruputPercentColumn" />
						<ItemTemplate>
							<asp:Literal ID="netThruputPercent" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.NetThruputPercentColumnName, myOperationHelper.PercentNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Quantity" HeaderStyle-CssClass="netQuantityColumn"
						SortExpression="NetQuantity">
						<ItemStyle CssClass="netQuantityColumn" />
						<EditItemTemplate>
							<fmcontrols:FMTextBox ID="netQuantityTextBox" alt="Net Quantity" runat="server" CssClass="tabletext netQuantityCell"
								Text='<%# BindColumn(Container, AutoDistributionOperationHelper.NetQuantityColumnName, myOperationHelper.VolumeTrxNumberFormat) %>'
								AutoPostBack="true" OnTextChanged="NetQuantityColumnTextChanged" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Percent" HeaderStyle-CssClass="netQuantityPercentColumn"
						SortExpression="NetQuantityPercent">
						<ItemStyle CssClass="netQuantityPercentColumn" />
						<ItemTemplate>
							<asp:Literal ID="netQuantityPercent" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.NetQuantityPercentColumnName, myOperationHelper.PercentNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Throughput" HeaderStyle-CssClass="massThruputColumn"
						SortExpression="MassThruput">
						<ItemStyle CssClass="massThruputColumn" />
						<ItemTemplate>
							<asp:Literal ID="massThruput" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.MassThruputColumnName, myOperationHelper.MassProductNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Percent" HeaderStyle-CssClass="massThruputPercentColumn"
						SortExpression="MassThruputPercent">
						<ItemStyle CssClass="massThruputPercentColumn" />
						<ItemTemplate>
							<asp:Literal ID="massThruputPercent" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.MassThruputPercentColumnName, myOperationHelper.PercentNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Quantity" HeaderStyle-CssClass="massQuantityColumn"
						SortExpression="MassQuantity">
						<ItemStyle CssClass="massQuantityColumn" />
						<EditItemTemplate>
							<fmcontrols:FMTextBox ID="massQuantityTextBox" alt="Mass Quantity" runat="server" CssClass="tabletext massQuantityCell"
								Text='<%# BindColumn(Container, AutoDistributionOperationHelper.MassQuantityColumnName, myOperationHelper.MassTrxNumberFormat) %>'
								AutoPostBack="true" OnTextChanged="MassQuantityColumnTextChanged" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Percent" HeaderStyle-CssClass="massQuantityPercentColumn"
						SortExpression="MassQuantityPercent">
						<ItemStyle CssClass="massQuantityPercentColumn" />
						<ItemTemplate>
							<asp:Literal ID="massQuantityPercent" runat="server" Text='<%# BindColumn(Container, AutoDistributionOperationHelper.MassQuantityPercentColumnName, myOperationHelper.PercentNumberFormat) %>' />
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
			</fmcontrols:FMBulkEditGridView>
		</div>
	</div>
	<%------------------------------------ bottomDiv ------------------------------------%>
	<div id="bottomOuterDiv" class="standardDivWidth">
		<div id="bottomDiv" runat="server" visible="false">
			<table class="tabletext gridFooter" id="gridFooterTable" cellspacing="0" cellpadding="3"
				rules="cols" border="1" style="border-width: 1px; border-style: transparent;
				border-collapse: collapse;"  role="presentation" aria-label="Footer layout">
				<tr class="tabletext tableHeader">
					<td class="ownerColumn">
						<fmcontrols:FMLabel ID="totalLabel" runat="server" />
					</td>
					<td class="grossThruputColumn footerBorder">
						<fmcontrols:FMLabel ID="totalGrossThruputLabel" runat="server" />
					</td>
					<td class="grossThruputPercentColumn footerBorder">
						<fmcontrols:FMLabel ID="totalGrossThruputPercentLabel" runat="server" />
					</td>
					<td class="grossQuantityColumn footerBorder">
						<fmcontrols:FMLabel ID="totalGrossLabel" runat="server" />
					</td>
					<td class="grossQuantityPercentColumn footerBorder">
						<fmcontrols:FMLabel ID="totalGrossPercentLabel" runat="server" />
					</td>
					<td class="netThruputColumn footerBorder">
						<fmcontrols:FMLabel ID="totalNetThruputLabel" runat="server" />
					</td>
					<td class="netThruputPercentColumn footerBorder">
						<fmcontrols:FMLabel ID="totalNetThruputPercentLabel" runat="server" />
					</td>
					<td class="netQuantityColumn footerBorder">
						<fmcontrols:FMLabel ID="totalNetLabel" runat="server" />
					</td>
					<td class="netQuantityPercentColumn footerBorder">
						<fmcontrols:FMLabel ID="totalNetPercentLabel" runat="server" />
					</td>
					<td class="massThruputColumn footerBorder">
						<fmcontrols:FMLabel ID="totalMassThruputLabel" runat="server" />
					</td>
					<td class="massThruputPercentColumn footerBorder">
						<fmcontrols:FMLabel ID="totalMassThruputPercentLabel" runat="server" />
					</td>
					<td class="massQuantityColumn footerBorder">
						<fmcontrols:FMLabel ID="totalMassLabel" runat="server" />
					</td>
					<td class="massQuantityPercentColumn footerBorder">
						<fmcontrols:FMLabel ID="totalMassPercentLabel" runat="server" />
					</td>
				</tr>
			</table>
			<table class="standardTableWidth"  role="presentation" aria-label="Buttons layout">
				<tr>
					<td style="text-align: right">
						<fmcontrols:FMButton ID="CloseBtn" runat="server" Width="70px" CssClass="formfieldtitle"
							Text="Close" OnClick="CloseButtonClick" />
						&nbsp;&nbsp;&nbsp;
						<fmcontrols:FMButton ID="CreateBtn" runat="server" Width="70px" CssClass="formfieldtitle"
							Text="Create" OnClick="CreateButtonClick" OnClientClick="return confirmCreate();" />
					</td>
				</tr>
			</table>
		</div>
	</div>
	</div>
</form>
</body>
</html>
