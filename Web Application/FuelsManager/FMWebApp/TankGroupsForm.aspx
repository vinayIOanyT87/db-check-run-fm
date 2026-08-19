<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="TankGroupsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TankGroupsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="288px" BackColor="Transparent">Tank Groups Configuration</FMControls:FMLabel>
			<table id="Table1" style="z-index: 100; left: 32px; width: 43.18%; position: absolute; top: 48px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td width="498" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="TankGroupsFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" alt="Page Size" />
					</td>
				</tr>
				<tr>
					<td style="width: 498px; height: 10px" width="498">
						<FMControls:FMDataGrid ID="TankGroupsDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="ID"
							GridLines="Vertical" Width="400px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							Style="left: 1px; top: 0px" aria-label="Tank Groups">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="ID">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></td>
				</tr>
				<tr>
					<td style="width: 498px; height: 50px" valign="middle" width="498">
						<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
				</tr>
			</table>
		</div>
	</form>
	<script type="text/javascript">
		var AddButton = document.getElementById("AddButton2");
		if (!AddButton.disabled)
			AddButton.focus();
	</script>
</body>
</html>
