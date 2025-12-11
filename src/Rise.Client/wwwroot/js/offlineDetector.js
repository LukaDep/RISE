let dotNetHelper = null;

window.initializeOfflineDetector = function (dotNetReference) {
  dotNetHelper = dotNetReference;

  // Set initial status
  updateStatus();

  // Add event listeners
  window.addEventListener('online', updateStatus);
  window.addEventListener('offline', updateStatus);
}

function updateStatus() {
  if (dotNetHelper) {
    const isOnline = navigator.onLine;
    dotNetHelper.invokeMethodAsync('UpdateOnlineStatus', isOnline);
  }
}