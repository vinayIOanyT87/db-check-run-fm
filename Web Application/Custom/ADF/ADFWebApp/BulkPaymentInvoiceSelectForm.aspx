<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="BulkPaymentInvoiceSelectForm.aspx.cs" AutoEventWireup="True" Inherits="ADFWebApp.BulkPaymentInvoiceSelectForm" ValidateRequest=false %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>FuelsManager - Select Bulk Payment Invoices</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../FuelsManager.css" rel="stylesheet">
	    <style type="text/css">
            #Text1
            {
                width: 261px;
            }
            #btn
            {
                width: 364px;
            }
            #txtSearchText
            {
                width: 294px;
            }
            #txtInvoiceNumber
            {
                width: 264px;
            }
            #txtInvoiceNumber0
            {
                width: 263px;
            }
            #txtAccountCode
            {
                width: 265px;
            }
            #txtAccountCode0
            {
                width: 265px;
            }
            #txtCostCentreCode
            {
                width: 263px;
            }
            #txtCostCentreCode0
            {
                width: 263px;
            }
            #txtSupplierInvoiceNumber
            {
                width: 262px;
            }
        </style>
        <script>
			// The following is needed to fix a multiple postback problem with
			// IE modal dialog windows.  In addition you have to set the target
			// property of the form tag to the window name
			window.name = "selectInvoiceAssociated";
			
			function ProductSelect(productTextBoxID)
			{
				var sFeatures			  = "dialogWidth: 855px; dialogHeight: 560px";
				var productTextBox        = document.getElementById(productTextBoxID);
				var result                = null;
				var companyID             = "";

				result = window.showModalDialog("../FMWebApp/ProductSelectForm.aspx?Type=MAX_PRODUCT" + 
				                                "&Map=MAX_MAP" + "&IDLink=" + encodeURIComponent(companyID), "", sFeatures);
				                                    
				if (result != null)
				{
					productTextBox.value = result[0];
					productTextBox.title = result[1];
	            }
			}
			
			function CompanySelect(role, CompanyTextBoxID)
			{
				var sFeatures		 = "dialogWidth: 855px; dialogHeight: 560px";
				var CompanyTextBox = document.getElementById(CompanyTextBoxID);
				var CompanyNameTextBox = document.getElementById("CompanyName" + CompanyTextBoxID);
				var result         = null;
				var ManagerString = null;
				var OwnerString = null;
				var ShipperString = null;
				var BillToString = null;
				var limitSelectionsBasedOnHierarchy = "false";
				
				// JS20100109 Removed supplier role because not needed in invoice selection
				
				result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + role, 
																	"", sFeatures);
				if (result != null)
				{				    
					CompanyTextBox.value = result[0];
					CompanyTextBox.title = result[1];
					CompanyNameTextBox.value = result[2];
				}
			}
        </script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="InvoiceSelectionForm" method="post" runat="server" target="selectInvoiceAssociated">
			<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
            <FMCONTROLS:FMLABEL id="lblHeading" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" CssClass="headline" Width="272px">Invoice 
                Selection</FMCONTROLS:FMLABEL>
			stOrigiO<TABLE id="Table1" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 737px; POSITION: absolute; TOP: 48px; HEIGHT: 74px"
				cellSpacing="1" cellPadding="1" width="737" border="0">
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblInvoiceNo" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">Invoice Number:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <input id="txtInvoiceNumber" type="text" runat="server"  /></td>
				</tr>
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblInvoiceNo1" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">Account Code:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <input id="txtAccountCode" type="text" runat="server"  /></td>
				</tr>
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblInvoiceNo2" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">Entered By:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <FMControls:FMDropDownList ID="ddlEnteredBy" runat="server">
                        </FMControls:FMDropDownList>
                    </td>
				</tr>
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblInvoiceNo3" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">Start Date:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <FMControls:FMDate ID="startDateCtrl" runat="server" />
                    </td>
				</tr>
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblInvoiceNo4" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">End Date:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <FMControls:FMDate ID="endDateCtrl" runat="server" />
                    </td>
				</tr>
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblInvoiceNo5" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">Cost Centre Code:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <input id="txtCostCentreCode" type="text" runat="server"  /></td>
				</tr>
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblInvoiceNo6" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">Supplier:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <FMControls:FMCompanyTextBox ID="txtSupplier" runat="server"></FMControls:FMCompanyTextBox>
                    </td>
				</tr>
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblSupplierInvoiceNumber" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">Suppliers Invoice Number:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <input id="txtSupplierInvoiceNumber" type="text" runat="server"  /></td>
				</tr>
				<tr>
					<td noWrap>
                        <FMCONTROLS:FMLABEL id="lblInvoiceNo7" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							Width="121px">Section:</FMCONTROLS:FMLABEL></td>
					<td noWrap>
                        <FMControls:FMDropDownList ID="ddlSection" runat="server">
                        </FMControls:FMDropDownList>
                    </td>
				</tr>
				<tr>
					<td noWrap>&nbsp;</td>
					<td noWrap style="text-align: right">
                        <FMCONTROLS:FMBUTTON id="btnRefresh" tabIndex="1" runat="server" 
                            CssClass="formfieldtitle" width="70px"
							Text="Refresh" onclick="btnRefresh_Click"></FMCONTROLS:FMBUTTON>&nbsp;
                        <FMCONTROLS:FMBUTTON id="btnShowAll" tabIndex="1" runat="server" 
                            CssClass="formfieldtitle" width="70px"
							Text="Show All" onclick="btnShowAll_Click"></FMCONTROLS:FMBUTTON>&nbsp;
                        <fmcontrols:fmbutton class="formfieldtitle" 
                                style="WIDTH: 67px" Runat="server" onclick="OK_Clicked" 
								Text="OK" CssClass="formfieldtitle" ID="btnOK" Width="70px"></fmcontrols:fmbutton>&nbsp; 
                            <fmcontrols:fmbutton class="formfieldtitle" style="WIDTH: 67px" 
                            Runat="server" onclick="Cancel_Clicked" 
								Text="Cancel" CssClass="formfieldtitle" ID="btnCancel" Width="70px"></fmcontrols:fmbutton></td>
				</tr>
				<tr>
					<td><br>
					</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td><FMCONTROLS:FMPAGESIZEDROPDOWN id="ddlPageSize" tabIndex="7" runat="server" 
                            Width="96px" onselectedindexchanged="ddlPageSize_SelectedIndexChanged" ></FMCONTROLS:FMPAGESIZEDROPDOWN></td>
				</tr>
				<tr>
					<td colSpan="2">
                        <FMCONTROLS:FMDATAGRID id="InvoiceDataGrid" tabIndex="5" 
                            runat="server" BackColor="White" CssClass="tabletext"
							Width="8.5in" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" 
                            BorderWidth="1px" AllowSorting="True"
							BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="20">
							<FooterStyle ForeColor="Black" BackColor="#333399"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#333399"></HeaderStyle>
							<Columns>
							<asp:TemplateColumn>
							    <ItemTemplate>
							        <FMControls:FMCheckBox ID="cbSelect" runat="server" />
							        <input type="hidden" id="hiddenSelect" runat="server" value="" />
							    </ItemTemplate> 
							</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Invoice ID">
									<ItemTemplate>
										<asp:Label id="lblGridInvoiceNumber" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.InvoiceNumber") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Invoice #">
									<ItemTemplate>
										<asp:Label id="lblGridSupplierInvoiceNumber" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.SupplierInvoiceNumber") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Order #">
									<ItemTemplate>
										<asp:Label ID="lblGridOrderNumber" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.OrderNumber") %>'> </asp:Label>
									</ItemTemplate>
							    </asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Quantity">
									<ItemTemplate>
										<asp:Label ID="lblGridQuantity" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.Quantity") %>'> </asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Supplier">
									<ItemTemplate>
										<asp:Label ID="lblGridSupplier" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.Supplier") %>'> </asp:Label>
									</ItemTemplate>
							    </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Total Amount">
									<ItemTemplate>
										<asp:Label ID="lblGridTotalAmount" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.TotalAmount") %>'> </asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Section">
									<ItemTemplate>
										<asp:Label ID="lblGridSection" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.Section") %>'> </asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Entered By">
									<ItemTemplate>
										<asp:Label ID="lblGridEnteredBy" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.EnteredBy") %>'> </asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Action Required">
									<ItemTemplate>
										<asp:Label ID="lblGridActionRequired" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.ActionRequired") %>'> </asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="TransID" Visible="False">
									<ItemTemplate>
										<asp:Label ID="lblGridTransID" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.TransID") %>'> </asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#333399" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></td>
				</tr>
				<tr>
					<td>&nbsp;</td>
				</tr>
			</TABLE>
		</form>
	    <p>
&nbsp;&nbsp;&nbsp;
        </p>
	</body>
</HTML>
