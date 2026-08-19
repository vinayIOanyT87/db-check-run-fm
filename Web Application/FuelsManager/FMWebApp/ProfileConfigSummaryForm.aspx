<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProfileConfigSummaryForm.aspx.cs" Inherits="FuelsManager.FMWebApp.ProfileConfigSummaryForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
		<title></title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
			.style1
			{
				width: 424px;
			}
		</style>
</head>
<body>
	<form id="ProfileConfigurationSummaryform" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position:absolute">
		<asp:image id="FadeImage" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
		<table style="Z-INDEX: 128; LEFT: 27px; POSITION: absolute; TOP: 39px; width: 698px; height: 236px;" 
			border="0" cellpadding="0" cellspacing="0">
			<tr>
				<td colspan="3">
					<FMControls:FMLabel id="ProfileConfigTitleLabel" runat="server" CssClass="headline" Width="272px" 
					BackColor="Transparent">Profiles Configuration</FMControls:FMLabel>
				</td>
			</tr>
			<tr>
				<td colspan="3">&nbsp;</td>
			</tr>
			<tr>
				<td class="style1">
					<FMControls:FMLabel id="FindLabel" runat="server" CssClass="formfieldtitle" 
						BackColor="Transparent">Find String:</FMControls:FMLabel>
				</td>
				<td>
					&nbsp;</td>
				<td>
					&nbsp;</td>
			</tr>
			<tr>
				<td class="style1">
					<FMControls:FMTextBox id="FindTextBox" runat="server" CssClass="formfield" Width="235px" MaxLength="100"></FMControls:FMTextBox>
					&nbsp;&nbsp;
					<FMCONTROLS:FMBUTTON id="FindBtn" runat="server" CssClass="formfieldtitle" 
						Text="Find" Width="64px" onclick="FindButtonOnClick"></FMCONTROLS:FMBUTTON>
					&nbsp; &nbsp;
					<FMCONTROLS:FMBUTTON id="ShowAllBtn" runat="server" CssClass="formfieldtitle" 
						Width="70px" Text="Show All" onclick="ShowAllButtonOnClick"></FMCONTROLS:FMBUTTON>
				</td>
				<td style="width:135px">
					&nbsp;</td>
				<td></td>
			</tr>
			<tr>
				<td colspan="3">&nbsp;</td>
			</tr>
			<tr>
				<td colspan="3">
					<FMCONTROLS:FMBUTTON id="AddTopBtn" runat="server" CssClass="formfieldtitle" 
						Text="Add" Width="98px" onclick="AddBtnOnClick"></FMCONTROLS:FMBUTTON>
					&nbsp;&nbsp;
					<FMControls:FMPageSizeDropDown ID="ProfileSummaryPageSizeDropDown" runat="server" onselectedindexchanged="PageSizeDropDown_SelectedIndexChanged" />
				</td>
			</tr>
			<tr>
				<td class="style1">&nbsp;</td>
				<td>&nbsp;</td>
			</tr>
			<tr>
				<td colspan="3">
				<FMCONTROLS:FMDataGridFixedPaging ID="ProfileDataGrid" runat="server"
								AutoGenerateColumns="False"
								DataKeyNames="SiteIndex, Index"
								BorderStyle="Solid" 
								BackColor="White" 
								GridLines="Vertical"
								Width="100%"
								BorderWidth="1px"
								AllowSorting="True"
								CellPadding="3"
								CssClass="tabletext"
								EmptyDataText="No records found"
								BorderColor="White"
								tabIndex="7"
								ShowHeaderWhenEmpty="True"
								ShowFooterWhenEmpty="False"
								FixedHeaders="True"
								GroupColumnOffset="0"
								GroupingDepth="0" Height="550px" FixedHeight="550px" ShowFooter="True">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

<FooterStyle BackColor="#000000" CssClass="GVFixedFooter" ForeColor="Black"></FooterStyle>

<HeaderStyle BackColor="#000000" CssClass="GVFixedHeader" Font-Bold="True" ForeColor="White"></HeaderStyle>

							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
									<ItemTemplate>
										<FMControls:FMEditLinkButton ID="EditLinkButton" runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="MobileDeviceProfileGuid" 
									HeaderText="MobileDeviceProfileGuid" Visible="False"></asp:BoundColumn>
								<asp:BoundColumn DataField="SiteGuid" HeaderText="SiteGuid" Visible="False">
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ProfileID" HeaderText="Profile ID" 
									SortExpression="ProfileID"></asp:BoundColumn>
								<asp:BoundColumn DataField="ProfileDescription" 
									HeaderText="Profile Description" SortExpression="ProfileDescription"></asp:BoundColumn>
								<asp:BoundColumn DataField="DeviceCount" HeaderText="# Devices" 
									ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" 
									ItemStyle-Width="100px" >
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>

									<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton ID="DeleteLinkButton" runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
						</FMControls:FMDataGridFixedPaging>
				</td>
			</tr>
			<tr>
				<td class="style1">&nbsp;</td>
				<td>&nbsp;</td>
			</tr>
			<tr>
				<td class="style1">
					<FMCONTROLS:FMBUTTON id="AddBottomBtn" runat="server" CssClass="formfieldtitle" 
						Text="Add" Width="98px" onclick="AddBtnOnClick"></FMCONTROLS:FMBUTTON>
				</td>
				<td>&nbsp;</td>
			</tr>
		</table>
	</div>
</form>
</body>
</html>
