<%@ Control language="c#" Codebehind="CompanyCarrierPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanyCarrierPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	    <SCRIPT>
		    function EntitySelect(entityTextBoxId, mode)
		    {
			    var entityTextBox = document.getElementById(entityTextBoxId);
			    var typeDropDownList = document.getElementById("tcCompanyTabs_tpCarrierPage_CompanyCarrierPage_TypeDropDownList");

			    if (typeDropDownList.value === "0")
			    {
			        showModalDialogFrame({
			            url: "../FMWebApp/CompanySelectForm.aspx?Role=CUSTOMER_SHIPTO&Map=AUTHORIZED_CARRIER_MAP&Mode=" + mode,
			            width: 855,
			            height: 690,
			            title: "Company Select",
			            onClose: function ()
			            {
			                if (this.returnValue != null)
			                {
			                    var result = this.returnValue;
			                    if (result != null && result.length > 0)
			                    {
			                        for (var i = 0; i < result.length; i++)
			                        {
			                            var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

			                            if ( i === 0 )
			                            {
			                                entityTextBox.value = newAsciiStr;
			                            }
			                            else
			                            {
			                                entityTextBox.value += "|" + newAsciiStr;
			                            }
			                        }

			                        entityTextBox.onchange();
			                    }
			                }
			            }
			        });
			    }
			    else if (typeDropDownList.value === "1")
			    {
			        showModalDialogFrame({
			            url: "../FMWebApp/PersonSelectForm.aspx?Map=true&Mode=" + mode,
			            width: 855,
			            height: 690,
			            title: "Person Select",
			            onClose: function ()
			            {
			                if (this.returnValue != null)
			                {
			                    var result = this.returnValue;
			                    if (result != null && result.length > 0)
			                    {
			                        for (var i = 0; i < result.length; i++)
			                        {
			                            var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

			                            if ( i === 0 )
			                            {
			                                entityTextBox.value = newAsciiStr;
			                            }
			                            else
			                            {
			                                entityTextBox.value += "|" + newAsciiStr;
			                            }
			                        }

			                        entityTextBox.onchange();
			                    }
			                }
			            }
			        });
			    }
		    }
	    </SCRIPT>
	</HEAD>
	<body>
	    <p>
            &nbsp;</p>
	    <table style="width:586px; position: absolute; top:5px; left: 10px;">
            <tr>
                <td class="style2">
	                <FMCONTROLS:FMLABEL id="Fmlabel2" style="Z-INDEX: 125; LEFT: 0px;" runat="server"
		                CssClass="formfieldtitle" Width="64px" BackColor="Transparent" Height="16px">Type:</FMCONTROLS:FMLABEL>
	            </td>
                <td class="style1">
	                <FMCONTROLS:FMDROPDOWNLIST id="TypeDropDownList" ToolTip="Type" style="Z-INDEX: 111; LEFT: 104px; "
		                tabIndex="16" runat="server" CssClass="formfield" Width="240px" AutoPostBack="True" onselectedindexchanged="TypeDropDownListSelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST>
	            </td>
                <td class="style3">
	                <FMCONTROLS:FMLABEL id="Fmlabel1" AssociatedControlID="SCACCodeTextbox" style="Z-INDEX: 118; LEFT: 400px; " runat="server"
		                CssClass="formfieldtitle" Width="136px" BackColor="Transparent">SCAC Code:</FMCONTROLS:FMLABEL>
	            </td>
                <td class="style4">
	                <asp:textbox id="SCACCodeTextbox" style="Z-INDEX: 128; LEFT: 528px; "
		                tabIndex="21" runat="server" CssClass="formfield" Width="160px" MaxLength="4"></asp:textbox>
	            </td>
            </tr>
            <tr>
               <td class="style2">
	                <FMCONTROLS:FMLABEL id="Fmlabel3" style="Z-INDEX: 125; LEFT: 0px;" runat="server"
		                CssClass="formfieldtitle" Width="64px" BackColor="Transparent" Height="16px">Assigned:</FMCONTROLS:FMLABEL>
	            </td>
                <td rowspan="11" class="style1" valign="top">
	                <table id="Table1" style="Z-INDEX:100; LEFT: 0px; HEIGHT: 10px; width: 261px;"
		                cellSpacing="0" cellPadding="1" border="0">
		                <tr>
			                <TD width="240px" height="10" valign="top">
                                <FMCONTROLS:FMDATAGRID id="AssignedEntitiesDataGrid" tabIndex="5" 
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
				                </FMCONTROLS:FMDATAGRID></TD>
		                </tr>
		                <tr>
			                <td width="368" height="36" valign="top">
				                <table width="300">
					                <tr>
						                <td width="84" height="10"><input class="formfieldtitle" id="CompanyCarrierPage_AssignButton" style="WIDTH: 80px"
								                onclick="EntitySelect('tcCompanyTabs_tpCarrierPage_CompanyCarrierPage_AssignEntitiesTextBox','Assign')" type="button" value="Assign" runat="server"></td>
						                <td height="10"><input class="formfieldtitle" id="CompanyCarrierPage_UnassignButton" style="WIDTH: 80px"
								                onclick="EntitySelect('tcCompanyTabs_tpCarrierPage_CompanyCarrierPage_UnassignEntitiesTextBox','Unassign')" type="button" value="Unassign" runat="server"></td>
						                <td><asp:textbox id="AssignEntitiesTextBox" ToolTip="Assign Entities" runat="server" Width="82px" BackColor="Transparent" BorderColor="Transparent" 
								                BorderStyle="None" AutoPostBack="True" ForeColor="Transparent" ontextchanged="AssignEntitiesTextBoxTextChanged"></asp:textbox></td>
						                <td><asp:textbox id="UnassignEntitiesTextBox" ToolTip="Unassign Entities" runat="server" Width="17px" 
                                                BackColor="Transparent" BorderColor="Transparent"
								                BorderStyle="None" AutoPostBack="True" ForeColor="Transparent"
                                                ontextchanged="UnassignEntitiesTextBoxTextChanged"></asp:textbox></td>
					                </tr>
				                </table>
			                </td>
		                </tr>
	                </TABLE>
	            </td>
                <td class="style3">
	<FMCONTROLS:FMLABEL id="Label5" AssociatedControlID="LicenseNumberTextbox" style="Z-INDEX: 118; LEFT: 400px; " runat="server"
		CssClass="formfieldtitle" Width="136px" BackColor="Transparent">License Number:</FMCONTROLS:FMLABEL>
	            </td>
                <td class="style4">
	<asp:textbox id="LicenseNumberTextbox" style="Z-INDEX: 119; LEFT: 528px; "
		tabIndex="22" runat="server" CssClass="formfield" MaxLength="20" Width="160px"></asp:textbox>
	            </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMLABEL id="Label12" style="Z-INDEX: 120; LEFT: 400px; " runat="server"
		CssClass="formfieldtitle" Width="116px" BackColor="Transparent">License Expiration:</FMCONTROLS:FMLABEL>
	            </td>
                <td class="style4">
	<FMCONTROLS:FMDATE id="LicenseExpirationDate" style="Z-INDEX: 121; LEFT: 528px;"
		tabIndex="23" runat="server" CssClass="formfield" Width="160px" MaxLength="20"></FMCONTROLS:FMDATE>
	            </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMLABEL id="Label16" AssociatedControlID="InsuranceCompanyTextbox" style="Z-INDEX: 123; LEFT: 400px; " runat="server"
		CssClass="formfieldtitle" Width="116px" BackColor="Transparent">Insurance Company:</FMCONTROLS:FMLABEL>
	            </td>
                <td class="style4">
	<asp:textbox id="InsuranceCompanyTextbox" style="Z-INDEX: 124; LEFT: 528px;"
		tabIndex="24" runat="server" CssClass="formfield" Width="160px" MaxLength="20"></asp:textbox>
	            </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMLABEL id="Label6" AssociatedControlID="InsurancePolicyTextbox" style="Z-INDEX: 125; LEFT: 400px; " runat="server"
		CssClass="formfieldtitle" Width="116px" BackColor="Transparent">Insurance Policy:</FMCONTROLS:FMLABEL>
	            </td>
                <td class="style4">
	<asp:textbox id="InsurancePolicyTextbox" style="Z-INDEX: 126; LEFT: 528px;"
		tabIndex="25" runat="server" CssClass="formfield" Width="160px" MaxLength="20"></asp:textbox>
	            </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMLABEL id="Label7" AssociatedControlID="LiabilityAmountTextbox" style="Z-INDEX: 127; LEFT: 400px; " runat="server"
		CssClass="formfieldtitle" Width="116px" BackColor="Transparent">Liability Amount:</FMCONTROLS:FMLABEL>
	            </td>
                <td class="style4">
	<asp:textbox id="LiabilityAmountTextbox" style="Z-INDEX: 128; LEFT: 528px;"
		tabIndex="26" runat="server" CssClass="formfield" Width="160px" MaxLength="8"></asp:textbox>
	            </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMCHECKBOX id="HazardousMaterialExclusionCheckBox" style="Z-INDEX: 129; LEFT: 400px; "
		tabIndex="27" runat="server" CssClass="formfieldtitle" Text="Hazardous Material Exclusion" Width="196px"
		TextAlign="Left"></FMCONTROLS:FMCHECKBOX>
	            </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMLABEL id="Label8" style="Z-INDEX: 130; LEFT: 400px; " runat="server"
		CssClass="formfieldtitle" Width="132px" BackColor="Transparent">Insurance Expiration:</FMCONTROLS:FMLABEL>
	            </td>
                <td class="style4">
	<FMCONTROLS:FMDATE id="InsuranceExpirationDate" style="Z-INDEX: 131; LEFT: 528px;"
		tabIndex="28" runat="server" CssClass="formfield" Width="160px" MaxLength="20"></FMCONTROLS:FMDATE>
	            </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMCHECKBOX id="FlushPermittedCheckBox" style="Z-INDEX: 113; LEFT: 400px; "
		tabIndex="30" runat="server" CssClass="formfieldtitle" Text="Flush Permitted" Height="8px" Width="120px"></FMCONTROLS:FMCHECKBOX>
	            </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMCHECKBOX id="PumpOffPermittedCheckBox" style="Z-INDEX: 114; LEFT: 400px; "
		tabIndex="31" runat="server" CssClass="formfieldtitle" Text="Pump Off Permitted" Height="8px" Width="152px"></FMCONTROLS:FMCHECKBOX>
	            </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	<FMCONTROLS:FMCHECKBOX id="AllowDriverEntryCheckBox" style="Z-INDEX: 116; LEFT: 400px; "
		tabIndex="33" runat="server" CssClass="formfieldtitle" Text="Allow Driver Entry" Height="24px" Width="128px"></FMCONTROLS:FMCHECKBOX>
	            </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style3">
	 <FMCONTROLS:FMCHECKBOX id="DeliveryToTerminalPermittedCheckBox" style="Z-INDEX: 115; LEFT: 400px; "
		                tabIndex="32" runat="server" CssClass="formfieldtitle" Text="Delivery To Terminal Permitted" Height="4px"
		                Width="216px"></FMCONTROLS:FMCHECKBOX>
	            </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style1">
                    &nbsp;</td>
                <td class="style3">
	  <FMCONTROLS:FMCHECKBOX id="PINRequiredCheckBox" style="Z-INDEX: 133; LEFT: 400px; "
		                tabIndex="34" runat="server" CssClass="formfieldtitle" Text="PIN Required" Height="27px" Width="176px"></FMCONTROLS:FMCHECKBOX>
	            </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td class="style1">
                    &nbsp;</td>
                <td class="style3">
	 <FMCONTROLS:FMCHECKBOX id="ScullyRequiredCheckBox" style="Z-INDEX: 131; LEFT: 400px; "
		                tabIndex="35" runat="server" CssClass="formfieldtitle" Text="Scully Required" Height="4px"
		                Width="216px"></FMCONTROLS:FMCHECKBOX>
	            </td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
        </table>
	</body>
</HTML>
