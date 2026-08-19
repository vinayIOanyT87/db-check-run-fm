<%@ Register TagPrefix="FMWebApp" TagName="ProfileGeneralSettingPage" Src="ProfileGeneralSettingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProfileFuelingEquipSettingPage" Src="ProfileFuelingEquipSettingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProfileValidationRuleSettingPage" Src="ProfileValidationRuleSettingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProfileDCUSettingPage" Src="ProfileDCUSettingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProfileTransactionSettingPage" Src="ProfileTransactionSettingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProfileAnalogSettingPage" Src="ProfileAnalogSettingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProfileOpsConfigSettingPage" Src="ProfileOpsConfigSettingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProfileCommunicationSettingPage" Src="ProfileCommunicationSettingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProfilePrinterSettingPage" Src="ProfilePrinterSettingPage.ascx" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProfileConfigurationForm.aspx.cs"
	Inherits="FuelsManager.FMWebApp.ProfileConfigurationForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title></title>
	<base target="_self" />
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
	<meta name="CODE_LANGUAGE" content="C#" />
	<meta name="vs_defaultClientScript" content="JavaScript" />
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
</head>
<body>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<form runat="server">
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
	<div id="pageContent" style="position: absolute">
		<asp:ScriptManager ID="ScriptManager" runat="server" />
		<FMControls:FMLabel ID="TitleLabel" Style="z-index: 102; left: 8px; position: absolute;
			top: 8px" runat="server" BackColor="Transparent" Width="296px" CssClass="headline">Profile Configuration</FMControls:FMLabel>
		<asp:Image ID="FadeImage" Style="z-index: 99; left: 0px; position: absolute; top: 0px"
			runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle">
		</asp:Image>
		<FMControls:FMTabContainer ID="tcProfileConfigTabs" runat="server" ActiveTabIndex="0"
			Style="z-index: 104; position: absolute; top: 40px; left: 32px; width: 780px;
			height: 361px">
			<ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralSettingsPage">
				<ContentTemplate>
					<FMWebApp:ProfileGeneralSettingPage runat="server" ID="ProfileGeneralSettingPage">
					</FMWebApp:ProfileGeneralSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel runat="server" HeaderText="Fueling Equipment" ID="tpFuelingEquipSettingsPage">
				<ContentTemplate>
					<FMWebApp:ProfileFuelingEquipSettingPage runat="server" ID="ProfileFuelingEquipSettingPage">
					</FMWebApp:ProfileFuelingEquipSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel runat="server" HeaderText="Validation Rules" ID="tpValidationRuleSettingsPage">
				<ContentTemplate>
					<FMWebApp:ProfileValidationRuleSettingPage runat="server" ID="ProfileValidationRuleSettingPage">
					</FMWebApp:ProfileValidationRuleSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel runat="server" HeaderText="Transaction" ID="tpTransactionSettingsPage">
				<ContentTemplate>
					<FMWebApp:ProfileTransactionSettingPage runat="server" ID="ProfileTransactionSettingPage">
					</FMWebApp:ProfileTransactionSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel runat="server" HeaderText="Data Capture Unit" ID="tpDCUSettingsPage">
				<ContentTemplate>
					<FMWebApp:ProfileDCUSettingPage runat="server" ID="ProfileDCUSettingPage"></FMWebApp:ProfileDCUSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel runat="server" HeaderText="Analog Input" ID="tpAnalogSettingsPage">
				<ContentTemplate>
					<FMWebApp:ProfileAnalogSettingPage runat="server" ID="ProfileAnalogSettingPage">
					</FMWebApp:ProfileAnalogSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel runat="server" HeaderText="Operational Configuration" ID="tpOpsConfigSettingPage">
				<ContentTemplate>
					<FMWebApp:ProfileOpsConfigSettingPage runat="server" ID="ProfileOpsConfigSettingPage">
					</FMWebApp:ProfileOpsConfigSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel runat="server" HeaderText="Communication" ID="tpCommunicationSettingPage">
				<ContentTemplate>
					<FMWebApp:ProfileCommunicationSettingPage runat="server" ID="ProfileCommunicationSettingPage">
					</FMWebApp:ProfileCommunicationSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
			<ajaxToolkit:TabPanel runat="server" HeaderText="Printer" ID="tpPrinterSettingPage">
				<ContentTemplate>
					<FMWebApp:ProfilePrinterSettingPage runat="server" ID="ProfilePrinterSettingPage">
					</FMWebApp:ProfilePrinterSettingPage>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
		</FMControls:FMTabContainer>
		<table style="z-index: 104; left: 32px; position: absolute; top: 640px; width: 780px">
			<tr>
				<td style="float: right">
					<table>
						<tr>
							<td>
								<FMControls:FMLabel ID="DenotesRequiredFieldLabel" runat="server" Width="176px" CssClass="formfieldtitle"
									Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
							</td>
							<td>
								&nbsp;&nbsp;
							</td>
							<td>
								<FMControls:FMButton ID="NewBtn" TabIndex="100" runat="server" Width="85px" CssClass="formfieldtitle"
									Text="New"></FMControls:FMButton>
							</td>
							<td>
								&nbsp;
							</td>
							<td>
								<FMControls:FMButton ID="OkBtn" TabIndex="101" runat="server" Width="85px" CssClass="formfieldtitle"
									Text="OK"></FMControls:FMButton>
							</td>
							<td>
								&nbsp;
							</td>
							<td>
								<FMControls:FMButton ID="CancelBtn" TabIndex="102" runat="server" Width="85px" CssClass="formfieldtitle"
									Text="Cancel"></FMControls:FMButton>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	</form>
</body>
</html>
