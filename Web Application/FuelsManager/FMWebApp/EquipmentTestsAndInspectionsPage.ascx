<%@ Control language="c#" Codebehind="EquipmentTestsAndInspectionsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentTestsAndInspectionsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<table id="Table1" style="z-index: 102; left: 0px; width: 238px; position: absolute; top: 16px; height: 10px"
    cellspacing="0" cellpadding="1" width="238" border="0">
    <tbody>
        <tr>
            <td>
                <FMControls:FMDataGrid ID="QualificationsDataGrid" runat="server" RowHeaderColumn="Inspection ID"
                    CssClass="tabletext" BackColor="White"
                    Width="320px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical"
                    BorderWidth="1px" AllowSorting="True"
                    BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="8"
                    OnItemDataBound="QualificationsDataGridItemDataBound">
                    <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn HeaderText="Edit">
                            <HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
                            <ItemStyle Wrap="False"></ItemStyle>
                            <ItemTemplate>
                                <itemstyle horizontalalign="Center" verticalalign="Middle"></itemstyle>
                                <FMControls:FMEditLinkButton runat="server" ID="EditButton" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <itemstyle horizontalalign="Center" verticalalign="Middle"></itemstyle>
                                <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                <FMControls:FMCancelLinkButton runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="False" HeaderText="Index">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Inspection ID">
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
                        <asp:TemplateColumn HeaderText="Number">
                            <HeaderStyle Wrap="False"></HeaderStyle>
                            <ItemStyle Wrap="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label Width="1.5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label1">
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox Width="1.5in" CssClass="tabletext" runat="server" Enabled="True" ID="NumberTextBox" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' DataTextField="Text" DataValueField="Value" MaxLength="50">
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Expiration Date">
                            <HeaderStyle Wrap="False"></HeaderStyle>
                            <ItemStyle Wrap="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label Width="1.5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ExpirationDate") %>' ID="Label15">
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <FMControls:FMDate ID="ExpirationDate" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ExpirationDate") %>'></FMControls:FMDate>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Delete">
                            <HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
                            <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            <ItemTemplate>
                                <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                </FMControls:FMDataGrid></td>
        </tr>
        <tr>
            <td height="21">
                <FMControls:FMButton ID="AddButton" Width="66px" runat="server" CssClass="formfieldtitle" Text="Add"></FMControls:FMButton></td>
        </tr>
    </tbody>
</table>
