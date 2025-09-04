using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
	[SerializeField]
	Text userID;

	[SerializeField]
	GameObject loginBonusDialog;

	private UserProfileModel userProfileModel;

	void Awake()
	{
		string DBPath = Application.persistentDataPath + "/Service.db";

		if (!File.Exists(DBPath)) {
			File.Create(DBPath);
		}

		UserProfile.CreateTable();
		UserLogin.CreateTable();
		UserQuest.CreateTable();
		UserCharacter.CreateTable();
		UserPresent.CreateTable();
		MasterLoginItem.CreateTable();
		MasterQuest.CreateTable();
		MasterCharacter.CreateTable();
		MasterGacha.CreateTable();
		MasterShop.CreateTable();
	}

	void Start()
	{
		loginBonusDialog.SetActive(false);

		userProfileModel = UserProfile.Get();
		if (!string.IsNullOrEmpty(userProfileModel.user_id)) {
			userID.text = "ID : " + userProfileModel.user_id;
		}
	}

	public void LoginButtonEvent()
	{
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			Action action = () => {
				UnityEngine.Debug.Log("登録が完了しました");
			};
			StartCoroutine(CommunicationManager.ConnectServer("registration", "", action));
		} else {
			Action action = () => {
				loginBonusDialog.SetActive(true);
			};
			StartCoroutine(CommunicationManager.ConnectServer("login", "&user_id=" + userProfileModel.user_id, action));
		}
	}
}