<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControlLogForm.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.ControlLogForm" EnableSessionState="True" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xml:lang="en" lang="en">
<head>
	<title></title>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" type="text/css" rel="stylesheet" />
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/controlLogForm.css" %>" rel="stylesheet" type="text/css" />
</head>
<body>
	<form id="mainForm" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<div  style="position: relative;">
				<%------------------------------------ background image and title ------------------------------------%>
				<FMControls:FMLabel ID="titleLabel" Style="z-index: 103; left: 8px; position: absolute; top: 8px"
					runat="server" CssClass="headline" Width="500px" BackColor="Transparent" Text="Control Log"/>
				<asp:Image ID="backgroundImage" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px"
					runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			</div>
			<script type="text/javascript" language="javascript">
				function OpenPrintFriendlyWindow() 
				{
					window_open('ControlLogPrintFriendlyForm.aspx');
				}
			</script>
			<div style="position: relative;">
				<table id="mainTable" style="z-index: 100; position: absolute; top: 48px; left: 32px; border-spacing: 0; padding: 1; border: 0; width: 800px; margin-top: 0px;">
					<%------------------------------------  filtering controls row ------------------------------------%>
					<tr>
						<td colspan="2">
							<FMControls:FMLabel ID="StartDateLabel" runat="server" BackColor="Transparent" Text="Order Number"
								Width="88px" CssClass="formfieldtitle">Start Date</FMControls:FMLabel>
							<FMControls:FMDate ID="StartDate" runat="server" TabIndex="1" Width="160px" CssClass="formfield"></FMControls:FMDate>
							<span>
								<FMControls:FMLabel ID="StopDateLabel" runat="server" BackColor="Transparent" Text="Order Number"
									Width="88px" CssClass="formfieldtitleBesideCalendar">End Date</FMControls:FMLabel></span>
							<span>
								<FMControls:FMDate ID="StopDate" runat="server" TabIndex="2" Width="160px" CssClass="formfield"></FMControls:FMDate>
							</span><span>
								<FMControls:FMCheckBox ID="ShowDeletedItemsCheckBox" Style="z-index: 202;" runat="server"
									CssClass="formfieldBesideCalendar" TabIndex="3" Text="Show Deleted Items" AutoPostBack="true"
									OnCheckedChanged="ShowDeletedItemsCheckBoxOnCheckedChanged" /></span>
						</td>
					</tr>
					<tr>
						<td>&nbsp;
						</td>
					</tr>
					<%------------------------------------ top button row ------------------------------------%>
					<tr>
						<td class="buttonCell">
							<FMControls:FMButton ID="topAddButton" Width="66px" runat="server" Text="Add" CssClass="formfieldtitle"
								TabIndex="4" OnClick="AddButtonClick" />
							&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="pageSizeDropDown" ToolTip="Page size" runat="server" TabIndex="5"
							OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />					
							&nbsp;&nbsp;
							<asp:LinkButton ID="PrintLinkBtn" OnClientClick="OpenPrintFriendlyWindow()" runat="server">Print Friendly</asp:LinkButton>
						</td>
						<td>&nbsp;
						</td>
					</tr>
					<tr>
						<%------------------------------------ Main Grid ------------------------------------%>
						<td valign="top">
							<FMControls:FMDataGrid ID="mainDataGrid" runat="server" CssClass="tabletext grid"
								AutoGenerateColumns="False" GridLines="Vertical" Enabled="true" AllowSorting="True"
								AllowPaging="True" PageSize="16" OnEditCommand="DataGridEditCommand" OnUpdateCommand="DataGridUpdateCommand"
								OnCancelCommand="DataGridCancelCommand" OnDeleteCommand="DataGridDeleteCommand"
								OnItemDataBound="DataGridItemDataBound" OnPageIndexChanged="DataGridPageIndexChanged"
								OnItemCreated="DataGrid_OnItemCreated" CellPadding="0" Width="800" TabIndex="6">
								<HeaderStyle CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>" />
								<FooterStyle CssClass="gridFooter" />
								<ItemStyle CssClass="gridItem" />
								<AlternatingItemStyle CssClass="gridAlternatingItem" />
								<SelectedItemStyle CssClass="gridSelectedItem" />
								<PagerStyle CssClass="tablepager pgr gridPager" Mode="NumericPages" BackColor="<%$ AppSettings: ColorHeaderBlue %>" />
								<Columns>
									<%------------------------------------ Edit/Delete columns ------------------------------------%>
									<asp:TemplateColumn HeaderText="Edit" ItemStyle-Width="60">
										<HeaderStyle></HeaderStyle>
										<ItemStyle Width="60"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMEditLinkButton ID="FMEditLinkButton1" runat="server" />
										</ItemTemplate>
										<EditItemTemplate>
											<FMControls:FMUpdateLinkButton ID="FMUpdateLinkButton1" runat="server" />&nbsp;
										<FMControls:FMCancelLinkButton ID="FMCancelLinkButton1" runat="server" />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<%------------------------------------ Hidden columns ------------------------------------%>
									<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
									<asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
										<ItemTemplate>
											<asp:Label runat="server" ID="siteGuidLabel" Text='<%# BindColumn(Container, SiteGuidColumnName) %>' />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
									<asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
										<ItemTemplate>
											<asp:Label runat="server" ID="identityGuidLabel" Text='<%# BindColumn(Container, IdentityGuidColumnName) %>' />
										</ItemTemplate>
									</asp:TemplateColumn>
									<%------------------------------------ Visible Data columns ------------------------------------%>
									<asp:TemplateColumn ItemStyle-Width="155">
										<HeaderTemplate>
											<FMControls:FMLabel ID="FMLabel1" runat="server" Text="Event Time" />
										</HeaderTemplate>
										<ItemTemplate>
											<asp:Label ID="eventTimeLabel" runat="server" Text='<%# BindColumn(Container, EventTimeColumnName) %>' />
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox ID="eventTimeTextBox" runat="server" MaxLength="50"
												Text='<%# BindColumn(Container, EventTimeColumnName) %>' />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn ItemStyle-Width="155">
										<HeaderTemplate>
											<FMControls:FMLabel ID="FMLabel2" runat="server" Text="Controller" />
										</HeaderTemplate>
										<ItemTemplate>
											<asp:Label ID="controllerColumnLabel" runat="server" Text='<%# BindColumn(Container, ControllerColumnName) %>' />
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox ID="controllerColumnTextBox" runat="server" MaxLength="255"
												Text='<%# BindColumn(Container, ControllerColumnName) %>' Enabled="false" ReadOnly="true" />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<HeaderTemplate>
											<FMControls:FMLabel ID="FMLabel3" runat="server" Text="Memo" />
										</HeaderTemplate>
										<ItemTemplate>
											<asp:Label ID="memoColumnLabel" runat="server" Text='<%# BindColumn(Container, MemoColumnName) %>' />
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox ID="memoColumnTextBox" runat="server" MaxLength="255"
												Text='<%# BindColumn(Container, MemoColumnName) %>' Width="345" />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Delete" ItemStyle-Width="50">
										<HeaderStyle></HeaderStyle>
										<ItemStyle Width="50"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMDeleteLinkButton ID="FMDeleteLinkButton1" runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</FMControls:FMDataGrid>
						</td>
						<td valign="top">
							<FMControls:FMButton ID="RefreshButton" Style="z-index: 105;" TabIndex="7" runat="server"
								Width="80px" CssClass="formfieldtitle" Text="Refresh" OnClick="RefreshButtonClick"></FMControls:FMButton>
							<FMControls:FMButton ID="CloseButton" Style="z-index: 105;" TabIndex="8" runat="server"
								Width="80px" CssClass="formfieldtitleWithPadding" Text="Close" OnClick="CloseButtonOnClick"></FMControls:FMButton>
						</td>
					</tr>
					<%------------ bottom button row ------------%>
					<tr>
						<td class="buttonCell">
							<FMControls:FMButton ID="bottomAddButton" runat="server" Text="Add" Width="66px" CssClass="formfieldtitle"
								OnClick="AddButtonClick" TabIndex="9" />
						</td>
					</tr>
				</table>
			</div>
		</div>
	</form>
</body>
</html>
