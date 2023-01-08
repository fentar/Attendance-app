using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;

public class StudentTable : MonoBehaviour {
	public RectTransform tableView;
	public RectTransform view;
	public GameObject StudentCell;

	private string conn, sqlQuery;
	IDbConnection dbconn;
	IDbCommand dbcmd;
	private IDataReader reader;

	string DatabaseName = "mydb.db";

	private int cells, thiscourseid;

	void Start () {
		
			thiscourseid = PlayerPrefs.GetInt("courseid");//get the selected course's id
			cells = 1;   //this is used later for the size of the view

			//string filepath = Application.dataPath + "/" + DatabaseName;
			//conn = "URI=file:" + filepath;

			string dbPath;
			#if UNITY_EDITOR
			dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
			#else
			// check if file exists in Application.persistentDataPath
			var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
			if (!File.Exists(filepath))
			{
			Debug.Log("Database not in Persistent path");
			// if it doesn't ->
			// open StreamingAssets directory and load the db ->

			#if UNITY_ANDROID
			var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
			while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
			// then save to Application.persistentDataPath
			File.WriteAllBytes(filepath, loadDb.bytes);

			#endif

			}

			dbPath = filepath;
			#endif
			conn = "URI=file:" + dbPath;

			dbconn = new SqliteConnection(conn);
			dbconn.Open();

			//we want to select all the students that have the course_id of the selected course
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("SELECT * FROM Student where c_id = "+ thiscourseid);
			dbcmd.CommandText = sqlQuery;
			IDataReader reader = dbcmd.ExecuteReader();


			while (reader.Read())//read the query's answer and create a cell for each row
			{
				GameObject cell = Instantiate(StudentCell);
				cell.transform.SetParent(view.transform, false);

				ShowStudents tableViewCellScript = cell.GetComponent<ShowStudents>();
				tableViewCellScript.Id = reader.GetInt32(0);
				tableViewCellScript.Name = reader.GetString(1);
				tableViewCellScript.Email = reader.GetString(2);
				tableViewCellScript.Phone = reader.GetString(3);
				tableViewCellScript.Attended = reader.GetString(4);
				tableViewCellScript.Paid = reader.GetString(5);
				tableViewCellScript.c_Id = reader.GetInt32(6);
				cells++;

			}

			//this is to set the size of the view according to how many lines there will be
			float height = tableView.rect.height;

			if (height < 100 * cells)
			{
				height = 100 * cells;
			}

			view.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			view.SetPositionAndRotation(new Vector3(0, -height/2 , 0), Quaternion.Euler(0, 0, 0));

			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			dbconn.Close();



	}

	// Update is called once per frame
	void Update () {
		
	}
}
