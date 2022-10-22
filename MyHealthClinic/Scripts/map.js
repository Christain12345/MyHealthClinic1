let map;

function initMap() {
    map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: -37.810, lng: 144.964 },
        zoom: 12,
    });

    // auto complete for start
    var start = document.getElementById("start");
    const autoComplete = new google.maps.places.Autocomplete(start);
    autoComplete.bindTo("bounds", map);

    // get direction
    const directionsRenderer = new google.maps.DirectionsRenderer();
    const directionsService = new google.maps.DirectionsService();
    directionsRenderer.setMap(map);
    directionsRenderer.setPanel(document.getElementById("sidebar"));
    var getDirection = document.getElementById("get-direction");
    getDirection.addEventListener("click", function () {
        directionsService.route({
            origin: {
                query: document.getElementById("start").value
            },
            destination: {
                query: document.getElementById("end").value
            },
            travelMode: google.maps.TravelMode[document.getElementById("mode").value]
        }, (response, status) => {
            if (status == "OK") {
                directionsRenderer.setDirections(response);
            } else {
                window.alert("Unable to direct due to " + status);
            }
        });
    });

}
function handleLocationError(browserHasGeolocation, infoWindow, pos) {
    infoWindow.setPosition(pos);
    infoWindow.setContent(
        browserHasGeolocation
            ? "Error: The Geolocation service failed."
            : "Error: Your browser doesn't support geolocation."
    );
    infoWindow.open(map);
}
