<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="CompanyOwnerPage.ascx.cs" AutoEventWireup="True" Inherits="FMWebApp.CompanyOwnerPage" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
	<FMCONTROLS:FMLABEL id="Label5" style="Z-INDEX: 114; LEFT: 0px; POSITION: absolute; TOP: 8px" runat="server"
		Width="136px" CssClass="formfieldtitle" BackColor="Transparent">Unavailable Inventories:</FMCONTROLS:FMLABEL>
	<TABLE id="Table2" style="Z-INDEX: 113; LEFT: 0px; WIDTH: 238px; POSITION: absolute; TOP: 30px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" width="238" border="0">
		<TR>
			<TD width="507" height="10"><FMCONTROLS:FMDATAGRID id="UnavailableInventoriesDataGrid" runat="server" Width="725px" CssClass="tabletext" RowHeaderColumn="Product ID"
					BackColor="White" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
					BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="4">
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
							<FMControls:FMUpdateLinkButton runat="server" ID="Fmupdatelinkbutton1" NAME="Fmupdatelinkbutton1" />&nbsp;
							<FMControls:FMCancelLinkButton runat="server" ID="Fmcancellinkbutton1" NAME="Fmcancellinkbutton1" />
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
								<asp:label Width=1.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' Tooltip='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' Style="overflow:hidden;white-space:nowrap;text-overflow:ellipsis" ID="Label2">
								</asp:label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:dropdownlist Width=1.5in CssClass=tabletext runat="server" Enabled="True" ID="ProductsDropDownList" DataSource="<%# EnumerateProducts()%>" DataTextField="Text" DataValueField="Value">
								</asp:dropdownlist>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Gross">
							<ItemTemplate>
								<asp:label Width=1in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Gross") %>' Tooltip='<%# DataBinder.Eval(Container, "DataItem.Gross") %>' Style="overflow:hidden;white-space:nowrap;text-overflow:ellipsis" ID="GrossLabel">
								</asp:label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:textbox Width=1in MaxLength=30 CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Gross") %>' Enabled="True" ID="GrossTextBox">
								</asp:textbox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Net">
							<ItemTemplate>
								<asp:label Width=1in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Net") %>' Tooltip='<%# DataBinder.Eval(Container, "DataItem.Net") %>' Style="overflow:hidden;white-space:nowrap;text-overflow:ellipsis" ID="NetLabel">
								</asp:label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:textbox Width=1in MaxLength=30 CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Net") %>' Enabled="True" ID="NetTextbox">
								</asp:textbox>
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
			<TD width="507" height="10"><FMCONTROLS:FMBUTTON id="AddProductButton" Width="66" tabIndex="1" runat="server" CssClass="formfieldtitle" Text="Add"></FMCONTROLS:FMBUTTON></TD>
		</TR>
	</TABLE>
	</body>
</HTML>
