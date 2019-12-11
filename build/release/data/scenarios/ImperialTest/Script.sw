// Globals

bool respawn;
bool triggerwinlose;
bool triggerdangergreywolf;
bool triggerdangercorvus;
bool triggerescapecorv;
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
int gloryhangar;

int tiea2;
int tiea3;
int tiea4;
int tiea5;
int tiea6;

float3 faction_empire_color = { 0, 0.8, 0 };
float3 faction_traitor_color = { 0.4, 0.5, 0.9 };
float3 faction_rebel_color = { 0.8, 0, 0 };

float3 _d = { 0, 0, 0 };
int _a = -1;


load:
	Scene.SetMaxBounds({10000, 2500, 15000});
	Scene.SetMinBounds({-10000, -2500, -20000});
	Scene.SetMaxAIBounds({10000, 2500, 15000});
	Scene.SetMinAIBounds({-10000, -2500, -20000});

	Player.SetLives(5);
	Score.SetScorePerLife(1000000);
	Score.SetScoreForNextLife(1000000);
	Score.Reset();

	UI.SetLine1Color(faction_empire_color);
	UI.SetLine2Color(faction_traitor_color);
	
	Script.Call("engagemusic");	
	Script.Call("spawnreset");	
	Script.Call("make_ships");
	Script.Call("make_fighters");
	Script.Call("makeplayer");
	AddEvent(2, "getchildren");
	AddEvent(3, "start");
	AddEvent(5, "spawnenemybombers");
	AddEvent(40, "spawn_traitorZ95");
	AddEvent(50, "spawn_allyGUN");
	AddEvent(65, "spawn_traitorDelta");
	AddEvent(80, "spawn_traitorZ95");
	AddEvent(95, "spawn_allyTIEA");
	AddEvent(120, "spawn_traitorYWING");
	AddEvent(135, "spawn_traitorTIEA");
	AddEvent(165, "spawn_traitorYWING");
	AddEvent(169, "spawn_traitorAWING");
	AddEvent(200, "spawn_traitorXWING");
	AddEvent(204, "spawn_traitorTIED");

	AddEvent(17, "message01");
	AddEvent(22, "message02");
	AddEvent(27, "message03");
	AddEvent(34, "message04");
	AddEvent(38, "message05");
	AddEvent(55, "message06");

	AddEvent(140, "enemybeginspawn");
	AddEvent(50, "corvusbeginspawn");


loadfaction:
	Faction.Add("Empire", faction_empire_color);
	Faction.Add("Traitors", faction_traitor_color);
	Faction.Add("Rebels", faction_rebel_color);
	Faction.MakeAlly("Traitors", "Rebels");
	Faction.SetWingSpawnLimit("Empire", 30);
	Faction.SetWingSpawnLimit("Traitors", 26);

	
loadscene:

	
getchildren:
	int[] greywolfc = Actor.GetChildren(greywolf);
	greywolfshd1 = greywolfc[30];
	greywolfshd2 = greywolfc[31];
	int[] corvusc = Actor.GetChildren(corvus);
	corvusshd1 = corvusc[18];
	corvusshd2 = corvusc[19];
	int[] gloryc = Actor.GetChildren(glory);	
	gloryhangar = gloryc[29];

	
engagemusic:
	Audio.SetMood(4);
	Audio.SetMusicDyn("PANIC-06");

	
makeplayer:
	Player.DecreaseLives();
	if (respawn) 
		if (!playerisship)
			Player.RequestSpawn(); 
		else
			Player.AssignActor(Actor.Spawn(GetPlayerActorType(), GetPlayerName(), "Empire", "", 0, { 7000, -300, 0 }, { 0, -120, 0 }));
	else 
		Script.Call("firstspawn");
	
	AddEvent(1, "setupplayer");

	
setupplayer:
	playerisship = Actor.IsLargeShip(Player.GetActor());
	if (respawn) 
		Script.Call("respawn");

	
firstspawn:
	Player.AssignActor(Actor.Spawn(GetPlayerActorType(), GetPlayerName(), "Empire", "", 0, { 4200, -950, 450 }, { 0, -130, 0 })); //{ 500, -300, 12500 }, { 0, -180, 0 }));
	if (Actor.IsLargeShip(Player.GetActor()))
		Actor.SetLocalPosition(Player.GetActor(), { 6000, -300, 0 });
	respawn = true;

	
