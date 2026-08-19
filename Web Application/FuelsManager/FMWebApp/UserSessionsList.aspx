<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserSessionsList.aspx.cs" Inherits="FMWebApp.UserSessionsList" %>

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
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
        <style>
            #ReturnButton {
                padding-left: 10px !important;
                padding-right: 10px !important;
            }
        </style>

     </head>
    <body role="application">
        <form id="userSessionsList" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
				<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
                <div>
                    <asp:ScriptManager ID="oScriptManager" runat="server" />
                    <FMControls:FMLabel id="UserSessionsLabel" style="left: 8px; POSITION: absolute; top: 8px; width: 300px" runat="server"
				        BackColor="Transparent" CssClass="headline">Currently Logged In Users</FMControls:FMLabel>
                    <table id="UserSessionsTable" style="z-index: 101; width: 100%; position: absolute; left: 30px; top: 40px; height: 150px" role="presentation" aria-label="layout">
                        <tr>
                            <td class="buttonCell">
                                <FMControls:FMButton ID="ReturnButton" runat="server" Text="Return to Dashboard" CssClass="formfieldtitle" 
                                    TabIndex="1" OnClick="ReturnButtonClick" />
                                &nbsp;&nbsp;
                                <FMControls:FMPageSizeDropDown ID="UserSessionsPageSizeDropDown" ToolTip="Page size" runat="server" 
                                    OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" Visible="true"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <FMControls:FMGridView id="UserSessionsGrid" runat="server" DataKeyNames="SessionGuid" RowHeaderColumn="Session Guid"
                                        BorderStyle="None" BackColor="White" AutoGenerateColumns="False" OnPageIndexChanging="UserSessionsGridPageIndexChanged"
							            GridLines="Vertical" Width="1200px" BorderWidth="1px" AllowSorting="True" 
                                        BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="1" CssClass="tabletext"
							            style="left: 1px; top: 0px" tabIndex="5" ShowFooter="false" FixedHeaders="true" EnableViewState="true" aria-label="User Sessions">
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle BackColor="White" BorderStyle="Solid" />
                                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
                                    <AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
                                    <Columns>
                                        <asp:BoundField HeaderText="Created Date" Visible="true" Datafield="CreatedDate" SortExpression="CreatedDate" DataFormatString="{0:g}"/>
                                        <asp:BoundField HeaderText="User ID" Visible="true" Datafield="UserID" SortExpression="UserID"/>
                                        <asp:BoundField HeaderText="Timeout" Visible="true" Datafield="Timeout" SortExpression="Timeout"/>
                                        <asp:BoundField HeaderText="Session Guid" Visible="true" Datafield="SessionGuid" SortExpression="CSRFToken"/>
                                        <asp:BoundField HeaderText="Login Site" Visible="true" Datafield="LoginSiteID" SortExpression="LoginSiteID"/>
                                        <asp:BoundField HeaderText="Current Site" Visible="true" Datafield="SiteID" SortExpression="SiteID"/>
                                        <asp:BoundField HeaderText="Web Server Name" Visible="true" Datafield="WebServerName" SortExpression="WebServerName"/>
                                        <asp:BoundField HeaderText="User Guid" Visible="true" Datafield="UserGuid" SortExpression="UserGuid"/>
                                        <asp:BoundField HeaderText="Synchronization Node Guid" Visible="true" Datafield="SynchronizationNodeGuid" SortExpression="SynchronizationNodeGuid"/>
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
