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
int vorknkx;

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

float z_tie_shd = 8;
float z_tiei_shd = 12;
float z_tiesa_shd = 15;
float z_tie_shdregen = 0.2;


load:
	Scene.SetMaxBounds({10000, 2500, 15000});
	Scene.SetMinBounds({-15000, -2500, -30000});
	Scene.SetMaxAIBounds({10000, 2500, 15000});
	Scene.SetMinAIBounds({-15000, -2500, -30000});

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
	AddEvent(1, "getchildren");
	AddEvent(1.5, "start");
	AddEvent(5, "spawnenemybombers");
	AddEvent(42, "spawn_traitorZDelta");
	AddEvent(62, "spawn_allyGUN");
	AddEvent(80, "spawn_allyEta");
	AddEvent(86, "spawn_allyBeta");
	AddEvent(90, "corvusbeginspawn");
	AddEvent(120, "spawn_traitorZIota");
	AddEvent(145, "spawn_allyTIEA");
	
	AddEvent(200, "spawn_traitorZTheta");
	AddEvent(210, "enemybeginspawn");
	
	
	AddEvent(7, "message01");
	AddEvent(12, "message02");
	AddEvent(17, "message03");
	AddEvent(24, "message04");
	AddEvent(28, "message05");
	AddEvent(35, "message06");



