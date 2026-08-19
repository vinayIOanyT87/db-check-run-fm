<%@ Page language="c#" AutoEventWireup="True" Codebehind="MaintenanceReasonsForm.aspx.cs" Inherits="FuelsManager.FMWebApp.MaintenanceReasonsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title></title>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	</head>
	
	<body ms_positioning="GridLayout" tabindex="-1">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			
			<!-- Top area -->
			<asp:ScriptManager ID="oScriptManager" runat="server" />

			<asp:Image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>

			<FMControls:FMLabel id="LabelHeadline" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="408px" BackColor="Transparent">Maintenance Reasons Configuration</FMControls:FMLabel>

			<!-- Button and droplist -->
			<table id="Table1" style="Z-INDEX: 100; LEFT: 32px;POSITION: absolute; TOP: 48px; HEIGHT: 10px"
				cellspacing="0" cellpadding="1" border="0">

				<tr>
					<td height="36" valign="middle">
						<FMControls:FMButton width="100px" id="AddButtonTop" runat="server" Text="Add" 
							CssClass="formfieldtitle" tabIndex="6" onclick="AddButtonClick" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="PageSizeDropDown" ToolTip="Page size" runat="server"
							onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
					</td>
				</tr>

				<tr>
					<td style="HEIGHT: 10px">

						<!-- Column 0 - Edit, Update, and Cancel buttons -->
						<!-- Column 1 - Delete button -->
						<!-- Column 2 - SiteGuid (hidden) -->
						<!-- Column 3 - Zero-based item number in this grid - not MaintenanceReasonGuid (hidden) -->
						<!-- Column 4 - ID -->
						<!-- Column 5 - Description -->
						<FMControls:FMDataGrid id="MaintenanceReasonsDataGrid" runat="server" BorderStyle="None" BackColor="White" RowHeaderColumn="ID"
							AutoGenerateColumns="False" GridLines="Vertical" Width="600px" BorderWidth="1px" AllowSorting="True"
							BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext" style="LEFT: 1px; TOP: 0px"
							PageSize="16" oncancelcommand="MaintenanceReasonsDataGridCancelCommand" 
							ondeletecommand="MaintenanceReasonsDataGridDeleteCommand" 
							oneditcommand="MaintenanceReasonsDataGridEditCommand" 
							onitemdatabound="MaintenanceReasonsDataGridItemDataBound" 
							onpageindexchanged="MaintenanceReasonsDataGridPageIndexChanged" 
							onupdatecommand="MaintenanceReasonsDataGridUpdateCommand">
							
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
							
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton runat="server" />&nbsp;
										<FMControls:FMCancelLinkButton runat="server" />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label runat="server" ID="IndexLabel"
											Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'></asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								
								<asp:TemplateColumn>
								   <HeaderTemplate><FMControls:FMLabel ID="ID" runat="server" Text="ID" /><span style="COLOR: red"> *</span></HeaderTemplate>
									<HeaderStyle></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server"
											Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ></asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" id="IDTextBox" ToolTip="Maintenance reason ID" CssClass="tabletext" MaxLength="30" style="width:200px" aria-required="true"
											Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ></asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								
								<asp:TemplateColumn>
								   <HeaderTemplate><FMControls:FMLabel ID="Description" runat="server" Text="Description" /><span style="COLOR: red"> *</span></HeaderTemplate>
									<HeaderStyle></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server" style="width:250px"
											Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' ></asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" ID="DescriptionTextBox" ToolTip="Description" CssClass="tabletext" MaxLength="50" style="width:250px" aria-required="true"
											Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' ></asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></TD>
				</tr>
			
				<tr>
					<td width="498" height="36" valign="middle">
						<FMControls:FMButton width="100px" id="AddButtonBottom" runat="server" Text="Add" 
							CssClass="formfieldtitle" tabIndex="6" onclick="AddButtonClick"/>
					</td>
				</tr>
			</table>
		</div>
</form>
	</body>
</html>
