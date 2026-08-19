<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="IntoPlaneImportWebPage.aspx.cs" Inherits="FuelsManager.Accounting.IntoPlaneImportWebPage" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <style type="text/css">
        .auto-style1 {
            width: 106px;
        }
    </style>
    <script>
        /*
        Use these functions to activate/inactivate Vcf, Temp, and Gravity text boxes
        based on whether or not there is data entered in the fields Vcf or Temp/Gravity.
        */
        function VcfChanged(txtObj) {
            if (txtObj.value === "") {
                document.getElementById(txtObj.id.replace("txtVcf", "txtTemperature")).disabled = false;
                document.getElementById(txtObj.id.replace("txtVcf", "txtGravity")).disabled = false;
            }
            else {
                document.getElementById(txtObj.id.replace("txtVcf", "txtTemperature")).disabled = true;
                document.getElementById(txtObj.id.replace("txtVcf", "txtGravity")).disabled = true;
            }
        }

        function TempChanged(txtObj) {
            if (txtObj.value === "" && document.getElementById(txtObj.id.replace("txtTemperature", "txtGravity")).value === "") {
                document.getElementById(txtObj.id.replace("txtTemperature", "txtVcf")).disabled = false;
            }
            else {
                document.getElementById(txtObj.id.replace("txtTemperature", "txtVcf")).disabled = true;
            }
        }

        function GravChanged(txtObj) {
            if (txtObj.value === "" && document.getElementById(txtObj.id.replace("txtGravity", "txtTemperature")).value === "") {
                document.getElementById(txtObj.id.replace("txtGravity", "txtVcf")).disabled = false;
            }
            else {
                document.getElementById(txtObj.id.replace("txtGravity", "txtVcf")).disabled = true;
            }
        }

        // Prevent action if file dropped outside dropzone
        window.addEventListener("dragenter", function (e) {
            if (e.target.id != "fileUpload") {
                e.preventDefault();
                e.dataTransfer.effectAllowed = "none";
                e.dataTransfer.dropEffect = "none";
            }
        }, false);

        window.addEventListener("dragover", function (e) {
            if (e.target.id != "fileUpload") {
                e.preventDefault();
                e.dataTransfer.effectAllowed = "none";
                e.dataTransfer.dropEffect = "none";
            }
        });
        window.addEventListener("drop", function (e) {
            if (e.target.id != "fileUpload") {
                e = e || event;
                e.preventDefault();
                e.dataTransfer.effectAllowed = "none";
                e.dataTransfer.dropEffect = "none";
            }
        }, false);

        $(document).ready(function () {
            $('#fileUpload').change(function () {
                var ext = this.value.match(/\.([^\.]+)$/)[1];
                switch (ext) {
                    case 'csv':
                        this.form.submit();
                        break;
                    default:
                        $('#FMTBFilePath').html("File type must be .csv");
                        $('#results').html("");
                        this.value = '';
                        return;
                }

                var path = $(this).val();
                if (path != '' && path != null) {
                    var q = path.substring(path.lastIndexOf('\\') + 1);
                    $('#FMTBFilePath').html(q);
                }
            });
        });
    </script>
