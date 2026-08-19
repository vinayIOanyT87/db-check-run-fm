<%@ Page Language="c#" CodeBehind="WebLinksForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.WebLinksForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title>WebLinksForm</title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMLabel ID="TitleLabel" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="600px" BackColor="Transparent">External Web Links</FMControls:FMLabel>
			<table id="Table1" style="z-index: 100; left: 32px; position: absolute; top: 48px; height: 10px"
				width="800" cellspacing="0" cellpadding="1" border="0">
				<tr>
					<td width="350" height="36" valign="middle">
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
								<asp:TemplateColumn HeaderText="Link Name">
									<HeaderStyle Width="2in"></HeaderStyle>
									<ItemTemplate>
										<asp:HyperLink ID="LinkNameHyperlink" runat="server" CssClass="DefaultLink" Text='<%# DataBinder.Eval(Container, "DataItem.LinkName") %>' NavigateUrl='<%# DataBinder.Eval(Container, "DataItem.LinkAddress") %>' Target="_blank" Width="2in">
										</asp:HyperLink>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Link Description">
									<HeaderStyle Width="6in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="LinkDescriptionLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LinkDescription") %>' Width="6in">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
