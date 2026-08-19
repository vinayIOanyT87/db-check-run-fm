<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="CompaniesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompaniesForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
      <title></title>
      <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
      <meta name="CODE_LANGUAGE" content="C#" />
      <meta name="vs_defaultClientScript" content="JavaScript" />
      <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
      <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
        <style>
		#grid_scroll_div {
			max-height: calc(100vh - 300px) !important;
			overflow: auto;
            width:680px
		}
	    </style>
        <form id="Form1" method="post" enctype="multipart/form-data" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 32px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="272px" BackColor="Transparent">Companies Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label" AssociatedControlID="CompanyRoleDropDownList" Style="z-index: 104; left: 32px; position: absolute; top: 40px" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Role:</FMControls:FMLabel>
                <FMControls:FMLabel ID="FindLabel" AssociatedControlID="FindTextBox" Style="z-index: 111; left: 224px; position: absolute; top: 40px"
                    runat="server" CssClass="formfieldtitle" BackColor="Transparent">Find String:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="CompanyRoleDropDownList" Style="z-index: 106; left: 32px; position: absolute; top: 64px; right: 967px;"
                    runat="server" CssClass="formfield" Width="136px" AutoPostBack="True" TabIndex="1" OnSelectedIndexChanged="CompanyRoleDropDownListSelectedIndexChanged">
                </FMControls:FMDropDownList>
                <asp:TextBox ID="FindTextBox" Style="z-index: 108; left: 224px; position: absolute; top: 64px" runat="server" CssClass="formfield" Width="300px" TabIndex="2"></asp:TextBox>
                <FMControls:FMButton ID="FindBtn" Style="z-index: 109; left: 536px; position: absolute; top: 58px" runat="server"
                    CssClass="formfieldtitle" Text="Find" Width="64px" TabIndex="3" OnClick="FindBtn_OnClick"></FMControls:FMButton>
                <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 110; left: 616px; position: absolute; top: 58px"
                    runat="server" CssClass="formfieldtitle" Width="64px" Text="Show All" TabIndex="4" OnClick="FindAllBtn_OnClick"></FMControls:FMButton>
                <FMControls:FMCheckBox ID="ShowHiddenCheckBox" Style="z-index: 110; left: 690px; position: absolute; top: 65px"
                    TabIndex="5" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Show Hidden" AutoPostBack="True" OnCheckedChanged="ShowHiddenCheckBox_OnCheckedChanged"></FMControls:FMCheckBox>
                <table id="Table1" style="z-index: 101; left: 32px; width: 600px; position: absolute; top: 96px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td width="350" height="36" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                                TabIndex="6" />
                            <FMControls:FMPageSizeDropDown ID="CompanySummaryPageSizeDropDown" ToolTip="Page Size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                            <FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 500px; height: 10px">
                            <FMControls:FMDataGridFixedPaging ID="CompaniesDataGrid"
                                RowHeaderColumn="Company ID"
                                Style="left: 1px; top: 0px" runat="server"
                                AutoGenerateColumns="False"
                                DataKeyNames="SiteGuid, IdentityGuid"
                                BorderStyle="Solid"
                                BackColor="White"
                                GridLines="Vertical"
                                Width="1200px"
                                BorderWidth="1px"
                                AllowSorting="True"
                                CellPadding="3"
                                AllowPaging="True"
                                CssClass="tabletext"
                                EmptyDataText="No records found"
                                PageSize="12"
                                BorderColor="White"
                                TabIndex="7"
                                ShowHeaderWhenEmpty="True"
                                ShowFooter="False"
                                ShowFooterWhenEmpty="False"
                                FixedHeaders="True"
                                GroupColumnOffset="0"
                                GroupingDepth="0" FixedHeight="410px" Height="410px">
                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

                                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Edit">
                                        <HeaderStyle Width="55px"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="IdentityGuid"
                                        HeaderText="IdentityGuid"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="ID" HeaderText="Company ID" SortExpression="ID"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="Code" HeaderText="Company Code" SortExpression="Code"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="Name" HeaderText="Name" SortExpression="Name"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="Address1" HeaderText="Address" SortExpression="Address1"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="City" HeaderText="City" SortExpression="City"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="State" HeaderText="State" SortExpression="State"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="HiddenDate" Visible="False"></asp:BoundColumn>
                                    <asp:TemplateColumn HeaderText="Delete">
                                        <HeaderStyle Width="0.5in"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMDeleteLinkButton runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
                            </FMControls:FMDataGridFixedPaging></td>
                    </tr>
                    <tr>
                        <td style="width: 350px; height: 36px" valign="middle">
                            <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
                                TabIndex="8"></FMControls:FMButton></td>
                    </tr>
                </table>
            </div>
        </form>
        <script language="jscript">
			var findBtn = document.getElementById("FindBtn");
			var findTbBtn = document.getElementById("FindTextBox");
			
			if (findBtn != null && findTbBtn != null)
			{
			    try
			    {
			        findBtn.setActive();
			        findTbBtn.focus();
			    }
                catch (err) { }
			}

			// Set the Find Button to be activated by the enter key.
			document.addEventListener('keydown', function (ev) {
			    if (ev.keyCode == 13) {
			        ev.returnValue = false;
			        ev.cancel = true;
			        document.all("FindBtn").click();
			    }
			});
        </script>
	</body>
</HTML>
