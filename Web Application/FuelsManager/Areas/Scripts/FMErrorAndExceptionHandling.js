// Polyfill to make ie support startsWidth
if (!String.prototype.startsWith) {
	String.prototype.startsWith = function (searchString, position) {
		position = position || 0;
		return this.indexOf(searchString, position) === position;
	};
}



var FMErrorAndExceptionHandling = {};

// define constants for the location of the messages
FMErrorAndExceptionHandling.stack_topleft = { "dir1": "down", "dir2": "right", "push": "top" };
FMErrorAndExceptionHandling.stack_bottomleft = { "dir1": "up", "dir2": "right", "push": "top" };
FMErrorAndExceptionHandling.stack_topright = { "dir1": "down", "dir2": "left", "push": "top" };
FMErrorAndExceptionHandling.stack_bottomright = { "dir1": "up", "dir2": "left", "push": "top", "firstpos1": 25, "firstpos2": 25 };
FMErrorAndExceptionHandling.stack_bar_top = { "dir1": "down", "dir2": "right", "push": "top", "spacing1": 0, "spacing2": 0};
FMErrorAndExceptionHandling.stack_bar_bottom = { "dir1": "up", "dir2": "right", "spacing1": 0, "spacing2": 0 };
FMErrorAndExceptionHandling.OnlyOneNotification = false;


FMErrorAndExceptionHandling.MakeNotificationDraggable = function (jQueryControl)
{
// make the message draggable. Note that jquery UiI draggable works only when you specify the top/left coordinates so notifications
	// that are displayed in the bottom right corner have to have the CSS converted to use those coordinates
	$(jQueryControl.elem).draggable({
		create: function ()
		{
			$(this).css({
				bottom: "auto !important"
			});
		},
		start: function( event, ui )
		{
			$(this).css({
				top: $(this).position().top,
				height: $(this).height()
			});
		}
	});
}

FMErrorAndExceptionHandling.FlagControlHavingError = function (key) {
    $("input[name='" + key + "']").parent().addClass("has-error");
}

FMErrorAndExceptionHandling.ClearControlErrors = function () {
	$(".has-error").removeClass("has-error");
}

FMErrorAndExceptionHandling.CloseNotifications = function () {
	PNotify.removeAll();
}

FMErrorAndExceptionHandling.ShowException = function(xhr, status, err, userDataHandler, messageAttributes)
{
	if ( FMErrorAndExceptionHandling.OnlyOneNotification )
	{
		FMErrorAndExceptionHandling.CloseNotifications();
	}

	// check if we are passing attributes about how to display the message
	if (messageAttributes === undefined || messageAttributes === null) messageAttributes = {};

	// cleanup the message before displaying it
	var message = xhr.responseText ? xhr.responseText : "";
	message.replace("<title>", "").replace("</title>", "");

	if (message.startsWith('<script type="text/javascript">') && message.indexOf('alert(') > 0) {
		message = message.replace('<script type="text/javascript">', '')
									.replace('</script>', '')
									.replace('<!--', '')
									.replace('-->', '')
									.replace('alert("', '')
									.replace('");', '')
									.replace(/(\r\n|\n|\r)/gm, ""); //also eliminate the line breaks
	}

	var addclass = messageAttributes.hasOwnProperty('addclass') ? messageAttributes.addclass : "stack-bottomright";
	addclass = addclass.replace('ui-pnotify-translucent', ''); // remove the translucent class since we are going to add it in 5 seconds

	// remove all error messages
	var errorMessage = new PNotify({
		title: 'Exception',
		text: message,
		cornerclass: 'ui-pnotify-sharp',
		type: 'error',
		icon: false,
		animation: "none",
		width: messageAttributes.hasOwnProperty('width') ? messageAttributes.width : "350px",
		addclass: addclass,
		stack: messageAttributes.hasOwnProperty('stack') ? messageAttributes.stack : FMErrorAndExceptionHandling.stack_bottomright,
		buttons: {
			sticker: false
		},
		hide: false,
		after_open: function (notice) {
			setTimeout(function () { notice.container.parent().addClass('ui-pnotify-translucent') }, 5000);
		}
	});
	if (userDataHandler) userDataHandler();
	
	// make the message draggable.
	FMErrorAndExceptionHandling.MakeNotificationDraggable(errorMessage);

}

