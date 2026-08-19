<%@ Page language="c#" Codebehind="FuelTicketForm.aspx.cs" AutoEventWireup="True" Inherits="TicketingWebApp.FuelTicketForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title>FuelTicketForm</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../FuelsManager.css" rel="stylesheet">
		<script language="jscript">
			function Line1MeterChange()
			{
				var oMeterStartTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl3_MeterStartTextBox");
				var oMeterEndTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl3_MeterEndTextBox");
				var oTotalTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl3_TotalTextBox");
				if(oMeterStartTextBox != null
				&& oMeterEndTextBox != null
				&& oTotalTextBox != null)
				{
					var MeterStart=parseInt(oMeterStartTextBox.value,10);
					var MeterEnd=parseInt(oMeterEndTextBox.value,10);
					oTotalTextBox.value=(MeterEnd-MeterStart).toString();
				}
			}

			function Line2MeterChange()
			{
				var oMeterStartTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl4_MeterStartTextBox");
				var oMeterEndTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl4_MeterEndTextBox");
				var oTotalTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl4_TotalTextBox");
				if(oMeterStartTextBox != null
				&& oMeterEndTextBox != null
				&& oTotalTextBox != null)
				{
					var MeterStart=parseInt(oMeterStartTextBox.value,10);
					var MeterEnd=parseInt(oMeterEndTextBox.value,10);
					oTotalTextBox.value=(MeterEnd-MeterStart).toString();
				}
			}

			function Line3MeterChange()
			{
				var oMeterStartTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl5_MeterStartTextBox");
				var oMeterEndTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl5_MeterEndTextBox");
				var oTotalTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl5_TotalTextBox");
				if(oMeterStartTextBox != null
				&& oMeterEndTextBox != null
				&& oTotalTextBox != null)
				{
					var MeterStart=parseInt(oMeterStartTextBox.value,10);
					var MeterEnd=parseInt(oMeterEndTextBox.value,10);
					oTotalTextBox.value=(MeterEnd-MeterStart).toString();
				}
			}

			function Line4MeterChange()
			{
				var oMeterStartTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl6_MeterStartTextBox");
				var oMeterEndTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl6_MeterEndTextBox");
				var oTotalTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl6_TotalTextBox");
				if(oMeterStartTextBox != null
				&& oMeterEndTextBox != null
				&& oTotalTextBox != null)
				{
					var MeterStart=parseInt(oMeterStartTextBox.value,10);
					var MeterEnd=parseInt(oMeterEndTextBox.value,10);
					oTotalTextBox.value=(MeterEnd-MeterStart).toString();
				}
			}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<asp:textbox id="DateTextbox" style="Z-INDEX: 105; LEFT: 80px; POSITION: absolute; TOP: 56px"
				tabIndex="1" runat="server" CssClass="formfield" Width="88px" MaxLength="20"></asp:textbox>
			<asp:image id="Image1" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="..\FMWebApp\images\Page_Fade_7.jpg"></asp:image><FMCONTROLS:FMDROPDOWNLIST id="TypeDropDownList" style="Z-INDEX: 148; LEFT: 80px; POSITION: absolute; TOP: 88px"
				tabIndex="3" runat="server" CssClass="formfield" Width="144px"></FMCONTROLS:FMDROPDOWNLIST><FMCONTROLS:FMLABEL id="FMLABEL9" style="Z-INDEX: 147; LEFT: 16px; POSITION: absolute; TOP: 88px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="48px">Type:</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="FMLABEL8" style="Z-INDEX: 118; LEFT: 16px; POSITION: absolute; TOP: 152px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="48px">Owner:</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="FMLABEL7" style="Z-INDEX: 108; LEFT: 16px; POSITION: absolute; TOP: 184px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="42px"> Airline:</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="FMLABEL6" style="Z-INDEX: 122; LEFT: 16px; POSITION: absolute; TOP: 216px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="48px">Agent:</FMCONTROLS:FMLABEL><asp:textbox id="FinalGaugeTextBox" style="Z-INDEX: 146; LEFT: 632px; POSITION: absolute; TOP: 120px"
				tabIndex="19" runat="server" CssClass="formfield" Width="120px" MaxLength="20"></asp:textbox><asp:textbox id="RequiredGaugeTextBox" style="Z-INDEX: 145; LEFT: 632px; POSITION: absolute; TOP: 88px"
				tabIndex="18" runat="server" CssClass="formfield" Width="120px" MaxLength="20"></asp:textbox><asp:textbox id="ArrivalGaugeTextBox" style="Z-INDEX: 144; LEFT: 632px; POSITION: absolute; TOP: 56px"
				tabIndex="17" runat="server" CssClass="formfield" Width="120px" MaxLength="20"></asp:textbox><FMCONTROLS:FMLABEL id="FMLabel5" style="Z-INDEX: 143; LEFT: 528px; POSITION: absolute; TOP: 120px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="73px">Final Gauge:</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="FMLabel4" style="Z-INDEX: 142; LEFT: 528px; POSITION: absolute; TOP: 88px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="93px">Required Gauge:</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="FMLabel3" style="Z-INDEX: 141; LEFT: 528px; POSITION: absolute; TOP: 56px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="80px">Arrival Gauge:</FMCONTROLS:FMLABEL><asp:dropdownlist id="ManagerDropDownList" style="Z-INDEX: 138; LEFT: 80px; POSITION: absolute; TOP: 120px"
				tabIndex="4" runat="server" CssClass="formfield" Width="144px"></asp:dropdownlist><FMCONTROLS:FMLABEL id="FMLabel1" style="Z-INDEX: 136; LEFT: 16px; POSITION: absolute; TOP: 120px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="48px">Manager:</FMCONTROLS:FMLABEL><asp:dropdownlist id="GateDropDownList" style="Z-INDEX: 135; LEFT: 344px; POSITION: absolute; TOP: 152px"
				tabIndex="11" runat="server" CssClass="formfield" Width="136px"></asp:dropdownlist><asp:dropdownlist id="AircraftDropDownList" style="Z-INDEX: 134; LEFT: 344px; POSITION: absolute; TOP: 56px"
				tabIndex="8" runat="server" CssClass="formfield" Width="136px" AutoPostBack="True"></asp:dropdownlist><asp:dropdownlist id="ShipToDropDownList" style="Z-INDEX: 132; LEFT: 80px; POSITION: absolute; TOP: 184px"
				tabIndex="6" runat="server" CssClass="formfield" Width="144px" AutoPostBack="True" onselectedindexchanged="ShipToDropDownList_SelectedIndexChanged"></asp:dropdownlist><asp:dropdownlist id="CarrierDropDownList" style="Z-INDEX: 124; LEFT: 80px; POSITION: absolute; TOP: 216px"
				tabIndex="7" runat="server" CssClass="formfield" Width="144px" AutoPostBack="True" onselectedindexchanged="CarrierDropDownList_SelectedIndexChanged"></asp:dropdownlist><FMCONTROLS:FMLABEL id="Label9" style="Z-INDEX: 123; LEFT: 16px; POSITION: absolute; TOP: 216px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="48px">Agent:</FMCONTROLS:FMLABEL><asp:dropdownlist id="OwnerDropDownList" style="Z-INDEX: 120; LEFT: 80px; POSITION: absolute; TOP: 152px"
				tabIndex="5" runat="server" CssClass="formfield" Width="144px"></asp:dropdownlist><FMCONTROLS:FMLABEL id="Label8" style="Z-INDEX: 119; LEFT: 16px; POSITION: absolute; TOP: 152px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="48px">Owner:</FMCONTROLS:FMLABEL><asp:checkbox id="FTZCheckBox" style="Z-INDEX: 117; LEFT: 528px; POSITION: absolute; TOP: 152px"
				tabIndex="16" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="42px" Text="FTZ" TextAlign="Left"></asp:checkbox><FMCONTROLS:FMLABEL id="Label7" style="Z-INDEX: 116; LEFT: 256px; POSITION: absolute; TOP: 152px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="34px">Gate:</FMCONTROLS:FMLABEL><asp:dropdownlist id="DestinationDropDownList" style="Z-INDEX: 115; LEFT: 344px; POSITION: absolute; TOP: 184px"
				tabIndex="12" runat="server" CssClass="formfield" Width="136px"></asp:dropdownlist><FMCONTROLS:FMLABEL id="Label6" style="Z-INDEX: 114; LEFT: 256px; POSITION: absolute; TOP: 184px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="66px">Destination:</FMCONTROLS:FMLABEL><asp:textbox id="AircraftTypeTextBox" style="Z-INDEX: 113; LEFT: 344px; POSITION: absolute; TOP: 88px"
				tabIndex="9" runat="server" CssClass="formfield" Width="136px" MaxLength="20"></asp:textbox><FMCONTROLS:FMLABEL id="Label4" style="Z-INDEX: 112; LEFT: 256px; POSITION: absolute; TOP: 88px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="80px">Aircraft Type:</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="Label3" style="Z-INDEX: 111; LEFT: 256px; POSITION: absolute; TOP: 56px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="88px">Tail Number:</FMCONTROLS:FMLABEL><asp:textbox id="FlightNumberTextBox" style="Z-INDEX: 110; LEFT: 344px; POSITION: absolute; TOP: 120px"
				tabIndex="10" runat="server" CssClass="formfield" Width="136px" MaxLength="20"></asp:textbox><FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 109; LEFT: 16px; POSITION: absolute; TOP: 184px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="42px"> Airline:</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="Label1" style="Z-INDEX: 107; LEFT: 256px; POSITION: absolute; TOP: 120px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="96px">Flight Number:</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="Label5" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 16px" runat="server"
				BackColor="Transparent" CssClass="headline" Width="136px">Fuel Ticket</FMCONTROLS:FMLABEL><FMCONTROLS:FMBUTTON id="DateButton" style="Z-INDEX: 106; LEFT: 192px; POSITION: absolute; TOP: 56px"
				tabIndex="2" runat="server" CssClass="formfieldtitle" Text="Set"></FMCONTROLS:FMBUTTON><asp:calendar id="DateCalendar" style="Z-INDEX: 149; LEFT: 48px; POSITION: absolute; TOP: 56px"
				runat="server" BackColor="White" CssClass="formfield" Width="154px" Visible="False" Height="176px" onselectionchanged="DateCalendar_SelectionChanged"></asp:calendar><FMCONTROLS:FMLABEL id="Label19" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 56px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="48px">Date:</FMCONTROLS:FMLABEL>
			<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 16px; WIDTH: 500px; POSITION: absolute; TOP: 248px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<TR>
					<TD style="WIDTH: 10px; HEIGHT: 10px" width="498"><FMCONTROLS:FMDATAGRID id="FuelTicketLineItemsDataGrid" tabIndex="20" runat="server" BackColor="White"
							CssClass="tabletext" Width="725px" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical"
							AutoGenerateColumns="False" BorderStyle="None" PageSize="4">
							<FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:LinkButton ID="DeleteButton" runat="server" Text="<img src=..\FMWebApp\Images\Delete.gif border=0 align=absmiddle alt='Delete this item'>"
											CommandName="Delete" CausesValidation="false"></asp:LinkButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Ticket">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemTemplate>
										<asp:TextBox width=.5in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DocumentNumber") %>' ID=DocumentTextBox>
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Pit">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemTemplate>
										<asp:TextBox width=.5in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PitID") %>' ID=PitIDTextBox>
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Equipment">
									<HeaderStyle Width="0.7in"></HeaderStyle>
									<ItemTemplate>
										<asp:dropdownlist width=.7in CssClass=tabletext runat="server" Enabled="True" ID="EquipmentDropDownList" DataSource="<%# EquipmentListItemCollection%>" DataTextField="Text" DataValueField="Value">
										</asp:dropdownlist>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Operator">
									<HeaderStyle Width="0.7in"></HeaderStyle>
									<ItemTemplate>
										<asp:dropdownlist width=.7in CssClass=tabletext runat="server" Enabled="True" ID="OperatorDropDownList" DataSource="<%# OperatorListItemCollection%>" DataTextField="Text" DataValueField="Value">
										</asp:dropdownlist>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Product">
									<HeaderStyle Width="0.7in"></HeaderStyle>
									<ItemTemplate>
										<asp:dropdownlist width=.7in CssClass=tabletext runat="server" Enabled="True" ID="ProductDropDownList" DataSource="<%# ProductListItemCollection%>" DataTextField="Text" DataValueField="Value">
										</asp:dropdownlist>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Meter Start">
									<HeaderStyle Width="0.7in"></HeaderStyle>
									<ItemTemplate>
										<asp:TextBox width=.7in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MeterStart") %>' ID="MeterStartTextBox" >
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Meter End">
									<HeaderStyle Width="0.7in"></HeaderStyle>
									<ItemTemplate>
										<asp:TextBox width=.7in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MeterEnd") %>' ID="MeterEndTextBox">
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Total">
									<HeaderStyle Width="0.7in"></HeaderStyle>
									<ItemTemplate>
										<asp:TextBox width=.7in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Total") %>' ID="TotalTextBox">
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Start Time">
									<HeaderStyle Width="0.7in"></HeaderStyle>
									<ItemTemplate>
										<asp:TextBox width=.7in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.StartTime") %>' ID="StartTimeTextBox">
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Stop Time">
									<HeaderStyle Width="0.6in"></HeaderStyle>
									<ItemTemplate>
										<asp:TextBox width=.6in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.StopTime") %>' ID="StopTimeTextBox">
										</asp:TextBox>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 10px" vAlign="middle" width="498">
						<table style="WIDTH: 730px; HEIGHT: 30px">
							<tr>
								<td style="WIDTH: 72%; HEIGHT: 10px"><FMCONTROLS:FMBUTTON id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
										tabIndex="60"></FMCONTROLS:FMBUTTON></td>
								<td style="WIDTH: 193px; HEIGHT: 10px"><FMCONTROLS:FMBUTTON id="SubmitButton" runat="server" Width="104px" Text="Submit" CssClass="formfieldtitle"
										tabIndex="61"></FMCONTROLS:FMBUTTON></td>
								<td style="WIDTH: 498px; HEIGHT: 10px"><FMCONTROLS:FMBUTTON id="CloseButton" runat="server" Width="104px" Text="Close" CssClass="formfieldtitle"
										tabIndex="62"></FMCONTROLS:FMBUTTON></td>
							</tr>
						</table>
					</TD>
				</TR>
			</TABLE>
		</div>
