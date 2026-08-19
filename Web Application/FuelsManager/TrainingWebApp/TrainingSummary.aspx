<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrainingSummary.aspx.cs" Inherits="FuelsManager.TrainingWebApp.TrainingSummary" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:Image ID="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: -1; left: 0px; position: absolute; top: 0px" runat="server"
                ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent" />
            <FMControls:FMLabel ID="FMLabel1" runat="server" CssClass="headline"
                Text="Training Summary" Style="left: 8px; top: 8px; position: absolute" BackColor="Transparent" Font-Bold="True" />
            <FMControls:FMLabel id="FindLabel" AssociatedControlID="FindTextBox" style="Z-INDEX: 110; LEFT: 24px; POSITION: absolute; TOP: 40px"
				runat="server" CssClass="formfieldtitle" BackColor="Transparent">Personnel:</FMControls:FMLabel>				
			<asp:TextBox id="FindTextBox" style="Z-INDEX: 107; LEFT: 128px; POSITION: absolute; TOP: 40px"
				runat="server" CssClass="formfield" Width="264px" tabIndex="1" MaxLength="100"></asp:TextBox>
			<FMControls:FMLabel id="FMLabel2" AssociatedControlID="ItemDropDownList" style="Z-INDEX: 110; LEFT: 24px; POSITION: absolute; TOP: 64px"
				runat="server" CssClass="formfieldtitle" BackColor="Transparent">Item:</FMControls:FMLabel>				
			<FMControls:FMDropDownList id="ItemDropDownList" style="Z-INDEX: 106; LEFT: 128px; POSITION: absolute; TOP: 64px"
				runat="server" CssClass="formfield" Width="136px" AutoPostBack="False" tabIndex="2"></FMControls:FMDropDownList>
			<FMControls:FMLabel id="FMLabel3" AssociatedControlID="DateFilterTypeDropDown" style="Z-INDEX: 110; LEFT: 24px; POSITION: absolute; TOP: 88px"
				runat="server" CssClass="formfieldtitle" BackColor="Transparent">Date Filter:</FMControls:FMLabel>				
			<FMCONTROLS:FMDROPDOWNLIST id="DateFilterTypeDropDown" style="Z-INDEX: 127; LEFT: 128px; POSITION: absolute; TOP: 88px"
				runat="server" CssClass="formfield" Sort="false" TabIndex="3" AutoPostBack="False" ></FMCONTROLS:FMDROPDOWNLIST>
			<FMCONTROLS:FMLABEL id="StartDateLabel" style="Z-INDEX: 121; LEFT: 24px; POSITION: absolute; TOP: 112px"
				runat="server" BackColor="Transparent" Text="Start Date" Height="16px" Width="100px" CssClass="formfieldtitle">Start Date</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMDATE id="StartDate" style="Z-INDEX: 201; LEFT: 128px; POSITION: absolute; TOP: 112px"
				runat="server" Width="160px" CssClass="formfield" TabIndex="4" AutoPostBack="False" ></FMCONTROLS:FMDATE>	    
			<FMCONTROLS:FMLABEL id="EndDateLabel" style="Z-INDEX: 122; LEFT: 24px; POSITION: absolute; TOP: 136px"
				runat="server" BackColor="Transparent" Text="End Date" Height="16px" Width="102px" CssClass="formfieldtitle"></FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMDATE id="EndDate" style="Z-INDEX: 124; LEFT: 128px; POSITION: absolute; TOP: 136px" runat="server"
				Width="160px" CssClass="formfield" TabIndex="5" AutoPostBack="False" ></FMCONTROLS:FMDATE>
            <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 109; left: 300px; position: absolute; top: 120px"
                runat="server" CssClass="formfieldtitle" Width="64px" Text="Show All" TabIndex="6" OnClick="FIndAllBtnOnClick"></FMControls:FMButton>
            <FMControls:FMButton ID="RefreshButton" Style="z-index: 108; left: 380px; position: absolute; top: 120px" runat="server"
                CssClass="formfieldtitle" Width="64px" Text="Refresh" TabIndex="7" OnClick="RefreshButtonOnClick"></FMControls:FMButton>
            <FMControls:FMCheckBox ID="HistoricalDataCheckBox" TabIndex="8" runat="server" Width="188px" CssClass="formfieldtitle"
                Style="z-index: 124; left: 460px; position: absolute; top: 120px" Text="Include Historical Data" AutoPostBack="False"></FMControls:FMCheckBox>
            <contenttemplate>
					 <table style="z-index:110; left:32px; top: 160px; width:575px; position:absolute" cellpadding="5">
						<tr>
							 <td colspan="4" style="vertical-align:top">
									 <FMControls:FMGridView ID="TrainingSummaryDataGrid" runat="server"  AutoGenerateColumns = "true"  AllowSorting="true" AllowPaging="false" RowHeaderColumn="Item ID"
										 FixedHeaders="true" Width="700px" PagerStyle-CssClass="pgr" ShowHeaderWhenEmpty="false" Height="600px" ShowFooter="true">
										  <Columns>
												<asp:TemplateField HeaderText="Edit">
													<HeaderStyle Width="0.5in" />
													<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
													<ItemTemplate>
														<FMControls:FMEditLinkButton ID="EditButton" OnCommand="TrainingSummaryDataGridRowCommandReceived" runat="server" />
													</ItemTemplate>
												</asp:TemplateField>
												<asp:TemplateField HeaderText="Delete">
													<HeaderStyle Width="25px" />
													<ItemTemplate>
														<FMControls:FMDeleteLinkButton ID="DeleteButton" OnCommand="TrainingSummaryDataGridRowCommandReceived" runat="server" CommandName="Delete" />
													</ItemTemplate>
												</asp:TemplateField>
										  </Columns>
								</FMControls:FMGridView>
							 </td>
						</tr>
					 </table>
			</contenttemplate>
        </div>
    </form>
</body>
</html>
