<%@ Page language="c#" Codebehind="TestResults.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.QualityControlWebApp.TestResults" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
	<head runat="server">
		<title></title>
		<base target="_self">
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<style type="text/css">
				.style1
				{
					 width: 80px;
				}
				.style2
				{
					 width: 250px;
				}
				.style3
				{
					 width: 80px;
				}
				.style4
				{
					 width: 480px;
				}	
		</style>
	</head>
	<body>
        <form id="TestResultsForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle"></asp:Image>
                <FMControls:FMLabel ID="MainLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
                    BackColor="Transparent" Width="296px" CssClass="headline">Testing Results</FMControls:FMLabel>
                <asp:ScriptManager ID="ScriptManager1" runat="server" />
                <table style="z-index: 103; width: 890px; left: 5px; position: absolute; top: 50px;" cellpadding="5">
                    <tr style="height: 30px">
                        <td class="style1">
                            <FMControls:FMLabel ID="FromDateLabel" CssClass="formfieldtitle"
                                runat="server" BackColor="Transparent" Width="120px">View results From
                            </FMControls:FMLabel>
                        </td>
                        <td class="style2">
                            <FMControls:FMDate ID="FromDate" FormatInfo="<%# this.DateFormat %>" TabIndex="1" Width="175px"
                                CssClass="formfield" runat="server" MaxLength="20"></FMControls:FMDate>
                        </td>
                        <td colspan="2">
                            <FMControls:FMLabel ID="ToDateLabel" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                                Width="30px">To
                            </FMControls:FMLabel>
                            <FMControls:FMDate ID="ToDate" FormatInfo="<%# this.DateFormat %>" TabIndex="2" Width="175px"
                                CssClass="formfield" runat="server" MaxLength="20"></FMControls:FMDate>
                        </td>
                    </tr>
                    <tr style="height: 30px">
                        <td class="style1">
                            <FMControls:FMLabel ID="AssetTypeLabel" AssociatedControlID="AssetTypeDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                                Width="80px">Asset Type:
                            </FMControls:FMLabel>
                        </td>
                        <td class="style2">
                            <FMControls:FMDropDownList Width="100px" CssClass="formfield" runat="server" Enabled="True" TabIndex="3" ID="AssetTypeDropDownList"
                                AutoPostBack="True" OnSelectedIndexChanged="AssetTypeDropDownListSelectedIndexChanged">
                            </FMControls:FMDropDownList>
                        </td>
                        <td class="style3">
                            <FMControls:FMLabel ID="AssetLabel" AssociatedControlID="AssetDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                                Width="80px">Asset:
                            </FMControls:FMLabel>
                        </td>
                        <td class="style4">
                            <FMControls:FMDropDownList ID="AssetDropDownList" TabIndex="4" Width="200px" CssClass="formfield" runat="server" Enabled="True"
                                AutoPostBack="False">
                            </FMControls:FMDropDownList>
                        </td>
                    </tr>
                    <tr style="height: 30px">
                        <td class="style1">
                            <FMControls:FMLabel ID="TestSetLabel" AssociatedControlID="TestSetDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                                Width="80px">Test Set:
                            </FMControls:FMLabel>
                        </td>
                        <td class="style2" style="vertical-align: top">
                            <FMControls:FMDropDownList ID="TestSetDropDownList" TabIndex="5" Width="200px" CssClass="formfield" runat="server" Enabled="True"
                                AutoPostBack="False">
                            </FMControls:FMDropDownList>
                        </td>
                        <td class="style3">
                            <FMControls:FMLabel ID="ResultLabel" AssociatedControlID="ResultDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
                                Width="80px">Result:
                            </FMControls:FMLabel>
                        </td>
                        <td class="style4">
                            <FMControls:FMDropDownList Width="100px" CssClass="formfield" runat="server" Enabled="True" TabIndex="6" ID="ResultDropDownList">
                            </FMControls:FMDropDownList>
                        </td>
                    </tr>
                    <tr style="height: 30px">
                        <td class="style1">
                            <FMControls:FMButton ID="AddTopButton" Text="Add" CssClass="formfieldtitle" TabIndex="7" runat="server" Width="80px"
                                OnCommand="AddCommand" />
                        </td>
                        <td class="style2">&nbsp;
                        </td>
                        <td class="style3">&nbsp;
                        </td>
                        <td class="style4" align="right">
                            <FMControls:FMButton ID="RefreshButton" Text="Refresh" CssClass="formfieldtitle" TabIndex="7" runat="server" Width="80px"
                                OnCommand="RefreshCommand"></FMControls:FMButton>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" style="vertical-align: top">
                            <FMControls:FMGridView ID="TestingResultsGridView" runat="server" AutoGenerateColumns="true" AllowSorting="true"
                                FixedHeaders="true" Width="890px" ShowFooter="true" Height="550px" AllowPaging="false" ShowHeaderWhenEmpty="true"
                                OnRowDataBound="TestingResultsGridViewRowDataBound" RowHeaderColumn="Test Set"
                                OnRowCommand="TestingResultsGridViewRowCommandReceived">
                                <Columns>
                                    <FMControls:FMEditCommandField HeaderText="Edit" EditText="Edit" />
                                </Columns>
                            </FMControls:FMGridView>
                        </td>
                    </tr>
                </table>
            </div>
        </form>
	</body>
</html>
