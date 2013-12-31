var geocoder;
var map;
var infowindow = new google.maps.InfoWindow();
var marker;
function initialize(lat, lng) {
    geocoder = new google.maps.Geocoder();
    var latlng = new google.maps.LatLng(lat, lng);
    var mapOptions = {
        zoom: 8,
        center: latlng,
        mapTypeId: 'roadmap'
    }
    map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);
}

function codeLatLng(lat, lng) {
    initialize(lat, lng);
    var latlng = new google.maps.LatLng(lat, lng);
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[1]) {
                map.setZoom(11);
                marker = new google.maps.Marker({
                    position: latlng,
                    map: map
                });
                map.panTo(marker.getPosition());
                var infowindow = new google.maps.InfoWindow(
                {
                    content: results[1].formatted_address,
                    size: new google.maps.Size(50, 50)
                });
                //infowindow.open(map, marker);
            } else {
                alert('No results found');
            }
        }
    });
}

function dothis(id) {
    $.post("/Information/GetMessage", { "locationId": id },
        function (data) {
            $("#shopName").text(data.shopName);
            $("#shopDescription").text(data.shopDescription);
            $("#shopOpenTime").html(data.shopOpenTime);
            $("#shopAddress").text(data.shopAddress);
            $("#shopContactNum").html(data.shopContactNum);
        });
}


