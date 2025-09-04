using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	[SerializeField]
	GameObject contents;

	[SerializeField]
	GameObject shopItemPrefab;

	[SerializeField]
	public GameObject Dialog;

	[SerializeField]
	public Text DialogCrystalAmount;

	private int contentsCount = 2;
	private float contentsWidth = 300.0f;
	private float contentsHeight = 180.0f;

	void Start()
    {
		Dictionary<string, MasterShopModel> masterShopModelList = MasterShop.GetMasterShopList();
		if (masterShopModelList.Count == 0) {
			UnityEngine.Debug.LogError("ショップのマスターデータがありません。");
			return;
		}
		int i = 0;
		foreach (MasterShopModel masterShopModel in masterShopModelList.Values) {
			GameObject shopItemObject = Instantiate(shopItemPrefab) as GameObject;
			shopItemObject.transform.SetParent(contents.transform);
			shopItemObject.transform.localPosition = new Vector3(150.0f + i % contentsCount * contentsWidth, -100.0f - i / contentsCount * contentsHeight, 0.0f);
			ShopItem shopItem = shopItemObject.GetComponent<ShopItem>();
			if (shopItem == null) {
				UnityEngine.Debug.LogError("ShopItemがアタッチされていません。");
				break;
			}

			shopItem.masterShopModel = masterShopModel;
			shopItemObject.SetActive(true);
			shopItemPrefab.SetActive(false);
			i++;
		}
	}
}