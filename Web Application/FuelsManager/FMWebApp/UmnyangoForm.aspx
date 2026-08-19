<%@ Page Language="c#" CodeBehind="UmnyangoForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.UmnyangoForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<!--<script src="../MenuBar/FMMenuBar_min.js" type="text/javascript"></script>-->
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta content="JavaScript" name="vs_defaultClientScript"/>
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
    <meta http-equiv="Content Type" content="text/html; charset=ISO-8859-1" />
    
    <link rel="stylesheet" type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>"/>
    <link rel="stylesheet" type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/CFS.css" %>"/>
    <link rel="stylesheet" type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Content/jquery-ui.css" %>">
<%=  (Global.AccessibilityEnabled ? string.Format("<link href='{0}/css/accessibility.css' media='screen' rel='stylesheet' type='text/css' />", HttpRuntime.AppDomainAppVirtualPath) : string.Empty) %>

    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CFS.js" %>" type="text/javascript" defer></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/scripts/KioskKeyRestrictions.js" %>" type="text/javascript"></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery.min.js" %>" type="text/javascript"></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-1.12.4.js" %>" type="text/javascript"></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui.js" %>" type="text/javascript"></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/UmnyangoForm.js" %>" type="text/javascript"></script>
   
    <style>
        html, body, form {
            height: 100%;
            width: 100%;
            padding: 0;
            margin: 0;
            border: 0;
            min-width: 640px;
            min-height: 480px;
        }

        .page {
            min-height: 100%;
            width: 100%;
            vertical-align: middle;
            text-align: center;
            padding: 0;
            margin: 0;
            border: 0;
            background-color: #006CB3;
        }

        .center {
            margin: auto;
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            width: 640px; 
            height: 560px;
            min-width: 640px;
            min-height: 480px;
        }

        input {
            border-radius: 4px;
            padding-left: 15px;  
            padding-right: 15px;  
            border-width: 1px;
            border-style: solid;
            outline: none;
            border-color: rgb( 225,225,225);
            font-size: 13px !important;
        }

        input::-webkit-input-placeholder {
            color: rgb( 169, 169, 169); /* WebKit, Blink, Edge */
        }
        input:-ms-input-placeholder {
            color: rgb( 169, 169, 169); /* Internet Explorer 10-11 */
        }
        input::placeholder {
            color: rgb( 169, 169, 169); /* Most modern browsers support this now. */
        }

        #LoginButton:focus {
            border-color: rgb( 196, 207, 225);
            border-style: dashed;
        }

    </style>

    <script>
        $(document).ready(function () {
            var surpressPageLinksTb = document.getElementById("SurpressLoginPageLinksTB");
            var defaultLinkDiv = document.getElementById("DefaultLoginPageLinkDiv");
            var pageTitleTb = document.getElementById("PageTitleTb");

            if (surpressPageLinksTb != null && defaultLinkDiv != null)
            {
                var inputValue = surpressPageLinksTb.value;

                if (inputValue != null && inputValue === "TRUE")
                {
                    defaultLinkDiv.style.display = 'none';
                }
            }

            // Set the Tab title of the parent form.
            if (pageTitleTb != null && pageTitleTb.value !== "EMPTY")
            {
                parent.document.title = pageTitleTb.value;
            }
         });
    </script>


</head>
    
