<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="CompanyCustomerBillToPage.ascx.cs" AutoEventWireup="True" Inherits="FMWebApp.CompanyCustomerBillToPage" %>
<html>
<head>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body>
    <FMControls:FMLabel ID="Label4" AssociatedControlID="TypeDropDownList" Style="z-index: 105; left: 0px; position: absolute; top: 16px" runat="server" Width="72px" CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
    <asp:DropDownList ID="TypeDropDownList" Style="z-index: 106; left: 72px; position: absolute; top: 16px" runat="server" Width="182px" CssClass="formfield"></asp:DropDownList>
</body>
</html>
