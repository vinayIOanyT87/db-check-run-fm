<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="FuelsManager.FMWebApp.Settings" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
    <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
    <form id="formSettings" runat="server" visible="True">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
    <FMControls:FMLabel ID="FMLabel1" Style="z-index: 101; left: 8px; position: absolute;
        top: 8px" runat="server" CssClass="headline" Width="500px" BackColor="Transparent">Enterprise Import/Export Settings Configuration</FMControls:FMLabel>
    <br />
    <table style="z-index: 101;" role="presentation" aria-label="layout">
        <tr>
            <td colspan="2" style="color: #FF0000">
                <asp:Label ID="LabelErrorMessage" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <asp:Panel ID="Panel1" runat="server"  Style="margin-left: 20px; margin-top: 20px; padding:15px;" BorderColor="LightSteelBlue" BorderStyle="Solid" BorderWidth="1px" Width="670px">
        <table style="z-index: 101;" role="presentation" aria-label="layout">
            <tr>
                <td>
                    <span>
                        <FMControls:FMLabel ID="FMLabel9" CssClass="headline" BackColor="Transparent" runat="server">Import</FMControls:FMLabel></span>
                </td>
            </tr>
            <tr>
                <td>
                    <span>
                        <FMControls:FMLabel ID="FMLabel5" CssClass="formfieldtitle" BackColor="Transparent"
                            runat="server">Archive Directory:
                        </FMControls:FMLabel></span>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxImportArchiveDir" Width="300" MaxLength="128" runat="server"
                        ToolTip="Directory must exist on import server." TabIndex="1"></asp:TextBox>                   
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <FMControls:FMCheckBox ID="CheckBoxLogImport" runat="server" 
                        CssClass="formfieldtitle" Text="Log Process Information For Each Record" 
                        TabIndex="2">
                    </FMControls:FMCheckBox>
                </td>
             
            </tr>
            <tr>
                <td colspan="2">
                    <FMControls:FMLabel ID="FMLabel11" CssClass="formfieldtitle" BackColor="Transparent"
                        runat="server">(Note: Warnings And Errors Are Always Logged.)</FMControls:FMLabel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel2" runat="server" Style="margin-left: 20px; padding:15px;" BorderColor="LightSteelBlue" BorderStyle="Solid" BorderWidth="1px" Width="670px">
        <table style="z-index: 101;" role="presentation" aria-label="layout">
            <tr>
                <td>
                    <span>
                        <FMControls:FMLabel ID="FMLabel10" CssClass="headline" BackColor="Transparent" runat="server">Export</FMControls:FMLabel>
                        </span>
                </td>
            </tr>
            <tr style="border-spacing: 5">
                <td>
                    <FMControls:FMLabel ID="FMLabel3" CssClass="formfieldtitle" BackColor="Transparent" AssociatedControlID="TextBoxNumAttempts"
                        runat="server">Number Of Retries:
                    </FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxNumAttempts" Width="40" MaxLength="4" runat="server" ToolTip="Number of times to try to send data."
                        TabIndex="3"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="FMLabel2" CssClass="formfieldtitle" BackColor="Transparent" AssociatedControlID="TextBoxAttemptsInMinutes"
                        runat="server">Retry Interval: 
                    </FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxAttemptsInMinutes" Width="40" MaxLength="3" runat="server"
                        ToolTip="This is the time to wait between retry intervals to send data that failed to complete."
                        TabIndex="4"></asp:TextBox>
                    &nbsp;<FMControls:FMLabel ID="FMLabel7" CssClass="formfieldtitle" BackColor="Transparent"
                        runat="server" Text ="(Min)"> 
                    </FMControls:FMLabel>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="FMLabel4" CssClass="formfieldtitle" BackColor="Transparent" AssociatedControlID="TextBoxExportArchiveDir"
                        runat="server">Archive Directory:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxExportArchiveDir" Width="300" MaxLength="128" runat="server"
                        ToolTip="Directory must exist on server. Directory is where  XML exported data will be saved" TabIndex="5"></asp:TextBox>
                 </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="FMLabel6" CssClass="formfieldtitle" BackColor="Transparent" AssociatedControlID="FMDropDownListSites"
                        runat="server">Exporting Site:</FMControls:FMLabel>
                </td>
                <td>
                    <FMControls:FMDropDownList ID="FMDropDownListSites" ToolTip = "Site from which data is to be exported." runat="server">
                    </FMControls:FMDropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="FMLabel8" CssClass="formfieldtitle" BackColor="Transparent" AssociatedControlID="TextBoxURLOfImportWebSvs"
                        runat="server">Target Web Service URL:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="TextBoxURLOfImportWebSvs" Width="425px" MaxLength="128" runat="server" ToolTip="Complete URL of the importing web service (ImportService.asmx)."
                        TabIndex="6"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <p>
        <FMControls:FMButton ID="Yes" Style="z-index: 101; left: 615px; position: absolute;
            top: 390px; width: 100px;" TabIndex="7" runat="server" CssClass="formfieldtitle"
            Text="Apply" OnClick="Yes_Click"></FMControls:FMButton>
    </p>
</form>
</body>
</html>
