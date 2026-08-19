<%@ Control language="c#" Codebehind="SiteVaporRecoveryPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteVaporRecoveryPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
	</HEAD>
	<body>
	<asp:textbox id="DeadbandTextBox" style="Z-INDEX: 122; LEFT: 184px; POSITION: absolute; TOP: 264px"
		tabIndex="17" runat="server" CssClass="formfield" Width="112px"></asp:textbox>
	<FMControls:FMLabel id="Label9" AssociatedControlID="DeadbandTextBox" style="Z-INDEX: 121; LEFT: 0px; POSITION: absolute; TOP: 272px" runat="server"
		CssClass="formfieldtitle" Width="104px" BackColor="Transparent">Deadband:</FMControls:FMLabel>
	<asp:textbox id="SetpointTextBox" style="Z-INDEX: 120; LEFT: 184px; POSITION: absolute; TOP: 232px"
		tabIndex="16" runat="server" CssClass="formfield" Width="112px"></asp:textbox>
	<FMControls:FMLabel id="Label8" AssociatedControlID="SetpointTextBox" style="Z-INDEX: 118; LEFT: 0px; POSITION: absolute; TOP: 240px" runat="server"
		CssClass="formfieldtitle" Width="104px" BackColor="Transparent">Setpoint:</FMControls:FMLabel>
	<asp:textbox id="VRUCurrentYearActualTextBox" ToolTip="Current year actual VRU" style="Z-INDEX: 117; LEFT: 328px; POSITION: absolute; TOP: 168px"
		tabIndex="15" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<asp:textbox id="VRUYearlyActualTextBox" ToolTip="Yearly actual VRU" style="Z-INDEX: 116; LEFT: 328px; POSITION: absolute; TOP: 136px"
		tabIndex="12" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<asp:textbox id="VRUDailyActualTextBox" ToolTip="Daily actual VRU" style="Z-INDEX: 115; LEFT: 328px; POSITION: absolute; TOP: 104px"
		tabIndex="9" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<asp:textbox id="VRUHourlyActualTextBox" ToolTip="Hourly Actual VRU" style="Z-INDEX: 114; LEFT: 328px; POSITION: absolute; TOP: 72px"
		tabIndex="6" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<asp:textbox id="VRURateActualTextBox" ToolTip="VRU actual limit" style="Z-INDEX: 113; LEFT: 328px; POSITION: absolute; TOP: 40px"
		tabIndex="3" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<asp:textbox id="VRUCurrentYearLimitTextBox" ToolTip="VRU current limit" style="Z-INDEX: 112; LEFT: 184px; POSITION: absolute; TOP: 168px"
		tabIndex="14" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<asp:textbox id="VRUYearlyLimitTextBox" ToolTip="VRU yearly limit" style="Z-INDEX: 111; LEFT: 184px; POSITION: absolute; TOP: 136px"
		tabIndex="11" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<asp:textbox id="VRUDailyLimitTextBox" ToolTip="VRU daily limit" style="Z-INDEX: 110; LEFT: 184px; POSITION: absolute; TOP: 104px"
		tabIndex="8" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<asp:textbox id="VRUHourlyLimitTextBox" ToolTip="VRU hourly limit " style="Z-INDEX: 109; LEFT: 184px; POSITION: absolute; TOP: 72px"
		tabIndex="5" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<FMControls:FMLabel id="ActualLabel" style="Z-INDEX: 108; LEFT: 328px; POSITION: absolute; TOP: 16px"
		runat="server" Width="40px" BackColor="Transparent" CssClass="formfieldtitle">Actual:</FMControls:FMLabel>
	<asp:textbox id="VRURateLimitTextBox" ToolTip="VRU current limit" style="Z-INDEX: 107; LEFT: 184px; POSITION: absolute; TOP: 40px; right: 885px;"
		tabIndex="2" runat="server" Width="112px" CssClass="formfield"></asp:textbox>
	<FMControls:FMLabel id="Label5" style="Z-INDEX: 106; LEFT: 0px; POSITION: absolute; TOP: 168px" runat="server"
		Width="104px" BackColor="Transparent" CssClass="formfieldtitle">Current Year:</FMControls:FMLabel>
	<FMControls:FMLabel id="Label4" style="Z-INDEX: 105; LEFT: 0px; POSITION: absolute; TOP: 136px" runat="server"
		Width="104px" BackColor="Transparent" CssClass="formfieldtitle">Yearly:</FMControls:FMLabel>
	<FMControls:FMLabel id="Label3" style="Z-INDEX: 104; LEFT: 0px; POSITION: absolute; TOP: 104px" runat="server"
		Width="104px" BackColor="Transparent" CssClass="formfieldtitle">Daily:</FMControls:FMLabel>
	<FMControls:FMLabel id="Label2" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 72px" runat="server"
		Width="104px" BackColor="Transparent" CssClass="formfieldtitle">Hourly:</FMControls:FMLabel>
	<FMControls:FMLabel id="Label1" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 40px" runat="server"
		Width="104px" BackColor="Transparent" CssClass="formfieldtitle">Current:</FMControls:FMLabel>
	<FMControls:FMLabel id="LimitLabel" style="Z-INDEX: 101; LEFT: 184px; POSITION: absolute; TOP: 16px"
		runat="server" Width="32px" BackColor="Transparent" CssClass="formfieldtitle">Limit:</FMControls:FMLabel>
	<FMControls:FMLabel id="Label10" style="Z-INDEX: 101; LEFT: 120px; POSITION: absolute; TOP: 16px" runat="server"
		Width="48px" BackColor="Transparent" CssClass="formfieldtitle">Enabled:</FMControls:FMLabel>
	<FMControls:FMCheckbox id="VRURateLimitEnabledCheckBox" ToolTip="Current" style="Z-INDEX: 103; LEFT: 120px; POSITION: absolute; TOP: 40px"
		runat="server" Width="24px" CssClass="formfieldtitle" BackColor="Transparent" tabIndex="1"></FMControls:FMCheckbox>
	<FMControls:FMCheckbox id="VRUHourlyLimitEnabledCheckBox" ToolTip="Hourly" style="Z-INDEX: 103; LEFT: 120px; POSITION: absolute; TOP: 72px"
		runat="server" Width="24px" CssClass="formfieldtitle" BackColor="Transparent" tabIndex="4"></FMControls:FMCheckbox>
	<FMControls:FMCheckbox id="VRUDailyLimitEnabledCheckBox" ToolTip="Daily" style="Z-INDEX: 103; LEFT: 120px; POSITION: absolute; TOP: 104px"
		runat="server" Width="24px" CssClass="formfieldtitle" BackColor="Transparent" tabIndex="7"></FMControls:FMCheckbox>
	<FMControls:FMCheckbox id="VRUYearlyLimitEnabledCheckBox" ToolTip="Yearly" style="Z-INDEX: 103; LEFT: 120px; POSITION: absolute; TOP: 136px"
		runat="server" Width="24px" CssClass="formfieldtitle" BackColor="Transparent" tabIndex="10"></FMControls:FMCheckbox>
	<FMControls:FMCheckbox id="VRUCurrentYearLimitEnabledCheckBox" ToolTip="Current year" style="Z-INDEX: 103; LEFT: 120px; POSITION: absolute; TOP: 168px"
		runat="server" Width="24px" CssClass="formfieldtitle" BackColor="Transparent" tabIndex="13"></FMControls:FMCheckbox>
	<asp:label id="CurrentYearUnitsLabel" style="Z-INDEX: 101; LEFT: 456px; POSITION: absolute; TOP: 168px" runat="server"
		Width="72px" BackColor="Transparent" CssClass="formfieldtitle">gal (US)</asp:label>
	<asp:label id="RateUnitsLabel" style="Z-INDEX: 101; LEFT: 456px; POSITION: absolute; TOP: 40px" runat="server"
		Width="72px" BackColor="Transparent" CssClass="formfieldtitle">gal (US)</asp:label>
	<asp:label id="YearlyUnitsLabel" style="Z-INDEX: 101; LEFT: 456px; POSITION: absolute; TOP: 136px" runat="server"
		Width="72px" BackColor="Transparent" CssClass="formfieldtitle">gal (US)</asp:label>
	<asp:label id="DailyUnitsLabel" style="Z-INDEX: 101; LEFT: 456px; POSITION: absolute; TOP: 104px" runat="server"
		Width="72px" BackColor="Transparent" CssClass="formfieldtitle">gal (US)</asp:label>
	<asp:label id="HourlyUnitsLabel" style="Z-INDEX: 101; LEFT: 456px; POSITION: absolute; TOP: 72px" runat="server"
		Width="72px" BackColor="Transparent" CssClass="formfieldtitle">gal (US)</asp:label>
	</body>
</HTML>

