<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="AllocationGroupsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AllocationGroupsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				BackColor="Transparent" Width="320px" CssClass="headline">Allocation Groups Configuration</FMControls:FMLabel>
			<table id="Table1" style="z-index: 100; left: 32px; width: 700px; position: absolute; top: 48px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<TD width="498" height="36" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="AllocationGroupsFormPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
					</TD>
				</tr>
				<tr>
					<td style="width: 685px; height: 10px" width="498">
						<FMControls:FMDataGrid ID="ApplicationStringsDataGrid" Style="left: 1px; top: 0px" runat="server" BorderStyle="None" RowHeaderColumn="Allocation Group ID"
							BackColor="White" AutoGenerateColumns="False" GridLines="Vertical" Width="685px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3"
							AllowPaging="True" CssClass="tabletext" PageSize="16" TabIndex="3" aria-label="Allocation Groups">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Select">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
									<ItemTemplate>
										<FMControls:FMSelectLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
									<EditItemTemplate>
                                        <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                        <FMControls:FMCancelLinkButton runat="server" />
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
								<asp:TemplateColumn HeaderText="Allocation Group ID">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' ID="Label1">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox Width="5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' CssClass="tabletext" ID="StringTextBox" ToolTip="Allocation Group ID" MaxLength="30" alt="Allocation Group ID">
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
						</FMControls:FMDataGrid></TD>
					<td style="width: 498px; height: 10px" width="498"></td>
				</tr>
				<tr>
					<td style="width: 250px; height: 31px" valign="left" width="250" colspan="1">
						<table role="presentation" aria-label="layout">
							<tbody>
								<tr>
									<td style="width: 122px">
										<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
											TabIndex="1"></FMControls:FMButton></td>
									<td>
										<FMControls:FMButton ID="ResetButton" runat="server" Width="98px" Text="Reset" CssClass="formfieldtitle"
											TabIndex="2"></FMControls:FMButton></td>
								</tr>
							</tbody>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
