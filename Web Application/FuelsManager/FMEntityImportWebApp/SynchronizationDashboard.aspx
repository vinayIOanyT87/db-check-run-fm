<%@ Page Language="C#" CodeBehind="SynchronizationDashboard.aspx.cs" AutoEventWireup="true" Inherits="FuelsManager.FMEntityImportWebApp.SynchronizationDashboard" %>

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
    </head>
    <body tabindex="-1">
        <form id="SynchronizationDashboard" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <div>
                    <table id="NodeHealthSummaryTable" style="position: absolute; width: 800px; left: 24px; top: 20px; height: 150px">
                        <tr>
                            <td>
                                <FMControls:FMPageSizeDropDown ID="NodeHealthPageSizeDropDown" ToolTip="Page size" runat="server" Visible="false"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:label id="NodeHealthLabel" runat="server" CssClass="paratextbold">Site Health</asp:label>
                                <FMControls:FMDropDownList runat="server" ID="statusFilterDropDown" OnSelectedIndexChanged="StatusFilterDropDownOnSelectedIndexChanged" AutoPostBack="True">
                                    <asp:ListItem Value="0">All</asp:ListItem>
                                    <asp:ListItem Value="1">Okay</asp:ListItem>
                                    <asp:ListItem Value="2">Caution</asp:ListItem>
                                    <asp:ListItem Value="3">Critical</asp:ListItem>
                                </FMControls:FMDropDownList>
                                <FMControls:FMGridView id="NodeHealthGrid" runat="server" DataKeyNames="nodeName" OnRowDataBound="NodeHealthRowDataBound"
                                        BorderStyle="None" BackColor="White" AutoGenerateColumns="False" 
							            GridLines="Vertical" Width="800px" BorderWidth="1px" AllowSorting="True" OnPageIndexChanging="NodeHealthGridPageIndexChanged"
                                        BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="1" CssClass="tabletext"
							            style="left: 1px; top: 0px" tabIndex="5" ShowFooter="true" FixedHeaders="true" Height="300px" EnableViewState="true">
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle BackColor="White" BorderStyle="Solid" />
                                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
                                    <AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
                                    <Columns>
                                        <asp:BoundField HeaderText=" Site Name" Visible="true" Datafield="SiteName" SortExpression="SiteName"/>
                                        <asp:BoundField HeaderText="Node Name" Visible="true" Datafield="NodeName" SortExpression="NodeName"/>
                                        <asp:BoundField HeaderText="DoDAAC" Visible="true" Datafield="DoDAAC" SortExpression="DoDAAC"/>
                                        <asp:BoundField HeaderText="Conflicts" Visible="true" Datafield="conflicts" SortExpression="Conflicts"/>
                                        <asp:BoundField HeaderText="Last Sync" Visible="true" Datafield="lastSyncDate" SortExpression="LastSyncDate"/>
                                        <asp:BoundField HeaderText="# Objects<br/>Synced" Visible="true" Datafield="syncCount" SortExpression="SyncCount" HtmlEncode="False"/>
                                        <asp:BoundField HeaderText="Time to Sync<br/>(Minutes)" Visible="true" Datafield="syncTimeMinutes" SortExpression="SyncTimeMinutes" HtmlEncode="False"/>
                                        <asp:BoundField HeaderText="Node<br/>Health" Visible="true" Datafield="nodeHealthIndicator" SortExpression="NodeHealthIndicator" HtmlEncode="False"/>
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
