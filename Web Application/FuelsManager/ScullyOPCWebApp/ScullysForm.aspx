<%@ Page language="c#" Codebehind="ScullysForm.aspx.cs" AutoEventWireup="True" Inherits="OPCWebApp.ScullyOPCWebApp.ScullysForm" %>
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
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div style="position:absolute">
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 32px; WIDTH: 43.18%; POSITION: absolute; TOP: 80px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD width="498" height="36" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Scully|Add" CssClass="formfieldtitle"
							tabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="ScullysFormPageSizeDropDown" StringPrefix="Scully" runat="server" onselectedindexchanged="PageSizeDropDown_SelectedIndexChanged" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 10px" width="498"><FMCONTROLS:FMDatagrid id="ScullysDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False"
							GridLines="Vertical" Width="400px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							style="LEFT: 1px; TOP: 0px" PageSize="12">
							<FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Scully|Edit">
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
								<asp:BoundColumn DataField="ID" HeaderText="Scully|ID">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Port" HeaderText="Scully|Port"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Scully|Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDatagrid></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 35px" vAlign="middle" width="498"><FMCONTROLS:FMButton id="AddButton" runat="server" Width="98px" Text="Scully|Add" CssClass="formfieldtitle"></FMCONTROLS:FMButton></TD>
				</TR>
			</TABLE>
			<asp:textbox id="SystemTextBox" style="Z-INDEX: 107; LEFT: 240px; POSITION: absolute; TOP: 40px"
				tabIndex="27" runat="server" CssClass="formfield" Width="152px" AutoPostBack="True" MaxLength="80" ontextchanged="SystemTextBox_TextChanged"></asp:textbox>
			<FMCONTROLS:FMDROPDOWNLIST id="SelectSystemModeDropDownList" style="Z-INDEX: 106; LEFT: 168px; POSITION: absolute; TOP: 40px"
				tabIndex="3" runat="server" CssClass="formfield" Width="58px" AutoPostBack="True" Height="24px" onselectedindexchanged="SelectSystemModeDropDownList_SelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST>
			<FMCONTROLS:FMLABEL id="Label3" style="Z-INDEX: 103; LEFT: 32px; POSITION: absolute; TOP: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent" Width="80px">Scully|System:</FMCONTROLS:FMLABEL>
			<asp:Image id="Image1" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="..\FMWebApp\images\fade.jpg"></asp:Image>
			<FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="400px" BackColor="Transparent">Scully|Scully Configuration</FMCONTROLS:FMLABEL>
			<asp:DropDownList id="SystemDropDownList" style="Z-INDEX: 105; LEFT: 240px; POSITION: absolute; TOP: 40px"
				runat="server" Width="144px" CssClass="formfield" AutoPostBack="True" onselectedindexchanged="SystemDropDownList_SelectedIndexChanged"></asp:DropDownList>
			<script language="jscript">
				var SystemTextBox=document.getElementById("SystemTextBox");
				if(SystemTextBox != null)
					SystemTextBox.focus();
			</script>
          </div>
		</form>
	</body>
</HTML>
