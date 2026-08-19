<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryPrinterFriendlyResults.aspx.cs" Inherits="FuelsManager.QueryWriterWebApp.QueryPrinterFriendlyResults" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="-1" />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
    <style type="text/css"">
        @page 
        {	
	        size: 8.5in 11in;
        }
        thead { display : table-header-group; }
        tfoot { display : table-footer-group; }
        .centered{
            text-align:center;
        }
    </style>

</head>
<body bgcolor="#FFFFFF" text="#000000">
        <table id="PrinterFriendResults" runat="server" role="presentation" aria-label="layout">
            <thead>
                <tr id="HeaderCUIRow" runat="server" class="centered">
                    <td style="font-weight:bold; font-size:large">
                        <asp:Literal ID="HeaderCUI" runat="server" text="CUI"/>
                    </td>
                </tr>
                <tr id="GlobalHeaderRow" runat="server">
                    <td>
                        <asp:Literal ID="GlobalHeader" runat="server" />
                    </td>
                </tr>
                <tr id="QueryTitleRow" runat="server">
                    <td style="font-weight:bold; font-size:large">
                        <asp:Literal ID="QueryTitle" runat="server" />
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
                            <FMControls:FMGroupingGridView
                                ID="ResultsGrid" 
                                runat="server" 
                                CssClass ="PrinterFriendlyGrid"
                                OnPreRender="ResultsGridPreRender" 
                                GroupColumnOffset="1"
                                AutoGenerateColumns="false" 
						  aria-label="Results">
                                <Columns>
                                    <asp:TemplateField HeaderText="Line" visible="false" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="LineNumber" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Internal__LineNumber") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </FMControls:FMGroupingGridView>
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
                <tr id="FooterCUIRow" runat="server" class="centered">
                    <td style="font-weight:bold; font-size:large">
                        <asp:Literal ID="FooterCUI" runat="server" text="CUI"/>
                    </td>
                </tr>
            </tfoot>
        </table>    
</body>
</html>
