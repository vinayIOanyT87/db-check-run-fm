<%@ Register TagPrefix="FMWebApp" TagName="LoadArmComponentPage" Src="LoadArmComponentPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="LoadArmInjectorPage" Src="LoadArmInjectorPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="LoadArmRecipePage" Src="LoadArmRecipePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="LoadArmGeneralPage" Src="LoadArmGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="LoadArmExternalComponentPage" Src="LoadArmExternalComponentPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="LoadArmFlowControlledAdditivePage" Src="LoadArmFlowControlledAdditivePage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="LoadArmForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.LoadArmForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<SCRIPT>
			function InputsButton_Click(index)
			{
				showModalDialogFrame({
				    url: "../FMWebApp/ExternalComponentInputForm.aspx?Index=" + index,
				    width: 725,
				    height: 690,
				    title: "External Component Input"
				});
			}

			function PermissivesButton_Click(mode,index)
			{
				showModalDialogFrame({
				    url: "../FMWebApp/PermissivesForm.aspx?Mode=" + mode + "&Index=" + index,
				    width: 725,
				    height: 690,
				    title: "Permissives Select"
				});
			}

            function OKButton_Click() {
                alert("Please make sure that both stations have the same Enable Dynamic Recipes setting before configuring a load arm as a Swing Arm.");
                return false;
            }
        </SCRIPT>
         <style type="text/css"> 
        .comboBoxInGrid
        { 
            position: relative; 
        } 
        .comboBoxInGrid ul 
        { 
            position: absolute !important; 
            left: 2px !important; 
            top: 22px !important; 
        } 
		.tabbedPages .ajax__tab_inner, .tabbedPages .grayedFMTab  {
			width: 95px !important;
		}
		table.tabbedPages {
			width: 97px !important;
			display: block;
		}
		.tabbedPages .grayedFMTab, .ajax__tab_active {
			height: 30px !important;
		}
         </style>
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
		<form method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <asp:ScriptManager ID="ScriptManager1" runat="server" />
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<asp:Label ID="ArmLabel" Style="z-index: 107; left: 32px; position: absolute; top: 8px" runat="server"
				CssClass="headline" BackColor="Transparent">Station: Rack 1 Arm: 1</asp:Label>
			<FMControls:FMTabContainer ID="tcLoadArmTabs" runat="server" ActiveTabIndex="0"
				Style="z-index: 101; position: absolute; top: 40px; left: 32px; width: 700px; height: 455px" aria-label="Load Arm Tabs">
				<ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage">
					<ContentTemplate>
						<FMWebApp:LoadArmGeneralPage ID="LoadArmGeneralPage" runat="server"></FMWebApp:LoadArmGeneralPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
				<ajaxToolkit:TabPanel runat="server" HeaderText="Recipes" ID="tpRecipePage">
					<ContentTemplate>
						<FMWebApp:LoadArmRecipePage ID="LoadArmRecipePage" runat="server"></FMWebApp:LoadArmRecipePage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
				<ajaxToolkit:TabPanel runat="server" HeaderText="Injectors" ID="tpInjectorPage">
					<ContentTemplate>
						<FMWebApp:LoadArmInjectorPage ID="LoadArmInjectorPage" runat="server"></FMWebApp:LoadArmInjectorPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
				<ajaxToolkit:TabPanel runat="server" HeaderText="Components" ID="tpComponentPage">
					<ContentTemplate>
						<FMWebApp:LoadArmComponentPage ID="LoadArmComponentPage" runat="server"></FMWebApp:LoadArmComponentPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
				<ajaxToolkit:TabPanel runat="server" HeaderText="External Components" ID="tpExternalComponentPage">
					<ContentTemplate>
						<FMWebApp:LoadArmExternalComponentPage ID="LoadArmExternalComponentPage" runat="server"></FMWebApp:LoadArmExternalComponentPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
				<ajaxToolkit:TabPanel runat="server" HeaderText="Flow Controlled Additives" ID="tpFlowControlledAdditivePage">
					<ContentTemplate>
						<FMWebApp:LoadArmFlowControlledAdditivePage ID="FlowControlledAdditivePage" runat="server"></FMWebApp:LoadArmFlowControlledAdditivePage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
			</FMControls:FMTabContainer>
			<table style="z-index: 104; left: 32px; position: absolute; top: 495px; width: 700px" role="presentation" aria-label="layout">
				<tr>
					<td style="float: right">
						<table role="presentation" aria-label="layout">
							<tr>
								<td>
									<FMControls:FMLabel ID="Label10" runat="server"
										Width="176px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel></td>
								<td>&nbsp;&nbsp;</td>
								<td>
									<FMControls:FMButton ID="OK" TabIndex="8"
										runat="server" Width="66px" Text="OK" CssClass="formfieldtitle" ClientIDMode="Static"></FMControls:FMButton></td>
								<td>&nbsp;</td>
								<td>
									<FMControls:FMButton ID="Cancel" TabIndex="9"
										runat="server" Width="66px" Text="Cancel" CssClass="formfieldtitle" ClientIDMode="Static"></FMControls:FMButton></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>

