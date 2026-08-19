<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OfflineSynchronizationEnterprise.aspx.cs" Inherits="FuelsManager.FMEntityImportWebApp.OfflineSynchronizationEnterprise" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="OfflineSynchronizationEnterprise" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:Image ID="FadeImage" Style="z-index: 101; left: 0px; position: absolute; top: 0px"
                runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
            <FMControls:FMLabel ID="OfflineSynchronizationEnterpriseLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px"
                runat="server" BackColor="Transparent" Width="272px"
                CssClass="headline">Offline Synchronization (Enterprise)</FMControls:FMLabel>
            <FMControls:FMLabel ID="InstructionsLabel" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 800px; POSITION: absolute; TOP: 40px;" runat="server" CssClass="notesHeader" Text="This page imports offline synchronization files into the Enterprise Server."/>
            <table id="MainTable" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 800px; POSITION: absolute; TOP: 80px;" cellspacing="1" cellpadding="1" border="0">
                <tr>
                    <td style="z-index: 103; max-width: 70px; height: 24px" colspan="3">
                        <FMControls:FMLabel ID="ImportFileLabel" runat="server" BackColor="Transparent" CssClass="notesHeader" >Import changes from a remote server.  Browse to select the file to import and then click the Import button.</FMControls:FMLabel>
                    </td>
                </tr>
                <tr>
                    <td style="z-index: 103; max-width: 430px;">
                        <input class="formfieldtitle" id="SyncImportFile" style="WIDTH: 430px; HEIGHT: 24px; vertical-align: middle;" type="file" name="SyncImportFile">
                    </td>
                    <td style="z-index: 103; width: 200px;" colspan="2">
                        <FMControls:FMButton ID="ImportBtn" OnClientClick="try{ResultsTB.value='';}catch(err){;} document.all['importProgress'].style.visibility='visible';" TabIndex="1" CssClass="formfieldtitle" runat="server" style="width: 96px;" Text="Import"></FMControls:FMButton>
                    </td>
                </tr>
            </table>
            <table id="SyncImportResultsTable" style="Z-INDEX: 105; LEFT: 8px; WIDTH: 800px; POSITION: absolute; TOP: 160px; HEIGHT: 272px;" cellspacing="1" cellpadding="1" border="0">
                <tr>
                    <td>
                        <FMControls:FMLabel ID="ResultsLabel" runat="server" BackColor="Transparent" Width="80px" CssClass="formfieldtitle">Results</FMControls:FMLabel></td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="ResultsTB" TabIndex="2" runat="server" Width="790px" CssClass="formfield"
                            Height="240px" TextMode="MultiLine" ReadOnly="True"></asp:TextBox></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
