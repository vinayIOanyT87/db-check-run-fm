<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="StationMeterPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.StationMeterPage" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
	<table style="z-index:110; left:0px; top: 10px; width:160px; position:absolute" cellpadding="5">
	<tr>
	    <td>      
		        <FMControls:FMLabel ID="MeterIDLabel" AssociatedControlID="MeterIDTextBox" runat="server" CssClass="formfieldtitle" Text="Meter ID:" Width="55px"/><span style="COLOR: red; width: 3px">*</span>
        </td>
	    <td>
		    <FMControls:FMTextBox ID="MeterIDTextBox" CssClass="formfield"  runat="server" MaxLength="30" Width="100px" TabIndex="1" />
	    </td>
        <td>&nbsp&nbsp&nbsp&nbsp</td>
        <td>
			<FMControls:FMLabel ID="RotatesBackwardLabel" runat="server" CssClass="formfieldtitle" Text="Rotates Backwards:" Width="110px" margin-left="30px"/>
		</td>
		<td>
			<FMControls:FMCheckBox ID="RotatesBackwardCheckBox" CssClass="formfield" runat="server" TabIndex="5" />
		</td>
    </tr>
    <tr>   
        <td>  
            <div style="width:115px">
                <FMControls:FMLabel ID="NumberOfDigitsLabel" AssociatedControlID="NumberOfDigitsTextBox" runat="server" CssClass="formfieldtitle" Text="Number of Digits:" Width="105px"/><span style="COLOR: red; width: 3px">*</span>
            </div>  
        </td>  
		<td>
			<FMControls:FMTextBox ID="NumberOfDigitsTextBox" CssClass="formfield"  runat="server" MaxLength="2" Width="100px" TabIndex="2" />
		</td>
        <td>&nbsp&nbsp&nbsp&nbsp</td>
        <td>
			<FMControls:FMLabel ID="ReceiptMeterLabel" runat="server" CssClass="formfieldtitle" Text="Receipt Meter:" Width="90px"/>
		</td>
		<td>
			<FMControls:FMCheckBox ID="ReceiptMeterCheckBox"  CssClass="formfield" runat="server" TabIndex="6" />
		</td>
    </tr>
    <tr>   
        <td>  
            <div style="width:115px">
                <FMControls:FMLabel ID="MeterFactorLabel" AssociatedControlID="MeterFactorTextBox" runat="server" CssClass="formfieldtitle" Text="Meter Factor:" Width="105px"/><span style="COLOR: red; width: 3px">*</span>
            </div>  
        </td>  
		<td>
			<FMControls:FMTextBox ID="MeterFactorTextBox" CssClass="formfield"  runat="server" MaxLength="8" Width="100px" TabIndex="3" />
		</td>
        <td colspan="3">&nbsp&nbsp&nbsp&nbsp</td>
    </tr>
    <tr>   
        <td>
            <div style="width:115px">
                <FMControls:FMLabel ID="FuelCompressionFactorLabel" AssociatedControlID="FuelCompressionFactorTextBox" runat="server" CssClass="formfieldtitle" Text="Fuel CP:" Width="105px"/><span style="COLOR: red; width: 3px">*</span>
            </div>  
		</td>
		<td>
			<FMControls:FMTextBox ID="FuelCompressionFactorTextBox" CssClass="formfield"  runat="server" MaxLength="8" Width="100px" TabIndex="4" />
		</td>
        <td colspan="3">&nbsp&nbsp&nbsp&nbsp</td>
    </tr>
	</table>
	<FMControls:FMDataGrid id="ProcessVariablesDataGrid" style="Z-INDEX: 107; LEFT: 0px; POSITION: absolute; TOP: 150px"
		runat="server" PageSize="1" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
		GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" Width="552px" CssClass="tabletext"
		BackColor="White"  aria-label="Process Variables">
		<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
		<EditItemStyle Wrap="False"></EditItemStyle>
		<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
		<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
		<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
		<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
		<Columns>
			<asp:ButtonColumn Text="&lt;img src=Images/Edit.gif border=0 align=absmiddle alt='Edit this item'&gt;"
				HeaderText="Edit" CommandName="Edit">
				<HeaderStyle Width="55px"></HeaderStyle>
				<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
			</asp:ButtonColumn>
			<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index"></asp:BoundColumn>
			<asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server"></asp:BoundColumn>
			<asp:BoundColumn DataField="OPCItemID" HeaderText="Item ID"></asp:BoundColumn>
		</Columns>
		<PagerStyle CssClass="tablepager" ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
	</FMControls:FMDataGrid>
	<FMControls:FMLabel id="AssociatedTankLabel" runat="server" CssClass="formfieldtitle" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 200px"
		Width="106px">Associated Tank:</FMControls:FMLabel>
	<asp:DropDownList id="AssociatedTanks" style="Z-INDEX: 102; LEFT: 116px; POSITION: absolute; TOP: 200px"
		runat="server" CssClass="formfield" Width="240px" AutoPostBack="True" tabIndex="7"></asp:DropDownList>
	<FMControls:FMLabel id="ArmsServicedLabel" CssClass="formfieldtitle" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 232px"
		runat="server" Width="106px">Arms Serviced:</FMControls:FMLabel>
	<asp:textbox id="ArmsServiced" style="Z-INDEX: 102; LEFT: 116px; POSITION: absolute; TOP: 232px"
		runat="server" CssClass="formfield" BackColor="White" Width="240px" MaxLength="30" tabIndex="8"></asp:textbox>
	</body>
</HTML>
