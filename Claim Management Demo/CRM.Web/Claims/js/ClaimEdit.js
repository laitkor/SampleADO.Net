myApp.ClaimEdit = myApp.ClaimEdit || {};

//#region Client Info
myApp.ClaimEdit = {
    Init: $(function () {
        $(document).on("click", 'input[name="ClaimDiarySch.Periodicity"]:checked', function () {
            switch ($(this).val()) {
                case "Daily":
                    $('#lblRecurEvery').text('day');
                    break;
                case "Weekly":
                    $('#lblRecurEvery').text('week(s)');
                    break;
                case "Monthly":
                    $('#lblRecurEvery').text('month(s)');
                    break;
                case "Yearly":
                    $('#lblRecurEvery').text('year(s)');
                    break;
                default:
                    $('#lblRecurEvery').text('');
            }
            $('#lblRecurEvery')
        });


        //Diary open check box check/uncheck
        $(document).on("click", '#chkHeader', function () {
            $('#chkHeader').closest('table').find('tbody input[type=checkbox]').prop('checked', $('#chkHeader').is(':checked'))
        })
        //prevent the form from submitting in a non-Ajax manner for all browsers
        $(document).on("submit", "form#FormDiaries, form#FormDiariesDelete", function (event) {

            eval($(this).attr("submit")); return false;
        });
        // set values diary complete form submit
        $(document).on("click", "form#FormDiaries, form#FormDiariesDelete input[type=submit]", function () {
            debugger;
            $("#FormDiaries :input[id^='diaries']").remove();
            $("#FormDiariesDelete :input[id^='diaries']").remove();
            $("#FormDiaries :input[id^='Stusdiaries']").remove();
            $("#FormDiariesDelete :input[id^='Stusdiaries']").remove();
            $('#chkHeader').closest('table').find('tbody input[type=checkbox]:checked').each(function (index, el) {
                $('form#FormDiaries, form#FormDiariesDelete').append('<input id="diaries_' + index + '_" name="diaries[' + index + ']" type="hidden" value="' + $(el).val() + '">');
                $('form#FormDiaries, form#FormDiariesDelete').append('<input id="Stusdiaries_' + index + '_" name="Stusdiaries[' + index + ']" type="hidden" value="' + $(this).attr("statusname") + '">');
            });
        });

        $("div[id^='lista'].als-container").als({
            visible_items: 10,
            scrolling_items: 1,
            orientation: "horizontal",
            circular: "no",
            autoscroll: "no",
            speed: 500,
            easing: "linear",
            direction: "right",
            start_from: 0
        });

        $('#menu-toggle').children().removeClass("fa fa-chevron-right");
        $('#menu-toggle').children().addClass("fa fa-chevron-left");

        $("#menu-toggle").click(function (e) {
            e.preventDefault();
            $("#left-wrapper").toggleClass("toggled");
            if ($('#menu-toggle').children().attr('class') == "fa fa-chevron-right") {
                $('#menu-toggle').children().removeClass("fa fa-chevron-right");
                $('#menu-toggle').children().addClass("fa fa-chevron-left");
            } else {
                $('#menu-toggle').children().removeClass("fa fa-chevron-left");
                $('#menu-toggle').children().addClass("fa fa-chevron-right");
            }
        });

        //Claim Close Click event
        $(document).on("click", 'a#ClaimClose[disabled!="disabled"]', function () {
            var claimId = $('input:hidden#ClaimID').val();
            var claimTypeId = $('input:hidden#ClaimTypeID').val();
            var obj = new myApp.ClaimEdit.ClaimClose(claimId, claimTypeId);
            obj.ClaimClose();
        })
        //Claim Reopen Click event
        $(document).on("click", 'a#ClaimReopen[disabled!="disabled"]', function () {
            var claimId = $('input:hidden#ClaimID').val();
            var claimTypeId = $('input:hidden#ClaimTypeID').val();
            var obj = new myApp.ClaimEdit.ClaimClose(claimId, claimTypeId);
            obj.ClaimReopened();
        });

        $(document).on('ready', function (e) {
            var obj = new myApp.ClaimEdit.ClaimClose($('input:hidden#ClaimID').val(), $('input:hidden#ClaimTypeID').val());
            obj.DisableControlsOnClaimClose();
        })

        //click event add special permission users
        $(document).on('click', 'button#btnAddUsers', function () {
            var userIds = $('#tblClaimPermissionUsers tbody td:first-child input:checkbox:checked').next('input:hidden#UserId').map(function () { return $(this).val(); }).get();
            if (userIds.length > 0) {
                var data = { userIds: userIds, claimId: $('#ClaimID').val() };
                myApp.ClaimEdit.AddUsersClaimAccessPermission(data);
            }
            else {
                mvcNotify.displayMessage("Please select at least one user.", 'warning');
            }
        });

        $(document).on('click', 'a#btnDeletePermissionUser', function () {
            var id = $(this).next('input:hidden').val();
            bootbox.confirm('Do you wish to delete it?', function (result) {
                if (result) {
                    var data = { id: id, claimId: $('#ClaimID').val() };
                    myApp.ClaimEdit.DeletePermissionUser(data);
                }
            })
        });

    }),
    DisableRecurrenceDiary: function () {
        if ($('#hdnRecurrenceDisabled').val() == "False") {
            $('input[name="Recurrence"]').prop('disabled', true);
            $('<input>', {
                type: 'hidden',
                id: 'Recurrence',
                name: 'Recurrence',
                value: '' + $('input[name="Recurrence"]:checked').val() + ''
            }).appendTo('#formeditdiary');

            $('input[name="Periodicity"]').prop('disabled', true);
            $('<input>', {
                type: 'hidden',
                id: 'Periodicity',
                name: 'Periodicity',
                value: '' + $('input[name="Periodicity"]:checked').val() + ''
            }).appendTo('#formeditdiary');

            $('input[name="RecursEvery"]').prop('disabled', true);
            $('<input>', {
                type: 'hidden',
                id: 'RecursEvery',
                name: 'RecursEvery',
                value: '' + $('input[name="RecursEvery"]').val() + ''
            }).appendTo('#formeditdiary');

            $('input[name="EndBy"]').prop('disabled', true);
            $('<input>', {
                type: 'hidden',
                id: 'EndBy',
                name: 'EndBy',
                value: '' + $('input[name="EndBy"]:checked').val() + ''
            }).appendTo('#formeditdiary');

            $('input[name="EndOccurences"]').prop('disabled', true);
            $('<input>', {
                type: 'hidden',
                id: 'EndOccurences',
                name: 'EndOccurences',
                value: '' + $('input[name="EndOccurences"]').val() + ''
            }).appendTo('#formeditdiary');

            $('input[name="EndDate"]').prop('disabled', true);
            $('<input>', {
                type: 'hidden',
                id: 'EndDate',
                name: 'EndDate',
                value: '' + $('input[name="EndDate"]').val() + ''
            }).appendTo('#formeditdiary');
        }
        else
            myApp.ClaimEdit.DiaryRecurrenceEnableDisable('#EndBy');
    },
    UpdateProgress: function UpdateProgress(claimId, claimDiaryId) {
        $.ajax({
            url: "/ClaimEdit/Progressbar/",
            type: "GET",
            data: { claimId: claimId, claimDiaryId: claimDiaryId },
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                var activeIndex = parseInt($(data).filter('input#hdnActiveIndex').val()) || 0;
                $('#PartailProgressBar').html('');
                $('#PartailProgressBar').html(data);
                $("div[id^='lista'].als-container").als({
                    visible_items: 10,
                    scrolling_items: 1,
                    orientation: "horizontal",
                    circular: "no",
                    autoscroll: "no",
                    speed: 500,
                    easing: "linear",
                    direction: "right",
                    start_from: activeIndex
                });
            }
        });
    },
    CheckReqOr: function (obj1, obj2) {
        debugger;
        $("#DiaryStatus").val($("#DiaryStatusID option:selected").text());
        if ($('#formeditdiary').valid()) {
            var val1 = $(obj1).val();
            var val2 = $(obj2).val();
            if (val1.trim() && val2.trim()) {
                mvcNotify.displayMessage('Only one field is required between Due In Days and Due In Hrs.', 'Warning');
                //bootbox.alert();
                $(obj1).focus();
                return false;
            }
            else if (!val1.trim() && !val2.trim()) {
                mvcNotify.displayMessage('The field Due In Days Or Due In Hrs must be a required.', 'Warning');
                //bootbox.alert('');
                $(obj1).focus();
                return false;
            }
            if ($('input[name=Recurrence]:checked').val() == "True") {
                if ($('input[name=EndBy]:checked').val() == "After") {
                    if ($('#ClaimDiarySch_EndOccurences').val() == "") {
                        mvcNotify.displayMessage('Range of recurrences End After required.', 'Warning');
                        $('#ClaimDiarySch_EndOccurences').focus();
                        return false;
                    }
                }
                if ($('input[name=EndBy]:checked').val() == "Date") {
                    if ($('#ClaimDiarySch_EndDate').val() == "") {
                        mvcNotify.displayMessage('Range of recurrences End By required.', 'Warning');
                        $('#ClaimDiarySch_EndDate').focus();
                        return false;
                    }
                }
            }
            return true;
        }
    },
    CalculateDueDate: function (obj) {
        var dueInDays = $('#DueInDays');
        var dueInHrs = $('#DueInHrs');
        var claimId = $('#ClaimID').val();
        var claimdryId = $('#formeditdiary #ClaimDiaryID').val();
        if (obj == 'Days')
            dueInHrs.val('');
        else if (obj == 'Hrs')
            dueInDays.val('');
        if (dueInDays.val() || dueInHrs.val()) {
            var hrs = ((parseInt(dueInDays.val()) || 0) * 24) + (parseInt(dueInHrs.val()) || 0);
            $.ajax({
                url: '/ClaimEdit/CalulateDueDate',
                dataType: "json",
                data: { hrs: hrs, claimId: claimId, ClaimDiaryID: claimdryId },
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    $('#DueDate').val(data);

                }
            });
        }
    },
    DiaryRecurrenceEnableDisable: function (ele) {
        //if ($(ele).is(':checked')) {

        if ($(ele).val() == "After") {
            $('input[name="EndBy"][value="After"]').prop('checked', true);
            $('#EndDate').prop('disabled', true);
            $('#EndOccurences').prop('disabled', false);
            $('#EndDate').val('');
        }
        else {
            $('input[name="EndBy"][value="After"]').prop('checked', false);
            $('#EndDate').prop('disabled', false);
            $('#EndOccurences').prop('disabled', true);
            $('#EndOccurences').val('');
        }
        //}
    },
    MarkCompleted: function () {
        if ($('#tblCompleted tbody input[type=checkbox]:checked').length <= 0) {
            mvcNotify.displayMessage("Please select at least one diary.", 'Warning');
            return false;
        }
    },
    SetDiaryNotesContent: function (data) {
        debugger;
        $('#ClaimDiaryID').val(data.claimDiaryId);
        var textArea = $('#PopupDiaryNotes').find('textarea[id^="DiaryNotes"]');
        tinymce.get(textArea.prop('id')).setContent(data.diaryNotes);
        $('#PopupDiaryNotes').modal('show');
    },
    SetBillReviewNotesContent: function (data) {
        debugger;
        $('#BillId').val(data.BillId);
        var textArea = $('#PopupBillNotes').find('textarea[id^="BillReviewNotes"]');
        tinymce.get(textArea.prop('id')).setContent(data.BillNotes);
        $('#PopupBillNotes').modal('show');
    },
    OnSuccessBillReviewNotes: function (data) {
        debugger;
        $('#BillId').val('0');
        var textArea = $('#PopupBillNotes').find('textarea[id^="BillReviewNotes"]');
        tinymce.get(textArea.prop('id')).setContent('');
        $('#PopupBillNotes').modal('hide');
        $('#dvNotesContent_' + data.id).html(data.notes.slice(0, 100));
    },
    OnSuccessDiaryNotes: function (data) {
        debugger;
        $('#ClaimDiaryID').val('0');
        var textArea = $('#PopupDiaryNotes').find('textarea[id^="DiaryNotes"]');
        tinymce.get(textArea.prop('id')).setContent('');
        $('#PopupDiaryNotes').modal('hide');
        $('#dvNotesContent_' + data.id).html(data.notes.slice(0, 100));
    },
    SetDiaryNotesContentView: function (data) {
        var textArea = $('#PopupDiaryNotesView').find('textarea[id^="DiaryNotesView"]');
        tinymce.get(textArea.prop('id')).setContent(data.diaryNotes);
        $('#PopupDiaryNotesView').modal('show');
    },
    AddRecipients: function () {
        debugger
        myApp.Common.ClosePopup('PopupClaimEdit3');
        var recipients = $('#tblRecipeints tbody td input:checkbox:checked').map(function () { return this.value; }).get().join(',');
        $('#txtRecipeints').val(recipients);
        myApp.Common.OpenPopup('PopupClaimEdit2');
    },
    AddReportRecipients: function () {
        myApp.Common.ClosePopup('PopupClaimEdit3');
        var recipients = $('#tblRecipeints tbody td input:checkbox:checked').map(function () { return this.value; }).get().join(',');
        $('#txtRecipeints').val(recipients);
        // myApp.Common.OpenPopup('PopupClaimEdit2');
        $('body').addClass('modal-open');
    },
    AddRecipientsNew: function () {
        //myApp.Common.ClosePopup('PopupClaimEdit3');
        var recipients = $('#tbl tbody td input:checkbox:checked').map(function () { return this.value; }).get().join(',');
        $('#claimId').val(recipients);
        if (recipients != "") {
            var AdjusterDetail = $('#AdjusterDetail option:selected').val();
            $('#adjusterId').val(AdjusterDetail);
            if (AdjusterDetail == "") {
                // alert('Please Select Adjuster Id');
                mvcNotify.displayMessage('Please Select Adjuster Id', 'Warning');

            }
            $('#cl').val(1);

        }
        else {
            //alert(' Please Select Claim Id');
            mvcNotify.displayMessage('Please Select Claim Id', 'Warning');
        }
        //myApp.Common.OpenPopup('PopupClaimEdit2');
    },

    AddRecipientsDiary: function () {
        myApp.Common.ClosePopup('PopupClaimEdit3');
        var recipients = $('#tblRecipeints tbody td input:checkbox:checked').map(function () { return this.value; }).get().join(',');
        $('#EmailRecepients').val(recipients);
    },
    OnSuccessAdjusterLetterAcknowledge: function (data) {
        debugger
        $('#AdjusterUserID').val(data.userId);
        $('#UserName').val(data.userName);

        $('#PartailProgressBar').html('');
        $('#PartailProgressBar').html(data.progressResult);
        var activeIndex = parseInt($(data.progressResult).filter('input#hdnActiveIndex').val()) || 0;
        $("div[id^='lista'].als-container").als({
            visible_items: 10,
            scrolling_items: 1,
            orientation: "horizontal",
            circular: "no",
            autoscroll: "no",
            speed: 500,
            easing: "linear",
            direction: "right",
            start_from: activeIndex
        });
        myApp.Common.ClosePopup('PopupClaimEdit2');
        // mvcNotify.displayMessage("Adjuster assigned successfully.", "success");----Coment By Khalda

        //----------Add new variable responseResult in response of ajax(by Khalda)----------
        if (data.responseResult == "Adjuster assigned successfully.") {
            mvcNotify.displayMessage("Adjuster assigned successfully.", "success");
            $.event.trigger({ type: "adjusterAssigned" });
        }
        else {
            mvcNotify.displayMessage(data.responseResult, "error");
        }
        //----------------------------------------------------------------------------------
    },
    TodayHoliday: function (claimIds) {
        var iscreate = true;
        $.ajax({
            url: '/ClaimEdit/IsTodayHoliday/',
            dataType: "json",
            data: { claimId: claimIds },
            contentType: 'application/json; charset=utf-8',
            async: false,
            success: function (data) {

                if (data.IsTodayHolidays == false) {
                    alert("You can't create diary today");
                    iscreate = false;
                }
            }
        });

        return iscreate;

    }, CalculateDaybydate: function (obj) {
        var dueInDays = $('#DueInDays');
        var dueInHrs = $('#DueInHrs');
        var duedates = $('#DueDate');
        var claimdryId = $('#formeditdiary #ClaimDiaryID').val();
        var iscreate = true;
        var claimId = $('#ClaimID').val();
        if (parseInt(claimdryId) == 0) {
            if (new Date(duedates.val()) <= new Date()) {
                alert(duedates.val() + " Date Not allowed");
                duedates.val('');
                dueInDays.val('');
                return false;
            }
        }

        $.ajax({
            url: '/ClaimEdit/CalculateDaybydates',
            dataType: "json",
            data: { dt1: duedates.val(), claimId: claimId, ClaimDiaryID: claimdryId },
            contentType: 'application/json; charset=utf-8',
            success: function (data) {

                if (data.CalculateDaybydates == false) {
                    alert(duedates.val() + " Date Not allowed! It may be holiday");
                    duedates.val('');
                    dueInDays.val('');
                    dueInHrs.val('');
                    iscreate = false;
                }
                else {

                    var startDay = new Date(duedates.val());
                    var endDay = new Date();
                    if (parseInt(claimdryId) > 0) {

                        endDay = new Date(parseInt(data.CalculateDaybydates.split("(")[1].split(")")[0]));

                    }


                    var dayDiff = Math.ceil((startDay - endDay) / (1000 * 60 * 60 * 24));

                    dayDiff = parseInt(dayDiff) - parseInt(data.day);

                    dueInHrs.val('');
                    dueInDays.val(dayDiff);
                }
            }
        });

        return iscreate;

    },
    ClaimClose: function (claimId, claimTypeId) {
        debugger;
        var claimId = claimId;
        var claimTypeId = claimTypeId;
        var claimClosed = false;
        var invoiceCreated = false;
        var invoiceApproved = false;

        var invoiceCreatedConfirm = function (result) {
            if (result) {
                //var data = { InvoiceCreated: true };
                //CheckInvoiceCreatedSuccess(data);
                changeClaimStatusWithReason();
            }
        };
        var invoiceApprovedConfirm = function (result) {
            if (result) {
                //var data = { InvoiceApproved: true };
                //CheckInvoiceCreatedSuccess(data);
                changeClaimStatusWithReason();
            }
        };
        var changeClaimStatusWithReason = function () {
            bootbox.dialog({
                title: claimClosed ? "Reopen Claim" : "Close Claim",
                message: '<div class="form-group">' +
                    '<label class="control-label" for="ClaimStatusReason">' +
                    'Reason: ' +
                    '</label> ' +
                    '' +
                    '<textarea id="ClaimStatusReason" name="ClaimStatusReason" type="text" class="form-control input-md" rows="3" cols="10"  style="resize:none" /> ',
                buttons: {
                    success: {
                        label: "Save",
                        className: "btn save btn-right",
                        callback: function () {
                            $.ajax({
                                url: '/ClaimEdit/ClaimClose/',
                                dataType: "json",
                                type: "POST",
                                data: JSON.stringify({ claimId: claimId, claimTypeId: claimTypeId, claimStatus: claimClosed, reason: $('textarea#ClaimStatusReason').val(), }),
                                contentType: 'application/json; charset=utf-8',
                                success: function (data) {
                                    if (data.diaryOpenCount > 0) {
                                        mvcNotify.displayMessage("There are Open Diaries that need to be completed prior to closing the claim.", 'warning');
                                    } else {

                                        if (data.claimTypeId == 1) {
                                            if (data.ClaimClosed == true) {
                                                mvcNotify.displayMessage("Claim Reopened Successfully.", 'success');
                                                $('#frmClaimDetails').find('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                                                $('#frmClaimDetails').find('input,select,a').not('a#ClaimReopen').not('a[class*="accordion-toggle"]').unbind('click', false).removeAttr('disabled');
                                            }
                                            else if (data.ClaimClosed == false) {
                                                mvcNotify.displayMessage("Claim Closed Successfully.", 'success');
                                                $('#frmClaimDetails').find('a#ClaimReopen').unbind('click', false).removeAttr('disabled');
                                                $('#frmClaimDetails').find('input,select').attr('disabled', 'disabled');
                                                $('#frmClaimDetails').find('a').not('a[class*="accordion-toggle"]').not('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                                            }
                                            else
                                                mvcNotify.displayMessage("Oops! Something went wrong.", 'error');
                                        }
                                        else if (data.claimTypeId == 2) {
                                            if (data.ClaimClosed == true) {
                                                mvcNotify.displayMessage("Claim Reopened Successfully.", 'success');
                                                $('#FormClaimWCDetail').find('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                                                $('#FormClaimWCDetail').find('input,select,a').not('a#ClaimReopen').not('a[class*="accordion-toggle"]').unbind('click', false).removeAttr('disabled');
                                            }
                                            else if (data.ClaimClosed == false) {

                                                mvcNotify.displayMessage("Claim Closed Successfully.", 'success');
                                                $('#FormClaimWCDetail').find('a#ClaimReopen').unbind('click', false).removeAttr('disabled');
                                                $('#FormClaimWCDetail').find('input,select').attr('disabled', 'disabled');
                                                $('#FormClaimWCDetail').find('a').not('a[class*="accordion-toggle"]').not('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                                            }
                                            else
                                                mvcNotify.displayMessage("Oops! Something went wrong.", 'error');
                                        }
                                        else if (data.claimTypeId == 3) {
                                            if (data.ClaimClosed == true) {
                                                mvcNotify.displayMessage("Claim Reopened Successfully.", 'success');
                                                $('#frmClaimDetails').find('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                                                $('#frmClaimDetails').find('input,select,a').not('a#ClaimReopen').not('a[class*="accordion-toggle"]').unbind('click', false).removeAttr('disabled');
                                                //$('[id*=ClaimantEditID]').each(function () { $(this).unbind('click', false).removeAttr('disabled'); });
                                            }
                                            else if (data.ClaimClosed == false) {
                                                // alert('hi')
                                                mvcNotify.displayMessage("Claim Closed Successfully.", 'success');
                                                $('#frmClaimDetails').find('a#ClaimReopen').unbind('click', false).removeAttr('disabled');
                                                $('#frmClaimDetails').find('input,select').attr('disabled', 'disabled');
                                                $('#frmClaimDetails').find('a').not('a[class*="accordion-toggle"]').not('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');

                                            }
                                            else
                                                mvcNotify.displayMessage("Oops! Something went wrong.", 'error');
                                        }
                                        else {
                                            mvcNotify.displayMessage("Oops! Something went wrong.", 'error');
                                        }



                                        //Update progress result
                                        $('#PartailProgressBar').html('');
                                        $('#ClaimOpenCloseStatus').html('');
                                        $('#ClaimOpenCloseStatus').html(data.ClaimOpenCloseStatus);
                                        $('#PartailProgressBar').html(data.progressResult);
                                        var activeIndex = parseInt($(data.progressResult).filter('input#hdnActiveIndex').val()) || 0;
                                        $("div[id^='lista'].als-container").als({
                                            visible_items: 10,
                                            scrolling_items: 1,
                                            orientation: "horizontal",
                                            circular: "no",
                                            autoscroll: "no",
                                            speed: 500,
                                            easing: "linear",
                                            direction: "right",
                                            start_from: activeIndex
                                        });
                                    }
                                }
                            });
                        }
                    },
                    cancel: { label: "Cancel", className: "btn cancel btn-left" }
                }
            });
        };
        var checkInvoiceCreatedSuccess = function (data) {
            if (data.ClaimClosed != undefined)
                claimClosed = data.ClaimClosed;
            if (data.InvoiceCreated != undefined)
                invoiceCreated = data.InvoiceCreated;
            if (data.InvoiceApproved != undefined)
                invoiceApproved = data.InvoiceApproved;
            if (data.Reserve != undefined)
                Reserve = data.Reserve;

            //if (Reserve == true) {
            //    //bootbox.confirm('Reserve Balance must be zero for closing claim.Do you still want to close the claim ?', invoiceCreatedConfirm);
            //    bootbox.confirm('Do you want to close the claim ?', invoiceCreatedConfirm);
            //}
            //else if (claimClosed == true) {
            //    bootbox.alert('Claim already closed.');
            //}
            //else {
            if (invoiceCreated == false) {
                bootbox.confirm("Do you wish to close this claim without creating invoice first?", invoiceCreatedConfirm);
            }
            else {
                if (invoiceApproved == false) {
                    bootbox.confirm("Do you wish to close this claim without approving the invoice for billing?", invoiceApprovedConfirm);
                }
                else {
                    changeClaimStatusWithReason();
                }
            }
            //}
        };
        var isCaimClosedSuccess = function (data) {
            if (data.ClaimClosed != undefined)
                claimClosed = data.ClaimClosed;
            if (claimClosed != true) {
                bootbox.alert('Claim already open.');
            }
            else {
                changeClaimStatusWithReason();
            }
        };
        this.ClaimClose = function () {
            $.ajax({
                url: '/ClaimEdit/CheckInvoiceCreated/',
                dataType: "json",
                data: { claimId: claimId },
                contentType: 'application/json; charset=utf-8',
                success: checkInvoiceCreatedSuccess
            });
        };
        this.ClaimReopened = function () {
            $.ajax({
                url: '/ClaimEdit/IsCaimClosed/',
                dataType: "json",
                data: { claimId: claimId },
                contentType: 'application/json; charset=utf-8',
                success: isCaimClosedSuccess
            });
        },
        this.DisableControlsOnClaimClose = function () {
            debugger;
            $.ajax({
                url: '/ClaimEdit/IsCaimClosed/',
                dataType: "json",
                data: { claimId: claimId },
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    debugger;
                    if (claimTypeId == 1) {
                        if (data.ClaimClosed) {
                            $('#ClaimPartialMain').find('input,select').attr('disabled', 'disabled');
                            $('#ClaimPartialMain').find('span[class*="input-group-addon"]').attr('style', 'display:none');
                            $('#ClaimPartialMain').find('a').not('a[class*="accordion-toggle"]').not('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                            $('#outstandingamt').html('$0.00');
                            // $('#lblpaid').html('$0.00');
                            if ($("#btmMarkComplete").attr("disabled", true)) {
                                $("#btmMarkComplete").attr("disabled", false);
                            }
                            // to unbind click in a tag use this tag in future
                            //.unbind('click', false);
                        }
                        else {
                            $('#ClaimPartialMain').find('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                            $('#ClaimPartialMain').find('a').not('a#ClaimReopen').not('a[class*="accordion-toggle"]').unbind('click', false).removeAttr('disabled');
                            $('#ClaimPartialMain').find('span[class*="input-group-addon"]').removeAttr('style');
                        }
                    }
                    else if (claimTypeId == 2) {
                        var divContainer = $('#ClaimPartialMain').closest('div.tab-content');
                        if (data.ClaimClosed) {
                            divContainer.find('input,select').attr('disabled', 'disabled');
                            divContainer.find('span[class*="input-group-addon"]').attr('style', 'display:none');
                            divContainer.find('a[class*="btn"]').attr('disabled', 'disabled');
                            //  divContainer.find('a').not('a[class*="accordion-toggle"]').not('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                            divContainer.find('a#ClaimReopen').attr('disabled', false);
                            $('#outstandingamt').html('$0.00');
                            //$('#lblpaid').html('$0.00');
                            if ($("#btmMarkComplete").attr("disabled", true)) {
                                $("#btmMarkComplete").attr("disabled", false);
                            }
                            // to unbind click in a tag use this tag in future
                            //.unbind('click', false);
                        }
                        else {
                            divContainer.find('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                            divContainer.find('a').not('a#ClaimReopen').not('a[class*="accordion-toggle"]').unbind('click', false).removeAttr('disabled');
                            divContainer.find('span[class*="input-group-addon"]').removeAttr('style');
                        }
                    }
                    else if (claimTypeId == 3) {
                        if (data.ClaimClosed) {
                            $('#ClaimPartialMain').find('input,select').attr('disabled', 'disabled');
                            $('#ClaimPartialMain').find('span[class*="input-group-addon"]').attr('style', 'display:none');
                            $('#ClaimPartialMain').find('a').not('a[class*="accordion-toggle"]').not('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');

                            $('[id*=ClaimantEditId]').each(function () {
                                $(this).unbind('click', false).removeAttr('disabled');
                            });
                            // to unbind click in a tag use this tag in future
                            //.unbind('click', false);
                            $('#outstandingamt').html('$0.00');
                            // $('#lblpaid').html('$0.00');
                            if ($("#btmMarkComplete").attr("disabled", true)) {
                                $("#btmMarkComplete").attr("disabled", false);
                            }
                        }
                        else {
                            $('#ClaimPartialMain').find('a#ClaimReopen').bind('click', false).attr('disabled', 'disabled');
                            $('#ClaimPartialMain').find('a').not('a#ClaimReopen').not('a[class*="accordion-toggle"]').unbind('click', false).removeAttr('disabled');
                            $('#ClaimPartialMain').find('span[class*="input-group-addon"]').removeAttr('style');
                        }
                    }
                }
            });
        }
        this.DisableControlsServerSide = function () {
            var claimStatus = $("#hfclaimstatus").val();
            if (claimStatus === "0") {
                mvcNotify.displayMessage('Can not save or update while claim is closed', 'Warning');
                $("#ClaimPartialMain").find("input,select").attr("disabled", "disabled");
                $("#ClaimPartialMain").find("a").not("a[class*=\"accordion-toggle\"]").not("a#ClaimReopen").bind("click", false).attr("disabled", "disabled");
                return false;
            }
            else {
                $("#ClaimPartialMain").find("a#ClaimReopen").bind("click", false).attr("disabled", "disabled");
                $("#ClaimPartialMain").find("a").not("a#ClaimReopen").not("a[class*=\"accordion-toggle\"]").unbind("click", false).removeAttr("disabled");
                return true;
            }
        }
    }, UpdateSideBar: function (claimId) {
        //alert('updateSideBar:'+claimId);
        var id = claimId;
        $.ajax({
            url: "/Claims/ClaimEdit/FillSideBar",
            type: "POST",
            data: { ids: id },
            success: function (data) {
                //alert(data);
                $('#sidebar-wrapper').empty();
                $('#sidebar-wrapper').html(data);


            }
        });

    },
    AddUsersClaimAccessPermission: function (json) {
        data = myApp.Common.AddAntiForgeryToken(json);
        $.ajax({
            url: "/ClaimEdit/AddUsersClaimAccessPermission",
            type: "POST",
            data: data,
            success: function (data) {
                mvcNotify.displayMessage("Users added successfully.", 'success');
                $('#partialClaimPermissionUsers').html(data);
                myApp.Common.ClosePopup('PopupClaimEdit');
            }
        });
    },
    DeletePermissionUser: function (data) {
        data = myApp.Common.AddAntiForgeryToken(data);
        $.ajax({
            url: "/ClaimEdit/DeletePermissionUser",
            type: "POST",
            data: data,
            success: function (data) {
                mvcNotify.displayMessage("User deleted successfully.", 'success');
                $('#partialClaimPermissionUsers').html(data);
            }
        });
    },
    Expensedefaultzero: function () {
        var cexpid = $('#ClientExpenseID').val();

        if (cexpid == 0) {
            $('#divqty').show();
            $('#divamt').hide();
            return false;
        }

        $.ajax({
            url: '/ClaimEdit/IsDefaultRateZero/',
            dataType: "json",
            data: { ClientExpenseID: cexpid },
            contentType: 'application/json; charset=utf-8',
            async: false,
            success: function (data) {
                debugger

                if (data.IsDefaultPercentage == true) {

                    //$('#divqty').hide();
                    //$('#divamt').show();
                    $('#divqty').css('display', 'none');
                    $('#divamt').css('display', 'block');

                    if ($('#ExpenseQty').val() == '') {
                        $('#ExpenseQty').val(0)
                    };


                }
                else {


                    if (data.IsDefaultPercentage != null && data.IsDefaultPercentage == true) {

                        $('#divamt').css('display', 'block');
                        $('#divqty').css('display', 'none');
                        if ($('#ExpenseQty').val() == '') {
                            $('#ExpenseQty').val(0)
                        };

                    } else {
                        $('#divqty').css('display', 'block');
                        $('#divamt').css('display', 'none');

                    }
                    //$('#divqty').show();
                    //$('#divamt').hide();

                }
            }
        });
    }
};
function BillingPayOverride(obj) {
    var dueInDays = $('#AdjCommOverride');
    var dueInHrs = $('#AdjCommFlatFeeOverride');

    if (obj == 'Commision')
        dueInHrs.val('');
    else if (obj == 'FlatFee')
        dueInDays.val('');
}

function ExpandCollaspeBill(id, link) {

    if ($(id).hasClass('accordion-body collapse in')) {
        $(link).find('i').removeClass('fa fa-minus');
        $(link).find('i').addClass('fa fa-plus');
    } else {
        $(link).find('i').removeClass('fa fa-plus');
        $(link).find('i').addClass('fa fa-minus');
    }
    return false;
}
function disbalebilling() {
    alert('ClaimNumber and PolicyNumber should be different from Adjuster ClaimNumber');
}

$(document).ready(function () {
    $("#billingTabAnchor").click(function () {
        var insurerClaimNumber = $("input[id=InsurerClaimNumber]").val();
        var policyNumber = $("input[id=PolicyNumber]").val();
        var adjFileNumber = $("input[id=AdjusterFile]").val();
        alert("insurerClaimNumber" + insurerClaimNumber + "::" + "policyNumber:" + policyNumber + ":: adjFileNumber" + adjFileNumber);
        if ((insurerClaimNumber == policyNumber) && (policyNumber == adjFileNumber)) {
            alert("ClaimNumber and PolicyNumber should be different from Adjuster ClaimNumber");
            return false;
        } else {
            return true;
        }
    });

    $("#ReportTo").hover(function () {
        $('#HoverReportTo').css({ opacity: 1 });
    }, function () {
        $('#HoverReportTo').css({ opacity: 0 });
    });
    $("#Insured").hover(function () {
        $('#HoverInsured').css({ opacity: 1 });
    }, function () {
        $('#HoverInsured').css({ opacity: 0 });
    });
    $("#Client").hover(function () {
        $('#HoverClient').css({ opacity: 1 });
    }, function () {
        $('#HoverClient').css({ opacity: 0 });
    });
    $("#Adjuster").hover(function () {
        $('#HoverAdjuster').css({ opacity: 1 });
    }, function () {
        $('#HoverAdjuster').css({ opacity: 0 });
    });
    $("#TypeOfLoss").hover(function () {
        $('#HoverTypeOfLoss').css({ opacity: 1 });
    }, function () {
        $('#HoverTypeOfLoss').css({ opacity: 0 });
    });

});

function calledFromAjaxSuccess(result) {
    if (result) {
        return true;

    } else {
        return false;
    }
}

function diffDays(d1, d2) {
    var ndays;
    var tv1 = d1.valueOf();  // msec since 1970
    var tv2 = d2.valueOf();

    ndays = (tv2 - tv1) / 1000 / 86400;
    ndays = Math.round(ndays - 0.5);
    return ndays;
}