// Globals

bool combat;
bool respawn;
bool triggerwinlose;
bool primary_completed;
bool scutz_arrived;
bool hammer_arrived;

float msg_time = 32;
float hammer_time = 240;

int outpost;
int hammer;
int hammer_hangar;
int tie_alpha2;
int tie_alpha3;
int ubote1;
int ubote2;
int ubote3;

int[] tie_delta_group;
int[] tie_gamma_group;

int scutz;
int mu;

float3 faction_empire_color = { 0, 0.8, 0 };
float3 faction_empire_hammer_color = { 0.4, 0.9, 0.7 };
float3 faction_neutral_color = { 0.7, 0.7, 0.7 };
float3 faction_neutral_rebel_color = { 0.8, 0.3, 0.3 };
float3 faction_rebel_color = { 0.8, 0, 0 };
float3 faction_mugaari_color = { 0.8, 0.1, 0.6 };

float3 faction_empire_laser_color = { 0.1, 1, 0.12 };
float3 faction_rebel_laser_color = { 1, 0, 0 };

float3 _d = { 0, 0, 0 };
int _a = -1;
float _f = 0;

load:
	Scene.SetMaxBounds({15000, 1500, 20000});
	Scene.SetMinBounds({-15000, -1500, -15000});
	Scene.SetMaxAIBounds({15000, 1500, 20000});
	Scene.SetMinAIBounds({-15000, -1500, -15000});

	Player.SetLives(5);
	Score.SetScorePerLife(1000000);
	Score.SetScoreForNextLife(1000000);
	Score.Reset();

	UI.SetLine1Color(faction_empire_hammer_color);
	UI.SetLine2Color(faction_empire_color);
	UI.SetLine3Color(faction_rebel_color);
	
	Script.Call("spawn_reset");
	Script.Call("actorp_reset");
	
	Script.Call("engagemusic");	
	Script.Call("make_ships");
	Script.Call("make_fighters");
	Script.Call("makeplayer");
	AddEvent(1.5, "start");
	
	AddEvent(3, "spawn_ywing1");
	AddEvent(7, "message1");
	AddEvent(10, "message2");
	AddEvent(14, "message3");
	AddEvent(20, "spawn_ywing2");
	AddEvent(28, "message4");
	AddEvent(msg_time, "message_protect");
	AddEvent(50, "spawn_xwing1");
	AddEvent(67, "spawn_xwing2");
	AddEvent(71, "spawn_ally_reinf");
	AddEvent(115, "spawn_scutz");
	AddEvent(90, "spawn_ywing3");
	AddEvent(130, "spawn_ywing4");
	AddEvent(180, "spawn_ubote");

	if (GetDifficulty() == "hard")
		AddEvent(195, "spawn_xwing3");

	AddEvent(hammer_time, "spawn_hammer");


loadfaction:
	Faction.Add("Empire", faction_empire_color, faction_empire_laser_color);
	Faction.Add("Empire_Hammer", faction_empire_hammer_color, faction_empire_laser_color);
	Faction.Add("Empire_Mu", faction_empire_hammer_color, faction_empire_laser_color);
	Faction.Add("Traitor", faction_empire_color, faction_empire_laser_color);
	Faction.Add("Neutral_Inspect", faction_neutral_color);
	Faction.Add("Neutral_Rebel", faction_neutral_rebel_color);
	Faction.Add("Rebels", faction_rebel_color, faction_rebel_laser_color);
	Faction.Add("Mugaari", faction_mugaari_color, faction_rebel_laser_color);
	Faction.MakeAlly("Empire", "Empire_Hammer");
	Faction.MakeAlly("Empire", "Empire_Mu");
	Faction.MakeAlly("Empire", "Neutral_Inspect");
	Faction.MakeAlly("Empire", "Neutral_Rebel");
	Faction.MakeAlly("Empire_Hammer", "Empire_Mu");
	Faction.MakeAlly("Empire_Hammer", "Neutral_Inspect");
	Faction.MakeAlly("Empire_Hammer", "Neutral_Rebel");
	Faction.MakeAlly("Rebels", "Empire_Mu");
	Faction.MakeAlly("Rebels", "Mugaari");
	Faction.MakeAlly("Rebels", "Neutral_Inspect");
	Faction.MakeAlly("Rebels", "Neutral_Rebel");
	Faction.MakeAlly("Mugaari", "Empire_Mu");
	Faction.MakeAlly("Mugaari", "Neutral_Inspect");
	Faction.MakeAlly("Mugaari", "Neutral_Rebel");

	Faction.SetWingSpawnLimit("Empire", 0);
	Faction.SetWingSpawnLimit("Empire_Hammer", 6);


