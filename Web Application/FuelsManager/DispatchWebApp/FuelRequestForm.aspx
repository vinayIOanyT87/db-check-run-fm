<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FuelRequestForm.aspx.cs" Inherits="FuelsManager.DispatchWebApp.FuelRequestForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="FMWebApp" TagName="FuelRequestServiceRequestPage" Src="FuelRequestServiceRequestPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="FuelRequestFillStandServiceRequestPage" Src="FuelRequestFillStandServiceRequestPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="FuelRequestDetailPage" Src="FuelRequestDetailPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="FuelRequestAdditionalDataPage" Src="FuelRequestAdditionalDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="FuelRequestContactPage" Src="FuelRequestContactPage.ascx" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <base target="_self" />
    <title></title>
</head>
<body>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
    <!-- This style is here to resolve an issue with the drop down for the FMComboBox appearing out of position -->
    <style type="text/css">
        .comboBoxInPanel
        {
            position: relative;
        }

            .comboBoxInPanel ul
            {
                position: absolute !important;
                left: 2px !important;
                top: 23px !important;
            }
    </style>
    <form id="fuelRequestForm" runat="server">
        <script type="text/javascript">
            function ShowConfirmationDialogAndClickButton(confirmMessage, controlName) {
                // Without setTimeout(), the controls on the page are not rendered before the dialog displays. 
                // In other words, the dialog pops up over an empty form.
                setTimeout(function () {
                    // If the user says OK, then click the Hidden OK Button
                    if (confirm(confirmMessage)) {
                        document.getElementById(controlName).click();
                    }
                }, 0);
            }

            function ShowAlertDialog(alertMessage) {
                setTimeout(function () {
                    alert(alertMessage);
                }, 0);
            }

            function ShowAlertAndClose(alertMessage) {
                setTimeout(function () {
                    alert(alertMessage);
                    window.close();
                }, 0);
            }

            function SetFocus(controlName, tabIndex) {
                setTimeout(function () {
                    //Find the tab control and change the tab index
                    var tabContainer = $find("<%=tcFuelRequest.ID%>");
                    if (tabContainer != null) {
                        tabContainer.set_activeTabIndex(parseInt(tabIndex));

                        // Focus on the control 
                        var controlToFocus = document.getElementById(controlName);

                        if (controlToFocus != null) {
                            controlToFocus.focus();
                        }
                    }
                }, 5); //It seems that we need a slightly longer wait period or otherwise focus may not get set
            }
        </script>
		<div style="position: relative;">
			<FMControls:FMLabel ID="titleLabel" Style="z-index: 103; left: 8px; position: absolute;
				top: 8px" runat="server" CssClass="headline" Width="500px" BackColor="Transparent"
				Text="Fuel Request" />
		</div>
        <div>
            <asp:ScriptManager ID="theScriptManager" runat="server" />
            <FMControls:FMTabContainer ID="tcFuelRequest" runat="server" Style="z-index: 105; left: 32px; position: absolute; top: 35px; width: 600px; height: 350px">
                <ajaxToolkit:TabPanel runat="server" HeaderText="Service Request" ID="tpServiceRequestPage">
                    <ContentTemplate>
                        <FMWebApp:FuelRequestServiceRequestPage runat="server" ID="FuelRequestServiceRequestPage"></FMWebApp:FuelRequestServiceRequestPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Service Request" ID="tpFillStandServiceRequestPage">
                    <ContentTemplate>
                        <FMWebApp:FuelRequestFillStandServiceRequestPage runat="server" ID="FuelRequestFillStandServiceRequestPage"></FMWebApp:FuelRequestFillStandServiceRequestPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Detail" ID="tpDetailPage">
                    <ContentTemplate>
                        <FMWebApp:FuelRequestDetailPage runat="server" ID="FuelRequestDetailPage"></FMWebApp:FuelRequestDetailPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Additional Data" ID="tpAdditionalDataPage">
                    <ContentTemplate>
                        <FMWebApp:FuelRequestAdditionalDataPage runat="server" ID="FuelRequestAdditionalDataPage"></FMWebApp:FuelRequestAdditionalDataPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Contact" ID="tpContactPage">
                    <ContentTemplate>
                        <FMWebApp:FuelRequestContactPage runat="server" ID="FuelRequestContactPage"></FMWebApp:FuelRequestContactPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </FMControls:FMTabContainer>
            <table style="z-index: 104; left: 32px; position: absolute; top: 452px; width: 300px">
                <tr>
                    <td style="padding-left: 75px;">
                        <FMControls:FMButton ID="OKButton" TabIndex="100"
                            runat="server" Width="66px" CssClass="formfieldtitle" Text="OK" OnClick="OkButtonClick" />
                    </td>
                    <td>
                        <FMControls:FMButton ID="HiddenOKButton" Style="display: none"
                            runat="server" OnClick="HiddenOkButtonClick" />
                    </td>
                    <td style="padding-left: 200px;">
                        <FMControls:FMButton ID="CancelButton" TabIndex="101"
                            runat="server" Width="66px" CssClass="formfieldtitle" Text="Cancel" OnClientClick="window.close(); return false;"/></td>
                    <td style="padding-left: 200px; padding-right: 75px;">
                        <FMControls:FMButton ID="ApplyButton" TabIndex="102"
                            runat="server" CssClass="formfieldtitle" Text="Apply" Width="66px" OnClick="ApplyButtonClick" />
                    </td>
                    <td>
                        <FMControls:FMButton ID="HiddenApplyButton" Style="display: none"
                            runat="server" OnClick="HiddenApplyButtonClick" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
