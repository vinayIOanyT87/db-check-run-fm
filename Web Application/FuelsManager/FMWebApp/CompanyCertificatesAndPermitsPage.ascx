<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="CompanyCertificatesAndPermitsPage.ascx.cs" AutoEventWireup="True" Inherits="FMWebApp.CompanyCertificatesAndPermitsPage" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
	<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 0px; WIDTH: 238px; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" width="238" border="0">
		<TBODY>
			<TR>
				<TD><FMControls:FMDataGrid id="QualificationsDataGrid" runat="server" CssClass="tabletext" BackColor="White" RowHeaderColumn="Certificate ID"
						Width="320px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
						BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="8">
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
									<FMControls:FMEditLinkButton runat="server" />
								</ItemTemplate>
								<EditItemTemplate>
			<FMControls:FMUpdateLinkButton runat="server" />&nbsp;
<FMControls:FMCancelLinkButton runat="server" />
		
</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn Visible="False" HeaderText="Index">
								<ItemTemplate>
									<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Certificate ID">
								<HeaderStyle Wrap="False"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width=2in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.QualificationID") %>' ID="Label11">
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:dropdownlist CssClass=tabletext runat="server" Enabled="True" ID="QualificationsDropDownList" DataSource="<%# EnumerateQualifications()%>" DataTextField="Text" DataValueField="Value">
									</asp:dropdownlist>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Number">
								<HeaderStyle Wrap="False"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width=1.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label1">
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox Width=1.5in CssClass=tabletext runat="server" Enabled="True" ID="NumberTextBox" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' DataTextField="Text" DataValueField="Value" MaxLength=50>
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Expiration Date">
								<HeaderStyle Wrap="False"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width=1.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ExpirationDate") %>' ID="Label15">
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<FMControls:FMDate ID="ExpirationDate" Text='<%# DataBinder.Eval(Container, "DataItem.ExpirationDate") %>' CssClass="tabletext" runat="server">
									</FMControls:FMDate>
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
			</TR>
			<TR>
				<TD height="21"><FMControls:FMButton id="AddButton" runat="server" Width="66" CssClass="formfieldtitle" Text="Add"></FMControls:FMButton></TD>
			</TR>
		</TBODY></TABLE>
	</body>
</HTML>
