using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.LevelPlay;

/// <summary>
/// LevelPlay (Unity Ads Mediation) を使って報酬付き動画広告を管理するシングルトン。
/// - SDK の初期化
/// - RewardedAd オブジェクトの生成
/// - 広告のロード／表示
/// - 各広告イベントのハンドリング
/// を行います。
/// </summary>
/// <remarks>
/// 実装は Unity LevelPlay 公式ドキュメント「動画リワード実装（Step-7）」
/// https://developers.is.com/ironsource-mobile/unity/rewarded-video-integration-unity-japanese/#step-7
/// を参考にしています。([developers.is.com](https://developers.is.com/ironsource-mobile/unity/rewarded-video-integration-unity-japanese/))
/// </remarks>
public class AdsManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス。
    /// </summary>
    public static AdsManager Instance { get; private set; }

    /// <summary>
    /// 広告視聴完了でリワードが付与されたときに発火する汎用イベント。
    /// <param name="rewardName">リワードの名前。</param>
    /// <param name="rewardAmount">リワードの数量。</param>
    /// </summary>
    public event Action<string, int> OnRewardGranted;

    #region Inspector

    [Header("LevelPlay Settings")]

    [Tooltip("published App Key at LevelPlay")]
    [SerializeField] private string appKey = "YOUR_APP_KEY";

    [Tooltip("RewardedAdUnitId for LevelPlay")]
    [SerializeField] private string rewardedAdUnitId = "YOUR_REWARDED_AD_UNIT_ID";

    #endregion

    #region Fields

    /// <summary>再利用可能な RewardedAd インスタンス</summary>
    private LevelPlayRewardedAd rewardedAd;

    #endregion

    #region Unity lifecycle

    private async void Awake()
    {
        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いで保持
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は破棄
        }
        await InitializeLevelPlayAsync();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Unity Services → LevelPlay の順で初期化。
    /// </summary>
    private async Task InitializeLevelPlayAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            // 成功／失敗コールバックを登録してから Init を呼ぶ
            LevelPlay.OnInitSuccess += OnInitSuccess;
            LevelPlay.OnInitFailed += OnInitFailed;

            LevelPlay.Init(appKey);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AdsManager] UnityServices.InitializeAsync 失敗: {e}");
        }
    }

    /// <summary>
    /// LevelPlay 初期化成功ハンドラ。
    /// </summary>
    private void OnInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("LevelPlay 初期化成功");

        // RewardedAd オブジェクトを生成（複数作る場合はここを拡張）
        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);

        RegisterRewardedAdEvents(rewardedAd);

        // 最初の広告をロード
        rewardedAd.LoadAd();
    }

    /// <summary>
    /// LevelPlay 初期化失敗ハンドラ。
    /// </summary>
    /// <param name="error"></param>
    private void OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError($"LevelPlay 初期化失敗: {error.ErrorCode} – {error.ErrorMessage}");
    }

    #endregion

    #region Event registration

    private void RegisterRewardedAdEvents(LevelPlayRewardedAd ad)
    {
        // ドキュメント推奨のイベント登録順序  ([developers.is.com](https://developers.is.com/ironsource-mobile/unity/rewarded-video-integration-unity-japanese/))
        ad.OnAdLoaded += OnAdLoaded;
        ad.OnAdLoadFailed += OnAdLoadFailed;
        ad.OnAdDisplayed += OnAdDisplayed;
        ad.OnAdDisplayFailed += OnAdDisplayFailed;
        ad.OnAdRewarded += OnAdRewarded;
        ad.OnAdClosed += OnAdClosed;

        // Optional
        ad.OnAdClicked += OnAdClicked;
        ad.OnAdInfoChanged += OnAdInfoChanged;
    }

    #endregion

    #region Public API

    /// <summary>
    /// 報酬付き動画を表示。<br/>
    /// <paramref name="placementName"/> を指定すると、対応するプレースメントで再生します。
    /// </summary>
    /// <param name="placementName">LevelPlay で設定したプレースメント (任意)</param>
    public void ShowRewardedAd(string placementName = null)
    {
        if (rewardedAd == null)
        {
            Debug.LogWarning("[AdsManager] RewardedAd 未生成");
            return;
        }

        bool isReady = rewardedAd.IsAdReady();
        bool isCapped = placementName != null && LevelPlayRewardedAd.IsPlacementCapped(placementName);

        if (isReady && !isCapped)
        {
            if (string.IsNullOrEmpty(placementName))
            {
                rewardedAd.ShowAd();
            }
            else
            {
                rewardedAd.ShowAd(placementName);
            }
        }
        else
        {
            Debug.Log("広告未準備 または 視聴回数上限");
        }
    }

    #endregion

    #region RewardedAd events

    private void OnAdLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.Log("報酬広告ロード完了");
    }

    private void OnAdLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError($"報酬広告ロード失敗: {error.ErrorCode} - {error.ErrorMessage}");
        // 失敗時はリトライするなどの対策を行う
    }

    private void OnAdDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("▶広告表示開始");
    }

    private void OnAdDisplayFailed(LevelPlayAdDisplayInfoError infoError)
    {
        Debug.LogError($"▶広告表示失敗: {infoError}");
    }

    private void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log($"リワード付与: {reward.Name} x{reward.Amount}");
        GrantReward(reward.Name, reward.Amount);
    }

    private void OnAdClosed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("広告クローズ");
        // 次回の表示に備えて再ロード
        rewardedAd.LoadAd();
    }

    private void OnAdClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("広告クリック");
    }

    private void OnAdInfoChanged(LevelPlayAdInfo adInfo)
    {
        Debug.Log("AdInfo 更新");
    }

    #endregion

    #region Reward handling

    /// <summary>
    /// ユーザーへリワードを付与するゲーム固有処理。
    /// </summary>
    private void GrantReward(string rewardName, int rewardAmount)
    {
        // TODO: ゲームロジックに合わせて実装
        Debug.Log($"[AdsManager] {rewardName} を {rewardAmount} 付与");
        OnRewardGranted?.Invoke(rewardName, rewardAmount);
    }

    #endregion
}