window.leafletMap = {
    map: null,
    imageLayer: null,
    initImage: function (elementId, imageUrl, bounds) {
        if (this.map) {
            this.map.remove();
        }
        this.map = L.map(elementId, {
            crs: L.CRS.Simple,
            minZoom: -2
        });
        this.imageLayer = L.imageOverlay(imageUrl, bounds).addTo(this.map);
        this.map.fitBounds(bounds);
    },
    addMarker: function(lat, lng) {
        if (this.map) {
            L.marker([lat, lng]).addTo(this.map);
        }
    }
};