<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="SiteSystemPage.ascx.cs"
    Inherits="FuelsManager.FMWebApp.SiteSystemPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <meta content>
    <style type="text/css">
        #systemtab .formfieldtitle {
            width: 220px;
        }

        #systemtab input.formfield {
            width: 196px;
        }

        #systemtab select.formfield {
            width: 200px;
            margin-bottom:2px;
        }

        #systemtab input.formfield.narrow {
            width: 55px;
        }

        #systemtab .box {
            border: 1px;
            border-color: darkgray;
            border-style: solid;
            margin: 10px;
            padding: 15px;
            width: 431px;
        }

        #systemtab .narrow-box {
            width: 190px;
            min-height: 84px;
            margin-top: 0;
        }

        #tcSiteTabs{
            min-width:1030px;
        }

        /* from load rack */
           .column {
				float: left;
			}
			.box span:first-child {
				font-size:15px;
			}
			.formfieldtitle {
				min-width:200px;
				margin-bottom: 1px;
			}

			input + .formfieldtitle {
				width:50px;
			}
			input.formfield {
				width:50px;
				margin-bottom: 2px;
			}
			input[type='checkbox'] + label {
				min-width:150px;
			}
			input[type='checkbox']
			{
				vertical-align: bottom;
			}
			.formfieldtitle label {
				margin-bottom:1px;
			}
    </style>
