using UnityEngine;
using System.Collections.Generic;
using SimpleJson;

public class Example : MonoBehaviour
{
	// Use this for initialization
	void Awake()
	{
		_Rester = gameObject.AddComponent< Rester >();
		_PlayerId = PlayerPrefs.GetString("PlayerId", string.Empty);
	}
	
	void OnGUI()
	{
		GUI.Label( new Rect( 20, 20, 400, 50), "PlayerId: " + _PlayerId );
		HelloButtonGUI();		
		
		NewPlayerButtonGUI();
	}
	
	void HelloButtonGUI()
	{
		if (GUI.Button(new Rect(20,60,100,50),"hello"))
		{
			_Rester.GetJSON( ServerURL + "/hello", ( err, result ) =>
			{
				if ( !string.IsNullOrEmpty( err ) )
				{
					Debug.Log( "Error: " +  err );
				}
				else if ( result != null )
				{
					Debug.Log( (string)result["Hello"] );
				}
			});
		}			
	}
	
	void NewPlayerButtonGUI()
	{
		if (GUI.Button(new Rect(150,60,100,50),"new"))
		{
			_Rester.PostJSON( ServerURL + "/players", new JsonObject(), ( err, result ) =>
			{
				if ( !string.IsNullOrEmpty( err ) )
				{
					Debug.Log( "Error: " +  err );
				}
				else
				{
					_PlayerId =  (string)result[ "_id"];
					PlayerPrefs.SetString( "PlayertId", _PlayerId );
					Debug.Log("New player: " + _PlayerId );
				}
			} );
		}		
	}
	
	
	public string ServerURL;
	
	private string _PlayerId = "";
	private Rester _Rester;
}
