<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="ProcessVariableMessagesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProcessVariableMessagesForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="416px" BackColor="Transparent">Process Variable Messages Configuration</FMControls:FMLabel>
			<table id="Table1" style="z-index: 100; left: 32px; width: 652px; position: absolute; top: 48px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td style="width: 652px; height: 36px" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="ProcessVariableMessagesFormPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
					</TD>
				</tr>
				<tr>
					<td style="width: 652px; height: 10px">
						<FMControls:FMDataGrid ID="ApplicationStringsDataGrid" Style="left: 1px; top: 0px" runat="server" CssClass="tabletext" RowHeaderColumn="Process Variable Message"
							AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="652px"
							GridLines="Vertical" AutoGenerateColumns="False" BackColor="White" BorderStyle="None" PageSize="16"
							aria-label="Process Variables">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" ID="EditButton" NAME="FMEditLinkButton" />
									</ItemTemplate>
									<EditItemTemplate>
								<FMControls:FMUpdateLinkButton runat="server" ID="FMUpdateLinkButton" NAME="FMUpdateLinkButton" />&nbsp;
							<FMControls:FMCancelLinkButton runat="server" ID="FMCancelLinkButton" NAME="FMCancelLinkButton" />
							</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>' ID="SiteGuidLabel">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>' ID="IdentityGuidLabel">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Process Variable Message">
									<HeaderStyle Width="525px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' ID="Label1" NAME="Label1">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox width="99%" runat="server" ToolTip="Process variable message" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' CssClass="tabletext" ID="StringTextBox" MaxLength="120">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" ID="DeleteButton" NAME="FMDeleteLinkButton" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></td>
				</tr>
				<tr>
					<td style="width: 498px; height: 35px" valign="middle" width="498">
						<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
