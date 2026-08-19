<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="AcculoadsForm.aspx.cs" AutoEventWireup="true" Inherits="AcculoadOPCWebApp.AcculoadsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
  </HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="304px" BackColor="Transparent">SmithMeter|Presets Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 32px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">SmithMeter|System:</FMControls:FMLabel>
			<FMControls:FMDropDownList ID="SelectSystemModeDropDownList" Style="z-index: 106; left: 168px; position: absolute; top: 40px"
				TabIndex="3" runat="server" CssClass="formfield" Width="58px" AutoPostBack="True" Height="24px"></FMControls:FMDropDownList>
			<asp:TextBox ID="SystemTextBox" Style="z-index: 107; left: 240px; position: absolute; top: 40px" TabIndex="27" runat="server" CssClass="formfield" Width="152px" AutoPostBack="True" MaxLength="80"></asp:TextBox>
			<asp:DropDownList ID="SystemDropDownList" Style="z-index: 105; left: 240px; position: absolute; top: 40px"
				runat="server" CssClass="formfield" Width="144px" AutoPostBack="True">
			</asp:DropDownList>

			<table id="Table1" style="z-index: 101; left: 32px; width: 43.18%; position: absolute; top: 80px; height: 10px"
				cellspacing="0" cellpadding="1" border="0">
				<tr>
					<td width="350" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="SmithMeter|Add" CssClass="formfieldtitle"
							TabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="AcculoadPresetsFormPageSizeDropDown" StringPrefix="SmithMeter" runat="server"
							TabIndex="7" />
					</td>
				</tr>
				<tr>
					<td style="width: 498px; height: 10px" width="498">
						<FMControls:FMDataGrid ID="AcculoadsDataGrid" Style="left: 1px; top: 0px" runat="server" PageSize="12"
							CssClass="tabletext" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="400px" GridLines="Vertical"
							AutoGenerateColumns="False" BackColor="White" BorderStyle="None">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>

							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>

							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>

							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>

							<Columns>
								<asp:TemplateColumn HeaderText="SmithMeter|Edit">
									<HeaderStyle Width="0.5in"></HeaderStyle>

									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>

									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />

									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index">
									<HeaderStyle Wrap="False"></HeaderStyle>

									<ItemStyle Wrap="False"></ItemStyle>

									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="SmithMeter|ID">
									<HeaderStyle Wrap="False"></HeaderStyle>

									<ItemStyle Wrap="False"></ItemStyle>

									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Type" HeaderText="SmithMeter|Type">
									<HeaderStyle Wrap="False"></HeaderStyle>

									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="SmithMeter|Network">
									<HeaderStyle Wrap="False"></HeaderStyle>

									<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>

									<ItemTemplate>
										<asp:CheckBox runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem.NetworkCommunications") %>' ID="NetworkCommunictionsCheckBox"></asp:CheckBox>

									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="Port" HeaderText="SmithMeter|Port">
									<HeaderStyle Wrap="False"></HeaderStyle>

									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="IPAddress" HeaderText="SmithMeter|IP Address">
									<HeaderStyle Wrap="False"></HeaderStyle>

									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="SmithMeter|Delete">
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
					<td style="width: 498px; height: 36px" valign="middle" width="498">
						<FMControls:FMButton ID="AddButton" runat="server" CssClass="formfieldtitle" Width="98px" Text="SmithMeter|Add"></FMControls:FMButton>
					</td>
				</tr>
			</table>
		</div>
	</form>
			<script language="jscript">
				var SystemTextBox=document.getElementById("SystemTextBox");
				if(SystemTextBox != null)
					SystemTextBox.focus();
			</script>
	</body>
</HTML>
