    function deleteItem (){

	    var url = this.href;
	    var token = $('#submitForm input[name=__RequestVerificationToken]').val();
	    var pointGuid = $('#submitForm input[name=PointGuid]').val();
	    var headers = {};
	    headers['__RequestVerificationToken'] = token;

	    //var messageAttributes = { addclass: 'stack-bottomright', stack: FCEEMappingsEditor.stack_bottomright_vcfsettings, width: '150px' };
	    // remove any notification
	    $.ajax({
		    url: url,
		    type: 'post',
		    headers: headers,

		    success: function (result) {
           
          		FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) {

                        let newurl = window.location.href;

						if (!inError){
							if (data){
								if (confirm("This item has FCEE Mappings associated with it.\nDo you wish to continue deleting this item and associated mappings?")){
									newurl = url.replace("/Delete/", "/DeleteConfirmed/");
								}
							}
							else if (confirm('Are you sure you wish to delete this item?')){
								newurl = url.replace("/Delete/", "/DeleteConfirmed/");
							}
						}

                        window.location=newurl;                         
					});	//,messageAttributes);  

		    },
		    error:
			    function (request, status, error) {
				    FMErrorAndExceptionHandling.ShowException(request, status, error, null);//, messageAttributes);
			    }
        });
		return false;
         
	}
 

    $(document).ready(function () {
	    let deleteButtons = $('a.deleteLinkClass');
	    if (deleteButtons){
            deleteButtons.attr("onclick","");
		    deleteButtons.off("click");
		    deleteButtons.on("click",deleteItem);
	    }
    });