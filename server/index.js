"use strict";

// EXPRESS INIT / HELLO WORLD
var express = require( 'express');
var bodyParser = require( 'body-parser');

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
var mongo = mongodb.MongoClient;
var db;
mongo.connect( 'mongodb://localhost:27017/test', ( err, inDB ) =>
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
		Coins: 200
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

app.get( '/players/:playerId', ( req, res ) => 
{
	var players = db.collection( 'players');
	players.findOne( req.playerId, ( err, result ) =>
	{
		if( err )
		{
			console.error( err );
			return res.status( 500 ).send( err );
		}
		else
		{
			console.log( "found player with id: " + req.playerId );
			return res.send( result );
		}
	} );
});


// parse application/json
app.use(bodyParser.json());
