<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StandardTransactionImportInterface.aspx.cs" Inherits="FuelsManager.Accounting.Standard_Transaction_Import_Interface" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>9.2 FM - Standard Transaction Import Interface</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="C#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />

    <link rel="stylesheet" type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" />
    <link rel="stylesheet" type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Content/TransactionImport.css" %>" />

    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Scripts/jquery-2.2.1.min.js" %>" type="text/javascript"></script>
</head>
<body>
    <form id="form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server" />
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />

        <div id="pageContent" class="transaction-import-frame">
            <div class="drag-drop-div-area">
                <div class="fileContainer">
                    <asp:Image ImageUrl="~/Content/icons/gray-upload-icon.png" runat="server" ID="uploadIcon" />
                    <p class="drag-drop-label">To begin drop your files here or click</p>
                    <button type="button" id="browseForFileButton" class="drag-drop-upload-button" onclick="getElementById('fileUpload').click()">Upload Data File</button>
                    <asp:FileUpload ID="fileUpload" accept=".csv, text/csv" ClientIDMode="Static" onchange="this.form.submit()" runat="server" />
                </div>
            </div>

            <div class="processing-div">
                <div class="processing-top-div">
                    <FMControls:FMLabel ID="filePathLabel" class="processing-file-name-text" Text="No file selected" Visible="true" runat="server"></FMControls:FMLabel>
                    <div id="progressStatus" style="display: none">
                        <asp:Image ImageUrl="~/Content/icons/progress-bar.gif" runat="server" ID="animation" />
                        <div>Importing data...</div>
                    </div>
                </div>
                <div id="results" runat="server"></div>
            </div>
            <br />
            <div id="preview" runat="server" class="itemconfiguration"></div>
            <br />
            <br />
            <div class="button-group">
                <FMControls:FMButton ID="cancelFileButton" class="cancel-button" runat="server" Text="Cancel" OnClick="ClearFileButton_Click" />                
                <asp:Button ID="importFileButton" class="import-data-button" runat="server" Text="Import Data" onclick="ImportFileButton_Click" ClientIDMode="Static" />
            </div>
        </div>
    </form>

    <script>
        $(document).ready(function ()
        {
            $('#fileUpload').change(function ()
            {
                var ext = this.value.match(/\.([^\.]+)$/)[1];
                switch (ext) {
                    case 'csv':
                        break;
                    default:
                        $('#filePathLabel').html("File type must be .csv");
                        this.value = '';
                        return;
                }

                var path = $(this).val();
                if (path != '' && path != null)
                {
                    var q = path.substring(path.lastIndexOf('\\') + 1);
                    $('#filePathLabel').html(q);
                }
            });

            $("#importFileButton").click(function () {
                $("#progressStatus").html("<img id='animation' src='../Content/icons/progress-bar.gif' style='border-width:0px;' /><div>Importing data...</div>");
                $("#progressStatus").show();
            });
        }); 

    </script>
</body>
</html>