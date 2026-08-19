<%@ Page language="c#" Codebehind="DanLoadsForm.aspx.cs" AutoEventWireup="True" Inherits="OPCWebApp.DanielOPCWebApp.DanLoadsForm" %>
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
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="FMLabel2" Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="304px" BackColor="Transparent">Daniel|Presets Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="FMLabel3" Style="z-index: 103; left: 32px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">Daniel|System:</FMControls:FMLabel>
			<FMControls:FMDropDownList ID="SelectSystemModeDropDownList" Style="z-index: 106; left: 144px; position: absolute; top: 40px"
				TabIndex="3" runat="server" CssClass="formfield" Width="58px" AutoPostBack="True" Height="24px" OnSelectedIndexChanged="SelectSystemModeDropDownList_SelectedIndexChanged">
			</FMControls:FMDropDownList>
			<asp:DropDownList ID="SystemDropDownList" Style="z-index: 105; left: 216px; position: absolute; top: 40px"
				runat="server" CssClass="formfield" Width="144px" AutoPostBack="True" OnSelectedIndexChanged="SystemDropDownList_SelectedIndexChanged">
			</asp:DropDownList>
			<asp:TextBox ID="SystemTextBox" Style="z-index: 107; left: 216px; position: absolute; top: 40px"
				TabIndex="27" runat="server" CssClass="formfield" Width="152px" AutoPostBack="True" MaxLength="80" OnTextChanged="SystemTextBox_TextChanged"></asp:TextBox>

			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 32px; WIDTH: 43.18%; POSITION: absolute; TOP: 80px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD width="350" height="36" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Daniel|Add" CssClass="formfieldtitle"
							tabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="DanLoadPresetsFormPageSizeDropDown" StringPrefix="Daniel" runat="server" tabIndex="7" onselectedindexchanged="PageSizeDropDown_SelectedIndexChanged" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 10px" width="498"><FMCONTROLS:FMDATAGRID id="DanLoadsDataGrid" style="LEFT: 1px; TOP: 0px" runat="server" PageSize="12" CssClass="tabletext"
							AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="400px" GridLines="Vertical" AutoGenerateColumns="False"
							BackColor="White" BorderStyle="None">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Daniel|Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
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
								<asp:BoundColumn DataField="ID" HeaderText="Daniel|ID">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Type" HeaderText="Daniel|Type"></asp:BoundColumn>
								<asp:BoundColumn DataField="Port" HeaderText="Daniel|Port"></asp:BoundColumn>
								<asp:BoundColumn DataField="Address" HeaderText="Daniel|Address"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Daniel|Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 36px" vAlign="middle" width="498">
						<FMCONTROLS:FMBUTTON id="AddButton" runat="server" CssClass="formfieldtitle" Width="98px" Text="Daniel|Add"></FMCONTROLS:FMBUTTON>
					</TD>
				</TR>
			</TABLE>
        </div>
		</form>
		<script type="text/javascript">
			var SystemTextBox=document.getElementById("SystemTextBox");
			if(SystemTextBox != null)
				SystemTextBox.focus();
		</script>
	</body>
</HTML>
