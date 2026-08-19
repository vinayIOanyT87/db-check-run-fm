//debugger;

// create a class with helper functions for the vcf settings editor
var FMVcfSettingsEditor = function()
{
	var _stack_bottomright_vcfsettings = { "dir1": 'up', "dir2": 'left', "firstpos1": 75, "firstpos2": 25, "context": $( '#ModulePropertyEditorPropertyScreen' ) };

	var GetStandardsAndRevisions = function( standardsOrganization )
	{
		var standardAndRevisionList = [];

		switch ( standardsOrganization )
		{
				case 'None':
					standardAndRevisionList.push( '1952' );
					standardAndRevisionList.push( '1980' );
					break;

				case 'API':
					standardAndRevisionList.push( '1952' );
					standardAndRevisionList.push( '1980' );
					standardAndRevisionList.push( 'Commodity (2004)' );
					break;

				case 'ASTM':
					standardAndRevisionList.push( 'D1250 (1952)' );
					standardAndRevisionList.push( 'D1250 (1980)' );
					standardAndRevisionList.push( 'D1555 (1980)' );
					standardAndRevisionList.push( 'D1555 (2004)' );
					standardAndRevisionList.push( 'D1555 (2009)' );
					standardAndRevisionList.push( 'D4311 (2004)' );
					standardAndRevisionList.push( 'D4311 (2009)' );
					standardAndRevisionList.push( 'IP' );
					break;

				case 'Custom':
					standardAndRevisionList.push( 'Polynomial (1952)' );
					standardAndRevisionList.push( 'Polynomial (1980)' );
					break;

				case 'JIS':
					standardAndRevisionList.push( 'None' );
					standardAndRevisionList.push( 'D1555' );
					standardAndRevisionList.push( 'D1250' );
					standardAndRevisionList.push( '2249 (1980)' );
					standardAndRevisionList.push( '2250 (1967)' );
					standardAndRevisionList.push( 'Chemical' );
					standardAndRevisionList.push( '2249 (1980) Table' );
					break;

				case 'GB/T':
					standardAndRevisionList.push( '1980' );
					break;

				case 'GOST':
					standardAndRevisionList.push( '1987' );
					break;


				default:
					break;
		}

		return standardAndRevisionList;
	};
	var GetCommoditiesOrTables = function( standardsOrganization, standardAndRevision )
	{
		var commodityOrTable = [];

		switch ( standardsOrganization )
		{
				case 'None':
					commodityOrTable.push( 'None' );
					break;

				case 'API':
					switch ( standardAndRevision )
					{
						case '1952':
						case '1980':
								commodityOrTable.push( '54A/53A' );
								commodityOrTable.push( '54B/53B' );
								commodityOrTable.push( '54C' );
								commodityOrTable.push( '54D' );
								commodityOrTable.push( '60A/59A' );
								commodityOrTable.push( '60B/59B' );
								commodityOrTable.push( '60D/59D' );
								commodityOrTable.push( '6A/5A' );
								commodityOrTable.push( '6B/5B' );
								commodityOrTable.push( '6C' );
								commodityOrTable.push( '6D' );
								if ( standardAndRevision === '1980'
									|| standardAndRevision === '2004' )
								{
									commodityOrTable.push( '24E/23E' );
								}
								break;
						case 'Commodity (2004)':
								commodityOrTable.push( 'Alpha 60 Supplied' );
								commodityOrTable.push( 'Crude Oils' );
								commodityOrTable.push( 'Lubrication Oils' );
								commodityOrTable.push( 'Refined Products' );
								break;

						default:
								break;
					}

				case 'ASTM':
					switch ( standardAndRevision )
					{
						case 'D1250 (1952)':
						case 'D1250 (1980)':
								commodityOrTable.push( 'LPG' );
								break;


						case 'D4311 (2004)':
						case 'D4311 (2009)':
						case 'IP':
								commodityOrTable.push( 'Asphalt' );
								break;

						case 'D1555 (1980)':
						case 'D1555 (2004)':
						case 'D1555 (2009)':
								commodityOrTable.push( 'Benzene' );
								commodityOrTable.push( 'Toluene' );
								commodityOrTable.push( 'Mixed Xylene' );
								commodityOrTable.push( 'Styrene' );
								commodityOrTable.push( 'o-Xylene' );
								commodityOrTable.push( 'p-Xylene' );
								commodityOrTable.push( 'Cylco-Hexane' );
								commodityOrTable.push( 'Ethyl-Benzene' );
								commodityOrTable.push( 'Cumene' );
								commodityOrTable.push( '300 °F/148.9 °C Aromatic' );
								commodityOrTable.push( '350 °F /176.7 °C Aromatic' );
								break;

						default:
								break;
					}
					break;

				case 'Custom':
					commodityOrTable.push( 'K-Factors' );
					break;


				case 'JIS':
					switch ( standardAndRevision )
					{
						case 'None':
								commodityOrTable.push( 'None' );
								break;

						case 'D1555':
								commodityOrTable.push( 'Benzene' );
								commodityOrTable.push( 'Toluene' );
								commodityOrTable.push( 'Mixed Xylene' );
								commodityOrTable.push( 'Styrene' );
								commodityOrTable.push( 'o-Xylene' );
								commodityOrTable.push( 'p-Xylene' );
								commodityOrTable.push( 'Cylco-Hexane' );
								commodityOrTable.push( 'Ethyl-Benzene' );
								commodityOrTable.push( 'Cumene' );
								commodityOrTable.push( '300 °F/148.9 °C Aromatic' );
								commodityOrTable.push( '350 °F /176.7 °C Aromatic' );
								break;

						case 'D1250':
								commodityOrTable.push( '2 (54)' );
								commodityOrTable.push( '54A (6X)' );
								commodityOrTable.push( '54B (6X)' );
								commodityOrTable.push( '55' );
								break;

						case '2249 (1980)':
								commodityOrTable.push( '54A/53A' );
								commodityOrTable.push( '54B/53B' );
								commodityOrTable.push( '54C' );
								commodityOrTable.push( '54D' );
								break;

						case '2250 (1967)':
								commodityOrTable.push( '2 (54)' );
								break;

						case 'Chemical':
								commodityOrTable.push( 'Chemical 1' );
								commodityOrTable.push( 'Chemical 2' );
								break;

						case '2249 (1980) Table':
								commodityOrTable.push( '54A/53A' );
								commodityOrTable.push( '54B/53B' );
								commodityOrTable.push( '54D' );
								break;


						default:
								break;
					}
					break;

				case 'GB/T':
					commodityOrTable.push( '60A/59A' );
					commodityOrTable.push( '60B/59B' );
					commodityOrTable.push( '60D/59D' );
					break;

				case 'GOST':
					commodityOrTable.push( '3900-85' );
					break;

				default:
					break;
		}

		return commodityOrTable;
	};
	var GetStandardTemperatures = function( organization, standardAndRevision, commodityOrTable )
	{
		var standardTemperatureList = [];
		switch ( commodityOrTable )
		{
			case 'None':
				if (organization === 'JIS') {
					standardTemperatureList.push('15 °C');
				}
				else {
					standardTemperatureList.push('60 °F');
					standardTemperatureList.push('15 °C');
				}

				break;

			case '54A/53A':
			case '54B/53B':
			case '54C':
			case '54D':
				if ( standardAndRevision === '2249Z (1980)' )
				{
					standardTemperatureList.push( '20 °C' );
				}
				else
				{
					standardTemperatureList.push( '15 °C' );
					standardTemperatureList.push( '30 °C' );
				}
				break;

			case '60A/59A':
			case '60B/59B':
			case '60D/59D':
				standardTemperatureList.push( '20 °C' );
				break;


			case '6A/5A':
			case '6B/5B':
			case '6C':
			case '6D':
			case '24E/23E':
				standardTemperatureList.push( '60 °F' );
				break;

			case 'Alpha 60 Supplied':
			case 'Crude Oils':
			case 'Lubrication Oils':
			case 'Refined Products':
				standardTemperatureList.push('60 °F');
				standardTemperatureList.push('15 °C');
				standardTemperatureList.push('20 °C');
				break;


			case 'LPG':
				standardTemperatureList.push( '15 °C' );
				break;

			case 'Asphalt':
				standardTemperatureList.push( '15 °C' );
				standardTemperatureList.push( '60 °F' );
				break;

			case 'Benzene':
			case 'Toluene':
			case 'Mixed Xylene':
			case 'Styrene':
			case 'o-Xylene':
			case 'p-Xylene':
			case 'Cylco-Hexane':
			case 'Ethyl-Benzene':
			case 'Cumene':
			case '300 °F/148.9 °C Aromatic':
			case '350 °F /176.7 °C Aromatic':
				standardTemperatureList.push( '15 °C' );
				standardTemperatureList.push( '60 °F' );
				break;

			case 'K-Factors':
				standardTemperatureList.push( '°F' );
				break;

			case '2 (54)':
				standardTemperatureList.push( '15 °C' );
				break;

			case '54A (6X)':
			case '54B (6X)':
				standardTemperatureList.push( '°C' );
				break;

			case '55':
				standardTemperatureList.push( '15 °C' );
				break;

			case 'Chemical 1':
				standardTemperatureList.push( '15 °C' );
				break;

			case 'Chemical 2':
				standardTemperatureList.push( '20 °C' );
				break;

			case '3900-85':
				standardTemperatureList.push( '20 °C' );
				break;

			default:
				break;
		}

		return standardTemperatureList;
	};

	var _EnableDisableDropDowns = function()
	{
		var currentStandardsOranization = $('#StandardOrganizationDropDownList').val();
		var dropDownsDisabled = false;
		if (currentStandardsOranization === 'None') {
			dropDownsDisabled = true;
		}
		document.getElementById('StandardAndRevisionDropDownList').disabled = dropDownsDisabled;
		document.getElementById('CommodityOrTableDropDownList').disabled = dropDownsDisabled;
		document.getElementById('StandardTemperatureDropDownList').disabled = dropDownsDisabled;
	}

	var _StandardOrganizationDropDownListChanged = function()
	{
		$('[name=StandardsOrganization]').val($('#StandardOrganizationDropDownList').val());
		var standardsAndRevisions = GetStandardsAndRevisions( $( '[name=StandardsOrganization]' ).val() );
		var current = $('[name=StandardAndRevision]').val();

		var items = '';
		$.each( standardsAndRevisions, function( i, standardAndRevision )
		{
				var selected = ( standardAndRevision === current ) ? 'selected' : '';
				items += '<option ' + selected + ' value=\'' + standardAndRevision + '\'>' + standardAndRevision + '</option>';
		} );
		$( '#StandardAndRevisionDropDownList' ).empty().html( items );

		$( '[name=StandardAndRevision]' ).val( $( '#StandardAndRevisionDropDownList' ).val() );

		_EnableDisableDropDowns();
		_StandardAndRevisionDropDownListChanged();
	};


	var _StandardAndRevisionDropDownListChanged = function()
	{
		var standardAndRevision = $( '#StandardAndRevisionDropDownList' ).val();
		$( '[name=StandardAndRevision]' ).val( standardAndRevision );

		if ( standardAndRevision === 'Commodity (2004)' )
		{
				$( '#AlternateTemperatureTextBox' ).prop( 'readonly', false );
				$( '#AlternateBasePressureTextBox' ).prop( 'readonly', false );
		}
		else
		{
				$( '#AlternateTemperatureTextBox' ).prop( 'readonly', true ).val( 0 );
				$( '#AlternateBasePressureTextBox' ).prop( 'readonly', true ).val( 0 );
		}

		var commoditiesAndTables = GetCommoditiesOrTables( $( '[name=StandardsOrganization]' ).val(), $( '[name=StandardAndRevision]' ).val() );
		var current = $( '[name=CommodityOrTable]' ).val();

		var items = '';
		$.each( commoditiesAndTables, function( i, commodityOrTable )
		{
				var selected = ( commodityOrTable === current ) ? 'selected' : '';
				items += '<option ' + selected + ' value=\'' + commodityOrTable + '\'>' + commodityOrTable + '</option>';
		} );
		$( '#CommodityOrTableDropDownList' ).empty().html( items );

		$( '[name=CommodityOrTable]' ).val( $( '#CommodityOrTableDropDownList' ).val() );

		_CommodityOrTableDropDownListChanged();
	};
	_CommodityOrTableDropDownListChanged = function()
	{
		var commodityOrTable = $( '#CommodityOrTableDropDownList' ).val();
		$( '[name=CommodityOrTable]' ).val( commodityOrTable );

		if ( commodityOrTable === 'K-Factors' )
		{
				$( '.kfactors' ).prop( 'readonly', false );
		}
		else
		{
				$( '.kfactors' ).prop( 'readonly', true ).val( 0 );
		}

		if ( commodityOrTable === 'Alpha 60 Supplied'
				|| commodityOrTable === '6C'
				|| commodityOrTable === '54C' )
		{
				$( '#AlphaTextBox' ).prop( 'readonly', false );
		}
		else
		{
				$( '#AlphaTextBox' ).prop( 'readonly', true ).val( 0 );
		}

		var standardTemperatures = GetStandardTemperatures($('[name=StandardsOrganization]').val(), $('[name=StandardAndRevision]').val(), $('[name=CommodityOrTable]').val());
		var current = $( '[name=StandardTemperature]' ).val();

		var items = '';
		$.each( standardTemperatures, function( i, standardTemperature )
		{
				var selected = ( standardTemperature === current ) ? 'selected' : '';
				items += '<option ' + selected + ' value=\'' + standardTemperature + '\'>' + standardTemperature + '</option>';
		} );
		$( '#StandardTemperatureDropDownList' ).empty().html( items );

		$( '[name=StandardTemperature]' ).val( $( '#StandardTemperatureDropDownList' ).val() );

		_StandardTemperatureDropDownListChanged();
	};
	_StandardTemperatureDropDownListChanged = function()
	{
		var standardTemp = $( '#StandardTemperatureDropDownList' ).val();
		$( '[name=StandardTemperature]' ).val( standardTemp );
		if ( standardTemp == null )
		{
				standardTemp = '60 °F';
		}

		var standardTempDisabled = true;
		var temperatureUnits = standardTemp;
		if ( standardTemp.length > 2 )
		{
				var res = standardTemp.split( ' ' );
				standardTemp = res[0];
				temperatureUnits = res[1];
		}
		else
		{
				standardTempDisabled = false;
				if ( temperatureUnits === '°C' )
				{
					standardTemp = '15';
				}
				else
				{
					standardTemp = '60';
				}
		}

		var standardTempText = $( '#StandardTemperatureLabel' ).text();
		standardTempText = standardTempText.substring( 0, standardTempText.length - 4 ) + '(' + temperatureUnits + ')';
		$( '#StandardTemperatureLabel' ).text( standardTempText );

		var alternateTempText = $( '#AlternateTemperatureLabel' ).text();
		alternateTempText = alternateTempText.substring( 0, alternateTempText.length - 4 ) + '(' + temperatureUnits + ')';
		$( '#AlternateTemperatureLabel' ).text( alternateTempText );
	};
	var _SaveChanges = function()
	{
		var url = $( '#urlSaveVcfSettings' ).val();
		var token = $( '#VcfSettingsEditorForm input[name=__RequestVerificationToken]' ).val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		var messageAttributes = { addclass: 'stack-bottomright', stack: FMVcfSettingsEditor.stack_bottomright_vcfsettings, width: '450px' };
		// remove any notification
		PNotify.removeStack( FMVcfSettingsEditor.stack_bottomright_vcfsettings );

		$.ajax( {
				url: url,
				type: 'post',
				headers: headers,
				data: $( '#VcfSettingsEditorForm' ).serialize(),
				success: function( result )
				{
					var standardOrganization = $( '#StandardOrganizationDropDownList' ).val();
					var standardAndRevision = $( '#StandardAndRevisionDropDownList' ).val();
					var commodityOrTable = $( '#CommodityOrTableDropDownList' ).val();
					var standardTemp = $( '#StandardTemperatureDropDownList' ).val();

					var title = standardOrganization + ' ' + standardAndRevision + ' ' + commodityOrTable + ( ( standardTemp === null ) ? '' : ' ' + standardTemp );
					$( '#PMEEditPropertyVcfModule' ).prop( 'title', title );
					FMErrorAndExceptionHandling.HandleMessages( result,
						function( data, inError )
						{
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
	return {
		StandardOrganizationDropDownListChanged: _StandardOrganizationDropDownListChanged,
		StandardAndRevisionDropDownListChanged: _StandardAndRevisionDropDownListChanged,
		CommodityOrTableDropDownListChanged: _CommodityOrTableDropDownListChanged,
		StandardTemperatureDropDownListChanged: _StandardTemperatureDropDownListChanged,
		SaveChanges: _SaveChanges,
		stack_bottomright_vcfsettings: _stack_bottomright_vcfsettings

	};
}();

// manually hookup to the submit the form
$( function()
{
	$( '#VcfSettingsEditorForm' ).submit( function()
	{
		var action = this.action;
		var method = this.method;

		FMVcfSettingsEditor.SaveChanges();

		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	} );
} );


//--------------------------------------- RUN after page has been loaded but before render -----------------------------

$( document ).ready( function()
{
	$( '#StandardOrganizationDropDownList' ).change( function()
	{
		FMVcfSettingsEditor.StandardOrganizationDropDownListChanged();
	} );

	$( '#StandardAndRevisionDropDownList' ).change( function()
	{
		FMVcfSettingsEditor.StandardAndRevisionDropDownListChanged();
	} );

	$( '#CommodityOrTableDropDownList' ).change( function()
	{
		FMVcfSettingsEditor.CommodityOrTableDropDownListChanged();
	} );

	$( '#StandardTemperatureDropDownList' ).change( function()
	{
		FMVcfSettingsEditor.StandardTemperatureDropDownListChanged();
	} );

	FMVcfSettingsEditor.StandardOrganizationDropDownListChanged();

	// Hide the Header, as VcfSettingsEditor provides one
	$('.modal-header').hide();

	if($('#Readonly').val() === 'True')
	{
		$('#PEMPESavePropertyScreen').attr('disabled',true);
	}

	FMErrorAndExceptionHandling.CloseNotifications();
} );