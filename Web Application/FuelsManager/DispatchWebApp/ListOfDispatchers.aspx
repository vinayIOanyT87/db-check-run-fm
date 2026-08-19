<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListOfDispatchers.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.ListOfDispatchers" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xml:lang="en" lang="en">
<head>
	<title></title>
	<meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/menu.css" %>" type="text/css" />
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body oncontextmenu="return false;">
	<form id="mainForm" runat="server">
	<!-- Main MenuBar -->
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
	<div id="pageContent">
		<div style="position: relative;">
		<FMControls:FMLabel ID="titleLabel" Style="z-index: 103; left: 8px; position: absolute;
			top: 8px" runat="server" CssClass="headline" Width="500px" BackColor="Transparent"
			Text="Dispatchers Logged Into System" />
	</div>
		<div style="position: relative;">
		<table id="mainTable" style="z-index: 100; position: absolute; top: 48px; left: 32px;
			border-spacing: 0; padding: 1px; border: 0; margin-top:0px;">
			<tr>
				<td>
					<!-- Datagrid for Dispatchers -->
					<FMControls:FMDataGrid ID="DispatchersDataGrid" Style="z-index: 102; left: 105px;
						top: 10px" TabIndex="5" runat="server" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
						Width="2.5in" CssClass="tabletext" PageSize="12" CellPadding="3" BorderColor="White"
						AllowSorting="True" BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False"
						BorderStyle="None">
						<FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>" />
						<Columns>
							<asp:BoundColumn DataField="ID" HeaderText="User ID">
								<HeaderStyle Width="2in"></HeaderStyle>
							</asp:BoundColumn>
						</Columns>
						 <Columns>
							<asp:BoundColumn DataField="Name" HeaderText="Name">
								<HeaderStyle Width="2in"></HeaderStyle>
							</asp:BoundColumn>
						</Columns>
						<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
							Mode="NumericPages"></PagerStyle>
					</FMControls:FMDataGrid>
				</td>
				<td style="vertical-align: top; padding-left: 50px;">
					<FMControls:FMButton ID="btnClose" OnClick="CloseOnClick" Text="Close" runat="server" CssClass="formfieldtitle"/>
				</td>
			</tr>
		</table>
	</div>
	</div>
	</form>
</body>
</html>
