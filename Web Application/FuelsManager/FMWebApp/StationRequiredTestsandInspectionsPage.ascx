<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StationRequiredTestsandInspectionsPage.ascx.cs" Inherits="FuelsManager.FMWebApp.StationRequiredTestsandInspectionsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
	<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 0px; WIDTH: 238px; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" width="238" border="0" role="presentation" aria-label="layout">
		<TBODY>
			<TR>
				<TD><FMControls:FMDataGrid id="QualificationsDataGrid" runat="server" CssClass="tabletext" BackColor="White"
						Width="320px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
						BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="8" aria-label="Qualifications">
						<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
						<Columns>
							<asp:TemplateColumn HeaderText="Edit">
								<HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<FMControls:FMEditLinkButton ID="FMEditLinkButton1" runat="server" />
								</ItemTemplate>
								<EditItemTemplate>
		                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
		                            <FMControls:FMUpdateLinkButton ID="FMUpdateLinkButton1" runat="server" />&nbsp;
                                <FMControls:FMCancelLinkButton ID="FMCancelLinkButton1" runat="server" />
                            </EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn Visible="False" HeaderText="Index">
								<ItemTemplate>
									<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Inspection ID">
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
							<asp:TemplateColumn HeaderText="Delete">
								<HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
								<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								<ItemTemplate>
									<FMControls:FMDeleteLinkButton ID="FMDeleteLinkButton1" runat="server" />
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
						<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
					</FMControls:FMDataGrid></TD>
			</TR>
			<TR>
				<TD height="21"><FMControls:FMButton id="AddButton" runat="server"  Width="100px" CssClass="formfieldtitle" Text="Add"></FMControls:FMButton></TD>
			</TR>
		</TBODY>
	</TABLE>
	</body>
</HTML>
