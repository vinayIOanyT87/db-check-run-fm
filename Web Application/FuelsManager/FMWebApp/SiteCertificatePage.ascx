<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SiteCertificatePage.ascx.cs" Inherits="FuelsManager.FMWebApp.SiteCertificatePage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
	</HEAD>
	<body>
		<FMControls:FMLabel id="Label3" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 16px" runat="server" AssociatedControlID="AssignedCertificatesListBox"
			CssClass="formfieldtitle" BackColor="Transparent">Assigned Certificates:</FMControls:FMLabel>
		<FMControls:FMLabel id="Label4" style="Z-INDEX: 106; LEFT: 308px; POSITION: absolute; TOP: 16px" runat="server" AssociatedControlID="UnassignedCertificatesListBox"
			CssClass="formfieldtitle" Width="144px">Unassigned Certificates:</FMControls:FMLabel>
		<asp:listbox id="AssignedCertificatesListBox" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 40px"
			runat="server" CssClass="formfield" BackColor="White" Height="126px" SelectionMode="Multiple"
			Width="252px"></asp:listbox>
		<asp:listbox id="UnassignedCertificatesListBox" style="Z-INDEX: 110; LEFT: 308px; POSITION: absolute; TOP: 40px"
			runat="server" CssClass="formfield" BackColor="White" Height="126px" SelectionMode="Multiple"
			Width="252px"></asp:listbox>
		<asp:button id="AssignCertificatesButton" style="Z-INDEX: 104; LEFT: 265px; POSITION: absolute; TOP: 64px; width: 15px;padding:2px 8px;"
			runat="server" Text="<<"></asp:button>
		<asp:button id="UnassignCertificatesButton" style="Z-INDEX: 105; LEFT: 265px; POSITION: absolute; TOP: 100px; width: 15px;padding: 2px 8px;"
			runat="server" Text=">>"></asp:button>
	</body>
</HTML>

