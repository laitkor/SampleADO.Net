
$(document).ready(function () {
    if ($('#btnSave').css('display') == 'none') {
        DisableClaimDetails();
    }

    //if (typeof insurerClaimNumber && typeof policyNumber && typeof adjFileNumber != "undefined")
    var insurerClaimNumber = $("input[id=InsurerClaimNumber]").val();
    var policyNumber = $("input[id=PolicyNumber]").val();
    var adjFileNumber = $("input[id=AdjusterFile]").val();

    if (typeof insurerClaimNumber && typeof policyNumber && typeof adjFileNumber != "undefined")
        if (insurerClaimNumber != policyNumber || policyNumber != adjFileNumber || adjFileNumber != insurerClaimNumber) {
            $("input[id=InsurerClaimNumber]").attr('class', 'valid post');

            $("input[id=PolicyNumber]").attr('class', 'valid post');

            $("input[id=AdjusterFile]").attr('class', 'valid post');

        }


    

});



function EnableClaimDetails() {

    $('#frmClaimDetails #InsurerClaimNumber').keyup(function () { return false; });
    $('#frmClaimDetails #PolicyNumber').keyup(function () { return false; });

    $("#btnSave").css("display", "block");
    $("#btnEdit").css("display", "none");
    $("#btnCancel").css("display", "block");
    $("#btnAddUser").css("display", "block");
    //$("#btnsearch").css("display", "block");
    $('#btnsearch').css('visibility', 'visible');
    $("#btnSearchAdjuster").css('visibility', 'visible');
    $('#frmClaimDetails .form-group .selectpicker-post').each(function () {
        $(this).removeClass("selectpicker-post");
        $(this).addClass("selectpicker");
        $(this).css("display", "block");
    });

    $('#frmClaimDetails .form-group :input[type=text]').each(function () {
        $(this).removeClass("post");
    });
    $('#frmClaimDetails .form-group :input[type=text]').attr("disabled", false);

    $('#Severity').each(function () {
        $(this).removeClass("post");
        $(this).addClass("text-box single-line");
    });
    $('#frmClaimDetails .form-group :input').attr("disabled", false);

    $('#frmClaimDetails .form-control').each(function () {

        $(this).removeClass("post");
        $(this).addClass("form-control datepicker post");
    });

    $('#frmClaimDetails .form-group select[class="post selectpicker"]').each(function () {
        $(this).removeClass("post");
    });

    $('#frmClaimDetails span[class=post]').each(function () {
        $(this).removeClass("post");
        $(this).addClass('input-group-addon');
    });
    $('#frmClaimDetails span[class=input-group-addon] i').each(function () {
        $(this).removeClass("post");
        $(this).addClass('fa fa-calendar');
    });
    $('#frmClaimDetails span[class=input-group-addon]').each(function () {
        $(this).removeAttr("style");
    });
    $("#Action").css("display", "block");
    $("#claimperm tbody tr").find("td:first").each(function () {
        $(this).show();
    });
    $("#claimperm thead tr").find("th:first").removeAttr('style')
    $('#frmClaimDetails .form-group div :input').each(function () {
        $(this).addClass("form-control");
    });
    $("input[id=DateEntered]").attr("disabled", true);
    var insurerClaimNumber = $("input[id=InsurerClaimNumber]").val();
    var policyNumber = $("input[id=PolicyNumber]").val();
    var adjFileNumber = $("input[id=AdjusterFile]").val();
    $("input[id=ClientName]").attr('class', 'valid post');
    if (typeof insurerClaimNumber && typeof policyNumber && typeof adjFileNumber != "undefined")
        if (insurerClaimNumber != policyNumber || policyNumber != adjFileNumber || adjFileNumber != insurerClaimNumber) {
            $("input[id=InsurerClaimNumber]").attr('class', 'valid post');

            $("input[id=PolicyNumber]").attr('class', 'valid post');

            $("input[id=AdjusterFile]").attr('class', 'valid post');

        }

    $('#frmClaimDetails input').not('.btn').each(function () {

        $(this).addClass("form-control");

    });
    $('#frmClaimDetails select').each(function () {

        $(this).addClass("form-control");

    });

    EnableCalendarClick($('#frmClaimDetails'));

}

function DisableClaimDetails() {

    $('#frmClaimDetails .form-group .selectpicker').each(function () {
        $(this).removeClass("selectpicker");
        $(this).addClass("selectpicker-post");
        $(this).removeClass("post");
        if ($(this).find(":selected").text() == '--select--') {
           // $(this).css("display", "none");
        }
    });
    $('#frmClaimDetails .form-group :input[type=text]').each(function () {
        $(this).addClass("post");
    });
    $('#frmClaimDetails .form-group :input[type=text]').attr("disabled", true);

    $('#Severity').each(function () {
        $(this).removeClass("text-box single-line");
        $(this).addClass("post");
    });
    $('#frmClaimDetails .form-group :input').attr("disabled", true);

    $('#frmClaimDetails .form-control').each(function () {
        $(this).removeClass("form-control datepicker post");
        $(this).addClass("post");
    });

    $('#frmClaimDetails .input-group-addon').each(function () {
        $(this).removeClass('input-group-addon');
        $(this).addClass("post");
    });

    $('#frmClaimDetails span[class=post] i').each(function () {
        $(this).removeClass('fa fa-calendar');
        $(this).addClass("post");
    });

    $("#frmClaimDetails .form-group span[class='text-danger field-validation-error']").each(function () {
        $(this).removeClass("text-danger field-validation-error");
        $(this).addClass("text-danger field-validation-valid");
        $(this).find("span").remove();
    });
    $("#claimperm tbody tr").find("td:first").each(function () {
        $(this).hide();
    });
    debugger
    $("#claimperm thead tr").find("th:first").removeAttr('style')
    $("#Action").css("display", "none");
    $("#btnEdit").attr("disabled", false);
    $("#btnAddUser").css("display", "none");
    //$("#btnsearch").css("display", "none");
    $('#btnsearch').css('visibility', 'hidden');
    $('#btnSearchAdjuster').css('visibility', 'hidden');
    $("#btnSave").css("display", "none");
    $("#btnEdit").css("display", "block");
    $("#btnCancel").css("display", "none");
    $('.btn-search-ad').hide();

    var insurerClaimNumber = $("input[id=InsurerClaimNumber]").val();
    var policyNumber = $("input[id=PolicyNumber]").val();
    var adjFileNumber = $("input[id=AdjusterFile]").val();

    if (typeof insurerClaimNumber && typeof policyNumber && typeof adjFileNumber != "undefined")
        if (insurerClaimNumber != policyNumber || policyNumber != adjFileNumber || adjFileNumber != insurerClaimNumber) {
            $("input[id=InsurerClaimNumber]").attr('class', 'valid post');

            $("input[id=PolicyNumber]").attr('class', 'valid post');

            $("input[id=AdjusterFile]").attr('class', 'valid post');

        }

   
    $('#frmClaimDetails select').removeClass('post');
    //$('#frmClaimDetails select').addClass('form-control');
    //$('#frmClaimDetails input[type="text"]').addClass('form-control');
}



