<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseDataExportImportSettings.aspx.cs"
    Inherits="FMWebApp.EnterpriseDataExportImportSettings" %>


      <%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
    <asp:Image ID="Image1" Style="z-index: 101; left: 0px; position: absolute; top: 0px"
        runat="server" ImageUrl="images\fade.jpg"></asp:Image>
    <FMControls:FMLabel ID="Label6" Style="z-index: 103; left: 8px; position: absolute;
        top: 8px" runat="server" CssClass="headline" Width="400px" BackColor="Transparent">Enterprise Data Export/Import Settings</FMControls:FMLabel>
        
 
    </div>
</form>
</body>
</html>
