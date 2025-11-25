// Utility function to convert base64 string to Uint8Array
function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - (base64String.length % 4)) % 4)
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/')

    const rawData = window.atob(base64)
    const outputArray = new Uint8Array(rawData.length)

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i)
    }
    return outputArray
}

// Make subscribeUser available globally for Blazor JSInterop
window.subscribeUser = async function (publicKey) {
    try {
        console.log('subscribeUser called')

        // Check if service worker is supported
        if (!('serviceWorker' in navigator)) {
            throw new Error('Service workers are not supported in this browser')
        }

        // Check if push notifications are supported
        if (!('PushManager' in window)) {
            throw new Error(
                'Push notifications are not supported in this browser'
            )
        }

        // Request notification permission if not already granted
        if (Notification.permission === 'denied') {
            throw new Error('Notification permission was denied')
        }

        if (Notification.permission !== 'granted') {
            const permission = await Notification.requestPermission()
            if (permission !== 'granted') {
                throw new Error('Notification permission was not granted')
            }
        }
        console.log(
            'subscribeUser passed all checks and will now be called moatjeu'
        )

        // Wait for service worker to be ready
        const registration = await navigator.serviceWorker.ready
        console.log('we hebben goed gewacht op de registration ')
        if (!registration) {
            console.error('Service worker registration not found')
            throw new Error('Service worker registration not found')
        }

        // Convert the public key
        const converted = urlBase64ToUint8Array(publicKey)
        console.log('we hebben goed gewacht op de converted ')

        // Subscribe to push notifications
        const subscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: converted,
        })

        console.log('Push subscription successful:', subscription)
        return subscription
    } catch (error) {
        console.error('Push subscription error:', error)

        let errorMessage = error.message

        throw new Error(errorMessage)
    }
}

// Returnt iets in deze aard: {
//   endpoint: "https://fcm.googleapis.com/fcm/send/e3J9....",
//   expirationTime: null,
//   keys: {
//     p256dh: "BDfQ8vQd8W...",
//     auth: "fF7e1g..."
//   }
// }
