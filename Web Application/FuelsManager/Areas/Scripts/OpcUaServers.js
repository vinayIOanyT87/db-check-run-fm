//--------------------------------------- RUN after page has been loaded but before render -----------------------------

// create a class with helper functions for the OpcUaServers view
var FMOpcUaServers = function()
{
    //set the position for the messages from the server
	var _stack_bottomright_opcuaeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 15, "firstpos2": 25, "context": $('#OPCEditorScreen') };

    var _selectionModeDropDownListChanged = function()
    {
        var url = $( '#urlSelectionModeSelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var selectionMode = $( '#SelectionModeDropDownList' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'selectionMode=' + selectionMode,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, function( domains, inError )
                {
                    if ( inError )
                    {
                        $( '#DomainDropDownList' ).empty();
                        $( '#ServerDropDownList' ).empty();

                        return;
                    }
                    var items;

                    $.each( domains, function( i, domain )
                    {
                        items += '<option value=\'' + domain + '\'>' + domain + '</option>';
                    } );

                    $( '#DomainDropDownList' ).empty().html( items );
                    $( '#ServerDropDownList' ).empty();
                    if ( selectionMode === 'Local' )
                    {
                        items = '';
                        items += '<option value=localhost>localhost</option>';
                        $( '#ServerDropDownList' ).empty().html( items );
                        _serverSelectionChanged();
                    }
                    $( '#OpcUaServerDropDownList' ).empty();
                    if ( $( '#OpcUaServerSecurityDropDownList' ).val() != null )
                    {
                        $( '#OpcUaServerSecurityDropDownList' ).empty();
                        _opcUaServerSecuritySelectionChanged();
                    }

                    _setSelectionModeControls();
                }, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'SelectionModeChanged failure' );
            }
        } );
    };
    var _domainSelectionChanged = function()
    {
        var url = $( '#urlDomainSelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var domain = $( '#DomainDropDownList' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'domain=' + domain,
            beforeSend: function()
            {
                $( '#ServerDropDownList' ).html( '<option> Loading...</option>' );
                $( '#OpcUaServerDropDownList' ).empty();
            },
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, function( servers, inError )
                {
                    if ( inError )
                    {
                        $( '#ServerDropDownList' ).empty().val( '' );
                        return;
                    }

                    var items = '';

                    $.each( servers, function( i, server )
                    {
                        items += '<option value=\'' + server + '\'>' + server + '</option>';
                    } );

                    if ( servers.length > 0 )
                    {
                        $( '#ServerDropDownList' ).empty().html( items );
                    }
                    else
                    {
                        $( '#ServerDropDownList' ).html( '<option>None</option>' );
                    }


                    if ( $( '#OpcUaServerSecurityDropDownList' ).val() != null )
                    {
                        $( '#OpcUaServerSecurityDropDownList' ).empty();
                        _opcUaServerSecuritySelectionChanged();
                    }
                }, notificationAttributes );
            },
            error: function( request, status, error )
            {
                $( '#ServerDropDownList' ).empty().val( '' );
                FMErrorAndExceptionHandling.ShowError( 'DomainSelectionChanged failure', null, notificationAttributes );
            }
        } );
    };
    var _serverSelectionChanged = function()
    {
        var url = $( '#urlServerSelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var selectionMode = $( '#SelectionModeDropDownList' ).val();
        var server;
        if ( selectionMode === 'Manual' )
        {
            server = $( '#ServerTextBox' ).val();
            if ( server === '' )
            {
                $( '#OpcUaServerDropDownList' ).hide();
                $( '#OpcUaServerTextBox' ).show();
            }
            else
            {
                $( '#OpcUaServerDropDownList' ).show();
                $( '#OpcUaServerTextBox' ).hide();
            }
        }
        else
        {
            server = $( '#ServerDropDownList' ).val();
        }

        var messageAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor, width: '450px' };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'server=' + server,
            beforeSend: function()
            {
                $( '#OpcUaServerDropDownList' ).html( '<option> Loading...</option>' );
            },
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response,
                    function( opcUaServers )
                    {
                        var items = '';

                        $.each( opcUaServers, function( i, opcUaServer )
                        {
                            items += '<option value=\'' + opcUaServer + '\'>' + opcUaServer + '</option>';
                        } );
                        if ( opcUaServers.length > 0 )
                        {
                            $( '#OpcUaServerDropDownList' ).empty().html( items );
                        }
                        else
                        {
                            $( '#OpcUaServerDropDownList' ).html( '<option>None</option>' );
                        }

                        if ( $( '#ServerSecurityPolicyDropDownList' ).val() != null )
                        {
                            $( '#ServerSecurityPolicyDropDownList' ).empty().val( '' );
                            _opcUaServerSecurityPolicySelectionChanged();
                        }
                    },
                    messageAttributes );
            },
            error: function( xhr, textStatus, error )
            {
                FMErrorAndExceptionHandling.ShowException( xhr,
                    textStatus,
                    error,
                    function()
                    {
                        $( '#OpcUaServerDropDownList' ).empty().val( '' );
                    }, messageAttributes );
            }
        } );
    };
    var _opcUaServerSelectionChanged = function()
    {
        var url = $( '#urlOpcUaServerSelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var selectionMode = $( '#SelectionModeDropDownList' ).val();
        var server = $( '#ServerTextBox' ).val();
        var opcUaServer;
        if ( selectionMode === 'Manual'
            && server === '' )
        {
            opcUaServer = $( '#OpcUaServerTextBox' ).val();
        }
        else
        {
            opcUaServer = $( '#OpcUaServerDropDownList' ).val();
        }

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'opcUaServer=' + opcUaServer,
            beforeSend: function()
            {
                $( '#OpcUaServerSecurityDropDownList' ).html( '<option> Loading...</option>' );
            },
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, function( opcUaServerSecurities, inError )
                {
                    if ( inError )
                    {
                        $( '#ServerDropDownList' ).empty().val( '' );
                        return;
                    }


                    if ( typeof opcUaServerSecurities === 'string' )
                    {
                        $( '#opcUaServersError' ).html( opcUaServerSecurities );
                        $( '#OpcUaServerSecurityDropDownList' ).empty().val( '' );
                        return;
                    }

                    var items = '';

                    $.each( opcUaServerSecurities, function( i, opcUaServerSecurity )
                    {
                        items += '<option value=\'' + opcUaServerSecurity + '\'>' + opcUaServerSecurity + '</option>';
                    } );

                    $( '#OpcUaServerSecurityDropDownList' ).empty().html( items );
                    _opcUaServerSecuritySelectionChanged();
                }, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowException( xhr,
                    status,
                    error,
                    function()
                    {
                        $( '#OpcUaServerSecurityDropDownList' ).empty().val( '' );
                    }, notificationAttributes );
            }
        } );
    };
    var _opcUaServerSecuritySelectionChanged = function()
    {
        var url = $( '#urlOpcUaServerSecuritySelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var opcUaServerSecurity = $( '#OpcUaServerSecurityDropDownList' ).val();

        var messageAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor, width: '450px' };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            type: 'POST',
            cache: false,
            headers: headers,
            data: 'opcUaServerSecurity=' + opcUaServerSecurity,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, function( opcUaServerSecurityOptions, inError )
                {
                    if ( !inError )
                    {
                        var endpointUrl = opcUaServerSecurityOptions[0];
                        $( '#EndpointUrlTextBox' ).val( endpointUrl ).attr( 'title', endpointUrl );

                        var items = '';
                        var securityModes = opcUaServerSecurityOptions[1];
                        var selectedSecurityMode = opcUaServerSecurityOptions[2];
                        $.each( securityModes, function( i, securityMode )
                        {
                            items += '<option value=\'' + securityMode.Value + '\'>' + securityMode.Key + '</option>';
                        } );

                        $( '#SecurityModeDropDownList' ).empty().html( items );
                        if ( selectedSecurityMode && selectedSecurityMode != '' )
                        {
                            $( '#SecurityModeDropDownList' ).val( selectedSecurityMode );
                        }

                        items = '';
                        var securityPolicies = opcUaServerSecurityOptions[3];
                        var selectedSecurityPolicy = opcUaServerSecurityOptions[4];
                        $.each( securityPolicies, function( i, securityPolicy )
                        {
                            items += '<option value=\'' + securityPolicy.Value + '\'>' + securityPolicy.Key + '</option>';
                        } );

                        $( '#SecurityPolicyDropDownList' ).empty().html( items );
                        if ( selectedSecurityPolicy && selectedSecurityPolicy != '' )
                        {
                            $( '#SecurityPolicyDropDownList' ).val( selectedSecurityPolicy );
                        }

                        items = '';
                        var messageEncodings = opcUaServerSecurityOptions[5];
                        var selectedMessageEncoding = opcUaServerSecurityOptions[6];
                        $.each( messageEncodings, function( i, messageEncoding )
                        {
                            items += '<option value=\'' + messageEncoding.Value + '\'>' + messageEncoding.Key + '</option>';
                        } );

                        $( '#MessageEncodingDropDownList' ).empty().html( items );
                        if ( selectedMessageEncoding && selectedMessageEncoding != '' )
                        {
                            $( '#MessageEncodingDropDownList' ).val( selectedMessageEncoding );
                        }
                    }
                }, messageAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'OpcUaServerSecuritySelectionChanged failure', null, messageAttributes );
            }
        } );
    };
    var _securityModeSelectionChanged = function()
    {
        var url = $( '#urlSecurityModeSelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var securityMode = $( '#SecurityModeDropDownList' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'securityMode=' + securityMode,
            beforeSend: function()
            {
                $( '#SecurityPolicyDropDownList' ).html( '<option> Loading...</option>' );
            },
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, function( securityPolicies, inError )
                {
                    if ( inError )
                    {
                        $( '#SecurityPolicyDropDownList' ).empty().val( '' );
                        return;
                    }

                    var items = '';
                    $.each( securityPolicies, function( i, securityPolicy )
                    {
                        items += '<option value=\'' + securityPolicy.Value + '\'>' + securityPolicy.Key + '</option>';
                    } );

                    $( '#SecurityPolicyDropDownList' ).empty().html( items );
                }, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'SecurityModeSelectionChanged failure', null, notificationAttributes );
                $( '#SecurityPolicyDropDownList' ).empty().val( '' );
            }
        } );
    };
    var _securityPolicySelectionChanged = function()
    {
        var url = $( '#urlSecurityPolicySelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var securityPolicy = $( '#SecurityPolicyDropDownList' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'securityPolicy=' + securityPolicy,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, null, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'SecurityPolicySelectionChanged failure', null, notificationAttributes );
            }
        } );
    };
    var _messageEncodingSelectionChanged = function()
    {
        var url = $( '#urlMessageEncodingSelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var messageEncoding = $( '#MessageEncodingDropDownList' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'messageEncoding=' + messageEncoding,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, null, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'MessageEncodingSelectionChanged failure', null, notificationAttributes );
            }
        } );
    };
    var _setSelectionModeControls = function()
    {
        var selectionMode = $( '#SelectionModeDropDownList' ).val();
        if ( selectionMode === 'Manual' )
        {
            $( '#ServerDropDownList' ).hide();
            $( '#OpcUaServerDropDownList' ).hide();
            $( '#ServerTextBox' ).show().val( '' );
            $( '#OpcUaServerTextBox' ).show().val( '' );
        }
        else
        {
            $( '#ServerDropDownList' ).show();
            $( '#OpcUaServerDropDownList' ).show();
            $( '#ServerTextBox' ).hide();
            $( '#OpcUaServerTextBox' ).hide();
        }
    };
    var _setUserIdentityControls = function()
    {
        var userTokenType = $( '#UserTokenTypeDropDownList' ).val();
        var label;
        if ( userTokenType === 'Certificate' )
        {
            label = $( '#TranslatedCertificate' ).val();
            $( '#UserNameTextBox' ).hide();
            $( '#UserPasswordTextBox' ).hide();
            $( '#CertificatePathTextBox' ).show();
            $( '#CertificatePasswordTextBox' ).show();
        }
        else
        {
            label = $( '#TranslatedUserName' ).val();
            $( '#UserNameTextBox' ).show();
            $( '#UserPasswordTextBox' ).show();
            $( '#CertificatePathTextBox' ).hide();
            $( '#CertificatePasswordTextBox' ).hide();
        }
        $( '#UserNameOrCertificatePathLabel' ).text( label );
    };
    var _userTokenTypeSelectionChanged = function()
    {
        var url = $( '#urlUserTokenTypeSelectionChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var userTokenType = $( '#UserTokenTypeDropDownList' ).val();
        _setUserIdentityControls();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'userTokenType=' + userTokenType,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, null, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'UserTokenTypeChanged failure', null, notificationAttributes );
            }
        } );
    };
    var _userNameChanged = function()
    {
        var url = $( '#urlUserNameChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var userName = $( '#UserNameTextBox' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'userName=' + userName,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, null, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'UserNameChanged failure', null, notificationAttributes );
            }
        } );
    };
    var _certificatePathChanged = function()
    {
        var url = $( '#urlCertificatePathChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var certificatePath = $( '#CertificatePathTextBox' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'certificatePath=' + certificatePath,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, null, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'SecurityPolicySelectionChanged failure', null, notificationAttributes );
            }
        } );
    };
    var _userPasswordChanged = function()
    {
        var url = $( '#urlUserPasswordChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var userPassword = $( '#UserPasswordTextBox' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'userPassword=' + userPassword,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, null, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'UserPasswordChanged failure', null, notificationAttributes );
            }
        } );
    };
    var _certificatePasswordChanged = function()
    {
        var url = $( '#urlCertificatePasswordChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var certificatePassword = $( '#CertificatePasswordTextBox' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'certificatePassword=' + certificatePassword,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, null, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'CertificatePasswordChanged failure', null, notificationAttributes );
            }
        } );
    };
    var _endpointChanged = function()
    {
        var url = $( '#urlEndpointChanged' ).val();
        var token = $( '#opcUaServersForm input[name=__RequestVerificationToken]' ).val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        var endpointUrl = $( '#EndpointUrlTextBox' ).val();

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack( FMOpcUaServers.stack_bottomright_opcuaeditor );

        $.ajax( {
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: 'endpointUrl=' + endpointUrl,
            success: function( response )
            {
                FMErrorAndExceptionHandling.HandleMessages( response, null, notificationAttributes );
            },
            error: function( request, status, error )
            {
                FMErrorAndExceptionHandling.ShowError( 'EndpointChanged  failure', null, notificationAttributes );
            }
        } );
    };
    var _close = function () {
        var url = $('#urlClose').val();
        var token = $('#opcUaServersForm input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        // specify location for server notifications
        var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaServers.stack_bottomright_opcuaeditor };
        // remove any notification
        PNotify.removeStack(FMOpcUaServers.stack_bottomright_opcuaeditor);

        $.ajax({
            url: url,
            cache: false,
            type: 'POST',
            headers: headers,
            data: '',
            success: function (response) {
                FMErrorAndExceptionHandling.HandleMessages(response, null, notificationAttributes);
            },
            error: function (request, status, error) {
                FMErrorAndExceptionHandling.ShowError('Close failure', null, notificationAttributes);
            }
        });
    };
    return {
        selectionModeDropDownListChanged: _selectionModeDropDownListChanged,
        domainSelectionChanged: _domainSelectionChanged,
        serverSelectionChanged: _serverSelectionChanged,
        opcUaServerSelectionChanged: _opcUaServerSelectionChanged,
        opcUaServerSecuritySelectionChanged: _opcUaServerSecuritySelectionChanged,
        securityModeSelectionChanged: _securityModeSelectionChanged,
        securityPolicySelectionChanged: _securityPolicySelectionChanged,
        messageEncodingSelectionChanged: _messageEncodingSelectionChanged,
        setSelectionModeControls: _setSelectionModeControls,
        setUserIdentityControls: _setUserIdentityControls,
        userTokenTypeSelectionChanged: _userTokenTypeSelectionChanged,
        userNameChanged: _userNameChanged,
        certificatePathChanged: _certificatePathChanged,
        userPasswordChanged: _userPasswordChanged,
        certificatePasswordChanged: _certificatePasswordChanged,
        endpointChanged: _endpointChanged,
        close: _close,
        stack_bottomright_opcuaeditor: _stack_bottomright_opcuaeditor
    };
}();


