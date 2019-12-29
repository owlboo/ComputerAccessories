

$('#submit').click(function (e) {
    var popupNotification = $('#notify').data('kendoNotification');
    if ($('#fullname').val().trim() == "") {
        //$("#validation").text("*Vui lòng nhập họ và tên của bạn");
        popupNotification.show({ title: "Thông báo", message: "Vui lòng nhập họ tên của bạn" }, "error");
        $('#fullname').focus();
        return;
    }

    if ($('#email').val().trim() == "") {
        popupNotification.show({ title: "Thông báo", message: "Vui lòng nhập Email của bạn" }, "error");
        $('#email').focus();
        return;
    }

    if ($('#phonenumber').val().trim() == "") {
        popupNotification.show({ title: "Thông báo", message: "Vui lòng nhập số điện thoại của bạn" }, "error");
        $('#phonenumber').focus();
        return;
    }

    if ($('#Pwd').val().length < 6) {
        popupNotification.show({ title: "Thông báo", message: "Mật khẩu phải có ít nhất 6 ký tự" }, "error");
        $('#Pwd').focus();
        return;
    }

    if ($('#Pwd').val() != $('#confirmPwd').val()) {
        popupNotification.show({ title: "Thông báo", message: "Nhập lại mật khẩu không chính xác" }, "error");
        $('#Pwd').text("");
        $('#confirmPwd').text("");
        return;
    }

    var provinceId = $('#provinceSelector').data('kendoDropDownList').value();
    var districtId = $('#districtSelector').data('kendoDropDownList').value();
    var wardId = $('#wardSelector').data('kendoDropDownList').value();
    if (provinceId == "") {
        popupNotification.show({ title: "Thông báo", message: "Vui lòng chọn Tỉnh/Thành phố" }, "error");
        return;
    }
    if (districtId == "") {
        popupNotification.show({ title: "Thông báo", message: "Vui lòng chọn Quận/Huyện" }, "error");
        return;
    }
    if (wardId == "") {
        popupNotification.show({ title: "Thông báo", message: "Vui lòng chọn Xã/Thị trấn" }, "error");
        return;
    }
    var data = {
        FullName: $('#fullname').val(),
        Email: $('#email').val(),
        PhoneNumber: $('#phonenumber').val(),
        Password: $('#Pwd').val(),
        ProvinceId: provinceId,
        DistrictId: districtId,
        WardId: wardId,
        PlaceDetail: $('#placeDetailText').val(),
        RoleName:"Customer"
    }

    $.ajax({
        url: "/Customer/Account/SignUp",
        type: "post",
        dataType: "json",
        data: data,
        success: function (result) {
            if (result.code == 1) {
                popupNotification.show({ title: "Thông báo", message: "Một email đã được gửi đến tài khoản: " + result.email + " để kích hoạt tài khoản của bạn" }, "success");
                setTimeout(window.HelperSDK.Helpers.Redirect(result.returnUrl), 6 * 1000);
            } else {
                popupNotification.show({ title: "Thông báo", message: result.err }, "error");
            }
        },
        error: function (err) {
            console.log(err);
        }
    });

});