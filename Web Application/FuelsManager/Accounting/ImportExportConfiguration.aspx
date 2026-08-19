<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="ImportExportConfiguration.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.ImportExportConfiguration" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<TABLE id="Table1" style="Z-INDEX: 100; LEFT: 32px; WIDTH: 100%; POSITION: absolute; TOP: 48px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<TR>
					<TD style="WIDTH: 100%; HEIGHT: 10px" width="498">
						<FMControls:FMDatagrid id="DataGrid1" style="LEFT: 1px; TOP: 0px" runat="server" AutoGenerateColumns="False"
							CssClass="tabletext" AllowPaging="True" BackColor="White" BorderWidth="1px" BorderColor="White" CellPadding="3">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<EditItemStyle ForeColor="Black" BackColor="#EEEEEE"></EditItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" CausesValidation="false"></FMControls:FMEditLinkButton>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton runat="server"></FMControls:FMUpdateLinkButton>
										<FMControls:FMCancelLinkButton runat="server"></FMControls:FMCancelLinkButton>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Type">
									<ItemTemplate>
										<asp:Label id=PluginType runat="server" CssClass="formfieldtitle" Text='<%# DataBinder.Eval(Container, "DataItem.PluginType") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:DropDownList id=PluginTypeDropDown CssClass="formfield" SelectedIndex='<%# GetPluginTypeIndex((string) DataBinder.Eval(Container, "DataItem.PluginType")) %>' DataTextField="PluginType" DataSource="<%# this.PluginDO.PluginList %>" Runat="server">
										</asp:DropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Display Name">
									<ItemTemplate>
										<asp:Label id=Label1 runat="server" CssClass="formfieldtitle" Text='<%# DataBinder.Eval(Container, "DataItem.DisplayName") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=DisplayName runat="server" CssClass="formfield" Text='<%# DataBinder.Eval(Container, "DataItem.DisplayName") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Last Export">
									<ItemTemplate>
										<asp:Label id=Label2 runat="server" CssClass="formfieldtitle" Text='<%# DataBinder.Eval(Container, "DataItem.LastExported") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Import">
									<ItemTemplate>
										<asp:Label id=Label3 runat="server" CssClass="formfieldtitle" Text='<%# DataBinder.Eval(Container, "DataItem.ImportAllowed") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Export">
									<ItemTemplate>
										<asp:Label id=Label4 runat="server" CssClass="formfieldtitle" Text='<%# DataBinder.Eval(Container, "DataItem.ExportAllowed") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server"></FMControls:FMDeleteLinkButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
								CssClass="tabletext" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDatagrid>
					</TD>
				</TR>
				<tr>
					<TD style="WIDTH: 498px; HEIGHT: 50px" vAlign="middle" width="498">
						<FMCONTROLS:FMButton id="AddButton" runat="server" Text="Add" onclick="AddButtonClick" />
					</TD>
				</tr>
			</TABLE>
			<asp:Image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image><FMControls:FMLabel id="ConfigurationLabel" style="Z-INDEX: 104; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" CssClass="headline" Width="232px" BackColor="Transparent">Import/Export Configuration</FMControls:FMLabel>
		</div>
</form>
		<script language="jscript">
			var AddButton = document.getElementById("AddButton");
			if ( AddButton.disabled == false )
			{
				AddButton.setActive();
				AddButton.focus();
			}
		</script>
	</body>
</HTML>
