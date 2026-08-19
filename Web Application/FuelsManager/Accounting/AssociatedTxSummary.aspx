<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="AssociatedTxSummary.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.AssociatedTxSummary" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<div style="LEFT: 0px; POSITION: absolute; TOP: 0px"><asp:image id="FadeImage" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
					ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image><FMCONTROLS:FMLABEL id="labInvoiceSummary" style="Z-INDEX: 101; LEFT: 10px; POSITION: absolute; TOP: 10px"
					runat="server" Text="Invoice Associated Transactions" CssClass="headline" Width="216px">Associated Transactions</FMCONTROLS:FMLABEL>
				<table style="Z-INDEX: 101; LEFT: 10px; POSITION: absolute; TOP: 40px" cellSpacing="1"
					cellPadding="1" border="0">
					<tr>
						<td>&nbsp;</td>
					</tr>
					<tr>
						<td><FMCONTROLS:FMBaseDataGrid id="dgTransactions" BackColor="White" CssClass="tabletext" AutoGenerateColumns="False"
								AllowPaging="True" BorderStyle="None" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
								BorderColor="White" CellPadding="3" PageSize="8" Width="736px" Runat="server">
								<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" Wrap="False" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Edit">
										<HeaderStyle Width="55px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMEditLinkButton runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMBaseDataGrid></td>
					</tr>
					<tr>
						<td>&nbsp;</td>
					</tr>
					<tr>
						<td align="right">
							<asp:Button ID="btnClose" Runat="server" Text="Close" CssClass="formfield" Width="67px" onclick="BtnCloseClick" />
						</td>
					</tr>
				</table>
			</div>
		</div>
</form>
	</body>
</HTML>

			
