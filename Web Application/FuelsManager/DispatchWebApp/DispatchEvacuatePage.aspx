<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchEvacuatePage.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.DispatchEvacuatePage" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/redmond/jquery.ui.theme.css" %>" type="text/css" />
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-1.7.1.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-ui-1.8.17.custom.min.js" %>" type="text/javascript"></script>
	<script type="text/javascript">
		function initializeWebControls() {
			// Set up the date range pickers.  Default to today's date.
			$("#fromDateInput").datepicker({
				onSelect: function (selectedDate) {
					$("#toDateInput").datepicker("option", "minDate", selectedDate);
				},
				buttonImage: "images/calendar.gif",
				buttonImageOnly: true,
				showOn: "button"
			});

			$("#fromDateInput").datepicker("setDate", new Date);

			$("#toDateInput").datepicker({
				onSelect: function (selectedDate) {
					$("#fromDateInput").datepicker("option", "maxDate", selectedDate);
				},
				buttonImage: "images/calendar.gif",
				buttonImageOnly: true,
				showOn: "button"
			});

			$("#toDateInput").datepicker("setDate", new Date);
		}
		$(document).ready(initializeWebControls);
	</script>
</head>
<body>
	<form runat="server">
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<div id="content" style="position: absolute">
		<asp:Image ID="fadeImage" runat="server" Style="z-index: 100; left: 0px; top: 0px;
			position: absolute;" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
		<FMControls:FMLabel ID="titleLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 8px; top: 8px; width: 800px" BackColor="Transparent" Text="Dispatch Evacuate"
			CssClass="headline" />
		<FMControls:FMLabel ID="fromDateLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 32px; top: 65px;" BackColor="Transparent" Text="From Date:" CssClass="formfieldtitle" />
		<div id="fromDateDiv" style="position: absolute; left: 32px; top: 85px; width: 110px">
			<input type="text" id="fromDateInput" runat="server" tabindex="1" style="z-index: 118;
				position: relative; height: 20px; width: 85px;" cssclass="formfieldNoWrap" />
		</div>
		<FMControls:FMLabel ID="toDateLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 244px; top: 65px;" BackColor="Transparent" Text="To Date:" CssClass="formfieldtitle" />
		<div id="toDateDiv" style="position: absolute; left: 244px; top: 85px; width: 110px">
			<input type="text" id="toDateInput" runat="server" tabindex="2" style="z-index: 118;
				position: relative; height: 20px; width: 85px;" cssclass="formfieldNoWrap" />
		</div>
		<FMControls:FMLabel ID="uploadMergeFileLabel" runat="server" Style="z-index: 118;
			position: absolute; left: 32px; top: 125px;" BackColor="Transparent" Text="Upload Merge File:"
			CssClass="formfieldtitle" />
		<asp:FileUpload ID="mergeFileUpload" runat="server" TabIndex="3"
			Style="z-index: 118; position: absolute; left: 32px; top: 145px; height: 26px; width: 594px;"
			CssClass="formfieldNoWrap" />
		<FMControls:FMLabel ID="statusLabel" runat="server" Style="z-index: 118; position: absolute;
			left: 32px; top: 185px;" BackColor="Transparent" Text="Status:" CssClass="formfieldtitle" />
		<asp:TextBox ID="statusTextBox" runat="server" TabIndex="4" TextMode="MultiLine"
			ReadOnly="True" Style="z-index: 118; position: absolute; left: 32px; top: 205px;
			width: 600px; height: 298px;" CssClass="formfield" />
		<FMControls:FMButton ID="evacuateButton" runat="server" Style="z-index: 118; position: absolute;
			left: 650px; top: 212px; height: 26px; width: 96px" TabIndex="5" CssClass="formfieldtitle"
			Text="Evacuate" OnClick="EvacuateButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="mergeButton" runat="server" Style="z-index: 118; position: absolute;
			left: 650px; top: 252px; height: 26px; width: 96px" TabIndex="6" CssClass="formfieldtitle"
			Text="Merge" OnClick="MergeButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="closeButton" runat="server" Style="z-index: 118; position: absolute;
			left: 650px; top: 292px; height: 26px; width: 96px" TabIndex="7" CssClass="formfieldtitle"
			Text="Close" OnClick="CloseButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="downloadXmlButton" runat="server" Style="z-index: 118; position: absolute;
			left: 650px; top: 332px; height: 26px; width: 96px" TabIndex="8" CssClass="formfieldtitle"
			Text="Download XML" OnClick="DownloadXmlButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="downloadDbButton" runat="server" Style="z-index: 118; position: absolute;
			left: 650px; top: 372px; height: 26px; width: 96px" TabIndex="9" CssClass="formfieldtitle"
			Text="Download DB" OnClick="DownloadDbButtonOnClick"></FMControls:FMButton>
	</div>
		</div>
	</form>
</body>
</html>
