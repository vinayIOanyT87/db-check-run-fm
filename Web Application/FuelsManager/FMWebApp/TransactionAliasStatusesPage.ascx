<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TransactionAliasStatusesPage.ascx.cs" Inherits="FuelsManager.FMWebApp.TransactionAliasStatusesPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<div style="Z-INDEX: 101; LEFT: 50px; POSITION: absolute; TOP: 50px">
	<table cellpadding="1" cellspacing="1" border="0" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 0px" role="presentation" aria-label="layout">
		<tr>
			<td>
				<FMControls:FMLabel runat="server" ID="labAssigned" AssociatedControlID="lbxAssigned" Text="Assigned Statuses" CssClass="formfieldtitle" />
			</td>
			<td>&nbsp;</td>
			<td>
				<FMControls:FMLabel runat="server" ID="labAvailable" AssociatedControlID="lbxAvailable" Text="Unassigned Statuses" CssClass="formfieldtitle" />
			</td>
		</tr>
		<tr>
			<td>
				<FMControls:FMListBox Runat="server" ID="lbxAssigned" CssClass="formfield" Width="150px" Height="200px" SelectionMode="Multiple"></FMControls:FMListBox>
			</td>
			<td align="center" valign="middle">
				<FMControls:FMButton Runat="server" ID="btnAssign" style="width:20px;" Text="<<" CssClass="formfieldtitle" onclick="btnAssign_Click"></FMControls:FMButton><br><br>
				<FMControls:FMButton Runat="server" ID="btnUnassign" style="width:20px;" Text=">>" CssClass="formfieldtitle" onclick="btnUnassign_Click"></FMControls:FMButton><br><br>
			</td>
			<td>
				<FMControls:FMListBox Runat="server" ID="lbxAvailable" CssClass="formfield" Width="150px" Height="200px" SelectionMode="Multiple"></FMControls:FMListBox>
			</td>
		</tr>
		<tr>
			<td colspan="3">
			    <FMControls:FMLabel runat="server" AssociatedControlID="ddlDefaultStatus" ID="labDefaultStatus" Text="Default Status" CssClass="formfieldtitle" />
			</td>
		</tr>
		<tr>
			<td colspan="3">
			    <FMControls:FMDropDownList runat="server" ID="ddlDefaultStatus" CssClass="formfield" Width="150px" Height="20px" 
			    onselectedindexchanged="ddlDefaultStatus_SelectedIndexChanged" Sort="false" />
			</td>
		</tr>
	</table>
</div>
