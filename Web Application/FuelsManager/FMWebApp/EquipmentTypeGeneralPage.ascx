<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EquipmentTypeGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.EquipmentTypeGeneralPage" %>
<html>
	<head>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</head>
	<body>
	<table style="width: 650px">
	    <tr>
	        <td>
	            <FMCONTROLS:FMLABEL id="EquipTypeLabel" AssociatedControlID="EquipmentTypeIDTextbox" 
			    style="Z-INDEX: 101; " runat="server"
			    BackColor="Transparent" CssClass="formfieldtitle">Equipment Type ID:</FMCONTROLS:FMLABEL>
	        </td>
	        <td>
	            <FMCONTROLS:FMLABEL id="EquipTypeIDRequiredLabel" 
                style="Z-INDEX: 102; width: 12px;" runat="server"
				BackColor="Transparent" Height="8px" ForeColor="Crimson">*</FMCONTROLS:FMLABEL>
                <asp:textbox id="EquipmentTypeIDTextbox" style="Z-INDEX: 109;" aria-required="true"
			    runat="server" CssClass="formfield" Width="264px" MaxLength="50" TabIndex="1"></asp:textbox>
	        </td>
	        <td>
                <FMControls:FMCheckBox ID="MultiCompartmentCheckBox" runat="server" style="Z-INDEX: 101; width: 200px" 
                BackColor="Transparent" CssClass="formfieldtitle" Text="Multi-Compartment"  TextAlign="Left"
                TabIndex="27"/>   
	        </td>
	    </tr>
	    <tr>
	        <td>
                <FMCONTROLS:FMLABEL id="DescriptionLabel" AssociatedControlID="DescriptionTextbox" 
                style="Z-INDEX: 104;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Description:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <asp:textbox id="DescriptionTextbox" 
                style="Z-INDEX: 110; width: 264px;" runat="server"
                CssClass="formfield" MaxLength="50" TabIndex="2"></asp:textbox>
	        </td>
	        <td></td>
	    </tr>
	    <tr>
	        <td>
                <FMCONTROLS:FMLABEL id="TypeLabel" AssociatedControlID="AttributeDropDownList" 
                style="Z-INDEX: 101; width: 140px;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Type:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <FMControls:FMDropDownList ID="AttributeDropDownList" runat="server" 
                    style="Z-INDEX: 101; width: 200px; " 
                    BackColor="Transparent" CssClass="formfield" TabIndex="3" AutoPostBack="True" 
                    onselectedindexchanged="AttributeDropDownList_SelectedIndexChanged">
                </FMControls:FMDropDownList> 
	        </td>
	        <td></td>
	    </tr>
	    <tr>
	        <td>
	            <FMCONTROLS:FMLABEL id="IssuePointLabel" AssociatedControlID="IssptTextbox"
			    style="Z-INDEX: 105;" runat="server"
			    BackColor="Transparent" CssClass="formfieldtitle">Issue Point:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <asp:textbox id="IssptTextbox" style="Z-INDEX: 109; "
                runat="server" CssClass="formfield" Width="150px" MaxLength="20" TabIndex="5"></asp:textbox>
	        </td>
	        <td></td>
	    </tr>
	    <tr>
	        <td>
                <FMCONTROLS:FMLABEL id="SafeFillLabel" AssociatedControlID="SafeFillTextbox" 
                style="Z-INDEX: 106; " runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Safe Fill:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <asp:textbox id="SafeFillTextbox" style="Z-INDEX: 109; width: 70px;"
                runat="server" CssClass="formfield" MaxLength="64" TabIndex="6"></asp:textbox>	
	        </td>
	        <td></td>
	    </tr>
	    <tr>
	        <td>
                <FMCONTROLS:FMLABEL id="CapacityLabel" AssociatedControlID="CapacityTextbox" 
                style="Z-INDEX: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Capacity:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <asp:textbox id="CapacityTextbox" runat="server" CssClass="formfield"  
                style="Z-INDEX: 109; width: 70px;" 
                TabIndex="6"></asp:textbox>
	        </td>
	        <td></td>
	    </tr>
	    <tr>
	        <td>
                <FMCONTROLS:FMLABEL id="ModelLabel" AssociatedControlID="ModelTextbox"
                style="Z-INDEX: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Model:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <asp:textbox id="ModelTextbox" style="Z-INDEX: 109; width: 150px;"
                runat="server" CssClass="formfield" MaxLength="20" TabIndex="7"></asp:textbox>	
	        </td>
	        <td></td>
	    </tr>
	    <tr>
	        <td>
                <FMCONTROLS:FMLABEL id="MakeLabel" AssociatedControlID="MakeTextbox" 
                style="Z-INDEX: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Make:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <asp:textbox id="MakeTextbox" style="Z-INDEX: 109; width: 150px;"
                runat="server" CssClass="formfield" MaxLength="32" TabIndex="8"></asp:textbox>	
	        </td>
	        <td></td>
	    </tr>
	    <tr>
	        <td>
                <FMCONTROLS:FMLABEL id="YearLabel" AssociatedControlID="YearTextbox" 
                style="Z-INDEX: 106;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Year:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <asp:textbox id="YearTextbox" style="Z-INDEX: 109; width: 70px;"
                runat="server" CssClass="formfield" MaxLength="4" TabIndex="10"></asp:textbox>	
	        </td>
	        <td></td>
	    </tr>
	    <tr>
	        <td>
                <FMCONTROLS:FMLABEL id="CompanyRoleLabel" AssociatedControlID="CompanyRoleDropDownList"
                style="Z-INDEX: 101; width: 140px;" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Company Role Constraint:</FMCONTROLS:FMLABEL>
	        </td>
	        <td style="padding-left: 9px;">
                <FMControls:FMDropDownList ID="CompanyRoleDropDownList" runat="server" 
                style="Z-INDEX: 101; width: 200px;" 
                BackColor="Transparent" CssClass="formfield" TabIndex="11">
                </FMControls:FMDropDownList> 
	        </td>
	        <td></td>
	    </tr>
	</table>      
	</body>
</html>
