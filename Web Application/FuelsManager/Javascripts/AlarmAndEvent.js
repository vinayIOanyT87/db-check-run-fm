

function CustomizeEmailMessage(alarmAndEvent) {
	//var companyTextBox = document.getElementById(companyTextBoxId);
	let x = alarmAndEvent.parentElement.querySelector("div");
	let guid = '';
	if (x) {
		guid = x.textContent;
	}
	//debugger;
	showModalDialogFrame({
		url: '../FMWebApp/CustomizeEmailMessageForm.aspx?guid='+guid,
		width: 700,
		height: 500,
		title: "Customize E-mail Message",
		onClose: function () {
			if (this.returnValue != null) {
				;
			}
		}
	}, 200);
}
