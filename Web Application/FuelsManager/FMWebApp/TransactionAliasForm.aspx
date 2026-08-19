<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasAssociationsPage" Src="TransactionAliasAssociationsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasStatusesPage" Src="TransactionAliasStatusesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasProductsPage" Src="TransactionAliasProductsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasEquipmentPage" Src="TransactionAliasEquipmentPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasUserDataPage" Src="TransactionAliasUserDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasFieldsPage" Src="TransactionAliasFieldsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasFieldOrderPage" Src="TransactionAliasFieldOrderPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasGeneralPage" Src="TransactionAliasGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasUnitsPage" Src="TransactionAliasUnitsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TransactionAliasFieldPlacementPage" Src="TransactionAliasFieldPlacementPage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Page  language="c#" Codebehind="TransactionAliasForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TransactionAliasForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<style type="text/css">
			.style1
			{
				 width: 340px;
			}
			.style2
			{
				 width: 340px;
			}
		</style>
        
        <script>
            function OrderClientClick() {
                showButtonGroup();
                __doPostBack("", "FIELDORDERPOSTBACK");
            }
            
            function hideButtonGroup() {
                if (window.$) {
                    $('#bottom-action-buttons').hide();
                }
            }

            function showButtonGroup() {
                if (window.$) {
                    $('#bottom-action-buttons').show();
                }
            }
        </script>
 	</HEAD>
	<body MS_POSITIONING="GridLayout" onload="javascript:LoadHiddenID();">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<script type="text/javascript">

		    function SetHelpKey(sender, e) {
		        CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
                switch(sender.get_element().getAttribute("id")){
		        case "<%= tpFieldPlacement.ClientID %>":
		            hideButtonGroup();
		            break;
                    case "<%= tpFieldOrderPage.ClientID %>":
                    OrderClientClick();
                    break;
		        default:
                    showButtonGroup();
		        }
		    }
		</script>
		<form id="Form1" method="post" runat="server" onsubmit="saveScroll()">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server" ScriptMode="Release"/>
			 <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			 <FMControls:FMLabel id="labHeader" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    Width="720px" BackColor="Transparent" CssClass="headline">Transaction Alias Configuration</FMControls:FMLabel>
                <FMControls:FMTabContainer ID="tcTransactionAliasTabs" runat="server" ActiveTabIndex="0" Style="z-index: 104;
                    position: absolute; top: 40px; left: 32px; width: 795px; height: 455px" aria-label="Transaction Alias Tabs">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/TransactionAliasGeneralPage.ascx" OnClientClick="SetHelpKey">
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasGeneralPage runat="server" ID="TransactionAliasGeneralPage">
                            </FMWebApp:TransactionAliasGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Units" ID="tpUnitsPage"  HelpKey="FMWebApp/TransactionAliasUnitsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasUnitsPage runat="server" ID="TransactionaliasUnitsPage">
                            </FMWebApp:TransactionAliasUnitsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Fields" ID="tpFieldsPage"  HelpKey="FMWebApp/TransactionAliasFieldsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasFieldsPage runat="server" ID="TransactionAliasFieldsPage">
                            </FMWebApp:TransactionAliasFieldsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserDataPage" HelpKey="FMWebApp/TransactionAliasUserDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasUserDataPage runat="server" ID="TransactionAliasUserDataPage">
                            </FMWebApp:TransactionAliasUserDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Field Order" ID="tpFieldOrderPage" OnClientClick="SetHelpKey" HelpKey="FMWebApp/TransactionAliasFieldOrderPage.ascx">
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasFieldOrderPage runat="server" ID="TransactionAliasFieldOrderPage">
                            </FMWebApp:TransactionAliasFieldOrderPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Products" ID="tpProductsPage" HelpKey="FMWebApp/TransactionAliasProductsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasProductsPage runat="server" ID="TransactionAliasProductsPage">
                            </FMWebApp:TransactionAliasProductsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Equipment" ID="tpEquipmentPage" HelpKey="FMWebApp/TransactionAliasEquipmentPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasEquipmentPage runat="server" ID="TransactionAliasEquipmentPage">
                            </FMWebApp:TransactionAliasEquipmentPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Statuses" ID="tpStatusesPage" HelpKey="FMWebApp/TransactionAliasStatusesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasStatusesPage runat="server" ID="TransactionAliasStatusesPage">
                            </FMWebApp:TransactionAliasStatusesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Associations" ID="tpAssociationsPage" HelpKey="FMWebApp/TransactionAliasAssociationsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasAssociationsPage runat="server" ID="TransactionAliasAssociationsPage">
                            </FMWebApp:TransactionAliasAssociationsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Layout" ID="tpFieldPlacement" HelpKey="" OnClientClick="SetHelpKey" Visible="False">
                        <ContentTemplate>
                            <FMWebApp:TransactionAliasFieldPlacementPage runat="server" ID="TransactionAliasFieldPlacementPage">
                            </FMWebApp:TransactionAliasFieldPlacementPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
                <table style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 550px; width:765px" id="bottom-action-buttons" role="presentation" aria-label="layout">
            		<tr><td style="float:right">
			            <table role="presentation" aria-label="layout">
			               <tr>
			                  <td><FMControls:FMLabel id="Label10" runat="server"
				                  Width="176px" ForeColor="Crimson" Height="8px" CssClass="formfieldtitle">* Denotes Required Field</FMControls:FMLabel></td>
			                  <td>&nbsp;&nbsp;</td>
							  <td><FMControls:FMButton id="New"  tabIndex="-1"
								  runat="server" Width="66px" CssClass="formfieldtitle" Text="New"></FMControls:FMButton></td>
							  <td>&nbsp;</td>
			                  <td><FMControls:FMButton id="OK" tabIndex="-1"
				                  runat="server" Text="OK" CssClass="formfieldtitle" style="min-width:75px"></FMControls:FMButton></td>
			                  <td>&nbsp;</td>
			                  <td><FMControls:FMButton id="Cancel" tabIndex="-1"
				                  runat="server" Text="Cancel" CssClass="formfieldtitle" style="min-width:75px"></FMControls:FMButton></td>
			               </tr>
			            </table>
        			</td></tr>
                </table>


			 <script type="text/javascript">
    			    var identifiertextbox = document.getElementById("tcTransactionAliasTabs_tpGeneralPage_TransactionAliasGeneralPage_Identifier");
    			    if(null != identifiertextbox)
    			    {
    			        if(identifiertextbox.attachEvent)
    			        {	
    			            identifiertextbox.attachEvent("onchange",ValidateIndentifier);
    			        }
    			    }

    			    function LoadHiddenID()
    			    {
    			        var identifiertextbox = document.getElementById("tcTransactionAliasTabs_tpGeneralPage_TransactionAliasGeneralPage_Identifier");
    			        var hiddenidtextbox = document.getElementById("HiddenID");
    			        if( null != identifiertextbox && null != hiddenidtextbox )
    			        {
    			            hiddenidtextbox.value = identifiertextbox.value;
    			        }
    			    }
				
    			    function ValidateIndentifier()
    			    {
    			        var identifiertextbox = document.getElementById("tcTransactionAliasTabs_tpGeneralPage_TransactionAliasGeneralPage_Identifier");
    			        var hiddenidtextbox = document.getElementById("HiddenID");
    			        if( null != identifiertextbox && null != hiddenidtextbox )
    			        {
    			            if( identifiertextbox.value != hiddenidtextbox.value && '' != hiddenidtextbox.value )
    			            {
    			                // Display orphaned transaction warning 
    			                if(!confirm("Trx records could be orphaned and cause ledger calc errors. Are you sure you want to rename the alias?"))
    			                {
    			                    identifiertextbox.value = hiddenidtextbox.value; 	
    			                }
    			            }
    			        }
    			    }	
             </script>
            </div>
		</form>
        <script>
            function saveScroll() {
                saveFieldDataGridScroll();
                saveUserDataGridScroll();
            }

            function restoreScroll() {
                restoreFieldDataGridScroll();
                restoreUserDataGridScroll();
            }

            window.onload = restoreScroll;

        </script>
	</body>
</HTML>
