<%@ register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ page language="c#" Codebehind="StationsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.StationsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<style>
        #grid_scroll_div {
            max-height: calc(100vh - 250px) !important;
		    overflow: auto;
        }
	</style>
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="248px" BackColor="Transparent">Stations Configuration</FMControls:FMLabel>
			<div Style="z-index: 103; left: 8px; position: absolute; top: 58px">
			<table id="Table1" style="width: 80%;" cellspacing="0" cellpadding="1" border="0" aria-label="layout">
				<tr>
					<td width="698" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="StationsFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" alt="Page size" />
					</td>
				</tr>
				<tr>
					<td style="width: 698px; height: 10px" width="698">
						<div id="grid_scroll_div">
						<FMControls:FMDataGrid ID="StationsDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="ID"
							GridLines="Vertical" Width="560px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							Style="left: 1px; top: 0px" PageSize="8" TabIndex="1" aria-label="Stations">
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
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Enable/Disable">
									<HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
									<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMButton runat="server" CssClass="tabletext" ID="EnableDisableButton" Text='<%# DataBinder.Eval(Container, "DataItem.EnableDisable") %>' Style="width: 80px;" CommandName="EnableDisableButton" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="ID" HeaderText="ID">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Enabled">
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox runat="server" CssClass="tabletext" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' ID="EnabledCheckbox" NAME="EnabledCheckbox"></asp:CheckBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="Type" HeaderText="Type"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Vapor Recovery">
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox runat="server" CssClass="tabletext" Enabled="false" Visible='<%# DataBinder.Eval(Container, "DataItem.LoadRack") %>' Checked='<%# DataBinder.Eval(Container, "DataItem.VaporRecovery") %>'></asp:CheckBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="MeterGuid" HeaderText="MeterGuid">
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
						</FMControls:FMDataGrid></div></td>
				</tr>
				<tr>
					<td style="width: 698px; height: 50px" valign="middle" width="698">
						<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
							TabIndex="2"></FMControls:FMButton></td>
				</tr>
			</table>
			</div>
		</div>
	</form>
	<script type="text/javascript">
		var AddButton = document.getElementById("AddButton2");
		if (!AddButton.disabled)
			AddButton.focus();
	</script>
</body>
</html>
