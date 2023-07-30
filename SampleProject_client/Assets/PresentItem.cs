using System;
using UnityEngine;
using UnityEngine.UI;

public class PresentItem : MonoBehaviour
{
	public UserPresentModel userPresentModel;

	[SerializeField]
	Image typeSprite;

	[SerializeField]
	Text amountLabel;

	[SerializeField]
	Text descriptionLabel;

	[SerializeField]
	Text limitedLabel;

	void Start()
	{
		//商品Imageの設定
		if (userPresentModel.item_type == (int)UserPresent.ItemType.Crystal || userPresentModel.item_type == (int)UserPresent.ItemType.CrystalFree) {
			typeSprite.sprite = Resources.Load<Sprite>("Crystal");
		} else if (userPresentModel.item_type == (int)UserPresent.ItemType.FriendCoin) {
			typeSprite.sprite = Resources.Load<Sprite>("FriendCoin");
		}

		amountLabel.text = "x" + userPresentModel.item_count.ToString();
		descriptionLabel.text = userPresentModel.description.ToString();
		limitedLabel.text = userPresentModel.limited_at + "まで";
	}

	public void PresentButtonEvent()
	{
		Action action = () => {
			GameObject presentManagerObject = GameObject.Find("PresentManager");
			if (presentManagerObject == null) {
				UnityEngine.Debug.LogError("PresentManagerが存在しません。");
				return;
			}
			PresentManager presentManager = presentManagerObject.GetComponent<PresentManager>();
			if (presentManager == null) {
				UnityEngine.Debug.LogError("PresentManagerアタッチされていません。");
				return;
			}
			presentManager.Dialog.SetActive(true);
		};
		UserProfileModel userProfileModel = UserProfile.Get();
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			UnityEngine.Debug.LogError("TitleSceneを起動してユーザー登録を行ってください。");
			return;
		}
		StartCoroutine(CommunicationManager.ConnectServer("present", "&user_id=" + userProfileModel.user_id + "&present_id=" + userPresentModel.present_id, action));
	}
}