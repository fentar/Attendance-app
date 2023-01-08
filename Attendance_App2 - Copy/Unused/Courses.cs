using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//References
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;

public class Courses : MonoBehaviour
{
	private string conn, sqlQuery;
	IDbConnection dbconn;
	IDbCommand dbcmd;
	private IDataReader reader;

	public int Id {get; set;}
	public string Name {get; set;}
	public string Description {get; set;}
	public string Time {get; set;}

	List<Courses> cour = new List<Courses>();

	string DatabaseName = "mydb.db";

	// Start is called before the first frame update
	public void showCourses()
	{
		using (dbconn = new SqliteConnection(conn))
		{
			string filepath = Application.dataPath + "/" + DatabaseName;
			conn = "URI=file:" + filepath;

			Debug.Log("Establishing connection to: " + conn);
			dbconn = new SqliteConnection(conn);
			dbconn.Open();

			IDbCommand dbcmd = dbconn.CreateCommand();
			string sqlQuery = "SELECT id, Name, Description, Time FROM Courses";
			dbcmd.CommandText = sqlQuery;
			IDataReader reader = dbcmd.ExecuteReader();

			while (reader.Read())
			{
				var co = new Courses();
				co.Id = reader.GetInt32(0);
				co.Name = reader.GetString(1);
				co.Description = reader.GetString(2);
				co.Time = reader.GetString(3);
				cour.Add(co);

				Debug.Log(" name =" + co.Name);
			}


			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			dbconn.Close();

			Courses[] coarr = cour.ToArray();

		}

	}
	public IEnumerator GetEnumerator()  
	{  
		foreach (object o in cour)  
		{  
			if(o == null)  
			{  
				break;  
			}  
			yield return o;  
		}  
	}  

	public void showCourses2(){
		SceneManager.LoadScene("Courses");
	}
}
	
