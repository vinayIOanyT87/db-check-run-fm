<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="TransactionAliasFieldsPage.ascx.cs"
	Inherits="FMWebApp.TransactionAliasFieldsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>


	<FMControls:FMLabel ID="fieldTypeLabel" AssociatedControlID="FieldTypeDropDownList" Style="z-index: 101; left: 0px; position: absolute;
		top: 16px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Field Type:</FMControls:FMLabel>
	<FMControls:FMDropDownList ID="FieldTypeDropDownList" Style="z-index: 101; left: 72px;
		position: absolute; top: 14px; right: 642px;" runat="server" Width="232px" CssClass="formfield"
		AutoPostBack="True" OnSelectedIndexChanged="FieldTypeDropDownList_SelectedIndexChanged">
	</FMControls:FMDropDownList>
	<FMControls:FMLabel ID="aliasTypeLabel" Style="z-index: 101; left: 410px; position: absolute;
		top: 16px" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Visible="False">Transaction Alias Type:</FMControls:FMLabel>
	<asp:Panel ID="aliasTypePanel" runat="server" Style="z-index: 118; left: 556px; position: absolute;
		top: 7px; width: 174px; height: 28px;" BorderColor="LightSteelBlue" BorderStyle="Solid"
		BorderWidth="1px" Visible="False" />
	<FMControls:FMRadioButton ID="rbStandardAlias" runat="server" Style="z-index: 118;
		left: 565px; position: absolute; top: 12px;" GroupName="AliasTypeGroup" Text="Standard"
		CssClass="formfieldNoWrap" AutoPostBack="True" OnCheckedChanged="OnAliasTypeChanged" Visible="false" />
	<FMControls:FMRadioButton ID="rbDispatchAlias" runat="server" Style="z-index: 118;
		left: 651px; position: absolute; top: 12px" GroupName="AliasTypeGroup" Text="Dispatch"
		CssClass="formfieldNoWrap" AutoPostBack="True" OnCheckedChanged="OnAliasTypeChanged" Visible="False" />
	<table id="FieldsPageTable" style="z-index: 99; left: 0; width: 38.42%; 
        position: absolute; top: 48px; height: 10px" role="presentation" aria-label="layout">
		<tr>
			<td>
				<FMControls:FMDataGridFixed ID="FieldDataGrid" runat="server" CssClass="tabletext"  RowHeaderColumn="Field Name"
					Width="900px" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
					GridLines="Vertical" AutoGenerateColumns="False" BackColor="White" BorderStyle="None"  
					AllowPaging="True" Height="405px" ShowFooter="False" OnItemDataBound="FieldDataGridItemDataBound" aria-label="Fields">
					<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
					<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
					<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
					<Columns>
						<asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
							<HeaderStyle Width="0.5in"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:Label ID="SiteGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn Visible="False" HeaderText="IdentityGuid">
							<HeaderStyle Width="0.5in"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:Label ID="IdentityGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn Visible="False" HeaderText="RequiredEnabled">
							<HeaderStyle Width="0.5in"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
                                <asp:CheckBox runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.RequiredEnabled") %>'
									ID="RequiredEnabledCheckBox"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
                        <asp:TemplateColumn Visible="False" HeaderText="FieldNameHidden">
							<HeaderStyle Width="0.5in"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:Label ID="FieldNameLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FieldName") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Field Name">
							<HeaderStyle Width="2in"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:Label ID="FieldNameLabelDisplay" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FieldName") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
   						<asp:TemplateColumn HeaderText="Display">
							<HeaderStyle Width="55px"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
                                <asp:CheckBox runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.Display") %>'
									ID="DisplayCheckBox" ToolTip="Display"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Display Name">
							<HeaderStyle Width="2in"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:TextBox ID="DisplayNameTextBox" Width="1.75in" runat="server" CssClass="tabletext" 
									Text='<%# DataBinder.Eval(Container, "DataItem.DisplayName") %>' ToolTip="Display Name">
								</asp:TextBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Required">
						    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.Required") %>'
									ID="RequiredCheckBox" ToolTip="Required"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="User Group">
							<HeaderStyle Width="2in"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:DropDownList Width="2.0in" CssClass="tabletext" runat="server" Enabled="True"
									ID="UserGroupDropDownList" DataSource="<%# EnumerateUserGroups()%>" DataTextField="Text"
									DataValueField="Value" ToolTip="User Group">
								</asp:DropDownList>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Clear on New/Copy (Dispatch-Only)">
						    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.ClearOnNew") %>'
									ID="ClearOnNewCheckBox" ToolTip="Clear on New/Copy (Dispatch-Only)"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</FMControls:FMDataGridFixed>
			</td>
		</tr>
	</table>
    <input id="SAVESCROLLHORZ" name="SAVESCROLLHORZ" value="0" type="hidden" runat="server" />
    <script>
        function saveFieldDataGridScroll() {
            var table = document.getElementById('FieldsPageTable');
            var container = table.childNodes[1].childNodes[0].childNodes[1].childNodes[1];
            document.getElementById('tcTransactionAliasTabs_tpFieldsPage_TransactionAliasFieldsPage_SAVESCROLLHORZ').value = container.scrollTop;
        }

        function restoreFieldDataGridScroll() {
            var table = document.getElementById('FieldsPageTable');
            var container = table.childNodes[1].childNodes[0].childNodes[1].childNodes[1];
            container.scrollTop = document.getElementById('tcTransactionAliasTabs_tpFieldsPage_TransactionAliasFieldsPage_SAVESCROLLHORZ').value;
        }
    </script>