</head>
<body ms_positioning="GridLayout">
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Content/TransactionImport.css" %>" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Scripts/jquery-2.2.1.min.js" %>" type="text/javascript"></script>
    <form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server" />
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" class="interplane-div">
            <fmcontrols:FMLabel ID="titleLable" runat="server" 
                CssClass="headline" BackColor="Transparent">IntoPlane Import</fmcontrols:FMLabel>

            <div class="transaction-import-frame">
                <div>
                    Instructions:
            <ul>
                <li>Browse to file to be imported.</li>
                <li>Filter import by Start Date, End Date, and Manager.</li>
                <li>Set either the VCF or Temperature/Gravity for each product if you would like to override the values included in the import file.</li>
                <li>Click the "Import" button to process the file.  This process may take several minutes.</li>
            </ul>
                </div>
                <div class="drag-drop-div-area">
                    <div class="fileContainer">
                        <asp:Image ImageUrl="~/Content/icons/gray-upload-icon.png" runat="server" ID="uploadIcon" />
                        <p class="drag-drop-label">To begin, drop your file here or click</p>
                        <button type="button" id="browseForFileButton" class="drag-drop-upload-button" onclick="getElementById('fileUpload').click()">Upload File to Import</button>
                        <asp:FileUpload ID="fileUpload" accept=".csv, text/csv" AllowMultiple="false" ClientIDMode="Static" onchange="" runat="server" />
                    </div>
                </div>
                <div class="processing-div">
                    <div class="processing-top-div">
                        <fmcontrols:FMLabel ID="FMTBFilePath" class="processing-file-name-text" Text="No file selected" Visible="true" runat="server"></fmcontrols:FMLabel>
                        <div id="progressStatus" style="display: none">
                            <asp:Image ImageUrl="~/Content/icons/progress-bar.gif" runat="server" ID="animation" />
                            <div>Importing data...</div>
                        </div>
                    </div>
                    <div id="results" runat="server"></div>
                </div>
                <div class="filter-div">
                    <p>Filter Transaction(s) to Import:</p>
                    <div>
                        <fmcontrols:FMLabel ID="fmlStartDate" runat="server" AssociatedControlID="StartDate">Start Date: </fmcontrols:FMLabel>
                        <fmcontrols:FMDate ID="StartDate" FormatInfo="<%# _dateFormat %>" TabIndex="1" Width="200px" CssClass="formfield" runat="server" MaxLength="20"></fmcontrols:FMDate>
                        <fmcontrols:FMLabel ID="fmlEndDate" runat="server" AssociatedControlID="EndDate">End Date: </fmcontrols:FMLabel>
                        <fmcontrols:FMDate ID="EndDate" FormatInfo="<%# _dateFormat %>" TabIndex="1" Width="200px" CssClass="formfield" runat="server" MaxLength="20"></fmcontrols:FMDate>
                        <fmcontrols:FMLabel ID="fmlmanagerlist" runat="server" AssociatedControlID="managerList$managerList_TextBox">Manager: </fmcontrols:FMLabel>
                        <fmcontrols:FMComboBox ID="managerList" ToolTip="Select Manager" runat="server"></fmcontrols:FMComboBox>
                    </div>
                    <div>

                    </div>
                </div>

                <div class="volume-correction-div">
                    <p>Desired VCF or Temperature/Gravity (Temperature Units: °C, Gravity Units: kg/m<sup>3</sup>)</p>
                    <fmcontrols:FMDataGridFixed ID="ProductDataGrid" runat="server" BackColor="White" CssClass="tabletext" Width="1000px"
                        BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="False" AllowPaging="False"
                        BorderColor="White" CellPadding="3" Height="500px">
                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                        <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                        <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                        <Columns>
                            <asp:TemplateColumn HeaderText="Product">
                                <HeaderStyle Width="55px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <fmcontrols:FMTextBox ID="txtProduct" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Product") %>' ReadOnly="true"></fmcontrols:FMTextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="VCF">
                                <HeaderStyle Width="55px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <fmcontrols:FMTextBox ID="txtVcf" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Vcf") %>' onkeyup="javascript:VcfChanged(this)"></fmcontrols:FMTextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Temperature">
                                <HeaderStyle Width="55px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <fmcontrols:FMTextBox ID="txtTemperature" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Temp") %>' onkeyup="javascript:TempChanged(this)"></fmcontrols:FMTextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Gravity">
                                <HeaderStyle Width="55px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <fmcontrols:FMTextBox ID="txtGravity" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Gravity") %>' onkeyup="javascript:GravChanged(this)"></fmcontrols:FMTextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </fmcontrols:FMDataGridFixed>
                </div>
                <div>
                    <fmcontrols:FMButton class="import-data-button" ID="ClearButton" runat="server" Text="Clear" OnClick="ClearButton_Click" Style="min-width: 100px" />
                    <fmcontrols:FMButton class="import-data-button" ID="ImportButton" runat="server" Text="Import" OnClick="UploadButton_Click" Style="min-width: 100px" />
                </div>
                <div class="results">
                    <p>
                        <fmcontrols:FMLabel ID="FMLResults" runat="server" AssociatedControlID="txtResults">Results: </fmcontrols:FMLabel>
                    </p>
                    <fmcontrols:FMLabel ID="txtResults" runat="server" Width="990px"></fmcontrols:FMLabel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
