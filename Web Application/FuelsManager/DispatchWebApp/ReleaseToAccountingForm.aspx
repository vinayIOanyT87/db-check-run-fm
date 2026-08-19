<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseToAccountingForm.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.ReleaseToAccountingForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>

<html>
<head runat="server">
	<base target="_self" />
	<title></title>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/redmond/jquery.ui.theme.css" %>" type="text/css" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/jquery-ui-timepicker-addon.css" %>" type="text/css" />
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-1.7.1.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-ui-1.8.17.custom.min.js" %>"" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-ui-timepicker-addon.js" %>" type="text/javascript"></script>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
	<script type="text/javascript">

	    function initLockOutDateControl(date) {

	        // Set up the lockout date picker.  Default to today's date.
	        $('#lockOutDateInput').datetimepicker({
	            showSecond: true,
	            timeFormat: 'hh:mm:ss TT'
	        });

	        $('#lockOutDateInput').datetimepicker('setDate', date);
	    }


		function showAlertDialog(alertMessage) {
			setTimeout(function () {
				window.alert(alertMessage);
			}, 0);
		}
	</script>
    <style>
        .ui-datepicker-current { display: none} /*could cause time different than server */
    </style>
</head>
<body>
	<form runat="server">
	<div style="position: absolute">
		<FMControls:FMLabel ID="titleLabel" runat="server" Style="z-index: 118; position: absolute;
                                                                                                     left: 8px; top: 8px" BackColor="Transparent" Text="Dispatch Release to Accounting"
			CssClass="headline" />
		<FMControls:FMLabel ID="lockOutDateLabel" runat="server" Style="z-index: 118; position: absolute;
                                                                                                                                                                left: 32px; top: 50px;" BackColor="Transparent" Text="Lock Out Date:" CssClass="formfieldtitle" />
		<div id="lockOutDateDiv" style="position: absolute; left: 32px; top: 70px">
			<input type="text" id="lockOutDateInput" runat="server" tabindex="1" style="z-index: 118;
                                                                                                                                                                                                                                                                                                                               position: relative; height: 20px; width: 160px;" cssclass="formfieldNoWrap" />
			<asp:HiddenField ID="TimeOffsetField" runat="server" />
		</div>
		<FMControls:FMButton ID="useCurrentDateButton" runat="server" Style="z-index: 118; position: absolute;
                                                                                                                                                                                                                                                                                                                                                                                                                      left: 32px; top: 110px; height: 26px; width: 160px" TabIndex="2" CssClass="formfieldtitle"
			Text="Use Current Date" OnClick="CurrentDateButtonOnClick" />
		<FMControls:FMButton ID="applyButton" runat="server" Style="z-index: 118; position: absolute;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                left: 70px; top: 380px; height: 26px; width: 96px" TabIndex="3" CssClass="formfieldtitle"
			Text="Apply" OnClick="ApplyButtonOnClick" />
		<FMControls:FMHtmlButton ID="closeButton" runat="server" Style="z-index: 118; position: absolute;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         left: 200px; top: 380px; height: 26px; width: 96px" TabIndex="4" CssClass="formfieldtitle"
			Text="Close"/>
	</div>
	</form>
</body>
</html>
