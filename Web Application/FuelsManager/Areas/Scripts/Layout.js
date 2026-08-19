var FMLayout = {};

FMLayout.dateFormat = 'mm/dd/yyyy';
window['FMLayout.dateFormat'] = FMLayout.dateFormat;
FMLayout.calendarLocation = '';
window['FMLayout.calendarLocation'] = FMLayout.calendarLocation;

FMLayout.InitializeLayout = function () {

	// Notification default styling
	PNotify.prototype.options.styling = 'bootstrap3';

    // This function disables all submit buttons when a submit button is pressed
    //  to prevent double postback.
    $('input[type=submit], input[type=button]').click(function (event) {
        $(this).closest('form').find('input[type=submit], input[type=button]').prop('disabled', 'disabled');
        FMLayout.ShowPleaseWait();
    });

    // Set autofocus to first control with autofocus attribute so long as focus not set already
    // Also, select all text in the control.
    $.each($('[autofocus]:not(:focus)').eq(0), function() {
        this.focus();
        this.select();
    });

    FMLayout.FMGridResizeToHeight();
    FMLayout.InitWatermark();
    FMLayout.InitDatePickerControls();
};

FMLayout.ShowPleaseWait = function() {
    // Display the "Please Wait" message centered on the screen (delayed display).
	$("#cshtmlPleaseWaitDiv").each(function () {
        var pleaseWait = $(this);
        setTimeout(function () {
			var waitImage = parent.document.getElementById("PleaseWaitDiv");

			if (waitImage != null && waitImage.style.display == "inline") {
				return;
			}
            pleaseWait.css('visibility', 'visible');
            pleaseWait.css('left', ($(window).width() / 2) - 100 + 'px');
            pleaseWait.css('top', ($(window).height() / 3) + 'px');
            pleaseWait.css('z-index', '200');
        }, 1000);
    });
};

FMLayout.FMGridResizeToHeight = function () {
    if ($('.FMGridContainer').length) {
        var top = $('.FMGridContainer').position().top;
        //console.log(top);
        $('.FMGridContainer').css('max-height', $(window).height() - top - 110 + 'px');

        $(window).on('resize', function() {
            $('.FMGridContainer').css('max-height', $(this).height() - top - 110 + 'px');
        });
    }
};

FMLayout.InitWatermark = function () {
    $(".watermark").focus(function () {
        var $tb = $(this);
        if ($tb.val() == this.title) {
            $tb.val("");
            $tb.removeClass("watermark");
        }
    });

    $(".watermark").blur(function () {
        var $tb = $(this);
        if ($.trim($tb.val()) == "") {
            $tb.val(this.title);
            $tb.addClass("watermark");
        }
    });

    $(".watermark").each(function () {
        $(this).blur();
        var $tb = $(this);
        if ($tb.val() != this.title) {
            $tb.removeClass("watermark");
        }
    });
};

FMLayout.InitDatePickerControls = function() {
    $(".datepicker").each(function() {
        $(this).datepicker({
            buttonImage: FMLayout.calendarLocation + '/calendar.gif',
            buttonImageOnly: true,
            showOn: "button",
            dateFormat: FMLayout.dateFormat
        });
    });
};

