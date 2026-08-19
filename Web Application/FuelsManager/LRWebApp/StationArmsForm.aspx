<%@ register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StationArmsForm.aspx.cs" Inherits="LoadRackWebApp.StationArmsForm" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body>
	<form id="form1" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="248px" BackColor="Transparent">Station Arms</FMControls:FMLabel>
			<table id="Table1" style="z-index: 100; left: 32px; width: 43.18%; position: absolute; top: 48px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td width="498" height="36" valign="middle">
						<FMControls:FMPageSizeDropDown ID="StationArmsFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
					</td>
				</tr>
				<tr>
					<td style="width: 498px; height: 10px" width="498">
						<FMControls:FMDataGrid ID="StationArmsDataGrid" runat="server"
							BorderStyle="None" BackColor="White" AutoGenerateColumns="False"
							GridLines="Vertical" Width="328px" BorderWidth="1px" AllowSorting="True" 
							BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							Style="left: 1px; top: 0px" PageSize="8" TabIndex="1" aria-label="Station Arms Grid">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:BoundColumn Visible="False" DataField="StationGuid" HeaderText="StationGuid">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="ArmIndex" HeaderText="ArmIndex">
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
									<HeaderStyle Width=".5in" />
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox Width=".5in" runat="server" CssClass="tabletext" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' ID="EnabledCheckbox" NAME="EnabledCheckbox"></asp:CheckBox>
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