</head>
<body>
    <div id="systemtab">
        <div class="column">

            <!-- this div sets off the End of Day/Month Configuration section -->
            <div class="box">
                <FMControls:FMLabel ID="EndOfDayMonthConfigurationLabel" runat="server" CssClass="formfieldtitle">End of Day/Month Configuration</FMControls:FMLabel><br />
                <FMControls:FMCheckBox ID="InhibitEndOfDayCheckBox" TabIndex="6" runat="server" CssClass="formfieldtitle" Text="Inhibit End Of Day Operations"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="InhibitEndOfMonthCheckBox" TabIndex="7" runat="server" CssClass="formfieldtitle" Text="Inhibit End Of Month Operations"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="InhibitAutomaticPhysicalInventoryCheckBox" TabIndex="10" runat="server" CssClass="formfieldtitle" Text="Inhibit Automatic Physical Inventory"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="InhibitAutomaticMeterCloseoutCheckBox" TabIndex="11" runat="server" CssClass="formfieldtitle" Text="Inhibit Automatic Meter Closeout"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="InhibitAutomaticReportGenerationCheckBox" TabIndex="12" runat="server" CssClass="formfieldtitle" Text="Inhibit Automatic Report Generation"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="InhibitAutomaticCloseoutCheckBox" TabIndex="13" runat="server" CssClass="formfieldtitle" Text="Inhibit Automatic Closeout"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="InhibitCloseoutOnUnpostedBol" TabIndex="15" runat="server" CssClass="formfieldtitle" Text="Inhibit Closeout On Unposted BOLs" Visible="true" /><br />

                <FMControls:FMLabel ID="Fmlabel6" AssociatedControlID="EndOfDayWarningPeriodTextBox" runat="server" CssClass="formfieldtitle">End Of Day Warning Period:</FMControls:FMLabel>
                <asp:TextBox ID="EndOfDayWarningPeriodTextBox" TabIndex="16" runat="server" CssClass="formfield narrow"  MaxLength="6"></asp:TextBox>
                <FMControls:FMLabel ID="EndOfDayWaningPeriodUnitsLabel" runat="server" CssClass="formfieldtitle">minutes</FMControls:FMLabel>
            </div>
            <!-- this div sets off the SCADA Configuration section -->
            <div class="box">
                <FMControls:FMLabel ID="ScadaConfigurationLabel" runat="server" CssClass="formfieldtitle">SCADA Configuration</FMControls:FMLabel><br />

                <FMControls:FMLabel ID="Label1" AssociatedControlID="ScadaSystemTextBox" runat="server" Style="min-width: 100px; width: 100px;" CssClass="formfieldtitle">SCADA System:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="SelectSystemModeDropDownList" TabIndex="24" runat="server" Width="58px" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="SelectSystemModeDropDownList_SelectedIndexChanged"></FMControls:FMDropDownList>
                <asp:TextBox ID="ScadaSystemTextBox" TabIndex="25" runat="server" CssClass="formfield"  MaxLength="80"></asp:TextBox>
                <asp:DropDownList ID="ScadaSystemDropDownList" TabIndex="26" runat="server" CssClass="formfield"></asp:DropDownList><br />

                <FMControls:FMLabel ID="Fmlabel1" AssociatedControlID="RefreshIntervalTextBox" Style="min-width: 100px; width: 100px;" runat="server" CssClass="formfieldtitle">Refresh Interval:</FMControls:FMLabel>
                <asp:TextBox ID="RefreshIntervalTextBox" TabIndex="27" runat="server" CssClass="formfield narrow" MaxLength="6"></asp:TextBox>
                <FMControls:FMLabel ID="Fmlabel2" runat="server" CssClass="formfieldtitle">secs</FMControls:FMLabel><br />

                <FMControls:FMCheckBox ID="InhibitTankScanCheckBox" TabIndex="28" runat="server" CssClass="formfieldtitle" Text="Inhibit Tank Scan"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="InhibitTemplateGraphicsCheckBox" TabIndex="29" runat="server" CssClass="formfieldtitle" Text="Inhibit Template Graphics"></FMControls:FMCheckBox>
            </div>


        </div>

        <div class="column">

            <!-- this div sets off the Log Configuration section -->
            <div class="box">

                <FMControls:FMLabel ID="FMLabel3" runat="server" CssClass="formfieldtitle">Log & Archive Configuration</FMControls:FMLabel><br />
                <FMControls:FMLabel ID="Label7" AssociatedControlID="MaximumDaysToRetainLogsTextBox" runat="server" CssClass="formfieldtitle">Maximum Days To Retain Logs:</FMControls:FMLabel>
                <asp:TextBox ID="MaximumDaysToRetainLogsTextBox" TabIndex="30" runat="server" CssClass="formfield narrow" MaxLength="6"></asp:TextBox><br />
                <FMControls:FMLabel ID="MaximumDaysToRetainArchiveLabel" AssociatedControlID="MaximumDaysToRetainArchiveTextBox" runat="server" CssClass="formfieldtitle">Maximum Days To Retain Archives:</FMControls:FMLabel>
                <asp:TextBox ID="MaximumDaysToRetainArchiveTextBox" TabIndex="31" runat="server" CssClass="formfield narrow" MaxLength="6"></asp:TextBox><br />
                <FMControls:FMCheckBox ID="EnableDebugLoggingCheckBox" TabIndex="32" runat="server" CssClass="formfieldtitle" Text="Enable Debug Logging"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="EnableAuditLoggingCheckBox" TabIndex="33" runat="server" CssClass="formfieldtitle" Text="Enable Audit Logging"></FMControls:FMCheckBox><br />
                <FMControls:FMCheckBox ID="AutomaticallyPrintAlarmsAndEventsCheckBox" TabIndex="34" runat="server" CssClass="formfieldtitle" Text="Automatically Print Alarms &amp; Events"></FMControls:FMCheckBox><br />
                <FMControls:FMLabel ID="Label2" AssociatedControlID="AlarmAndEventPrinterDropDownList" runat="server" CssClass="formfieldtitle">Alarm & Event Printer:</FMControls:FMLabel>
                <asp:DropDownList ID="AlarmAndEventPrinterDropDownList" TabIndex="35" runat="server" CssClass="formfield"></asp:DropDownList><br />
                <br />
                <FMControls:FMLabel ID="AlarmAndEventEmailConfiguration" runat="server" CssClass="formfieldtitle">Alarm and Event E-mail Configuration</FMControls:FMLabel><br />
                <FMControls:FMLabel ID="Label3" runat="server" AssociatedControlID="MailServerTextBox" CssClass="formfieldtitle">Mail Server:</FMControls:FMLabel>
                <asp:TextBox ID="MailServerTextBox" TabIndex="38" runat="server" CssClass="formfield" MaxLength="50"></asp:TextBox><br />

                <FMControls:FMLabel ID="Fmlabel10" AssociatedControlID="MailFromTextBox" runat="server" CssClass="formfieldtitle">Mail From:</FMControls:FMLabel>
                <asp:TextBox ID="MailFromTextBox" TabIndex="39" runat="server" CssClass="formfield" MaxLength="50"></asp:TextBox><br />

                <FMControls:FMLabel ID="Label4" AssociatedControlID="MailUserNameTextBox" runat="server" CssClass="formfieldtitle">Mail User Name:</FMControls:FMLabel>
                <asp:TextBox ID="MailUserNameTextBox" TabIndex="40" runat="server" CssClass="formfield" MaxLength="50"></asp:TextBox><br />

                <FMControls:FMLabel ID="Label5" AssociatedControlID="MailPasswordTextBox" runat="server" CssClass="formfieldtitle">Mail Password:</FMControls:FMLabel>
                <asp:TextBox ID="MailPasswordTextBox" TabIndex="44" runat="server" CssClass="formfield" MaxLength="50" TextMode="Password" AutoCompleteType="None"></asp:TextBox>

                <asp:TextBox ID="InitialMailPasswordTextBox" TabIndex="45" runat="server" CssClass="formfield" ForeColor="Transparent" BorderColor="Transparent" Style="display:none"></asp:TextBox><br />

                <FMControls:FMLabel ID="Label6" AssociatedControlID="ConnectionModeDropDownList" runat="server" CssClass="formfieldtitle">Connection Mode:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="ConnectionModeDropDownList" TabIndex="46" runat="server" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="ConnectionModeDropDownList_SelectedIndexChanged">
                </FMControls:FMDropDownList><br />
                <FMControls:FMLabel ID="Label8" AssociatedControlID="DialupNameDropDownList" runat="server" CssClass="formfieldtitle">Dial-up Name:</FMControls:FMLabel>
                <asp:DropDownList ID="DialupNameDropDownList" TabIndex="48" runat="server" CssClass="formfield">
                </asp:DropDownList><br />
            </div>
                        <div class="column">

            <!-- this div sets off the Inhibit Autoloads Configuration section -->
            <div class="box narrow-box">
                <FMControls:FMLabel ID="Fmlabel11" runat="server" CssClass="formfieldtitle">Inhibit Autoload Selection for</FMControls:FMLabel>
                <FMControls:FMCheckBox ID="InhibitBOLSummaryAutoSelection" TabIndex="52" runat="server" CssClass="formfieldtitle" Text="BOL Summary" Visible="True"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="InhibitOrderSummaryAutoSelection" TabIndex="53" runat="server" CssClass="formfieldtitle" Text="Order Summary" Visible="True"></FMControls:FMCheckBox>
                <FMControls:FMCheckBox ID="InhibitSupplyOrderSummaryAutoSelection" TabIndex="55" runat="server" CssClass="formfieldtitle" Text="Supply Order Summary" Visible="True"></FMControls:FMCheckBox>
            </div>
            </div>
            <div class="column">

            <!-- this div sets off the Enterprise Query Credentials Configuration section -->
            <div class="box narrow-box">
                <FMControls:FMLabel ID="EnterpriseQueryCredentialsHeader" runat="server" CssClass="formfieldtitle">Enterprise Query Credentials</FMControls:FMLabel><br /><br />
                <FMControls:FMLabel ID="EnterpriseManagmentAdvisory1" runat="server" CssClass="formfieldtitle">Entering credentials will enable the</FMControls:FMLabel><br />
                <FMControls:FMLabel ID="EnterpriseManagmentAdvisory2" runat="server" CssClass="formfieldtitle">use of remote entity management</FMControls:FMLabel><br /><br />

                <FMControls:FMLabel ID="EnterpriseQueryUserNameLabel" AssociatedControlID="EnterpriseQueryUserNameTextbox" runat="server" CssClass="formfieldtitle" Style="width: 80px; min-width: 80px;">User Name:</FMControls:FMLabel>
                <asp:TextBox ID="EnterpriseQueryUserNameTextbox" TabIndex="60" runat="server" CssClass="formfield" Width="100px" MaxLength="50"></asp:TextBox><br />

                <FMControls:FMLabel ID="EnterpriseQueryPasswordLabel" AssociatedControlID="EnterpriseQueryPasswordTextbox" runat="server" CssClass="formfieldtitle" Style="width: 80px; min-width: 80px;">Password:</FMControls:FMLabel>
                <asp:TextBox ID="EnterpriseQueryPasswordTextbox" TabIndex="61" runat="server" CssClass="formfield" Width="100px" MaxLength="50" TextMode="Password" AutoCompleteType="None"></asp:TextBox><br />

                <FMControls:FMLabel ID="EnterpriseQuerySiteGroupLabel" AssociatedControlID="EnterpriseQuerySiteGroupTextbox" runat="server" CssClass="formfieldtitle" Style="width: 80px; min-width: 80px;">Site Group:</FMControls:FMLabel>
                <asp:TextBox ID="EnterpriseQuerySiteGroupTextbox" TabIndex="62" runat="server" CssClass="formfield" Width="100px" MaxLength="50" AutoCompleteType="None"></asp:TextBox>
            </div>
                </div>
        </div>

        <script type="text/javascript">
            var oInitialMailPasswordTextBox = document.getElementById("tcSiteTabs_tpSystemPage_SiteSystemPage_InitialMailPasswordTextBox");
            var oMailPasswordTextBox = document.getElementById("tcSiteTabs_tpSystemPage_SiteSystemPage_MailPasswordTextBox");
            if (oInitialMailPasswordTextBox != null && oMailPasswordTextBox != null)
                oMailPasswordTextBox.value = oInitialMailPasswordTextBox.value;
        </script>
    </div>
</body>
</html>
