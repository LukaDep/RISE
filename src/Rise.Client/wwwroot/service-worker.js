self.addEventListener('install', (event) => {
    console.log('Service worker installing...')
    self.skipWaiting()
})

self.addEventListener('activate', (event) => {
    console.log('Service worker activating...')
})

self.addEventListener('fetch', (event) => {
    // Only cache http/https requests
    if (!event.request.url.startsWith('http')) {
        return
    }

    // Only cache GET requests (Cache API doesn't support POST, PUT, etc.)
    if (event.request.method !== 'GET') {
        return
    }

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

self.addEventListener('push', (event) => {
    const data = event.data?.json() || {}
    event.waitUntil(
        self.registration.showNotification(data.title, { body: data.body })
    )
})
