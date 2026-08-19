<%@ Page language="c#" Codebehind="ScullyForm.aspx.cs" AutoEventWireup="True" Inherits="OPCWebApp.ScullyOPCWebApp.ScullyForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<HTML>
	<HEAD>
		<title>ScullyForm</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div style="position:absolute">
			<asp:image id="Image1" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="..\FMWebApp\images\fade.jpg" BackColor="Transparent"></asp:image>
			<FMControls:FMBUTTON id="CancelButton" style="Z-INDEX: 111; LEFT: 400px; POSITION: absolute; TOP: 304px"
				runat="server" CssClass="formfieldtitle" Width="98px" Text="Scully|Cancel"></FMControls:FMBUTTON>
			<FMControls:FMBUTTON id="OKButton" style="Z-INDEX: 110; LEFT: 272px; POSITION: absolute; TOP: 304px"
				runat="server" CssClass="formfieldtitle" Width="107px" Text="Scully|OK"></FMControls:FMBUTTON>
			<asp:DropDownList id="PortDropDownList" style="Z-INDEX: 109; LEFT: 88px; POSITION: absolute; TOP: 76px"
				runat="server" Width="136px" CssClass="formfield" tabIndex="3"></asp:DropDownList>
			<FMControls:FMLabel id="Label4" style="Z-INDEX: 108; LEFT: 16px; POSITION: absolute; TOP: 76px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Scully|Port:</FMControls:FMLabel>
			<FMControls:FMLabel id="IDRequiredLabel" style="Z-INDEX: 105; LEFT: 72px; POSITION: absolute; TOP: 48px"
				runat="server" BackColor="Transparent" Width="8px" ForeColor="Crimson" Height="8px" CssClass="formfieldtitle">*</FMControls:FMLabel>
			<FMControls:FMLabel id="Label2" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="384px" BackColor="Transparent">Scully|Scully Configuration</FMControls:FMLabel>
			<FMControls:FMLabel id="Label1" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Scully|ID:</FMControls:FMLabel>
			<asp:TextBox id="IDTextBox" style="Z-INDEX: 104; LEFT: 88px; POSITION: absolute; TOP: 48px" runat="server"
				CssClass="formfield" Width="136px" tabIndex="1"></asp:TextBox>
			<FMControls:FMLabel id="DeviceID" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 112px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Scully|Device ID:</FMControls:FMLabel>
			<asp:TextBox id="DeviceIDTextBox" style="Z-INDEX: 104; LEFT: 88px; POSITION: absolute; TOP: 112px" runat="server"
				CssClass="formfield" Width="136px" tabIndex="1"></asp:TextBox>
				
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				document.getElementById("IDTextBox").focus();
			</script>
            </div>
		</form>
	</body>
</HTML>
