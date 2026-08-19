<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="ClientSyncSettingsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.ClientSyncSettingsPage" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<table id="Table1" style="width: 445px; margin-left: 20px;" cellspacing="0" cellpadding="5" border="0">
    <tr>
        <td colspan="5" style="white-space: nowrap;">
            <FMControls:FMLabel ID="InstructionsLabel" runat="server" CssClass="notesHeader" Text="These settings should only be configured if this server will synchronize with an Enterprise Server" Style="left: -24px; position: relative" />
        </td>
    </tr>
    <tr>
        <td>
            <span>
                <FMControls:FMLabel runat="server" ID="SiteOrSiteGroupIDLabel" CssClass="formfieldtitle" Style="width: auto" Text="Site / Site Group ID:" />
            </span>
            <span style="COLOR: red; width: 10px;">*</span>
        </td>
        <td>
            <asp:TextBox ID="SiteOrSiteGroupIDTextBox" TabIndex="1" runat="server" Width="128px" CssClass="formfield" MaxLength="30" aria-required="true" AutoPostBack="true"></asp:TextBox>
        </td>
        <td>&nbsp;</td>
        <td colspan="3">
            <FMControls:FMLabel ID="EnterpriseServiceSettings" runat="server" CssClass="headline" Text="Enterprise Service Settings" Style="left: -24px; position: relative; font-size: medium" />
        </td>
    </tr>
    <tr>
        <td>
            <span>
                <FMControls:FMLabel runat="server" ID="EnterpriseURLLabel" CssClass="formfieldtitle" Text="Enterprise URL:" />
            </span>
            <span style="COLOR: red; width: 10px;">*</span>
        </td>
        <td>
            <asp:TextBox ID="EnterpriseURLTextBox" TabIndex="2" runat="server" Width="275px" CssClass="formfield" aria-required="true"></asp:TextBox>
        </td>
        <td>&nbsp;</td>
        <td>
            <FMControls:FMLabel runat="server" ID="EnterpriseServiceMaxRetryAttempt" CssClass="formfieldtitle" Text="Max Retries:" />
        </td>
        <td colspan="2">
            <asp:TextBox ID="EntepriseServiceMaxRetryAttemptTextBox" TabIndex="4" runat="server" Width="50px" CssClass="formfield"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel runat="server" ID="SuspendSyncLabel" CssClass="formfieldtitle" Text="Suspend Synchronization:" AssociatedControlID="SuspendSyncCheckbox" />
        </td>
        <td>
            <FMControls:FMCheckBox ID="SuspendSyncCheckbox" TabIndex="3" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text=""></FMControls:FMCheckBox>
        </td>
        <td>&nbsp;</td>
        <td>
            <FMControls:FMLabel runat="server" ID="EnterpriseServiceRetryWaitTime" CssClass="formfieldtitle" Text="Retry Wait Time:" />
        </td>
        <td>
            <asp:TextBox ID="EnterpriseServiceRetryWaitTimeTextBox" TabIndex="5" runat="server" Width="100px" CssClass="formfield"></asp:TextBox>
        </td>
        <td>
            <asp:Label runat="server" ID="EnterpriseServiceRetryWaitTimeTip" Text="(Milliseconds)" CssClass="formfieldtitle" ></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <FMControls:FMLabel ID="ServerAuthSectionLabel" runat="server" CssClass="headline" Text="Server Authentication" Style="left: -24px; position: relative; font-size: medium" />
        </td>
        <td class="styleSpacerColumn"></td>
        <td colspan="3">
            <FMControls:FMLabel ID="FuelsManagerAuthSectionLabel" runat="server" CssClass="headline" Text="FuelsManager Authentication" Style="left: -24px; position: relative; font-size: medium" />
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel runat="server" ID="ServerAuthUserNameLabel" CssClass="formfieldtitle" Text="User Name:" />
        </td>
        <td>
            <asp:TextBox ID="ServerAuthUserNameTextBox" TabIndex="6" runat="server" Width="275px" CssClass="formfield"></asp:TextBox>
        </td>
        <td>&nbsp;</td>
        <td>
            <FMControls:FMLabel runat="server" ID="FuelsManagerAuthUserNameLabel" CssClass="formfieldtitle" Text="User Name:" />
        </td>
        <td colspan="2">
            <asp:TextBox ID="FuelsManagerAuthUserNameTextBox" TabIndex="10" runat="server" Width="275px" CssClass="formfield"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel runat="server" ID="ServerAuthPasswordLabel" CssClass="formfieldtitle" Text="Password:" />
        </td>
        <td>
            <asp:TextBox ID="ServerAuthPasswordTextBox" TabIndex="7" runat="server" Width="275px" CssClass="formfield" TextMode="Password"></asp:TextBox>
        </td>
        <td>&nbsp;</td>
        <td>
            <FMControls:FMLabel runat="server" ID="FuelsManagerAuthPasswordLabel" CssClass="formfieldtitle" Text="Password:" />
        </td>
        <td colspan="2">
            <asp:TextBox ID="FuelsManagerAuthPasswordTextBox" TabIndex="11" runat="server" Width="275px" MaxLength="25" CssClass="formfield" TextMode="Password"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel runat="server" ID="ServerAuthDomainNameLabel" CssClass="formfieldtitle" Text="Domain:" />
        </td>
        <td>
            <asp:TextBox ID="ServerAuthDomainNameTextBox" TabIndex="8" runat="server" Width="275px" CssClass="formfield"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel runat="server" ID="ServerAuthClientCertificateLabel" CssClass="formfieldtitle" Text="Client Certificate:" />
        </td>
        <td>
            <asp:TextBox ID="ServerAuthClientCertificateTextBox" TabIndex="9" runat="server" Width="275px" CssClass="formfield"></asp:TextBox>
        </td>
        <td>&nbsp;</td>
        <td>
            <FMControls:FMLabel runat="server" ID="FuelsManagerAuthClientCertificateLabel" CssClass="formfieldtitle" Text="Client Certificate:" />
        </td>
        <td colspan="2">
            <asp:TextBox ID="FuelsManagerAuthClientCertificateTextBox" TabIndex="12" runat="server" Width="275px" CssClass="formfield"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="6">
            <FMControls:FMLabel ID="MessageSecuritySectionLabel" runat="server" CssClass="headline" Text="Message Security" Style="left: -24px; position: relative; font-size: medium" />
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel runat="server" ID="MessageSecuritySigningCertificateLabel" CssClass="formfieldtitle" Text="Signing Certificate:" />
        </td>
        <td>
            <asp:TextBox ID="MessageSecuritySigningCertificateTextBox" TabIndex="13" runat="server" Width="275px" CssClass="formfield"></asp:TextBox>
        </td>
        <td>&nbsp;</td>
        <td>
            <FMControls:FMLabel runat="server" ID="MessageSecurityOfflineEncryptionCertificateLabel" CssClass="formfieldtitle" Text="Offline Encryption Certificate:" />
        </td>
        <td colspan="2">
            <asp:TextBox ID="MessageSecurityOfflineEncryptionCertificateTextBox" TabIndex="14" runat="server" Width="275px" CssClass="formfield"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>
            <FMControls:FMLabel runat="server" ID="MessageSecurityOfflineDecryptionCertificateLabel" CssClass="formfieldtitle" Text="Offline Decryption Certificate:" />
        </td>
        <td colspan="2">
            <asp:TextBox ID="MessageSecurityOfflineDecryptionCertificateTextBox" TabIndex="15" runat="server" Width="275px" CssClass="formfield"></asp:TextBox>
        </td>
    </tr>
</table>
