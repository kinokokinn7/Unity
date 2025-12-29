using System.Threading.Tasks;
using UnityEngine;

public class PlayerEffectManager : IPlayerEffectManager
{
    private Player _player;

    public void Initialize(Player player)
    {
        _player = player;
    }

    public async Task PlayLevelUpEffect()
    {
        await RotateWithEffects();
    }

    public async Task PlayGoalEffect()
    {
         // 回転
        await RotateWithEffects();
        // ジャンプ
        await JumpWithEffects();
    }

    private async Task RotateWithEffects()
    {
        if (_player == null) return;

        float duration = 1.0f;
        float elapsedTime = 0.0f;
        
        // Renderer取得 (Playerの子要素にあると想定)
        var renderer = _player.GetComponentInChildren<Renderer>();
        if (renderer == null) return;

        Color originalColor = renderer.material.color;

        // 最初に正面を向く
        _player.transform.rotation = Quaternion.Euler(0, 180, 0);

        while (elapsedTime < duration)
        {
            if (_player == null) return; // 途中でDestroyされた場合

            float t = elapsedTime / duration;

            // 回転
            _player.transform.Rotate(0, 360 * Time.deltaTime / duration, 0);

            // 色を変える
            renderer.material.color = Color.Lerp(originalColor, Color.yellow, t);

            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        if (_player != null && renderer != null)
        {
            // 元の色に戻す
            renderer.material.color = originalColor;
        }
    }

    private async Task JumpWithEffects()
    {
        if (_player == null) return;

        float duration = 0.5f;
        float elapsedTime = 0.0f;
        Vector3 originalPosition = _player.transform.position;

        // 最初に正面を向く
        _player.transform.rotation = Quaternion.Euler(0, 180, 0);

        while (elapsedTime < duration)
        {
             if (_player == null) return;

            float t = elapsedTime / duration;

            // ジャンプ
            _player.transform.position = Mathf.Sin(t * Mathf.PI) * Vector3.up + originalPosition;

            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
        
        // 位置ずれ補正はPlayer側の移動ロジックで戻る可能性があるが、一応戻すことはしない（高さだけ戻る）
        if(_player != null)
        {
             _player.transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z);
        }
    }
}
