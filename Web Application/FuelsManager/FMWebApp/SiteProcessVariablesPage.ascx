<%@ Control language="c#" Codebehind="SiteProcessVariablesPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteProcessVariablesPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
    <head>
    </head>
    <body>
	    <!-- this div sets off the Watchdog/Heartbeat Configuration section -->
        <div style="position: absolute; top: 0; left: 0; height: 128px; width: 350px; border: 1px; border-color: darkgray; border-style: solid">
        </div>
	    <FMControls:FMLabel id="WatchdogHeartbeatConfiguration" style="Z-INDEX: 111; LEFT: 14px; POSITION: absolute; TOP: 16px; right: 1289px;"
		    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px" Height="16px">Watchdog/Heartbeat Configuration</FMControls:FMLabel>
        <FMControls:FMLabel ID="Label" AssociatedControlID="WatchdogPeriodTextBox" Style="z-index: 111; left: 24px; position: absolute; top: 35px" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
                            Width="104px">
            Watchdog Period:
        </FMControls:FMLabel>
        <asp:TextBox ID="WatchdogPeriodTextBox" Style="z-index: 110; left: 187px; position: absolute; top: 35px" TabIndex="27" runat="server" CssClass="formfield" Width="89px" MaxLength="6">
        </asp:TextBox>
        <FMControls:FMLabel ID="Fmlabel2" Style="z-index: 107; left: 296px; position: absolute;
            top: 35px" runat="server" CssClass="formfieldtitle">secs</FMControls:FMLabel>
        <FMControls:FMLabel ID="Fmlabel3" AssociatedControlID="WatchdogModeDropDownList" Style="z-index: 107; left: 24px; position: absolute; top: 56px" runat="server" CssClass="formfieldtitle">
            Watchdog Mode:
        </FMControls:FMLabel>
        <FMControls:FMDropDownList ID="WatchdogModeDropDownList" Style="z-index: 101; left: 187px; position: absolute; top: 56px" TabIndex="28" runat="server" CssClass="formfield"
                                    Width="89px" AutoPostBack="True" OnSelectedIndexChanged="WatchdogModeDropDownList_SelectedIndexChanged">
        </FMControls:FMDropDownList>
        <FMControls:FMLabel ID="Fmlabel4" AssociatedControlID="CounterStartTextBox" Style="z-index: 111; left: 24px; position: absolute; top: 77px" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
                            Width="104px">Counter Start:</FMControls:FMLabel>
        <asp:TextBox ID="CounterStartTextBox" Style="z-index: 110; left: 187px; position: absolute; top: 77px" TabIndex="29" runat="server" CssClass="formfield" Width="89px">
        </asp:TextBox>
        <FMControls:FMLabel ID="Fmlabel5" AssociatedControlID="CounterEndTextBox" Style="z-index: 111; left: 24px; position: absolute; top: 98px" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
                            Width="104px">Counter End:</FMControls:FMLabel>
        <asp:TextBox ID="CounterEndTextBox" Style="z-index: 110; left: 187px; position: absolute; top: 98px" TabIndex="34" runat="server" CssClass="formfield" Width="89px">
        </asp:TextBox>
	    <!-- this div sets off the Watchdog/Heartbeat Configuration section -->
        <div style="position: absolute; top: 0; left: 370px; height: 128px; width: 350px; border: 1px; border-color: darkgray; border-style: solid">
        </div>
	    <FMControls:FMLabel id="VruSetpointAndDeadbandSettings" style="Z-INDEX: 111; LEFT: 384px; POSITION: absolute; TOP: 16px; right: 1289px;"
		    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px" Height="16px">VRU Setpoint and Deadband Settings</FMControls:FMLabel>
        <FMControls:FMLabel id="Label8" AssociatedControlID="SetpointTextBox" style="Z-INDEX: 118; LEFT: 394px; POSITION: absolute; TOP: 37px" runat="server"
                            CssClass="formfieldtitle" Width="104px" BackColor="Transparent">
            Setpoint:
        </FMControls:FMLabel>
        <asp:textbox id="SetpointTextBox" style="Z-INDEX: 120; LEFT: 542px; POSITION: absolute; TOP: 37px;"
                        tabIndex="16" runat="server" CssClass="formfield" Width="112px">
        </asp:textbox>
        <FMControls:FMLabel id="Label9" AssociatedControlID="DeadbandTextBox" style="Z-INDEX: 121; LEFT: 394px; POSITION: absolute; TOP: 58px" runat="server"
                            CssClass="formfieldtitle" Width="104px" BackColor="Transparent">
            Deadband:
        </FMControls:FMLabel>
        <asp:textbox id="DeadbandTextBox" style="Z-INDEX: 122; LEFT: 542px; POSITION: absolute; TOP: 58px"
                        tabIndex="17" runat="server" CssClass="formfield" Width="112px">
        </asp:textbox>
        <TABLE id="Table1" style="Z-INDEX: 102; LEFT: 0px; WIDTH: 7.28%; POSITION: absolute; TOP: 144px; HEIGHT: 10px"
               cellSpacing="0" cellPadding="1" border="0" aria-label="layout">
            <tr>
                <td height="29">
                    <FMControls:FMLabel id="Label4" runat="server" BackColor="Transparent" Width="136px" CssClass="formfieldtitle">Site Outputs:</FMControls:FMLabel>
                </td>
            </tr>
            <tr>
                <TD width="710">
                    <FMControls:FMDataGrid id="SiteOutputsDataGrid" runat="server" BackColor="White" Width="656px" CssClass="tabletext"
                                           Height="97px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
                                           CellPadding="3" PageSize="4" tabIndex="1">
                        <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                        <SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                        <EditItemStyle Wrap="False"></EditItemStyle>
                        <AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
                        <ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                        <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                        <Columns>
                            <asp:TemplateColumn HeaderText="Edit">
                                <HeaderStyle Width="0.5in"/>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
                                <ItemTemplate>
                                    <FMControls:FMEditLinkButton runat="server"/>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
                                <ItemTemplate>
                                    <FMControls:FMLabel ID="SiteOutputIdentityGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>'>
                                    </FMControls:FMLabel>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="Type" HeaderText="Type"></asp:BoundColumn>
                            <asp:BoundColumn DataField="Host" HeaderText="System"></asp:BoundColumn>
                            <asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server"></asp:BoundColumn>
                            <asp:BoundColumn DataField="OPCItemID" HeaderText="Item ID"></asp:BoundColumn>
                        </Columns>
                        <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                    </FMControls:FMDataGrid>
                </TD>
            </tr>
        </TABLE>
        <table style="Z-INDEX: 102; LEFT: 0px; WIDTH: 39.37%; POSITION: absolute; TOP: 320px; HEIGHT: 10px"
               cellSpacing="0" cellPadding="1" border="0" aria-label="layout">
            <tr>
                <td height="29" width="517">
                    <FMControls:FMLabel id="Label5" runat="server" BackColor="Transparent" Width="168px" CssClass="formfieldtitle">Site Permissives:</FMControls:FMLabel>
                </td>
            </tr>
            <tr>
                <td width="517">
                    <FMControls:FMDataGrid id="SitePermissivesDataGrid" runat="server" BackColor="White" Width="656px" CssClass="tabletext"
                                           Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
                                           BorderColor="White" CellPadding="3" PageSize="4" AllowPaging="True" tabIndex="2">
                        <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
                        </FooterStyle>

                        <SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C">
                        </SelectedItemStyle>

                        <EditItemStyle Wrap="False">
                        </EditItemStyle>

                        <AlternatingItemStyle Wrap="False" BackColor="Gainsboro">
                        </AlternatingItemStyle>

                        <ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE">
                        </ItemStyle>

                        <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
                        </HeaderStyle>

                        <Columns>
                            <asp:TemplateColumn HeaderText="Edit">
                                <HeaderStyle Width="0.5in">
                                </HeaderStyle>

                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle">
                                </ItemStyle>

                                <ItemTemplate>
                                    <FMControls:FMEditLinkButton runat="server"/>

                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
                                <ItemTemplate>
                                    <FMControls:FMLabel ID="SitePermissiveIdentityGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>'>
                                    </FMControls:FMLabel>

                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="Host" HeaderText="System"></asp:BoundColumn>
                            <asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server"></asp:BoundColumn>
                            <asp:BoundColumn DataField="OPCItemID" HeaderText="Item ID"></asp:BoundColumn>
                            <asp:BoundColumn DataField="MessageID" HeaderText="Message"></asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Delete">
                                <HeaderStyle Width="0.5in">
                                </HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle">
                                </ItemStyle>
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton runat="server"/>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>

                        <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages">
                        </PagerStyle>
                    </FMControls:FMDataGrid>
                </td>
            </tr>
            <tr>
                <td width="517">
                    <FMControls:FMButton id="AddSitePermissiveButton" tabIndex="3" runat="server" Width="67px" CssClass="formfieldtitle"
                                         Text="Add">
                    </FMControls:FMButton>
                </td>
            </tr>
        </table>
    </body>
</html>