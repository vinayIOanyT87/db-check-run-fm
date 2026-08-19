<%@ Page language="c#" Codebehind="CardReadersForm.aspx.cs" AutoEventWireup="true" Inherits="OPCWebApp.AcculoadOPCWebApp.CardReadersForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>

			<FMControls:FMLabel ID="Label2" Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="416px" BackColor="Transparent">SmithMeter|Card Readers Configuration</FMControls:FMLabel>

			<FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 32px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">SmithMeter|System:</FMControls:FMLabel>

			<FMControls:FMDropDownList ID="SelectSystemModeDropDownList" Style="z-index: 106; left: 152px; position: absolute; top: 40px"
				TabIndex="3" runat="server" CssClass="formfield" Width="58px" AutoPostBack="True" Height="24px">
			</FMControls:FMDropDownList>

			<asp:TextBox ID="SystemTextBox" Style="z-index: 107; left: 224px; position: absolute; top: 40px"
				TabIndex="27" runat="server" CssClass="formfield" Width="152px" AutoPostBack="True" MaxLength="80"></asp:TextBox>

			<asp:DropDownList ID="SystemDropDownList" Style="z-index: 105; left: 232px; position: absolute; top: 40px"
				runat="server" CssClass="formfield" Width="144px" AutoPostBack="True">
			</asp:DropDownList>
			
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 32px; WIDTH: 43.18%; POSITION: absolute; TOP: 72px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD width="498" height="36" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="SmithMeter|Add" CssClass="formfieldtitle" tabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="CardReadersFormPageSizeDropDown" StringPrefix="SmithMeter" runat="server" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 10px" width="498">
						<FMCONTROLS:FMDatagrid id="CardReadersDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False"
							GridLines="Vertical" Width="400px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							style="LEFT: 1px; TOP: 0px" PageSize="12">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="SmithMeter|Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="Index" HeaderText="SmithMeter|Index">
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
								<HeaderStyle Wrap="False">
								</HeaderStyle>

								<ItemStyle Wrap="False">
								</ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="SmithMeter|Network">
								<HeaderStyle Wrap="False">
								</HeaderStyle>

								<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle">
								</ItemStyle>

								<ItemTemplate>
																		<asp:CheckBox runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem.NetworkCommunications") %>' ID="NetworkCommunictionsCheckBox">
																		</asp:CheckBox>
																	
								</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="Port" HeaderText="SmithMeter|Port">
								<HeaderStyle Wrap="False">
								</HeaderStyle>

								<ItemStyle Wrap="False">
								</ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="IPAddress" HeaderText="SmithMeter|IP Address">
								<HeaderStyle Wrap="False">
								</HeaderStyle>

								<ItemStyle Wrap="False">
								</ItemStyle>
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
						</FMCONTROLS:FMDatagrid></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 39px" vAlign="middle" width="498"><FMCONTROLS:FMButton id="AddButton" runat="server" Width="98px" Text="SmithMeter|Add" CssClass="formfieldtitle"></FMCONTROLS:FMButton></TD>
				</TR>
			</TABLE>

			<script language="jscript">
				var SystemTextBox=document.getElementById("SystemTextBox");
				if(SystemTextBox != null)
					SystemTextBox.focus();
			</script>
		</div>
</form>
	</body>
</HTML>
