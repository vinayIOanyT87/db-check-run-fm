<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoDistributionRuleForm.aspx.cs"
	Inherits="FuelsManager.Accounting.AutoDistributionRuleForm" %>

<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<link type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<style type="text/css"">
		/*--------------- Common/Shared ---------------*/
		.leftMargin
		{
			left: 16px;
		}
		.nowrap
		{
			white-space:nowrap;
		}
		/*--------------- For top half data entry fields ---------------*/
		.rowHeight
		{
			height: 24px;
		}
		.pageWidth
		{
			width: 700px;
		}
		.cell1Width
		{
			width: 190px;
		}
		.cell2Width
		{
			width: 300px;
		}
		.cell3Width
		{
			width: 90px;
		}
		/*--------------- For Steps/lists ---------------*/
		.stepText
		{
			font-style: italic;
		}
		.separatorLine
		{
			width: 100%;
			color: Black;
			height: 2pt;
		}
		.leftMarginCell, .rightMarginCell
		{
			width: 60px;
		}
		.rightMarginCell
		{
			width: 155px;
		}
		.middleListCell
		{
			width: 40px;
		}
		.leftListCell, .rightListCell
		{
			width: 220px;
		}
		.leftListBox, .rightListBox
		{
			width: 200px;
			height: 120px;
		}
		.assignButton
		{
			
		}
	</style>
