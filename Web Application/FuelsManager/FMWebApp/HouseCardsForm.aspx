<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="HouseCardsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.HouseCardsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<TABLE id="Table1" style="Z-INDEX: 100; LEFT: 32px; WIDTH: 760px; POSITION: absolute; TOP: 48px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD style="width:760px; HEIGHT: 36px" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							tabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="HouseCardsFormPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
					</TD>
				</tr>
				<TR>
					<TD style="width:760px; HEIGHT: 10px">
                        <FMControls:FMDataGrid id="HouseCardsDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="ID"
							GridLines="Vertical" Width="760px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							style="LEFT: 1px; TOP: 0px" PageSize="16">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="tabletext" BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" CssClass="tabletext" BackColor="#EEEEEE"></ItemStyle>
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
								<asp:TemplateColumn>
									<HeaderTemplate><FMControls:FMLabel ID="IDHeader" runat="server" Text="ID" /><span style="COLOR: red"> *</span></HeaderTemplate>
									<ItemTemplate>
										<asp:Label id=Label4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=IDTextBox runat="server" ToolTip="ID" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in" MaxLength="50">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Number">
									<HeaderTemplate><FMControls:FMLabel ID="NumberHeader" runat="server" Text="Number" /><span style="COLOR: red"> *</span></HeaderTemplate>
									<ItemTemplate>
										<asp:Label width="2in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Number") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="NumberTextBox" ToolTip="Number" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.Number") %>' width="2in" MaxLength="50">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Driver">
									<ItemTemplate>
										<asp:Label width="2in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DriverID") %>' ID="Label3">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:dropdownlist width="2in" ToolTip="Driver" CssClass=tabletext runat="server" Enabled="True" ID="DriversDropDownList" DataSource="<%# EnumerateDrivers()%>" DataTextField="Text" DataValueField="Value">
										</asp:dropdownlist>
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
					<TD style="WIDTH: 760px; HEIGHT: 34px" vAlign="middle"><FMControls:FMButton id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></TD>
				</TR>
			</TABLE>
			<asp:Image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image><FMControls:FMLabel id="Label2" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="256px" BackColor="Transparent">House Cards Configuration</FMControls:FMLabel>
		</div>
</form>
	</body>
</HTML>
