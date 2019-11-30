(function ($) {
    'user strict'
    if (!window.HelperSDK) window.HelperSDK = {};
    function pad(number, length) { var str = "" + number; while (str.length < length) { str = '0' + str; } return str; }

    window.HelperSDK.Helpers = {
        Redirect: function (returnUrl) {
            window.location.href = window.location.protocol + "//" + window.location.host + returnUrl;
        },
        GetJsonSource: function (controller, action) {
            return window.location.protocol + '//' + window.location.host + "/" + controller + "/" + action;
        },
        //ReturnLocation: function (page) {
        //    switch (page) {
        //        case "Product":
        //            Redirect("/Admin/Product/ProductManagement");
        //            break;
        //        case "Account":
        //            Redirect("/Admin/MainService/AccontManager");
        //            break;
        //        case "Attribute":
        //            Redirect("/Admin/MainService/Attributes");
        //        case "Category":
        //            Redirect("/Admin/MainService/Category");
        //        case "Brand":
        //            Redirect("/Admin/Brand/Brand");
        //        case "Bill":
        //            Redirect("/Admin/Bill/BillMangement");
        //    }
        //}
    }
})(jQuery);