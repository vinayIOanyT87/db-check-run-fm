<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExStarsViewHistory.ascx.cs" Inherits="FuelsManager.Accounting.ExStarsViewHistory" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
    <head>
        <title>View ExSTARS History</title>
	    <style type="text/css">
            .auto-style2 {
                width: 158px;
            }
            .auto-style4 {
                width: 202px;
            }
            .auto-style5 {
                width: 154px;
            }
            .auto-style6 {
                height: 36px;
            }
            .auto-style7 {
                width: 46px;
            }
            .auto-style8 {
                width: 48px;
            }
            .auto-style9 {
                width: 31px;
            }
        </style>
	</HEAD>
	<body >
        <link href="<%=HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	    <table style="width: 835px" >
	        <tr>
	            <td colspan="2" >
	                <FMControls:FMLabel runat="server" Text="Manager" CssClass="formfield"/>
	            </td>
                <td colspan="4" >
	                <FMControls:FMLabel runat="server" Text="Date Range" CssClass="formfield"/>
	            </td>
	        </tr>
            <tr>
                <td class="auto-style2"><FMControls:FMDropDownList runat="server" ID="ddManager" CssClass="formfield" style="LEFT: 1px; TOP: 1px" Height="21px" Width="186px"/></td>
                <td class="auto-style8"> <FMControls:FMLabel runat="server" Text="From" CssClass="formfield" style="LEFT: 1px; TOP: 1px" Width="50px" height="15px" /></td>
                <td class="auto-style7"><FMControls:FMDate runat="server" ID="dtStartDate" Width="130px" style="LEFT: 1px; TOP: 1px; margin-left: 0px;" Height="15px" CssClass="formfield"/> </td>
                <td class="auto-style9"><FMControls:FMLabel runat="server" Text="To" CssClass="formfield" style="LEFT: 1px; TOP: 1px" Width="10px"/></td>
                <td class="auto-style5"><FMControls:FMDate runat="server" ID="dtEndDate"  Width="130px" style="LEFT: 1px; TOP: 1px" Height="15px" CssClass="formfield"/> </td>
                <td class="auto-style4"><FMControls:FMButton runat="server" ID="btnViewHistory" Text="View History" OnClick="btnViewHistory_Click" style="LEFT: 1px; TOP: 1px" Height="25px"  /></td>
            </tr>
            <tr>
                <td colspan="6" class="auto-style6">
                    <FMControls:FMLabel ID="lblClickToView" runat="server" Text="Click To View" CssClass="formfield" Visible="False"/>
                </td>
                </tr>
            </table>
        <FMControls:FMDataGrid runat="server" ID="dataGrid" OnItemCommand="dataGrid_Command" AutoGenerateColumns="False" BackColor="White" CssClass="tabletext"
							    Width="1026px" HorizontalAlign="Left" GridLines="Vertical" CellPadding="1" BorderWidth="1px"
							    BorderColor="#999999" BorderStyle="None">
			<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
			<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
			<HeaderStyle Font-Bold="True" Wrap="False" ForeColor="White" CssClass="tablecolheadcentered" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
            <Columns>  
                <asp:ButtonColumn  HeaderText="Open" Text="EasyRead"  CommandName="EasyRead" />
                <asp:ButtonColumn  HeaderText="Open" Text="EDI" CommandName="EDI" />
                <asp:ButtonColumn  HeaderText="Open" Text="151" CommandName="errorReport"/>
                <asp:BoundColumn ReadOnly="True" HeaderText="Manager"            DataField="Manager" SortExpression="Manager" />
                <asp:BoundColumn ReadOnly="True" HeaderText="Start Date"            DataField="StartDate" SortExpression="StartDate" />                
                <asp:BoundColumn ReadOnly="True"  HeaderText="End Date"           DataField="EndDate" SortExpression="EndDate" />
                <asp:BoundColumn ReadOnly="True" HeaderText="Report Type"        DataField="ReportType" SortExpression="ReportType" />
                <asp:BoundColumn ReadOnly="True" HeaderText="Mod"                DataField="Modifier" SortExpression="Modifier" />
                <asp:BoundColumn ReadOnly="True"  HeaderText="Ctrl Number"        DataField="CtrlNumber" SortExpression="CtrlNumber" />
                <asp:BoundColumn ReadOnly="True" HeaderText="Orig Ctrl"   DataField="OrigCtrlNumber" SortExpression="OrigCtrlNumber" />
                <asp:BoundColumn ReadOnly="True"  HeaderText="Errors"        DataField="ErrorCount" ItemStyle-HorizontalAlign="Right" SortExpression="ErrorCount"  >
<ItemStyle HorizontalAlign="Right"></ItemStyle>
                </asp:BoundColumn>
                <asp:BoundColumn ReadOnly="True" HeaderText="Warnings"      DataField="WarningCount" ItemStyle-HorizontalAlign="Right" SortExpression="WarningCount" >
<ItemStyle HorizontalAlign="Right"></ItemStyle>
                </asp:BoundColumn>
                <asp:BoundColumn ReadOnly="True" HeaderText="Response Loaded"    DataField="ResponseLoaded" SortExpression="WarningCount" />                
                <asp:BoundColumn ReadOnly="True" HeaderText="Created Date"       DataField="CreatedDate" SortExpression="CreatedDate" />
                <asp:BoundColumn ReadOnly="True" Visible="False"      DataField="FilingsGuidAsStr" />
                
            </Columns>
        </FMControls:FMDataGrid>                     

        </body> 
</HTML>
