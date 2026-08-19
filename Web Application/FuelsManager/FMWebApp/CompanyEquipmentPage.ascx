<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanyEquipmentPage.ascx.cs" Inherits="FMWebApp.CompanyEquipmentPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	    <style type="text/css">
            .style1
            {
                width: 240px;
            }
            .style2
            {
                width: 46px;
            }
            .style3
            {
                width: 140px;
            }
            .style4
            {
                width: 306px;
            }
        </style>
	<SCRIPT>
	    function EquipmentSelect(equipmentTextBoxId, mode)
	    {
	        var equipmentTextBox = document.getElementById(equipmentTextBoxId);
	        var typeDropDownList = document.getElementById("tcCompanyTabs_tpEquipmentPage_CompanyEquipmentPage_TypeDropDownList");

	        showModalDialogFrame({
	            url: "../FMWebApp/EquipmentSelectForm.aspx?Type=" + typeDropDownList.value + "&EntityType=Companies&Map=true&Source=CompanyEquipment&Mode=" + mode,
	            width: 855,
	            height: 690,
	            title: "Equipment Select",
	            onClose: function ()
	            {
						if (this.returnValue != null) {
							var result = this.returnValue;
							if (result != null && result.length > 0) {
								for (var i = 0; i < result.length; i++) {
									var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

									if (i === 0) {
										equipmentTextBox.value = newAsciiStr;
									}
									else {
										equipmentTextBox.value += "|" + newAsciiStr;
									}
								}

								equipmentTextBox.onchange();
							}
						}
	            }
	        });
	    }
</SCRIPT>
	</HEAD>
	<body>
	    <p>
            &nbsp;</p>
	    <table style="width:300px; position: absolute; top:5px; left: 10px;">
            <tr>
                <td class="style2" valign="top">
                    <FMCONTROLS:FMLABEL id="Fmlabel2" AssociatedControlID="TypeDropDownList" style="Z-INDEX: 125; LEFT: 0px;" runat="server"
                    CssClass="formfieldtitle" Width="64px" BackColor="Transparent" Height="16px">Type:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style1">
                    <FMCONTROLS:FMDROPDOWNLIST id="TypeDropDownList" style="Z-INDEX: 111; "
                    tabIndex="16" runat="server" CssClass="formfield" Width="240px" AutoPostBack="True" onselectedindexchanged="TypeDropDownList_SelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST>
                </td>
            </tr>
             <tr>
                <td class="style2" valign="top">
                    <FMCONTROLS:FMLABEL id="Fmlabel3" style="Z-INDEX: 125; LEFT: 0px;" runat="server"
                    CssClass="formfieldtitle" Width="64px" BackColor="Transparent" Height="16px">Assigned:</FMCONTROLS:FMLABEL>
                </td>
                <td class="style1" valign="top">
	                <table id="Table1" style="Z-INDEX:100; LEFT: 0px; HEIGHT: 10px; "
		                cellSpacing="0" cellPadding="1" border="0">
		                <tr>
			                <TD height="10" valign="top">
                                <FMCONTROLS:FMDATAGRID id="AssignedEquipmentDataGrid" tabIndex="5" RowHeaderColumn="ID"
                                    runat="server" CssClass="tabletext" Height="10px"
					                Width="240px" BackColor="White" PageSize="12" AllowPaging="True" CellPadding="3" 
                                    BorderColor="White" AllowSorting="True" BorderWidth="1px"
					                GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None">
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
		                </tr>
            		    <tr>
			                <td height="36" valign="top">
				                <table>
					                <tr>
						                <td width="84px" height="10"><input class="formfieldtitle" id="CompanyEquipmentPage_AssignButton" style="WIDTH: 80px"
								                onclick="EquipmentSelect('tcCompanyTabs_tpEquipmentPage_CompanyEquipmentPage_AssignEquipmentTextBox', 'Assign')" type="button" value="Assign" runat="server"></td>
						                <td height="10"><input class="formfieldtitle" id="CompanyEquipmentPage_UnassignButton" style="WIDTH: 80px"
								                onclick="EquipmentSelect('tcCompanyTabs_tpEquipmentPage_CompanyEquipmentPage_UnassignEquipmentTextBox', 'Unassign')" type="button" value="Unassign" runat="server"></td>
						                <td><asp:textbox id="AssignEquipmentTextBox" ToolTip="Assign Equipment" runat="server" Width="82px" BackColor="Transparent" BorderColor="Transparent"
								                BorderStyle="None" AutoPostBack="True" ForeColor="Transparent" ontextchanged="AssignEquipmentTextBox_TextChanged"></asp:textbox></td>
						                <td><asp:textbox id="UnassignEquipmentTextBox" ToolTip="Unassign Equipment" runat="server" Width="17px" 
                                                BackColor="Transparent" BorderColor="Transparent"
								                BorderStyle="None" AutoPostBack="True" ForeColor="Transparent"
                                                ontextchanged="UnassignEquipmentTextBox_TextChanged"></asp:textbox></td>
					                </tr>
				                </table>
			                </td>
		                </tr>
    	            </table>
                </td>
            </tr>
        </table>
	</body>
</HTML>
