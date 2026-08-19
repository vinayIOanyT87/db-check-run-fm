<%@ Page Language="c#" AutoEventWireup="True" Codebehind="ViewsForm.aspx.cs" Inherits="FuelsManager.FMWebApp.ViewsForm" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
        <title></title>
        <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
        <meta content="C#" name="CODE_LANGUAGE">
        <meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
        <form id="Form1" method="post" enctype="multipart/form-data" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="ConfigurationLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px"
                    runat="server" CssClass="headline" BackColor="Transparent" Width="718px">Views Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label1" AssociatedControlID="TypeDropDownList" Style="z-index: 116; left: 24px; position: absolute; top: 48px" runat="server"
                    CssClass="formfieldtitle">Type:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="TypeDropDownList" Style="z-index: 108; left: 104px; position: absolute; top: 48px"
                    TabIndex="1" runat="server" CssClass="formfield" AutoPostBack="True" Width="464px" OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged">
                </FMControls:FMDropDownList>
                <FMControls:FMLabel ID="Fmlabel1" AssociatedControlID="ViewsDropDownList" Style="z-index: 116; left: 24px; position: absolute; top: 80px" runat="server"
                    CssClass="formfieldtitle">Views:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="ViewsDropDownList" Style="z-index: 108; left: 104px; position: absolute; top: 80px"
                    TabIndex="2" runat="server" CssClass="formfield" AutoPostBack="True" Width="464px" OnSelectedIndexChanged="ViewsDropDownListSelectedIndexChanged">
                </FMControls:FMDropDownList>
                <FMControls:FMButton ID="UpButton" Style="z-index: 118; left: 35px; position: absolute; top: 208px; min-width:50px" TabIndex="3"
                    runat="server" CssClass="formfieldtitle" Width="36px" Text="Up"></FMControls:FMButton>
                <FMControls:FMButton ID="DownButton" Style="z-index: 118; left: 35px; position: absolute; top: 250px; min-width:50px"
                    TabIndex="4" runat="server" CssClass="formfieldtitle" Text="Down"></FMControls:FMButton>
                <FMControls:FMLabel ID="Label7" AssociatedControlID="AssignedColumnsListBox" Style="z-index: 122; left: 104px; position: absolute; top: 120px" runat="server"
                    CssClass="formfieldtitle">Assigned Columns:</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label4" AssociatedControlID="UnassignedColumnsListBox" Style="z-index: 113; left: 368px; position: absolute; top: 120px" runat="server"
                    CssClass="formfieldtitle" Width="152px">Unassigned Columns:</FMControls:FMLabel>
                <asp:ListBox ID="AssignedColumnsListBox" Style="z-index: 108; left: 104px; position: absolute; top: 152px"
                    TabIndex="5" runat="server" CssClass="formfield" Width="208px" SelectionMode="Multiple" Height="192px"></asp:ListBox>
                <FMControls:FMButton ID="AssignColumnsButton" Style="z-index: 115; left: 328px; position: absolute; top: 208px; width:20px"
                    TabIndex="6" runat="server" CssClass="formfieldtitle" Text="<<" />
                <FMControls:FMButton ID="UnassignColumnsButton" Style="z-index: 118; left: 328px; position: absolute; top: 250px; width:20px"
                    TabIndex="7" runat="server" CssClass="formfieldtitle" Text=">>" OnClick="UnassignColumnsButtonClick" />
                <asp:ListBox ID="UnassignedColumnsListBox" Style="z-index: 110; left: 368px; position: absolute; top: 152px"
                    TabIndex="8" runat="server" CssClass="formfield" Width="208px" SelectionMode="Multiple" Height="192px"></asp:ListBox>
                <FMControls:FMButton ID="SaveButton" Style="z-index: 118; left: 470px; position: absolute; top: 352px; min-width:100px"
                    TabIndex="9" runat="server" CssClass="formfieldtitle" Text="Apply"></FMControls:FMButton>
                <ajaxToolkit:ConfirmButtonExtender ID="cbeApply" runat="server" TargetControlID="SaveButton" Enabled="false"
                    ConfirmText="You are configuring a View against a child record version of a Transaction Alias. Doing so will prevent Transaction Alias Record Versioning from being turned off at the parent sitegroup. Are you sure you want to proceed with this View definition?" />
                <FMControls:FMButton ID="CreateDefaultViewsButton" Style="z-index: 118; left: 35px; position: absolute; top: 400px; min-width:130px"
                    TabIndex="9" runat="server" CssClass="formfieldtitle" Text="Create Default Views"></FMControls:FMButton>
                <script>
                    document.getElementById("TypeDropDownList").focus()
                </script>
            </div>
        </form>
	</body>
</HTML>
