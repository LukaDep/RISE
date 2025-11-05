self.addEventListener('install', (event) => {
    console.log('Service worker installing...')
    self.skipWaiting()
})

self.addEventListener('activate', (event) => {
    console.log('Service worker activating...')
})

self.addEventListener('fetch', (event) => {
    event.respondWith(
        fetch(event.request)
            .then((networkResponse) => {
                // If online, update the cache and return the network response
                return caches.open('blazor-cache').then((cache) => {
                    cache.put(event.request, networkResponse.clone())
                    return networkResponse
                })
            })
            .catch(() => {
                // If offline, serve cached data
                return caches.match(event.request).then((response) => {
                    if (response) {
                        return response
                    }
                })
            })
    )
})
