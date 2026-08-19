<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FuelRequestDetailPage.ascx.cs" Inherits="FuelsManager.DispatchWebApp.FuelRequestDetailPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<head>
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<html>
<body>
    <asp:Panel ID="DetailPanel" runat="server" GroupingText="Detail" CssClass="formfieldtitle">
        <table>
            <tr>
                <td>
                    <FMControls:FMLabel ID="QuantityLabel" runat="server" CssClass="formfieldtitle" Text="Quantity:" Width="85px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="QuantityTextBox" ToolTip="Quantity" runat="server" CssClass="formfield" Width="150px" OnTextChanged="QuantityTextBox_TextChanged" />
                </td>

                <td>
                    <FMControls:FMLabel ID="RegistrationIDLabel" runat="server" CssClass="formfieldtitle" Text="Registration ID:" Width="90px" />
                </td>
                <td>
                    <asp:Panel ID="RegistrationIDComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="RegistrationIDComboBox" ToolTip="Registration ID" runat="server" CssClass="formfield" DropDownStyle="DropDownList" AutoPostBack="true" DataTextField="ID" DataValueField="MasterRecordGuid" Width="150px" OnSelectedIndexChanged="RegistrationIDComboBox_SelectedIndexChanged" MaxLength="30"/>
                    </asp:Panel>
                </td>
                <td>
                    <FMControls:FMLabel ID="DifferentialPressureLabel" runat="server" CssClass="formfieldtitle" Text="Differential Pressure:" Width="125px" />
                    <FMControls:FMLabel ID="VarianceLabel" runat="server" CssClass="formfieldtitle" Text="Variance:" Width="125px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="DifferentialPressureAndVarianceTextBox" ToolTip="Differential Pressure" runat="server" CssClass="formfield" Width="50px" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="RadioNumberLabel" runat="server" CssClass="formfieldtitle" Text="Radio Number:" Width="85px" />
                </td>
                <td>
                    <FMControls:FMTextBox ID="RadioNumberTextBox" ToolTip="Radio Number" runat="server" CssClass="formfield" Width="150px" />
                </td>

                <td>
                    <FMControls:FMLabel ID="OperatorLabel" runat="server" CssClass="formfieldtitle" Text="Operator:" Width="90px" />
                </td>
                <td>
                    <asp:Panel ID="OperatorComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                        <FMControls:FMComboBox ID="OperatorComboBox" ToolTip="Operator" runat="server" CssClass="formfield" DropDownStyle="DropDownList" Width="150px" DataTextField="FullName" DataValueField="IdentityGuid" MaxLength="50"/>
                    </asp:Panel>
                </td>
            </tr>

        </table>
    </asp:Panel>
    <asp:Panel ID="ServiceHistoryPanel" runat="server" GroupingText="Service History" CssClass="formfieldtitle">
        <table>
            <tr>
                <td>
                    <FMControls:FMLabel ID="RequestDateTimeLabel" runat="server" CssClass="formfieldtitle" Text="Request Date:" Width="110px" />
                </td>
                <td>
                    <FMControls:FMDateTime ID="RequestDateTimeControl" runat="server" CssClass="formfield" Width="330px" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="DispatchDateTimeLabel" runat="server" CssClass="formfieldtitle" Text="Dispatch Date:" Width="110px" />
                </td>
                <td>
                    <FMControls:FMDateTime ID="DispatchDateTimeControl" runat="server" CssClass="formfield" Width="330px" />
                </td>

                <td>
                    <FMControls:FMCheckBox ID="IgnoreDispatchDateTimeCheckBox" runat="server" CssClass="formfieldtitle" Text="Ignore" OnCheckedChanged="IgnoreDispatchDateTimeCheckBox_CheckedChanged" AutoPostBack="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="ArrivalDateTimeLabel" runat="server" CssClass="formfieldtitle" Text="Arrival Date:" Width="110px" />
                </td>
                <td>
                    <FMControls:FMDateTime ID="ArrivalDateTimeControl" runat="server" CssClass="formfield" Width="330px" />
                </td>

                <td>
                    <FMControls:FMCheckBox ID="IgnoreArrivalDateTimeCheckBox" runat="server" CssClass="formfieldtitle" Text="Ignore" OnCheckedChanged="IgnoreArrivalDateTimeCheckBox_CheckedChanged" AutoPostBack="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="StartDateTimeLabel" runat="server" CssClass="formfieldtitle" Text="Start Date:" Width="110px" />
                </td>
                <td>
                    <FMControls:FMDateTime ID="StartDateTimeControl" runat="server" CssClass="formfield" Width="330px" />
                </td>

                <td>
                    <FMControls:FMCheckBox ID="IgnoreStartDateTimeCheckBox" runat="server" CssClass="formfieldtitle" Text="Ignore" OnCheckedChanged="IgnoreStartDateTimeCheckBox_CheckedChanged" AutoPostBack="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="StopDateTimeLabel" runat="server" CssClass="formfieldtitle" Text="Stop Date:" Width="110px" />
                </td>
                <td>
                    <FMControls:FMDateTime ID="StopDateTimeControl" runat="server" CssClass="formfield" Width="330px" />
                </td>
                <td>
                    <FMControls:FMCheckBox ID="IgnoreStopDateTimeCheckBox" runat="server" CssClass="formfieldtitle" Text="Ignore" OnCheckedChanged="IgnoreStopDateTimeCheckBox_CheckedChanged" AutoPostBack="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="CompletionDateTimeLabel" runat="server" CssClass="formfieldtitle" Text="Completion Date:" Width="110px" />
                </td>
                <td>
                    <FMControls:FMDateTime ID="CompletionDateTimeControl" runat="server" CssClass="formfield" Width="330px" />
                </td>

            </tr>
        </table>
    </asp:Panel>
</body>
</html>
