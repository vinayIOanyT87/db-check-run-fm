<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="OfflineSynchronizationImportPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.OfflineSynchronizationImportPage" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<div style="position: absolute">
    <table id="MainTable" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 800px; POSITION: absolute; TOP: 0px;" cellspacing="1" cellpadding="1" border="0">
        <tr>
            <td style="z-index: 103; max-width: 70px; height: 24px" colspan="2">
                <FMControls:FMLabel ID="InstructionsLabel"  Style="left: -24px; position: relative;" runat="server" CssClass="notesHeader" Text="Import changes from enterprise server.  Browse to select the file to import and then click the Import button."/>
            </td>
        </tr>
        <tr>
            <td style="z-index: 103; max-width: 430px;">
                <input class="formfieldtitle" id="SyncImportFile" style="WIDTH: 430px; HEIGHT: 24px; vertical-align: middle;" type="file" name="SyncImportFile">
            </td>
            <td style="z-index: 103; width: 200px;">
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
<span id="importProgress"
style="Z-INDEX: 105; LEFT: 150px; WIDTH: 449px; POSITION: absolute; TOP: 172px; HEIGHT: 44px; visibility: hidden">Import in progress...<br>
<img alt="Importing" src="../FMWebApp/images/progress-bar-clipart5.gif" /></span>
