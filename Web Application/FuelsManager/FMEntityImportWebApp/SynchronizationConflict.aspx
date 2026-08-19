<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SynchronizationConflict.aspx.cs" Inherits="FuelsManager.FMEntityImportWebApp.SynchronizationConflict" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>

    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/MenuBar/menu.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Content/bootstrap.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/autocomplete.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/json2.js" %>"></script>

</head>
<body tabindex="-1" ms_positioning="GridLayout">
    <form id="SynchronizationConflict" method="post" runat="server">
    <div style="position: absolute">
        <asp:ScriptManager ID="ScriptManager" runat="server" />
        <asp:Image ID="Image1" Style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
            BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
        <FMControls:FMLabel ID="SynchronizationDetailsLabel" Style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
            CssClass="headline" Width="720px" BackColor="Transparent">Synchronization Conflict / Error Details</FMControls:FMLabel>
        <table id="Table1" style="Z-INDEX: 113; LEFT: 24px; WIDTH: 750px; POSITION: absolute; TOP: 50px; HEIGHT: 10px"
            cellspacing="0" cellpadding="1" border="0">
           <tr>
                <td style="height: 30px;">
            <FMControls:FMCheckBox runat="server" Text="Unique Identifiers" ID="UniqueIdentifiersCheckbox" Checked="False" CssClass="formfieldtitle"
                style="Z-INDEX: 120;" TabIndex="11"
                OnCheckedChanged="UniqueIdentifiers_CheckBoxChanged" AutoPostBack="True"/>    
                </td>
            </tr>
           <tr>
                <td style="height: 30px;">
					 <FMControls:FMButton ID="ClearButton" Style="Z-INDEX: 107;" TabIndex="100"
						 runat="server" CssClass="formfieldtitle" Width="67px" Text="Clear" OnClientClick="window.close();window.returnValue=true;"></FMControls:FMButton>
                </td>
            </tr>
           <tr>
                <td style="height: 30px;">
                    <FMControls:FMLabel ID="ConflictParametersLabel" Style="Z-INDEX: 118;" runat="server" CssClass="formfieldtitle">Parameters :</FMControls:FMLabel>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;">
                        <FMControls:FMGridView ID="SyncConflictDataGrid" runat="server" CssClass="tabletext"
                        BackColor="White" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
                        OnPageIndexChanging="SyncConflictDataGrid_OnPageIndexChanging"
                        CellPadding="3" PageSize="8" EnableViewState="true">
                        <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                        <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                        <Columns>
                            <asp:BoundField HeaderText="Name" DataField="Key" ItemStyle-Width="120px"></asp:BoundField>
                            <asp:BoundField HeaderText="Value" DataField="Value" ItemStyle-Width="300px"></asp:BoundField>
                            <asp:BoundField HeaderText="ReferenceTable" DataField="ReferenceTable" ItemStyle-Width="300px"></asp:BoundField>
                        </Columns>
                        <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></PagerStyle>
                    </FMControls:FMGridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
