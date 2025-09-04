using System;
using System.Collections.Generic;

[Serializable]
public class MasterGachaModel
{
	public int gacha_id;
	public string banner_id;
	public int cost_type;
	public int cost_amount;
	public int draw_count;
	public string open_at;
	public string close_at;
	public string description;
}

public static class MasterGacha
{
	public enum CostType
	{
		Crystal = 1,
		CrystalFree = 2,
		FriendCoin = 3,
	}

	public static void CreateTable()
	{
		string query = "create table if not exists master_gacha (gacha_id int, banner_id text, cost_type int, cost_amount int, draw_count int, open_at text, close_at text, description text, primary key(gacha_id))";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteQuery(query);
	}

	public static void Set(MasterGachaModel[] master_gacha_model_list)
	{
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		foreach (MasterGachaModel masterGachaModel in master_gacha_model_list) {
			string query = "insert or replace into master_gacha (gacha_id, banner_id, cost_type, cost_amount, draw_count, open_at, close_at, description) values(" +
				masterGachaModel.gacha_id + ", \"" +
				masterGachaModel.banner_id + "\", " +
				masterGachaModel.cost_type + ", " +
				masterGachaModel.cost_amount + ", " +
				masterGachaModel.draw_count + ", \"" +
				masterGachaModel.open_at + "\", \"" +
				masterGachaModel.close_at + "\", \"" +
				masterGachaModel.description + "\")";
			sqlDB.ExecuteNonQuery(query);
		}
	}

	public static Dictionary<int, MasterGachaModel> GetMasterGachaList()
	{
		Dictionary<int, MasterGachaModel> masterGachaListModel = new Dictionary<int, MasterGachaModel>();
		string query = "select * from master_gacha";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		DataTable dataTable = sqlDB.ExecuteQuery(query);
		foreach (DataRow dr in dataTable.Rows) {
			MasterGachaModel masterGachaModel = new MasterGachaModel();
			masterGachaModel.gacha_id = int.Parse(dr["gacha_id"].ToString());
			masterGachaModel.banner_id = dr["banner_id"].ToString();
			masterGachaModel.cost_type = int.Parse(dr["cost_type"].ToString());
			masterGachaModel.cost_amount = int.Parse(dr["cost_amount"].ToString());
			masterGachaModel.draw_count = int.Parse(dr["draw_count"].ToString());
			masterGachaModel.open_at = dr["open_at"].ToString();
			masterGachaModel.close_at = dr["close_at"].ToString();
			masterGachaModel.description = dr["description"].ToString();
			masterGachaListModel.Add(masterGachaModel.gacha_id, masterGachaModel);
		}

		return masterGachaListModel;
	}
}