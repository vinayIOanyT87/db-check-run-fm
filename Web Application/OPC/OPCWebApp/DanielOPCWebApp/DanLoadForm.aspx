<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="DanLoadForm.aspx.cs" AutoEventWireup="True" Inherits="OPCWebApp.DanielOPCWebApp.DanLoadForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
	</HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 101; left: 8px; position: absolute; top: 8px" runat="server"
				BackColor="Transparent" Width="320px" CssClass="headline">Daniel|Preset Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label1" AssociatedControlID="IDTextBox" Style="z-index: 102; left: 16px; position: absolute; top: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Daniel|ID:</FMControls:FMLabel>
			<FMControls:FMLabel ID="UserNameRequiredLabel" Style="z-index: 104; left: 72px; position: absolute; top: 48px"
				runat="server" BackColor="Transparent" Width="8px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
			<asp:TextBox ID="IDTextBox" Style="z-index: 103; left: 128px; position: absolute; top: 48px" aria-required="true"
				TabIndex="1" runat="server" Width="136px" CssClass="formfield"></asp:TextBox>
			<FMControls:FMLabel ID="Label3" Style="z-index: 105; left: 16px; position: absolute; top: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Daniel|Type:</FMControls:FMLabel>
			<asp:DropDownList ID="TypeDropDownList" Style="z-index: 106; left: 128px; position: absolute; top: 80px"
				TabIndex="2" runat="server" Width="136px" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownList_SelectedIndexChanged">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label4" Style="z-index: 107; left: 16px; position: absolute; top: 112px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Daniel|Port:</FMControls:FMLabel>
			<asp:DropDownList ID="PortDropDownList" Style="z-index: 108; left: 128px; position: absolute; top: 112px"
				TabIndex="3" runat="server" Width="136px" CssClass="formfield">
			</asp:DropDownList>
			<FMControls:FMLabel ID="FMLABEL1" Style="z-index: 112; left: 16px; position: absolute; top: 144px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Daniel|Address:</FMControls:FMLabel>
			<asp:DropDownList ID="AddressDropDownList" Style="z-index: 113; left: 128px; position: absolute; top: 144px"
				TabIndex="3" runat="server" Width="136px" CssClass="formfield">
			</asp:DropDownList>
			<FMControls:FMButton ID="OKButton" Style="z-index: 110; left: 264px; position: absolute; top: 184px"
				TabIndex="100" runat="server" Text="Daniel|OK" Width="107px" CssClass="formfieldtitle"></FMControls:FMButton>
			<FMControls:FMButton ID="CancelButton" Style="z-index: 111; left: 384px; position: absolute; top: 184px"
				TabIndex="101" runat="server" Text="Daniel|Cancel" Width="98px" CssClass="formfieldtitle"></FMControls:FMButton>
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				if (!document.getElementById("IDTextBox").disabled)
					document.getElementById("IDTextBox").focus();
			</script>
		</div>
</form>
	</body>
</HTML>
