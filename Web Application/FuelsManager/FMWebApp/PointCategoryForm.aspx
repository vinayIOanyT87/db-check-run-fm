<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PointCategoryForm.aspx.cs" Inherits="FuelsManager.FMWebApp.PointCategoryForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>

<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout" tabindex="-1">
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <table id="Table1" style="Z-INDEX: 100; LEFT: 32px; WIDTH: 43.18%; POSITION: absolute; TOP: 48px; HEIGHT: 10px"
                cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td width="498" height="36" valign="middle">
                        <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                            TabIndex="6" />
                        &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="PointCategoryPageSizeDropDown" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td style="WIDTH: 498px; HEIGHT: 10px">
                        <FMControls:FMDataGrid ID="ApplicationStringsDataGrid" Style="LEFT: 1px; TOP: 0px;" runat="server" CssClass="tabletext"
                            AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="600px" GridLines="Vertical" AutoGenerateColumns="False"
                            BackColor="White" BorderStyle="None" PageSize="16">
                            <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>

                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>

                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>

                            <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>

                            <Columns>
                                <asp:TemplateColumn HeaderText="Edit" >
                                    <HeaderStyle Width="70px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="70px"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" Width="70px"/>

                                    </ItemTemplate>

                                    <EditItemTemplate>
                                        <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                        <FMControls:FMCancelLinkButton runat="server" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>' ID="SiteGuidLabel">
                                        </asp:Label>

                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>' ID="IdentityGuidLabel">
                                        </asp:Label>

                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Point Category">
                                    <HeaderStyle Width="499px"></HeaderStyle>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' >
                                        </asp:Label>
                                    </ItemTemplate>

                                    <EditItemTemplate>
                                        <asp:TextBox Width="480px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' CssClass="tabletext" ID="StringTextBox" MaxLength="120">
                                        </asp:TextBox>

                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Delete">
                                    <HeaderStyle Width="30px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="30px"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMDeleteLinkButton runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>

                            <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMDataGrid></td>
                </tr>
                <tr>
                    <td style="WIDTH: 498px; HEIGHT: 35px" valign="middle" width="498">
                        <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
                </tr>
            </table>
            <asp:Image ID="Image1" Style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="Label2" Style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
                CssClass="headline" Width="416px" BackColor="Transparent">Point Categories Configuration</FMControls:FMLabel>
        </div>
    </form>
</body>
</html>
