// Globals

bool respawn;
bool triggerwinlose;
bool triggerdangergreywolf;
bool triggerdangercorvus;
bool playerisship;

int greywolf;
int corvus;
int ebolo;
int daring;

int glory;
int nebu;
int corv1;
int corv2;
int corv3;
int corv4;

int greywolfshd1;
int greywolfshd2;
int corvusshd1;
int corvusshd2;

int tiea2;
int tiea3;
int tiea4;
int tied1;
int tied2;

float3 faction_empire_color = { 0, 0.8, 0 };
float3 faction_traitor_color = { 0.4, 0.5, 0.9 };
float3 faction_rebel_color = { 0.8, 0, 0 };


load:

	SetMaxBounds({10000, 1500, 15000});
	SetMinBounds({-10000, -1500, -20000});
	SetMaxAIBounds({10000, 1500, 15000});
	SetMinAIBounds({-10000, -1500, -20000});

	Player.SetLives(5);
	Player.SetScorePerLife(1000000);
	Player.SetScoreForNextLife(1000000);
	Player.ResetScore();

	SetUILine1Color(faction_empire_color);
	SetUILine2Color(faction_traitor_color);
	
  CallScript("engagemusic");	
	CallScript("spawnreset");	
	CallScript("makeimperials");
	CallScript("makeplayer");
	AddEvent(2, "getchildren");
	AddEvent(3, "start");
	AddEvent(5, "spawnenemybombers");
	AddEvent(40, "spawn_traitorZ95");
	AddEvent(50, "spawn_allyGUN");
	AddEvent(80, "spawn_traitorZ95");
	AddEvent(95, "spawn_allyTIEA");
	AddEvent(120, "spawn_traitorYWING");
	AddEvent(135, "spawn_traitorTIEA");
	AddEvent(165, "spawn_traitorYWING");
	AddEvent(170, "spawn_traitorAWING");
	AddEvent(200, "spawn_traitorXWING");
	AddEvent(205, "spawn_traitorTIED");
//	AddEvent(240, "spawnenemybombers2");
//	AddEvent(270, "messagebombercorvus");
//	AddEvent(240, "spawn_traitorXWING");
//	AddEvent(325, "spawn_traitorTIED");
//	AddEvent(360, "messagebombergreywolf");
//	AddEvent(380, "spawnenemybombers3");

	AddEvent(17, "message01");
	AddEvent(22, "message02");
	AddEvent(27, "message03");
	AddEvent(34, "message04");
	AddEvent(38, "message05");
	AddEvent(45, "message06");

	AddEvent(140, "enemybeginspawn");
	AddEvent(50, "corvusbeginspawn");


loadfaction:
	AddFaction("Empire", faction_empire_color);
	AddFaction("Traitors", faction_traitor_color);
	AddFaction("Rebels", faction_rebel_color);
	Faction.MakeAlly("Traitors", "Rebels");
	Faction.SetWingSpawnLimit("Empire", 30);
	Faction.SetWingSpawnLimit("Traitors", 24);

