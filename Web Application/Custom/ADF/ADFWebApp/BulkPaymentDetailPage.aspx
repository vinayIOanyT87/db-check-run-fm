<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BulkPaymentDetailPage.aspx.cs" Inherits="ADFWebApp.BulkPaymentDetailPage" %>

<%@ Register assembly="FMControls" namespace="FMControls" tagprefix="FMCONTROLS" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
    <head>
        <title>BulkPaymentDetailPage</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <link href="../FuelsManager.css" rel="stylesheet" />
        <script type="text/javascript" src="ADFCustomScripts.js"></script>
        <script type="text/javascript" src="ADFCustomScripts2.js"></script>
    </head>
    <body ms_positioning="GridLayout">
	    <form id="Form1" method="post" runat="server">
		    <asp:image id="FadeImage" 
                style="Z-INDEX: -100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
			    ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
		    <FMCONTROLS:FMLABEL id="lblHeading" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
			    runat="server" BackColor="Transparent" CssClass="headline" Width="272px">
                Bulk Payment Details</FMCONTROLS:FMLABEL>
		    <br />
		    <br />
		    <table id="tblHeader" runat="server" style="z-index:100" cellpadding="2" 
                cellspacing="3">
			    <tr>
				    <td>
                        <FMCONTROLS:FMLabel ID="lblBulkPaymentID" runat="server" 
                            CssClass="formfieldtitle">Bulk Payment ID</FMCONTROLS:FMLabel></td>
				    <td>
                        <asp:TextBox ID="tbBulkPaymentID" runat="server"></asp:TextBox>
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblLocation" runat="server" CssClass="formfieldtitle">
                        Location</FMCONTROLS:FMLabel></td>
				    <td class="style1">
                        <asp:TextBox ID="tbLocation" runat="server"></asp:TextBox>
                    </td>
			    </tr>			        
			    <tr>
				    <td>
                        <FMCONTROLS:FMLabel ID="lblRomanNumber" runat="server" 
                            CssClass="formfieldtitle">ROMAN Payment Number</FMCONTROLS:FMLabel></td>
				    <td>
                        <asp:TextBox ID="tbRomanNumber" runat="server"></asp:TextBox>
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblPaymentType" runat="server" 
                            CssClass="formfieldtitle">
                        Payment Type</FMCONTROLS:FMLabel></td>
				    <td class="style1">
                        <FMCONTROLS:FMDropDownList ID="ddlPaymentType" runat="server">
                        </FMCONTROLS:FMDropDownList>
                    </td>
			    </tr>				
			    <tr>
				    <td><FMCONTROLS:FMLabel ID="lblSection" runat="server" CssClass="formfieldtitle">
                        Section</FMCONTROLS:FMLabel></td>
				    <td>
                        <FMCONTROLS:FMDropDownList ID="ddlSection" runat="server">
                        </FMCONTROLS:FMDropDownList>
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblSupplier" runat="server" CssClass="formfieldtitle">
                        Supplier</FMCONTROLS:FMLabel></td>
				    <td class="style1">
                        <FMCONTROLS:FMCompanyTextBox ID="tbSupplier" runat="server"></FMCONTROLS:FMCompanyTextBox>
                    </td>
			    </tr>				
			    <tr>
				    <td><FMCONTROLS:FMLabel ID="lblLastEditedBy" runat="server" 
                            CssClass="formfieldtitle">
                        Last Edited By</FMCONTROLS:FMLabel></td>
				    <td>
                        <asp:TextBox ID="tbLastEdit" runat="server"></asp:TextBox>
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblForeignCurrency" runat="server" 
                            CssClass="formfieldtitle">Foreign Currency</FMCONTROLS:FMLabel></td>
				    <td class="style1">
                        <FMCONTROLS:FMDropDownList ID="ddlForeignCurrency" runat="server" 
                            AutoPostBack="True">
                        </FMCONTROLS:FMDropDownList>
                    </td>
			    </tr>				
			    <tr>
				    <td><FMCONTROLS:FMLabel ID="lblPaymentDueDate" runat="server" 
                            CssClass="formfieldtitle">
                        Payment Due Date</FMCONTROLS:FMLabel></td>
				    <td>
                        <FMCONTROLS:FMDateTime ID="dtPaymentDueDate" runat="server" />
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblForeignRate" runat="server" 
                            CssClass="formfieldtitle">Foreign Currency Rate</FMCONTROLS:FMLabel></td>
				    <td class="style1">
                        <asp:TextBox ID="tbForeignRate" runat="server"></asp:TextBox>
                    </td>
			    </tr>				
			    <tr>
				    <td>
                        <FMCONTROLS:FMLabel ID="lblTransactionDate" runat="server" 
                            CssClass="formfieldtitle">Transaction Date</FMCONTROLS:FMLabel></td>
                            <td>
                                <FMCONTROLS:FMDateTime ID="dtTransactionDate" runat="server" />
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblDiscountRate" runat="server" 
                            CssClass="formfieldtitle">
                        Discount Rate</FMCONTROLS:FMLabel></td>
				    <td class="style1">
                        <asp:TextBox ID="tbDiscountRate" runat="server"></asp:TextBox>
                    </td>
			    </tr>				
			    <tr>
				    <td><FMCONTROLS:FMLabel ID="lblExcise" runat="server" 
                            CssClass="formfieldtitle">
                        Excise</FMCONTROLS:FMLabel></td>
				    <td>
                        <asp:TextBox ID="tbExcise" runat="server"></asp:TextBox>
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblTotalForeign" runat="server" 
                            CssClass="formfieldtitle">
                        Total Foreign Price</FMCONTROLS:FMLabel></td>
				    <td class="style1">
				        <asp:TextBox ID="tbTotalForeign" runat="server"></asp:TextBox>
				    </td>
			    </tr>				
			    <tr>
				    <td><FMCONTROLS:FMLabel ID="lblOnCost" runat="server" 
                            CssClass="formfieldtitle">
                        On-Cost</FMCONTROLS:FMLabel></td>
				    <td>
                        <asp:TextBox ID="tbOnCost" runat="server"></asp:TextBox>
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblTotal" runat="server" CssClass="formfieldtitle">
                        Total AUD</FMCONTROLS:FMLabel></td>
				    <td class="style1">
				        <asp:TextBox ID="tbTotal" runat="server"></asp:TextBox>
				    </td>
			    </tr>				
			    <tr>
				    <td><FMCONTROLS:FMLabel ID="lblGST" runat="server" CssClass="formfieldtitle">
                        GST</FMCONTROLS:FMLabel></td>
				    <td>
				        <asp:TextBox ID="tbGST" runat="server"></asp:TextBox>
                    </td>
				    <td><FMCONTROLS:FMLabel ID="lblTotalPaid" runat="server" CssClass="formfieldtitle">
                        Total AUD Paid</FMCONTROLS:FMLabel></td>
				    <td class="style1">
				        <asp:TextBox ID="tbTotalPaid" runat="server"></asp:TextBox>
				    </td>
			    </tr>				
			    <tr>
				    <td>
                        <br />
                    </td>
				    <td>&nbsp;</td>
				    <td>&nbsp;</td>
				    <td class="style1">&nbsp;</td>
			    </tr>
            </table>
            <table id="tblGrid" runat="server" style="z-index:100" cellpadding="2" 
                cellspacing="3">				
                <tr>
                    <td>
                        <input class="formfieldtitle" id="btnAssociateTop" style="WIDTH: 130px" onclick="BulkPaymentInvoiceSelect('InvSelectionTextBox')"
					            type="button" value="Associate" />
				    </td>
                    <td><FMCONTROLS:FMPageSizeDropDown ID="ddlPageSize" runat="server" CssClass="formfield">
                        </FMCONTROLS:FMPageSizeDropDown></td>
                </tr>
			    <tr>
			        <td colspan="2">
                        <FMCONTROLS:FMDATAGRID id="resultGrid" tabIndex="5" 
                                runat="server" BackColor="White" CssClass="tabletext"
			                    Width="800px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" 
                                BorderWidth="1px" AllowSorting="True"
			                    BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="20" 
                            FixedHeaders="False" FixedHeight="">
		                    <FooterStyle ForeColor="Black" BackColor="#333399"></FooterStyle>
		                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
		                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
		                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
		                    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#333399"></HeaderStyle>
		                    <Columns>
						    <asp:TemplateColumn HeaderText="Edit">
						        <ItemTemplate>
						            <FMCONTROLS:FMEditLinkButton id="EditButton" runat="server" />
						        </ItemTemplate>
							    <EditItemTemplate>
								    <FMControls:FMUpdateLinkButton id="UpdateLinkButton" runat="server" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.InvoiceTransID") %>'></FMControls:FMUpdateLinkButton>
								    <FMControls:FMCancelLinkButton id="CancelLinkButton" runat="server"></FMControls:FMCancelLinkButton>
							    </EditItemTemplate>
						    </asp:TemplateColumn>
						    <asp:TemplateColumn HeaderText="Remove">
                                <ItemTemplate>
                                    <FMControls:fmdeletelinkbutton ID="DeleteButton" runat="server" CommandArgument='<%# DataBinder.Eval(Container, "DataItem.InvoiceTransID") %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <FMControls:FMLabel ID="DeleteButtonEdit" runat="server" Text="" />
                                </EditItemTemplate>
                                <HeaderStyle Width="0.5in" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <ItemTemplate>
                                    <asp:TextBox ID="tbInvoiceTransID" runat="server"
                                        Text='<%# DataBinder.Eval(Container, "DataItem.InvoiceTransID") %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Product">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" readonly="true"
                                        Text='<%# DataBinder.Eval(Container, "DataItem.Product") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.Product") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="FM Invoice Number">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.InvoiceNumber") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.InvoiceNumber") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Vol on Delivery Docket">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox3" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.Quantity") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.Quantity") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Rebate Flag">
                                <ItemTemplate>
                                    <FMCONTROLS:FMCheckBox ID="cbRebateItem" runat="server" enabled="false"
                                            checked='<%# DataBinder.Eval(Container, "DataItem.RebateChecked") %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <FMCONTROLS:FMCheckBox ID="cbRebateEdit" runat="server" 
                                            checked='<%# DataBinder.Eval(Container, "DataItem.RebateChecked") %>' />
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Rebate Number">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbLiRebateNumber" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.RebateNumber") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLiRebateNumber" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.RebateNumber") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Account Code">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbLiAccountCode" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.AccountCode") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLiAccountCode" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.AccountCode") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Cost Centre Code">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbLiCostCentreCode" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.CostCentreCode") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLiCostCentreCode" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.CostCentreCode") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Foreign Total">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbLiForeignTotal" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.ForeignTotal") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLiForeignTotal" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.ForeignTotal") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Excise Value">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbLiExcise" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.Excise") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLiExcise" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.Excise") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="GST Value">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbLiGST" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.GST") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLiGST" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.GST") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="On-Cost Value">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbLiOnCost" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.OnCost") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLiOnCost" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.OnCost") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Total Price">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbLiTotalPrice" runat="server" readonly="true" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.TotalPrice") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLiTotalPrice" runat="server" 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.TotalPrice") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
					    </Columns>
		                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#333399" Mode="NumericPages"></PagerStyle>
	                    </FMCONTROLS:FMDATAGRID>
	                </td>
	            </tr>
	            <tr>
	                <td>
	                    <input class="formfieldtitle" id="btnAssociateBottom" style="WIDTH: 130px" onclick="BulkPaymentInvoiceSelect('InvSelectionTextBox')"
					            type="button" value="Associate" />
                    </td>
	                <td><FMControls:FMButton ID="btnApply" runat="server" 
                            CssClass="formfieldtitle" Text="Apply" style="width: 65px" />
                    &nbsp;
                        <FMControls:FMButton ID="btnCancel" runat="server" 
                            CssClass="formfieldtitle" Text="Close" style="width: 65px" />
                    &nbsp;
                        <FMControls:FMButton ID="btnDelete" runat="server" 
                            CssClass="formfieldtitle" Text="Delete" style="width: 65px" />
                    &nbsp; <FMControls:FMButton ID="btnViewPrintable" runat="server" 
                            CssClass="formfieldtitle" Text="View Printable" style="width: 130px" 
                            Width="113px" />
                        <asp:textbox id="InvSelectionTextBox" runat="server" Width="130px" 
                            BackColor="White" BorderStyle="None"
				        BorderColor="White" ForeColor="White" AutoPostBack="True"></asp:textbox>
                    </td>		            
	            </tr>
	        </table>
        </form>
    			
    </body>
</html>
