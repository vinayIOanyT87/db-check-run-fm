<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="ProductAuthorizedCustomersPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProductAuthorizedCustomersPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<script>
	function CompanySelect(role, companyTextBoxId)
	{
		var companyTextBox = document.getElementById(companyTextBoxId);

		showModalDialogFrame(
			{
			url: '../FMWebApp/CompanySelectForm.aspx?Role=' + role + '&Map=PRODUCT_COMPANY_MAP',
			width: 855,
			height: 560,
			title: "Company Select",
			onClose: function ()
			{
				if (this.returnValue != null)
				{
					var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
					var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

					companyTextBox.value = asciiValue1;
					companyTextBox.title = asciiValue2;
				}
			}
		});
	}

	function InstructionsButton_Click(itemIndex)
	{
		showModalDialogFrame(
			{
			url: '../FMWebApp/SpecialInstructionsForm.aspx?mode=product&ItemIndex=' + itemIndex,
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

	function InstructionsReadOnlyButton_Click(itemIndex)
	{
		showModalDialogFrame(
			{
			url: '../FMWebApp/SpecialInstructionsForm.aspx?mode=productReadOnly&ItemIndex=' + itemIndex,
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
</script>

<FMControls:FMLabel ID="Fmlabel2" AssociatedControlID="TypeDropDownList" Style="Z-INDEX: 125; LEFT: 0px; POSITION: absolute; TOP: 16px" runat="server"
    Width="64px" CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
<FMControls:FMDropDownList ID="TypeDropDownList" Style="Z-INDEX: 111; LEFT: 104px; POSITION: absolute; TOP: 16px"
    runat="server" Width="240px" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged">
</FMControls:FMDropDownList>
<table id="Table1" style="z-index: 101; left: 0px; width: 400px; position: absolute; top: 50px; height: 10px"
    cellspacing="0" cellpadding="1" border="0">
    <tr>
        <td width="507" height="10">
            <FMControls:FMDataGrid ID="AuthorizedCustomersDataGrid" runat="server" Width="750px" CssClass="tabletext" RowHeaderColumn="Customer ID"
                BackColor="White" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
                CellPadding="3" AllowPaging="True" PageSize="11">
                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                <Columns>
                    <asp:TemplateColumn HeaderText="Edit">
                        <HeaderStyle Width="70px"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        <ItemTemplate>
                            <FMControls:FMEditLinkButton ID="RecordEditBtn" runat="server" />
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
                    <asp:TemplateColumn HeaderText="Customer ID">
                        <ItemStyle Wrap="False"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CustomerID") %>' ID="CompanyIDLabel">
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <FMControls:FMDropDownList Width="1in" CssClass="tabletext" runat="server" Enabled="True" ID="CompanyGroupDropDownList" DataSource="<%# EnumerateCustomers()%>" DataTextField="Text" DataValueField="Value">
                            </FMControls:FMDropDownList>
                            <FMControls:FMCompanyTextBox Role="CUSTOMER_SHIPTO" CssClass="tabletext" runat="server" Enabled="True" ID="CompanyTextBox" Text='<%# DataBinder.Eval(Container, "DataItem.CustomerID") %>'>
                            </FMControls:FMCompanyTextBox>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Additive Profile">
                        <ItemTemplate>
                            <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.AdditiveProfileID") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.AdditiveProfileID") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label3">
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList Width="1in" CssClass="tabletext" runat="server" Enabled="True" ID="AdditiveProfilesDropDownList" DataSource="<%# EnumerateAdditiveProfiles()%>" DataTextField="Text" DataValueField="Value">
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
                            <asp:Label Width=".65in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label7">
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox Width=".65in" MaxLength="10" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' Enabled="True" ID="ShipToProductCodeTextBox">
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
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        <ItemTemplate>
                            <input class="tabletext small" id="InstructionsButton" onclick='<%# DataBinder.Eval(Container, "DataItem.SpecialInstructionsClick") %>' type="button" value='<%# DataBinder.Eval(Container, "DataItem.SpecialInstructionsText") %>' runat="server" name="InstructionsButton">
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Delete">
                        <HeaderStyle Width="30px"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        <ItemTemplate>
                            <FMControls:FMDeleteLinkButton ID="RecordDeleteBtn" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
            </FMControls:FMDataGrid></td>
    </tr>
    <tr>
        <td width="507" height="10">
            <FMControls:FMButton ID="AddButton" runat="server" Width="66px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
    </tr>
</table>
