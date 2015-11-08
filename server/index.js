"use strict";

// EXPRESS INIT / HELLO WORLD
var express = require( 'express');
var app = express();

app.get( '/hello', function( req,res ) 
{
	console.log("Hello World!");
	var obj = { Hello: "World "};

	res.send( obj );
});

//app.listen( process.env.PORT || 3000 );	

/// MONGO INIT
var mongodb = require( 'mongodb');
var MongoClient = mongodb.MongoClient;
var ObjectID = mongodb.ObjectID;

var db;
var dbURL = process.env.DBURL || 'mongodb://localhost:27017/test';
MongoClient.connect( dbURL, ( err, inDB ) =>
{
	if ( err )
	{
		console.log( "err: " + err );
		return;
	}

	db = inDB;
	app.listen( process.env.PORT || 3000 );	
} );

// NEW PLAYER
app.post( '/players', ( req, res ) =>
{
	var players = db.collection( 'players');

	var newPlayer = 
	{
		Name : "player" + Math.ceil( Math.random() * 10000 ),
		Health: 100,
		Coins: 80,
		Kills: 0
	};

	players.insertOne( newPlayer, ( err, result ) =>
	{
		if( err )
		{
			console.error( err );
			return res.status( 500 ).send( err );
		}
		else
		{
			//console.log( JSON.stringify( result.ops, null, '  ' ) );
			console.log( "new player with id: " +  result.insertedId );
			var playerWithId = result.ops[0];
			return res.send( playerWithId );
		}
	} );
});

function dbGetPlayerData( playerId, callback )
{
	var players = db.collection( 'players');
	console.log( playerId );
	players.findOne( { _id: ObjectID.createFromHexString( playerId ) }, callback );
}

function dbSetPlayerData( playerId, playerData, callback )
{
	var players = db.collection( 'players');
	players.updateOne( { _id: ObjectID.createFromHexString( playerId ) }, playerData, callback );
}

// LOAD PLAYER DATA
app.get( '/players/:playerId', ( req, res ) => 
{
	dbGetPlayerData( req.params.playerId, ( err, playerData ) =>
	{
		if ( playerData )
		{
			console.log( "found player with id: " + req.playerId );
			return res.send( playerData );
		}
	} );
});

// BUY POTION
app.post( '/players/:playerId/buyPotion', ( req, res ) => 
{
	dbGetPlayerData( req.params.playerId, ( err, playerData ) =>
	{
		if ( playerData )
		{
			if ( playerData.Coins >= 50 && playerData.Health > 0 )
			{
				playerData.Coins -= 50;
				playerData.Health = 100;
			}

			dbSetPlayerData( req.params.playerId, playerData, ( err  ) =>
			{
				return res.send( playerData );
			} );
		}
	} );
});

// FIGHT
app.post( '/players/:playerId/fight', ( req, res ) => 
{
	dbGetPlayerData( req.params.playerId, ( err, playerData ) =>
	{
		if ( playerData )
		{
			if ( playerData.Health > 0 )
			{
				playerData.Kills++;
				playerData.Coins += Math.ceil( Math.random() * 5 );
				playerData.Health -= Math.ceil( Math.random() * 11 );
			}

			dbSetPlayerData( req.params.playerId, playerData, ( err  ) =>
			{
				return res.send( playerData );
			} );
		}
	} );
});
