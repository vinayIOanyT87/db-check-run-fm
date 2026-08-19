<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MigrationDataExportPageDownload.aspx.cs"
    Inherits="FuelsManager.FMEntityImportWebApp.MigrationDataExportPageDownload" %>

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
    </style>
</head>
<body>
    <form id="MigrationDataExportPageDownload" method="post" submitdisabledcontrols="true" enctype="multipart/form-data" runat="server" >
    </form>
    <div id="waitDiv" style="z-index: 500; left: 305px; top: 300px; position: absolute; display: none;">
        <img src="../FMWebApp/images/pleaseWait.jpg" alt="Please Wait" />
    </div>
</body>
</html>
