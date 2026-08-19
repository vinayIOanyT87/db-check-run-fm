<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="TankLoadArmAssignmentForm.aspx.cs" AutoEventWireup="True" Inherits="LoadRackWebApp.TankLoadArmAssignmentForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				BackColor="Transparent" Width="300px" CssClass="headline">Tank Assignment Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label8" Style="z-index: 112; left: 16px; position: absolute; top: 40px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Tank Assignment:</FMControls:FMLabel>
			<table id="Table1" style="z-index: 101; left: 16px; width: 10px; position: absolute; top: 64px; height: 10px"
				cellspacing="0" cellpadding="1" width="700" border="0" role="presentation" aria-label="layout">
				<tbody>
					<tr>
						<td width="498" height="36" valign="middle">
							<FMControls:FMPageSizeDropDown ID="ArmAssignFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
							<FMControls:FMDropDownList ID="StationFilterDropDown" Style="left: 175px; position: absolute; top: 8px" TabIndex="1"
								runat="server" Width="200px" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="StationFilterDropDownSelectedIndexChanged" ToolTip="Station Filter">
							</FMControls:FMDropDownList>
						</td>
					</tr>
					<tr>
						<td style="width: 686px; height: 10px" width="686">
							<FMControls:FMDataGrid ID="LocationAssignmentDataGrid" TabIndex="1" runat="server" BackColor="White" Width="432px"
								CssClass="tabletext" AllowPaging="True" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
								BorderColor="White" CellPadding="3" PageSize="16" aria-label="Location Assignment Grid">
								<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Edit">
										<HeaderStyle Width="55px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<ASP:LINKBUTTON id="EditButton" runat="server" CausesValidation="false" CommandName="Edit" Text="<img src=../FMWebApp/Images/Edit.gif border=0 align=absmiddle alt='Edit this item'>"></ASP:LINKBUTTON>
										</ItemTemplate>
										<EditItemTemplate>
											<ASP:LINKBUTTON id="Linkbutton1" runat="server" CommandName="Update" Text="<img src=../FMWebApp/Images/Update.gif border=0 align=absmiddle alt='Update this item'>"></ASP:LINKBUTTON>&nbsp; 
											<ASP:LINKBUTTON id="Linkbutton2" runat="server" CausesValidation="false" CommandName="Cancel" Text="<img src=../FMWebApp/Images/Cancel.gif border=0 align=absmiddle alt='Cancel Edit on this item'>"></ASP:LINKBUTTON>
										
</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Station">
										<ItemTemplate>
											<asp:Label runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.StationID") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="false" HeaderText="StationGuid">
										<ItemTemplate>
											<asp:Label runat="server" ID="StationGuidLabel" Text='<%# DataBinder.Eval(Container, "DataItem.StationGuid") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Arm">
										<ItemTemplate>
											<asp:Label runat="server" CssClass="tabletext" ID="ArmLabel" Text='<%# DataBinder.Eval(Container, "DataItem.Arm") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="false" HeaderText="ComponentIndex">
										<ItemTemplate>
											<asp:Label runat="server" ID="ComponentIndexLabel" Text='<%# DataBinder.Eval(Container, "DataItem.ComponentIndex") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Product Type">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductType") %>' ID="ProductTypeLabel">
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Product">
										<ItemTemplate>
											<asp:Label runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="false" HeaderText="ProductGuid">
										<ItemTemplate>
											<asp:Label runat="server" ID="ProductGuidLabel" Text='<%# DataBinder.Eval(Container, "DataItem.ProductGuid") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Location Type">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LocationType") %>' ID="LocationTypeLabel">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<FMControls:FMDropDownList CssClass="tabletext" runat="server" Enabled="True" ID="TypeDropDownList" DataSource="<%# EnumerateLocationTypes()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged">
											</FMControls:FMDropDownList>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Location">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LocationID") %>' ID="Label3">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:DropDownList CssClass="tabletext" runat="server" Enabled="True" ID="LocationDropDownList" DataSource="<%# EnumerateLocations()%>" DataTextField="Text" DataValueField="Value">
											</asp:DropDownList>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="false" HeaderText="LocationGuid">
										<ItemTemplate>
											<asp:Label runat="server" ID="LocationGuidLabel" Text='<%# DataBinder.Eval(Container, "DataItem.LocationGuid") %>'>
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMControls:FMDataGrid></td>
					</tr>
				</tbody>
			</table>
		</div>
	</form>
</body>
</html>
