<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileAnalogSettingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProfileAnalogSettingPage" %>

<html>
	<head>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
			 .style15
			{
				width: 61px;
			}
			.style16
			{
				width: 71px;
			}
			.style17
			{
				width: 76px;
			}
			.style18
			{
				width: 87px;
			}
			.style19
			{
				width: 88px;
			}
			.style20
			{
				width: 161px;
			}
		</style>
	</head>
	<body>
		<table style="Z-INDEX: 105; width:800px; LEFT: 5px; POSITION: absolute; TOP: 50px; height: 91px;">
			<tr>
				<td>
					<FMControls:FMButton ID="AddBtnTop" runat="server" CssClass="formfieldtitle" 
						onclick="AddBtnOnClick" Text="Add" Width="98px" />
				</td>
			</tr>
			<tr>
				<td>
					<FMCONTROLS:FMDataGridFixedPaging ID="AnalogInputDataGrid" runat="server"
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
						onitemdatabound="AnalogInputItemDataBound">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

							<FooterStyle BackColor="#000000" CssClass="GVFixedFooter" ForeColor="Black"></FooterStyle>

							<HeaderStyle BackColor="#000000" CssClass="GVFixedHeader" Font-Bold="True" ForeColor="White"></HeaderStyle>

							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:BoundColumn DataField="InputNumber" HeaderText="Inputs">
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="MobileDeviceAnalogInputGuid" 
									HeaderText="MobileDeviceAnalogInputGuid" Visible="False"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Low Limit">
									<ItemTemplate>
										<FMControls:FMTextBox ID="LowLimitTextBox" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="High Limit">
									<ItemTemplate>
										<FMControls:FMTextBox ID="HighLimitTextBox" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Parameter A">
									<ItemTemplate>
										<FMControls:FMTextBox ID="ParameterATextBox" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Parameter B">
									<ItemTemplate>
										<FMControls:FMTextBox ID="ParameterBTextBox" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Parameter C">
									<ItemTemplate>
										<FMControls:FMTextBox ID="ParameterCTextBox" CssClass="formfield" runat="server"></FMControls:FMTextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Analog Formula">
									<ItemTemplate>
										<FMControls:FMDropDownList ID="AnalogFormulaDropDown" Width="145" CssClass="formfield" runat="server">
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
