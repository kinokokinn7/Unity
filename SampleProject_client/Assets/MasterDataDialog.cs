using System;
using UnityEngine;
using UnityEngine.UI;

public class MasterDataDialog : MonoBehaviour
{
	[SerializeField]
	Text messageLabel;

	[SerializeField]
	GameObject button;

	public void ButtonEvent()
	{
		button.SetActive(false);

		Action action = () => {
			messageLabel.text = "マスターデータを更新しました。";
			Destroy(gameObject);
		};
		StartCoroutine(CommunicationManager.ConnectServer("master_data", "", action));
	}
}