loadfaction:
	Faction.Add("Empire", faction_empire_color);
	Faction.Add("Traitors", faction_traitor_color);
	Faction.Add("Rebels", faction_rebel_color);
	Faction.MakeAlly("Traitors", "Rebels");
	Faction.SetWingSpawnLimit("Empire", 24);
	Faction.SetWingSpawnLimit("Traitors", 20);

	
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
	Actor.SetMaxShd(greywolf, 1.2 * Actor.GetMaxShd(greywolf));
	Actor.SetMaxHull(greywolf, 1.2 * Actor.GetMaxHull(greywolf));
	Actor.SetShd(greywolf, Actor.GetMaxShd(greywolf));
	Actor.SetHull(greywolf, Actor.GetMaxHull(greywolf));
	Actor.SetProperty(greywolf, "Spawner.Enabled", true);
	Actor.SetProperty(greywolf, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIEI"});
	AI.QueueLast(greywolf, "move", {-1000, 400, -3000}, 15);
	AI.QueueLast(greywolf, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(greywolf, "lock");

	corvus = Actor.Spawn("INTD", "INT CORVUS", "Empire", "CORVUS", 0, { 3500, -500, 500 }, { 0, -130, 0 }, { "CriticalAllies" });
	Actor.SetMaxShd(corvus, 2 * Actor.GetMaxShd(corvus));
	Actor.SetMaxHull(corvus, 2 * Actor.GetMaxHull(corvus));
	Actor.SetShd(corvus, Actor.GetMaxShd(corvus));
	Actor.SetHull(corvus, Actor.GetMaxHull(corvus));
	AI.QueueLast(corvus, "move", {2400, -500, -1000}, 3);
	AI.QueueLast(corvus, "lock");

	ebolo = Actor.Spawn("STRKC", "EBOLO", "Empire", "EBOLO", 0, { 3000, -20, 350 }, { 0, -110, 25 });
	Actor.SetMaxShd(ebolo, 2 * Actor.GetMaxShd(ebolo));
	Actor.SetMaxHull(ebolo, 1.5 * Actor.GetMaxHull(ebolo));
	Actor.SetShd(ebolo, Actor.GetMaxShd(ebolo));
	Actor.SetHull(ebolo, Actor.GetMaxHull(ebolo));
	AI.QueueLast(ebolo, "move", {-2470, 500, -7350}, 20);
	AI.QueueLast(ebolo, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(ebolo, "lock");
	
	daring = Actor.Spawn("STRKC", "DARING", "Empire", "DARING", 0, { 3200, -300, -1450 }, { 0, -150, 25 });
	Actor.SetMaxShd(daring, 2 * Actor.GetMaxShd(daring));
	Actor.SetMaxHull(daring, 1.5 * Actor.GetMaxHull(daring));
	Actor.SetShd(daring, Actor.GetMaxShd(daring));
	Actor.SetHull(daring, Actor.GetMaxHull(daring));
	AI.QueueLast(daring, "move", {-3470, 300, -7350}, 10);
	AI.QueueLast(daring, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(daring, "lock");
	
	// Traitors
	
	glory = Actor.Spawn("IMPL", "ISD GLORY", "Traitors", "GLORY", 0, { -9750, 0, -32000 }, { 0, 40, 0 }, { "CriticalEnemies" });
	Actor.SetProperty(glory, "Spawner.Enabled", true);
	Actor.SetProperty(glory, "Spawner.SpawnTypes", {"TIE","TIEI","TIEI","TIEI"});
	Actor.SetMaxShd(glory, 2 * Actor.GetMaxShd(glory));
	Actor.SetMaxHull(glory, 2 * Actor.GetMaxHull(glory));
	Actor.SetShd(glory, Actor.GetMaxShd(glory));
	Actor.SetHull(glory, Actor.GetMaxHull(glory));
	AI.QueueLast(glory, "move", {-1000, 100, -6000}, 70);
	AI.QueueLast(glory, "move", {4000, 200, -10000}, 10);
	AI.QueueLast(glory, "rotate", {2000, 210, -20000}, 0);
	AI.QueueLast(glory, "lock");

	corv1 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -10750, 250, -30000 }, { 0, 20, 0 });
	Actor.SetMaxShd(corv1, 1.25 * Actor.GetMaxShd(corv1));
	Actor.SetMaxHull(corv1, 1.25 * Actor.GetMaxHull(corv1));
	Actor.SetShd(corv1, Actor.GetMaxShd(corv1));
	Actor.SetHull(corv1, Actor.GetMaxHull(corv1));
	AI.QueueLast(corv1, "move", {0, 250, -3000}, 70);
	AI.QueueLast(corv1, "move", {3000, 250, -5000}, 10);
	AI.QueueLast(corv1, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv1, "lock");

	corv2 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -12750, -100, -16000 }, { 0, 75, 0 });
	AI.QueueLast(corv2, "move", {1550, 200, -2200}, 100);
	AI.QueueLast(corv2, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv2, "lock");

	corv3 = Actor.Spawn("CORV", "CRV Z-DIVINE WIND", "Traitors", "", 0, { -6750, 300, -17500 }, { 0, 45, 0 });
	Actor.SetMaxShd(corv3, 2 * Actor.GetMaxShd(corv3));
	Actor.SetMaxHull(corv3, 2 * Actor.GetMaxHull(corv3));
	Actor.SetShd(corv3, Actor.GetMaxShd(corv3));
	Actor.SetHull(corv3, Actor.GetMaxHull(corv3));
	AI.QueueLast(corv3, "move", {750, -100, -800}, 100);
	AI.QueueLast(corv3, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv3, "lock");

	corv4 = Actor.Spawn("STRKC", "", "Traitors", "", 0, { -2750, 50, -19500 }, { 0, 20, 0 });
	AI.QueueLast(corv4, "move", {-750, 100, -1400}, 100);
	AI.QueueLast(corv4, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv4, "lock");
	
	nebu = Actor.Spawn("NEB2", "", "Traitors", "", 0, { -6750, -450, -21000 }, { 0, 30, 0 });
	AI.QueueLast(nebu, "move", {6250, -450, -3200}, 100);
	AI.QueueLast(nebu, "lock");
	
	
