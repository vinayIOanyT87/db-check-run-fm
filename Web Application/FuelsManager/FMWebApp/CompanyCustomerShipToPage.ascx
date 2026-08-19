<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="CompanyCustomerShipToPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanyCustomerShipToPage" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<SCRIPT>
		function CompanySelect(role, companyTextBoxId, mode)
		{
			var companyTextBox = document.getElementById(companyTextBoxId);

			showModalDialogFrame({
                url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&Map=AUTHORIZED_CARRIER_MAP&Mode=" + mode + "&All=true",
			    width: 870,
			    height: 710,
				title: "Company Select",
				onClose: function ()
				{
					if (this.returnValue != null) {
						var result = this.returnValue;
						if (result != null && result.length > 0) {
							for (var i = 0; i < result.length; i++) {
								var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

								if (i === 0) {
									companyTextBox.value = newAsciiStr;
								}
								else {
									companyTextBox.value += "|" + newAsciiStr;
								}
							}

							companyTextBox.onchange();
						}
					}
			    }
			});
		}
		
		function InstructionsButton_Click ( itemIndex )
		{		
			showModalDialogFrame({
                url: "../FMWebApp/SpecialInstructionsForm.aspx?mode=company&ItemIndex=" + itemIndex,
                width: 725,
                height: 530,
                title: "Special Instructions",
                onClose: function ()
				{
					if (this.returnValue != null)
					{
                        __doPostBack('InstructionsButton', '');
                    }
                }
			});
		}
		
    </SCRIPT>
	<body>
        <FMControls:FMLabel ID="Label5" Style="z-index: 114; left: 0px; position: absolute; top: 8px" runat="server"
            CssClass="formfieldtitle" BackColor="Transparent">Authorized Products:</FMControls:FMLabel>
        <table id="Table2" style="z-index: 114; left: 0px; width: 238px; position: absolute; top: 30px; height: 10px"
            cellspacing="0" cellpadding="1" width="238" border="0">
            <tr>
                <td width="507" height="10">
                    <FMControls:FMDataGrid ID="AuthorizedProductsDataGrid" runat="server" Width="700px" CssClass="tabletext" RowHeaderColumn="Product ID"
                        BackColor="White" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
                        CellPadding="3" AllowPaging="True" PageSize="4">
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
                                    <FMControls:FMEditLinkButton ID="EditButton" runat="server" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                <FMControls:FMCancelLinkButton runat="server" />

                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="False" HeaderText="Index">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Product ID">
                                <ItemTemplate>
                                    <asp:Label Width="1.5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label2">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList Width="1.5in" CssClass="tabletext" runat="server" Enabled="True" ID="ProductsDropDownList" DataSource="<%# EnumerateProducts()%>" DataTextField="Text" DataValueField="Value">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Additive Profile">
                                <ItemTemplate>
                                    <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.AdditiveProfileID") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.AdditiveProfileID") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label3">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tabletext" runat="server" Enabled="True" ID="AdditiveProfilesDropDownList" DataSource="<%# EnumerateAdditiveProfiles()%>" DataTextField="Text" DataValueField="Value">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="ID">
                                <ItemTemplate>
                                    <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductID") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToProductID") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label6">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Width="1in" MaxLength="30" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductID") %>' Enabled="True" ID="ShipToProductIDTextBox">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Code">
                                <ItemTemplate>
                                    <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label7">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Width="1in" MaxLength="10" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' Enabled="True" ID="ShipToProductCodeTextBox">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Station Text">
                                <ItemTemplate>
                                    <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToLoadRackDisplayText") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToLoadRackDisplayText") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label8">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Width=".5in" MaxLength="10" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToLoadRackDisplayText") %>' Enabled="True" ID="ShipToLoadRackDisplayTextTextBox">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Instructions">
                                <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <input class="tabletext" id="InstructionsButton" onclick='<%# DataBinder.Eval(Container, "DataItem.SpecialInstructionsClick") %>' type="button" value='<%# DataBinder.Eval(Container, "DataItem.SpecialInstructionsText") %>' runat="server">
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Delete">
                                <HeaderStyle Width="0.4in"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                    </FMControls:FMDataGrid></td>
            </tr>
            <tr>
                <td width="507" height="10">
                    <FMControls:FMButton ID="AddProductButton" Width="66" TabIndex="1" runat="server" CssClass="formfieldtitle" Text="Add"></FMControls:FMButton></td>
            </tr>
        </table>
        <FMControls:FMLabel ID="Fmlabel1" Style="z-index: 110; left: 0px; position: absolute; top: 280px" runat="server"
            Width="120px" CssClass="formfieldtitle" BackColor="Transparent">Authorized Carriers:</FMControls:FMLabel>
        <table id="Table1" style="z-index: 113; left: 0px; position: absolute; top: 300px; height: 10px"
            cellspacing="0" cellpadding="1" width="424" border="0">
            <tr>
                <td width="341" height="10">
                    <FMControls:FMDataGrid ID="AuthorizedCarriersDataGrid" TabIndex="5" runat="server" Width="416px" CssClass="tabletext"
                        BackColor="White" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
                        CellPadding="3" AllowPaging="True" PageSize="4">
                        <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                        <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                        <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                        <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                        <Columns>
                            <asp:TemplateColumn HeaderText="ID">
                                <HeaderStyle Width="3in"></HeaderStyle>
                                <ItemStyle Wrap="False"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label Width="2.5in" runat="server" ID="IDLabel"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                    </FMControls:FMDataGrid></td>
            </tr>
            <tr>
                <td width="341" height="36">
                    <table>
                        <tr>
                            <td width="83" height="10">
                                <input class="formfieldtitle" id="CompanyCustomerShipToPage_AssignButton" style="width: 80px"
                                    onclick="CompanySelect('CARRIER', 'tcCompanyTabs_tpCustomerShipToPage_CompanyCustomerShipToPage_AssignCompaniesTextBox', 'Assign')" type="button"
                                    value="Assign" runat="server"></td>
                            <td height="10">
                                <input class="formfieldtitle" id="CompanyCustomerShipToPage_UnassignButton" style="width: 80px"
                                    onclick="CompanySelect('CARRIER', 'tcCompanyTabs_tpCustomerShipToPage_CompanyCustomerShipToPage_UnassignCompaniesTextBox', 'Unassign')"
                                    type="button" value="Unassign" runat="server"></td>
                            <td>
                                <asp:TextBox ID="AssignCompaniesTextBox" ToolTip="Assign Companies" runat="server" Width="82px" BackColor="Transparent" BorderStyle="None"
                                    BorderColor="Transparent" ForeColor="Transparent" AutoPostBack="True" OnTextChanged="AssignCompaniesTextBoxTextChanged"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="UnassignCompaniesTextBox" ToolTip="Unassign Companies" runat="server" Width="82px" BackColor="Transparent" BorderStyle="None"
                                    BorderColor="Transparent" ForeColor="Transparent" AutoPostBack="True" OnTextChanged="UnassignCompaniesTextBoxTextChanged"></asp:TextBox></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <FMControls:FMLabel ID="Label4" Style="z-index: 110; left: 448px; position: absolute; top: 280px" runat="server"
            Width="72px" CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
        <asp:DropDownList ID="TypeDropDownList" ToolTip="Type" Style="z-index: 111; left: 520px; position: absolute; top: 280px"
            TabIndex="16" runat="server" Width="158px" CssClass="formfield">
        </asp:DropDownList>
        <FMControls:FMCheckBox ID="PurchaseOrderRequiredCheckBox" Style="z-index: 112; left: 448px; position: absolute; top: 304px"
            TabIndex="17" runat="server" Width="200px" CssClass="formfieldtitle" Text="Purchase Order Required"
            Height="27px"></FMControls:FMCheckBox>
        <FMControls:FMCheckBox ID="DisableShipToAllocationsCheckCheckBox" Style="z-index: 112; left: 448px; position: absolute; top: 328px"
            TabIndex="18" runat="server" Width="232px" CssClass="formfieldtitle" Text="Disable Ship To Allocations Check"
            Height="27px"></FMControls:FMCheckBox>
        <FMControls:FMCheckBox ID="DisableBillToAllocationsCheckCheckBox" Style="z-index: 112; left: 448px; position: absolute; top: 352px"
            TabIndex="19" runat="server" Width="208px" CssClass="formfieldtitle" Text="Disable Bill To Allocations Check"
            Height="27px"></FMControls:FMCheckBox>
        <FMControls:FMCheckBox ID="DisableShipperAllocationsCheckCheckBox" Style="z-index: 112; left: 448px; position: absolute; top: 376px"
            TabIndex="20" runat="server" Width="240px" CssClass="formfieldtitle" Text="Disable Shipper Allocations Check"
            Height="27px"></FMControls:FMCheckBox>
        <FMControls:FMCheckBox ID="DisableOwnerAllocationsCheckCheckBox" Style="z-index: 112; left: 448px; position: absolute; top: 400px"
            TabIndex="21" runat="server" Width="232px" CssClass="formfieldtitle" Text="Disable Owner Allocations Check"
            Height="27px"></FMControls:FMCheckBox>
	</body>
</HTML>
