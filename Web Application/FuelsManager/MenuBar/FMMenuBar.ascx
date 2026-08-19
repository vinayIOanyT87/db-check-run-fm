<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FMMenuBar.ascx.cs" Inherits="FuelsManager.FMWebApp.FMMenuBar" %>

<%@ Import Namespace="FMBusinessObjects.DataObjects" %>
<%@ Import Namespace="FMBusinessObjects.UtilityObjects" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI.HtmlControls" Assembly="System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>

<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS_menubar.css" %>" media="screen" rel="stylesheet" type="text/css" />
<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/MenuBar/jquery.contextmenu.css" %>" rel="stylesheet" type="text/css" />
<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Content/select2.min.css" %>" rel="stylesheet" type="text/css" />
<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/MenuBar/menu.css" %>" media="screen" rel="stylesheet" type="text/css" />
<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Content/bootstrap.css" %>" media="screen" rel="stylesheet" type="text/css" />
<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/Content/redmond/jquery.ui.base.css" %>" rel="stylesheet" type="text/css" />
<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/Content/redmond/jquery.ui.theme.css" %>" rel="stylesheet" type="text/css" />
<%=  Global.LinkAccessibilityCssUrl(Session) %>

 
<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CFS_menubar.js" %>" type="text/javascript" ></script><!--Must come after CFS.css -->

<% if (Global.IsFdsIM)
    { %>
<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/duplicatesessionprevention.js" %>" type="text/javascript" defer> </script>
<% } %>

<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>" defer></script>

<!--[if IE 6]>
<style>
body {behavior: url("csshover3.htc");}
#menu li .drop {background:url("<%= ResolveUrl("~/FMWebApp/images/drop.png") %>") no-repeat right 8px; 
}
</style>
<![endif]-->
<style>

    .licenseStatusLinksBar {
    display:table-cell;
    width:max-content;
    margin:0;
    background: #3B577F;
    color: yellow;
    vertical-align:middle;
    text-align: right;
    }
</style>


<script type='text/javascript' >
    if (typeof(rndTokenStr) == "undefined")
    {
        rndTokenStr = new Object();
    }
    rndTokenStr = '<%= security.CSRFToken %>';
    /* <%= GetRandomLengthString() %> */
</script>

<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Scripts/jquery-2.2.1.min.js" %>" type="text/javascript"></script>
<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/MenuBar/jquery.contextmenu.min.js" %>" type="text/javascript"></script>
<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Scripts/select2.full.min.js" %>" type="text/javascript"></script>
<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/Scripts/Layout.js" %>" type="text/javascript"></script>
<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/scripts/KioskKeyRestrictions.js" %>" type="text/javascript"></script>
<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/MenuBar/FMMenuBar.js" %>" type="text/javascript"></script>

<%-- Load jQuery only if it is not already loaded. --%>
<script>
    var MenuStartup = {};
    MenuStartup.LoadjQuery = function ()
    {
        if (!window.jQuery) {
            var headTag = document.getElementsByTagName("head")[0];
            var jqTag = document.createElement('script');
            jqTag.type = 'text/javascript';
            jqTag.src = '../Scripts/jquery-2.2.1.min.js';
            headTag.appendChild(jqTag);
        }
    };

    // on ready
    $(function () {
        FMMenu.SetPageContentSize();
    });

    $(window).on('resize', function () {
        FMMenu.SetPageContentSize();
    });

    var logoutPage = "<%= ResolveUrl("~/FMWebApp/LogoutForm.aspx") %>";
    var mainPage = "http://localhost" + "<%= ResolveUrl("~/FMWebApp/FuelsManagerForm.aspx") %>";
</script>