</head>
<body>
	<form id="mainForm" runat="server" method="post">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">

	<%------------------------------------ background image ------------------------------------%>
	<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>

	<%------------------------------------ top margin and title ------------------------------------%>
	<div id="topMargin" class="leftMargin" style="position: relative; top: 0px">
	</div>
	<fmcontrols:FMLabel ID="titleLabel" Style="z-index: 153; position: relative" runat="server"
		CssClass="headline" Width="500px" BackColor="Transparent" />
	<div id="spacer" class="leftMargin" style="position: relative; height: 8px">
	</div>

	<%------------------------------------ Top half data entry fields ------------------------------------%>
	<div class="leftMargin" style="position: relative; top: 0px;">
		<table class="pageWidth" style="z-index: 103;" role="presentation" aria-label="layout">
			<%------------------------------------ Line 1 ------------------------------------%>
			<tr>
				<td class="rowHeight cell1Width">
					<fmcontrols:FMLabel ID="idLabel" AssociatedControlID="IDTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent"><%=GetTranslatedText(IDLabelText)%>:</fmcontrols:FMLabel>
					<span style="COLOR: red; width: 3px">*</span>
				</td>
				<td class="cell2Width">
					<asp:TextBox ID="IDTextBox" TabIndex="1" Width="136px" CssClass="formfield" runat="server" aria-required="true"
						MaxLength="50" />
				</td>
				<td colspan="2">
					<fmcontrols:FMCheckBox ID="enabledCheckBox" TabIndex="17" TextAlign="Left"
						CssClass="formfieldtitle" runat="server" ></fmcontrols:FMCheckBox>
				</td>
			</tr>
			<%------------------------------------ Line 2 ------------------------------------%>
			<tr>
				<td class="rowHeight">
					<fmcontrols:FMLabel ID="descriptionLabel" AssociatedControlID="descriptionTextBox" CssClass="formfieldtitle" runat="server"
						BackColor="Transparent"><%=GetTranslatedText(DescriptionLabelText)%>:</fmcontrols:FMLabel>
				</td>
				<td>
					<asp:TextBox ID="descriptionTextBox" TabIndex="2" Width="251px" CssClass="formfield"
						runat="server" MaxLength="255" />
				</td>
				<td colspan="2">
					<fmcontrols:FMCheckBox ID="defaultEOMCheckBox" TabIndex="17" TextAlign="Left"
						CssClass="formfieldtitle" runat="server" />
				</td>
			</tr>
			<%------------------------------------ Line 3 ------------------------------------%>
			<tr>
				<td class="rowHeight nowrap">
					<fmcontrols:FMLabel ID="distributionAliasLabel" AssociatedControlID="distributionAliasDropDownList" CssClass="formfieldtitle" runat="server"
						BackColor="Transparent"><%=GetTranslatedText(DistributionAliasLabelText)%>:</fmcontrols:FMLabel>
                    <span style="COLOR: red; width: 3px">*</span>
				</td>
				<td>
					<fmcontrols:FMDropDownList ID="distributionAliasDropDownList" Style="z-index: 107;" aria-required="true"
						TabIndex="4" CssClass="formfield" runat="server" AutoPostBack="True" />
				</td>
				<td class="cell3Width nowrap">
					<fmcontrols:FMLabel ID="defaultReasonLabel" AssociatedControlID="reasonCodeDropDownList" CssClass="formfieldtitle nowrap" runat="server"
						BackColor="Transparent"><%=GetTranslatedText(DefaultReasonLabelText)%>:</fmcontrols:FMLabel>
                    <span style="COLOR: red; width: 3px">*</span>
				</td>
				<td>
					<fmcontrols:FMDropDownList ID="reasonCodeDropDownList" Style="z-index: 107;" TabIndex="4" aria-required="true"
						CssClass="formfield" runat="server" AutoPostBack="True" />
				</td>
			</tr>
			<%------------------------------------ Line 4 ------------------------------------%>
			<tr>
				<td style="vertical-align: top">
					<fmcontrols:FMLabel ID="defaultNoteLabel" AssociatedControlID="defaultNotesTextBox" CssClass="formfieldtitle" runat="server"
						BackColor="Transparent"><%=GetTranslatedText(DefaultNoteLabelText)%>:</fmcontrols:FMLabel>
				</td>
				<td colspan="3">
					<fmcontrols:FMTextBox ID="defaultNotesTextBox" TabIndex="12" Width="99%" CssClass="formfield" TextMode="MultiLine"
						runat="server" MaxLength="1000" Height="70px"></fmcontrols:FMTextBox>
				</td>

			</tr>
		</table>
	</div>
	<%------------------------------------ Steps/Lists ------------------------------------%>
	<div class="leftMargin" style="position: relative; top: 0px;">
		<table class="pageWidth" role="presentation" aria-label="layout">
			<tr>
				<td class="leftMarginCell" />
				<td class="leftListCell" />
				<td class="middleListCell" />
				<td class="rightListCell" />
				<td class="rightMarginCell" />
			</tr>
			<tr>
				<td colspan="5">
					<hr />
				</td>
			</tr>
			<%------------------------------------ Managers ------------------------------------%>
			<tr>
				<td class="formfieldtitle stepText">
					Step 1:
				</td>
				<td class="formfieldtitle leftListCell" colspan="2">
					<fmcontrols:FMLabel ID="selectedManagersLabel" AssociatedControlID="selectedManagersList" runat="server" CssClass="formfieldtitle"
						><%=GetTranslatedText(SelectedManagersLabelText)%>:</fmcontrols:FMLabel>
				</td>
				<td class="formfieldtitle rightListCell">
					<fmcontrols:FMLabel ID="availableManagersLabel" AssociatedControlID="availableManagersList" runat="server" CssClass="formfieldtitle"
						><%=GetTranslatedText(AvailableManagersLabelText)%>:</fmcontrols:FMLabel>
				</td>
			</tr>
			<tr>
				<td />
				<td rowspan="2">
					<fmcontrols:FMListBox ID="selectedManagersList" runat="server" CssClass="formfield leftListBox"
						Sort="false" SelectionMode="Multiple" />
				</td>
				<td>
					<fmcontrols:FMButton ID="assignManagerButton" runat="server" CssClass="formfieldtitle assignButton"
						Text="<<" />
				</td>
				<td rowspan="2">
					<fmcontrols:FMListBox ID="availableManagersList" runat="server" CssClass="formfield rightListBox"
						Sort="false" SelectionMode="Multiple" />
				</td>
			</tr>
			<tr>
				<td />
				<td>
					<fmcontrols:FMButton ID="unassignManagerButton" runat="server" CssClass="formfieldtitle assignButton"
						Text=">>" />
				</td>
				<td />
			</tr>
			<tr>
				<td />
				<td class="formfieldtitle stepText" colspan="4">
					Company Group is prefixed with *.
				</td>
			</tr>
			<tr>
				<td colspan="5">
					<hr />
				</td>
			</tr>
			<%------------------------------------ Products ------------------------------------%>
			<tr>
				<td class="formfieldtitle stepText">
					Step 2:
				</td>
				<td class="formfieldtitle leftListCell" colspan="2">
					<fmcontrols:FMLabel ID="selectedProductsLabel" AssociatedControlID="selectedProductsList" runat="server" CssClass="formfieldtitle"
						><%=GetTranslatedText(SelectedProductsLabelText)%>:</fmcontrols:FMLabel>
				</td>
				<td class="formfieldtitle rightListCell">
					<fmcontrols:FMLabel ID="availableProductsLabel" AssociatedControlID="availableProductsList" runat="server" CssClass="formfieldtitle"
						><%=GetTranslatedText(AvailableProductsLabelText)%>:</fmcontrols:FMLabel>
				</td>
			</tr>
			<tr>
				<td />
				<td rowspan="2">
					<fmcontrols:FMListBox ID="selectedProductsList" runat="server" CssClass="formfield leftListBox"
						Sort="false" SelectionMode="Multiple" />
				</td>
				<td >
					<fmcontrols:FMButton ID="assignProductButton" runat="server" CssClass="formfieldtitle assignButton"
						Text="<<" />
				</td>
				<td rowspan="2">
					<fmcontrols:FMListBox ID="availableProductsList" runat="server" CssClass="formfield rightListBox"
						Sort="false" SelectionMode="Multiple" />
				</td>
			</tr>
			<tr>
				<td />
				<td >
					<fmcontrols:FMButton ID="unassignProductButton" runat="server" CssClass="formfieldtitle assignButton"
						Text=">>" />
				</td>
			</tr>
			<tr>
				<td />
				<td class="formfieldtitle stepText" colspan="4">
					Product Group is prefixed with *.
				</td>
			</tr>
			<tr>
				<td colspan="5">
					<hr />
				</td>
			</tr>
			<%------------------------------------ Throughput Transactions ------------------------------------%>
			<tr>
				<td class="formfieldtitle stepText">
					Step 3:
				</td>
				<td class="formfieldtitle leftListCell" colspan="2">
					<fmcontrols:FMLabel ID="selectedTransactionsLabel" AssociatedControlID="selectedTransactionsList" runat="server" CssClass="formfieldtitle"
						><%=GetTranslatedText(SelectedTransactionsLabelText)%>:</fmcontrols:FMLabel>
				</td>
				<td class="formfieldtitle rightListCell" colspan="2">
					<fmcontrols:FMLabel ID="availableTransactionsLabel" AssociatedControlID="availableTransactionsList" runat="server" CssClass="formfieldtitle nowrap"
						><%=GetTranslatedText(AvailableTransactionsLabelText)%>:</fmcontrols:FMLabel>
				</td>
			</tr>
			<tr>
				<td />
				<td rowspan="2">
					<fmcontrols:FMListBox ID="selectedTransactionsList" runat="server" CssClass="formfield leftListBox"
						Sort="false" SelectionMode="Multiple" />
				</td>
				<td >
					<fmcontrols:FMButton ID="assignTransactionButton" runat="server" CssClass="formfieldtitle assignButton"
						Text="<<" />
				</td>
				<td rowspan="2">
					<fmcontrols:FMListBox ID="availableTransactionsList" runat="server" CssClass="formfield rightListBox"
						Sort="false" SelectionMode="Multiple" />
				</td>
			</tr>
			<tr>
				<td />
				<td >
					<fmcontrols:FMButton ID="unassignTransactionButton" runat="server" CssClass="formfieldtitle assignButton"
						Text=">>" />
				</td>
				<td />
			</tr>
			<tr>
				<td />
				<td class="formfieldtitle stepText" colspan="4">
					Absolute values will be used for calculation.
				</td>
			</tr>
			<tr>
				<td colspan="5">
					<hr />
				</td>
			</tr>
			<%------------------------------------ Owners ------------------------------------%>
			<tr>
				<td class="formfieldtitle stepText">
					Step 4:
				</td>
				<td class="formfieldtitle leftListCell" colspan="2">
					<fmcontrols:FMLabel ID="selectedOwnersLabel" AssociatedControlID="selectedOwnersList" runat="server" CssClass="formfieldtitle"
						><%=GetTranslatedText(SelectedOwnersLabelText)%>:</fmcontrols:FMLabel>
				</td>
				<td class="formfieldtitle rightListCell">
					<fmcontrols:FMLabel ID="availableOwnersLabel" AssociatedControlID="availableOwnersList" runat="server" CssClass="formfieldtitle"
						><%=GetTranslatedText(AvailableOwnersLabelText)%>:</fmcontrols:FMLabel>
				</td>
			</tr>
			<tr>
				<td />
				<td rowspan="2">
					<fmcontrols:FMListBox ID="selectedOwnersList" runat="server" CssClass="formfield leftListBox"
						Sort="false" SelectionMode="Multiple" />
				</td>
				<td>
					<fmcontrols:FMButton ID="assignOwnerButton" runat="server" CssClass="formfieldtitle assignButton"
						Text="<<" />
				</td>
				<td rowspan="2">
					<fmcontrols:FMListBox ID="availableOwnersList" runat="server" CssClass="formfield rightListBox"
						Sort="false" SelectionMode="Multiple" />
				</td>
			</tr>
			<tr>
				<td />
				<td>
					<fmcontrols:FMButton ID="unassignOwnerButton" runat="server" CssClass="formfieldtitle assignButton"
						Text=">>" />
				</td>
				<td />
			</tr>
			<tr>
				<td />
				<td class="formfieldtitle stepText" colspan="4">
					Company Group is prefixed with *.
				</td>
			</tr>
		</table>
	</div>
	<%------------------------------------ top margin and title ------------------------------------%>
	<div class="leftMargin" style="position: relative; top: 0px; height: 10px">
	</div>
	<div class="leftMargin pageWidth" style="position: relative; top: 0px; text-align: right">
	    <FMControls:FMLabel id="RequiredFieldsLabel"  runat="server"
		Width="176px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
		<fmcontrols:FMButton ID="NewButton" runat="server" Text="New" CssClass="formfieldtitle"
			OnClick="New_Command" Width="75px" />
		&nbsp;&nbsp;
		<fmcontrols:FMButton ID="OKButton" runat="server" Text="OK" CssClass="formfieldtitle"
			OnClick="OK_Command" Width="75px" />
		&nbsp;&nbsp;
		<fmcontrols:FMButton ID="CancelButton" runat="server" Text="Cancel" CssClass="formfieldtitle"
			OnClick="Cancel_Command" Width="75px" />
	</div>
    <div id="bottomSpacer" class="leftMargin" style="position: relative; top: 0px; height: 15px">
	</div>
	</div>
</form>
</body>
</html>
