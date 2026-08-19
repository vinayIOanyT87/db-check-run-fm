<%@ Page Async="true" Language="c#" CodeBehind="UserAuthForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Wingware.JFQCUserAuthForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>

<%@ OutputCache Location="None" VaryByParam="None" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
        <title></title>
        <base target="_self">
        <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
        <meta content="C#" name="CODE_LANGUAGE">
        <meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<script type="text/javascript">
            closeWindow = function()
            {
                if (window.$dlg && window.$dlg.dialogWindow) {
                    window.$dlg.dialogWindow.dialog('close');
                }
                else {
                    setTimeout(closeIFrameWindow, 250);
                }
            }
        </script>
        <form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <asp:Timer runat="server" ID="Timer"  Interval="1000"  OnTick="Timer_Tick" />
            <asp:UpdatePanel runat="server"  ID="TimedPanel"  UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger  ControlID="Timer" EventName="Tick" />
                </Triggers>
                <ContentTemplate>
                    <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                        ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
                    <table id="Table1" style="z-index: 101; left: 8px; position: absolute; top: 10px; height: 10px" cellspacing="0" cellpadding="1" border="0">
                        <tr valign="middle">
                            <td height="36" valign="middle">
						        <FMControls:FMLabel ID="action" runat="server" CssClass="formfield" Text="Contacting Dispatch server..." Visible="true" ForeColor="Black" Font-Size="Medium" />
						        <FMControls:FMLabel ID="resultSuccess" runat="server" CssClass="formfield" Text="Connection Succeeded!" Visible="false" ForeColor="Green" Font-Size="Medium" />
						        <FMControls:FMLabel ID="resultFailed" runat="server" CssClass="formfield" Text="Connection Failed!" Visible="false" ForeColor="Red" Font-Size="Medium" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                        </tr>
				        <tr valign="middle">
					        <TD height="36" vAlign="middle">
						        <FMControls:FMLabel ID="lblErrorCaption" runat="server" CssClass="formfield" Text="Error received while attempting to connect to Dispatch server:" Visible="false" ForeColor="Red" Font-Size="Medium" />
					        </TD>
				        </tr>
				        <tr valign="middle">
					        <TD height="36" vAlign="middle">
						        <FMControls:FMLabel ID="lblError" runat="server" CssClass="formfield" Text="abc" Visible="false" ForeColor="Black" Font-Size="Medium" />
					        </TD>
				        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </form>
    </body>
</HTML>
