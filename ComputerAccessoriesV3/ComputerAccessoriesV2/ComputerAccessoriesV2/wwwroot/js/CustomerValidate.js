
function AddToCart(productId, quantity) {
    debugger;
    var data = {
        productId: productId,
        quantity: quantity
    };

    $.ajax({
        url: "/Home/AddToCart",
        type: "post",
        dataType: "json",
        data: data,
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
            } else {
                window.HelperSDK.Helpers.Redirect(result.returnUrl);
            }
        }
    });
}