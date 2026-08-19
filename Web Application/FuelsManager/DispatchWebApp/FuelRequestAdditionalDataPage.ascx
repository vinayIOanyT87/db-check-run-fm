<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FuelRequestAdditionalDataPage.ascx.cs" Inherits="FuelsManager.DispatchWebApp.FuelRequestAdditionalDataPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<head>
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<html>
<body>
    <table>
        <tr>
            <td>
                <FMControls:FMLabel ID="UserData1Label" runat="server" CssClass="formfieldtitle" Text="User Data 1:" Width="90px" />
                <FMControls:FMLabel ID="NoUserDataFieldsLabel" runat="server" CssClass="formfieldtitle" Text="No Line Item User Data Fields Configured" Width="120px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData1TextBox" ToolTip="User Data 1" runat="server" CssClass="formfield" Width="125px" Visible="true" />
                <asp:Panel ID="UserData1ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData1ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData2Label" runat="server" CssClass="formfieldtitle" Text="User Data 2:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData2TextBox" ToolTip="User Data 2" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData2ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData2ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData3Label" runat="server" CssClass="formfieldtitle" Text="User Data 3:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData3TextBox" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData3ComboBoxPanel" ToolTip="User Data 3" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData3ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="UserData4Label" runat="server" CssClass="formfieldtitle" Text="User Data 4:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData4TextBox" ToolTip="User Data 4" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData4ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData4ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData5Label" runat="server" CssClass="formfieldtitle" Text="User Data 5:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData5TextBox" ToolTip="User Data 5" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData5ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData5ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"  />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData6Label" runat="server" CssClass="formfieldtitle" Text="User Data 6:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData6TextBox" ToolTip="User Data 6" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData6ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData6ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="UserData7Label" runat="server" CssClass="formfieldtitle" Text="User Data 7:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData7TextBox" ToolTip="User Data 7" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData7ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData7ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData8Label" runat="server" CssClass="formfieldtitle" Text="User Data 8:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData8TextBox" ToolTip="User Data 8" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData8ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData8ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"/>
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData9Label" runat="server" CssClass="formfieldtitle" Text="User Data 9:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData9TextBox" ToolTip="User Data 9" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData9ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData9ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="UserData10Label" runat="server" CssClass="formfieldtitle" Text="User Data 10:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData10TextBox" ToolTip="User Data 10" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData10ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData10ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"  />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData11Label" runat="server" CssClass="formfieldtitle" Text="User Data 11:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData11TextBox" ToolTip="User Data 11" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData11ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData11ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"  />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData12Label" runat="server" CssClass="formfieldtitle" Text="User Data 12:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData12TextBox" ToolTip="User Data 12" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData12ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData12ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"  />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="UserData13Label" runat="server" CssClass="formfieldtitle" Text="User Data 13:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData13TextBox" ToolTip="User Data 13" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData13ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData13ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"  />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData14Label" runat="server" CssClass="formfieldtitle" Text="User Data 14:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData14TextBox" ToolTip="User Data 14" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData14ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData14ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"/>
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData15Label" runat="server" CssClass="formfieldtitle" Text="User Data 15:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData15TextBox" ToolTip="User Data 15" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData15ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData15ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"  />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="UserData16Label" runat="server" CssClass="formfieldtitle" Text="User Data 16:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData16TextBox" ToolTip="User Data 16" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData16ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData16ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData17Label" runat="server" CssClass="formfieldtitle" Text="User Data 17:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData17TextBox" ToolTip="User Data 17" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData17ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData17ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"/>
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData18Label" runat="server" CssClass="formfieldtitle" Text="User Data 18:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData18TextBox" ToolTip="User Data 18" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData18ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData18ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="UserData19Label" runat="server" CssClass="formfieldtitle" Text="User Data 19:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData19TextBox" ToolTip="User Data 19" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData19ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData19ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData20Label" runat="server" CssClass="formfieldtitle" Text="User Data 20:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData20TextBox" ToolTip="User Data 20" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData20ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData20ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData21Label" runat="server" CssClass="formfieldtitle" Text="User Data 21:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData21TextBox" ToolTip="User Data 21" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData21ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData21ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="UserData22Label" runat="server" CssClass="formfieldtitle" Text="User Data 22:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData22TextBox" ToolTip="User Data 22" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData22ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData22ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"  />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData23Label" runat="server" CssClass="formfieldtitle" Text="User Data 23:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData23TextBox" ToolTip="User Data 23" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData23ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData23ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList"  />
                </asp:Panel>
            </td>
            <td>
                <FMControls:FMLabel ID="UserData24Label" runat="server" CssClass="formfieldtitle" Text="User Data 24:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="UserData24TextBox" ToolTip="User Data 24" runat="server" CssClass="formfield" Width="125px" />
                <asp:Panel ID="UserData24ComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px" Visible="false">
                    <FMControls:FMComboBox ID="UserData24ComboBox" runat="server" CssClass="formfield" Width="100px" DataTextField="ID" DataValueField="ID" DropDownStyle="DropDownList" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="SerialNumberLabel" runat="server" CssClass="formfieldtitle" Text="Serial Number:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="SerialNumberTextBox" ToolTip="Serial Number" runat="server" CssClass="formfield" Width="125px" />
            </td>
            <td>
                <FMControls:FMLabel ID="GrossGalLabel" runat="server" CssClass="formfieldtitle" Text="Gross Gal:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="GrossGalTextBox" ToolTip="Gross Gallon" runat="server" CssClass="formfield" Width="125px" />
            </td>
            <td>
                <FMControls:FMLabel ID="IssuePointLabel" runat="server" CssClass="formfieldtitle" Text="Iss Pt:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="IssuePointTextBox" ToolTip="Issue Point" runat="server" CssClass="formfield" Width="125px" />
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="TransIDLabel" runat="server" CssClass="formfieldtitle" Text="Trans ID:" Width="90px" />
            </td>
            <td colspan="3">
                <FMControls:FMTextBox ID="TransIDTextBox" ToolTip="Transaction ID" runat="server" CssClass="formfield" Width="355px" ReadOnly="true" Enabled="false" />
            </td>
            <td>
                <FMControls:FMLabel ID="IssuePointNumberLabel" runat="server" CssClass="formfieldtitle" Text="Iss Pt Num:" Width="90px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="IssuePointNumberTextBox" ToolTip="Issue Point Number" runat="server" CssClass="formfield" Width="125px" />
            </td>

        </tr>
    </table>

</body>
</html>
