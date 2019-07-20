load:
	respawn = false;
	triggerwinlose = false;
	triggerdangergreywolf = false;
	triggerdangercorvus = false;

	SetMaxBounds(10000, 1500, 15000);
	SetMinBounds(-10000, -1500, -20000);
	SetMaxAIBounds(10000, 1500, 15000);
	SetMinAIBounds(-10000, -1500, -20000);
	
	SetMusic("battle_4_1");
	SetMusicLoop("battle_4_2");
	
	Player.SetLives(5);
	Player.SetScorePerLife(1000000);
	Player.SetScoreForNextLife(1000000);
	Player.ResetScore();
	
	SetUILine1Color(0, 0.8, 0);
	SetUILine2Color(0.4, 0.5, 0.9);
	
	CallScript("spawnreset");	
	CallScript("makeplayer");

loadfaction:
	AddFaction("You", 0.6, 0.6, 0.6);
	AddFaction("Objects", 0.4, 0.5, 0.9);

loadscene:
	greywolf = Actor.Spawn("Imperial-I Star Destroyer", "ISD GREY WOLF (Thrawn)", "", "GREY WOLF", 0, "Empire", 1000, 400, 12000, 0, -180, 0, "CriticalAllies");
	Actor.SetProperty(greywolf, "DamageModifier", 0.8);
	Actor.SetProperty(greywolf, "SetSpawnerEnable", true);
	Actor.QueueLast(greywolf, "move", -1000, 400, -3000, 25);
	Actor.QueueLast(greywolf, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(greywolf, "lock");

makeplayer:
	Player.DecreaseLives();
	if (respawn) 
		Player.RequestSpawn(); 
	else 
		CallScript("firstspawn");
	
	AddEvent(5, "setupplayer");

setupplayer:
	//Actor.SetProperty(Player.GetActor(), "DamageModifier", 0.25);
	Actor.RegisterEvents(Player.GetActor());

firstspawn:
	Player.AssignActor(Actor.Spawn(GetPlayerActorType(), "(Player)", "", "(Player)", 0, "Empire", 500, -300, 12500, 0, -180, 0));
	respawn = true;

gametick:

win:

start:
	Player.SetMovementEnabled(true);

	