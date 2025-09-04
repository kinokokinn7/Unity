using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;

[Serializable]
public class ResponseObjects
{
	public int master_data_version;
	public UserProfileModel user_profile;
	public UserLoginModel user_login;
	public UserQuestModel[] user_quest;
	public UserCharacterModel[] user_character;
	public UserPresentModel[] user_present;
	public MasterLoginItemModel[] master_login_item;
	public MasterQuestModel[] master_quest;
	public MasterCharacterModel[] master_character;
	public MasterGachaModel[] master_gacha;
	public MasterShopModel[] master_shop;
}

public class CommunicationManager : MonoBehaviour
{
	//接続先のサーバーのアドレスを設定
	const string URL = "";
	private const string ERROR_MASTER_DATA_UPDATE = "1";
	private const string ERROR_DB_UPDATE = "2";
	private const string ERROR_INVALID_DATA = "3";
	private const string ERROR_INVALID_SCHEDULE = "4";
	private const string ERROR_COST_SHORTAGE = "5";
	private const string ERROR_FORCE_UPDATE = "6";

	public static IEnumerator ConnectServer(string endpoint, string paramater, Action action = null)
	{
		UnityWebRequest unityWebRequest = UnityWebRequest.Get(URL + endpoint + "?client_master_version=" + LocalDataManager.GetMasterDataVersion() + paramater);
		yield return unityWebRequest.SendWebRequest();

		if (!string.IsNullOrEmpty(unityWebRequest.error)) {
			//エラー
			UnityEngine.Debug.LogError(unityWebRequest.error);
			if (unityWebRequest.error == "Cannot resolve destination host") {
				UnityEngine.Debug.LogError("CommunicationManagerクラスのURLまたはエンドポイントを正しく設定してください");
			}
		}
		//レスポンスを取得
		string text = unityWebRequest.downloadHandler.text;
		UnityEngine.Debug.Log("レスポンス : " + text);

		if (text.All(char.IsNumber)) {
			//エラーの場合
			switch (text) {
				case ERROR_MASTER_DATA_UPDATE:
					UnityEngine.Debug.Log("マスターデータを更新します。");
					GameObject masterDataDialog = Instantiate(Resources.Load("MasterDataDialog")) as GameObject;
					GameObject canvas = GameObject.Find("Canvas");
					masterDataDialog.transform.SetParent(canvas.transform);
					masterDataDialog.transform.localPosition = Vector3.zero;
					break;
				case ERROR_DB_UPDATE:
					UnityEngine.Debug.LogError("サーバーでエラーが発生しました。[データベース更新エラー]");
					break;
				case ERROR_INVALID_DATA:
					UnityEngine.Debug.LogError("サーバーでエラーが発生しました。[不正なデータ]");
					break;
				case ERROR_INVALID_SCHEDULE:
					UnityEngine.Debug.LogError("サーバーでエラーが発生しました。[期間外]");
					break;
				case ERROR_COST_SHORTAGE:
					UnityEngine.Debug.LogError("サーバーでエラーが発生しました。[アイテム不足]");
					break;
				default:
					break;
			}
		} else {
			ResponseObjects responseObjects = JsonUtility.FromJson<ResponseObjects>(text);

			if (responseObjects.master_data_version != 0) {
				LocalDataManager.SetMasterDataVersion(responseObjects.master_data_version);
			}
			//SQLiteへ保存
			if (!string.IsNullOrEmpty(responseObjects.user_profile.user_id)) {
				UserProfile.Set(responseObjects.user_profile);
			}
			if (!string.IsNullOrEmpty(responseObjects.user_login.user_id)) {
				UserLogin.Set(responseObjects.user_login);
			}
			if (responseObjects.user_quest != null) {
				UserQuest.Set(responseObjects.user_quest);
			}
			if (responseObjects.user_character != null) {
				UserCharacter.Set(responseObjects.user_character);
			}
			if (responseObjects.user_present != null) {
				UserPresent.Set(responseObjects.user_present);
			}
			if (responseObjects.master_login_item != null) {
				MasterLoginItem.Set(responseObjects.master_login_item);
			}
			if (responseObjects.master_quest != null) {
				MasterQuest.Set(responseObjects.master_quest);
			}
			if (responseObjects.master_character != null) {
				MasterCharacter.Set(responseObjects.master_character);
			}
			if (responseObjects.master_gacha != null) {
				MasterGacha.Set(responseObjects.master_gacha);
			}
			if (responseObjects.master_shop != null) {
				MasterShop.Set(responseObjects.master_shop);
			}

			if (action != null) {
				action();
				action = null;
			}
		}
	}
}