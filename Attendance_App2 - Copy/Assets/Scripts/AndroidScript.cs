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

public class AndroidScript : MonoBehaviour
{
	private string conn, sqlQuery;
	IDbConnection dbconn;
	IDbCommand dbcmd;
	private IDataReader reader;
	public InputField t_name, t_Description, t_Phone, t_Time;
	public Toggle t_attend, t_paid;



	string DatabaseName = "mydb.db";
	// Start is called before the first frame update
	void Start()
	{
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


	}
	//Insert calls
	public void insert_button()
	{
		insert_function(t_name.text, t_Description.text, t_Time.text);

	}
	public void insert_button2()
	{
		insert_student(t_name.text, t_Description.text, t_Phone.text, t_attend.isOn, t_paid.isOn);

	}
	public void insert_button3()
	{
		
		edit_course(t_name.text, t_Description.text,  t_Time.text);

	}
	public void insert_button4()
	{

	edit_student(t_name.text, t_Description.text,  t_Phone.text, t_attend.isOn, t_paid.isOn);

	}


	//Insert Course To Database
	private void insert_function(string name, string description, string time)
	{
		using (dbconn = new SqliteConnection(conn))
		{
			//DateTime now = DateTime.Now; //present date and time, change it to any string
			//string tim = now.ToString("F"); //convert to string
			int courseid;

			dbconn.Open(); //Open connection to the database.
			Debug.Log("Connection open  ");
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("insert into Courses (Name, Description, Time) values (\"{0}\",\"{1}\",\"{2}\")", name, description, time);
			//the id field is autoincremented so it's added on its own
			dbcmd.CommandText = sqlQuery;
			dbcmd.ExecuteScalar();
			dbconn.Close();

			dbconn.Open();
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("select id from Courses where Name = '" + name)+"'";//get the id from the just added course
			dbcmd.CommandText = sqlQuery;
			courseid = Convert.ToInt32(dbcmd.ExecuteScalar());
			PlayerPrefs.SetInt("courseid", courseid);//save the course id for the students to be added into
			dbconn.Close();
		}

		Debug.Log("Insert Done  ");
		SceneManager.LoadScene("Student");

	}

	private void insert_student(string name, string email, string phone, bool attend, bool paid)
	{
		string att = attend.ToString();//take the boolean variables and convert it to string so it can be saved in the SQLite database
		string pay = paid.ToString();
		int cid = PlayerPrefs.GetInt("courseid");//get the course's id to add to the students as c_id

		using (dbconn = new SqliteConnection(conn))
		{
			dbconn.Open(); //Open connection to the database.
			Debug.Log("Connection open  ");
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("insert into Student (Name, Email, Phone, Attended, Paid, c_id) values (\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\")",
			name, email, phone, att, pay, cid);
			dbcmd.CommandText = sqlQuery;
			dbcmd.ExecuteScalar();
			dbconn.Close();
		}

		Debug.Log("Insert Done  ");
		
		//set the UI fields to blank to add the next student
		t_name.text = "";
		t_Description.text ="";
		t_Phone.text ="";
		t_attend.isOn = true;
		t_paid.isOn = true;

	}

	private void edit_course(string name, string description, string time)
	{
		using (dbconn = new SqliteConnection(conn))
		{
			
			//DateTime now = DateTime.Now; //present date and time, change it to any string
			//string tim = now.ToString("F"); //convert to string
			int courseid = PlayerPrefs.GetInt("courseid");//get the selected course's id

			dbconn.Open(); //Open connection to the database.
			Debug.Log("Connection open  ");
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("update Courses SET Name = '" + name +"', Description = '"+ description +"', Time = '"+ time +
				"' where id = "+ courseid);
			dbcmd.CommandText = sqlQuery;
			dbcmd.ExecuteScalar();
			dbconn.Close();

		}
	
		Debug.Log("Edit Complete  ");
		SceneManager.LoadScene("Courses");

	}

	private void edit_student(string name, string email, string phone, bool attend, bool paid)
	{
		using (dbconn = new SqliteConnection(conn))
		{
			string att = attend.ToString();//take the boolean variable and convert it to string so it can be saved in the SQLite database
			string pay = paid.ToString();
			int studentid = PlayerPrefs.GetInt("studentid");//get the selected student's id

			dbconn.Open(); //Open connection to the database.
			Debug.Log("Connection open  ");
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("update Student SET Name = '" + name +"', Email = '"+ email +"', Phone = '"+ phone +"', Attended = '"+ 
			att +"', Paid = '" + pay + "' where id = "+ studentid);
			dbcmd.CommandText = sqlQuery;
			dbcmd.ExecuteScalar();
			dbconn.Close();

		}
	
		Debug.Log("Edit Complete  ");
		SceneManager.LoadScene("ShowStudents");

	}


	// Update is called once per frame
	void Update()
	{
	
	}
}