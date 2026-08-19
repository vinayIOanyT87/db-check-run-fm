<%@ Page language="c#" Codebehind="TankDetailForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TankDetailForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body bgColor="#ffffff" MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="TankDetail" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<asp:image id="DetailBar6" style="Z-INDEX: 136; LEFT: 496px; POSITION: absolute; TOP: 134px"
				tabIndex="-1" runat="server" Height="56px" Width="41px" BorderStyle="Solid" BorderColor="Gray"
				BorderWidth="1px" ImageUrl="images\barbck.gif"></asp:image>
			<asp:image id="DetailBarbackground6" style="Z-INDEX: 135; LEFT: 496px; POSITION: absolute; TOP: 134px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" BackColor="#C0FFFF"
				Width="41px" Height="56px" ImageUrl="images\bar.gif"></asp:image>
			<asp:image id="DetailBarbackground5" style="Z-INDEX: 129; LEFT: 464px; POSITION: absolute; TOP: 100px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" BackColor="#C0FFFF"
				Width="40px" Height="138px" ImageUrl="images\bar.gif"></asp:image>
			<asp:image id="DetailBar5" style="Z-INDEX: 131; LEFT: 464px; POSITION: absolute; TOP: 100px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" Width="40px"
				Height="138px" ImageUrl="images\barbck.gif"></asp:image>
			<asp:image id="DetailBarbackground4" style="Z-INDEX: 132; LEFT: 352px; POSITION: absolute; TOP: 179px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" BackColor="#C0FFFF"
				Width="41px" Height="56px" ImageUrl="images\bar.gif"></asp:image>
			<asp:image id="DetailBar4" style="Z-INDEX: 133; LEFT: 352px; POSITION: absolute; TOP: 179px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" Width="41px"
				Height="56px" ImageUrl="images\barbck.gif"></asp:image>
			<asp:image id="DetailBarbackground3" style="Z-INDEX: 128; LEFT: 464px; POSITION: absolute; TOP: 104px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" BackColor="#C0FFFF"
				Width="40px" Height="138px" ImageUrl="images\bar.gif"></asp:image>
			<asp:image id="DetailBar3" style="Z-INDEX: 130; LEFT: 464px; POSITION: absolute; TOP: 104px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" Width="40px"
				Height="138px" ImageUrl="images\barbck.gif"></asp:image>
			<asp:image id="DetailBarbackground7" style="Z-INDEX: 126; LEFT: 480px; POSITION: absolute; TOP: 134px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" BackColor="#C0FFFF"
				Width="41px" Height="96px" ImageUrl="images\bar.gif"></asp:image>
			<asp:image id="DetailBar7" style="Z-INDEX: 127; LEFT: 480px; POSITION: absolute; TOP: 134px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" Width="41px"
				Height="96px" ImageUrl="images\barbck.gif"></asp:image>
			<asp:image id="DetailBarbackground1" style="Z-INDEX: 126; LEFT: 376px; POSITION: absolute; TOP: 148px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" BackColor="#C0FFFF"
				Width="41px" Height="56px" ImageUrl="images\bar.gif"></asp:image>
			<asp:image id="DetailBar1" style="Z-INDEX: 127; LEFT: 376px; POSITION: absolute; TOP: 148px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" Width="41px"
				Height="56px" ImageUrl="images\barbck.gif"></asp:image>
			<asp:label id="DensityText" style="Z-INDEX: 122; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 260px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="#E6E7E8" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="TempText" style="Z-INDEX: 123; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 242px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="White" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="MassText" style="Z-INDEX: 117; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 224px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="#E6E7E8" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="NetVolumeText" style="Z-INDEX: 118; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 206px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="White" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="GrossVolumeText" style="Z-INDEX: 116; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 188px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="#E6E7E8" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="MinVolumeText" style="Z-INDEX: 115; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 170px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="White" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="MaxVolumeText" style="Z-INDEX: 114; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 152px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="#E6E7E8" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="LevelText" style="Z-INDEX: 113; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 134px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="White" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="LastUpdateText" style="Z-INDEX: 112; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 116px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="#E6E7E8" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="StatusText" style="Z-INDEX: 111; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 98px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="White" Width="140px" Height="18px">Site</asp:label>
			<asp:label id="ProductText" style="Z-INDEX: 111; LEFT: 160px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 80px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tabletext"
				BorderStyle="Solid" BackColor="#E6E7E8" Width="140px" Height="18px">Site</asp:label>
			<FMControls:FMLabel id="Label24" style="Z-INDEX: 121; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 260px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="#E6E7E8" Width="128px" Height="18px">Density</FMControls:FMLabel>
			<FMControls:FMLabel id="Label22" style="Z-INDEX: 120; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 242px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="White" Width="128px" Height="18px">Temperature</FMControls:FMLabel>
			<FMControls:FMLabel id="Label9" style="Z-INDEX: 109; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 224px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="#E6E7E8" Width="128px" Height="18px">Mass</FMControls:FMLabel>
			<FMControls:FMLabel id="Label8" style="Z-INDEX: 108; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 206px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="White" Width="128px" Height="18px">Net Volume</FMControls:FMLabel>
			<FMControls:FMLabel id="Label7" style="Z-INDEX: 107; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 188px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="#E6E7E8" Width="128px" Height="18px">Gross Volume</FMControls:FMLabel>
			<FMControls:FMLabel id="Label6" style="Z-INDEX: 106; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 170px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="White" Width="128px" Height="18px">Min Volume</FMControls:FMLabel>
			<FMControls:FMLabel id="Label5" style="Z-INDEX: 105; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 152px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="#E6E7E8" Width="128px" Height="18px">Max Volume</FMControls:FMLabel>
			<FMControls:FMLabel id="Label4" style="Z-INDEX: 104; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 134px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="White" Width="128px" Height="18px">Level</FMControls:FMLabel>
			<FMControls:FMLabel id="Label3" style="Z-INDEX: 103; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 116px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="#E6E7E8" Width="128px" Height="18px">Last Level Update</FMControls:FMLabel>
			<FMControls:FMLabel id="FMLabel1" style="Z-INDEX: 137; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 98px; line-height: 18px;"
				tabIndex="-1" runat="server" Height="18px" Width="128px" BackColor="White" BorderStyle="Solid" CssClass="tablerowhead"
				BorderColor="White" BorderWidth="1px">Status</FMControls:FMLabel>
			<FMControls:FMLabel id="Label2" style="Z-INDEX: 102; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 80px; line-height: 18px;"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="White" CssClass="tablerowhead" BorderStyle="Solid"
				BackColor="#E6E7E8" Width="128px" Height="18px">Product</FMControls:FMLabel>
			<asp:label id="InventoryInfo" style="Z-INDEX: 134; LEFT: 32px; TEXT-INDENT: 2px; POSITION: absolute; TOP: 40px"
				tabIndex="-1" runat="server" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Width="616px" Height="18px"
				BorderStyle="Solid" BorderColor="<%$ AppSettings: ColorHeaderBlue %>" BorderWidth="2px">Label</asp:label>
			<asp:image id="TankImage" style="Z-INDEX: 119; LEFT: 312px; POSITION: absolute; TOP: 64px"
				tabIndex="-1" runat="server" Width="320px" Height="216px" BackColor="Transparent" ImageUrl="~/FMWebApp/images/ffs_0x_0240_g.gif"></asp:image>
			<asp:image id="DetailBarbackground" style="Z-INDEX: 124; LEFT: 392px; POSITION: absolute; TOP: 112px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" BackColor="#C0FFFF"
				Width="41px" Height="144px" ImageUrl="images\bar.gif"></asp:image>
			<asp:image id="DetailBar" style="Z-INDEX: 125; LEFT: 392px; POSITION: absolute; TOP: 112px"
				tabIndex="-1" runat="server" BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid" Width="41px"
				Height="144px" ImageUrl="images\barbck.gif"></asp:image>
			<asp:image id="Image1" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
			<FMControls:FMLabel id="Label11" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" BackColor="Transparent" Width="208px">Inventory Management</FMControls:FMLabel>
		</div>
</form>
	</body>
</HTML>
