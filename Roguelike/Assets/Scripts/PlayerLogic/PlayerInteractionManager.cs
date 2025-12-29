using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;
using Roguelike.Window;

public class PlayerInteractionManager : IPlayerInteractionManager
{
    private Player _player;
    private MessageWindow _messageWindow;
    private IPlayerEffectManager _effectManager;

    public bool DoWaitEvent { get; private set; } = false;

    [Inject]
    public void Construct(MessageWindow messageWindow, IPlayerEffectManager effectManager)
    {
        _messageWindow = messageWindow;
        _effectManager = effectManager;
    }

    public void Initialize(Player player)
    {
        _player = player;
    }

    public void SetDoWaitEvent(bool wait)
    {
        DoWaitEvent = wait;
    }

    public void CheckEvent()
    {
        DoWaitEvent = true;
        _player.StartCoroutine(RunEvents());
    }

    private IEnumerator RunEvents()
    {
        // 敵のターン処理
        foreach (var enemy in UnityEngine.Object.FindObjectsOfType<Enemy>())
        {
            enemy.MoveStart();
            yield return new WaitWhile(() => enemy.IsNowAttacking);
        }
        
        // 全ての敵の攻撃が終わるのを待つ
        yield return new WaitWhile(() =>
            UnityEngine.Object.FindObjectsOfType<Enemy>().Any(_e => _e.IsNowAttacking));

        // ゴール判定
        if (_player != null && _player.Map != null)
        {
            var mass = _player.Map[_player.Pos.x, _player.Pos.y];
            if (mass.Type == MassType.Goal)
            {
                _player.StartCoroutine(Goal());
            }
            else
            {
                DoWaitEvent = false;
            }
        }
        else
        {
            DoWaitEvent = false;
        }
    }

    private IEnumerator Goal()
    {
        if (_player == null) yield break;

        _player.CanMove = false;

        SoundEffectManager.Instance.PlayStairSound();
        yield return _player.StartCoroutine(FadeController.Instance.FadeOut());

        var mapSceneManager = UnityEngine.Object.FindObjectOfType<MapSceneManager>();
        
        // 階層を1つ下げる
        mapSceneManager.CurrentFloor += 1;
        // マップを新規生成
        mapSceneManager.GenerateMap();
        // BGMを再生する
        SoundEffectManager.Instance.PlayDungeonBGM();

        // プレイヤーのデータを引き継ぐ (新しいシーン上のプレイヤーを探す)
        var newPlayer = UnityEngine.Object.FindObjectOfType<Player>();
        if (newPlayer != null)
        {
            newPlayer.Hp = _player.Hp;
            newPlayer.FoodValue.CurrentValue = _player.FoodValue.CurrentValue;
            newPlayer.Exp = _player.Exp;
            newPlayer.Level = _player.Level;
            newPlayer.Attack = _player.Attack;
            newPlayer.Defence = _player.Defence;
            newPlayer.CurrentWeapon = _player.CurrentWeapon;
            newPlayer.CurrentArmor = _player.CurrentArmor;
        }

        // セーブする
        var saveController = UnityEngine.Object.FindObjectOfType<SaveLoadController>();
        if (saveController != null)
        {
            var itemInventory = UnityEngine.Object.FindObjectOfType<ItemInventory>();
            // 注意: 古いPlayerインスタンスを渡しているが、データクラスとしては機能するはず。
            // 正しくは新しいPlayerを渡すべきだが、Goal処理のコンテキストでは自身のデータが最新。
            // ただしMapは新規生成後なので、SaveLoadControllerが参照するMapが正しいか要確認。
            // MapSceneManager.GenerateMap()でMapインスタンスが再生成されている場合、
            // FindObjectOfType<Map>() で最新が取れるはず。
            var map = UnityEngine.Object.FindObjectOfType<Map>();
            saveController.Save(newPlayer, map, itemInventory);
        }

        // 古いPlayerのCanMoveを戻す必要はないが（Destroyされるかシーン遷移するため）、一応
        _player.CanMove = true;
    }
}
