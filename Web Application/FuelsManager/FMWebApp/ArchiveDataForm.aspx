<%@ Page Language="c#" AutoEventWireup="True" Codebehind="ArchiveDataForm.aspx.cs" Inherits="FuelsManager.FMWebApp.ArchiveDataForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
        <title></title>
        <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
        <meta content="C#" name="CODE_LANGUAGE">
        <meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</head>
<body>
    <script type="text/javascript">
        function formSubmit() {
            // Display a wait message
            var waitImage = document.getElementById("waitDiv");
            waitImage.style.display = "inline";
            //var archiveButton = document.getElementById("ArchiveButton");
            //archiveButton.disabled = true;
        }
    </script>
    <form id="form1" method="post" enctype="multipart/form-data" runat="server" onsubmit="formSubmit();">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position: absolute">
            <asp:Image ID="Image2" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="ConfigurationLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px"
                runat="server" CssClass="headline" BackColor="Transparent" Width="718px">Archive Data</FMControls:FMLabel>

            <FMControls:FMLabel ID="StartDateLabel"
                Style="z-index: 121; left: 40px; position: absolute; top: 50px" runat="server"
                BackColor="Transparent" Text="Start Date" Height="16px" Width="100px"
                CssClass="formfieldtitle" />

            <FMControls:FMDate ID="StartDate" Style="z-index: 203; left: 120px; position: absolute; top: 50px"
                runat="server" Width="150px" CssClass="formfield" TabIndex="1"></FMControls:FMDate>

            <FMControls:FMLabel ID="EndDateLabel"
                Style="z-index: 122; left: 40px; position: absolute; top: 80px" runat="server"
                BackColor="Transparent" Text="End Date" Height="16px" Width="100px"
                CssClass="formfieldtitle" />

            <FMControls:FMDate ID="EndDate" Style="z-index: 202; left: 120px; position: absolute; top: 80px"
                runat="server" Width="150px" CssClass="formfield" TabIndex="2"></FMControls:FMDate>

            <FMControls:FMLabel ID="FMLABEL1"
                Style="z-index: 121; left: 40px; position: absolute; top: 134px" runat="server"
                BackColor="Transparent" Text="Select Data to Archive" Height="16px" Width="200px"
                CssClass="formfieldtitle" />

            <asp:Panel ID="GeneralPanel"
                Style="z-index: 103; left: 40px; position: absolute; top: 154px; width: 700px; height: 52px;"
                runat="server" BorderColor="LightSteelBlue" BorderStyle="Solid"
                BorderWidth="1px" />

            <FMControls:FMCheckBox ID="chkAccounting" TabIndex="3" runat="server" CssClass="formfieldtitle"
                Style="z-index: 124; left: 53px; position: absolute; top: 170px; width: 120px; right: 1236px;"
                Text="Accounting Data"></FMControls:FMCheckBox>

            <FMControls:FMCheckBox ID="chkQC" TabIndex="4" runat="server" Width="138px" CssClass="formfieldtitle"
                Style="z-index: 124; left: 181px; position: absolute; top: 170px; right: 1090px;"
                Text="Quality Control Data"></FMControls:FMCheckBox>

            <FMControls:FMCheckBox ID="chkMaintenance" TabIndex="5" runat="server" Width="138px" CssClass="formfieldtitle"
                Style="z-index: 124; left: 327px; position: absolute; top: 170px"
                Text="Maintenance Data"></FMControls:FMCheckBox>

            <FMControls:FMCheckBox ID="chkAlarm" TabIndex="6" runat="server" CssClass="formfieldtitle"
                Style="z-index: 124; left: 473px; position: absolute; top: 170px; width: 148px;"
                Text="Alarm and Event Data"></FMControls:FMCheckBox>

            <FMControls:FMCheckBox ID="chkAudit" TabIndex="7" runat="server" CssClass="formfieldtitle"
                Style="z-index: 124; left: 629px; position: absolute; top: 170px; width: 99px;"
                Text="Audit Data"></FMControls:FMCheckBox>

            <FMControls:FMButton ID="ArchiveButton" TabIndex="7" Style="z-index: 103; left: 40px; position: absolute; top: 232px"
                runat="server" CssClass="formfieldtitle" Width="72px" Text="Archive"
                OnClick="ArchiveButton_Click" />

            <asp:TextBox ID="ResultsTextBox" TabIndex="7" ToolTip="Results from archive data" Style="z-index: 107; left: 40px; position: absolute; top: 279px; height: 126px; width: 700px;"
                runat="server" CssClass="formfield" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>

        </div>
    </form>
    <div id="waitDiv" style="z-index: 500; left: 375px; top: 250px; position: absolute; display: none;">
        <img src="../FMWebApp/images/pleaseWait.jpg" alt="Please Wait" />
    </div>
</body>
</html>
