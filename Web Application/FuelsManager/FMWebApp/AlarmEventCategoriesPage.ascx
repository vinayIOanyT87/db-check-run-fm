<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="AlarmEventCategoriesPage.ascx.cs"
    Inherits="FuelsManager.FMWebApp.AlarmEventCategoriesPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
                <FMControls:FMPageSizeDropDown ID="AlarmCatsPageSizeDropDown" ToolTip="Page Size" runat="server" TabIndex="7"
                    OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td style="width: 498px; height: 10px" width="498">
                <FMControls:FMDataGrid ID="CategoriesDataGrid" runat="server" CssClass="tabletext" RowHeaderColumn="Category Name"
                    AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
                    Width="448px" GridLines="Vertical" AutoGenerateColumns="False" BackColor="White"
                    BorderStyle="None" PageSize="8" TabIndex="1">
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
                            <EditItemTemplate>
                                <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                <FMControls:FMCancelLinkButton runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>'
                                    ID="SiteGuidLabel">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="False" HeaderText="Index">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'
                                    ID="IndexLabel">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Category Name">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>'
                                    ID="Label1">
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox Width="3in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>'
                                    CssClass="tabletext" ID="StringTextBox" MaxLength="30">
                                </asp:TextBox>
                            </EditItemTemplate>
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
            <td style="width: 498px; height: 10px" valign="middle" width="498">
                <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
                    TabIndex="2"></FMControls:FMButton>
            </td>
        </tr>
    </table>
</body>
</html>
