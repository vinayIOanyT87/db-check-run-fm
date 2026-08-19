var FMUserAdministration = FMUserAdministration ||
	{
	    imageRootPath: ""
        , firstTimeCall: true
        , userInfoTabKey: "UserInfoTab"
        , userPermissionAndGroupTabKey: "UserPermissionAndGroupTab"
        , userAdminAndAuditTabKey: "UserAdminAndAuditTab"
	};

//============================================================
// This function will handle the user info section selection
// event.
//============================================================
FMUserAdministration.ShowUserInfoSection = function ()
{
    $("#PermissionGroupItem").removeClass("selected");
    $("#AdminAuditItem").removeClass("selected");
    $("#UserInfoItem").addClass("selected");

    $("#PermissionGroupItemBtag").removeClass("selected");
    $("#AdminAuditItemBtag").removeClass("selected");
    $("#UserInfoItemBtag").addClass("selected");

    $("#UserInfoSection").removeClass("hidden");
    $("#PermissionGroupSection").addClass("hidden");
    $("#AdminAuditSection").addClass("hidden");

    FMUserAdministration.UpdateUserAdminHelpKey(FMUserAdministration.userInfoTabKey);
}

//============================================================
// This function will handle the user permission and group 
// section selection event.
//============================================================
FMUserAdministration.ShowPermissionGroupSection = function ()
{
    $("#AdminAuditItem").removeClass("selected");
    $("#UserInfoItem").removeClass("selected");
    $("#PermissionGroupItem").addClass("selected");

    $("#AdminAuditItemBtag").removeClass("selected");
    $("#UserInfoItemBtag").removeClass("selected");
    $("#PermissionGroupItemBtag").addClass("selected");

    $("#PermissionGroupSection").removeClass("hidden");
    $("#UserInfoSection").addClass("hidden");
    $("#AdminAuditSection").addClass("hidden");

    FMUserAdministration.UpdateUserAdminHelpKey(FMUserAdministration.userPermissionAndGroupTabKey);
}

//============================================================
// This function will handle the user administration and audit 
// section selection event.
//============================================================
FMUserAdministration.ShowAdminAuditSection = function ()
{
    $("#UserInfoItem").removeClass("selected");
    $("#PermissionGroupItem").removeClass("selected");
    $("#AdminAuditItem").addClass("selected");

    $("#UserInfoItemBtag").removeClass("selected");
    $("#PermissionGroupItemBtag").removeClass("selected");
    $("#AdminAuditItemBtag").addClass("selected");

    $("#AdminAuditSection").removeClass("hidden");
    $("#PermissionGroupSection").addClass("hidden");
    $("#UserInfoSection").addClass("hidden");

    FMUserAdministration.UpdateUserAdminHelpKey(FMUserAdministration.userAdminAndAuditTabKey);

    if (FMUserAdministration.firstTimeCall)
    {
        FMUserAdminAudit.HandleRefreshBtnEvent();
        FMUserAdministration.firstTimeCall = false;
    }
}

//=====================================================================
// This function will populate the user info section.
//=====================================================================
FMUserAdministration.PopulateUserInfo = function ()
{
    var model = FMUserAdministration.GetUserAdministrationModel();

    if (model != null)
    {
        $("#UserInfo_UserID").val(model.UserId);
        $("#UserInfo_UserName").val(model.UserName);
        $("#UserInfo_EmailAddress").val(model.EmailAddress);
    }
}


//=====================================================================
// This function will return the user administration model as a string.
//=====================================================================
FMUserAdministration.GetUserAdministrationModelString = function ()
{
    return $('#UserAdministrationModel').val();
};

//=====================================================================
// This function will return the user administration model as an object.
//=====================================================================
FMUserAdministration.GetUserAdministrationModel = function ()
{
    var strModel = FMUserAdministration.GetUserAdministrationModelString();

    if (strModel === undefined)
    {
        return undefined;
    }

    var model = JSON.parse(strModel);
    return model;
};

//===================================================================
// This function will set the help key based on the selected tab.
//===================================================================
FMUserAdministration.UpdateUserAdminHelpKey = function (tabType)
{
    switch (tabType)
    {
        case FMUserAdministration.userInfoTabKey:
            window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../UserAdministrationArea/UserConfiguration/UserConfigurationView/UserInfoTab";
            break;
        case FMUserAdministration.userPermissionAndGroupTabKey:
            window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../UserAdministrationArea/UserConfiguration/UserConfigurationView/UserPermissionGroupTab";
            break;
        case FMUserAdministration.userAdminAndAuditTabKey:
            window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../UserAdministrationArea/UserConfiguration/UserConfigurationView/UserAdminAuditTab";
            break;
    }
}