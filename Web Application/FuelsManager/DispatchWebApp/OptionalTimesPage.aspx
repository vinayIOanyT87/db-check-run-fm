<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OptionalTimesPage.aspx.cs" Inherits="FuelsManager.DispatchWebApp.OptionalTimesPage" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xml:lang="en" lang="en">
<head runat="server">
    <base target="_self" />
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
</head>
<body>
	<form id="OptionalTimesForm" runat="server">
	<div style="position: relative;">
		<FMControls:FMLabel ID="titleLabel" Style="z-index: 103; left: 8px; position: absolute;
			top: 8px" runat="server" CssClass="headline" Width="500px" BackColor="Transparent"
			Text="Optional Times" />
	</div>
	<div style="position: relative;">
		<asp:ScriptManager ID="theScriptManager" runat="server" />
		<table id="mainTable" style="z-index: 100; position: absolute; top: 48px; left: 32px;
			border-spacing: 0; padding: 1px; border: 0; margin-top:0px;">
			<tr>
				<td class="auto-style1">
					<FMControls:FMLabel ID="OptionalTimesSelectionLabel" runat="server" Text="Select Optional Times to Use">
					</FMControls:FMLabel>
				</td>
			</tr>
			<tr>
				<td class="auto-style1">
					<FMControls:FMCheckBox ID="ArrivalTimeCheckbox" runat="server" Text="Use Arrival Time" />
				</td>
			</tr>
			<tr>
				<td class="auto-style1">
					<FMControls:FMCheckBox ID="StartTimeCheckbox" runat="server" Text="Use Start Time" />
				</td>
			</tr>
			<tr>
				<td class="auto-style1">
					<FMControls:FMCheckBox ID="StopTimeCheckbox" runat="server" Text="Use Stop Time" />
				</td>
			</tr>
			<tr>
				<td class="auto-style1">&nbsp;</td>
			</tr>
			<tr>
				<td class="auto-style1">
					<FMControls:FMButton ID="OkButton" runat="server" Text="Ok" Width="64px" OnClick="OkButtonOnClick" />
					&nbsp;&nbsp;&nbsp;&nbsp;
					<FMControls:FMButton ID="CancelButton" runat="server" Text="Cancel" Width="64px" OnClientClick="window.close(); return false;" />
				</td>
			</tr>
		</table>
	</div>
	</form>
</body>
</html>
