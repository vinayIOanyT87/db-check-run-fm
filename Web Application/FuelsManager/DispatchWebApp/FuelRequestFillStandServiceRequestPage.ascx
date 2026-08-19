<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FuelRequestFillStandServiceRequestPage.ascx.cs" Inherits="FuelsManager.DispatchWebApp.FuelRequestFillStandServiceRequestPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<head>
    <title></title>
    <base target="_self" />
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<html>
<body>
    <asp:Panel ID="VehiclePanel" runat="server" GroupingText="Vehicle" CssClass="formfieldtitle" Width="725px">
        <table>
            <tr>
                <td>
                    <FMControls:FMLabel ID="RefCodeLabel" runat="server" CssClass="formfieldtitle" Text="Ref Code:" Width="75px" />
                </td>
                <td>
                    <asp:Panel ID="RefCodeComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="RefCodeComboBox" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="RefCodeComboBoxSelectedIndexChanged" DataTextField="XRef" DataValueField="MasterRecordGuid" MaxLength="10"/>
                    </asp:Panel>
                </td>
                <td style="padding-left: 145px">
                    <FMControls:FMLabel ID="RegistrationIDLabel" runat="server" CssClass="formfieldtitle" Text="Registration ID:" Width="95px" />
                </td>
                <td>
                    <asp:Panel ID="RegistrationIDComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="RegistrationIDComboBox" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="RegistrationIDComboBoxSelectedIndexChanged" DataTextField="ID" DataValueField="MasterRecordGuid" MaxLength="30"/>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="TypeLabel" runat="server" CssClass="formfieldtitle" Text="Type:" Width="75px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="TypeTextBox" runat="server" CssClass="formfield" Width="150px" ReadOnly="true" Enabled="false" />
                </td>

                <td style="padding-left: 145px">
                    <FMControls:FMLabel ID="GradeLabel" runat="server" CssClass="formfieldtitle" Text="Grade:" Width="95px"/>
                </td>
                <td>
                    <asp:Panel ID="GradeComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="GradeComboBox" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" DataTextField="ID" DataValueField="MasterRecordGuid"  MaxLength="30"/>
                    </asp:Panel>
                </td>

            </tr>
            <tr>
                <td></td>
                <td></td>

                <td style="padding-left: 145px">
                    <FMControls:FMLabel ID="LocationLabel" runat="server" CssClass="formfieldtitle" Text="Location:" Width="95px" />
                </td>

                <td>
                    <asp:Panel ID="LocationComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="LocationComboBox" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" DataTextField="ID" DataValueField="MasterRecordGuid" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="RequestPanel" runat="server" GroupingText="Request" CssClass="formfieldtitle" Width="725px">
        <table>
            <tr>
                <td>
                    <FMControls:FMLabel ID="RequestedByLabel" runat="server" CssClass="formfieldtitle" Text="Requested By:" Width="90px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="RequestedByTextBox" runat="server" CssClass="formfield" Width="150px" MaxLength="50"/>
                </td>

                <td>
                    <FMControls:FMLabel ID="RequestTypeLabel" runat="server" CssClass="formfieldtitle" Text="Request Type:" Width="90px" />
                </td>
                <td>
                    <asp:Panel ID="RequestTypeComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="RequestTypeComboBox" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="RequestTypeComboBoxSelectedIndexChanged">
                            <asp:ListItem>Fill</asp:ListItem>
                            <asp:ListItem>Partial Fill</asp:ListItem>
                            <asp:ListItem>Return to Bulk</asp:ListItem>
														<asp:ListItem>Partial Return to Bulk</asp:ListItem>
                        </FMControls:FMComboBox>
                    </asp:Panel>
                </td>

                <td>
                    <FMControls:FMCheckBox ID="RequestCancelledCheckBox" runat="server" CssClass="formfieldtitle" Text="Request Cancelled" Width="130px" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="CommentsLabel" runat="server" CssClass="formfieldtitle" Text="Comments:" Width="90px" />
                </td>
                <td colspan="4">
                    <FMControls:FMTextBox ID="CommentsTextBox" runat="server" CssClass="formfield" TextMode="MultiLine" Width="600px" MaxLength="1000"/>
                </td>
            </tr>
        </table>
    </asp:Panel>


</body>
</html>
