<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="EnterpriseSyncSettingsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.EnterpriseSyncSettingsPage" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<table id="Table1" style="width: 445px; margin-left: 20px;" cellspacing="0" cellpadding="5" border="0">
    <tr>
        <td colspan="5" style="white-space: nowrap;">
                <FMControls:FMLabel ID="InstructionsLabel" runat="server" CssClass="notesHeader" Text="These settings should only be configured if this server will process incoming synchronization requests from remote servers." Style="left: -24px; position: relative" />
        </td>
    </tr>
    <tr>
        <td style="white-space:nowrap;">
            <FMControls:FMLabel runat="server" ID="EnableGlobalSynchronizationLabel" AssociatedControlID="EnableGlobalSynchronizationCheckBox" CssClass="formfieldtitle" Text="Allow Synchronization:" />
        </td>
        <td>
            <FMControls:FMCheckBox ID="EnableGlobalSynchronizationCheckBox" TabIndex="1" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text=""></FMControls:FMCheckBox>
        </td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="5" style="white-space: nowrap;">
            <span>
                <FMControls:FMLabel ID="FuelsManagerAuthSectionLabel" runat="server" CssClass="headline" Text="FuelsManager Authentication" Style="left: -24px; position: relative; font-size: medium" />
            </span>
            <span style="COLOR: Crimson; font-size: 12px; font-weight: bold; width: auto;">*</span>
        </td>
    </tr>
    <tr>
        <td style="white-space:nowrap;">
            <FMControls:FMLabel runat="server" ID="FuelsManagerAcceptUserIDLabel" AssociatedControlID="FuelsManagerAcceptUserIDCheckBox" CssClass="formfieldtitle" Text="Accept User ID / Password:" />
        </td>
        <td>
            <FMControls:FMCheckBox ID="FuelsManagerAcceptUserIDCheckBox" TabIndex="2" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text=""></FMControls:FMCheckBox>
        </td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td style="white-space:nowrap;">
            <FMControls:FMLabel runat="server" ID="FuelsManagerAcceptClientCertificateLabel" AssociatedControlID="FuelsManagerAcceptClientCertificateCheckBox" CssClass="formfieldtitle" Text="Accept Client Certificate:" />
        </td>
        <td>
            <FMControls:FMCheckBox ID="FuelsManagerAcceptClientCertificateCheckBox" TabIndex="3" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text=""></FMControls:FMCheckBox>
        </td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="5" style="white-space: nowrap;">
            <FMControls:FMLabel ID="MessageSecuritySectionLabel" runat="server" CssClass="headline" Text="Message Security" Style="left: -24px; position: relative; font-size: medium" />
        </td>
    </tr>
    <tr>
        <td style="white-space:nowrap;">
            <FMControls:FMLabel runat="server" ID="MessageSecurityClientSignatureRequiredLabel" AssociatedControlID="MessageSecurityClientSignatureRequiredCheckBox" CssClass="formfieldtitle" Text="Client Signature Required:" />
        </td>
        <td>
            <FMControls:FMCheckBox ID="MessageSecurityClientSignatureRequiredCheckBox" TabIndex="4" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text=""></FMControls:FMCheckBox>
        </td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td style="white-space:nowrap;">
            <FMControls:FMLabel runat="server" ID="MessageSecurityClientEncryptionRequiredLabel" AssociatedControlID="MessageSecurityClientEncryptionRequiredCheckBox" CssClass="formfieldtitle" Text="Client Encryption Required:" />
        </td>
        <td>
            <FMControls:FMCheckBox ID="MessageSecurityClientEncryptionRequiredCheckBox" TabIndex="5" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text=""></FMControls:FMCheckBox>
        </td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="5" style="white-space: nowrap;">
            <FMControls:FMLabel ID="FuelsManagerAuthInstructionLabel" Style="Z-INDEX: 103" runat="server"
                CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Requires at least one method of authentication to be selected in order to validate synchronization permissions.</FMControls:FMLabel>
        </td>
    </tr>
    <tr>
        <td colspan="5" style="white-space: nowrap;">
            <FMControls:FMLabel ID="NodeHealthSectionLabel" runat="server" CssClass="headline" Text="Node Health" Style="left: -24px; position: relative; font-size: medium" />
        </td>
    </tr>
    <tr>
	    <td>
		    <FMControls:FMLabel runat="server" ID="NodeHealthCriticalThresholdLabel" AssociatedControlID="NodeHealthCriticalThresholdHoursTextBox" CssClass="formfieldtitle" Text="Critical threshold - Node has sync conflicts or more than " />
	    </td>
	    <td>
		    <asp:TextBox ID="NodeHealthCriticalThresholdHoursTextBox" TabIndex="5" runat="server" Width="25px" CssClass="formfield"></asp:TextBox>
	    </td>
	    <td>
		    <asp:Label runat="server" ID="NodeHealthCriticalThresholdLabel2" Text="hours without sync" CssClass="formfieldtitle" ></asp:Label>
	    </td>
    </tr>
    <tr>
	    <td>
		    <FMControls:FMLabel runat="server" ID="NodeHealthCautionThresholdLabel" AssociatedControlID="NodeHealthCautionThresholdHoursTextBox" CssClass="formfieldtitle" Text="Caution threshold - Node has no sync conflicts and more than " />
	    </td>
	    <td>
		    <asp:TextBox ID="NodeHealthCautionThresholdHoursTextBox" TabIndex="5" runat="server" Width="25px" CssClass="formfield"></asp:TextBox>
	    </td>
	    <td>
		    <asp:Label runat="server" ID="NodeHealthCautionThresholdLabel2" Text="hours without sync" CssClass="formfieldtitle" ></asp:Label>
	    </td>
    </tr>
</table>
