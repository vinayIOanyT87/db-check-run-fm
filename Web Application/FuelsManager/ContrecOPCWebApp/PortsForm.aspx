<%@ Page language="c#" Codebehind="PortsForm.aspx.cs" AutoEventWireup="True" Inherits="OPCWebApp.ContrecOPCWebApp.PortsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
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
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="FMLabel2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="336px" BackColor="Transparent">Contrec|Ports Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="FMLabel3" Style="z-index: 104; left: 32px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">Contrec|System:</FMControls:FMLabel>
			<FMControls:FMDropDownList ID="SelectSystemModeDropDownList" Style="z-index: 106; left: 128px; position: absolute; top: 40px"
				TabIndex="3" runat="server" CssClass="formfield" Width="58px" AutoPostBack="True" Height="24px" OnSelectedIndexChanged="SelectSystemModeDropDownList_SelectedIndexChanged">
			</FMControls:FMDropDownList>
			<asp:DropDownList ID="SystemDropDownList" Style="z-index: 105; left: 208px; position: absolute; top: 40px"
				runat="server" Width="144px" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="SystemDropDownList_SelectedIndexChanged">
			</asp:DropDownList>
			<asp:TextBox ID="SystemTextBox" Style="z-index: 107; left: 200px; position: absolute; top: 40px"
				TabIndex="27" runat="server" CssClass="formfield" Width="152px" AutoPostBack="True" MaxLength="80" OnTextChanged="SystemTextBox_TextChanged"></asp:TextBox>

			<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 32px; WIDTH: 43.18%; POSITION: absolute; TOP: 80px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD vAlign="middle" width="498" height="36"><FMCONTROLS:FMBUTTON id="AddButton2" tabIndex="6" runat="server" CssClass="formfieldtitle" Text="Contrec|Add"
							width="100px"></FMCONTROLS:FMBUTTON>&nbsp;&nbsp;
						<FMCONTROLS:FMPAGESIZEDROPDOWN id="ContrecPortsFormPageSizeDropDown" runat="server" StringPrefix="Contrec"></FMCONTROLS:FMPAGESIZEDROPDOWN></TD>
				</tr>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 10px" width="498"><FMCONTROLS:FMDATAGRID id="PortsDataGrid" style="LEFT: 1px; TOP: 0px" runat="server" CssClass="tabletext"
							PageSize="12" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="344px" GridLines="Vertical" AutoGenerateColumns="False"
							BackColor="White" BorderStyle="None">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Contrec|Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton1" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="Contrec|ID">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Contrec|Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 50px" vAlign="middle" width="498"><FMCONTROLS:FMBUTTON id="AddButton" runat="server" CssClass="formfieldtitle" Text="Contrec|Add" Width="98px"></FMCONTROLS:FMBUTTON></TD>
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
