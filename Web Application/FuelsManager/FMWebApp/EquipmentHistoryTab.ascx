<%@ Control Language="c#" AutoEventWireup="true" CodeBehind="EquipmentHistoryTab.ascx.cs" Inherits="FuelsManager.FMWebApp.EquipmentHistoryTab" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %> 
<%@ Import Namespace="FuelsManager.Areas.AssetTrackingArea.ViewModels" %>
<html>
<head>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/redmond/jquery.ui.theme.css" %>" type="text/css" />

    <style>
        .divTable {
            width: 50%;
            height: 50%;
            display: table;
            padding: 3px;
        }

        .divTableRow {
            width: 100%;
            height: 100%;
            display: table-row;
            padding: 3px;
        }

        .divTableCell {
            width: 100%;
            height: 100%;
            display: table-cell;
            padding: 3px;
            vertical-align: top;
        }

        .divTableCell2 {
            width: 100%;
            height: 100%;
            display: table-cell;
            padding-top: 3px;
            padding-bottom: 3px;
            padding-right: 3px;
            padding-left: 15px;
            vertical-align: top;
        }

        /* The Modal (background) */
        .confirmModal {
            display: none; /* Hidden by default */
            position: fixed; /* Stay in place */
            z-index: 1; /* Sit on top */
            padding-top: 100px; /* Location of the box */
            left: 0;
            top: 0;
            width: 100%; /* Full width */
            height: 100%; /* Full height */
            overflow: auto; /* Enable scroll if needed */
            background-color: rgb(0,0,0); /* Fallback color */
            background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
        }

        /* Modal Content */
        .confirmModal-content {
	        background-color: #fefefe;
	        margin: auto;
	        padding: 20px;
	        border: 1px solid #888;
	        width: 20%;
        }

        /* The Close Button */
        .close {
            color: #aaaaaa;
            float: right;
            font-size: 28px;
            font-weight: bold;
        }

        .close:hover,
        .close:focus {
            color: #000;
            text-decoration: none;
            cursor: pointer;
        }
    </style>
</head>
<script>
    function Init()
    {
        // Reset the dates from the postback.
        var startDate = $("#tcEquipment_TabPanel1_EquipmentHistoryTab_EquipHistoryStartDateHidden").val();
        var endDate = $("#tcEquipment_TabPanel1_EquipmentHistoryTab_EquipHistoryEndDateHidden").val();

        $("#EquipHistoryStartDate").val(startDate);
        $("#EquipHistoryEndDate").val(endDate);

        // Create the Start and End date pickers.
        $(function ()
        {
            $("#EquipHistoryStartDate").datepicker(
            {
                dateFormat: "yy/mm/dd",
                onSelect: StartDateOnSelect
            });
        });

        $(function ()
        {
            $("#EquipHistoryEndDate").datepicker(
            {
                dateFormat: "yy/mm/dd",
                onSelect: EndDateOnSelect
        });
        });
    }

    //===============================================================================
    // This function will handle the start date picker on select event. It will
    // clear out the period dropdown and populate the hidden start date field.
    //===============================================================================
    function StartDateOnSelect(startDateStr, obj)
    {
        // Clear the period dropdown.
        var periodDropdownElement = document.getElementById("tcEquipment_TabPanel1_EquipmentHistoryTab_PeriodDropdown");
        periodDropdownElement.selectedIndex = 0;

        $("#tcEquipment_TabPanel1_EquipmentHistoryTab_EquipHistoryStartDateHidden").val(startDateStr);
    }

    //===============================================================================
    // This function will handle the end date picker on select event. It will
    // clear out the period dropdown and populate the hidden start date field.
    //===============================================================================
    function EndDateOnSelect(endDateStr, obj)
    {
        // Clear the period dropdown.
        var periodDropdownElement = document.getElementById("tcEquipment_TabPanel1_EquipmentHistoryTab_PeriodDropdown");
        periodDropdownElement.selectedIndex = 0;

        $("#tcEquipment_TabPanel1_EquipmentHistoryTab_EquipHistoryEndDateHidden").val(endDateStr);
    }

    //====================================================================================
    // This function will handle the period filter on change event. 
    //====================================================================================
     function PeriodDropdownOnChange ()
    {
        // Clear the Date filters
        $("#EquipHistoryStartDate").val("");
        $("#EquipHistoryEndDate").val("");
        $("#tcEquipment_TabPanel1_EquipmentHistoryTab_EquipHistoryStartDateHidden").val("");
        $("#tcEquipment_TabPanel1_EquipmentHistoryTab_EquipHistoryEndDateHidden").val("");
     }
