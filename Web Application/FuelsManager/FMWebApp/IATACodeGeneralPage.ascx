<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="IATACodeGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.IATACodeGeneralPage" %>
<html>
<head>
    <title>Delivery Location General Page</title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <style type="text/css">
        .style1 {
            width: 80px;
            height: 10px;
        }

        .style2 {
            width: 320px;
            height: 10px;
        }

        .style3 {
            width: 113px;
            height: 10px;
        }

        .style4 {
            width: 570px;
            height: 10px;
        }

        .style5 {
            width: 80px;
            height: 10px;
        }

        .style6 {
            width: 320px;
            height: 10px;
        }

        .style7 {
            width: 113px;
            height: 10px;
        }

        .style8 {
            width: 570px;
            height: 10px;
        }

        .auto-style4 {
            width: 100px;
            height: 10px;
        }

        .auto-style7 {
            width: 616px;
            height: 10px;
        }
    </style>
</head>
<script type="text/javascript">
    function DisplayCalculateCoordinates()
    {
        var latTextBox = document.getElementById("tcIATACodeTabs_tpGeneralPage_IATACodeGeneralPage_LatitudeTextbox");
        var longTextBox = document.getElementById("tcIATACodeTabs_tpGeneralPage_IATACodeGeneralPage_LongitudeTextbox");
        var zoomTextbox = document.getElementById("tcIATACodeTabs_tpGeneralPage_IATACodeGeneralPage_ZoomTextbox");
        var argu = "?";

        if (latTextBox.value == null || latTextBox.value === "")
        {
            argu = argu + "latitudeStr=-9999";
        }
        else
        {
            argu = argu + "latitudeStr=" + latTextBox.value;
        }

        if (longTextBox.value == null || longTextBox.value === "")
        {
            argu = argu + "&longitudeStr=-9999";
        }
        else
        {
            argu = argu + "&longitudeStr=" + longTextBox.value;
        }

        if (zoomTextbox.value == null || zoomTextbox.value === "")
        {
            argu = argu + "&zoomStr=-9999";
        }
        else
        {
            argu = argu + "&zoomStr=" + zoomTextbox.value;
        }

        showModalDialogFrame({
            url: "../AssetTrackingArea/AssetCalculateCoordinates/CalculateCoordinates" + argu,
            width: 855,
            height: 560,
            title: "Calculate Coordinates",
            onClose: function () {
                if (this.returnValue != null)
                {
                    var latitudeValue = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
                    var longitudeValue = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);
                    var zoomValue = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[2]);

                    // -9999 means the user pressed cancel.
                    if (latitudeValue !== "-9999" && longitudeValue !== "-9999" && zoomValue !== "-9999")
                    {
                        latTextBox.value = latitudeValue;
                        longTextBox.value = longitudeValue;
                        zoomTextbox.value = zoomValue;
                    }
                }
            }
        });
    }
</script>
<body>
    <table style="width: 66%; left: 5px; position: absolute; top: 6px; height: 300px; margin-right: 0px;">
        <tr>
            <td class="auto-style4">
                <span style="width: 90px">
                    <FMControls:FMLabel ID="IdLabel" AssociatedControlID="IdentifierTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                        Width="80px">IATA Code:</FMControls:FMLabel><span style="color: red; width: 3px">*</span>
                </span>
            </td>
            <td class="auto-style7">
                <asp:TextBox ID="IdentifierTextbox" TabIndex="1" Width="200px" CssClass="formfield" runat="server"
                    MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style4">
                <FMControls:FMLabel ID="NameLabel" AssociatedControlID="NameTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                    Width="100px">Name:</FMControls:FMLabel>
            </td>
            <td class="auto-style7">
                <asp:TextBox ID="NameTextBox" TabIndex="2" Width="112px" CssClass="formfield" runat="server"
                    MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style4">
                <FMControls:FMLabel ID="CountryLabel" AssociatedControlID="CountryTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                    Width="60px">Country:</FMControls:FMLabel>
            </td>
            <td class="auto-style7">
                <asp:TextBox ID="CountryTextbox" TabIndex="10" Width="168px" CssClass="formfield" runat="server"
                    MaxLength="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style4">
                <FMControls:FMLabel ID="TimeZoneLabel" AssociatedControlID="TimeZoneTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                    Width="60px">TimeZone:</FMControls:FMLabel>
            </td>
            <td class="auto-style7">
                <asp:TextBox ID="TimeZoneTextbox" TabIndex="10" Width="168px" CssClass="formfield" runat="server"
                    MaxLength="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style4">
                <FMControls:FMLabel ID="LatitudeLabel" AssociatedControlID="LatitudeTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                    Width="60px">Latitude:</FMControls:FMLabel>
            </td>
            <td class="auto-style7">
                <asp:TextBox ID="LatitudeTextbox" TabIndex="10" Width="168px" CssClass="formfield" runat="server"
                    MaxLength="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style4">
                <FMControls:FMLabel ID="LongitudeLabel" AssociatedControlID="LongitudeTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                    Width="60px">Longitude:</FMControls:FMLabel>
            </td>
            <td class="auto-style7">
                <asp:TextBox ID="LongitudeTextbox" TabIndex="10" Width="168px" CssClass="formfield" runat="server"
                    MaxLength="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style4">
                <FMControls:FMLabel ID="ZoomLabel" AssociatedControlID="ZoomTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                    Width="60px">Zoom:</FMControls:FMLabel>
            </td>
            <td class="auto-style7">
                <asp:TextBox ID="ZoomTextbox" TabIndex="10" Width="168px" CssClass="formfield" runat="server"
                    MaxLength="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <input ID="CalculateCoordBtn" type="button" onclick="DisplayCalculateCoordinates();" class="formfieldtitle"
                       value="Calculate delivery location coordinates"
                       Style="cursor: pointer;"/>          
            </td>
            <td>&nbsp;</td>
        </tr>
    </table>

</body>
</html>
