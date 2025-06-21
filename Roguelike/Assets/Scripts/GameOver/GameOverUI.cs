using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    /// <summary>
    /// 「広告を見て復活」ボタン。
    /// </summary>
    [SerializeField] Button reviveButton;

    /// <summary>
    /// 「ギブアップ」ボタン。
    /// </summary>
    [SerializeField] Button giveUpButton;

#if UNITY_EDITOR
    [Header("Editor Debug")]
    [Tooltip("Editor上で広告をスキップして即復活するか")]
    [SerializeField] bool simulateAdsInEditor = true;
#endif

    /// <summary>
    /// プレイヤー。
    /// </summary>
    Player _player;

    /// <summary>
    /// パネルが表示中かどうか。
    /// </summary>
    private bool _isActive;

    void Awake()
    {
        reviveButton.onClick.AddListener(OnClickRevive);
        giveUpButton.onClick.AddListener(OnClickGiveUp);

        _isActive = false;
        gameObject.SetActive(false); // 初期状態では非表示
    }

    private void OnEnable()
    {
        _isActive = true;
    }

    private void OnDisable()
    {
        _isActive = false;
    }

    private void Update()
    {
        if (!_isActive) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // もしパネルが表示中なら、ギブアップボタンを押したのと同じ処理を行う
            OnClickGiveUp();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            // もしパネルが表示中なら、広告を見て復活ボタンを押したのと同じ処理を行う
            OnClickRevive();
        }
    }

    void OnClickRevive()
    {
        // もしボタンが無効化されていたら、何もしない
        if (!reviveButton.interactable) return;

#if UNITY_EDITOR
        if (simulateAdsInEditor)
        {
            // エディタでは広告をスキップして即復活テスト
            HandleReward("revive", 1);
        }
#endif
        // 1. 広告視聴終了をハンドリングする
        AdsManager.Instance.OnRewardGranted += HandleReward;

        // 2. 必要ならプレースメント名を指定
        AdsManager.Instance.ShowRewardedAd("revive");
        // 2重タップ防止
        reviveButton.interactable = false;
        giveUpButton.interactable = true;

        _isActive = false; // パネルを非表示にする
    }

    void HandleReward(string rewardName, int rewardAmount)
    {
        AdsManager.Instance.OnRewardGranted -= HandleReward;

        // プレイヤーを復活
        _player = FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.LogError("Player not found in the scene.");
            return;
        }
        _player.ReviveFromAd();

        // パネルを閉じる
        gameObject.SetActive(false);
    }

    void OnClickGiveUp()
    {
        // 広告の報酬待ちを念のため解除
        AdsManager.Instance.OnRewardGranted -= HandleReward;

        // 2重タップ防止
        reviveButton.interactable = false;
        giveUpButton.interactable = false;

        // プレイヤーを削除
        _player = FindObjectOfType<Player>();
        if (_player != null)
        {
            _player.Destroy();
        }

        // セーブデータを破棄
        SaveData.Destroy();

        // タイトル画面へ戻る
        TitleManager.Instance.GoToTitle();

        // ゲームオーバーのパネルを閉じる
        _isActive = false; // パネルを非表示にする
        gameObject.SetActive(false);
    }
}
