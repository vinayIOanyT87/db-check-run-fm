<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="WebLinksConfigurationForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.WebLinksConfigurationForm" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title>WebLinksConfigurationForm</title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body ms_positioning="GridLayout" tabindex="-1" role="application">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMLabel ID="TitleLabel" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="600px" BackColor="Transparent">External Web Links Configuration</FMControls:FMLabel>
			<table id="Table1" style="z-index: 100; left: 32px; position: absolute; top: 48px; height: 10px"
				cellspacing="0" cellpadding="1" border="0">
				<tr>
					<td width="350" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="TopAddButton" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="1" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="PageSizeDropDown" ToolTip="Page size" runat="server" TabIndex="2" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMDataGrid ID="DataGrid" runat="server" BorderStyle="None" BackColor="White" RowHeaderColumn="Link Name"
							AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
							BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext" Style="left: 1px; top: 0px"
							PageSize="16">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton runat="server" />&nbsp;
										<FMControls:FMCancelLinkButton runat="server" />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Guid">
									<ItemTemplate>
										<asp:Label ID="GuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LinkGuid") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Link Name">
									<HeaderStyle Width="1.5in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="LinkNameLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LinkName") %>' Width="1.5in">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="LinkNameTextBox" runat="server" CssClass="tabletext" aria-required="true" Text='<%# DataBinder.Eval(Container, "DataItem.LinkName") %>' Width="1.5in" MaxLength="100" ToolTip="Link Name">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Link Description">
									<HeaderStyle Width="3in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="LinkDescriptionLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LinkDescription") %>' Width="3in">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="LinkDescriptionTextBox" runat="server" CssClass="tabletext" aria-required="true" Text='<%# DataBinder.Eval(Container, "DataItem.LinkDescription") %>' Width="3in" MaxLength="200" ToolTip="Link Description">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Link Address">
									<HeaderStyle Width="4in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="LinkAddressLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LinkAddress") %>' Width="4in">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="LinkAddressTextBox" runat="server" CssClass="tabletext" aria-required="true" Text='<%# DataBinder.Eval(Container, "DataItem.LinkAddress") %>' Width="4in" MaxLength="2000" ToolTip="Link Address">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></td>
				</tr>
				<tr>
					<td style="width: 498px; height: 50px" valign="middle" width="498">
						<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton>
					</td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
