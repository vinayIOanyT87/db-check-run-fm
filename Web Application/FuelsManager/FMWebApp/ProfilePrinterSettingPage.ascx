<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfilePrinterSettingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProfilePrinterSettings" %>

<html>
	<head>
		<title></title>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	</head>
	<body>
		<table style="Z-INDEX: 105; width:1056px; LEFT: 5px; POSITION: absolute; TOP: 50px; height: 91px;">
			<tr>
				<td>
					<FMControls:FMButton ID="AddBtnTop" runat="server" CssClass="formfieldtitle" 
						onclick="AddBtnOnClick" Text="Add" Width="98px" />
				</td>
			</tr>
			<tr>
				<td>
					<FMCONTROLS:FMDataGridFixedPaging ID="PrinterDataGrid" runat="server"
								AutoGenerateColumns="False"
								DataKeyNames="SiteIndex, Index"
								BorderStyle="Solid" 
								BackColor="White" 
								GridLines="Vertical"
								Width="100%"
								BorderWidth="1px"
								AllowSorting="True"
								CellPadding="3"
								CssClass="tabletext"
								EmptyDataText="No records found"
								BorderColor="White"
								tabIndex="7"
								ShowHeaderWhenEmpty="True"
								ShowFooterWhenEmpty="False"
								FixedHeaders="True"
								GroupColumnOffset="0"
								GroupingDepth="0" Height="550px" FixedHeight="550px" ShowFooter="True" 
								onitemdatabound="PrinterItemDataBound">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

							<FooterStyle BackColor="#000000" CssClass="GVFixedFooter" ForeColor="Black"></FooterStyle>

							<HeaderStyle BackColor="#000000" CssClass="GVFixedHeader" Font-Bold="True" ForeColor="White"></HeaderStyle>

							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:BoundColumn DataField="MobileDevicePrinterGuid" 
									HeaderText="MobileDevicePrinterGuid" Visible="False"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Printer ID">
									<ItemTemplate>
										<FMControls:FMTextBox ID="PrinterIdTB" Columns="30" MaxLength="30" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Baud Rate">
									<ItemTemplate>
										<FMControls:FMTextBox ID="BaudRateTB" Columns="8" MaxLength="8" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="COM Port">
									<ItemTemplate>
										<FMControls:FMDropDownList ID="ComPortDD" CssClass="formfield" runat="server" Height="20px" 
											Width="70px">
										</FMControls:FMDropDownList>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Data Bits">
									<ItemTemplate>
										<FMControls:FMTextBox ID="DataBitsTB" Columns="8" MaxLength="8" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Stop Bits">
									<ItemTemplate>
										<FMControls:FMTextBox ID="StopBitsTB" Columns="8" MaxLength="8" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Use Xon Xoff">
									<ItemTemplate>
										<FMControls:FMTextBox ID="UseXonXoffTB" Columns="8" MaxLength="8" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Xon Char">
									<ItemTemplate>
										<FMControls:FMTextBox ID="XonCharTB" Columns="8" MaxLength="8" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Xoff Char">
									<ItemTemplate>
										<FMControls:FMTextBox ID="XoffCharTB" Columns="8" MaxLength="8" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Buffer Size">
									<ItemTemplate>
										<FMControls:FMTextBox ID="BufferSizeTB" Columns="8" MaxLength="8" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Parity">
									<ItemTemplate>
										<FMControls:FMDropDownList ID="ParityDD" CssClass="formfield" runat="server" Height="20px" 
											Width="70px">
										</FMControls:FMDropDownList>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton ID="DeleteLinkButton" runat="server"></FMControls:FMDeleteLinkButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
						</FMControls:FMDataGridFixedPaging>
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMButton ID="AddBtnBottom" runat="server" CssClass="formfieldtitle" 
						onclick="AddBtnOnClick" Text="Add" Width="98px" />
				</td>
			</tr>
		</table>
	</body>
</html>