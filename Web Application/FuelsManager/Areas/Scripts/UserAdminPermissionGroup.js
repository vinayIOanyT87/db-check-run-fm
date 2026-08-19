var FMUserAdminPermissionGroup = FMUserAdminPermissionGroup ||
	{
	    imageRootPath: ""
        , RowSiteLabelPrefix: "RowSiteLabel_"   // append site guid
        , SiteArrowPrefix: "SiteArrow_"         // append site guid
        , SiteLabelPrefix: "SiteLabel_"         // append site guid
        , RowSiteGroupPrefix: "RowSiteGroup_"   // append site guid
        , TableGroupPrefix: "Table-Group_"      // append site guid
        , RowGroupLabelPrefix: "RowGroupLabel_" // append group guid
        , GroupArrowPrefix: "GroupArrow_"       // append group guid
        , GroupLabelPrefix: "GroupLabel_"       // append group guid
        , RowGroupRightPrefix: "RowGroupRight_" // append group guid
        , TableRightsPrefix: "TableRights_"     // append group guid
        , RowRightLabelPrefix: "RowRightLabel_" // append right index and group guid
        , RightLabelPrefix: "RightLabel_"       // append right index and group guid
        , SmallArrowUpImage: "SmallArrowUp.png"
        , SmallArrowDownImage: "SmallArrowDown.png"
        , siteExpandStateList: []
        , groupExpandStateList: []
        , rightDescriptionList: []
        , lastSelectedRightId: ""
	};

//===================================================================
// Create the user permission tree.
//===================================================================
FMUserAdminPermissionGroup.CreatePermissionTree = function() {
    var tableSite = document.getElementById("TableSites");
    var userAdminModel = FMUserAdministration.GetUserAdministrationModel();

    if (tableSite != null && userAdminModel != null)
    {
        var permissionGroupModel = userAdminModel.PermissionGroupModel;
        var siteGroupRightList = permissionGroupModel.SiteGroupRightList;

        if (siteGroupRightList.length === 0)
        {
            return;
        }

        FMUserAdminPermissionGroup.siteExpandStateList = new Array();
        FMUserAdminPermissionGroup.groupExpandStateList = new Array();
        FMUserAdminPermissionGroup.rightDescriptionList = new Array();

        for (var nextSite = 0; nextSite < siteGroupRightList.length; nextSite++)
        {
            var siteModel = siteGroupRightList[nextSite];
            FMUserAdminPermissionGroup.CreateSiteRow(siteModel, tableSite);
        }
    }
}

//========================================================================================
// Create a Site row with information for each user site.
//========================================================================================
FMUserAdminPermissionGroup.CreateSiteRow = function (siteModel, tableSite) 
{
    // Add new site row to bottom of table
    var newSiteLabelRow = tableSite.insertRow(tableSite.rows.length);
    newSiteLabelRow.id = FMUserAdminPermissionGroup.RowSiteLabelPrefix + siteModel.SiteGuidStr;

    // Create columns "<td>" for the new table row.
    var siteLabelCell = newSiteLabelRow.insertCell(0);

    var newSiteArrowImage = document.createElement("img");
    newSiteArrowImage.src = FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowDownImage;
    newSiteArrowImage.width = "18";
    newSiteArrowImage.height = "18";
    newSiteArrowImage.id = FMUserAdminPermissionGroup.SiteArrowPrefix + siteModel.SiteGuidStr;
    newSiteArrowImage.setAttribute("onclick", "FMUserAdminPermissionGroup.ExpandCollapseSite('" + newSiteArrowImage.id + "')");
    siteLabelCell.appendChild(newSiteArrowImage);

    // Create Site Label
    var newSiteLabel = document.createElement("label");
    newSiteLabel.innerHTML = siteModel.SiteName;
    newSiteLabel.id = FMUserAdminPermissionGroup.SiteLabelPrefix + siteModel.SiteGuidStr;
    newSiteLabel.classList.add("formfieldtitle");
    siteLabelCell.appendChild(newSiteLabel);

    // Create the site row group table
    var newSiteGroupRow = tableSite.insertRow(tableSite.rows.length);
    newSiteGroupRow.id = FMUserAdminPermissionGroup.RowSiteGroupPrefix + siteModel.SiteGuidStr;

    var siteGroupCell = newSiteGroupRow.insertCell(0);
    var groupTable = document.createElement("TABLE");
    groupTable.id = FMUserAdminPermissionGroup.TableGroupPrefix + siteModel.SiteGuidStr;
    groupTable.style.marginLeft = "20px";
    siteGroupCell.appendChild(groupTable);

    // Set the site expand/collapse state for the current row.
    var siteExpand = FMUserAdminPermissionGroup.CreateSiteExpandObject();
    siteExpand.Key = newSiteGroupRow.id;
    siteExpand.State = 1;
    FMUserAdminPermissionGroup.siteExpandStateList.push(siteExpand);

    // Create group rows for each group.
    var groupList = siteModel.GroupList;
    for (var nextGrp = 0; nextGrp < groupList.length; nextGrp++)
    {
        var groupModel = groupList[nextGrp];
        FMUserAdminPermissionGroup.CreateGroupRow(groupModel, groupTable, siteModel.SiteGuidStr);
    }
}