respawn:
	Squad.AddToSquad(Player.GetActor(), tiea2);
	Squad.AddToSquad(Player.GetActor(), tiea3);
	Squad.AddToSquad(Player.GetActor(), tiea4);
	Squad.AddToSquad(Player.GetActor(), tiea5);
	Squad.AddToSquad(Player.GetActor(), tiea6);

	
make_ships:

	// Empire
	
	greywolf = Actor.Spawn("IMPL", "ISD GREY WOLF (Thrawn)", "Empire", "GREY WOLF", 0, { 1000, 400, 12000 }, { 0, -180, 0 }, { "CriticalAllies" });
	Actor.SetArmorAll(greywolf, 0.8);
	Actor.SetProperty(greywolf, "Spawner.Enabled", true);
	Actor.SetProperty(greywolf, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIEI","TIESA"});
	AI.QueueLast(greywolf, "move", {-1000, 400, -3000}, 25);
	AI.QueueLast(greywolf, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(greywolf, "lock");

	corvus = Actor.Spawn("INTD", "INT CORVUS", "Empire", "CORVUS", 0, { 3500, -500, 500 }, { 0, -130, 0 }, { "CriticalAllies" });
	Actor.SetArmorAll(corvus, 0.5);
	AI.QueueLast(corvus, "move", {2400, -500, -1000}, 12);
	AI.QueueLast(corvus, "move", {0, -600, 4000}, 20);
	AI.QueueLast(corvus, "rotate", {-2400, -600, 7000}, 0);
	AI.QueueLast(corvus, "lock");

	ebolo = Actor.Spawn("STRK", "EBOLO", "Empire", "EBOLO", 0, { 3000, -120, 350 }, { 0, -210, 25 });
	Actor.SetArmorAll(ebolo, 0.8);
	AI.QueueLast(ebolo, "move", {-400, 200, -350}, 50);
	AI.QueueLast(ebolo, "move", {1000, 150, -3000}, 25);
	AI.QueueLast(ebolo, "move", {-8470, -300, -10350}, 100);
	AI.QueueLast(ebolo, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(ebolo, "lock");
	
	daring = Actor.Spawn("STRK", "DARING", "Empire", "DARING", 0, { 3200, -300, -1450 }, { 0, -150, 25 });
	Actor.SetArmorAll(daring, 0.8);
	AI.QueueLast(daring, "move", {-7780, -450, 450}, 15);
	AI.QueueLast(daring, "move", {-8470, -300, -7350}, 100);
	AI.QueueLast(daring, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(daring, "lock");
	
	// Traitors
	
	glory = Actor.Spawn("IMPL", "ISD GLORY", "Traitors", "GLORY", 0, { -6750, 0, -22000 }, { 0, 40, 0 }, { "CriticalEnemies" });
	Actor.SetProperty(glory, "Spawner.Enabled", true);
	Actor.SetProperty(glory, "Spawner.SpawnTypes", {"TIE","TIEI","TIEI","TIEI"});
	Actor.SetArmorAll(glory, 0.6);
	AI.QueueLast(glory, "move", {-1000, 100, -6000}, 70);
	AI.QueueLast(glory, "move", {4000, 200, -10000}, 10);
	AI.QueueLast(glory, "rotate", {2000, 210, -20000}, 0);
	AI.QueueLast(glory, "lock");

	corv1 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -7750, 250, -20000 }, { 0, 20, 0 });
	Actor.SetArmorAll(corv1, 0.8);
	AI.QueueLast(corv1, "move", {0, 250, -3000}, 70);
	AI.QueueLast(corv1, "move", {3000, 250, -5000}, 10);
	AI.QueueLast(corv1, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv1, "lock");

	corv2 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -8750, -100, -11000 }, { 0, 75, 0 });
	AI.QueueLast(corv2, "move", {1550, 200, -2200}, 100);
	AI.QueueLast(corv2, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv2, "lock");

	corv3 = Actor.Spawn("CORV", "CRV DIVINE WIND", "Traitors", "", 0, { -3750, 300, -12500 }, { 0, 45, 0 });
	Actor.SetArmorAll(corv3, 0.5);
	AI.QueueLast(corv3, "move", {2750, -100, -800}, 100);
	AI.QueueLast(corv3, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv3, "lock");

	corv4 = Actor.Spawn("STRK", "", "Traitors", "", 0, { -1750, 50, -14500 }, { 0, 20, 0 });
	AI.QueueLast(corv4, "move", {-750, 100, -1400}, 100);
	AI.QueueLast(corv4, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv4, "lock");
	
	nebu = Actor.Spawn("NEB2", "", "Traitors", "", 0, { -3750, -450, -16000 }, { 0, 30, 0 });
	Actor.SetArmorAll(nebu, 0.5);
	AI.QueueLast(nebu, "move", {6250, -450, -3200}, 100);
	AI.QueueLast(nebu, "hyperspaceout");
	AI.QueueLast(nebu, "delete");
	
	
