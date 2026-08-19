<%@ Page language="c#" Codebehind="Run.aspx.cs" AutoEventWireup="false" Inherits="StandardXMLImportExport.Run" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Run</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 8px" runat="server"
			BackColor="Transparent" ImageUrl="../FMWebApp/images/Page_Fade_7.jpg"></asp:image>
		<form id="Form1" method="post" runat="server">
			<asp:CheckBox ID="IgnoreDatesCheckBox" Style="z-index: 113; left: 8px; position: absolute; top: 40px"
				runat="server" Text="Ignore Dates" AutoPostBack="True" TabIndex="1" EnableViewState="False"></asp:CheckBox>
			<asp:Label ID="FromDateLabel" Style="z-index: 102; left: 8px; position: absolute; top: 72px"
				runat="server"> From</asp:Label>
			<asp:Label ID="ToDateLabel" Style="z-index: 103; left: 296px; position: absolute; top: 72px"
				runat="server">To</asp:Label>
			<asp:Calendar ID="FromCalendar" Style="z-index: 104; left: 8px; position: absolute; top: 96px"
				runat="server" Visible="False"></asp:Calendar>
			<asp:TextBox ID="FromTextBox" Style="z-index: 109; left: 8px; position: absolute; top: 96px"
				runat="server" TabIndex="2"></asp:TextBox>
			<asp:Button ID="FromButton" Style="z-index: 111; left: 168px; position: absolute; top: 96px"
				runat="server" Text="Set" TabIndex="3"></asp:Button>
			<asp:Calendar ID="ToCalendar" Style="z-index: 105; left: 296px; position: absolute; top: 96px"
				runat="server" Visible="False"></asp:Calendar>
			<asp:TextBox ID="ToTextBox" Style="z-index: 110; left: 296px; position: absolute; top: 96px"
				runat="server" TabIndex="4"></asp:TextBox>
			<asp:Button ID="ToButton" Style="z-index: 112; left: 456px; position: absolute; top: 96px" runat="server"
				Text="Set" TabIndex="5"></asp:Button>
			<asp:Label ID="FileLabel" Style="z-index: 108; left: 8px; position: absolute; top: 304px" runat="server"> Import from file</asp:Label>
			<input id="FileSelector" style="z-index: 106; left: 8px; width: 528px; position: absolute; top: 328px; height: 22px"
				type="file" size="68" runat="server" tabindex="6">
			<asp:Button ID="ImportButton" Style="z-index: 107; left: 192px; position: absolute; top: 376px"
				runat="server" Text="Import" TabIndex="7"></asp:Button>
		</form>
	</body>
</HTML>
