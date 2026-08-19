function Ok() {
    let subject = document.querySelector("#SubjectTextBox");
	let body = document.querySelector("#BodyTextBox");
	//  debugger;
	$.ajax({
		cache: false,
		type: "POST",
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		url: "CustomizeEmailMessageForm.aspx/Save",
		data: JSON.stringify({subject: subject.value, body: body.value}),
		success: function (response, xhr, settings) {
						if (response == "Exception" || settings.getResponseHeader('content-type').indexOf('text/html') >= 0) {
							console.log(response);
						}
						var result = new Array();
						window.returnValue = result;
						setWindowReturnValue(result);
						CloseDlg();
					},
		error: function (xhr, status, error) {
						var exception = JSON.parse(xhr.responseText);
						alert(exception.Message);
						/*window.top.location = "../";*/
					}
			});


}
function Cancel() {
    var result = new Array();
	window.returnValue = result;
	setWindowReturnValue(result);
	CloseDlg();
}
function CloseDlg() {
    window.close();
	closeDialogWindow();
}
