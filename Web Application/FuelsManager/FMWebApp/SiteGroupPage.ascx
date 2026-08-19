<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="SiteGroupPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteGroupPage" %>
<html>
<head>
</head>
<body>
	<FMControls:FMLabel ID="Label3" Style="z-index: 101; left: 0px; position: absolute; top: 16px" runat="server" AssociatedControlID="AssignedSitesListBox"
		CssClass="formfieldtitle" BackColor="Transparent">Assigned Sites:</FMControls:FMLabel>
	<FMControls:FMLabel ID="Label4" Style="z-index: 106; left: 308px; position: absolute; top: 16px" runat="server" AssociatedControlID="UnassignedSitesListBox"
		CssClass="formfieldtitle" Width="144px">Unassigned Sites:</FMControls:FMLabel>
	<asp:ListBox ID="AssignedSitesListBox" Style="z-index: 103; left: 0px; position: absolute; top: 40px"
		runat="server" CssClass="formfield" BackColor="White" Height="126px" SelectionMode="Multiple"
		Width="252px"></asp:ListBox>
	<asp:ListBox ID="UnassignedSitesListBox" Style="z-index: 110; left: 308px; position: absolute; top: 40px"
		runat="server" CssClass="formfield" BackColor="White" Height="126px" SelectionMode="Multiple"
		Width="252px"></asp:ListBox>
	<asp:Button ID="AssignSitesButton" Style="z-index: 104; left: 266px; width: 15px;padding: 2px 8px; position: absolute; top: 64px"
		runat="server" Text="<<"></asp:Button>
	<asp:Button ID="UnassignSitesButton" Style="z-index: 105; left: 266px; width: 15px;padding: 2px 8px; position: absolute; top: 100px"
		runat="server" Text=">>"></asp:Button>
</body>
</html>
