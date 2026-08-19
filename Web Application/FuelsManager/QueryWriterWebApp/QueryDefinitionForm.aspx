<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryDefinitionForm.aspx.cs" Inherits="FuelsManager.QueryWriterWebApp.QueryDefinitionForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="QueryWriter" TagName="QueryDefinitionBasicPage" Src="QueryDefinitionBasic.ascx" %>
<%@ Register TagPrefix="QueryWriter" TagName="QueryDefinitionAdvancedPage" Src="QueryDefinitionAdvanced.ascx" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
</head>
<body>
<script type="text/javascript">
	function SetHelpKey(sender, e) {
		if (sender.innerText == "Advanced Settings")
		{
			CurrentHelpKey="QueryWriterWebApp/QueryDefinitionAdvanced.ascx";
		}
		else
		{
			CurrentHelpKey="QueryWriterWebApp/QueryDefinitionBasic.ascx";
		}
	}
</script>

    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

	    <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
		    
        <table style="z-index:110; left:8px; top: 10px; position:absolute" role="presentation" aria-label="layout" role="presentation" aria-label="layout">
            <tr>
                <td>
                    <FMControls:FMLabel ID="PageTitle" runat="server" CssClass="headline" style="z-index: 110; left:8px; top:10px; position:absolute; width:500px" Text="Query" />
                    
                    <asp:UpdatePanel ID="UP2" runat="server">
                        <ContentTemplate>
                            <table cellpadding="3" style="z-index:104; left:32px; top:21px; position:absolute; width:600px" role="presentation" aria-label="layout">
                                <tr>
                                    <td>
                                        <FMControls:FMMenuTab ID="Menu1" runat="server" MultiViewID="MultiView1" CssClass="menuItem" ToolTip="Query Tab">
                                            <Items>
                                                <asp:MenuItem Text="<div style='cursor:pointer;height:100%;width:100%' onclick='javascript:SetHelpKey(this);'>Basic Settings</div>" Value="0" ToolTip="Basic Settings"/>
                                                <asp:MenuItem Text="<div style='cursor:pointer;height:100%;width:100%' onclick='javascript:SetHelpKey(this);'>Advanced Settings</div>" Value="1" ToolTip="Advanced Options"/>
                                            </Items>
                                        </FMControls:FMMenuTab>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <asp:UpdatePanel ID="UP1" runat="server"> 
                        <ContentTemplate>
                            <div style="left:32px; top:70px; position:absolute; z-index:110">
                                <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0"> 
                                    <asp:View ID="View1" runat="server">
        				                <QueryWriter:QueryDefinitionBasicPage runat="server" ID="QueryDefinitionBasicPage1" />
                                    </asp:View>
                                    <asp:View ID="View2" runat="server"> 
        				                <QueryWriter:QueryDefinitionAdvancedPage runat="server" ID="QueryDefinitionAdvancedPage1" />
                                    </asp:View>
                                </asp:MultiView>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="Menu1" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
</form>
</body>
</html>
