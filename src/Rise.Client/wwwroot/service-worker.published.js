// In development, always fetch from network and bypass cache.
// In production, the published version enables offline support.

self.importScripts('./service-worker-assets.js')

const cacheName = 'blazor-pwa-cache-' + self.assetsManifest.version
const offlineAssetsInclude = [
    /\.dll$/,
    /\.pdb$/,
    /\.wasm$/,
    /\.html$/,
    /\.js$/,
    /\.css$/,
    /\.json$/,
    /\.png$/,
    /\.jpe?g$/,
    /\.gif$/,
    /\.ico$/,
    /\.svg$/,
    /\.webp$/,
]
const offlineAssetsExclude = [/^service-worker\.js$/]

self.addEventListener('install', (event) => {
    console.info('Service worker: Install')

    event.waitUntil(
        caches.open(cacheName).then((cache) => {
            // Cache all files from the assets manifest except excluded ones
            const assetsToCache = self.assetsManifest.assets
                .map((asset) => new URL(asset.url, self.location).toString())
                .filter((url) =>
                    offlineAssetsInclude.some((pattern) => pattern.test(url))
                )
                .filter(
                    (url) =>
                        !offlineAssetsExclude.some((pattern) =>
                            pattern.test(url)
                        )
                )
            return cache.addAll(assetsToCache)
        })
    )
})

self.addEventListener('activate', (event) => {
    console.info('Service worker: Activate')

    event.waitUntil(
        caches.keys().then((cacheNames) => {
            return Promise.all(
                cacheNames
                    .filter((name) => name !== cacheName)
                    .map((name) => caches.delete(name))
            )
        })
    )
})

self.addEventListener('fetch', (event) => {
    if (event.request.method !== 'GET') return

    const requestUrl = new URL(event.request.url)

    // If request is a navigation to a page (not an API or file)
    if (event.request.mode === 'navigate') {
        event.respondWith(
            caches
                .match('index.html')
                .then((cachedResponse) => cachedResponse || fetch('index.html'))
        )
        return
    }

    event.respondWith(
        caches.match(event.request).then(
            (cachedResponse) =>
                cachedResponse ||
                fetch(event.request).then((response) => {
                    return caches.open(cacheName).then((cache) => {
                        cache.put(event.request, response.clone())
                        return response
                    })
                })
        )
    )
})