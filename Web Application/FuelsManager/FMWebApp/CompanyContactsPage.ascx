<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanyContactsPage.ascx.cs" Inherits="FMWebApp.CompanyContactsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	    <style type="text/css">
            .style1
            {
                width: 80px;
			 	height: 10px;
            }
            .style2
            {
                width: 350px;
			 	height: 10px;
            }
            .style4
            {
                width: 570px;
			 	height: 10px;
            }
        </style>
	</HEAD>
	<body>
	    <table style="Z-INDEX: 103; width:66%; LEFT: 5px; POSITION: absolute; TOP: 5px; height: 310px;">
            <tr>
                <td class="style1">
                <span style="width: 90px">
                    <FMCONTROLS:FMLABEL id="Contact1Label" CssClass="ehsubhead" runat="server" BackColor="Transparent" 
                    Width="100px">Contact 1</FMCONTROLS:FMLABEL>
                    </span>
                </td>
                <td class="style2">
                    &nbsp;</td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                <span style="width: 90px">
                    <FMCONTROLS:FMLABEL id="Contact2Label" CssClass="ehsubhead" runat="server" BackColor="Transparent" 
                    Width="100px">Contact 2</FMCONTROLS:FMLABEL>
                    </span>
                </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1NameLabel" AssociatedControlID="Contact1NameTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Name:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1NameTextBox" tabIndex="1" Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2NameLabel" AssociatedControlID="Contact2NameTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Name:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2NameTextBox" tabIndex="12" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1Address1Label" AssociatedControlID="Contact1Address1TextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Address:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1Address1TextBox" tabIndex="2" Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2Address1Label" AssociatedControlID="Contact2Address1TextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Address:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2Address1TextBox" tabIndex="13" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    &nbsp;</td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1Address2TextBox" ToolTip="Contact 1 Address 2" tabIndex="3" Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    &nbsp;</td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2Address2TextBox" ToolTip="Contact 2 Address 2" tabIndex="14" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1CityLabel" AssociatedControlID="Contact1CityTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">City:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1CityTextBox" tabIndex="4" Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="60"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2CityLabel" AssociatedControlID="Contact2CityTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">City:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2CityTextBox" tabIndex="15" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="60"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1StateLabel" AssociatedControlID="Contact1StateTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">State:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1StateTextBox" tabIndex="5" Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="20"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2StateLabel" AssociatedControlID="Contact2StateTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">State:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2StateTextBox" tabIndex="16" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="20"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
             <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1ZipLabel" AssociatedControlID="Contact1ZipTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Zip:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1ZipTextBox" tabIndex="6" Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="11"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2ZipLabel" AssociatedControlID="Contact2ZipTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Zip:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2ZipTextBox" tabIndex="17" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="11"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1CountryLabel" AssociatedControlID="Contact1CountryTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Country:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1CountryTextBox" tabIndex="7" Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2CountryLabel" AssociatedControlID="Contact2CountryTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Country:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2CountryTextBox" tabIndex="18" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1PhoneOfficeLabel" AssociatedControlID="Contact1PhoneOfficeTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Phone (office):</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1PhoneOfficeTextBox" tabIndex="8" Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="20"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2PhoneOfficeLabel" AssociatedControlID="Contact2PhoneOfficeTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Phone (office):</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2PhoneOfficeTextBox" tabIndex="19" 
                        Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="20"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1PhoneMobileLabel" AssociatedControlID="Contact1PhoneMobileTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Phone (mobile):</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1PhoneMobileTextBox" tabIndex="9" 
                        Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="20"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2PhoneMobileLabel" AssociatedControlID="Contact2PhoneMobileTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Phone (mobile):</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2PhoneMobileTextBox" tabIndex="20" 
                        Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="20"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1FaxLabel" AssociatedControlID="Contact1FaxTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Fax:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1FaxTextBox" tabIndex="10" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="20"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2FaxLabel" AssociatedControlID="Contact2FaxTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Fax:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2FaxTextBox" tabIndex="21" Width="168px" 
                        CssClass="formfield" runat="server" 
                    MaxLength="20"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact1EmailAddressLabel" AssociatedControlID="Contact1EmailAddressTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">E-mail Address:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <FMCONTROLS:FMTEXTBOX id="Contact1EmailAddressTextBox" tabIndex="11" 
                        Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
                <td>&nbsp&nbsp</td>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Contact2EmailAddressLabel" AssociatedControlID="Contact2EmailAddressTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">E-mail Address:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <FMCONTROLS:FMTEXTBOX id="Contact2EmailAddressTextBox" tabIndex="22" 
                        Width="168px" CssClass="formfield" runat="server" 
                    MaxLength="30"></FMCONTROLS:FMTEXTBOX>
                </td>
            </tr>
        </table>
	</body>
</HTML>