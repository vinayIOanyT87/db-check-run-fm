<%@ Register TagPrefix="FMWebApp" TagName="CompanyGroupsPage" Src="CompanyGroupsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyUserDataPage" Src="CompanyUserDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyShipperPage" Src="CompanyShipperPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyOwnerPage" Src="CompanyOwnerPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyManagerPage" Src="CompanyManagerPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyAccessSchedulePage" Src="CompanyAccessSchedulePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyGeneralPage" Src="CompanyGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyContactsPage" Src="CompanyContactsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyCustomerShipToPage" Src="CompanyCustomerShipToPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyCustomerBillToPage" Src="CompanyCustomerBillToPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyCertificatesAndPermitsPage" Src="CompanyCertificatesAndPermitsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyCarrierPage" Src="CompanyCarrierPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanySupplierPage" Src="CompanySupplierPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyNotesPage" Src="CompanyNotesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyEquipmentPage" Src="CompanyEquipmentPage.ascx" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Page language="c#" Codebehind="CompanyForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanyForm"  %>
<!DOCTYPE html> 
<HTML>
	<head runat="server">
		<title></title>
		<base target="_self">
        <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
        <meta name="CODE_LANGUAGE" content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <script type="text/javascript">
             <!--

    function CheckIfExpirationDateIsToday() {
        var today = new Date();

        var monthtextbox = window.document.getElementById("tcCompanyTabs_tpGeneralPage_CompanyGeneralPage_ExpirationDate Month");
        var daytextbox = window.document.getElementById("tcCompanyTabs_tpGeneralPage_CompanyGeneralPage_ExpirationDate Day");
        var yeartextbox = window.document.getElementById("tcCompanyTabs_tpGeneralPage_CompanyGeneralPage_ExpirationDate Year");

        var month = monthtextbox.value;
        var day = daytextbox.value;
        var year = yeartextbox.value;
        var todaysMonth = today.getMonth();

        todaysMonth++;  // This is because the month enumeration is 0 based.
        var testYear = today.getFullYear();
        if (year.length == 2) {
            testYear = testYear.toString().substring(2);
        }

        if (month == '' && day == '' && year == '') {
            monthtextbox.value = todaysMonth;
            daytextbox.value = today.getDate();
            yeartextbox.value = testYear;

            month = monthtextbox.value;
            day = daytextbox.value;
            year = yeartextbox.value;
        }

        if (isNaN(month) || isNaN(day) || isNaN(year)
            || !(month > 0 && month < 13) || !(day > 0 && day < 32)
            || !(year.length == 2 || year.length == 4)) {
            alert("Expiration Date has in invalid date format.  Please enter date in correct format.");
            return false;
        }

        if ((month == todaysMonth) & (day == today.getDate()) & (year == testYear)) {
            var r = confirm("Warning: Expiration Date is set to the current date. To change the Expiration Date, click Cancel. To continue, click OK.");
            if (r != true) {

                return false; // return back to the dialog
            }
            else {
                return true;
            }
        }
    }
            //-->
        </script>
	</head>
	<body tabindex="-1">
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" rel="stylesheet" type="text/css" />
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
		<script type="text/javascript">
            var theMoment = new Date();
            var theDisplacement = (theMoment.getTimezoneOffset() / 60);
            document.cookie = "Displacement=" + theDisplacement;
            function SetHelpKey(sender, e) {
                CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
            }

		</script>
		<form id="CompanyForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server" />
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle"></asp:Image>
                <FMControls:FMLabel ID="CompanyTitleLabel"
                    Style="z-index: 102; left: 8px; position: absolute; top: 8px; width: 600px;" runat="server"
                    BackColor="Transparent" CssClass="headline">Company Configuration</FMControls:FMLabel>
                <FMControls:FMTabContainer ID="tcCompanyTabs" runat="server" ActiveTabIndex="0"
                    Style="z-index: 104; position: absolute; top: 35px; left: 12px; width: 780px; height: 440px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/CompanyGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyGeneralPage runat="server" ID="CompanyGeneralPage"></FMWebApp:CompanyGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Notes" ID="tpEquipmentPage" HelpKey="FMWebApp/CompanyEquipmentPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyEquipmentPage runat="server" ID="CompanyEquipmentPage"></FMWebApp:CompanyEquipmentPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Contacts" ID="tpContactsPage" HelpKey="FMWebApp/CompanyContactsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyContactsPage runat="server" ID="CompanyContactsPage"></FMWebApp:CompanyContactsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="tpCustomerBillToPage" HelpKey="FMWebApp/CompanyCustomerBillToPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyCustomerBillToPage runat="server" ID="CompanyCustomerBillToPage"></FMWebApp:CompanyCustomerBillToPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="tpCarrierPage" HelpKey="FMWebApp/CompanyCarrierPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyCarrierPage runat="server" ID="CompanyCarrierPage"></FMWebApp:CompanyCarrierPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="tpManagerPage" HelpKey="FMWebApp/CompanyManagerPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyManagerPage runat="server" ID="CompanyManagerPage"></FMWebApp:CompanyManagerPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="tpOwnerPage" HelpKey="FMWebApp/CompanyOwnerPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyOwnerPage runat="server" ID="CompanyOwnerPage"></FMWebApp:CompanyOwnerPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="tpShipperPage" HelpKey="FMWebApp/CompanyEquipmentPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyShipperPage runat="server" ID="CompanyShipperPage"></FMWebApp:CompanyShipperPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="tpCustomerShipToPage" HelpKey="FMWebApp/CompanyShipperPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyCustomerShipToPage runat="server" ID="CompanyCustomerShipToPage"></FMWebApp:CompanyCustomerShipToPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TabPanel1" ID="tpSupplierPage" HelpKey="FMWebApp/CompanySupplierPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanySupplierPage runat="server" ID="CompanySupplierPage"></FMWebApp:CompanySupplierPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Access Schedule" ID="tpAccessSchedulePage" HelpKey="FMWebApp/CompanyAccessSchedulePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyAccessSchedulePage runat="server" ID="CompanyAccessSchedulePage"></FMWebApp:CompanyAccessSchedulePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Certificates &amp; Permits" ID="tpCertificatesAndPermitsPage" HelpKey="FMWebApp/CompanyCertificatesAndPermitsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyCertificatesAndPermitsPage runat="server" ID="CompanyCertificatesAndPermitsPage"></FMWebApp:CompanyCertificatesAndPermitsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Groups" ID="tpGroupsPage" HelpKey="FMWebApp/CompanyGroupsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyGroupsPage runat="server" ID="CompanyGroupsPage"></FMWebApp:CompanyGroupsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserDataPage" HelpKey="FMWebApp/CompanyUserDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyUserDataPage runat="server" ID="CompanyUserDataPage"></FMWebApp:CompanyUserDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Notes" ID="tpNotesPage" HelpKey="FMWebApp/CompanyNotesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyNotesPage runat="server" ID="CompanyNotesPage"></FMWebApp:CompanyNotesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
                <table width="700px" style="z-index: 200; left: 32px; position: absolute; top: 550px">
                    <tr>
                        <td align="right">
                            <table>
                                <tr>
                                    <td>
                                        <FMControls:FMLabel ID="DenotesRequiredFieldLabel" runat="server"
                                            Width="176px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel></td>
                                    <td>&nbsp;&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="New" TabIndex="100"
                                            runat="server" Width="66px" CssClass="formfieldtitle" OnClientClick="return CheckIfExpirationDateIsToday()" Text="New"></FMControls:FMButton></td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="OK" TabIndex="101"
                                            runat="server" Width="66px" CssClass="formfieldtitle" OnClientClick="return CheckIfExpirationDateIsToday()" Text="OK"></FMControls:FMButton></td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="Cancel" TabIndex="102"
                                            runat="server" Width="66px" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel"></FMControls:FMButton></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
    </form>
	<script type="text/javascript">
        var okButton = document.getElementById("OK");
        if (!okButton.disabled)
            okButton.setActive();
	</script>
    </body>
</HTML>
