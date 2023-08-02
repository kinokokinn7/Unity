using UnityEngine;

public static class LocalDataManager
{
	public static void SetMasterDataVersion(int version)
	{
		PlayerPrefs.SetInt("master_data_version", version);
	}

	public static int GetMasterDataVersion()
	{
		return PlayerPrefs.GetInt("master_data_version", 0);
	}
}