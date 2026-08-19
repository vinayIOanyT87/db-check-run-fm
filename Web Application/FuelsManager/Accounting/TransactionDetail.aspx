<%@ Page Language="c#" CodeBehind="TransactionDetail.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.TransactionDetailBase" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title></title>
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />

	<!-- inside a server side block (the head block in this case), the asp code blocks are not allowed and are not expected to work.  This breaks the Transaction Detail
		when configured for combo boxes.  The cure for this while preserving the ability to generate app-rooted links in to add or update the script and link elements in the
		code behind -->
	<link runat="server" id="cssFuelsManager" href="~/CSS/FuelsManager.css" rel="stylesheet" type="text/css" />
	<link runat="server" id="cssJQueryUi" href="~/Javascripts/jquery-ui-1.10.3.custom/css/ui-lightness/jquery-ui-1.10.3.custom.css" rel="stylesheet" type="text/css" />
	<link runat="server" id="cssDispatchUi" href="~/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" rel="stylesheet" type="text/css" />

	<script type="text/javascript">
		var transactionDetailEndRequestRegistered = false;

		function initializeTransactionDetailPage()
		{
			// This is important so the autocomplete controls continue to work after
			// a postback through UpdatePanel; otherwise, the autocomplete controls
			// will stop working after the postback.
			if (!transactionDetailEndRequestRegistered &&
				window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager)
			{
				var prm = Sys.WebForms.PageRequestManager.getInstance();
				if (prm != null)
				{
					prm.add_endRequest(EndRequest);
					transactionDetailEndRequestRegistered = true;
				}
			}

			// Initialize the autocomplete controls.
			FMControlsLib.InitializeAutoComplete();
			$("#UpdatePanel1").addClass( $("#quickLinksPanelState").attr( "value" ) );
		}

		function hideWaitDiv()
		{
			var waitImage = document.getElementById("waitDiv");
			if (waitImage != null)
			{
				waitImage.style.display = "none";
			}
		}

		function EndRequest()
		{
			// Always remove the Please Wait message, even if later initialization fails.
			hideWaitDiv();

			// Re-initialize the autocomplete controls after update panel postback.
			FMControlsLib.InitializeAutoComplete();
			resizeRequest();
		}

		function resizeRequest()
		{
			var panel = document.getElementById('UpdatePanel1');
			var lineItemDataGrid = document.getElementById('LineItemDataGrid');
			var fieldRowDataGrid = document.getElementById('FieldRowCell');
			var gaugeReadingsDataGrid = document.getElementById('GaugeReadingsDataGrid');
			var transportDataGrid = document.getElementById('TransportDataGrid');
			var panelWidth = 0;

			if (lineItemDataGrid != null)
			{
				panelWidth = Math.max(lineItemDataGrid.clientWidth, lineItemDataGrid.offsetWidth, panelWidth);
			}

			if (fieldRowDataGrid != null)
			{
				panelWidth = Math.max(fieldRowDataGrid.clientWidth, fieldRowDataGrid.offsetWidth, panelWidth);
			}

			if (gaugeReadingsDataGrid != null)
			{
				panelWidth = Math.max(gaugeReadingsDataGrid.clientWidth, gaugeReadingsDataGrid.offsetWidth, panelWidth);
			}

			if (transportDataGrid != null)
			{
				panelWidth = Math.max(transportDataGrid.clientWidth, transportDataGrid.offsetWidth, panelWidth);
			}

			panelWidth += 100;

			if (panel != null)
			{
				panelWidth = Math.max(panel.clientWidth, panel.offsetWidth, panelWidth);
				panel.style.width = panelWidth + 'px';
			}
		}

		function onLoadHandler()
		{
			initializeTransactionDetailPage();
			window.onresize = resizeRequest;
			resizeRequest();
		}
    </script>


	<style>
		#UpdatePanel1 {
			width: calc( 100vw - 20px ) !important;
		}

		#UpdatePanel1.collapsed #scrollingarea {
			overflow: auto; 
			height: calc( 100vh - 170px);
		}

		#UpdatePanel1.expanded #scrollingarea {
			overflow: auto; 
			height: calc( 100vh - 200px);
		}
    </style>

