<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="PersonLoadRackPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonLoadRackPage" %>
<html>
<head>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<script>
    function CompanySelect(role, companyTextBoxId) {
        var companyTextBox = document.getElementById(companyTextBoxId);

        showModalDialogFrame({
            url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&Unassigned=true",
            width: 855,
            height: 560,
            title: "Company Select",
            onClose: function () {
                if (this.returnValue != null) {
                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

                    companyTextBox.value = asciiValue1;
                    companyTextBox.title = asciiValue2;
                }
            }
        });
    }
</script>
<body>
    <table id="TableHeader" style="z-index: 102; left: 0px; width: 700px; position: absolute; top: 10px; height: 10px">
        <tr>
            <td>
                <FMControls:FMLabel ID="CardLabel" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle" Width="24px">Card</FMControls:FMLabel>
                <FMControls:FMLabel ID="CardLongLabel" AssociatedControlID="CardNumberTextbox"
                    runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle" Width="24px">Long:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="CardNumberTextbox"
                    TabIndex="2" runat="server" CssClass="formfield" Width="160px" MaxLength="30"></asp:TextBox>
            </td>
            <td>
                <FMControls:FMLabel ID="CardShortLabel" runat="server" AssociatedControlID="ShortCardNumberTextbox"
                    BackColor="Transparent" CssClass="formfieldtitle">Short:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="ShortCardNumberTextbox"
                    TabIndex="3" runat="server" CssClass="formfield" Width="40px" MaxLength="5"></asp:TextBox>
            </td>
            <td>
                <FMControls:FMLabel ID="SignatureStationLabel"
                    runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">Signature Station:</FMControls:FMLabel>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="PINLabel" AssociatedControlID="PINNumberTextbox"
                    runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">PIN:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="PINNumberTextbox"
                    TabIndex="4" runat="server" CssClass="formfield" MaxLength="4" TextMode="Password" AutoCompleteType="None" Width="51px"></asp:TextBox>
            </td>

            <td colspan="2">
                <FMControls:FMCheckBox ID="PINRequiredCheckbox"
                    TabIndex="6" runat="server" CssClass="formfieldtitle" Text="PIN Required"
                    TextAlign="Left"></FMControls:FMCheckBox>
            </td> 
             <td>
                <FMControls:FMCheckBox ID="signatureOnFile"
                    TabIndex="-1" runat="server" CssClass="formfieldtitle" Text="Signature On File"
                    TextAlign="Left"></FMControls:FMCheckBox>
            </td>                    
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="ConfirmPINLabel" AssociatedControlID="ConfirmPINTextBox"
                    runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">Confirm PIN:</FMControls:FMLabel>
            </td>
            <td colspan="3">
                <asp:TextBox ID="ConfirmPINTextBox"
                    TabIndex="5" runat="server" CssClass="formfield" MaxLength="4" TextMode="Password" AutoCompleteType="None" Width="51px"></asp:TextBox>
            </td>  
            <td>
                <asp:DropDownList ID="signatureStationList"
                    TabIndex="13" runat="server" CssClass="formfield" Width="160px">
                </asp:DropDownList>
            </td>            
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="LastActivityLabel" AssociatedControlID="LastActivityTextbox"
                    runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">Last Activity:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="LastActivityTextbox"
                    TabIndex="7" runat="server" CssClass="formfield" Width="160px" MaxLength="29"
                    Enabled="False"></asp:TextBox>
            </td>
            <td colspan="2">
                <FMControls:FMCheckBox ID="CardedInCheckBox"
                    TabIndex="8" runat="server" CssClass="formfieldtitle"
                    Enabled="False" Text="Carded In"
                    TextAlign="Left"></FMControls:FMCheckBox>
            </td>
             <td colspan="2">
                <FMControls:FMButton ID="clearSignature"
                    TabIndex="14" runat="server" CssClass="formfieldtitle" Width="128px"
                    Text="Clear Signature"></FMControls:FMButton>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="LockedOutDateLabel" AssociatedControlID="LockedOutDateTextbox"
                    runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle" Height="24px">Locked Out Date:</FMControls:FMLabel>
            </td>
            <td>
                <asp:TextBox ID="LockedOutDateTextbox"
                    TabIndex="9" runat="server" CssClass="formfield" Width="67px" MaxLength="20" Enabled="False"
                    ReadOnly="True"></asp:TextBox>
            </td>
            <td colspan="2">
                <FMControls:FMCheckBox ID="LockedOutCheckBox"
                    TabIndex="10" runat="server" CssClass="formfieldtitle"
                    Text="Locked Out" TextAlign="Left"
                    AutoPostBack="True" OnCheckedChanged="LockedOutCheckBoxCheckedChanged"></FMControls:FMCheckBox>
            </td>
             <td>
                <FMControls:FMButton ID="captureSignature"
                    TabIndex="15" runat="server" CssClass="formfieldtitle" Width="128px"
                    Text="Capture Signature"></FMControls:FMButton>
            </td>    
        </tr>
        <tr>
            <td></td>
            <td></td>
            <td>
                <FMCONTROLS:FMCHECKBOX id="InhibitInactivityLockOutCheckBox"
			    tabIndex="11" runat="server" CssClass="formfieldtitle" 
                Text="Inhibit Inactivity Lockout" TextAlign="Left"
			    oncheckedchanged="LockedOutCheckBox_CheckedChanged"></FMCONTROLS:FMCHECKBOX>
            </td>            
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="LockOutReasonlabel" AssociatedControlID="LockedOutReasonTextbox"
                    runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">Lock Out Reason:</FMControls:FMLabel>
            </td>
            <td colspan="3">

                <asp:TextBox ID="LockedOutReasonTextbox"
                    TabIndex="12" runat="server" CssClass="formfield" MaxLength="80"
                    Style="height: 56px; width: 308px;"
                    TextMode="MultiLine"></asp:TextBox>
            </td>
            <td>
    <!--    This will look very strange but it is exotic syntax from Microsoft for include files.  
	            I know it looks like the include file is commented out, but that is the proper syntax! 
	            The include is to add the VarecEnrollment OCX object when the product is not in DESC mode
	            so that TWIC enrollment will work on this page.-->
    <!--    JS20100924 WI-18064 Added ADF key check becaused of unsupported OCX on SOE125 
	            JS20100927 WI-18064 Also removed Desc key check because it was no longer relevant following
	            hardware key changes (i.e. HardwareKey was removed from Primus as a variable thus could not
	            be used here).
	            sjiang: All defense project do not support TWIC enrollment
	    -->
    <%  
        bool isDefense = FMBusinessObjects.ChannelFactories.FMChannelHelper.MakeCall<FMBusinessObjects.BusinessInterfaces.IHardwareKey, bool>(x => x.IsDefenseKey());

        if (isDefense == false && this.IsLicenseChildVersionEnabled())
        { %>
    <!-- #Include virtual="VarecEnrollment.aspx"-->
    <% } %>
                <!-- Used for TWIC Enrollment functionality -->
                <asp:TextBox ID="DummyTextBox1"
                    CssClass="formfield" runat="server" Visible="True"
                    BackColor="White" ForeColor="White"
                    BorderStyle="None" BorderColor="White"></asp:TextBox>
                <asp:TextBox ID="DummyTextBox2"
                    CssClass="formfield" runat="server" BackColor="White" ForeColor="White"
                    BorderStyle="None" BorderColor="White"></asp:TextBox>
            </td>             
        </tr>
        <tr>
            <td colspan="5">
                <TABLE id="Table1" style="Z-INDEX: 100; LEFT: 0px; WIDTH: 43.18%; POSITION: absolute; HEIGHT: 10px"
				    cellSpacing="0" cellPadding="1" border="0">
				    <TR>
					    <TD style="WIDTH: 498px; HEIGHT: 10px" width="498"><FMControls:FMDataGrid id="AssignedCompaniesDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False"
							    GridLines="Vertical" Width="560px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
							    style="LEFT: 1px; TOP: 0px" PageSize="8" tabIndex="1">
							    <FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
							    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
							    <Columns>
								    <asp:TemplateColumn HeaderText="Edit">
									    <HeaderStyle Width="0.5in"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									    <ItemTemplate>
										    <FMControls:FMEditLinkButton ID="FMEditLinkButton1" runat="server" Enabled='<%# Eval("EditingEnabled").ToString().Equals("True") %>' />
									    </ItemTemplate>
									    <EditItemTemplate>
			                            <FMControls:FMUpdateLinkButton ID="FMUpdateLinkButton1" runat="server" />&nbsp;
                                        <FMControls:FMCancelLinkButton ID="FMCancelLinkButton1" runat="server" />
                               </EditItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn Visible="False" HeaderText="Index">
									    <HeaderStyle Width="2in"></HeaderStyle>
									    <ItemTemplate>
										    <asp:Label id="IndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' width="2in">
										    </asp:Label>
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn HeaderText="ID">
									    <HeaderStyle Width="2in"></HeaderStyle>
									    <ItemTemplate>
										    <asp:Label id="label111" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in">
										    </asp:Label>
									    </ItemTemplate>
								    <EditItemTemplate>
									    <asp:dropdownlist CssClass=tabletext runat="server" Enabled="True" ID="CompaniesDropDownList" DataSource="<%# EnumerateCarrierCompanies()%>" DataTextField="Text" DataValueField="Value">
									    </asp:dropdownlist>
								    </EditItemTemplate>
								    </asp:TemplateColumn>
								    <asp:TemplateColumn HeaderText="Delete">
									    <HeaderStyle Width="0.5in"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									    <ItemTemplate>
										    <FMControls:FMDeleteLinkButton ID="FMDeleteLinkButton1" runat="server" Enabled='<%# Eval("EditingEnabled").ToString().Equals("True") %>' />
									    </ItemTemplate>
								    </asp:TemplateColumn>
							    </Columns>
							    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
						    </FMControls:FMDataGrid></TD>
				    </TR>
				    <TR>
					    <TD style="WIDTH: 498px; HEIGHT: 50px" vAlign="middle" width="498"><FMControls:FMButton id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
							    tabIndex="28"></FMControls:FMButton></TD>
				    </TR>
			    </TABLE>
            </td>
        </tr> 
    </table>
    <script type="text/javascript">
        var oTWICEnrollButton = document.getElementById("TWICEnrollButton");
        if (oTWICEnrollButton != null) {
            if (oTWICEnrollButton.addEventListener) {
                oTWICEnrollButton.addEventListener("click", GetTWICEnrollment, false);
            }
            else if (oTWICEnrollButton.attachEvent) {
                oTWICEnrollButton.attachEvent("onclick", GetTWICEnrollment);
            }
        }

        var oDummyTextBox1 = document.getElementById("tcPersonTabs_tpLoadRackPage_PersonLoadRackPage_DummyTextBox1");
        if (oDummyTextBox1 != null)
            oDummyTextBox1.readOnly = true;
        var oDummyTextBox2 = document.getElementById("tcPersonTabs_tpLoadRackPage_PersonLoadRackPage_DummyTextBox2");
        if (oDummyTextBox2 != null)
            oDummyTextBox2.readOnly = true;

        function GetTWICEnrollment() {
            var oDummyTextBox1 = document.getElementById("tcPersonTabs_tpLoadRackPage_PersonLoadRackPage_DummyTextBox1");
            var oDummyTextBox2 = document.getElementById("tcPersonTabs_tpLoadRackPage_PersonLoadRackPage_DummyTextBox2");
            if ((oDummyTextBox1 != null) && (oDummyTextBox2 != null)) {
                var result = document.varecenrollment.GetTWICData();
                if (1 == result) {
                    oDummyTextBox1.readOnly = false;
                    oDummyTextBox2.readOnly = false;
                    oDummyTextBox1.value = document.varecenrollment.TWIC_PersonIdentifier;
                    oDummyTextBox2.value = document.varecenrollment.TWIC_ExpirationDate;
                    __doPostBack('GetTWICEnrollment', '');
                }
                else {
                    oDummyTextBox1.value = "";
                    oDummyTextBox2.value = "";
                    alert(document.varecenrollment.TWIC_ErrorMessage);
                }
            }
        }

	 </script>
</body>
</html>