loadscene:


engagemusic:
	Audio.SetMood(2);
    Audio.SetMusicDyn("TRO-IN");

	
makeplayer:
	Player.DecreaseLives();
	if (respawn) 
		Player.RequestSpawn(); 
	else 
		Script.Call("firstspawn");
	AddEvent(0.5, "squadform");


firstspawn:
	Player.AssignActor(Actor.Spawn(GetPlayerActorType(), GetPlayerName(), "Empire", "", 0, { 1200, 500, -3800 }, { 0, 0, 0 }));
	Player.SetMovementEnabled(false);
	Actor.SetProperty(Player.GetActor(), "Movement.Speed", 30);
	respawn = true;

	
squadform:
	Squad.JoinSquad(Player.GetActor(), tie_alpha2);
	Squad.JoinSquad(Player.GetActor(), tie_alpha3);


make_ships:

	// Empire
	outpost = Actor.Spawn("XQ1", "OUTPOST D-34", "Empire", "OUTP D-34", 0, { 0, 0, 0 }, { 0, -20, 0 }, { "CriticalAllies" });
	Actor.SetProperty(outpost, "Spawner.Enabled", true);
	Actor.SetProperty(outpost, "Spawner.SpawnTypes", {"TIE"});
	AI.QueueLast(outpost, "lock");

	Actor.Spawn("MINE1", "", "Empire", "", 0, { 2200, 150, 2600 }, { Random(360), Random(360), Random(360) });
	Actor.Spawn("MINE2", "", "Empire", "", 0, { -2100, -200, 2600 }, { Random(360), Random(360), Random(360) });
	Actor.Spawn("MINE2", "", "Empire", "", 0, { 3400, -100, 3400 }, { Random(360), Random(360), Random(360) });
	Actor.Spawn("MINE1", "", "Empire", "", 0, { -3200, 50, 3400 }, { Random(360), Random(360), Random(360) });
	Actor.Spawn("MINE3", "", "Empire", "", 0, { -400, 850, -1400 }, { Random(360), Random(360), Random(360) });

	
	
make_fighters:

	tie_alpha2 = Actor.Spawn("TIE", "ALPHA 2", "Empire", "", 0, { 600, 400, -4100 }, { 0, 5, 0 });
	//actorp_id = tie_alpha2;
	//actorp_multiplier = 2;
	//Script.Call("actorp_multHull");

	tie_alpha3 = Actor.Spawn("TIE", "ALPHA 3", "Empire", "", 0, { 1800, 350, -4000 }, { 0, -5, 0 });
	//actorp_id = tie_alpha3;
	//actorp_multiplier = 2;
	//Script.Call("actorp_multHull");
	
	Squad.JoinSquad(Player.GetActor(), tie_alpha2);
	Squad.JoinSquad(Player.GetActor(), tie_alpha3);

	
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	spawn_wait = 5;
	spawn_type = "TIE";
	spawn_name = "DELTA";
	spawn_spacing = 500;
	spawn_pos = { 4500, -300, 1500 };
	spawn_rot = { 20, -120 ,0 };
	Script.Call("spawn3");
	tie_delta_group = spawn_ids;
	
	spawn_wait = 8;
	spawn_name = "GAMMA";
	spawn_spacing = 500;
	spawn_pos = { -6500, -200, -2500 };
	spawn_rot = { -40, 120 ,0 };
	Script.Call("spawn3");
	tie_gamma_group = spawn_ids;


