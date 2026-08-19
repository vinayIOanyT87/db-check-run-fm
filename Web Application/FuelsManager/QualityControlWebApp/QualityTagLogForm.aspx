<%@ Page language="c#" AutoEventWireup="True" Codebehind="QualityTagLogForm.aspx.cs" Inherits="FuelsManager.QualityControlWebApp.QualityTagLogForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title></title>
		<meta name="generator" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="code_language" Content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
    </head>

	<body>
        <form id="QualityTagLogForm" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <div>

                    <!-- Top area -->
                    <asp:ScriptManager ID="oScriptManager" runat="server" />

                    <asp:Image ID="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>"
                        Style="z-index: -3; left: 0px; position: absolute; top: 0px" runat="server"
                        ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>

                    <asp:Label ID="TitleLabel"
                        Style="z-index: 105; left: 16px; position: absolute; top: 9px" runat="server"
                        CssClass="headline">Quality Tag Summary</asp:Label>

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
                                Width="150px" runat="server" CssClass="formfield" TabIndex="3"></FMControls:FMDate>

                            <FMControls:FMLabel ID="EndDateLabel" Style="z-index: 122; left: 16px; position: absolute; top: 118px"
                                runat="server" BackColor="Transparent" Height="16px" Width="102px"
                                CssClass="formfieldtitle">End Date:</FMControls:FMLabel>

                            <FMControls:FMDate ID="EndDate" Style="z-index: 202; left: 96px; position: absolute; top: 116px"
                                Width="150px" runat="server" CssClass="formfield" TabIndex="3"></FMControls:FMDate>

                            <FMControls:FMDropDownList ID="QualityTagDropDown" Style="top: 56px; z-index: 127; left: 400px; position: absolute"
                                Width="130px" runat="server" CssClass="formfield" Sort="false" TabIndex="3" AutoPostBack="true">
                            </FMControls:FMDropDownList>

                            <FMControls:FMLabel ID="FMQualityTagLabel" AssociatedControlID="QualityTagDropDown"
                                Style="z-index: 110; left: 290px; position: absolute; top: 58px; width: 94px;"
                                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Quality Tags:</FMControls:FMLabel>

                            <FMControls:FMDropDownList ID="TagStatusFilterDropDown" Style="top: 89px; z-index: 127; left: 400px; position: absolute"
                                Width="130px" runat="server" CssClass="formfield" Sort="false" TabIndex="3" AutoPostBack="true">
                            </FMControls:FMDropDownList>

                            <FMControls:FMLabel ID="FMLabel1" AssociatedControlID="TagStatusFilterDropDown"
                                Style="z-index: 110; left: 290px; position: absolute; top: 92px; width: 100px;"
                                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Tag Status Filter:</FMControls:FMLabel>

                            <FMControls:FMDropDownList ID="TaggedByDropDown" Style="top: 119px; z-index: 127; left: 400px; position: absolute"
                                Width="130px" runat="server" CssClass="formfield" Sort="false" TabIndex="3" AutoPostBack="true">
                            </FMControls:FMDropDownList>

                            <FMControls:FMLabel ID="TaggedByFMLabel2" AssociatedControlID="TaggedByDropDown"
                                Style="z-index: 110; left: 290px; position: absolute; top: 122px; width: 94px;"
                                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Tagged By:</FMControls:FMLabel>

                            <FMControls:FMDropDownList ID="RemovedByDropDown" Style="top: 149px; z-index: 127; left: 400px; position: absolute"
                                Width="130px" runat="server" CssClass="formfield" Sort="false" TabIndex="3" AutoPostBack="true">
                            </FMControls:FMDropDownList>

                            <FMControls:FMLabel ID="RemovedByDropDownFMLabel" AssociatedControlID="RemovedByDropDown"
                                Style="z-index: 110; left: 290px; position: absolute; top: 152px; width: 94px;"
                                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Removed By:</FMControls:FMLabel>

                            <FMControls:FMDropDownList ID="AssetIDDropDownList" Style="top: 179px; z-index: 127; left: 400px; position: absolute"
                                Width="130px" runat="server" CssClass="formfield" Sort="false" TabIndex="3" AutoPostBack="true">
                            </FMControls:FMDropDownList>

                            <FMControls:FMLabel ID="FMLabel2" AssociatedControlID="AssetIDDropDownList"
                                Style="z-index: 110; left: 290px; position: absolute; top: 182px; width: 94px;"
                                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Asset ID:</FMControls:FMLabel>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <!-- Right column -->
                    <FMControls:FMButton ID="RefreshButton" Style="z-index: 108; left: 650px; position: absolute; top: 56px"
                        runat="server" CssClass="formfieldtitle" Text="Refresh" Width="108px" TabIndex="7"
                        OnClick="RefreshButtonOnClick" EnableViewState="False"></FMControls:FMButton>

                    <FMControls:FMCheckBox ID="HistoricalDataCheckBox" TabIndex="8" runat="server" CssClass="formfieldtitle"
                        Text="Include Historical Data" Style="z-index: 124; left: 680px; position: absolute; top: 89px; width: 188px;"></FMControls:FMCheckBox>

                    <!-- The GridView -->
                    <asp:UpdatePanel ID="UpdatePanelGridView" runat="server">
                        <ContentTemplate>
                            <table style="z-index: 110; left: 32px; top: 220px; width: 575px; position: absolute;" cellpadding="5">

                                <tr>
                                    <td width="498" height="36" valign="middle">

                                        <FMControls:FMButton ID="FMButtonAddTop" runat="server"
                                            CssClass="formfieldtitle" Text="Add" Width="100px"
                                            TabIndex="7" OnClick="AddButtonOnClick"
                                            EnableViewState="False"></FMControls:FMButton>
                                    </td>
                                </tr>

                                <tr>
                                    <td style="vertical-align: top">
                                        <FMControls:FMGridView ID="FMGridViewQualityTagLog" runat="server" RowHeaderColumn="Asset ID"
                                            AutoGenerateColumns="true" AllowSorting="true" AllowPaging="false"
                                            FixedHeaders="true" Width="900px" Height="450px" ShowFooter="true"
                                            ShowHeaderWhenEmpty="True" ShowFooterWhenEmpty="False" AutoDetermineWidth="True">
                                            <Columns>
                                                <FMControls:FMEditCommandField HeaderText="Edit"
                                                    EditText="Remove Tag from Equipment" />
                                                <FMControls:FMDeleteCommandField HeaderText="Delete"
                                                    DeleteText="Delete Tag" />
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
                            <asp:AsyncPostBackTrigger ControlID="FMGridViewQualityTagLog" EventName="Sorting" />
                            <asp:AsyncPostBackTrigger ControlID="FMGridViewQualityTagLog" EventName="PageIndexChanging" />
                        </Triggers>
                    </asp:UpdatePanel>

                </div>

                <!-- ==================================================================== -->
                <!--                        Client side code                              -->
                <!-- ==================================================================== -->

                <script type="text/javascript">
                    document.getElementById("RefreshButton").setActive();
                    document.getElementById("RefreshButton").focus();
                </script>

            </div>
        </form>
    </body>
</html>
