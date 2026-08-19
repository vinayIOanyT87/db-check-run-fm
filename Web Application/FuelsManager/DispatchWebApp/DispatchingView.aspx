<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchingView.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.DispatchingView" %>

<%@ Import Namespace="FMBusinessObjects.DataObjects" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register src="FMDispatchHiddenFields.ascx" tagname="FMDispatchHiddenFields" tagprefix="DispatchWebApp" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>

<html xml:lang="en" lang="en">
<head>
	<title></title>
	<meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />

	<link rel="Stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/slickGrid2.0/slick.grid.css" %>" type="text/css" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/menu.css" %>" type="text/css" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/dispatch.css" %>" type="text/css" />

	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-1.7.1.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-ui-1.8.17.custom.min.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery.event.drop-2.0.min.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery.event.drag-2.0.min.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery.hotkey.js" %>" type="text/javascript"></script>

	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/slickGrid2.0/slick.core.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/slickGrid2.0/slick.formatters.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/slickGrid2.0/plugins/slick.cellrangedecorator.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/slickGrid2.0/plugins/slick.cellrangeselector.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/slickGrid2.0/plugins/slick.rowselectionmodel.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/slickGrid2.0/slick.grid.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/slickGrid2.0/slick.dataview.js" %>" type="text/javascript"></script>

	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/firebugx.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/canvasExtensions.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/dispatch.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/dispatchingView.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/fuelsManagerServiceInterface.js" %>" type="text/javascript"></script>

	<script type="text/javascript">
		$(function () { $('#RadioDialog').dialog({ autoOpen: false }); });
		$(function () { $('#StandbyRegistrationSelectionForm').dialog({ autoOpen: false }); });
		$(function () { $('#WarningDialog').dialog({ autoOpen: false }); });
		$(function () { $('#AliasSelectionDialog').dialog({ autoOpen: false }); });

		DispatchingViewLib.securityToken = '<%= Security.Token.ToString() %>';
		DispatchingViewLib.hasModifyRight = '<%= Security.HasRight(RIGHT.MODIFY_DISPATCH) %>';
		DispatchingViewLib.siteGuid = '<%= Security.SiteGuid.ToString() %>';
		DispatchingViewLib.referenceTransId = '<%= ReferenceTransId %>';
		DispatchingViewLib.jsonEquipmentGridColumnDefinitions = '<%= JsonEquipmentGridColumnDefinitions %>';
		DispatchingViewLib.jsonPersonnelGridColumnDefinitions = '<%= JsonPersonnelGridColumnDefinitions %>';
		DispatchingViewLib.jsonRequestGridColumnDefinitions = '<%= JsonRequestGridColumnDefinitions %>';

		DispatchLib.currentUserGuid = '<%= Security.UserGuid.ToString() %>';
		DispatchLib.displayCurrentTime = Boolean(parseInt('<%= DisplayCurrentTimeInt %>'));
		DispatchLib.displayMilitaryJulianDate = Boolean(parseInt('<%= DisplayMilitaryJulianDateInt %>'));
		DispatchLib.resetTabularViewSessionOperation = '<%= ResetTabularViewSessionOperation %>';
		DispatchLib.standByStatusValues = '<%= StandByStatusValues %>';
		
		FuelsManagerServiceLib.serviceAddress = '<%= DispatchRequestServiceAddress %>';
		FuelsManagerServiceLib.enableServiceRequests = Boolean(parseInt('<%= EnableServiceRequestsInt %>'));
		FuelsManagerServiceLib.serviceRequestRefreshPeriod = parseInt('<%= ServiceRequestRefreshPeriod %>');
		FuelsManagerServiceLib.serviceRequestAutomaticRestartDelay = parseInt('<%= ServiceRequestAutomaticRestartDelay %>');

		window.sessionStorage.statusFilter = "Requested,Dispatched";

		$(document).ready(DispatchLib.applicationLoad);
		$(document).ready(DispatchingViewLib.dispatchingViewLoad);
	</script>
