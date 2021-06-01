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
int corv1_mines = 12;
int corv2;
int corv2_mines = 12;
int corv3;
int corv3_mines = 12;
int corv4;
int corv4_mines = 12;
int vorknkx;
int vork_mines = 16;

int greywolfshd1;
int greywolfshd2;
int corvusshd1;
int corvusshd2;
int gloryhangar;

int tiea2;

float3 faction_empire_color = { 0, 0.8, 0 };
float3 faction_traitor_color = { 0.4, 0.5, 0.9 };
float3 faction_rebel_color = { 0.8, 0, 0 };

float3 faction_empire_laser_color = { 0.1, 1, 0.12 };
float3 faction_rebel_laser_color = { 1, 0, 0 };

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
	
	Script.Call("spawn_reset");
	Script.Call("actorp_reset");
	
	Script.Call("engagemusic");	
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
	Faction.Add("Empire", faction_empire_color, faction_empire_laser_color);
	Faction.Add("Traitors", faction_traitor_color, faction_empire_laser_color);
	Faction.Add("Rebels", faction_rebel_color, faction_rebel_laser_color);
	Faction.MakeAlly("Traitors", "Rebels");
	Faction.SetWingSpawnLimit("Empire", 24);
	Faction.SetWingSpawnLimit("Traitors", 20);

	
loadscene:

	
getchildren:
	int[] greywolfc = Actor.GetChildrenByType(greywolf, "ISD_SHD");
	greywolfshd1 = greywolfc[0];
	greywolfshd2 = greywolfc[1];
	int[] corvusc = Actor.GetChildrenByType(corvus, "SHD");
	corvusshd1 = corvusc[0];
	corvusshd2 = corvusc[1];
	int[] gloryc = Actor.GetChildrenByType(glory, "HANGAR");	
	gloryhangar = gloryc[0];

	
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
		Script.Call("make_squad");

	
firstspawn:
	Player.AssignActor(Actor.Spawn(GetPlayerActorType(), GetPlayerName(), "Empire", "", 0, { 4200, -950, 450 }, { 0, -130, 0 })); //{ 500, -300, 12500 }, { 0, -180, 0 }));
	Player.SetMovementEnabled(false);
	Actor.SetProperty(Player.GetActor(), "Movement.Speed", 30);
	if (Actor.IsLargeShip(Player.GetActor()))
		Actor.SetLocalPosition(Player.GetActor(), { 6000, -300, 0 });
	respawn = true;

	
make_squad:
	Squad.JoinSquad(Player.GetActor(), tiea2);

	
