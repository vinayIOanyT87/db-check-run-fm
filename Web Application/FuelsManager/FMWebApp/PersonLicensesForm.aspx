<%@ Page language="c#" Codebehind="PersonLicensesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonLicensesForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
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
        <div id="pageContent" style="position:absolute">
			<asp:Image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMLabel id="Label3" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="312px" BackColor="Transparent">Personnel Licenses Configuration</FMControls:FMLabel>
			<TABLE id="Table1" style="Z-INDEX: 100; LEFT: 32px; WIDTH: 640px; POSITION: absolute; TOP: 48px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD style="WIDTH: 640px; HEIGHT: 36px" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							tabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="PersonLicensesFormPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 640px; HEIGHT: 10px">
						<FMControls:FMDataGrid id="QualificationsDataGrid" runat="server" BorderStyle="None" BackColor="White" RowHeaderColumn="License ID"
							AutoGenerateColumns="False" GridLines="Vertical" Width="640px" BorderWidth="1px" AllowSorting="True"
							BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext" style="LEFT: 1px; TOP: 0px"
							PageSize="16">
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
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="License ID">
									<HeaderStyle Width="2in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=IDTextBox runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in" MaxLength="50" ToolTip="Personnel license ID" aria-required="true">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Description">
									<HeaderStyle Width="3in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label width=3in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' ID="Label4">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox width=3in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' CssClass="tabletext" ID="DescriptionTextBox" ToolTip="Personnel License description" MaxLength="50">
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
				</TR>
				<TR>
					<TD style="WIDTH: 640px; HEIGHT: 50px" vAlign="middle"><FMControls:FMButton id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></TD>
				</TR>
			</TABLE>
		</div>
</form>
	</body>
</HTML>
