<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportDisplay.aspx.cs" Inherits="FuelsManager.FMReportWebMain.ReportDisplay" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
</head>
<body>

    <form id="form1" runat="server">
    <asp:Label ID="ErrorLabel" runat="server" Visible="False" Width="695px" Text = "Hello World!"></asp:Label>
            <br><br>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    
    <rsweb:ReportViewer ID="ReportViewer1" runat="server">
    </rsweb:ReportViewer>
    
    </form>
</body>
</html>
