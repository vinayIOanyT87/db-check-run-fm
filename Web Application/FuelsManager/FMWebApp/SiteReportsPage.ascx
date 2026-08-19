<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="SiteReportsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteReportsPage" %>
<html>
<head>
    <title>Site Leak Detection Configuration</title>
    <style>
        #siteReportsTab .formfieldtitle {
            min-width: 240px;
            padding: 5px;
        }

        #siteReportsTab .formfieldtitle {
            width: 240px;
        }

        #siteReportsTab input.formfield {
            width: 256px;
        }

        #siteReportsTab select.formfield {
            width: 260px;
            margin-bottom: 2px;
        }

        .formfieldtitle {
            min-width: 200px;
            margin-bottom: 1px;
        }

        input + .formfieldtitle {
            width: 50px;
        }

        input.formfield {
            width: 50px;
            margin-bottom: 2px;
        }

        input[type='checkbox'] + label {
            min-width: 150px;
        }

        input[type='checkbox'] {
            vertical-align: bottom;
        }

        .formfieldtitle label {
            margin-bottom: 1px;
        }
        .reportGroup {
            margin-top: 20px;
        }
        #siteReportsTab .column
        {
            padding-bottom: 20px;
        }
        
    </style>
</head>
<body>
    <div id="siteReportsTab">
        <!-- this div sets off the Reports Configuration section -->
        <div class="reportGroup">
             <div>
                 <FMControls:FMLabel ID="Fmlabel8" AssociatedControlID="ReportDirectoryTextBox" runat="server" CssClass="formfieldtitle">Report Directory:</FMControls:FMLabel>
                 <asp:TextBox ID="ReportDirectoryTextBox" TabIndex="1" runat="server" CssClass="formfield" MaxLength="80"></asp:TextBox>
             </div>
        </div>
        <div class="reportGroup">
            <div>
                <FMControls:FMLabel ID="FMLabel5" AssociatedControlID="MeterRecReportDropDownList" runat="server" CssClass="formfieldtitle">Meter Reconciliation Report:</FMControls:FMLabel>
                <asp:DropDownList ID="MeterRecReportDropDownList" TabIndex="2" runat="server" CssClass="formfield"></asp:DropDownList>
            </div>
        </div>
        <div class="reportGroup">
				<div>
				  <FMControls:FMCheckbox id="EnableAutomaticMovementTicketPrintingCheckBox" Text="Enable Automatic Movement Ticket Printing" BackColor="Transparent"
												 CssClass="formfieldtitle" runat="server" tabIndex="3">
				  </FMControls:FMCheckbox>
				</div>
				<br />
            <div>
                <FMControls:FMLabel ID="FMLabel9" AssociatedControlID="MovementTicketReportList" runat="server" CssClass="formfieldtitle">Movement Ticket Report:</FMControls:FMLabel>
                <asp:DropDownList ID="MovementTicketReportList" TabIndex="4" runat="server" CssClass="formfield"></asp:DropDownList>
            </div>
            <div>
                <FMControls:FMLabel ID="FMLabel12" AssociatedControlID="MovementTicketPrinter" runat="server" CssClass="formfieldtitle">Movement Ticket Printer:</FMControls:FMLabel>
                <asp:DropDownList ID="MovementTicketPrinter" TabIndex="5" runat="server" CssClass="formfield"></asp:DropDownList>
            </div>

				<br />
				<div>
				  <FMControls:FMCheckbox id="EnableMovementTicketArchivingCheckBox" Text="Enable Movement Ticket PDF Archiving" BackColor="Transparent"
												 CssClass="formfieldtitle" runat="server" tabIndex="6">
				  </FMControls:FMCheckbox>
				</div>
				<br />
           <div>
                <FMControls:FMLabel ID="FMLabel3" AssociatedControlID="Mvmt_Ticket_Archive_Directory" runat="server" CssClass="formfieldtitle">Movement Ticket Archive Directory:</FMControls:FMLabel>
                <asp:TextBox ID="Mvmt_Ticket_Archive_Directory" TabIndex="7" runat="server" CssClass="formfield" MaxLength="255"></asp:TextBox>
           </div>

           <div>
                <FMControls:FMLabel ID="FMLabel4" AssociatedControlID="Mvmt_Ticket_Export_FileName" runat="server" CssClass="formfieldtitle">Movement Ticket Export Default File Name:</FMControls:FMLabel>
                <asp:TextBox ID="Mvmt_Ticket_Export_FileName" TabIndex="8" runat="server" CssClass="formfield" MaxLength="80"></asp:TextBox>
           </div>
        </div>
        <div class="reportGroup">
            <div>
                <FMControls:FMLabel ID="LeakReportLabel" AssociatedControlID="LeakReportList" runat="server" CssClass="formfieldtitle">Leak Detection Report:</FMControls:FMLabel>
                <asp:DropDownList ID="LeakReportList" TabIndex="9" runat="server" CssClass="formfield"></asp:DropDownList>
            </div>
            <div>
                <FMControls:FMLabel ID="ReportPrinterLabel" AssociatedControlID="LeakReportPrinterDropDownList" runat="server" CssClass="formfieldtitle">Leak Detection Report Printer:</FMControls:FMLabel>
                <asp:DropDownList ID="LeakReportPrinterDropDownList" TabIndex="10" runat="server" CssClass="formfield"></asp:DropDownList>
            </div>
        </div>
        <div class="reportGroup">
            <div>
                <FMControls:FMCheckBox ID="ManageReportsCheckBox" TabIndex="11" runat="server" CssClass="formfieldtitle" Text="Manage Reports" onclick="managedReportCheckBoxToggled();"></FMControls:FMCheckBox>
            </div>
				<br />
            <div>
                <FMControls:FMLabel ID="Fmlabel7" AssociatedControlID="ManagedReportDirectoryTextBox" runat="server" CssClass="formfieldtitle">Managed Report Directory:</FMControls:FMLabel>
                 <asp:TextBox ID="ManagedReportDirectoryTextBox" TabIndex="12" runat="server" CssClass="formfield" MaxLength="80"></asp:TextBox>
             </div>
         </div>

        <div class="reportGroup">
            <div>
                <FMControls:FMLabel ID="CloseoutTimeLabel" AssociatedControlID="CloseoutTimeControl" runat="server" CssClass="formfieldtitle">IM Closeout Report Time:</FMControls:FMLabel>
                <FMControls:FMTime ID="CloseoutTimeControl" TabIndex="13" runat="server" CssClass="formfield"></FMControls:FMTime>
            </div>
        </div>

        <!-- this div sets off the Point Group CSV Export section -->
			<br />
         <FMControls:FMLabel ID="Fmlabel1" AssociatedControlID="PG_Export_Archive_Directory" runat="server" CssClass="formfieldtitle">Point Group Export Directory:</FMControls:FMLabel>
         <asp:TextBox ID="PG_Export_Archive_Directory" TabIndex="14" runat="server" CssClass="formfield" MaxLength="255"></asp:TextBox>
			<br />
         <FMControls:FMLabel ID="Fmlabel2" AssociatedControlID="PG_Export_Default_FileName" runat="server" CssClass="formfieldtitle">Point Group Export Default File Name:</FMControls:FMLabel>
         <asp:TextBox ID="PG_Export_Default_FileName" TabIndex="15" runat="server" CssClass="formfield" MaxLength="80"></asp:TextBox>
    </div>
    <script>
        function managedReportCheckBoxToggled() {

            if (document.getElementById('<%=ManageReportsCheckBox.ClientID%>').checked) {
                /*$('#*/<%=ReportDirectoryTextBox.ClientID%>/*').attr('disabled', 'disabled');*/
                $('#<%=ManagedReportDirectoryTextBox.ClientID%>').removeAttr('disabled');
            } else {
               /* $('#*/<%=ReportDirectoryTextBox.ClientID%>/*').removeAttr('disabled');*/
                $('#<%=ManagedReportDirectoryTextBox.ClientID%>').attr('disabled','disabled');
            }
        }

    </script>
</body>
</html>
