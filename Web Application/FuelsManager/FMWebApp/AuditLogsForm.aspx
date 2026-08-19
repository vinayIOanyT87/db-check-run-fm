<%@ Page language="c#" Codebehind="AuditLogsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AuditLogsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
        <script>
            var theMoment = new Date();
            var theDisplacement = (theMoment.getTimezoneOffset() / 60);
            document.cookie = "Displacement=" + theDisplacement;
            function EnableButtons()
            {
                $("input").removeAttr("disabled");
            }
        </script>
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                			<asp:image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 98; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<asp:dropdownlist id="UserDropDownList" style="Z-INDEX: 119; LEFT: 88px; POSITION: absolute; TOP: 160px"
				runat="server" Width="300px" CssClass="formfield" tabIndex="10" onselectedindexchanged="UserDropDownList_SelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label3" AssociatedControlID="UserDropDownList" style="Z-INDEX: 118; LEFT: 24px; POSITION: absolute; TOP: 163px" runat="server"
				Width="62px" CssClass="formfieldtitle">User:</FMControls:FMLabel>
			<asp:dropdownlist id="IDDropDownList" style="Z-INDEX: 117; LEFT: 88px; POSITION: absolute; TOP: 130px"
				runat="server" CssClass="formfield" Width="300px" tabIndex="9" onselectedindexchanged="IDDropDownList_SelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label2" AssociatedControlID="IDDropDownList" style="Z-INDEX: 116; LEFT: 24px; POSITION: absolute; TOP: 133px" runat="server"
				CssClass="formfieldtitle" Width="62px">ID:</FMControls:FMLabel>
			<asp:dropdownlist id="TypeIDDropDownList" style="Z-INDEX: 115; LEFT: 88px; POSITION: absolute; TOP: 99px"
				runat="server" CssClass="formfield" Width="300px" AutoPostBack="True" tabIndex="8" onselectedindexchanged="TypeIDDropDownList_SelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label1" AssociatedControlID="TypeIDDropDownList" style="Z-INDEX: 114; LEFT: 24px; POSITION: absolute; TOP: 102px" runat="server"
				CssClass="formfieldtitle" Width="62px">Type ID:</FMControls:FMLabel>
			<FMControls:FMButton id="ExportButton" style="Z-INDEX: 113; LEFT: 604px; POSITION: absolute; TOP: 190px"
				runat="server" CssClass="formfieldtitle" Width="55px" Text="Export" tabIndex="14"></FMControls:FMButton>
			<FMControls:FMButton id="RefreshButton" style="Z-INDEX: 113; LEFT: 666px; POSITION: absolute; TOP: 190px"
				runat="server" CssClass="formfieldtitle" Width="55px" Text="Refresh" tabIndex="15"></FMControls:FMButton>
			<asp:dropdownlist id="ActionIDDropDownList" style="Z-INDEX: 112; LEFT: 88px; POSITION: absolute; TOP: 70px"
				runat="server" CssClass="formfield" Width="300px" tabIndex="7" onselectedindexchanged="ActionIDDropDownList_SelectedIndexChanged" AutoPostBack="True"></asp:dropdownlist>
			<FMControls:FMLabel id="Label7" AssociatedControlID="ActionIDDropDownList" style="Z-INDEX: 111; LEFT: 24px; POSITION: absolute; TOP: 73px" runat="server"
				CssClass="formfieldtitle" Width="62px">Action ID:</FMControls:FMLabel>
			<FMControls:FMDateTime id="EndingDateTime" style="Z-INDEX: 106; LEFT: 475px; POSITION: absolute; TOP: 72px"
				runat="server" CssClass="formfield" Width="330px" tabIndex="11" Height="25px"></FMControls:FMDateTime>
			<FMControls:FMDateTime id="BeginningDateTime" style="Z-INDEX: 105; LEFT: 475px; POSITION: absolute; TOP: 40px"
				runat="server" CssClass="formfield" Width="330px" tabIndex="10" Height="25px"></FMControls:FMDateTime>
            <FMControls:FMCheckBox ID="ArchiveCheckBox" runat="server" style="Z-INDEX: 106; LEFT: 475px; POSITION: absolute; TOP: 102px" 
                BackColor="Transparent" CssClass="formfieldtitle" Text="Use Archive Data" TabIndex="12" />
			<FMControls:FMLabel id="Label6" style="Z-INDEX: 104; LEFT: 410px; POSITION: absolute; TOP: 72px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="64px">Ending</FMControls:FMLabel>
			<FMControls:FMLabel id="Label5" style="Z-INDEX: 103; LEFT: 410px; POSITION: absolute; TOP: 40px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Width="64px">Beginning</FMControls:FMLabel>
			<asp:dropdownlist id="SiteDropDownList" AutoPostBack="True"  style="Z-INDEX: 112; LEFT: 88px; POSITION: absolute; TOP: 40px; width: 300px;"  onselectedindexchanged="SiteDropDownList_SelectedIndexChanged"
				tabIndex="6" runat="server" CssClass="formfield"></asp:dropdownlist>
			<FMCONTROLS:FMLABEL id="FMLABEL1" AssociatedControlID="SiteDropDownList" style="Z-INDEX: 111; LEFT: 24px; POSITION: absolute; TOP: 43px" runat="server"
				CssClass="formfieldtitle" Width="46px">Site:</FMCONTROLS:FMLABEL>
			<FMControls:FMLabel id="Label27" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" CssClass="headline" Width="136px">Audit Log</FMControls:FMLabel>
			<FMControls:FMLabel id="SourceFilterLabel" style="Z-INDEX: 104; LEFT: 410px; POSITION: absolute; TOP: 163px; width: 83px;" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Source Filter:</FMControls:FMLabel>
			<asp:TextBox id="SourceFilterTextBox" style="Z-INDEX: 108; LEFT: 493px; POSITION: absolute; TOP: 160px"
				runat="server" CssClass="formfield" Width="228px" tabIndex="13" MaxLength="100"></asp:TextBox>
                <table id="Table1" style="z-index: 101; left: 16px; width: 710px; position: absolute; top: 200px; height: 10px"
                    cellspacing="0" cellpadding="1" width="700" border="0">
                    <tbody>
                        <tr>
                            <td width="350" height="36" valign="middle">
                                <FMControls:FMPageSizeDropDown ID="AuditLogsPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 713px; height: 10px" width="713">
                                <FMControls:FMDataGrid ID="AuditLogsDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="Date &amp; Time"
                                    GridLines="Vertical" Width="680px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3"
                                    AllowPaging="True" CssClass="tabletext" Style="left: 1px; top: 0px" TabIndex="11" PageSize="10">
                                    <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                                    <Columns>
                                        <asp:BoundColumn DataField="CreatedDate" HeaderText="Date &amp; Time">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="ActionID" HeaderText="Action ID">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="TypeID" HeaderText="Type ID">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="ID" HeaderText="ID">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="PropertyID" HeaderText="Property ID">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="NewValue" HeaderText="New Value">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="OldValue" HeaderText="Old Value">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="CreatedBy" HeaderText="User">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="SiteID" HeaderText="SiteID">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="SourceNode" HeaderText="Source">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                    </Columns>
                                </FMControls:FMDataGrid>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </form>
	</body>
</HTML>
