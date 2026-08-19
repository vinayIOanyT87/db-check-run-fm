<%@ Page language="c#" Codebehind="EntityOwnershipForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EntityOwnershipForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label3" Style="z-index: 101; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="288px" BackColor="Transparent">Entity Ownership Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="EntityTypeLabel" AssociatedControlID="EntityTypeListBox" Style="z-index: 103; left: 16px; position: absolute; top: 40px"
				runat="server" CssClass="formfieldtitle" BackColor="Transparent">Entity Type:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label1" AssociatedControlID="EntityListBox" Style="z-index: 105; left: 280px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">Entities:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label2" AssociatedControlID="SiteDropDownList" Style="z-index: 109; left: 608px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">Sites:</FMControls:FMLabel>
			<asp:DropDownList ID="SiteDropDownList" Style="z-index: 108; left: 645px; position: absolute; top: 40px"
				runat="server" CssClass="formfield" Width="200px" TabIndex="4">
			</asp:DropDownList>
			<FMControls:FMButton ID="AssignButton" Style="z-index: 107; left: 855px; position: absolute; top: 34px; height:25px;"
				runat="server" Width="20px" Text=">>" CssClass="formfieldtitle" TabIndex="3"></FMControls:FMButton>
			<FMControls:FMListBox ID="EntityTypeListBox" Style="z-index: 102; left: 16px; position: absolute; top: 64px"
				runat="server" CssClass="formfield" Width="232px" Height="450px" AutoPostBack="True" TabIndex="1" OnSelectedIndexChanged="EntityTypeListBox_SelectedIndexChanged">
			</FMControls:FMListBox>
			<asp:ListBox ID="EntityListBox" Style="z-index: 104; left: 250px; position: absolute; top: 64px"
				runat="server" CssClass="formfield" Width="650px" Height="450px" SelectionMode="Multiple"
				TabIndex="2"></asp:ListBox>
		</div>
</form>
		<script language="jscript">
			document.getElementById("EntityTypeListBox").focus();
		</script>
	</body>
</HTML>
