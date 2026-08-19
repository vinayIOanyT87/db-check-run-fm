<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TransactionAliasEquipmentPage.ascx.cs" Inherits="FuelsManager.FMWebApp.TransactionAliasEquipmentPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<FMControls:FMLabel id="Fmlabel1" AssociatedControlID="EquipmentDropDownList" style="Z-INDEX: 125; LEFT: 16px; POSITION: absolute; TOP: 32px" runat="server"
	CssClass="formfieldtitle">Equipment</FMControls:FMLabel>
<FMControls:FMDropDownList id="EquipmentDropDownList" style="Z-INDEX: 125; LEFT: 16px; POSITION: absolute; TOP: 56px"
	runat="server" Width="192px" CssClass="formfield" AutoPostBack="True" onselectedindexchanged="EquipmentDropDownListSelectedIndexChanged"></FMControls:FMDropDownList>
<FMControls:FMLabel id="Fmlabel2" AssociatedControlID="AssignedTypesListBox" style="Z-INDEX: 125; LEFT: 16px; POSITION: absolute; TOP: 88px" runat="server"
	CssClass="formfieldtitle">Assigned Equipment Types:</FMControls:FMLabel>
<FMControls:FMListBox id="AssignedTypesListBox" style="Z-INDEX: 109; LEFT: 16px; POSITION: absolute; TOP: 112px"
	runat="server" BackColor="White" CssClass="formfield" Width="152px" SelectionMode="Multiple" Height="126px"
	tabIndex="8"></FMControls:FMListBox>
<FMControls:FMLabel id="Fmlabel3" AssociatedControlID="UnassignedTypesListBox" style="Z-INDEX: 125; LEFT: 216px; POSITION: absolute; TOP: 88px" runat="server"
	CssClass="formfieldtitle">Unassigned Equipment Types:</FMControls:FMLabel>
<FMControls:FMListBox id="UnassignedTypesListBox" style="Z-INDEX: 109; LEFT: 216px; POSITION: absolute; TOP: 112px"
	runat="server" BackColor="White" CssClass="formfield" Width="152px" SelectionMode="Multiple" Height="126px"
	tabIndex="8"></FMControls:FMListBox>
<FMControls:FMButton id="UnassignButton" CssClass="formfieldtitle" runat="server" Width="25px" Text="<<"
	style="Z-INDEX: 125; LEFT: 180px; POSITION: absolute; TOP: 140px"></FMControls:FMButton>
<FMControls:FMButton id="AssignButton" CssClass="formfieldtitle" runat="server" Width="25px" Text=">>"
	style="Z-INDEX: 125; LEFT: 180px; POSITION: absolute; TOP: 180px"></FMControls:FMButton>
