<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ReportLandingPage.aspx.cs"  
    Inherits="FuelsManager.FMReportWebMain.ReportLandingPage" EnableEventValidation="false"
    ValidateRequest="false"%>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
    <body role="application">
        <style>
            #pageContent{
                max-height: calc(100vh - 110px) !important;
			    overflow: auto;
            }
        </style>
        <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" >
            <asp:Label ID="ErrorLabel" runat="server" Visible="False" Width="695px"></asp:Label>
            <!-- The default AsyncPostBackTimeout is 90 seconds. In order to give reports time to run it has been increased to 600 seconds (ten minutes) -->
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="600">
            </asp:ScriptManager>            
            <rsweb:ReportViewer  ID="RptViewer" runat="server" Height="1300px" Width="800px" SizeToReportContent="True" ZoomMode="PageWidth" ShowZoomControl="false">
            </rsweb:ReportViewer>
          </div>    
        </form>       
    </body>
    <script type="text/javascript">
        // Capture the AsyncPostBackTimeout error. This must be added below the ScriptManager!
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        function EndRequestHandler(sender, args) {
            if (args.get_error() != undefined) {

                var errorMessage = args.get_error().message;
                args.set_errorHandled(true);
                alert(errorMessage);
            }
        }

    </script>
</html>
