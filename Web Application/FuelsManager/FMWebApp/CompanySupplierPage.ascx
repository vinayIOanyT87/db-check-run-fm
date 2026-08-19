<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="CompanySupplierPage.ascx.cs" Inherits="FMWebApp.CompanySupplierPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
		<FMCONTROLS:FMLABEL id="Label5" style="Z-INDEX: 114; LEFT: 0px; POSITION: absolute; TOP: 8px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Authorized Products:</FMCONTROLS:FMLABEL>
	<TABLE id="Table2" style="Z-INDEX: 114; LEFT: 0px; WIDTH: 238px; POSITION: absolute; TOP: 30px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" width="238" border="0">
		<TR>
			<TD width="507" height="10"><FMCONTROLS:FMDATAGRID id="AuthorizedProductsDataGrid" runat="server" Width="725px" CssClass="tabletext" RowHeaderColumn="Product ID"
					BackColor="White" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
					CellPadding="3" AllowPaging="True" PageSize="4">
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
								<FMControls:FMEditLinkButton ID="EditButton" runat="server" />
							</ItemTemplate>
							<EditItemTemplate>
									<FMControls:FMUpdateLinkButton ID="FMUpdateLinkButton1" runat="server" />&nbsp;
									<FMControls:FMCancelLinkButton ID="FMCancelLinkButton1" runat="server" />
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn Visible="False" HeaderText="Index">
							<ItemTemplate>
								<asp:label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
								</asp:label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Product ID">
							<ItemTemplate>
								<asp:label Width=2.0in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' Tooltip='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' Style="overflow:hidden;white-space:nowrap;text-overflow:ellipsis" ID="Label2">
								</asp:label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:dropdownlist Width=2.0in CssClass=tabletext runat="server" Enabled="True" ID="ProductsDropDownList" DataSource="<%# EnumerateProducts()%>" DataTextField="Text" DataValueField="Value">
								</asp:dropdownlist>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Delete">
							<HeaderStyle Width="0.4in"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
					<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
				</FMCONTROLS:FMDATAGRID></TD>
		</TR>
		<TR>
			<TD width="507" height="10"><FMCONTROLS:FMBUTTON id="AddProductButton" tabIndex="1" Width="66" runat="server" CssClass="formfieldtitle" Text="Add"></FMCONTROLS:FMBUTTON></TD>
		</TR>
	</TABLE>
	</body>
</HTML>
