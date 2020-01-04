function AddToCart(productId, quantity, idButtonView, idSpantextInside) {
    var data = {
        productId: productId,
        quantity: quantity
    };

    $.ajax({
        url: "/Home/AddToCart",
        type: "post",
        dataType: "json",
        data: data,
        beforeSend: function () {
            document.getElementById(idButtonView).disable = true;
            document.getElementById(idSpantextInside).classList.add("lds-dual-ring");
        },
        complete: function () {
            document.getElementById(idButtonView).disable = false;
            document.getElementById(idSpantextInside).classList.remove("lds-dual-ring");
        },
        success: function (result) {
            if (result.code == 1) {
                $('#kendoNoti').data('kendoNotification').show({
                        title: "Đã thêm sản phẩm",
                        message: "Sản phẩm " +
                            result.name +
                            " đã được thêm vào giỏ với số lượng " +
                            result.quantity
                    },
                    "success");
                $('#sumProduct').text(result.sum);
                $.get("/Home/GetTotalPrice", function (data) {
                    $('#sumPrice').text(data.totalPrice);
                });
            } else {
                window.HelperSDK.Helpers.Redirect(result.returnUrl);
            }
        }
    });
}