make_fighters:

	// Empire initial spawns
	
	tiea2 = Actor.Spawn("TIEA", "Alpha-2", "Empire", "", 0, { 700, -620, 10500 }, { 0, -180, 0 });
	Actor.SetMaxShd(tiea2, 4 * Actor.GetMaxShd(tiea2));
	Actor.SetShd(tiea2, Actor.GetMaxShd(tiea2));
	AI.QueueLast(tiea2, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea2);

	tiea3 = Actor.Spawn("TIEA", "Alpha-3", "Empire", "", 0, { 6000, 300, -500 }, { 0, -90, 0 });
	Actor.SetMaxShd(tiea3, 4 * Actor.GetMaxShd(tiea3));
	Actor.SetShd(tiea3, Actor.GetMaxShd(tiea3));
	AI.QueueLast(tiea3, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea3);

	tiea4 = Actor.Spawn("TIEA", "Alpha-4", "Empire", "", 0, { 6500, 600, -750 }, { 0, -90, 0 });
	Actor.SetMaxShd(tiea4, 4 * Actor.GetMaxShd(tiea4));
	Actor.SetShd(tiea4, Actor.GetMaxShd(tiea4));
	AI.QueueLast(tiea4, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea4);

	tiea5 = Actor.Spawn("TIEA", "Alpha-5", "Empire", "", 0, { 7000, 300, -500 }, { 0, -90, 0 });
	Actor.SetMaxShd(tiea5, 4 * Actor.GetMaxShd(tiea5));
	Actor.SetShd(tiea5, Actor.GetMaxShd(tiea5));
	AI.QueueLast(tiea5, "wait", 2.5);
	Squad.AddToSquad(Player.GetActor(), tiea5);

	tiea6 = Actor.Spawn("TIEA", "Alpha-6", "Empire", "", 0, { 7500, 600, -750 }, { 0, -90, 0 });
	Actor.SetMaxShd(tiea6, 4 * Actor.GetMaxShd(tiea6));
	Actor.SetShd(tiea6, Actor.GetMaxShd(tiea6));
	AI.QueueLast(tiea6, "wait", 2.5);
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
	spawn_pos = { 400, -100, -5000 };
	spawn_rot = { 0, 0, 0 };
	spawn_type = "TIE";
	Script.Call("spawn4");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tie_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	spawn_pos = { -400, -400, -5200 };
	Script.Call("spawn4");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tie_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	spawn_pos = { -2000, 0, -9500 };
	spawn_target = corvus;
	spawn_type = "TIESA";
	Script.Call("spawn2");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tiesa_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	spawn_pos = { -4600, 200, -7200 };
	spawn_target = corvus;
	spawn_type = "TIE";
	Script.Call("spawn4");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tie_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	spawn_pos = { -5400, -150, -6600 };
	spawn_target = -1;
	Script.Call("spawn4");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tie_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	// Empire spawn reserves
	
	for (int i = 1; i <= 8; i += 1)
		Actor.QueueAtSpawner(Actor.Spawn("TIE", "", "Empire", "", 0, _d, _d), greywolf);
	
	// Traitor spawn reserves
	
	for (int i = 1; i <= 8; i += 1)
	{
		_a = Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d);
		Actor.SetMaxShd(_a, z_tiei_shd);
		Actor.SetShd(_a, Actor.GetMaxShd(_a));
		Actor.SetProperty(_a, "SelfRegenRate", z_tie_shdregen);
		Actor.QueueAtSpawner(_a, glory);
		
		//Actor.QueueAtSpawner(Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d), glory);
	}

