<%@ Page Language="c#" CodeBehind="SynchronizationSessionConflicts.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.SynchronizationSessionConflicts" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/MenuBar/menu.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Content/bootstrap.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>

	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/autocomplete.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/json2.js" %>"></script>


</head>
<body tabindex="-1" ms_positioning="GridLayout">
    <form id="SynchronizationSessionSummaryDetails" method="post" runat="server">
        <div style="position: absolute" >
            <asp:ScriptManager ID="ScriptManager" runat="server" />
            <asp:Image ID="Image1" Style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="labSynchronizationSummary" Style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
                CssClass="headline" Width="720px" BackColor="Transparent">Synchronization Session Conflict / Error Summary</FMControls:FMLabel>
            <table id="Table1" style="Z-INDEX: 113; LEFT: 24px; WIDTH: 900px; POSITION: absolute; TOP: 70px; HEIGHT: 10px"
                cellspacing="0" cellpadding="1" border="0">
               <tr>
					<td>
						<FMControls:FMLabel ID="StatusLabel" Style="Z-INDEX: 118;" runat="server"
						Width="20px" CssClass="formfieldtitle">Status:</FMControls:FMLabel>							
					</td>
				   <td>
						<asp:DropDownList ID="StatusDropDownList" Style="Z-INDEX: 117;"
							runat="server" CssClass="formfield" AutoPostBack="True" Width="216px" TabIndex="9" OnSelectedIndexChanged="StatusDropDownListSelectedIndexChanged">
						</asp:DropDownList>
					</td>
					<td>
						<FMControls:FMButton ID="RefreshButton" Style="Z-INDEX: 107;" TabIndex="100"
							runat="server" CssClass="formfieldtitle" Width="67px" Text="Refresh"></FMControls:FMButton>						
					</td>
				</tr>
                <tr>
	                    <td colspan="3" style="height: 30px;">
                        <FMControls:FMLabel ID="ConflictErrorLabel" Style="Z-INDEX: 118; LEFT: 0px; POSITION: absolute;" runat="server"
                            Width="62px" CssClass="formfieldtitle">Conflicts / Errors</FMControls:FMLabel>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 10px;">
                         <FMControls:FMDataGridFixedPaging ID="SyncSessionConflictDataGrid" 
                            runat="server"
                            CssClass="tabletext"
                            Style="left: 1px; top: 0px"
                            BackColor="White"
                            BorderStyle="None"
                            AutoGenerateColumns="False"
                            GridLines="Vertical"
                            Width="950px"
                            BorderWidth="1px"
                            AllowSorting="True"
                            BorderColor="White"
                            CellPadding="3"
                            EmptyDataText="No records found"
                            AllowPaging="True"
                            PageSize="6"
                            ShowHeaderWhenEmpty="True"
                            ShowFooterWhenEmpty="False"
                            FixedHeaders="True">
                            <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="View">
                                    <HeaderStyle Width="0.5in" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <FMControls:FMViewLinkButton ID="FMViewConflictLinkButton" runat="server" Text="View Conflict" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn HeaderText="Conflict/Error ID" DataField="IdentityGuid" ItemStyle-Width="400px" Visible="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Table Name" DataField="TableName" ItemStyle-Width="120px"  ItemStyle-Wrap="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Remote Node" DataField="TargetNodeName" ItemStyle-Width="120px"  ItemStyle-Wrap="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Type" DataField="SyncConflictTypeIndex" ItemStyle-Width="120px" ItemStyle-Wrap="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Conflict / Error" DataField="ConflictDescription" ItemStyle-Width="250px"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Status" DataField="SyncConflictResolutionStatusIndex" ItemStyle-Width="50px" ItemStyle-Wrap="false"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Retrys" DataField="Retrys" ItemStyle-Width="50px" ItemStyle-Wrap="false"></asp:BoundColumn>
                              </Columns>
                            <PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMDataGridFixedPaging>
                    </td>
                </tr>
            </table>
            <script type="text/javascript">
            	function ViewConflict(syncConflictId) {
                  showModalDialogFrame({
                     url: "SynchronizationConflict.aspx?SyncConflictGuid=" + syncConflictId + "&<%= this.Security.CSRFTokenWithParamName %>",
                     width: 800,
                     height: 500,
                     title: "Sync Session Conflict",
                     onClose: function () {}
                  });
            	}
            </script>
        </div>
    </form>
</body>
</html>
