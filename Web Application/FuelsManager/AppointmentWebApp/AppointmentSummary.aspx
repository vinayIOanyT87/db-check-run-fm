<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppointmentSummary.aspx.cs" Inherits="FuelsManager.AppointmentWebApp.AppointmentSummary" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" runat="server">
</head>
<body>
	<form id="form1" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMLabel ID="PageTitle" runat="server" CssClass="headline"
				Text="Scheduler Summary" Style="left: 12px; top: 8px; position: absolute" BackColor="Transparent"
				Font-Bold="True" Width="296px" />
			<div>
				<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
				<FMControls:FMLabel ID="FMLabel2" AssociatedControlID="TypeDropDownList" Style="z-index: 110; left: 24px; position: absolute; top: 40px"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
				<FMControls:FMDropDownList ID="TypeDropDownList" Style="z-index: 106; left: 128px; position: absolute; top: 40px"
					runat="server" CssClass="formfield" Width="136px" AutoPostBack="False" TabIndex="2" OnSelectedIndexChanged="TypeDropDown_OnSelectedIndexChanged">
				</FMControls:FMDropDownList>
				<FMControls:FMButton ID="RefreshButton"
					Style="z-index: 108; left: 300px; position: absolute; top: 40px" runat="server"
					CssClass="formfieldtitle" Width="64px" Text="Refresh" TabIndex="6"
					OnClick="RefreshButtonOnClick"></FMControls:FMButton>
				<FMControls:FMLabel ID="FMLabel3" AssociatedControlID="AssetDropDownList" Style="z-index: 110; left: 24px; position: absolute; top: 64px"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Asset:</FMControls:FMLabel>
				<FMControls:FMDropDownList ID="AssetDropDownList" Style="z-index: 106; left: 128px; position: absolute; top: 64px"
					runat="server" CssClass="formfield" Width="136px" AutoPostBack="False" TabIndex="3" OnSelectedIndexChanged="AssetDropDown_OnSelectedIndexChanged">
				</FMControls:FMDropDownList>
				<FMControls:FMLabel ID="StartDateLabel" AssociatedControlID="StartDate" Style="z-index: 121; left: 24px; position: absolute; top: 88px"
					runat="server" BackColor="Transparent" Text="Start Date" Height="16px" Width="100px" CssClass="formfieldtitle">Start Date</FMControls:FMLabel>
				<FMControls:FMDate ID="StartDate" Style="z-index: 124; left: 128px; position: absolute; top: 88px"
					runat="server" Width="160px" CssClass="formfield" TabIndex="4"></FMControls:FMDate>
				<FMControls:FMLabel ID="EndDateLabel" AssociatedControlID="EndDate" Style="z-index: 122; left: 24px; position: absolute; top: 112px"
					runat="server" BackColor="Transparent" Text="End Date" Height="16px" Width="102px" CssClass="formfieldtitle"></FMControls:FMLabel>
				<FMControls:FMDate ID="EndDate"
					Style="z-index: 124; left: 128px; position: absolute; top: 112px" runat="server"
					Width="160px" CssClass="formfield" TabIndex="5"></FMControls:FMDate>
				<table style="z-index: 110; left: 32px; top: 136px; width: 575px; position: absolute" cellpadding="5" role="presentation" aria-label="layout">
					<tr>
						<td width="350" height="36" valign="middle">
							<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
								TabIndex="7" OnClick="OnAddNewAppointment" />
						</td>
					</tr>
					<tr>
						<td colspan="4" style="vertical-align: top">
							<FMControls:FMGridView ID="AppointmentSummaryDataGrid" runat="server" AutoGenerateColumns="true" AllowSorting="true"
								FixedHeaders="true" Width="700px" AllowPaging="false" Height="600px" ShowFooter="true" RowHeaderColumn="Asset"
								aria-label="Appointment Summary">
								<Columns>
									<FMControls:FMEditCommandField EditText="Edit Item" />
									<FMControls:FMDeleteCommandField DeleteText="Delete Item" />
									<asp:TemplateField HeaderText="Result">
										<ItemTemplate>
											<FMControls:FMButton ID="AddResultButton" CommandName="AddResult" runat="server" ToolTip="Add"
												CssClass="formfield" Text="Add" OnClick="AddResultButtonClick" />
										</ItemTemplate>
									</asp:TemplateField>
								</Columns>
							</FMControls:FMGridView>
						</td>
					</tr>
					<tr>
						<td>
							<table style="height: 29px" role="presentation" aria-label="layout">
								<tr>
									<td style="width: 163px; height: 36px" valign="middle" width="163">
										<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
											TabIndex="8" OnClick="OnAddNewAppointment"></FMControls:FMButton>
									</td>
									<td>
										<FMControls:FMButton ID="PrintButton" runat="server" CssClass="formfieldtitle" TabIndex="9" Text="View Printable" Style="width: 125px" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</div>
		</div>
	</form>
</body>
</html>