gametick:
	UI.SetLine1Text("WINGS: " + Faction.GetWingCount("Empire"));
	UI.SetLine2Text("ENEMY: " + Faction.GetWingCount("Traitors"));
	
	if (!triggerwinlose)
	{
		float hp = Actor.GetHP(greywolf);
		if (hp <= 0) 
			Script.Call("losegreywolf");
		else if (!triggerdangergreywolf && hp < 450) 
			Script.Call("dangergreywolf");

		hp = Actor.GetHP(corvus);
		if (hp <= 0) 
			Script.Call("losecorvus");
		else if (!triggerdangercorvus && hp < 300) 
			Script.Call("dangercorvus");
		
		foreach (int spr in {glory, nebu})
		{
			foreach (int sph in Actor.GetChildren(spr))
			{
				if (Actor.GetActorType(sph) == "HANGAR")
				{
					foreach (int sp in Actor.GetChildren(sph))
					{
						if (Actor.GetMaxShd(sp) == 0)
						{
							string atype = Actor.GetActorType(sp);
							if (atype == "TIE")
							{
								Actor.SetMaxShd(sp, z_tie_shd);
								Actor.SetShd(sp, Actor.GetMaxShd(sp));
								Actor.SetProperty(sp, "SelfRegenRate", z_tie_shdregen);
							}
							else if (atype == "TIEI")
							{
								Actor.SetMaxShd(sp, z_tiei_shd);
								Actor.SetShd(sp, Actor.GetMaxShd(sp));
								Actor.SetProperty(sp, "SelfRegenRate", z_tie_shdregen);
							}
							else if (atype == "TIESA")
							{
								Actor.SetMaxShd(sp, z_tiesa_shd);
								Actor.SetShd(sp, Actor.GetMaxShd(sp));
								Actor.SetProperty(sp, "SelfRegenRate", z_tie_shdregen);
							}
						}
					}
				}
			}
		}
		
		if (!triggerescapecorv)
		{
			hp = Actor.GetHP(glory);
			if (hp < 100) 
				Script.Call("spawn_escapeCORV");
		}	
		else
		{
			if (GetGameStateB("Escaped"))
				Script.Call("loseescaped");
				
			if (GetGameStateB("EpsilonSpawned"))
			{
				SetGameStateB("EpsilonSpawned", false);
				Script.Call("announce_allyEpsilon");
				Audio.SetMood(-11);
			}
			
			if (!GetGameStateB("VorknkxDestroyed"))
			{
				hp = Actor.GetHP(vorknkx);
				if (hp <= 0) 
				{
					Script.Call("announce_VorknkxDestroyed");
					SetGameStateB("VorknkxDestroyed", true);
					Audio.SetMood(-4);
				}			
				else if (!GetGameStateB("VorknkxShieldsDown"))
				{
					float shd = Actor.GetShd(vorknkx);
					if (shd <= 0) 
					{
						Script.Call("announce_VorknkxShieldsDown");
						SetGameStateB("VorknkxShieldsDown", true);
					}
				}
			}
		}
	
		if (Faction.GetShipCount("Traitors") < 1 && !triggerwinlose) 
			Script.Call("win");

		if (GetGameStateB("EtaSpawned"))
		{
			SetGameStateB("EtaSpawned", false);
			Script.Call("announce_allyEta");
			Audio.SetMood(-11);
		}
			
		if (GetGameStateB("BetaSpawned"))
		{
			SetGameStateB("BetaSpawned", false);
			Script.Call("announce_allyBeta");
			Audio.SetMood(-11);
		}
		
		if (!GetGameStateB("EboloDestroyed"))
		{
			hp = Actor.GetHP(ebolo);
			if (hp <= 0) 
			{
				Script.Call("announce_EboloDestroyed");
				SetGameStateB("EboloDestroyed", true);
				Audio.SetMood(-14);
			}			
			else if (!GetGameStateB("EboloShieldsDown"))
			{
				float shd = Actor.GetShd(ebolo);
				if (shd <= 0) 
				{
					Script.Call("announce_EboloShieldsDown");
					SetGameStateB("EboloShieldsDown", true);
				}
			}
		}
		
		if (!GetGameStateB("DaringDestroyed"))
		{
			hp = Actor.GetHP(daring);
			if (hp <= 0) 
			{
				Script.Call("announce_DaringDestroyed");
				SetGameStateB("DaringDestroyed", true);
				Audio.SetMood(-14);
			}			
			else if (!GetGameStateB("DaringShieldsDown"))
			{
				float shd = Actor.GetShd(daring);
				if (shd <= 0) 
				{
					Script.Call("announce_DaringShieldsDown");
					SetGameStateB("DaringShieldsDown", true);
				}
			}
		}
		
		if (!GetGameStateB("GloryDestroyed"))
		{
			hp = Actor.GetHP(glory);
			if (hp <= 0) 
			{
				Script.Call("announce_GloryDestroyed");
				SetGameStateB("GloryDestroyed", true);
				Audio.SetMood(-5);
				AddEvent(15, "setmood4");
			}			
			else if (!GetGameStateB("GloryShieldsDown"))
			{
				float shd = Actor.GetShd(glory);
				if (shd <= 0) 
				{
					Script.Call("announce_GloryShieldsDown");
					SetGameStateB("GloryShieldsDown", true);
				}
			}
		}
	}
	
	
setmood4:
	Audio.SetMood(4);

	
win:
	triggerwinlose = true;
	Script.Call("messagewin");
	AddEvent(4, "messagewin2");
	SetGameStateB("GameWon",true);
	AddEvent(8, "fadeout");

	
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
	Audio.SetMood(-7);

	
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
	Audio.SetMood(-7);

	
loseescaped:
	triggerwinlose = true;
	Script.Call("messageloseescaped");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");
	Audio.SetMood(-7);
	
	
start:
	Player.SetMovementEnabled(true);

	
