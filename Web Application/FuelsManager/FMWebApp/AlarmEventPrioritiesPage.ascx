<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="AlarmEventPrioritiesPage.ascx.cs"
    Inherits="FuelsManager.FMWebApp.AlarmEventPrioritiesPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<html>
<head>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <script>
        $(document).ready(function () {
            // change the colors in the dropdowns to match the selection just performed
            $("#tcAlarmEventConfig_tpPriorities_AlarmEventPrioritiesPage_PrioritiesDataGrid").on('change', 'select', function (a, b) {
                $(this).attr('style', ($(this).find('option:selected').attr('style')) + ';width:2.75in');
            });
        });
    </script>
    <table id="Table1" style="z-index: 100; left: 0px; width: 43.18%; position: absolute;
        top: 20px; height: 10px" cellspacing="0" cellpadding="1" border="0">
        <tr>
            <td width="350" height="36" valign="middle">
                <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                    TabIndex="6" />
                &nbsp;&nbsp;
                <FMControls:FMPageSizeDropDown ID="AlarmPriorityPageSizeDropDown" ToolTip="Page Size" runat="server"
                    TabIndex="7" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td style="width: 498px; height: 10px" width="498">
                <FMControls:FMDataGrid ID="PrioritiesDataGrid" runat="server" CssClass="tabletext" RowHeaderColumn="Priority Name"
                    AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
                    Width="648px" GridLines="Vertical" AutoGenerateColumns="False" BackColor="White"
                    BorderStyle="None" PageSize="8" TabIndex="1">
                    <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
                    </HeaderStyle>
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
                        <asp:TemplateColumn Visible="False" HeaderText="Index">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'
                                    ID="IndexLabel">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Priority Name">
                            <ItemTemplate>
                                <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'
                                    ID="Label1">
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'
                                    CssClass="tabletext" ID="IDTextBox" MaxLength="32">
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Steady" ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Label Width="3in" runat="server" Text='Alarm' ID="SteadyLabel"></asp:Label><br />
                                <asp:Label Width="3in" runat="server" Text='Background color: white, Text color: black' ID="SteadyColorLabel"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <FMControls:FMDropDownList Width="2.75in" runat="server" CssClass="tabletext" ID="BackgroundSteadyDropDownList">
                                </FMControls:FMDropDownList>
								<br />
                                <FMControls:FMDropDownList Width="2.75in" runat="server" CssClass="tabletext" ID="TextSteadyDropDownList">
                                </FMControls:FMDropDownList>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Alternate" ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Label Width="3in" runat="server" Text='Alarm' ID="AlternateLabel"></asp:Label><br />
                                <asp:Label Width="3in" runat="server" Text='Background color: white, Text color: black' ID="AlternateColorLabel"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <FMControls:FMDropDownList Width="2.75in" runat="server" CssClass="tabletext" ID="BackgroundAlternateDropDownList">
                                </FMControls:FMDropDownList>
								<br />
                                <FMControls:FMDropDownList Width="2.75in" runat="server" CssClass="tabletext" ID="TextAlternateDropDownList">
                                </FMControls:FMDropDownList>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Sound File">
                            <ItemTemplate>
                                <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SoundFile") %>'
                                    ID="Label2">
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SoundFile") %>'
                                    CssClass="tabletext" ID="SoundFileTextBox">
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
	                    <asp:TemplateColumn HeaderText="Priority">
		                    <ItemTemplate>
			                    <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Priority") %>'
			                               ID="PriorityLabel">
			                    </asp:Label>
		                    </ItemTemplate>
		                    <EditItemTemplate>
			                    <asp:TextBox Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Priority") %>'
			                                 CssClass="tabletext" ID="PriorityTextBox">
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
            <td style="width: 498px; height: 10px" valign="middle" width="498">
                <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
                    TabIndex="2"></FMControls:FMButton>
            </td>
        </tr>
    </table>
</body>
</html>
