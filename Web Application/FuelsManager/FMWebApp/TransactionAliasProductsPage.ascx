<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TransactionAliasProductsPage.ascx.cs" Inherits="FuelsManager.FMWebApp.TransactionAliasProductsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<FMControls:FMButton id="UnexcludeProductsButton" runat="server" Width="25px" CssClass="formfieldtitle"
	Text=">>" style="Z-INDEX: 111; LEFT: 165px; POSITION: absolute; TOP: 145px"></FMControls:FMButton>
<FMControls:FMButton id="ExcludeProductsButton" runat="server" Width="25px" CssClass="formfieldtitle"
	Text="<<" style="Z-INDEX: 111; LEFT: 165px; POSITION: absolute; TOP: 105px"></FMControls:FMButton>
<FMControls:FMLabel id="Fmlabel1" AssociatedControlID="UnexcludedProductsListBox" style="Z-INDEX: 112; LEFT: 208px; POSITION: absolute; TOP: 16px" runat="server"
	CssClass="formfieldtitle" Width="144px">Included Products:</FMControls:FMLabel>
<FMControls:FMLabel id="Label3" AssociatedControlID="ExcludedProductsListBox" style="Z-INDEX: 111; LEFT: 0px; POSITION: absolute; TOP: 16px" runat="server"
	BackColor="Transparent" CssClass="formfieldtitle">Excluded Products:</FMControls:FMLabel>
<asp:listbox id="ExcludedProductsListBox" style="Z-INDEX: 109; LEFT: 0px; POSITION: absolute; TOP: 48px"
	runat="server" BackColor="White" CssClass="formfield" Width="152px" SelectionMode="Multiple"
	Height="208px" tabIndex="8"></asp:listbox>
<asp:listbox id="UnexcludedProductsListBox" style="Z-INDEX: 110; LEFT: 208px; POSITION: absolute; TOP: 48px"
	runat="server" BackColor="White" CssClass="formfield" Width="152px" SelectionMode="Multiple"
	Height="208px" tabIndex="9"></asp:listbox>

