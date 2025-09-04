using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
	[SerializeField]
	Text costLabel;

	[SerializeField]
	Text amountLabel;

	public MasterShopModel masterShopModel;

	void Start()
	{
		costLabel.text = masterShopModel.cost + " 円";
		amountLabel.text = "× " + masterShopModel.amount;
	}

	public void ShopButtonEvent()
	{
		UserProfileModel userProfileModel = UserProfile.Get();
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			UnityEngine.Debug.LogError("TitleSceneを起動してユーザー登録を行ってください。");
			return;
		}
		//決済処理を行う

		//決済処理後
		Action action = () => {
			GameObject shopManagerObject = GameObject.Find("ShopManager");
			if (shopManagerObject == null) {
				UnityEngine.Debug.LogError("ShopManagerが存在しません。");
				return;
			}
			ShopManager shopManager = shopManagerObject.GetComponent<ShopManager>();
			if (shopManager == null) {
				UnityEngine.Debug.LogError("ShopManagerがアタッチされていません。");
				return;
			}
			shopManager.Dialog.SetActive(true);
			shopManager.DialogCrystalAmount.text = "× " + masterShopModel.amount;
		};
	
		StartCoroutine(CommunicationManager.ConnectServer("shop", "&user_id=" + userProfileModel.user_id + "&shop_id=" + masterShopModel.shop_id, action));
	}
}