<%@ Page language="c#" Codebehind="CurrenciesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FinanceWebApp.CurrenciesForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
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
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image><FMCONTROLS:FMLABEL id="labCurrencyConfig" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" CssClass="headline" Width="272px">Currency Configuration</FMCONTROLS:FMLABEL>
			<div style="Z-INDEX: 102; LEFT: 25px; POSITION: absolute; TOP: 50px">
				<table cellSpacing="0" cellPadding="0" border="0">
					<tr>
						<td><FMCONTROLS:FMBUTTON id="btnAddTop" runat="server" CssClass="formfieldtitle" width="100px" text="Add" onclick="BtnAddTopClick"></FMCONTROLS:FMBUTTON>&nbsp;&nbsp;
							<FMCONTROLS:FMPAGESIZEDROPDOWN id="ddlPageSize" runat="server" onselectedindexchanged="DdlPageSizeSelectedIndexChanged"></FMCONTROLS:FMPAGESIZEDROPDOWN></td>
					</tr>
					<tr>
						<td>&nbsp;</td>
					</tr>
					<tr>
						<td><FMCONTROLS:FMDATAGRID id="dgCurrencies" runat="server" BackColor="White" CssClass="tabletext" Width="400px"
								PageSize="16" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
								GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None">
								<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Edit">
										<HeaderStyle Width="55px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMEditLinkButton runat="server" ID="btnEdit" CommandName="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid")%>' />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:BoundColumn Visible="False" DataField="IdentityGuid"></asp:BoundColumn>
									<asp:BoundColumn DataField="UnitDisplayName" HeaderText="Unit Display Name"></asp:BoundColumn>
									<asp:BoundColumn DataField="Country" HeaderText="Country">
										<HeaderStyle Wrap="False"></HeaderStyle>
										<ItemStyle Wrap="False"></ItemStyle>
										<FooterStyle Wrap="False"></FooterStyle>
									</asp:BoundColumn>
									<asp:TemplateColumn HeaderText="Delete">
										<HeaderStyle width="0.5in"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMDeleteLinkButton runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid")%>' 
											CommandName="Delete" ID="linkDelete" Name="linkDelete"/>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMDATAGRID></td>
					</tr>
					<tr>
						<td>&nbsp;</td>
					</tr>
					<tr>
						<td><FMCONTROLS:FMBUTTON id="btnAddBottom" runat="server" CssClass="formfieldtitle" width="100px" text="Add" onclick="BtnAddBottomClick" /></td>
					</tr>
				</table>
			</div>
		</div>
</form>
	</body>
</HTML>
