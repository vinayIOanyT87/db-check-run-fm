<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="Accounting" TagName="TransactionFilterControl" Src="TransactionFilterControl.ascx" %>
<%@ Page language="c#" Codebehind="SelectAssociatedTxDialog.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.SelectAssociatedTxDialog" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<base target="_self" />
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
		<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
		<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken_min.js" %>" type="text/javascript"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
		<script>
		    // The following is needed to fix a multiple postback problem with
		    // IE modal dialog windows.  In addition you have to set the target
		    // property of the form tag to the window name
		    window.name = "selectAssociated";

		    // This function is called by the all company text box/button controls.
		    function CompanySelect(role, companyTextBoxId)
		    {
		        var companyTextBox  = document.getElementById(companyTextBoxId);
		        var managerString   = null;
		        var ownerString     = null;
		        var shipperString   = null;
		        var billToString    = null;
		        var limitSelectionsBasedOnHierarchy = "false";
		        var url = null;

		        if (role === "CARRIER")
		        {
		            var companyShipToTextBox = document.getElementById("TransactionFields.ShipToFG");
		            if (companyShipToTextBox != null)
		            {
		                var shipToId = companyShipToTextBox.value;

		                if (shipToId.substr(0, 1) === "<")
		                {
		                    shipToId = "";
		                }

		                url = "../FMWebApp/CompanySelectForm.aspx?Unassigned=true&Role=" + role + "&Map=AUTHORIZED_CARRIER_MAP" + "&IDLink=" + encodeURIComponent(shipToId);
		            }
		            else
		            {
		                url = "../FMWebApp/CompanySelectForm.aspx?Unassigned=true&Role=" + role;
		            }

		            showModalDialogFrame({
		                url: url,
		                width: 855,
		                height: 690,
		                title: "Company Select",
		                onClose: function ()
		                {
		                    if (this.returnValue != null)
		                    {
		                        if (companyTextBox.value !== this.returnValue[0])
		                        {
		                            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                            var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);
		                            var asciiValue3 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[2]);

		                            companyTextBox.value = asciiValue1;
		                            companyTextBox.title = asciiValue2;
		                            companyTextBox.value = asciiValue3;

		                            __doPostBack(companyTextBox.id, asciiValue1);
		                            return;
		                        }
		                        completeCompanySelection(this.returnValue, role, companyTextBoxId);
		                    }
		                }
		            });
		        }
		        else
		        {
		            limitSelectionsBasedOnHierarchy = document.getElementById("LimitSelectionsBasedOnHierarchy");

		            if (limitSelectionsBasedOnHierarchy != null && limitSelectionsBasedOnHierarchy.value == "true")
		            {
		                managerString   = document.getElementById("TransactionFields.ManagerFG");
		                ownerString     = document.getElementById("TransactionFields.OwnerFG");
		                shipperString   = document.getElementById("TransactionFields.ShipperFG");
		                billToString    = document.getElementById("TransactionFields.BilltoFG");

		                if ( role === "MANAGER" )
		                {
		                    url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role;
		                }
		                else if ( role === "OWNER" && managerString != null )
		                {
		                    url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value;
		                }
		                else if ( role === "SHIPPER" && managerString != null && ownerString != null )
		                {
		                    url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value + "&OwnerST=" + ownerString.value;
		                }
		                else if ( role === "CUSTOMER_BILLTO" && managerString != null && ownerString != null && shipperString != null )
		                {
		                    url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value + "&OwnerST=" + ownerString.value + "&ShipperST=" + shipperString.value;
		                }
		                else if ( role === "CUSTOMER_SHIPTO" && managerString != null && ownerString != null && shipperString != null && billToString != null )
		                {
		                    url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value + "&OwnerST=" + ownerString.value + "&ShipperST=" + shipperString.value + "&BillToST=" + billToString.value;
		                }

		                showModalDialogFrame({
		                    url: url,
		                    width: 855,
		                    height: 690,
		                    title: "Company Select",
		                    onClose: function ()
		                    {
		                        if (this.returnValue != null)
		                        {
		                            HandleCompanySelection(this.returnValue, role, companyTextBoxId);
		                        }
		                    }
		                });
		            }
		            else
		            {
		                showModalDialogFrame({
		                    url: "../FMWebApp/CompanySelectForm.aspx?Null=true&Role=" + role + role,
		                    width: 855,
		                    height: 690,
		                    title: "Company Select",
		                    onClose: function ()
		                    {
		                        if (this.returnValue != null)
		                        {
		                            completeCompanySelection(this.returnValue, role, companyTextBoxId);
		                        }
		                    }
		                });
		            }
		        }
		    }

		    function HandleCompanySelection(result, role, companyTextBoxId)
		    {
		        var companyTextBox  = document.getElementById(companyTextBoxId);
		        var managerString   = document.getElementById("TransactionFields.ManagerFG");
		        var ownerString     = document.getElementById("TransactionFields.OwnerFG");
		        var shipperString   = document.getElementById("TransactionFields.ShipperFG");
		        var billToString    = document.getElementById("TransactionFields.BillToFG");

		        var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(result[0]);
		        var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(result[1]);

		        if (role === "MANAGER" && result != null && (managerString !== asciiValue1))
		        {
		            companyTextBox.value = asciiValue1;
		            companyTextBox.title = asciiValue2;
		            __doPostBack(companyTextBox.id, asciiValue1);
		        }
		        else if (role === "OWNER" && result != null && (ownerString !== asciiValue1))
		        {
		            companyTextBox.value = asciiValue1;
		            companyTextBox.title = asciiValue2;
		            __doPostBack(companyTextBox.id, asciiValue1);
		        }
		        else if (role === "SHIPPER" && result != null && (shipperString !== asciiValue1))
		        {
		            companyTextBox.value = asciiValue1;
		            companyTextBox.title = asciiValue2;
		            __doPostBack(companyTextBox.id, asciiValue1);
		        }
		        else if (role === "CUSTOMER_BILLTO" && result != null && (billToString !== asciiValue1))
		        {
		            companyTextBox.value = asciiValue1;
		            companyTextBox.title = asciiValue2;
		            __doPostBack(companyTextBox.id, asciiValue1);
		        }
		    }

		    function completeCompanySelection(result, role, companyTextBoxId)
		    {
		        var companyTextBox = document.getElementById(companyTextBoxId);
		        var companyNameTextBox = document.getElementById("CompanyName" + companyTextBoxId);

		        if (result != null)
		        {
		            if (role === "CARRIER")
		            {
		                var carrierTextBox = document.getElementById("TransactionFields.CarrierFG");
		                var oldCarrierValue = carrierTextBox.value;
		            }

		            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(result[0]);
		            var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(result[1]);
		            var asciiValue3 = ReplaceNonBreakingSpaceHexWithSpace(result[2]);

		            companyTextBox.value = asciiValue1;
		            companyTextBox.title = asciiValue2;
		            companyNameTextBox.value = asciiValue3;

		            if (role === "CUSTOMER_SHIPTO")
		            {
		                //__mydoPostBack('SHIPTO_REFRESH', result[0]);
		            }
		        }
		    }

		    function ProductSelect(productTextBoxId)
		    {
		        var productTextBox = document.getElementById(productTextBoxId);
		        var companyShipToTextBox = document.getElementById("TransactionFields.ShipToFG");
		        var companyManagerTextBox = document.getElementById("TransactionFields.ManagerFG");
		        var companyId = "";

		        if (companyShipToTextBox == null)
		        {
		            if (companyManagerTextBox != null)
		            {
		                companyId = companyManagerTextBox.value + "|manager";
		            }
		        }
		        else
		        {
		            companyId = companyShipToTextBox.value + "|shipto";
		        }

		        if (companyId.substr(0, 1) === "<")
		        {
		            companyId = "";
		        }

		        showModalDialogFrame({
		            url: "../FMWebApp/ProductSelectForm.aspx?Type=MAX_PRODUCT" + "&Map=MAX_MAP" + "&IDLink=" + encodeURIComponent(companyId),
		            width: 855,
		            height: 690,
		            title: "Product Select",
		            onClose: function ()
		            {
		                if (this.returnValue != null)
		                {
		                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                    productTextBox.value = asciiValue1;
		                    productTextBox.title = asciiValue2;
		                }
		            }
		        });
		    }

		    function SetSelected(selected)
		    {
		        var transGrid = document.getElementById("dgTransactions");
		        if (transGrid != null)
		        {
		            for (var index = 1; index < transGrid.rows.length; index++)
		            {
		                transGrid.rows(index).cells(0).childNodes[0].checked = selected;
		            }
		        }
		    }

		    function OkClicked()
		    {
		        var result = new Array("OK_Clicked");
		        setWindowReturnValue(result);
		        closeDialogWindow();
		    }

		    function CancelClicked()
		    {
		        var result = new Array();
		        setWindowReturnValue(result);
		        closeDialogWindow();
		    }
		</script>
	</HEAD>
	<body xmlns:FMControls="urn:http://schemas.varec.com/FMControls" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server" target="selectAssociated">
			<asp:image id="backImage" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" Runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<div style="Z-INDEX: 101; LEFT: 10px; WIDTH: 7.0in; POSITION: absolute; TOP: 10px">
				<table cellSpacing="1" cellPadding="1" width="100%" border="0">
					<tr>
						<td valign="top"><Accounting:TransactionFilterControl id="filterControl" runat="server"></Accounting:TransactionFilterControl></td>
						<td align="right" valign="top"><fmcontrols:fmbutton class="formfieldtitle" 
                                style="WIDTH: 67px" Runat="server" onclick="OK_Clicked" 
								Text="OK" CssClass="formfieldtitle"></fmcontrols:fmbutton>&nbsp; 
                            <fmcontrols:fmbutton class="formfieldtitle" style="WIDTH: 67px" Runat="server" onclick="Cancel_Clicked" 
								Text="Cancel" CssClass="formfieldtitle"></fmcontrols:fmbutton></td>
					</tr>
					<tr>
						<td colSpan="2">&nbsp;</td>
					</tr>
					<tr>
						<td colSpan="2"><input class="formfieldtitle" style="WIDTH: 75px" onclick="SetSelected(true)" type="button"
								value="Select All">&nbsp; <input class="formfieldtitle" style="WIDTH: 75px" onclick="SetSelected(false)" type="button"
								value="Clear All"></td>
					</tr>
					<tr>
						<td colSpan="2"><FMCONTROLS:FMDATAGRID id="dgTransactions" Runat="server" 
                                BackColor="White" CssClass="tabletext" BorderStyle="None"
								AutoGenerateColumns="True" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" 
                                BorderColor="White" Cellpadding="3"
								Width="8.5in">
								<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
								<SelectedItemStyle Font-Bold="true" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="true" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Selection">
										<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Font-Bold="False" 
                                            Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
                                            Font-Underline="False"></ItemStyle>
										<ItemTemplate>
											<asp:CheckBox ID="chkSelected" Runat="server"></asp:CheckBox>
											<input type="hidden" runat="server" id="hidTransID" name="hidTransID" value='' />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMDATAGRID></td>
					</tr>
					<tr>
						<td colSpan="2"><input class="formfieldtitle" style="WIDTH: 75px" onclick="SetSelected(true)" type="button"
								value="Select All">&nbsp; <input class="formfieldtitle" style="WIDTH: 75px" onclick="SetSelected(false)" type="button"
								value="Clear All"></td>
					</tr>
				</table>
			</div>
		</form>
	</body>
</HTML>
