<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TransactionAliasGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.TransactionAliasGeneralPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

		<p>

		<FMCONTROLS:FMLABEL id="ModifyAssignedUserGroupsLabel" AssociatedControlID="ModifyAssignedUserGroupsListBox" style="Z-INDEX: 120; LEFT: 5px; POSITION: absolute; TOP: 275px; width: 233px; bottom: 98px;"
			runat="server" BackColor="Transparent" CssClass="formfieldtitle">Modify Assigned User Groups:</FMCONTROLS:FMLABEL>
		</p>
		
<p>

		<FMCONTROLS:FMLABEL id="ViewAssignedUserGroupsLabel" AssociatedControlID="ViewAssignedUserGroupsListBox" style="Z-INDEX: 120; LEFT: 5px; POSITION: absolute; TOP: 365px; width: 209px; bottom: 107px; right: 828px;"
			runat="server" BackColor="Transparent" CssClass="formfieldtitle">View Assigned User Groups:</FMCONTROLS:FMLABEL>
		</p>		
        <p>
            &nbsp;</p>
		<p>

		<FMCONTROLS:FMCHECKBOX id="BulkShipmentCheckBox" tabIndex="10" runat="server" CssClass="formfieldtitle" 
			BackColor="Transparent" Text="Bulk Shipment" 
                
                
                style="z-index:120; position: absolute; top:195px; left: 230px; right: 659px; width: 153px;"></FMCONTROLS:FMCHECKBOX>

		</p>
        <p>

		<FMCONTROLS:FMCHECKBOX id="MultipleLineItemCheckBox" tabIndex="12" runat="server" CssClass="formfieldtitle" 
		    style="z-index:120; position: absolute; top:220px; left: 230px"
			BackColor="Transparent" AutoPostBack="True" Text="Multiple Line Items" 
                oncheckedchanged="MultipleLineItemCheckBoxCheckedChanged"></FMCONTROLS:FMCHECKBOX>

		</p>
        <p>
            &nbsp;</p>
        <p>
            &nbsp;</p>
		<FMCONTROLS:FMLABEL id="IDLabel" AssociatedControlID="Identifier" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 16px" runat="server"
			CssClass="formfieldtitle" Width="24px">ID:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMLABEL id="IDRequiredLabel" style="Z-INDEX: 101; LEFT: 128px; POSITION: absolute; TOP: 16px"
			runat="server" Width="8px" ForeColor="Crimson" Height="8px">*</FMCONTROLS:FMLABEL>
		<asp:textbox id="Identifier" style="Z-INDEX: 101; LEFT: 152px; POSITION: absolute; TOP: 16px" aria-required="true"
			tabIndex="1" runat="server" CssClass="formfield" Width="156px" MaxLength="30"></asp:textbox>
		<input type="hidden" id="HiddenID">
			
		<FMCONTROLS:FMLABEL id="AliasLabel" AssociatedControlID="AliasDropDown" style="Z-INDEX: 125; LEFT: 344px; POSITION: absolute; TOP: 19px"
			runat="server" CssClass="formfieldtitle">Associated Transaction</FMCONTROLS:FMLABEL>
		<asp:dropdownlist id="AliasDropDown" style="Z-INDEX: 125; LEFT: 496px; POSITION: absolute; TOP: 16px"
			tabIndex="2" runat="server" CssClass="formfield" Width="160px"></asp:dropdownlist>
			
		<FMCONTROLS:FMLABEL id="Label4"  AssociatedControlID="TransactionTypeDropDownList"
			
			style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 44px; right: 935px; width: 110px;" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Transaction Type:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMDROPDOWNLIST id="TransactionTypeDropDownList" style="Z-INDEX: 101; LEFT: 152px; POSITION: absolute; TOP: 44px"
			tabIndex="3" runat="server" CssClass="formfield" Width="344px" 
			onchange="TransactionTypeDropDownListChange()" 
			onselectedindexchanged="TransactionTypeDropDownListSelectedIndexChanged"></FMCONTROLS:FMDROPDOWNLIST>
		<FMCONTROLS:FMLABEL id="lblShowCompanyName" AssociatedControlID="ShowCompanyNameDropDownList" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 76px"
			runat="server" CssClass="formfieldtitle" BackColor="Transparent">Show Company Name:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMDROPDOWNLIST id="ShowCompanyNameDropDownList" style="Z-INDEX: 101; LEFT: 152px; POSITION: absolute; TOP: 74px"
			runat="server" Width="344px" tabIndex="4" CssClass="formfield"></FMCONTROLS:FMDROPDOWNLIST>

		<FMCONTROLS:FMLABEL id="ReportLabel" AssociatedControlID="ReportTextBox" style="Z-INDEX: 125; LEFT: 0px; POSITION: absolute; TOP: 107px; right: 1184px;"
			runat="server" CssClass="formfieldtitle">Report:</FMCONTROLS:FMLABEL>
		<asp:textbox id="ReportTextBox" style="Z-INDEX: 125; LEFT: 152px; POSITION: absolute; TOP: 104px; right: 729px;"
			tabIndex="5" runat="server" CssClass="formfield" Width="344px"></asp:textbox>
		<fmcontrols:fmbutton id="ReportSetButton" style="Z-INDEX: 125; LEFT: 504px; POSITION: absolute; TOP: 98px; width: 32px; height:25px;"
			runat="server" CssClass="formfield" Text="Set" onclick="ReportSetButtonClick" 
					TabIndex="6"></fmcontrols:fmbutton>
		<asp:dropdownlist id="ReportDropDown" style="Z-INDEX: 125; LEFT: 152px; POSITION: absolute; TOP: 104px; right: 729px;"
			tabIndex="5" runat="server" AutoPostBack="True" CssClass="formfield" Width="344px" 
					onselectedindexchanged="ReportDropDownSelectedIndexChanged"></asp:dropdownlist>

		<FMCONTROLS:FMLABEL id="PreLoadReportLabel" AssociatedControlID="PreLoadReportTextbox" style="Z-INDEX: 125; LEFT: 0px; POSITION: absolute; TOP: 140px"
			runat="server" CssClass="formfieldtitle" Width="136px">Preload Report:</FMCONTROLS:FMLABEL>
		<asp:dropdownlist id="PreLoadReportDropDown" style="Z-INDEX: 125; LEFT: 152px; POSITION: absolute; TOP: 135px; right: 729px;"
			tabIndex="7" runat="server" Width="344px" CssClass="formfield" AutoPostBack="True" 
					onselectedindexchanged="PreLoadReportDropDownSelectIndexChanged"></asp:dropdownlist>
		<fmcontrols:fmbutton id="PreLoadReportSetButton" style="Z-INDEX: 125; LEFT: 504px; POSITION: absolute; TOP: 131px; right: 689px; width:32px; height:25px;"
			runat="server" CssClass="formfield" Text="Set" 
			onclick="PreLoadReportSetButtonClick" TabIndex="8"></fmcontrols:fmbutton>
		<asp:textbox id="PreLoadReportTextbox" tabIndex="7" style="Z-INDEX: 125; LEFT: 152px; POSITION: absolute; TOP: 135px"
			runat="server" Width="344px" CssClass="formfield" ToolTip="Preload Report textbox"></asp:textbox>
		
		<FMCONTROLS:FMCHECKBOX id="MeterCloseoutCheckBox" tabIndex="9" runat="server" CssClass="formfieldtitle" 
			BackColor="Transparent" Text="Meter Closeout" style="z-index:120; position: absolute; top:170px; left: 0px"></FMCONTROLS:FMCHECKBOX>

		<FMCONTROLS:FMCHECKBOX id="DistributedImpactCheckBox" tabIndex="11" runat="server" CssClass="formfieldtitle" 
			BackColor="Transparent" Text="Distributed Impact" style="z-index:120; position: absolute; top:195px; left: 0px"></FMCONTROLS:FMCHECKBOX>

		<FMCONTROLS:FMCHECKBOX id="MultipleWeightReadingCheckBox" tabIndex="13" runat="server" CssClass="formfieldtitle" 
		    style="z-index:120; position: absolute; top:220px; left: 0px"
			BackColor="Transparent" AutoPostBack="True" Text="Multiple Weight Readings" oncheckedchanged="MultipleGaugeReadingCheckBoxCheckedChanged"></FMCONTROLS:FMCHECKBOX>

		<FMCONTROLS:FMCHECKBOX id="MultipleTransportLineItemCheckBox" tabIndex="14" 
            runat="server" CssClass="formfieldtitle" 
		    style="z-index:120; position: absolute; top:247px; left: 229px"
			BackColor="Transparent" AutoPostBack="True" Text="Multiple Transport Line Items" 
            oncheckedchanged="MultipleTransportLineItemCheckBoxCheckedChanged"></FMCONTROLS:FMCHECKBOX>

		<FMCONTROLS:FMCHECKBOX id="LimitSelectionsBasedOnHierarchyCheckBox" 
            tabIndex="15" runat="server" CssClass="formfieldtitle" 
		    style="z-index:120; position: absolute; top:170px; left: 230px"
			BackColor="Transparent" Text="Limit Selections Based On Hierarchy" 
            ></FMCONTROLS:FMCHECKBOX>

		<FMCONTROLS:FMCHECKBOX id="UseComboBoxControlsCheckBox" tabIndex="16" 
            runat="server" CssClass="formfieldtitle" 
		    style="z-index:120; position: absolute; top:195px; left: 490px"
			BackColor="Transparent" Text="Use Combo Box Controls" AutoPostBack="True" 
            oncheckedchanged="UseComboBoxControlsCheckBoxCheckedChanged"></FMCONTROLS:FMCHECKBOX>

		<FMCONTROLS:FMCHECKBOX id="PermitNonReferenceDataCheckBox" tabIndex="17" 
            runat="server" CssClass="formfieldtitle" 
		    style="z-index:120; position: absolute; top:171px; left: 490px"
			BackColor="Transparent" Text="Permit Non Reference Data" AutoPostBack="True" 
            oncheckedchanged="PermitNonReferenceDataCheckBoxCheckedChanged"></FMCONTROLS:FMCHECKBOX>

        <FMCONTROLS:FMCHECKBOX id="IncludeInDispatchCheckBox" tabIndex="18" 
            runat="server" CssClass="formfieldtitle" 
		    style="z-index:120; position: absolute; top:245px; left: 0px"
			BackColor="Transparent" Text="Include In Dispatch"></FMCONTROLS:FMCHECKBOX>
        
        <FMCONTROLS:FMCHECKBOX id="EnableAutoCompleteCheckBox" tabIndex="18" 
            runat="server" CssClass="formfieldtitle" 
		    style="z-index:120; position: absolute; top:220px; left: 490px"
			BackColor="Transparent" Text="Use Auto Complete Controls" AutoPostBack="True" 
            oncheckedchanged="EnableAutoCompleteCheckBoxCheckedChanged"></FMCONTROLS:FMCHECKBOX>

		<asp:listbox id="ViewAssignedUserGroupsListBox" style="Z-INDEX: 121; LEFT: 0px; POSITION: absolute; TOP: 385px; bottom: 14px; right: 802px;"
			tabIndex="18" runat="server" BackColor="White" Width="240px" CssClass="formfield" Height="65px"
			SelectionMode="Multiple"></asp:listbox>
		<asp:listbox id="ViewUnassignedUserGroupsListBox" style="Z-INDEX: 124; LEFT: 288px; POSITION: absolute; TOP: 385px"
			tabIndex="21" runat="server" BackColor="White" Width="240px" CssClass="formfield" Height="65px"
			SelectionMode="Multiple"></asp:listbox>
			
		<FMCONTROLS:FMButton id="ViewAssignGroupsButton" style="Z-INDEX: 122; LEFT: 251px; POSITION: absolute; TOP: 385px; width:20px; height:25px;"
			tabIndex="19" runat="server" CssClass="formfieldtitle" Text="<<"></FMCONTROLS:FMButton>
		<FMCONTROLS:FMButton id="ViewUnassignGroupsButton" style="Z-INDEX: 123; LEFT: 251px; POSITION: absolute; TOP: 422px; width:20px; height:25px;"
			tabIndex="20" runat="server" CssClass="formfieldtitle" Text=">>"></FMCONTROLS:FMButton>

		<asp:listbox id="ModifyAssignedUserGroupsListBox" style="Z-INDEX: 121; LEFT: 0px; POSITION: absolute; TOP: 295px; bottom: 122px;"
			tabIndex="18" runat="server" BackColor="White" Width="240px" CssClass="formfield" Height="65px"
			SelectionMode="Multiple"></asp:listbox>
		<FMCONTROLS:FMButton id="ModifyAssignGroupsButton" style="Z-INDEX: 122; LEFT: 251px; POSITION: absolute; TOP: 295px; width:20px; height:25px;"
			tabIndex="19" runat="server" CssClass="formfieldtitle" Text="<<"></FMCONTROLS:FMButton>
		<FMCONTROLS:FMButton id="ModifyUnassignGroupsButton" style="Z-INDEX: 123; LEFT: 251px; POSITION: absolute; TOP: 332px; width:20px; height:25px;"
			tabIndex="20" runat="server" CssClass="formfieldtitle" Text=">>"></FMCONTROLS:FMButton>
		<FMCONTROLS:FMLABEL id="ModifyUnassignedUserGroupsLabel" AssociatedControlID="ModifyUnassignedUserGroupsListBox" style="Z-INDEX: 112; LEFT: 288px; POSITION: absolute; TOP: 275px; width: 235px;"
			runat="server" CssClass="formfieldtitle">Modify Unassigned User Groups:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMLABEL id="ViewUnassignedUserGroupsLabel" AssociatedControlID="ViewUnassignedUserGroupsListBox" style="Z-INDEX: 112; LEFT: 288px; POSITION: absolute; TOP: 365px; width: 234px;"
			runat="server" CssClass="formfieldtitle">View Unassigned User Groups:</FMCONTROLS:FMLABEL>
			
		<asp:listbox id="ModifyUnassignedUserGroupsListBox" style="Z-INDEX: 124; LEFT: 288px; POSITION: absolute; TOP: 295px; right: 514px;"
			tabIndex="21" runat="server" BackColor="White" Width="240px" CssClass="formfield" Height="65px"
			SelectionMode="Multiple"></asp:listbox>
        

