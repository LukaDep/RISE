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

/**
 * Subscribes the user to push notifications using the provided VAPID public key.
 * This function checks for service worker and push notification support, requests
 * notification permission from the user, and creates a push subscription.
 *
 * @param {string} publicKey - The base64-encoded VAPID public key for the application server.
 * @returns {Promise<Object>} A promise that resolves to the push subscription object containing:
 *   - endpoint: {string} The subscription endpoint URL
 *   - expirationTime: {number|null} Expiration time of the subscription (null if none)
 *   - keys: {Object} The encryption keys
 *     - p256dh: {string} The P-256 Diffie-Hellman key
 *     - auth: {string} The authentication secret
 * @throws {Error} Throws an error if:
 *   - Service workers are not supported
 *   - Push notifications are not supported
 *   - Notification permission is denied
 *   - Notification permission is not granted
 *   - Service worker registration fails
 *   - Push subscription fails
 */
window.subscribeUser = async function (publicKey) {
    console.log('subscribeUser called with key:', publicKey)
    try {
        console.log('Checking service worker support...')
        if (!('serviceWorker' in navigator)) {
            throw new Error('Service workers are not supported in this browser')
        }

        console.log('Checking push manager support...')
        if (!('PushManager' in window)) {
            throw new Error(
                'Push notifications are not supported in this browser'
            )
        }

        console.log('Checking notification permission...')
        if (Notification.permission === 'denied') {
            throw new Error('Notification permission was denied')
        }

        console.log('Requesting permission if needed...')
        if (Notification.permission !== 'granted') {
            const permission = await Notification.requestPermission()
            console.log('Permission result:', permission)
            if (permission !== 'granted') {
                throw new Error('Notification permission was not granted')
            }
        }

        console.log('Waiting for service worker ready...')
        const registration = await navigator.serviceWorker.ready
        console.log('Service worker ready:', !!registration)
        if (!registration) {
            throw new Error('Service worker registration not found')
        }

        console.log('Converting VAPID key...')
        const converted = urlBase64ToUint8Array(publicKey)

        console.log('Subscribing to push...')
        const subscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: converted,
        })

        console.log('Subscription successful:', subscription)
        return subscription
    } catch (error) {
        console.error('subscribeUser error:', error.message)
        throw new Error(error.message)
    }
}

window.checkExistingSubscription = async function () {
    console.log('checkExistingSubscription called')
    if (!('serviceWorker' in navigator)) {
        console.log('Service worker not supported')
        return false
    }

    try {
        console.log('Waiting for service worker ready...')
        const reg = await navigator.serviceWorker.ready
        console.log('Service worker ready, getting subscription...')
        const sub = await reg.pushManager.getSubscription()
        const exists = sub !== null
        console.log('Existing subscription:', exists)
        return exists
    } catch (error) {
        console.error('checkExistingSubscription error:', error)
        return false
    }
}

/**
 * Gets the existing push subscription or creates a new one if permission is granted.
 * This is used to sync the subscription with the server after login.
 * If permission is granted but no subscription exists, it will create one.
 *
 * @param {string} publicKey - The base64-encoded VAPID public key for the application server.
 * @returns {Promise<Object|null>} A promise that resolves to the push subscription object or null if not possible.
 */
window.getExistingSubscription = async function (publicKey) {
    console.log('getExistingSubscription called')
    if (!('serviceWorker' in navigator)) {
        console.log('Service worker not supported')
        return null
    }

    if (!('PushManager' in window)) {
        console.log('Push manager not supported')
        return null
    }

    // Only proceed if permission is already granted
    if (Notification.permission !== 'granted') {
        console.log('Notification permission not granted, returning null')
        return null
    }

    try {
        console.log('Waiting for service worker ready...')
        const reg = await navigator.serviceWorker.ready
        console.log('Service worker ready, getting subscription...')
        let sub = await reg.pushManager.getSubscription()

        if (sub === null) {
            console.log(
                'No existing subscription found, creating new one since permission is granted...'
            )

            // Permission is granted but no subscription exists - create one
            const converted = urlBase64ToUint8Array(publicKey)
            sub = await reg.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: converted,
            })

            console.log('New subscription created:', sub)
        } else {
            console.log('Existing subscription found:', sub)
        }

        return sub
    } catch (error) {
        console.error('getExistingSubscription error:', error)
        return null
    }
}
