// create a class with helper functions for the strap table editor
var FMStrapTableEditor = function()
{
    var _valuesChanged = false;
    var _processingEndEditRow = false;
    var _deletingStrapEntry = false;
	 var _addingStrapEntry = false;
	 var _inError = false;
    var _stack_bottomright_straptable = { "dir1": 'up', "dir2": 'left', "firstpos1": 135, "firstpos2": 25, "context": $( '#ModulePropertyEditorPropertyScreen' ) };

    // pad number with leading zeros
    var _pad = function( str, max )
    {
        str = str.toString();
        return str.length < max ? _pad( '0' + str, max ) : str;
    };

    var _convertFeetInch8thReadings = function( reading )
    {
        var convertedReading = '';
        reading = reading.trim();
        var negative = false;
        if ( reading.substring( 0, 1 ) === '-' )
        {
            negative = true;
            reading = reading.substring( 1 );
        }


        var rawValues = reading.split( '-' );

        // if we have only 1 number assume that it's only feet
        if ( rawValues.length === 1 )
        {
            convertedReading = ( isNaN( parseInt( rawValues[0] ) ) ? '0' : parseInt( rawValues[0] ) ) + '-00' + '-0';
        } // if we have only 2 number assume that its feet and inches
        else if ( rawValues.length === 2 )
        {
            convertedReading = ( isNaN( parseInt( rawValues[0] ) ) ? '0' : parseInt( rawValues[0] ) ) + '-' + ( isNaN( parseInt( rawValues[1] ) ) ? '0' : parseInt( rawValues[1] ) ) + '-00';
        }
        else
        {
            convertedReading = reading;
        }


        // convert the string 00-00-00 into an array of values
        var values = convertedReading.split( '-' );
        if ( values.length === 3 )
        {
            // we need to apply a conversion, for example 0-0-10 should be 0-1-2 since 10 1/8 is the equivalent to 1 foot and 2 1/8
            var eighth = parseInt( values[2] );
            if ( isNaN( eighth ) )
            {
                eighth = 0;
            }
            var moveForward = 0;
            if ( eighth >= 8 )
            {
                moveForward = parseInt( eighth / 8 );
                eighth = eighth % 8;
            }

            var inches = parseInt( values[1] );
            if ( isNaN( inches ) )
            {
                inches = 0;
            }
            inches += moveForward;
            moveForward = 0;
            if ( inches >= 12 )
            {
                moveForward = parseInt( inches / 12 );
                inches = inches % 12;
            }

            var feet = parseInt( values[0] );
            if ( isNaN( feet ) )
            {
                feet = 0;
            }

            feet += moveForward;

            return ( ( negative ) ? '-' : '' ) + _pad( feet, 2 ) + '-' + _pad( inches, 2 ) + '-' + _pad( eighth, 1 );
        }
        // if we cannot process the reading return it as is
        return reading;
    };


    var _convertFeetInch16thReadings = function( reading )
    {
        var convertedReading;
        reading = reading.trim();
        var negative = false;
        if ( reading.substring( 0, 1 ) === '-' )
        {
            negative = true;
            reading = reading.substring( 1 );
        }

        var rawValues = reading.split( '-' );

        // if we have only 1 number assume that it's only feet
        if ( rawValues.length === 1 )
        {
            convertedReading = ( isNaN( parseInt( rawValues[0] ) ) ? '0' : parseInt( rawValues[0] ) ) + '-00' + '-00';
        } // if we have only 2 number assume that its feet and inches
        else if ( rawValues.length === 2 )
        {
            convertedReading = ( isNaN( parseInt( rawValues[0] ) ) ? '0' : parseInt( rawValues[0] ) ) + '-' + ( isNaN( parseInt( rawValues[1] ) ) ? '00' : parseInt( rawValues[1] ) ) + '-00';
        }
        else
        {
            convertedReading = reading;
        }


        // convert the string 00-00-00 into an array of values
        var values = convertedReading.split( '-' );
        if ( values.length === 3 )
        {
            var sixteenth = parseInt( values[2] );
            if ( isNaN( sixteenth ) )
            {
                sixteenth = 0;
            }
            var moveForward = 0;
            if ( sixteenth >= 16 )
            {
                moveForward = parseInt( sixteenth / 16 );
                sixteenth = sixteenth % 16;
            }

            var inches = parseInt( values[1] );
            if ( isNaN( inches ) )
            {
                inches = 0;
            }
            inches += moveForward;
            moveForward = 0;
            if ( inches >= 12 )
            {
                moveForward = parseInt( inches / 12 );
                inches = inches % 12;
            }

            var feet = parseInt( values[0] );
            if ( isNaN( feet ) )
            {
                feet = 0;
            }
            feet += moveForward;

            return ( ( negative ) ? '-' : '' ) + _pad( feet, 2 ) + '-' + _pad( inches, 2 ) + '-' + _pad( sixteenth, 2 );
        }
        // if we cannot process the reading return it as is
        return reading;
    };


    var _saveChanges = function()
    {
        var url = $( '#urlSaveStrapTable' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        // notification position
        var messageAttributes = { addclass: 'stack-bottomright', stack: FMStrapTableEditor.stack_bottomright_straptable, width: '450px' };
        // remove previous notifications
        PNotify.removeStack( FMStrapTableEditor.stack_bottomright_straptable );

        $.ajax( {
            url: url,
            type: 'post',
            data: ( function()
            {
                // serialize the form
                var serializedData = $( '#strapTableForm' ).serialize();

                // i want to serialize the table so MVC binds it automatically to the strap object
                var strapTables = [];

                var tabs = $( '#tabList li.tab-table' ).each( function()
                {
                    var tabContents = $( this ).find( 'a' ).attr( 'data-target' );
                    var strapTableEntries = [];
                    strapTables.push( strapTableEntries );
                    $( tabContents + ' .table tr' ).each( function()
                    {
                        if ( $( this ).find( 'td.strapTableLevelColumn' ).length > 0 )
                        {
                            strapTableEntries.push( {
                                LevelEntry: $( this ).find( 'td.strapTableLevelColumn' ).text().trim(),
                                VolumeEntry: $( this ).find( 'td.strapTableVolumeColumn' ).text().trim()
                            } );
                        }
                    } );
                } );
                serializedData = 'editorEntries=' + JSON.stringify( strapTables ) + '&' + serializedData;
                return serializedData;
            } )(),
            success: function( result )
            {
                FMErrorAndExceptionHandling.HandleMessages( result,
                    function( data, inError )
                    {
                        if ( !inError )
                        {
                            FMStrapTableEditor.valuesChanged = false;
                        }
                    },
                    messageAttributes );
            },
            error:
                function( request, status, error )
                {
                    FMErrorAndExceptionHandling.ShowException( request, status, error, null, messageAttributes );
                }
        } );
    };
    var _validateRegionalSettings = function( action, method )
    {
        // notification position
        var messageAttributes = { addclass: 'stack-bottomright', stack: FMStrapTableEditor.stack_bottomright_straptable, width: '450px' };
        // remove previous notifications
        PNotify.removeStack( FMStrapTableEditor.stack_bottomright_straptable );

        var url = $( '#urlValidateRegionalSettings' ).val();
        var token = $( '#strapTableForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        $.ajax( {
            url: url,
            type: 'POST',
            headers: headers,
            success: function( result )
            {
                FMErrorAndExceptionHandling.HandleMessages( result,
                    function( data, inError )
                    {
                        if ( !inError )
                        {
                            $( '#StrapTableExportName' ).closest( '.form-group' ).removeClass( 'has-error' );

                            var tabContents = $( '#tabList li.active' ).find( 'a' ).attr( 'data-target' );
                            var tableIndex = tabContents.substring( tabContents.indexOf( '_' ) + 1 );
                            var href = $( '#exportLink' ).attr( 'href' );
                            if ( href.indexOf( '?' ) != -1 )
                            {
                                href = href.substring( 0, href.indexOf( '?' ) );
                            }

                            FMStrapTableEditor.saveChanges();

                        	$( '#exportLink' ).attr( 'href', href + '?pointGuid=' + $( '#PointGuid' ).val() + '&pointPropertyGuid=' + $( '#PointPropertyGuid' ).val() + '&exportTableIndex=' + tableIndex + '&isTemplatePoint=' + $( '#IsTemplatePoint' ).val());
                            $( '#exportLink' )[0].click(); // click on a hidden link to trigger the export (we cannot use AJAX for security reasons)
                        }
                    },
                    messageAttributes );
            },
            error:
                function( request, status, error )
                {
                    FMErrorAndExceptionHandling.ShowException( request, status, error, null, messageAttributes );
                }

        } );
    }; // change controls to be able to rename a tab
    var _renameTab = function( control )
    {
        var firstChildren = $( control ).children().first();

        // the <a>nchor in the tab control either contains a label (to display the name) or an input box (to change the name)
        if ( firstChildren.is( 'label' ) )
        {
            var oldName = firstChildren.text();

            // replace the label with an input, and concantenante with the hidden input
            var innerHtml = $( control ).html();
            innerHtml = innerHtml.substring( innerHtml.indexOf( '<input' ) );
            $( control ).html( '<input id=\'' + $( control ).attr( 'data-target' ).replace( '#', '' ) + '\' type=\'text\' class=\'tab-rename\' value=\'' + oldName + '\' onblur=\'FMStrapTableEditor.endRenameTab( this );\' onkeyup=\'if(event.keyCode == 13){ FMStrapTableEditor.endRenameTab( this );}\'> </input> ' + innerHtml + '</input>' );
            $( control ).children().first().focus().val( oldName );

            // terminate the edit if user presses the Esc key
            $( '#' + $( control ).attr( 'data-target' ).replace( '#', '' ) ).on( 'keyup', function( e )
            {
                if ( e.keyCode === 27 )
                { // escape key maps to keycode `27`
                    FMStrapTableEditor.endRenameTab( this );
                    return false;
                }
                return true;
            } );
        }
    }; // Finish renaming tab. When we lose focus on the tab we want to finish the renaming.
    var _endRenameTab = function( control )
    {
        var name = $( control ).val();
        // replace the input with the label, and update the hidden input
        var innerHtml = $( control ).parent().html();
        innerHtml = innerHtml.substring( innerHtml.indexOf( '>' ) + 1 );
        var oldName = innerHtml.substring( innerHtml.indexOf( 'value=' ) );
        oldName = oldName.substring( 7 );
        oldName = oldName.substring( 0, oldName.indexOf( '"' ) );
        innerHtml = innerHtml.substring( 0, innerHtml.indexOf( 'value' ) );
        $( control ).parent().html( '<label class="tab-name">' + name + '</label>' + innerHtml + 'value=\"' + name + '\" ></input>' );

        $( '#TableForStrapDropDownList option' ).each( function()
        {
            if ( $( this ).text() === oldName )
            {
                $( this ).text( name );
            }
        } );

        $( '#TableForWaterVolumeDropDownList option' ).each( function()
        {
            if ( $( this ).text() === oldName )
            {
                $( this ).text( name );
            }
        } );

        $( '#TableForSolidsVolumeDropDownList option' ).each( function()
        {
            if ( $( this ).text() === oldName )
            {
                $( this ).text( name );
            }
        } );
    };
    var _addTable = function()
    {
		// if we are already in edit or add mode we cannot add a table until we get out of edit
		if ($("#LevelEdit").length > 0) {
			return;
		}

        var url = $( '#urlAddStrapTable' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        // notification position
        var messageAttributes = { addclass: 'stack-bottomright', stack: FMStrapTableEditor.stack_bottomright_straptable, width: '450px' };
        // remove previous notifications
        PNotify.removeStack( FMStrapTableEditor.stack_bottomright_straptable );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: ( function()
            {
                // serialize the form
                var serializedData = $( '#strapTableForm' ).serialize();

                // i want to serialize the table so MVC binds it automatically to the strap object
                var strapTables = [];

                var tabs = $( '#tabList li.tab-table' ).each( function()
                {
                    var tabContents = $( this ).find( 'a' ).attr( 'data-target' );
                    var strapTableEntries = [];
                    strapTables.push( strapTableEntries );
                    $( tabContents + ' .table tr' ).each( function()
                    {
                        if ( $( this ).find( 'td.strapTableLevelColumn' ).length > 0 )
                        {
                            strapTableEntries.push( {
                                LevelEntry: $( this ).find( 'td.strapTableLevelColumn' ).text().trim(),
                                VolumeEntry: $( this ).find( 'td.strapTableVolumeColumn' ).text().trim()
                            } );
                        }
                    } );
                } );
                serializedData = 'editorEntries=' + JSON.stringify( strapTables ) + '&' + serializedData;
                return serializedData;
            } )(),
            success: function( result )
            {
                FMErrorAndExceptionHandling.HandleMessages( result,
                    function( data, inError )
                    {
                        if ( !inError )
                        {
                            $( '#moduleEditorPropertyScreen' ).html( data );
                            FMStrapTableEditor.valuesChanged = true;
                            var tabCount = $( '#tabList li.tab-table' ).length;
                            $( '#SEDeleteTable' ).prop( 'disabled', ( tabCount <= 1 ) ? 'disabled' : '' );
                            _enableAddTab( tabCount );
                        }
                    },
                    messageAttributes );
            },
            error:
                function( request, status, error )
                {
                    FMErrorAndExceptionHandling.ShowException( request, status, error, null, messageAttributes );
                }
        } );
    };
    var _enableAddTab = function( tabCount )
    {
        if ( tabCount >= 6 )
        {
            $( '#tabList li.tab-add-table' ).addClass( 'disabled' );
            $( '#addTable' ).click( function( e )
            {
                e.preventDefault();;
            } );
        }
        else
        {
            $( '#tabList li.tab-add-table' ).removeClass( 'disabled' );
            $( '#addTable' ).click( function()
            {
                FMStrapTableEditor.addTable();
            } );
        }
    };
    var _addRow = function()
    {
    	// if we are already in edit or add mode we cannot add a row
    	if ( $( "#LevelEdit" ).length > 0 )
	    {
		    return;
	    }

        var tabContents = $( '#tabList li.active' ).find( 'a' ).attr( 'data-target' );
        var STEStrapTable = $( tabContents + ' .table' ).DataTable();
        var rowIndex = STEStrapTable.rows().count();

        STEStrapTable.row.add( {
            "0": rowIndex + 1,
            "1": '',
            "2": ''
        } );

        STEStrapTable.columns.adjust().draw();

        var row = STEStrapTable.row( rowIndex ).node();

        FMStrapTableEditor.editRow( STEStrapTable.row( rowIndex ).node(), true );
    };

    var _onblurEditRow = function( input, add )
    {

        if ( $( '#LevelEdit' ).is( ':focus' ) )
        {
            return;
        }
        if ( $( '#VolumeEdit' ).is( ':focus' ) )
        {
            return;
        }
	    // if the user presses the add or delete entry there is no need to go through the endEditRowProcess
			if (FMStrapTableEditor.deletingStrapEntry !== true && FMStrapTableEditor.addingStrapEntry !== true)
			{

			  if ( !FMStrapTableEditor.processingEndEditRow )
			  {
				  FMStrapTableEditor.endEditRow( null, input, add );
			  }
			  else
			  {
				  FMStrapTableEditor.EnableDisableDeleteButton();
			  }
			}

    };
    var _editRow = function( row, add )
    {

        $('#PEMPESavePropertyScreen').prop( 'disabled', true );
        var tabContents = $( '#tabList li.active' ).find( 'a' ).attr( 'data-target' );
        var STEStrapTable = $( tabContents + ' .table' ).DataTable();
        var data = STEStrapTable.row( row ).data();
        var cell = $( '>td', row );
        
        // Empty row, handle as add
        if ( cell.length < 3 )
        {
            FMStrapTableEditor.addRow();
            return;
        }
        // FJM Note: In chrome the blur event seems to call some event hadlers in a different sequence than in IE and it may result on the next field not being in focus
        //           when the onblur event handler fires.  Added a timeout (asynchronous) to let the other event finish before evaluating the event.
        $( cell[1] ).html( '<input id="LevelEdit" type="text" value="' + data[1] + '" onblur="var that = this; setTimeout( function() { FMStrapTableEditor.onblurEditRow(that, ' + add + ');}, 100 );" onkeyup="if(event.keyCode == 13){ FMStrapTableEditor.endEditRow( event, this, ' + add + ');}">' );
        $( cell[2] ).html( '<input id="VolumeEdit" type="text" value="' + data[2] + '" onblur="var that = this; setTimeout( function() { FMStrapTableEditor.onblurEditRow(that, ' + add + ');}, 100 );" onkeyup="if(event.keyCode == 13){ FMStrapTableEditor.endEditRow( event, this, ' + add + ');}" onkeydown="if(event.keyCode == 9){ FMStrapTableEditor.endEditRow( event, this, ' + add + ');}">' );

        // NOTE: IE seems to have a problem identifying the column type for sorting so we need to force the type so it knows how to sort.
        var levelUnit = $( '#LevelUnit' ).val();

        var numFormatInfoString = $('#NumberFormatInfoString').val();
        var numformatInfo = JSON.parse(numFormatInfoString);

        // add the mask to the editor fields
        // if feet-in-16th or feet-in-8th use 00-00-00 as mask, otherwise is just plain numeric
        if ( levelUnit === 'FmlFtIn16Th' )
        {
            $( '#LevelEdit' ).mask( 'S99-99-99', {
                translation: {
                    'S': {
                        pattern: /-/,
                        optional: true
                    },
                    placeholder: '__-__-__'
                }
            } );
        }
        else if ( levelUnit === 'FmlFtIn8Th' )
        {
            $( '#LevelEdit' ).mask( 'S99-99-9', {
                translation: {
                    'S': {
                        pattern: /-/,
                        optional: true
                    },
                    placeholder: '__-__-__'
                }
            } );
        }
        else
        {

            var numLevelDecimals = parseInt( $( '#LevelDecimalPlaces' ).val() );
            $('#LevelEdit').val($('#LevelEdit').val().replace(numformatInfo.NumberGroupSeparator, ''));
            if ( numLevelDecimals === 0 )
            {
                $( '#LevelEdit' ).numeric( { decimal: false, negative: true } );
            }
            else
            {
            	$('#LevelEdit').numeric({ decimal: numformatInfo.NumberDecimalSeparator, negative: true, decimalPlaces: numLevelDecimals });
            }
        }
        
        var numVolumeDecimals = parseInt($('#VolumeDecimalPlaces').val());
        $('#VolumeEdit').val($('#VolumeEdit').val().replace(numformatInfo.NumberGroupSeparator, ''));

        if (numVolumeDecimals === 0) {
        	$('#VolumeEdit').numeric({ decimal: false, negative: true });
        }
        else {
        	$('#VolumeEdit').numeric({ decimal: numformatInfo.NumberDecimalSeparator, negative: true, decimalPlaces: numVolumeDecimals });
        }
        // terminate the edit if user presses the Esc key
        $( '#LevelEdit' ).on( 'keyup', function( e )
        {
            if ( e.keyCode === 27 )
            { // escape key maps to keycode `27`
                FMStrapTableEditor.cancelEditRow( this, add );
                return false;
            }
            return true;
        } );

        $( '#VolumeEdit' ).on( 'keyup', function( e )
        {
            if ( e.keyCode === 27 )
            { // escape key maps to keycode `27`
                FMStrapTableEditor.cancelEditRow( this, add );
                return false;
            }
            return true;
        });

	    window.setTimeout( function()
	    {
		    $( '#LevelEdit' ).focus();
	    }, 100 );
    };
    var _parseLevel = function( a )
    {
        var levelUnit = $('#LevelUnit').val();
        if (levelUnit === 'FmlFtIn16Th' || levelUnit === 'FmlFtIn8Th') {
            // we are going to remove all special characters (,-.;) since the format is fixed it should work
            var value = a.toString();
            var negative = false;
            if (value.substring(0, 1) === '-') {
                negative = true;
                value = value.substring(1);
            }

            value = ((negative) ? '-' : '') + value.replace(/[-,.]/g, '');
            return parseFloat(value);
        }
        else {  // bds
            var numFormatInfoString = $('#NumberFormatInfoString').val();
            var numformatInfo = JSON.parse(numFormatInfoString);
            var newLevel = FMFormatValues.ParseValue(levelUnit, numformatInfo, a);

            var value = newLevel.toString();
            return parseFloat(value );
        }
    };


    var _endEditRow = function( event, input, add )
    {
        if ( event && event.shiftKey && event.keyCode === 9 )
        {
            return;
        }
        // only run this method once, don't call it again if we are already in the middle of processing
        if ( FMStrapTableEditor.processingEndEditRow === true )
        {
            return;
        }
    	  FMStrapTableEditor.inError = false;
        FMStrapTableEditor.processingEndEditRow = true;
        var tabContents = $('#tabList li.active').find('a').attr('data-target');
        if ( !tabContents )
        {
	        return;
        }
        var STEStrapTable = $( tabContents + ' .table' ).DataTable();
        var row = $( input ).parent().parent();
        var data = STEStrapTable.row( row ).data();
        var cell = $( '>td', row );
        if ($('#LevelEdit').val() != '' && $('#VolumeEdit').val() != '' && data !== undefined )
        {
            var numFormatInfoString = $( '#NumberFormatInfoString' ).val();
            var numformatInfo = JSON.parse( numFormatInfoString );

            var levelNumDecimals = isNaN( parseInt( $( '#LevelDecimalPlaces' ).val() ) ) ? 0 : parseInt( $( '#LevelDecimalPlaces' ).val() );
            var levelUnit = $( '#LevelUnit' ).val();

            var newFormattedLevel;
            if ( levelUnit === 'FmlFtIn16Th' )
            {
                newFormattedLevel = FMStrapTableEditor.convertFeetInch16thReadings( $( '#LevelEdit' ).val() );
            }
            else if ( levelUnit === 'FmlFtIn8Th' )
            {
                newFormattedLevel = FMStrapTableEditor.convertFeetInch8thReadings( $( '#LevelEdit' ).val() );
            }
            else
            {
                
                numformatInfo.NumberDecimalDigits = levelNumDecimals;
                var newLevel = FMFormatValues.ParseValue( levelUnit, numformatInfo, $( '#LevelEdit' ).val() );
                newFormattedLevel = FMFormatValues.FormatValueFullPrecision( levelUnit, numformatInfo, newLevel );
            }


            // Test for duplicate level
            var firstMatchAssending = STEStrapTable.rows().data().filter( function( value, index, instance )
            {
                return value[0] !== data[0] && value[1] === newFormattedLevel ? true : false;
            } )[0];
            if ( firstMatchAssending )
            {
            	FMStrapTableEditor.inError = true;
                FMLayout.Alert( $( '#SEDuplicateLevel' ).val(), 'Error', function()
                {
                		FMStrapTableEditor.processingEndEditRow = false;
                		FMStrapTableEditor.EnableDisableDeleteButton();
                		setTimeout(function () { input.focus(); }, 100);
                });
                return;
            }

            var firstMatchDescending = STEStrapTable.rows().data().filter( function( value, index, instance )
            {
                return value[0] !== data[0] && value[1] === newFormattedLevel ? true : false;
            } )[0];
            if ( firstMatchDescending )
            {
            	FMStrapTableEditor.inError = true;
                FMLayout.Alert( $( '#SEDuplicateLevel' ).val(), 'Error', function()
                {
                		FMStrapTableEditor.processingEndEditRow = false;
                		FMStrapTableEditor.EnableDisableDeleteButton();
                    setTimeout(function () { input.focus(); }, 100);
                });
                return;
            }
            
            var volumeNumDecimals = isNaN( parseInt( $( '#VolumeDecimalPlaces' ).val() ) ) ? 0 : parseInt( $( '#VolumeDecimalPlaces' ).val() );
            var volumeUnit = $( '#VolumeUnit' ).val();
            
            numformatInfo.NumberDecimalDigits = volumeNumDecimals;
            var newVolume = FMFormatValues.ParseValue( volumeUnit, numformatInfo, $( '#VolumeEdit' ).val() );
            var newFormattedVolume = FMFormatValues.FormatValueFullPrecision( volumeUnit, numformatInfo, newVolume );
            
            if (newFormattedVolume < 0) {
                FMStrapTableEditor.inError = true;
                FMLayout.Alert($('#SEVolumeNegative').val(), 'Error', function () {
                    FMStrapTableEditor.processingEndEditRow = false;
                    FMStrapTableEditor.EnableDisableDeleteButton();
                    setTimeout(function () { input.focus(); }, 100);
                });
                return;
            }

            // Test for duplicate volume
            var firstMatchAssending = STEStrapTable.rows().data().filter( function( value, index, instance )
            {
                return value[0] !== data[0] && value[2] === newFormattedVolume ? true : false;
            } )[0];
            if ( firstMatchAssending )
            {
            	FMStrapTableEditor.inError = true;
                FMLayout.Alert( $( '#SEDuplicateVolume' ).val(), 'Error', function()
                {
                		FMStrapTableEditor.processingEndEditRow = false;
                		FMStrapTableEditor.EnableDisableDeleteButton();
                    setTimeout(function () { input.focus(); }, 100);
                });
                return;
            }

            var firstMatchDescending = STEStrapTable.rows().data().filter( function( value, index, instance )
            {
                return value[0] !== data[0] && value[2] === newFormattedVolume ? true : false;
            } )[0];
            if ( firstMatchDescending )
            {
            	FMStrapTableEditor.inError = true;
                FMLayout.Alert( $( '#SEDuplicateVolume' ).val(), 'Error', function()
                {
                		FMStrapTableEditor.processingEndEditRow = false;
                		FMStrapTableEditor.EnableDisableDeleteButton();
                    setTimeout(function () { input.focus(); }, 100);
                });
                return;
            }
            
            //find the level before the new value
            var levelFixed = 0.0;

            levelFixed = FMStrapTableEditor.parseLevel(newFormattedLevel);
            var rowBefore = STEStrapTable.rows().data().filter( function( value, index, instance )
            {
                return value[0] !== data[0] && FMStrapTableEditor.parseLevel( value[1] ) < levelFixed ? true : false;
            } ).reverse()[0];

            //find the level after the new value
            var rowAfter = STEStrapTable.rows().data().filter( function( value, index, instance )
            {
                return value[0] !== data[0] && FMStrapTableEditor.parseLevel( value[1] ) > levelFixed ? true : false;
            } )[0];

            
            if ( rowBefore )
            {
                var beforeVolume = FMFormatValues.ParseValue( volumeUnit, numformatInfo, rowBefore[2] );
                if ( math.compare( newVolume, beforeVolume ) == -1 )
                {
                	FMStrapTableEditor.inError = true;
                	FMLayout.Alert( $( '#SEVolumeSequence' ).val(), 'Error', function()
                	{
                		FMStrapTableEditor.processingEndEditRow = false;
                		FMStrapTableEditor.EnableDisableDeleteButton();
                		setTimeout(function () { input.focus(); }, 100);
                	});
                  return;
                }
            }

            if ( rowAfter )
            {
                var afterVolume = FMFormatValues.ParseValue( volumeUnit, numformatInfo, rowAfter[2] );
                if ( math.compare( newVolume, afterVolume ) == 1 )
                {
                	FMStrapTableEditor.inError = true;
                	FMLayout.Alert($('#SEVolumeSequence').val(), 'Error', function () {
                		FMStrapTableEditor.processingEndEditRow = false;
                		FMStrapTableEditor.EnableDisableDeleteButton();
                		setTimeout(function () { input.focus(); }, 100);
                	});
                  return;
                }
            }

            $("#VolumeEdit").removeNumeric(); // remove numeric mask if there was one
            $("#LevelEdit").removeNumeric(); // remove numeric mask if there was one
            $("#LevelEdit").unmask(); // if it had a mask remove it
            
            data[1] = newFormattedLevel;
            data[2] = newFormattedVolume;
            cell[1].innerHTML = data[1];
            cell[2].innerHTML = data[2];

            FMStrapTableEditor.valuesChanged = true;

            if ( !add )
            {
                STEStrapTable.row( row ).invalidate();
            }
            STEStrapTable.order( STEStrapTable.order() );
            STEStrapTable.columns.adjust().draw();

            STEStrapTable.column( 0, { search: 'applied', order: 'applied' } ).nodes().each( function( cell, i )
            {
                cell.innerHTML = i + 1;
            } );

            $('#PEMPESavePropertyScreen').removeAttr( 'disabled' );
            if ( event && event.keyCode === 9 && add )
            {
                FMStrapTableEditor.processingEndEditRow = false;
                FMStrapTableEditor.addRow();
                FMStrapTableEditor.EnableDisableDeleteButton();
                return;
            }
        }
        else
        {
            FMStrapTableEditor.cancelEditRow( input, add );
        }
        FMStrapTableEditor.processingEndEditRow = false;
	     FMStrapTableEditor.EnableDisableDeleteButton();
    };

    var _cancelEditRow = function( input, add )
    {
    		var tabContents = $('#tabList li.active').find('a').attr('data-target');
    		if (!tabContents) {
    			return;
    		}
        var STEStrapTable = $( tabContents + ' .table' ).DataTable();
        var row = $( input ).parent().parent();
        var data = STEStrapTable.row( row ).data();
        var cell = $('>td', row);
	     if ( data !== undefined )
	     {
		    cell[1].innerHTML = data[1];
		    cell[2].innerHTML = data[2];
	     }

	     if ( add )
        {
            STEStrapTable.row( row ).remove();
            STEStrapTable.columns.adjust().draw();
        }
        FMStrapTableEditor.inError = false;

        var tableIndex = tabContents.substring( tabContents.indexOf( '_' ) + 1 );
        $( '#addStrapEntryButton_' + tableIndex ).removeAttr( 'disabled' );
        $('#PEMPESavePropertyScreen').removeAttr( 'disabled' );
    };

    var _deleteRows = function()
    {
        FMLayout.Confirm( $( '#SEDeleteConfirmation' ).val(),
            null,
            function()
            {
                var tabContents = $( '#tabList li.active' ).find( 'a' ).attr( 'data-target' );
                var STEStrapTable = $( tabContents + ' .table' ).DataTable();
                STEStrapTable.rows( '.selected' ).remove().draw( false );
                FMStrapTableEditor.valuesChanged = true;
                STEStrapTable.column( 0, { search: 'applied', order: 'applied' } ).nodes().each( function( cell, i )
                {
                    cell.innerHTML = i + 1;
                });
                FMStrapTableEditor.EnableDisableDeleteButton();
	            FMStrapTableEditor.inError = false;
            } );
    };
    var _deleteTable = function()
    {
        // Check if table is selected
        var activeTab = $( '#tabList li.active' );
        if ( !activeTab )
        {
            return;
        }

        var tableDescription = $( '#tabList li.active .tab-name' ).text();

        if ( $( '#TableForStrapDropDownList option:selected' ).text() === tableDescription
            || $( '#TableForWaterVolumeDropDownList option:selected' ).text() === tableDescription
            || $( '#TableForSolidsVolumeDropDownList option:selected' ).text() === tableDescription )
        {
            FMLayout.Alert( $( '#SEDeleteTableErrorTableSelected' ).val(), 'Error' );
            return;
        }

        FMLayout.Confirm( $( '#SEDeleteTableConfirmation' ).val() + ' ' + tableDescription + '?',
            null,
            function()
            {
                var tabContents = $( activeTab ).find( 'a' ).attr( 'data-target' );
                var tableIndex = tabContents.substring( tabContents.indexOf( '_' ) + 1 );

                var url = $( '#urlDeleteStrapTable' ).val();
                var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
                var headers = {};
                headers['__RequestVerificationToken'] = token;

                $.ajax( {
                    url: url,
                    cache: false,
                    type: 'POST',
                    headers: headers,
                    data: ( function()
                    {
                        // serialize the form
                        var serializedData = $( '#strapTableForm' ).serialize();

                        // i want to serialize the table so MVC binds it automatically to the strap object
                        var strapTables = [];

                        var tabs = $( '#tabList li.tab-table' ).each( function()
                        {
                            var tabContents = $( this ).find( 'a' ).attr( 'data-target' );
                            var strapTableEntries = [];
                            strapTables.push( strapTableEntries );
                            $( tabContents + ' .table tr' ).each( function()
                            {
                                if ( $( this ).find( 'td.strapTableLevelColumn' ).length > 0 )
                                {
                                    strapTableEntries.push( {
                                        LevelEntry: $( this ).find( 'td.strapTableLevelColumn' ).text().trim(),
                                        VolumeEntry: $( this ).find( 'td.strapTableVolumeColumn' ).text().trim()
                                    } );
                                }
                            } );
                        } );

                        serializedData = 'editorEntries=' + JSON.stringify( strapTables ) + '&' + serializedData + '&deleteTableIndex=' + tableIndex;
                        return serializedData;
                    } )(),
                    success: function( result )
                    {
                        var messageAttributes = { addclass: 'stack-bottomright', stack: FMStrapTableEditor.stack_bottomright_straptable, width: '450px' };

                        FMErrorAndExceptionHandling.HandleMessages( result,
                            function( data, inError )
                            {
                                if ( !inError )
                                {
                                    $( '#moduleEditorPropertyScreen' ).html( data );
                                    FMStrapTableEditor.valuesChanged = true;

                                    var tabCount = $( '#tabList li.tab-table' ).length;
                                    $( '#SEDeleteTable' ).prop( 'disabled', ( tabCount <= 1 ) ? 'disabled' : '' );
                                    FMStrapTableEditor.enableAddTab( tabCount );
                                }
                            },
                            messageAttributes );
                    },
                    error:
                        function( request, status, error )
                        {
                            var messageAttributes = { addclass: 'stack-bottomright', stack: FMStrapTableEditor.stack_bottomright_straptable, width: '450px' };
                            FMErrorAndExceptionHandling.ShowException( request, status, error, null, messageAttributes );
                        }
                } );
            } );
    };
    _resetSettingInputMask = function( control, unitControl, numDecimalsControl )
    {
        var unit = $( '#' + unitControl ).val();
        var precision = $( '#' + numDecimalsControl ).val();
        $( control ).removeNumeric(); // remove numeric mask if there was one
        $( control ).unmask(); // if it had a mask remove it

        // add the mask to the edit fields and populate them with the initial formatted value
        // if feet-in-16th or feet-in-8th use 00-00-00 as mask, otherwise is just plain numeric
        if ( unit === 'FmlFtIn16Th' )
        { //"FmlFtIn16Th"
            $( control ).mask( '99-99-99', {
                placeholder: '__-__-__'
            } );
        }
        else if ( unit === 'FmlFtIn8Th' )
        { //"FmlFtIn8Th"
            $( control ).mask( '99-99-9', {
                placeholder: '__-__-__'
            } );
        }
        else
        {
                $( control ).attr( 'placeholder', '' );
                $( control ).numeric( {
                    decimal: '.',
                    negative: true,
                    decimalPlaces: parseInt( 9 )
                } );
        }
    };
    return {
		  valuesChanged: _valuesChanged,
		  inError: _inError,
		  deletingStrapEntry: _deletingStrapEntry,
		  addingStrapEntry: _addingStrapEntry,
        convertFeetInch8thReadings: _convertFeetInch8thReadings,
        convertFeetInch16thReadings: _convertFeetInch16thReadings,
        saveChanges: _saveChanges,
        validateRegionalSettings: _validateRegionalSettings,
        renameTab: _renameTab,
        endRenameTab: _endRenameTab,
        addTable: _addTable,
        enableAddTab: _enableAddTab,
        addRow: _addRow,
        editRow: _editRow,
        endEditRow: _endEditRow,
        cancelEditRow: _cancelEditRow,
        onblurEditRow: _onblurEditRow,
        parseLevel: _parseLevel,
        deleteRows: _deleteRows,
        deleteTable: _deleteTable,
        resetSettingInputMask: _resetSettingInputMask,
        processingEndEditRow: _processingEndEditRow,
        stack_bottomright_straptable: _stack_bottomright_straptable
    };
}();

