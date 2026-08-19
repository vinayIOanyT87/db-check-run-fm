<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="TransactionList.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.TransactionList" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<asp:Image ID="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMButton ID="RefreshButton" Style="Z-INDEX: 115; LEFT: 632px; POSITION: absolute; TOP: 74px"
				runat="server" Text="Refresh" CssClass="formfieldtitle" Width="72px" OnClientClick="ResetExportControl()">
			</FMControls:FMButton>
			<FMControls:FMLabel ID="TransactionTypeLabel" AssociatedControlID="TransactionTypeDropDownList" Style="Z-INDEX: 112; LEFT: 16px; POSITION: absolute; TOP: 104px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Transaction Type:</FMControls:FMLabel>
			<asp:Label ID="ProductValueLabel" Style="Z-INDEX: 111; LEFT: 432px; POSITION: absolute; TOP: 104px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px">ProductValueLabel</asp:Label>
			<FMControls:FMLabel ID="ProductLabel" Style="Z-INDEX: 110; LEFT: 320px; POSITION: absolute; TOP: 104px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Product:</FMControls:FMLabel>
			<asp:Label ID="OwnerValueLabel" Style="Z-INDEX: 109; LEFT: 432px; POSITION: absolute; TOP: 72px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px">OwnerValueLabel</asp:Label>
			<FMControls:FMLabel ID="OwnerLabel" Style="Z-INDEX: 108; LEFT: 320px; POSITION: absolute; TOP: 72px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Owner:</FMControls:FMLabel>
			<asp:Label ID="SiteValueLabel" Style="Z-INDEX: 107; LEFT: 136px; POSITION: absolute; TOP: 72px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="144px">SiteValueLabel</asp:Label>
			<FMControls:FMLabel ID="SiteLabel" Style="Z-INDEX: 106; LEFT: 16px; POSITION: absolute; TOP: 72px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Site:</FMControls:FMLabel>
			<asp:Label ID="DateValueLabel" Style="Z-INDEX: 105; LEFT: 136px; POSITION: absolute; TOP: 40px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="144px">DateValueLabel</asp:Label>
			<FMControls:FMLabel ID="DateLabel" Style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 40px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Date:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label2" Style="Z-INDEX: 101; LEFT: 16px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" CssClass="headline" Width="100px"> Accounting</FMControls:FMLabel>
			<FMControls:FMLabel ID="ManagerLabel" Style="Z-INDEX: 102; LEFT: 320px; POSITION: absolute; TOP: 40px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Manager:</FMControls:FMLabel>
			<asp:Label ID="ManagerValueLabel" Style="Z-INDEX: 103; LEFT: 432px; POSITION: absolute; TOP: 40px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="184px">ManagerValueLabel</asp:Label>
			<asp:DropDownList ID="TransactionTypeDropDownList" Style="Z-INDEX: 113; LEFT: 136px; POSITION: absolute; TOP: 104px"
				runat="server" CssClass="formfield" Width="160px" OnSelectedIndexChanged="TransactionTypeDropDownListSelectedIndexChanged">
			</asp:DropDownList>
			<FMControls:FMButton ID="CloseButton" Style="Z-INDEX: 114; LEFT: 632px; POSITION: absolute; TOP: 38px"
				runat="server" Text="Close" CssClass="formfieldtitle" Width="72px" OnClientClick="ResetExportControl()">
			</FMControls:FMButton>
			<FMControls:FMLabel ID="DateTypeLabel" Style="z-index: 108; left: 320px; position: absolute;
				top: 136px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Date Type:</FMControls:FMLabel>
			<asp:Label ID="DateTypeValueLabel" Style="z-index: 107; left: 432px; position: absolute;
				top: 136px" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="144px">DateTypeLabel</asp:Label>
			<FMControls:FMDropDownList ID="ExportDropDown" runat="server" Width="80px" CssClass="formfield" 
					Style="z-index: 116; left: 632px; position: absolute; top: 143px"
					AutoPostBack="False" onselectedindexchanged="ExportDropDownOnChanged">
				<asp:ListItem Value="">&lt;Format&gt;</asp:ListItem>
				<asp:ListItem Value="CSV">CSV</asp:ListItem>
				<asp:ListItem Value="Excel">Excel</asp:ListItem>
				<asp:ListItem Value="PDF">PDF</asp:ListItem>
				<asp:ListItem Value="Word">Word</asp:ListItem>
			</FMControls:FMDropDownList>
			<FMControls:FMButton ID="ExportButton" Style="z-index: 115; left: 720px; position: absolute; top: 136px" 
				runat="server" Text="Export" CssClass="formfieldtitle" Width="72px" OnClick="ExportButtonOnClick">
			</FMControls:FMButton>
			<table id="Table1" style="Z-INDEX: 101; LEFT: 16px; WIDTH: 600px; POSITION: absolute; TOP: 136px; HEIGHT: 10px"
				cellspacing="0" cellpadding="1" border="0">
				<tbody>
					<tr>
						<td width="350" height="36" valign="middle">
							<FMControls:FMButton ID="AddButton" runat="server" Width="96px" CssClass="formfieldtitle" Text="Add"></FMControls:FMButton>
						</td>
					</tr>
					<tr>
						<td style="WIDTH: 500px; HEIGHT: 10px">
							<FMControls:FMDataGridFixed ID="TransactionDataGrid" runat="server" BackColor="White" CssClass="tabletext" Width="1450px"
								BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" AllowPaging="True"
								BorderColor="White" CellPadding="3" Height="500px">
								<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Edit">
										<HeaderStyle Width="55px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMEditLinkButton runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</FMControls:FMDataGridFixed>
						</td>
					</tr>
					<tr>
						<td>
							<FMControls:FMButton ID="AddButton1" runat="server" Text="Add" CssClass="formfieldtitle" Width="96px"></FMControls:FMButton>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
		<script type="text/javascript">
			function ResetExportControl() {
				$("#ExportDropDown").prop('selectedIndex', 0);
			}
		</script>
	</form>
</body>
</html>
