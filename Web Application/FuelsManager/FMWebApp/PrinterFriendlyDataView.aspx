<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrinterFriendlyDataView.aspx.cs" Inherits="FuelsManager.FMWebApp.PrinterFriendlyDataView" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="-1" />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
 	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
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
            <tr id="TitleRow" runat="server">
                <td style="font-weight:bold; font-size:large">
                    <asp:Label ID="TitleLabel" runat="server" />
                </td>
            </tr>
        </thead>
        <tbody>
            <tr id="MainBody" runat="server">
                <td>
                    <form id="Form1" runat="server">
                        <FMControls:FMGroupingGridView
                            ID="ResultsGrid" 
                            runat="server" 
                            CssClass ="PrinterFriendlyGrid"
                            GroupColumnOffset="1"
                            CellPadding="3" >
                        <HeaderStyle HorizontalAlign="Left" />
                        </FMControls:FMGroupingGridView>
                    </form>
                </td>
            </tr>
        </tbody>
    </table>    
</body>
</html>
