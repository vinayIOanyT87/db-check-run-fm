<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MobileDeviceSummaryPage.aspx.cs" Inherits="FuelsManager.FMWebApp.MobileDeviceSummaryPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="../MenuBar/FMMenuBar.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
		<title></title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
	<form id="MobileDeviceSummaryform" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
	<div id="pageContent" style="position:absolute">
		<asp:image id="FadeImage" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image> 
		<table style="Z-INDEX: 128; LEFT: 27px; POSITION: absolute; TOP: 39px; width: 698px; height: 236px;" 
				border="0" cellpadding="0" cellspacing="0">
			<tr>
				<td colspan="3">
					<FMControls:FMLabel id="MobileDeviceSummaryTitleLabel" runat="server" CssClass="headline" Width="272px" 
					BackColor="Transparent">Mobile Devices</FMControls:FMLabel>
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
					<FMCONTROLS:FMButton id="FindBtn" runat="server" CssClass="formfieldtitle" 
						Text="Find" Width="64px" onclick="FindButtonOnClick"></FMCONTROLS:FMButton>
					&nbsp; &nbsp;
					<FMCONTROLS:FMButton id="ShowAllBtn" runat="server" CssClass="formfieldtitle" 
						Width="70px" Text="Show All" onclick="ShowAllButtonOnClick"></FMCONTROLS:FMButton>
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
					<FMCONTROLS:FMButton id="AddTopBtn" runat="server" CssClass="formfieldtitle" 
						Text="Add" Width="98px" onclick="AddBtnOnClick"></FMCONTROLS:FMButton>
					&nbsp;&nbsp;
					<FMControls:FMPageSizeDropDown ID="MobileDeviceSummaryPageSizeDropDown" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
				</td>
			</tr>
			<tr>
				<td class="style1">&nbsp;</td>
				<td>&nbsp;</td>
			</tr>
			<tr>
				<td colspan="3">
				<FMCONTROLS:FMDataGridFixedPaging ID="MobileDeviceDataGrid" runat="server"
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
								ShowFooterWhenEmpty="True"
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
								<asp:BoundColumn DataField="mobileDeviceGuid" 
									HeaderText="mobileDeviceGuid" Visible="False"></asp:BoundColumn>
								<asp:BoundColumn DataField="SiteGuid" HeaderText="SiteGuid" Visible="False">
								</asp:BoundColumn>
								<asp:BoundColumn DataField="mobileDeviceId" HeaderText="Mobile Device ID" 
									SortExpression="mobileDeviceId"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" 
									HeaderText="Mobile Device Description" SortExpression="Description"></asp:BoundColumn>
								<asp:BoundColumn DataField="mobileDeviceType" HeaderText="Device Type" 
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
					<FMCONTROLS:FMButton id="AddBottomBtn" runat="server" CssClass="formfieldtitle" 
						Text="Add" Width="98px" onclick="AddBtnOnClick"></FMCONTROLS:FMButton>
				</td>
				<td>&nbsp;</td>
			</tr>
		</table>  
	</div>
	</form>
</body>
</html>
