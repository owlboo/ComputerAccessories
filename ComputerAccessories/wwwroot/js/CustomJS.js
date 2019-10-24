$(document).ready(function () {
    debugger;
    GetCategory();
    
});

function GetCategory() {
    debugger;
    $.ajax({
        url: "/MainService/GetCategory",
        type: "get",
        dataType: "json",
        success: function (result) {
            if (result != null) {
                //var html = "<li><a href='/Admin/MainService/CreateNewProduct'><i class='fa fa - circle - o'></i> Thêm mới</a></li>\n";
                var html = "<li><a href='pages/UI/general.html'><i class='fa fa-angle-right'></i> Tất cả</a ></li > \n";
                for (var i = 0; i < result.length; i++) {
                    html += "<li><a href='pages/UI/general.html'><i class='fa fa-angle-right'></i>" + result[i].categoryName + "</a ></li > \n";
                }
                $('#groupCategory').html(html);
            }
        }
    });
}

