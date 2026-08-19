<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssetTrackingDeviceSelectForm.aspx.cs" Inherits="FuelsManager.FMWebApp.AssetTrackingDeviceSelectForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ OutputCache Location="None" VaryByParam="None" %>
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
 	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
<%=  Global.LinkAccessibilityCssUrl(Session) %>

    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>"  defer="defer"></script>
  <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
  <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
</head>
<body ms_positioning="GridLayout">
	<script>
		function Select(deviceId, title)
		{
			var result = new Array();
			result[0] = deviceId;
			result[1] = title;

			setWindowReturnValue(result);
			closeDialogWindow();
		}

		function MultipleSelect()
		{
			var result = new Array();
			var assetTrackingDeviceTable = document.getElementById("AssetTrackDeviceDataGrid");

			if (assetTrackingDeviceTable != null) {
				var resultIndex = 0;
				for (var index = 0; index < assetTrackingDeviceTable.rows.length; index++) {
					if (assetTrackingDeviceTable.rows[index].className === "GVFixedFooter" ||
						assetTrackingDeviceTable.rows[index].className === "GVFixedHeader") {
						continue;
					}

					if (assetTrackingDeviceTable.rows[index].cells[0].childNodes[0].checked) {
						result[resultIndex] = assetTrackingDeviceTable.rows[index].cells[3].innerText;
						resultIndex++;
					}
				}
			}

			setWindowReturnValue(result);
			closeDialogWindow();
		}

		function NoSelect()
		{
			var result = new Array();
			setWindowReturnValue(result);
			closeDialogWindow();
		}
	</script>
	<form id="DeviceSelectForm" method="post" runat="server">
		<asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
			ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
		<asp:TextBox ID="FindTextBox" alt="Find text" Style="z-index: 101; left: 8px; position: absolute; top: 14px" TabIndex="2"
			runat="server" Width="300px" CssClass="formfield"></asp:TextBox>
		<FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
			TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="FindAllBtnOnClick"></FMControls:FMButton>
		<FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px" TabIndex="3"
			runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtnOnClick"></FMControls:FMButton>

		<table id="MainTable" style="z-index: 101; left: 8px; width: 50%; position: absolute; top: 45px; height: 10px"
							cellspacing="0" cellpadding="1" border="0">
			<tr>
				<td width="350" height="36" valign="middle">
					<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
						TabIndex="6" />
					<FMControls:FMLabel Width="500px" ID="lblWarning" runat="server"
						CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
				</td>
			</tr>
			<tr>
				<td style="width: 549px; height: 10px" width="549">
					<FMControls:FMDataGridFixed ID="AssetTrackDeviceDataGrid" TabIndex="5" runat="server" BackColor="White" Width="800px"
						CssClass="tabletext" PageSize="12" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
						GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" Height="380px">
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<Columns>
							<asp:TemplateColumn>
								<HeaderStyle Width="0.125in"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Edit">
								<HeaderStyle Width="55px"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								<ItemTemplate>
									<FMControls:FMEditLinkButton runat="server" ID="EditLinkBtn" NAME="EditLinkBtn" />
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
							<asp:BoundColumn DataField="DeviceId" HeaderText="Device ID">
								<HeaderStyle Width="2in"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Description" HeaderText="Description">
								<HeaderStyle Width="1in"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="ModelNumber" HeaderText="Model Number">
								<HeaderStyle Width="1in"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="SerialNumber" HeaderText="Serial Number">
								<HeaderStyle Width="2in"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Active" HeaderText="Active">
								<HeaderStyle Width="2in"></HeaderStyle>
							</asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Delete">
								<HeaderStyle Width="0.5in"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								<ItemTemplate>
									<FMControls:FMDeleteLinkButton runat="server" ID="DeleteLinkBtn" NAME="DeleteLinkBtn" />
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</FMControls:FMDataGridFixed></td>
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
