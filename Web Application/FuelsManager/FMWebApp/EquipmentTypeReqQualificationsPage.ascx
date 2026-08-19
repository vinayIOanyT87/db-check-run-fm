<%@ Control language="c#" Codebehind="EquipmentTypeReqQualificationsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentTypeReqQualificationsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
        <table id="Table1" style="z-index: 102; left: 0px; width: 238px; position: absolute; top: 16px; height: 10px"
            cellspacing="0" cellpadding="1" width="238" border="0">
            <tbody>
                <tr>
                    <td>
                        <FMControls:FMDataGrid ID="QualificationsDataGrid" runat="server" CssClass="tabletext" BackColor="White" RowHeaderColumn="Qualification ID"
                            Width="320px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
                            BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="8">
                            <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
                                    <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" ID="EditButton" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMUpdateLinkButton runat="server" ID="Fmupdatelinkbutton1" />&nbsp;
                                        <FMControls:FMCancelLinkButton runat="server" ID="Fmcancellinkbutton1" />

                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="False" HeaderText="Index">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Qualification ID">
                                    <HeaderStyle Wrap="False"></HeaderStyle>
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label Width="2in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.QualificationID") %>' ID="Label11">
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tabletext" runat="server" Enabled="True" ID="QualificationsDropDownList" DataSource="<%# EnumerateQualifications()%>" DataTextField="Text" DataValueField="Value">
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Delete">
                                    <HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
                                    <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMDeleteLinkButton runat="server" ID="DeleteButton" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMDataGrid></td>
                </tr>
                <tr>
                    <td height="21">
                        <FMControls:FMButton ID="AddButton" runat="server" Width="66px" CssClass="formfieldtitle" Text="Add"></FMControls:FMButton></td>
                </tr>
            </tbody>
        </table>
    </body>
</HTML>
