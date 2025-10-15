window.leafletMap = {
    map: null,
    imageLayer: null,
    initImage: function (elementId, imageUrl, bounds) {
        if (this.map) {
            this.map.remove();
        }
        this.map = L.map(elementId, {
            crs: L.CRS.Simple,
            minZoom: -2,
            maxBounds: bounds,
            maxBoundsViscosity: 1.0,
            zoomControl: false // Disable default zoom control
        });
        this.imageLayer = L.imageOverlay(imageUrl, bounds).addTo(this.map);
        this.map.fitBounds(bounds);

        var minZoom = this.map.getBoundsZoom(bounds, false);
        this.map.setMinZoom(minZoom);

        // Add zoom control to bottom right AFTER everything else
        L.control.zoom({ position: 'bottomright' }).addTo(this.map);
    },
    addMarker: function(lat, lng, name) {
        if (this.map) {
            L.marker([lat, lng]).addTo(this.map).bindPopup(name);
        }
    },
    initTileMap: function(elementId, centerLat, centerLng, zoom) {
        if (this.map) {
            this.map.remove();
        }
        this.map = L.map(elementId).setView([centerLat, centerLng], zoom);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(this.map);

        // Move zoom control for tile map as well
        this.map.zoomControl.setPosition('bottomright');
    },
    addLatLngMarker: function(lat, lng, name) {
        if (this.map) {
            L.marker([lat, lng]).addTo(this.map).bindPopup(name);
        }
    }
};