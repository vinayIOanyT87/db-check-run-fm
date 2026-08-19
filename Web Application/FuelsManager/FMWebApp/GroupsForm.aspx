<%@ Page language="c#" Codebehind="GroupsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.GroupsForm" %>
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
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="GroupsForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
		   <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
		   <FMControls:FMLabel id="Label2" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="312px" BackColor="Transparent">User Groups Configuration</FMControls:FMLabel>
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 32px; WIDTH: 40.49%; POSITION: absolute; TOP: 48px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<TD width="539" height="36" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							tabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="GroupsFormPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDown_SelectedIndexChanged" />
					</TD>
				</tr>
				<TR>
					<TD style="HEIGHT: 10px" width="80%"><FMControls:FMDataGrid id="GroupsDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False"
							GridLines="Vertical" Width="624px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							PageSize="16" tabIndex="2" aria-label="User Groups">
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
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="User Group Name">
									<HeaderStyle Width="2in"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="AdUserMapping" HeaderText="AD Mapping Name"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" HeaderText="Description"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 33px" vAlign="middle" width="80%"><FMControls:FMButton id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
							tabIndex="1"></FMControls:FMButton></TD>
				</TR>
			</TABLE>
			
		</div>
</form>
		<script type="text/javascript">
			var AddButton=document.getElementById("AddButton2");
			if(!AddButton.disabled)
			{
				AddButton.focus();
				AddButton.setActive();
			}
		</script>
	</body>
</HTML>
