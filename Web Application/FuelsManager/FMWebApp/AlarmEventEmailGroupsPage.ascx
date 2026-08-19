<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="AlarmEventEmailGroupsPage.ascx.cs"
    Inherits="FuelsManager.FMWebApp.AlarmEventEmailGroupsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
<head>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <table id="Table1" style="z-index: 100; left: 0px; width: 43.18%; position: absolute;
        top: 20px; height: 10px" cellspacing="0" cellpadding="1" border="0">
        <tr>
            <td width="350" height="36" valign="middle">
                <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                    TabIndex="6" />
                &nbsp;&nbsp;
                <FMControls:FMPageSizeDropDown ID="AlarmEmailPageSizeDropDown" ToolTip="Page size" runat="server" TabIndex="7"
                    OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td style="width: 498px; height: 10px" width="498">
                <FMControls:FMDataGrid ID="EmailGroupsDataGrid" runat="server" BorderStyle="None" RowHeaderColumn="Email Group Name"
                    BackColor="White" AutoGenerateColumns="False" GridLines="Vertical" Width="400px"
                    BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True"
                    CssClass="tabletext" Style="left: 1px; top: 0px" PageSize="8" TabIndex="1">
                    <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
                    </HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn HeaderText="Edit">
                            <HeaderStyle Width="55px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            <ItemTemplate>
                                <FMControls:FMEditLinkButton runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>'
                                    ID="SiteGuidLabel">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid">
                            <HeaderStyle Wrap="False"></HeaderStyle>
                            <ItemStyle Wrap="False"></ItemStyle>
                            <FooterStyle Wrap="False"></FooterStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="ID" HeaderText="Email Group Name">
                            <HeaderStyle Wrap="False"></HeaderStyle>
                            <ItemStyle Wrap="False"></ItemStyle>
                            <FooterStyle Wrap="False"></FooterStyle>
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Enabled">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            <ItemTemplate>
                                <asp:CheckBox runat="server" CssClass="tabletext" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>'
                                    ID="Checkbox1"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Delete">
                            <HeaderStyle Width="0.5in"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            <ItemTemplate>
                                <FMControls:FMDeleteLinkButton runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
                        Mode="NumericPages"></PagerStyle>
                </FMControls:FMDataGrid>
            </td>
        </tr>
        <tr>
            <td style="width: 498px; height: 22px" valign="middle" width="498">
                <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
                    TabIndex="2"></FMControls:FMButton>
            </td>
        </tr>
    </table>
</body>
</html>
