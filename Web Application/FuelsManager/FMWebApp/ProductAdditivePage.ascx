<%@ Control Language="c#" CodeBehind="ProductAdditivePage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductAdditivePage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<FMControls:FMLabel ID="Label3" Style="z-index: 123; left: 0px; position: absolute; top: 24px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Additive Profiles:</FMControls:FMLabel>
<table id="Table1"
    style="z-index: 133; left: 0px; width: 238px; position: absolute; top: 56px; height: 10px"
    cellspacing="0" cellpadding="1" width="238" border="0">
    <tr>
        <td height="10">
            <FMControls:FMDataGrid ID="AdditiveProfilesDataGrid" runat="server" CssClass="tabletext" BackColor="White" Width="320px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="12" Height="56px" RowHeaderColumn="Additive Profile ID">
                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>

                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>

                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>

                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>

                <Columns>
                    <asp:TemplateColumn HeaderText="Additive Profile ID">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label1" NAME="Label1"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Rate">
                        <ItemTemplate>
                            <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Rate") %>' ID="Label4"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Cycle Volume">
                        <ItemTemplate>
                            <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CycleVolume") %>' ID="Label5"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Tolerance">
                        <ItemTemplate>
                            <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tolerance") %>' ID="Label6"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>

                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
            </FMControls:FMDataGrid></td>
    </tr>
</table>
