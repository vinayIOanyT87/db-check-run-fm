<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataTransmissionExport.aspx.cs"
    Inherits="FuelsManager.FMEntityImportWebApp.DataTransmissionExport" %>

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
</head>
<body>
    <form id="DataTransmissionExportForm" method="post" enctype="multipart/form-data"
    runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
    <asp:Image ID="FadeImage" Style="z-index: 101; left: 0px; position: absolute; top: 0px"
        runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent">
    </asp:Image>
    <fmcontrols:fmlabel id="DataTransmissionExportTitleLabel" style="z-index: 102; left: 8px;
        position: absolute; top: 8px" runat="server" backcolor="Transparent" width="272px"
        cssclass="headline">Data Transmission Export</fmcontrols:fmlabel>
    <table id="MainTable" style="z-index: 103; left: 8px; position: absolute; top: 48px;
        height: 74px" cellspacing="1" cellpadding="1" width="800" border="0">
        <tr>
            <td style="z-index: 103;" colspan="3">
                <fmcontrols:fmlabel id="PageDescriptionExport" runat="server" cssclass="formfield">
              This page provides a method for manually triggering an export of companies, equipment, fuel cards, personnel, products, standing offers, equipment types, IATA codes, and transaction data for transfer from this site to an enterprise site group. Select the desired type below and click the Export button. Do not press the Cancel button on the file dialog.  If Cancel button is pressed, must then use 'Re-process from date' option.           
            </fmcontrols:fmlabel>
            </td>
        </tr>
        <tr>
            <td>
                <fmcontrols:fmradiobuttonlist id="FMRadioButtonList1" runat="server" onselectedindexchanged="FMRadioButtonList1SelectedIndexChanged"
                    autopostback="True" CssClass="formfieldtitle">
                    <asp:ListItem Selected="True">All changes since last export</asp:ListItem>
                    <asp:ListItem>Re-process date range:</asp:ListItem>
                </fmcontrols:fmradiobuttonlist>
            </td>
            <td align="left" valign="bottom">
                <fmcontrols:fmlabel id="FMLabelFromDate" runat="server" cssclass="formfieldtitle">From Date:</fmcontrols:fmlabel>
            </td>
            <td align="left" valign="bottom">
                <fmcontrols:fmlabel id="FMLabelToDate" runat="server" cssclass="formfieldtitle">To Date:</fmcontrols:fmlabel>
            </td>
        </tr>
        <tr>
			<td>
			</td>
            <td align="left" valign="bottom">
                <fmcontrols:fmdatetime id="FMDateFromDate" runat="server" />
            </td>
            <td align="left" valign="bottom">
                <fmcontrols:fmdatetime id="FMDateToDate" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <fmcontrols:fmbutton id="ExportBtn" tabindex="2" runat="server" text="Export" width="70px"
                    cssclass="formfieldtitle" onclick="ExportBtnClick"></fmcontrols:fmbutton>
            </td>
        </tr>
    </table>
    </div>
</form>
</body>
</html>
