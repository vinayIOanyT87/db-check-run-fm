<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="SitesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SitesForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<form id="SitesForm" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="ConfigurationLabel" Style="z-index: 104; left: 8px; position: absolute; top: 8px"
				runat="server" CssClass="headline" Width="232px" BackColor="Transparent">Sites Configuration</FMControls:FMLabel>
			<table id="Table1" style="z-index: 102; left: 32px; width: 448px; position: absolute; top: 48px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" width="448" role="presentation" aria-label="layout">
				<tr>
					<td width="539" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="6" />
					</td>
				</tr>
				<tr>
					<td style="width: 642px; height: 10px" width="642">
						<FMControls:FMDataGridFixed ID="SitesDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="Site Name"
							GridLines="Vertical" Width="453px" BorderWidth="1px" AllowSorting="True" BorderColor="#999999" CellPadding="3"
							TabIndex="1" AllowPaging="True" CssClass="tabletext" Style="left: 1px; top: 0px" PageSize="14" aria-label="Sites">
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
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="Site Name"></asp:BoundColumn>                              
								<asp:BoundColumn DataField="AdSiteMapping" HeaderText="AD Mapping Name"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Enabled">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox runat="server" CssClass="tabletext" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' ID="Checkbox2" NAME="Checkbox1"></asp:CheckBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Group">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox runat="server" CssClass="tabletext" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.SiteGroup") %>' ID="Checkbox1" NAME="Checkbox1"></asp:CheckBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</FMControls:FMDataGridFixed></td>
				</tr>
				<tr>
					<td style="width: 642px; height: 39px" valign="middle" width="642">
						<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" TabIndex="2" CssClass="formfieldtitle"></FMControls:FMButton></td>
				</tr>
			</table>
		</div>
</form>
		<script language="jscript">
			var AddButton=document.getElementById("AddButton2");
			if(!AddButton.disabled)
				AddButton.focus();
		</script>
	</body>
</HTML>
