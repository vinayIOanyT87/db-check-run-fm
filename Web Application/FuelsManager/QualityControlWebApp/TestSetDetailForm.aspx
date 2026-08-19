<%@ Page Language="c#" CodeBehind="TestSetDetailForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.QualityControlWebApp.TestSetDetailForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
	<meta name="CODE_LANGUAGE" content="C#"/>
	<meta name="vs_defaultClientScript" content="JavaScript"/>
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="TestSetTitleLabel" Style="z-index: 103; left: 8px; position: absolute; top: 8px"
				runat="server" CssClass="headline" Width="500px" BackColor="Transparent">Test Set Detail Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label2" AssociatedControlID="TestSetNameTextbox" Style="z-index: 101; left: 9px; position: absolute; top: 50px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Test Set Name:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label6" Style="z-index: 102; left: 108px; position: absolute; top: 48px; width: 12px;"
				runat="server" BackColor="Transparent" Height="8px"
				ForeColor="Crimson">*</FMControls:FMLabel>
			<asp:TextBox ID="TestSetNameTextbox" Style="z-index: 109; left: 123px; position: absolute; top: 48px"
				runat="server" CssClass="formfield" Width="264px" MaxLength="80" aria-required="true"></asp:TextBox>
			<FMControls:FMCheckBox ID="DLAEnergyCheckBox" Style="z-index: 109; left: 450px; position: absolute; top: 48px"
				runat="server" CssClass="formfield" Width="264px" Text="Send to EBS"></FMControls:FMCheckBox>
			<FMControls:FMLabel ID="FMLABEL1" Style="z-index: 101; left: 12px; position: absolute; top: 87px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Test Assignment:</FMControls:FMLabel>

			<table id="Table1" style="z-index: 100; left: 9px; width: 43.18%; position: absolute; top: 111px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td colspan="2" width="498" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							TabIndex="6" OnClick="AddButtonClick" />
						&nbsp;&nbsp;
				<FMControls:FMPageSizeDropDown ID="TestsDataGridPageSizeDropDown" ToolTip="Page size" runat="server"
					OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
					</td>
				</tr>
				<tr>
					<td colspan="2" style="width: 498px; height: 10px" width="498">
						<FMControls:FMDataGrid ID="TestsDataGrid" runat="server" BorderStyle="None" BackColor="White" RowHeaderColumn="Test Name"
							AutoGenerateColumns="False" GridLines="Vertical" Width="407px" BorderWidth="1px"
							AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							Style="left: 1px; top: 0px" PageSize="5" OnEditCommand="TestsDataGridEditCommand"
							OnCancelCommand="TestsDataGridCancelCommand" OnDeleteCommand="TestsDataGridDeleteCommand"
							OnItemDataBound="TestsDataGridItemDataBound" OnPageIndexChanged="TestsDataGridPageIndexChanged"
							OnUpdateCommand="TestsDataGridUpdateCommand" UseDataDictionary="True" aria-label="Test Set Detail Grid">
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
										<FMControls:FMUpdateLinkButton runat="server" />&nbsp;<FMControls:FMCancelLinkButton
											runat="server" />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'
											ID="IndexLabel">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Test Name">
									<HeaderStyle Width="2in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="TestName" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TestName") %>'
											Width="2in">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:DropDownList ID="TestNameDropDownList" runat="server" CssClass="tabletext" Width="2in"
											MaxLength="30">
										</asp:DropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
								Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid>
					</td>
				</tr>
				<tr>
					<td style="height: 50px" valign="middle">
						<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
							OnClick="AddButtonClick" />
					</td>
					<td>
						<FMControls:FMButton ID="OK" Style="z-index: 120;" runat="server" CssClass="formfieldtitle"
							Text="OK" Width="56px" OnClick="OkClick" />
						&nbsp;
				<FMControls:FMButton ID="Cancel" Style="z-index: 121;" runat="server" CssClass="formfieldtitle"
					Text="Cancel" Width="56px" OnClick="CancelClick" />
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<FMControls:FMLabel ID="FMLABEL7" Style="z-index: 122" runat="server" BackColor="Transparent"
							CssClass="formfieldtitle" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
					</td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
