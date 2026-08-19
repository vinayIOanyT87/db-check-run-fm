<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LedgerViewsForm.aspx.cs" Inherits="FuelsManager.Accounting.LedgerViewsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
        <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
        <table style="z-index:110; left:32px; top: 10px; width:575px; position:absolute" cellpadding="5" role="presentation" aria-label="layout">
            <tr>
                <td colspan="2">
                    <FMControls:FMLabel id="TitleLabel" runat="server" CssClass="headline" Text="Ledger Views" style="left:-24px; position:relative" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMButton id="AddButton" runat="server" Text="Add" style="width:75px" CssClass="formfieldtitle" />
                </td>
                <td align="right">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <FMControls:FMGridView ID="LedgerViewGrid" runat="server" FixedHeaders="true" Width="800px" AllowPaging="false" Height="550px" ShowFooter="true" aria-label="Ledger View" RowHeaderColumn="Name">
                        <Columns>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="IdentityGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Edit">
                                <HeaderStyle Width="45px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <FMControls:FMEditLinkButton OnCommand="LedgerViewGridRowCommand" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name">
                                <HeaderStyle Width="190px" />
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="ColumnNameLabel"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Products">
                                <HeaderStyle Width="190px" />
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductList") %>' ID="ColumnProductsLabel"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="User Groups">
                                <HeaderStyle Width="190px" />
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.UserGroupList") %>' ID="ColumnUserGroupsLabel"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <HeaderStyle Width="45px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton runat="server" CommandName="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
		            </FMControls:FMGridView>
                </td>
            </tr>
            <tr>
                <td style="width: 75px">
                    <FMControls:FMButton id="AddButton2" runat="server" Text="Add" style="width:75px" CssClass="formfieldtitle" />
                </td>
                <td style="width: 500px">
                    <FMControls:FMButton id="CreateDefaultLedgerViewButton" runat="server" Text="Create Default View" style="width:125px" CssClass="formfieldtitle" />
                </td>
            </tr>
            <tr>
            </tr>
        </table>
    </div>
</form>
</body>
</html>
