<%@ Register TagPrefix="FMWebApp" TagName="MobileDeviceGeneralSettingPage" Src="MobileDeviceGeneralSettingPage.ascx" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MobileDeviceConfigurationPage.aspx.cs" Inherits="FuelsManager.FMWebApp.MobileDeviceConfigurationPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
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
				top: 8px" runat="server" BackColor="Transparent" Width="296px" CssClass="headline">Mobile Device Configuration</FMControls:FMLabel>
			<asp:Image ID="FadeImage" Style="z-index: 99; left: 0px; position: absolute; top: 0px"
				runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle">
			</asp:Image>
			<FMControls:FMTabContainer ID="tcMobileDeviceConfigTabs" runat="server" ActiveTabIndex="0"
				Style="z-index: 104; position: absolute; top: 40px; left: 32px; width: 780px;
				height: 361px">
				<ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralSettingsPage">
					<ContentTemplate>
						<FMWebApp:MobileDeviceGeneralSettingPage runat="server" ID="MobileDeviceGeneralSettingPage">
						</FMWebApp:MobileDeviceGeneralSettingPage>
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
										Text="New" onclick="NewButtonOnClick"></FMControls:FMButton>
								</td>
								<td>
									&nbsp;
								</td>
								<td>
									<FMControls:FMButton ID="OkBtn" TabIndex="101" runat="server" Width="85px" CssClass="formfieldtitle"
										Text="OK" onclick="OkButtonOnClick"></FMControls:FMButton>
								</td>
								<td>
									&nbsp;
								</td>
								<td>
									<FMControls:FMButton ID="CancelBtn" TabIndex="102" runat="server" Width="85px" CssClass="formfieldtitle"
										Text="Cancel" onclick="CancelButtonOnClick"></FMControls:FMButton>
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
