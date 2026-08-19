<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="TransactionAliasFieldPlacementPage.ascx.cs"
    Inherits="FuelsManager.FMWebApp.TransactionAliasFieldPlacementPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%--<% this.Page.ClientScript.RegisterStartupScript(typeof(Page),"main", "alert('hi')"); %>
<% this.Page.ClientScript.RegisterStartupScript(typeof(Page),"main", "./AngularAppBinaries/js/main.js"); %>--%>
<script>
    window.serverUrl = "<%= string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "/FMWebAPI/api") %>";
    window.currentAuthenticationToken = "<%= this.Security.Token %>";
    window.transactionAliasName = "<%= TransactionAliasID %>";
    window.pingTimeout = 10;
</script>
<app-transaction-layout-modification id="angularWebApp"></app-transaction-layout-modification>