using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Mono.Data.Sqlite; 
using System.Data; 
using System;

public class ShowStudents2 : MonoBehaviour
{

	public Text studentsss;
	//studentsss = gameObject.GetComponent<Text>();
	// Start is called before the first frame update
	void Start()
	{
		string conn = "URI=file:" + Application.dataPath + "/mydb.db"; //Path to database.
		IDbConnection dbconn;
		dbconn = (IDbConnection) new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.



		IDbCommand dbcmd = dbconn.CreateCommand();
		string sqlQuery = "SELECT Name FROM Student";
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();
		while (reader.Read())
		{
			//int value = reader.GetInt32(0);
			string name = reader.GetString(0);
			//int rand = reader.GetInt32(2);

			studentsss.text = " Name = "+name+"";
		}
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}
	public void click(){
		
	}
	// Update is called once per frame
	void Update()
	{

	}
}
