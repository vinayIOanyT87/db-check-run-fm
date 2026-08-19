<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TransactionAliasAssociationsPage.ascx.cs" Inherits="FuelsManager.FMWebApp.TransactionAliasAssociationsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<div style="Z-INDEX: 101; LEFT: 50px; POSITION: absolute; TOP: 25px">
	<table cellpadding="1" cellspacing="1" border="0" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 0px" role="presentation" aria-label="layout">
		<tr>
			<td>
				<FMControls:FMLabel runat="server" ID="labAvailable" AssociatedControlID="lbxAssociated" Text="Associated Aliases" CssClass="formfieldtitle" />
			</td>
			<td>&nbsp;</td>
			<td>
				<FMControls:FMLabel runat="server" ID="labAssigned" AssociatedControlID="lbxAvailable" Text="Available Aliases" CssClass="formfieldtitle" />
			</td>
		</tr>
		<tr>
			<td>
				<asp:ListBox Runat="server" ID="lbxAssociated" CssClass="formfield" Width="200px" Height="250px"
					SelectionMode="Multiple"></asp:ListBox>
			</td>
			<td align="center" valign="middle">
				<FMControls:FMButton Runat="server" ID="btnAssign" style="width:20px; margin-bottom: 10px;" Text="<<" CssClass="formfield" onclick="BtnAssignClick"></FMControls:FMButton><br>
				<FMControls:FMButton Runat="server" ID="btnUnassign" style="width:20px;" Text=">>" CssClass="formfield" onclick="BtnUnassignClick"></FMControls:FMButton>
			</td>
			<td>
				<asp:ListBox Runat="server" ID="lbxAvailable" CssClass="formfield" Width="200px" Height="250px"
					SelectionMode="Multiple"></asp:ListBox>
			</td>
		</tr>
	</table>
	<table cellpadding="1" cellspacing="1" border="0" width="100%" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 300px" role="presentation" aria-label="layout">
		<tr>
			<td nowrap>
				<FMControls:FMCheckBox ID="chkAggregate" Runat="server" Text="Aggregate Associated Transactions" CssClass="formfieldtitle" />
			</td>
			<td nowrap>
				<FMControls:FMCheckBox ID="chkTotalQtyWarning" runat="server" Text="Enable Total Quantity Exceeded Warning"
					CssClass="formfieldtitle" />
			</td>
		</tr>
		<tr>
			<td nowrap>
				<FMControls:FMCheckBox ID="chkTotalValueWarning" runat="server" Text="Enable Total Value Exceeded Warning"
					CssClass="formfieldtitle" />
			</td>
			<td nowrap>
				<FMControls:FMCheckBox ID="chkQtyToleranceWarning" runat="server" Text="Enable Quantity Tolerance Exceeded Warning"
					CssClass="formfieldtitle" />
			</td>
		</tr>
		<tr>
			<td nowrap>
				<FMControls:FMCheckBox id="chkValueToleranceWarning" runat="server" Text="Enable Value Tolerance Exceeded Warning"
					CssClass="formfieldtitle" />
			</td>
			<td>&nbsp;</td>
		</tr>
	</table>
</div>
