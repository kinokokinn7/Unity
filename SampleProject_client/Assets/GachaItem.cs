using System;
using UnityEngine;
using UnityEngine.UI;

public class GachaItem : MonoBehaviour
{
	public MasterGachaModel masterGachaModel;

	[SerializeField]
	Image banner;

	[SerializeField]
	Image costType;

	[SerializeField]
	Text costAmountLabel;

	[SerializeField]
	Text drawCountLabel;

	[SerializeField]
	Text periodLabel;

	[SerializeField]
	Text descriptionLabel;

	void Start()
	{
		//Resourcesから画像を読み込む場合
		Sprite sprite = Resources.Load<Sprite>(masterGachaModel.banner_id);
		if (sprite == null) {
			UnityEngine.Debug.LogError("ガチャバナーの画像がありません。");
			return;
		}
		banner.sprite = sprite;

		Sprite currencySprite = null;
		if (masterGachaModel.cost_type == (int)MasterGacha.CostType.Crystal || masterGachaModel.cost_type == (int)MasterGacha.CostType.CrystalFree) {
			currencySprite = Resources.Load<Sprite>("Crystal");
		} else if (masterGachaModel.cost_type == (int)MasterGacha.CostType.FriendCoin) {
			currencySprite = Resources.Load<Sprite>("FriendCoin");
		}
		if (currencySprite == null) {
			UnityEngine.Debug.LogError("通貨の画像がありません。");
			return;
		}
		costType.sprite = currencySprite;
	
		costAmountLabel.text = masterGachaModel.cost_amount.ToString();
		drawCountLabel.text = masterGachaModel.draw_count.ToString() + "回";
		periodLabel.text = masterGachaModel.open_at + "から\n" + masterGachaModel.close_at + "まで";
		descriptionLabel.text = masterGachaModel.description;
	}

	//ガチャをひくボタンをタップで呼ばれる関数
	public void PressEvent()
	{
		Action action = () => {
			GameObject gachaManagerObject = GameObject.Find("GachaManager");
			if (gachaManagerObject == null) {
				UnityEngine.Debug.LogError("GachaManagerが存在しません。");
				return;
			}
			GachaManager gachaManager = gachaManagerObject.GetComponent<GachaManager>();
			if (gachaManager == null) {
				UnityEngine.Debug.LogError("gachaManagerアタッチされていません。");
				return;
			}
			gachaManager.GachaList.SetActive(false);
			gachaManager.GachaResult.SetActive(true);
		};
		UserProfileModel userProfileModel = UserProfile.Get();
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			UnityEngine.Debug.LogError("TitleSceneを起動してユーザー登録を行ってください。");
			return;
		}
		StartCoroutine(CommunicationManager.ConnectServer("gacha", "&user_id=" + userProfileModel.user_id + "&gacha_id=" + masterGachaModel.gacha_id, action));
	}
}