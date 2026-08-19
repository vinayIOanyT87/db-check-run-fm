<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Page language="c#" Codebehind="OperationsForm.aspx.cs" AutoEventWireup="True" Inherits="LoadRackWebApp.OperationsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<style>
			#bar {
				height:100%; 
				background-color:#486899;
			}

			#barwrapper {
				background-color: white;
				height: 6px;
				width: 200px;
				position: absolute;
				top: 60px;
				left: 480px;
			}
			
			#progresswrapper {
			LEFT: 480px; 
			POSITION: absolute; 
			TOP: 70px; 
			display: flex;
			}
			#LabelLastEOD {
				LEFT: 470px; 
				POSITION: absolute; 
				TOP: 90px; 
				font-size: 10px;
			}
		</style>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
            <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<asp:label id="Label2" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" CssClass="headline" Width="224px">Operations</asp:label>
			<asp:panel id="Panel1" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 40px"
				runat="server" BorderWidth="1px" BorderStyle="Solid" BorderColor="LightSteelBlue" Height="88px" Width="685px"></asp:panel>

			<asp:ScriptManager ID="ScriptManager1" runat="server">
			</asp:ScriptManager>
			<asp:UpdatePanel ID="UpdatePanel1" runat="server">
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="TimerControl1" />
			</Triggers>
				<ContentTemplate>
					<fieldset>
						<FMCONTROLS:FMBUTTON id="InitiateEndOfDayButton" OnClientClick="return InitiateEndOfDay();" style="Z-INDEX: 106; LEFT: 32px; POSITION: absolute; TOP: 50px"
							tabIndex="1" runat="server" width="120px" Text="Initiate End Of Day" CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON>
						<FMCONTROLS:FMLABEL id="FMLABEL6" style="Z-INDEX: 106; LEFT: 200px; POSITION: absolute; TOP: 56px;" runat="server"
								CssClass="formfieldtitle" Width="140px">Current Inventory Date</FMCONTROLS:FMLABEL>
						<FMCONTROLS:FMDATE id="CurrentInventoryDateControl" style="Z-INDEX: 115; LEFT: 340px; POSITION: absolute; TOP: 56px"
							tabIndex="4" runat="server" CssClass="formfield" Width="140px"></FMCONTROLS:FMDATE>
						<div id="barwrapper" runat="server" >
							<div id="bar" runat="server" style=""></div>
						</div>
						<span id="progresswrapper" >
							<img ID="ajaxloaderimg" runat="server" style="margin-right: 10px;" src="..\FMWebApp\images\ajax-loader.gif"/>
							<asp:Label ID="LabelProgress" runat="server" style="font-size: 10px;" BackColor="Transparent" CssClass="formfieldtitle" Width="224px" Text=""></asp:Label>
						</span>
						<asp:Label ID="LabelLastEOD" runat="server" style="font-size: 10px;" BackColor="Transparent" CssClass="formfieldtitle" Width="224px" Text=""></asp:Label>
					</fieldset>
				</ContentTemplate>
			</asp:UpdatePanel>
			<asp:Timer ID="TimerControl1" runat="server" Interval=1000 OnTick="TimerControl1_Tick"></asp:Timer>
			<FMCONTROLS:FMLABEL id="FMLABEL7" style="Z-INDEX: 106; LEFT: 12px; POSITION: absolute; TOP: 110px;" runat="server"
					CssClass="formfieldtitle" Width="700px">Warning: Running a manual EOD may prevent the automatic EOD from running and will increment the inventory date.</FMCONTROLS:FMLABEL>
			<asp:panel id="Panel2" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 134px" runat="server"
				BorderWidth="1px" BorderStyle="Solid" BorderColor="LightSteelBlue" Height="80px" 
				Width="685px"></asp:panel>
			<FMCONTROLS:FMLABEL id="Fmlabel1" style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 142px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="224px">House Card Assignment</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label" AssociatedControlID="PersonnelDropDownList1" style="Z-INDEX: 105; LEFT: 32px; POSITION: absolute; TOP: 166px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Personnel:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMDROPDOWNLIST id="PersonnelDropDownList1" style="Z-INDEX: 108; LEFT: 32px; POSITION: absolute; TOP: 182px"
				tabIndex="2" runat="server" CssClass="formfield" Width="248px" AutoPostBack="True">
			</FMCONTROLS:FMDROPDOWNLIST>
			<FMCONTROLS:FMLABEL id="Label3" AssociatedControlID="CardDropDownList" style="Z-INDEX: 109; LEFT: 288px; POSITION: absolute; TOP: 166px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Card:</FMCONTROLS:FMLABEL>
			<asp:dropdownlist id="CardDropDownList" style="Z-INDEX: 110; LEFT: 288px; POSITION: absolute; TOP: 182px"
				tabIndex="3" runat="server" CssClass="formfield" Width="209px" AutoPostBack="True"></asp:dropdownlist>
			<FMCONTROLS:FMBUTTON id="AssignButton" style="Z-INDEX: 111; LEFT: 520px; POSITION: absolute; TOP: 174px; width: 66px;"
				tabIndex="4" runat="server" Text="Assign" CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON>
			<asp:panel id="Panel3" style="Z-INDEX: 112; LEFT: 8px; POSITION: absolute; TOP: 220px" runat="server"
				BorderWidth="1px" BorderStyle="Solid" BorderColor="LightSteelBlue" Height="56px" Width="685px"></asp:panel>
			<FMCONTROLS:FMLABEL id="FMLabel9" style="Z-INDEX: 115; LEFT: 32px; POSITION: absolute; TOP: 230px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="224px">Tank Data Mode</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="FMLabel10" style="Z-INDEX: 115; LEFT: 32px; POSITION: absolute; TOP: 254px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="224px">Current:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="CurrentTankDataMode" style="Z-INDEX: 115; LEFT: 100px; POSITION: absolute; TOP: 254px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="224px">Realtime Data</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="FMLabel11" style="Z-INDEX: 115; LEFT: 312px; POSITION: absolute; TOP: 254px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Set To:</FMCONTROLS:FMLABEL>
            <FMCONTROLS:FMBUTTON id="TankDataButton" OnClientClick="return ModifyTankData();" style="Z-INDEX: 113; LEFT: 383px; POSITION: absolute; TOP: 240px"
				tabIndex="5" runat="server" Text="Use Last Known Good Tank Data" 
				CssClass="formfieldtitle" Width="200px"></FMCONTROLS:FMBUTTON>
				
			<asp:panel id="Panel4" style="Z-INDEX: 114; LEFT: 8px; POSITION: absolute; TOP: 281px" runat="server"
				BorderWidth="1px" BorderStyle="Solid" BorderColor="LightSteelBlue" Height="94px" 
				Width="685px"></asp:panel>
			<FMCONTROLS:FMBUTTON id="ResetLastActivityDate" style="Z-INDEX: 118; LEFT: 300px; POSITION: absolute; TOP: 326px; width: 66px;"
				tabIndex="7" runat="server" Text="Reset" CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMDROPDOWNLIST id="PersonnelDropDownList2" style="Z-INDEX: 117; LEFT: 32px; POSITION: absolute; TOP: 334px"
				tabIndex="6" runat="server" CssClass="formfield" Width="248px" AutoPostBack="True" OnSelectedIndexChanged="PersonnelDropDownList2_SelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST>
			<FMCONTROLS:FMLABEL id="FMLabel3" AssociatedControlID="PersonnelDropDownList2" style="Z-INDEX: 116; LEFT: 32px; POSITION: absolute; TOP: 316px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Personnel:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="FMLabel2" style="Z-INDEX: 115; LEFT: 32px; POSITION: absolute; TOP: 292px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="224px">Reset Personnel Last Activity Date</FMCONTROLS:FMLABEL>
			<asp:panel id="Panel5" style="Z-INDEX: 119; LEFT: 8px; POSITION: absolute; TOP: 382px" runat="server"
				BorderWidth="1px" BorderStyle="Solid" BorderColor="LightSteelBlue" Height="56px" Width="685px"></asp:panel>
            <asp:panel id="Panel6" style="Z-INDEX: 125; LEFT: 8px; POSITION: absolute; TOP: 442px" runat="server"
				BorderWidth="1px" BorderStyle="Solid" BorderColor="LightSteelBlue" Height="88px" Width="685px">
			</asp:panel>
			<FMCONTROLS:FMLABEL id="FMLabel5" AssociatedControlID="BOLNumberTextBox" style="Z-INDEX: 122; LEFT: 32px; POSITION: absolute; TOP: 390px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="80px">BOL Number:</FMCONTROLS:FMLABEL>
			<asp:textbox id="BOLNumberTextBox" style="Z-INDEX: 124; LEFT: 32px; POSITION: absolute; TOP: 406px"
				tabIndex="8" runat="server" CssClass="formfield" Width="88px"></asp:textbox>
			<FMCONTROLS:FMLABEL id="FMLabel4" AssociatedControlID="DataExchangeDropDownList" style="Z-INDEX: 120; LEFT: 144px; POSITION: absolute; TOP: 390px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Data Exchange Profile:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMDROPDOWNLIST id="DataExchangeDropDownList" style="Z-INDEX: 121; LEFT: 144px; POSITION: absolute; TOP: 406px"
				tabIndex="9" runat="server" CssClass="formfield" Width="248px" AutoPostBack="True"></FMCONTROLS:FMDROPDOWNLIST>
			<FMCONTROLS:FMBUTTON id="SendPIDXTransButton" style="Z-INDEX: 123; LEFT: 408px; POSITION: absolute; TOP: 400px; width: 66px;"
				tabIndex="10" runat="server" Text="Send" CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMLABEL id="FMLabel8" AssociatedControlID="StationDropdownList" style="Z-INDEX: 122; LEFT: 32px; POSITION: absolute; TOP: 460px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="80px">Station:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMDROPDOWNLIST id="StationDropdownList" style="Z-INDEX: 117; LEFT: 32px; POSITION: absolute; TOP: 476px"
				tabIndex="6" runat="server" CssClass="formfield" Width="248px" AutoPostBack="True"></FMCONTROLS:FMDROPDOWNLIST>
			<FMCONTROLS:FMBUTTON id="DownloadAccessConfiguration" OnClientClick="return DownloadAccessConfig();" style="Z-INDEX: 126; LEFT: 332px; POSITION: absolute; TOP: 470px"
				tabIndex="1" runat="server" Text="Download Access Configuration" 
				CssClass="formfieldtitle" Width="220px"></FMCONTROLS:FMBUTTON>
			<asp:panel id="Panel7" style="Z-INDEX: 125; LEFT: 8px; POSITION: absolute; TOP: 528px" runat="server"
					BorderWidth="1px" BorderStyle="Solid" BorderColor="LightSteelBlue" Height="53px" Width="685px"></asp:panel>		
            <FMCONTROLS:FMBUTTON  id="ResetOwnerAllocationsButton" OnClientClick="return ResetOwnerAllocations();" style="Z-INDEX: 126; LEFT: 32px; POSITION: absolute; TOP: 540px"
					tabIndex="1" width="150px" runat="server" Text="Reset Owner Allocations" CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON>
            <script type="text/javascript">
			function InitiateEndOfDay()
			{
				var oInitiateEndOfDayButton=document.getElementById("InitiateEndOfDayButton");
				if(oInitiateEndOfDayButton != null)
				    return confirm(oInitiateEndOfDayButton.value);

			    return false;
			}

			function ModifyTankData()
			{
				var oTankDataButton=document.getElementById("TankDataButton");
				if(oTankDataButton != null)
				    return confirm(oTankDataButton.value);

			    return false;
			}

			function ResetOwnerAllocations()
			{
				var oResetOwnerAllocationsButton=document.getElementById("ResetOwnerAllocationsButton");
				if(oResetOwnerAllocationsButton != null)
				    return confirm(oResetOwnerAllocationsButton.value);

			    return false;
			}

			function DownloadAccessConfig() {
				var oDownloadAccessConfigurationButton = document.getElementById("DownloadAccessConfiguration");
				if (oDownloadAccessConfigurationButton != null)
				    return confirm(oDownloadAccessConfigurationButton.value);

			    return false;
			}
		</script>	
            <p>
			<asp:image id="Image2" alt="<%$ AppSettings: PageFadeImageAlt %>"
					style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="../FMWebApp/Images/Page_Fade_Blank.bmp"></asp:image>
			</p>		
		</div>
</form>
		</body>
</HTML>