$( document ).ready( function() {

    $( '#SelectionModeDropDownList' ).change( function()
    {
        FMOpcUaServers.selectionModeDropDownListChanged();
    } );

    $( '#DomainDropDownList' ).change( function()
    {
        FMOpcUaServers.domainSelectionChanged();
    } );

    $( '#ServerDropDownList' ).change( function()
    {
        FMOpcUaServers.serverSelectionChanged();
    } );

    $( '#OpcUaServerDropDownList' ).change( function()
    {
        FMOpcUaServers.opcUaServerSelectionChanged();
    } );

    $( '#OpcUaServerSecurityDropDownList' ).change( function()
    {
        FMOpcUaServers.opcUaServerSecuritySelectionChanged();
    } );

    $( '#SecurityModeDropDownList' ).change( function()
    {
        FMOpcUaServers.securityModeSelectionChanged();
    } );

    $( '#SecurityPolicyDropDownList' ).change( function()
    {
        FMOpcUaServers.securityPolicySelectionChanged();
    } );

    $( '#MessageEncodingDropDownList' ).change( function()
    {
        FMOpcUaServers.messageEncodingSelectionChanged();
    } );

    $( '#ServerTextBox' ).change( function()
    {
        FMOpcUaServers.serverSelectionChanged();
    } );

    $( '#OpcUaServerTextBox' ).change( function()
    {
        FMOpcUaServers.opcUaServerSelectionChanged();
    } );

    $( '#UserTokenTypeDropDownList' ).change( function()
    {
        FMOpcUaServers.userTokenTypeSelectionChanged();
    } );

    $( '#UserNameTextBox' ).change( function()
    {
        FMOpcUaServers.userNameChanged();
    } );

    $( '#CertificatePathTextBox' ).change( function()
    {
        FMOpcUaServers.certificatePathChanged();
    } );

    $( '#UserPasswordTextBox' ).change( function()
    {
        FMOpcUaServers.userPasswordChanged();
    } );

    $( '#CertificatePasswordTextBox' ).change( function()
    {
        FMOpcUaServers.certificatePasswordChanged();
    } );

    $( '#EndpointUrlTextBox' ).blur( function()
    {
        FMOpcUaServers.endpointChanged();
    } );


    FMOpcUaServers.setSelectionModeControls();
    FMOpcUaServers.setUserIdentityControls();

    $( '#UserPasswordTextBox' ).val( '*********' );

    $( '#CertificatePasswordTextBox' ).val( '*********' );

    $('#PEMPECloseOPCEditor').unbind().on('click', function () {
        FMOpcUaServers.close();
    });
/*
    $( '#OKButton' ).on( 'click', function()
    {
        var endpointUrl = $( '#EndpointUrlTextBox' ).val();
        $( '#ServerEndpointTextBox' ).val( endpointUrl ).attr( 'title', endpointUrl );
        FMOpcUaBrowser.serverEndpointChanged();
        $( '#OpcUaServerSelectionModalScreen' ).modal( 'hide' );
        $( '#OpcUaServerSelectionScreenBody' ).html( '' );
        $( '#OKButton' ).unbind( 'click' );
        $( '#CancelButton' ).unbind( 'click' );
    } );

    $( '#CancelButton' ).on( 'click', function()
    {
        $( '#OpcUaServerSelectionScreenBody' ).html( '' );
        $( '#OKButton' ).unbind( 'click' );
        $( '#CancelButton' ).unbind( 'click' );
    });
*/
    FMErrorAndExceptionHandling.CloseNotifications();
} );