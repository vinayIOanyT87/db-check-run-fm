<%@ Page language="c#" Codebehind="OPCConnectionForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.OPCConnectionForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<base target="_self">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <script>
            function saveScroll() {
                if (document.getElementById('ItemsTreeView') != null) {
                    document.getElementById('__SAVESCROLLVERT').value = document.getElementById('ItemsTreeView').scrollWidth;
                    document.getElementById('__SAVESCROLLHORZ').value = document.getElementById('ItemsTreeView').scrollTop;
                }
            }

		function restoreScroll() {
			if (document.getElementById('ItemsTreeView') != null) {
				document.getElementById('ItemsTreeView').scrollWidth = document.getElementById('__SAVESCROLLVERT').value;
				document.getElementById('ItemsTreeView').scrollTop = document.getElementById('__SAVESCROLLHORZ').value;
			}
		}


		window.onload = restoreScroll;
	</script>
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<form id="Form1" method="post" runat="server" onsubmit="saveScroll()">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label8" Style="z-index: 120; left: 8px; position: absolute; top: 8px" runat="server"
				Width="312px" CssClass="headline" BackColor="Transparent">OPC Connection Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label3" AssociatedControlID="ProcessVariableIDTextBox" Style="z-index: 107; left: 32px; position: absolute; top: 56px" runat="server"
				CssClass="formfieldtitle">Process Variable:</FMControls:FMLabel>
			<asp:TextBox ID="ProcessVariableIDTextBox" Style="z-index: 108; left: 184px; position: absolute; top: 56px"
				runat="server" CssClass="formfield" Width="248px" ReadOnly="True" Enabled="False"></asp:TextBox>
			<FMControls:FMLabel ID="Label4" AssociatedControlID="MaximumTextBox" Style="z-index: 109; left: 440px; position: absolute; top: 56px" runat="server"
				CssClass="formfieldtitle">Maximum:</FMControls:FMLabel>
			<asp:TextBox ID="MaximumTextBox" Style="z-index: 110; left: 504px; position: absolute; top: 56px"
				runat="server" BackColor="White" CssClass="formfield" Width="107px"></asp:TextBox>
			<FMControls:FMLabel ID="Label6" AssociatedControlID="DataTypeDropDownList" Style="z-index: 113; left: 32px; position: absolute; top: 88px" runat="server"
				CssClass="formfieldtitle">Data Type:</FMControls:FMLabel>
			<asp:DropDownList ID="DataTypeDropDownList" Style="z-index: 114; left: 184px; position: absolute; top: 88px"
				runat="server" AutoPostBack="True" BackColor="White" CssClass="formfield" Width="248px" OnSelectedIndexChanged="DataTypeDropDownListSelectedIndexChanged">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label5" AssociatedControlID="MinimumTextBox" Style="z-index: 111; left: 440px; position: absolute; top: 88px" runat="server"
				CssClass="formfieldtitle">Minimum:</FMControls:FMLabel>
			<asp:TextBox ID="MinimumTextBox" Style="z-index: 112; left: 504px; position: absolute; top: 88px"
				runat="server" BackColor="White" CssClass="formfield" Width="106px"></asp:TextBox>
			<FMControls:FMLabel ID="Label11" AssociatedControlID="SystemDropDownList" Style="z-index: 124; left: 32px; position: absolute; top: 120px" runat="server"
				CssClass="formfieldtitle">System:</FMControls:FMLabel>
			<FMControls:FMDropDownList ID="SelectSystemModeDropDownList" ToolTip="System Mode" Style="z-index: 125; left: 184px; position: absolute; top: 120px"
				runat="server" Width="58px" CssClass="formfield" Height="24px" AutoPostBack="True" OnSelectedIndexChanged="SelectSystemModeDropDownListSelectedIndexChanged">
			</FMControls:FMDropDownList>
			<asp:DropDownList ID="SystemDropDownList" Style="z-index: 125; left: 254px; position: absolute; top: 120px"
				runat="server" Width="178px" CssClass="formfield" BackColor="White" AutoPostBack="True" OnSelectedIndexChanged="SystemDropDownListSelectedIndexChanged">
			</asp:DropDownList>
			<asp:TextBox ID="SystemTextBox" ToolTip="System Textbox" Style="z-index: 125; left: 254px; position: absolute; top: 120px"
				runat="server" Width="178px" CssClass="formfield" AutoPostBack="True" MaxLength="80"></asp:TextBox>
			<FMControls:FMLabel ID="Label1" AssociatedControlID="OPCServerDropDownList" Style="z-index: 102; left: 32px; position: absolute; top: 152px" runat="server"
				CssClass="formfieldtitle" Width="80px">OPC Server:</FMControls:FMLabel>
			<asp:DropDownList ID="OPCServerDropDownList" Style="z-index: 101; left: 184px; position: absolute; top: 152px"
				runat="server" AutoPostBack="True" BackColor="White" CssClass="formfield" Width="248px" OnSelectedIndexChanged="OpcServerDropDownListSelectedIndexChanged">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label7" AssociatedControlID="OPCItemIDTextBox" Style="z-index: 115; left: 32px; position: absolute; top: 184px" runat="server"
				CssClass="formfieldtitle">OPC Item ID:</FMControls:FMLabel>
			<asp:TextBox ID="OPCItemIDTextBox" Style="z-index: 116; left: 184px; position: absolute; top: 184px"
				runat="server" CssClass="formfield" Width="248px"></asp:TextBox>
			<FMControls:FMLabel ID="Label10" AssociatedControlID="ServerEngineeringUnitsDropDownList" Style="z-index: 121; left: 32px; position: absolute; top: 216px" runat="server"
				CssClass="formfieldtitle">Server Engineering Units:</FMControls:FMLabel>
			<asp:DropDownList ID="ServerEngineeringUnitsDropDownList" Style="z-index: 122; left: 184px; position: absolute; top: 216px"
				runat="server" Width="128px" CssClass="formfield" BackColor="White" AutoPostBack="True">
			</asp:DropDownList>
			<FMControls:FMLabel ID="FMLabel1" AssociatedControlID="MessageDropDownList" Style="z-index: 126; left: 32px; position: absolute; top: 248px" runat="server"
				CssClass="formfieldtitle">Associated Message:</FMControls:FMLabel>
			<asp:DropDownList ID="MessageDropDownList" Style="z-index: 127; left: 184px; position: absolute; top: 248px"
				runat="server" Width="248px" CssClass="formfield" BackColor="White" AutoPostBack="True">
			</asp:DropDownList>
			<FMControls:FMLabel ID="AvailableOPCItemsLabel" Style="z-index: 104; left: 32px; position: absolute; top: 280px"
				runat="server" CssClass="formfieldtitle" Width="168px">Available OPC Items:</FMControls:FMLabel>
			<FMControls:FMLabel ID="DataTypeFilterLabel" AssociatedControlID="DataTypeFilterDropDownList" Style="z-index: 118; left: 32px; position: absolute; top: 312px"
				runat="server" CssClass="formfieldtitle" Width="120px">Data Type Filter:</FMControls:FMLabel>
			<asp:DropDownList ID="DataTypeFilterDropDownList" Style="z-index: 119; left: 184px; position: absolute; top: 312px"
				runat="server" AutoPostBack="True" BackColor="White" CssClass="formfield" Width="128px" OnSelectedIndexChanged="DataTypeFilterDropDownListSelectedIndexChanged">
			</asp:DropDownList>
			<asp:TreeView ID="ItemsTreeView" ToolTip="Available OPC Items" BackColor="Transparent" Style="overflow: auto; z-index: 103; left: 32px; position: absolute; top: 344px"
				runat="server" AutoPostBack="True" Width="208px" Height="118px" SelectedNodeStyle-BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px" ForeColor="Black" CssClass="formfield">
			</asp:TreeView>
			<asp:ListBox ID="ItemsHeirarchicalListBox" Style="z-index: 117; left: 240px; position: absolute; top: 344px"
				runat="server" AutoPostBack="True" Width="368px" Height="120px" SelectionMode="Multiple" CssClass="formfield" OnSelectedIndexChanged="ItemsHeirarchicalListBoxSelectedIndexChanged"></asp:ListBox>
			<FMControls:FMButton ID="OK" Style="z-index: 105; left: 440px; position: absolute; top: 474px" runat="server"
				Width="67px" Text="OK" CssClass="formfieldtitle"></FMControls:FMButton>
			<FMControls:FMButton ID="Cancel" Style="z-index: 106; left: 544px; position: absolute; top: 474px" runat="server"
				Text="Cancel" CssClass="formfieldtitle"></FMControls:FMButton>
			<FMControls:FMButton id="HiddenOk" style="Z-INDEX: 105; LEFT: 440px; POSITION: absolute; TOP: 478px" runat="server"
				Width="67px" Text="HiddenOK" CssClass="formfieldtitle" visible="false"></FMControls:FMButton>
			<input id="__SAVESCROLLVERT" name="__SAVESCROLLVERT" value="0" type="hidden" runat="server" />
			<input id="__SAVESCROLLHORZ" name="__SAVESCROLLHORZ" value="0" type="hidden" runat="server" />
			<script>
			    var okButton = document.getElementById("OK");
			    if (!okButton.disabled)
			        okButton.setActive();
				document.getElementById("HiddonOk").setActive();
			</script>
		</div>
</form>
	</body>
</HTML>
