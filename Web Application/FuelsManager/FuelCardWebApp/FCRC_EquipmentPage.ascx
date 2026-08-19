<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FCRC_EquipmentPage.ascx.cs" Inherits="FuelsManager.FuelCardWebApp.FCRC_EquipmentPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
	<head>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<script type="text/javascript">
			function EntitySelect(entityTextBoxId, mode)
			{
				var entityTextBox = document.getElementById(entityTextBoxId);
				var typeDropDownList = document.getElementById("tcFCRCDetailTabs_tpEquipmentPage_FCRC_EquipmentPage_TypeDropDownList");

				showModalDialogFrame({
				    url: "../FMWebApp/EquipmentSelectForm.aspx?Type=" + typeDropDownList.value + "&EntityType=Fuel Card&Source=FuelCardEquipment&Mode=" + mode,
				    width: 855,
				    height: 560,
				    onClose: function ()
				    {
				        if (this.returnValue != null && this.returnValue.length > 0)
				        {
				            for (var nextItem = 0; nextItem < this.returnValue.length; nextItem++)
				            {
				                var newAsciiValue = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[nextItem]);

				                if ( nextItem === 0 )
				                {
				                    entityTextBox.value = newAsciiValue;
				                }
				                else
				                {
				                    entityTextBox.value += "|" + newAsciiValue;
				                }
				            }

				            entityTextBox.onchange();
				        }
				    }
				});
			}
		</script>
	</head>
	<body>
		<TABLE id="Table1" style="Z-INDEX: 113; LEFT: 0px; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
			cellSpacing="0" cellPadding="1" width="350" border="0">
			<TR>
			    <td>
		            <FMCONTROLS:FMLABEL id="Fmlabel2" style="Z-INDEX: 125;" runat="server"
			            CssClass="formfieldtitle" Width="64px" BackColor="Transparent">Assigned:</FMCONTROLS:FMLABEL>
		            <FMCONTROLS:FMDROPDOWNLIST id="TypeDropDownList" style="Z-INDEX: 111; LEFT: 104px;"
			            tabIndex="16" runat="server" CssClass="formfield" Width="240px" AutoPostBack="True" onselectedindexchanged="TypeDropDownListSelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST>
			        <br />
			        <br />
			    </td>
			</tr>
			<tr>
				<TD width="368" height="10">
					<FMCONTROLS:FMDATAGRID id="AssignedEquipmentDataGrid" tabIndex="5" 
						runat="server" CssClass="tabletext" Height="10px"
						Width="368px" BackColor="White" PageSize="12" AllowPaging="True" CellPadding="3" 
						BorderColor="White" AllowSorting="True" BorderWidth="1px"
						GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" 
						onitemdatabound="AssignedEquipmentDataGridItemDataBound" 
						onpageindexchanged="AssignedEquipmentDataGridPageIndexChanged">
						<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
						<Columns>
							<asp:TemplateColumn HeaderText="ID">
								<HeaderStyle Width="3in"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width="2.5in" runat="server" ID="IDLabel"></asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
						<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
					</FMCONTROLS:FMDATAGRID>
				</TD>
			</TR>
			<TR>
				<TD width="368" height="36">
					<table>
						<tr>
							<td width="84" height="10"><input class="formfieldtitle" id="FCRC_EquipmentPage_AssignButton" style="WIDTH: 80px"
									onclick="EntitySelect('AssignEntitiesTextBox','Assign')" type="button" value="Assign"/></td>
							<td height="10"><input class="formfieldtitle" id="FCRC_EquipmentPage_UnassignButton" style="WIDTH: 80px"
									onclick="EntitySelect('UnassignEntitiesTextBox','Unassign')" type="button" value="Unassign"/></td>
							<td><asp:textbox id="AssignEntitiesTextBox" ClientIDMode="Static" runat="server" Width="82px" BackColor="White" BorderColor="White"
									BorderStyle="None" AutoPostBack="True" ForeColor="White" ontextchanged="AssignEntitiesTextBoxTextChanged" 
									style="visibility:hidden"></asp:textbox></td>
							<td><asp:textbox id="UnassignEntitiesTextBox" ClientIDMode="Static" runat="server" Width="82px" BackColor="White" BorderColor="White"
									BorderStyle="None" AutoPostBack="True" ForeColor="White" ontextchanged="UnassignEntitiesTextBoxTextChanged"
									style="visibility:hidden"></asp:textbox></td>
						</tr>
					</table>
				</TD>
			</TR>
		</TABLE>
	</body>
</html>
