<%@ Page Language="c#" CodeBehind="SynchronizationSessionSummary.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.SynchronizationSessionSummary" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">

	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/autocomplete.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/json2.js" %>"></script>
</head>
<body tabindex="-1" ms_positioning="GridLayout">
    <form id="SynchronizationSessionSummary" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:ScriptManager ID="ScriptManager" runat="server" />
            <asp:Image ID="Image1" Style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="labSynchronizationSummary" Style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
                CssClass="headline" Width="720px" BackColor="Transparent">Synchronization Session Summary</FMControls:FMLabel>
            <FMControls:FMLabel ID="NodeLabel" Style="Z-INDEX: 118; LEFT: 24px; POSITION: absolute; TOP: 40px" runat="server"
                Width="62px" CssClass="formfieldtitle">Node:</FMControls:FMLabel>
            <asp:DropDownList ID="NodeDropDownList" Style="Z-INDEX: 117; LEFT: 115px; POSITION: absolute; TOP: 40px"
                runat="server" CssClass="formfield" Width="216px" TabIndex="9" OnSelectedIndexChanged="NodeDropDownList_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList>
            <FMControls:FMLabel ID="SessionTypLabel" Style="Z-INDEX: 118; LEFT: 24px; POSITION: absolute; TOP: 70px" runat="server"
                Width="62px" CssClass="formfieldtitle">Transfer Type:</FMControls:FMLabel>
            <asp:DropDownList ID="TransferTypeDropDownList" Style="Z-INDEX: 119; LEFT: 115px; POSITION: absolute; TOP: 70px"
                runat="server" Width="216px" CssClass="formfield" TabIndex="10" OnSelectedIndexChanged="TransferTypeDropDownList_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList>
            <FMControls:FMCheckBox runat="server" Text="With Conflicts" ID="WithConflictsCheckbox" Checked="False" CssClass="formfieldtitle"
                style="Z-INDEX: 120; POSITION: absolute; TOP: 98px; LEFT: 22px; width: 163px; bottom: 465px; height: 19px;" TabIndex="11"
                OnCheckedChanged="WithConflicts_CheckBoxChanged" AutoPostBack="True"/>    
            <FMControls:FMLabel ID="BeginningLabel" Style="Z-INDEX: 103; LEFT: 350px; POSITION: absolute; TOP: 40px" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle" Width="64px">Beginning:</FMControls:FMLabel>
            <FMControls:FMDateTime ID="BeginningDateTime" Style="Z-INDEX: 104; LEFT: 425px; POSITION: absolute; TOP: 40px"
                runat="server" CssClass="formfield" Width="680px" TabIndex="1" Height="25px"></FMControls:FMDateTime>
            <FMControls:FMLabel ID="EndingLabel" Style="Z-INDEX: 105; LEFT: 350px; POSITION: absolute; TOP: 70px" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle" Width="64px">Ending:</FMControls:FMLabel>
            <FMControls:FMDateTime ID="EndingDateTime" Style="Z-INDEX: 106; LEFT: 425px; POSITION: absolute; TOP: 70px"
                runat="server" CssClass="formfield" Width="680px" TabIndex="4" Height="25px"></FMControls:FMDateTime>
            <FMControls:FMButton ID="RefreshButton" Style="Z-INDEX: 107; LEFT: 600px; POSITION: absolute; TOP: 100px;" TabIndex="100"
                runat="server" CssClass="formfieldtitle" Width="67px" Text="Refresh" onclick="RefreshButton_Click"></FMControls:FMButton>
            <FMControls:FMButton ID="UnresolvedButton" Style="Z-INDEX: 107; LEFT: 680px; POSITION: absolute; TOP: 100px;" TabIndex="100"
                runat="server" CssClass="formfieldtitle" Width="110px" Text="View Unresolved" OnClientClick="ViewUnresolvedConflicts(); return false;"></FMControls:FMButton>
            <table id="Table1" style="Z-INDEX: 113; LEFT: 24px; WIDTH: 900px; POSITION: absolute; TOP: 120px; HEIGHT: 10px"
                cellspacing="0" cellpadding="1" border="0">
                 <tr>
                     <td width="350" height="36" valign="middle">
                        <FMControls:FMPageSizeDropDown ID="SyncSessoinSummaryPageSizeDropDown" ToolTip="Page Size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                     </td>
                 </tr>
                 <tr>
                    <td style="width: 500px; height: 10px;">
                        <FMControls:FMDataGridFixedPaging ID="SyncSessionSummaryDataGrid"
                            runat="server"
                            CssClass="tabletext"
                            Style="left: 1px; top: 0px"
                            BackColor="White"
                            BorderStyle="None"
                            AutoGenerateColumns="False"
                            GridLines="Vertical"
                            Width="1000px"
                            BorderWidth="1px"
                            AllowSorting="True"
                            BorderColor="White"
                            CellPadding="3"
                            EmptyDataText="No records found"
                            AllowPaging="True"
                            PageSize="12"
                            ShowHeaderWhenEmpty="True"
                            ShowFooterWhenEmpty="False"
                            FixedHeaders="True">
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="View">
                                    <HeaderStyle Width="0.5in" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <FMControls:FMViewLinkButton ID="FMViewConflictsLinkButton" runat="server" Text="View Conflicts" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn HeaderText="Session ID" DataField="SyncSessionLogGuid" ItemStyle-Width="400px" Visible="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Start Date Time" DataField="StartDate" ItemStyle-Width="225px" ItemStyle-Wrap="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="End Date Time" DataField="EndDate" ItemStyle-Width="225px" ItemStyle-Wrap="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Remote Node" DataField="SourceNodeMachineName" ItemStyle-Width="25px" ItemStyle-Wrap="false" ></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Transfer Type" DataField="TransferTypeID" ItemStyle-Width="100px" ItemStyle-Wrap="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Status" DataField="SyncSessionStatusID" ItemStyle-Width="175px" ItemStyle-Wrap="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Conflicts" DataField="Conflicts" ItemStyle-Width="30px" ItemStyle-Wrap="false"></asp:BoundColumn>
                             </Columns>
                            <PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMDataGridFixedPaging>            
                    </td>
                </tr>
            </table>
            <script type="text/javascript">
               function ViewSessionConflicts(syncSessionId) {
                  showModalDialogFrame({
                     url: "SynchronizationSessionConflicts.aspx?SessionGuid=" + syncSessionId,
                     width: 1024,
                     height: 768,
                     title: "Sync Session Conflicts",
                     onClose: function () { }
                  });
               }

	            function ViewUnresolvedConflicts() {
                  var nodeDropDownList = document.getElementById("NodeDropDownList");
						showModalDialogFrame({
							url: "SynchronizationSessionConflicts.aspx?SyncNodeGuid=" + nodeDropDownList.value,
							width: 1024,
							height: 768,
                     title: "Sync Session Unresolved Conflicts",
                     onClose: function () { }
					   });
	            }
            </script>
        </div>
    </form>
</body>
</html>
