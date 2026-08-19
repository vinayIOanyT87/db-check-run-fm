<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="OfflineSynchronizationExportPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.OfflineSynchronizationExportPage" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<div style="position: absolute">
    <table style="z-index: 103; left: 8px; position: absolute; top: 0px;" cellspacing="1" cellpadding="1" width="800" border="0">
        <tr>
            <td style="z-index: 103; max-width: 70px; height: 24px" colspan="3">
                <FMControls:FMLabel ID="InstructionsLabel"  Style="left: -24px; position: relative;" runat="server" CssClass="notesHeader" Text="Generate new synchronization request containing local changes."/>
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMRadioButtonList ID="ExportTypeRadioBtnList" runat="server" OnSelectedIndexChanged="ExportTypeRadioBtnListSelectedIndexChanged"
                    AutoPostBack="True" CssClass="formfieldtitle">
                    <asp:ListItem Value="AllChanges" Selected="True">All changes since last synchronization</asp:ListItem>
                    <asp:ListItem Value="DateRange" >Re-process date range:</asp:ListItem>
                </FMControls:FMRadioButtonList>
            </td>
            <td align="left" valign="bottom">
                <FMControls:FMLabel ID="FMLabelFromDate" runat="server" CssClass="formfieldtitle">From Date:</FMControls:FMLabel>
            </td>
            <td align="left" valign="bottom">
                <FMControls:FMLabel ID="FMLabelToDate" runat="server" CssClass="formfieldtitle">To Date:</FMControls:FMLabel>
            </td>
        </tr>
        <tr>
            <td></td>
            <td align="left" valign="bottom">
                <FMControls:FMDateTime ID="FMDateFromDate" runat="server" />
            </td>
            <td align="left" valign="bottom">
                <FMControls:FMDateTime ID="FMDateToDate" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <FMControls:FMButton ID="ExportBtn" TabIndex="2" runat="server" Text="Export" Width="96px"
                    CssClass="formfieldtitle" OnClick="ExportBtnClickCommand"></FMControls:FMButton>
            </td>
        </tr>
    </table>
</div>