</form>
		<script language="jscript">
			var oMeter1StartTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl3_MeterStartTextBox");
			if(oMeter1StartTextBox != null)
				oMeter1StartTextBox.attachEvent("onchange",Line1MeterChange);
			var oMeter1EndTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl3_MeterEndTextBox");
			if(oMeter1EndTextBox != null)
				oMeter1EndTextBox.attachEvent("onchange",Line1MeterChange);

			var oMeter2StartTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl4_MeterStartTextBox");
			if(oMeter2StartTextBox != null)
				oMeter2StartTextBox.attachEvent("onchange",Line2MeterChange);
			var oMeter2EndTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl4_MeterEndTextBox");
			if(oMeter2EndTextBox != null)
				oMeter2EndTextBox.attachEvent("onchange",Line2MeterChange);

			var oMeter3StartTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl5_MeterStartTextBox");
			if(oMeter3StartTextBox != null)
				oMeter3StartTextBox.attachEvent("onchange",Line3MeterChange);
			var oMeter3EndTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl5_MeterEndTextBox");
			if(oMeter3EndTextBox != null)
				oMeter3EndTextBox.attachEvent("onchange",Line3MeterChange);

			var oMeter4StartTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl6_MeterStartTextBox");
			if(oMeter4StartTextBox != null)
				oMeter4StartTextBox.attachEvent("onchange",Line4MeterChange);
			var oMeter4EndTextBox=document.getElementById("FuelTicketLineItemsDataGrid__ctl6_MeterEndTextBox");
			if(oMeter4EndTextBox != null)
				oMeter4EndTextBox.attachEvent("onchange",Line4MeterChange);
		</script>
	</body>
</HTML>
