<%@ Page Language="c#" AutoEventWireup="True" CodeBehind="MaintenanceLogForm.aspx.cs" Inherits="FuelsManager.MaintenanceWebApp.MaintenanceLogForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	</head>

	<body>
        <form id="MaintenanceLogForm" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <!-- Top area -->
                <asp:ScriptManager ID="oScriptManager" runat="server" />

                <asp:Image ID="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>"
                    Style="z-index: -3; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>

                <asp:Label ID="TitleLabel"
                    Style="z-index: 105; left: 16px; position: absolute; top: 9px" runat="server"
                    CssClass="headline">Maintenance Log</asp:Label>


                <!-- Left column -->
                <asp:UpdatePanel ID="DateAreaUpdatePanel" runat="server">
                    <ContentTemplate>

                        <FMControls:FMLabel ID="FMLabelDateTypeFilter" AssociatedControlID="DateFilterTypeDropDown" Style="z-index: 110; left: 16px; position: absolute; top: 58px"
                            Width="102px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Date Filter:</FMControls:FMLabel>

                        <FMControls:FMDropDownList ID="DateFilterTypeDropDown" Style="top: 56px; z-index: 127; left: 96px; position: absolute"
                            Width="130px" runat="server" CssClass="formfield" Sort="false" TabIndex="3" AutoPostBack="true"
                            OnSelectedIndexChanged="DateFilterTypeDropDownSelectedIndexChanged">
                        </FMControls:FMDropDownList>


                        <FMControls:FMLabel ID="StartDateLabel" Style="z-index: 121; left: 16px; position: absolute; top: 88px"
                            runat="server" BackColor="Transparent" Height="16px" Width="102px"
                            CssClass="formfieldtitle">Start Date:</FMControls:FMLabel>

                        <FMControls:FMDate ID="StartDate" Style="z-index: 203; left: 96px; position: absolute; top: 86px;"
                            Width="150px" runat="server" CssClass="formfield" TabIndex="4"></FMControls:FMDate>


                        <FMControls:FMLabel ID="EndDateLabel" Style="z-index: 122; left: 16px; position: absolute; top: 118px"
                            runat="server" BackColor="Transparent" Height="16px" Width="102px"
                            CssClass="formfieldtitle">End Date:</FMControls:FMLabel>

                        <FMControls:FMDate ID="EndDate" Style="z-index: 202; left: 96px; position: absolute; top: 116px"
                            Width="150px" runat="server" CssClass="formfield" TabIndex="5"></FMControls:FMDate>

                    </ContentTemplate>
                </asp:UpdatePanel>

                <!-- Right column -->
                <FMControls:FMButton ID="RefreshButton" Style="z-index: 108; left: 331px; position: absolute; top: 56px"
                    runat="server" CssClass="formfieldtitle" Text="Refresh" Width="108px" TabIndex="7"
                    OnClick="RefreshButtonOnClick" EnableViewState="False"></FMControls:FMButton>

                <FMControls:FMCheckBox ID="HistoricalDataCheckBox" TabIndex="8" runat="server" CssClass="formfieldtitle"
                    Text="&nbsp;Historical Data"
                    Style="z-index: 124; left: 328px; position: absolute; top: 89px; width: 108px;"
                    AutoPostBack="False"></FMControls:FMCheckBox>


                <!-- The GridView -->
                <asp:UpdatePanel ID="UpdatePanelGridView" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table style="z-index: 110; left: 32px; top: 160px; width: 700px; position: absolute;" cellpadding="5">

                            <tr>
                                <td height="36" valign="middle">

                                    <FMControls:FMButton ID="FMButtonAddTop" runat="server"
                                        CssClass="formfieldtitle" Text="Add" Width="100px"
                                        TabIndex="7" OnClick="AddButtonOnClick"
                                        EnableViewState="False"></FMControls:FMButton>
                                </td>
                            </tr>

                            <tr>
                                <td style="vertical-align: top">
                                    <FMControls:FMGridView ID="FMGridViewMaintenanceLog" runat="server" RowHeaderColumn="Asset ID"
                                        AutoGenerateColumns="true" AllowSorting="true" AllowPaging="false"
                                        Width="850px" ShowHeaderWhenEmpty="true" FixedHeaders="true" ShowFooter="true" Height="550px">
                                        <Columns>
                                            <FMControls:FMViewCommandField HeaderText="View" />
                                        </Columns>
                                        <Columns>
                                            <FMControls:FMDeleteCommandField HeaderText="Delete" />
                                        </Columns>
                                    </FMControls:FMGridView>
                                </td>
                            </tr>

                            <tr>
                                <td width="498" height="36" valign="middle">
                                    <FMControls:FMButton ID="FMButtonAddBottom" runat="server"
                                        CssClass="formfieldtitle" Text="Add" Width="100px"
                                        TabIndex="7" OnClick="AddButtonOnClick"
                                        EnableViewState="False"></FMControls:FMButton>
                                </td>
                            </tr>

                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="FMGridViewMaintenanceLog" EventName="Sorting" />
                        <asp:AsyncPostBackTrigger ControlID="FMGridViewMaintenanceLog" EventName="PageIndexChanging" />
                    </Triggers>
                </asp:UpdatePanel>

			<!-- ==================================================================== -->
			<!--                        Client side code                              -->
			<!-- ==================================================================== -->

			<script type="text/jscript">
				if (document.getElementById("RefreshButton")            != null)  document.getElementById("RefreshButton").setActive();
				if (document.getElementById("PageSizeDropDownMaintLog") != null)  document.getElementById("PageSizeDropDownMaintLog").focus();
            </script>

        </div>
        </form>
	</body>
</html>
