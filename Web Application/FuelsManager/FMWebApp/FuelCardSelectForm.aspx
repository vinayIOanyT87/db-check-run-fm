<%@ Page Language="c#" CodeBehind="FuelCardSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.FuelCardSelectForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
	<title></title>
	<base target="_self">
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<%=  Global.LinkAccessibilityCssUrl(Session) %>

	<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
</head>
<body ms_positioning="GridLayout">
	<script type="text/javascript">
		function Select(FuelCardID, Title)
		{
			var Result = new Array();
			Result[0] = FuelCardID;
			Result[1] = Title;
			setWindowReturnValue(Result);
			closeDialogWindow();
		}

		function MultipleSelect()
		{
			var Result = new Array();
			var FuelCardTable = document.getElementById("FuelCardDataGrid");
			if (FuelCardTable != null)
			{
				var resultIndex = 0;
				for (index = 0; index < FuelCardTable.rows.length; index++)
				{
					if (FuelCardTable.rows[index].className == "GVFixedFooter" ||
						FuelCardTable.rows[index].className == "GVFixedHeader")
					{
						continue;
					}

					if (FuelCardTable.rows[index].cells[0].childNodes[0].checked)
					{
						Result[resultIndex] = FuelCardTable.rows[index].cells[2].innerText;
						resultIndex++;
					}
				}
			}
			setWindowReturnValue(Result);
			closeDialogWindow();
		}

		function NoSelect()
		{
			var Result = new Array();
			setWindowReturnValue(Result);
			closeDialogWindow();
		}
	</script>
	<form id="Form1" method="post" runat="server">
		<asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
			ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
		<asp:TextBox ID="FindTextBox" Style="z-index: 101; left: 8px; position: absolute; top: 14px" TabIndex="2"
			runat="server" Width="300px" CssClass="formfield" MaxLength="100"></asp:TextBox>
		<FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px" TabIndex="3"
			runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtn_OnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
			TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="FindAllBtn_OnClick"></FMControls:FMButton>
		<table id="Table1" style="z-index: 101; left: 8px; width: 50%; position: absolute; top: 45px; height: 10px"
			cellspacing="0" cellpadding="1" border="0">
			<tr>
				<td width="350" height="36" valign="middle">
					<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" TabIndex="6" />
					<FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
				</td>
			</tr>
			<tr>
				<td style="width: 549px; height: 10px" width="549">
					<FMControls:FMDataGrid ID="FuelCardDataGrid" TabIndex="5" runat="server" BackColor="White" Width="8.5in" RowHeaderColumn="Fuel Card ID"
						CssClass="tabletext" PageSize="12" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
						GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None">
						<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
						<Columns>
							<asp:TemplateColumn>
								<HeaderStyle Width="0.125in"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Edit">
								<HeaderStyle Width="55px"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								<ItemTemplate>
									<FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton1" NAME="Fmeditlinkbutton1" />
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
							<asp:BoundColumn DataField="ID" HeaderText="ID"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Manager">
								<ItemTemplate>
									<asp:Label runat="server" ID="ManagerLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.ManagerTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.Manager") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Owner">
								<ItemTemplate>
									<asp:Label runat="server" ID="OwnerLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.OwnerTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.Owner") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Shipper">
								<ItemTemplate>
									<asp:Label runat="server" ID="ShipperLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipperTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.Shipper") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Bill To">
								<ItemTemplate>
									<asp:Label runat="server" ID="BillToLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.BillToTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.BillTo") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Ship To">
								<ItemTemplate>
									<asp:Label runat="server" ID="ShipToLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.ShipTo") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn HeaderText="Provider" DataField="Provider"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="Activity Status" DataField="Status"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Delete">
								<HeaderStyle Width="0.5in"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								<ItemTemplate>
									<FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" NAME="Fmdeletelinkbutton1" />
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
						<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
					</FMControls:FMDataGrid></td>
				<tr>
					<td width="350" height="36" valign="middle">
						<FMControls:FMButton ID="AddButton1" TabIndex="3" runat="server" CssClass="formfieldtitle" Width="100px"
							Text="Add"></FMControls:FMButton>
					</td>
				</tr>
		</table>
	</form>
</body>
</html>
