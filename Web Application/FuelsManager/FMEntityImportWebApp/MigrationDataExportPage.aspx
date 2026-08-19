<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MigrationDataExportPage.aspx.cs"
    Inherits="FuelsManager.FMEntityImportWebApp.MigrationDataExportPage" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
        th, td {
            padding: 1px;
        }

        table {
            border-collapse: separate;
            border-spacing: 1px;
        }

        table {
            border-collapse: collapse;
            border-spacing: 0;
        }

        .style1 {
            width: 90px;
        }

        .style2 {
            width: 362px;
        }

        .formfield {
            margin-right: 0px;
        }

        .formfield {
            margin-right: 0px;
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

        function downloadFile() {
        	var url = AddCSRFTokenToUrl("MigrationDataExportPageDownload.aspx");
            var temp = document.createElement('div');
            temp.innerHTML = '<iframe name="migrationDataExportDownload" src="' + url + '"></iframe>';
            document.body.appendChild(temp.firstChild);
        };

    </script>
    <form id="MigrationDataExportPageForm" method="post" submitdisabledcontrols="true" enctype="multipart/form-data" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute; width: 800px;">
            <asp:ScriptManager ID="ScriptManager" runat="server" />
            <asp:Image ID="FadeImage" Style="z-index: 101; left: -3px; position: absolute; top: -4px; margin-right: 0px;"
                runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"
                BackColor="Transparent"></asp:Image>
            <FMControls:FMLabel ID="DataTransmissionExportTitleLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px; width: 272px;"
                runat="server" BackColor="Transparent" CssClass="headline">Migration - Export ID and GUID Mapping Data</FMControls:FMLabel>
            <table id="Table1" style="z-index: 103; left: 8px; position: absolute; top: 38px; width: 100%; border: none;">
                <tr>
                    <td style="z-index: 103;">
                        <FMControls:FMLabel ID="Fmlabel2" runat="server" CssClass="formfield">
                <span style="font-weight: bolder;">Use this page to download ID to GUID mapping data for the selected Site as part of the base server migration.</span>
                        </FMControls:FMLabel>
                    </td>
                </tr>
            </table>
            <table id="Table2" style="z-index: 103; left: 8px; position: absolute; top: 78px; width: 100%; border: solid; border-color: ActiveBorder; border-width: 1px;">
                <tr>
                    <td class="style1">
                        <FMControls:FMLabel ID="SelectedSiteLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Selected Site:</FMControls:FMLabel>
                    </td>
                    <td class="style2">
                        <FMControls:FMLabel ID="SelectedSiteText" runat="server" BackColor="Transparent" CssClass="formfieldtitle"></FMControls:FMLabel>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMButton ID="ExportBtn" TabIndex="2" runat="server" Text="Export" Width="70px"
                            CssClass="formfieldtitle" OnClick="ExportBtnClick"></FMControls:FMButton>
                    </td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
            <table id="MigrationDataExportResults" style="z-index: 105; left: 8px; width: 738px; position: absolute; top: 175px; height: 296px; border: none;">
                <tr>
                    <td>
                        <FMControls:FMLabel ID="ResultsLabel" runat="server" BackColor="Transparent" Width="80px" CssClass="formfieldtitle"
                            Visible="False">Results</FMControls:FMLabel>
                        <asp:TextBox ID="ResultsTB" TabIndex="2" runat="server" Width="800px" CssClass="formfield"
                            Height="300px" TextMode="MultiLine" ReadOnly="True" Style="border: solid; border-color: ActiveBorder; border-width: 1px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <div id="waitDiv" style="z-index: 500; left: 305px; top: 300px; position: absolute; display: none;">
        <img src="../FMWebApp/images/pleaseWait.jpg" alt="Please Wait" />
    </div>
</body>
</html>
