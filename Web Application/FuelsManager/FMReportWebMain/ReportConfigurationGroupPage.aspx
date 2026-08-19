<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="ReportConfigurationGroupPage.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMReportWebMain.ReportConfigurationGroupPage" %>
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
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<asp:label id="GroupLabel" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 16px"
				runat="server" CssClass="headline" Width="744px">Report Group Configuration</asp:label>
			<TABLE id="AssignmentTable" style="Z-INDEX: 102; LEFT: 16px; WIDTH: 646px; POSITION: absolute; TOP: 56px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<TD style="WIDTH: 646px; HEIGHT: 36px" vAlign="middle">
						<FMControls:FMButton id="AddGroupButton2" runat="server" style="min-width: 100px" CssClass="formfieldtitle" Text="Add" onclick="AddGroupButtonOnClick" />
						&nbsp;&nbsp;
						<FMControls:FMButton id="CloseButton2" runat="server" style="min-width: 100px" CssClass="formfieldtitle" Text="Close" onclick="CloseButtonOnClick" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="ReportGroupsFormPageSizeDropDown2" ToolTip="Page size" runat="server" tabIndex="7" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 646px; HEIGHT: 10px">
                        <FMCONTROLS:FMBaseDataGrid id="GroupDataGrid" runat="server" BackColor="White" Width="646px" CssClass="tabletext"
							AllowPaging="True" AllowSorting="True" BorderColor="White" BorderWidth="1px" CellPadding="3" Height="10px" GridLines="Vertical" PageSize="5"
							AutoGenerateColumns="False" onselectedindexchanged="GridMoveCommand" aria-label="Group Grid">
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
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton runat="server" />&nbsp;
										<FMControls:FMCancelLinkButton runat="server" />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="GroupGuid">
									<ItemTemplate>
										<asp:Label id=IdentityGuidLabel runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.GroupGuid") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Groups">
									<ItemTemplate>
										<asp:Label id=GroupNameLabel runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.GroupName") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=GroupNameTextBox ToolTip="Groups" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.GroupName") %>' MaxLength="30" CssClass="tabletext">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:ButtonColumn Text="&lt;img src=../FMWebApp/images/Up.gif border=0 align=absmiddle alt='Move this item'&gt;"
									HeaderText="Order" CommandName="Select">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								</asp:ButtonColumn>
                                <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMBaseDataGrid></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 646px; HEIGHT: 49px" vAlign="middle" width="646">
						<FMControls:FMButton id="AddGroupButton" runat="server" style="min-width: 100px" CssClass="formfieldtitle" Text="Add" onclick="AddGroupButtonOnClick" />
						&nbsp;&nbsp;
						<FMControls:FMButton id="CloseButton" runat="server" style="min-width: 100px" CssClass="formfieldtitle" Text="Close" onclick="CloseButtonOnClick" />
					</TD>
				</TR>
			</TABLE>

		</div>
</form>
		<script type="text/javascript">
			var AddGroupButton=document.getElementById("AddGroupButton");
			if(!AddGroupButton.disabled) {
				document.getElementById("AddGroupButton").setActive();
				document.getElementById("AddGroupButton").focus();
			}
		</script>
	</body>
</HTML>