loadscene:
	greywolf = Actor.Spawn("IMPL", "ISD GREY WOLF (Thrawn)", "Empire", "GREY WOLF", 0, { 1000, 400, 12000 }, { 0, -180, 0 }, "CriticalAllies");
	Actor.SetProperty(greywolf, "DamageModifier", 0.8);
	Actor.SetProperty(greywolf, "SetSpawnerEnable", true);
	Actor.QueueLast(greywolf, "move", -1000, 400, -3000, 25);
	Actor.QueueLast(greywolf, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(greywolf, "lock");

	corvus = Actor.Spawn("INTD", "INT CORVUS", "Empire", "CORVUS", 0, { 3500, -500, 500 }, { 0, -130, 0 }, "CriticalAllies");
	Actor.SetProperty(corvus, "DamageModifier", 0.6);
	Actor.QueueLast(corvus, "move", 2400, -500, -1000, 12);
	Actor.QueueLast(corvus, "move", 0, -500, 4000, 20);
	Actor.QueueLast(corvus, "rotate", -2400, -500, 7000, 0);
	Actor.QueueLast(corvus, "lock");

	ebolo = Actor.Spawn("ARQT", "EBOLO", "Empire", "EBOLO", 0, { 3000, -120, 350 }, { 0, -210, 25 });
	Actor.SetProperty(ebolo, "DamageModifier", 0.4);
	Actor.QueueLast(ebolo, "move", -400, 200, -350, 50);
	Actor.QueueLast(ebolo, "move", 1000, 150, -3000, 25);
	Actor.QueueLast(ebolo, "move", -8470, -300, -350, 100);
	Actor.QueueLast(ebolo, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(ebolo, "lock");
	
	daring = Actor.Spawn("ARQT", "DARING", "Empire", "DARING", 0, { 3200, -300, -1450 }, { 0, -150, 25 });
	Actor.SetProperty(daring, "DamageModifier", 0.4);
	Actor.QueueLast(daring, "move", -7780, -450, 450, 15);
	Actor.QueueLast(daring, "move", -8470, -300, -350, 100);
	Actor.QueueLast(daring, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(daring, "lock");

	glory = Actor.Spawn("DEVA", "ISD GLORY", "Traitors", "GLORY", 0, { -6750, 0, -22000 }, { 0, 40, 0 }, "CriticalEnemies");
	Actor.SetProperty(glory, "SetSpawnerEnable", true);
	Actor.SetProperty(glory, "DamageModifier", 0.75);
	Actor.QueueLast(glory, "move", -1000, 100, -6000, 70);
	Actor.QueueLast(glory, "move", 4000, 200, -10000, 10);
	Actor.QueueLast(glory, "rotate", 2000, 210, -20000, 0);
	Actor.QueueLast(glory, "lock");

	corv1 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -7750, 250, -20000 }, { 0, 20, 0 });
	Actor.QueueLast(corv1, "move", 0, 250, -3000, 70);
	Actor.QueueLast(corv1, "move", 3000, 250, -5000, 10);
	Actor.QueueLast(corv1, "rotate", 0, 500, 4000, 0);
	Actor.QueueLast(corv1, "lock");

	nebu = Actor.Spawn("NEBL", "", "Traitors", "", 0, { -3750, -450, -16000 }, { 0, 30, 0 });
	Actor.QueueLast(nebu, "move", 6250, -450, -3200, 100);
	Actor.QueueLast(nebu, "hyperspaceout");
	Actor.QueueLast(nebu, "delete");

	corv2 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -8750, -100, -11000 }, { 0, 75, 0 });
	Actor.QueueLast(corv2, "move", 1550, 200, -2200, 100);
	Actor.QueueLast(corv2, "rotate", 0, 500, 4000, 0);
	Actor.QueueLast(corv2, "lock");

	corv3 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -3750, 300, -12500 }, { 0, 45, 0 });
	Actor.QueueLast(corv3, "move", 2750, -100, -800, 100);
	Actor.QueueLast(corv3, "rotate", 0, 500, 4000, 0);
	Actor.QueueLast(corv3, "lock");

	corv4 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -1750, 50, -14500 }, { 0, 20, 0 });
	Actor.QueueLast(corv4, "move", -750, 100, -1400, 100);
	Actor.QueueLast(corv4, "rotate", 0, 500, 4000, 0);
	Actor.QueueLast(corv4, "lock");

getchildren:
	int[] greywolfc = Actor.GetChildren(greywolf);
	greywolfshd1 = greywolfc[30];
	greywolfshd2 = greywolfc[31];
	int[] corvusc = Actor.GetChildren(corvus);
	corvusshd1 = corvusc[18];
	corvusshd2 = corvusc[19];

engagemusic:
	SetMood(4);
	SetMusicDyn("PANIC-06");

makeplayer:
	Player.DecreaseLives();
	if (respawn) 
		if (!playerisship)
			Player.RequestSpawn(); 
		else
			Player.AssignActor(Actor.Spawn(GetPlayerActorType(), "(Player)", "Empire", "", 0, { 7000, -300, 0 }, { 0, -120, 0 }));
	else 
		CallScript("firstspawn");
	
	AddEvent(5, "setupplayer");

setupplayer:
	Actor.RegisterEvents(Player.GetActor());
	playerisship = Actor.IsLargeShip(Player.GetActor());
	if (respawn) 
		CallScript("respawn");

firstspawn:
	Player.AssignActor(Actor.Spawn(GetPlayerActorType(), "(Player)", "Empire", "", 0, { 500, -300, 12500 }, { 0, -180, 0 }));
	respawn = true;

respawn:
	Actor.AddToSquad(Player.GetActor(), tiea2);
	Actor.AddToSquad(Player.GetActor(), tiea3);
	Actor.AddToSquad(Player.GetActor(), tiea4);

