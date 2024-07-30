using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameHeaven
{
    public class AdManager : TSingleton<AdManager>
    {
        [Header("Ad Unit IDs")]
        [SerializeField] private string androidBannerAdUnitId;
        [SerializeField] private string iosBannerAdUnitId;
        [SerializeField] private string androidInterstitialAdUnitId;
        [SerializeField] private string iosInterstitialAdUnitId;
        string _adUnitId = null;


        private BannerView _bannerView;

        public void Start()
        {
            DontDestroyOnLoad(gameObject);
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(initStatus => { });


#if UNITY_ANDROID
            _adUnitId = androidBannerAdUnitId;
#elif UNITY_IOS
        _adUnitId = iosBannerAdUnitId;
#else
        _adUnitId = androidBannerAdUnitId;
        //Debug.LogError("Unsupported platform");
        //return;
#endif

            LoadAd();
        }

        // SceneManager를 사용하여 씬이 로드될 때 호출되는 메서드
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 로드된 씬이 변경될 때 광고를 업데이트하는 등의 작업을 수행할 수 있습니다.
            LoadAd();
        }


        public void CreateBannerView()
        {
            Debug.Log("Creating banner view");

            // If we already have a banner, destroy the old one.
            if (_bannerView != null)
            {
                DestroyBannerView();
            }
            //_bannerView = new BannerView(_adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
            
            // Custom banner size (e.g., 320x100)
            //AdSize adSize = new AdSize(320, 100); // 사용자 정의 크기
            //_bannerView = new BannerView(_adUnitId, adSize, AdPosition.Bottom);
            AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            _bannerView = new BannerView(_adUnitId, adSize, AdPosition.Bottom);

        }
        public void LoadAd()
        {
            if (_bannerView == null)
            {
                CreateBannerView();
            }

            var adRequest = new AdRequest();

            // send the request to load the ad.
            Debug.Log("Loading banner ad.");
            _bannerView.LoadAd(adRequest);
            ListenToAdEvents();
        }
        private void ListenToAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            _bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                    + _bannerView.GetResponseInfo());
            };
            // Raised when an ad fails to load into the banner view.
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : "
                    + error);
            };
            // Raised when the ad is estimated to have earned money.
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _bannerView.OnAdClicked += () =>
            {
                Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Banner view full screen content closed.");
            };
        }
        public void DestroyBannerView()
        {
            if (_bannerView != null)
            {
                Debug.Log("Destroying banner view.");
                _bannerView.Destroy();
                _bannerView = null;
            }
        }
    }

}