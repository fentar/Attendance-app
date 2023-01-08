using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;

public class ShowStudents : MonoBehaviour
{
	//This script is for the appearance of the table Students

	private string conn, sqlQuery;
	IDbConnection dbconn;
	IDbCommand dbcmd;
	private IDataReader reader;
	string DatabaseName = "mydb.db";

	//these are the input fields in the UI
	public Text t_Name;
	public Text t_Email;
	public Text t_Phone;
	public Toggle t_Attended;
	public Toggle t_Paid;

	//these are the private variables that will be assigned to the UI fields
	private int id;
	private string named;
	private string email;
	private string phone;
	private string attended;
	private string paid;
	private int c_id;

	//These methods take the UI fields and assigns the corresponding values to them
	//Only the Name field will be shown

	public int Id {
		//get;
		set{
			id = value;
		}
	}
	public string Name {
		//get; 
		set{
			named = value;
			t_Name.text = named;
		}
	}
	public string Email {
		//get; 
		set{
			email = value;
			//t_Email.text = email;
		}
	}
	public string Phone {
		//get; 
		set{
			phone = value;
			//t_Phone.text = phone;
		}
	}
	public string Attended {
		//get; 
		set{
			attended = value;
			//if (attended == "True"){
			//	t_Attended.isOn = true;}
			//else{
			//	t_Attended.isOn = false;	}		
		}
	}
	public string Paid {
		//get; 
		set{
			paid = value;
			//if (paid == "True"){
			//	t_Paid.isOn = true;}
			//else{
			//	t_Paid.isOn = false;	}		
		}
	}
	public int c_Id {
		//get;
		set{
			c_id = value;
		}
	}

	public void delete_student(){

		//take the selected row's id and delete the student that has that id
		using (dbconn = new SqliteConnection(conn))
		{
			dbconn.Open();
			dbcmd = dbconn.CreateCommand();
			sqlQuery = string.Format("delete from Student where id = '" + this.id)+"'";
			dbcmd.CommandText = sqlQuery;
			dbcmd.ExecuteScalar();
			dbconn.Close();
			Debug.Log("Student Deleted  ");

			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.name);

		}

	}

	public void edit_student(){
		//save the selected row's data and go to the scene EditStudent
		//so the EditStudent Scene will show the selected Student's data
		PlayerPrefs.SetInt("studentid", id);
		PlayerPrefs.SetString("studentname", named);
		PlayerPrefs.SetString("studentemail", email);
		PlayerPrefs.SetString("studentphone", phone);
		PlayerPrefs.SetString("studentattended", attended);
		PlayerPrefs.SetString("studentpaid", paid);
		PlayerPrefs.SetInt("studentc_id", c_id);
		SceneManager.LoadScene("EditStudent");
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