</head>
<body oncontextmenu="return false;">
	<form runat="server">
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<DispatchWebApp:FMDispatchHiddenFields ID ="hiddenFields" runat="server"/>
			<div id="dispatchingViewPanel" style="position: absolute">
		<asp:HiddenField runat="server" ID="OperatorGridSelection" Value="" />
		<asp:HiddenField runat="server" ID="EquipmentGridSelection" Value="" />
		<asp:HiddenField runat="server" ID="RequestGridSelection" Value="" />
		<asp:HiddenField runat="server" ID="RadioTextValue" Value="" />
		<asp:HiddenField runat="server" ID="WarningLoopValue" Value="" />
		<asp:HiddenField runat="server" ID="AliasSelectValue" Value="" />
		<span id="currentTime" style="margin-right: 5px"></span><span id="currentDate"></span>
		<table id="dispatchingCenterTable" class="centerTable" style="margin-top: 0px">
			<tr>
				<td>
					<table>
						<tr>
							<td id="equipmentGridCell" style="border-color: black; border-style: solid; border-width: 1px;
								background-color: #F9F9F9">
								<div id="equipmentGridHeader" class="grid-header">
									<FMControls:FMLabel ID="FMLabel4" runat="server">Servicing Unit</FMControls:FMLabel>
									<FMControls:FMLabel ID="FMLabel2" runat="server" Style="margin-left: 15px; margin-top: 3px"
										Text="Select:" />
									<select id="EquipmentSelect" title="Equipment select" tabindex="1" style="margin-left: 5px; margin-top: 3px; width: 150px">
										<option value="{None}"><%=GetTranslatedText("{None}")%></option>
									</select>
									<select id="EquipmentFilter" title="Equipment select filter" tabindex="17" style="margin-left: 15px; margin-top: 3px">
										<option value="0"><%=GetTranslatedText("All Servicing Units")%></option>
										<option value="1"><%=GetTranslatedText("Show Hydrant Service Units Only")%></option>
										<option value="2"><%=GetTranslatedText("Show In-Service Units Only")%></option>
										<option value="3"><%=GetTranslatedText("Show Vehicular Units Only")%></option>
										<option value="4"><%=GetTranslatedText("Show Flight-Line Status")%></option>
									</select>
								</div>
								<div id="gridEquipment" tabindex="12"></div>
							</td>
							<td style="margin-right: 12px"></td>
							<td id="personnelGridCell" style="border-color: black; border-style: solid; border-width: 1px;
								background-color: #F9F9F9">
								<div id="personnelGridHeader" class="grid-header">
									<FMControls:FMLabel ID="FMLabel5" runat="server">Operator</FMControls:FMLabel>
									<FMControls:FMLabel ID="FMLabel3" runat="server" Style="margin-left: 15px; margin-top: 3px"
										Text="Select:" />
									<select id="OperatorSelect" title="Operator select" tabindex="2" style="margin-left: 5px; margin-top: 3px">
										<option value="{None}"><%=GetTranslatedText("{None}")%></option>
									</select>
									<select id="OperatorFilter" title="Operator select filter" tabindex="18" style="margin-left: 15px; margin-top: 3px">
										<option value="0"><%=GetTranslatedText("Show All Personnel")%></option>
										<option value="1"><%=GetTranslatedText("Show On-Duty and Standby Personnel")%></option>
										<option value="2"><%=GetTranslatedText("Show On-Duty Personnel Only")%></option>
										<option value="3"><%=GetTranslatedText("Show Standby Personnel Only")%></option>
										<option value="4"><%=GetTranslatedText("Show Flight-Line Status")%></option>
									</select>
								</div>
								<div id="gridPersonnel" tabindex="13"></div>
							</td>
							<td style="margin-right: 10px" ></td>
							<td style="vertical-align: top; text-align: center">
								<FMControls:FMHtmlButton runat="server" ID="HomeButton" TabIndex="14" CssClass="formfieldtitle"
									Text="&Home" style="height:27px; width: 85px"/>
								<br/><br/>
								<FMControls:FMHtmlButton runat="server" ID="OutButton" TabIndex="15" CssClass="formfieldtitle"
									Text="&Out" style="height:27px; width: 85px"/>
								<br/><br/>
								<FMControls:FMHtmlButton runat="server" ID="StandbyButton" TabIndex="16" CssClass="formfieldtitle"
									Text="Stand &By" style="height:27px; width: 85px"/>
								<br/>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr style="margin-top: 15px">
				<td>
					<table>
						<tr>
							<td id="requestGridCell" style="border-color: black; border-style: solid; border-width: 1px;
								background-color: #F9F9F9">
								<div id="requestGridHeader" class="grid-header">
									<FMControls:FMLabel ID="FMLabel1" runat="server">Active Request Queue</FMControls:FMLabel>
								</div>
								<div id="gridRequests" tabindex="3"></div>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr style="margin-top: 20px">
				<td id="requestCommandButtons" style="text-align: center">
					<FMControls:FMHtmlButton runat="server" ID="DispatchButton" TabIndex="4" CssClass="formfieldtitle"
						Text="&Dispatch" style="height:27px; width: 75px"/>
					&nbsp;&nbsp;
					<FMControls:FMHtmlButton runat="server" ID="UnDispatchButton" TabIndex="5" CssClass="formfieldtitle"
						Text="&Undispatch" style="height:27px; width: 85px"/>
					&nbsp;&nbsp;
					<FMControls:FMHtmlButton runat="server" ID="RadioButton" TabIndex="6" CssClass="formfieldtitle"
						Text="R&adio" style="height:27px; width: 75px" submit="false" />
					&nbsp;&nbsp;
					<FMControls:FMHtmlButton runat="server" ID="FillStandButton" TabIndex="7" CssClass="formfieldtitle"
						Text="&Fill Stand" style="height:27px; width: 85px"/>
					&nbsp;&nbsp;
					<FMControls:FMHtmlButton runat="server" ID="ReturnToBulkButton" TabIndex="8" CssClass="formfieldtitle"
						Text="Return to Bul&k" style="height:27px; width: 130px"/>
					&nbsp;&nbsp;
					<FMControls:FMHtmlButton runat="server" ID="StandbyButton2" TabIndex="9" CssClass="formfieldtitle"
						Text="Stand &By" style="height:27px; width: 85px"/>
					&nbsp;&nbsp;
					<FMControls:FMHtmlButton runat="server" ID="CloseButton" TabIndex="11" CssClass="formfieldtitle"
						Text="&Close" style="height:27px; width: 75px"/>
				</td>
			</tr>
		</table>
	</div>
		</div>
	</form>
	<div id="StandbyRegistrationSelectionForm" title="<%=this.GetTranslatedText("Select Registration ID") %>">
		<table>
			<tr>
				<td style="padding-right: 10px; padding-bottom: 15px">
				    <%=this.GetTranslatedText("Operator Name:") %>
				</td>
				<td style="padding-bottom: 15px">
					<input id="OperatorNameText" alt="Operator name" type="text" style="width: 250px; background-color: #DDDDDD"
						readonly="readonly" />
				</td>
			</tr>
			<tr>
				<td style="padding-right: 10px; padding-bottom: 15px">
				    <%=this.GetTranslatedText("Employee ID:") %>
				</td>
				<td style="padding-bottom: 15px">
					<input id="EmployeeIdText" alt="Employee ID" type="text" style="width: 250px; background-color: #DDDDDD"
						readonly="readonly" />
				</td>
			</tr>
			<tr>
				<td style="padding-right: 10px; padding-bottom: 15px">
				    <%=this.GetTranslatedText("Registration ID:") %>
				</td>
				<td style="padding-bottom: 15px">
					<select id="RegistrationSelect" title="Registration select" style="width: 250px" ></select>
				</td>
			</tr>
		</table>
	</div>
	<div id="RadioDialog" title="Radio Number">
		Radio number:
		<br />
		<input id="RadioTextBox" alt="Radio" type="text" style="width:300px" />
	</div>
	<div id="WarningDialog" title="Dispatch">
		<FMControls:FMLabel runat="server" ID="WarningTextLabel" CssClass="formfieldtitle" />
	</div>
	<div id="AliasSelectionDialog" title="Dispatch">
		<FMControls:FMLabel runat="server" ID="AliasPrompt" CssClass="formfieldtitle" /><br />
		<br />
		<select id="AliasSelect" title="Alias select" style="margin-left: 5px; margin-top: 3px; width: 275px">
		</select>
	</div>
</body>
</html>
