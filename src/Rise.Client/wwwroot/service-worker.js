self.addEventListener('install', (event) => {
    console.log('Service worker installing...')
    self.skipWaiting()
})

self.addEventListener('activate', (event) => {
    console.log('Service worker activating...')
})

self.addEventListener('fetch', (event) => {
    if (
        !event.request.url.startsWith('http') ||
        (event.request.method !== 'GET')
    ) {
        return
    }

    event.respondWith(
        fetch(event.request)
            .then((networkResponse) => {
                return caches.open('blazor-cache').then((cache) => {
                    cache.put(event.request, networkResponse.clone())
                    return networkResponse
                })
            })
            .catch(() => {
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

    const notificationOptions = {
        body: data.body,
        icon: '/icon-192.png',
        badge: '/icon-192.png',
        data: {
            url: data.url,
        },
    }

    event.waitUntil(
        self.registration.showNotification(
            data.title || 'Nieuwe melding',
            notificationOptions
        )
    )
})

self.addEventListener('notificationclick', (event) => {
    event.notification.close()

    const urlToOpen = event.notification.data?.url || '/'

    event.waitUntil(
        clients
            .matchAll({ type: 'window', includeUnowned: true })
            .then((windowClients) => {
                for (let client of windowClients) {
                    if (client.url === urlToOpen && 'focus' in client) {
                        return client.focus()
                    }
                }

                if (clients.openWindow) {
                    return clients.openWindow(urlToOpen)
                }
            })
    )
})
