using System;

[Serializable]
public class MasterLoginItemModel
{
	public int login_day;
	public int item_type;
	public int item_count;
}

public static class MasterLoginItem
{
	public enum ItemType
	{
		Crystal = 1,
		CrystalFree = 2,
		FriendCoin = 3,
	}

	public static void CreateTable()
	{
		string query = "create table if not exists master_login_item (login_day int, item_type int, item_count int, primary key(login_day))";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteQuery(query);
	}

	public static void Set(MasterLoginItemModel[] master_login_item_model_list)
	{
		foreach (MasterLoginItemModel masterLoginItemModel in master_login_item_model_list) {
			string query = "insert or replace into master_login_item (login_day, item_type, item_count) values(" +
				masterLoginItemModel.login_day + ", " +
				masterLoginItemModel.item_type + ", " +
				masterLoginItemModel.item_count + ")";
			SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
			sqlDB.ExecuteNonQuery(query);
		}
	}

	public static MasterLoginItemModel GetMasterLoginItem(int login_day)
	{
		MasterLoginItemModel masterLoginItemModel = new MasterLoginItemModel();
		string query = "select * from master_login_item where login_day = " + login_day.ToString();
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		DataTable dataTable = sqlDB.ExecuteQuery(query);
		foreach (DataRow dr in dataTable.Rows) {
			masterLoginItemModel.login_day = int.Parse(dr["login_day"].ToString());
			masterLoginItemModel.item_type = int.Parse(dr["item_type"].ToString());
			masterLoginItemModel.item_count = int.Parse(dr["item_count"].ToString());
		}

		return masterLoginItemModel;
	}
}