using System;

[Serializable]
public class UserLoginModel
{
	public string user_id;
	public int login_day;
	public string last_login_at;
}

public static class UserLogin
{
	public static void CreateTable()
	{
		//前チャプタよりカラムを追加
		string query = "create table if not exists user_login (user_id text, login_day int, last_login_at text, primary key(user_id))";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteQuery(query);
	}

	public static void Set(UserLoginModel user_login)
	{
		string query = "insert or replace into user_login (user_id, login_day, last_login_at) values (\"" + user_login.user_id + "\", " + user_login.login_day + ", \"" + user_login.last_login_at + "\")";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		sqlDB.ExecuteNonQuery(query);
	}

	public static UserLoginModel Get()
	{
		string query = "select * from user_login";
		SqliteDatabase sqlDB = new SqliteDatabase("Service.db");
		DataTable dataTable = sqlDB.ExecuteQuery(query);
		UserLoginModel userLoginModel = new UserLoginModel();
		foreach (DataRow dr in dataTable.Rows) {
			userLoginModel.user_id = dr["user_id"].ToString();
			userLoginModel.login_day = int.Parse(dr["login_day"].ToString());
			userLoginModel.last_login_at = dr["last_login_at"].ToString();
		}

		return userLoginModel;
	}
}