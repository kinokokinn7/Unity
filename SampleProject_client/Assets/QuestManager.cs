using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
	[SerializeField]
	GameObject questStartUI;

	[SerializeField]
	Text questNameText;

	[SerializeField]
	GameObject questUI;

	[SerializeField]
	Slider questSlider;

	[SerializeField]
	Animation enemyAnimation;

	[SerializeField]
	GameObject questEndUI;

	[SerializeField]
	Text scoreText;

	[SerializeField]
	Text clearTimeText;

	[SerializeField]
	Text resultText;

	private UserProfileModel userProfileModel;
	//スコア計算用
	private int score = 0;
	//クリアタイム計算用
	private float time = 0;

	void Start()
	{
		questStartUI.SetActive(true);
		questUI.SetActive(false);
		questEndUI.SetActive(false);

		userProfileModel = UserProfile.Get();
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			UnityEngine.Debug.LogError("TitleSceneを起動してユーザー登録を行ってください。");
		}

		MasterQuestModel masterQuestModel = MasterQuest.GetMasterQuest(1); //quest_idが1の場合
		if (masterQuestModel.quest_id == 0) {
			UnityEngine.Debug.LogError("MasterQuestのマスターデータを設定してください。");
			return;
		}
		questNameText.text = masterQuestModel.quest_name;
		questSlider.value = 10;
	}

	void Update()
	{
		if (!questUI.activeSelf) {
			return;
		}

		//クリアタイム計算
		time += Time.deltaTime * 100;

		if (Input.GetMouseButton(0)) {
			questSlider.value -= 1;
			//スコア計算
			score += 100 * UnityEngine.Random.Range(1, 100);
			enemyAnimation.Play();
		}

		if (questSlider.value <= 0) {
			questUI.SetActive(false);
			finishButtonEvent();
		}
	}

	public void StartButtonEvent()
	{
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			UnityEngine.Debug.LogError("TitleSceneを起動してユーザー登録を行ってください。");
			return;
		}

		Action action = () => {
			questStartUI.SetActive(false);
			questUI.SetActive(true);
			questEndUI.SetActive(false);
		};
		StartCoroutine(CommunicationManager.ConnectServer("quest_start", "&user_id=" + userProfileModel.user_id + "&quest_id=1", action));
	}

	private void finishButtonEvent()
	{
		Action action = () => {
			UserQuestModel userQuestModel = UserQuest.Get(1);
			if (userQuestModel.quest_id == 0) {
				UnityEngine.Debug.LogError("ユーザーのクエストの情報がありません。");
				return;
			}
			scoreText.text = userQuestModel.score.ToString();
			clearTimeText.text = userQuestModel.clear_time.ToString();
			resultText.text = "合格";
			questStartUI.SetActive(false);
			questUI.SetActive(false);
			questEndUI.SetActive(true);
		};
		StartCoroutine(CommunicationManager.ConnectServer("quest_end", "&user_id=" + userProfileModel.user_id + "&quest_id=1&score="+ score + "&clear_time=" + Mathf.CeilToInt(time), action));
	}
}
