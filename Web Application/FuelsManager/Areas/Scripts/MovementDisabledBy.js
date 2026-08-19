//=======================================================================
// RUN after page has been loaded but before render
//=======================================================================
$(document).ready(function () {

	// Hide the Header, as MovementDisabledBy provides one
	$('.modal-header').hide();

	FMErrorAndExceptionHandling.CloseNotifications();
});