</head>
<body ms_positioning="GridLayout" xmlns:fmcontrols="urn:http://schemas.varec.com/FMControls" onresize="resizeRequest();" onload="onLoadHandler();">
	<form id="Form1" method="post" runat="server" submitdisabledcontrols="true" onsubmit="formSubmit();">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: relative; height: 100%; padding: 10px 15px">
			<asp:ScriptManager ID="ScriptManager" runat="server" />
			<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<input type="hidden" name="__MYEVENTTARGET">
					<input type="hidden" name="__MYEVENTARGUMENT">
					<asp:Button ID="EnterKeyButton" Style="z-index: -111; left: 8px; position: absolute; top: 0px"
						runat="server" Height="0px" ForeColor="Transparent" BorderStyle="None" Width="0px" BackColor="Transparent" OnClick="EnterKeyButtonClick"></asp:Button>
					<input id="EnterKeySource" style="left: 24px; top: 16px" type="hidden" runat="server">
					<asp:Image ID="FadeImage" Style="z-index: -101; left: 0px; position: absolute; top: 0px" runat="server"
						BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
					<div id="scrollingarea">
						<table id="TransDetailTable" style="z-index: 104; left: 24px; top: 39px;" runat="server" role="presentation" aria-label="layout">
							<tr>
								<td colspan="5">
									<fmcontrols:fmlabel id="lblPageTitle" runat="server"
										cssclass="headline" width="500px" backcolor="Transparent"></fmcontrols:fmlabel>
								</td>
							</tr>
							<tr id="PreviousNextRow">
								<td align="left">
									<asp:UpdatePanel ID="NextPreviousButtonPanel" runat="server" UpdateMode="Conditional">
										<ContentTemplate>
											<fmcontrols:fmbutton id="PreviousButton" style="z-index: 101" runat="server"
												width="152px" text="<< Previous Transaction" onclick="PreviousButtonClick" cssclass="formfieldtitle" />
											&nbsp;&nbsp;
											<fmcontrols:fmbutton id="NextButton" style="z-index: 103" runat="server" width="136px"
												text="Next Transaction >>" onclick="NextButtonClick" cssclass="formfieldtitle" />
										</ContentTemplate>
									</asp:UpdatePanel>
								</td>
								<td align="left"></td>
								<td align="left"></td>
							</tr>
							<tr id="FieldRow">
								<td id="FieldRowCell" colspan="2">
									<asp:Table ID="FieldTable" runat="server"></asp:Table>
								</td>
								<td></td>
							</tr>
							<tr id="GaugeReadingsRow">
								<td>
									<asp:UpdatePanel ID="GuageReadingsPanel" runat="server" UpdateMode="Conditional">
										<ContentTemplate>
											<fmcontrols:fmbasedatagrid onkeypress="javascript:DataGridKeyPress('AGR');" id="GaugeReadingsDataGrid" style="left: 1px; top: 0px"
												tabindex="1" runat="server" borderstyle="None" backcolor="White" pagesize="8" cssclass="tabletext"
												cellpadding="3" bordercolor="#999999" allowsorting="True" borderwidth="1px" gridlines="Vertical"
												autogeneratecolumns="False" aria-label="Gauge Readings">
												<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
												<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
												<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
												<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
												<Columns>
													<asp:TemplateColumn HeaderText="Edit">
														<HeaderStyle Width="55px"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<fmcontrols:fmeditlinkbutton ID="EditButton2" runat="server" />
														</ItemTemplate>
														<EditItemTemplate>
															<fmcontrols:fmupdatelinkbutton ID="UpdateButton2" runat="server" />&nbsp;
															<fmcontrols:fmcancellinkbutton ID="CancelButton2" runat="server" />
														</EditItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn HeaderText="Delete">
														<HeaderStyle Width="0.5in"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<fmcontrols:fmdeletelinkbutton runat="server" ID="DeleteButton2" />
														</ItemTemplate>
													</asp:TemplateColumn>
												</Columns>
												<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
											</fmcontrols:fmbasedatagrid>
											<fmcontrols:fmbutton id="NewAGRButton" style="z-index: 108; left: 0px; top: 0px"
												runat="server" text="Add" onclick="AgrNewButtonClick" cssclass="formfieldtitle" />
										</ContentTemplate>
									</asp:UpdatePanel>
								</td>
								<td></td>
							</tr>
							<tr id="LineItemPageRow">
								<td colspan="3">
									<asp:UpdatePanel ID="LineItemsPanel" runat="server" UpdateMode="Conditional">
										<ContentTemplate>
											<fmcontrols:fmbasedatagrid onkeypress="javascript:DataGridKeyPress('LineItem');" id="LineItemDataGrid" style="left: 1px; top: 0px"
												tabindex="1" runat="server" borderstyle="None" backcolor="White" pagesize="8" cssclass="tabletext" cellpadding="3"
												bordercolor="#999999" allowsorting="True" borderwidth="1px" gridlines="Vertical" autogeneratecolumns="False" aria-label="Line items">
												<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
												<SelectedItemStyle Wrap="False"></SelectedItemStyle>
												<EditItemStyle CssClass="tabletext"></EditItemStyle>
												<AlternatingItemStyle CssClass="tabletext" BackColor="Gainsboro"></AlternatingItemStyle>
												<ItemStyle Wrap="False" ForeColor="Black" CssClass="tabletext" BackColor="#EEEEEE"></ItemStyle>
												<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
												<Columns>
													<asp:TemplateColumn HeaderText="Add Subline-item">
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<fmcontrols:fmaddsublineitemlinkbutton runat="server" />
														</ItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn HeaderText="Edit">
														<HeaderStyle Width="55px"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<fmcontrols:fmeditlinkbutton runat="server" />
														</ItemTemplate>
														<EditItemTemplate>
															<fmcontrols:fmupdatelinkbutton runat="server" />&nbsp;
															<fmcontrols:fmcancellinkbutton runat="server" />
														</EditItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn HeaderText="Delete">
														<HeaderStyle Width="0.5in"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<fmcontrols:fmdeletelinkbutton runat="server" />
														</ItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn HeaderText="Transactions">
														<HeaderStyle Width="0.5in"></HeaderStyle>
														<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<fmcontrols:fmviewassociatedtxlinkbutton id="FMViewAssociatedTxLinkButton1" 
																runat="server"></fmcontrols:fmviewassociatedtxlinkbutton>&nbsp;&nbsp;
															<fmcontrols:fmaddassociatedtxlinkbutton id="lbAddAssociatedTx1" runat="server"></fmcontrols:fmaddassociatedtxlinkbutton>
															<fmcontrols:fmelipsebutton ID="btnAddAssocTx1" Runat="server" 
																CssClass="formfieldtitle" Enabled="false"></fmcontrols:fmelipsebutton>
														</ItemTemplate>
														<EditItemTemplate>
															<fmcontrols:fmviewassociatedtxlinkbutton id="FMViewAssociatedTxLinkButton2" 
																runat="server"></fmcontrols:fmviewassociatedtxlinkbutton>&nbsp;&nbsp;
															<fmcontrols:fmaddassociatedtxlinkbutton id="lbAddAssociatedTx2" Enabled="false" 
																runat="server"></fmcontrols:fmaddassociatedtxlinkbutton>
															<fmcontrols:fmelipsebutton ID="btnAddAssocTx2" Runat="server" 
																CssClass="formfieldtitle" Enabled="true"></fmcontrols:fmelipsebutton>
														</EditItemTemplate>
													</asp:TemplateColumn>
												</Columns>
												<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
											</fmcontrols:fmbasedatagrid>
											<fmcontrols:fmbutton id="NewLineItemButton" style="z-index: 107; left: 0px; top: 0px; width: 100px; height: 22px; margin-top:10px"
												runat="server" text="Add" onclick="NewLineItemButtonClick" cssclass="formfieldtitle" />
										</ContentTemplate>
									</asp:UpdatePanel>
								</td>
								<td></td>
							</tr>
							<tr id="TransportLineItemPageRow">
								<td colspan="2">
									<asp:UpdatePanel ID="TransportLineItemPanel" runat="server" UpdateMode="Conditional">
										<ContentTemplate>
											<fmcontrols:fmbasedatagrid onkeypress="javascript:DataGridKeyPress('TransportLineItem');" id="TransportDataGrid" style="left: 1px; top: 0px"
												tabindex="1" runat="server" borderstyle="None" backcolor="White" pagesize="8" cssclass="tabletext"
												cellpadding="3" bordercolor="#999999" allowsorting="True" borderwidth="1px" gridlines="Vertical"
												autogeneratecolumns="False" aria-label="Transport line items">
												<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
												<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
												<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
												<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
												<Columns>
													<asp:TemplateColumn HeaderText="Edit">
														<HeaderStyle Width="55px"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<fmcontrols:fmeditlinkbutton ID="EditButton3" runat="server" />
														</ItemTemplate>
														<EditItemTemplate>
															<fmcontrols:fmupdatelinkbutton ID="UpdateButton3" runat="server" />&nbsp;
															<fmcontrols:fmcancellinkbutton ID="CancelButton3" runat="server" />
														</EditItemTemplate>
													</asp:TemplateColumn>
													<asp:TemplateColumn HeaderText="Delete">
														<HeaderStyle Width="0.5in"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<fmcontrols:fmdeletelinkbutton runat="server" ID="DeleteButton3" />
														</ItemTemplate>
													</asp:TemplateColumn>
												</Columns>
												<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
											</fmcontrols:fmbasedatagrid>
											<fmcontrols:fmbutton id="NewTransportButton" style="z-index: 108; left: 0px; top: 0px; width: 100px; height: 22px"
												runat="server" text="Add" onclick="TransportInfoNewButtonClick" cssclass="formfieldtitle" />
										</ContentTemplate>
									</asp:UpdatePanel>
								</td>
								<td></td>
							</tr>
							<tr>
								<td colspan="2">
									<fmcontrols:fmlabel id="DenotesRequiredFieldLabel" runat="server" width="176px" cssclass="formfieldtitle" height="8px" forecolor="Crimson">* Denotes Required Field</fmcontrols:fmlabel>
								</td>
								<td valign="top" align="left">&nbsp;</td>
							</tr>
						</table>
						<table id="TableFooter" style="z-index: 104;position: fixed;bottom: 30px;" runat="server" role="presentation" aria-label="layout">
							<tr>
								<td colspan="2">
									<asp:UpdatePanel ID="MainButtonPanel" runat="server" UpdateMode="Conditional">
										<ContentTemplate>
											<table role="presentation" aria-label="layout">
												<tr>
													<td>
														<fmcontrols:fmhtmlbutton id="SaveButton" style="z-index: 102; left: 0px; width: 100px; height: 22px"
															runat="server" text="&Apply" accesskey="A" cssclass="formfieldtitle" onserverclick="SaveButtonClick" />
													</td>
													<td>
														<fmcontrols:fmhtmlbutton id="NewButton" style="z-index: 102; left: 0px; width: 100px; height: 22px"
															runat="server" text="&New" onserverclick="NewButtonClick" accesskey="N" cssclass="formfieldtitle" />
													</td>
													<td>
														<fmcontrols:fmdeletebutton id="DeleteButton" style="z-index: 104; left: 0px; width: 100px; height: 22px" runat="server"
															text="Delete" onclick="DeleteButtonClick" cssclass="formfieldtitle" />
													</td>
													<td>
														<fmcontrols:fmconfirmationbutton id="ReverseButton" style="z-index: 105; left: 0px; width: 100px; height: 22px" runat="server"
															text="Reverse" confirmationtext="Reverse this transaction?" onclick="ReverseButtonClick" cssclass="formfieldtitle" />
													</td>
													<td>
														<fmcontrols:fmconfirmationbutton id="ReverseUpdateButton" style="z-index: 108; width: 100px; height: 22px" runat="server"
															text="Reverse / Update" confirmationtext="Reverse this transaction and create an update?" onclick="ReverseUpdateButtonClick" cssclass="formfieldtitle" />
													</td>
													<td>
														<fmcontrols:fmbutton id="CloseButton" style="z-index: 106; left: 0px; width: 100px; height: 22px" runat="server"
															text="Close" onclick="CloseButtonClick" accesskey="C" cssclass="formfieldtitle" />
													</td>
													<td>
														<fmcontrols:fmbutton id="ViewPrintableBtn" style="z-index: 108; width: 100px; height: 22px" runat="server" text="View Printable" onclick="ViewPrintableBtnClick" cssclass="formfieldtitle" />
													</td>
													<td>
														<fmcontrols:fmbutton id="CombineBtn" style="z-index: 108; width: 100px; height: 22px" runat="server" text="Combine" onclick="CombineBtnClick" cssclass="formfieldtitle" />
													</td>
													<td>&nbsp;</td>
													<td>&nbsp;</td>
												</tr>
											</table>
										</ContentTemplate>
									</asp:UpdatePanel>
								</td>
								<td valign="top" align="left">
									<table role="presentation" aria-label="layout">
									</table>
								</td>
							</tr>
							<tr align="center">
								<td>
									<fmcontrols:fmlabel id="TransIDLabelLabel" style="z-index: 103; top: 0px" runat="server"
										cssclass="tabletext">Transaction ID:</fmcontrols:fmlabel>
									<asp:Label ID="TransIDLabel" runat="server" CssClass="tabletext"></asp:Label></td>
								<td></td>
							</tr>
							<tr align="center">
								<td>
									<fmcontrols:fmlabel id="HelpText" runat="server" style="z-index: 103; top: 0px" cssclass="tabletext"
										text="Alt+A for Apply.  Alt+N for New" />
								</td>
							</tr>
						
						</table>
						<div>
							<input id="LimitSelectionsBasedOnHierarchy" style="z-index: 1; left: 505px; position: absolute; top: 486px"
								runat="server" width="0px" type="hidden" forecolor="White" />
							<asp:UpdatePanel ID="ProductUserDataValuesPanel" runat="server" UpdateMode="Conditional">
								<ContentTemplate>
									<asp:HiddenField ID="ProductUserDataValues" runat="server" />
								</ContentTemplate>
							</asp:UpdatePanel>
							<div id="waitDiv" style="z-index: 500; left: 375px; top: 250px; position: fixed; display: none;">
								<img src="../FMWebApp/images/pleaseWait.jpg" />
							</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</form>

	<script>
		var FuelsManagerLib = {};
		FuelsManagerLib.token = '<%= security.Token %>';

		document.addEventListener('keydown', function (ev)
		{
			if (ev.keyCode == 13
				&& ev.srcElement != null
				&& ev.srcElement.onclick != null
				&& (ev.srcElement.type == "submit"
					|| ev.srcElement.type == "button"
					|| ev.srcElement.tagName == "A")
				&& (ev.srcElement.id == "SaveButton"
					|| ev.srcElement.id == "NewButton"
					|| ev.srcElement.id == "DeleteButton"
					|| ev.srcElement.id == "ReverseButton"
					|| ev.srcElement.id == "ReverseUpdateButton"
					|| ev.srcElement.id == "CloseButton"
					|| ev.srcElement.id == "ViewPrintableBtn"
					|| ev.srcElement.id == "CombineBtn"
					|| ev.srcElement.id == "PreviousButton"
					|| ev.srcElement.id == "NextButton"
					|| ev.srcElement.id == "NewTransportButton"
					|| ev.srcElement.id == "NewAGRButton"
					|| ev.srcElement.id == "NewLineItemButton"
					|| ev.srcElement.id.indexOf("Date SetButton") != -1
					|| ev.srcElement.id.indexOf("Select Button") != -1))
			{
				if (ev.srcElement.tagName == "A")
					window.location.href = ev.srcElement.href;
				else
					ev.srcElement.onclick();
				ev.preventDefault();
			}
		});

	</script>
</body>
</html>
