<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="CompanyGroupsPage.ascx.cs" Inherits="FMWebApp.CompanyGroupsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
		<FMControls:FMLabel id="Label3" AssociatedControlID="AssignedGroupsListBox" style="Z-INDEX: 112; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
			CssClass="formfieldtitle">Assigned Groups:</FMControls:FMLabel>
		<FMControls:FMLabel id="Label5" AssociatedControlID="UnassignedGroupsListBox" style="Z-INDEX: 114; LEFT: 272px; POSITION: absolute; TOP: 8px" runat="server"
			CssClass="formfieldtitle" Width="160px">Unassigned Groups:</FMControls:FMLabel>
		<asp:listbox id="AssignedGroupsListBox" style="Z-INDEX: 109; LEFT: 8px; POSITION: absolute; TOP: 32px"
			runat="server" CssClass="formfield" Height="96px" Width="208px" SelectionMode="Multiple"></asp:listbox>
		<FMControls:FMButton id="AssignGroupsButton" style="Z-INDEX: 117; LEFT: 232px; POSITION: absolute; TOP: 48px; width: 20px;"
			runat="server" CssClass="formfieldtitle" Text="<<" />
		<FMControls:FMButton id="UnassignGroupsButton" style="Z-INDEX: 120; LEFT: 232px; POSITION: absolute; TOP: 88px; width: 20px; "
			runat="server" CssClass="formfieldtitle" Text=">>" />
		<asp:listbox id="UnassignedGroupsListBox" style="Z-INDEX: 111; LEFT: 272px; POSITION: absolute; TOP: 32px"
			runat="server" CssClass="formfield" Height="96px" Width="208px" SelectionMode="Multiple"></asp:listbox>
	</body>
</HTML>
