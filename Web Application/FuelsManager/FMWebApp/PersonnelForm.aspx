<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="PersonnelForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonnelForm" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="C#"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<style>
        #grid_scroll_div {
            max-height: calc(100vh - 400px) !important;
		    overflow: auto;
        }
	</style>    
    <script>
        var theMoment = new Date();
        var theDisplacement = (theMoment.getTimezoneOffset() / 60);
        document.cookie = "Displacement=" + theDisplacement;
    </script>
    <form id="ReferenceForm" method="post" enctype="multipart/form-data" runat="server" defaultbutton="FindBtn">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <div>
                <!-- Top area -->
                <asp:ScriptManager ID="oScriptManager" runat="server" />
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label2" Style="z-index: 104; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="256px" BackColor="Transparent">Personnel Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label" AssociatedControlID="PersonRoleDropDownList" Style="z-index: 105; left: 32px; position: absolute; top: 107px" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Role:</FMControls:FMLabel>
                <FMControls:FMLabel ID="FindLabel" AssociatedControlID="FindTextBox" Style="z-index: 110; left: 32px; position: absolute; top: 48px"
                    runat="server" CssClass="formfieldtitle" BackColor="Transparent">Find String:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="PersonRoleDropDownList" Style="z-index: 106; left: 32px; position: absolute; top: 123px" Height="20"
                    runat="server" CssClass="formfield" Width="112px" AutoPostBack="True" TabIndex="1">
                </FMControls:FMDropDownList>
                <asp:TextBox ID="FindTextBox" Style="z-index: 107; left: 32px; position: absolute; top: 64px"
                    runat="server" Width="308px" TabIndex="2" MaxLength="100"></asp:TextBox>
                <FMControls:FMButton ID="FindBtn" Style="z-index: 108; left: 434px; position: absolute; top: 58px" runat="server"
                    CssClass="formfieldtitle" Width="64px" Text="Find" TabIndex="3"></FMControls:FMButton>

                <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 109; left: 518px; position: absolute; top: 58px"
                    runat="server" CssClass="formfieldtitle" Width="64px" Text="Show All" TabIndex="4"></FMControls:FMButton>
                <table id="Table1" style="z-index: 101; left: 32px; width: 60%; position: absolute; top: 200px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td width="650" height="36" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                                TabIndex="6" />
        					&nbsp;&nbsp;
		    				<FMControls:FMPageSizeDropDown ID="StationsFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" alt="Page size" />

                            <FMControls:FMLabel Width="500px" ID="lblWarning" runat="server"
                                CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 650px; height: 12px;">
                            <div id="grid_scroll_div">
                            <FMControls:FMGridView ID="PersonnelDataGrid" runat="server" DataKeyNames="SiteGuid,IdentityGuid" RowHeaderColumn="Personnel ID"
                                BorderStyle="None" 
                                BackColor="White" AutoGenerateColumns="False"
                                GridLines="Vertical" Width="1200px" BorderWidth="3px" AllowSorting="True"
                                BorderColor="White" CellPadding="2" CellSpacing="1" AllowPaging="True" PageSize="10" CssClass="tabletext"
                                Style="left: 1px; top: 0px;" TabIndex="5" ShowFooter="true"
                                FixedHeaders="True" HeaderStyle="position: absolute !important">
                                <PagerSettings Mode="Numeric" />
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <EditRowStyle BackColor="White" BorderStyle="Solid" />
                                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" Height="20px"></SelectedRowStyle>
                                <AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext" Height="20px"></AlternatingRowStyle>
                                <Columns>
                                    <asp:TemplateField HeaderText="Assign">
                                        <HeaderStyle Width="0.5in" />
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <FMControls:FMSelectLinkButton ID="AssignButton" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit">
                                        <HeaderStyle Width="46px" />
                                        <ItemStyle Width="48px" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Bottom" />
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton ID="EditButton" runat="server" OnCommand="PersonnelDataGridRowCommand" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Literal ID="SiteGuidText" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Literal ID="EntityGuidText" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Personnel ID" Visible="true" DataField="PersonID" SortExpression="PersonID"></asp:BoundField>
                                    <asp:BoundField HeaderText="First" Visible="true" DataField="FirstName" SortExpression="FirstName"></asp:BoundField>
                                    <asp:BoundField HeaderText="Middle" Visible="true" DataField="MiddleName" SortExpression="MiddleName"></asp:BoundField>
                                    <asp:BoundField HeaderText="Last" Visible="true" DataField="LastName" SortExpression="LastName"></asp:BoundField>
                                    <asp:BoundField HeaderText="Short Card Number" Visible="true" DataField="ShortCardNumber" SortExpression="ShortCardNumber"></asp:BoundField>

                                    <asp:TemplateField HeaderText="Locked Out">
                                        <HeaderStyle Width="70px" />
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <FMControls:FMCheckBox ID="GlobalLockedOut" runat="server" Enabled="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Enterprise Only" SortExpression="Remote">
                                        <HeaderStyle Width="80px" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMCheckBox ID="RemoteCheckBox" runat="server" Enabled="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Literal ID="MasterRecordGuidText" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MasterRecordGuid").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Literal ID="RemoteText" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Remote").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="0.5in" HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" Width="48px"></ItemStyle>
                                    </asp:TemplateField>
                                </Columns>
                            </FMControls:FMGridView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 163px; height: 36px; padding-top: 15px" valign="middle" width="163">
                            <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
                                TabIndex="6"></FMControls:FMButton></td>
                    </tr>
                </table>
                <FMControls:FMButton ID="SearchEnterpriseBtn" Style="z-index: 108; left: 350px; position: absolute; top: 58px"
                    TabIndex="3" runat="server" Width="64px" CssClass="formfieldtitle" Text="Search" OnClick="SearchEnterpriseBtnOnClick"></FMControls:FMButton>

                <FMControls:FMCheckBox ID="ShowHiddenCheckBox" Style="z-index: 110; left: 499px; position: absolute; top: 160px" TabIndex="5"
                    CssClass="formfieldtitle" runat="server" Text="Show Hidden" AutoPostBack="True" OnCheckedChanged="ShowHiddenCheckBox_OnCheckedChanged"></FMControls:FMCheckBox>

                <FMControls:FMLabel ID="PersonnelIDLabel" AssociatedControlID="PersonnelIDSearchBox" Style="z-index: 106; left: 149px; position: absolute; top: 107px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Visible="false">Personnel ID:</FMControls:FMLabel>
                <asp:TextBox ID="PersonnelIDSearchBox" Style="z-index: 107; left: 149px; position: absolute; top: 123px; height:20px; padding:0px 2px"
                    runat="server" Width="105px" TabIndex="2" MaxLength="100" Visible="false"></asp:TextBox>

                <FMControls:FMLabel ID="FirstLabel" AssociatedControlID="FirstSearchBox" Style="z-index: 106; left: 267.5px; position: absolute; top: 107px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Visible="false">First:</FMControls:FMLabel>
                <asp:TextBox ID="FirstSearchBox" Style="z-index: 107; left: 267.5px; position: absolute; top: 123px; height:20px; padding:0px 2px"
                    runat="server" Width="105px" TabIndex="2" MaxLength="100" Visible="false"></asp:TextBox>

                <FMControls:FMLabel ID="LastLabel" AssociatedControlID="LastSearchBox" Style="z-index: 106; left: 385.5px; position: absolute; top: 107px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Visible="false">Last:</FMControls:FMLabel>
                <asp:TextBox ID="LastSearchBox" Style="z-index: 107; left: 385.5px; position: absolute; top: 123px; height:20px; padding:0px 2px"
                    runat="server" Width="105px" TabIndex="2" MaxLength="100" Visible="false"></asp:TextBox>

                <FMControls:FMLabel ID="ShortCardLabel" AssociatedControlID="ShortCardSearchBox" Style="z-index: 106; left: 504px; position: absolute; top: 107px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Visible="false">Short Card:</FMControls:FMLabel>
                <asp:TextBox ID="ShortCardSearchBox" Style="z-index: 107; left: 504px; position: absolute; top: 123px; height:20px; padding:0px 2px"
                    runat="server" Width="105px" TabIndex="2" MaxLength="100" Visible="false"></asp:TextBox>




            </div>
        </div>
    </form>
    <script language="jscript">
        var findBtn = document.getElementById("FindBtn");
        var findTbBtn = document.getElementById("FindTextBox");

        if (findBtn != null && findTbBtn != null) {
            try {
                findBtn.setActive();
                findTbBtn.focus();
            }
            catch (err) { }
        }
    </script>

</body>
</HTML>
