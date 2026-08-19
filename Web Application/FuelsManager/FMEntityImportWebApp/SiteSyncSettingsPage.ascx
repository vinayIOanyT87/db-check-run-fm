<%@ Control Language="c#" CodeBehind="SiteSyncSettingsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.SiteSyncSettingsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<table id="Table1" style="WIDTH: 238px; " cellspacing="0" cellpadding="1" border="0">
    <tr>
        <td style="width: 445px; ">
            <FMControls:FMDataGridFixed ID="SiteSyncSettingDataGrid" runat="server" Width="700px" CssClass="tabletext" RowHeaderColumn="Site / Site Group"
                BackColor="White" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
                CellPadding="3" PageSize="8" FixedHeight="300px" Height="300px" aria-label="Site Sync Settings Grid">
                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                <Columns>
                    <asp:TemplateColumn HeaderText="Edit">
                        <HeaderStyle Width="0.5in" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemTemplate>
                            <FMControls:FMEditLinkButton ID="FMEditLinkButton" runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <FMControls:FMUpdateLinkButton ID="FMUpdateLinkButton" runat="server" />
                            <FMControls:FMCancelLinkButton ID="FMCancelLinkButton" runat="server" />
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn Visible="False" HeaderText="Index">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>' ID="IndexLabel">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Site / Site Group">
                        <ItemStyle Wrap="False"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteID") %>' ID="SiteIDLabelRO"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Sync Disabled">
                        <HeaderStyle Width="1.5in" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemTemplate>
                            <asp:CheckBox runat="server" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.DisableSyncTransferFlag") %>'></asp:CheckBox>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.DisableSyncTransferFlag") %>' ID="DisableSyncTransferFlag"></asp:CheckBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Periodic Sync Enabled">
                        <HeaderStyle Width="1.5in" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemTemplate>
                            <asp:CheckBox runat="server" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.EnablePeriodicSyncFlag") %>'></asp:CheckBox>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.EnablePeriodicSyncFlag") %>' ID="EnablePeriodicSyncFlag"></asp:CheckBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Periodic Sync Interval (minutes)">
                        <ItemTemplate>
                            <asp:Label Width="50px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PeriodicSyncIntervalMinutes") %>'>
                            </asp:Label>
                            <asp:Label Width="75px" runat="server" Text='minutes'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox Width="50px" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PeriodicSyncIntervalMinutes") %>' ID="PeriodicSyncIntervalMinutes">
                            </asp:TextBox>&nbsp;
                            <asp:Label Width="75px" runat="server" Text='minutes' ID="PeriodicSyncIntervalMinutesLabel" >
                            </asp:Label>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
            </FMControls:FMDataGridFixed></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td style="white-space: nowrap;">
            <FMControls:FMLabel ID="DisableAllSitesLabel"  runat="server" CssClass="formfieldtitle" Text="Sync Disable/Enable All Sites:" />
				&nbsp;
				<FMControls:FMButton ID="Disable" TabIndex="101"
 				runat ="server" CssClass="formfieldtitle" Width="67px" Text="Disable"></FMControls:FMButton>
				&nbsp;
				<FMControls:FMButton ID="Enable" TabIndex="101"
 				runat ="server" CssClass="formfieldtitle" Width="67px" Text="Enable"></FMControls:FMButton>
    </tr>
</table>
