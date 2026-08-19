<%@ Page Language="c#" CodeBehind="SynchronizationConfigForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.SynchronizationConfigForm" %>

<%@ Register TagPrefix="FMWebApp" TagName="ClientSyncSettingsPage" Src="ClientSyncSettingsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EnterpriseSyncSettingsPage" Src="EnterpriseSyncSettingsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteSyncSettingsPage" Src="SiteSyncSettingsPage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<body tabindex="-1">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<script type="text/javascript">
	function SetHelpKey(sender, e) {
		CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
	}
</script>

    <form id="SynchronizationConfigForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
            <asp:ScriptManager ID="ScriptManager" runat="server" />     
			<FMControls:FMTabContainer ID="tcSyncConfigTabs" runat="server" ActiveTabIndex="0" 
                Style="z-index: 104; position: absolute; top: 40px; left: 32px; width: 700px; height: 450px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Client Settings" ID="tpClientSyncSettings" HelpKey="FMEntityImportWebApp/ClientSyncSettingsPage.ascx" OnClientClick='SetHelpKey'>
					<ContentTemplate>
						<FMWebApp:ClientSyncSettingsPage runat="server" ID="ClientSyncSettingsPage">
						</FMWebApp:ClientSyncSettingsPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Enterprise Settings" ID="tpEnterpriseSyncSettings" HelpKey="FMEntityImportWebApp/EnterpriseSyncSettingsPage.ascx" OnClientClick='SetHelpKey'>
					<ContentTemplate>
						<FMWebApp:EnterpriseSyncSettingsPage runat="server" ID="EnterpriseSyncSettingsPage">
						</FMWebApp:EnterpriseSyncSettingsPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Site Settings" ID="tpSiteSyncSettingsPage" HelpKey="FMEntityImportWebApp/SiteSyncSettingsPage.ascx" OnClientClick='SetHelpKey'>
					<ContentTemplate>
						<FMWebApp:SiteSyncSettingsPage runat="server" ID="SiteSyncSettingsPage">
						</FMWebApp:SiteSyncSettingsPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
			</FMControls:FMTabContainer>
			<table style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 475px; width: 500px">
				<tr><td >
                    <table>
                        <tr>
                            <td><FMControls:FMLabel ID="RepositorySectionLabel" runat="server" CssClass="headline" 
                                Text="Synchronization Data Store Identification" Style="font-size: medium"></FMControls:FMLabel></td>
                        </tr>
                    </table>
				</td></tr>
                <tr><td>
                    <table>
                        <tr>
                            <td><FMControls:FMLabel runat="server" ID="DataStoreIDLabel" CssClass="formfieldtitle" 
                                Text="Data Store ID:"></FMControls:FMLabel></td>
                            <td><asp:TextBox ID="DataStoreIDTextBox" TabIndex="16" runat="server" Width="275px" 
                                CssClass="formfield" Enabled="False" ReadOnly="True"></asp:TextBox></td>
                            <td>&nbsp;</td>
                            <td><FMControls:FMLabel runat="server" ID="DataStoreNameLabel" CssClass="formfieldtitle" 
                                Text="Data Store Name:"></FMControls:FMLabel></td>
                            <td><asp:TextBox ID="DataStoreNameTextBox" TabIndex="17" runat="server" Width="275px" 
                                CssClass="formfield"></asp:TextBox></td>
                        </tr>
                    </table>
                </td></tr>
                <tr><td style="float:right">
                    <table>
                        <tr>
                            <td><FMControls:FMLabel ID="Label10" runat="server" CssClass="formfieldtitle" Height="14px" 
                                ForeColor="Crimson" Width="176px">* Denotes Required Field</FMControls:FMLabel></td>
                            <td>&nbsp;&nbsp;</td>
                            <td><FMControls:FMButton ID="OK" TabIndex="100" 
							    runat="server" CssClass="formfieldtitle" Width="67px" Text="OK"></FMControls:FMButton></td>
                            <td>&nbsp;</td>
                            <td><FMControls:FMButton ID="Cancel" Style="margin-left: 10px;" TabIndex="101"
							    runat="server" CssClass="formfieldtitle" Width="67px" Text="Cancel"></FMControls:FMButton></td>
                        </tr>
                    </table>
                </td></tr>
			</table>
            <asp:Image ID="Image1" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
			    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="labSyncSettings" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
			    CssClass="headline" Width="720px" BackColor="Transparent">Synchronization Settings</FMControls:FMLabel>
	       <script type="text/javascript">
		      var okButton = document.getElementById("OK");
		      if (!okButton.disabled)
			     okButton.setActive();
	       </script>
        </div>
    </form>
</body>
</html>
