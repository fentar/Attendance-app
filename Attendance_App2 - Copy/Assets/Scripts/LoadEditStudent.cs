using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadEditStudent : MonoBehaviour
{
	public InputField t_name, t_Email, t_Phone;
	public Toggle t_Attended, t_Paid;

	// Start is called before the first frame update
	void Start()
	{
		//Fill the Scene's fields with the selected Student's data
		t_name.text = PlayerPrefs.GetString("studentname"); 
		t_Email.text = PlayerPrefs.GetString("studentemail");
		t_Phone.text = PlayerPrefs.GetString("studentphone");

		//because sqlite doesn't store boolean variables, check with string and show boolean
		if (PlayerPrefs.GetString("studentattended") == "True"){
			t_Attended.isOn = true;}
		else{
			t_Attended.isOn = false;}
		if (PlayerPrefs.GetString("studentpaid") == "True"){
			t_Paid.isOn = true;}
		else{
			t_Paid.isOn = false;}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
