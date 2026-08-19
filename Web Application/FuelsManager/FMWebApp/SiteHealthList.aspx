<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SiteHealthList.aspx.cs" Inherits="FMWebApp.SiteHealthList" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
        <meta content="C#" name="CODE_LANGUAGE" />
        <meta content="JavaScript" name="vs_defaultClientScript" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>

        <style>
            #ReturnButton {
                padding-left: 10px !important;
                padding-right: 10px !important;
            }
        </style>
     </head>
    <body role="application">
        <script type ="text/javascript">
	    function showSessionConflict(syncSessionGuid) {
		    $.showModalDialogFrame({
			    url: '../FMEntityImportWebApp/SynchronizationSessionConflicts.aspx?SessionGuid=' + syncSessionGuid,
			    width: 1024,
			    height: 768,
			    title: "Synchronization Session Conflict",
		    });
	    }


    </script>
        <form id="siteHealthList" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
				<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
                <div>
                    <asp:ScriptManager ID="oScriptManager" runat="server" />
                    <FMControls:FMLabel id="SiteHealthLabel" style="left: 8px; POSITION: absolute; top: 8px" runat="server"
				        BackColor="Transparent" CssClass="headline">Site&nbsp;Health</FMControls:FMLabel>
                    <table id="NodeHealthSummaryTable" style="z-index: 101; width: 100%; position: absolute; left: 30px; top: 40px; height: 150px" role="presentation" aria-label="layout">
                        <tr>
                            <td class="buttonCell">
                                <FMControls:FMButton ID="ReturnButton" runat="server" Text="Return to Dashboard" CssClass="formfieldtitle"
                                    TabIndex="1" OnClick="ReturnButtonClick" />
                                &nbsp;&nbsp;
                                <FMControls:FMPageSizeDropDown ID="NodeHealthPageSizeDropDown" ToolTip="Page size" runat="server" 
                                    OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" Visible="true"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <FMControls:FMGridView id="NodeHealthGrid" runat="server" DataKeyNames="SyncSessionGuid" OnRowDataBound="NodeHealthRowDataBound" RowHeaderColumn="Site Name"
                                        BorderStyle="None" BackColor="White" AutoGenerateColumns="False" 
							            GridLines="Vertical" Width="1200px" BorderWidth="1px" AllowSorting="True" OnPageIndexChanging="NodeHealthGridPageIndexChanged"
                                        BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="1" CssClass="tabletext"
							            style="left: 1px; top: 0px" tabIndex="5" ShowFooter="true" FixedHeaders="true" EnableViewState="true" aria-label="Node Health">
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle BackColor="White" BorderStyle="Solid" />
                                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
                                    <AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
                                    <Columns>
                                        <asp:BoundField HeaderText="Sync Session Guid" Visible="false" Datafield="SyncSessionGuid"  />
                                        <asp:BoundField HeaderText="Site Name" Visible="true" Datafield="SiteName" SortExpression="SiteName"/>
                                        <asp:BoundField HeaderText="Node Name" Visible="true" Datafield="NodeName" SortExpression="NodeName"/>
                                        <asp:BoundField HeaderText="Site ID" Visible="true" Datafield="SiteID" SortExpression="SiteID"/>
                                        <asp:BoundField HeaderText="Conflicts" Visible="true" Datafield="conflicts" SortExpression="Conflicts"/>
                                        <asp:BoundField HeaderText="Last Sync" Visible="true" Datafield="lastSyncDate" SortExpression="LastSyncDate"/>
                                        <asp:BoundField HeaderText="# Objects<br/>Synced" Visible="true" Datafield="syncCount" SortExpression="SyncCount" HtmlEncode="False"/>
                                        <asp:BoundField HeaderText="Time to Sync<br/>(Minutes)" Visible="true" Datafield="syncTimeMinutes" SortExpression="SyncTimeMinutes" HtmlEncode="False"/>
                                        <asp:BoundField HeaderText="Node<br/>Health" Visible="true" Datafield="nodeHealthIndicator" SortExpression="NodeHealthIndicator" HtmlEncode="False"/>
                                        <asp:BoundField HeaderText="Notes" Visible="true" Datafield="notes" SortExpression="Notes" HtmlEncode="False"/>
                                    </Columns>
                                </FMControls:FMGridView>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
    </body>
</html>
