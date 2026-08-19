<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="ProductMessagesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductMessagesForm" %>
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
	<body MS_POSITIONING="GridLayout" role="application">
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="550px" BackColor="Transparent">Product Messages Configuration</FMControls:FMLabel>
                <table id="Table1" style="z-index: 100; left: 32px; width: 660px; position: absolute; top: 48px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td style="width: 660px; height: 36px" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
                            &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="ProductMessagesFormPageSizeDropDown" ToolTip="Page Size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 660px; height: 10px">
                            <FMControls:FMDataGrid ID="ApplicationStringsDataGrid" Style="left: 1px; top: 0px" RowHeaderColumn="Product Messages"
                                runat="server" CssClass="tabletext" AllowPaging="True" CellPadding="3" BorderColor="White"
                                AllowSorting="True" BorderWidth="1px" Width="660px" GridLines="Vertical" AutoGenerateColumns="False"
                                BackColor="White" BorderStyle="None" PageSize="16">
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
                                    <asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>'
                                                ID="SiteGuidLabel">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>'
                                                ID="IdentityGuidLabel">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Product Messages">
                                        <HeaderStyle Width="530px"></HeaderStyle>
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>'
                                                ID="Label1">
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Width="525px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.String") %>'
                                                CssClass="tabletext" ID="StringTextBox" ToolTip="Product Messages" MaxLength="120" aria-required="true">
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
                        <td style="width: 498px; height: 50px" valign="middle" width="498">
                            <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
                    </tr>
                </table>
            </div>
        </form>
	</body>
</HTML>