FMErrorAndExceptionHandling.ShowNotification = function (textMessage, userDataHandler, messageAttributes)
{
	if (FMErrorAndExceptionHandling.OnlyOneNotification) {
		FMErrorAndExceptionHandling.CloseNotifications();
	}

	// check if we are passing attributes about how to display the message
	if (messageAttributes === undefined || messageAttributes === null) messageAttributes = {};
	var addclass = messageAttributes.hasOwnProperty('addclass') ? messageAttributes.addclass : "stack-bottomright";
	addclass = addclass.replace('ui-pnotify-translucent', ''); // remove the translucent class since we are going to add it in 5 seconds

	var notificationMessage = new PNotify({
		title: 'Notification',
		text: textMessage,
		cornerclass: 'ui-pnotify-sharp',
		type: 'info',
		icon: false,
		animation: "none",
		width: messageAttributes.hasOwnProperty('width') ? messageAttributes.width : "300px",
		addclass: addclass,
		stack: messageAttributes.hasOwnProperty('stack') ? messageAttributes.stack : FMErrorAndExceptionHandling.stack_bottomright,
		buttons: {
			sticker: false
		},
		hide: false,
		after_open: function (notice) {
			setTimeout(function () { notice.container.parent().addClass('ui-pnotify-translucent') }, 5000);
		}
	});
	if (userDataHandler) userDataHandler( notificationMessage );

	// make the message draggable.
	FMErrorAndExceptionHandling.MakeNotificationDraggable(notificationMessage);

}

FMErrorAndExceptionHandling.ShowError = function (textMessage, userDataHandler, messageAttributes)
{
	if (FMErrorAndExceptionHandling.OnlyOneNotification) {
		FMErrorAndExceptionHandling.CloseNotifications();
	}

	// check if we are passing attributes about how to display the message
	if (messageAttributes === undefined || messageAttributes === null) messageAttributes = {};

	var addclass = messageAttributes.hasOwnProperty('addclass') ? messageAttributes.addclass : "stack-bottomright";
	addclass = addclass.replace('ui-pnotify-translucent', ''); // remove the translucent class since we are going to add it in 5 seconds
	var notificationMessage = new PNotify({
		title: 'Error',
		text: textMessage,
		cornerclass: 'ui-pnotify-sharp',
		type: 'error',
		icon: false,
		animation: "none",
		width: messageAttributes.hasOwnProperty('width') ? messageAttributes.width : "350px",
		addclass: addclass,
		stack: messageAttributes.hasOwnProperty('stack') ? messageAttributes.stack : FMErrorAndExceptionHandling.stack_bottomright,
		buttons: {
			sticker: false
		},
		hide: false,
		after_open: function (notice) {
			setTimeout(function() { notice.container.parent().addClass('ui-pnotify-translucent') }, 5000);  // make it translucent in 5 seconds
		}
	});
	if (userDataHandler) userDataHandler();

	// make the message draggable.
	FMErrorAndExceptionHandling.MakeNotificationDraggable(notificationMessage);

}


FMErrorAndExceptionHandling.ShowSuccess = function (textMessage, userDataHandler, messageAttributes)
{
	if (FMErrorAndExceptionHandling.OnlyOneNotification) {
		FMErrorAndExceptionHandling.CloseNotifications();
	}

	var addclass = messageAttributes.hasOwnProperty('addclass') ? messageAttributes.addclass : "stack-bottomright";
	addclass = addclass.replace('ui-pnotify-translucent', ''); // remove the translucent class since we are going to add it in 5 seconds

// display the success message
	var successMessage = new PNotify({
		title: 'Success',
		text: textMessage,
		cornerclass: 'ui-pnotify-sharp',
		type: 'success',
		icon: false,
		animation: "none",
		buttons: {
			sticker: false
		},
		width: messageAttributes.hasOwnProperty('width') ? messageAttributes.width : "300px",
		addclass: addclass,
		stack: messageAttributes.hasOwnProperty('stack') ? messageAttributes.stack : FMErrorAndExceptionHandling.stack_bottomright,
		after_open: function (notice) {
			setTimeout(function () { notice.container.parent().addClass('ui-pnotify-translucent') }, 5000);  // make it translucent in 5 seconds
		}
	});
	if (userDataHandler) userDataHandler();

	// make the message draggable.
	FMErrorAndExceptionHandling.MakeNotificationDraggable(successMessage);

}