<script>
    /* THE FOLLOWING JAVASCRIPT IS PART OF THE ALARM NOTIFICATION PROTOTYPE, IF SELECTED WE WILL NEED TO MOVE IT TO FMMenuBar.js */
    // Retrieve alerts from the server
    var failures = 0;
    var xhrStatus0Failures = 0;
    let nextAlertTimeoutWhenError = 1000;

    var checkAlerts = function()
    {
        $.ajax( {
            type: "GET",
            cache: false,
            url: "../InventoryManagement/AlarmSummary2/AlarmNotificationsForMenu",
            data: {"_":new Date().getTime()},
           statusCode: {
               403:function() { console.log("Security object missing from Session."); }
            },
            success: function( response, textStatus, xhr)
            {
                nextAlertTimeoutWhenError = 1000;
                if (response == "Exception" || xhr.getResponseHeader('content-type').indexOf('text/html') >= 0 ) {
                    failures++;
                    console.log("Controller Ping or Alarm Notification refresh has failed " + failures + " times.")
                    console.log("response=" + response);
                }
          //      console.log("checkAlerts response " + response + " textStatus=" + textStatus);
                if (response < 0){
                    //Invalid session. Controller ping will alert user.
                    console.log("Invalid Session.");
                    return;
                }

                var audio = document.getElementById("mhbAlarmsCritical");

                if (response.AlarmDetail && response.AlarmDetail.AlarmSummaries.length > 0)
                {
                    $("#mhbAlarms").removeClass("hidden");
                    $( "#mhbAlarmCount" ).removeClass( "hidden" );
                    $("#mhbAlarmCount").text(response.NumberOfAlarms);

                    audio.pause();

                    var alarm = response.AlarmDetail.AlarmSummaries[0];

                    if (alarm.IsNormal) {
                        $(".badge-notify.blink").css("color", "#" + alarm.NormalUnacknowledgedAlarmTextSteadyColor);
                        $(".badge-notify.blink").css("background-color", "#" + alarm.NormalUnacknowledgedAlarmBackgroundSteadyColor);
                    }
                    else {
                        $(".badge-notify.blink").css("color", "#" + alarm.AlarmTextSteadyColor);
                        $(".badge-notify.blink").css("background-color", "#" + alarm.AlarmBackgroundSteadyColor);
                    }

                    try {
                        if (alarm.SoundFile) {
                            $("#mhbAlarmsCritical-audio").attr("src", $("#mhbAlarmsCritical").attr("data-default-sound-path") + alarm.SoundFile);
                        }
                        else {
                            $("#mhbAlarmsCritical-audio").attr("src", $("#mhbAlarmsCritical").attr("data-default-audio"));
                        }
                    }
                    catch (e) {
                    }

                    if (!alarm.Silenced) {
                        try {
                            audio.src = $("#mhbAlarmsCritical-audio").attr("src");
                        }
                        catch (e) {
                        }

                        try {
                            audio.load();
                            var playPromise = audio.play();
                            if (playPromise !== undefined) {
                                playPromise.then(function () {
                                    // Automatic playback started!
                                }).catch(function (error) {
                                    // Automatic playback failed.
                                    audio.src = $("#mhbAlarmsCritical").attr("data-default-audio");
                                    audio.load();
                                    audio.play();
                                });
                            }
                            else // ie does not support promises and we need a different type of check
                            {
                                audio.onerror = function () {
                                    // Automatic playback failed.
                                    audio.src = $("#mhbAlarmsCritical").attr("data-default-audio");
                                    audio.load();
                                    audio.play();

                                }
                            }
                        }
                        catch (e) {
                            audio.src = $("#mhbAlarmsCritical").attr("data-default-audio");
                            audio.load();
                            audio.play();
                        }
                    }

                }
                else
                {
                    $( "#mhbAlarms" ).removeClass( "hidden" ).addClass( "hidden" );
                    $("#mhbAlarmCount").removeClass("hidden").addClass("hidden");
                    audio.pause();
                }

                setTimeout( function()
                {
                    checkAlerts();
                }, 1000 );
            },
            error: function( xhr, textStatus, error )
            {
                if (xhr.status === 200 || xhr.status === 500) {
                    //debugger;
                    console.log("Alerts refresh has failed with HTTP status code " + xhr.status + ".")
                    if (nextAlertTimeoutWhenError > 60000) {
                        window.alert("Controller ping experiencing consecutive errors. See event logs.");
                        nextAlertTimeoutWhenError = 2000;
                    }
                    else {
                        nextAlertTimeoutWhenError += nextAlertTimeoutWhenError;
                    }

                    setTimeout(function () {
                        checkAlerts();
                    }, nextAlertTimeoutWhenError);
                    return;
                }

                if(xhr.status !== 503 &&
                    xhr.status !== 404)
                {
                    setTimeout( function()
                    {
                        checkAlerts();
                    }, 1000 );
                }
            }

        } );
    }

    // Retrieve Unresolved Conflicts from the server
    var checkSyncUnresolvedConflicts = function()
    {
        $.ajax( {
            type: "GET",
            cache: false,
            url: "../InventoryManagement/AlarmSummary2/SyncUnresolvedConflictsCount",
            data: {"_":new Date().getTime()},
           statusCode: {
               403: function () { console.log("Security object missing from Session."); }
            },
            success: function( response, textStatus, xhr )
            {
                if (response == "Exception" || xhr.getResponseHeader('content-type').indexOf('text/html') >= 0 ) {
                    failures++;
                    console.log("checkUnresolvedConflicts refresh has failed " + failures + " times.")
                }
           //     console.log("checkUnresolvedConflicts response " + response + " textStatus=" + textStatus);
                if (response < 0){
                    //Invalid session. Controller ping will alert user.
                    console.log("Invalid Session.");
                    return;
                }
                if (response > 0)
                {
                    $("#mhbConflicts").removeClass("hidden");
                    $( "#mhbConflictsCount" ).removeClass( "hidden" );
                    $("#mhbConflictsCount").text(response);
                }
                else
                {
                    $( "#mhbConflicts" ).removeClass( "hidden" ).addClass( "hidden" );
                    $("#mhbConflictsCount").removeClass("hidden").addClass("hidden");
                }

                setTimeout( function()
                {
                    checkSyncUnresolvedConflicts();
                }, 10000 );
            },
            error: function( xhr, textStatus, error )
            {
                if(xhr.status !== 503 &&
                    xhr.status !== 404)
                {
                    setTimeout( function()
                    {
                        checkSyncUnresolvedConflicts();
                    }, 10000 );
                }
            }

        } );
    }

    let nextControllerPingTimeoutWhenError = 1000;
    let failure403 = 0;
    var serverOffLine = false;
    var pingServer = function()
    {
        //debugger;
        $.ajax( {
            type: "GET",
            cache: false,
            url: "../InventoryManagement/AlarmSummary2/ControllerPingMechanism",
            data: {"_":new Date().getTime()},
            statusCode: {
                403: function () { console.log("Security object missing from Session."); }
             },
            success: function( response, textStatus, xhr )
            {
                nextControllerPingTimeoutWhenError = 1000;
                failure403 = 0;

                xhrStatus0Failures = 0;
                // if we get here and the server has failed, redirect to the log in page
                if(serverOffLine === true || xhr.getResponseHeader('content-type').indexOf('text/html') >= 0 )
                {
                    window.location.href = "<%=ResolveUrl("~/FMWebApp/LogoutForm.aspx")%>";
                    return;
                }
          //      console.log("pingServer response " + response + " textStatus=" + textStatus);
                if (response < 0){
                    window.location.href = "<%=ResolveUrl("~/FMWebApp/LogoutForm.aspx")%>";
                    if (response == -2) {
                        console.log("Session timed out.");
                        alert("Session timed out.");
                        return;
                    }
                    console.log("Invalid Session.");
                    alert("Invalid Session.");
                    return;
                }
                
                setTimeout( function()
                {
                    pingServer();
                }, 1000 );
            },
            error: function( xhr, textStatus, error )
            {
                //debugger;
                if (xhr.status == 403) {
                    failure403++;
                    console.log("Controller Ping refresh has failed " + failure403 + " time(s)." + xhr.status);
                    if (failure403 > 4) {
                        window.alert("HTTP status code 403 error received " + failure403 + " times in a row. Ending session.");
                        failure403 = 0;
                        window.location.href = "<%=ResolveUrl("~/FMWebApp/LogoutForm.aspx")%>"   ;
                        return;
                    }
                    setTimeout(function () {
                            pingServer();
                        }, 1000);
                }
                else {
                    if (xhr.status === 200 || xhr.status === 500) {
                        //debugger;
                        console.log("Controller Ping refresh has failed with HTTP status code  " + xhr.status + ".")
                        if (nextControllerPingTimeoutWhenError > 60000) {
                            window.alert("Controller ping experiencing consecutive errors. See event logs.");
                            nextControllerPingTimeoutWhenError = 2000;
                        }
                        else {
                            nextControllerPingTimeoutWhenError += nextControllerPingTimeoutWhenError;
                        }

                        setTimeout(function () {
                            pingServer();
                        }, nextControllerPingTimeoutWhenError);
                        return;
                    }
                    else {
                        failures++;
                        console.log("Controller Ping or Alarm Notification refresh has failed " + failures + " time(s)." + xhr.status);
                        if (failures > 4) {
                            window.alert("Cannot Communicate with Server! Press OK to Retry and return to Log In Screen.");
                            window.location.href = "<%=ResolveUrl("~/FMWebApp/LogoutForm.aspx")%>";
                            return;
                        }
                    }

                    if (xhr.status === 0) {
                        xhrStatus0Failures++;
                        if (xhrStatus0Failures > 4) {
                            console.log("Controller Ping received xhr.status = 0 more than 4 times in a row. ");
                        }
                    }
                    else {
                        xhrStatus0Failures = 0;
                    }

                    if (xhr.status === 503 ||
                        xhr.status === 404 ||
                        xhrStatus0Failures > 4) {
                        serverOffLine = true;
                        var delay = 5; // 5 second delay
                        var now = new Date();
                        var desiredTime = new Date().setSeconds(now.getSeconds() + delay);

                        while (now < desiredTime) {
                            now = new Date(); // update the current time
                        }

                        setTimeout(function () {
                            pingServer();
                        }, 10);
                    }
                    else {
                        setTimeout(function () {
                            pingServer();
                        }, 1000);
                    }
                }

            }

        } );
    }

    //-------------------------------------------------------------------
    // This function will reset the menu bar left image if configured.
    // It is called by document.ready().
    //-------------------------------------------------------------------
	var ResetMenuBarLeftImageUrl = function ()
	{
        var menuBarLeftUrlTextBox = document.getElementById("FMM_MenuLeftImageUrlTB");
        var menuBarLeftImageElement = document.getElementById("MenuLeftImage");

        if (menuBarLeftUrlTextBox != null && menuBarLeftImageElement != null && menuBarLeftUrlTextBox.value !== "EMPTY")
		{
			menuBarLeftImageElement.src = menuBarLeftUrlTextBox.value;
        }
	}

	$( document ).ready( function()
	{
		//debugger;   //bds
		var sessionValue = "<%= this.IsViewOperateOnly() %>";
		if (sessionValue.toLowerCase() === "true") {
			$("#bdyMenuBarBody").removeClass("hidden").addClass("hidden");
		}
        	       
		// when the screen has been fully loaded start checking alerts
		var hasRights = "<%= this.IsAlarmCheckAvailable() %>";
		if ( hasRights.toLowerCase() === "true" )
		{
          checkAlerts();
        }

        // start the check for Sync Conflicts
        checkSyncUnresolvedConflicts();

        // start the ping mechanism
        pingServer();


        // Reset the menu bar left image
        ResetMenuBarLeftImageUrl();
    });

    function FetchMobileDispatchSite() {
        showModalDialogFrame({
            url: "../MobileDispatch/UserAuthForm.aspx",
            width: 450,
            height: 250,
        });
    }

