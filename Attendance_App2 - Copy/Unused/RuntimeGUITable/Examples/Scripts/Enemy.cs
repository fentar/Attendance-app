using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Normal, Boss }

[System.Serializable]
public class Enemy
{
	public string name;

	public Sprite icon;

	public int hp;

	public float power;

	public bool canSwim;

	public EnemyType type;

	public void Method()
	{
		Debug.Log("Method called on " + name);
	}

}

