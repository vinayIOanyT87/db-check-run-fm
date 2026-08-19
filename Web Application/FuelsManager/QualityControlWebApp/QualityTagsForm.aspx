<%@ Page language="c#" Codebehind="QualityTagsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.QualityControlWebApp.QualityTagsForm" %>
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
	<body tabindex="-1" ms_positioning="GridLayout" role="application">
	<form id="Form1" method="post" enctype="multipart/form-data" runat="server" defaultbutton="FindBtn">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="364px" BackColor="Transparent">Quality Tag Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="FindStringLabel" AssociatedControlID="FindTextBox" Style="z-index: 106; left: 32px; position: absolute; top: 40px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Find String:</FMControls:FMLabel>
			<asp:TextBox ID="FindTextBox" Style="z-index: 107; left: 32px; position: absolute; top: 64px"
				runat="server" Width="288px" TabIndex="2" MaxLength="100"></asp:TextBox>
			<FMControls:FMButton ID="FindBtn" Style="z-index: 108; left: 336px; position: absolute; top: 58px"
				TabIndex="3" runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtnOnClick"></FMControls:FMButton>
			<FMControls:FMButton ID="ShowAllButton" Style="z-index: 109; left: 416px; position: absolute; top: 58px"
				TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="ShowAllBtnOnClick"></FMControls:FMButton>
			<table id="Table1" style="z-index: 101; left: 32px; width: 50%; position: absolute; top: 96px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td width="350" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="6" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="QualityTagsFormPageSizeDropDown" ToolTip="Page size" runat="server" TabIndex="7" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
					</td>
				</tr>
				<tr>
					<td style="width: 407px; height: 10px" width="407">
						<FMControls:FMGridView ID="QualityTagsDataGrid" runat="server" RowHeaderColumn="Quality Tag Name"
							AutoGenerateColumns="false"
							FixedHeaders="false" Width="700px"
							DataKeyNames="SiteGuid,Index"
							PagerStyle-CssClass="pgr"
							AllowSorting="true"
							aria-label="Qualification Tags Grid">
							<FooterStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="Black"></FooterStyle>
							<PagerStyle CssClass="pgr"></PagerStyle>
							<HeaderStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" CssClass="tablecolhead" Font-Bold="True" ForeColor="White" Height="12px"></HeaderStyle>
							<EditRowStyle BackColor="White" BorderStyle="Solid" />
							<SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White"></SelectedRowStyle>
							<AlternatingRowStyle BackColor="#DCDCDC" CssClass="tabletext"></AlternatingRowStyle>
							<Columns>
								<FMControls:FMEditCommandField HeaderText="Edit" />
								<asp:BoundField HeaderText="SiteGuid" Visible="false" DataField="SiteGuid" />
								<asp:BoundField HeaderText="Index" Visible="false" DataField="Index" />
								<asp:BoundField HeaderText="Quality Tag Name" Visible="true" DataField="Name" SortExpression="Name" />
								<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Quality Tag Type" SortExpression="Severity">
									<ItemTemplate>
										<asp:Label ID="lblSeverity" runat="server" Text='<%# Eval("Severity") %>'></asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMDropDownList ID="ddlSeverity" Font-Size="XX-Small" runat="server"
											DataSource="<%# EnumerateSeverityNames() %>" />
									</EditItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Quality Tag Active" SortExpression="Active">
									<ItemTemplate>
										<FMControls:FMCheckBox ID="ddlActive" Font-Size="XX-Small" runat="server"
											Checked='<%# DataBinder.Eval(Container, "DataItem.Active") %>' Enabled="false" />
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMCheckBox ID="ddlActive" Font-Size="XX-Small" runat="server"
											Checked='<%# DataBinder.Eval(Container, "DataItem.Active") %>' Enabled="true" />
									</EditItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Delete" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' />
									</ItemTemplate>
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</asp:TemplateField>
							</Columns>

							<RowStyle BackColor="#EEEEEE" CssClass="tabletext" ForeColor="Black"></RowStyle>
						</FMControls:FMGridView>
					</td>
				</tr>
				<tr>
					<td style="height: 58px">
						<table style="height: 29px" role="presentation" aria-label="layout">
							<tr>
								<td style="width: 163px; height: 36px" valign="middle" width="163">
									<FMControls:FMButton ID="AddButton" runat="server" CssClass="formfieldtitle" Width="98px" Text="Add"
										TabIndex="6"></FMControls:FMButton></td>
								<td style="width: 480px">&nbsp;</td>
								<td style="width: 100px">&nbsp;</td>
								<td>&nbsp;</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</form>
	<script type="text/javascript">

		var findBtn = document.getElementById("FindBtn");
		var findTbBtn = document.getElementById("FindTextBox");

		if (findBtn != null && findTbBtn != null) {
			try {
				findBtn.setActive();
				findTbBtn.focus();
			}
			catch (err) { }
		}
	</script>

</body>
</html>
