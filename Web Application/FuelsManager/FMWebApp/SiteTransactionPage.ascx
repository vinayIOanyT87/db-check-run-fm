<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SiteTransactionPage.ascx.cs" Inherits="FuelsManager.FMWebApp.SiteTransactionPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
    <HEAD>
    </HEAD>
    <body>
        <!-- this div sets off the BOL Print Configuration section -->
        <div style="position: absolute; top: 0px; left: 0px; height: 310px; width: 320px; border: 1px; border-color: darkgray; border-style: solid">
        </div>
        <FMControls:FMLabel id="StationPromptConfigurationLabel" style="Z-INDEX: 111; LEFT: 14px; POSITION: absolute; TOP: 16px; right: 1289px;"
                            runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px" Height="16px">BOL Print Configuration</FMControls:FMLabel>
        <FMControls:FMCheckbox id="EnableAutomaticPrintingCheckBox" Text="Enable Automatic BOL Printing" BackColor="Transparent"
                               CssClass="formfieldtitle" runat="server" style="Z-INDEX: 102; LEFT: 14px; POSITION: absolute; TOP: 41px"
                               tabIndex="3">
        </FMControls:FMCheckbox>

        <FMControls:FMCheckbox id="EnableBOLPDFArchivingCheckbox" Text="Enable BOL PDF Archiving" BackColor="Transparent"
	CssClass="formfieldtitle" runat="server" style="Z-INDEX: 102; LEFT: 13px; POSITION: absolute; TOP: 74px" tabIndex="3"></FMControls:FMCheckbox>
    <FMControls:FMLabel id="Fmlabel9" runat="server" AssociatedControlID="BOLPDFArchivingPathTextBox" style="Z-INDEX: 102; LEFT: 18px; POSITION: absolute; TOP: 105px; width: 50px;" CssClass="formfieldtitle">Path: </FMControls:FMLabel>
    <asp:textbox id="BOLPDFArchivingPathTextBox" style="Z-INDEX: 110; LEFT: 119px; POSITION: absolute; TOP: 100px; width: 175px;" tabIndex="22" runat="server" CssClass="formfield"></asp:textbox>
        <FMControls:FMLabel id="FMLabel6" style="Z-INDEX: 111; LEFT: 42px; POSITION: absolute; TOP: 62px; right: 1289px; color: red; font-style: italic"
                            runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px" Height="16px">
            (BOL Printers are configured via the stations)
        </FMControls:FMLabel>
        <FMControls:FMLabel id="FMLabel7" style="Z-INDEX: 111; LEFT: 14px; POSITION: absolute; TOP: 162px; right: 1289px;"
                            runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px" Height="16px">
            BOL Exception Print Configuration
        </FMControls:FMLabel>
        <FMControls:FMCheckbox id="PrintBrokenBlendsCheckBox" Text="Print Broken Blend BOLs" BackColor="Transparent"
                               CssClass="formfieldtitle" runat="server" style="Z-INDEX: 102; LEFT: 14px; POSITION: absolute; TOP: 183px"
                               tabIndex="1">
        </FMControls:FMCheckbox>
        <FMControls:FMCheckbox id="PrintImproperAdditizationCheckBox" style="Z-INDEX: 108; LEFT: 14px; POSITION: absolute; TOP: 204px"
                               runat="server" Text="Print BOLs With Improper Additization" BackColor="Transparent" CssClass="formfieldtitle"
                               tabIndex="2">
        </FMControls:FMCheckbox>
        <FMControls:FMCheckbox id="PrintOverweightBOLCheckBox" style="Z-INDEX: 108; LEFT: 14px; POSITION: absolute; TOP: 225px"
                               runat="server" Text="Print Overweight BOLs" BackColor="Transparent" CssClass="formfieldtitle" tabIndex="2">
        </FMControls:FMCheckbox>
        <FMControls:FMLabel id="Label3" AssociatedControlID="ExceptionBOLPrinterDropDownList" style="Z-INDEX: 120; LEFT: 14px; POSITION: absolute; TOP: 256px" runat="server"
                            BackColor="Transparent" CssClass="formfieldtitle" Width="227px">
            Exception BOL Printer:
        </FMControls:FMLabel>
        <asp:dropdownlist id="ExceptionBOLPrinterDropDownList" style="Z-INDEX: 121; LEFT: 14px; POSITION: absolute; TOP: 277px"
                          runat="server" CssClass="formfield" Width="240px" tabIndex="4">
        </asp:dropdownlist>
        <!-- this div sets off the Accounting Settings section -->
        <div style="position: absolute; top: 330px; left: 0px; height: 142px; width: 320px; border: 1px; border-color: darkgray; border-style: solid">
        </div>
        <FMControls:FMLabel id="FMLabel8" style="Z-INDEX: 111; LEFT: 14px; POSITION: absolute; TOP: 346px; right: 1289px;"
                            runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px" Height="16px">
            Accounting Settings
        </FMControls:FMLabel>
        <FMCONTROLS:FMCHECKBOX id="EnableAdditiveAccountingCheckBox" style="Z-INDEX: 101; LEFT: 14px; POSITION: absolute; TOP: 367px"
                                tabIndex="4" runat="server" CssClass="formfieldtitle" BackColor="Transparent"
                                Text="Enable Additive Accounting">
        </FMCONTROLS:FMCHECKBOX>
        <FMControls:FMCheckBox ID="EnforceSingleOwner" Style="z-index: 111; left: 14px; position: absolute;
            top: 388px" TabIndex="14" runat="server" CssClass="formfieldtitle" BackColor="Transparent"
            Text="Enforce Single Owner" Visible="True"></FMControls:FMCheckBox>
        <FMCONTROLS:FMLABEL id="FMLABEL4" AssociatedControlID="InventoryTransactionDropDownList" 
                            style="Z-INDEX: 109; LEFT: 14px; POSITION: absolute; TOP: 409px; width: 147px;" runat="server"
                            CssClass="formfieldtitle" BackColor="Transparent">Inventory Transaction:</FMCONTROLS:FMLABEL>
        <fmcontrols:fmdropdownlist id="InventoryTransactionDropDownList" style="Z-INDEX: 102; LEFT: 182px; POSITION: absolute; TOP: 409px"
                                    tabIndex="33" runat="server" CssClass="formfield" Width="125px" Sort="false">
        </fmcontrols:fmdropdownlist>
        <FMCONTROLS:FMLABEL id="FMLABEL5" AssociatedControlID="AdjustmentTransactionDropDownList" 
                            style="Z-INDEX: 109; LEFT: 14px; POSITION: absolute; TOP: 430px; width: 159px;" runat="server"
                            CssClass="formfieldtitle" BackColor="Transparent">Adjustment Transaction:</FMCONTROLS:FMLABEL>
        <fmcontrols:fmdropdownlist id="AdjustmentTransactionDropDownList" style="Z-INDEX: 102; POSITION: absolute; TOP: 430px; left: 182px;"
                                    tabIndex="32" runat="server" CssClass="formfield" Width="125px" Sort="false">
        </fmcontrols:fmdropdownlist>
        <FMControls:FMLabel id="Label15" AssociatedControlID="OpenTransactionWindowDropDownList" runat="server" Width="160px" style="Z-INDEX: 102; LEFT: 14px; POSITION: absolute; TOP: 451px"
                            CssClass="formfieldtitle">
            Open Transaction Window:
        </FMControls:FMLabel>
        <asp:dropdownlist id="OpenTransactionWindowDropDownList" style="Z-INDEX: 121; LEFT: 182px; POSITION: absolute; TOP: 451px"
                            runat="server" CssClass="formfield" Width="73px" tabIndex="6">
        </asp:dropdownlist>
        <FMControls:FMLabel id="Label16" runat="server" Width="32px" style="Z-INDEX: 102; LEFT: 262px; POSITION: absolute; TOP: 451px"
                            CssClass="formfieldtitle">months</FMControls:FMLabel>
        <!-- this div sets off the Transaction Number Settings section -->
        <div style="position: absolute; top: 0px; left: 340px; height: 370px; width: 320px; border: 1px; border-color: darkgray; border-style: solid">
        </div>
        <FMControls:FMLabel id="TransactionNumberSettingsLabel" style="Z-INDEX: 111; LEFT: 354px; POSITION: absolute; TOP: 16px; right: 1289px;"
                            runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="192px" Height="16px">
            Transaction Number Settings
        </FMControls:FMLabel>
        <FMControls:FMLabel id="Label14" AssociatedControlID="NumberPrefixTextBox" runat="server" Width="88px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 41px"
                            CssClass="formfieldtitle">
            BOL # Prefix:
        </FMControls:FMLabel>
        <asp:textbox id="NumberPrefixTextBox" style="Z-INDEX: 110; LEFT: 529px; POSITION: absolute; TOP: 41px"
                     tabIndex="5" runat="server" CssClass="formfield" MaxLength="10" Width="72px">
        </asp:textbox>
        <FMControls:FMLabel id="Label4" AssociatedControlID="AutomaticBOLStartNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 62px"
                            CssClass="formfieldtitle">
            Automatic BOL Start #:
        </FMControls:FMLabel>
        <asp:textbox id="AutomaticBOLStartNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 62px"
                     tabIndex="11" runat="server" CssClass="formfield" Width="89px">
        </asp:textbox>
        <FMControls:FMLabel id="Label6" AssociatedControlID="AutomaticBOLEndNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 83px"
                            CssClass="formfieldtitle">
            Automatic BOL End #:
        </FMControls:FMLabel>
        <asp:textbox id="AutomaticBOLEndNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 83px"
                     tabIndex="12" runat="server" CssClass="formfield" Width="89px" AutoPostBack="True" ontextchanged="AutomaticBOLEndNumberTextBox_TextChanged">
        </asp:textbox>
        <FMControls:FMLabel id="Label7" AssociatedControlID="AutomaticBOLNextNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 104px"
                            CssClass="formfieldtitle">
            Automatic BOL Next #:
        </FMControls:FMLabel>
        <asp:textbox id="AutomaticBOLNextNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 104px"
                     tabIndex="13" runat="server" CssClass="formfield" Width="89px">
        </asp:textbox>
        <FMControls:FMCheckbox id="SeparateManualBOLNumberingCheckBox" Text="Separate Manual BOL Numbering" BackColor="Transparent"
                               CssClass="formfieldtitle" runat="server" style="Z-INDEX: 102; LEFT: 374px; POSITION: absolute; TOP: 125px"
                               AutoPostBack="True" oncheckedchanged="SeparateManualBOLNumberingCheckBox_CheckedChanged">
        </FMControls:FMCheckbox>
        <FMControls:FMLabel id="Label8" AssociatedControlID="ManualBOLStartNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 146px"
                            CssClass="formfieldtitle">
            Manual BOL Start #:
        </FMControls:FMLabel>
        <asp:textbox id="ManualBOLStartNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 146px"
                     tabIndex="14" runat="server" CssClass="formfield" Width="89px">
        </asp:textbox>
        <FMControls:FMLabel id="Label9" AssociatedControlID="ManualBOLEndNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 167px"
                            CssClass="formfieldtitle">
            Manual BOL End #:
        </FMControls:FMLabel>
        <asp:textbox id="ManualBOLEndNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 167px"
                     tabIndex="15" runat="server" CssClass="formfield" Width="89px" AutoPostBack="True" ontextchanged="ManualBOLEndNumberTextBox_TextChanged">
        </asp:textbox>
        <FMControls:FMLabel id="Label10" AssociatedControlID="ManualBOLNextNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 188px;"
                            CssClass="formfieldtitle">
            Manual BOL Next #:
        </FMControls:FMLabel>
        <asp:textbox id="ManualBOLNextNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 188px"
                     tabIndex="16" runat="server" CssClass="formfield" Width="89px">
        </asp:textbox>
        <FMControls:FMLabel id="Label11" AssociatedControlID="TransactionStartNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 219px"
                            CssClass="formfieldtitle">
            Transaction Start #:
        </FMControls:FMLabel>
        <asp:textbox id="TransactionStartNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 219px"
                     tabIndex="17" runat="server" CssClass="formfield" Width="89px">
        </asp:textbox>
        <FMControls:FMLabel id="Label12" AssociatedControlID="TransactionEndNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 240px"
                            CssClass="formfieldtitle">
            Transaction End #:
        </FMControls:FMLabel>
        <asp:textbox id="TransactionEndNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 240px"
                     tabIndex="18" runat="server" CssClass="formfield" Width="89px" AutoPostBack="True" ontextchanged="TransactionEndNumberTextBox_TextChanged">
        </asp:textbox>
        <FMControls:FMLabel id="Label13" AssociatedControlID="TransactionNextNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 261px"
                            CssClass="formfieldtitle">
            Transaction Next #:
        </FMControls:FMLabel>
        <asp:textbox id="TransactionNextNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 261px"
                     tabIndex="19" runat="server" CssClass="formfield" Width="89px">
        </asp:textbox>
        <FMControls:FMLabel id="Fmlabel1" AssociatedControlID="OrderStartNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 292px"
                            CssClass="formfieldtitle">
            Order Start #:
        </FMControls:FMLabel>
        <asp:textbox id="OrderStartNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 292px"
                     tabIndex="20" runat="server" CssClass="formfield" Width="89px">
        </asp:textbox>
        <FMControls:FMLabel id="Fmlabel2" AssociatedControlID="OrderEndNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 313px"
                            CssClass="formfieldtitle">
            Order End #:
        </FMControls:FMLabel>
        <asp:textbox id="OrderEndNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 313px"
                     tabIndex="21" runat="server" CssClass="formfield" Width="89px" AutoPostBack="True" ontextchanged="OrderEndNumberTextBox_TextChanged">
        </asp:textbox>
        <FMControls:FMLabel id="Fmlabel3" AssociatedControlID="OrderNextNumberTextBox" runat="server" Width="136px" style="Z-INDEX: 102; LEFT: 364px; POSITION: absolute; TOP: 334px"
                            CssClass="formfieldtitle">
            Order Next #:
        </FMControls:FMLabel>
        <asp:textbox id="OrderNextNumberTextBox" style="Z-INDEX: 110; LEFT: 514px; POSITION: absolute; TOP: 334px"
                     tabIndex="22" runat="server" CssClass="formfield" Width="89px">
        </asp:textbox>
    </body>
</HTML>