corvusbeginspawn:
	Faction.SetWingSpawnLimit("Empire", 30);
	Actor.SetProperty(corvus, "Spawner.Enabled", true);
	Actor.SetProperty(corvus, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIEI"});


enemybeginspawn:
	Faction.SetWingSpawnLimit("Traitors", 26);
	Actor.SetProperty(nebu, "Spawner.Enabled", true);
	Actor.SetProperty(nebu, "Spawner.SpawnTypes", {"TIE","TIEI"});
	Actor.SetProperty(glory, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIE","TIEI","TIEI","TIEI","TIEI","JV7"});
	AI.QueueFirst(ebolo, "move", {-3470, -100, -10350}, 100);
	AI.QueueFirst(daring, "move", {-3470, -300, -7350}, 100);
	
	
spawnenemybombers:
	spawn_faction = "Traitors";
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_pos = { -2000, 250, -22500 };
	spawn_rot = { 0, 0 ,0 };
	spawn_type = "TIESA";
	spawn_target = corvusshd1;
	Script.Call("spawn1");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tiesa_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	spawn_pos = { -4500, 50, -16000 };
	spawn_target = corvusshd2;
	Script.Call("spawn2");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tiesa_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	spawn_pos = { -5000,-150,-24500 };
	spawn_target = corvus;
	Script.Call("spawn2");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tiesa_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	spawn_pos = { -25000, 300, -23500 };
	spawn_type = "TIEI";
	spawn_target = corvusshd1;
	Script.Call("spawn1");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tiei_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	spawn_pos = { -24000, 200, 24500 };
	spawn_type = "TIE";
	spawn_target = corvus;
	Script.Call("spawn2");
	foreach (int a in spawn_ids)
	{
		Actor.SetMaxShd(a, z_tie_shd);
		Actor.SetShd(a, Actor.GetMaxShd(a));
		Actor.SetProperty(a, "SelfRegenRate", z_tie_shdregen);
	}	
	
	
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


spawn_traitorZDelta:
	for (int i = 1; i <= 6; i += 1)
	{
		_a = Actor.Spawn("TIED", "Z-Delta-" + i, "Traitors", "", 0, _d, _d);
		if (i <= 4)
			AI.QueueLast(_a, "attackactor", daring, -1, -1, false);
		else
			AI.QueueLast(_a, "attackactor", corvus, -1, -1, false);
		
		Actor.QueueAtSpawner(_a, glory);
	}
	
	for (int i = 1; i <= 2; i += 1)
	{
		_a = Actor.Spawn("TIESA", "", "Traitors", "", 0, _d, _d);
		AI.QueueLast(_a, "attackactor", corvus, -1, -1, false, 40);
		Actor.QueueAtSpawner(_a, glory);
	}
	
	Audio.SetMood(-21);
	Script.Call("message_traitorZDelta");

	
spawn_traitorZIota:
	
	for (int k = 0; k <= 2; k += 1)
	{
		for (int i = 1; i <= 2; i += 1)
		{
			_a = Actor.Spawn("TIED", "Z-Iota-" + (k * 2 + i), "Traitors", "", 0, _d, _d);
			Actor.QueueAtSpawner(_a, glory);
		}
		
		for (int i = 1; i <= 2; i += 1)
		{
			if (k == 2)
				_a = Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d);
			else
				_a = Actor.Spawn("TIESA", "", "Traitors", "", 0, _d, _d);
			AI.QueueLast(_a, "attackactor", greywolf, -1, -1, false, 30);
			Actor.QueueAtSpawner(_a, glory);
		}
	}

	Audio.SetMood(-21);
	Script.Call("message_traitorZIota");


spawn_traitorZTheta:
	for (int k = 0; k <= 2; k += 1)
	{
		for (int i = 1; i <= 2; i += 1)
		{
			_a = Actor.Spawn("TIED", "Z-Theta-" + (k * 2 + i), "Traitors", "", 0, _d, _d);
			Actor.QueueAtSpawner(_a, glory);
		}
		
		for (int i = 1; i <= 2; i += 1)
		{
			_a = Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d);
			Actor.QueueAtSpawner(_a, glory);
		}
	}

	Audio.SetMood(-21);
	Script.Call("message_traitorZTheta");

	
