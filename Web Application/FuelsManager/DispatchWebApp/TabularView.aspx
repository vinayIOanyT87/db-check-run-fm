<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TabularView.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.TabularView" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
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
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/tabularView.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/fuelsManagerServiceInterface.js" %>" type="text/javascript"></script>

	<script type="text/javascript">
		$(function () { $('#CancelConfirmationDialog').dialog({ autoOpen: false }); });
		$(function () { $('#UnCancelConfirmationDialog').dialog({ autoOpen: false }); });
		$(function () { $('#CancelCommentDialog').dialog({ autoOpen: false }); });
		$(function () { $('#StandbyButtonDialog').dialog({ autoOpen: false }); });
		$(function () { $('#ChangeOperatorStatusDialog').dialog({ autoOpen: false }); });
		$(function () { $('#RegistrationSelectForm').dialog({ autoOpen: false }); });

		TabularViewLib.securityToken = '<%= Security.Token.ToString() %>';
		TabularViewLib.siteGuid = '<%= Security.SiteGuid.ToString() %>';
		TabularViewLib.jsonTransactionAliasNames = '<%= JsonTransactionAliasNames %>';
		TabularViewLib.jsonTransactionStatusValues = '<%= JsonTransactionStatusValues %>';
		TabularViewLib.jsonGridColumnDefinitions = '<%= JsonTabularGridColumnDefinitions %>';
		TabularViewLib.jsonOperationalLockDateValue = '<%= JsonOperationLockDateValue %>';
		TabularViewLib.jsonOptionalTimesArrivalFlagValue = '<%= JsonOptionalTimeArrivalFlag %>';
		TabularViewLib.jsonOptionalTimesStartFlagValue = '<%= JsonOptionalTimeStartFlag %>';
		TabularViewLib.jsonOptionalTimesStopFlagValue = '<%= JsonOptionalTimeStopFlag %>';

		DispatchLib.currentUserGuid = '<%= Security.UserGuid.ToString() %>';
		DispatchLib.displayCurrentTime = Boolean(parseInt('<%= DisplayCurrentTimeInt %>'));
		DispatchLib.displayMilitaryJulianDate = Boolean(parseInt('<%= DisplayMilitaryJulianDateInt %>'));
		DispatchLib.resetTabularViewSessionOperation = '<%= ResetTabularViewSessionOperation %>';

		FuelsManagerServiceLib.serviceAddress = '<%= DispatchRequestServiceAddress %>';
		FuelsManagerServiceLib.enableServiceRequests = Boolean(parseInt('<%= EnableServiceRequestsInt %>'));
		FuelsManagerServiceLib.serviceRequestRefreshPeriod = parseInt('<%= ServiceRequestRefreshPeriod %>');
		FuelsManagerServiceLib.serviceRequestAutomaticRestartDelay = parseInt('<%= ServiceRequestAutomaticRestartDelay %>');

		window.sessionStorage.statusFilter = '';

		$(document).ready(DispatchLib.applicationLoad);
		$(document).ready(TabularViewLib.tabularPageLoad);
		$(document).ready(function () {
			var value = DispatchLib.getQueryStringParams();
			if (value.triggerStandbyStatusBoard == 'true') {
				TabularViewLib.StandbyButtonOnClick();
			} else {
				value = DispatchLib.getQueryStringParams();
				if (value.showReleaseToAccounting == 'true') {
					TabularViewLib.ReleaseToAccountingButtonOnClick();
				}
			}
		});
	</script>