FMStrapTableEditor.MinimumRowCount = 4;

FMStrapTableEditor.EnableDisableDeleteButton = function()
{
	var tabContents = $('#tabList li.active').find('a').attr('data-target');
	var STEStrapTable = $(tabContents + ' .table').DataTable();
	var selectedCount = STEStrapTable.rows('.selected').count();
	var rowsCount = STEStrapTable.rows().count();
	$(tabContents + ' .deleteStrapEntriesButton').prop('disabled', (selectedCount === 0 || rowsCount <= FMStrapTableEditor.MinimumRowCount) ? 'disabled' : '');
	$(tabContents + ' .deleteStrapEntriesButton').removeClass('strapTableDeleteButtonDisabled');

	if (rowsCount <= FMStrapTableEditor.MinimumRowCount)
	{
		$(tabContents + ' .deleteStrapEntriesButton').addClass('strapTableDeleteButtonDisabled');
	}
}


// manually hookup to the submit the form to make sure we pass all the entries from the table
$( function()
{
    $( '#strapTableForm' ).submit( function()
    {
        var action = this.action;
        var method = this.method;

        FMStrapTableEditor.saveChanges();

        // it is important to return false in order to
        // cancel the default submission of the form
        // and perform the AJAX call
        return false;
    } );
} );


