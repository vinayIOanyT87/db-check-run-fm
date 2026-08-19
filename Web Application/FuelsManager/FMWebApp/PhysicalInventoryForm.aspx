<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="PhysicalInventoryForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PhysicalInventoryForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
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
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="PhysicalInventory" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 32px; POSITION: absolute; TOP: 80px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<TR>
					<TD style="WIDTH: 407px; HEIGHT: 10px" width="407">
					    <FMControls:FMDataGridFixed id="PhysicalInventoryDataGrid" runat="server" BorderStyle="None" BackColor="White"
							AutoGenerateColumns="False" GridLines="Vertical" Width="900px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3"
							CssClass="tabletext" PageSize="16" Height="435px">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Select">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMSelectLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="TankGuid" HeaderText="IdentityGuid">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Tank ID">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TankID") %>' Style="overflow:hidden;white-space:nowrap;text-overflow:ellipsis">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Product">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' Style="overflow:hidden;white-space:nowrap;text-overflow:ellipsis">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="Status" HeaderText="Status"></asp:BoundColumn>
								<asp:BoundColumn DataField="LevelTimeStamp" HeaderText="Last Level Update">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Level" HeaderText="Level">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="GrossVolume" HeaderText="Gross">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="AvailableGrossVolume" HeaderText="Available">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="RemainingGrossVolume" HeaderText="Remaining">
									<HeaderStyle Wrap="False"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="NetVolume" HeaderText="Net">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="AvailableNetVolume" HeaderText="Available">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="RemainingNetVolume" HeaderText="Remaining">
									<HeaderStyle Wrap="False"></HeaderStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Market">
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox runat="server" CssClass=tabletext Enabled=false Checked='<%# DataBinder.Eval(Container, "DataItem.Market") %>' ID="MarketCheckbox" NAME="MarketCheckbox">
										</asp:CheckBox>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</FMControls:FMDataGridFixed></TD>
				</TR>
			</TABLE>
			<FMControls:FMLabel id="Label3" AssociatedControlID="ProductDropDownList" style="Z-INDEX: 106; LEFT: 32px; POSITION: absolute; TOP: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">Product:</FMControls:FMLabel>
			<FMControls:FMDropDownList id="ProductDropDownList" style="Z-INDEX: 105; LEFT: 88px; POSITION: absolute; TOP: 40px"
				runat="server" CssClass="formfield" Width="128px" AutoPostBack="True" onselectedindexchanged="ProductDropDownList_SelectedIndexChanged"></FMControls:FMDropDownList>
			<asp:Image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMLabel id="Label2" style="Z-INDEX: 104; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="224px" BackColor="Transparent">Physical Inventory</FMControls:FMLabel>
		</div>
</form>
	</body>
</HTML>