gametick:
	int wing_reb = Faction.GetWingCount("Rebels") + Faction.GetShipCount("Rebels");
	int wing_mug = Faction.GetWingCount("Mugaari");
	int wing_emp = Faction.GetWingCount("Empire");
	int wing_ham = Faction.GetWingCount("Empire_Hammer");
	float tm = hammer_time - GetGameTime();
	
	if (tm >= 0 && GetGameTime() > msg_time)
	{
		UI.SetLine1Color(faction_empire_hammer_color);
		UI.SetLine2Color(faction_empire_color);
		UI.SetLine3Color(faction_rebel_color);
		UI.SetLine1Text("HAMMER: " + Math.FormatAsTime(tm));
		UI.SetLine2Text("WINGS: " + (wing_emp + wing_ham));
		UI.SetLine3Text((wing_reb + wing_mug == 0) ? "" : "ENEMY: " + (wing_reb + wing_mug));	
	}
	else
	{
		UI.SetLine1Color(faction_empire_color);
		UI.SetLine2Color(faction_rebel_color);
		UI.SetLine1Text("WINGS: " + (wing_emp + wing_ham));
		UI.SetLine2Text((wing_reb + wing_mug == 0) ? "" : "ENEMY: " + (wing_reb + wing_mug));
		UI.SetLine3Text("");
	}
	
	if (!triggerwinlose)
	{
		if (combat && (wing_reb + wing_mug > 0) && Audio.GetMood() != 4)
			Audio.SetMood(4);
		else if (combat && (wing_reb + wing_mug == 0) && Audio.GetMood() != 0 && Audio.GetMood() != 5)
			Audio.SetMood(0);
	
		if (!GetGameStateB("OutpostDestroyed"))
		{
			float hp = Actor.GetHP(outpost);
			if (hp <= 0) 
			{
				SetGameStateB("OutpostDestroyed", true);
				Script.Call("lose_outpostlost");
			}
			
			if (!GetGameStateB("OutpostWeakened"))
			{
				if (hp <= 600) 
				{
					SetGameStateB("OutpostWeakened", true);
					Script.Call("messagewarn_outpost_1");
				}
			}
			
			if (!GetGameStateB("OutpostDanger"))
			{
				if (hp <= 300) 
					{
					SetGameStateB("OutpostDanger", true);
					for (float time = 0; time < 3; time += 0.4)
					{
						AddEvent(time, "messagewarn_outpost_o");
						AddEvent(time + 0.2, "messagewarn_outpost_y");
					}
				}
			}
		}
		
		if (GetGameStateB("ScutzDiscovered"))
		{
			if (!GetGameStateB("ScutzDisabled"))
			{
				float shd = Actor.GetShd(scutz);
				if (shd <= 0) 
				{
					float3 pos = Actor.GetGlobalPosition(scutz);
					float3 fac = Actor.GetGlobalDirection(scutz);
					pos += fac * 5000;
					AI.ForceClearQueue(scutz);
					AI.QueueLast(scutz, "rotate", pos, 0, 1, false);
					AI.QueueLast(scutz, "lock");
					Actor.SetFaction(scutz, "Neutral_Inspect");
					Actor.SetProperty(scutz, "Regen.Self", 0);
					Script.Call("message_scutz_disabled");
					SetGameStateB("ScutzDisabled", true);
				}
			}
		}
		
		if (hammer_arrived && !GetGameStateB("HammerAngry"))
		{
			float maxhp = Actor.GetMaxHP(hammer);
			float hp = Actor.GetHP(hammer);
			if (hp <= maxhp - 250) 
			{
				Actor.SetFaction(Player.GetActor(), "Traitor");
				SetGameStateB("HammerAngry", true);
				Script.Call("lose_hammerangry");
			}
		}
		
		if (GetGameStateB("MuDispatched"))
		{
			if (!GetGameStateB("MuBoarding"))
			{
				if (Math.GetActorDistance(mu, scutz) < 600)
				{
					SetGameStateB("MuBoarding", true);
					Script.Call("mu_boarding");
				}
			}
		}
		
		if (!GetGameStateB("ZetaSpawned") && GetDifficulty() != "hard")
		{
			if (wing_emp + wing_ham < 5)
			{
				SetGameStateB("ZetaSpawned", true);
				Script.Call("spawn_ally_reinf2");
			}
		}
		
		if (GetGameStateB("RebelsCaptured") && !GetGameStateB("RebelsCaptureAnnounced"))
		{
			SetGameStateB("RebelsCaptureAnnounced", true);
			Script.Call("message_rebelscaptured");
			AddEvent(4, "message_rebelscaptured_2");
		}
		
		if (primary_completed)
		{
			if (Math.GetActorDistance(Player.GetActor(), hammer_hangar) < 400)
			{
				Script.Call("win");
			}
		}

		Script.Call("inspection");
	}


