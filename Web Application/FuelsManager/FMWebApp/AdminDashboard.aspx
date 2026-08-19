<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="FMWebApp.AdminDashboard" %>
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
	</head>
	<body tabindex="-1" role="application">
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
		<form id="FMAdminDashboard" method="post" runat="server">
			<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
			<div id="pageContent" style="position:absolute">
				<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
				<FMControls:FMLabel id="EnterpriseQueueLabel" style="left: 8px; POSITION: absolute; top: 8px" runat="server"
					BackColor="Transparent" CssClass="headline">Administrator&nbsp;Dashboard</FMControls:FMLabel>
				<div style="position: absolute; left: 24px; top: 20px; display:table">
					<div style="display:table-row">
						<div style="display:table-cell">
							<div style="display:table">
								<div style="display:table-row">
									<div style="display:table-cell;vertical-align:top">
										<table id="NodeHealthSummaryTable" style="width: 100%; height: 150px; vertical-align:top" role="presentation" aria-label="layout">
											<tr>
												<td>
													<FMControls:FMPageSizeDropDown ID="NodeHealthPageSizeDropDown" ToolTip="Page size" runat="server" Visible="false"/>
												</td>
											</tr>  
											<tr>
												<td>
													<asp:LinkButton ID="NodeHealthListLink" runat="server" CssClass="paratextbold" OnClick="NodeHealthListLinkOnClick">Site Health</asp:LinkButton>
													<FMControls:FMGridView id="NodeHealthGrid" runat="server" DataKeyNames="SyncSessionGuid" OnRowDataBound="NodeHealthRowDataBound"
															BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="Node Name"
															GridLines="Vertical" Width="1000px" BorderWidth="1px" AllowSorting="True" OnPageIndexChanging="NodeHealthGridPageIndexChanged"
															BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="1" CssClass="tabletext"
															tabIndex="5" ShowFooter="false" FixedHeaders="true" Height="300px" EnableViewState="true" aria-label="Node Health">
														<PagerStyle CssClass="pgr"></PagerStyle>
														<EditRowStyle BackColor="White" BorderStyle="Solid" />
														<SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
														<AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
														<Columns>
															<asp:BoundField HeaderText="Sync Session Guid" Visible="false" Datafield="SyncSessionGuid"  />
															<asp:BoundField HeaderText="Site Name" Visible="true" Datafield="SiteName" SortExpression="SiteName" />
															<asp:BoundField HeaderText="Node Name" Visible="true" Datafield="NodeName" SortExpression="NodeName"/>
															<asp:BoundField HeaderText="Site ID" Visible="true" Datafield="SiteID" SortExpression="siteID"/>
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
									<div style="display:table-cell; vertical-align:top">
               
										<table id="NodeHealthTotalsTable" style="width: 200px; height: 150px" role="presentation" aria-label="layout">
											<tr>
												<td>
													<FMControls:FMPageSizeDropDown ID="NodeHealthTotalsPageSizeDropDown" ToolTip="Page size" runat="server" Visible="false"/>
												</td>
											</tr>
											<tr>
												<td>
												  <asp:LinkButton ID="LinkButton1" runat="server" CssClass="paratextbold" OnClick="NodeHealthListLinkOnClick">Total Node Health</asp:LinkButton>
												  <FMControls:FMGridView id="NodeHealthTotalsGrid" runat="server" DataKeyNames="nodeHealthIndicator" OnRowDataBound="NodeHealthTotalsRowDataBound"
															BorderStyle="None" BackColor="White" AutoGenerateColumns="False" 
															GridLines="Vertical" Width="200px" BorderWidth="1px" AllowSorting="True" OnPageIndexChanging="NodeHealthTotalsGridPageIndexChanged"
															BorderColor="White" CellPadding="3" AllowPaging="False" PageSize="3" CssClass="tabletext" RowHeaderColumn="Node<br/>Health"
															tabIndex="5" ShowFooter="false" ShowFooterWhenEmpty="true" FixedHeaders="true" Height="300px" EnableViewState="true" aria-label="Node Health Total">
														<PagerStyle CssClass="pgr"></PagerStyle>
														<EditRowStyle BackColor="White" BorderStyle="Solid" />
														<SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
														<AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
														<Columns>
															<asp:BoundField HeaderText="Node<br/>Health" Visible="true" Datafield="nodeHealthIndicator" HtmlEncode="False"/>
															<asp:BoundField HeaderText="Total" Visible="true" Datafield="total"  HtmlEncode="False"/>
														</Columns>
													</FMControls:FMGridView>

												</td>
											</tr>
									</table>
								</div>
							</div>
						</div>
					</div>
					</div>
					<hr style="background-color:white; height: 2px; border: none"/>
					<div style="display:table-row">
						<div style="display:table-cell; border-color:black; border-style:none; border-width:thick">
							<table id="UserSessionsTable" style="z-index: 101; width: 100%; height: 10px" role="presentation" aria-label="layout">
								<tr>
									<td>
										<FMControls:FMButton ID="DeleteSelectedSessions" runat="server" CssClass="formfieldtitle" style="margin-right: 20px;" Text="Delete Sessions" onclick="DeleteSelectedSessions_Command"/>
										<FMControls:FMButton ID="SelectAllSessions" runat="server" CssClass="formfieldtitle" style="padding-left: 10px; padding-right: 10px;" Text="Select All" onclick="SelectAllSessions_Command"/>
										<FMControls:FMButton ID="DeselectAllSessions" runat="server" CssClass="formfieldtitle"  Text="Deselect All" onclick="DeselectAllSessions_Command"/>
										<FMControls:FMCheckbox ID="ExcludeActiveSessions" runat="server" CssClass="formfieldtitle"  Text="Exclude active sessions"/>
									</td>									
									<td style="float: right">
										<FMControls:FMPageSizeDropDown ID="UserSessionsPageSizeDropDown" ToolTip="Page size" runat="server" Visible="false"/>
									</td>
								</tr>
								<tr>
									<td style="width: 498px; height: 10px" colspan="2">
										<asp:LinkButton ID="UserSessionsListLink" runat="server" CssClass="paratextbold" OnClick="UserSessionsListLinkOnClick">Currently Logged In Users</asp:LinkButton>
										<FMControls:FMGridView id="UserSessionsGrid" runat="server" DataKeyNames="SessionGuid" RowHeaderColumn="Session Guid"
												BorderStyle="None" BackColor="White" AutoGenerateColumns="False" OnPageIndexChanging="UserSessionsGridPageIndexChanged"
												GridLines="Vertical" Width="1500px" BorderWidth="1px" AllowSorting="True" 
												BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="1" CssClass="tabletext"
												tabIndex="5" ShowFooter="false" ShowFooterWhenEmpty="true" FixedHeaders="true" Height="625px" EnableViewState="true" aria-label="User Sessions">
											<PagerStyle CssClass="pgr"></PagerStyle>
											<EditRowStyle BackColor="White" BorderStyle="Solid" />
											<SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
											<AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
											<Columns>
  												 <asp:TemplateField HeaderText="Select"  ItemStyle-HorizontalAlign="Center">                                                                     
   													 <ItemTemplate>
														 <asp:CheckBox ID="DeleteCheckBox" runat="server" />
													 </ItemTemplate>
													 <HeaderStyle Width="70px" />
												 </asp:TemplateField>												
												<asp:BoundField HeaderText="Session<br/>Status" Visible="true" Datafield="Status" SortExpression="Status"  HtmlEncode="False"/>
												<asp:BoundField HeaderText="Created Date" Visible="true" Datafield="CreatedDate" SortExpression="CreatedDate" DataFormatString="{0:g}"/>
												<asp:BoundField HeaderText="User ID" Visible="true" Datafield="UserID" SortExpression="UserID"/>
												<asp:BoundField HeaderText="Timeout" Visible="true" Datafield="Timeout" SortExpression="Timeout"/>
												<asp:BoundField HeaderText="Session Guid" Visible="true" Datafield="SessionGuid" SortExpression="SessionGuid"/>
												<asp:BoundField HeaderText="Login Site" Visible="true" Datafield="LoginSiteID" SortExpression="LoginSiteID"/>
												<asp:BoundField HeaderText="Current Site" Visible="true" Datafield="SiteID" SortExpression="SiteID"/>
												<asp:BoundField HeaderText="Web Server Name" Visible="true" Datafield="WebServerName" SortExpression="WebServerName"/>
												<asp:BoundField HeaderText="User Guid" Visible="true" Datafield="UserGuid" SortExpression="UserGuid"/>
												<asp:BoundField HeaderText="Synchronization Node Guid" Visible="true" Datafield="SynchronizationNodeGuid" SortExpression="SynchronizationNodeGuid"/>
												<FMControls:FMDeleteCommandField DeleteText="Delete" />

											</Columns>
										</FMControls:FMGridView>
									</td>
								</tr>
							</table>
						</div>
					</div>
				</div>
				<div>
					<asp:ScriptManager ID="oScriptManager" runat="server" />
  
				</div>
			</div>
		</form>
	</body>
</html>
