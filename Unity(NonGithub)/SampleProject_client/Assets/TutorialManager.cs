using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	[SerializeField]
	Text tutorialText;

	[SerializeField]
	GameObject tutorialButton;

	private UserProfileModel userProfileModel;

	void Start()
    {
		tutorialButton.SetActive(false);

		userProfileModel = UserProfile.Get();
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			tutorialText.text = "TitleSceneを起動して\nユーザー登録を\n行ってください。";
		} else {
			if (UserProfile.TUTORIAL_QUEST <= userProfileModel.tutorial_progress) {
				tutorialText.text = "チュートリアルは\n完了しています。";
			} else {
				tutorialText.text = "これはチュートリアルの\n説明文です。";
				tutorialButton.SetActive(true);
			}
		}
	}

	public void TutorialButtonEvent()
	{
		if (string.IsNullOrEmpty(userProfileModel.user_id)) {
			return;
		}

		if (UserProfile.TUTORIAL_QUEST <= userProfileModel.tutorial_progress) {
			return;
		}

		Action action = () => {
			tutorialButton.SetActive(false);
			tutorialText.text = "チュートリアルが\n完了しました。";
		};
		StartCoroutine(CommunicationManager.ConnectServer("quest_tutorial", "&user_id=" + userProfileModel.user_id, action));
	}
}