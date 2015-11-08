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
		GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 28;
		
		GUI.Label( new Rect( 20, 20, 640, 50), "_id: " + _PlayerId );
		
		HelloButtonGUI();	
		NewPlayerButtonGUI();
		FightButtonGUI();
		BuyPotionButtonGUI();

		PlayerDataGUI();
	}
	
	void PlayerDataGUI()
	{
		if ( _PlayerData != null )
		{
			GUI.Label( new Rect( 20, 120, 400, 50), "Name: " + _PlayerData.Name );
			GUI.Label( new Rect( 20, 150, 400, 50), "Health: " +  _PlayerData.Health );
			GUI.Label( new Rect( 20, 180, 400, 50), "Coins: " +  _PlayerData.Coins );
			GUI.Label( new Rect( 20, 210, 400, 50), "Kills: " +  _PlayerData.Kills );
		}
	}
	
	void HelloButtonGUI()
	{
		if (GUI.Button(new Rect(20,60,100,50),"hello"))
		{
			_Rester.GetJSON( ServerURL + "/hello", ( err, result ) =>
			{
				Debug.Log( (string)result["Hello"] );
			});
		}			
	}
	
	void NewPlayerButtonGUI()
	{
		if (GUI.Button(new Rect(150,60,100,50),"new"))
		{
			_Rester.PostJSON( ServerURL + "/players", new JsonObject(), ( err, result ) =>
			{
				_PlayerId =  (string)result[ "_id"];
				PlayerPrefs.SetString( "PlayerId", _PlayerId );
				Debug.Log("New player: " + _PlayerId );
				
				_PlayerData = new PlayerData( result );
			} );
		}		
	}
	
	void FightButtonGUI()
	{
		if (GUI.Button(new Rect(280,60,100,50),"fight"))
		{
			_Rester.PostJSON( ServerURL + "/players/" + _PlayerId + "/fight", new JsonObject(), ( err, result ) =>
			{	
				_PlayerData = new PlayerData( result );
			} );
		}		
	}	
		
	void BuyPotionButtonGUI()
	{
		if (GUI.Button(new Rect(410,60,100,50),"potion"))
		{
			_Rester.PostJSON( ServerURL + "/players/" + _PlayerId + "/buyPotion", new JsonObject(), ( err, result ) =>
			{
				_PlayerData = new PlayerData( result );
			} );
		}		
	}	
	
	
	
	void LoadPlayerData( )
	{
		_Rester.GetJSON( ServerURL + "/players/" + _PlayerId, ( err, result ) =>
		{	
			_PlayerData = new PlayerData( result );
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
			Kills = System.Convert.ToInt32( inJSON[ "Kills"] );
		}
		public string Name;
		public int Health;
		public int Coins;
		public int Kills;
	}
		
}
