<%@ Page Language="c#" CodeBehind="OfflineSynchronization.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.OfflineSynchronization" %>

<%@ Register TagPrefix="FMWebApp" TagName="OfflineSynchronizationExportPage" Src="OfflineSynchronizationExportPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="OfflineSynchronizationImportPage" Src="OfflineSynchronizationImportPage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<body tabindex="-1" ms_positioning="GridLayout">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    <form id="OfflineSynchronization" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:ScriptManager ID="ScriptManager" runat="server" />
            <asp:Image ID="Image1" Style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="labSyncSettings" Style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
                CssClass="headline" Width="720px" BackColor="Transparent">Export / Import Offline Data Synchronization</FMControls:FMLabel>
            <FMControls:FMLabel ID="PageDescriptionExport" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 800px; POSITION: absolute; TOP: 40px;" runat="server" CssClass="notesHeader" >
                This page provides a method for manually performing an offline synchronization between this server and an enterprise server.  An offline synchronization is completed when changes
                for this site have been uploaded to the enterprise server and enterprise changes for this site have been downloaded and imported into this server.
                An offline synchronization export file will be generated which is uploaded to the Enterprise synchronization server.
            </FMControls:FMLabel>
            <FMControls:FMTabContainer ID="tcOfflineSyncTabs" runat="server" ActiveTabIndex="0" TabWidth="80px" Style="z-index: 104; position: absolute; top: 100px; left: 32px; width: 700px; height: 555px">
                <ajaxToolkit:TabPanel runat="server" HeaderText="Export To Enterprise" ID="tpExportToEnterprise">
                    <ContentTemplate>
                        <FMWebApp:OfflineSynchronizationExportPage runat="server" ID="OfflineSynchronizationExportPage"></FMWebApp:OfflineSynchronizationExportPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Import From Enterprise" ID="tpImportFromEnterprise">
                    <ContentTemplate>
                        <FMWebApp:OfflineSynchronizationImportPage runat="server" ID="OfflineSynchronizationImportPage"></FMWebApp:OfflineSynchronizationImportPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </FMControls:FMTabContainer>
            <table style="Z-INDEX: 104; width: 700px">
                <tr>
                    <td style="float: right">
                        <table>
                            <tr>
                                <td>
                                    <FMControls:FMLabel ID="Label10" Style="Z-INDEX: 107" runat="server"
                                        CssClass="formfieldtitle" Height="8px" ForeColor="Crimson" Width="176px">* Denotes Required Field</FMControls:FMLabel></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
