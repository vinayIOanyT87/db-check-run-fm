<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchGridColumnConfigurationPage.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.DispatchGridColumnConfigurationPage" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/dispatch.js" %>" type="text/javascript"></script>
</head>
<body>
	<form id="form1" runat="server">
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<div id="content" style="position:absolute">
		<asp:Image ID="fadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; top: 0px; position: absolute"
			runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
		<FMControls:FMLabel ID="titleLabel" Style="z-index: 118; position: absolute; left: 8px;
			top: 8px; width: 800px" runat="server" BackColor="Transparent" CssClass="headline">Dispatch Grid Column Configuration</FMControls:FMLabel>
		<FMControls:FMLabel ID="gridTypeLabel" AssociatedControlID="gridTypeDropDownList" Style="z-index: 116; left: 30px; position: absolute;
			top: 50px" runat="server" CssClass="formfieldtitle">Grid Type:</FMControls:FMLabel>
		<asp:DropDownList ID="gridTypeDropDownList" Style="z-index: 108; left: 100px;
			position: absolute; top: 48px" TabIndex="2" runat="server" CssClass="formfield"
			AutoPostBack="True" Width="272px" OnSelectedIndexChanged="GridTypeDropDownListSelectedIndexChanged">
		</asp:DropDownList>
		<FMControls:FMLabel ID="selectedColumnsLabel" AssociatedControlID="selectedColumnsListBox" Style="z-index: 118; position: absolute;
			left: 100px; top: 90px" runat="server" Text="Selected Columns" CssClass="formfieldtitle" />
		<FMControls:FMLabel ID="availableColumnsLabel" AssociatedControlID="availableColumnsListBox" Style="z-index: 118; position: absolute;
			left: 386px; top: 90px" runat="server" Text="Available Columns" CssClass="formfieldtitle" />
		<asp:ListBox ID="selectedColumnsListBox" Style="z-index: 118; position: absolute;
			left: 100px; top: 115px; height: 188px;" runat="server" CssClass="formfield" Width="226px"
			SelectionMode="Multiple">
		</asp:ListBox>
		<asp:ListBox ID="availableColumnsListBox" Style="z-index: 118; position: absolute;
			left: 386px; top: 115px;" runat="server" CssClass="formfield" Height="188px" Width="226px"
			SelectionMode="Multiple">
		</asp:ListBox>
		<FMControls:FMButton ID="upButton" Style="z-index: 118; position: absolute; left: 40px;
			top: 162px" TabIndex="1" runat="server" CssClass="formfieldtitle" Width="48px"
			Text="Up" OnClick="UpButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="downButton" Style="z-index: 118; position: absolute; left: 40px;
			top: 206px" TabIndex="2" runat="server" CssClass="formfieldtitle" Width="48px"
			Text="Down" OnClick="DownButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="selectColumnsButton" Style="z-index: 118; position: absolute;
			left: 340px; top: 162px" TabIndex="3" runat="server" CssClass="formfieldtitle"
			Width="32px" Text="<<" OnClick="SelectColumnsButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="unselectColumnsButton" Style="z-index: 118; position: absolute;
			left: 340px; top: 206px" TabIndex="4" runat="server" CssClass="formfieldtitle"
			Width="32px" Text=">>" OnClick="UnselectColumnsButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="applyButton" Style="z-index: 118; position: absolute; left: 540px;
			top: 316px" TabIndex="5" runat="server" CssClass="formfieldtitle" Text="Apply"
			Width="72px" OnClick="ApplyButtonOnClick" OnClientClick="applyButtonOnClientClick()"></FMControls:FMButton>
		</div>
		</div>
	</form>
	<script type="text/javascript">
		function applyButtonOnClientClick() {
			DispatchLib.currentUserGuid = '<%= Security.UserGuid.ToString() %>';
			DispatchLib.clearGridUserSettings();
		}
	</script>
</body>
</html>
