<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FuelRequestServiceRequestPage.ascx.cs" Inherits="FuelsManager.DispatchWebApp.FuelRequestServiceRequestPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<head>
    <title></title>
    <base target="_self" />
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<html>
<body>
    <asp:Panel ID="AircraftPanel" runat="server" GroupingText="Aircraft" CssClass="formfieldtitle">
        <table>
            <tr>
                <td>
                    <FMControls:FMLabel ID="RefIDLabel" runat="server" CssClass="formfieldtitle" Text="Ref ID:" Width="55px" />
                </td>
                <td>
                    <asp:Panel ID="RefIDComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="RefIDComboBox" ToolTip="Reference ID" runat="server" CssClass="formfield" Width="150px" DataTextField="XRef" DataValueField="MasterRecordGuid" AutoPostBack="true" OnSelectedIndexChanged="RefIDComboBoxSelectedIndexChanged" MaxLength="10"/>
                    </asp:Panel>
                </td>

                <td>
                    <FMControls:FMLabel ID="AircraftIDLabel" runat="server" CssClass="formfieldtitle" Text="Aircraft ID:" Width="65px" />
                </td>
                <td>
                    <asp:Panel ID="AircraftIDComboBoxPanel" ToolTip="Aircraft ID" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="AircraftIDComboBox" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" DataTextField="ID" DataValueField="MasterRecordGuid" AutoPostBack="true" OnSelectedIndexChanged="AircraftIDComboBoxSelectedIndexChanged" OnItemInserted="ComboBoxItemInserted" MaxLength="30"/>
                    </asp:Panel>
                </td>
                <td>
                    <FMControls:FMLabel ID="MDSLabel" runat="server" CssClass="formfieldtitle" Text="MDS:" Width="50px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="MDSTextBox" ToolTip="MDS" runat="server" CssClass="formfield" Width="150px" MaxLength="20"/>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="LocationLabel" runat="server" CssClass="formfieldtitle" Text="Location:" Width="55px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="LocationTextBox" ToolTip="Location" runat="server" CssClass="formfield" Width="150px" />
                </td>

                <td>
                    <FMControls:FMLabel ID="GradeLabel" runat="server" CssClass="formfieldtitle" Text="Grade:" Width="65px" />
                </td>
                <td>
                    <asp:Panel ID="GradeComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="GradeComboBox" ToolTip="Grade" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" AutoPostBack="true" DataTextField="ID" DataValueField="MasterRecordGuid" OnSelectedIndexChanged="GradeComboBoxSelectedIndexChanged" MaxLength="30"/>
                    </asp:Panel>
                </td>
                <td>
                    <FMControls:FMLabel ID="ActivityLabel" runat="server" CssClass="formfieldtitle" Text="Activity:" Width="50px" />
                </td>
                <td>
                    <asp:Panel ID="ActivityComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="ActivityComboBox" ToolTip="Activity" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" AutoPostBack="true" DataTextField="ID" DataValueField="IdentityGuid" OnSelectedIndexChanged="ActivityComboBox_SelectedIndexChanged" MaxLength="50"/>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <FMControls:FMCheckBox ID="FuelAdditiveCheckBox" runat="server" CssClass="formfieldtitle" Text="Fuel Additive" Width="95px" Style="text-align: center" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="RequestPanel" runat="server" GroupingText="Request" CssClass="formfieldtitle">
        <table>
            <tr>
                <td>
                    <FMControls:FMLabel ID="RequestedByLabel" runat="server" CssClass="formfieldtitle" Text="Requested By:" Width="85px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="RequestedByTextBox" ToolTip="Requested by" runat="server" CssClass="formfield" Width="175px" MaxLength="50"/>
                </td>

                <td>
                    <FMControls:FMLabel ID="RequestTypeLabel" runat="server" CssClass="formfieldtitle" Text="Request Type:" Width="85px" />
                </td>
                <td>
                    <asp:Panel ID="RequestTypeComboBoxPanel" ToolTip="Requested Type" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="RequestTypeComboBox" runat="server" CssClass="formfield" Width="150px" DropDownStyle="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="RequestTypeComboBoxSelectedIndexChanged">
                            <asp:ListItem>Refuel</asp:ListItem>
                            <asp:ListItem>Defuel</asp:ListItem>
                        </FMControls:FMComboBox>
                    </asp:Panel>
                </td>

                <td>
                    <FMControls:FMCheckBox ID="RequestCancelledCheckBox" runat="server" CssClass="formfieldtitle" Text="Request Cancelled" Width="130px" AutoPostBack="True" OnCheckedChanged="RequestCancelledCheckboxChecked" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="CommentsLabel" runat="server" CssClass="formfieldtitle" Text="Comments:" Width="85px" />
                </td>
                <td colspan="4">
                    <FMControls:FMTextBox ID="CommentsTextBox" ToolTip="Comments" runat="server" CssClass="formfield" TextMode="MultiLine" Width="625px" MaxLength="1000"/>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="BillingInfoPanel" runat="server" GroupingText="Billing Info" CssClass="formfieldtitle">
        <table>
            <tr>
                <td>
                    <FMControls:FMLabel ID="DODAACLabel" runat="server" CssClass="formfieldtitle" Text="DoDAAC:" Width="85px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="DODAACTextBox" ToolTip="DoDAAC" runat="server" CssClass="formfield" Width="125px" MaxLength="100"/>
                </td>

                <td>
                    <FMControls:FMLabel ID="SuppDODAACLabel" runat="server" CssClass="formfieldtitle" Text="Supp DoDAAC:" Width="100px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="SuppDODAACTextBox" ToolTip="Supplier DoDAAC" runat="server" CssClass="formfield" Width="125px" MaxLength="100"/>
                </td>
                <td>
                    <FMControls:FMLabel ID="BOSLabel" runat="server" CssClass="formfieldtitle" Text="BOS:" Width="85px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="BOSTextBox" ToolTip="BoS" runat="server" CssClass="formfield" Width="125px" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="UseCodeLabel" runat="server" CssClass="formfieldtitle" Text="Use Code:" Width="85px" />
                </td>
                <td>
                    <asp:Panel ID="UseCodeComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="UseCodeComboBox" ToolTip="Use Code" runat="server" CssClass="formfield" Width="125px" DropDownStyle="DropDownList" DataTextField="ID" DataValueField="ID" />
                    </asp:Panel>
                </td>

                <td>
                    <FMControls:FMLabel ID="SignalCodeLabel" runat="server" CssClass="formfieldtitle" Text="Signal Code:" Width="100px" />
                </td>
                <td>
                    <asp:Panel ID="SignalCodeComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="SignalCodeComboBox" ToolTip="Signal Code" runat="server" CssClass="formfield" Width="125px" DropDownStyle="DropDownList" DataTextField="ID" DataValueField="ID" />
                    </asp:Panel>
                </td>
                <td>
                    <FMControls:FMLabel ID="FundCodeLabel" runat="server" CssClass="formfieldtitle" Text="Fund Code:" Width="85px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="FundCodeTextBox" ToolTip="Fund Code" runat="server" CssClass="formfield" Width="125px" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="CardNumberLabel" runat="server" CssClass="formfieldtitle" Text="Card No:" Width="85px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="CardNumberTextBox" ToolTip="Card number" runat="server" CssClass="formfield" Width="125px" MaxLength="30"/>
                </td>

                <td>
                    <FMControls:FMLabel ID="RPTTECAPCLabel" runat="server" CssClass="formfieldtitle" Text="RPT/TEC/APC:" Width="100px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="RPTTECAPCTextBox" ToolTip="RPT/TEC/APC" runat="server" CssClass="formfield" Width="125px" />
                </td>

            </tr>
        </table>
    </asp:Panel>
</body>
</html>
