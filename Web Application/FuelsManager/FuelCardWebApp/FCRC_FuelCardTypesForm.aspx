<%@ Page Language="c#" CodeBehind="FCRC_FuelCardTypesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FuelCardWebApp.FCRC_FuelCardTypesForm" %>
<%@ Register TagPrefix="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
        <title></title>
        <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
        <meta name="CODE_LANGUAGE" content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body ms_positioning="GridLayout" tabindex="-1">
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="368px" BackColor="Transparent">Fuel Card Types Configuration</FMControls:FMLabel>
                <table id="Table1" style="z-index: 100; left: 32px; width: 498px; position: absolute; top: 48px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td>
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                                TabIndex="6" />
                            &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="FuelCardTypesFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 498px; height: 10px">
                            <FMControls:FMDataGrid ID="ApplicationStringsDataGrid" Style="left: 1px; top: 0px" runat="server" CssClass="tabletext" RowHeaderColumn="Fuel Card Type ID"
                                AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="340px" GridLines="Vertical" AutoGenerateColumns="False"
                                BackColor="White" BorderStyle="None" PageSize="16">
                                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></SelectedItemStyle>
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Edit">
                                        <HeaderStyle Width="55px"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton runat="server" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <FMControls:FMUpdateLinkButton runat="server" />
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
                                    <asp:TemplateColumn HeaderText="Fuel Card Type ID">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' ID="Label1">
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Width="2in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>' CssClass="tabletext" ID="StringTextBox" ToolTip="Fuel card type ID" MaxLength="30">
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
                                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                            </FMControls:FMDataGrid></td>
                    </tr>
                    <tr>
                        <td style="width: 498px; height: 35px" valign="middle" width="498">
                            <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
                    </tr>
                </table>
            </div>
        </form>
	</body>
</HTML>