make_ships:

	// Empire
	
	greywolf = Actor.Spawn("ISD", "ISD GREY WOLF (Thrawn)", "Empire", "GREY WOLF", 0, { 1000, 400, 12000 }, { 0, -180, 0 }, { "CriticalAllies" });
	actorp_id = greywolf;
	actorp_multiplier = 1.2;
	Script.Call("actorp_multShd");
	Script.Call("actorp_multHull");
	Actor.SetProperty(greywolf, "Spawner.Enabled", true);
	Actor.SetProperty(greywolf, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIEI"});
	AI.QueueLast(greywolf, "move", {-1000, 400, -3000}, 15);
	AI.QueueLast(greywolf, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(greywolf, "lock");

	corvus = Actor.Spawn("INTD", "INT CORVUS", "Empire", "CORVUS", 0, { 3500, -500, 500 }, { 0, -130, 0 }, { "CriticalAllies" });
	actorp_id = corvus;
	actorp_multiplier = 2;
	Script.Call("actorp_multShd");
	Script.Call("actorp_multHull");
	AI.QueueLast(corvus, "move", {2400, -500, -1000}, 3);
	AI.QueueLast(corvus, "lock");

	ebolo = Actor.Spawn("STRKC", "EBOLO", "Empire", "EBOLO", 0, { 3000, -20, 350 }, { 0, -110, 25 });
	actorp_id = ebolo;
	actorp_multiplier = 2;
	Script.Call("actorp_multShd");
	actorp_multiplier = 1.5;
	Script.Call("actorp_multHull");
	AI.QueueLast(ebolo, "move", {200, 450, -3500}, 6);
	AI.QueueLast(ebolo, "move", {-2470, 500, -7350}, 6);
	AI.QueueLast(ebolo, "move", {3100, 500, -11350}, 4);
	AI.QueueLast(ebolo, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(ebolo, "lock");
	
	daring = Actor.Spawn("STRKC", "DARING", "Empire", "DARING", 0, { 3200, -300, -1450 }, { 0, -150, 25 });
	actorp_id = daring;
	actorp_multiplier = 2;
	Script.Call("actorp_multShd");
	actorp_multiplier = 1.5;
	Script.Call("actorp_multHull");
	AI.QueueLast(daring, "move", {-140, 350, -7250}, 6);
	AI.QueueLast(daring, "move", {1470, 350, -10050}, 3);
	AI.QueueLast(daring, "move", {5100, 300, -12350}, 3);
	AI.QueueLast(daring, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(daring, "lock");
	
	// Traitors
	
	glory = Actor.Spawn("ISD", "ISD GLORY", "Traitors", "GLORY", 0, { -9750, 0, -32000 }, { 0, 40, 0 }, { "CriticalEnemies" });
	Actor.SetProperty(glory, "Spawner.Enabled", true);
	Actor.SetProperty(glory, "Spawner.SpawnTypes", {"TIE","TIEI","TIEI","TIEI"});
	actorp_id = glory;
	actorp_multiplier = 2;
	Script.Call("actorp_multShd");
	Script.Call("actorp_multHull");
	AI.QueueLast(glory, "move", {-1000, 100, -6000}, 70);
	AI.QueueLast(glory, "move", {4000, 200, -10000}, 10);
	AI.QueueLast(glory, "rotate", {2000, 210, -20000}, 0);
	AI.QueueLast(glory, "lock");

	corv1 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -10750, 250, -30000 }, { 0, 20, 0 });
	actorp_id = corv1;
	actorp_multiplier = 1.25;
	Script.Call("actorp_multShd");
	Script.Call("actorp_multHull");
	AI.QueueLast(corv1, "move", {0, 250, -3000}, 70);
	AI.QueueLast(corv1, "setgamestateb", "BeginCorv1Mine", true);
	AI.QueueLast(corv1, "move", {3000, 250, -5000}, 10);
	AI.QueueLast(corv1, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv1, "lock");
	
	corv2 = Actor.Spawn("CORV", "", "Traitors", "", 0, { -12750, -100, -16000 }, { 0, 75, 0 });
	AI.QueueLast(corv2, "move", {1550, 200, -2200}, 100);
	AI.QueueLast(corv2, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv2, "lock");

	corv3 = Actor.Spawn("CORV", "CRV Z-DIVINE WIND", "Traitors", "", 0, { -6750, 300, -17500 }, { 0, 45, 0 });
	actorp_id = corv3;
	actorp_multiplier = 2;
	Script.Call("actorp_multShd");
	Script.Call("actorp_multHull");
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
	
	AddEvent(3, "spawn_traitorMines_corv2");
	AddEvent(4, "spawn_traitorMines_corv3");
	AddEvent(5, "spawn_traitorMines_corv4");

	
make_fighters:

	// Empire initial spawns
	
	tiea2 = Actor.Spawn("TIEA", "ALPHA 2", "Empire", "", 0, { 700, -620, 10500 }, { 0, -180, 0 });

	Actor.Spawn("TIEI", "", "Empire", "", 0, { 6000, 300, -500 }, { 0, -90, 0 });
	Actor.Spawn("TIEI", "", "Empire", "", 0, { 6500, 600, -750 }, { 0, -90, 0 });
	Actor.Spawn("TIEI", "", "Empire", "", 0, { 7000, 300, -500 }, { 0, -90, 0 });
	Actor.Spawn("TIEI", "", "Empire", "", 0, { 7500, 600, -750 }, { 0, -90, 0 });

	foreach (int a in {tiea2})
	{
		actorp_id = a;
		actorp_multiplier = 4;
		Script.Call("actorp_multShd");
		AI.QueueLast(a, "wait", 2.5);
		Squad.JoinSquad(Player.GetActor(), a);
	}
	
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
		actorp_id = a;
		actorp_value = z_tie_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	spawn_pos = { -400, -400, -5200 };
	Script.Call("spawn4");
	foreach (int a in spawn_ids)
	{
		actorp_id = a;
		actorp_value = z_tie_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	spawn_pos = { -2000, 0, -9500 };
	spawn_target = corvus;
	spawn_type = "TIESA";
	Script.Call("spawn2");
	foreach (int a in spawn_ids)
	{
		actorp_id = a;
		actorp_value = z_tiesa_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	spawn_pos = { -4600, 200, -7200 };
	spawn_target = corvus;
	spawn_type = "TIE";
	Script.Call("spawn4");
	foreach (int a in spawn_ids)
	{
		actorp_id = a;
		actorp_value = z_tie_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	spawn_pos = { -5400, -150, -6600 };
	spawn_target = -1;
	Script.Call("spawn4");
	foreach (int a in spawn_ids)
	{
		actorp_id = a;
		actorp_value = z_tie_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	// Empire spawn reserves
	
	for (int i = 1; i <= 8; i += 1)
		Actor.QueueAtSpawner(Actor.Spawn("TIE", "", "Empire", "", 0, _d, _d), greywolf);
	
	// Traitor spawn reserves
	
	for (int i = 1; i <= 8; i += 1)
	{
		int a = Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d);
		actorp_id = a;
		actorp_value = z_tiei_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
		Actor.QueueAtSpawner(a, glory);
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
								actorp_id = sp;
								actorp_value = z_tie_shd;
								Script.Call("actorp_setShd");
								Actor.SetProperty(sp, "Regen.Self", z_tie_shdregen);
							}
							else if (atype == "TIEI")
							{
								actorp_id = sp;
								actorp_value = z_tiei_shd;
								Script.Call("actorp_setShd");
								Actor.SetProperty(sp, "Regen.Self", z_tie_shdregen);
							}
							else if (atype == "TIESA")
							{
								actorp_id = sp;
								actorp_value = z_tiesa_shd;
								Script.Call("actorp_setShd");
								Actor.SetProperty(sp, "Regen.Self", z_tie_shdregen);
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
		
		if (GetGameStateB("BeginCorv1Mine"))
		{
			SetGameStateB("BeginCorv1Mine", false);
			AddEvent(3, "spawn_traitorMines_corv1");
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
		actorp_id = a;
		actorp_value = z_tiesa_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	spawn_pos = { -4500, 50, -16000 };
	spawn_target = corvusshd2;
	Script.Call("spawn2");
	foreach (int a in spawn_ids)
	{
		actorp_id = a;
		actorp_value = z_tiesa_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	spawn_pos = { -5000,-150,-24500 };
	spawn_target = corvus;
	Script.Call("spawn2");
	foreach (int a in spawn_ids)
	{
		actorp_id = a;
		actorp_value = z_tiesa_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	spawn_pos = { -25000, 300, -23500 };
	spawn_type = "TIEI";
	spawn_target = corvusshd1;
	Script.Call("spawn1");
	foreach (int a in spawn_ids)
	{
		actorp_id = a;
		actorp_value = z_tiei_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	spawn_pos = { -24000, 200, 24500 };
	spawn_type = "TIE";
	spawn_target = corvus;
	Script.Call("spawn2");
	foreach (int a in spawn_ids)
	{
		actorp_id = a;
		actorp_value = z_tie_shd;
		Script.Call("actorp_setShd");
		Actor.SetProperty(a, "Regen.Self", z_tie_shdregen);
	}	
	
	
spawn_allyGUN:
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_type = "GUN";
	spawn_target = corv2;
	spawn_pos = { 2400,500,8000 };
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
	spawn_pos = { 2400,500,8000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn4");
	Audio.SetMood(-11);
	Script.Call("message_allyTIEA");


spawn_traitorZDelta:
	for (int i = 1; i <= 6; i += 1)
	{
		_a = Actor.Spawn("TIED", "Z-DELTA " + i, "Traitors", "", 0, _d, _d);
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
			_a = Actor.Spawn("TIED", "Z-IOTA " + (k * 2 + i), "Traitors", "", 0, _d, _d);
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
			_a = Actor.Spawn("TIED", "Z-THETA " + (k * 2 + i), "Traitors", "", 0, _d, _d);
			Actor.QueueAtSpawner(_a, glory);
			if (i == 2)
				Squad.JoinSquad(_a, glory);
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
	actorp_id = vorknkx;
	actorp_multiplier = 2;
	Script.Call("actorp_multShd");
	Actor.SetProperty(greywolf, "Spawner.SpawnTypes", {"TIE","TIE","TIEI","TIEI"});
	AI.QueueLast(vorknkx, "wait", 2.5);
	AI.QueueLast(vorknkx, "move", {-9750, 0, -22000}, 100);
	AI.QueueLast(vorknkx, "hyperspaceout");
	AI.QueueLast(vorknkx, "setgamestateb", "Escaped", true);
	AI.QueueLast(vorknkx, "delete");
	Script.Call("messageescape");
	AddEvent(6, "spawn_traitorMines_vork");
	AddEvent(10, "messageescape2");
	AddEvent(17, "spawn_allyEpsilon");


spawn_traitorMines_corv1:
	if (Actor.IsAlive(corv1) && corv1_mines > 0)
	{
		float3 pos = Actor.GetGlobalPosition(corv1);
		float3 fac = Actor.GetGlobalDirection(corv1);
		pos -= fac * 57;
		pos -= {Random(-20, 20), 55, Random(-20, 20)};
		float3 rot = { Random(360), Random(360), Random(360)};
		
		Actor.Spawn("MINE2", "", "Traitors", "", 0, pos, rot);
		corv1_mines -= 1;
		AddEvent(5 + 10 * Random(), "spawn_traitorMines_corv1");
	}


spawn_traitorMines_corv2:
	if (Actor.IsAlive(corv2) && corv2_mines > 0)
	{
		float3 pos = Actor.GetGlobalPosition(corv2);
		float3 fac = Actor.GetGlobalDirection(corv2);
		pos -= fac * 57;
		pos -= {Random(-20, 20), 55, Random(-20, 20)};
		float3 rot = { Random(360), Random(360), Random(360)};
		
		string type = (corv2_mines == 13) ? "MINE3" : "MINE1";
		Actor.Spawn(type, "", "Traitors", "", 0, pos, rot);
		corv2_mines -= 1;
		AddEvent(5 + 10 * Random(), "spawn_traitorMines_corv2");
	}


spawn_traitorMines_corv3:
	if (Actor.IsAlive(corv3) && corv3_mines > 0)
	{
		float3 pos = Actor.GetGlobalPosition(corv3);
		float3 fac = Actor.GetGlobalDirection(corv3);
		pos -= fac * 57;
		pos -= {Random(-20, 20), 55, Random(-20, 20)};
		float3 rot = { Random(360), Random(360), Random(360)};
		
		string type = (corv3_mines == 12) ? "MINE3" : "MINE1";
		Actor.Spawn(type, "", "Traitors", "", 0, pos, rot);
		corv3_mines -= 1;
		AddEvent(5 + 10 * Random(), "spawn_traitorMines_corv3");
	}


spawn_traitorMines_corv4:
	if (Actor.IsAlive(corv4) && corv4_mines > 0)
	{
		float3 pos = Actor.GetGlobalPosition(corv4);
		float3 fac = Actor.GetGlobalDirection(corv4);
		pos -= fac * 57;
		pos -= {Random(-20, 20), 55, Random(-20, 20)};
		float3 rot = { Random(360), Random(360), Random(360)};
		
		string type = (corv4_mines == 6) ? "MINE3" : "MINE2";
		Actor.Spawn(type, "", "Traitors", "", 0, pos, rot);
		corv4_mines -= 1;
		AddEvent(5 + 10 * Random(), "spawn_traitorMines_corv4");
	}


spawn_traitorMines_vork:
	if (Actor.IsAlive(vorknkx) && vork_mines > 0)
	{
		float3 pos = Actor.GetGlobalPosition(vorknkx);
		float3 fac = Actor.GetGlobalDirection(vorknkx);
		pos -= fac * 57;
		pos -= {Random(-20, 20), 55, Random(-20, 20)};
		float3 rot = { Random(360), Random(360), Random(360)};
		
		string type = (vork_mines == 4 || vork_mines == 11 || vork_mines == 12) ? "MINE3" : "MINE2";
		Actor.Spawn(type, "", "Traitors", "", 0, pos, rot);
		vork_mines -= 1;
		AddEvent(1.5 + 4.5 * Random(), "spawn_traitorMines_vork");
	}


spawn_allyEpsilon:
	_a = Actor.Spawn("TIESA", "EPSILON 1", "Empire", "", 0, _d, _d);
	Actor.QueueAtSpawner(_a, greywolf);
	AI.QueueLast(_a, "setgamestateb", "EpsilonSpawned", true);
	AI.QueueLast(_a, "attackactor", vorknkx, -1, -1, false, 40);

	_a = Actor.Spawn("TIESA", "EPSILON 2", "Empire", "", 0, _d, _d);
	AI.QueueLast(_a, "attackactor", vorknkx, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	_a = Actor.Spawn("TIEI", "GAMMA 1", "Empire", "", 0, _d, _d);
	AI.QueueLast(_a, "attackactor", vorknkx, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	
	_a = Actor.Spawn("TIEI", "GAMMA 2", "Empire", "", 0, _d, _d);
	AI.QueueLast(_a, "attackactor", vorknkx, -1, -1, false, 40);
	Actor.QueueAtSpawner(_a, greywolf);
	

spawn_allyEta:
	for (int i = 1; i <= 4; i += 1)
	{
		int a = Actor.Spawn("TIE", "TEA " + i, "Empire", "", 0, _d, _d);
		actorp_id = a;
		actorp_multiplier = 2;
		Script.Call("actorp_multShd");
		Actor.QueueAtSpawner(a, greywolf);
		if (i == 1)
			AI.QueueLast(a, "setgamestateb", "EtaSpawned", true);
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
				_a = Actor.Spawn("TIESA", "BETA " + (k * 3 + i), "Empire", "", 0, _d, _d);
				
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


