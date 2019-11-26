
//$('#submit').click(function (e) {
//    debugger;
//    var form = $('#__AjaxAntiForgeryForm');
//    var token = $('input[name="__RequestVerificationToken"]', form).val();
//    e.preventDefault();
//    if ($('#fullname').val().trim() == "") {
//        $("#validation").text("*Vui lòng nhập họ và tên của bạn");
//        $('#fullname').focus();
//        return;
//    }
//    if ($('#username').val().trim() == "") {
//        $("#validation").text("*Vui lòng nhập tên đăng nhập của bạn");
//        $('#username').focus();
//        return;
//    }

//    if ($('#email').val().trim() == "") {
//        $("#validation").text("*Vui lòng nhập Email của bạn");
//        $('#email').focus();
//        return;
//    }

//    if ($('#phonenumber').val().trim() == "") {
//        $("#validation").text("*Vui lòng nhập Email của bạn");
//        $('#phonenumber').focus();
//        return;
//    }

//    if ($('#Pwd').val().length < 8) {
//        $("#validation").text("*Mật khẩu phải có ít nhất 8 ký tự");
//        $('#Pwd').focus();
//        return;
//    }

//    if ($('#Pwd').val() != $('#confirmPwd').val()) {
//        $("#validation").text("*Nhập lại mật khẩu không chính xác");
//        $('#Pwd').text("");
//        $('#confirmPwd').text("");
//        return;
//    }
//    var model = {
//        FullName: $('#fullname').val(),
//        UserName: $('#username').val(),
//        Email: $('#email').val(),
//        PhoneNumber: $('#phonenumber').val(),
//        Password: $('#Pwd').val()
//    }

//    var data = JSON.stringify(model);
//    $.ajax({
//        //url: "/Customer/Account/SignUpPost?FullName=" + $('#fullname').val()
//        //    + "&UserName=" + $('#username').val() + "&Email=" + $('#email').val() +
//        //    "&PhoneNumber=" + $('#phonenumber').val() + "&Password="+$('#Pwd').val()+"",
//        url: "/Account/SignUpPost",
//        type: "post",
//        dataType: "json",
//        //data: {
//        //    __RequestVerificationToken: token,
//        //    model: data
//        //},
//        data: data,
//        success: function (result) {

//        },
//        error: function (err) {
//            console.log(err);
//        }
//    });

//});