inspection:	
	if (!GetGameStateB("ScutzInspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), scutz) < 200)
		{
			SetGameStateB("ScutzInspected", true);
			Script.Call("message_inspected_scutz");
			
			Actor.SetFaction(scutz, "Neutral_Rebel");
			AddEvent(0.2, "scutz_discovered");
			AddEvent(6, "message_located");
			AddEvent(9, "spawn_mu");
			AddEvent(10, "message_muapproach");
			Audio.SetMood(-5);
		}
	}

	if (!GetGameStateB("PriComplete"))
	{
		if (hammer_arrived)
		{
			AddEvent(10, "announce_primaryComplete");
			SetGameStateB("PriComplete", true);
			Audio.SetMood(-4);
		}
	}

	
scutz_discovered:
	SetGameStateB("ScutzDiscovered", true);


setmood4:
	Audio.SetMood(4);


setmood0:
	Audio.SetMood(0);

	
win:
	triggerwinlose = true;
	SetGameStateB("GameWon",true);
	Script.Call("messagewin");
	AddEvent(1, "fadeout");
	AddEvent(0.001, "slow");


slow:
	if (Actor.IsAlive(Player.GetActor()))
	{
		float minspd = Actor.GetProperty(Player.GetActor(), "Movement.MinSpeed") - 75 * GetLastFrameTime();
		if (minspd <= 0)
			minspd = 0;
		float spd = Actor.GetProperty(Player.GetActor(), "Movement.Speed") - 200 * GetLastFrameTime();
		Actor.SetProperty(Player.GetActor(), "Movement.MinSpeed", minspd);
		Actor.SetProperty(Player.GetActor(), "Movement.Speed", (minspd > spd) ? minspd : spd);
		AddEvent(0.001, "slow");
	}

	
lose_outpostlost:
	triggerwinlose = true;
	Script.Call("messagelose_outpostlost");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");
	Audio.SetMood(-7);

	
lose_hammerangry:
	triggerwinlose = true;
	Script.Call("messagelose_hammerlost");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");
	Audio.SetMood(-7);


fadeout:
	Scene.FadeOut();
	
	
start:
	Player.SetMovementEnabled(true);


mu_boarding:
	Script.Call("message_muboarding");
	AddEvent(60, "mu_boardingcomplete");
	float3 pos = Actor.GetGlobalPosition(scutz);
	float3 fac = Actor.GetGlobalDirection(scutz);
	float3 pos1 = pos + fac * 1000;
	AI.ForceClearQueue(mu);
	AI.QueueLast(mu, "move", Actor.GetGlobalPosition(scutz), 25, 500, false);
	AI.QueueLast(mu, "rotate", pos1, 0, 1, false);
	AI.QueueLast(mu, "lock");


mu_boardingcomplete:
	if (!triggerwinlose && Actor.IsAlive(scutz))
	{
		SetGameStateB("MuBoardingComplete", true);
		Script.Call("message_muboardingcomplete");
		AI.ForceClearQueue(mu);
		AI.QueueLast(mu, "rotate", {2300, 100, 5000}, 200, 1, false);
		AI.QueueLast(mu, "wait", 10);
		AI.QueueLast(mu, "hyperspaceout");
		AI.QueueLast(mu, "setgamestateb", "MuDispatched", false);
		AI.QueueLast(mu, "setgamestateb", "RebelsCaptured", true);
		AI.QueueLast(mu, "delete");
	}


spawn_ally_reinf:
	for (int i = 0; i < 4; i += 1)
	{	
		_a = Actor.Spawn("TIE", "ETA " + (i + 1), "Empire", "", 0, _d, _d);
		Actor.QueueAtSpawner(_a, outpost);
	}

	Audio.SetMood(-11);
	Script.Call("message_reinf");
	
	
spawn_ally_reinf2:
	for (int i = 0; i < 4; i += 1)
	{	
		_a = Actor.Spawn("TIE", "ZETA " + (i + 1), "Empire", "", 0, _d, _d);
		Actor.QueueAtSpawner(_a, outpost);
	}

	Audio.SetMood(-11);
	Script.Call("message_reinf");


spawn_mu:
	mu = Actor.Spawn("GUN", "MU 1", "Empire_Mu", "", 0, {15000,0,-100000}, {0,0,0});

	Actor.SetProperty(mu, "Movement.MinSpeed", 0);
	Actor.SetProperty(mu, "AI.CanEvade", false);
	Actor.SetProperty(mu, "AI.CanRetaliate", false);
	actorp_id = mu;
	actorp_value = 40;
	Script.Call("actorp_setShd");
	actorp_value = 30;
	Script.Call("actorp_setHull");

	AI.QueueLast(mu, "hyperspacein", { 3600, 500, 7600 });
	AI.QueueLast(mu, "setgamestateb", "MuDispatched", true);
	AI.QueueLast(mu, "attackactor", scutz, 600, 200, false, 30);
	AI.QueueLast(mu, "followactor", scutz, 400, false);
	
	AI.ForceClearQueue(scutz);
	AI.QueueLast(scutz, "move", { -6000, -200, -8000 }, 500, 500, false);
	AI.QueueLast(scutz, "hyperspaceout");
	AI.QueueLast(scutz, "setgamestateb", "ScutzEscaped", true);
	AI.QueueLast(scutz, "delete");


spawn_ywing1:
	spawn_faction = "Mugaari";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "YWING";
	spawn_name = "PETDUR";
	spawn_target = outpost;
	spawn_spacing = 750;
	spawn_pos = { 0, -200, 14500 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn2");
	Actor.SetProperty(spawn_ids[0], "AI.CanEvade", false);
	Actor.SetProperty(spawn_ids[0], "AI.CanRetaliate", false);
	
	Audio.SetMood(-31);
	Script.Call("message_enemyYWING1");
	Script.Call("setmood4");
	combat = true;


spawn_ywing2:
	spawn_faction = "Rebels";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "YWING";
	spawn_name = "GOLD";
	spawn_target = outpost;
	spawn_spacing = 1500;
	spawn_pos = { 0, 200, 19000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn5");
	AddEvent(10, "clear3");
	AddEvent(15, "clear4");

	Audio.SetMood(-21);
	Script.Call("message_enemyYWING2");
	Script.Call("setmood4");


spawn_ywing3:
	spawn_faction = "Mugaari";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "YWING";
	spawn_name = "LAIRE";
	spawn_target = outpost;
	spawn_spacing = 750;
	spawn_pos = { 2200, -400, 19500 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn3");
	Actor.SetProperty(spawn_ids[0], "AI.CanEvade", false);
	Actor.SetProperty(spawn_ids[0], "AI.CanRetaliate", false);
	
	Audio.SetMood(-31);
	Script.Call("message_enemyYWING3");
	Script.Call("setmood4");
	
	
spawn_xwing1:
	spawn_faction = "Mugaari";
	spawn_hyperspace = true;
	spawn_wait = 6;
	spawn_type = (GetDifficulty() == "hard") ? "XWING" : "Z95";
	spawn_name = "DERK";
	spawn_target = outpost;
	spawn_spacing = 500;
	spawn_pos = { 1600, -200, 19500 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn2");
	if (GetDifficulty() == "hard")
	{
		AddEvent(5, "clear0");
		AddEvent(8, "clear1");
	}
	
	Audio.SetMood(-31);
	Script.Call("message_enemy" + spawn_type + "1");
	Script.Call("setmood4");
	
	
spawn_xwing2:
	spawn_faction = "Rebels";
	spawn_hyperspace = true;
	spawn_wait = 2;
	spawn_type = (GetDifficulty() != "easy") ? "XWING" : "Z95";
	spawn_name = "BLUE";
	spawn_target = -1;
	spawn_spacing = 2500;
	spawn_pos = { -5200, 400, 18500 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn3");
	
	Audio.SetMood(-21);
	Script.Call("message_enemy" + spawn_type + "2");
	Script.Call("setmood4");


spawn_xwing3:
	spawn_faction = "Rebels";
	spawn_hyperspace = true;
	spawn_wait = 2;
	spawn_type = "XWING";
	spawn_name = "GRAY";
	spawn_target = -1;
	spawn_spacing = 1500;
	spawn_pos = { -1200, 400, 12500 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn4");
	
	Audio.SetMood(-21);
	Script.Call("message_enemyXWING3");
	Script.Call("setmood4");


spawn_ywing4:
	spawn_faction = "Rebels";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "YWING";
	spawn_name = "RED";
	spawn_target = outpost;
	spawn_spacing = 400;
	spawn_pos = { 4000, 700, 17000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn5");
	AddEvent(3, "clear0");
	AddEvent(3, "clear2");
	Actor.SetProperty(spawn_ids[3], "AI.CanEvade", false);
	Actor.SetProperty(spawn_ids[3], "AI.CanRetaliate", false);
	Actor.SetProperty(spawn_ids[4], "AI.CanEvade", false);
	Actor.SetProperty(spawn_ids[4], "AI.CanRetaliate", false);
	
	Audio.SetMood(-21);
	Script.Call("message_enemyYWING4");
	Script.Call("setmood4");


spawn_scutz:
	spawn_faction = "Neutral_Inspect";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "T4A";
	spawn_name = "SCUTZ 1";
	spawn_target = -1;
	spawn_pos = { 0, -200, 19000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn1");
	Audio.SetMood(-31);
	Script.Call("message_scutz");

	AddEvent(0.5, "spawn_removechildren");
	foreach (int a in spawn_ids)
	{
		Actor.SetProperty(a, "Movement.MinSpeed", 0);
		Actor.SetArmor(a, "MISSILE", 0.1);
		actorp_id = a;
		actorp_value = 10;
		Script.Call("actorp_setShd");
		actorp_value = 40;
		Script.Call("actorp_setHull");
		
		AI.QueueLast(a, "move", { -9000, -200, 3000 }, 250, 500, false);
		AI.QueueLast(a, "move", { -6000, -200, -8000 }, 250, 500, false);
		AI.QueueLast(a, "hyperspaceout");
		AI.QueueLast(a, "setgamestateb", "ScutzEscaped", true);
		AI.QueueLast(a, "delete");
	}
	scutz = spawn_ids[0];
	scutz_arrived = true;


spawn_ubote:
	ubote1 = Actor.Spawn("CORV", "UBOTE 1", "Rebels", "", 0, {10000,0,100000}, {0,0,0}, {"CriticalEnemies"});
	AI.QueueLast(ubote1, "hyperspacein", { -2600, 200, 8600 });
	AI.QueueLast(ubote1, "move", {-2600, 200, 3600}, 100);
	AI.QueueLast(ubote1, "rotate", {-2600, 200, -10000}, 0);
	AI.QueueLast(ubote1, "lock");

	ubote2 = Actor.Spawn("CORV", "UBOTE 2", "Rebels", "", 0, {11000,0,100000}, {0,0,0}, {"CriticalEnemies"});
	AI.QueueLast(ubote2, "hyperspacein", {500, -300, 8600 });
	AI.QueueLast(ubote2, "move", {2500, -300, 2800}, 100);
	AI.QueueLast(ubote2, "rotate", {1500, -300, -10000}, 0);
	AI.QueueLast(ubote2, "lock");
	
	ubote3 = Actor.Spawn("CORV", "UBOTE 3", "Rebels", "", 0, {9000,0,100000}, {0,0,0}, {"CriticalEnemies"});
	AI.QueueLast(ubote3, "hyperspacein", { -5600, -150, 8600 });
	AI.QueueLast(ubote3, "move", {-6100, -150, 2200}, 100);
	AI.QueueLast(ubote3, "rotate", {-1600, -150, -10000}, 0);
	AI.QueueLast(ubote3, "lock");
	
	Audio.SetMood(-22);
	Script.Call("message_enemyUbote");


spawn_hammer:
	hammer_arrived = true;
	hammer = Actor.Spawn("ISD", "HAMMER", "Empire_Hammer", "HAMMER", 0, {10000,0,-100000}, {0,0,0}, {"CriticalAllies"});
	
	Actor.SetProperty(hammer, "Spawner.Enabled", true);
	Actor.SetProperty(hammer, "Spawner.SpawnTypes", {"TIE"});
	AI.QueueLast(hammer, "hyperspacein", { 5600, 700, 4600 });
	AI.QueueLast(hammer, "rotate", {0, 600, 13000}, 0);
	AI.QueueLast(hammer, "lock");
	Audio.SetMood(-12);
	Script.Call("message_hammer");
	AddEvent(0.1, "gethangar");
	AddEvent(4, "message_hammer_2");
	AddEvent(15, "ubote_retreat");


spawn_removechildren:
	foreach (int a in spawn_ids)
	{
		foreach (int chd in Actor.GetChildren(a))
		{
			AI.QueueLast(chd, "delete");
		}
	}


clear0:
	AI.ForceClearQueue(spawn_ids[0]);

clear1:
	AI.ForceClearQueue(spawn_ids[1]);

clear2:
	AI.ForceClearQueue(spawn_ids[2]);

clear3:
	AI.ForceClearQueue(spawn_ids[3]);

clear4:
	AI.ForceClearQueue(spawn_ids[4]);

gethangar:
	int[] list = Actor.GetChildrenByType(hammer, "HANGAR");
	hammer_hangar = list[0];

ubote_retreat:
	AI.ForceClearQueue(ubote1);
	AI.QueueLast(ubote1, "move", { -2600, 200, 10600 }, 500, 500, false);
	AI.QueueLast(ubote1, "hyperspaceout");
	AI.QueueLast(ubote1, "delete");

	AI.ForceClearQueue(ubote2);
	AI.QueueLast(ubote2, "move", { 600, 150, 10600 }, 500, 500, false);
	AI.QueueLast(ubote2, "hyperspaceout");
	AI.QueueLast(ubote2, "delete");

	AI.ForceClearQueue(ubote3);
	AI.QueueLast(ubote3, "move", { -4600, 150, 10600 }, 500, 500, false);
	AI.QueueLast(ubote3, "hyperspaceout");
	AI.QueueLast(ubote3, "delete");

	
announce_primaryComplete:
	AddEvent(3, "message_primaryobj_completed");
	AddEvent(7, "message_returntobase");
	AddEvent(7.1, "setComplete");


setComplete:
	primary_completed = true;
