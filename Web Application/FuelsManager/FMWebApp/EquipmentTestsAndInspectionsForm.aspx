<%@ Page language="c#" Codebehind="EquipmentTestsAndInspectionsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentTestsAndInspectionsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
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
	<body MS_POSITIONING="GridLayout" tabindex="-1">
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="408px" BackColor="Transparent">Equipment Tests & Inspections Configuration</FMControls:FMLabel>
                <table id="Table1" style="z-index: 100; left: 32px; width: 640px; position: absolute; top: 48px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td style="width: 640px; height: 36px" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                                TabIndex="6" />
                            &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="EquipmentTestsFormPageSizeDropDown" runat="server" ToolTip="Page size" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 640px; height: 10px">
                            <FMControls:FMDataGrid ID="QualificationsDataGrid" runat="server" BorderStyle="None" BackColor="White" RowHeaderColumn="ID"
                                AutoGenerateColumns="False" GridLines="Vertical" Width="640px" BorderWidth="1px" AllowSorting="True"
                                BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext" Style="left: 1px; top: 0px"
                                PageSize="16" OnSelectedIndexChanged="QualificationsDataGrid_SelectedIndexChanged">
                                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
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
                                            <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                            <FMControls:FMCancelLinkButton runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
                                    <asp:TemplateColumn Visible="False" HeaderText="Index">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="ID">
                                        <HeaderStyle Width="2in"></HeaderStyle>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' Width="2in">
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="IDTextBox" runat="server" ToolTip="Equipment test ID" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' Width="2in" MaxLength="50" aria-required="true">
                                            </asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Description">
                                        <HeaderStyle Width="3in"></HeaderStyle>
                                        <ItemTemplate>
                                            <asp:Label Width="3in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' ID="Label4">
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Width="3in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' CssClass="tabletext" ToolTip="Equipment test description" ID="DescriptionTextBox" MaxLength="50">
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
                        <td style="width: 498px; height: 50px" valign="middle" width="498">
                            <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
                    </tr>
                </table>
            </div>
        </form>
    </body>
</HTML>
