

function CompanySelect(role, CompanyTextBoxID) {
    var sFeatures = "dialogWidth: 855px; dialogHeight: 560px";
    var CompanyTextBox = document.getElementById(CompanyTextBoxID);
    var CompanyNameTextBox = document.getElementById("CompanyName" + CompanyTextBoxID);

    var result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=SUPPLIER",
		        "", sFeatures);

    if (result != null) {
        CompanyTextBox.value = result[0];
        CompanyTextBox.title = result[1];
        CompanyNameTextBox.value = result[2];
    }
}