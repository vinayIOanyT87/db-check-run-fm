<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" AutoEventWireup="True" CodeBehind="DataDictionaryForm.aspx.cs" Inherits="FuelsManager.FMWebApp.DataDictionaryForm" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="C#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/lib/dispatch.js" %>" type="text/javascript"></script>
    <script>
        function EnableButtons() {
            $("input").removeAttr("disabled");
        }
    </script>
</head>
<body ms_positioning="GridLayout" tabindex="-1">
    <form id="Form1" method="post" enctype="multipart/form-data" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:Image ID="Image2" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="ConfigurationLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px"
                runat="server" CssClass="headline" BackColor="Transparent" Width="718px">Data Dictionary Configuration</FMControls:FMLabel>
            <FMControls:FMCheckBox ID="UseDataDictionaryCheckBox" Style="z-index: 107; position: absolute; left: 24px; top: 43px; width: 300px"
                runat="server" CssClass="formfieldtitle" Text="Use Data Dictionary Glossary" AutoPostBack="True" OnCheckedChanged="DataDictionaryChanged" />
            <FMControls:FMLabel ID="FindStringLabel" AssociatedControlID="FindTextBox" runat="server" CssClass="formfieldtitle" BackColor="Transparent"
                Style="z-index: 107; left: 24px; position: absolute; top: 78px">Find String:</FMControls:FMLabel>
            <asp:TextBox ID="FindTextBox" runat="server" Style="z-index: 107; left: 120px; position: absolute; top: 78px"
                Width="320px" TabIndex="1" MaxLength="100"></asp:TextBox>
            <FMControls:FMButton ID="FindBtn" TabIndex="2" Width="64px" runat="server" CssClass="formfieldtitle"
                Text="Find" Height="24px" Style="z-index: 107; left: 480px; position: absolute; top: 73px" OnClick="FindButtonOnClick"></FMControls:FMButton>
            <FMControls:FMButton ID="ShowAllButton" TabIndex="3" Width="64px" runat="server" CssClass="formfieldtitle"
                Text="Show All" Height="24px" Style="padding-left: 0; padding-right: 0; z-index: 107; left: 568px; position: absolute; top: 73px" OnClick="ShowAllButtonOnClick"></FMControls:FMButton>
            <asp:Button ID="AButton" Style="padding-left: 0; padding-right: 0; z-index: 104; left: 24px; position: absolute; top: 118px" Text="A"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="4"></asp:Button>
            <asp:Button ID="BButton" Style="padding-left: 0; padding-right: 0; z-index: 118; left: 48px; position: absolute; top: 118px" Text="B"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="5"></asp:Button>
            <asp:Button ID="CButton" Style="padding-left: 0; padding-right: 0; z-index: 116; left: 72px; position: absolute; top: 118px" Text="C"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="6"></asp:Button>
            <asp:Button ID="DButton" Style="padding-left: 0; padding-right: 0; z-index: 108; left: 96px; position: absolute; top: 118px" Text="D"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="7"></asp:Button>
            <asp:Button ID="EButton" Style="padding-left: 0; padding-right: 0; z-index: 120; left: 120px; position: absolute; top: 118px" Text="E"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="8"></asp:Button>
            <asp:Button ID="FButton" Style="padding-left: 0; padding-right: 0; z-index: 110; left: 144px; position: absolute; top: 118px" Text="F"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="9"></asp:Button>
            <asp:Button ID="GButton" Style="padding-left: 0; padding-right: 0; z-index: 124; left: 168px; position: absolute; top: 118px" Text="G"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="10"></asp:Button>
            <asp:Button ID="HButton" Style="padding-left: 0; padding-right: 0; z-index: 112; left: 192px; position: absolute; top: 118px" Text="H"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="11"></asp:Button>
            <asp:Button ID="IButton" Style="padding-left: 0; padding-right: 0; z-index: 130; left: 216px; position: absolute; top: 118px" Text="I"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="12"></asp:Button>
            <asp:Button ID="JButton" Style="padding-left: 0; padding-right: 0; z-index: 114; left: 240px; position: absolute; top: 118px" Text="J"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="13"></asp:Button>
            <asp:Button ID="KButton" Style="padding-left: 0; padding-right: 0; z-index: 125; left: 264px; position: absolute; top: 118px" Text="K"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="14"></asp:Button>
            <asp:Button ID="LButton" Style="padding-left: 0; padding-right: 0; z-index: 105; left: 288px; position: absolute; top: 118px" Text="L"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="15"></asp:Button>
            <asp:Button ID="MButton" Style="padding-left: 0; padding-right: 0; z-index: 122; left: 312px; position: absolute; top: 118px" Text="M"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="16"></asp:Button>
            <asp:Button ID="NButton" Style="padding-left: 0; padding-right: 0; z-index: 106; left: 336px; position: absolute; top: 118px" Text="N"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="17"></asp:Button>
            <asp:Button ID="OButton" Style="padding-left: 0; padding-right: 0; z-index: 123; left: 360px; position: absolute; top: 118px" Text="O"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="18"></asp:Button>
            <asp:Button ID="PButton" Style="padding-left: 0; padding-right: 0; z-index: 107; left: 384px; position: absolute; top: 118px" Text="P"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="19"></asp:Button>
            <asp:Button ID="QButton" Style="padding-left: 0; padding-right: 0; z-index: 109; left: 408px; position: absolute; top: 118px" Text="Q"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="20"></asp:Button>
            <asp:Button ID="RButton" Style="padding-left: 0; padding-right: 0; z-index: 111; left: 432px; position: absolute; top: 118px" Text="R"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="21"></asp:Button>
            <asp:Button ID="SButton" Style="padding-left: 0; padding-right: 0; z-index: 113; left: 456px; position: absolute; top: 118px" Text="S"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="22"></asp:Button>
            <asp:Button ID="TButton" Style="padding-left: 0; padding-right: 0; z-index: 115; left: 480px; position: absolute; top: 118px" Text="T"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="23"></asp:Button>
            <asp:Button ID="UButton" Style="padding-left: 0; padding-right: 0; z-index: 117; left: 504px; position: absolute; top: 118px" Text="U"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="24"></asp:Button>
            <asp:Button ID="VButton" Style="padding-left: 0; padding-right: 0; z-index: 119; left: 528px; position: absolute; top: 118px" Text="V"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="25"></asp:Button>
            <asp:Button ID="WButton" Style="padding-left: 0; padding-right: 0; z-index: 121; left: 552px; position: absolute; top: 118px" Text="W"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="26"></asp:Button>
            <asp:Button ID="XButton" Style="padding-left: 0; padding-right: 0; z-index: 126; left: 576px; position: absolute; top: 118px" Text="X"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="27"></asp:Button>
            <asp:Button ID="YButton" Style="padding-left: 0; padding-right: 0; z-index: 127; left: 600px; position: absolute; top: 118px" Text="Y"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="28"></asp:Button>
            <asp:Button ID="ZButton" Style="padding-left: 0; padding-right: 0; z-index: 128; left: 624px; position: absolute; top: 118px" Text="Z"
                CssClass="formfieldtitle" runat="server" Width="18px" TabIndex="29"></asp:Button>
            <asp:Button ID="NonAlphaButton" Style="padding-left: 0; padding-right: 0; z-index: 133; left: 648px; position: absolute; top: 118px"
                Text="Non-Alpha" CssClass="formfieldtitle" runat="server" Width="72px" TabIndex="30"></asp:Button>
            <table style="z-index: 100; left: 24px; width: 38.42%; position: absolute; top: 158px; height: 10px">
                <tr>
                    <td width="350" height="36" valign="middle">
                        <FMControls:FMPageSizeDropDown ID="DataDictFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 647px">
                        <FMControls:FMDataGrid ID="DataDictionaryDataGrid" BorderStyle="None" CssClass="tabletext" BackColor="White" RowHeaderColumn="Key"
                            runat="server" PageSize="12" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
                            CellPadding="3" AllowPaging="True" Width="696px" TabIndex="31">
                            <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Width="55px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMUpdateLinkButton runat="server" />&nbsp;<FMControls:FMCancelLinkButton runat="server" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Key">
                                    <HeaderStyle Width="1.75in"></HeaderStyle>
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="KeyLabel" Width="1.75in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Key") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Value">
                                    <HeaderStyle Width="1.75in"></HeaderStyle>
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="ValueLabel" Width="1.75in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Value") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="ValueTextBox" Width="1.75in" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.Value") %>' MaxLength="100">
                                        </asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMDataGrid></td>
                </tr>
                <tr>
                    <td>
                        <table style="width: 688px; height: 29px">
                            <tr>
                                <td style="width: 554px">
                                    <input class="formfieldtitle" alt="Select file to upload" id="File1" type="file" size="65" name="file" tabindex="32"></td>
                                <td style="width: 100px">
                                    <FMControls:FMButton ID="ExportButton" Text="Export" width="75px" CssClass="formfieldtitle" runat="server" TabIndex="33"></FMControls:FMButton></td>
                                <td>
                                    <FMControls:FMButton ID="ImportButton" Text="Import" width="75px" CssClass="formfieldtitle" runat="server" TabIndex="34"></FMControls:FMButton></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            &nbsp;
            <asp:HiddenField ID="dataDictionaryChangedField" runat="server" Value="false" />
        </div>
    </form>
    <script>
        var FindBtn = document.getElementById("FindBtn");
        var findTextBox = document.getElementById("FindTextBox");


        if (FindBtn != null && findTextBox != null) {
            try {
                FindBtn.setActive();
                findTextBox.focus();
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

        var dictionaryChange = document.getElementById("dataDictionaryChangedField");

        if (dictionaryChange.value == 'true') {
            DispatchLib.currentUserGuid = '<%= Security.UserGuid.ToString() %>';
            DispatchLib.clearGridUserSettings();
            dictionaryChange.value = 'false';
        }
    </script>
</body>
</html>