//========================================================================================
// Create a Group row with information for a site.
//========================================================================================
FMUserAdminPermissionGroup.CreateGroupRow = function (groupModel, groupTable, siteGuidStr)
{
    // Add new group row to bottom of table
    var newGroupLabelRow = groupTable.insertRow(groupTable.rows.length);
    newGroupLabelRow.id = FMUserAdminPermissionGroup.RowGroupLabelPrefix + groupModel.GroupGuidStr + "_" + siteGuidStr;

    // Create columns "<td>" for the new table row.
    var groupLabelCell = newGroupLabelRow.insertCell(0);

    var newGroupArrowImage = document.createElement("img");
    newGroupArrowImage.src = FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowUpImage;
    newGroupArrowImage.width = "18";
    newGroupArrowImage.height = "18";
    newGroupArrowImage.id = FMUserAdminPermissionGroup.GroupArrowPrefix + groupModel.GroupGuidStr + "_" + siteGuidStr;
    newGroupArrowImage.setAttribute("onclick", "FMUserAdminPermissionGroup.ExpandCollapseGroup('" + newGroupArrowImage.id + "')");
    groupLabelCell.appendChild(newGroupArrowImage);

    // Create Group Label
    var newGroupLabel = document.createElement("label");
    newGroupLabel.innerHTML = groupModel.GroupName;
    newGroupLabel.id = FMUserAdminPermissionGroup.GroupLabelPrefix + groupModel.GroupGuidStr + "_" + siteGuidStr;
    newGroupLabel.classList.add("formfieldtitle");
    groupLabelCell.appendChild(newGroupLabel);

    // Create the group row Right table
    var newGroupRightRow = groupTable.insertRow(groupTable.rows.length);
    newGroupRightRow.id = FMUserAdminPermissionGroup.RowGroupRightPrefix + groupModel.GroupGuidStr + "_" + siteGuidStr;
    newGroupRightRow.style.display = "none";

    var groupRightCell = newGroupRightRow.insertCell(0);
    var rightTable = document.createElement("TABLE");
    rightTable.id = FMUserAdminPermissionGroup.TableRightsPrefix + groupModel.GroupGuidStr + "_" + siteGuidStr;
    rightTable.style.marginLeft = "35px";
    groupRightCell.appendChild(rightTable);

    // Set the group expand/collapse state for the current row.
    var groupExpand = FMUserAdminPermissionGroup.CreateGroupExpandObject();
    groupExpand.Key = newGroupRightRow.id;
    groupExpand.State = 0;
    FMUserAdminPermissionGroup.groupExpandStateList.push(groupExpand);

    // Create right rows for each group right.
    var rightList = groupModel.RightList;
    for (var nextRight = 0; nextRight < rightList.length; nextRight++)
    {
        var rightModel = rightList[nextRight];
        FMUserAdminPermissionGroup.CreateRightRow(rightModel, rightTable, groupModel.GroupGuidStr, siteGuidStr);
    }
}

//========================================================================================
// Create a Right row with information for a group.
//========================================================================================
FMUserAdminPermissionGroup.CreateRightRow = function (rightModel, rightTable, groupGuidStr, siteGuidStr)
{
    // Add new group row to bottom of table
    var newRightLabelRow = rightTable.insertRow(rightTable.rows.length);
    newRightLabelRow.id = FMUserAdminPermissionGroup.RowRightLabelPrefix + rightModel.RightIndexStr + "_" + groupGuidStr + "_" + siteGuidStr;

    // Create columns "<td>" for the new table row.
    var rightLabelCell = newRightLabelRow.insertCell(0);

    // Create Right Label
    var newRightLabel = document.createElement("label");
    newRightLabel.innerHTML = rightModel.Name;
    newRightLabel.id = FMUserAdminPermissionGroup.RightLabelPrefix + rightModel.RightIndexStr + "_" + groupGuidStr + "_" + siteGuidStr;
    newRightLabel.setAttribute("onclick", "FMUserAdminPermissionGroup.PopulateDescriptionEvent('" + newRightLabel.id + "')");
    newRightLabel.classList.add("formfieldtitle");
    rightLabelCell.appendChild(newRightLabel);

    // Create a right description list for searching.
    var rightDescriptionObj = FMUserAdminPermissionGroup.CreateRightDescriptionObject();
    rightDescriptionObj.Key = newRightLabel.id;
    rightDescriptionObj.Description = rightModel.Description;
    FMUserAdminPermissionGroup.rightDescriptionList.push(rightDescriptionObj);
}

