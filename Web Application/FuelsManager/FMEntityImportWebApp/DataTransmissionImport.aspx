<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataTransmissionImport.aspx.cs"
    Inherits="FuelsManager.FMEntityImportWebApp.DataTransmissionImport" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="C#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" type="text/css" rel="stylesheet" />
    <style type="text/css">
        .style1
        {
            width: 397px;
        }
    </style>
</head>
<body>
    <script type="text/javascript">
        function formSubmit() {
            // Display a wait message
            var waitImage = document.getElementById("waitDiv");
            waitImage.style.display = "inline";
        }
    </script>
    <form id="DataTransmissionImportForm" method="post" enctype="multipart/form-data"
    runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
    <asp:Image ID="FadeImage" Style="z-index: 101; left: -3px; position: absolute; top: -4px;
        margin-right: 0px;" runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"
        BackColor="Transparent"></asp:Image>
    <fmcontrols:fmlabel id="DataTransmissionImportTitleLabel" style="z-index: 102; left: 8px;
        position: absolute; top: 8px" runat="server" backcolor="Transparent" width="272px"
        cssclass="headline">Data Transmission Import</fmcontrols:fmlabel>
    <table id="MainTable" style="z-index: 103; left: 8px; position: absolute; top: 48px;
        height: 400px" cellspacing="1" cellpadding="2" width="700px" border="0">
        <tr>
            <td style="z-index: 103;" colspan="2"> 
                <fmcontrols:fmlabel id="PageDescriptionImport" runat="server" cssclass="formfield">   
                This page imports data files from the base level. Browse to select the file for upload and then click the Import button. If you want to re-import a file, select the option to allow import of previously completed file. Please note that selecting this option could result in overwriting the archived copy of the file stored on the enterprise server.
            </fmcontrols:fmlabel>
            </td>
        </tr>
        <tr align="left">
            <td align="left" class="style1">
                <input id="ImportFile" type="file" style="font-weight: bold; width: 500px; height: 21px;"
                    tabindex="1" class="formfieldtitle" name="file" size="51" />
            </td>
            <td align="right">
                <fmcontrols:fmbutton id="ImportBtn" tabindex="2" runat="server" text="Import" width="70px"
                    onclick="ImportBtnClick" cssclass="formfieldtitle"></fmcontrols:fmbutton>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMCheckBox ID="AllowReprocessCheckBox" runat="server" Text="Allow import of previously completed file" CssClass="formfieldtitle" />
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
                <fmcontrols:fmlabel id="FMLabelResults" text="Results" runat="server" cssclass="formfieldtitle"></fmcontrols:fmlabel>
                &nbsp;&nbsp;&nbsp;
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
                <fmcontrols:fmtextbox id="FMTextBoxResults" height="400px" runat="server" width="99%" AutoPostBack="False" 
                    visible="true" cssclass="formfield" TextMode="MultiLine" ReadOnly="True"></fmcontrols:fmtextbox>
            </td>
        </tr>
    </table>
    </div>
</form>
    <div id="waitDiv" style="z-index: 500; left: 375px; top: 250px; position:absolute; display: none;"><img src="../FMWebApp/images/pleaseWait.jpg" alt="Please Wait" /></div>
</body>
</html>
