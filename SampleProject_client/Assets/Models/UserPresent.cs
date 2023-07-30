using System;
using System.Collections.Generic;

[Serializable]
public class UserPresentModel
{
	public int present_id;
	public int item_type;
	public int item_count;
	public string description;
	public string limited_at;
}

public static class UserPresent
{
	public enum ItemType
	{
		Crystal = 1,
		CrystalFree = 2,
		FriendCoin = 3,
	}

	public static void CreateTable()
	{
		string query = "create table if not exists user_present (present_id int, item_type int, item_count int, description text, limited_at text, primary key(present_id))";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteQuery(query);
	}

	public static void Set(UserPresentModel[] user_present_model_list)
	{
		//プレゼントが取得されてもデータが残り続けないように一度ドロップする
		string dropQuery = "drop table if exists user_present";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteQuery(dropQuery);

		CreateTable();

		foreach (UserPresentModel userPresentModel in user_present_model_list) {
			string query = "insert or replace into user_present (present_id, item_type, item_count, description, limited_at) values(" +
				userPresentModel.present_id + ", " +
			userPresentModel.item_type + ", " +
			userPresentModel.item_count + ", \"" +
				userPresentModel.description + "\", \"" +
				userPresentModel.limited_at + "\")";
			sqlDB.ExecuteNonQuery(query);
		}
	}

	public static Dictionary<int, UserPresentModel> GetUserPresentList()
	{
		Dictionary<int, UserPresentModel> userPresentListModel = new Dictionary<int, UserPresentModel>();

		string query = "select * from user_present";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		DataTable dataTable = sqlDB.ExecuteQuery(query);

		foreach (DataRow dr in dataTable.Rows) {
			UserPresentModel userPresentModel = new UserPresentModel();
			userPresentModel.present_id = int.Parse(dr["present_id"].ToString());
			userPresentModel.item_type = int.Parse(dr["item_type"].ToString());
			userPresentModel.item_count = int.Parse(dr["item_count"].ToString());
			userPresentModel.description = dr["description"].ToString();
			userPresentModel.limited_at = dr["limited_at"].ToString();
			userPresentListModel.Add(userPresentModel.present_id, userPresentModel);
		}

		return userPresentListModel;
	}
}