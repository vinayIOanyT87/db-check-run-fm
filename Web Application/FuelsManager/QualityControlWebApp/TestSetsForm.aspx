<%@ Page language="c#" Codebehind="TestSetsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.QualityControlWebApp.TestSetsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body ms_positioning="GridLayout" tabindex="-1">
	<form id="Form1" method="post" runat="server" defaultbutton="FindBtn">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="408px" BackColor="Transparent">Test Set Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="FindStringLabel" AssociatedControlID="FindTextBox" Style="z-index: 106; left: 24px; position: absolute; top: 40px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Find String:</FMControls:FMLabel>
			<asp:TextBox ID="FindTextBox" Style="z-index: 107; left: 24px; position: absolute; top: 64px"
				runat="server" Width="288px" TabIndex="2" MaxLength="100"></asp:TextBox>
			<FMControls:FMButton ID="FindBtn" Style="z-index: 108; left: 328px; position: absolute; top: 58px;"
				TabIndex="3" runat="server" Width="64px" CssClass="formfieldtitle" Text="Find"
				OnClick="FindBtnClick"></FMControls:FMButton>
			<FMControls:FMButton ID="ShowAllButton" Style="z-index: 109; left: 408px; position: absolute; top: 58px"
				TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All"
				OnClick="ShowAllButtonClick"></FMControls:FMButton>

			<table id="Table1" style="z-index: 100; left: 24px; width: 43.18%; position: absolute; top: 129px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td width="498" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="6" OnClick="AddButtonClick" />
					</td>
				</tr>
				<tr>
					<td style="width: 498px; height: 10px" width="498">
						<FMControls:FMDataGridFixed ID="TestSetDataGrid" runat="server" RowHeaderColumn="Test Set Name"
							BorderStyle="None" BackColor="White"
							AutoGenerateColumns="False" GridLines="Vertical" Width="407px" BorderWidth="1px" AllowSorting="True"
							BorderColor="White" CellPadding="3" AllowPaging="False" CssClass="tabletext" Style="left: 1px; top: 0px"
							Height="625px" FixedHeaders="true"
							OnDeleteCommand="TestSetDataGridDeleteCommand"
							OnEditCommand="TestSetDataGridEditCommand"
							OnItemDataBound="TestSetDataGridItemDataBound"
							OnPageIndexChanged="TestSetDataGridPageIndexChanged"
							OnSortCommand="TestSetDataGridSortCommand" aria-label="Test Set Grid">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" ID="EditLinkButton" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="SiteGuid">
									<ItemTemplate>
										<asp:Label ID="SiteGuid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGuid") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="Index">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Test Set Name" SortExpression="TestSetName">
									<HeaderStyle Width="2in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="TestSetName" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TestSetName") %>' Width="2in">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" ID="DeleteLinkButton" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</FMControls:FMDataGridFixed></td>
				</tr>
				<tr>
					<td style="width: 498px; height: 50px" valign="middle" width="498">
						<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add"
							CssClass="formfieldtitle" OnClick="AddButtonClick"></FMControls:FMButton></td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>

