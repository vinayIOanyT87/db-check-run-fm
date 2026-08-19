<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPasswordForm.aspx.cs"
    Inherits="FuelsManager.FMWebApp.ForgotPasswordForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>"  defer="defer"></script>
</head>
<body style="background-color: #0D256B">
    <table class="loginpage" width="100%">
        <tr>
            <td>
                <form id="form1" runat="server">
                    <table align="center" style="position: relative; top: 40%">
                        <tr>
                            <td>
                                <table align="center">
                                    <tr>
                                        <td>
                                            <div style="width: 400px; height: 250px; border-style: solid; border-width: thin; border-color: Rgb(200,200,200)">
                                                <asp:Image ID="fadeImage" Style="z-index: 100; width: 400px; height: 250px" runat="server"
                                                    ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                                            </div>
                                            <FMControls:FMLabel ID="titleLabel" Style="z-index: 118; left: 16px; top: 16px; position: absolute"
                                                runat="server" BackColor="Transparent" CssClass="headline">Forgot Your Password</FMControls:FMLabel>
                                            <FMControls:FMLabel ID="ResetPasswordLabel" Style="z-index: 118; left: 32px; position: absolute; top: 100px; height: 16px;"
                                                runat="server" BackColor="Transparent" Text="Press the 'Reset Password' button to reset your password.  A<br> temporary password will be sent to your email address on file."
                                                Width="360px" CssClass="formfieldtitle" />
                                            <FMControls:FMLabel ID="NoteLabel" Style="z-index: 118; left: 32px; position: absolute; top: 140px; height: 16px;"
                                                runat="server" BackColor="Transparent" Text="*Please note that email processing could be delayed based on your network and server performance."
                                                Width="360px" CssClass="notestext" color="grey"/>
                                            <FMControls:FMButton ID="ResetPasswordButton" Style="z-index: 118; left: 140px; position: absolute; top: 200px"
                                                TabIndex="30" runat="server" CssClass="formfieldtitle" Text="Reset Password"
                                                Width="120px" OnClick="ResetPasswordButton_Click"></FMControls:FMButton>
                                            <FMControls:FMButton ID="CancelButton" Style="z-index: 118; left: 280px; position: absolute; top: 200px"
                                                TabIndex="30" runat="server" CssClass="formfieldtitle" Text="Cancel"
                                                Width="70px" OnClick="CancelButton_Click"></FMControls:FMButton>
                                            <div style="width: 400px; height: 200px">
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </form>
            </td>
        </tr>
    </table>
</body>
</html>
