<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppointmentDetailPage.aspx.cs" Inherits="FuelsManager.AppointmentWebApp.AddAppointmentPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" Content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <style type="text/css" runat="server"> 
            /* Correct placement of Ajax combo box drop-down lists */
            #TypeDropDownList ul 
            { 
                position: absolute !important; 
                left: 66px !important; 
                top: 60px !important; 
            }
            #CategoryComboBox ul 
            { 
                position: absolute !important; 
                left: 425px !important; 
                top: 60px !important; 
            }
        </style>
</head>
<body>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
         <script type="text/javascript">
             function ShowSelected() {
                 var lb = document.getElementById('SelectedForListBox');
                 if (lb != null) {
                     var options = lb.options;
                     for (var i = options.length - 1; i > 0; i--) {
                         if (options[i].selected == true) {
                             options[i].focus();
                             options[i].selected = true;
                             return;
                         }
                     }
                 }
             }
             Sys.Application.add_load(ShowSelected);
         </script> 
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel id="MainHeaderLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="800px" BackColor="Transparent">Scheduler Detail</FMControls:FMLabel>
    <div>
			<FMControls:FMLabel id="FMLabel2" AssociatedControlID="TypeDropDownList$TypeDropDownList_TextBox" style="Z-INDEX: 110; LEFT: 24px; POSITION: absolute; TOP: 40px"
				runat="server" CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>				
			<FMControls:FMLabel id="FMLabel3" AssociatedControlID="CategoryComboBox$CategoryComboBox_TextBox" style="Z-INDEX: 110; LEFT: 320px; POSITION: absolute; TOP: 40px"
				runat="server" CssClass="formfieldtitle" BackColor="Transparent">Category:</FMControls:FMLabel>	
			<FMControls:FMLabel id="FMLabel4" AssociatedControlID="SelectedForListBox" style="Z-INDEX: 110; LEFT: 24px; POSITION: absolute; TOP: 70px"
				runat="server" CssClass="formfieldtitle" BackColor="Transparent">For:</FMControls:FMLabel>		
			<asp:UpdatePanel ID="UpdatePanel2" runat="server" >
				<ContentTemplate>
				<FMControls:FMComboBox id="TypeDropDownList" 
						RenderMode="Block" runat="server"  width = "136" DropDownStyle="DropDownList" style="position: relative;visibility:hidden"
						AutoCompleteMode="Suggest" CssClass="formfield" AutoPostBack="True" EnableViewState="true" TabIndex="3"
						OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged"/>
				<FMControls:FMListBox id="SelectedForListBox" runat="server" style="position:absolute; top:70px; left: 65px; width:225px; height:450px;" SelectionMode="Multiple" />
				<FMControls:FMComboBox ID="CategoryComboBox" style="position: relative;visibility:hidden"
						RenderMode="Block" runat="server" width = "136" DropDownStyle="DropDownList" 
						AutoCompleteMode="Suggest" CssClass="formfield" AutoPostBack="true" EnableViewState="true" TabIndex="5"
						OnSelectedIndexChanged="CategoryDropDownListSelectedIndexChanged"/>
				</ContentTemplate>
			</asp:UpdatePanel>
				
			<asp:UpdatePanel ID="UpdatePanel1" runat="server" >
				<ContentTemplate>
			        <FMCONTROLS:FMRadioButton id="SingleRadioButton" style="Z-INDEX: 142; LEFT: 320px; POSITION: absolute; TOP: 100px"
					        tabIndex="8" runat="server" GroupName="SingleReoccuringGroup" Text="Single" CssClass="formfieldtitle"
					        Width="80px" AutoPostBack="True" OnCheckedChanged="OnSingleReOccuringCheckedChanged"/>
			        <FMCONTROLS:FMRadioButton id="ReOccuringRadioButton" style="Z-INDEX: 142; LEFT: 400px; POSITION: absolute; TOP: 100px"
					        tabIndex="9" runat="server" GroupName="SingleReoccuringGroup" Text="Recurring" CssClass="formfieldtitle"
					        Width="120px" AutoPostBack="True"  OnCheckedChanged="OnSingleReOccuringCheckedChanged"/>
			        <FMControls:FMLabel id="FindLabel" AssociatedControlID="DurationTextBox" style="Z-INDEX: 110; LEFT: 320px; POSITION: absolute; TOP: 130px"
				        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Duration:</FMControls:FMLabel>				
			        <asp:TextBox id="DurationTextBox" style="Z-INDEX: 107; LEFT: 388px; POSITION: absolute; TOP: 130px"
				        runat="server" CssClass="formfield" Width="60px" tabIndex="10" MaxLength="5" ></asp:TextBox>
			        <FMControls:FMLabel id="FMLabel1" style="Z-INDEX: 110; LEFT: 460px; POSITION: absolute; TOP: 130px"
				        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Minutes</FMControls:FMLabel>				
			        <FMCONTROLS:FMCHECKBOX id="WeekendCheckBox" style="Z-INDEX: 110; LEFT: 318px; POSITION: absolute; TOP: 160px"
				        tabIndex="11" runat="server" Width="138px" CssClass="formfieldtitle" Text="Use Weekends" AutoPostBack="true" OnCheckedChanged="WeekendCheckBox_OnCheckedChanged"/>
			        <FMCONTROLS:FMCHECKBOX id="HolidayCheckBox" style="Z-INDEX: 110; LEFT: 460px; POSITION: absolute; TOP: 160px"
				        tabIndex="12" runat="server" Width="138px" CssClass="formfieldtitle" Text="Use Holidays" />
				
					<FMControls:FMLabel id="PeriodLabel" AssociatedControlID="PeriodComboBox" style="Z-INDEX: 110; LEFT: 320px; POSITION: absolute; TOP: 220px"
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Period:</FMControls:FMLabel>				
					<FMControls:FMDropDownList ID="PeriodComboBox" tabIndex="14"
						style="Z-INDEX: 110; LEFT: 424px; POSITION: absolute; TOP: 220px" sort="false"
								runat="server" width = "136" CssClass="formfield" AutoPostBack="true" 
						EnableViewState="true"  
						OnSelectedIndexChanged="PeriodComboBox_OnSelectionChanged" />
								
						<!-- Daily Options -->
					<FMControls:FMLabel id="DailyOptionLable" AssociatedControlID="DailyTextBox" style="Z-INDEX: 110; LEFT: 424px; POSITION: absolute; TOP: 250px" 
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Happens Every:</FMControls:FMLabel>				
					<asp:TextBox id="DailyTextBox" style="Z-INDEX: 107; LEFT: 530px; POSITION: absolute; TOP: 250px"
						runat="server" CssClass="formfield" Width="35px" tabIndex="15" MaxLength="4"></asp:TextBox>
					<FMControls:FMLabel id="TimePeriodLabel" style="Z-INDEX: 110; LEFT: 574px; POSITION: absolute; TOP: 250px"
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Day(s)</FMControls:FMLabel>				
								
						<!-- weekly Options -->
					<FMControls:FMLabel id="WeeklyHappensOnLabel" AssociatedControlID="DayOfTheWeekDownList" style="Z-INDEX: 110; LEFT: 320px; POSITION: absolute; TOP: 280px" 
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Happens On:</FMControls:FMLabel>				
					<FMControls:FMDropDownList ID="DayOfTheWeekDownList" style="Z-INDEX: 110; LEFT: 432px; POSITION: absolute; TOP: 280px" sort="false"
								runat="server" width = "136" CssClass="formfield" AutoPostBack="true" EnableViewState="true" tabIndex="16"/>
								
						<!-- monthly Options -->
					<FMCONTROLS:FMRadioButton id="MonthlySelectMonthDay" style="Z-INDEX: 142; LEFT: 320px; POSITION: absolute; TOP: 250px"
						runat="server" GroupName="MonthlyGroup" Text="Happens on day:" CssClass="formfieldtitle" tabIndex="17"
						Width="120px" AutoPostBack="True" OnCheckedChanged="OnMonthlyOptionsCheckedChanged" />
					<asp:TextBox id="MonthDayTextBox" style="Z-INDEX: 107; LEFT: 442px; POSITION: absolute; TOP: 250px"
						runat="server" CssClass="formfield" Width="25px" tabIndex="19" MaxLength="3" alt="Day of the month"></asp:TextBox>
					<FMControls:FMLabel id="ReoccuresLabel" AssociatedControlID="ReOccuresTextBox" style="Z-INDEX: 110; LEFT: 492px; POSITION: absolute; TOP: 250px" 
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Re-Occurs Every:</FMControls:FMLabel>				
					<asp:TextBox id="ReOccuresTextBox" style="Z-INDEX: 107; LEFT: 602px; POSITION: absolute; TOP: 250px"
						runat="server" CssClass="formfield" Width="20px" tabIndex="20" MaxLength="3"></asp:TextBox>
					<FMControls:FMLabel id="MonthLabel" style="Z-INDEX: 110; LEFT: 632px; POSITION: absolute; TOP: 250px" 
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Month(s)</FMControls:FMLabel>				
						
						
						
					<FMCONTROLS:FMRadioButton id="MonthlySelectByDayAndMonth" style="Z-INDEX: 141; LEFT: 320px; POSITION: absolute; TOP: 280px"
						tabIndex="18" runat="server" GroupName="MonthlyGroup" Text="Happens on the:" CssClass="formfieldtitle"
						AutoPostBack="True" OnCheckedChanged="OnMonthlyOptionsCheckedChanged" />
					<FMControls:FMDropDownList ID="MonthDayDropDownList" style="Z-INDEX: 110; LEFT: 442px; POSITION: absolute; TOP: 280px" sort="false"
								runat="server" width = "100" CssClass="formfield" AutoPostBack="true" EnableViewState="true" tabIndex="21"/>
					<FMControls:FMDropDownList ID="MonthDayOfTheWeekDropDownList" style="Z-INDEX: 110; LEFT: 552px; POSITION: absolute; TOP: 280px" sort="false"
								runat="server" width = "100" CssClass="formfield" AutoPostBack="true" EnableViewState="true" tabIndex="22"/>
					<FMControls:FMLabel id="MonthOfLabel" AssociatedControlID="TextBox1" style="Z-INDEX: 110; LEFT: 662px; POSITION: absolute; TOP: 280px" 
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Every</FMControls:FMLabel>				
					<asp:TextBox id="TextBox1" style="Z-INDEX: 107; LEFT: 702px; POSITION: absolute; TOP: 280px"
						runat="server" CssClass="formfield" Width="20px" tabIndex="23" MaxLength="3"></asp:TextBox>
					<FMControls:FMLabel id="MonthOptin1MonthLabel" style="Z-INDEX: 180; LEFT: 752px; POSITION: absolute; TOP: 280px" 
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Month(s)</FMControls:FMLabel>				
								
						
						<!-- yearly Options -->
					<FMCONTROLS:FMRadioButton id="YearlyHappensEveryYearOn" style="Z-INDEX: 142; LEFT: 320px; POSITION: absolute; TOP: 250px"
						tabIndex="17" runat="server" GroupName="YearlyGroup" Text="Happens Every Year On:" CssClass="formfieldtitle"
						Width="160px" AutoPostBack="True" OnCheckedChanged="OnMonthlyOptionsCheckedChanged" />
					<FMControls:FMDropDownList ID="YearlyMonthOption1DownList" style="Z-INDEX: 110; LEFT: 482px; POSITION: absolute; TOP: 250px" sort="false"
								runat="server" width = "100" CssClass="formfield" AutoPostBack="true" EnableViewState="true" tabIndex="19" alt="Yearly month"/>
					<asp:TextBox id="YearlyDayOption1TextBox" style="Z-INDEX: 107; LEFT: 602px; POSITION: absolute; TOP: 250px"
						runat="server" CssClass="formfield" Width="20px" tabIndex="20" MaxLength="2" alt="Yearly Day"></asp:TextBox>
						
						
						
					<FMControls:FMLabel id="DescriptionLabel" AssociatedControlID="DescriptionTextBox" style="Z-INDEX: 110; LEFT: 320px; POSITION: absolute; TOP: 190px"
						runat="server" CssClass="formfieldtitle" BackColor="Transparent">Description:</FMControls:FMLabel>				
					<asp:TextBox id="DescriptionTextBox" style="Z-INDEX: 107; LEFT: 424px; POSITION: absolute; TOP: 190px"
						runat="server" CssClass="formfield" Width="275px" tabIndex="13" MaxLength="50"></asp:TextBox>
					<FMCONTROLS:FMRadioButton id="YearlyHappensOnThe" style="Z-INDEX: 141; LEFT: 320px; POSITION: absolute; TOP: 280px"
						tabIndex="18" runat="server" GroupName="YearlyGroup" Text="Happens on the:" CssClass="formfieldtitle"
						AutoPostBack="True" OnCheckedChanged="OnMonthlyOptionsCheckedChanged" />
					<!-- for yearly we reuse the monthly MonthDayDropDownList and MonthDayOfTheWeekDropDownList lists -->
					<FMControls:FMDropDownList ID="YearlyMonthOption2DownList" style="Z-INDEX: 110; LEFT: 702px; POSITION: absolute; TOP: 280px" sort="false"
								runat="server" width="100" CssClass="formfield" AutoPostBack="true" EnableViewState="true" tabIndex="23" alt="Yearly month option 2" />
								
				<FMControls:FMLabel id="TestSetLabel" AssociatedControlID="TestSetDropDownList" style="Z-INDEX: 110; LEFT: 320px; POSITION: absolute; TOP: 190px"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Test Set:</FMControls:FMLabel>				
				<FMControls:FMDropDownList ID="TestSetDropDownList" 
					style="Z-INDEX: 110; LEFT: 424px; POSITION: absolute; TOP: 190px" sort="false"
					tabIndex="13" runat="server" width = "190" CssClass="formfield" AutoPostBack="false" EnableViewState="true" />
								
				</ContentTemplate>
			</asp:UpdatePanel>
			<FMControls:FMButton id="New" style="Z-INDEX: 106; LEFT: 512px; POSITION: absolute; TOP: 504px" TabIndex="1"
				runat="server" Width="70px" CssClass="formfieldtitle" Text="New" ></FMControls:FMButton>	
			<FMControls:FMButton id="OK" style="Z-INDEX: 106; LEFT: 592px; POSITION: absolute; TOP: 504px" TabIndex="1"
				runat="server" Width="70px" CssClass="formfieldtitle" Text="OK"></FMControls:FMButton>
			<FMControls:FMButton id="Cancel"	style="Z-INDEX: 103; LEFT: 672px; POSITION: absolute; TOP: 504px" TabIndex="2"
				runat="server" Width="70px" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel" ></FMControls:FMButton>
			<FMCONTROLS:FMLABEL id="FMLABEL6" style="Z-INDEX: 121; LEFT: 318px; POSITION: absolute; TOP: 70px"
				runat="server" BackColor="Transparent" Text="Start Date" Height="16px" Width="100px" CssClass="formfieldtitle">Start Date</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMDATETIME id="StartDate" style="Z-INDEX: 201; LEFT: 380px; POSITION: absolute; TOP: 70px"
					runat="server" CssClass="formfield" Width="260px" TabIndex="6"></FMCONTROLS:FMDATETIME>
				
    </div>
    
            <asp:button id="HiddenButton" runat="server" CausesValidation="False" style="DISPLAY: none; POSITION: static" Text="Button" />

				<script type="text/jscript">
				    function pageLoad() {

					var comboboxSelectType = $get('<%=TypeDropDownList.ClientID + "_" + TypeDropDownList.ClientID %>' + '_Table');
					comboboxSelectType.style.position = "absolute";
					comboboxSelectType.style.left = "65px";
					comboboxSelectType.style.top = "40px";
					comboboxSelectType.style.visibility = "visible";
					comboboxSelectType.visible = "true";

					var comboboxCategory = $get('<%=CategoryComboBox.ClientID + "_" + CategoryComboBox.ClientID %>' + '_Table');
					comboboxCategory.style.position = "absolute";
					comboboxCategory.style.left = "424px";
					comboboxCategory.style.top = "40px";
					comboboxCategory.style.visibility = "visible";

				}
				</script>
    </div>
</form>
</body>
</html>
