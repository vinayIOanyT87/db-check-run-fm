<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="AlarmEventAssignmentPage.ascx.cs"
    Inherits="FuelsManager.FMWebApp.AlarmEventAssignmentPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<html>
<head>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
       <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/AlarmAndEvent.js" %>"  type="text/javascript"></script>
</head>
<body>
    <FMControls:FMLabel ID="Label1" AssociatedControlID="SourceDropDownList" Style="z-index: 100; left: 8px; position: absolute; top: 16px"
        runat="server" BackColor="Transparent" CssClass="formfieldtitle">Source:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="SourceDropDownList" Style="z-index: 101; left: 80px; position: absolute; top: 16px"
        runat="server" CssClass="formfield" Width="384px"
        AutoPostBack="True" OnSelectedIndexChanged="SourceDropDownListSelectedIndexChanged">
    </FMControls:FMDropDownList>
    <FMControls:FMLabel ID="Label2" AssociatedControlID="TypeDropDownList" Style="z-index: 100; left: 8px; position: absolute; top: 48px"
        runat="server" BackColor="Transparent" CssClass="formfieldtitle">Type:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="TypeDropDownList" Style="z-index: 101; left: 80px; position: absolute; top: 48px"
        runat="server" CssClass="formfield" Width="96px"
        AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged">
    </FMControls:FMDropDownList>
   <table id="Table4" style="z-index: 100; left: 0px; width: 43.18%; position: absolute; top: 80px; height: 10px"
        cellspacing="0" cellpadding="1" border="0">
        <tr>
            <td width="350" height="36" valign="middle">
                <FMControls:FMPageSizeDropDown ID="AlarmAssignPageSizeDropDown" ToolTip="Page size" runat="server"
                    OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td style="width: 501px; height: 10px" width="501">
                <FMControls:FMDataGrid ID="AssignmentDataGrid" runat="server" BackColor="White" CssClass="tabletext" RowHeaderColumn="ID"
                    Width="663px" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True"
                    BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None"
                    PageSize="5">
                    <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablecol" BackColor="#008A8C"></SelectedItemStyle>
                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                    <ItemStyle ForeColor="Black" CssClass="tablecol" BackColor="#EEEEEE"></ItemStyle>
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
                        <asp:TemplateColumn HeaderText="ID">
                            <ItemTemplate>
                                <asp:Label Width="3in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'
                                    ID="IDLabel">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Category">
                            <ItemTemplate>
                                <asp:Label Width="1.5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CategoryID") %>'
                                    ID="CategoryIDLabel">
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList Width="1.5in" runat="server" CssClass="tabletext" ID="CategoryDropDownList">
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Priority">
                            <ItemTemplate>
                                <asp:Label Width="1.5in" runat="server" Visible='<%# DataBinder.Eval(Container, "DataItem.Alarm") %>'
                                    Text='<%# DataBinder.Eval(Container, "DataItem.PriorityID") %>' ID="PriorityIDLabel">
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList Width="1.5in" runat="server" Visible='<%# DataBinder.Eval(Container, "DataItem.Alarm") %>'
                                    CssClass="tabletext" ID="PriorityDropDownList">
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Enabled">
                            <ItemTemplate>
                                <asp:Label Width="1in" runat="server" CssClass="tabletext" ID="lblEnabled" Text='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox Width="1in" runat="server" ID="EnabledCheckbox" Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>'></asp:CheckBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Email Form">
                            <HeaderStyle Width="55px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            <ItemTemplate>
                                <FMControls:FMElipseButton runat="server" ID="EditEmailForm" OnClick="CustomizeEmailMessage(this)" />
                                <div style="display:none" runat="server" ID="IdentityGuid"><%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %></div>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="False" HeaderText="EnabledOriginalValue">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblEnabledOriginal" Text='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="False" HeaderText="CategoryGuidOriginalValue">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCategoryGuidOriginal" Text='<%# DataBinder.Eval(Container, "DataItem.CategoryGuid") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="False" HeaderText="PriorityGuidOriginalValue">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblPriorityGuidOriginal" Text='<%# DataBinder.Eval(Container, "DataItem.PriorityGuid") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
                        Mode="NumericPages"></PagerStyle>
                </FMControls:FMDataGrid>
            </td>
        </tr>
    </table>
</body>
</html>
