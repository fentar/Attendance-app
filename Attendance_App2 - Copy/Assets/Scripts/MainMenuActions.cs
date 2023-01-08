using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuActions : MonoBehaviour
{
	// Update is called once per frame
	void Update()
	{
		//Pressing the Escape button (or the Back button on Android), will load the appropriate scene
		if (Input.GetKeyDown(KeyCode.Escape)){
			if (SceneManager.GetActiveScene().name == "Home"){
				Application.Quit();
			}
			if (SceneManager.GetActiveScene().name == "EditStudent"){
				SceneManager.LoadScene("ShowStudents");
			}
			if (SceneManager.GetActiveScene().name == "EditCourse"){
				SceneManager.LoadScene("Courses");
			}
			if (SceneManager.GetActiveScene().name == "Student"){
				SceneManager.LoadScene("ShowStudents");
			}
			if (SceneManager.GetActiveScene().name == "New Course"){
				SceneManager.LoadScene("Home");
			}
			if (SceneManager.GetActiveScene().name == "Courses"){
				SceneManager.LoadScene("Home");
			}
			if (SceneManager.GetActiveScene().name == "ShowStudents"){
				SceneManager.LoadScene("Courses");
			}
		}

	}
    
	//The methods to Load every Scene (the Edit scenes are loaded through other scripts)
    public void LoadHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void LoadCourses()
    {
		SceneManager.LoadScene("Courses");
    }

	public void LoadNewCourse()
	{
		SceneManager.LoadScene("New Course");
	}

	public void LoadStudents()
	{
		SceneManager.LoadScene("ShowStudents");
	}

	public void LoadNewStudent()
	{
		SceneManager.LoadScene("Student");
	}
}
