<%@ Page Language="c#" CodeBehind="StandingOfferPriceForm.aspx.cs" AutoEventWireup="True"
	Inherits="FuelsManager.FinanceWebApp.StandingOfferPriceForm" EnableSessionState="True" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body ms_positioning="GridLayout">
	<form id="StandingOfferForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
	<asp:ScriptManager ID="ScriptManager" runat="server" />
	<asp:UpdatePanel ID="UpdatePanel1" runat="server">
		<ContentTemplate>
			<script type="text/javascript">
				function CompanySelect(Role, CompanyTextBoxID) {
					var sFeatures = "dialogWidth: 855px; dialogHeight: 560px";
					var CompanyTextBox = document.getElementById(CompanyTextBoxID);
					var Result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Null=true&Role=" + Role, "", sFeatures);

					if (Result != null) {
						CompanyTextBox.value = Result[0];
						CompanyTextBox.title = Result[1];
					}
				}

				function ProductSelect(productTextBoxID) {
					var sFeatures = "dialogWidth: 8.81in; dialogHeight: 6in";
					var productTextBox = document.getElementById(productTextBoxID);
					var result = null;
					var companyID = "";

					result = window.showModalDialog("../FMWebApp/ProductSelectForm.aspx?Null=true&Type=MaxProduct" +
															  "&Map=MAX_MAP" + "&IDLink=" + encodeURIComponent(companyID), "", sFeatures);

					if (result != null) {
						productTextBox.value = result[0];
						productTextBox.title = result[1];
					}
				}
			</script>
			<asp:Image ID="FadeImage" Style="z-index: 101; left: 0px; position: absolute; top: 0px"
				runat="server" BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>">
			</asp:Image>
			<FMControls:FMLabel ID="StandingOfferTitleLabel" Style="z-index: 102; left: 8px;
				position: absolute; top: 8px" runat="server" BackColor="Transparent" CssClass="headline"
				Width="272px">Price List Configuration</FMControls:FMLabel>
			<table id="Table1" style="z-index: 103; left: 8px; width: 853px; position: absolute;
				top: 48px; height: 74px" cellspacing="1" cellpadding="1" border="0">
				<tr>
					<td style="width:70px">
						<FMControls:FMLabel ID="SiteLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							Width="64px">Site</FMControls:FMLabel>
					</td>
					<td style="width:215px">
						<asp:TextBox ID="SiteTextBox" TabIndex="2" runat="server" CssClass="formfield" Width="180px"
							ReadOnly="True" BackColor="#DDDDDD"></asp:TextBox>
					</td>
					<td style="width:128px">
						<FMControls:FMLabel ID="LocationLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							Width="64px">Delivery Location</FMControls:FMLabel>
					</td>
					<td colspan="4" style="width:470px">
						<FMControls:FMLocationSelectDropDown ID="LocationSelect" TabIndex="5" runat="server"
							CssClass="formfield" Width="408px" Height="20px">
							<asp:ListItem Value="10000000-0000-0000-0000-000000000000" Selected="True">{All}</asp:ListItem>
						</FMControls:FMLocationSelectDropDown>
					</td>
				</tr>
				<tr>
					<td nowrap="nowrap">
						<FMControls:FMLabel ID="SupplierLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							Width="64px">Supplier</FMControls:FMLabel>
					</td>
					<td nowrap="nowrap">
						<FMControls:FMCompanyTextBox ID="SupplierTextBox" runat="server" CssClass="formfield"
							Width="130px" Role="SUPPLIER"></FMControls:FMCompanyTextBox>
					</td>
					<td>
						<FMControls:FMLabel ID="EffectiveDateLabel" runat="server" BackColor="Transparent"
							CssClass="formfieldtitle" Width="64px">Effective Start Date</FMControls:FMLabel>
					</td>
					<td style="width:120px">
						<FMControls:FMDate ID="EffectiveDateDate" runat="server" CssClass="formfield" Width="120px">
						</FMControls:FMDate>
					</td>
					<td style="width:120px">
						<FMControls:FMLabel ID="EndDateLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							Width="64px">Effective End Date</FMControls:FMLabel>
					</td>
					<td style="width:120px">
						<FMControls:FMDate ID="EndDateTextBox" runat="server" CssClass="formfield" Width="120px">
						</FMControls:FMDate>
					</td>
					<td style="width:80px">
						<FMControls:FMButton ID="RefreshButton" TabIndex="1" runat="server" CssClass="formfieldtitle"
							Width="70px" Text="Refresh" OnClick="RefreshBtnOnClick"></FMControls:FMButton>
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMLabel ID="ProductLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							Width="64px">Product</FMControls:FMLabel>
					</td>
					<td>
						<FMControls:FMProductTextBox ID="ProductTextBox" TabIndex="4" runat="server" CssClass="formfield"
							Width="130px"></FMControls:FMProductTextBox>
					</td>
					<td>
						<FMControls:FMLabel ID="ReferenceNumberLabel" runat="server" BackColor="Transparent"
							CssClass="formfieldtitle" Width="64px">Reference Number</FMControls:FMLabel>
					</td>
					<td class="style1">
						<asp:TextBox ID="ReferenceNumberTextBox" TabIndex="2" runat="server" CssClass="formfield"
							Width="107px" Columns="20" MaxLength="20"></asp:TextBox>
					</td>
					<td>
						&nbsp;
					</td>
					<td class="style2">
						&nbsp;
					</td>
				</tr>
				<tr>
					<td>
						<br>
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMButton ID="AddButton1" TabIndex="1" runat="server" CssClass="formfieldtitle"
							Width="65px" Text="Add" OnClick="AddBtn1OnClick"></FMControls:FMButton>
					</td>
					<td>
						<FMControls:FMPageSizeDropDown ID="PageSizeDropDown" TabIndex="7" runat="server"
							Width="96px" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged">
						</FMControls:FMPageSizeDropDown>
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" CssClass="formfield"
							Text="abc" Visible="false" ForeColor="Red" />
					</td>
				</tr>
				<tr>
					<td colspan="7">
						<FMControls:FMDataGrid ID="StandingOfferDataGrid" TabIndex="5" runat="server" BackColor="White"
							CssClass="tabletext" Width="736px" BorderStyle="None" AutoGenerateColumns="False"
							GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
							CellPadding="3" AllowPaging="True" PageSize="20">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
							</HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton ID="EditLinkButton" runat="server"></FMControls:FMEditLinkButton>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton ID="UpdateLinkButton" runat="server"></FMControls:FMUpdateLinkButton>
										<FMControls:FMCancelLinkButton ID="CancelLinkButton" runat="server"></FMControls:FMCancelLinkButton>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label ID="StandingOfferIndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.StandingOfferIndex") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
									<ItemTemplate>
										<asp:Label ID="SiteGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Price List ID">
									<ItemTemplate>
										<asp:Label ID="StandingOfferIDLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.StandingOfferID") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderTemplate>
										<FMControls:FMLabel ID="SupplierHdr" runat="server" Text="Supplier" /><span style="color: red">
											*</span></HeaderTemplate>
									<ItemStyle Wrap="False"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="SupplierGridLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SupplierID") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMCompanyTextBox ID="FMSupplierGridTextBox" runat="server" CssClass="formfield"
											Width="145px" AutoPostBack="True" Role="SUPPLIER" Text='<%# DataBinder.Eval(Container, "DataItem.SupplierID") %>'>
										&nbsp;&nbsp;
										</FMControls:FMCompanyTextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderTemplate>
										<FMControls:FMLabel ID="ProductHdr" runat="server" Text="Product" /><span style="color: red">
											*</span></HeaderTemplate>
									<ItemStyle Wrap="False"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="ProductGridLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMProductTextBox ID="FMProductGridTextBox" TabIndex="4" runat="server"
											CssClass="formfield" Width="169px" AutoPostBack="True" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>'>
										&nbsp;&nbsp;
										</FMControls:FMProductTextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderTemplate>
										<FMControls:FMLabel ID="LowerBoundHdr" runat="server" Text="Lower Bound" /><span
											style="color: red"> *</span></HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="LowerBoundLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LowerBound") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="LowerBoundTextBox" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.LowerBound") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderTemplate>
										<FMControls:FMLabel ID="UpperBoundHdr" runat="server" Text="Upper Bound" /><span
											style="color: red"> *</span></HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="UpperBoundLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.UpperBound") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="UpperBoundTextBox" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.UpperBound") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delivery Location">
									<ItemTemplate>
										<asp:Label ID="LocationGridLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LocationID") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMLocationSelectDropDown ID="FMLocationDropDownList" runat="server" CssClass="formfield"
											SelectedLocationGuid='<%# DataBinder.Eval(Container, "DataItem.LocationGuid") %>'>
										</FMControls:FMLocationSelectDropDown>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Price List Price">
									<ItemTemplate>
										<asp:Label ID="StandingOfferPriceLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.StandingOfferPrice") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="StandingOfferPriceTextBox" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.StandingOfferPrice") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderTemplate>
										<FMControls:FMLabel ID="EffectiveDateHdr" runat="server" Text="Effective Date" /><span
											style="color: red"> *</span></HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="EffectiveDateGridLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.EffectiveDate") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMDate ID="EffectiveGridDate" runat="server" CssClass="formfield" Width="160px"
											Text='<%# DataBinder.Eval(Container, "DataItem.EffectiveDate") %>'></FMControls:FMDate>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderTemplate>
										<FMControls:FMLabel ID="ExpirationDateHdr" runat="server" Text="Expiration Date" /><span
											style="color: red"> *</span></HeaderTemplate>
									<ItemTemplate>
										<asp:Label ID="ExpirationDateGridLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ExpirationDate") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMDate ID="ExpirationGridDate" runat="server" Width="160px" CssClass="formfield"
											Text='<%# DataBinder.Eval(Container, "DataItem.ExpirationDate") %>'></FMControls:FMDate>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Reference Number">
									<ItemTemplate>
										<asp:Label ID="ReferenceNumberLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ReferenceNumber") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="ReferenceNumberTextBox" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ReferenceNumber") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton ID="DeleteLinkButton" runat="server"></FMControls:FMDeleteLinkButton>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" ID="btnDeleteEdit" NAME="btnDeleteEdit"
											Enabled="false" />
									</EditItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
								Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid>
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMButton ID="AddButton2" TabIndex="1" runat="server" CssClass="formfieldtitle"
							Width="65px" Text="Add" OnClick="AddBtn2OnClick"></FMControls:FMButton>
					</td>
				</tr>
			</table>
		</ContentTemplate>
	</asp:UpdatePanel>
	</div>
</form>
</body>
</html>
