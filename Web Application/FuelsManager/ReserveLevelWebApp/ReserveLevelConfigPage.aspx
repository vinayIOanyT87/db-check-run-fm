<%@ Page language="c#" Codebehind="ReserveLevelConfigPage.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.ReserveLevelWebApp.ReserveLevelConfigPage" %>
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
	<body MS_POSITIONING="GridLayout">
		<form id="ReserveLevelForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
            <FMCONTROLS:FMLABEL id="PageTitle" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" Text="Reserve Level Configuration" Width="500px" CssClass="headline"></FMCONTROLS:FMLABEL>
			<TABLE id="Table1" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 808px; POSITION: absolute; TOP: 32px; HEIGHT: 75px"
				cellSpacing="0" cellPadding="1" width="808" border="0" role="presentation" aria-label="layout">
				<TBODY>
					<TR>
						<TD><FMCONTROLS:FMBUTTON id="Add1Btn" tabIndex="6" runat="server" Text="Add" CssClass="formfieldtitle" width="100px" onclick="AddButton_Clicked"></FMCONTROLS:FMBUTTON>&nbsp;&nbsp;
							<FMCONTROLS:FMPAGESIZEDROPDOWN id="PageSizeDropdown" ToolTip="Page size" tabIndex="7" runat="server"></FMCONTROLS:FMPAGESIZEDROPDOWN></TD>
					</TR>
					<TR>
						<TD><FMCONTROLS:FMDATAGRID id="ReserveLevelDataGrid" runat="server" BackColor="White" Width="792px" CssClass="tabletext"  RowHeaderColumn="Product"
								BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
								BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="16" aria-label="Reserve Levels">
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
											<FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton1" />
										</ItemTemplate>
										<EditItemTemplate>
											<FMControls:FMUpdateLinkButton runat="server" ID="Fmupdatelinkbutton1" />&nbsp;
											<FMControls:FMCancelLinkButton runat="server" ID="Fmcancellinkbutton1" />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
									<asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
										<ItemTemplate>
											<asp:Label id="ReserveLevelGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ReserveLevelGuid") %>' >
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Product">
										<HeaderStyle Width="2in"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="ProductLabel" runat="server" CssClass="formfield" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' width="2in">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:DropDownList id="ProductDropDownList" runat="server" CssClass="tabletext" Width="196px" DataSource='<%# EnumerateProducts %>' DataTextField="Text" DataValueField="TextValue" ToolTip="Product">
											</asp:DropDownList>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Minimum Level">
										<HeaderStyle Width="2in"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="MinLevelLabel" runat="server" CssClass="formfield" Text='<%# DataBinder.Eval(Container, "DataItem.MinimumLevel") %>' width="3in">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="MinLevelTextBox" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.MinimumLevel") %>' width="2in" Columns="15" MaxLength="15" ToolTip="Minimum level">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Warning Level">
										<HeaderStyle Width="2in"></HeaderStyle>
										<ItemTemplate>
											<asp:Label id="WarningLevelLabel" runat="server" CssClass="formfield" Text='<%# DataBinder.Eval(Container, "DataItem.WarningLevel") %>' width="3in">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id="WarningLevelTextBox" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.WarningLevel") %>' MaxLength="15" width="2in" Columns="15" ToolTip="Warning level">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Delete">
										<HeaderStyle Width="0.5in"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMDATAGRID></TD>
					</TR>
					<TR>
						<TD><FMCONTROLS:FMBUTTON id="Add2Btn" tabIndex="6" runat="server" Text="Add" CssClass="formfieldtitle" width="100px" onclick="AddButton_Clicked"></FMCONTROLS:FMBUTTON></TD>
					</TR>
				</TBODY>
			</TABLE>
		</div>
</form>
	
	</body>
</HTML>
