<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="CompanyNotesPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanyNotesPage" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
	<FMCONTROLS:FMLabel id="NoteLabel" AssociatedControlID="NoteText" runat="server" Text="Company Notes" CssClass="formfieldtitle" style="Z-INDEX: 150; LEFT: 0px; POSITION: absolute; TOP: 10px" />
	<FMControls:FMTextBox id="NoteText" tabIndex="1" runat="server" CssClass="formfield" Width="710px" TextMode="MultiLine"
		Height="365px" style="Z-INDEX: 200; LEFT: 0px; POSITION: absolute; TOP: 30px" MaxLength="2000" />
	</body>
</HTML>
