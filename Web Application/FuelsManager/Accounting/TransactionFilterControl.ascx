<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TransactionFilterControl.ascx.cs" Inherits="FuelsManager.Accounting.TransactionFilterControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table border="0" cellpadding="0" cellspacing="0" width="100%">
	<tr>
		<td valign="top">
			<asp:Table ID="tblFilter" Runat="server" Width="100%"></asp:Table>
		</td>
		<td valign="top">
			<FMControls:FMButton id="btnRefresh" runat="server" Text="Refresh" Width="67px" 
                onclick="BtnRefreshClick" CssClass="formfieldtitle"></FMControls:FMButton>
		</td>
	</tr>
</table>
