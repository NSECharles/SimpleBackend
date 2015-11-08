using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(20,20,100,50),"SaveStuff"))
		{
			Debug.Log("Clicked the button with text");
		}			
	}
}