spawn_escapeCORV:
	triggerescapecorv = true;
	float3 pos = Actor.GetGlobalPosition(gloryhangar);
	float3 rot = Actor.GetGlobalRotation(glory);
	rot = {rot[0] + 45, rot[1], rot[2]};
	vorknkx = Actor.Spawn("CORV", "CRV VORKNKX", "Traitors", "VORKNKX", 0, pos, rot, { "CriticalEnemies" });
	Actor.SetMaxShd(vorknkx, 2 * Actor.GetMaxShd(vorknkx));
	Actor.SetMaxHull(vorknkx, 2 * Actor.GetMaxHull(vorknkx));
	Actor.SetShd(vorknkx, Actor.GetMaxShd(vorknkx));
	Actor.SetHull(vorknkx, Actor.GetMaxHull(vorknkx));
	Actor.SetProperty(greywolf, "Spawner.SpawnTypes", {"TIE","TIE","TIEI","TIEI"});
	AI.QueueLast(vorknkx, "wait", 2.5);
	AI.QueueLast(vorknkx, "move", {-9750, 0, -22000}, 100);
	AI.QueueLast(vorknkx, "hyperspaceout");
	AI.QueueLast(vorknkx, "setgamestateb", "Escaped", true);
	AI.QueueLast(vorknkx, "delete");
	Script.Call("messageescape");
	AddEvent(10, "messageescape2");
	AddEvent(17, "spawn_allyEpsilon");


spawn_allyEpsilon:
	_a = Actor.Spawn("TIESA", "Epsilon-1", "Empire", "", 0, _d, _d);
	Actor.QueueAtSpawner(_a, greywolf);
	AI.QueueLast(_a, "setgamestateb", "EpsilonSpawned", true);
	AI.QueueLast(_a, "attackactor", vorknkx, -1, -1, false, 40);

	_a = Actor.Spawn("TIESA", "Epsilon-2", "Empire", "", 0, _d, _d);
	AI.QueueLast(_a, "attackactor", vorknkx, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	_a = Actor.Spawn("TIEI", "Gamma-1", "Empire", "", 0, _d, _d);
	AI.QueueLast(_a, "attackactor", vorknkx, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	_a = Actor.Spawn("TIEI", "Gamma-2", "Empire", "", 0, _d, _d);
	AI.QueueLast(_a, "attackactor", vorknkx, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	

spawn_allyEta:
	for (int i = 1; i <= 4; i += 1)
	{
		_a = Actor.Spawn("TIEI", "Eta-" + i, "Empire", "", 0, _d, _d);
		Actor.SetMaxHull(_a, 2 * Actor.GetMaxHull(_a));
		Actor.SetHull(_a, Actor.GetMaxHull(_a));
		Actor.QueueAtSpawner(_a, greywolf);
		if (i == 1)
			AI.QueueLast(_a, "setgamestateb", "EtaSpawned", true);
	}
	
	
spawn_allyBeta:

	for (int k = 0; k <= 1; k += 1)
	{
		for (int i = 1; i <= 4; i += 1)
		{
			if (i == 4)
			{
				_a = Actor.Spawn("TIE", "", "Empire", "", 0, _d, _d);
				Actor.QueueAtSpawner(_a, greywolf);
				AI.QueueLast(_a, "attackactor", glory, -1, -1, false, 120);
			}
			else
			{
				_a = Actor.Spawn("TIESA", "Beta-" + (k * 3 + i), "Empire", "", 0, _d, _d);
				
				Actor.QueueAtSpawner(_a, greywolf);
				if (i == 1 && k == 0)
				{
					AI.QueueLast(_a, "setgamestateb", "BetaSpawned", true);
				}
				AI.QueueLast(_a, "attackactor", glory, -1, -1, false, 125);
			}	
		}
	}

	
announce_allyEta:
	Script.Call("message_allyEta");

	
announce_allyBeta:
	Script.Call("message_allyBeta");
	AddEvent(4, "message_allyBeta2");
	
	
announce_allyEpsilon:
	Script.Call("message_allyEpsilon");
	AddEvent(3, "message_allyGamma");
	AddEvent(12, "messageescape3");

	
announce_EboloShieldsDown:
	Script.Call("message_EboloShieldsDown");

	
announce_DaringShieldsDown:
	Script.Call("message_DaringShieldsDown");

	
announce_EboloDestroyed:
	Script.Call("message_EboloDestroyed");

	
announce_DaringDestroyed:
	Script.Call("message_DaringDestroyed");
	
	
announce_GloryShieldsDown:
	Script.Call("message_GloryShieldsDown");

	
announce_VorknkxShieldsDown:
	Script.Call("message_VorknkxShieldsDown");

	
announce_GloryDestroyed:
	Script.Call("message_GloryDestroyed");

	
announce_VorknkxDestroyed:
	Script.Call("message_VorknkxDestroyed");

