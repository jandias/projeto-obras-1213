
$(document).ready(function () {
    $('#departmentId').attr('disabled', 'disabled');
    $('.btn-success').attr('disabled', 'disabled');
    $('#shopId').change(function () {
        if ($(this).val() != -1) {
            $("#departmentId").empty().load("/Comms/AjaxDepartments", "shopId=" + $(this).val(), 
                function (response, status, xhr) {
                    if (status == "success") {
                        $('#departmentId').removeAttr('disabled');
                    }
                }
                );
        }
        else {
            $('#departmentId').attr('disabled', 'disabled');
            $('.btn-success').attr('disabled', 'disabled');
        }
    });
    $('#departmentId').change(function () {
        if ($(this).val() != -1) {
            $('.btn-success').removeAttr('disabled');
        }
        else {
            $('.btn-success').attr('disabled', 'disabled');
        }
    });
//    $('form').submit(function () {
//        var data = new FormData();
//        var serialized = $(this).serialize();
//        data.append("CommFile", $("input")[0].files[0] );
//        //data.append($("#shopId").attr('name'), $("#shopId").attr('value'));

//        $.ajax({
//            url: $(this).attr('action'),
//            data: data,
//            cache: false,
//            contentType: false,
//            processData: false,
//            type: 'POST',
//            success: function (data) {
//                alert(data);
//            }
//        });
        
//            //function (jsonData) {
//            //    if (jsonData["success"] == true)
//            //        alert(jsonData);
//            //    else
//            //        alert("Falhou a operação:\n" + jsonData["errorMsg"]);
//            //},
//            //'json');
//        return false;
//    });
});
