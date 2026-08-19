var FMPointLeakAnalysis = function () {

    var _Init = function () {

        $('#PointLeakAnalysisScreen').on('shown.bs.modal', function () {
            FMPointLeakAnalysis.PrevHelpKey = window.parent.CurrentHelpKey;
            window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexLeakAnalysis";
        })

        $('#PointLeakAnalysisScreen').on('hidden.bs.modal', function () {
            window.parent.CurrentHelpKey = FMPointLeakAnalysis.PrevHelpKey;
            _ReportCleanup();
        })

        $('#RunLeakAnalysis').on('click', function () {
            _ReportCleanup();
            FMPointLeakAnalysis.RunAnalysisClicked();
        })

        $('#PrintPreview').on('click', function () {
            FMPointLeakAnalysis.PrintPreviewClicked();
        })

        $('.resultvalue').hide();
        $('.units').hide();
    };

    var _FinishPointLeakAnalysis = function (success) {
        if (success) {
            $('#PointLeakAnalysisScreen').modal('show');
        }
    };



    var _GetForm = function (url, pointIdString, pointGuidString) {
        FMPointLeakAnalysis.PointGuid = pointGuidString;
        FMPointLeakAnalysis.PointId = pointIdString;
        var callData = {
            pointIdString: pointIdString,
            pointGuidString: pointGuidString
        };


        FMErrorAndExceptionHandling.CloseNotifications();
        $('body').modalmanager('loading');

        var token = $('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        $.ajax({
            type: 'Post',
            url: url,
            dataType: 'json',
            data: JSON.stringify(callData),
            headers: headers,
            cache: false,
            success: function (response) {
                var modalManager = $('body').data('modalmanager');
                modalManager.removeLoading();
                FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
                    if (!inError) {
                        // replace the holder with the partial view
                        $("#PointLeakAnalysis").html(data);

                        FMPointLeakAnalysis.FinishPointLeakAnalysis(true);
                    }
                    else {
                        FMPointLeakAnalysis.FinishPointLeakAnalysis(false);
                    }
                });
            },
            error: function (xhr, textStatus, error) {

                var modalManager = $('body').data('modalmanager');
                modalManager.removeLoading();
                FMErrorAndExceptionHandling.ShowException(xhr,
                    textStatus,
                    error,
                    function () {
                        // remove the loading of the modal
                        FMPointLeakAnalysis.FinishPointLeakAnalysis(false);
                    });
            }
        });
    };

    var roundNumber = function (number, decimalPlaces) {
        if (isNaN(parseFloat(number))) return number;
        return parseFloat(number).toFixed(decimalPlaces);
    }

    var _formatElapsedTime = function (ticks) {
        ticks = Math.abs(ticks);
        var seconds = ticks / 10000000;
        var HH = Math.floor(seconds / 3600);
        var MM = Math.floor((seconds % 3600) / 60);
        var SS = Math.floor(seconds % 60);
        return ((HH < 10) ? ("0" + HH) : HH) + ":" + ((MM < 10) ? ("0" + MM) : MM) + ":" + ((SS < 10) ? ("0" + SS) : SS);
    }

    var _RunAnalysisClicked = function (url, pointIdString, pointGuidString) {
        var callData = {
            pointIdString: FMPointLeakAnalysis.PointId,
            pointGuidString: FMPointLeakAnalysis.PointGuid,
            startTimeString: $('#LeakAnalysisStartTime').val(),
            endTimeString: $('#LeakAnalysisEndTime').val()
        };

        FMErrorAndExceptionHandling.CloseNotifications();
        $('body').modalmanager('loading');

        var token = $('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        url = $('#urlPointLeakAnalysisRun').val();

        // remove previous notifications
        PNotify.removeStack();

        $.ajax({
            type: 'Post',
            url: url,
            dataType: 'json',
            data: JSON.stringify(callData),
            headers: headers,
            cache: false,
            success: function (response) {
                var modalManager = $('body').data('modalmanager');
                modalManager.removeLoading();
                FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
                        $('.resultvalue').css('display', 'inline-block');
                        $('.units').show();
                        $('#leakRate').text(roundNumber(data.LeakRate, data.LeakRatePrecision));
                        $('#minVolumeUsed').text(roundNumber(data.MinValue, data.VolumePrecision));
                        $('#maxVolumeUsed').text(roundNumber(data.MaxValue, data.VolumePrecision));
                        $('#elapsedTime').text(_formatElapsedTime(data.ReportTime));
                        $('#minTempUsed').text(roundNumber(data.MinTemperature, data.TemperaturePrecision));
                        $('#maxTempUsed').text(roundNumber(data.MaxTemperature, data.TemperaturePrecision));
                        $('#tempChange').text(roundNumber(data.GraphTemperatureDelta, data.TemperaturePrecision));
                        $('#testAnalysisStatus').html(data.TestResult + "<br>" + data.AnalysisStatusMessage.join("<br>"));
                        $('#LeakReportGuid').val(data.LeakRecordId);
                        $('#maxVolumeUsedUnits').text(FMConvertEngUnits.GetEngineeringUnitAbbreviation(data.VolumeUnits));
                        $('#mainVolumeUsedUnits').text(FMConvertEngUnits.GetEngineeringUnitAbbreviation(data.VolumeUnits));
                        $('#minTempUsedUnits').text(FMConvertEngUnits.GetEngineeringUnitAbbreviation(data.TemperatureUnits));
                        $('#maxTempUsedUnits').text(FMConvertEngUnits.GetEngineeringUnitAbbreviation(data.TemperatureUnits));
                        $('#tempChangeUnits').text(FMConvertEngUnits.GetEngineeringUnitAbbreviation(data.TemperatureUnits));
                        $('#leakRateUnits').text(FMConvertEngUnits.GetEngineeringUnitAbbreviation(data.LeakRateUnits));
                        if (data.EnableReportPrint) {
                            $("#PrintPreview").prop("disabled", false);
                        }
                        
                });
            },
            error: function (xhr, textStatus, error) {

                var modalManager = $('body').data('modalmanager');
                modalManager.removeLoading();
                FMErrorAndExceptionHandling.ShowException(xhr,
                    textStatus,
                    error,
                    function () {
                        // remove the loading of the modal
                        FMPointLeakAnalysis.FinishPointLeakAnalysis(false);
                        $("#PrintPreview").prop("disabled", true);
                    });
            }
        });
    };


    var _ReportCleanup = function (url, leakReportIdString) {

        leakReportIdString = $('#LeakReportGuid').val();
        if (leakReportIdString == '' || leakReportIdString == '00000000-0000-0000-0000-000000000000') {
            return;
        }
        var callData = {
            leakReportIdString: leakReportIdString
        };

        var token = $('input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;
        url = $('#urlPointLeakAnalysisReportCleanUp').val();

        // remove previous notifications
        PNotify.removeStack();
        
        $.ajax({
            type: 'Post',
            url: url,
            dataType: 'json',
            data: JSON.stringify(callData),
            headers: headers,
            cache: false,
            success: function (response) {
                
            },
            error: function (xhr, textStatus, error) {
            }
        });
    };

    var _PrintPreviewClicked = function () {
        var leakReportGuid = $('#LeakReportGuid').val();
        var leakReportName = $('#LeakReportName').val();

        if (leakReportGuid === undefined || leakReportGuid === "00000000-0000-0000-0000-000000000000") {
            return;
        }

        url = $('#urlReportViewer').val();

        // remove previous notifications
        PNotify.removeStack();

        url += "?ReportType=10";
        url += "&ReportName=" + leakReportName;
        url += "&LeakReportId=" + leakReportGuid;
        url += "&CSRFToken=" + window.csrfToken;

        window.open(url);
    };

    return {
        GetForm: _GetForm,
        RunAnalysisClicked: _RunAnalysisClicked,
        PrintPreviewClicked: _PrintPreviewClicked,
        Init: _Init,
        FinishPointLeakAnalysis: _FinishPointLeakAnalysis,
        PrevHelpKey: "",
        PointId: "",
        PointGuid: ""
    };
}();
