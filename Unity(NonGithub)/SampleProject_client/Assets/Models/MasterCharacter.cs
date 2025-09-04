using System;

[Serializable]
public class MasterCharacterModel
{
	public int character_id;
	public string asset_id;
	public string character_name;
	public int rarity;
	public int type;
	public int sell_point;
}

public static class MasterCharacter
{
	public static void CreateTable()
	{
		string query = "create table if not exists master_character (character_id int, asset_id text, character_name text, rarity int, type int, sell_point int, primary key(character_id))";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteQuery(query);
	}

	public static void Set(MasterCharacterModel[] master_character_model_list)
	{
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		foreach (MasterCharacterModel masterCharacterModel in master_character_model_list) {
			string query = "insert or replace into master_character (character_id, asset_id, character_name, rarity, type, sell_point) values(" +
				masterCharacterModel.character_id + ", \"" +
				masterCharacterModel.asset_id + "\", \"" +
				masterCharacterModel.character_name + "\", " +
				masterCharacterModel.rarity + ", " +
				masterCharacterModel.type + ", " +
				masterCharacterModel.sell_point + ")";
			sqlDB.ExecuteNonQuery(query);
		}
	}

	public static MasterCharacterModel GetMasterCharacter(int character_id)
	{
		MasterCharacterModel masterCharacterModel = new MasterCharacterModel();
		string query = "select * from master_character where character_id = " + character_id;
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		DataTable dataTable = sqlDB.ExecuteQuery(query);
		foreach (DataRow dr in dataTable.Rows) {
			masterCharacterModel.character_id = int.Parse(dr["character_id"].ToString());
			masterCharacterModel.asset_id = dr["asset_id"].ToString();
			masterCharacterModel.character_name = dr["character_name"].ToString();
			masterCharacterModel.rarity = int.Parse(dr["rarity"].ToString());
			masterCharacterModel.type = int.Parse(dr["type"].ToString());
			masterCharacterModel.sell_point = int.Parse(dr["sell_point"].ToString());
		}

		return masterCharacterModel;
	}
}