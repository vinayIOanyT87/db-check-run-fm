<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchToolbarConfigurationPage.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.DispatchToolbarConfigurationPage" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
	<form id="form1" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<div id="content" style="position:absolute">
		<asp:Image ID="fadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; top: 0px; position: absolute"
			runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
		<FMControls:FMLabel ID="titleLabel" Style="z-index: 118; position: absolute; left: 8px;
			top: 8px; width: 800px" runat="server" BackColor="Transparent" CssClass="headline">Dispatch Toolbar Configuration</FMControls:FMLabel>
		<FMControls:FMLabel ID="toolbarTypeLabel" AssociatedControlID="toolbarTypeDropDownList" Style="z-index: 116; left: 14px; position: absolute;
			top: 50px" runat="server" CssClass="formfieldtitle">Toolbar Type:</FMControls:FMLabel>
		<asp:DropDownList ID="toolbarTypeDropDownList" Style="z-index: 108; left: 100px;
			position: absolute; top: 48px" TabIndex="2" runat="server" CssClass="formfield"
			AutoPostBack="True" Width="226px" OnSelectedIndexChanged="ToolbarTypeDropDownListSelectedIndexChanged">
		</asp:DropDownList>
		<FMControls:FMLabel ID="selectedCommandsLabel" AssociatedControlID="selectedCommandsListBox" Style="z-index: 118; position: absolute;
			left: 100px; top: 90px" runat="server" Text="Selected Commands" CssClass="formfieldtitle" />
		<FMControls:FMLabel ID="availableCommandsLabel" AssociatedControlID="availableCommandsListBox" Style="z-index: 118; position: absolute;
			left: 386px; top: 90px" runat="server" Text="Available Commands" CssClass="formfieldtitle" />
		<asp:ListBox ID="selectedCommandsListBox" Style="z-index: 118; position: absolute;
			left: 100px; top: 115px; height: 275px;" runat="server" CssClass="formfield" Width="226px"
			SelectionMode="Multiple">
		</asp:ListBox>
		<asp:ListBox ID="availableCommandsListBox" Style="z-index: 118; position: absolute;
			left: 386px; top: 115px;" runat="server" CssClass="formfield" Height="275px" Width="226px"
			SelectionMode="Multiple">
		</asp:ListBox>
		<FMControls:FMButton ID="upButton" Style="z-index: 118; position: absolute; left: 40px;
			top: 162px" TabIndex="1" runat="server" CssClass="formfieldtitle" Width="48px"
			Text="Up" OnClick="UpButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="downButton" Style="z-index: 118; position: absolute; left: 40px;
			top: 206px" TabIndex="2" runat="server" CssClass="formfieldtitle" Width="48px"
			Text="Down" OnClick="DownButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="selectCommandButton" Style="z-index: 118; position: absolute;
			left: 340px; top: 162px" TabIndex="3" runat="server" CssClass="formfieldtitle"
			Width="32px" Text="<<" OnClick="SelectCommandButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="unselectCommandButton" Style="z-index: 118; position: absolute;
			left: 340px; top: 206px" TabIndex="4" runat="server" CssClass="formfieldtitle"
			Width="32px" Text=">>" OnClick="UnselectCommandButtonOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="applyButton" Style="z-index: 118; position: absolute; left: 540px;
			top: 403px" TabIndex="5" runat="server" CssClass="formfieldtitle" Text="Apply"
			Width="72px" OnClick="ApplyButtonOnClick"></FMControls:FMButton>
		</div>
		</div>
	</form>
</body>
</html>
