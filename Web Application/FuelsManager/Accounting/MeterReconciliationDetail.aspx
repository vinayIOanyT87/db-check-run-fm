<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeterReconciliationDetail.aspx.cs" Inherits="FuelsManager.Accounting.MeterReconciliationDetail" %>
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
</head>
<body>
	<form id="form1" runat="server">
	<script type="text/jscript">
			function MeterAssetSelect(meterAssetTextBoxID) {
				var meterAssetTextBox = document.getElementById(meterAssetTextBoxID);

				showModalDialogFrame({
					url: "../FMWebApp/MeterAssetSelectForm.aspx?MeterAssetTextBoxID=" + meterAssetTextBoxID + "&All=false",
					width: 855,
					height: 690,
					title: "Select Meter Asset",
					onClose: function () {
						if (this.returnValue != null) {
							var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
							var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);
							meterAssetTextBox.value = asciiValue1;
							meterAssetTextBox.title = asciiValue2;
						}
					}
				});
			}

			function MeterSelect(meterTextBoxID) {
				var meterTextBox = document.getElementById(meterTextBoxID);

				showModalDialogFrame({
					url: "../FMWebApp/MeterSelectForm.aspx?MeterTextBoxID=" + meterTextBoxID + "&All=false&FilterOnAsset=true",
					width: 855,
					height: 690,
					title: "Select Meter",
					onClose: function () {
						
						if (this.returnValue != null) {
							var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
							var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);
							meterTextBox.value = asciiValue1;
							meterTextBox.title = asciiValue2;
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
						<FMControls:FMLabel id="TitleLabel" runat="server" CssClass="headline" Text="Meter Reconciliation Detail" style="left:0px; position:relative" />
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMLabel id="AssetLabel" AssociatedControlID="AssetTextBox" runat="server" CssClass="formfieldtitle" Text="Asset ID:" Width="55px" style="left:0px; position:relative" />
					</td>
					<td>
						<FMControls:FMMeterAssetTextBox id="AssetTextBox" runat="server" CssClass="formfield" Width="150px" TabIndex="1"/>
					</td>
					<td>
						<FMControls:FMLabel id="MeterIDLabel" AssociatedControlID="MeterIDTextBox" runat="server" CssClass="formfieldtitle" Text="Meter ID:" Width="55px" style="left:0px; position:relative" />
					</td>
					<td>
						<FMControls:FMMeterTextBox id="MeterIDTextBox" runat="server" 
							CssClass="formfield" Width="150px" TabIndex="2" />
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMLabel id="InventoryDateLabel" runat="server" CssClass="formfieldtitle" Text="Inventory Date:" Width="90px"  style="left:0px; position:relative" />
					</td>
					<td >		
						<FMControls:FMDate id="InventoryDate" ToolTip="Inventory Date"  FormatInfo="<%# _dateFormat %>" Width="130px" 
							CssClass="formfield" runat="server" MaxLength="20" TabIndex="3"></FMControls:FMDate>
					</td>
					<td >
						<FMControls:FMLabel id="SkipToleranceValueLabel" AssociatedControlID="SkipToleranceValueTextBox" runat="server" CssClass="formfieldtitle" Text="Meter Skip Tolerance:" style="left:0px; position:relative" />
					</td>
					<td align="left">
						<FMControls:FMTextBox id="SkipToleranceValueTextBox" runat="server" CssClass="formfield" Text="" style="left:0px; position:relative; Width:150px" TabIndex="4" MaxLength="4"/>             
					</td>
					<td align="center"> 
						<FMControls:FMButton id="RefreshButton" runat="server" CssClass="formfieldtitle" Text="Refresh" onclick="RefreshButton_Click" Width="60px" TabIndex="5"/>
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<FMControls:FMLabel id="MeterCloseoutGridLabel" runat="server" 
							CssClass="formfieldtitle" Text="Meter Closeout Transaction" Width="200px"  
							style="left:0px; position:relative" Font-Italic="True" />
					</td>
						
				</tr>
				<tr>
					<td colspan="7">
						<FMControls:FMGridViewConfigurable ID="SummaryGrid" runat="server" FixedHeaders="true" Width="1100px" AllowPaging="false"
							ShowFooter="false" Height="550px" OnRowCommand="SummaryGrid_RowCommand" OnRowDataBound="SummaryGrid_RowDataBound"
							DataKeyNames="CurrentCloseoutTransactionID"    RowHeaderColumn="Product"
							ListViewStandardType="METER_RECONCILIATION_SUMMARY" ListViewType="STANDARD" FixedColumns="Edit Closeout,TransactionGuidText,TransactionIDHidden" TabIndex="6" aria-label="Summary">
							<Columns>
							   
								 <asp:TemplateField HeaderText="Edit Closeout">
									<HeaderStyle Width="100px" />
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
									<ItemTemplate>
										<FMControls:FMEditLinkButton ID="ViewCloseoutButton" CommandName="ViewCloseout" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>   
						 
								 <asp:TemplateField HeaderText="Product">
									<HeaderStyle Width="200px" />
									<ItemTemplate>
										<asp:Label ID="ProductIDGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.Product") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>     
							   
								<asp:TemplateField HeaderText="Meter Start" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterStartGridColumn" runat="server"/>
									</ItemTemplate>
								</asp:TemplateField>
						
								<asp:TemplateField HeaderText="Meter Stop" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterStopGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
					
								<asp:TemplateField HeaderText="Meter Difference" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterTotalGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
																														  
								<asp:TemplateField HeaderText="Transaction Meter Total" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="TransactionMeterTotalGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Meter Variance" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="MeterVarianceGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Transaction Volume Total" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="TransactionVolumeTotalGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Volume Variance" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="VolumeVarianceGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								 <asp:TemplateField HeaderText="Rotates Backwards">
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<HeaderStyle Width="125px" />
									<ItemTemplate>
										<FMControls:FMCheckBox ID="RotatesBackwardsCheckBox" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.RotatesBackwardsFlag") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Carrier">
									<HeaderStyle Width="100px" />
									<ItemTemplate >
										<asp:Label ID="CarrierGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.Carrier") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>  
									 
								<asp:TemplateField HeaderText="Error">
									<HeaderStyle Width="200px" />
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
									<ItemTemplate >
										<asp:Image ID="ErrorImage" runat="server" ImageUrl="~/FMWebApp/images/Annotate_Warning.ico" Visible="False" ToolTip="" Width="20px" Height="20px"/>
									</ItemTemplate>
								</asp:TemplateField>   
								<asp:TemplateField HeaderText="TransactionGuidText" Visible="false">
									<ItemTemplate>
										<asp:Literal ID="TransactionGuidText" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TransactionGuid").ToString() %>' />
									</ItemTemplate>
								</asp:TemplateField>
<%--								<asp:TemplateField HeaderText="TransactionAliasHidden" Visible="false">
									<ItemTemplate>
										<asp:Literal ID="TransactionAliasGridHidden" Text='<%# DataBinder.Eval(Container, "DataItem.TransactionAlias") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>--%>
								<asp:TemplateField HeaderText="TransactionIDHidden" Visible="false">
									<HeaderStyle Width="200px" />
									<ItemTemplate>
										<asp:Literal ID="TransactionIDGridHidden" Text='<%# DataBinder.Eval(Container, "DataItem.CurrentCloseoutTransactionID") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
						</FMControls:FMGridViewConfigurable>
					</td>
				</tr>
				<tr>
					 <td>
						<FMControls:FMLabel id="TransactionSummaryGridLabel" runat="server" CssClass="formfieldtitle" Text="Meter Transactions" Width="200px"  style="left:0px; position:relative" Font-Italic="True" />
					</td>
				</tr>
				<tr>
					<td colspan="7">
						<FMControls:FMGridViewConfigurable ID="DetailGrid" runat="server" FixedHeaders="true" Width="1100px" AllowPaging="false" ShowFooter="true"
						Height="550px" AllowSorting="true" DataKeyNames="TransactionID" OnRowCommand="DetailGrid_RowCommand" OnSorting="DetailGrid_Sorting"
						OnRowDataBound="DetailGrid_RowDataBound"   RowHeaderColumn="Transaction ID" 
						ListViewStandardType="METER_RECONCILIATION_DETAIL" ListViewType="STANDARD" FixedColumns="Edit,TransactionGuidText,TransactionAliasHidden,TransactionIDHidden" TabIndex="7" aria-label="Details">
							<Columns>
								<asp:TemplateField HeaderText="Edit">
									<HeaderStyle Width="100px" />
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
									<ItemTemplate>
										<FMControls:FMEditLinkButton ID="DetailViewTransactionButton" CommandName="ViewTransaction" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Transaction ID" SortExpression="TransactionID">
									<HeaderStyle Width="200px" />
									<ItemTemplate>
										<asp:Label ID="DetailTransactionIDGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.TransactionID") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Product" SortExpression="Product">
									<HeaderStyle Width="200px" />
									<ItemTemplate>
										<asp:Label ID="DetailProductGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.Product") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
		  
								<asp:TemplateField HeaderText="Meter Start" SortExpression="MeterStart" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailMeterStartGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
			   
								<asp:TemplateField HeaderText="Meter Stop" SortExpression="MeterStop" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailMeterStopGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
					  
								<asp:TemplateField HeaderText="Meter Difference" SortExpression="MeterTotal" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailMeterTotalGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Volume" SortExpression="Volume" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailVolumeGridColumn" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Meter Skip" SortExpression="MeterSkip" ItemStyle-HorizontalAlign="Right">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailMeterSkipGridLabel" runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
									
								<asp:TemplateField HeaderText="Carrier" SortExpression="Carrier">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailCarrierGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.Carrier") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
						   
								<asp:TemplateField HeaderText="Station ID" SortExpression="StationID">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailStationGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.StationID") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
						   
								<asp:TemplateField HeaderText="Transaction Alias" SortExpression="TransactionAlias">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailTransactionAliasGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.TransactionAlias") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
						   
								<asp:TemplateField HeaderText="Flight Number" SortExpression="FlightNumber">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailFlightNumberGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.FlightNumber") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>

								<asp:TemplateField HeaderText="Ticket Number" SortExpression="TicketNumber">
									<HeaderStyle Width="100px" />
									<ItemTemplate>
										<asp:Label ID="DetailTicketNumberGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.TicketNumber") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="TransactionGuidText" Visible="false">
									<ItemTemplate>
										<asp:Literal ID="DetailTransactionGuidText" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TransactionGuid").ToString() %>' />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="TransactionAliasHidden" Visible="false">
									<ItemTemplate>
										<asp:Literal ID="DetailTransactionAliasGridHidden" Text='<%# DataBinder.Eval(Container, "DataItem.TransactionAlias") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="TransactionIDHidden" Visible="false">
									<HeaderStyle Width="200px" />
									<ItemTemplate>
										<asp:Literal ID="DetailTransactionIDGridHidden" Text='<%# DataBinder.Eval(Container, "DataItem.TransactionID") %>' runat="server" />
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
						</FMControls:FMGridViewConfigurable>
					</td>
				</tr>
				<tr>
					<td colspan="4"></td>
					<td  align="center" colspan="2">
						<FMControls:FMButton id="CloseButton" runat="server" CssClass="formfieldtitle" Text="Close" onclick="CloseButton_Click" Width="60px" TabIndex="8"/>
					</td>
				</tr>
			  
			</table>
		</div>
	</form>
</body>
</html>
