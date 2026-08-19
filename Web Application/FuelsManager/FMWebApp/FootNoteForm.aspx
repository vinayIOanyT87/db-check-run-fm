
<%@ Page Language="c#" CodeBehind="FootNoteForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.FootNoteForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
      <title></title>
      <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
      <meta name="CODE_LANGUAGE" content="C#">
      <meta name="vs_defaultClientScript" content="JavaScript">
      <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
      <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" />
      <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
      <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js"%>" ></script>
      <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
      <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
      <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
		<SCRIPT>
			function EntitySelect(entityTextBoxId, mode)
			{
				var entityTextBox = document.getElementById(entityTextBoxId);
				var typeDropDownList = document.getElementById("TypeDropDownList");

				if (typeDropDownList.value === "0")
				{
				    showModalDialogFrame({
				        url: "../FMWebApp/CompanySelectForm.aspx?Role=CUSTOMER_SHIPTO&Map=FOOT_NOTE_SHIPTO&Mode=" + mode + "&All=true",
				        width: 855,
				        height: 690,
				        title: "Company Select",
				        onClose: function ()
				        {
							  if (this.returnValue != null) {
								  var result = this.returnValue;
								  if (result != null && result.length > 0) {
									  for (var i = 0; i < result.length; i++) {
										  var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

										  if (i === 0) {
											  entityTextBox.value = newAsciiStr;
										  }
										  else {
											  entityTextBox.value += "|" + newAsciiStr;
										  }
									  }

									  entityTextBox.onchange();
								  }
							  }
				        }
				    });
				}
				else if (typeDropDownList.value === "1")
				{
				    showModalDialogFrame({
				        url: "../FMWebApp/CompanySelectForm.aspx?Role=SHIPPER&Map=FOOT_NOTE_SHIPPER&Mode=" + mode + "&All=true",
				        width: 855,
				        height: 690,
				        title: "Company Select",
				        onClose: function ()
				        {
							  if (this.returnValue != null) {
								  var result = this.returnValue;
								  if (result != null && result.length > 0) {
									  for (var i = 0; i < result.length; i++) {
										  var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

										  if (i === 0) {
											  entityTextBox.value = newAsciiStr;
										  }
										  else {
											  entityTextBox.value += "|" + newAsciiStr;
										  }
									  }

									  entityTextBox.onchange();
								  }
							  }
				        }
				    });
				}
				else if (typeDropDownList.value === "2")
				{
				    showModalDialogFrame({
				        url: "../FMWebApp/StateSelectForm.aspx?Mode=" + mode,
				        width: 855,
				        height: 690,
				        title: "State Select",
				        onClose: function ()
				        {
							  if (this.returnValue != null) {
								  var result = this.returnValue;
								  if (result != null && result.length > 0) {
									  for (var i = 0; i < result.length; i++) {
										  var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

										  if (i === 0) {
											  entityTextBox.value = newAsciiStr;
										  }
										  else {
											  entityTextBox.value += "|" + newAsciiStr;
										  }
									  }

									  entityTextBox.onchange();
								  }
							  }
				        }
				    });
				}
				else if (typeDropDownList.value === "3")
				{
				    showModalDialogFrame({
				        url: "../FMWebApp/ProductSelectForm.aspx?Map=FOOT_NOTE_PRODUCT&Mode=" + mode + "&All=true",
				        width: 855,
				        height: 690,
				        title: "Product Select",
				        onClose: function ()
				        {
							  if (this.returnValue != null) {
								  var result = this.returnValue;
								  if (result != null && result.length > 0) {
									  for (var i = 0; i < result.length; i++) {
										  var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

										  if (i === 0) {
											  entityTextBox.value = newAsciiStr;
										  }
										  else {
											  entityTextBox.value += "|" + newAsciiStr;
										  }
									  }

									  entityTextBox.onchange();
								  }
							  }
				        }
				    });
				}
				else if (typeDropDownList.value === "4") {
				    showModalDialogFrame({
				        url: "../FMWebApp/AdditiveProfileSelectForm.aspx?Mode=" + mode, 
				        width: 855,
				        height: 690,
				        title: "Additive Profile Select",
				        onClose: function () {
							  if (this.returnValue != null) {
								  var result = this.returnValue;
								  if (result != null && result.length > 0) {
									  for (var i = 0; i < result.length; i++) {
										  var newAsciiStr = ReplaceNonBreakingSpaceHexWithSpace(result[i]);

										  if (i === 0) {
											  entityTextBox.value = newAsciiStr;
										  }
										  else {
											  entityTextBox.value += "|" + newAsciiStr;
										  }
									  }

									  entityTextBox.onchange();
								  }
							  }
				        }
				    });
				}
			}
