<%@ Page language="c#" Codebehind="UsersForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.UsersForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body ms_positioning="GridLayout" tabindex="-1">
		<form id="UsersForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
		   <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
		   <FMControls:FMLabel id="Label2" style="Z-INDEX: 104; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="240px" BackColor="Transparent">Users Configuration</FMControls:FMLabel>
		   	<FMControls:FMLabel id="FindLabel" AssociatedControlID="FindTextBox" style="Z-INDEX: 109; LEFT: 40px; POSITION: absolute; TOP: 36px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">Find String:</FMControls:FMLabel>
		   <asp:TextBox id="FindTextBox" style="Z-INDEX: 106; LEFT: 112px; POSITION: absolute; TOP: 33px"
				runat="server" CssClass="formfield" tabIndex="1" MaxLength="100"></asp:TextBox>
			<FMControls:FMButton id="FindBtn" style="z-index: 107; left: 256px; position: absolute; top: 32px" runat="server"
				CssClass="formfieldtitle" Width="64px" Text="Find" tabIndex="2" onclick="FindBtnOnClick"></FMControls:FMButton>
			<FMControls:FMButton id="ShowAllBtn" style="Z-INDEX: 108; LEFT: 336px; POSITION: absolute; TOP: 32px"
				runat="server" CssClass="formfieldtitle" Width="64px" Text="Show All" tabIndex="3" onclick="FindAllBtnOnClick"></FMControls:FMButton>
			

			<TABLE id="Table1" style="z-index: 101; left: 32px; width: 43.18%; position: absolute; top: 64px; height: 10px"
				cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<TD width="539" height="36" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							tabIndex="4" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 10px" width="498">
					<FMControls:FMDataGridFixed id="UsersDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="User ID"
							GridLines="Vertical" 
                            Width="800px" 
                            BorderWidth="1px" 
                            AllowSorting="True" 
                            BorderColor="White" 
                            CellPadding="3" 
                            AllowPaging="True" 
                            CssClass="tabletext"
							style="LEFT: 1px; TOP: 0px" 
                            FixedHeight="470px" 
                            Height="470px"
                            PageSize="16" 
                            tabIndex="5">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
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
								<asp:BoundColumn DataField="ID" HeaderText="User ID">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<FooterStyle Wrap="False"></FooterStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Name" HeaderText="Name"></asp:BoundColumn>
								<asp:BoundColumn DataField="ActiveDirectoryUserStr" HeaderText="Managed By Active Directory"></asp:BoundColumn>
								<asp:BoundColumn DataField="EmailAddress" HeaderText="Email Address">
								    <ItemStyle Width="300px" />
								</asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="ActiveDirectoryUser" HeaderText="ActiveDirectoryUser"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</FMControls:FMDataGridFixed></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 50px" vAlign="middle" width="498">
						<FMControls:FMButton id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle" tabIndex="6"></FMControls:FMButton>
					</TD>
				</TR>
			</TABLE>

		</div>
</form>
		<script type="text/javascript">
		    var b = document.getElementById("FindBtn"), c = document.getElementById("FindTextBox"); if (null != b && null != c) try { b.setActive(), c.focus() } catch (d) { } document.addEventListener("keydown", function (a) { 13 == a.keyCode && (a.returnValue = !1, a.cancel = !0, document.all("FindBtn").click()) });
            
            <%--  This is the source that was minimized for the above script...
            var findBtn = document.getElementById("FindBtn");
			var findTbBtn = document.getElementById("FindTextBox");
			
			if (findBtn != null && findTbBtn != null)
			{
			    try
			    {
			        findBtn.setActive();
			        findTbBtn.focus();
			    }
			    catch (err){}
			}
			// Set the Find Button to be activated by the enter key.
			document.addEventListener('keydown', function (ev) {
			    if (ev.keyCode == 13) {
			        ev.returnValue = false;
			        ev.cancel = true;
			        document.all("FindBtn").click();
			    }
			});
            --%>
		</script>
	</body>
</HTML>
