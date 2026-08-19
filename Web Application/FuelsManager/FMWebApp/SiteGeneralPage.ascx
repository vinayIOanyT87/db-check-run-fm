<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="SiteGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.SiteGeneralPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <style type="text/css">
        .style1 {
            width: 78px;
        }

        .style2 {
            width: 177px;
        }

        .style3 {
            width: 160px;
        }
    </style>
</head>
<script type="text/javascript">
    function DisplayCalculateCoordinates()
    {
        var latTextBox = document.getElementById("tcSiteTabs_tpGeneralPage_SiteGeneralPage_SiteLatitudeTextBox");
        var longTextBox = document.getElementById("tcSiteTabs_tpGeneralPage_SiteGeneralPage_SiteLongitudeTextBox");
        var zoomTextbox = document.getElementById("tcSiteTabs_tpGeneralPage_SiteGeneralPage_SiteZoomTextBox");
        var argu = "?";

        if (latTextBox.value == null || latTextBox.value === "")
        {
            argu = argu + "latitudeStr=-9999";
        }
        else {
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
    <table style="left: 1px; position: absolute; top: 14px; width: 56%;" role="presentation" aria-label="layout">
        <tr>
            <td class="style1">
                <span style="width: 109px">
                    <FMControls:FMLabel ID="IDLabel" AssociatedControlID="Identifier" runat="server" Width="90px"
                        CssClass="formfieldtitle">Site Name:</FMControls:FMLabel>
                    <span style="color: red; width: 10px">*</span>
                </span>
            </td>
            <td class="style2">
                <asp:TextBox ID="Identifier" TabIndex="1" runat="server" Width="128px" CssClass="formfield" MaxLength="30" aria-required="true"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
            <td class="style3">
                <span style="width: 180px">
                    <FMControls:FMLabel ID="Label15" AssociatedControlID="EmergencyContactTextbox" runat="server" Width="160px" CssClass="formfieldtitle"
                        BackColor="Transparent">Emergency Contact:</FMControls:FMLabel>
                    <FMControls:FMLabel ID="RequiredFMLABEL5" Width="90px" runat="server" Style="color: red; width: 10px">*</FMControls:FMLabel>
                </span>
            </td>
            <td>
                <asp:TextBox ID="EmergencyContactTextbox" TabIndex="17" runat="server" aria-required="true"
                    Width="192px" CssClass="formfield" MaxLength="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <FMControls:FMLabel ID="Label7" AssociatedControlID="NumberTextbox" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Site|Number:</FMControls:FMLabel>
            </td>
            <td class="style2">
                <asp:TextBox ID="NumberTextbox" TabIndex="2" runat="server" Width="128px" CssClass="formfield" MaxLength="30"
                    AutoPostBack="True"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
            <td class="style3">
                <FMControls:FMLabel ID="Label16" AssociatedControlID="EmergencyPhoneTextbox" runat="server" Width="116px" CssClass="formfieldtitle"
                    BackColor="Transparent">Emergency Phone:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="EmergencyPhoneTextbox" TabIndex="18" runat="server"
                    Width="104px" CssClass="formfield"
                    MaxLength="20"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <FMControls:FMLabel ID="Label1" AssociatedControlID="SPLCCodeTextbox" runat="server" Width="100px" CssClass="formfieldtitle">SPLC Code:</FMControls:FMLabel>
            </td>
            <td class="style2">
                <asp:TextBox ID="SPLCCodeTextbox" TabIndex="3" runat="server" Width="128px" CssClass="formfield"
                    MaxLength="30"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
            <td class="style3">
                <FMControls:FMLabel ID="Fmlabel2" AssociatedControlID="TerminalControlNumberTextbox" runat="server" Width="171px"
                    CssClass="formfieldtitle">Terminal Control Number:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="TerminalControlNumberTextbox" TabIndex="19" runat="server"
                    Width="104px" CssClass="formfield"
                    MaxLength="9"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <span style="width: 109px">
                    <FMControls:FMLabel ID="Label13" AssociatedControlID="Address1Textbox" Width="92px" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent">Address:</FMControls:FMLabel>
                    <FMControls:FMLabel ID="RequiredFMLABEL0" Width="90px" runat="server" Style="color: red; width: 10px">*</FMControls:FMLabel>
                </span>
            </td>
            <td class="style2">
                <asp:TextBox ID="Address1Textbox" TabIndex="4" runat="server" Width="128px" CssClass="formfield" MaxLength="30" aria-required="true"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
            <td class="style3">
                <FMControls:FMLabel ID="FMLABEL1" AssociatedControlID="EmailAddressTextbox" runat="server" Width="116px" CssClass="formfieldtitle"
                    BackColor="Transparent">E-mail Address:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="EmailAddressTextbox" TabIndex="20" runat="server" Width="200px" CssClass="formfield"
                    MaxLength="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">&nbsp;</td>
            <td class="style2">
                <asp:TextBox ID="Address2Textbox" TabIndex="5" runat="server" Width="128px" CssClass="formfield" MaxLength="30"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
            <td class="style3">
                <FMControls:FMLabel ID="Fmlabel3" AssociatedControlID="TimeZoneDropDownList" runat="server" Width="144px" CssClass="formfieldtitle">Time Zone:</FMControls:FMLabel>

            </td>
            <td>
                <FMControls:FMDropDownList ID="TimeZoneDropDownList" TabIndex="21"
                    runat="server" Width="250px" CssClass="formfield"
                    MaxLength="6" AutoPostBack="True">
                </FMControls:FMDropDownList>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="style1">
                <span style="width: 109px">
                    <FMControls:FMLabel ID="CityLabel" AssociatedControlID="CityTextbox" Width="93px" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent">City:</FMControls:FMLabel>
                    <FMControls:FMLabel ID="RequiredFMLABEL1" Width="90px" runat="server" Style="color: red; width: 10px">*</FMControls:FMLabel>
                </span>
            </td>
            <td class="style2">
                <asp:TextBox ID="CityTextbox" TabIndex="6" runat="server" Width="128px" CssClass="formfield" MaxLength="60" aria-required="true"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
            <td class="style3">
                <FMControls:FMCheckBox ID="InhibitSiteLedgerRollupCheckbox" runat="server"
                    CssClass="formfieldtitle" TabIndex="22" Text="Inhibit Ledger Roll Up" />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="style1">
                <span style="width: 109px">
                    <FMControls:FMLabel ID="Label2" AssociatedControlID="StateTextbox" Width="92px" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent">State:</FMControls:FMLabel>
                    <FMControls:FMLabel ID="RequiredFMLABEL2" Width="90px" runat="server" Style="color: red; width: 10px">*</FMControls:FMLabel>
                </span>
            </td>
            <td class="style2">
                <asp:TextBox ID="StateTextbox" TabIndex="7" runat="server" Width="128px" CssClass="formfield" MaxLength="20" aria-required="true"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
	         <td class="style3">
		        <FMControls:FMCheckBox ID="EnterpriseCheckbox" runat="server" CssClass="formfieldtitle" TabIndex="23" Text="Enterprise" AutoPostBack="true"/>
	         </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="style1" nowrap="nowrap">
                <span style="width: 115px">
                    <FMControls:FMLabel ID="Label9" AssociatedControlID="ZipTextbox" Width="93px" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent" Height="16px">Zip/Postal Code:</FMControls:FMLabel>
                    <FMControls:FMLabel ID="RequiredFMLABEL3" Width="10px" runat="server" Style="color: red;">*</FMControls:FMLabel>
                </span>
            </td>
            <td class="style2">
                <asp:TextBox ID="ZipTextbox" TabIndex="8" runat="server" Width="80px" CssClass="formfield" MaxLength="11" aria-required="true"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
	         <td class="style3">
		        <FMControls:FMCheckBox ID="OperateTagGroupsCheckBox" runat="server" 
		                               CssClass="formfieldtitle" TabIndex="24" Text="Operate Tab Groups" />
	         </td>
	         <td valign="middle" align="left" style='white-space: nowrap'>
		        <FMControls:FMLabel ID="MaxOparteTabsAllowedLabel" AssociatedControlID="MaxOperateTabsAllowed" runat="server">Max Tabs Allowed In Operate:</FMControls:FMLabel>
              <FMControls:FMLabel ID="FMLabel4" runat="server" Style="color: red;">*</FMControls:FMLabel>
	           <asp:TextBox ID="MaxOperateTabsAllowed" TabIndex="25" runat="server" Width="40px" CssClass="formfield" MaxLength="3"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <FMControls:FMLabel ID="Label10" AssociatedControlID="CountryTextbox" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Country:</FMControls:FMLabel>
            </td>
            <td class="style2">
                <asp:TextBox ID="CountryTextbox" TabIndex="9" runat="server" Width="128px" CssClass="formfield" MaxLength="30"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
	        <td class="style3">
		        <FMControls:FMLabel ID="UserNameLabel" AssociatedControlID="UserName" runat="server" Width="68px" CssClass="formfieldtitle">Local Administrator:</FMControls:FMLabel>
	        </td>
	        <td>
		        <asp:TextBox ID="UserName" TabIndex="26" runat="server" Width="88px" CssClass="formfield" MaxLength="50"></asp:TextBox>
	        </td>
        </tr>
        <tr>
            <td class="style1">
                <span style="width: 109px">
                    <FMControls:FMLabel ID="Label11" AssociatedControlID="PhoneTextbox" Width="91px" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent" Height="16px">Phone:</FMControls:FMLabel>
                    <FMControls:FMLabel ID="RequiredFMLABEL4" Width="90px" runat="server" Style="color: red; width: 10px">*</FMControls:FMLabel>
                </span>
            </td>
            <td class="style2">
                <asp:TextBox ID="PhoneTextbox" TabIndex="10" runat="server" Width="91px" CssClass="formfield" MaxLength="20" aria-required="true"></asp:TextBox>
            </td>
            <td>&nbsp&nbsp</td>
	        <td class="style3">
		        <FMControls:FMLabel ID="PasswordLabel" AssociatedControlID="PasswordTextBox" runat="server" Width="68px" CssClass="formfieldtitle">Password:</FMControls:FMLabel>
	        </td>
	        <td>
		        <asp:TextBox ID="PasswordTextBox" TabIndex="27" runat="server" Width="88px" CssClass="formfield" TextMode="Password" MaxLength="25" autocomplete="new-password"></asp:TextBox>
	        </td>
        </tr>
        <tr>
            <td class="style1">
                <FMControls:FMLabel ID="Label14" AssociatedControlID="FaxTextbox" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Fax:</FMControls:FMLabel>
            </td>
            <td class="style2">
                <asp:TextBox ID="FaxTextbox" TabIndex="11" runat="server" Width="128px" CssClass="formfield" MaxLength="20"></asp:TextBox>
            </td>
	        <td>&nbsp&nbsp</td>
	        <td class="style3">
		        <FMControls:FMLabel ID="Label3" AssociatedControlID="ReenterPasswordTextBox" runat="server" Width="128px" CssClass="formfieldtitle">Re-enter Password:</FMControls:FMLabel>
	        </td>
	        <td>
		        <asp:TextBox ID="ReenterPasswordTextBox" TabIndex="28" runat="server" Width="88px" CssClass="formfield" TextMode="Password" MaxLength="25" autocomplete="new-password"></asp:TextBox>
	        </td>
        </tr>
        <tr>
            <td class="style1">
                <FMControls:FMLabel ID="Label17" AssociatedControlID="IATADropDownList" runat="server" CssClass="formfieldtitle"
                    BackColor="Transparent">Delivery Location:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="IATADropDownList" TabIndex="12" runat="server"
                    Width="129px" CssClass="formfield"
                    MaxLength="6" AutoPostBack="false">
                </asp:DropDownList>
            </td>
            <td>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</td>
            <td>&nbsp;</td>
	        <td>
		        <FMControls:FMCheckBox ID="EnabledCheckBox" TabIndex="29" runat="server" Width="88px" CssClass="formfieldtitle"
		                               BackColor="Transparent" Text="Enabled"></FMControls:FMCheckBox>
	        </td>
        </tr>
        <tr>
            <td class="style1">
                <FMControls:FMLabel ID="SiteLatitudeLabel" AssociatedControlID="SiteLatitudeTextBox" runat="server" CssClass="formfieldtitle"
                    BackColor="Transparent">Latitude:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="SiteLatitudeTextBox" TabIndex="13" runat="server" Width="128px" CssClass="formfield" MaxLength="20"></asp:TextBox>
            </td>
            <td>&nbsp;</td>
	        <td>&nbsp;</td>
	        <td>
		        <FMControls:FMCheckBox ID="GroupCheckBox" TabIndex="30" runat="server" Width="100px" CssClass="formfieldtitle"
		                               Text="Group" AutoPostBack="True"></FMControls:FMCheckBox>
	        </td>
        </tr>
        <tr>
            <td class="style1">
                <FMControls:FMLabel ID="SiteLongitudeLabel" AssociatedControlID="SiteLongitudeTextBox" runat="server" CssClass="formfieldtitle"
                    BackColor="Transparent">Longitude:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="SiteLongitudeTextBox" TabIndex="14" runat="server" Width="128px" CssClass="formfield" MaxLength="20"></asp:TextBox>
            </td>
            <td>&nbsp;</td>
	        <td>
	            <input ID="CalculateCoordBtn" type="button" onclick="DisplayCalculateCoordinates();" class="formfieldtitle"
                       value="Calculate site coordinates"
                       Style="cursor: pointer;" <%= (this.IsDefense ? "disabled" : "")%>/>
	        </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="SiteZoomLabel" AssociatedControlID="SiteZoomTextBox" runat="server" CssClass="formfieldtitle"
                    BackColor="Transparent">Zoom:</FMControls:FMLabel>                
            </td>
            <td>
                <asp:TextBox ID="SiteZoomTextBox" TabIndex="15" runat="server" Width="128px" CssClass="formfield" MaxLength="2"></asp:TextBox>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="style1">
                <FMControls:FMLabel ID="AdGrpMappingLabel" AssociatedControlID="AdGrpDropdownList" runat="server" CssClass="formfieldtitle"
                    BackColor="Transparent">Active Directory Site:</FMControls:FMLabel>
            </td>
            <td>
                <asp:DropDownList ID="AdGrpDropdownList" TabIndex="16" runat="server"
                    Width="129px" CssClass="formfield"
                    MaxLength="6" AutoPostBack="false">
                </asp:DropDownList>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
</body>
</html>
