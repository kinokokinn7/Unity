using System;

[Serializable]
public class UserProfileModel
{
	public string user_id;
	public string user_name;
	public int crystal;
	public int crystal_free;
	public int friend_coin;
	public int tutorial_progress;
}

public static class UserProfile
{
	public const int TUTORIAL_START = 0;
	public const int TUTORIAL_QUEST = 10;
	public const int TUTORIAL_GACHA = 20;
	public const int TUTORIAL_FINISH = 999;

	public static void CreateTable()
	{
		string query = "create table if not exists user_profile (user_id text, user_name text, crystal int, crystal_free int, friend_coin int, tutorial_progress int, primary key(user_id))";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteQuery(query);
	}

	public static void Set(UserProfileModel user_profile)
	{
		string query = "insert or replace into user_profile (user_id, user_name, crystal, crystal_free, friend_coin, tutorial_progress) values (\"" + user_profile.user_id + "\", \"" + user_profile.user_name + "\", " + user_profile.crystal + ", " + user_profile.crystal_free + ", " + user_profile.friend_coin + ", " + user_profile.tutorial_progress + ")";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteNonQuery(query);
	}

	public static UserProfileModel Get()
	{
		string query = "select * from user_profile";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		DataTable dataTable = sqlDB.ExecuteQuery(query);
		UserProfileModel userProfileModel = new UserProfileModel();
		foreach (DataRow dr in dataTable.Rows) {
			userProfileModel.user_id = dr["user_id"].ToString();
			userProfileModel.user_name = dr["user_name"].ToString();
			userProfileModel.crystal = int.Parse(dr["crystal"].ToString());
			userProfileModel.crystal_free = int.Parse(dr["crystal_free"].ToString());
			userProfileModel.friend_coin = int.Parse(dr["friend_coin"].ToString());
			userProfileModel.tutorial_progress = int.Parse(dr["tutorial_progress"].ToString());
		}
		return userProfileModel;
	}
}