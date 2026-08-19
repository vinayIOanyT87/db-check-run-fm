<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="WeightScaleForm.aspx.cs" AutoEventWireup="True" Inherits="WeightScaleOPCWebApp.WeightScaleForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
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
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<FMCONTROLS:FMBUTTON id="CancelButton" style="Z-INDEX: 111; LEFT: 400px; POSITION: absolute; TOP: 304px"
				runat="server" CssClass="formfieldtitle" Width="98px" Text="WeightScale|Cancel"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMBUTTON id="OKButton" style="Z-INDEX: 110; LEFT: 272px; POSITION: absolute; TOP: 304px"
				runat="server" CssClass="formfieldtitle" Width="107px" Text="WeightScale|OK"></FMCONTROLS:FMBUTTON>
			<asp:DropDownList id="PortDropDownList" style="Z-INDEX: 109; LEFT: 88px; POSITION: absolute; TOP: 112px"
				runat="server" Width="136px" CssClass="formfield" tabIndex="3"></asp:DropDownList>
			<FMCONTROLS:FMLABEL id="Label4" style="Z-INDEX: 108; LEFT: 16px; POSITION: absolute; TOP: 112px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|Port:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label3" style="Z-INDEX: 106; LEFT: 16px; POSITION: absolute; TOP: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|Type:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="IDRequiredLabel" style="Z-INDEX: 105; LEFT: 72px; POSITION: absolute; TOP: 48px"
				runat="server" BackColor="Transparent" Width="8px" ForeColor="Crimson" Height="8px" CssClass="formfieldtitle">*</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="384px" BackColor="Transparent">WeightScale|Weight Scale Configuration</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label1" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|ID:</FMCONTROLS:FMLABEL>
			<asp:TextBox id="IDTextBox" style="Z-INDEX: 104; LEFT: 88px; POSITION: absolute; TOP: 48px" runat="server"
				CssClass="formfield" Width="136px" tabIndex="1"></asp:TextBox>
			<asp:DropDownList id="TypeDropDownList" style="Z-INDEX: 107; LEFT: 88px; POSITION: absolute; TOP: 80px"
				runat="server" Width="136px" CssClass="formfield" tabIndex="2"></asp:DropDownList>
			<FMControls:FMLabel id="DeviceID" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 148px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|Device ID:</FMControls:FMLabel>
			<asp:TextBox id="DeviceIDTextBox" style="Z-INDEX: 104; LEFT: 88px; POSITION: absolute; TOP: 148px" runat="server"
				CssClass="formfield" Width="136px" tabIndex="1"></asp:TextBox>
				
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				document.getElementById("IDTextBox").focus();
			</script>
		</div>
</form>
	</body>
</HTML>
