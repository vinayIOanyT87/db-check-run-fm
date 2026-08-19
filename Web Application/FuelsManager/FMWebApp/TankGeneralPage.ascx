<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TankGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.TankGeneralPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
    <script type="text/javascript">
        function DisplayCalculateCoordinates()
        {
            var tankTypeDd = document.getElementById("tcTankTabs_tpGeneralPage_TankGeneralPage_TankTypeDropdown");
            if (tankTypeDd)
            {
                var selectedItem = tankTypeDd.options[tankTypeDd.selectedIndex].value;
                if (selectedItem !== "1")
                {
                    return;
                }
            }

            var latTextBox  = document.getElementById("tcTankTabs_tpGeneralPage_TankGeneralPage_LatitudeTextBox");
            var longTextBox = document.getElementById("tcTankTabs_tpGeneralPage_TankGeneralPage_LongitudeTextBox");
            var zoomTextbox = document.getElementById("tcTankTabs_tpGeneralPage_TankGeneralPage_ZoomTextBox");
            var argu        = "?";

            if (latTextBox.value == null || latTextBox.value === "")
            {
                argu = argu + "latitudeStr=-9999";
            }
            else
            {
                argu = argu + "latitudeStr=" + latTextBox.value;
            }

            if (longTextBox.value == null || longTextBox.value === "")
            {
                argu = argu + "&longitudeStr=-9999";
            }
            else
            {
                argu = argu + "&longitudeStr=" + longTextBox.value;
            }

            if (zoomTextbox.value == null || zoomTextbox.value === "")
            {
                argu = argu + "&zoomStr=-9999";
            }
            else
            {
                argu = argu + "&zoomStr=" + zoomTextbox.value;
            }

            showModalDialogFrame({
                url: "../AssetTrackingArea/AssetCalculateCoordinates/CalculateCoordinates" + argu,
                width: 855,
                height: 560,
                title: "Calculate Coordinates",
                onClose: function () {
                    if (this.returnValue != null)
                    {
                        var latitudeValue = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
                        var longitudeValue = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);
                        var zoomValue = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[2]);

                        // -9999 means the user pressed cancel.
                        if (latitudeValue !== "-9999" && longitudeValue !== "-9999" && zoomValue !== "-9999")
                        {
                            latTextBox.value = latitudeValue;
                            longTextBox.value = longitudeValue;
                            zoomTextbox.value = zoomValue;
                        }
                    }
                }
            });
        }
    </script>
	<body id="Body1" runat="server" style="BACKGROUND-REPEAT: no-repeat; BACKGROUND-COLOR: white"
		tabIndex="-1" background="<%$ AppSettings: PageFadeImage %>" MS_POSITIONING="GridLayout">
	
			<TABLE id="Table1" style="Z-INDEX: 102" cellSpacing="0" cellPadding="4" border="0" role="presentation" aria-label="layout">
				<tr>
					<td style="WIDTH: 30px"><FMCONTROLS:FMLABEL id="Label1" AssociatedControlID="TankID" runat="server" BackColor="Transparent" CssClass="formfieldtitle">ID:</FMCONTROLS:FMLABEL>&nbsp;&nbsp;<FMCONTROLS:FMLABEL id="Label4" runat="server" BackColor="Transparent" ForeColor="Crimson">*</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 50px"><asp:textbox id="TankID" runat="server" BackColor="White" CssClass="formfield" MaxLength="50"></asp:textbox></td>
					<td style="WIDTH: 80px"><FMCONTROLS:FMLABEL id="Label7" AssociatedControlID="ProductTypeDropDownList"  runat="server" BackColor="Transparent" CssClass="formfieldtitle">Product Type:</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 50px"><FMCONTROLS:FMDROPDOWNLIST id="ProductTypeDropDownList" runat="server" BackColor="White" CssClass="formfield"
							ForeColor="Black" AutoPostBack="True" onselectedindexchanged="ProductTypeDropDownListSelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST></td>					
                    <td style="WIDTH: 50px"><FMCONTROLS:FMLABEL id="FMLabel1" AssociatedControlID="ManagersDropDownList" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Manager:</FMCONTROLS:FMLABEL></td>
					<td><asp:dropdownlist id="ManagersDropDownList" runat="server" BackColor="White" CssClass="formfield"
							ForeColor="Black"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td><FMCONTROLS:FMLABEL id="Label2" AssociatedControlID="VesselTypeDropDownList" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Vessel Type:</FMCONTROLS:FMLABEL></td>
					<td><FMCONTROLS:FMDROPDOWNLIST id="VesselTypeDropDownList" runat="server" BackColor="White" CssClass="formfield"
							ForeColor="Black"></FMCONTROLS:FMDROPDOWNLIST></td>
					<td style="WIDTH: 80px"><FMCONTROLS:FMLABEL id="Label3" AssociatedControlID="ProductsDropDownList" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Product:</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 50px"><asp:dropdownlist id="ProductsDropDownList" runat="server" BackColor="White" CssClass="formfield"
							ForeColor="Black"></asp:dropdownlist><br>
					</td>					
                    <td style="WIDTH: 30px"><FMCONTROLS:FMLABEL id="FMLABEL2" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Owner:</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 50px"><asp:dropdownlist id="OwnersDownlist" runat="server" BackColor="White" CssClass="formfield"
							ForeColor="Black"></asp:dropdownlist></td>		
				</tr>
                <tr>
					<td style="WIDTH: 30px">
					    <FMCONTROLS:FMLABEL id="TankTypeLabel" AssociatedControlID="TankTypeDropdown" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Tank Type:</FMCONTROLS:FMLABEL>
					</td>    
                    <td>
                        <FMCONTROLS:FMDROPDOWNLIST id="TankTypeDropdown" runat="server" BackColor="White" CssClass="formfield"
							ForeColor="Black" AutoPostBack="True" onselectedindexchanged="TankTypeDropdownSelectedIndexChanged">
                        </FMCONTROLS:FMDROPDOWNLIST>
                    </td>  
                    <td>
					    <FMCONTROLS:FMLABEL id="TrackingDeviceLabel" AssociatedControlID="TrackingDeviceDropdown" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Tracking Device:</FMCONTROLS:FMLABEL>                        
                    </td>   
                    <td>
                        <FMCONTROLS:FMDROPDOWNLIST id="TrackingDeviceDropdown" runat="server" BackColor="White" CssClass="formfield"
							ForeColor="Black"></FMCONTROLS:FMDROPDOWNLIST>                        
                    </td>  
                    <td>
                        <FMCONTROLS:FMLABEL id="TankConfigNumberLabel" AssociatedControlID="TankConfigNumberDropdown" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Tank Configuration Number:</FMCONTROLS:FMLABEL>
                    </td>    
                    <td>
                        <FMCONTROLS:FMDROPDOWNLIST id="TankConfigNumberDropdown" runat="server" BackColor="White" CssClass="formfield"
							ForeColor="Black" AutoPostBack="True" onselectedindexchanged="TankConfigurationNumberSelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST>                        
                    </td>   
                </tr>
                <tr>
                    <td>
  					    <FMCONTROLS:FMLABEL id="LatitudeLabel" AssociatedControlID="LatitudeTextBox" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Latitude:</FMCONTROLS:FMLABEL>                                              
                    </td>   
                    <td style="WIDTH: 30px">
                        <asp:textbox id="LatitudeTextBox" runat="server" BackColor="White" CssClass="formfield" MaxLength="20"></asp:textbox>
                    </td>    
                    <td>
  					    <FMCONTROLS:FMLABEL id="LongitudeLabel" AssociatedControlID="LongitudeTextBox" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Longitude:</FMCONTROLS:FMLABEL>                                                                  
                    </td>  
                    <td>
                          <asp:textbox id="LongitudeTextBox" runat="server" BackColor="White" CssClass="formfield" MaxLength="20"></asp:textbox>                      
                    </td>
                    <td>
                        <FMCONTROLS:FMLABEL id="ZoomLabel" AssociatedControlID="ZoomTextBox" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Zoom:</FMCONTROLS:FMLABEL>                                              
                    </td>
                    <td>
                        <asp:textbox id="ZoomTextBox" runat="server" BackColor="White" CssClass="formfield" MaxLength="2"></asp:textbox>
                    </td>
                </tr>
                <tr>
                    <td>
                         <input ID="CalculateCoordBtn" type="button" onclick="DisplayCalculateCoordinates();" class="formfieldtitle"
                               value="Calculate tank coordinates"
                               Style="border: none; background: none; text-decoration: underline; cursor: pointer;"/>
                    </td>
                    <td>
					    <FMControls:FMCheckBox id="HiddenCheckBox" TextAlign="Left" Text="Hidden" CssClass="formfieldtitle" runat="server"></FMControls:FMCheckBox>
					</td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
				<tr>
					<td colSpan="6" style="WIDTH: 677px">
						<FMCONTROLS:FMDATAGRID id="ProcessVariablesDataGrid" runat="server" BackColor="White" CssClass="tabletext"  RowHeaderColumn="Type"
							Width="800px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
							CellPadding="3" PageSize="15" aria-label="Process Variables">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<EditItemStyle Wrap="False"></EditItemStyle>
							<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton ID="FMEditLinkButton1" runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index"></asp:BoundColumn>
								<asp:BoundColumn DataField="TypeID" HeaderText="Type"></asp:BoundColumn>
								<asp:BoundColumn DataField="EngineeringUnits" HeaderText="Units"></asp:BoundColumn>
								<asp:BoundColumn DataField="Maximum" HeaderText="Maximum"></asp:BoundColumn>
								<asp:BoundColumn DataField="Minimum" HeaderText="Minimum"></asp:BoundColumn>
								<asp:BoundColumn DataField="Host" HeaderText="System"></asp:BoundColumn>
								<asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server"></asp:BoundColumn>
								<asp:BoundColumn DataField="OPCItemID" HeaderText="Item ID"></asp:BoundColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID>
					</td>
				</tr>	
			</TABLE>
	</body>
</HTML>