FMErrorAndExceptionHandling.HandleMessages = function (response, userDataHandler, messageAttributes)
{
    if ( typeof response === 'undefined' || response === null )
    {
        return;
    }

	var inError = false;  // NOTE, if there are no error or successful messages we assume it's succesful (we do not display any messages).

	//some exceptions are returned as a html string to display an alert, check if we have one of them
	if (typeof response === 'string') {
	    inError = true;
		// display the error message
		var errorMessage = response;
		// if we are getting one of those alert extract just the message from an exception to display
		if (response.startsWith('<script type="text/javascript">') && response.indexOf('alert(') > 0) {
			errorMessage = response.replace('<script type="text/javascript">', '')
										.replace('</script>', '')
										.replace('<!--', '')
										.replace('-->', '')
										.replace('alert("', '')
										.replace('");', '')
										.replace(/(\r\n|\n|\r)/gm, ""); //also eliminate the line breaks
		}
		FMErrorAndExceptionHandling.ShowError(errorMessage, null, messageAttributes);
		if (userDataHandler) userDataHandler(null, inError);
		return;
	}

	// make sure we are passing an object for processing or we have to ignore it and report as an error
	if (typeof response === 'object' && !response.hasOwnProperty('ErrorMessage')) {
		// display the error message
		FMErrorAndExceptionHandling.ShowError("Invalid response format from the server.", null, messageAttributes);
		if (userDataHandler) userDataHandler(null, inError);

		return;
	}

    // check if we are passing attributes about how to display the message
	if (messageAttributes === undefined || messageAttributes === null) messageAttributes = {};

	// remove the red border around the controls with errors
	FMErrorAndExceptionHandling.ClearControlErrors();

	var count = 0;
	// count the number of error messages
    if (response && response.ErrorMessage) {
        $.each(response.ErrorMessage, function (key, message) { count = count + 1; });
    }

    // we are getting a JSON object with the results of the Save 
    if (count > 0) { // if save had errors
	    inError = true;
	    var messageList = [];
        // add messages but avoid duplicates
        var seen = {};

        $.each(response.ErrorMessage, function (key, message) {
            // set the control to be flagged as having an error
           
            $.each(message, function (i, val) {
                if (!seen[message[i]]) {
                    seen[message[i]] = true;
                    FMErrorAndExceptionHandling.FlagControlHavingError(key);
                    messageList.push("<div>" + message[i] + "</div>");
                }
            });
        });
        var messages = messageList.join('');
    	  // display the error message
        FMErrorAndExceptionHandling.ShowError(messages, null, messageAttributes);
    }

	// count the number of success errors
    count = 0;
    if (response && response.SuccessMessage) {
        $.each(response.SuccessMessage, function (key, message) { count = count + 1; });
    }
 
    // we are getting a JSON object with the results of the Save 
    if (count > 0) { // if save was succesful
        var messageList = [];
        // add messages but avoid duplicates
        var seen = {};
        $.each(response.SuccessMessage, function (key, message) {
            $.each(message, function (i, val) {
                if (!seen[message[i]]) {
                    seen[message[i]] = true;
                    messageList.push("<div>" + message[i] + "</div>");
                }
            });
        });
        var messages = messageList.join('');
    	// display the success message
        FMErrorAndExceptionHandling.ShowSuccess(messages, null, messageAttributes);
    }
    
    if (userDataHandler) userDataHandler(response.Data, inError);

}
