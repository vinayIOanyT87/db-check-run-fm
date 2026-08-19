<%@ Page Language="c#" CodeBehind="DOTHazardousMessagesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.DOTHazardousMessagesForm" %>

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
<body ms_positioning="GridLayout" role="application">
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                CssClass="headline" Width="416px" BackColor="Transparent">DOT Hazardous Messages Configuration</FMControls:FMLabel>
            <table id="Table1" style="z-index: 100; left: 32px; width: 43.18%; position: absolute; top: 48px; height: 10px"
                cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td width="498" height="36" valign="middle">
                        <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
                        &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="DOTMessagesFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 498px; height: 10px">
                        <FMControls:FMDataGrid ID="ApplicationStringsDataGrid" Style="left: 1px; top: 0px;" runat="server" CssClass="tabletext" RowHeaderColumn="DOT Hazardous Message"
                            AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="600px" GridLines="Vertical" AutoGenerateColumns="False"
                            BackColor="White" BorderStyle="None" PageSize="16">
                            <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>

                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>

                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>

                            <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>

                            <Columns>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Width="70px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="70px"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" Width="70px" />

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
                                <asp:TemplateColumn HeaderText="DOT Hazardous Message">
                                    <HeaderStyle Width="499px"></HeaderStyle>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>'>
                                        </asp:Label>
                                    </ItemTemplate>

                                    <EditItemTemplate>
                                        <asp:TextBox Width="480px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' CssClass="tabletext" ID="StringTextBox" ToolTip="DOT Hazardous Message" MaxLength="120" aria-required="true">
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
                    <td style="width: 498px; height: 35px" valign="middle">
                        <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