//--------------------------------------- RUN after page has been loaded but before render -----------------------------

$( document ).ready( function()
{
    // create special ordering for strap tables (it seems to fail since we could have a combination of commas, periods and dashes)

    
    $.fn.dataTable.ext.type.order['strap-table-asc'] = function( a, b )
    {
        // for an add place at the bottom
        var levelUnit = $('#LevelUnit').val();
        var c;
        var d;

        if (a == '') {
            return 1;
        }

        if (levelUnit === 'FmlFtIn16Th' || levelUnit === 'FmlFtIn8Th') {
            var value = a.toString();
            var negative = false;
            if (value.substring(0, 1) === '-') {
                negative = true;
                value = value.substring(1);
            }

            value = ((negative) ? '-' : '') + value.replace(/[-,.]/g, '');
            c = parseFloat(value);

            var value1 = b.toString();
            var negative = false;
            if (value1.substring(0, 1) === '-') {
                negative = true;
                value1 = value1.substring(1);
            }

            value1 = ((negative) ? '-' : '') + value1.replace(/[-,.]/g, '');
            d = parseFloat(value1);
        }
        else {
            var numFormatInfoString = $('#NumberFormatInfoString').val();
            var numformatInfo = JSON.parse(numFormatInfoString);
            var newLevel = FMFormatValues.ParseValue(levelUnit, numformatInfo, a);

            var value = newLevel.toString();
            c = parseFloat(value);

            var newLevel1 = FMFormatValues.ParseValue(levelUnit, numformatInfo, b);

            var value1 = newLevel1.toString();
            d = parseFloat(value1);
        }


        return ((c < d) ? -1 : ((c > d) ? 1 : 0));
    };
    $.fn.dataTable.ext.type.order['strap-table-desc'] = function( a, b )
    {
        // for an add place at the top
        var levelUnit = $('#LevelUnit').val();
        var c;
        var d;
        
        if (a == '') {
            return -1;
        }
        if (levelUnit === 'FmlFtIn16Th' || levelUnit === 'FmlFtIn8Th') {
            var value = a.toString();
            var negative = false;
            if (value.substring(0, 1) === '-') {
                negative = true;
                value = value.substring(1);
            }

            value = ((negative) ? '-' : '') + value.replace(/[-,.]/g, '');
            c = parseFloat(value);

            var value1 = b.toString();
            var negative = false;
            if (value1.substring(0, 1) === '-') {
                negative = true;
                value1 = value1.substring(1);
            }

            value1 = ((negative) ? '-' : '') + value1.replace(/[-,.]/g, '');
            d = parseFloat(value1);
        }
        else {
            var numFormatInfoString = $('#NumberFormatInfoString').val();
            var numformatInfo = JSON.parse(numFormatInfoString);
            var newLevel = FMFormatValues.ParseValue(levelUnit, numformatInfo, a);

            var value = newLevel.toString();
            c = parseFloat(value);

            var newLevel1 = FMFormatValues.ParseValue(levelUnit, numformatInfo, b);

            var value1 = newLevel1.toString();
            d = parseFloat(value1);
        }
        return ((c < d) ? 1 : ((c > d) ? -1 : 0));
    };
    
    $( '#mainTab' ).scrollingTabs( { scrollToTabEdge: true } );

    var activeTab = $( '#strapTableForm input[name=ActiveTab]' ).val();

    $( 'a[data-target="#StrapTable_' + activeTab + '"]' ).tab( 'show' );

    var tabCount = $( '#tabList li.tab-table' ).length;

    for ( var strapTableIndex = 0; strapTableIndex < tabCount; strapTableIndex++ )
    {
        var STEStrapTable = $( '#StrapTableEditorTable_' + strapTableIndex ).DataTable(
            {
                "retrieve": true,
                "select": true,
                "order": [1, 'asc'],
                "ordering": true,
                "scrollY": '358px',
                "sScrollX": '100%',
                "sScrollXInner": '100%',
                "scrollCollapse": false,
                "paging": false,
                "autoWidth": false,
                "columnDefs": [
                    { "targets": [0], "name": 'Number', "orderable": false, className: 'text-center'},
                    { "targets": [1], "name": 'Level', "orderable": true, "type": 'strap-table', className: 'strapTableLevelColumn text-center' },
                    { "targets": [2], "name": 'Volume', "orderable": false, className: 'strapTableVolumeColumn text-right' }
                ],
                "dom": 'rt',
                "fnInitComplete": function()
                {
                    // custom scroll bars
                    $( this ).parent()
                        .niceScroll( {
                            cursorwidth: '10px',
                            autohidemode: false,
                            cursorcolor: '#486899',
                            background: 'rgb(240, 240, 240)',
                            horizrailenabled: false
                        });
                }
            } );

        STEStrapTable.draw();

        $( '#StrapTableEditorTable_' + strapTableIndex + ' tbody' ).keyup( function( e )
        {
            if ( e.keyCode === 46 )
            {
                var tabContents = $( '#tabList li.active' ).find( 'a' ).attr( 'data-target' );
                var STEStrapTable = $( tabContents + ' .table' ).DataTable();
                if ( $( '#LevelEdit' ).length === 0 && STEStrapTable.rows( '.selected' ).count() > 0 )
                {
                    FMStrapTableEditor.deleteRows();
                }
            }
        } );

        // enable delete button when 2 or more rows and selected rows
        $( '#StrapTableEditorTable_' + strapTableIndex + ' tbody' ).on( 'click', 'tr', function()
        {
        	  $(this).toggleClass('selected');
	        FMStrapTableEditor.EnableDisableDeleteButton();
        } );

        // when clicking on the import button we want to display the file selection box
        $( '#straptableEditorPartial [value="importButton_' + strapTableIndex + '"]' ).click( function()
        {
        	// we need to make sure this executes after the onblur event of the edits which has a delay of 100 ms
	        window.setTimeout( function()
	        {
        			if ( FMStrapTableEditor.inError === true )
					  {
						  return;
					  }
			
					$( '#importStrapTable' ).val( '' );
					$( '#importStrapTable' ).click();
	        }, 110 );

        } );

        // click on the export button
        $( '#straptableEditorPartial [value="exportButton_' + strapTableIndex + '"]' ).click( function()
        {
        		// we need to make sure this executes after the onblur event of the edits which has a delay of 100 ms
        		window.setTimeout(function () {
        			if (FMStrapTableEditor.inError === true) {
        				return;
        			}

	            FMStrapTableEditor.validateRegionalSettings();
        		}, 110);

        } );


        // click on the delete button
        $( '#deleteStrapEntriesButton_' + strapTableIndex ).on( 'click', function()
        {
        	window.setTimeout(function () {
	        	FMStrapTableEditor.deleteRows();
        	}, 110);

        });

        $('#deleteStrapEntriesButton_' + strapTableIndex).on("mousedown", function (e)
        {
	        FMStrapTableEditor.deletingStrapEntry = true;
        });

        $('#deleteStrapEntriesButton_' + strapTableIndex).on("mouseup", function (e)
        {
	        setTimeout( function() { FMStrapTableEditor.deletingStrapEntry = false; }, 100 );
        });

        $('#addStrapEntryButton_' + strapTableIndex).on("mousedown", function (e)
        {
	        FMStrapTableEditor.addingStrapEntry = true;
        });

        $('#addStrapEntryButton_' + strapTableIndex).on("mouseup", function (e)
        {
				setTimeout(function () { FMStrapTableEditor.addingStrapEntry = false; }, 100);
        });

        // double click to edit a row 
        $( '#StrapTableEditorTable_' + strapTableIndex + ' tbody' ).on( 'dblclick', 'tr', function()
        {
	        var that = this;

        		window.setTimeout(function () {
        			if ($('#LevelEdit').length === 0) {
        				FMStrapTableEditor.editRow(that, false);
        			}
        		}, 100);
        } );


        // click to add a strap table entry row
        $( '#addStrapEntryButton_' + strapTableIndex ).on( 'click', function()
        {
	        window.setTimeout( function()
	        {
					if (FMStrapTableEditor.inError === true) {
        				return;
					}
					FMStrapTableEditor.addRow();
	        }, 110 );

        } );

        // set the input mask for all settings of type length (level)
        $( 'input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].DatumHeight\'], input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].RoofLandingHeight\'], input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].RoofFloatingHeight\']' ).each(
            function()
            {
                FMStrapTableEditor.resetSettingInputMask( this, 'LevelUnit', 'LevelDecimalPlaces' );
            }
        );

        $( 'input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].DatumHeight\'], input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].RoofLandingHeight\'], input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].RoofFloatingHeight\']' ).on( 'blur', function()
        {
            var numFormatInfoString = $( '#NumberFormatInfoString' ).val();
            var numformatInfo = JSON.parse( numFormatInfoString );

            var levelNumDecimals = isNaN( parseInt( $( '#LevelDecimalPlaces' ).val() ) ) ? 0 : parseInt( $( '#LevelDecimalPlaces' ).val() );
            var levelUnit = $( '#LevelUnit' ).val();

            numformatInfo.NumberDecimalDigits = levelNumDecimals;

            var newFormattedLevel;
            if ( levelUnit === 'FmlFtIn16Th' )
            {
                newFormattedLevel = FMStrapTableEditor.convertFeetInch16thReadings( $( this ).val() );
            }
            else if ( levelUnit === 'FmlFtIn8Th' )
            {
                newFormattedLevel = FMStrapTableEditor.convertFeetInch8thReadings( $( this ).val() );
            }
            else
            {
                
                numformatInfo.NumberDecimalDigits = levelNumDecimals;
                var newLevel = FMFormatValues.ParseValue( levelUnit, numformatInfo, $( this ).val() );
                newFormattedLevel = FMFormatValues.FormatValueFullPrecision( levelUnit, numformatInfo, newLevel );
            }

            $( this ).val( newFormattedLevel );
        } );

        $( 'input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].StrapDensity\']' ).on( 'blur', function()
        {
            var numFormatInfoString = $( '#NumberFormatInfoString' ).val();
            var numformatInfo = JSON.parse( numFormatInfoString );

            var densityNumDecimals = isNaN( parseInt( $( '#DensityDecimalPlaces' ).val() ) ) ? 0 : parseInt( $( '#DensityDecimalPlaces' ).val() );
            var densityUnit = $( '#DensityUnit' ).val();

            var newFormattedDensity;
            numformatInfo.NumberDecimalDigits = densityNumDecimals;
            var newDensity = FMFormatValues.ParseValue( densityUnit, numformatInfo, $( this ).val() );
            newFormattedDensity = FMFormatValues.FormatValueFullPrecision( densityUnit, numformatInfo, newDensity );

            $( this ).val( newFormattedDensity );
        } );

        $( 'input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].RoofMass\']' ).on( 'blur', function()
        {
            var numFormatInfoString = $( '#NumberFormatInfoString' ).val();
            var numformatInfo = JSON.parse( numFormatInfoString );

            var massNumDecimals = isNaN( parseInt( $( '#MassDecimalPlaces' ).val() ) ) ? 0 : parseInt( $( '#MassDecimalPlaces' ).val() );
            var massUnit = $( '#MassUnit' ).val();

            var newFormattedMass;
            numformatInfo.NumberDecimalDigits = massNumDecimals;
            var newMass = FMFormatValues.ParseValue( massUnit, numformatInfo, $( this ).val() );
            newFormattedMass = FMFormatValues.FormatValueFullPrecision( massUnit, numformatInfo, newMass );

            $( this ).val( newFormattedMass );
        } );

        $( 'input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].StrapTemperature\'], input[name=\'EditorStrapTableSettings[' + strapTableIndex + '].TankShellReferenceTemperature\']' ).on( 'blur', function()
        {
            var numFormatInfoString = $( '#NumberFormatInfoString' ).val();
            var numformatInfo = JSON.parse( numFormatInfoString );

            var temperatureNumDecimals = isNaN( parseInt( $( '#TemperatureDecimalPlaces' ).val() ) ) ? 0 : parseInt( $( '#TemperatureDecimalPlaces' ).val() );
            var temperatureUnit = $( '#TemperatureUnit' ).val();

            var newFormattedtemperature;
            numformatInfo.NumberDecimalDigits = temperatureNumDecimals;
            var newtemperature = FMFormatValues.ParseValue( temperatureUnit, numformatInfo, $( this ).val() );
            newFormattedtemperature = FMFormatValues.FormatValueFullPrecision( temperatureUnit, numformatInfo, newtemperature );

            $( this ).val( newFormattedtemperature );
        } );
    }

    // when file in selection bax has changed we want to call the import
    $( '#importStrapTable' ).change( function()
    {
        // if we have files
        if ( $( this )[0].files.length > 0 )
        {
            var formdata = new FormData(); //FormData object
            
            var token = $( '#strapTableForm input[name=__RequestVerificationToken]' ).val();
            var headers = {};
            headers['__RequestVerificationToken'] = token;
            //Appending the file to FormData object
            formdata.append( $( this )[0].files[0].name, $( this )[0].files[0] );


            // serialize the form
            var serializedData = $( '#strapTableForm' ).serializeArray();

            $.each( serializedData, function( key, input )
            {
                formdata.append( input.name, input.value );
            } );


            // i want to serialize the table so MVC binds it automatically to the strap object
            var strapTables = [];

            var tabs = $( '#tabList li.tab-table' ).each( function()
            {
                var tabContents = $( this ).find( 'a' ).attr( 'data-target' );
                var strapTableEntries = [];
                strapTables.push( strapTableEntries );
                $( tabContents + ' .table tr' ).each( function()
                {
                    if ( $( this ).find( 'td.strapTableLevelColumn' ).length > 0 )
                    {
                        strapTableEntries.push( {
                            LevelEntry: $( this ).find( 'td.strapTableLevelColumn' ).text().trim(),
                            VolumeEntry: $( this ).find( 'td.strapTableVolumeColumn' ).text().trim()
                        } );
                    }
                } );
            } );


            formdata.append( 'editorEntries', JSON.stringify( strapTables ) );

            var tabContents = $( '#tabList li.active' ).find( 'a' ).attr( 'data-target' );
            formdata.append( 'importTableIndex', tabContents.substring( tabContents.indexOf( '_' ) + 1 ) );
            var url = $( this ).attr( 'data-url' );

            // notification position
            var messageAttributes = { addclass: 'stack-bottomright', stack: FMStrapTableEditor.stack_bottomright_straptable, width: '450px' };
            // remove previous notifications
            PNotify.removeStack( FMStrapTableEditor.stack_bottomright_straptable );

            $.ajax( {
                type: 'POST',
                url: url,
                contentType: false,
                processData: false,
                enctype: 'multipart/form-data',
                headers: headers,
                data: formdata,
                success: function( result )
                {

                    var displaySelectionDialog = "";
                    // see if the text is success and there are strap entries at the end
                    
                    if (typeof result != 'undefined' && result != null) {
                        var count = 0;
                        if (result && result.SuccessMessage) {
                            $.each(result.SuccessMessage, function (key, message) { count = count + 1; });
                        }
                        if (count > 0) { // if save was succesful
                            var messageList = [];
                            // add messages but avoid duplicates
                            var seen = {};
                            $.each(result.SuccessMessage, function (key, message) {
                                $.each(message, function (i, val) {
                                    if (!seen[message[i]]) {
                                        seen[message[i]] = true;
                                        messageList.push(message[i]);
                                    }
                                });
                            });
                            var messages = messageList.join('');
                            var res = messages.split("-");
                            
                            if (res.length === 3) {  // entrie do not match
                                displaySelectionDialog = "Actual Strap Table Entries Do Not Match Configured!\n\r";
                                displaySelectionDialog += "Configured " + res[1] + ". Actual Found " + res[2] + ".\n\r";
	                            displaySelectionDialog += "Keep Imported Strap Table?";
                            }
                            else if (res.length === 4) { // precision in file is greater
                                displaySelectionDialog = "Configured Variable Precision is Less Than File Precision!\n\r";
                                displaySelectionDialog += "Precision in File: " + res[3] + ".";
	                            displaySelectionDialog += " Keep Imported Strap Table?";
                            }
                            else if (res.length === 6) { // both
                                displaySelectionDialog = "Actual Strap Table Entries Do Not Match Configured!\n\r";
                                displaySelectionDialog += "Configured " + res[1] + ". Actual Found " + res[2] + "\n\r";
                                displaySelectionDialog += " AND "
                                displaySelectionDialog += "Configured Variable Precision is Less Than File Precision!\n\r";
                                displaySelectionDialog += "Precision in File: " + res[5] + ".";
	                            displaySelectionDialog += " Keep Imported Strap Table?";
                            }
                        }
                    }
                    var returnValue = true;
                    if (displaySelectionDialog.length > 0) {
                        FMLayout.ConfirmYesNo(displaySelectionDialog,
                            "",
                            function () {
                                FMErrorAndExceptionHandling.HandleMessages(result,
                                    function (data, inError) {
                                        if (!inError) {

                                            //we need to reload the new table, replace the holder with the partial view
                                            $('#moduleEditorPropertyScreen').html(data);
                                        }
                                    },
                                    messageAttributes);
                            },
							function () {
							    returnValue = false;
							});

                        //returnValue = confirm(displaySelectionDialog);
                    }
                    else{
                        FMErrorAndExceptionHandling.HandleMessages(result,
                            function (data, inError) {
                                if (!inError) {

                                    //we need to reload the new table, replace the holder with the partial view
                                    $('#moduleEditorPropertyScreen').html(data);
                                }
                            },
                            messageAttributes);
                    }
                },
                error: function( request, status, error )
                {
                    FMErrorAndExceptionHandling.ShowException( request, status, error, null, messageAttributes );
                }
            } );
        }
        return false;
    } );

    // adjust the column headers and store the Active Tab
    $( '#tabList li.tab-table' ).on( 'click', function()
    {
        setTimeout( function()
        {
            var tabContents = $( '#tabList li.active' ).find( 'a' ).attr( 'data-target' );
            var STEStrapTable = $( tabContents + ' .table' ).DataTable();
            STEStrapTable.columns.adjust().draw();
            $( '#strapTableForm input[name=ActiveTab]' ).val( $( '#tabList li.active' ).index() );
        }, 110);

	    if ( $( '#LevelEdit' ).length !== 0 )
	    {
		    return false;
	    }
    } );

    // adjust the column headers for the actvie tab
    setTimeout( function()
	 {
			 var tabContents = $('#tabList li.active').find('a').attr('data-target');
			 var STEStrapTable = $(tabContents + ' .table').DataTable();
			 STEStrapTable.columns.adjust().draw();
   }, 100 );

    // Add Strap Table Delete button to footer
    var leftFooter = $( '#ModulePropertyEditorPropertyScreen .modal-footer .btn-toolbar .pull-left' );
    leftFooter.html( '<button id=\"SEDeleteTable\" type=\"button\" class=\"formfieldtitle pushButton\" style=\"width: 100px;margin-left: 22px;\">' + $( '#SEDeleteTableText' ).val() + '</button>' );

    // Add alignment for the close button in the footer
    $( '#PEMPEClosePropertyScreen' ).css( 'marginRight', '42px' );

    // Hide the Header, as StrapTableEditor provides one
    $( '.modal-header' ).hide();

    $( '#SEDeleteTable' ).prop( 'disabled', ( tabCount <= 1 ) ? 'disabled' : '' );

    FMStrapTableEditor.enableAddTab( tabCount );

    $( '#SEDeleteTable' ).on( 'click', function()
    {
        FMStrapTableEditor.deleteTable();
    });

    FMErrorAndExceptionHandling.CloseNotifications();
} );