</script>
<div id="bdyMenuBarBody" onresize="FMMenuBarLib.positionQuickLinksLabel()">
	<div id="divMenu" class="mainMenu">
		<div class="mainMenuLogoDiv">
			<img id="MenuLeftImage" style="width: 50px; height: 50px" src="../FMWebApp/images/Varec_w.png" />
		</div>
		<div class="LeftMenuSection">
			<asp:PlaceHolder runat="server" ID="phMenu"></asp:PlaceHolder>
		</div>
		<div class="RightMenuSection">
			<ul id="ulHeaderBar">
				<li id="mhbFullScreenLink" class="hidden">
					<div>
						<a id="mhbFullScreenButton" href="javascript:FMMenuBarLib.fullScreenButtonOnClick()" style="color: aqua; display: none">Full Screen</a>
					</div>
				</li>
				<li id="mhbConflicts" class="hidden">
					<div class="menuConflictsIcon">
						<a onclick="return FMMenu.Nav('../FMEntityImportWebApp/SynchronizationSessionSummary.aspx?WithConflicts=true', '7033', '00000000-0000-0000-0000-000000000000',  '<%= this.security.Token.ToString()%>');" id="Synchronization Session Summary" href="../FMEntityImportWebApp/SynchronizationSessionSummary.aspx">
							<span class="glyphicon glyphicon-transfer mhbConflictsCountIcon" style="font-size: 20px; width: 100%; text-align: left; line-height: 48px;">
								<span id="mhbConflictsCount" class="notify blink hidden">0</span>
							</span>
						</a>
					</div>
				</li>
				<li id="mhbAlarms" class="hidden">
					<div class="menuAlarmIcon">
						<a onclick="return FMMenu.Nav('../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndex?id=alarmsummary', '7041', '00000000-0000-0000-0000-000000000000',  '<%= this.security.Token.ToString()%>');" id="Operations_Inventory Management_Operate" href="../MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndex">
							<span class="glyphicon glyphicon-bell mhbAlarmCountIcon" style="font-size: 20px; width: 100%; text-align: left; line-height: 48px;">
								<span id="mhbAlarmCount" class="badge-notify blink hidden">0</span>
							</span>
						</a>
					</div>
					<audio loop id="mhbAlarmsCritical" data-default-audio="../Areas/InventoryManagement/AlarmDefault.mp3" data-default-sound-path="../FMWebApp/sounds/">
						<source id="mhbAlarmsCritical-audio" src="">
					</audio>
				</li>
				<li id="mhQuickLinksDisplay">
					<div id="quickLinkExpansionContractionDiv" class="quickLinkBox">
						<a id="collapseQuickLinksBtn" class="collapseQuickLinks" href="#" title="Collapse Quick Links"
							onclick="FMMenuBarLib.collapseQuickLinks()"></a>
						<a id="expandQuickLinksBtn" class="expandQuickLinks"
							href="#" title="Expand Quick Links" onclick="FMMenuBarLib.expandQuickLinks()"></a>
					</div>
				</li>
				<li id="mhLocationIcon">
					<div>
						<img src="../FMWebApp/images/Menu-Icon-Place-Tag.svg">
					</div>
				</li>
				<li id="mhbSiteDropDown">
					<div>
						<script type="text/javascript">
                            $(document).ready(function () {
                                $("#SiteSelect").select2({ templateResult: FMMenu.SiteSelectAdded, minimumResultsForSearch: Infinity });
                            });
                        </script>
						<!--style="-moz-min-width: auto; -ms-max-width: 100%; -o-max-width: ; -webkit-min-width: auto; max-width: 100%"-->
						<select id="SiteSelect" style="-moz-min-width: 250px; -ms-max-width: 250px; -o-min-width: 250px; -webkit-min-width: 250px; max-width: 250px;" disabled="disabled"></select>
					</div>
				</li>
				<li id="mhbUserIcon">
					<div class="menuUserIcon">
						<img src="../FMWebApp/images/Menu-Icon-Male.svg">
					</div>
					<div class="menuUserDropdown">
						<div class="topSection">
							<div class="menuUserIcon">
								<img src="../FMWebApp/images/Menu-Icon-Male.svg">
							</div>
						</div>
						<div class="middleSection">
							<asp:Label runat="server" ID="lblLoginUserAndSite" EnableViewState="false">Administrator | SiteAdmin</asp:Label>
							<asp:Label runat="server" ID="lblLoginSite" EnableViewState="false">SiteAdmin</asp:Label>
						</div>
						<div class="bottomSection">
							<asp:HtmlAnchor ID="menuSettings" runat="server" OnServerClick="RedirectToUserSettings">SETTINGS</asp:HtmlAnchor>
                            <asp:HtmlAnchor ID="MyProfile" runat="server" OnServerClick="RedirectToMyProfile">MY PROFILE</asp:HtmlAnchor>
							<a id="menuLogout" runat="server" onclick="document.location.href=logoutPage; return false;">LOGOUT</a>
                        </div>
					</div>
				</li>
			</ul>
		</div>
		<div class="RightMenuSectionSpacer">
		
		</div>
	</div>
			<div id="divQuickLinks" class="quickLinksDiv">
				<div class="quickLinkBarLeft">
					<ul id="quickLinksBar" class="quickLinksBar">
						<asp:PlaceHolder runat="server" ID="phQuickLinks"></asp:PlaceHolder>
					</ul>
				</div>
				<div id="licenseStatus" class="licenseStatusLinksBar" runat="server">
					License status
				</div>
				<div class="quickLinkBarRight">
					<img src="../FMWebApp/images/Menu-Icon-Boxed-Arrow.svg" id="quickLinksShowExtra" onclick="createMenu(event)">
				</div>
			</div>
    
    <div id="cuiTopDiv" class="cuiDiv" runat="server">CUI</div>  
    <div id="cuiBottomDiv" class="cuiDiv cuiBottomDiv" runat="server">CUI</div>

    <%-- Do not make these div tags self closing. It will cause script errors. --%>
    <div id="divSeparatorLine" style="height: 0px; border-style: solid; border-width: 1px 0 0 0;border-color: #CCCCCC"></div>
    <div id="divSpacing" style="height: 3px"></div>
	<asp:HiddenField ID="quickLinksPanelState" ClientIDMode="Static" runat="server" Value="expanded" />
    <asp:HiddenField ID="dbVer" ClientIDMode="Static" runat="server" Value="1.0.0.0" />
    <asp:HiddenField ID="fmVer" ClientIDMode="Static" runat="server" Value="1.0.0.0" />
    <asp:HiddenField ID="bsName" ClientIDMode="Static" runat="server" Value="FMBusinessServices" />
    <asp:HiddenField ID="bsVer" ClientIDMode="Static" runat="server" Value="1.0.0.0" />
    <asp:HiddenField ID="ppPath" ClientIDMode="Static" runat="server" Value="" />
    <asp:HiddenField ID="pageTitle" ClientIDMode="Static" runat="server" Value="EMPTY" />
    <asp:HiddenField ID="webServerName" ClientIDMode="Static" runat="server" Value="" />
	<asp:TextBox ID="MenuLeftImageUrlTB" runat="server" CssClass="hidden" TabIndex="-1" Width="0px"/>

</div>

<asp:PlaceHolder runat="server" ID="phHideIfDialogScript">
    <%-- // This script block is active when ShowIfDialog="false" --%>
	<script type="text/javascript">
	    FMMenuBarLib.hideEvenIfNotDialog = false;
	    FMMenuBarLib.onPreRender();
	</script>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="phHideEvenIfNotDialogScript">
    <%-- // This script block is active when ShowIfDialog="false" --%>
	<script type="text/javascript">
	    FMMenuBarLib.hideEvenIfNotDialog = true;
	    FMMenuBarLib.onPreRender();
	</script>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="phShowEvenIfDialogScript">
	<%-- // This script block is active when ShowIfDialog="true" --%>
	<script type="text/javascript">
	    FMMenuBarLib.onLoad();
	</script>
</asp:PlaceHolder>



<script type="text/javascript">
    FMMenuBarLib.ShowHideExpansionIcon();

    $(window).resize(function () {
        FMMenuBarLib.ShowHideExpansionIcon();
    });
</script>

<input id="DaysLeft" type="hidden" value="<%= this.DaysLeft %>" />
<input id="ApplicationRoot" type="hidden" value="<%= ApplicationRoot %>" />
