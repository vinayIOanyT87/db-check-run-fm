<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoDistributionRulesForm.aspx.cs"
	Inherits="FuelsManager.Accounting.AutoDistributionRulesForm" %>

<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<link type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<style type="text/css">
		.addButton
		{
			width: 100px;
		}
		.leftMargin
		{
			left: 16px;
		}
		/*--------------- grid styles ---------------*/
		.grid
		{
			background-color: White;
			width: 1000px;
			left: 1px;
			top: 0px;
			border: 1px none;
		}
		.grid td
		{
			padding: 3px;
			border: 1px solid #999999;
			border-bottom: none;
			border-top: none;
		}
		.gridHeader
		{
			color: White;
			font-weight: bold;
			text-align:left;
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
			width: 50px;
		}
	</style>
</head>
<body>
	<form id="mainForm" runat="server" method="post" DefaultButton="findBtn">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
	<%------------------------------------ background image ------------------------------------%>
	<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
	<%------------------------------------ Scripts ------------------------------------%>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Scripts/select2.full.min.js" %>"></script>
	<script type='text/javascript'>
		function CompanySelect(Role, CompanyTextBoxID) {
			var CompanyTextBox = document.getElementById(CompanyTextBoxID);
			showModalDialogFrame({
				url: '../FMWebApp/CompanySelectForm.aspx?Role=' + Role + '',
				width: 855,
				height: 690,
				title: "Company Select",
				onClose: function () {
					if (this.returnValue != null) {
						CompanyTextBox.value = this.returnValue[0];
						CompanyTextBox.title = this.returnValue[1];
						<%= Page.ClientScript.GetPostBackEventReference(this, PostBackRefreshDataArgument) %>
					}
				}
			});
		}

		function ProductSelect(productTextBoxID) {
			var productTextBox        = document.getElementById(productTextBoxID);
			showModalDialogFrame({
				url: '../FMWebApp/ProductSelectForm.aspx?Type=MAX_PRODUCT' + '&Map=MAX_MAP',
				width: 855,
				height: 690,
				onClose: function () {
					if (this.returnValue != null) {
						productTextBox.value = this.returnValue[0];
						productTextBox.title = this.returnValue[1];
						<%= Page.ClientScript.GetPostBackEventReference(this, PostBackRefreshDataArgument) %>
					}
				}
			});
		}
	</script>



	<%------------------------------------ top margin and title ------------------------------------%>
	<div id="topMargin" class="leftMargin" style="position: relative; top: 0px">
	</div>
	<fmcontrols:FMLabel ID="titleLabel" Style="position: relative; padding-left: 15px" runat="server"
		CssClass="headline" Width="500px" BackColor="Transparent">
		<%=GetTranslatedText(PageTitle)%></fmcontrols:FMLabel>
	<div id="spacer" class="leftMargin" style="position: relative; height: 8px">
	</div>
	<%------------------------------------ Criteria Section ------------------------------------%>
	<div id="criteriaSection" class="leftMargin" style="position: relative; top: 0px">
		<table style="position: relative; top: 0px; width:400px" role="presentation" aria-label="layout">
			<tr>
				<td style="position: relative; top: 0px; width:200px">
					<table>
						<tr>
							<td>
								<fmcontrols:FMLabel ID="managerLabel" runat="server" CssClass="formfieldtitle" Style="position: relative; width: 56px;" BackColor="Transparent"><%=GetTranslatedText(ManagerLabelText)%>:</fmcontrols:FMLabel>
							</td>
							<td>
								<nobr>
								<FMControls:FMCompanyTextBox runat="server" ID="managerTextBox" Role="MANAGER" 
									CssClass="formfield" style="POSITION: relative; "
									tabIndex="1"/>
							</nobr>
							</td>
						</tr>
						<tr>
							<td>
								<fmcontrols:FMLabel ID="productLabel" runat="server" CssClass="formfieldtitle" Style="position: relative; width: 56px;" BackColor="Transparent"><%=GetTranslatedText(ProductLabelText)%>:</fmcontrols:FMLabel>
							</td>
							<td>
								<nobr>
								<FMControls:FMProductTextBox runat="server" ID="productTextBox" 
									CssClass="formfield" style="POSITION: relative; "
									tabIndex="2"/>
							</nobr>
							</td>
						</tr>
					</table>
				</td>
				<td style="position: relative; top: 0px; width:20px">
					&nbsp;
				</td>
				<td style="vertical-align: top; position: relative; top: 0px; width:200px">
					<span style="vertical-align: middle">
						<nobr>						
						<FMCONTROLS:FMLABEL id="findStringLabel" AssociatedControlID="findTextBox" style="POSITION: relative; height: 15px; vertical-align:middle"
								runat="server" CssClass="formfieldtitle" BackColor="Transparent"><%=GetTranslatedText(FindStringLabelText)%>:</FMCONTROLS:FMLABEL>
						&nbsp;&nbsp;
						<asp:textbox id="findTextBox" style="POSITION: relative; width: 200px; vertical-align:middle"
							tabIndex="3" runat="server" MaxLength="100"/>
						&nbsp;&nbsp;
						<FMCONTROLS:FMBUTTON id="findBtn" style="POSITION: relative; vertical-align:middle" tabIndex="3"
							runat="server" Text="Find" CssClass="formfieldtitle" Width="64px" onclick="FindBtn_OnClick" />
						&nbsp;&nbsp;
						<FMCONTROLS:FMBUTTON id="showAllButton" style="POSITION: relative; vertical-align:middle"
							tabIndex="4" runat="server" Text="Show All" CssClass="formfieldtitle" Width="64px" onclick="ShowAllButton_OnClick"	/>
						
					</nobr>
					</span>
				</td>
			</tr>
		</table>
	</div>
	<%------------------------------------ Criteria Section ------------------------------------%>
	<div class="leftMargin" style="position: relative; height: 10px;">
		&nbsp;</div>
	<%------------------------------------ Grid Section ------------------------------------%>
	<div class="leftMargin" style="position: relative; height: 100px; width: 98%">
		<table id="mainTable" style="position: relative; border-spacing: 0; padding:1; border:0" role="presentation" aria-label="layout">
			<%------------------------------------ top add button row ------------------------------------%>
			<tr>
				<td class="buttonCell">
					<fmcontrols:FMButton ID="topAddButton" runat="server" Text="Add" CssClass="addButton formfieldtitle"
						TabIndex="1" OnClick="AddButton_Click" />
					&nbsp;&nbsp;
					<fmcontrols:FMPageSizeDropDown ID="pageSizeDropDown" ToolTip="Page size"  runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
				</td>
			</tr>
			<tr>
				<%------------------------------------ Main Grid ------------------------------------%>
				<td class="datagridCell">

					<fmcontrols:FMDataGrid ID="mainDataGrid" runat="server" CssClass="tabletext grid"
						AutoGenerateColumns="False" GridLines="Vertical" AllowSorting="True" AllowPaging="True"
						PageSize="16" OnEditCommand="DataGrid_EditCommand" OnItemDataBound="DataGrid_ItemDataBound"
						OnDeleteCommand="DataGrid_DeleteCommand" OnPageIndexChanged="DataGrid_PageIndexChanged" 
						OnSortCommand="DataGrid_SortCommand" aria-label="Main Data">

						<HeaderStyle CssClass="tablecolhead GVFixedHeader"  ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"/>
						<FooterStyle CssClass="gridFooter" />
						<ItemStyle CssClass="gridItem" ForeColor="Black" BackColor="#EEEEEE" />
						<AlternatingItemStyle CssClass="gridAlternatingItem" BackColor="Gainsboro" />
						<SelectedItemStyle CssClass="gridSelectedItem" />

						<PagerStyle CssClass="tablepager pgr gridPager" Mode="NumericPages" />

						<Columns>
							<%------------------------------------ Edit columns ------------------------------------%>
							<asp:TemplateColumn HeaderText="Edit">
								<HeaderStyle CssClass="editHeader"></HeaderStyle>
								<ItemStyle CssClass="editItem"></ItemStyle>
								<ItemTemplate>
									<fmcontrols:FMEditLinkButton runat="server" />
								</ItemTemplate>
							</asp:TemplateColumn>
							<%------------------------------------ Hidden columns ------------------------------------%>
							<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
							<asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
								<ItemTemplate>
									<asp:Label runat="server" ID="identityGuidLabel" Text='<%# BindColumn(Container, RuleGuidColumnName) %>' />
								</ItemTemplate>
							</asp:TemplateColumn>
							<%------------------------------------ Visible Data columns ------------------------------------%>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<fmcontrols:FMLabel runat="server"><%=GetTranslatedText(DefaultEOMColumnName)%></fmcontrols:FMLabel>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label ID="DefaultEOMLabel" runat="server" Text='<%# GetTranslatedText(DefaultEOMColumnName) %>' />
								</ItemTemplate>
								<ItemTemplate>
									<asp:CheckBox ID="DefaultEOMTextBox" alt="Default EOM" runat="server" CssClass="tabletext"
										MaxLength="255" Enabled="false" Checked='<%# BindColumn(Container, DefaultEOMColumnName) %>' >
									</asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<fmcontrols:FMLabel runat="server"><%=GetTranslatedText(DescriptionColumnName)%></fmcontrols:FMLabel>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label ID="DescriptionLabel" runat="server" Text='<%# BindColumn(Container, DescriptionColumnName) %>' />
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox ID="DescriptionTextBox" alt="Description" runat="server" CssClass="tabletext"
										MaxLength="255" Text='<%# BindColumn(Container, DescriptionColumnName) %>' >
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<fmcontrols:FMLabel runat="server"><%=GetTranslatedText(EnabledColumnName)%></fmcontrols:FMLabel>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label ID="EnabledLabel" runat="server" Text='<%# BindColumn(Container, EnabledColumnName) %>' />
								</ItemTemplate>
								<ItemTemplate>
									<asp:CheckBox ID="EnabledTextBox" alt="Enabled" runat="server" CssClass="tabletext"
										MaxLength="255" Enabled="false" Checked='<%# BindColumn(Container, EnabledColumnName) %>'>
									</asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<fmcontrols:FMLabel runat="server"><%=GetTranslatedText(ManagerListColumnName)%></fmcontrols:FMLabel>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label ID="ManagersLabel" runat="server" Text='<%# BindColumn(Container, ManagerListColumnName) %>' />
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox ID="ManagersTextBox" alt="Managers" runat="server" CssClass="tabletext"
										MaxLength="255" Text='<%# BindColumn(Container, ManagerListColumnName) %>' >
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<fmcontrols:FMLabel runat="server"><%=GetTranslatedText(OwnerListColumnName)%></fmcontrols:FMLabel>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label ID="OwnersLabel" runat="server" Text='<%# BindColumn(Container, OwnerListColumnName) %>' />
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox ID="OwnersTextBox" alt="Owners" runat="server" CssClass="tabletext"
										MaxLength="255" Text='<%# BindColumn(Container, OwnerListColumnName) %>' >
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<fmcontrols:FMLabel runat="server"><%=GetTranslatedText(ProductListColumnName)%></fmcontrols:FMLabel>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label ID="ProductsLabel" runat="server" Text='<%# BindColumn(Container, ProductListColumnName) %>' />
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox ID="ProductsTextBox" alt="Products" runat="server" CssClass="tabletext"
										MaxLength="255" Text='<%# BindColumn(Container, ProductListColumnName) %>' >
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<fmcontrols:FMLabel runat="server"><%=GetTranslatedText(ReasonCodeColumnName)%></fmcontrols:FMLabel>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label ID="ReasonCodeLabel" runat="server" Text='<%# BindColumn(Container, ReasonCodeColumnName) %>' />
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox ID="ReasonCodeTextBox" alt="Reason Code" runat="server" CssClass="tabletext"
										MaxLength="255" Text='<%# BindColumn(Container, ReasonCodeColumnName) %>' >
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderTemplate>
									<fmcontrols:FMLabel runat="server"><%=GetTranslatedText(RuleIDColumnName)%></fmcontrols:FMLabel>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label ID="RuleIDLabel" runat="server" Text='<%# BindColumn(Container, RuleIDColumnName) %>' />
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox ID="RuleIDTextBox" alt="Rule ID" runat="server" CssClass="tabletext"
										MaxLength="255" Text='<%# BindColumn(Container, RuleIDColumnName) %>' >
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<%------------------------------------ Delete columns ----------------------------------%>
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
			<%------------ bottom add button row ------------%>
			<tr>
				<td class="buttonCell">
					<fmcontrols:FMButton ID="bottomAddButton" runat="server" Text="Add" CssClass="formfieldtitle addButton"
						OnClick="AddButton_Click" />
				</td>
			</tr>
		</table>
	</div>
	</div>
</form>
</body>
</html>
