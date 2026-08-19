<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="TransactionAliasesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TransactionAliasesForm" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
</head>
<body ms_positioning="GridLayout">
		<style>
		#grid_scroll_div {
			max-height: calc(100vh - 240px) !important;
			overflow: auto;
		}
	</style>
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="352px" BackColor="Transparent">Transaction Aliases Configuration</FMControls:FMLabel>
			<table id="Table1" style="z-index: 100; left: 32px; width: 43.18%; position: absolute; top: 48px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
				<tr>
					<td width="350" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" TabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="AliasesFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" alt="Page size" />
					</td>
				</tr>
				<tr>
					<td style="width: 498px; height: 10px" width="498">
						<div id="grid_scroll_div">
						<FMControls:FMDataGrid ID="AliasesDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="ID"
							GridLines="Vertical" Width="400px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" PageSize="10" AllowPaging="True" CssClass="tabletext"
							Style="left: 1px; top: 0px" TabIndex="1" FixedHeight="539px" aria-label="Aliases">
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
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="AliasName" HeaderText="ID">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteTrxAliasLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></div></td>
				</tr>
				<tr>
					<td style="width: 498px; height: 50px" valign="middle" width="498">
						<table style="width: 392px; height: 28px" role="presentation" aria-label="layout">
							<tr>
								<td style="width: 115px">
									<FMControls:FMButton ID="AddButton" OnClientClick="return openAddScreen();" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
										TabIndex="2" ></FMControls:FMButton>
								</td>
								<td>
									<FMControls:FMButton ID="CreateDefaultButton" OnClientClick="return DefaultSelect();" runat="server" Width="120px" Text="Create Default" CssClass="formfieldtitle"
										TabIndex="3"></FMControls:FMButton>
									<div id="MarketDefaultSelect" hidden="hidden">
										<ul>
											<li>Aviation</li>
											<li>Terminal Automation</li>
										</ul>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</form>	
		
	<script type="text/javascript">

		var AddButton = document.getElementById("AddButton2");
		if (!AddButton.disabled)
			AddButton.focus();

		function openAddScreen() {
			var editorMode = $("#AddButton").attr( "data-editormode");
			if ( editorMode == "1" )
			{
				showModalDialogFrame({
					url: "../AccountingArea/TransactionAlias/TransactionAliasAdd",
					width: 855,
					height: 560,
					onClose: function () {
						if (this.returnValue != null) {
							FMMenu.Nav('../../MenuBar/FMMenuBar.aspx?target=../AccountingArea/TransactionAlias/TransactionAliasDetail/' + this.returnValue, '4013', '00000000-0000-0000-0000-000000000000');
							window_location('../../MenuBar/FMMenuBar.aspx?target=../AccountingArea/TransactionAlias/TransactionAliasDetail/' + this.returnValue, '_self');
							return false;
						}
					}
				});
				return false; }
			else {
				return true;
			}
		}

		function DefaultSelect() {
			showModalDialogFrame({
				url: "../FMWebApp/TransactionAliasDefaultSelectForm.aspx",
				width: 200,
				height: 200,
				title: "Transaction Defaults Select",
				onClose: function () {
					if (this.returnValue != null) {
						var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);

						__doPostBack('CreateDefaultButton', asciiValue1);
					}
				}
			});

			return false;
		}
	</script>

</body>
</html>
