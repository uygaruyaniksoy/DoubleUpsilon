using GoogleMobileAds.Api;

namespace DoubleUpsilon {
    public static class AdmobUtilities {
        private static bool _initialized = false;
        private static InterstitialAd _interstitial;

        private static void InitializeIfNecessary() {
            if (_initialized) return;

            MobileAds.Initialize(initStatus => { });
        }

        public static void PrepareInterstitial(string adUnitId) {
            _interstitial = new InterstitialAd(adUnitId);
            var request      = new AdRequest.Builder().Build();
            
            _interstitial.LoadAd(request);
        }

        public static void ShowInterstitial() {
            InitializeIfNecessary();

            if (_interstitial.IsLoaded()) {
                _interstitial.Show();
                _interstitial.Destroy();
            } else {
                _interstitial.OnAdLoaded += (sender, args) => _interstitial.Show();
                _interstitial.OnAdClosed += (sender, args) => _interstitial.Destroy();
            }
        }
    }
}