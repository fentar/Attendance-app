using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(InputField))]
public class ExpandableInputField : MonoBehaviour 
{

	public Text containerText;

	void Awake()
	{
		if (!Application.isPlaying)
			containerText.text = GetComponent<InputField>().text;
	}

	void Update()
	{
		if (!Application.isPlaying)
			containerText.text = GetComponent<InputField>().text;
	}

	public void UpdateContainer(string text)
	{
		containerText.text = text;
	}

}
