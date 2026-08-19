<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DemandSelectForm.aspx.cs" AutoEventWireup="true" Inherits="ADFWebApp.DemandSelectForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FM" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Weighted Average Cost Summary</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../FuelsManager.css" rel="stylesheet">
		<script type="text/javascript">
		function CompanySelect(Role, CompanyTextBoxID)
		{
			var sFeatures="dialogWidth: 855px; dialogHeight: 560px";
			var CompanyTextBox = document.getElementById(CompanyTextBoxID);
			var Result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + Role + "", "", sFeatures);

			if(Result != null)
			{
				CompanyTextBox.value = Result[0];
				CompanyTextBox.title = Result[1];
			}
		}
		
		function SetSelected(selected)
		{
			var transGrid = document.getElementById("demandGrid");
			if (transGrid != null)
			{
				for (index=1; index < transGrid.rows.length; index++)
				{
					transGrid.rows(index).cells(0).all[0].checked = selected;
				}
			}
		}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:image id="FadeImage" 
                style="Z-INDEX: -100; LEFT: 0px; POSITION: absolute; TOP: -1px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<br />
			<table id="tblGrid" runat="server" style="z-index:100" cellpadding="2" 
                cellspacing="3">
				<tr>
					<td><FM:FMLABEL id="lblDateFilter" runat="server" CssClass="formfieldtitle">Date 
                        Filter:</FM:FMLABEL></td>
					<td>
                        <FM:FMDropDownList ID="ddlDateFilter" runat="server">
                            <asp:ListItem>Inventory Date</asp:ListItem>
                            <asp:ListItem Selected="True">Transaction Date</asp:ListItem>
                        </FM:FMDropDownList>
                    </td>
					<td><FM:FMLABEL id="lblDocRef" runat="server" CssClass="formfieldtitle">Doc Ref 
                        Number:</FM:FMLABEL></td>
					<td>
                        <asp:TextBox ID="tbDocRef" runat="server"></asp:TextBox>
                    </td>
					<td><FM:FMButton id="btnRefresh" runat="server" CssClass="formfieldtitle" 
                            Text="Refresh" onclick="btnRefresh_Click" Width="64px" />&nbsp;
                        <FM:FMButton id="btnOK" runat="server" CssClass="formfieldtitle" 
                            Text="OK" onclick="btnOK_Click" Width="64px" />&nbsp;
                        <FM:FMButton id="btnCancel" runat="server" CssClass="formfieldtitle" 
                            Text="Cancel" onclick="btnCancel_Click" Width="64px" /></td>
				</tr>			        
				<tr>
					<td><FM:FMLABEL id="labStartDate" runat="server" CssClass="formfieldtitle">Start Date:</FM:FMLABEL></td>
					<td><FM:FMDATE id="startDateControl" runat="server" CssClass="formfield" Width="136px"></FM:FMDATE></td>
					<td><FM:FMLABEL id="labEndDate" runat="server" CssClass="formfieldtitle">End Date:</FM:FMLABEL></td>
					<td><FM:FMDATE id="endDateControl" runat="server" CssClass="formfield" Width="136px"></FM:FMDATE></td>
					<td>&nbsp;</td>
				</tr>			        
				<tr>
					<td><FM:FMLabel ID="lblPONumber" runat="server" CssClass="formfieldtitle">PO Number:</FM:FMLabel></td>
					<td>&nbsp;</td>
					<td><FM:FMLabel ID="lblOwner" runat="server" CssClass="formfieldtitle">Owner:</FM:FMLabel></td>
					<td>
                        <FM:FMCompanyTextBox ID="tbOwner" runat="server"></FM:FMCompanyTextBox>
                    </td>
					<td>&nbsp;</td>
				</tr>				
				<tr>
					<td><FM:FMLabel ID="lblManager" runat="server" CssClass="formfieldtitle">Manager:</FM:FMLabel></td>
					<td>
                        <FM:FMCompanyTextBox ID="tbManager" runat="server"></FM:FMCompanyTextBox>
                    </td>
					<td><FM:FMLabel ID="lblSupplier" runat="server" CssClass="formfieldtitle">Supplier:</FM:FMLabel></td>
					<td>
                        <FM:FMCompanyTextBox ID="tbSupplier" runat="server"></FM:FMCompanyTextBox>
                    </td>
					<td>&nbsp;</td>
				</tr>				
				<tr>
					<td>
                        <br />
                    </td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
				</tr>				
                <tr>
                    <td colspan="2">
                        <input class="formfieldtitle" style="WIDTH: 75px" onclick="SetSelected(true)" type="button"
								value="Select All">&nbsp; <input class="formfieldtitle" style="WIDTH: 75px" onclick="SetSelected(false)" type="button"
								value="Clear All"><td colspan="3">&nbsp;</td>
                </tr>
				<tr>
				    <td colspan="5">
	                    <FM:FMDATAGRID id="demandGrid" tabIndex="5" 
                                runat="server" BackColor="White" CssClass="tabletext"
				                Width="736px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" 
                                BorderWidth="1px" AllowSorting="True"
				                BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="20">
			                <FooterStyle ForeColor="Black" BackColor="#333399"></FooterStyle>
			                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
			                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
			                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
			                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#333399"></HeaderStyle>
			                <Columns>
			                    <asp:TemplateColumn HeaderText="Selection">
			                        <ItemTemplate>
						                <FMControls:FMCheckBox ID="chkDemand" runat="server" />
					                </ItemTemplate>
			                    </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Transaction Type">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblTransactionType" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Date") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Transaction Date">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblTransactionDate" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Value") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Inventory Date">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblInventoryDate" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Source") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
			                    <asp:TemplateColumn Visible="False" HeaderText="Doc Ref Number">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblDocRef" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DocRefNumber") %>' />
                                    </ItemTemplate>
			                    </asp:TemplateColumn>
			                    <asp:TemplateColumn Visible="False" HeaderText="Demand Status">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblDemandStatus" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DemandStatus") %>' />
                                    </ItemTemplate>
			                    </asp:TemplateColumn>
			                    <asp:TemplateColumn HeaderText="Fuel Type">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblFuelType" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FuelType") %>' />
                                    </ItemTemplate>
			                    </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Delivery Location">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblDeliveryLocation" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DeliveryLocation") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Required Delivery Date">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblReqDeliveryDate" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ReqDeliveryDate") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Requested By">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblRequestedBy" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.RequestedBy") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
			                </Columns>
			                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#333399" Mode="NumericPages"></PagerStyle>
		                </FM:FMDATAGRID>
		            </td>
		        </tr>				
                <tr>
                    <td colspan="2">
                        <input class="formfieldtitle" style="WIDTH: 75px" onclick="SetSelected(true)" type="button"
								value="Select All">&nbsp; <input class="formfieldtitle" style="WIDTH: 75px" onclick="SetSelected(false)" type="button"
								value="Clear All"><td colspan="3">&nbsp;</td>
                </tr>
		    </table>
        </form>
				
	</body>
</HTML>
