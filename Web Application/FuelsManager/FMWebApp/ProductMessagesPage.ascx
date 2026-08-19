<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="ProductMessagesPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductMessagesPage" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
<body>
    <FMControls:FMLabel ID="Label9" AssociatedControlID="TypeDropDownList" Style="z-index: 134; left: 0px; position: absolute; top: 24px" CssClass="formfieldtitle"
        runat="server" BackColor="Transparent">Type:</FMControls:FMLabel>
    <FMControls:FMDropDownList ID="TypeDropDownList" Style="z-index: 135; left: 56px; position: absolute; top: 24px"
        CssClass="formfield" Width="240px" runat="server" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged">
    </FMControls:FMDropDownList>
    <FMControls:FMButton ID="UpButton" Style="z-index: 126; left: 0px; position: absolute; top: 96px" CssClass="formfieldtitle"
        Width="40px" Text="Up" runat="server"></FMControls:FMButton>
    <FMControls:FMButton ID="DownButton" Style="z-index: 127; left: 0px; position: absolute; top: 144px"
        CssClass="formfieldtitle" CssStyle="padding-left:0" Width="40px" Text="Down" runat="server"></FMControls:FMButton>
    <FMControls:FMLabel ID="Label11" AssociatedControlID="AssignedMessagesListBox" Style="z-index: 128; left: 56px; position: absolute; top: 56px" CssClass="formfieldtitle"
        runat="server" BackColor="Transparent">Assigned Messages:</FMControls:FMLabel>
    <asp:ListBox ID="AssignedMessagesListBox" Style="z-index: 129; left: 56px; position: absolute; top: 72px"
        CssClass="formfield" Width="632px" runat="server" SelectionMode="Multiple" Height="112px" 
        Rows="2"></asp:ListBox>
    <FMControls:FMButton ID="AssignMessagesButton" Style="z-index: 131; left: 256px; position: absolute; top: 200px"
        CssClass="formfieldtitle" Text="Assign" runat="server" Width="80px" />
    <FMControls:FMButton ID="UnassignMessagesButton" Style="z-index: 132; left: 392px; position: absolute; top: 200px"
        runat="server" Text="Unassign" Width="80px" CssClass="formfieldtitle" />
    <FMControls:FMLabel ID="Label12" AssociatedControlID="UnassignedMessagesListBox" Style="z-index: 130; left: 56px; position: absolute; top: 224px" CssClass="formfieldtitle"
        Width="203px" runat="server">Unassigned Messages:</FMControls:FMLabel>
    <asp:ListBox ID="UnassignedMessagesListBox" Style="z-index: 133; left: 56px; position: absolute; top: 240px"
        CssClass="formfield" Width="632px" runat="server" SelectionMode="Multiple" Height="112px"></asp:ListBox>
</body>
</HTML>
