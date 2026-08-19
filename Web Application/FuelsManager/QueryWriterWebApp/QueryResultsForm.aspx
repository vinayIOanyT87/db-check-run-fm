<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryResultsForm.aspx.cs" Inherits="FuelsManager.QueryWriterWebApp.QueryResultsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <script type="text/javascript" language="javascript">
            // Get a PageRequestManager reference.
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            // Hook the _initializeRequest event and add our own handler.
            prm.add_initializeRequest(InitializeRequest);

            function InitializeRequest(sender, args) {
                // Check to be sure this async postback is actually
                //   requesting the file download.

                if (sender._postBackSettings.sourceElement.id == "DownloadResultsLink") {
                    // Create an IFRAME.
                    var iframe = document.createElement("iframe");

                    // Point the IFRAME to GenerateFile, with the
                    //   desired region as a querystring argument.
                    // The "query" function will append the CSRF token to the string.
                    iframe.src = AddCSRFTokenToUrl("GenerateFile.aspx?MODE=CSV");

                    // This makes the IFRAME invisible to the user.
                    iframe.style.display = "none";

                    // Add the IFRAME to the page.  This will trigger
                    //   a request to GenerateFile now.
                    document.body.appendChild(iframe);
                }
                else if (sender._postBackSettings.sourceElement.id == "PrinterFriendlyLink") {
                    window_open('QueryPrinterFriendlyResults.aspx');
                }
            }
        </script>

	    <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
        <table style="z-index:110; left:8px; top: 10px; position:absolute" cellpadding="3" role="presentation" aria-label="layout">
            <tr style="height:45px">
                <td>
                    <table id="HeaderTable" runat="server" style="width:900px; left:-3px; position:relative" cellpadding="3" role="presentation" aria-label="header layout">
                        <tr>
                            <td>
                                <asp:Label ID="TitleLabel" runat="server" CssClass="headline" Text="User-Defined Query Title" /><br />
                                <asp:LinkButton ID="QueryDefinitionLink" runat="server" CssClass="QueryResultsLinkStyle" Text="Edit Query Definition" OnClick="QueryDefinitionLinkClick" />
                            </td>
                            <td align="right" valign="top">
                                <FMControls:FMButton ID="RefreshButton" runat="server" CssClass="formfieldtitle" Text="Refresh" OnClick="RefreshButtonClick"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"><hr style="color:Black; size:1pt"/></td>
                        </tr>
                        <tr id="FilterControlsRow">
                            <td colspan="2">
                                <table id="FitlerControlsTable" runat="server" role="presentation" aria-label="filter layout"></table>
                            </td>
                        </tr>
                        <tr id="ExtraLineRow" runat="server">
                            <td colspan="2"><hr style="color:Black; size:1pt"/></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="RecordsPanel" runat="server">
                                    <ContentTemplate>
                                        <FMControls:FMLabel ID="RecordsMessageLabel" runat="server" CssClass="formfield" Text="Put Records Message here" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ResultsGrid" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td style="text-align:right">
			                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
			                        <ContentTemplate>
                                        <asp:LinkButton ID="PrinterFriendlyLink" runat="server" CssClass="QueryResultsLinkStyle" Text="Printer-Friendly Results" />
                                        &nbsp;
                                        <asp:LinkButton ID="DownloadResultsLink" runat="server" CssClass="QueryResultsLinkStyle" Text="Download Results" />
			                        </ContentTemplate>
			                    </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <FMControls:FMPageSizeDropDown ID="PageSizeDropDown" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" alt="Page Size"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="GridUpdatePanel" runat="server">
                        <ContentTemplate>
                            <FMControls:FMGridView
                                ID="ResultsGrid" 
                                runat="server" 
                                FixedHeaders="false" 
                                ShowHeaderWhenEmpty="true" 
                                AllowSorting="true" 
                                GroupColumnOffset="4"
                                GroupingDepth="0" aria-label="Results">
                                <Columns>
                                    <asp:TemplateField HeaderText="Edit" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton runat="server" />
                                            <FMControls:FMLabel ID="TotalText" runat="server" CssClass="formfield" Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <FMControls:FMDeleteCommandField Visible="false" />
                                    <asp:TemplateField Visible = "false">
                                        <ItemTemplate>
                                            <asp:Literal ID="EntityGuid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.EntityGuid").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Line" Visible="false" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
						                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Internal__LineNumber") %>' ID="QueryNameLabel"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
		                    </FMControls:FMGridView>
		                </ContentTemplate>
		            </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td id="AdditionalInformation" runat="server">
                </td>
            </tr>
        </table>
    </div>
</form>
</body>
</html>