makeimperials:
	tiea2 = Actor.Spawn("TIEA", "Alpha-2", "Empire", "", 0, { 700, -620, 10500 }, { 0, -180, 0 });
	Actor.SetProperty(tiea2, "DamageModifier", 0.25);
	Actor.QueueLast(tiea2, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea2);

	tiea3 = Actor.Spawn("TIEA", "Alpha-3", "Empire", "", 4, { 6000, 300, -500 }, { 0, -90, 0 });
	Actor.SetProperty(tiea3, "DamageModifier", 0.25);
	Actor.QueueLast(tiea3, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea3);

	tiea4 = Actor.Spawn("TIEA", "Alpha-4", "Empire", "", 4, { 6500, 600, -750 }, { 0, -90, 0 });
	Actor.SetProperty(tiea4, "DamageModifier", 0.25);
	Actor.QueueLast(tiea4, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea4);

	tied1 = Actor.Spawn("TIED", "Delta-1", "Empire", "", 4, { 7000, 300, -500 }, { 0, -90, 0 });
	Actor.SetProperty(tied1, "DamageModifier", 0.25);
	Actor.QueueLast(tied1, "wait", 2.5);

	tied2 = Actor.Spawn("TIED", "Delta-2", "Empire", "", 4, { 7500, 600, -750 }, { 0, -90, 0 });
	Actor.SetProperty(tied2, "DamageModifier", 0.25);
	Actor.QueueLast(tied2, "wait", 2.5);
	Actor.AddToSquad(tied1, tied2);

	spawn_hyperspace = false;
	spawn_faction = "Empire";
	damagemod = 0.25;
	spawn_wait = 3.5;
	spawn_type = "TIEI";
	spawn_target = -1;
	spawn_pos = { 200,0,8000 };
	spawn_rot = { 0, -180, 0 };
	CallScript("spawn4");

	spawn_pos = { 1300, 150, 11000 };
	spawn_rot = { 0, -180, 0 };

	CallScript("spawn4");

	spawn_faction = "Traitors";
	damagemod = 0.5;
	spawn_wait = 0;
	spawn_pos = { 400, 0, -2000 };
	spawn_rot = { 0, 0, 0 };
	spawn_type = "TIE";
	CallScript("spawn4");

	spawn_pos = { -400, 0, -2000 };
	CallScript("spawn4");

	spawn_pos = { -2000, 0, -3500 };
	spawn_target = corvus;
	spawn_type = "TIESA";
	CallScript("spawn4");

	spawn_pos = { -4600, 0, -3000 };
	spawn_target = corvus;
	spawn_type = "TIE";
	CallScript("spawn4");

	spawn_pos = { -5400, 0, -3000 };
	spawn_target = -1;
	CallScript("spawn4");


gametick:
	SetUILine1Text("WINGS: " + Faction.GetWingCount("Empire"));
	SetUILine2Text("ENEMY: " + Faction.GetWingCount("Traitors"));
	
	float hp = Actor.GetProperty(greywolf, "Strength");
	if (hp <= 0 && !triggerwinlose) 
		CallScript("losegreywolf");
	
	if (!triggerwinlose && !triggerdangergreywolf && hp < 450) 
		CallScript("dangergreywolf");

	hp = Actor.GetProperty(corvus, "Strength");
	if (hp <= 0 && !triggerwinlose) 
		CallScript("losecorvus");
	
	if (!triggerwinlose && !triggerdangercorvus && hp < 300) 
		CallScript("dangercorvus");

	if (Faction.GetShipCount("Traitors") < 1 && !triggerwinlose) 
		CallScript("win");

win:
	triggerwinlose = true;
	CallScript("messagewin");
	SetGameStateB("GameWon",true);
	AddEvent(3, "fadeout");

fadeout:
	FadeOut();

dangergreywolf:
	triggerdangergreywolf = true;
	for (float time = 0; time < 3; time += 0.4)
	{
		AddEvent(time, "dangergreywolfmsgO");
		AddEvent(time + 0.2, "dangergreywolfmsgY");
	}

losegreywolf:
	triggerwinlose = true;
	CallScript("messagelosegreywolf");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");

dangercorvus:
	triggerdangercorvus = true;
	float time = 0;
	while (time < 3)
	{
		AddEvent(time, "dangercorvusmsgO");
		AddEvent(time + 0.2, "dangercorvusmsgY");
		time += 0.4;
	}

losecorvus:
	triggerwinlose = true;
	CallScript("messagelosecorvus");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");

start:
	Player.SetMovementEnabled(true);

corvusbeginspawn:
	Faction.SetWingSpawnLimit("Empire", 36);
	Actor.SetProperty(corvus, "SetSpawnerEnable", true);

enemybeginspawn:
	Faction.SetWingSpawnLimit("Traitors", 28);
	Actor.SetProperty(nebu, "SetSpawnerEnable", true);

