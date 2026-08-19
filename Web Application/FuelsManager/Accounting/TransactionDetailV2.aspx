<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransactionDetailV2.aspx.cs" Inherits="FuelsManager.Accounting.TransactionDetailV2" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
</head>
<body ms_positioning="GridLayout" xmlns:fmcontrols="urn:http://schemas.varec.com/FMControls">
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
    <%--<link href="../Javascripts/jquery-ui-1.10.3.custom/css/ui-lightness/jquery-ui-1.10.3.custom.css" rel="stylesheet" />--%>

    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/TransactionDetail_min.js" %>"></script>

    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/autocomplete.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <%--<script type="text/javascript" language="javascript" src="../Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js"></script>--%>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/json2.js" %>"></script>

    <form id="Form1" method="post" runat="server" submitdisabledcontrols="true" onsubmit="formSubmit();">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
    </form>
    <script>
        window.transactionAliasName = "<%= this.TransactionAliasID %>";
        window.existingTransactionGuid = "<%= this.ExistingTransactionGuid %>";
        window.modifyTransaction = "<%= this.ModifyTransaction.ToString().ToLower() %>";
        window.previousUrl = "<%= this.PreviousUrl %>";
        window.extendedAddScenario = <%= this.ExtendedAddScenario.ToString().ToLower() %>;
        window.prepopulateFollowingObject = !<%= this.ExtendedAddScenario.ToString().ToLower() %> ?
            null :
            {
                'ManagerID': '<%= this.Manager %>',
                'OwnerID': '<%= this.Owner %>',
                'Product': '<%= this.Product %>',
                'InventoryDate': '<%= this.InventoryDate %>'
            }
    </script>
    <script>
        function formSubmit() {
            var element = document.activeElement;

            if (element != null && element.id != '') {
                document.cookie = " ActiveElement=" + element.id;
            }

            var updatePanelDiv = document.getElementById("UpdatePanel1");
            if (updatePanelDiv != null) {
                updatePanelDiv.disabled = true;
            }
        }
    </script>
    <app-insert-transaction></app-insert-transaction>
</body>
</html>
