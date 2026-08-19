<%@ Page language="c#" Codebehind="TanksForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TanksForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
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
<body tabindex="-1" ms_positioning="GridLayout">
	<form id="TanksForm" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 104; left: 8px; position: absolute; top: 8px" runat="server"
				BackColor="Transparent" Width="216px" CssClass="headline">Tanks Configuration</FMControls:FMLabel>
			<table id="Table1" style="z-index: 102; left: 32px; width: 0%; position: absolute; top: 48px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td width="539" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="TanksFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" alt="Page Size" />
                       <FMControls:FMCheckBox id="ShowHiddenCheckBox" tabIndex="3" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Show Hidden" AutoPostBack="True" OnCheckedChanged="ShowHiddenCheckBox_OnCheckedChanged"></FMControls:FMCheckBox>  
					</TD>                       
				</tr>
				<tr>
					<td style="width: 539px">
						<FMControls:FMDataGrid ID="TanksDataGrid" TabIndex="1" runat="server" BorderStyle="None" BackColor="White" RowHeaderColumn="ID"
							AutoGenerateColumns="False" GridLines="Vertical" Width="552px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3"
							AllowPaging="True" CssClass="tabletext" PageSize="16" aria-label="Tanks">
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
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="ID"></asp:BoundColumn>
								<asp:BoundColumn DataField="ProductID" HeaderText="Product"></asp:BoundColumn>
								<asp:BoundColumn DataField="ManagerID" HeaderText="Manager"></asp:BoundColumn>
                                <asp:BoundColumn DataField="OwnerID" HeaderText="Owner"></asp:BoundColumn>								
                                <asp:BoundColumn DataField="HiddenDate" Visible="False"></asp:BoundColumn>
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
					<td style="width: 539px">
						<table style="width: 550px; height: 28px" role="presentation" aria-label="layout">
							<tr>
								<td style="width: 428px">
									<FMControls:FMButton ID="AddButton" TabIndex="2" runat="server" Width="96px" CssClass="formfieldtitle"
										Text="Add"></FMControls:FMButton></td>
								<td style="width: 88px">
								<td style="width: 80px">
								<td style="width: 163px">
								<td>
									<FMControls:FMButton ID="AutoCreateButton" TabIndex="5" runat="server" Width="100px" CssClass="formfieldtitle"
										Text="Auto Create"></FMControls:FMButton></td>
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
	</script>
</body>
</html>