/* MODAL DIALOG REPLACEMENTS */
FMLayout.Alert = function (output_msg, title_msg, okAction) {
	if (!title_msg) {
		title_msg = 'Alert';
	}

	var dialogBox = $('<div></div>');

	if (!output_msg) {
		dialogBox.html('No Message to Display.');
	}
	else if (typeof output_msg === 'string' || output_msg instanceof String) {
		dialogBox.html(output_msg);
	}
	else if (output_msg instanceof jQuery) {
		dialogBox.html(output_msg.html());
	}

	if (!okAction) {
		okAction = function () {
		};
	}

	dialogBox.html(output_msg).dialog({
		title: title_msg,
		resizable: false,
		modal: true,
		dialogClass: 'dialog-title-icon-info',
		close: function () //moved okAction in here because below it will not happen if the access key is used to Ok the dialog
		{
			okAction();
		},
		buttons: {
			"Ok": function () {
				//okAction();
				$(this).dialog('close');
			}
		},
		create: function () {
			// add the keyboard shortcut since jquery ui dialog does not support it out of the box
			$(this).parent().find('button').each(function () {
				if ($(this).text() === 'Ok') {
					$(this).html('<u>O</u>k');
					$(this).attr('accesskey', 'o');
				}
			});
		}
	});
};
FMLayout.Confirm = function (output_msg, title_msg, okAction, cancelAction) {
	if (!title_msg) {
		title_msg = 'Confirm';
	}

	var dialogBox = $('<div></div>');

	if (!output_msg) {
		dialogBox.html('No Message to Display.');
	}
	else if (typeof output_msg === 'string' || output_msg instanceof String) {
		dialogBox.html(output_msg);
	}
	else if (output_msg instanceof jQuery) {
		dialogBox.html(output_msg.html());
	}

	if (!okAction) {
		okAction = function () {
		};
	}
	if (!cancelAction) {
		cancelAction = function () {
		};
	}

	dialogBox.html(output_msg).dialog({
		title: title_msg,
		resizable: false,
		modal: true,
		dialogClass: 'no-close dialog-title-icon-info',
		create: function () {
			// add the keyboard shortcut since jquery ui dialog does not support it out of the box
			$(this).parent().find('button').each(function () {
				if ($(this).text() === 'Ok') {
					$(this).html('<u>O</u>k');
					$(this).attr('accesskey', 'o');
				}
				if ($(this).text() === 'Cancel') {
					$(this).html('<u>C</u>ancel');
					$(this).attr('accesskey', 'c');
				}
			});
		},
		buttons: {
			"Ok": function () {
				okAction();
				$(this).dialog('close');
			},
			"Cancel": function () {
				cancelAction();
				$(this).dialog('close');
			}
		}
	});
};
FMLayout.ConfirmYesNo = function (output_msg, title_msg, yesAction, noAction) {
	if (!title_msg) {
		title_msg = 'Confirm';
	}

	var dialogBox = $('<div></div>');

	if (!output_msg) {
		dialogBox.html('No Message to Display.');
	}
	else if (typeof output_msg === 'string' || output_msg instanceof String) {
		dialogBox.html(output_msg);
	}
	else if (output_msg instanceof jQuery) {
		dialogBox.html(output_msg.html());
	}

	if (!yesAction) {
		yesAction = function () {
		};
	}
	if (!noAction) {
		noAction = function () {
		};
	}

	dialogBox.html(output_msg).dialog({
		title: title_msg,
		resizable: false,
		modal: true,
		dialogClass: 'no-close dialog-title-icon-info',
		create: function () {
			// add the keyboard shortcut since jquery ui dialog does not support it out of the box
			$(this).parent().find('button').each(function () {
				if ($(this).text() === 'Yes') {
					$(this).html('<u>Y</u>es');
					$(this).attr('accesskey', 'y');
				}
				if ($(this).text() === 'No') {
					$(this).html('<u>N</u>o');
					$(this).attr('accesskey', 'n');
				}
			});
		},
		buttons: {
			"Yes": function () {
				yesAction();
				$(this).dialog('close');
			},
			"No": function () {
				noAction();
				$(this).dialog('close');
			}
		}
	});
};
FMLayout.ConfirmSaveCancel = function (output_msg, title_msg, SaveAction, CancelAction) {
	if (!title_msg) {
		title_msg = 'Save Changes';
	}

	var dialogBox = $('<div></div>');

	if (!output_msg) {
		dialogBox.html('No Message to Display.');
	}
	else if (typeof output_msg === 'string' || output_msg instanceof String) {
		dialogBox.html(output_msg);
	}
	else if (output_msg instanceof jQuery) {
		dialogBox.html(output_msg.html());
	}

	if (!SaveAction) {
		SaveAction = function () {
		};
	}
	if (!CancelAction) {
		CancelAction = function () {
		};
	}

	dialogBox.html(output_msg).dialog({
		title: title_msg,
		resizable: false,
		modal: true,
		dialogClass: 'no-close',
		create: function () {
			// add the keyboard shortcut since jquery ui dialog does not support it out of the box
			$(this).parent().find('button').each(function () {
				if ($(this).text() === 'Save') {
					$(this).html('<u>S</u>ave');
					$(this).attr('accesskey', 's');
				}
				if ($(this).text() === 'Cancel') {
					$(this).html('<u>C</u>ancel');
					$(this).attr('accesskey', 'c');
				}
			});
		},
		buttons: {
			"Save": function () {
				if (SaveAction() !== false)  // return false will not close the modal form
				{
					$(this).dialog('close');
				}
			},
			"Cancel": function () {
				CancelAction();
				$(this).dialog('close');
			}
		}
	});
};
