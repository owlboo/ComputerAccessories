$("#provinceSelector").change(async function () {
    var option = { method: 'GET' }
    var url = "https://" + window.location.host + "/Region/GetDistrict?provinceId=" + $("#provinceSelector").children("option:selected").val();
    var respone = await fetch(url, option);
    var json = await respone.json();

    var districtSelector = document.getElementById("districtSelector");
    districtSelector.innerText = null;
    
    for (var i = 0; i < json.length; i++) {
        var option = document.createElement("option");
        option.text = json[i].districtName;
        option.value = json[i].districtId;
        districtSelector.add(option);
    }
})

$("#districtSelector").change(async function () {
    var option = { method: 'GET' }
    var url = "https://" + window.location.host + "/Region/GetWard?districtId=" + $("#districtSelector").children("option:selected").val();
    var respone = await fetch(url, option);
    var json = await respone.json();

    var wardSelector = document.getElementById("wardSelector");
    wardSelector.innerText = null;
    for (var i = 0; i < json.length; i++) {
        var option = document.createElement("option");
        option.text = json[i].wardName;
        option.value = json[i].wardId;
        wardSelector.add(option);
    }
})
