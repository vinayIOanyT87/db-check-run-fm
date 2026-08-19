<%@ Page language="c#" Codebehind="PortsForm.aspx.cs" AutoEventWireup="True" Inherits="WeightScaleOPCWebApp.PortsForm" %>
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
			    <TABLE id="Table1" style="Z-INDEX: 102; LEFT: 32px; WIDTH: 43.18%; POSITION: absolute; TOP: 80px; HEIGHT: 10px"
				    cellSpacing="0" cellPadding="1" border="0">
				    <tr>
					    <TD vAlign="middle" width="498" height="36"><FMCONTROLS:FMBUTTON id="AddButton2" tabIndex="6" runat="server" CssClass="formfieldtitle" Text="WeightScale|Add"
							width="100px"></FMCONTROLS:FMBUTTON>&nbsp;&nbsp;
						<FMCONTROLS:FMPAGESIZEDROPDOWN id="WeightScalePortsFormPageSizeDropDown" runat="server" StringPrefix="WeightScale"></FMCONTROLS:FMPAGESIZEDROPDOWN></TD>
				    </tr>
				    <TR>
					    <TD style="WIDTH: 498px; HEIGHT: 10px" width="498"><FMCONTROLS:FMDATAGRID id="PortsDataGrid" style="LEFT: 1px; TOP: 0px" runat="server" CssClass="tabletext"
							PageSize="12" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="344px" GridLines="Vertical" AutoGenerateColumns="False"
							BackColor="White" BorderStyle="None">
							<FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="WeightScale|Edit">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="WeightScale|Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="WeightScale|ID">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				    </TR>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 50px" vAlign="middle" width="498"><FMCONTROLS:FMBUTTON id="AddButton" runat="server" CssClass="formfieldtitle" Text="WeightScale|Add" Width="98px"></FMCONTROLS:FMBUTTON></TD>
				</TR>
			</TABLE>
			<asp:textbox id="SystemTextBox" style="Z-INDEX: 107; LEFT: 200px; POSITION: absolute; TOP: 40px"
				tabIndex="27" runat="server" CssClass="formfield" Width="152px" AutoPostBack="True" MaxLength="80" ontextchanged="SystemTextBox_TextChanged"></asp:textbox><FMCONTROLS:FMDROPDOWNLIST id="SelectSystemModeDropDownList" style="Z-INDEX: 106; LEFT: 128px; POSITION: absolute; TOP: 40px"
				tabIndex="3" runat="server" CssClass="formfield" Width="58px" AutoPostBack="True" Height="24px" onselectedindexchanged="SelectSystemModeDropDownList_SelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST><FMCONTROLS:FMLABEL id="Label3" style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">WeightScale|System:</FMCONTROLS:FMLABEL><asp:image id="Image1" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="..\FMWebApp\images\fade.jpg"></asp:image><FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="336px" BackColor="Transparent">WeightScale|Ports Configuration</FMCONTROLS:FMLABEL>
			<asp:DropDownList id="SystemDropDownList" style="Z-INDEX: 105; LEFT: 208px; POSITION: absolute; TOP: 40px"
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
