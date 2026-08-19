function checkDuplicateSessions() {
	var bc = new BroadcastChannel('check_duplicate_session');
	bc.onmessage = function (ev) {
		try {
			//alert("Multiple application sessions detected. Logging out.");
			top.location = "../FMWebApp/LogoutForm.aspx";
		}
		catch (e) {
			self.location = "../FMWebApp/LogoutForm.aspx";
		}
	} /* receive */
	bc.postMessage('Check Sessions'); /* send */
}
window.addEventListener("load", function (e) {
	checkDuplicateSessions();
});