//==========================================================================
// This function will handle the right label onclick event. It wil
// populate the Description test area.
//==========================================================================
FMUserAdminPermissionGroup.PopulateDescriptionEvent = function (rightLabelId)
{
    // Clear right description text area.
    $("#RightDescriptionTB").val("");
    FMUserAdminPermissionGroup.ResetRightLabelColor();

    for (var nextRight = 0; nextRight < FMUserAdminPermissionGroup.rightDescriptionList.length; nextRight++)
    {
        var rightDescriptionObj = FMUserAdminPermissionGroup.rightDescriptionList[nextRight];
        if (rightDescriptionObj.Key === rightLabelId)
        {
            $("#RightDescriptionTB").val(rightDescriptionObj.Description);
            $("#" + rightLabelId).css("color", "#567eb9");
            FMUserAdminPermissionGroup.lastSelectedRightId = rightLabelId;
        }
    }
}

//================================================================
// This function will reset the Right Label text coloring back to
// black.
//================================================================
FMUserAdminPermissionGroup.ResetRightLabelColor = function ()
{
    if (FMUserAdminPermissionGroup.lastSelectedRightId !== "")
    {
        $("#" + FMUserAdminPermissionGroup.lastSelectedRightId).css("color", "black");
        FMUserAdminPermissionGroup.lastSelectedRightId = "";
    }
}

//==================================================================
// This function will expand/collapse the site group row based on
// current state.
//==================================================================
FMUserAdminPermissionGroup.ExpandCollapseSite = function (rowImageId)
{
    // Clear right description text area.
    $("#RightDescriptionTB").val("");
    FMUserAdminPermissionGroup.ResetRightLabelColor();

    var rowGuid = FMUserAdminPermissionGroup.ExtractGuidFromId(rowImageId);

    if (rowGuid !== "")
    {
        var searchKey = FMUserAdminPermissionGroup.RowSiteGroupPrefix + rowGuid;

        for (var nextState = 0; nextState < FMUserAdminPermissionGroup.siteExpandStateList.length; nextState++)
        {
            var siteExpand = FMUserAdminPermissionGroup.siteExpandStateList[nextState];
            if (siteExpand.Key === searchKey)
            {
                // Site row is collapsed, expand row.
                if (siteExpand.State === 0)
                {
                    siteExpand.State = 1;
                    $("#" + searchKey).show();
                    $("#" + rowImageId).attr("src", FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowDownImage);
                }
                else
                {
                    siteExpand.State = 0;
                    $("#" + searchKey).hide();
                    $("#" + rowImageId).attr("src", FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowUpImage);
                }

                break;
            }
        }
    }
}

//==============================================================
// This function handles the Expand All and Collapse All event.
// It will expand or collapse all the nodes.
//==============================================================
FMUserAdminPermissionGroup.ExpandCollapseAll = function (state)
{
    // Clear right description text area.
    $("#RightDescriptionTB").val("");
    FMUserAdminPermissionGroup.ResetRightLabelColor();

    for (var nextState = 0; nextState < FMUserAdminPermissionGroup.siteExpandStateList.length; nextState++)
    {
        var siteExpand = FMUserAdminPermissionGroup.siteExpandStateList[nextState];
        var parts = siteExpand.Key.split("_");
        var prefixKey = parts[0] + "_";
        var rowImageId = FMUserAdminPermissionGroup.SiteArrowPrefix + parts[1];

        if (prefixKey === FMUserAdminPermissionGroup.RowSiteGroupPrefix)
        {
            if (state === "EXPAND")
            {
                siteExpand.State = 1;
                $("#" + siteExpand.Key).show();
                $("#" + rowImageId).attr("src", FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowDownImage);
                FMUserAdminPermissionGroup.ExpandCollapseAllGroups(state, parts[1]);
            }
            else if(state === "COLLAPSE")
            {
                siteExpand.State = 0;
                $("#" + siteExpand.Key).hide();
                $("#" + rowImageId).attr("src", FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowUpImage);
                FMUserAdminPermissionGroup.ExpandCollapseAllGroups(state, parts[1]);
            }
        }
    }
}

