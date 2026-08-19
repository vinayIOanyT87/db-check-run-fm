<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeterReconciliationSummary.aspx.cs" Inherits="FuelsManager.Accounting.MeterReconciliationSummary" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
  
<html>
<head id="Head1" runat="server">
	<title></title>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
</head>
<body>
	<form id="form1" runat="server">
		<script type="text/jscript">
		    function MeterSelect(meterTextBoxId) {
		        var meterTextBox = document.getElementById(meterTextBoxId);

		        showModalDialogFrame({
		            url: "../FMWebApp/MeterSelectForm.aspx?MeterTextBoxID=" + meterTextBoxId + "&All=true&FilterOnAsset=true",
		            width: 855,
		            height: 560,
		            onClose: function () {
		                if (this.returnValue != null && this.returnValue.length > 1) {
		                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                    meterTextBox.value = asciiValue1;
		                    meterTextBox.title = asciiValue2;
		                }
		            }
		        });
		    }

		    function MeterAssetSelect(meterAssetTextBoxId) {
		        var meterAssetTextBox = document.getElementById(meterAssetTextBoxId);

		        showModalDialogFrame({
		            url: "../FMWebApp/MeterAssetSelectForm.aspx?MeterAssetTextBoxID=" + meterAssetTextBoxId + "&All=true",
		            width: 855,
		            height: 560,
		            onClose: function () {
		                if (this.returnValue != null && this.returnValue.length > 1) {
		                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                    meterAssetTextBox.value = asciiValue1;
		                    meterAssetTextBox.title = asciiValue2;
		                }
		            }
		        });
		    }

		    function CompanySelect(role, companyTextBoxId) {
		        var companyTextBox = document.getElementById(companyTextBoxId);

		        showModalDialogFrame({
		            url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&All=true",
		            width: 855,
		            height: 560,
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

		    function ProductSelect(productTextBoxId) {
		        var productTextBox = document.getElementById(productTextBoxId);

		        showModalDialogFrame({
		            url: "../FMWebApp/ProductSelectForm.aspx?Type=MaxProduct&Map=MAX_MAP&All=true",
		            width: 855,
		            height: 560,
		            onClose: function () {
		                if (this.returnValue != null) {
		                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                    productTextBox.value = asciiValue1;
		                    productTextBox.title = asciiValue2;
		                }
		            }
		        });
		    }
		</script>
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position:absolute">
		<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<table style="z-index:110; left:15px; top: 10px; width:1000px; position:absolute" cellpadding="5" role="presentation" aria-label="layout">
				<tr>
					<td colspan="4">
						<FMControls:FMLabel id="TitleLabel" runat="server" CssClass="headline" Text="Meter Reconciliation" style="left:0px; position:relative" />
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMLabel id="InventoryDateLabel" runat="server" CssClass="formfieldtitle" Text="Inventory Date:" style="left:0px; position:relative" />
					</td>
					<td colspan="3">
				
						<FMControls:FMDate id="InventoryDate"  FormatInfo="<%# _dateFormat %>" TabIndex="1" Width="130px" 
							CssClass="formfield" runat="server" MaxLength="20"></FMControls:FMDate>
					</td>
					<td ></td>
					<td align="right" >
						<FMControls:FMButton id="RefreshButton" runat="server" CssClass="formfieldtitle" Text="Refresh" onclick="RefreshButton_Click" Width="100px" TabIndex="2" />
					</td>
					<td></td>
				</tr>
				<tr>

					<td>
						<FMControls:FMLabel id="AssetLabel" runat="server" CssClass="formfieldtitle" Text="Asset ID:" style="left:0px; position:relative" />
					</td>
					<td>
						<FMControls:FMMeterAssetTextBox id="AssetTextBox" runat="server" CssClass="formfield" Width="150px" TabIndex="3"/>
					</td>
					<td>
						<FMControls:FMLabel id="MeterIDLabel" runat="server" CssClass="formfieldtitle" Text="Meter ID:" Width="55px" style="left:0px; position:relative" />
					</td>
					<td>
						<FMControls:FMMeterTextBox id="MeterIDTextBox" runat="server" CssClass="formfield" Width="150px" TabIndex="4"/>
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMLabel id="ManagerLabel" runat="server" CssClass="formfieldtitle" Text="Manager:" style="left:0px; position:relative" />
					</td>
					<td>
						<FMControls:FMCompanyTextBox id="ManagerTextBox" runat="server" CssClass="formfield" Width="150px" TabIndex="5" Role="MANAGER"/>
					</td>
					<td align="left">
						<FMControls:FMLabel id="ProductLabel" runat="server" CssClass="formfieldtitle" Text="Product:" style="left:0px; position:relative" />
					</td>
					<td>
						<FMControls:FMProductTextBox id="ProductTextBox" runat="server" CssClass="formfield" Width="150px" TabIndex="6"/>
					</td>
					<td>
						<FMControls:FMLabel id="CarrierLabel" runat="server" CssClass="formfieldtitle" Text="Carrier:" style="left:0px; position:relative" />
					</td>
					<td>
						<FMControls:FMCompanyTextBox id="CarrierTextBox" runat="server" CssClass="formfield" Width="150px" TabIndex="7" Role="CARRIER"/>
					</td>
				</tr>
				<tr>
 
					<td>
						<FMControls:FMLabel id="ToleranceLabel" AssociatedControlID="ToleranceDropDownList" runat="server" CssClass="formfieldtitle" Text="In / Out of Tolerance:" style="left:0px; position:relative" />
					</td>
					<td>
						<FMControls:FMDropDownList id="ToleranceDropDownList" runat="server" CssClass="formfield" Width="150px" TabIndex="8">
							<asp:ListItem Value="0">All</asp:ListItem>
							<asp:ListItem Value="1">In</asp:ListItem>
							<asp:ListItem Value="2">Out</asp:ListItem>
						</FMControls:FMDropDownList>
					</td>
					<td>
						<FMControls:FMLabel id="ToleranceValueLabel" AssociatedControlID="ToleranceValueTextBox" runat="server" CssClass="formfieldtitle" Text="Tolerance Value:" style="left:0px; position:relative" />
					</td>
					<td align="left">
						<FMControls:FMTextBox id="ToleranceValueTextBox" runat="server" CssClass="formfield" Text="" style="left:0px; position:relative; Width:150px" TabIndex="9" MaxLength="4"/>
						<FMControls:FMLabel id="ToleranceIsPercentLabel" runat="server" CssClass="formfieldtitle" Text="%" style="top:-1px; left:5px; position:relative; Width:10px"  />
					</td>
				</tr>
				<tr>
					<td colspan="6">
						<FMControls:FMGridViewConfigurable ID="SummaryGrid" runat="server" FixedHeaders="true" Width="1000px" AllowPaging="false" ShowFooter="true" Height="550px" AllowSorting="true" 
						AutoGenerateColumns="false" OnRowCommand="SummaryGrid_RowCommand" OnRowDataBound="SummaryGrid_RowDataBound" OnSorting="SummaryGrid_Sorting" EnableViewState="true"
						 DataKeyNames="MeterGuid, AssetGuid, AssetID" ListViewStandardType="METER_RECONCILIATION_SUMMARY" ListViewType="STANDARD" FixedColumns="View Details" TabIndex="10"
							  RowHeaderColumn="Asset ID"
							aria-label="Summary">          
							<Columns>

								<asp:TemplateField HeaderText="View Details">
									<HeaderStyle Width="100px" />
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
									<ItemTemplate>
										<FMControls:FMViewLinkButton ID="ViewDetailsButton" runat="server" CommandName="ViewDetails"/>
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Asset ID" SortExpression="AssetID">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="AssetIDGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.AssetID") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>       
												
								<asp:TemplateField HeaderText="Meter ID" SortExpression="MeterID">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterIDGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.MeterID") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Rotates Backwards" SortExpression="RotatesBackwardsFlag">
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<FMControls:FMCheckBox ID="RotatesBackwardsCheckBox" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.RotatesBackwardsFlag") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Meter Start" SortExpression="MeterStart" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterStartGridColumn" runat="server"/>
									</ItemTemplate>                               
								</asp:TemplateField>     
												 
								<asp:TemplateField HeaderText="Meter Stop" SortExpression="MeterStop" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterStopGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>    
											 
								<asp:TemplateField HeaderText="Meter Difference" SortExpression="MeterTotal" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterTotalGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>    
																													   
								<asp:TemplateField HeaderText="Transaction Meter Total" SortExpression="TransactionMeterTotal" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="TransactionMeterTotalGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Meter Variance" SortExpression="MeterVariance" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterVarianceGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Transaction Volume Total" SortExpression="TransactionVolumeTotal" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="TransactionVolumeTotalGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Volume Variance" SortExpression="VolumeVariance" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="VolumeVarianceGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Product" SortExpression="Product">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="ProductIDGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.Product") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>    
								 
								<asp:TemplateField HeaderText="Carrier" SortExpression="Carrier">
									<HeaderStyle Width="100px" />
									<ItemTemplate >
										<asp:Label ID="CarrierGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.Carrier") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>     

								<asp:TemplateField HeaderText="Error" SortExpression="IsError">
									<HeaderStyle Width="80px" />
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
									<ItemTemplate >
										<asp:Image ID="ErrorImage" runat="server" ImageUrl="../FMWebApp/images/Annotate_Warning.ico" Visible="False" ToolTip="" Width="20px" Height="20px"/>
									</ItemTemplate>
								</asp:TemplateField>    
													 
							</Columns>
						</FMControls:FMGridViewConfigurable>
					</td>
				</tr>
				<tr>
					<td colspan="5" align="left">
						 <FMControls:FMButton id="ReportButton" runat="server" CssClass="formfieldtitle" Text="Generate Report" onclick="ReportButton_Click" Width="115px" TabIndex="11"/>
					</td>
				</tr>
			   
			</table>
			</div>
	</form>
</body>
</html>
