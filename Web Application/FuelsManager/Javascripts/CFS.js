//Contains functions to check for Cross-Frame Scripting vulnerability.
//Cross-Frame Scripting exists if top window and current window differ in their location domain or root application path.
function checkForCrossFrameScripting() {
	if (window.cfsChecked) {
		return;
	}
	var cfsDetected = true;
	//The following STYLE and script are for preventing Cross-Frame Scripting attack on browsers not supporting x-frame-options:sameorigin header
	var selfLocation = self.location.host.toUpperCase();
	try {
		let topLocation = top.location.host.toUpperCase();
		if (topLocation === selfLocation) {
			var p1 = top.location.pathname.split("/", 5);
			var p2 = self.location.pathname.split("/", 5);
			if (p1.length > 1 && p2.length > 1 && p1[1] === p2[1]) {
				window.cfsChecked = true;
				cfsDetected = false;
			}
		}
	}
	catch (e) { ; }

	if (cfsDetected) {
		try {
			top.location = "../FMWebApp/LogoutForm.aspx";
		}
		catch (e) {
			alert("Cross Frame Scripting detected. Please enter correct URL into browser address bar.");
			self.location = "../FMWebApp/LogoutForm.aspx";
		}
	}
	else {
		document.body.style.display = 'block';
	}
}
window.addEventListener("load", function (e) {
	checkForCrossFrameScripting();
});



