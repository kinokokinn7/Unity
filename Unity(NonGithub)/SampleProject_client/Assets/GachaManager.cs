using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
	[SerializeField]
	public GameObject GachaList;

	[SerializeField]
	GameObject contents;

	[SerializeField]
	GameObject gachaItemPrefab;

	[SerializeField]
	public GameObject GachaResult;

	private float contentsWidth = 400.0f;

	void Start()
	{
		gachaItemPrefab.SetActive(false);
		GachaResult.SetActive(false);

		Dictionary<int, MasterGachaModel> masterGachaModelList = MasterGacha.GetMasterGachaList();
		if (masterGachaModelList.Count == 0) {
			UnityEngine.Debug.LogError("master_gachaのデータが取得できませんでした。マスターデータを確認してください。");
			return;
		}

		int i = 0;
		foreach (MasterGachaModel masterGachaModel in masterGachaModelList.Values) {
			GameObject gachaItemObject = Instantiate(gachaItemPrefab) as GameObject;
			gachaItemObject.transform.SetParent(contents.transform);
			gachaItemObject.transform.localPosition = new Vector3(0.0f + i * contentsWidth, 0.0f, 0.0f);
			GachaItem gachaItem = gachaItemObject.GetComponent<GachaItem>();
			if (gachaItem == null) {
				UnityEngine.Debug.LogError("GachaItemがアタッチされていません。");
				break;
			}
			gachaItem.masterGachaModel = masterGachaModel;
			gachaItemObject.SetActive(true);
			i++;
		}
	}
}