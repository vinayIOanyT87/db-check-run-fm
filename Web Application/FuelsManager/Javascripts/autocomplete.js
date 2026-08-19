// This file contains functions to support jQuery UI AutoComplete controls on 
// Transaction Detail.

var FMControlsLib = {};

FMControlsLib.InitializeAutoComplete = function() {
    // This selector will find all DIV tags with the autoCompleteFieldKey 
    // attribute and execute the function for each one.
    var selector = 'div[autoCompleteFieldKey]';    
    $(selector).each(function() {
        FMControlsLib.InitializeFMAutoComplete(
            window.FuelsManagerLib.token,
            $(this).find('input:first')[0].id,
            $(this).attr("autoCompleteFieldKey"),
            $(this).attr("callBack"),
            $(this).attr("dependField"),
			$(this).attr("lineItem"));
    });
};

FMControlsLib.autoSelectionChanged = function (event, ui) {
    var value = ui.item ? ui.item.label : this.value;
    $(this).val(value);

    if ($(this).attr("autoPost") == 'true')
    {
		// Disable Cancel & Update
    	if (this.id.indexOf("LineItemDataGrid") != -1) {
    		var len = this.id.indexOf("_TransactionFields.LineItemProductFG");
    		var cancelId = this.id.substr(0, len) + '_CancelButton';
    		var cancelButton = document.getElementById(cancelId);
    		if (cancelButton != null)
    			cancelButton.href = 'javascript:';
    		var updateId = this.id.substr(0, len) + '_UpdateButton';
    		var updateButton = document.getElementById(updateId);
    		if (updateButton != null)
    			updateButton.href = 'javascript:';
    	}

        // Set focus so the re-focus routines work properly.
    	this.focus();

        // Call postback on the update panel of the control
        __mydoPostBack(this.id, '');
    }
};

FMControlsLib.autoSelectionSelected = function (event, ui) {
	var value = ui.item ? ui.item.label : this.value;
	$(this).val(value);

	if ($(this).attr("autoPost") == 'true') {
		// Clear autoPost to preclued change firing
		$(this).attr("autoPost",'false');

		// Call postback on the update panel of the control
		__mydoPostBack(this.id, '');
	}
};


FMControlsLib.callFailed = function callFailed(result) 
{
    if (typeof console != "undefined") 
    {
        console.log('WebMethod call failed.');
        if (typeof (result.responseText) === 'string') {
            console.log(result.responseText);
        }
        
    }
};

FMControlsLib.InitializeFMAutoComplete = function (securityToken, controlId, fieldKey, callBack, dependField, lineitem)
{
    if (typeof console != "undefined")
    {
        console.log('Autocomplete Init: ' + controlId);
    }

    // Globally replace all "." with escape sequence "\\."
    var controlSelector = "#" + controlId.replace(/\./g, '\\.');

    $(controlSelector).autocomplete({
        source: function(request, response) {
        	var dataPacket;
        	var dependentFieldValue = "!N";

        	if (dependField != null)
        	{
        		var control = document.getElementById(dependField);

        		if (control != null)
        		{
        			if (control.value !=null)
        			{
        				dependentFieldValue = control.value;
        			}
				}
	        }

            if (typeof lineitem != "undefined")
            {
                dataPacket = {
                    token: securityToken,
                    maxRows: 12,
                    startsWith: request.term,
                    fieldKey: fieldKey,
                    dependFieldVal: dependentFieldValue,
                    lineItemID: lineitem
                };
            }
            else
            {
                dataPacket = {
                    token: securityToken,
                    maxRows: 12,
                    startsWith: request.term,
                    fieldKey: fieldKey,
                    dependFieldVal: dependentFieldValue
                };
            }

            $.ajax({
                type: "post",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: callBack,
                data: JSON.stringify(dataPacket),
                success: function(data) {
                    response($.map(data.d, function(item) {
                        return {
                            label: item,
                            value: item
                        };
                    }));
                },
                error: FMControlsLib.callFailed
            });
        },
        minLength: 1,
        delay: 15,
        change: FMControlsLib.autoSelectionChanged,
        select: FMControlsLib.autoSelectionSelected
    });
};
