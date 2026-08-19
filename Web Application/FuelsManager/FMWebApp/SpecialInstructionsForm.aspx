<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="SpecialInstructionsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SpecialInstructionsForm" EnableViewState="True" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title></title>
    <base target="_self">
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script type="text/javascript">
        function OKButton_Click()
        {
            closeDialogWindow();
        }

        function CancelButton_Click()
        {
            var result = new Array();
            setWindowReturnValue(result);
            closeDialogWindow();
        }

        $( document ).ready( function()
        {
            var textArea = $("#SpecialInstructionsText");

            if (textArea != null)
            {
                try
                {
                    $("#SpecialInstructionsText").focus();
                }
                catch (err)
                { }
            }
        } );

    </script>
</head>
<body ms_positioning="GridLayout">
    <form id="SpecialInstructionForm" name="SpecialInstructionFormName" method="post" runat="server">
        <asp:Image ID="FadeImage" Style="z-index: -1; left: 0px; position: absolute; top: 0px" runat="server"
            BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
        <table style="z-index: 110" cellspacing="5">
            <tr>
                <td>
                    <FMControls:FMLabel ID="PageTitle" runat="server" BackColor="Transparent" Text="Special Instructions" CssClass="headline"></FMControls:FMLabel></td>
                <td align="right">
                    <FMControls:FMButton ID="OKButton" CssClass="formfieldtitle" Text="OK" Width="75" runat="server" OnClick="OKButton_Click" />
                    &nbsp;
						<FMControls:FMButton ID="CancelButton" CssClass="formfieldtitle" Text="Cancel" Width="75" runat="server" OnClick="CancelButton_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <FMControls:FMTextBox ID="SpecialInstructionsText" name="SpecialInstructionsText" TabIndex="1" runat="server" CssClass="FormField" Width="700px"
                        TextMode="MultiLine" Height="410px" MaxLength="2000" /></td>
            </tr>
        </table>
    </form>
</body>
</html>
