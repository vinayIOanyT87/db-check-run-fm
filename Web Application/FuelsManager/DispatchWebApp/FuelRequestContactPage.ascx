<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FuelRequestContactPage.ascx.cs" Inherits="FuelsManager.DispatchWebApp.FuelRequestContactPage" %>
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
                <FMControls:FMLabel ID="ContactLabel" runat="server" CssClass="formfieldtitle" Text="Contact:" Width="75px" />
            </td>
            <td colspan="5">
                <FMControls:FMTextBox ID="ContactTextBox" ToolTip="Contact" runat="server" CssClass="formfield" Width="523px" MaxLength="50" />
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="AddressLabel" runat="server" CssClass="formfieldtitle" Text="Address:" Width="75px" />
            </td>
            <td colspan="5">
                <FMControls:FMTextBox ID="AddressTextBox" ToolTip="Address" runat="server" CssClass="formfield" Width="523px" />
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="CityLabel" runat="server" CssClass="formfieldtitle" Text="City:" Width="75px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="CityTextBox" ToolTip="City" runat="server" CssClass="formfield" Width="125px" />
            </td>
            <td>
                <FMControls:FMLabel ID="StateLabel" runat="server" CssClass="formfieldtitle" Text="State:" Width="50px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="StateTextBox" ToolTip="State" runat="server" CssClass="formfield" Width="125px" />
            </td>
            <td>
                <FMControls:FMLabel ID="ZipLabel" runat="server" CssClass="formfieldtitle" Text="Zip:" Width="50px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="ZipTextBox" ToolTip="Zip" runat="server" CssClass="formfield" Width="135px" />
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="PhoneLabel" runat="server" CssClass="formfieldtitle" Text="Phone:" Width="75px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="PhoneTextBox" ToolTip="Phone" runat="server" CssClass="formfield" Width="125px" MaxLength="50"/>
            </td>
            <td>
                <FMControls:FMLabel ID="FaxLabel" runat="server" CssClass="formfieldtitle" Text="Fax:" Width="50px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="FaxTextBox" ToolTip="Fax" runat="server" CssClass="formfield" Width="125px" />
            </td>
            <td>
                <FMControls:FMLabel ID="EmailLabel" runat="server" CssClass="formfieldtitle" Text="Email:" Width="50px" />
            </td>
            <td>
                <FMControls:FMTextBox ID="EmailTextBox" ToolTip="Email" runat="server" CssClass="formfield" Width="135px" />
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="MemoLabel" runat="server" CssClass="formfieldtitle" Text="Memo:" Width="75px" />
            </td>
            <td colspan="5">
                <FMControls:FMTextBox ID="MemoTextBox" ToolTip="Memo" TextMode="MultiLine" runat="server" CssClass="formfield" MaxLength="1000" Width="525px" Height="100px" />
            </td>
        </tr>
    </table>
</body>
</html>