spawnenemybombers:
	spawn_faction = "Traitors";
	damagemod = 1;
	spawn_wait = 0;
	spawn_pos = { -2000, 250, -22500 };
	spawn_rot = { 0, 0 ,0 };
	spawn_type = "TIESA";
	spawn_target = corvusshd1;
	CallScript("spawn1");

	spawn_pos = { -4000, 50,-12000 };
	spawn_target = corvusshd2;
	CallScript("spawn2");

	spawn_pos = { -5000,-150,-24500 };
	spawn_target = corvus;
	CallScript("spawn4");

	spawn_pos = { -25000,300,-23500 };
	spawn_type = "TIEI";
	spawn_target = corvusshd1;
	CallScript("spawn1");

	spawn_pos = { -24000, 200, 24500 };
	spawn_type = "TIE";
	spawn_target = corvus;
	CallScript("spawn2");

spawnenemybombers2:
	damagemod = 1;
	spawn_wait = 0;
	spawn_pos = { -10000, 250, -12500 };
	spawn_rot = { 0, 0 ,0 };
	spawn_type = "TIE";
	spawn_target = corvusshd1;
	CallScript("spawn2");

	spawn_pos = { -11000, 150 , 13500 };
	spawn_target = corvusshd2;
	CallScript("spawn2");

	spawn_pos = { -11000, 50 , 13500 };
	spawn_type = "TIESA";
	spawn_target = corvus;
	CallScript("spawn2");

spawnenemybombers3:
	//spawn_faction = "Traitors";
	damagemod = 1;
	spawn_wait = 0;
	spawn_pos = { -10000,250,-12500 };
	spawn_rot = { 0, 0 ,0 };
	spawn_type = "TIESA";
	spawn_target = greywolfshd1;
	CallScript("spawn4");

	spawn_pos = { -11000, 450,-13500 };
	spawn_target = greywolfshd2;
	CallScript("spawn4");

	spawn_pos = { -11000,650,-13500 };
	spawn_type = "GUN";
	spawn_target = greywolf;
	CallScript("spawn3");

	spawn_pos = { -12000,150,-13500 };
	spawn_type = "TIE";
	CallScript("spawn4");

	spawn_pos = { -10000,150,-13500 };
	CallScript("spawn4");

spawn_allyGUN:
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	damagemod = 1;
	spawn_wait = 0;
	spawn_type = "GUN";
	spawn_target = corv2;
	spawn_pos = { 2400,500,10000 };
	spawn_rot = { 0, 180 ,0 };
	CallScript("spawn4");
	SetMood(-11);
	CallScript("message_allyGUN");


spawn_allyTIEA:
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	damagemod = 1;
	spawn_wait = 0;
	spawn_type = "TIEA";
	spawn_target = corv4;
	spawn_pos = { 2400,500,10000 };
	spawn_rot = { 0, 180 ,0 };
	CallScript("spawn4");
	SetMood(-11);
	CallScript("message_allyTIEA");


spawn_traitorZ95:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	damagemod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "Z95";
	spawn_pos = { 300,0,-15000 };
	spawn_rot = { 0, 0 ,0 };
	CallScript("spawn4");
	SetMood(-21);
	CallScript("message_traitorZ95");


spawn_traitorXWING:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	damagemod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "XWING";
	spawn_pos = { -1000,0,-12500 };
	spawn_rot = { 0, 0 ,0 };
	CallScript("spawn3");
	SetMood(-21);
	CallScript("message_traitorXWING");


	spawn_traitorYWING:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	damagemod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "YWING";
	spawn_pos = { -3000,0,-16000 };
	spawn_rot = { 0, 0 ,0 };
	CallScript("spawn3");

	SetMood(-21);
	CallScript("message_traitorYWING");


spawn_traitorAWING:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	damagemod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "AWING";
	spawn_pos = { -2500,0,-14000 };
	spawn_rot = { 0, 0 ,0 };
	CallScript("spawn3");

	SetMood(-21);
	CallScript("message_traitorAWING");


spawn_traitorTIED:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	damagemod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "TIED";
	spawn_pos = { -1600,0,-17000 };
	spawn_rot = { 0, 0 ,0 };

	CallScript("spawn2");

	SetMood(-21);
	CallScript("message_traitorTIED");


spawn_traitorTIEA:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	damagemod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "TIEA";
	spawn_pos = { -4200,0,-17000 };
	spawn_rot = { 0, 0 ,0 };
	CallScript("spawn2");

	SetMood(-21);
	CallScript("message_traitorTIEA");



stage1:
	SetStageNumber(1);


stage2:
	SetStageNumber(2);
		
	