<%@ Page language="c#" Codebehind="AlarmAndEventLogsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AlarmAndEventLogsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
        <script>
            var theMoment = new Date();
            var theDisplacement = (theMoment.getTimezoneOffset() / 60);
            document.cookie = "Displacement=" + theDisplacement;
            function printDiv(divName) {
                var documentCopy = document.cloneNode(true);
                documentCopy.getElementById('pnlContainer').style.overflow = "visible";
                documentCopy.getElementById('pnlContainer').style.fontFamily = "Helvetica";
                documentCopy.getElementById('Table1').style.top = "5px";
                documentCopy.getElementById('AlarmAndEventLogsDataGrid').style.fontSize = "11.5px";
                documentCopy.getElementById('SelectAllButton').remove();
                documentCopy.getElementById('ClearAllButton').remove();
                documentCopy.getElementById('AcknowledgeButton').remove();
                var totalRows = documentCopy.getElementById('AlarmAndEventLogsDataGrid').rows;
                for (var i = 0; i < totalRows.length; i++) {
                    if (totalRows[i].cells.length > 1) {
                        totalRows[i].deleteCell(0);
                    }
                }
                var printContents = documentCopy.getElementById(divName).innerHTML;
                let child = window.open("about:blank", "myChild");
                child.document.write(printContents);
                child.document.close();
            }
            function csvExport() {
                csv = []
                rows = $('#AlarmAndEventLogsDataGrid tr');
                for (i = 0; i < rows.length-1; i++) {
                    cells = $(rows[i]).find('td,th');
                    csv_row = [];
                    for (j = 1; j < cells.length; j++) {
                        txt = cells[j].innerText;
                        if (txt != ' ')
                        csv_row.push(txt.replace(",", "-"));
                    }
                    csv.push(csv_row.join(","));
                }
                const BOM = "\uFEFF"
                var output = csv.join("\n")
                output = BOM + output;
                var blob = new Blob([output], { type: 'text/html;charset=UTF-8' });
                var url = URL.createObjectURL(blob);

                var pom = document.createElement('a');
                pom.href = url;
                pom.setAttribute('download', 'export.csv');
                pom.click();
            }
        </script>
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 98; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label27" Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
                    BackColor="Transparent" CssClass="headline" Width="648px">Alarm & Event Log</FMControls:FMLabel>
                <FMControls:FMLabel ID="FMLABEL1" AssociatedControlID="SiteDropDownList" Style="z-index: 111; left: 24px; position: absolute; top: 40px" runat="server"
                    CssClass="formfieldtitle" Width="46px">Site:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="SiteDropDownList" Style="z-index: 112; left: 88px; position: absolute; top: 40px"
                    TabIndex="4" runat="server" CssClass="formfield" Width="230px">
                </FMControls:FMDropDownList>
                <FMControls:FMLabel ID="Label7" AssociatedControlID="SourceDropDownList" Style="z-index: 111; left: 24px; position: absolute; top: 72px" runat="server"
                    CssClass="formfieldtitle" Width="46px">Source:</FMControls:FMLabel>
                <asp:DropDownList ID="SourceDropDownList" Style="z-index: 112; left: 88px; position: absolute; top: 72px"
                    TabIndex="5" runat="server" CssClass="formfield" Width="230px" AutoPostBack="True" OnSelectedIndexChanged="SourceDropDownListSelectedIndexChanged">
                </asp:DropDownList>
                <FMControls:FMLabel ID="Label2" AssociatedControlID="IDDropDownList" Style="z-index: 114; left: 24px; position: absolute; top: 104px" runat="server"
                    CssClass="formfieldtitle" Width="6px" Height="16px">ID:</FMControls:FMLabel>
                <asp:DropDownList ID="IDDropDownList" Style="z-index: 115; left: 88px; position: absolute; top: 104px"
                    TabIndex="6" runat="server" CssClass="formfield" Width="230px" OnSelectedIndexChanged="IDDropDownListSelectedIndexChanged" AutoPostBack="True">
                </asp:DropDownList>
                <FMControls:FMLabel ID="Label1" AssociatedControlID="CategoryDropDownList" Style="z-index: 118; left: 24px; position: absolute; top: 136px" runat="server"
                    CssClass="formfieldtitle" Width="46px">Category:</FMControls:FMLabel>
                <asp:DropDownList ID="CategoryDropDownList" Style="z-index: 119; left: 88px; position: absolute; top: 136px"
                    TabIndex="7" runat="server" CssClass="formfield" Width="230px" OnSelectedIndexChanged="CategoryDropDownListSelectedIndexChanged">
                </asp:DropDownList>
                <FMControls:FMLabel ID="Label8" AssociatedControlID="PriorityDropDownList" Style="z-index: 124; left: 24px; position: absolute; top: 168px" runat="server"
                    CssClass="formfieldtitle" Width="46px">Priority:</FMControls:FMLabel>
                <asp:DropDownList ID="PriorityDropDownList" Style="z-index: 125; left: 88px; position: absolute; top: 168px"
                    TabIndex="8" runat="server" CssClass="formfield" Width="230px" OnSelectedIndexChanged="PriorityDropDownListSelectedIndexChanged">
                </asp:DropDownList>
                <FMControls:FMLabel ID="Label3" AssociatedControlID="TypeDropDownList" Style="z-index: 121; left: 24px; position: absolute; top: 200px" runat="server"
                    CssClass="formfieldtitle" Width="46px">Type:</FMControls:FMLabel>
                <asp:DropDownList ID="TypeDropDownList" Style="z-index: 122; left: 88px; position: absolute; top: 200px"
                    TabIndex="9" runat="server" CssClass="formfield" Width="230px" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownListSelectedIndexChanged">
                </asp:DropDownList>
                <FMControls:FMLabel ID="Label5" Style="z-index: 103; left: 328px; position: absolute; top: 40px" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle" Width="62px">Beginning</FMControls:FMLabel>
                <FMControls:FMDateTime ID="BeginningDateTime" Style="z-index: 105; left: 392px; position: absolute; top: 40px"
                    TabIndex="10" runat="server" CssClass="formfield" Width="330px" Height="25px"></FMControls:FMDateTime>
                <FMControls:FMCheckBox ID="ArchiveCheckBox" runat="server" style="Z-INDEX: 106; LEFT: 392px; POSITION: absolute; TOP: 104px" 
                BackColor="Transparent" CssClass="formfieldtitle" Text="Use Archive Data" />
                <FMControls:FMLabel ID="Label6" Style="z-index: 104; left: 328px; position: absolute; top: 72px" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle" Width="54px">Ending</FMControls:FMLabel>
                <FMControls:FMDateTime ID="EndingDateTime" Style="z-index: 106; left: 392px; position: absolute; top: 72px"
                    TabIndex="11" runat="server" CssClass="formfield" Width="330px" Height="25px"></FMControls:FMDateTime>
                <FMControls:FMButton ID="RefreshButton" Style="z-index: 113; left: 672px; position: absolute; top: 180px; width:100px"
                    TabIndex="12" runat="server" CssClass="formfieldtitle" Width="55px" Text="Refresh"></FMControls:FMButton>
                <input ID="PrintView" type="button" Style="z-index: 113; left: 783px; position: absolute; top: 180px; width:100px"
                    TabIndex="12" runat="server" CssClass="formfieldtitle" Width="55px" Value="Printable View" onclick="printDiv('printableArea');return false;"/>
                <input ID="CSV" type="button" Style="z-index: 113; left: 908px; position: absolute; top: 180px; width:40px"
                    TabIndex="12" runat="server" CssClass="formfieldtitle" Width="55px" Value="CSV" onclick="csvExport();return false;"/>
                <div id="printableArea">
                <table id="Table1" style="z-index: 101; left: 16px; width: 710px; position: absolute; top: 232px; height: 10px"
                    cellspacing="0" cellpadding="1" width="700" border="0">
                    <tbody>
                        <tr>
                            <td style="width: 713px; height: 10px" width="713">
                                <FMControls:FMDataGridFixed ID="AlarmAndEventLogsDataGrid" TabIndex="13" runat="server" BackColor="White" CssClass="tabletext"
                                    Width="950px" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None"
                                    AllowPaging="True" PageSize="8">
                                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                    <Columns>
                                        <asp:TemplateColumn HeaderText="Select">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <FMControls:FMCheckBox runat="server" CssClass="tabletext" Enabled='<%# DataBinder.Eval(Container, "DataItem.Alarm") %>' Checked='<%# DataBinder.Eval(Container, "DataItem.Selected") %>' ID="SelectedCheckBox"></FMControls:FMCheckBox>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:BoundColumn Visible="False" DataField="SequenceNumber"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="CreatedDate" HeaderText="Date &amp; Time">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="SiteID" HeaderText="Site">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Source" HeaderText="Source">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="ID" HeaderText="ID">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Data" HeaderText="Data">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="CategoryID" HeaderText="Category">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="PriorityID" HeaderText="Priority">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="UpdatedBy" HeaderText="User ID">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:TemplateColumn HeaderText="Acknowledged">
                                            <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <FMControls:FMCheckBox runat="server" CssClass="tabletext" Enabled="false" Visible='<%# DataBinder.Eval(Container, "DataItem.Alarm") %>' Checked='<%# DataBinder.Eval(Container, "DataItem.Acknowledged") %>' ID="AcknowledgedCheckBox"></FMControls:FMCheckBox>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
                                </FMControls:FMDataGridFixed></td>
                        </tr>
                        <tr>
                            <td>
                                <FMControls:FMButton ID="SelectAllButton" Style="left: 0px; position: absolute" TabIndex="14" runat="server"
                                    CssClass="formfieldtitle" Width="98px" Text="Select All"></FMControls:FMButton>
                                <FMControls:FMButton ID="ClearAllButton" Style="left: 120px; position: absolute" TabIndex="15" runat="server"
                                    CssClass="formfieldtitle" Width="98px" Text="Clear All"></FMControls:FMButton>
                                <FMControls:FMButton ID="AcknowledgeButton" Style="left: 240px; position: absolute" TabIndex="16" runat="server"
                                    CssClass="formfieldtitle" Width="98px" Text="Acknowledge" OnClick="AcknowledgeButtonClick"></FMControls:FMButton>

                            </td>
                        </tr>
                    </tbody>
                </table>
                </div>
            </div>
        </form>
    </body>
</HTML>

