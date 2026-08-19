<%@ Page Language="c#" CodeBehind="ProductGroupForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductGroupForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout">
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="ProductGroupTitleLabel"
                Style="z-index: 101; left: 8px; position: absolute; top: 8px" runat="server"
                BackColor="Transparent" CssClass="headline" Width="450px">Product Group Configuration</FMControls:FMLabel>
            <FMControls:FMLabel ID="Label1" AssociatedControlID="Name" Style="z-index: 102; left: 80px; position: absolute; top: 40px" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Product Group ID:</FMControls:FMLabel>
            <FMControls:FMLabel ID="Label8" Style="z-index: 104; left: 224px; position: absolute; top: 40px" runat="server"
                BackColor="Transparent" Height="8px" ForeColor="Crimson" Width="8px">*</FMControls:FMLabel>
            <asp:TextBox ID="Name" Style="z-index: 103; left: 240px; position: absolute; top: 40px" runat="server" aria-required="true"
                BackColor="White" CssClass="formfield" Width="136px" MaxLength="30" ></asp:TextBox>
            <FMControls:FMLabel ID="Label3" AssociatedControlID="AssignedProductsListBox" Style="z-index: 105; left: 80px; position: absolute; top: 72px" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Assigned Products:</FMControls:FMLabel>
            <asp:ListBox ID="AssignedProductsListBox" Style="z-index: 107; left: 80px; position: absolute; top: 88px"
                runat="server" BackColor="White" CssClass="formfield" Height="72px" Width="300px" SelectionMode="Multiple" ></asp:ListBox>
            <asp:Button ID="AssignProductsButton" Style="z-index: 108; left: 392px; position: absolute; top: 90px; padding-left: 1px; padding-right: 1px; width:20px;"
                runat="server" CssClass="formfieldtitle" Text="<<" ToolTip="Assign" ></asp:Button>
            <asp:Button ID="UnassignProductsButton" Style="z-index: 109; left: 392px; position: absolute; top: 128px; padding-left: 1px; padding-right: 1px; width:20px;"
                runat="server" CssClass="formfieldtitle" Text=">>" ToolTip="Unassign" ></asp:Button>
            <FMControls:FMLabel ID="Label4" AssociatedControlID="UnassignedProductsListBox" Style="z-index: 106; left: 424px; position: absolute; top: 72px" runat="server"
                CssClass="formfieldtitle" Width="144px">Unassigned Products:</FMControls:FMLabel>
            <asp:ListBox ID="UnassignedProductsListBox" Style="z-index: 110; left: 424px; position: absolute; top: 88px"
                runat="server" BackColor="White" CssClass="formfield" Height="72px" Width="300px" SelectionMode="Multiple" ></asp:ListBox>
            <FMControls:FMLabel ID="Label9" AssociatedControlID="TypeDropDownList" Style="z-index: 134; left: 80px; position: absolute; top: 176px" CssClass="formfieldtitle"
                runat="server" BackColor="Transparent">Type:</FMControls:FMLabel>
            <FMControls:FMDropDownList ID="TypeDropDownList" Style="z-index: 135; left: 144px; position: absolute; top: 176px"
                CssClass="formfield" Width="240px" runat="server" AutoPostBack="True"  OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged">
            </FMControls:FMDropDownList>
            <FMControls:FMButton ID="UpButton" Style="z-index: 122; left: 32px; position: absolute; top: 232px" runat="server"
                CssClass="formfieldtitle" Width="40px" Text="Up" ToolTip="Move Up" ></FMControls:FMButton>
            <FMControls:FMButton ID="DownButton" Style="z-index: 126; left: 32px; position: absolute; top: 272px;"
                runat="server" CssClass="formfieldtitle" Width="40px" Text="Down" ToolTip="Move Down" ></FMControls:FMButton>
            <FMControls:FMLabel ID="FMLabel1" AssociatedControlID="AssignedMessagesListBox" Style="z-index: 114; left: 80px; position: absolute; top: 200px" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle">Assigned Messages:</FMControls:FMLabel>
            <asp:ListBox ID="AssignedMessagesListBox" Style="z-index: 118; left: 80px; position: absolute; top: 216px"
                runat="server" BackColor="White" CssClass="formfield" Height="112px" Width="648px" SelectionMode="Multiple" ></asp:ListBox>
            <FMControls:FMButton ID="AssignMessagesButton" Style="z-index: 123; left: 328px; position: absolute; top: 335px"
                runat="server" CssClass="formfieldtitle" Text="Assign" Width="75px" ToolTip="Assign"  />
            <FMControls:FMButton ID="UnassignMessagesButton" Style="z-index: 125; left: 424px; position: absolute; top: 335px"
                runat="server" CssClass="formfieldtitle" Text="Unassign" Width="70px" ToolTip="Unassign"  />
            <FMControls:FMLabel ID="FMLabel3" AssociatedControlID="UnassignedMessagesListBox" Style="z-index: 116; left: 80px; position: absolute; top: 360px" runat="server"
                CssClass="formfieldtitle" Width="176px">Unassigned Messages:</FMControls:FMLabel>
            <asp:ListBox ID="UnassignedMessagesListBox" Style="z-index: 120; left: 80px; position: absolute; top: 376px"
                runat="server" BackColor="White" CssClass="formfield" Height="112px" Width="648px" SelectionMode="Multiple" ></asp:ListBox>
            <FMControls:FMButton ID="OK" Style="z-index: 111; left: 576px; position: absolute; top: 496px" runat="server"
                CssClass="formfieldtitle" Width="67px" Text="OK" ></FMControls:FMButton>
            <FMControls:FMButton ID="Cancel" Style="z-index: 112; left: 664px; position: absolute; top: 496px" runat="server"
                CssClass="formfieldtitle" Width="67px" Text="Cancel" ></FMControls:FMButton>
            <FMControls:FMLabel ID="Label10" Style="z-index: 113; left: 584px; position: absolute; top: 528px" runat="server"
                CssClass="formfieldtitle" Height="8px" ForeColor="Crimson" Width="144px">* Denotes Required Field</FMControls:FMLabel>
            <script>
                var okButton = document.getElementById("OK");
                if (!okButton.disabled)
                    okButton.setActive();
            </script>
        </div>
    </form>
</body>
</html>
