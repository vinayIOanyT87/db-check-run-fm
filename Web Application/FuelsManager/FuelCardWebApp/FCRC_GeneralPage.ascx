<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FCRC_GeneralPage.ascx.cs" Inherits="FuelsManager.FuelCardWebApp.FCRC_GeneralPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
    <head>
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    </head>
    <body>

		<script language="jscript">
			function CompanySelect(role, companyTextBoxId)
			{
			    var companyTextBox = document.getElementById(companyTextBoxId);

			    showModalDialogFrame({
			        url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&All=false&Null=true",
			        width: 855,
			        height: 560,
			        onClose: function ()
			        {
			            if (this.returnValue != null)
			            {
			                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
			                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

			                companyTextBox.value = asciiValue1;
			                companyTextBox.title = asciiValue2;
			            }
			        }
			    });
			}
		</script>

        <script type="text/javascript">
            function CheckIfExpirationDateIsToday() {
                var today = new Date();

                var monthtextbox = window.document.getElementById("tcFCRCDetailTabs_tpGeneralPage_FCRC_GeneralPage_ExpirationDate Month");
                var daytextbox = window.document.getElementById("tcFCRCDetailTabs_tpGeneralPage_FCRC_GeneralPage_ExpirationDate Day");
                var yeartextbox = window.document.getElementById("tcFCRCDetailTabs_tpGeneralPage_FCRC_GeneralPage_ExpirationDate Year");

                var month = monthtextbox.value;
                var day = daytextbox.value;
                var year = yeartextbox.value;
                var todaysMonth = today.getMonth();
                todaysMonth++;  // This is because the month enumeration is 0 based.
                if ((month == todaysMonth) & (day == today.getDate()) & (year == today.getFullYear())) {
                    var r = confirm("Warning: the Expiration Date is today's date. If you would like to continue saving the record press OK, otherwise press Cancel to revise the Expiration Date.");
                    if (r != true) {

                        return false; // return back to the dialog
                    }
                    else {
                        return true;
                    }
                }
            }
        </script>

        <FMControls:FMLabel ID="FuelCardIdLabel"
            Style="z-index: 101; left: 16px; position: absolute; top: 21px; height: 19px; width: 105px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Fuel Card Number:</FMControls:FMLabel>

        <FMControls:FMLabel ID="FuelCardIdRequired"
            Style="z-index: 102; left: 182px; position: absolute; top: 21px; width: 12px;" runat="server"
            BackColor="Transparent" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>

        <asp:TextBox ID="FuelCardID" Style="z-index: 109; left: 195px; position: absolute; top: 21px" aria-required="true"
            runat="server" CssClass="formfield" Width="264px" MaxLength="50"></asp:TextBox>

        <FMControls:FMLabel ID="FuelCardTypeLabel"
            Style="z-index: 101; left: 495px; position: absolute; top: 21px; height: 19px; width: 90px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Fuel Card Type:</FMControls:FMLabel>

        <FMControls:FMDropDownList ID="FuelCardTypeDropDownList" Style="z-index: 111; left: 675px; position: absolute; top: 21px;"
            runat="server" CssClass="formfield" Width="120px">
        </FMControls:FMDropDownList>

        <FMControls:FMLabel ID="ProviderNameLabel"
            Style="z-index: 104; left: 16px; position: absolute; top: 51px; width: 105px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Provider Name:</FMControls:FMLabel>

        <asp:TextBox ID="ProviderName"
            Style="z-index: 110; left: 195px; position: absolute; top: 51px" runat="server"
            CssClass="formfield" Width="264px"></asp:TextBox>

        <FMControls:FMLabel ID="ProviderIDLabel"
            Style="z-index: 104; left: 495px; position: absolute; top: 51px; width: 90px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Provider ID:</FMControls:FMLabel>

        <asp:TextBox ID="ProviderID"
            Style="z-index: 110; left: 675px; position: absolute; top: 51px;" runat="server"
            CssClass="formfield" Width="264px"></asp:TextBox>

        <FMControls:FMLabel ID="ActivationStatusLabel"
            Style="z-index: 105; left: 16px; position: absolute; top: 81px" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle" Height="1.3em">Activation Status:</FMControls:FMLabel>

        <FMControls:FMDropDownList ID="StatusDropDownList" Style="z-index: 111; left: 195px; position: absolute; top: 81px;"
            runat="server" CssClass="formfield" Width="120px">
            <asp:ListItem Value="0" Selected="True">Active</asp:ListItem>
            <asp:ListItem Value="1">Inactive</asp:ListItem>
            <asp:ListItem Value="2">Cancelled</asp:ListItem>
            <asp:ListItem Value="3">Locked</asp:ListItem>
            <asp:ListItem Value="4">Lost/Stolen</asp:ListItem>
        </FMControls:FMDropDownList>

        <FMControls:FMLabel ID="ManagerLabel"
            Style="z-index: 106; left: 495px; position: absolute; top: 81px; width: 90px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Manager:</FMControls:FMLabel>

        <FMControls:FMLabel ID="ManagerRequired"
            Style="z-index: 102; left: 662px; position: absolute; top: 81px; width: 12px;" runat="server"
            BackColor="Transparent" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>

        <FMControls:FMCompanyTextBox runat="server" ID="ManagerSelect" Role="MANAGER" Style="z-index: 105; left: 675px; position: absolute; top: 81px" aria-required="true"
            TabIndex="1" CssClass="formfield" Width="240px" />

        <FMControls:FMLabel ID="InactivityPeriodLabel"
            Style="z-index: 108; left: 16px; position: absolute; top: 111px" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Inactivity Period:</FMControls:FMLabel>

        <FMControls:FMDropDownList ID="InactivityPeriodDropDownList" Style="z-index: 114; left: 195px; position: absolute; top: 111px;"
            runat="server" Sort="False" CssClass="formfield" Width="60px">
        </FMControls:FMDropDownList>

        <FMControls:FMLabel ID="MonthsLabel" Style="z-index: 115; left: 270px; position: absolute; top: 111px"
            runat="server" BackColor="Transparent" CssClass="formfieldtitle">months</FMControls:FMLabel>

        <FMControls:FMLabel ID="OwnerLabel"
            Style="z-index: 106; left: 495px; position: absolute; top: 111px; width: 90px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Owner:</FMControls:FMLabel>

        <FMControls:FMLabel ID="OwnerRequired"
            Style="z-index: 102; left: 662px; position: absolute; top: 111px; width: 12px;" runat="server"
            BackColor="Transparent" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>

        <FMControls:FMCompanyTextBox runat="server" ID="OwnerSelect" Role="OWNER" Style="z-index: 105; left: 675px; position: absolute; top: 111px" aria-required="true"
            TabIndex="1" CssClass="formfield" Width="240px" />

        <FMControls:FMLabel ID="ExpirationDateLabel"
            Style="z-index: 108; left: 16px; position: absolute; top: 141px" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Expiration Date:</FMControls:FMLabel>

        <FMControls:FMDate ID="ExpirationDate" Width="175px" CssClass="formfield" Style="left: 195px; position: absolute; top: 141px;" runat="server" MaxLength="20"></FMControls:FMDate>

        <FMControls:FMLabel ID="ShipperLabel"
            Style="z-index: 106; left: 495px; position: absolute; top: 141px; width: 90px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Shipper:</FMControls:FMLabel>

        <FMControls:FMCompanyTextBox runat="server" ID="ShipperSelect" Role="SHIPPER" Style="z-index: 105; left: 675px; position: absolute; top: 141px"
            TabIndex="1" CssClass="formfield" Width="240px" />

        <FMControls:FMLabel ID="PINLabel"
            Style="z-index: 106; left: 16px; position: absolute; top: 171px; width: 105px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">PIN:</FMControls:FMLabel>

        <FMControls:FMTextBox runat="server" ID="PIN" CssClass="formfield" Style="z-index: 105; left: 195px; position: absolute; top: 171px" Width="275px" TextMode="Password"></FMControls:FMTextBox>

        <FMControls:FMLabel ID="BillToLabel"
            Style="z-index: 106; left: 495px; position: absolute; top: 171px; width: 90px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Bill To:</FMControls:FMLabel>

        <FMControls:FMCompanyTextBox runat="server" ID="BillToSelect" Role="CUSTOMER_BILLTO" Style="z-index: 105; left: 675px; position: absolute; top: 171px"
            CssClass="formfield" Width="240px" />

        <FMControls:FMLabel ID="ConfirmPINLabel"
            Style="z-index: 106; left: 16px; position: absolute; top: 201px; width: 105px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Confirm PIN:</FMControls:FMLabel>

        <FMControls:FMTextBox runat="server" ID="ConfirmPIN" CssClass="formfield" Style="z-index: 105; left: 195px; position: absolute; top: 201px" Width="275px" TextMode="Password"></FMControls:FMTextBox>

        <FMControls:FMLabel ID="ShipToLabel"
            Style="z-index: 106; left: 495px; position: absolute; top: 201px; width: 90px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Ship To:</FMControls:FMLabel>

        <FMControls:FMLabel ID="ShipToRequired"
            Style="z-index: 102; left: 662px; position: absolute; top: 201px; width: 12px;" runat="server"
            BackColor="Transparent" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>

        <FMControls:FMCompanyTextBox runat="server" ID="ShipToSelect" Role="CUSTOMER_SHIPTO" Style="z-index: 105; left: 675px; position: absolute; top: 201px" aria-required="true"
            TabIndex="1" CssClass="formfield" Width="240px" />

        <FMControls:FMLabel ID="TransientCardFlagLabel"
            Style="z-index: 106; left: 16px; position: absolute; top: 231px; width: 105px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Transient Card:</FMControls:FMLabel>

        <FMControls:FMCheckBox ID="TransientCardFlag" runat="server" CssClass="formfieldtitle" Style="z-index: 105; left: 192px; position: absolute; top: 231px" BackColor="Transparent" Text=""></FMControls:FMCheckBox>
        <FMControls:FMLabel ID="HiddenLabel"
            Style="z-index: 106; left: 495px; position: absolute; top: 231px; width: 90px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Hidden:</FMControls:FMLabel>
        <FMControls:FMCheckBox ID="HiddenCheckBox" TextAlign="Left" Text="" CssClass="formfieldtitle" runat="server" Style="z-index: 106; left: 671px; position: absolute; top: 231px; width: 90px;"></FMControls:FMCheckBox>
        <FMControls:FMLabel ID="NotesLabel" Style="z-index: 106; left: 16px; position: absolute; top: 261px; width: 105px;" runat="server"
            BackColor="Transparent" CssClass="formfieldtitle">Notes:</FMControls:FMLabel>
        <FMControls:FMTextBox ID="Notes"
            Style="position: absolute; z-index: 106; left: 195px; top: 261px; width: 720px;" runat="server"
            CssClass="formfield" MaxLength="1024" Height="100px" TextMode="MultiLine" />
	</body>
</html>

