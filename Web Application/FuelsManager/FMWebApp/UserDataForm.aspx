<%@ Page Language="c#" AutoEventWireup="True" CodeBehind="UserDataForm.aspx.cs" Inherits="FuelsManager.FMWebApp.UserDataForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
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
    <form id="Form1" method="post" enctype="multipart/form-data" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="ConfigurationLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px"
                runat="server" CssClass="headline" BackColor="Transparent" Width="718px">User Data Configuration</FMControls:FMLabel>
            <FMControls:FMLabel ID="Label1" AssociatedControlID="EntityTypeDropDownList" Style="z-index: 116; left: 24px; position: absolute; top: 48px" runat="server"
                CssClass="formfieldtitle"> Entity:</FMControls:FMLabel>
            <FMControls:FMDropDownList ID="EntityTypeDropDownList" Style="z-index: 108; left: 104px; position: absolute; top: 48px"
                TabIndex="1" runat="server" CssClass="formfield" Width="112px" AutoPostBack="True" OnSelectedIndexChanged="EntityTypeDropDownList_SelectedIndexChanged">
            </FMControls:FMDropDownList>
            <table style="z-index: 100; left: 24px; width: 38.42%; position: absolute; top: 80px; height: 10px">
                <tr>
                    <td style="width: 647px">
                        <FMControls:FMDataGrid ID="UserDataFieldDataGrid" runat="server" CssClass="tabletext" Width="696px" CellPadding="3"
                            BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False" PageSize="8"
                            BackColor="White" BorderStyle="None" TabIndex="2" Style="left: 24px; top: 24px">
                            <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Width=".5 in"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMUpdateLinkButton runat="server" />&nbsp;<FMControls:FMCancelLinkButton runat="server" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="SiteGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="IdentityGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Number">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="NumberLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Number") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Display Name">
                                    <HeaderStyle Width="2in"></HeaderStyle>
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="DisplayNameLabel" runat="server" Width="1.75in" Text='<%# DataBinder.Eval(Container, "DataItem.DisplayName") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="DisplayNameTextBox" runat="server" Width="1.75in" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ValueList") %>' MaxLength="30" Columns="30">
                                        </asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Type">
                                    <HeaderStyle Width="1in"></HeaderStyle>
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="TypeLabel" Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Type") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMDropDownList Width="1in" runat="server" CssClass="tabletext" Enabled="True" ID="TypeDropDownList" DataSource="<%# EnumerateUserDataTypes()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownList_SelectedIndexChanged">
                                        </FMControls:FMDropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Value List">
                                    <HeaderStyle Width="1.75in"></HeaderStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="ValueListLabel" Width="3.0in" runat="server" Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" Text='<%# DataBinder.Eval(Container, "DataItem.ValueList") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="ValueListTextBox" TextMode="MultiLine" Width="3.0in" runat="server" CssClass="tabletext" MaxLength="100"></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Required">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMCheckBox runat="server" CssClass="tabletext" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.Required") %>' ID="RequiredCheckBox" ToolTip="Required"></FMControls:FMCheckBox>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMCheckBox runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.Required") %>' ID="RequiredCheckBox" ToolTip="Required"></FMControls:FMCheckBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="false" HeaderText="OriginalRequired">
                                    <ItemTemplate>
                                        <FMControls:FMCheckBox runat="server" CssClass="tabletext" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.OriginalRequired") %>' ID="OriginalCheckBox" ToolTip="Original required"></FMControls:FMCheckBox>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMDataGrid>
                    </td>
                </tr>
            </table>
            <script>
                document.getElementById("EntityTypeDropDownList").focus();
            </script>
        </div>
    </form>
</body>
</html>
