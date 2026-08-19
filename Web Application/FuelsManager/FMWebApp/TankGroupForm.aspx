<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="TankGroupForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TankGroupForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body tabindex="-1" ms_positioning="GridLayout">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="TankGroupTitleLabel"
				Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
				BackColor="Transparent" CssClass="headline" Width="500px">Tank Group Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label1" AssociatedControlID="Name" Style="z-index: 103; left: 16px; position: absolute; top: 40px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Tank Group ID:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label8" Style="z-index: 105; left: 224px; position: absolute; top: 40px" runat="server"
				BackColor="Transparent" Height="8px" ForeColor="Crimson" Width="8px">*</FMControls:FMLabel>
			<asp:TextBox ID="Name" Style="z-index: 104; left: 240px; position: absolute; top: 40px" runat="server" aria-required="true"
				BackColor="White" CssClass="formfield" Width="216px" MaxLength="30" TabIndex="1"></asp:TextBox>
			<FMControls:FMLabel ID="FMLABEL1" AssociatedControlID="ProductsDropDownList" Style="z-index: 115; left: 16px; position: absolute; top: 72px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Product:</FMControls:FMLabel>
			<asp:DropDownList ID="ProductsDropDownList" Style="z-index: 116; left: 240px; position: absolute; top: 72px"
				runat="server" BackColor="White" Width="216px" ForeColor="Black" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="ProductsDropDownListSelectedIndexChanged">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label3" AssociatedControlID="AssignedTanksListBox" Style="z-index: 106; left: 16px; position: absolute; top: 120px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Assigned Tanks:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label4" AssociatedControlID="UnassignedTanksListBox" Style="z-index: 107; left: 264px; position: absolute; top: 120px" runat="server"
				CssClass="formfieldtitle" Width="144px">Unassigned Tanks:</FMControls:FMLabel>
			<asp:ListBox ID="AssignedTanksListBox" Style="z-index: 108; left: 16px; position: absolute; top: 136px"
				runat="server" BackColor="White" CssClass="formfield" Height="237px" Width="200px" SelectionMode="Multiple" TabIndex="2"></asp:ListBox>
			<asp:ListBox ID="UnassignedTanksListBox" Style="z-index: 111; left: 264px; position: absolute; top: 136px"
				runat="server" BackColor="White" CssClass="formfield" Height="237px" Width="200px" SelectionMode="Multiple" TabIndex="5"></asp:ListBox>
			<asp:Button ID="AssignTanksButton" Style="z-index: 109; left: 232px; position: absolute; top: 208px; padding-left: 1px; padding-right: 1px"
				runat="server" CssClass="formfieldtitle" Text="<<" Width="20px" ToolTip="Assign" TabIndex="3"></asp:Button>
			<asp:Button ID="UnassignTanksButton" Style="z-index: 110; left: 232px; position: absolute; top: 245px; padding-left: 1px; padding-right: 1px"
				runat="server" CssClass="formfieldtitle" Text=">>" Width="20px" ToolTip="Unassign" TabIndex="4"></asp:Button>
			<FMControls:FMButton ID="OK" Style="z-index: 112; left: 312px; position: absolute; top: 384px" runat="server"
				CssClass="formfieldtitle" Width="67px" Text="OK" TabIndex="100"></FMControls:FMButton>
			<FMControls:FMButton ID="Cancel" Style="z-index: 113; left: 400px; position: absolute; top: 384px" runat="server"
				CssClass="formfieldtitle" Text="Cancel" Width="67px" TabIndex="101"></FMControls:FMButton>
			<FMControls:FMLabel ID="Label10" Style="z-index: 114; left: 320px; position: absolute; top: 416px" runat="server"
				CssClass="formfieldtitle" Height="8px" ForeColor="Crimson" Width="144px">* Denotes Required Field</FMControls:FMLabel>
		</div>
	</form>
</body>
</html>
