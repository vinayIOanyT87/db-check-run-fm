<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SiteNotesPage.ascx.cs" Inherits="FuelsManager.FMWebApp.SiteNotesPage" %>
<html>
	<head>
	</head>
	<body>
	<FMCONTROLS:FMLabel id="NoteLabel" AssociatedControlID="NoteText" runat="server" Text="Site Notes" CssClass="formfieldtitle" style="Z-INDEX: 150; LEFT: 0px; POSITION: absolute; TOP: 10px" />
	<FMControls:FMTextBox id="NoteText" tabIndex="1" runat="server" CssClass="FormField" Width="700px" TextMode="MultiLine"
		Height="365px" style="Z-INDEX: 200; LEFT: 0px; POSITION: absolute; TOP: 30px" MaxLength="2000" />
	</body>
</html>

