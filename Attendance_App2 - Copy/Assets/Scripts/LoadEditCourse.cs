using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadEditCourse : MonoBehaviour
{
	public InputField t_name, t_Description, t_Time;
    // Start is called before the first frame update
    void Start()
    {
		//Fill the Scene's fields with the selected Course's data
		t_name.text = PlayerPrefs.GetString("coursename"); 
		t_Description.text = PlayerPrefs.GetString("coursedesc");
		t_Time.text = PlayerPrefs.GetString("coursetime");
    }

    // Update is called once per frame
    void Update()
    {
		
    }
}
