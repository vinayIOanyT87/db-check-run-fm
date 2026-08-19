<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TransactionAliasUserDataPage.ascx.cs" Inherits="FMWebApp.TransactionAliasUserDataPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

		<FMCONTROLS:FMLABEL id="labFieldType" AssociatedControlID="ddlFieldType" style="LEFT: 0px; POSITION: absolute; TOP: 16px" runat="server"
			CssClass="formfieldtitle" text="Field Type:" />
		<FMCONTROLS:FMDROPDOWNLIST id="ddlFieldType" style="Z-INDEX: 101; LEFT: 72px; POSITION: absolute; TOP: 14px"
			runat="server" Width="232px" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="ddlFieldType_SelectedIndexChanged"/>
	<FMControls:FMLabel ID="aliasTypeLabel" Style="z-index: 101; left: 410px; position: absolute;
		top: 16px" runat="server" CssClass="formfieldtitle" BackColor="Transparent" visible="False">Transaction Alias Type:</FMControls:FMLabel>
	<asp:Panel ID="aliasTypePanel" runat="server" Style="z-index: 118; left: 556px; position: absolute;
		top: 7px; width: 174px; height: 28px;" BorderColor="LightSteelBlue" BorderStyle="Solid"
		BorderWidth="1px" visible="False" />
	<FMControls:FMRadioButton ID="rbStandardAlias" runat="server" Style="z-index: 118;
		left: 565px; position: absolute; top: 12px;" GroupName="AliasTypeGroup" Text="Standard"
		CssClass="formfieldNoWrap" AutoPostBack="True" OnCheckedChanged="OnAliasTypeChanged" visible="False"/>
	<FMControls:FMRadioButton ID="rbDispatchAlias" runat="server" Style="z-index: 118;
		left: 651px; position: absolute; top: 12px" GroupName="AliasTypeGroup" Text="Dispatch"
		CssClass="formfieldNoWrap" AutoPostBack="True" OnCheckedChanged="OnAliasTypeChanged" visible="False" />
    <table id="UserDataPageTable" style="Z-INDEX: 100; LEFT: 0px; WIDTH: 50%; POSITION: absolute; TOP: 48px; HEIGHT: 10px" role="presentation" aria-label="layout">
	<tr>
		<td>
			<FMCONTROLS:FMDATAGRIDFIXED id="UserDataFieldDataGrid"  RowHeaderColumn="Item No."
				runat="server" CssClass="tabletext" CellPadding="3"
				BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" 
				AutoGenerateColumns="False" BackColor="White"
				BorderStyle="None" Height="405px" Width="790px" ShowFooter="false" aria-label="User data Fields">
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Edit">
						<HeaderStyle Width="55px"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<FMControls:FMEditLinkButton runat="server" />
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMUpdateLinkButton runat="server" />&nbsp;<FMControls:FMCancelLinkButton runat="server" />
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
						<HeaderStyle Width="0.5in"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="SiteGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>'>
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False" HeaderText="UserDataFieldGuid">
						<HeaderStyle Width="0.5in"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="UserDataFieldGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.UserDataFieldGuid") %>'>
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Item No.">
						<HeaderStyle Width="0.5in"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id=NumberLabel runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Number") %>'>
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Display Name">
						<HeaderStyle Width="1in"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id=DisplayNameLabel runat="server" Width="1in" Text='<%# DataBinder.Eval(Container, "DataItem.DisplayName") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id=DisplayNameTextBox runat="server" Width="1in" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ValueList") %>' MaxLength="30" Columns="30">
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Type">
						<HeaderStyle Width="1in"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="TypeLabel" Width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Type") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMDropDownList ToolTip="Type" Width=".5in" runat="server" CssClass="tabletext" Enabled="True" ID="TypeDropDownList" DataSource="<%# EnumerateUserDataTypes()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownList_SelectedIndexChanged">
							</FMControls:FMDropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Value List">
						<HeaderStyle Width="1.75in"></HeaderStyle>
						<ItemTemplate>
							<asp:Label id=ValueListLabel Width=2.0in runat="server" Style="overflow:hidden;white-space:nowrap;text-overflow:ellipsis" Text='<%# DataBinder.Eval(Container, "DataItem.ValueList") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id="ValueListTextBox" ToolTip="Value List" TextMode="MultiLine" Width="2.0in" runat="server" CssClass="tabletext" MaxLength="255"></asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Required">
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<asp:CheckBox Runat="server" CssClass="tabletext" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.Required") %>' ID="RequiredCheckBox"></asp:CheckBox>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:CheckBox Runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.Required") %>' ID="RequiredCheckBox"></asp:CheckBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="User Group">
						<HeaderStyle Width="1.5in"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label id="UserGroupLabel" Width=1.75in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.UserGroup") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:dropdownlist Width=2.0in CssClass=tabletext ToolTip="User Group" runat="server" Enabled="True" ID="UserGroupDropDownList" DataSource="<%# EnumerateUserGroups()%>" DataTextField="Text" DataValueField="Value">
							</asp:dropdownlist>
						</EditItemTemplate>
					</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Clear on New/Copy">
							<ItemTemplate>
								<asp:CheckBox runat="server" CssClass="tabletext" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.ClearOnNew") %>'
									ID="ClearOnNewCheckBox"></asp:CheckBox>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:CheckBox runat="server" CssClass="tabletext" Checked='<%# DataBinder.Eval(Container, "DataItem.ClearOnNew") %>'
									ID="ClearOnNewCheckBox"></asp:CheckBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
				</Columns>
			</FMCONTROLS:FMDATAGRIDFIXED></td>
	    </tr>
    </table>
    <input id="SAVESCROLLHORZ" name="SAVESCROLLHORZ" value="0" type="hidden" runat="server" />
    <script>
        function saveUserDataGridScroll() {
            var table = document.getElementById('UserDataPageTable');
            var container = table.childNodes[1].childNodes[0].childNodes[1].childNodes[1];
            document.getElementById('tcTransactionAliasTabs_tpUserDataPage_TransactionAliasUserDataPage_SAVESCROLLHORZ').value = container.scrollTop;
        }

        function restoreUserDataGridScroll() {
            var table = document.getElementById('UserDataPageTable');
            var container = table.childNodes[1].childNodes[0].childNodes[1].childNodes[1];
            container.scrollTop = document.getElementById('tcTransactionAliasTabs_tpUserDataPage_TransactionAliasUserDataPage_SAVESCROLLHORZ').value;
        }
    </script>

