<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="AcculoadForm.aspx.cs" AutoEventWireup="true" Inherits="AcculoadOPCWebApp.AcculoadForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 101; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="320px" BackColor="Transparent">SmithMeter|Preset Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label1" AssociatedControlID="IDTextBox" Style="z-index: 103; left: 16px; position: absolute; top: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|ID:</FMControls:FMLabel>
			<FMControls:FMLabel ID="UserNameRequiredLabel" Style="z-index: 106; left: 72px; position: absolute; top: 48px"
				runat="server" BackColor="Transparent" Width="8px" ForeColor="Crimson" Height="8px" CssClass="formfieldtitle">*</FMControls:FMLabel>
			<asp:TextBox ID="IDTextBox" Style="z-index: 104; left: 104px; position: absolute; top: 48px" aria-required="true"
				runat="server" CssClass="formfield" Width="136px" TabIndex="1"></asp:TextBox>
			<FMControls:FMLabel ID="Label3" Style="z-index: 107; left: 16px; position: absolute; top: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Type:</FMControls:FMLabel>
			<asp:DropDownList ID="TypeDropDownList" Style="z-index: 108; left: 104px; position: absolute; top: 80px"
				runat="server" Width="136px" CssClass="formfield" TabIndex="2" AutoPostBack="True">
			</asp:DropDownList>
			<FMControls:FMRadioButton ID="SerialCommunicationsRadioButton" Style="z-index: 142; left: 88px; position: absolute; top: 112px"
				TabIndex="3" runat="server" GroupName="Communications" Text="SmithMeter|Serial Communications" CssClass="formfieldtitle"
				Width="232px" AutoPostBack="True"></FMControls:FMRadioButton>
			<FMControls:FMLabel ID="Label4" Style="z-index: 109; left: 16px; position: absolute; top: 144px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Port:</FMControls:FMLabel>
			<asp:DropDownList ID="PortDropDownList" Style="z-index: 110; left: 104px; position: absolute; top: 144px"
				runat="server" Width="136px" CssClass="formfield" TabIndex="3">
			</asp:DropDownList>
			<FMControls:FMRadioButton ID="NetworkCommunicationsRadioButton" Style="z-index: 141; left: 88px; position: absolute; top: 168px"
				TabIndex="6" runat="server" GroupName="Communications" Text="SmithMeter|Network Communications" CssClass="formfieldtitle"
				AutoPostBack="True"></FMControls:FMRadioButton>
			<FMControls:FMLabel ID="Label5" Style="z-index: 143; left: 16px; position: absolute; top: 192px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|IP Address:</FMControls:FMLabel>
			<asp:TextBox ID="IPAddressTextBox" Style="z-index: 145; left: 104px; position: absolute; top: 192px"
				runat="server" Width="168px" CssClass="formfield" TabIndex="7"></asp:TextBox>
			<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 16px; WIDTH: 39.37%; POSITION: absolute; TOP: 224px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<TBODY>
					<tr>
						<TD style="WIDTH: 710px; HEIGHT: 10px">
							<FMCONTROLS:FMDatagrid id="ArmDataGrid" runat="server" BackColor="White" Width="616px" CssClass="tabletext"
								Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px"
								AllowSorting="True" BorderColor="White" CellPadding="3" PageSize="4" AllowPaging="True" tabIndex="4">
								<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<EditItemStyle Wrap="False"></EditItemStyle>
								<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn Visible="False" HeaderText="Index">
										<ItemTemplate>
											<FMCONTROLS:FMLABEL ID="IndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
											</FMCONTROLS:FMLABEL>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="SmithMeter|Arm">
										<HeaderStyle Width="0.25in"></HeaderStyle>
										<ItemTemplate>
											<FMCONTROLS:FMLABEL runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Number") %>' ID="Label9">
											</FMCONTROLS:FMLABEL>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox ID="ArmTextBox" CssClass=tabletext Width=".25in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Number") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="SmithMeter|Address">
										<ItemTemplate>
											<FMCONTROLS:FMLABEL runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Address") %>'>
											</FMCONTROLS:FMLABEL>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox ID="AddressTextBox" CssClass=tabletext Width=".25in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Address") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="SmithMeter|Type">
										<ItemTemplate>
											<FMCONTROLS:FMLABEL runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Type") %>' ID="Label10">
											</FMCONTROLS:FMLABEL>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:dropdownlist CssClass=tabletext runat="server" Enabled="True" ID="ArmTypesDropDownList" DataSource="<%# EnumerateArmTypes()%>" DataTextField="Text" DataValueField="Value">
											</asp:dropdownlist>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="SmithMeter|Products">
										<ItemTemplate>
											<FMCONTROLS:FMLABEL runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Products") %>' ID="Label11">
											</FMCONTROLS:FMLABEL>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox ID="ProductsTextbox" CssClass=tabletext Width=".25in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Products") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="SmithMeter|Delete">
										<HeaderStyle Width="0.5in"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMDeleteLinkButton runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMDatagrid>
						</TD>
					</tr>
					<tr>
						<td style="HEIGHT: 34px">
							<table cellSpacing="0" cellPadding="1" border="0" style="WIDTH: 564px; HEIGHT: 18px">
								<tr>
									<td><FMCONTROLS:FMButton id="AddButton" runat="server" Width="98px" Text="SmithMeter|Add" CssClass="formfieldtitle"
											tabIndex="5"></FMCONTROLS:FMButton></td>
									<td style="WIDTH: 300px"></td>
									<td style="WIDTH: 777px"><FMCONTROLS:FMLABEL id="Label12" runat="server" BackColor="Transparent" Height="8px" ForeColor="Crimson"
											Width="146px" CssClass="formfieldtitle">SmithMeter|* Denotes Required Field</FMCONTROLS:FMLABEL></td>
									<td style="WIDTH: 229px"><FMCONTROLS:FMButton id="OKButton" runat="server" Width="107px" Text="SmithMeter|OK" CssClass="formfieldtitle"
											tabIndex="6"></FMCONTROLS:FMButton></td>
									<td style="WIDTH: 130px"><FMCONTROLS:FMButton id="CancelButton" runat="server" Width="98px" Text="SmithMeter|Cancel" CssClass="formfieldtitle"
											tabIndex="7"></FMCONTROLS:FMButton></td>
								</tr>
							</table>
						</td>
					</tr>
				</TBODY>
			</TABLE>
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				if(!document.getElementById("IDTextBox").disabled)
					document.getElementById("IDTextBox").focus();
			</script>
		</div>
</form>
	</body>
</HTML>
