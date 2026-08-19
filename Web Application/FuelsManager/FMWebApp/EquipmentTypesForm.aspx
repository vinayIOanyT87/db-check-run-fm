<%@ Page language="c#" Codebehind="EquipmentTypesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentTypesForm" %>
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
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    </HEAD>
<style type="text/css">
    .GVFixedFooter {
        font-weight: bold;
        position: relative;
        bottom: expression(getScrollBottom(this.parentNode.parentNode.parentNode.parentNode));
    }
    </style>
    <script language="javascript" type="text/javascript">
        function getScrollBottom(p_oElem) {
            return p_oElem.scrollHeight - p_oElem.scrollTop - p_oElem.clientHeight;
        }
    </script>
    <body tabIndex="-1" MS_POSITIONING="GridLayout">
        <form id="Form1" method="post" enctype="multipart/form-data" runat="server" defaultbutton="FindBtn">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="364px" BackColor="Transparent">Equipment Types Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="FindStringLabel" Style="z-index: 106; left: 32px; position: absolute; top: 40px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle">Find String:</FMControls:FMLabel>
                <asp:TextBox ID="FindTextBox" Style="z-index: 107; left: 32px; position: absolute; top: 64px"
                    runat="server" Width="288px" TabIndex="2"></asp:TextBox>
                <FMControls:FMButton ID="FindBtn" Style="z-index: 108; left: 336px; position: absolute; top: 64px"
                    TabIndex="3" runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtnOnClick"></FMControls:FMButton>
                <FMControls:FMButton ID="ShowAllButton" Style="z-index: 109; left: 416px; position: absolute; top: 64px"
                    TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="ShowAllBtnOnClick"></FMControls:FMButton>
                <table id="Table1" style="z-index: 101; left: 32px; width: 50%; position: absolute; top: 96px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td width="350" height="36" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                                TabIndex="6" />
                            <FMControls:FMPageSizeDropDown ID="PageSizeDropDown" ToolTip="Page size" runat="server" TabIndex="7" OnSelectedIndexChanged="PageSizeDropDown_OnSelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 407px; height: 10px" width="407">
                            <FMControls:FMGridView ID="EquipmentTypesDataGrid" runat="server"
                                RowHeaderColumn="Type ID"
                                AutoGenerateColumns="False"
                                Width="900px"
                                DataKeyNames="SiteGuid,EquipmentTypeGuid"
                                AllowSorting="True"
                                ShowHeaderWhenEmpty="True"
                                FixedHeaders="False"
                                ShowFooter="False"
                                AllowPaging="True"
                                BackColor="White" BorderStyle="Solid" BorderWidth="1px"
                                CellPadding="3" CssClass="tabletext" EmptyDataText="No records found"
                                GridLines="Vertical" GroupColumnOffset="0" GroupingDepth="0"
                                ShowFooterWhenEmpty="False">
                                <HeaderStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" CssClass="GVFixedHeader" Font-Bold="True" ForeColor="White" Height="12px"></HeaderStyle>
                                <FooterStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" CssClass="pgr" ForeColor="Black"></FooterStyle>
                                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
                                <AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
                                <Columns>
                                    <asp:TemplateField HeaderText="Edit" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton ID="EditButton" runat="server" OnCommand="EquipmentTypesDataGridRowCommand" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="SiteGuid" Visible="false" DataField="SiteGuid" />
                                    <asp:BoundField HeaderText="EquipmentTypeGuid" Visible="false"
                                        DataField="EquipmentTypeGuid" />
                                    <asp:BoundField HeaderText="Type ID" Visible="true" DataField="EqTypeName" SortExpression="EqTypeName" />
                                    <asp:BoundField HeaderText="Description" Visible="true" DataField="EqTypeDescription" SortExpression="EqTypeDescription" />
                                    <asp:BoundField HeaderText="Issue Point" Visible="true" DataField="Isspt" SortExpression="Isspt" />
                                    <asp:BoundField HeaderText="Capacity" Visible="true" DataField="Capacity" SortExpression="Capacity" />
                                    <asp:BoundField HeaderText="Safe Fill" Visible="true" DataField="SafeFill" SortExpression="SafeFill" />
                                    <asp:BoundField HeaderText="Model" Visible="true" DataField="Model" SortExpression="Model" />
                                    <asp:BoundField HeaderText="Make" Visible="true" DataField="Make" SortExpression="Make" />
                                    <asp:BoundField HeaderText="Year" Visible="true" DataField="Year" SortExpression="Year" />
                                    <asp:BoundField HeaderText="Type" Visible="true" DataField="LookupEquipmentTypeIndex" SortExpression="LookupEquipmentTypeIndex" />
                                    <asp:TemplateField HeaderText="Delete" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>	<!-- moved -->
                                            <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle BackColor="#EEEEEE" CssClass="tabletext" ForeColor="Black" />
                            </FMControls:FMGridView>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 163px; height: 36px" valign="middle" width="163">
                            <FMControls:FMButton ID="AddButton" runat="server" CssClass="formfieldtitle" Width="98px" Text="Add"
                                TabIndex="6"></FMControls:FMButton></td>
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
                catch (err){}
            }
        </script>
    </body>
</HTML>
