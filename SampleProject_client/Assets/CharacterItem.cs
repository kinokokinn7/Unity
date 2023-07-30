using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterItem : MonoBehaviour
{
	[SerializeField]
	Image characterAsset;

	[SerializeField]
	Image typeAsset;

	public UserCharacterModel userCharacterModel;
	private MasterCharacterModel masterCharacterModel;

	void Start()
	{
		masterCharacterModel = MasterCharacter.GetMasterCharacter(userCharacterModel.character_id);
		if (masterCharacterModel.character_id == 0) {
			UnityEngine.Debug.LogError("キャラクターのマスターデータを設定してください。");
			return;
		}

		//Resourcesから画像を読み込む場合
		Sprite characterSprite = Resources.Load<Sprite>(masterCharacterModel.asset_id);
		if (characterSprite == null) {
			UnityEngine.Debug.LogError("キャラクターの画像を設定してください。");
			return;
		}
		characterAsset.sprite = characterSprite;
		Sprite typeSprite = Resources.Load<Sprite>("CharacterTypeFrame_" + masterCharacterModel.type);
		if (typeSprite == null) {
			UnityEngine.Debug.LogError("キャラクターのタイプ画像を設定してください。");
			return;
		}
		typeAsset.sprite = typeSprite;
	}

	public void SellButtonEvent()
	{
		Action action = () => {
			GameObject characterManagerObject = GameObject.Find("CharacterManager");
			if (characterManagerObject == null) {
				UnityEngine.Debug.LogError("CharacterManagerが存在しません。");
				return;
			}
			CharacterManager characterManager = characterManagerObject.GetComponent<CharacterManager>();
			if (characterManager == null) {
				UnityEngine.Debug.LogError("CharacterManagerアタッチされていません。");
				return;
			}
			characterManager.CharacterList.SetActive(false);
			characterManager.Dialog.SetActive(true);
			characterManager.DialogSellPoint.text = "× " + masterCharacterModel.sell_point;
		};
		UserProfileModel userProfileModel = UserProfile.Get();
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			UnityEngine.Debug.LogError("TitleSceneを起動してユーザー登録を行ってください。");
			return;
		}
		StartCoroutine(CommunicationManager.ConnectServer("character_sell", "&user_id=" + userProfileModel.user_id + "&id=" + userCharacterModel.id, action));
	}
}