</SCRIPT>
</HEAD>
	<body MS_POSITIONING="GridLayout">
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label6" Style="z-index: 101; left: 8px; position: absolute; top: 8px" runat="server"
                    BackColor="Transparent" CssClass="headline" Width="272px">Footnote Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label1" AssociatedControlID="Name" Style="z-index: 102; left: 8px; position: absolute; top: 40px" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">Footnote:</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label8" Style="z-index: 104; left: 72px; position: absolute; top: 40px" runat="server"
                    BackColor="Transparent" Height="8px" ForeColor="Crimson" Width="8px">*</FMControls:FMLabel>
                <FMControls:FMTextBox ID="Name" Style="z-index: 103; left: 96px; position: absolute; top: 40px; resize: none;" runat="server"
                    BackColor="White" CssClass="formfield" Width="625px" MaxLength="250" TabIndex="1" Height="80px" aria-required="true" TextMode="MultiLine"/>
                <FMControls:FMLabel ID="startDateLabel" Style="z-index: 125; left: 8px; position: absolute; top: 146px" runat="server"
                    CssClass="formfieldtitle" Width="64px" BackColor="Transparent">Start Date:</FMControls:FMLabel>
                <FMControls:FMDate ID="startDate" Style="z-index: 150; left: 96px; position: absolute; top: 146px"
                    TabIndex="2" runat="server" Width="160px" CssClass="formfield" Height="25px"></FMControls:FMDate>
                <FMControls:FMLabel ID="endDateLabel" Style="z-index: 125; left: 308px; position: absolute; top: 146px" runat="server"
                    CssClass="formfieldtitle" Width="64px" BackColor="Transparent">End Date:</FMControls:FMLabel>
                <FMControls:FMDate ID="endDate" Style="z-index: 150; left: 396px; position: absolute; top: 146px"
                    TabIndex="3" runat="server" Width="160px" CssClass="formfield" Height="25px"></FMControls:FMDate>
                <FMControls:FMLabel ID="Fmlabel2" Style="z-index: 125; left: 8px; position: absolute; top: 190px" runat="server"
                    CssClass="formfieldtitle" Width="64px" BackColor="Transparent">Assigned:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="TypeDropDownList" Style="z-index: 111; left: 96px; position: absolute; top: 189px"
                    TabIndex="4" runat="server" CssClass="formfield" Width="240px" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged">
                </FMControls:FMDropDownList>
                <table id="Table1" style="z-index: 113; left: 96px; position: absolute; top: 226px; height: 10px"
                    cellspacing="0" cellpadding="1" width="350" border="0">
                    <tr>
                        <td width="368" height="10">
                            <FMControls:FMDataGrid ID="AssignedEntitiesDataGrid" TabIndex="5" runat="server" CssClass="tabletext" Height="10px"
                                Width="400px" BackColor="White" PageSize="12" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
                                GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None">
                                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="ID">
                                        <HeaderStyle Width="3in"></HeaderStyle>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                        <ItemTemplate>
                                            <asp:Label Width="2.5in" runat="server" ID="IDLabel"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                            </FMControls:FMDataGrid></td>
                    </tr>
                    <tr>
                        <td width="368" height="36">
                            <table>
                                <tr>
                                    <td width="84" height="10">
                                        <input class="formfieldtitle" id="FootNoteForm_AssignButton" style="width: 80px" onclick="EntitySelect('AssignEntitiesTextBox', 'Assign')"
                                            type="button" value="Assign" TabIndex="6"></td>
                                    <td height="10">
                                        <input class="formfieldtitle" id="FootNoteForm_UnassignButton" style="width: 80px" onclick="EntitySelect('UnassignEntitiesTextBox', 'Unassign')"
                                            type="button" value="Unassign" TabIndex="7"></td>
                                    <td>
                                        <asp:TextBox ID="AssignEntitiesTextBox" runat="server" Width="82px" BackColor="Transparent" BorderColor="Transparent"
                                            BorderStyle="None" AutoPostBack="True" ForeColor="White" OnTextChanged="AssignEntitiesTextBoxTextChanged"></asp:TextBox></td>
                                    <td>
                                        <asp:TextBox ID="UnassignEntitiesTextBox" runat="server" Width="82px" BackColor="Transparent" BorderColor="Transparent"
                                            BorderStyle="None" AutoPostBack="True" ForeColor="White" OnTextChanged="UnassignEntitiesTextBoxTextChanged"></asp:TextBox></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <FMControls:FMButton ID="OK" Style="z-index: 111; left: 576px; position: absolute; top: 496px" runat="server"
                    CssClass="formfieldtitle" Width="67px" Text="OK" TabIndex="100"></FMControls:FMButton>
                <FMControls:FMButton ID="Cancel" Style="z-index: 112; left: 664px; position: absolute; top: 496px" runat="server"
                    CssClass="formfieldtitle" Width="67px" Text="Cancel" TabIndex="101"></FMControls:FMButton>
                <FMControls:FMLabel ID="Label10" Style="z-index: 113; left: 584px; position: absolute; top: 528px" runat="server"
                    CssClass="formfieldtitle" Height="8px" ForeColor="Crimson" Width="144px">* Denotes Required Field</FMControls:FMLabel>
            </div>
        </form>
	</body>
</HTML>