make_fighters:

	// Empire initial spawns
	
	tiea2 = Actor.Spawn("TIEA", "Alpha-2", "Empire", "", 0, { 700, -620, 10500 }, { 0, -180, 0 });
	Actor.SetArmorAll(tiea2, 0.25);
	AI.QueueLast(tiea2, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea2);

	tiea3 = Actor.Spawn("TIEA", "Alpha-3", "Empire", "", 0, { 6000, 300, -500 }, { 0, -90, 0 });
	Actor.SetArmorAll(tiea3, 0.25);
	AI.QueueLast(tiea3, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea3);

	tiea4 = Actor.Spawn("TIEA", "Alpha-4", "Empire", "", 0, { 6500, 600, -750 }, { 0, -90, 0 });
	Actor.SetArmorAll(tiea4, 0.25);
	AI.QueueLast(tiea4, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea4);

	tiea5 = Actor.Spawn("TIEA", "Alpha-5", "Empire", "", 0, { 7000, 300, -500 }, { 0, -90, 0 });
	Actor.SetArmorAll(tiea5, 0.25);
	AI.QueueLast(tiea5, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea5);

	tiea6 = Actor.Spawn("TIEA", "Alpha-6", "Empire", "", 0, { 7500, 600, -750 }, { 0, -90, 0 });
	Actor.SetArmorAll(tiea5, 0.25);
	AI.QueueLast(tiea5, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea6);
	
	spawn_hyperspace = false;
	spawn_faction = "Empire";
	spawn_dmgmod = 0.25;
	spawn_wait = 3.5;
	spawn_type = "TIEI";
	spawn_target = -1;
	spawn_pos = { 200,0,8000 };
	spawn_rot = { 0, -180, 0 };
	Script.Call("spawn4");

	spawn_pos = { 1300, 150, 11000 };
	spawn_rot = { 0, -180, 0 };

	Script.Call("spawn4");

	// Traitor initial spawns
	
	spawn_faction = "Traitors";
	spawn_dmgmod = 0.5;
	spawn_wait = 0;
	spawn_pos = { 400, 0, -4000 };
	spawn_rot = { 0, 0, 0 };
	spawn_type = "TIE";
	Script.Call("spawn4");

	spawn_pos = { -400, 0, -4200 };
	Script.Call("spawn4");

	spawn_pos = { -2000, 0, -7500 };
	spawn_target = corvus;
	spawn_type = "TIESA";
	Script.Call("spawn4");

	spawn_pos = { -4600, 0, -5200 };
	spawn_target = corvus;
	spawn_type = "TIE";
	Script.Call("spawn4");

	spawn_pos = { -5400, 0, -5200 };
	spawn_target = -1;
	Script.Call("spawn4");

	// Empire spawn reserves
	
	for (int i = 1; i <= 8; i += 1)
		Actor.QueueAtSpawner(Actor.Spawn("TIE", "", "Empire", "", 0, _d, _d), greywolf);
	
	for (int i = 1; i <= 4; i += 1)
	{
		_a = Actor.Spawn("TIEI", "Eta-" + i, "Empire", "", 0, _d, _d);
		Actor.SetArmorAll(_a, 0.2);
		Actor.QueueAtSpawner(_a, greywolf);
	}

	for (int i = 1; i <= 4; i += 1)
		Actor.QueueAtSpawner(Actor.Spawn("TIE", "", "Empire", "", 0, _d, _d), greywolf);
	
	_a = Actor.Spawn("TIESA", "Beta-1", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", glory, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);

	_a = Actor.Spawn("TIESA", "Beta-2", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", glory, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	_a = Actor.Spawn("TIESA", "Beta-3", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", glory, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	Actor.QueueAtSpawner(Actor.Spawn("TIE", "", "Empire", "", 0, _d, _d), greywolf);

	_a = Actor.Spawn("TIESA", "Beta-4", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", nebu, -1, -1, false, 45);
	Actor.QueueAtSpawner(_a, greywolf);

	_a = Actor.Spawn("TIESA", "Beta-5", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", nebu, -1, -1, false, 45);
	Actor.QueueAtSpawner(_a, greywolf);
	
	_a = Actor.Spawn("TIESA", "Beta-6", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", nebu, -1, -1, false, 45);
	Actor.QueueAtSpawner(_a, greywolf);
	
	Actor.QueueAtSpawner(Actor.Spawn("TIE", "", "Empire", "", 0, _d, _d), greywolf);

	// Traitor spawn reserves
	
	for (int i = 1; i <= 4; i += 1)
		Actor.QueueAtSpawner(Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d), glory);
	

gametick:
	UI.SetLine1Text("WINGS: " + Faction.GetWingCount("Empire"));
	UI.SetLine2Text("ENEMY: " + Faction.GetWingCount("Traitors"));
	
	if (!triggerwinlose)
	{
		float hp = Actor.GetProperty(greywolf, "Strength");
		if (hp <= 0) 
			Script.Call("losegreywolf");
		else if (!triggerdangergreywolf && hp < 450) 
			Script.Call("dangergreywolf");

		hp = Actor.GetProperty(corvus, "Strength");
		if (hp <= 0) 
			Script.Call("losecorvus");
		else if (!triggerdangercorvus && hp < 300) 
			Script.Call("dangercorvus");
		
		if (!triggerescapecorv)
		{
			hp = Actor.GetProperty(glory, "Strength");
			if (hp < 100) 
				Script.Call("spawn_escapeCORV");
		}	
		else
		{
			if (GetGameStateB("Escaped"))
				Script.Call("loseescaped");
		}
	
		if (Faction.GetShipCount("Traitors") < 1 && !triggerwinlose) 
			Script.Call("win");
	}
	
win:
	triggerwinlose = true;
	Script.Call("messagewin");
	SetGameStateB("GameWon",true);
	AddEvent(3, "fadeout");

	
fadeout:
	Scene.FadeOut();

	
dangergreywolf:
	triggerdangergreywolf = true;
	for (float time = 0; time < 3; time += 0.4)
	{
		AddEvent(time, "dangergreywolfmsgO");
		AddEvent(time + 0.2, "dangergreywolfmsgY");
	}

	
losegreywolf:
	triggerwinlose = true;
	Script.Call("messagelosegreywolf");
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
	Script.Call("messagelosecorvus");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");

	
loseescaped:
	triggerwinlose = true;
	Script.Call("messageloseescaped");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");
	
start:
	Player.SetMovementEnabled(true);

	
corvusbeginspawn:
	Faction.SetWingSpawnLimit("Empire", 40);
	Actor.SetProperty(corvus, "Spawner.Enabled", true);
	Actor.SetProperty(corvus, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIEI"});
	Actor.SetProperty(greywolf, "Spawner.SpawnTypes", true);
	Actor.SetArmorAll(corvus, 0.6);


enemybeginspawn:
	Faction.SetWingSpawnLimit("Traitors", 36); //28
	Actor.SetProperty(nebu, "Spawner.Enabled", true);
	Actor.SetProperty(nebu, "Spawner.SpawnTypes", {"Z95","YWING"});
	Actor.SetProperty(glory, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIE","TIEI","TIEI","TIEI","TIEI","JV7"});
	
	Actor.QueueAtSpawner(Actor.Spawn("TIED", "Iota-1", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIED", "Iota-2", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIESA", "", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIESA", "", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIED", "Iota-3", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIED", "Iota-4", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIESA", "", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIESA", "", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIED", "Iota-5", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIED", "Iota-6", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d), glory);
	Actor.QueueAtSpawner(Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d), glory);

	Audio.SetMood(-21);
	Script.Call("message_traitorIota");

	
spawnenemybombers:
	spawn_faction = "Traitors";
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_pos = { -2000, 250, -22500 };
	spawn_rot = { 0, 0 ,0 };
	spawn_type = "TIESA";
	spawn_target = corvusshd1;
	Script.Call("spawn1");

	spawn_pos = { -4000, 50,-14000 };
	spawn_target = corvusshd2;
	Script.Call("spawn2");

	spawn_pos = { -5000,-150,-24500 };
	spawn_target = corvus;
	Script.Call("spawn4");

	spawn_pos = { -25000,300,-23500 };
	spawn_type = "TIEI";
	spawn_target = corvusshd1;
	Script.Call("spawn1");

	spawn_pos = { -24000, 200, 24500 };
	spawn_type = "TIE";
	spawn_target = corvus;
	Script.Call("spawn2");

	
spawn_allyGUN:
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_type = "GUN";
	spawn_target = corv2;
	spawn_pos = { 2400,500,10000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn4");
	Audio.SetMood(-11);
	Script.Call("message_allyGUN");


spawn_allyTIEA:
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_type = "TIEA";
	spawn_target = corv4;
	spawn_pos = { 2400,500,10000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn4");
	Audio.SetMood(-11);
	Script.Call("message_allyTIEA");


spawn_traitorZ95:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "Z95";
	spawn_pos = { 300,0,-15000 };
	spawn_rot = { 0, 0 ,0 };
	Script.Call("spawn4");
	Audio.SetMood(-21);
	Script.Call("message_traitorZ95");


spawn_traitorXWING:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "XWING";
	spawn_pos = { -1000,0,-12500 };
	spawn_rot = { 0, 0 ,0 };
	Script.Call("spawn3");
	Audio.SetMood(-21);
	Script.Call("message_traitorXWING");


	spawn_traitorYWING:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "YWING";
	spawn_pos = { -3000,0,-16000 };
	spawn_rot = { 0, 0 ,0 };
	Script.Call("spawn3");

	Audio.SetMood(-21);
	Script.Call("message_traitorYWING");


spawn_traitorDelta:
	for (int i = 1; i <= 4; i += 1)
	{
		_a = Actor.Spawn("TIED", "Delta-" + i, "Traitors", "", 0, _d, _d);
		AI.QueueLast(_a, "attackactor", daring, -1, -1, false);
		Actor.QueueAtSpawner(_a, glory);
	}
	
	for (int i = 1; i <= 4; i += 1)
	{
		_a = Actor.Spawn("TIESA", "", "Traitors", "", 0, _d, _d);
		AI.QueueLast(_a, "attackactor", corvus, -1, -1, false, 30);
		Actor.QueueAtSpawner(_a, glory);
	}
	
	Audio.SetMood(-21);
	Script.Call("message_traitorDelta");


spawn_traitorAWING:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "AWING";
	spawn_pos = { -2500,0,-14000 };
	spawn_rot = { 0, 0 ,0 };
	Script.Call("spawn3");

	Audio.SetMood(-21);
	Script.Call("message_traitorAWING");


spawn_traitorTIED:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "TIED";
	spawn_pos = { -1600,0,-17000 };
	spawn_rot = { 0, 0 ,0 };

	Script.Call("spawn2");

	Audio.SetMood(-21);
	Script.Call("message_traitorTIED");


spawn_traitorTIEA:
	spawn_faction = "Traitors";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_target = -1;

	spawn_type = "TIEA";
	spawn_pos = { -4200,0,-17000 };
	spawn_rot = { 0, 0 ,0 };
	Script.Call("spawn2");

	Audio.SetMood(-21);
	Script.Call("message_traitorTIEA");
	
	
spawn_escapeCORV:
	triggerescapecorv = true;
	float3 pos = Actor.GetGlobalPosition(gloryhangar);
	float3 rot = Actor.GetGlobalRotation(glory);
	rot = {rot[0] + 45, rot[1], rot[2]};
	int esccorv = Actor.Spawn("CORV", "CRV VORKNKX", "Traitors", "VORKNKX", 0, pos, rot, { "CriticalEnemies" });
	Actor.SetArmorAll(esccorv, 0.75);
	Actor.SetProperty(greywolf, "Spawner.SpawnTypes", {"TIE","TIE","TIEI","TIESA","TIESA"});
	AI.QueueLast(esccorv, "wait", 2.5);
	AI.QueueLast(esccorv, "move", {-6750, 0, -12000}, 100);
	AI.QueueLast(esccorv, "hyperspaceout");
	AI.QueueLast(esccorv, "setgamestateb","Escaped",true);
	AI.QueueLast(esccorv, "delete");
	Script.Call("messageescape");

	_a = Actor.Spawn("TIESA", "Epsilon-1", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", esccorv, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);

	_a = Actor.Spawn("TIESA", "Epsilon-2", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", esccorv, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	_a = Actor.Spawn("TIEI", "Gamma-1", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", esccorv, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	_a = Actor.Spawn("TIEI", "Gamma-2", "Empire", "", 0, _d, _d);
	Actor.SetArmorAll(_a, 0.25);
	AI.QueueLast(_a, "attackactor", esccorv, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	

