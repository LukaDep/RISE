window.leafletMap = {
    map: null,
    imageLayer: null,
    initImage: function (elementId, imageUrl, bounds) {
        if (this.map) {
            this.map.remove()
        }
        this.map = L.map(elementId, {
            crs: L.CRS.Simple,
            minZoom: -2,
            maxBounds: bounds,
            maxBoundsViscosity: 1.0,
            zoomControl: false,
        })
        this.imageLayer = L.imageOverlay(imageUrl, bounds).addTo(this.map)
        this.map.fitBounds(bounds)

        var minZoom = this.map.getBoundsZoom(bounds, false)
        this.map.setMinZoom(minZoom)

        L.control.zoom({ position: 'bottomright' }).addTo(this.map)
    },
    addMarker: function (lat, lng, name) {
        if (this.map) {
            L.marker([lat, lng]).addTo(this.map).bindPopup(name)
        }
    },
    initTileMap: function (elementId, centerLat, centerLng, zoom) {
        if (this.map) {
            this.map.remove()
        }
        this.map = L.map(elementId).setView([centerLat, centerLng], zoom)
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors',
        }).addTo(this.map)

        this.map.zoomControl.setPosition('bottomright')
    },
    addLatLngMarker: function (lat, lng, name) {
        if (this.map) {
            L.marker([lat, lng]).addTo(this.map).bindPopup(name)
        }
    },
    addMarkerWithGoogleLink: function (lat, lng, name, open) {
        if (!this.map) return
        var marker = L.marker([lat, lng]).addTo(this.map)
        var gmaps =
            'https://www.google.com/maps/search/?api=1&query=' + lat + ',' + lng
        var safeName = (name || '').replace(/</g, '&lt;').replace(/>/g, '&gt;')
        var html =
            '<div><strong>' +
            safeName +
            '</strong><br/><a href="' +
            gmaps +
            '" target="_blank" rel="noopener">Open in Google Maps</a></div>'
        marker.bindPopup(html)
        try {
            if (open) {
                marker.openPopup()
            }
        } catch (e) {
            console.error('Error opening marker popup', e)
        }
    },
    setView: function (lat, lng, zoom) {
        if (!this.map) return
        if (typeof zoom === 'number') {
            this.map.setView([lat, lng], zoom)
        } else {
            this.map.setView([lat, lng])
        }
    },
}
