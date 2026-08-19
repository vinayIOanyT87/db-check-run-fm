<%@ Page language="c#" Codebehind="MessagesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.MessagesForm" %>
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
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
	</HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
		<SCRIPT>
			function CompanySelect(role, companyTextBoxId)
			{
				var companyTextBox = document.getElementById(companyTextBoxId);

				showModalDialogFrame({
				    url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&All=true",
				    width: 890,
				    height: 770,
				    title: "Company Select",
				    onClose: function ()
				    {
				        if (this.returnValue != null)
				        {
				            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				            var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				            companyTextBox.value = asciiValue1;
				            companyTextBox.title = asciiValue2;
				            companyTextBox.onchange();
				        }
				    }
				});
			}
		</SCRIPT>
		<form id="Form2" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				BackColor="Transparent" Width="224px" CssClass="headline">Messages Configuraton</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label8" Style="z-index: 104; left: 32px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle">Carrier:</FMControls:FMLabel>
			<FMControls:FMCompanyTextBox Role="CARRIER" ID="CarrierTextBox" Style="z-index: 125; left: 80px; position: absolute; top: 40px"
				TabIndex="1" runat="server" Width="201px" CssClass="formfield" AutoPostBack="True" OnTextChanged="CarrierTextBoxTextChanged"></FMControls:FMCompanyTextBox>
			<FMControls:FMLabel ID="Label9" AssociatedControlID="DriversDropDownList" Style="z-index: 105; left: 344px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle">Driver:</FMControls:FMLabel>
			<asp:DropDownList ID="DriversDropDownList" Style="z-index: 108; left: 400px; position: absolute; top: 40px"
				TabIndex="2" runat="server" Width="144px" CssClass="formfield" DataValueField="Value" DataTextField="Text" AutoPostBack="True" OnSelectedIndexChanged="DriversDropDownListSelectedIndexChanged">
			</asp:DropDownList>
			<table id="Table1" style="z-index: 101; left: 32px; width: 960px; position: absolute; top: 72px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td style="width: 960px; height: 36px" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="LRMessagesFormPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
					</TD>
				</tr>
				<tr>
					<td style="width: 960px; height: 10px">
						<FMControls:FMDataGrid ID="MessageDataGrid" Style="left: 1px; top: 0px" TabIndex="3" runat="server" PageSize="12" RowHeaderColumn="Carrier"
							BorderStyle="None" BackColor="White" AutoGenerateColumns="False" GridLines="Vertical" Width="960px" BorderWidth="1px" AllowSorting="True" BorderColor="White"
							CellPadding="3" AllowPaging="True" CssClass="tabletext" aria-label="Messages">
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
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                        <FMControls:FMCancelLinkButton runat="server" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Carrier">
									<HeaderStyle Width="3in"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
									<ItemTemplate>
										<asp:Label Width="2.5in" runat="server" ID="CarrierLabel"></asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMCompanyTextBox Role="CARRIER" Width="2.5in" CssClass="tabletext" runat="server" Enabled="True"
											ID="ItemCarrierTextBox" ToolTip="Carrier" AutoPostBack="True" OnTextChanged="ItemCarrierTextBoxTextChanged"></FMControls:FMCompanyTextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Driver">
									<HeaderStyle Width="1.5in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label Width="1.5in" runat="server" ID="DriverLabel"></asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:DropDownList Width="1.5in" CssClass="tabletext" runat="server" Enabled="True" ID="DriverDropDownList" DataSource="<%# EnumerateDrivers()%>" DataTextField="Text" DataValueField="Value" ToolTip="Driver">
										</asp:DropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Message">
									<HeaderStyle Width="2in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label Width="2in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label1">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox Width="2in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' CssClass="tabletext" ID="IDTextBox" ToolTip="Message" MaxLength="30">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Location">
									<HeaderStyle Width="1in"></HeaderStyle>
									<ItemTemplate>
										<FMControls:FMLabel Width="1in" runat="server" Text='<%# GetTranslatedText( DataBinder.Eval(Container, "DataItem.LocationType") as string )%>' ID="LocationLabel">
										</FMControls:FMLabel>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMDropDownList Width="1in" CssClass="tabletext" runat="server" Enabled="True" ID="LocationTypeDropDownList" ToolTip="Location" DataSource="<%# EnumerateLocationTypes()%>" DataTextField="Text" DataValueField="Value">
										</FMControls:FMDropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Frequency">
									<HeaderStyle Width="1in"></HeaderStyle>
									<ItemTemplate>
										<FMControls:FMLabel Width="1in" runat="server" Text='<%# GetTranslatedText( DataBinder.Eval(Container, "DataItem.FrequencyType") as string )%>' ID="Label5">
										</FMControls:FMLabel>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMDropDownList Width="1in" CssClass="tabletext" runat="server" Enabled="True" ID="FrequencyTypeDropDownList" ToolTip="Frequency" DataSource="<%# EnumerateFrequencyTypes()%>" DataTextField="Text" DataValueField="Value">
										</FMControls:FMDropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></td>
				</tr>
				<tr>
					<td style="width: 960px; height: 18px" valign="middle">
						<FMControls:FMButton ID="AddButton" TabIndex="4" runat="server" Width="98px" CssClass="formfieldtitle"
							Text="Add"></FMControls:FMButton></td>
				</tr>
			</table>
			<script>
				var oAddButton = document.getElementById("AddButton");
				if (oAddButton != null) {
					try {
						oAddButton.setActive();
					}
					catch (err) { }
				}
			</script>
		</div>
	</form>
</body>
</html>
