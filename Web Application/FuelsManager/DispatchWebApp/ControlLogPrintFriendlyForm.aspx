<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControlLogPrintFriendlyForm.aspx.cs" Inherits="FuelsManager.DispatchWebApp.ControlLogPrintFriendlyForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="-1" />
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
 	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
   <style type="text/css"">
        @page 
        {	
	        size: 8.5in 11in;
        }
        thead { display : table-header-group; }
        tfoot { display : table-footer-group; }
    </style>

</head>
<body bgcolor="#FFFFFF" text="#000000">
        <table id="PrinterFriendResults" runat="server">
            <thead>
                <tr id="GlobalHeaderRow" runat="server">
                    <td>
                        <asp:Literal ID="GlobalHeader" runat="server" />
                    </td>
                </tr>
                <tr id="ControllerLogTitleRow" runat="server">
                    <td style="font-weight:bold; font-size:large">
                        <asp:Literal ID="ControllerLogTitle" runat="server" />
                    </td>
                </tr>
                <tr id="LocalHeaderRow" runat="server">
                    <td>
                        <asp:Literal ID="LocalHeader" runat="server" />
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr id="MainBody" runat="server">
                    <td>
                        <form runat="server">
                           <asp:Table ID="ControllerLogPrintTable" GridLines="Both"
								HorizontalAlign="Center"
								BorderWidth="2"
								Font-Bold="True"
								Font-Names="Verdana"
								Font-Size="8pt"
								CellPadding="15"
								CellSpacing="0"
								runat="server"/>
                        </form>
                    </td>
                </tr>
                <tr>					
					<td id="AdditionalInformation" runat="server">
                    </td>
                </tr>
            </tbody>
            <tfoot>
                <tr id="LocalFooterRow" runat="server">
                    <td>
                        <asp:Literal ID="LocalFooter" runat="server" />
                    </td>
                </tr>
                <tr id="GlobalFooterRow" runat="server">
                    <td>
                        <asp:Literal ID="GlobalFooter" runat="server" />
                    </td>
                </tr>
            </tfoot>
        </table>   
</body>
</html>