//================================================================================
// This function handles the expand/collapse all groups event.
//================================================================================
FMUserAdminPermissionGroup.ExpandCollapseAllGroups = function (state, siteGuidKey)
{
    for (var nextState = 0; nextState < FMUserAdminPermissionGroup.groupExpandStateList.length; nextState++)
    {
        var groupExpand = FMUserAdminPermissionGroup.groupExpandStateList[nextState];
        var parts = groupExpand.Key.split("_");
        var prefixKey = parts[0] + "_";
        var groupGuid = parts[1];
        var siteGuid = parts[2];
        var rowImageId = FMUserAdminPermissionGroup.GroupArrowPrefix + groupGuid + "_" + siteGuid;

        if (prefixKey === FMUserAdminPermissionGroup.RowGroupRightPrefix && siteGuid === siteGuidKey)
        {
            if (state === "EXPAND")
            {
                groupExpand.State = 1;
                $("#" + groupExpand.Key).show();
                $("#" + rowImageId).attr("src", FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowDownImage);
            }
            else if (state === "COLLAPSE")
            {
                groupExpand.State = 0;
                $("#" + groupExpand.Key).hide();
                $("#" + rowImageId).attr("src", FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowUpImage);
            }
        }
    }  
}

//==================================================================
// This function will expand/collapse the group right row based on
// current state.
//==================================================================
FMUserAdminPermissionGroup.ExpandCollapseGroup = function (rowImageId)
{
    // Clear right description text area.
    $("#RightDescriptionTB").val("");
    FMUserAdminPermissionGroup.ResetRightLabelColor();

    var rowGuid = FMUserAdminPermissionGroup.ExtractTwoGuidFromId(rowImageId);

    if (rowGuid !== "")
    {
        var searchKey = FMUserAdminPermissionGroup.RowGroupRightPrefix + rowGuid;

        for (var nextState = 0; nextState < FMUserAdminPermissionGroup.groupExpandStateList.length; nextState++)
        {
            var groupExpand = FMUserAdminPermissionGroup.groupExpandStateList[nextState];
            if (groupExpand.Key === searchKey)
            {
                // Group row is collapsed, expand row.
                if (groupExpand.State === 0)
                {
                    groupExpand.State = 1;
                    $("#" + searchKey).show();
                    $("#" + rowImageId).attr("src", FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowDownImage);
                }
                else
                {
                    groupExpand.State = 0;
                    $("#" + searchKey).hide();
                    $("#" + rowImageId).attr("src", FMUserAdminPermissionGroup.imageRootPath + FMUserAdminPermissionGroup.SmallArrowUpImage);
                }

                break;
            }
        }
    }
}

//=======================================================
// This function will parse a tag ID that has a prefix
// followed by Guid and return the Guid.
//=======================================================
FMUserAdminPermissionGroup.ExtractGuidFromId = function (id)
{
    var parts = id.split("_");
    if (parts != null && parts.length > 1)
    {
        return parts[1];
    }

    return "";
}

//=======================================================
// This function will parse a tag ID that has a prefix
// followed by Guid and return the Guid.
//=======================================================
FMUserAdminPermissionGroup.ExtractTwoGuidFromId = function (id) {
    var parts = id.split("_");
    if (parts != null && parts.length > 2)
    {
        return (parts[1] + "_" + parts[2]);
    }

    return "";
}

//=============================================================
// This function will return a site expand object.
//=============================================================
FMUserAdminPermissionGroup.CreateSiteExpandObject = function ()
{
    var siteExpand = new Object();
    siteExpand.Key = "";
    siteExpand.State = 0;

    return siteExpand;
}

//=============================================================
// This function will return a group expand object.
//=============================================================
FMUserAdminPermissionGroup.CreateGroupExpandObject = function ()
{
    var groupExpand = new Object();
    groupExpand.Key = "";
    groupExpand.State = 0;

    return groupExpand;
}

//=============================================================
// This function will return a right description object.
//=============================================================
FMUserAdminPermissionGroup.CreateRightDescriptionObject = function ()
{
    var rightDescriptionObject = new Object();
    rightDescriptionObject.Key = "";
    rightDescriptionObject.Description = "";

    return rightDescriptionObject;
}