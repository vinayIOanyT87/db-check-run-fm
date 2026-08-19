<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileGeneralSettingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProfileGeneralSettingPage" %>

<html>
	<head>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
			.style1
			{
				width: 292px;
			}
			.style2
			{
				width: 265px;
			}
		</style>
	</head>
	<body>
		<table style="Z-INDEX: 103; width:64%; LEFT: 5px; POSITION: absolute; TOP: 50px; height: 192px;">     
			<tr>
				<td>
					<FMCONTROLS:FMLABEL id="ProfileIDLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">Profile ID</FMCONTROLS:FMLABEL>                
				</td>
				<td class="style1" >
					<asp:textbox id="ProfileIDTB" tabIndex="1" Width="271px" CssClass="formfield" runat="server" 
						MaxLength="50" Columns="50"></asp:textbox>               
				</td>
				<td class="style2" >
					<FMCONTROLS:FMCHECKBOX id="ShowProductScreenCB" TextAlign="Right" 
						Text="Show Product Screen" Height="27px" 
						width="160px" CssClass="formfieldtitle" runat="server" TabIndex="14"></FMCONTROLS:FMCHECKBOX>
				</td>
			</tr>
			<tr>
				<td >
					<FMCONTROLS:FMLABEL id="DescriptionLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">Description</FMCONTROLS:FMLABEL>
				</td>
				<td class="style1" >
					<asp:textbox id="DescriptionTB" tabIndex="2" Width="278px" CssClass="formfield" runat="server" 
					MaxLength="200" TextMode="MultiLine"></asp:textbox>                
				</td>
				<td class="style2" >
					<FMCONTROLS:FMCHECKBOX id="GenerateTicketNumberCB" TextAlign="Right" 
						Text="Generate Ticket Number" Height="27px" 
						width="160px" CssClass="formfieldtitle" runat="server" TabIndex="15"></FMCONTROLS:FMCHECKBOX>
				</td>
			</tr>
			<tr>
				<td >
					<FMCONTROLS:FMLABEL id="SearchTypeLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">Search Type</FMCONTROLS:FMLABEL>                
				</td>
				<td class="style1" >
					<FMCONTROLS:FMDropDownList id="SearchTypeDD" tabIndex="3" TextAlign="Left" 
						Height="27px" Width="168px" 
					CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
				</td>
				<td class="style2" >
					<FMCONTROLS:FMCHECKBOX id="ShowOperatorFieldCB" TextAlign="Right" 
						Text="Show Operator Field in Flight List" Height="27px" 
						width="255px" CssClass="formfieldtitle" runat="server" TabIndex="16"></FMCONTROLS:FMCHECKBOX>
				</td>
		   </tr>
			<tr>
				<td >
					<FMCONTROLS:FMLABEL id="ShutDownHotKeyLabel" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">Shut Down Hot-Key</FMCONTROLS:FMLABEL>
				</td>
				<td class="style1" >
					<table>
						<tr>
							<td>
								<FMCONTROLS:FMDropDownList id="ShutdownHotKey1DD" tabIndex="4" 
									TextAlign="Left" Height="27px" Width="60px" 
									CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
									onselectedindexchanged="ShutdownHotKey1OnChange"></FMCONTROLS:FMDropDownList>
							</td>
							<td>
								<FMCONTROLS:FMDropDownList id="ShutdownHotKey2DD" tabIndex="5" 
									TextAlign="Left" Height="27px" Width="60px" 
									CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
									onselectedindexchanged="ShutdownHotKey2OnChange"></FMCONTROLS:FMDropDownList>
							</td>
							<td>
								<FMCONTROLS:FMDropDownList id="ShutdownHotKey3DD" tabIndex="6" 
									TextAlign="Left" Height="27px" Width="60px" 
									CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
									onselectedindexchanged="ShutdownHotKey3OnChange"></FMCONTROLS:FMDropDownList>
							</td>
							<td>
								<FMCONTROLS:FMDropDownList id="ShutdownHotKey4DD" tabIndex="7" TextAlign="Left" 
									Height="27px" Width="60px" 
									CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
							</td>
						</tr>
					</table>	
				</td>
				<td class="style2" >
					<FMCONTROLS:FMCHECKBOX id="MonitorScreenTransitionTimingCB" TextAlign="Right" 
						Text="Monitor Screen Transition Timing" Height="27px" 
						width="255px" CssClass="formfieldtitle" runat="server" TabIndex="17"></FMCONTROLS:FMCHECKBOX>
				</td>
		  </tr>
			<tr>
				<td >
				<FMCONTROLS:FMLABEL id="AdminPasswordLabel" CssClass="formfieldtitle" 
						runat="server" BackColor="Transparent" Width="161px">Admin Password</FMCONTROLS:FMLABEL>
				</td>
				<td class="style1" >
				<FMControls:FMTextBox id="AdminPasswordTextbox" tabIndex="8" Width="169px" 
					CssClass="formfield" runat="server" 
					MaxLength="24" Columns="24" TextMode="Password"></FMControls:FMTextBox>               
				</td>
				<td class="style2" >
					<FMCONTROLS:FMCHECKBOX id="BypassFsrCheckCB" TextAlign="Right" 
						Text="Bypass FSR Check on Screen Transition" Height="27px" 
						width="255px" CssClass="formfieldtitle" runat="server" TabIndex="18"></FMCONTROLS:FMCHECKBOX>
				</td>
		  </tr>
			<tr>
				<td >
				<FMCONTROLS:FMLABEL id="VehicleIdLbl" CssClass="formfieldtitle" 
						runat="server" BackColor="Transparent" Width="161px">Vehicle ID</FMCONTROLS:FMLABEL>
				</td>
				<td class="style1" >
				<FMControls:FMTextBox id="VehicleIdTB" tabIndex="9" Width="169px" 
					CssClass="formfield" runat="server"
					MaxLength="50" Columns="50"></FMControls:FMTextBox>               
				</td>
				<td class="style2" >
					<FMCONTROLS:FMCHECKBOX id="ShowFuelUpdateCheckStatusWindowCB" TextAlign="Right" 
						Text="Show Fuel Update Check Status Window" Height="27px" 
						width="255px" CssClass="formfieldtitle" runat="server" TabIndex="19"></FMCONTROLS:FMCHECKBOX>
				</td>
		  </tr>
		  <tr>
		  	<td>
				<FMCONTROLS:FMLABEL id="AllowableFailedLoginAttemptsLbl" CssClass="formfieldtitle" 
						runat="server" BackColor="Transparent" Width="161px">Allowable Failed Login Attempts</FMCONTROLS:FMLABEL>
		  	</td>
			<td class="style1" >
				<FMControls:FMTextBox id="AllowableFailedLoginAttemptsTB" tabIndex="10" 
					Width="168px" CssClass="formfield" runat="server" 
					MaxLength="3" Columns="3"></FMControls:FMTextBox>
			</td>
			<td class="style2">
		  		<FMCONTROLS:FMCHECKBOX id="MakeDefaultProfileCB" TextAlign="Right" 
						Text="Make Default Profile" Height="27px" 
						width="255px" CssClass="formfieldtitle" runat="server" TabIndex="22"></FMCONTROLS:FMCHECKBOX>		  		
				</td>
		  </tr>
		  <tr>
		  	<td>
				<FMCONTROLS:FMLABEL id="FuelDistributionPrecisionLbl" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent" Width="161px">Fuel Distribution Precision</FMCONTROLS:FMLABEL>
			  </td>
		  	<td class="style1">
				<FMControls:FMTextBox id="FuelDistributionPrecisionTB" tabIndex="11" 
					Width="168px" CssClass="formfield" runat="server" 
					MaxLength="3" Columns="3"></FMControls:FMTextBox>
			  </td>
		  	<td class="style2">
				<FMCONTROLS:FMCHECKBOX id="LoggingOptionCB" TextAlign="Right" 
						Text="Logging Option" Height="27px" 
						width="255px" CssClass="formfieldtitle" runat="server" TabIndex="21"></FMCONTROLS:FMCHECKBOX>
		  	</td>
		  </tr>
		  <tr>
		  	<td>
				<FMCONTROLS:FMLABEL id="DefaultPrinterLbl" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent" Width="161px">Default Printer</FMCONTROLS:FMLABEL>
			</td>
		  	<td class="style1" >
				<FMCONTROLS:FMDropDownList id="DefaultPrinterDD" tabIndex="12" TextAlign="Left" 
					Height="27px" Width="168px" 
					CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
			</td>
		  	<td class="style2">
				<FMCONTROLS:FMCHECKBOX id="UseDefaultPrinterCB" TextAlign="Right" 
						Text="Use Default Printer" Height="27px" 
						width="255px" CssClass="formfieldtitle" runat="server" TabIndex="20" AutoPostBack="True" 
					oncheckedchanged="UseDefaultPrinterCheckedChange"></FMCONTROLS:FMCHECKBOX>
			</td>
		  </tr>
		</table>	
	</body>
</html>