</head>
<body oncontextmenu="return false;">
	<form runat="server">
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<DispatchWebApp:FMDispatchHiddenFields ID ="hiddenFields" runat="server"/>
			<div id="tabularViewPanel" style="position:absolute">
				<asp:HiddenField runat="server" ID="RequestGridSelection" Value=""/>
				<asp:HiddenField runat="server" ID="LoopStorage" Value=""/>
				<asp:HiddenField runat="server" ID="CancelCommentText" Value=""/>
				<span id="currentTime" style="margin-right: 5px"></span>
				<span id="currentDate"></span>
				<FMControls:FMToolbar runat="server" ID="toolBarTabular" CssClass="listToolBar"/>
				<table id="tabularCenterTable" class="centerTable" style="margin-top: 5px">
					<tr>
						<td id="tabularGridCell" style="border-color: black; border-style:solid; border-width: 1px; background-color:#F9F9F9">
							<div id="gridHeader" class="grid-header">
								<FMControls:FMLabel runat="server">Dispatch Tabular View</FMControls:FMLabel>
								<FMControls:FMLabel runat="server" style="margin-left: 50px; margin-top: 3px">Begin:</FMControls:FMLabel>
								<input type="text" alt="View begin date" id="begindate" tabindex="1" style="width: 100px"/>
								<FMControls:FMLabel runat="server" style="margin-left: 15px; margin-top: 3px">End:</FMControls:FMLabel>
								<input type="text" alt="View end date" id="enddate" tabindex="2" style="width: 100px"/>
								<FMControls:FMLabel runat="server" style="margin-left: 15px; margin-top: 3px">Status:</FMControls:FMLabel>
								<select id="StatusSelect" title="Status" tabindex="3" style="margin-left: 15px; margin-top: 3px">
									<option value="{All}"><%=GetTranslatedText("{All}")%></option>
								</select>
								<FMControls:FMLabel ID="RequestTypeLabel" runat="server" style="margin-left: 15px; margin-top: 3px">Request Type:</FMControls:FMLabel>
								<select id="RequestTypeSelect" title="Request" tabindex="4" style="margin-left: 15px; margin-top: 3px">
									<option value="{All}"><%=GetTranslatedText("{All}")%></option>
								</select>
								<FMControls:FMLabel runat="server" style="margin-left: 15px; margin-top: 3px">Vehicle ID:</FMControls:FMLabel>
								<select id="VehicleSelect" title="Vehicle" tabindex="5" style="margin-left: 15px; margin-top: 3px">
									<option value="{All}"><%=GetTranslatedText("{All}")%></option>
								</select>
							</div>
							<div id="gridTabular"/>
						</td>
					</tr>
				</table>
				<ul id="contextMenu" style="display:none;position:absolute">
					<li runat="server" id="ArrivedItem" data="Arrived">Set Arrived</li>
					<li runat="server" id="StartedItem" data="Started">Set Service Started</li>
					<li runat="server" id="StoppedItem" data="Stopped">Set Service Stopped</li>
					<li runat="server" id="CompletedItem" data="Completed">Set Service Completed</li>
					<li runat="server" id="FillstandCompleteItem" data="FillstandComplete">Set Fillstand Completed</li>
					<li><hr/></li>
					<li runat="server" id="RelogItem" data="Relog">Relog Request</li>
					<li runat="server" id="CancelItem" data="Cancel">Cancel Request</li>
					<li runat="server" id="UncancelItem" data="Uncancel">Uncancel Request</li>
					<li><hr/></li>
					<li runat="server" id="QualityResultsItem" data="QualityResults">Show Quality Results</li>
					<li runat="server" id="TrainingAssignmentsItem" data="TrainingAssignments">Show Training Assignments</li>
				</ul>
			</div>
		</div>
		<script type="text/javascript">
			function PopupWindow(rowNum) {
				if (rowNum != undefined && rowNum != null) {
					var transId = TabularViewLib.data[rowNum].TransId;
					window.window_location_assign("../DispatchWebApp/ControlLogForm.aspx?transId=" + transId +
											"&NavigateAction=openClick");
				}
				return false;
			}
		</script>
	</form>

	<div id="StandbyButtonDialog" title="<%=GetTranslatedText("Standby Status Board")%>">
		<FMControls:FMLabel ID="personnelLabel" runat="server" style="display: none">Personnel</FMControls:FMLabel>
		<FMControls:FMLabel ID="equipmentLabel" runat="server" style="display: none">Equipment</FMControls:FMLabel>
		<FMControls:FMLabel ID="dispatchLabel" runat="server" style="display: none">Dispatch</FMControls:FMLabel>
		<FMControls:FMLabel ID="closeLabel" runat="server" style="display: none">Close</FMControls:FMLabel>
		<div id="gridStandby" style="width:100%; height:100%" />
	</div>

	<div id="CancelConfirmationDialog" title="Dispatch">
		<FMControls:FMLabel runat="server" ID="WarningTextLabel" CssClass="formfieldtitle" Text="Once an operation is canceled it cannot be un-canceled"/>
		<br/>
		<FMControls:FMLabel runat="server" ID="WarningTextLabel2" CssClass="formfieldtitle" Text="Are you sure you want to cancel this request(s)?"/>
	</div>
	
	<div id="CancelCommentDialog" title="Dispatch">
		<FMControls:FMLabel runat="server" ID="CancelCommentHeading" CssClass="formfieldtitle" Text="Cancellation comment for: "/>
		<br/>
		<textarea id="CancelCommentTextBox" title="Cancel comment" rows="5" cols="250"></textarea>

	</div>

	<div id="UnCancelConfirmationDialog" title="Dispatch">
		<FMControls:FMLabel runat="server" ID="FMLabel2" CssClass="formfieldtitle" Text="Are you sure you want to uncancel this request(s)?"/>
	</div>
    
    <div id="ChangeOperatorStatusDialog" title="<%=this.GetTranslatedText("Change Operator Status") %>">
		<FMControls:FMLabel ID="operatorLabel" runat="server" style="display: none"><%=this.GetTranslatedText("Operator Name") %></FMControls:FMLabel>
		<FMControls:FMLabel ID="statusLabel" runat="server" style="display: none"><%=this.GetTranslatedText("Status Code") %></FMControls:FMLabel>
		<FMControls:FMLabel ID="vehicleLabel" runat="server" style="display: none"><%=this.GetTranslatedText("Vehicle") %></FMControls:FMLabel>
		<FMControls:FMLabel ID="closeLabel2" runat="server" style="display: none"><%=this.GetTranslatedText("Close") %></FMControls:FMLabel>
        <table>
            <tr>
                <td style="vertical-align:top">
                    <div id="gridStatus" style="width:525px; height:100%"></div>
                </td>
                <td style="vertical-align:top">
                    <input type="button" id="HomeButton" class="ui-state-focus" value="Home" style="position:relative; height: 35px; left: 20px; width: 75px"/><br/><br/>
                    <input type="button" id="OutButton" class="ui-state-focus" value="Out" style="position:relative; height: 35px; left: 20px; width: 75px"/><br/><br/>
                    <input type="button" id="StandButton" class="ui-state-focus" value="Standby" style="position:relative; height: 35px; left: 20px; width: 75px"/>
                </td>
            </tr>
        </table>
    </div>
    
    <div id="RegistrationSelectForm" title="<%=this.GetTranslatedText("Select Registration ID") %>">
		<FMControls:FMLabel ID="cancelLabel2" runat="server" style="display: none"><%=this.GetTranslatedText("Cancel") %></FMControls:FMLabel>
		<FMControls:FMLabel ID="okLabel2" runat="server" style="display: none"><%=this.GetTranslatedText("OK") %></FMControls:FMLabel>
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
                    <input id="EmployeeGuid" alt="Employee Guid" type="text" style="visibility:hidden; width:0;" />
                    <input id="AssignedEquipmentGuid" alt="Assigned Equipment Guid" type="text" style="visibility:hidden; width:0;" />
                    <input id="EquipmentID" type="text" alt="Equipment ID" style="visibility:hidden; width:0;" />
				</td>
			</tr>
			<tr>
				<td style="padding-right: 10px; padding-bottom: 15px">
				    <%=this.GetTranslatedText("Registration ID:") %>
				</td>
				<td style="padding-bottom: 15px">
					<select id="RegistrationSelect" title="Registration" style="width: 250px" />
				</td>
			</tr>
		</table>
    </div>
	
</body>
</html>
