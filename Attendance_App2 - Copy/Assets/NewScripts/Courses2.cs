using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;

public class Courses2 : MonoBehaviour
{
	//This script is for the appearance of the table Courses

	private string conn, sqlQuery;
	IDbConnection dbconn;
	IDbCommand dbcmd;
	private IDataReader reader;
	string DatabaseName = "mydb.db";

	//these are the input fields in the UI
	public Text t_Id;
	public Text t_Name;
	public Text t_Description;
	public Text t_Time;

	//these are the private variables that will be assigned to the UI fields
	private int id;
	private string named;
	private string description;
	private string time;

	//These methods take the UI fields and assigns the corresponding values to them
	//Only the Name field will be shown

	public int Id {
		//get;
		set{
			id = value;
			//t_Id.text = id.ToString();
		}
	}
		
	public string Name {
		//get; 
		set{
			named = value;
			t_Name.text = named;
		}
	}
	public string Description {
		//get; 
		set{
			description = value;
			//t_Description.text = description;
		}
	}
	public string Time {
		//get; 
		set{
			time = value;
			//t_Time.text = time;
		}
	}

	public void delete_course(){
		
		//take the selected row's id and delete the course that has that id
		using (dbconn = new SqliteConnection(conn))
		{
			dbconn.Open();
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("delete from Courses where id = '" + this.id)+"'";
			dbcmd.CommandText = sqlQuery;
			dbcmd.ExecuteScalar();
			dbconn.Close();
			Debug.Log("Course Deleted  ");

			//Also delete the students that were enrolled to this course
			dbconn.Open();
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("delete from Student where c_id = '" + this.id)+"'";
			dbcmd.CommandText = sqlQuery;
			dbcmd.ExecuteScalar();
			dbconn.Close();

			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.name);//reload the scene to show changes

		}

	}

	public void edit_course(){
		//save the selected row's data and go to the scene EditCourse
		//so the EditCourse Scene will show the selected Course's data
		PlayerPrefs.SetInt("courseid", id);
		PlayerPrefs.SetString("coursename", named);
		PlayerPrefs.SetString("coursedesc", description);
		PlayerPrefs.SetString("coursetime", time);
		SceneManager.LoadScene("EditCourse");
	}

	public void show_students(){
		//save this course's id and go to scene ShowStudents to display the students
		PlayerPrefs.SetInt("courseid", id);
		SceneManager.LoadScene("ShowStudents");
	}

    // Start is called before the first frame update
    void Start()
    {
		
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
    }

    // Update is called once per frame
    void Update()
    {
	
    }
}
