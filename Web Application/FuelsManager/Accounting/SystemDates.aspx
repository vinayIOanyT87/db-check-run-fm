<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="SystemDates.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.SystemDates" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
	<head>
		<title></title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	</head>
	<body ms_positioning="GridLayout">
		<form id="Form1" method="post" runat="server">
			<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
			<div id="pageContent" style="position: absolute">
				<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
				<FMControls:FMLabel ID="TitleLabel" Style="z-index: 111; left: 8px; position: absolute; top: 8px" runat="server"
					CssClass="headline">Lockout Dates Configuration</FMControls:FMLabel>
				<FMControls:FMLabel ID="OperationsLabel" Style="left: 8px; position: absolute; top: 40px"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Operations Date:</FMControls:FMLabel>
				<FMControls:FMLabel ID="AccountingLabel" Style="left: 350px; position: absolute; top: 40px"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Accounting Date :</FMControls:FMLabel>
				<FMControls:FMDateTime ID="Operations" ToolTip="Operations Date" Style="left: 8px; position: absolute; top: 64px" runat="server"
					TabIndex="1" Width="300px" CssClass="formfield" Height="25px"></FMControls:FMDateTime>
				<FMControls:FMDate ID="Accounting" ToolTip="Accounting Date" Style="left: 350px; position: absolute; top: 64px"
					runat="server" TabIndex="3" CssClass="formfield" Width="160px" Height="25px"></FMControls:FMDate>
				<FMControls:FMButton ID="SaveButton" Style="z-index: 110; left: 564px; position: absolute; top: 50px"
					runat="server" Text="Apply" Width="75px" CssClass="formfieldtitle" TabIndex="5" OnClick="SaveButtonClick"></FMControls:FMButton>
			</div>
		</form>
		<script type="text/javascript">
			document.getElementById("SaveButton").setActive();
			document.getElementById("Operations").focus();
		</script>
	</body>
</html>