<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="CompanyGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanyGeneralPage" %>
<HTML>    
	<HEAD>
	    <title>Company General Page</title>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	    <style type="text/css">
            .style1
            {
                width: 80px;
			 	height: 10px;
            }
            .style2
            {
                width: 320px;
			 	height: 10px;
            }
            .style3
            {
                width: 113px;
			 	height: 10px;
            }
            .style4
            {
                width: 570px;
			 	height: 10px;
            }
          .style5
			 {
			 	width: 80px;
			 	height: 10px;
			 }
			 .style6
			 {
			 	width: 320px;
			 	height: 10px;
			 }
			 .style7
			 {
			 	width: 113px;
			 	height: 10px;
			 }
			 .style8
			 {
			 	width: 570px;
			 	height: 10px;
			 }
            .auto-style1 {
                width: 443px;
                height: 10px;
            }
            .auto-style2 {
                width: 443px;
            }
            .auto-style4 {
                width: 100px;
                height: 10px;
            }
            .auto-style5 {
                width: 625px;
                height: 10px;
            }
            .auto-style6 {
                width: 625px;
            }
            .auto-style7 {
                width: 616px;
                height: 10px;
            }
        </style>
	</HEAD>
	<body>
	    <table style="Z-INDEX: 103; width:66%; LEFT: 5px; POSITION: absolute; TOP: 5px; height: 300px;">
            <tr>
                <td class="style1">
                <span style="width: 90px">
                    <FMCONTROLS:FMLABEL id="Label1" AssociatedControlID="IdentifierTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="80px">Company ID:</FMCONTROLS:FMLABEL><span style="COLOR: red; width: 3px">*</span>
                    </span>
                </td>
                <td class="style2">
                    <asp:textbox id="IdentifierTextbox" tabIndex="1" Width="200px" CssClass="formfield" runat="server" aria-required="true"
                    MaxLength="100"></asp:textbox>
                </td>
                <td class="style3">
                    <FMCONTROLS:FMCHECKBOX id="CreditOKCheckBox" tabIndex="17" TextAlign="Left" Text="Credit OK" Height="27px" 
                    CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMCHECKBOX>
                </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style1">
                    <FMCONTROLS:FMLABEL id="Fmlabel1" AssociatedControlID="CodeTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">Company Code:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <asp:textbox id="CodeTextbox" tabIndex="2" Width="200px" CssClass="formfield" runat="server" 
                    MaxLength="10"></asp:textbox>
                </td>
                <td class="style3">
                    <FMCONTROLS:FMLABEL id="Label4" AssociatedControlID="LastActivityTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="140px">Last Activity:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                    <asp:textbox id="LastActivityTextbox" tabIndex="18" Width="200px" CssClass="formfield" runat="server" 
                    Enabled="False" MaxLength="29"></asp:textbox>
                </td>
           </tr>
            <tr>
                <td>
                    <FMCONTROLS:FMLABEL id="CompanyIataCodeLbl" AssociatedControlID="CompanyIataCodeTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">IATA Code:</FMCONTROLS:FMLABEL>
                </td>
                <td>
                    <asp:textbox id="CompanyIataCodeTextbox" tabIndex="2" Width="200px" CssClass="formfield" runat="server" 
                    MaxLength="50"></asp:textbox>
                </td>
                <td class="style3">
                    <FMCONTROLS:FMLABEL id="EffectiveDateLbl" Width="140px" CssClass="formfieldtitle" runat="server" 
                    BackColor="Transparent">Effective Date:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                	<FMCONTROLS:FMDATE id="EffectiveDate"  FormatInfo="<%# this.DateFormat %>" tabIndex="19" Width="175px" 
                	CssClass="formfield" runat="server" MaxLength="20"></FMCONTROLS:FMDATE>
                </td>
            </tr>
            <tr>
                <td>
                    <FMCONTROLS:FMLABEL id="CompanyIcaoCodeLbl" AssociatedControlID="CompanyIcaoCodeTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                    Width="100px">ICAO Code:</FMCONTROLS:FMLABEL>
                </td>
                <td>
                    <asp:textbox id="CompanyIcaoCodeTextbox" tabIndex="2" Width="200px" CssClass="formfield" runat="server" 
                    MaxLength="50"></asp:textbox>
                </td>
                <td class="style3">
                	<FMCONTROLS:FMLABEL id="ExpirationDateLbl" Width="140px" CssClass="formfieldtitle" runat="server" 
                	BackColor="Transparent">Expiration Date:</FMCONTROLS:FMLABEL><span style="COLOR: red; width: 3px">*</span>
                </td>
                <td class="style4">
                	<FMCONTROLS:FMDATE id="ExpirationDate" runat="server" FormatInfo="<%# this.DateFormat %>" tabIndex="20" aria-required="true"
                	Width="175px" CssClass="formfield" MaxLength="20"></FMCONTROLS:FMDATE>
                </td>
            </tr>
            <tr>
                <td class="style1">
	                <FMCONTROLS:FMLABEL id="Fmlabel2" AssociatedControlID="AccountNumberTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
	                Width="60px">Account #:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                    <asp:textbox id="AccountNumberTextbox" tabIndex="3" Width="200px" CssClass="formfield" runat="server" 
                    MaxLength="10"></asp:textbox>
                </td>
                <td class="style3">
                	<FMCONTROLS:FMLABEL id="EmergencyContactLbl" AssociatedControlID="EmergencyContactTextbox" Width="140px" CssClass="formfieldtitle" runat="server" 
                	BackColor="Transparent">Emergency Contact:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                	<asp:textbox id="EmergencyContactTextbox" tabIndex="21" Width="200px" CssClass="formfield" 
                	runat="server" MaxLength="30"></asp:textbox>
                </td>
          </tr>
            <tr>
                <td class="style5">
                	<FMCONTROLS:FMLABEL id="Label5" AssociatedControlID="NameTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="60px">Name:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style6">
                	<asp:textbox id="NameTextbox" tabIndex="4" Style="Width:200px; margin-right:80px" CssClass="formfield" runat="server" 
                	MaxLength="64"></asp:textbox>
                </td>
                <td class="style3">
                	<FMCONTROLS:FMLABEL id="EmergencyPhoneLbl" AssociatedControlID="EmergencyPhoneTextbox" Width="140px" CssClass="formfieldtitle" runat="server" 
                	BackColor="Transparent">Emergency Phone:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                	<asp:textbox id="EmergencyPhoneTextbox" tabIndex="22" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="20"></asp:textbox>
                </td>
          </tr>
            <tr>
                <td class="style1">
                	<FMCONTROLS:FMLABEL id="Label13" AssociatedControlID="Address1Textbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="60px">Address:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                	<asp:textbox id="Address1Textbox" tabIndex="5" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="60"></asp:textbox>
                </td>
                <td class="auto-style1">
                	<FMCONTROLS:FMLABEL id="TaxNumberLabel" AssociatedControlID="TaxNumberTextbox" Width="140px" CssClass="formfieldtitle" runat="server" 
                	BackColor="Transparent">Tax Number (ISA06):</FMCONTROLS:FMLABEL>
                </td>
                <td class="auto-style5">
                	<asp:textbox id="TaxNumberTextbox" tabIndex="23" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="20"></asp:textbox>
                </td>
          </tr>
            <tr>
                <td class="style1">
                    &nbsp;</td>
                <td class="style2">
                	<asp:textbox id="Address2Textbox" ToolTip="Address 2" tabIndex="6" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="60"></asp:textbox>
                </td>
                <td class="style3">
                	<FMCONTROLS:FMLABEL id="EPANumberLabel" AssociatedControlID="EPANumberTextBox" CssClass="formfieldtitle" runat="server" 
                	BackColor="Transparent" Width="140px">EPA Number:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                	<asp:TextBox ID="EPANumberTextBox" tabIndex="24" Width="200px" CssClass="formfield" 
                	Runat="server" MaxLength="20"></asp:TextBox>
                </td>
          </tr>
            <tr>
                <td class="auto-style4">
                	<FMCONTROLS:FMLABEL id="CityLabel" AssociatedControlID="CityTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="60px">City:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                	<asp:textbox id="CityTextbox" tabIndex="7" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="60"></asp:textbox>
                </td>
                <td class="style3">
                	<FMCONTROLS:FMLABEL id="FEINLabel" AssociatedControlID="FEINNumberTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="140px">FEIN Number:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                	<asp:TextBox ID="FEINNumberTextBox" tabIndex="25" Width="200px" CssClass="formfield" Runat="server" 
                	MaxLength="20"></asp:TextBox>
                </td>
          </tr>
            <tr>
                <td class="style1">
                	<FMCONTROLS:FMLABEL id="Label7" AssociatedControlID="StateTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="60px">State:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                	<asp:textbox id="StateTextbox" tabIndex="8" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="20"></asp:textbox>
                </td>
                <td class="auto-style2">
                    <FMCONTROLS:FMLABEL id="FMLABEL7" AssociatedControlID="ddlConsortiumTypes" CssClass="formfieldtitle" runat="server" 
                                        BackColor="Transparent">Consortium Type:</FMCONTROLS:FMLABEL>
                </td>
                <td class="auto-style6">
                    <FMCONTROLS:FMDROPDOWNLIST id="ddlConsortiumTypes" style="Z-INDEX: 111;"
                        tabIndex="26" runat="server" CssClass="formfield" Width="204px" AutoPostBack="True"></FMCONTROLS:FMDROPDOWNLIST>
                </td>
          </tr>
            <tr>
                <td class="style1">
                	<FMCONTROLS:FMLABEL id="Label9" AssociatedControlID="ZipTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="60px" >Zip:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                	<asp:textbox id="ZipTextbox" tabIndex="9" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="11"></asp:textbox>
                </td>
                <td class="auto-style2">
                	<FMControls:FMCheckBox id="HiddenCheckBox" tabIndex="27" TextAlign="Left" Text="Hidden" 
                	Width="120px" CssClass="formfieldtitle" runat="server"></FMControls:FMCheckBox>
                </td>
                <td></td>
          </tr>
            <tr>
                <td class="style1">
                	<FMCONTROLS:FMLABEL id="CountryLabel" AssociatedControlID="CountryTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="60px">Country:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                	<asp:textbox id="CountryTextbox" tabIndex="10" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="30"></asp:textbox>
                </td>
                <td class="style3">
                	<FMCONTROLS:FMLABEL id="Fmlabel6" AssociatedControlID="LoadRackDisplayTextbox" CssClass="formfieldtitle" BackColor="Transparent" runat="server" 
                	Width="140px">Load Rack Display Text:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                	<asp:textbox id="LoadRackDisplayTextbox" tabIndex="28" CssClass="formfield" runat="server" MaxLength="30"
                	Width="200px"></asp:textbox>
                </td>
          </tr>
            <tr>
                <td class="style1">
                	<FMCONTROLS:FMLABEL id="Label11" AssociatedControlID="PhoneTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="60px">Phone:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                	<asp:textbox id="PhoneTextbox" tabIndex="11" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="20"></asp:textbox>
                </td>
                <td class="style3">
                	<FMCONTROLS:FMCHECKBOX id="LockedOutCheckBox" tabIndex="29" TextAlign="Left" Text="Locked Out" 
                	Width="120px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
                	oncheckedchanged="LockedOutCheckBoxCheckedChanged"></FMCONTROLS:FMCHECKBOX>
                </td>
          </tr>
            <tr>
                <td class="style1">
                	<FMCONTROLS:FMLABEL id="Label14" AssociatedControlID="FaxTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
                	Width="60px">Fax:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style2">
                	<asp:textbox id="FaxTextbox" tabIndex="12" Width="200px" CssClass="formfield" runat="server" 
                	MaxLength="20"></asp:textbox>
                </td>
                <td class="style3">
                	<FMCONTROLS:FMLABEL id="FMLABEL4" AssociatedControlID="LockedOutDateTextbox" CssClass="formfieldtitle" runat="server" 
                	BackColor="Transparent">Locked Out Date:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
               	    <asp:textbox id="LockedOutDateTextbox" tabIndex="30" Width="200px" CssClass="formfield" runat="server" 
               	    Enabled="False" MaxLength="20" ReadOnly="True"></asp:textbox>
                </td>
          </tr>
          <tr>
                <td colspan="2">
                     <table>
                        <tr>
                           <td>
                              <FMCONTROLS:FMLABEL id="Label3" AssociatedControlID="AssignedRolesListBox" Width="96px" CssClass="formfieldtitle" runat="server" 
	                                               BackColor="Transparent">Assigned Roles:</FMCONTROLS:FMLABEL>
                          </td>
                           <td>
                           &nbsp;
                           </td>
                           <td>
                              <FMCONTROLS:FMLABEL id="FMLABEL3" AssociatedControlID="UnassignedRolesListBox" CssClass="formfieldtitle" runat="server" 
                                                  BackColor="Transparent">Unassigned Roles:</FMCONTROLS:FMLABEL>
                           </td>
                        </tr>
                        <tr>
                           <td>
	                           <FMCONTROLS:FMLISTBOX id="AssignedRolesListBox" tabIndex="16" Height="68px" Width="134px" 
	                                        CssClass="formfield" runat="server" BackColor="White" SelectionMode="Multiple"></FMCONTROLS:FMLISTBOX>
                           </td>
                           <td valign="middle">
                              <FMCONTROLS:FMButton id="AssignRolesButton" tabIndex="14" Height="18px" ToolTip = "Assign Role(s)" 
                                          Text="<<" CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMButton>	
                              <div style="height: 10px;"></div>
                              <FMCONTROLS:FMButton id="UnassignRolesButton" tabIndex="15" Height="18px" ToolTip = "Unassign Role(s)" 
                                          Text=">>" CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMButton>	
                          </td>
                           <td>
                              <FMCONTROLS:FMLISTBOX id="UnassignedRolesListBox" tabIndex="16" Height="68px" Width="134px" 
                                           CssClass="formfield" runat="server" 
			                                  BackColor="White" SelectionMode="Multiple"></FMCONTROLS:FMLISTBOX>
                          </td>
                        </tr>
                     </table>
                </td>
                <td class="auto-style1">
                	<FMCONTROLS:FMLABEL id="FMLABEL5" AssociatedControlID="LockedOutReasonTextbox" Width="140px" CssClass="formfieldtitle" runat="server" 
                	BackColor="Transparent">Locked Out Reason:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style4">
                	<FMControls:FMTextBox id="LockedOutReasonTextbox" tabIndex="31" Height="40px" Width="200px" CssClass="formfield" 
                	runat="server" MaxLength="80" TextMode="MultiLine" />
                </td>               
          </tr>         
          <tr>
                <td class="auto-style2">
                </td>
                <td class="auto-style6">
                </td>
          </tr>      
          <tr>
                <td class="auto-style2">
                </td>
                <td class="auto-style6">
                	&nbsp;</td>
          </tr>         
          <tr>
                 <td class="auto-style1">
                </td>
              <td class="auto-style5">
                </td>
          </tr>         
          <tr>
                
          </tr>
        </table>
		
	</body>
</HTML>
