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
		
		if ( !string.IsNullOrEmpty( _PlayerId ) )
		{
			LoadPlayerData();			
		}
	}
	
	void OnGUI()
	{
		GUI.Label( new Rect( 20, 20, 400, 50), "PlayerId: " + _PlayerId );
		HelloButtonGUI();		
		
		NewPlayerButtonGUI();
		
		PlayerDataGUI();
	}
	
	void PlayerDataGUI()
	{
		if ( _PlayerData != null )
		{
			GUI.Label( new Rect( 20, 120, 400, 50), "Name: " + _PlayerData.Name );
			GUI.Label( new Rect( 20, 150, 400, 50), "Health: " +  _PlayerData.Health );
			GUI.Label( new Rect( 20, 180, 400, 50), "Coins: " +  _PlayerData.Coins );
		}
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
					PlayerPrefs.SetString( "PlayerId", _PlayerId );
					Debug.Log("New player: " + _PlayerId );
					
					_PlayerData = new PlayerData( result );
				}
			} );
		}		
	}
	
	void LoadPlayerData( )
	{
		_Rester.GetJSON( ServerURL + "/players/" + _PlayerId, ( err, result ) =>
		{
			if ( !string.IsNullOrEmpty( err ) )
			{
				Debug.Log( "Error: " +  err );
			}
			else
			{				
				_PlayerData = new PlayerData( result );
			}
		} );
	}
	
	public string ServerURL;
	
	private string _PlayerId = "";
	private Rester _Rester;
	private PlayerData _PlayerData;
	
	private class PlayerData
	{
		public PlayerData( JsonObject inJSON )
		{
			Name = (string) inJSON[ "Name"];
			Health = System.Convert.ToInt32( inJSON[ "Health"] );
			Coins = System.Convert.ToInt32( inJSON[ "Coins"] );
		}
		public string Name;
		public int Health;
		public int Coins;
	}
		
}
