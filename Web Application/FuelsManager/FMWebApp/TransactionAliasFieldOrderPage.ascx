<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="TransactionAliasFieldOrderPage.ascx.cs"
	Inherits="FuelsManager.FMWebApp.TransactionAliasFieldOrderPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

	<FMControls:FMLabel ID="sectionTypeLabel" AssociatedControlID="sectionTypeDropDownList" Style="z-index: 101; left: 0px; position: absolute;
		top: 16px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Transaction Section:</FMControls:FMLabel>
	<FMControls:FMDropDownList ID="sectionTypeDropDownList" Style="z-index: 102; left: 144px;
		position: absolute; top: 16px" runat="server" Width="176px" CssClass="formfield"
		AutoPostBack="True" TabIndex="1" OnSelectedIndexChanged="SectionTypeDropDownListSelectedIndexChanged">
	</FMControls:FMDropDownList>
	<FMControls:FMLabel ID="fieldListLabel" AssociatedControlID="fieldsListBox" Style="z-index: 104; left: 72px; position: absolute;
		top: 64px; height: 15px;" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Field Order List:</FMControls:FMLabel>
	<asp:ListBox ID="fieldsListBox" runat="server" Style="z-index: 99; left: 64px; position: absolute;
		top: 86px; right: 1605px;" Width="176px" Height="280px" CssClass="formfield"
		SelectionMode="Multiple" TabIndex="2"></asp:ListBox>
	<FMControls:FMButton ID="upButton" Style="z-index: 138; left: 256px; position: absolute;
		top: 110px; min-width: 55px" runat="server" CssClass="formfieldtitle" Text="Up" TabIndex="3"
		OnClick="UpButtonOnClick"></FMControls:FMButton>
	<FMControls:FMButton ID="downButton" Style="z-index: 137; left: 256px; position: absolute;
		top: 158px; padding-left: 1px; min-width: 55px" runat="server" CssClass="formfieldtitle" 
		Text="Down" TabIndex="4" OnClick="DownButtonOnClick"></FMControls:FMButton>
	<FMControls:FMLabel ID="dispatchFieldListLabel" AssociatedControlID="dispatchFieldsListBox" Style="z-index: 104; left: 422px;
		position: absolute; top: 64px" runat="server" CssClass="formfieldtitle" BackColor="Transparent" visible="False">Dispatch Field Order List:</FMControls:FMLabel>
	<asp:ListBox ID="dispatchFieldsListBox" runat="server" Style="z-index: 99; left: 414px;
		position: absolute; top: 86px" Width="176px" Height="280px" CssClass="formfield"
		SelectionMode="Multiple" TabIndex="5" visible="False"></asp:ListBox>
	<FMControls:FMButton ID="dispatchUpButton" Style="z-index: 138; left: 606px; position: absolute;
		top: 110px" runat="server" CssClass="formfieldtitle" Width="40px" Text="Up" TabIndex="6"
		OnClick="UpButtonOnClick" visible="False"></FMControls:FMButton>
	<FMControls:FMButton ID="dispatchDownButton" Style="z-index: 137; left: 606px; position: absolute;
		top: 158px; padding-left: 1px" runat="server" CssClass="formfieldtitle" Width="40px"
		Text="Down" TabIndex="7" OnClick="DownButtonOnClick" visible="False"></FMControls:FMButton>

