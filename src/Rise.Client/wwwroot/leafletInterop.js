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
                maxBoundsViscosity: 1.0 
            });
            this.imageLayer = L.imageOverlay(imageUrl, bounds).addTo(this.map);
            this.map.fitBounds(bounds);

            var minZoom = this.map.getBoundsZoom(bounds, false);
            this.map.setMinZoom(minZoom);
        },
        addMarker: function(lat, lng, name) {
            if (this.map) {
                L.marker([lat, lng]).addTo(this.map) .bindPopup(name);
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
        },
        addLatLngMarker: function(lat, lng, name) {
            if (this.map) {
                L.marker([lat, lng]).addTo(this.map).bindPopup(name);
            }
        }
    };