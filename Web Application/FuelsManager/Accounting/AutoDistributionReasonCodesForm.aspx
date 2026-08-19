<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoDistributionReasonCodesForm.aspx.cs"
	Inherits="FuelsManager.Accounting.AutoDistributionReasonCodesForm" %>
<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<link type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<style type="text/css" runat="server">
		.buttonCell
		{
			height: 36px;
			vertical-align: middle;
		}
		.addButton
		{
			width: 100px;
		}
		.datagridCell
		{
			height: 10px;
		}
		/*--------------- grid styles ---------------*/
		.grid
		{
			background-color: White;
			width: 600px;
			left: 1px;
			top: 0px;
			border: 1px none;
		}		
		.grid td
		{
			padding: 3px;
			border: 1px solid white;
			border-bottom: none;
			border-top: none;
		}
		.gridHeader
		{
			color: White;
			font-weight: bold;
		}
		.gridFooter
		{
			color: Black;
		}
		.gridPager
		{
			color: White;
		}
		.gridItem
		{
			color: Black;
			background-color: #EEEEEE;
		}
		.gridAlternatingItem
		{
			background-color: Gainsboro;
		}
		.gridSelectedItem
		{
			color: White;
			background-color: #008A8C;
			font-weight: bold;
		}
		/*--------------- column styles ---------------*/
		.editHeader, .deleteHeader
		{
			width: 80px;
		}
		.editItem, .deleteItem
		{
			text-align: center;
			vertical-align: middle;
		}
		.reasonCodeHeader, .reasonCodeItem, .reasonCodeEditItem
		{
			width: 150px;
		}
		.descriptionHeader, .descriptionItem, .descriptionEditItem
		{
			width: 280px;
		}
	</style>
</head>
<body>
	<form id="mainForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
	<%------------------------------------ background image and title ------------------------------------%>
	<fmcontrols:FMLabel ID="titleLabel" Style="z-index: 103; left: 8px; position: absolute;
		top: 8px" runat="server" CssClass="headline" Width="500px" BackColor="Transparent">
		<%=GetTranslatedText(PageTitle)%></fmcontrols:FMLabel>
	<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
	<table id="mainTable" style="z-index: 100; position: absolute; top: 48px; left: 32px;
		height: 10px; width: 43.18%; border-spacing: 0; padding:1; border:0;" role="presentation" aria-label="layout">
		<%------------------------------------ top button row ------------------------------------%>
		<tr>
			<td class="buttonCell">
				<fmcontrols:FMButton ID="topAddButton" runat="server" Text="Add" CssClass="addButton formfieldtitle"
					TabIndex="1" OnClick="AddButtonClick" />
				&nbsp;&nbsp;
				<fmcontrols:FMPageSizeDropDown ID="pageSizeDropDown" ToolTip="Page size"  runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
			</td>
		</tr>
		<tr>
			<%------------------------------------ Main Grid ------------------------------------%>
			<td class="datagridCell">
				<fmcontrols:FMDataGrid ID="mainDataGrid" runat="server" CssClass="tabletext grid"
					AutoGenerateColumns="False" GridLines="Vertical" AllowSorting="True" AllowPaging="True"
					PageSize="16" OnEditCommand="DataGridEditCommand" OnUpdateCommand="DataGridUpdateCommand"
					OnCancelCommand="DataGridCancelCommand" OnDeleteCommand="DataGridDeleteCommand"
					OnItemDataBound="DataGridItemDataBound" OnPageIndexChanged="DataGridPageIndexChanged" aria-label="Main Data">
					<HeaderStyle CssClass="tablecolhead GVFixedHeader" BackColor="<%$ AppSettings: ColorHeaderBlue %>"/>
					<FooterStyle CssClass="gridFooter" />
					<ItemStyle CssClass="gridItem" />
					<AlternatingItemStyle CssClass="gridAlternatingItem" />
					<SelectedItemStyle CssClass="gridSelectedItem" />
					<PagerStyle CssClass="tablepager pgr gridPager" Mode="NumericPages" />
					<Columns>
						<%------------------------------------ Edit/Delete columns ------------------------------------%>
						<asp:TemplateColumn HeaderText="Edit">
							<HeaderStyle CssClass="editHeader"></HeaderStyle>
							<ItemStyle CssClass="editItem"></ItemStyle>
							<ItemTemplate>
								<fmcontrols:FMEditLinkButton runat="server" />
							</ItemTemplate>
							<EditItemTemplate>
								<fmcontrols:FMUpdateLinkButton runat="server" />&nbsp;
								<fmcontrols:FMCancelLinkButton runat="server" />
							</EditItemTemplate>
						</asp:TemplateColumn>
						<%------------------------------------ Hidden columns ------------------------------------%>
						<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
						<asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
							<ItemTemplate>
								<asp:Label runat="server" ID="identityGuidLabel" Text='<%# BindColumn(Container, ReasonCodeGuidColumnName) %>' />
							</ItemTemplate>
						</asp:TemplateColumn>
						<%------------------------------------ Visible Data columns ------------------------------------%>
						<asp:TemplateColumn>
							<HeaderTemplate>
								<fmcontrols:FMLabel CssClass="reasonCodeHeader" runat="server"><%=GetTranslatedText(ReasonCodeColumnHeader)%></fmcontrols:FMLabel>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label ID="reasonCodeLabel" runat="server" CssClass="reasonCodeItem" Text='<%# BindColumn(Container, ReasonCodeColumnName) %>' />
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox ID="reasonCodeTextBox" ToolTip="Reason code" runat="server" CssClass="tabletext reasonCodeEditItem"
									MaxLength="50" Text='<%# BindColumn(Container, ReasonCodeColumnName) %>' />
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<HeaderTemplate>
								<fmcontrols:FMLabel CssClass="descriptionHeader" runat="server"><%=GetTranslatedText(DescriptionColumnHeader)%></fmcontrols:FMLabel>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label ID="descriptionLabel" runat="server" CssClass="descriptionItem" Text='<%# BindColumn(Container, DescriptionColumnHeader) %>' />
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox ID="descriptionTextBox" alt="Description" runat="server" CssClass="tabletext descriptionEditItem"
									MaxLength="255" Text='<%# BindColumn(Container, DescriptionColumnHeader) %>' />
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Delete">
							<HeaderStyle CssClass="deleteHeader"></HeaderStyle>
							<ItemStyle CssClass="deleteItem"></ItemStyle>
							<ItemTemplate>
								<fmcontrols:FMDeleteLinkButton runat="server" />
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</fmcontrols:FMDataGrid>
			</td>
		</tr>
		<%------------ bottom button row ------------%>
		<tr>
			<td class="buttonCell">
				<fmcontrols:FMButton ID="bottomAddButton" runat="server" Text="Add" CssClass="formfieldtitle addButton"
					OnClick="AddButtonClick" />
			</td>
		</tr>
	</table>
	</div>
</form>
</body>
</html>
