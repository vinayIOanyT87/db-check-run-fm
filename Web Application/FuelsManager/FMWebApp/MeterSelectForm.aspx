<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeterSelectForm.aspx.cs" Inherits="FuelsManager.FMWebApp.MeterSelectForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html >
<html>
<head>
    <title></title>
    <base target="_self" />
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
<%=  Global.LinkAccessibilityCssUrl(Session) %>

    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
</head>
<body>
    <script type="text/javascript">
        function Select(meterId, title)
        {
            var result = new Array();
            result[0] = meterId;
            result[1] = title;
            setWindowReturnValue(result);
            closeDialogWindow();
        }
    </script>
    <form id="Form1" method="post" runat="server">
        <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px"
            runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
        <asp:TextBox ID="FindTextBox" Style="z-index: 101; left: 10px; position: absolute; top: 14px"
            TabIndex="2" runat="server" Width="300px" CssClass="formfield" MaxLength="30"></asp:TextBox>
        <FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px"
            TabIndex="3" runat="server" Width="64px" CssClass="formfieldtitle"
            Text="Find" OnClick="FindBtn_OnClick"></FMControls:FMButton>
        <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
            TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle"
            Text="Show All" OnClick="FindAllBtn_OnClick"></FMControls:FMButton>
        <table id="Table1" style="z-index: 101; left: 8px; width: 50%; position: absolute; top: 45px; height: 10px"
            cellspacing="0" cellpadding="1" border="0">
            <tr>
                <td style="width: 549px; height: 10px" width="549">
                    <FMControls:FMGridView ID="MeterGrid" TabIndex="5" runat="server" RowHeaderColumn="ID"
                        Width="800px" Height="460px" CssClass="tabletext" OnRowCommand="MeterGrid_RowCommand" DataKeyNames="ID" AllowPaging="false" FixedHeaders="true" ShowFooter="false">
                        <Columns>
                            <asp:TemplateField HeaderText="">
                                <HeaderStyle Width="100px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <FMControls:FMSelectLinkButton ID="SelectButton" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ID">
                                <HeaderStyle Width="400px"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label ID="MeterIDGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Number of Digits">
                                <HeaderStyle Width="150px"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label ID="NumberOfDigitsGridColumn" Text='<%# DataBinder.Eval(Container, "DataItem.NumberOfDigitsString") %>'
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Rotates Backwards">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <HeaderStyle Width="150px" />
                                <ItemTemplate>
                                    <FMControls:FMCheckBox ID="RotatesBackwardsCheckBox" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.RotatesBackwardsFlag") %>'
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Receipt Meter">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <HeaderStyle Width="100px" />
                                <ItemTemplate>
                                    <FMControls:FMCheckBox ID="ReceiptMeterCheckBox" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.ReceiptMeterFlag") %>'
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </FMControls:FMGridView>
                </td>
            </tr>
        </table>
    </form>
    <script type="text/javascript">
        var findBtn = document.getElementById("FindBtn");
        var findTbBtn = document.getElementById("FindTextBox");

        if (findBtn != null && findTbBtn != null) {
            try {
                findBtn.setActive();
                findTbBtn.focus();
            }
            catch (err) { }
        }

        // Set the Find Button to be activated by the enter key.
        document.addEventListener('keydown', function (ev) {
            if (ev.keyCode == 13) {
                ev.returnValue = false;
                ev.cancel = true;
                document.all("FindBtn").click();
            }
        });
    </script>
</body>
</html>
