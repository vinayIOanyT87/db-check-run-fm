<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TankMeterAssignmentPage.ascx.cs" Inherits="FuelsManager.FMWebApp.TankMeterAssignmentPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title></title>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body role="application">
	<table style="z-index:110; left:0px; top: 10px; width:500px; position:absolute" cellpadding="5" role="presentation" aria-label="layout">

		<tr>
			<td>
				<FMControls:FMGridView ID="MeterGrid" runat="server" FixedHeaders="false" Width="600px"  RowHeaderColumn="Meter ID"
						AllowPaging="false" ShowFooter="true" ShowFooterWhenEmpty="true" EmptyDataText="No meters assigned" TabIndex="1" aria-label="Meter">
					<Columns>
						<FMControls:FMEditCommandField EditText="Edit Meter" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="70px" ItemStyle-Width="70px"/>
						<asp:TemplateField>
						<HeaderTemplate><FMControls:FMLabel ID="MeterIDHeaderLabel" Text="Meter ID" runat="server" /> <span style="COLOR: red"> *</span></HeaderTemplate>
							<HeaderStyle Width="110px" />
							<ItemTemplate>
								<FMControls:FMLabel ID="MeterIDLabel" Text='<%# DataBinder.Eval(Container, "DataItem.MeterID") %>' runat="server" />
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMTextBox ID="MeterIDTextBox" ToolTip="Meter ID" Text='<%# DataBinder.Eval(Container, "DataItem.MeterID") %>' runat="server" MaxLength="30" aria-required="true"/>
							</EditItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate><FMControls:FMLabel ID="NumberOfDigitsHeaderLabel" Text="Number of Digits" runat="server" /> <span style="COLOR: red"> *</span></HeaderTemplate>
							<HeaderStyle Width="110px" />
							<ItemTemplate>
								<FMControls:FMLabel ID="NumberOfDigitsLabel" Text='<%# DataBinder.Eval(Container, "DataItem.NumberOfDigits") %>' runat="server" />
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMTextBox ID="NumberOfDigitsTextBox" ToolTip="Number of Digits" Text='<%# DataBinder.Eval(Container, "DataItem.NumberOfDigits") %>' runat="server" MaxLength="2" aria-required="true" />
							</EditItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Rotates Backwards">
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<HeaderStyle Width="115px" />
							<ItemTemplate>
								<FMControls:FMCheckBox ID="RotatesBackwardsDisplayCheckBox" Checked='<%# DataBinder.Eval(Container, "DataItem.RotatesBackwardsFlag") %>' runat="server" Enabled="false"/>
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMCheckBox ID="RotatesBackwardsEditCheckBox" Checked='<%# DataBinder.Eval(Container, "DataItem.RotatesBackwardsFlag") %>' runat="server" />
							</EditItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Receipt Meter">
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<HeaderStyle Width="100px" />
							<ItemTemplate>
								<FMControls:FMCheckBox ID="ReceiptMeterDisplayCheckBox" Checked='<%# DataBinder.Eval(Container, "DataItem.ReceiptMeterFlag") %>' runat="server" Enabled="false" />
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMCheckBox ID="ReceiptMeterEditCheckBox" Checked='<%# DataBinder.Eval(Container, "DataItem.ReceiptMeterFlag") %>' runat="server" />
							</EditItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Delete">
								<HeaderStyle Width="25px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								<ItemTemplate>
									<FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" CommandName="Delete"/>
								</ItemTemplate>
						</asp:TemplateField>       
					</Columns>

				</FMControls:FMGridView>
			</td>
		</tr>                 
		<tr>
			<td colspan="5" align="left">
				<FMControls:FMButton id="AddButton" runat="server" CssClass="formfieldtitle" Text="Add"  Width="100px" TabIndex="2"/>
			</td>
		</tr>                                                                                                                                           
	</table>
</body>
</html>