</script>
<body onload="Init();">
    <div style="display: none">
        <asp:TextBox ID="EquipHistoryStartDateHidden" runat="server" Text=""></asp:TextBox>
        <asp:TextBox ID="EquipHistoryEndDateHidden" runat="server" Text=""></asp:TextBox>
    </div>
    <div class="divTable">
        <div class="divTableRow">
            <div class="equipmentHistoryFilterRow">
                <div class="divTableCell">
                    <FMControls:FMLabel ID="PeriodLabel"
                        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Enter Time Period:</FMControls:FMLabel>
                </div>
                <div class="divTableCell">
                    <select id="PeriodDropdown" runat="server" cssclass="formfieldtitle" style="width: 60px; height: 15px; font-size: 10px;" onchange="PeriodDropdownOnChange();">
                    </select>
                </div>
                <div class="divTableCell">
                    <FMControls:FMLabel ID="SixtyDaysLabel"
                        runat="server" CssClass="formfieldtitle" BackColor="Transparent">of 60 days</FMControls:FMLabel>
                </div>
                <div class="divTableCell2">
                    <label></label>
                </div>
                <div class="divTableCell">
                    <FMControls:FMLabel ID="FromDateLabel"
                        runat="server" CssClass="formfieldtitle" BackColor="Transparent">From date</FMControls:FMLabel>
                </div>
                <div class="divTableCell">
                    <input id="EquipHistoryStartDate" type="text" style="width: 80px;" readonly="readonly" />
                </div>
                <div class="divTableCell2">
                    <FMControls:FMLabel ID="EndDateLabel"
                        runat="server" CssClass="formfieldtitle" BackColor="Transparent">End date</FMControls:FMLabel>
                </div>
                <div class="divTableCell">
                    <input id="EquipHistoryEndDate" type="text" style="width: 80px;" readonly="readonly" />
                </div>
                <div class="divTableCell2">
                    <FMControls:FMButton ID="EquipHistoryRefreshBtn" Style="width: 80px;" runat="server" CssClass="formfieldtitle" Text="Refresh" CommandName="Refresh" OnClick="EquipmentHistoryRefreshOnClick"></FMControls:FMButton>
                </div>
            </div>
        </div>
        <table style="padding-top: 10px;">
            <tr>
                <td>
                    <FMControls:FMCheckBox ID="CompartmentCheckbox" Text="Show Compartment" CssClass="formfieldtitle" runat="server"></FMControls:FMCheckBox>
                </td>
            </tr>
        </table>
    </div>
    <div style="position: absolute">
        <table>
            <tr>
                <td></td>
            </tr>
            <tr>
                <td></td>
            </tr>
            <tr>
                <td>
                   <FMControls:FMPageSizeDropDown ID="EquipmentHistoryPageSizeDropDown" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <FMControls:FMDataGridFixedPaging ID="EquipmentHistoryGrid" 
                                runat="server"
                                style="LEFT: 1px; TOP: 0px" 
                                PageSize="12"						  
                                AutoGenerateColumns="False" 
                                Width="1150px"
                                AllowSorting="false" 					
                                ShowHeaderWhenEmpty="True" 
                                FixedHeaders="True" 
                                UseAccessibleHeader="False"                            					
                                AllowPaging="True" 
                                ShowFooter="False" 
                                ShowFooterWhenEmpty="False" 					
                                BackColor="White" 
                                BorderStyle="Solid" 
                                BorderWidth="1px" 					
                                CellPadding="3" 
                                CssClass="tabletext" 
                                EmptyDataText="No records found"
                                Height="320px"	
                                FixedHeight="320px"				
                                GridLines="Vertical" 
                                GroupColumnOffset="0" 
                                GroupingDepth="0">
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						        <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Device ID">
                                        <ItemTemplate>
                                            <asp:Label ID="DeviceIdLabel" runat="server" Text='<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).AssetTrackingDeviceId %>' />
                                        </ItemTemplate>
                                        <HeaderStyle Width="150px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Compartment">
                                        <ItemTemplate>
                                            <asp:Label ID="ExpandLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).CompartmentName %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="20px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="GPS Coordinates">
                                        <ItemTemplate>
                                            <asp:Label ID="GpsLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).GpsCoordinatesStr %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="200px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Message Timestamp">
                                        <ItemTemplate>
                                            <asp:Label ID="TimestampLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).SessionDatetimeStr %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="275px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Product">
                                        <ItemTemplate>
                                            <asp:Label ID="ProductLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).ProductId %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="120px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Volume">
                                        <ItemTemplate>
                                            <asp:Label ID="VolumLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).VolumeStr %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="120px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Water">
                                        <ItemTemplate>
                                            <asp:Label ID="WaterLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).WaterStr %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="120px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Density">
                                        <ItemTemplate>
                                            <asp:Label ID="DensityLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).DensityStr %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="120px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Dielectric">
                                        <ItemTemplate>
                                            <asp:Label ID="DielectricLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).DielectricStr %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="120px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Remarks">
                                        <ItemTemplate>
                                            <asp:Label ID="RemarksLabel" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).Remarks %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="320px" />
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Contaminated" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="MessageStateLabel" Visible="False" CssClass="formfield" runat="server" Text="<%# ((AssetEquipmentHistoryRecordModel)Container.DataItem).MessageState.ToString() %>"></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="20px" />
                                    </asp:TemplateColumn>
                                </Columns>
                                <PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
                            </FMControls:FMDataGridFixedPaging>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