<body id="PageBody" runat="server" ms_positioning="GridLayout">
    <form id="Form1" method="post" runat="server">
		 <div class="page" id="warnDiv" runat="server" align="center" Visible="true" style="background-image: url(images/Warn_Box_7.jpg); background-position: center; background-repeat: no-repeat;">
			 <div class="center">
				 <asp:Label ID="TitleLabel" runat="server" CssClass="headline" Style="position: relative; left: -10px; top: 50px" />
				 <br />
				 <br />
				 <div id="WarningLabel" runat="server" class="formfield" style="text-align: justify; width: 90%; position: relative; left: 20px; top: 50px"> </div>
				 <br />
				 <FMControls:FMButton ID="AcceptButton" runat="server" CssClass="formfieldtitle" Text="Accept" Style="position: relative; left: -10px; top: 60px;width: 150px;padding-left:3px;padding-right:3px;background-color: #006CB3;color: white;height: 30px;" UseSubmitBehavior="false"
					 OnClientClick="this.disabled=true;" />&nbsp;
			 </div>
		 </div>
		 <div class="page" id="splashDiv" runat="server" visible ="false" style="background:url(images/splash_box_v1.jpg) center center no-repeat;">
                <div class="center">
	                 <FMControls:FMLabel ID="FMLoginLabel" runat="server" CssClass="formfieldtitle"
                        Style="left: 35px; top: 50px; color: black; font-size: 28px; position: absolute" 
                        Width="72px" Text="Welcome to" />
	                 <FMControls:FMLabel ID="FMLabel1" runat="server" CssClass="formfieldtitle"
                        Style="left: 35px; top: 80px; color: black; font-size: 30px; position: absolute" 
                        Width="72px" Text="<b>FuelsManager</b>" />
                    <asp:TextBox ID="UserNameTextBox" TabIndex="1" runat="server" Height="32px" placeholder="User Name"
                        style="color: #666666;position: absolute; top: 175px; left: 35px" Width="255px" CssClass="formfield" MaxLength="50" />

                    <asp:TextBox ID="PasswordTextBox" TabIndex="2" runat="server" Width="255px" TextMode="Password" placeholder="Password"
                        Style="color: #666666;z-index: 100; position: absolute; top: 225px; left: 35px" Height="32px" CssClass="formfield" MaxLength="25" AutoCompleteType="Disabled" AutoComplete="off" />

                    <asp:TextBox ID="SiteTextBox" TabIndex="3" runat="server" Height="32px" placeholder="Site"
                        Style="color: #666666;z-index: 90; position: absolute; top: 275px; left: 35px" Width="255px" CssClass="formfield" />
                    
                    <asp:DropDownList  ID="SiteListDropDown" TabIndex="3" runat="server" Height="32px" Visible="false"
                        Style="color: #666666;z-index: 90; position: absolute; top: 275px; left: 35px" Width="255px" CssClass="formfield" />

                    <FMControls:FMButton ID="LoginButton" runat="server" Text="Log in" CssClass="formfieldtitle" Width="88px"
                        TabIndex="4" Style="width: 150px;position:absolute;top: 330px;left:35px;padding-left:3px;padding-right:3px;background-color: #006CB3;color: white;height: 30px;" />

                    <asp:TextBox ID="InitialPasswordTextBox" TabIndex="-1" runat="server" CssClass="formfield" Width="0px"
                        Height="24" BorderStyle="None" Enabled="False" BorderColor="Transparent" BackColor="Transparent"
                        ForeColor="Transparent" ReadOnly="True" />

                    <asp:LinkButton ID="ChangePasswordButton" CssClass="DefaultLoginPageLink" runat="server"
                            Text="Change Password" TabIndex="5" Style="position: absolute; top: 340px; left: 207px" OnClick="ChangePasswordButtonCommand" />

                    <div id="DivPasswordForgotPassword" class="DefaultLoginPageLink" style="color: rgb( 163, 163,163); font-family: Arial,Helvetica,sans-serif; font-size: 11px; position: absolute; left: 303px; top: 340px" runat="server">
                        <asp:LinkButton ID="PasswordHintButton" title=" " CssClass="DefaultLoginPageLink" runat="server" Text="Password Hint" TabIndex="6" style="z-index: 100"/>&nbsp;
                        <asp:LinkButton ID="ForgotPasswordButton" CssClass="DefaultLoginPageLink" runat="server" Text="Forgot Password" TabIndex="7" OnClick="ForgotPasswordButtonClick" />
                    </div>
                    
                    <asp:Label ID="ServiceInterruptionLabel" runat="server" CssClass="formfieldtitle" Style="left: 35px; top: 390px; color: #ff0000; font-family: Arial, Helvetica,sans-serif; font-size: 12px; position: absolute" Width="72px" Visible="false">There is currently a service interruption.</asp:Label>	

                    <FMControls:FMLabel ID="FMLabelBuildVersion" runat="server" CssClass="formfieldtitle"
                        Style="left: 35px; top: 420px; color: #666666; font-family: Arial, Helvetica,sans-serif; font-size: 12px; position: absolute" 
                        Width="72px" Text="BUILD VERSION BUILD DATE" />

                    <div id="DefaultLoginPageLinkDiv" class="DefaultLoginPageLink" style="color: #666666; font-family: Arial,Helvetica,sans-serif; font-size: 11px; position: absolute; left: 35px; top: 435px; text-align:left">
                        Varec, Inc. 5834 Peachtree Corners East, Norcross (Peachtree Corners), GA 30092 USA<br />
                        <asp:HyperLink ID="ContactUsHyperLink" CssClass="DefaultLoginPageLink" runat="server" Target="_top" EnableViewState="false">Contact Us</asp:HyperLink>
                        |
                        <asp:HyperLink ID="SupportHyperLink" CssClass="DefaultLoginPageLink" runat="server" Target="_top" EnableViewState="false">Support</asp:HyperLink>
                        <asp:Label runat="server" ID="PrivacySeparatorLabel" EnableViewState="false"> | </asp:Label>
                        <asp:HyperLink ID="PrivacyHyperLink" CssClass="DefaultLoginPageLink" runat="server" Target="_top" EnableViewState="false">Privacy Statement</asp:HyperLink>
                        <asp:Label runat="server" ID="CopyrightSeparatorLabel" EnableViewState="false"> | </asp:Label>
                        <asp:HyperLink ID="CopyrightHyperLink" CssClass="DefaultLoginPageLink" runat="server" Target="_top" EnableViewState="false">Copyright</asp:HyperLink>
                        <asp:Label runat="server" ID="DlaPrivacyPolicySeparatorLabel" Visible="False" EnableViewState="false"> | </asp:Label>
                        <asp:LinkButton ID="DlaPrivacyPolicyLink" CssClass="DefaultLoginPageLink" runat="server" Target="_top" EnableViewState="false" Visible="False" OnClientClick="openPrivacyPolicy()">Privacy Policy</asp:LinkButton>
                        <asp:HiddenField ID="ppPath" ClientIDMode="Static" runat="server" Value="" />
                    </div>

                </div>
            </div>
        <asp:HiddenField ID="ppPasswordHint" runat="server"/>
        <asp:TextBox ID="pointgroupreportgeneration" runat="server" CssClass="hidden" TabIndex="-1" Width="0px"/>
        <asp:TextBox ID="SurpressLoginPageLinksTB" runat="server" CssClass="hidden" TabIndex="-1" Width="0px"/>
        <asp:TextBox ID="PageTitleTb" runat="server" CssClass="hidden" TabIndex="-1" Width="0px"/>
        <asp:HiddenField ID="LoginCSRFToken" ClientIDMode="Static" runat="server" Value="" />

    </form>

    <div id="JSONWarningLabel" style="text-align: center; width: 675px; visibility: hidden;">
        <p id="JSONWarningText" style="padding: 15px; color: black; background-color: white; font-weight: bold; border: solid 2px black; font-size: 18px" />
    </div>
</body>
</html>
