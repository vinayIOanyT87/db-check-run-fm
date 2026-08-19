<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="CompanyGroupGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.CompanyGroupGeneralPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
<SCRIPT>
	function EntitySelect(entityTextBoxId, mode)
	{
		var entityTextBox = document.getElementById(entityTextBoxId);
		
		if (mode === 'Unassign')
		{
		    showModalDialogFrame({
                url: "../FMWebApp/CompanySelectForm.aspx?Role=MAX_COMPANY_ROLE&Map=COMPANY_GROUP_COMPANY_MAP&Mode=" + mode,
                width: 855,
                height: 690,
                title: "Company Select",
		        onClose: function ()
		        {
					  if (this.returnValue != null && this.returnValue.length > 0) {
						  for (i = 0; i < this.returnValue.length; i++) {
							  if (i == 0)
								  entityTextBox.value = this.returnValue[i];
							  else
								  entityTextBox.value += "|" + this.returnValue[i];
						  }
						  entityTextBox.onchange();
					  }
		        }
		    });
		}
		else
		{
		    showModalDialogFrame({
                url: "../FMWebApp/CompanySelectForm.aspx?Role=CUSTOMER_SHIPTO&Map=COMPANY_GROUP_COMPANY_MAP&Mode=" + mode,
                width: 855,
                height: 690,
                title: "Company Select",
		        onClose: function ()
		        {
		            if (this.returnValue != null && this.returnValue.length > 0)
		            {
							for (i = 0; i < this.returnValue.length; i++) {
								if (i == 0)
									entityTextBox.value = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[i]);
								else
									entityTextBox.value += "|" + ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[i]);
							}
							entityTextBox.onchange();
		            }
		        }
		    });
		}
	}
</SCRIPT>
	<body>
        <FMControls:FMLabel ID="Fmlabel1" Style="z-index: 102; left: 0px; position: absolute; top: 48px" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Assigned Companies:</FMControls:FMLabel>
        <FMControls:FMLabel ID="Label1" AssociatedControlID="Name" Style="z-index: 102; left: 0px; position: absolute; top: 16px" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Company Group ID:</FMControls:FMLabel>
        <FMControls:FMLabel ID="Label8" Style="z-index: 104; left: 144px; position: absolute; top: 16px" runat="server"
            BackColor="Transparent" Width="8px" ForeColor="Crimson" Height="8px">*</FMControls:FMLabel>
        <asp:TextBox ID="Name" Style="z-index: 103; left: 160px; position: absolute; top: 16px" runat="server" aria-required="true"
            BackColor="White" Width="240px" CssClass="formfield" MaxLength="30"></asp:TextBox>
        <table id="Table1" style="z-index: 113; left: 0px; position: absolute; top: 80px; height: 10px"
            cellspacing="0" cellpadding="1" width="350" border="0">
            <tr>
                <td width="368" height="10">
                    <FMControls:FMDataGrid ID="AssignedEntitiesDataGrid" TabIndex="5" runat="server" CssClass="tabletext" Height="10px" RowHeaderColumn="ID"
                        Width="400px" BackColor="White" PageSize="12" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
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
                    </FMControls:FMDataGrid></td>
            </tr>
            <tr>
                <td width="368" height="36">
                    <table>
                        <tr>
                            <td width="83" height="10">
                                <input class="formfieldtitle" id="CompanyGroupGeneralPage_AssignButton" style="width: 80px"
                                    onclick="EntitySelect('AssignEntitiesTextBox', 'Assign')" type="button" value="Assign"></td>
                            <td height="10">
                                <input class="formfieldtitle" id="CompanyGroupGeneralPage_UnassignButton" style="width: 80px"
                                    onclick="EntitySelect('UnassignEntitiesTextBox', 'Unassign')" type="button"
                                    value="Unassign"></td>
                            <td>
                                <asp:TextBox ID="AssignEntitiesTextBox" ToolTip="Assign Entities" ClientIDMode="Static" runat="server" Width="82px"
                                    BackColor="Transparent" BorderColor="White"
                                    BorderStyle="None" AutoPostBack="True" ForeColor="White"
                                    OnTextChanged="AssignEntitiesTextBoxTextChanged"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="UnassignEntitiesTextBox" ToolTip="Unassign Entities" ClientIDMode="Static" runat="server" Width="82px" BackColor="Transparent" BorderColor="White"
                                    BorderStyle="None" AutoPostBack="True" ForeColor="White" OnTextChanged="UnassignEntitiesTextBoxTextChanged"></asp:TextBox></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
	</body>
</HTML>
