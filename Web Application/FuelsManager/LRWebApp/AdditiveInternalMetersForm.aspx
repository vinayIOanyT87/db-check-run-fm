<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdditiveInternalMetersForm.aspx.cs" Inherits="LoadRackWebApp.AdditiveInternalMetersForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
		 <title></title>
		 <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</head>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<asp:label id="Label1" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" Width="224px" CssClass="headline">Additive Internal Meters</asp:label>
			<table id="Table1" style="Z-INDEX: 101; LEFT: 16px; WIDTH: 10px; POSITION: absolute; TOP: 64px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" width="700" border="0" role="presentation" aria-label="layout">
				<tbody>
					<tr>
						<td vAlign="middle" width="498" height="36"><fmcontrols:fmpagesizedropdown id="InternalMetersFormPageSizeDropDown" ToolTip="Page size" runat="server"></fmcontrols:fmpagesizedropdown>
                            <fmcontrols:fmdropdownlist id="StationFilterDropDown" ToolTip="Station filter" style="LEFT: 175px; POSITION: absolute; TOP: 8px" tabIndex="1"
								runat="server" Width="200px" CssClass="formfield" AutoPostBack="True"></fmcontrols:fmdropdownlist></td>
					</tr>
					<tr>
						<td style="WIDTH: 686px; HEIGHT: 10px" width="686">
						    <FMCONTROLS:FMDATAGRID id="InternalMetersDataGrid" tabIndex="2" runat="server" BackColor="White" Width="432px"
								CssClass="tabletext" PageSize="16" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False"
								BorderStyle="None" AllowPaging="True" aria-label="Internal Meters Grid">
								<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<COLUMNS>
									<ASP:TEMPLATECOLUMN HeaderText="Station">
										<ITEMTEMPLATE>
											<asp:Label id="StationLabel" runat="server" CssClass="tabletext" NAME="StationLabel" Text='<%# DataBinder.Eval(Container, "DataItem.StationID") %>'>
											</asp:Label>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
									<ASP:TEMPLATECOLUMN HeaderText="StationGuid" Visible="false">
										<ITEMTEMPLATE>
											<asp:Label id="StationGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.StationGuid") %>'>
											</asp:Label>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
									<ASP:TEMPLATECOLUMN HeaderText="Arm">
										<ITEMTEMPLATE>
											<asp:Label id="ArmLabel" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.Arm") %>'>
											</asp:Label>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
									<ASP:TEMPLATECOLUMN HeaderText="ArmGuid" Visible="false">
										<ITEMTEMPLATE>
											<asp:Label id="ArmGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ArmGuid") %>'>
											</asp:Label>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
									<ASP:TEMPLATECOLUMN HeaderText="ComponentIndex" Visible="false">
										<ITEMTEMPLATE>
											<asp:Label id="ComponentIndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ComponentIndex") %>'>
											</asp:Label>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
									<ASP:TEMPLATECOLUMN HeaderText="Meter">
										<ITEMTEMPLATE>
											<asp:Label id="MeterLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Meter") %>'>
											</asp:Label>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
									<ASP:TEMPLATECOLUMN HeaderText="Additive">
										<ITEMTEMPLATE>
											<asp:Label id="AdditiveLabel" runat="server" CssClass="tabletext" NAME="AdditiveLabel" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>'>
											</asp:Label>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
									<ASP:TEMPLATECOLUMN HeaderText="ProductGuid" Visible="false">
										<ITEMTEMPLATE>
											<asp:Label id="ProductGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductGuid") %>'>
											</asp:Label>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
									<ASP:TEMPLATECOLUMN HeaderText="Value">
										<ITEMTEMPLATE>
											<asp:Label id="MeterValueLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MeterValue") %>'>
											</asp:Label>
											<asp:textbox Width=1in MaxLength="30" CssClass="tabletext" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container, "DataItem.MeterValue") %>' Enabled="True" ID="MeterValueTextBox">
											</asp:textbox>
										</ITEMTEMPLATE>
									</ASP:TEMPLATECOLUMN>
								</COLUMNS>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMDATAGRID>
						</td>
					</tr>
					<tr>
						<td width="507" height="10">
							<FMCONTROLS:FMBUTTON id="EditButton" tabIndex="3" runat="server" CssClass="formfieldtitle" Width=" 80px"
								Text="Edit"></FMCONTROLS:FMBUTTON>
							<FMCONTROLS:FMBUTTON id="ApplyButton" tabIndex="4" runat="server" CssClass="formfieldtitle" Width=" 80px"
								style="LEFT: 90px; POSITION: absolute" Text="Apply"></FMCONTROLS:FMBUTTON>
							<FMCONTROLS:FMBUTTON id="CancelButton" tabIndex="5" runat="server" CssClass="formfieldtitle" Width=" 80px"
								style="LEFT: 180px; POSITION: absolute" Text="Cancel"></FMCONTROLS:FMBUTTON>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
</form>
	</body>
</html>
