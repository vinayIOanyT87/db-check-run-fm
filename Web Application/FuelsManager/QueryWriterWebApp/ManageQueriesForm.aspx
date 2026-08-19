<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageQueriesForm.aspx.cs" Inherits="FuelsManager.QueryWriterWebApp.ManageQueriesForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <title />
	<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
	<meta name="CODE_LANGUAGE" content="C#"/>
	<meta name="vs_defaultClientScript" content="JavaScript"/>
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data" >
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <script type="text/javascript" language="javascript">
                // Get a PageRequestManager reference.
                var prm = Sys.WebForms.PageRequestManager.getInstance();

                // Hook the _initializeRequest event and add our own handler.
                prm.add_initializeRequest(InitializeRequest);

                function InitializeRequest(sender) {
                    // Check to be sure this async postback is actually
                    //   requesting the file download.

                    if (sender._postBackSettings.sourceElement.id == "ExportButton") {
                        // Create an IFRAME.
                        var iframe = document.createElement("iframe");

                        // Point the IFRAME to GenerateFile, with the
                        //   desired region as a querystring argument.
                        // The "query" function will append the CSRF token to the string.
                        iframe.src = AddCSRFTokenToUrl("GenerateFile.aspx?Mode=Multiple");

                        // This makes the IFRAME invisible to the user.
                        iframe.style.display = "none";

                        // Add the IFRAME to the page.  This will trigger
                        //   a request to GenerateFile now.
                        document.body.appendChild(iframe);
                    }
                }
            </script>

		    <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
                <table style="z-index:110; left:32px; top: 10px; width:575px; position:absolute" cellpadding="5">
		            <tr>
		                <td colspan="4">
                            <FMControls:FMLabel id="TitleLabel" runat="server" CssClass="headline" Text="Queries" style="left:-24px; position:relative" />
		                </td>
		            </tr>
		            <tr>
		                <td colspan="4">
		                    <FMControls:FMButton ID="AddButton1" runat="server" CssClass="formfieldtitle" style="width:100px" Text="Add" />
		                </td>
		            </tr>
		            <tr>
		                <td colspan="4" style="vertical-align:top">
                            <FMControls:FMGridView ID="QueryGrid" runat="server" FixedHeaders="true" Width="700px" AllowPaging="false" RowHeaderColumn="Name"
                                ShowFooter="true" Height="550px" aria-label="Query">
                                <Columns>
                                    <asp:TemplateField HeaderText="View">
                                        <HeaderStyle Width="0.5in" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMViewLinkButton ID="ViewButton" OnCommand="QueryGridRowCommand" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit">
                                        <HeaderStyle Width="0.5in" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton ID="EditButton" OnCommand="QueryGridRowCommand" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name">
                                        <HeaderStyle Width="200px" />
                                        <ItemTemplate>
								            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.QueryName") %>' ID="QueryNameLabel"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="True" HeaderText="Description">
                                        <HeaderStyle />
                                        <ItemTemplate>
								            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.QueryDescription") %>' ID="QueryDescriptionLabel"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete">
												 <HeaderStyle Width="0.5in"></HeaderStyle>
												 <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
												 <ItemTemplate>
													 <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
												 </ItemTemplate>
											 </asp:TemplateField>
                                </Columns>
				            </FMControls:FMGridView>
		                </td>
		            </tr>
		            <tr>
				        <td style="WIDTH: 163px; HEIGHT: 36px" valign="middle" width="163">
                            <FMControls:FMButton id="AddButton2" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle" />
				        </td>
				        <td style="WIDTH: 394px">
				            <input class="formfieldtitle" id="File1" title="File import" type="file" name="file" size="46" style="width: 394px; height: 22px" />
				        </td>
				        <td style="WIDTH: 100px" align="left">
				            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
				                <ContentTemplate>
				                    <FMCONTROLS:FMBUTTON id="ExportButton" Text="Export" style="min-width:100px" CssClass="formfieldtitle" runat="server" tabIndex="10" />
				                </ContentTemplate>
				            </asp:UpdatePanel>
				        </td>
				        <td style="width: 100px">
				            <FMCONTROLS:FMBUTTON id="ImportButton" Text="Import" style="min-width:100px" CssClass="formfieldtitle" runat="server" />
				        </td>
		            </tr>
                </table>

    </div>
</form>
</body>
</html>
