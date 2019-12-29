// Globals

bool respawn;
bool triggerwinlose;
bool primary_completed;
bool glich_arrived;

int outpost;
int tie_delta1;
int tie_gamma1;

int sigma;
int glich;

int onece1;
int onece2;
int onece3;
int onece4;
int onece5;

int dayta1;
int dayta2;

int yander1;
int yander2;
int yander3;

int taloos1;
int taloos2;

int insp_onece;
int insp_dayta;
int insp_yander;
int insp_taloos;

int[] ravtin_group;
int[] tough_group;
int[] stress_group;

float3 faction_empire_color = { 0, 0.8, 0 };
float3 faction_empire_trans_color = { 0.4, 0.9, 0.5 };
float3 faction_neutral_color = { 0.7, 0.7, 0.7 };
float3 faction_neutral_rebel_color = { 0.8, 0.3, 0.3 };
float3 faction_rebel_color = { 0.8, 0, 0 };

float3 _d = { 0, 0, 0 };
int _a = -1;
float _f = 0;

load:
	Scene.SetMaxBounds({10000, 1500, 10000});
	Scene.SetMinBounds({-10000, -1500, -10000});
	Scene.SetMaxAIBounds({10000, 1500, 10000});
	Scene.SetMinAIBounds({-10000, -1500, -10000});

	Player.SetLives(5);
	Score.SetScorePerLife(1000000);
	Score.SetScoreForNextLife(1000000);
	Score.Reset();

	UI.SetLine1Color(faction_empire_color);
	UI.SetLine2Color(faction_neutral_color);
	UI.SetLine3Color(faction_rebel_color);
	
	Script.Call("spawn_reset");
	Script.Call("actorp_reset");
	
	Script.Call("engagemusic");	
	Script.Call("make_ships");
	Script.Call("make_fighters");
	Script.Call("makeplayer");
	AddEvent(0.1, "setup_outpost");
	AddEvent(1.5, "start");
	
	AddEvent(8, "spawn_onece");
	AddEvent(11, "spawn_dayta");


loadfaction:
	Faction.Add("Empire", faction_empire_color);
	Faction.Add("Empire_Trans", faction_empire_color);
	Faction.Add("Neutral_Inspect", faction_neutral_color);
	Faction.Add("Neutral_OK", faction_empire_trans_color);
	Faction.Add("Neutral_Rebel", faction_neutral_rebel_color);
	Faction.Add("Rebels", faction_rebel_color);
	Faction.MakeAlly("Empire", "Neutral_Inspect");
	Faction.MakeAlly("Empire", "Neutral_OK");
	Faction.MakeAlly("Empire", "Neutral_Rebel");
	Faction.MakeAlly("Empire", "Empire_Trans");
	Faction.MakeAlly("Rebels", "Neutral_Inspect");
	Faction.MakeAlly("Rebels", "Neutral_OK");
	Faction.MakeAlly("Rebels", "Neutral_Rebel");
	Faction.MakeAlly("Rebels", "Empire_Trans");
	Faction.MakeAlly("Empire_Trans", "Neutral_Inspect");
	Faction.MakeAlly("Empire_Trans", "Neutral_OK");
	Faction.MakeAlly("Neutral_Rebel", "Neutral_Inspect");
	Faction.MakeAlly("Neutral_Rebel", "Neutral_OK");
	Faction.MakeAlly("Empire_Trans", "Neutral_Rebel");

	Faction.SetWingSpawnLimit("Empire", 0);

	
loadscene:

	
engagemusic:
	Audio.SetMood(1);
    Audio.SetMusicDyn("TRO-IN");

	
makeplayer:
	Player.DecreaseLives();
	if (respawn) 
		Player.RequestSpawn(); 
	else 
		Script.Call("firstspawn");


firstspawn:
	Player.AssignActor(Actor.Spawn(GetPlayerActorType(), GetPlayerName(), "Empire", "", 0, { 0, 0, -2800 }, { 0, 0, 0 }));
	Player.SetMovementEnabled(false);
	Actor.SetProperty(Player.GetActor(), "Movement.Speed", 30);
	respawn = true;

	
make_ships:

	// Empire
	
	outpost = Actor.Spawn("XQ1", "OUTPOST D-34", "Empire", "OUTP D-34", 0, { 0, 0, 0 }, { 0, -20, 0 }, { "CriticalAllies" });
	Actor.SetProperty(outpost, "Spawner.Enabled", true);
	Actor.SetProperty(outpost, "Spawner.SpawnTypes", {"TIE"});
	Actor.SetProperty(outpost, "AI.HuntWeight", 0);


	AI.QueueLast(outpost, "lock");

	
make_fighters:

	tie_delta1 = Actor.Spawn("TIE", "DELTA 1", "Empire", "", 0, { 2500, -300, 1500 }, { 0, -120, 0 });
	actorp_id = tie_delta1;
	actorp_multiplier = 2;
	Script.Call("actorp_multHull");
	for (int i = 1; i <= 20; i += 1)
	{
		AI.QueueLast(tie_delta1, "move", {4500, -250, 6500}, 35);
		AI.QueueLast(tie_delta1, "move", {-6500, -200, 4500}, 35);
		AI.QueueLast(tie_delta1, "move", {4500, -150, -6500}, 35);
		AI.QueueLast(tie_delta1, "move", {6500, -200, -4500}, 35);
	}
	AI.QueueLast(tie_delta1, "lock");

	tie_gamma1 = Actor.Spawn("TIE", "GAMMA 1", "Empire", "", 0, { -3000, 200, -1350 }, { 0, 120, 0 });
	actorp_id = tie_gamma1;
	actorp_multiplier = 2;
	Script.Call("actorp_multHull");
	for (int i = 1; i <= 20; i += 1)
	{
		AI.QueueLast(tie_gamma1, "move", {5350, 250, 7000}, 35);
		AI.QueueLast(tie_gamma1, "move", {-7000, 200, 5350}, 35);
		AI.QueueLast(tie_gamma1, "move", {5350, 150, -7000}, 35);
		AI.QueueLast(tie_gamma1, "move", {7000, 200, -5350}, 35);
	}
	AI.QueueLast(tie_gamma1, "lock");


setup_outpost:
	int i = 0;
	foreach (int chd in Actor.GetChildren(outpost))
	{
		Actor.SetProperty(chd, "AI.HuntWeight", 1);
		if (GetDifficulty() == "hard" && i >= 3)
		{
			AI.QueueLast(chd, "delete");
		}
		i += 1;
	}


gametick:
	UI.SetLine1Text("WINGS: " + Faction.GetWingCount("Empire"));
	
	int neut = Faction.GetWingCount("Neutral_Inspect");
	int rebel = Faction.GetWingCount("Rebels");

	if (neut == 0)
	{
		UI.SetLine2Color(faction_rebel_color);
		UI.SetLine2Text((rebel == 0) ? "" : "ENEMY: " + rebel);
		UI.SetLine3Text("");
	}
	else
	{
		UI.SetLine2Color(faction_neutral_color);
		UI.SetLine3Color(faction_rebel_color);
		UI.SetLine2Text("INSPECT: " + neut);
		UI.SetLine3Text((rebel == 0) ? "" : "ENEMY: " + rebel);
	}
	
	if (!triggerwinlose)
	{
		if (!GetGameStateB("OutpostDestroyed"))
		{
			float hp = Actor.GetHP(outpost);
			if (hp <= 0) 
			{
				SetGameStateB("OutpostDestroyed", true);
				Script.Call("lose_outpostlost");
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
		
		if (GetGameStateB("Onece3Discovered"))
		{
			if (!GetGameStateB("Onece3Disabled"))
			{
				float shd = Actor.GetShd(onece3);
				if (shd <= 0) 
				{
					float3 pos = Actor.GetGlobalPosition(onece3);
					float3 fac = Actor.GetGlobalDirection(onece3);
					pos += fac * 5000;
					AI.ForceClearQueue(onece3);
					AI.QueueLast(onece3, "rotate", pos, 0, 1, false);
					AI.QueueLast(onece3, "lock");
					Actor.RemoveFromRegister(onece3, "CriticalEnemies");
					Actor.AddToRegister(onece3, "CriticalAllies");
					Actor.SetFaction(onece3, "Neutral_OK");
					Actor.SetProperty(onece3, "Regen.Self", 0);
					Script.Call("message_onece3_disabled");
					SetGameStateB("Onece3Disabled", true);
				}
			}
		}
		
		if (GetGameStateB("SigmaDispatched"))
		{
			float hp = Actor.GetHP(sigma);
			if (hp <= 0) 
			{
				Script.Call("lose_sigmalost");
			}
			
			if (!GetGameStateB("SigmaDanger"))
			{
				float shd = Actor.GetShd(sigma);
				if (shd <= 0) 
					{
					SetGameStateB("SigmaDanger", true);
					for (float time = 0; time < 3; time += 0.4)
					{
						AddEvent(time, "messagewarn_sigma_o");
						AddEvent(time + 0.2, "messagewarn_sigma_y");
					}
					Script.Call("respite");
				}
			}
			
			if (!GetGameStateB("SigmaBoarding"))
			{
				if (Math.GetActorDistance(sigma, onece3) < 500)
				{
					SetGameStateB("SigmaBoarding", true);
					Script.Call("sigma_boarding");
				}
			}
		}
		
		if (glich_arrived && !GetGameStateB("GlichDestroyed") && !GetGameStateB("GlichEscaped"))
		{
			float hp = Actor.GetHP(glich);
			if (hp <= 0) 
			{
				SetGameStateB("GlichDestroyed", true);
				Script.Call("message_glich_destroyed");
			}
		}
		
		if (GetGameStateB("Onece3Escaped"))
		{
			Script.Call("lose_onece3escaped");
		}
		
		if (GetGameStateB("OneceArrived") && !GetGameStateB("RebelsCaptured"))
		{
			float hp = Actor.GetHP(onece3);
			if (hp <= 0) 
			{
				Script.Call("lose_onece3lost");
			}
			
			if (!GetGameStateB("Onece3Danger"))
			{
				if (hp <= 6) 
					{
					SetGameStateB("Onece3Danger", true);
					for (float time = 0; time < 3; time += 0.4)
					{
						AddEvent(time, "messagewarn_onece3_o");
						AddEvent(time + 0.2, "messagewarn_onece3_y");
					}
				}
			}
		}
		
		if (GetGameStateB("RebelsCaptured") && !GetGameStateB("RebelsCaptureAnnounced"))
		{
			SetGameStateB("RebelsCaptureAnnounced", true);
			Script.Call("message_rebelscaptured");
		}
		
		if (primary_completed)
		{
			if (Math.GetActorDistance(Player.GetActor(), outpost) < 300)
			{
				Script.Call("win");
			}
		}

		Script.Call("inspection");
		Script.Call("avoidance");
	}


inspection:
	if (!GetGameStateB("Onece1Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), onece1) < 200)
		{
			Actor.SetFaction(onece1, "Neutral_OK");
			SetGameStateB("Onece1Inspected", true);
			Script.Call("message_inspected_onece1");
			insp_onece += 1;
		}
	}
	
	if (!GetGameStateB("Onece2Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), onece2) < 200)
		{
			Actor.SetFaction(onece2, "Neutral_OK");
			SetGameStateB("Onece2Inspected", true);
			Script.Call("message_inspected_onece2");
			insp_onece += 1;
		}
	}

	if (!GetGameStateB("Onece3Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), onece3) < 200)
		{
			SetGameStateB("Onece3Inspected", true);
			Script.Call("message_inspected_onece3");
			insp_onece += 1;
			
			Squad.RemoveFromSquad(onece3);
			Actor.AddToRegister(onece3, "CriticalEnemies");
			Actor.SetFaction(onece3, "Neutral_Rebel");
			AddEvent(0.2, "onece3_discovered");
			AddEvent(9, "spawn_onece3_trans");
			Audio.SetMood(-4);
		}
	}
	
	if (!GetGameStateB("Onece4Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), onece4) < 200)
		{
			Actor.SetFaction(onece4, "Neutral_OK");
			SetGameStateB("Onece4Inspected", true);
			Script.Call("message_inspected_onece4");
			insp_onece += 1;
		}
	}
	
	if (!GetGameStateB("Onece5Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), onece5) < 200)
		{
			Actor.SetFaction(onece5, "Neutral_OK");
			SetGameStateB("Onece5Inspected", true);
			Script.Call("message_inspected_onece5");
			insp_onece += 1;
		}
	}
	
	if (!GetGameStateB("Dayta1Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), dayta1) < 200)
		{
			Actor.SetFaction(dayta1, "Neutral_OK");
			SetGameStateB("Dayta1Inspected", true);
			Script.Call("message_inspected_dayta1");
			insp_dayta += 1;
		}
	}
	
	if (!GetGameStateB("Dayta2Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), dayta2) < 200)
		{
			Actor.SetFaction(dayta2, "Neutral_OK");
			SetGameStateB("Dayta2Inspected", true);
			Script.Call("message_inspected_dayta2");
			insp_dayta += 1;
		}
	}

	if (!GetGameStateB("Yander1Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), yander1) < 200)
		{
			Actor.SetFaction(yander1, "Neutral_OK");
			SetGameStateB("Yander1Inspected", true);
			Script.Call("message_inspected_yander1");
			insp_yander += 1;
		}
	}
	
	if (!GetGameStateB("Yander2Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), yander2) < 200)
		{
			Actor.SetFaction(yander2, "Neutral_OK");
			SetGameStateB("Yander2Inspected", true);
			Script.Call("message_inspected_yander2");
			insp_yander += 1;
		}
	}
	
	if (!GetGameStateB("Yander3Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), yander3) < 200)
		{
			Actor.SetFaction(yander3, "Neutral_OK");
			SetGameStateB("Yander3Inspected", true);
			Script.Call("message_inspected_yander3");
			insp_yander += 1;
		}
	}
	
	if (!GetGameStateB("Taloos1Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), taloos1) < 400)
		{
			Actor.SetFaction(taloos1, "Neutral_OK");
			SetGameStateB("Taloos1Inspected", true);
			Script.Call("message_inspected_taloos1");
			insp_taloos += 1;
		}
	}
	
	if (!GetGameStateB("Taloos2Inspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), taloos2) < 400)
		{
			Actor.SetFaction(taloos2, "Neutral_OK");
			SetGameStateB("Taloos2Inspected", true);
			Script.Call("message_inspected_taloos2");
			insp_taloos += 1;
		}
	}
	
	if (!GetGameStateB("GlichInspected"))
	{
		if (Math.GetActorDistance(Player.GetActor(), glich) < 200)
		{
			SetGameStateB("GlichInspected", true);
			Script.Call("message_inspected_glich");
		}
	}
	
	if (!GetGameStateB("AllOneceInspected"))
	{
		if (insp_onece >= 5)
		{
			SetGameStateB("AllOneceInspected", true);
			AddEvent(3, "message_inspected_onece100%");
		}
	}
	
	if (!GetGameStateB("AllDaytaInspected"))
	{
		if (insp_taloos >= 2)
		{
			SetGameStateB("AllDaytaInspected", true);
			AddEvent(3, "message_inspected_dayta100%");
		}
	}
	
	if (!GetGameStateB("AllYanderInspected"))
	{
		if (insp_yander >= 3)
		{
			SetGameStateB("AllYanderInspected", true);
			AddEvent(3, "message_inspected_yander100%");
		}
	}

	if (!GetGameStateB("AllTaloosInspected"))
	{
		if (insp_taloos >= 2)
		{
			SetGameStateB("AllTaloosInspected", true);
			AddEvent(3, "message_inspected_taloos100%");
		}
	}
	
	if (!GetGameStateB("AllInspected"))
	{
		if (GetGameStateB("AllOneceInspected") && GetGameStateB("AllDaytaInspected") && GetGameStateB("AllYanderInspected") && GetGameStateB("AllTaloosInspected"))
		{
			SetGameStateB("AllInspected", true);
			AddEvent(7, "message_inspected_all");
		}
	}

	if (!GetGameStateB("SecondaryObsComplete"))
	{
		if (GetGameStateB("AllInspected"))
		{
			SetGameStateB("SecondaryObsComplete", true);
			Audio.SetMood(-5);
		}
	}

	if (!GetGameStateB("AllComplete"))
	{
		if (GetGameStateB("AllInspected") && GetGameStateB("GlichDestroyed") && glich_arrived && Faction.GetWingCount("Rebels") == 0)
		{
			Script.Call("announce_allComplete");
			SetGameStateB("AllComplete", true);
			Audio.SetMood(-6);
		}
	}
	
	if (!GetGameStateB("PriComplete") && !(GetGameStateB("AllComplete")))
	{
		if (GetGameStateB("RebelsCaptured") && glich_arrived && Faction.GetWingCount("Rebels") == 0)
		{
			Script.Call("announce_primaryComplete");
			SetGameStateB("PriComplete", true);
			Audio.SetMood(-4);
		}
	}


onece3_discovered:
	SetGameStateB("Onece3Discovered", true);


avoidance:
	bool combat = false;
	bool dhard = GetDifficulty() == "hard";
	if (!IsNull(ravtin_group))
	{
		foreach (int a in ravtin_group)
		{
			if (GetGameStateF("t" + a) < GetGameTime() + 10)
			{
				_a = a;
				_f = dhard ? 4000 : 4500;
				_d = { Random(-2000, 2000), Random(-600, 600), 7000 + Random(-1000, 1000) };
				Script.Call("avoidance_logic");
			}
			
			if (Actor.IsAlive(a))
				combat = true;
		}
	}

	if (!IsNull(tough_group))
	{
		foreach (int a in tough_group)
		{
			if (GetGameStateF("t" + a) < GetGameTime() + 10)
			{
				_a = a;
				_f = dhard ? 3500 :4500;
				_d = { Random(-2000, 2000), Random(-600, 600), 7000 + Random(-1000, 1000) };
				Script.Call("avoidance_logic");
			}

			if (Actor.IsAlive(a))
				combat = true;
		}
	}

	if (!IsNull(stress_group))
	{
		foreach (int a in stress_group)
		{
			if (GetGameStateF("t" + a) < GetGameTime() + 10)
			{
				_a = a;
				_f = dhard ? 3500 :5000;
				_d = { Random(-2000, 2000), Random(-600, 600), 7000 + Random(-1000, 2000) };
				Script.Call("avoidance_logic");
			}

			if (Actor.IsAlive(a))
				combat = true;
		}
	}
	
	if (!IsNull(ravtin_group))
		if (combat && Audio.GetMood() != 4)
			Audio.SetMood(4);
		else if (!combat && Audio.GetMood() != 0 && Audio.GetMood() != 5)
			Audio.SetMood(0);


avoidance_logic:
	float dist = Math.GetActorDistance(_a, outpost);
	if (dist < _f)
	{
		SetGameStateF("t" + _a, GetGameTime());
		AI.ForceClearQueue(_a);
		Actor.SetProperty(_a, "AI.CanEvade", false);
		Actor.SetProperty(_a, "AI.CanRetaliate", false);
		AI.QueueLast(_a, "move", _d, 100, 200, false);
	}
	else if (dist > _f + 500)
	{
		Actor.SetProperty(_a, "AI.CanEvade", true);
		Actor.SetProperty(_a, "AI.CanRetaliate", true);
	}

	
respite:
	float tm = 5;
	if (GetDifficulty() == "hard")
		tm = 2.5;
	
	if (!IsNull(ravtin_group))
	{
		foreach (int a in ravtin_group)
		{
			foreach (int chd in Actor.GetChildren(a))
			{
				AI.ForceClearQueue(a);
				AI.QueueLast(a, "attackactor", outpost, 10, 10, true, 5);
			}
		}
	}
	
	if (!IsNull(tough_group))
	{
		foreach (int a in tough_group)
		{
			foreach (int chd in Actor.GetChildren(a))
			{
				AI.ForceClearQueue(a);
				AI.QueueLast(a, "attackactor", outpost, 10, 10, true, 5);
			}
		}
	}
	
	if (!IsNull(stress_group))
	{
		foreach (int a in stress_group)
		{
			foreach (int chd in Actor.GetChildren(a))
			{
				AI.ForceClearQueue(a);
				AI.QueueLast(a, "attackactor", outpost, 10, 10, true, 5);
			}
		}
	}


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


lose_onece3lost:
	triggerwinlose = true;
	Script.Call("messagelose_onece3lost");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");
	Audio.SetMood(-7);


lose_onece3escaped:
	triggerwinlose = true;
	Script.Call("messagelose_onece3escaped");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");
	Audio.SetMood(-7);


lose_sigmalost:
	triggerwinlose = true;
	Script.Call("messagelose_sigmalost");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");
	Audio.SetMood(-7);


fadeout:
	Scene.FadeOut();
	
	
start:
	Player.SetMovementEnabled(true);


prewake_fighters:
	AI.ForceClearQueue(tie_delta1);
	AI.ForceClearQueue(tie_gamma1);
	float3 pos = Actor.GetGlobalPosition(onece3);
	AI.QueueLast(tie_delta1, "move", pos + {0, -250, 300}, 50);
	for (int i = 1; i <= 5; i += 1)
	{
		AI.QueueLast(tie_delta1, "move", pos + {0, -250, -300}, 10);
		AI.QueueLast(tie_delta1, "move", pos + {0, -250, 300}, 10);
	}
	AI.QueueLast(tie_delta1, "lock");

	AI.QueueLast(tie_gamma1, "move", pos + {0, 250, 300}, 50);
	for (int i = 1; i <= 5; i += 1)
	{
		AI.QueueLast(tie_gamma1, "move", pos + {0, 250, -300}, 10);
		AI.QueueLast(tie_gamma1, "move", pos + {0, 250, 300}, 10);
	}
	AI.QueueLast(tie_gamma1, "lock");


wake_fighters:
	AI.ForceClearQueue(tie_delta1);
	AI.ForceClearQueue(tie_gamma1);


sigma_boarding:
	Script.Call("message_sigmaboarding");
	AddEvent(60, "sigma_boardingcomplete");
	float3 pos = Actor.GetGlobalPosition(onece3);
	float3 fac = Actor.GetGlobalDirection(onece3);
	float3 pos1 = pos + fac * 1000;
	AI.ForceClearQueue(sigma);
	AI.QueueLast(sigma, "move", Actor.GetGlobalPosition(onece3), 5, 200, false);
	AI.QueueLast(sigma, "rotate", pos1, 0, 1, false);
	AI.QueueLast(sigma, "lock");


sigma_boardingcomplete:
	if (!triggerwinlose)
	{
		SetGameStateB("SigmaBoardingComplete", true);
		Script.Call("message_sigmaboardingcomplete");
		float3 pos = Actor.GetGlobalPosition(outpost);
		float3 fac = Actor.GetGlobalDirection(outpost);
		float3 pos1 = pos + fac * 2000 + {40, 25, 0};
		float3 pos2 = pos + fac * 200 + {40, 25, 0};
		AI.ForceClearQueue(sigma);
		AI.QueueLast(sigma, "move", pos1, 100, 50, false);
		AI.QueueLast(sigma, "rotate", pos2, 0, 1, false);
		AI.QueueLast(sigma, "move", pos2, 50, 10, false);
		AI.QueueLast(sigma, "setgamestateb", "SigmaDispatched", false);
		AI.QueueLast(sigma, "wait", 1);
		AI.QueueLast(sigma, "delete");
		
		float3 poso1 = pos + fac * 2000 + {-65, 25, 0};
		float3 poso2 = pos + fac * 200 + {-65, 25, 0};
		AI.ForceClearQueue(onece3);
		AI.QueueLast(onece3, "move", poso1, 100, 40, false);
		AI.QueueLast(onece3, "rotate", poso2, 0, 1, false);
		AI.QueueLast(onece3, "move", poso2, 50, 8, false);
		AI.QueueLast(onece3, "setgamestateb", "RebelsCaptured", true);
		AI.QueueLast(onece3, "wait", 1);
		AI.QueueLast(onece3, "delete");
	}


spawn_ally_support:
	_a = Actor.Spawn("TIE", "ALPHA 2", "Empire", "", 0, _d, _d);
	Actor.QueueAtSpawner(_a, outpost);

	_a = Actor.Spawn("TIE", "ALPHA 3", "Empire", "", 0, _d, _d);
	Actor.QueueAtSpawner(_a, outpost);


spawn_ally_reinf:
	_a = Actor.Spawn("TIE", "DETLA 2", "Empire", "", 0, _d, _d);
	Actor.QueueAtSpawner(_a, outpost);

	_a = Actor.Spawn("TIE", "DETLA 3", "Empire", "", 0, _d, _d);
	Actor.QueueAtSpawner(_a, outpost);
	
	_a = Actor.Spawn("TIE", "GAMMA 2", "Empire", "", 0, _d, _d);
	Actor.QueueAtSpawner(_a, outpost);
	
	_a = Actor.Spawn("TIE", "GAMMA 3", "Empire", "", 0, _d, _d);
	Actor.QueueAtSpawner(_a, outpost);

	Audio.SetMood(-11);
	Script.Call("message_reinf");


spawn_onece3_trans:
	sigma = Actor.Spawn("JV7", "SIGMA 1", "Empire_Trans", "", 0, _d, _d);
	Actor.QueueAtSpawner(sigma, outpost);
	Actor.SetProperty(sigma, "Movement.MinSpeed", 0);
	Actor.SetProperty(sigma, "AI.CanEvade", false);
	Actor.SetProperty(sigma, "AI.CanRetaliate", false);
	Actor.AddToRegister(sigma, "CriticalAllies");
	actorp_id = sigma;
	actorp_value = 40;
	Script.Call("actorp_setShd");
	actorp_value = 30;
	Script.Call("actorp_setHull");
	float3 pos = Actor.GetGlobalPosition(outpost);
	float3 fac = Actor.GetGlobalDirection(outpost);
	float3 pos1 = pos + fac * 1000 + {0, 25, 0};
	AI.QueueLast(sigma, "setgamestateb", "SigmaDispatched", true);
	AI.QueueLast(sigma, "move", pos1, 100, 10, false);
	AI.QueueLast(sigma, "attackactor", onece3, 600, 200, false, 30);
	AI.QueueLast(sigma, "followactor", onece3, 400, false);
	
	AI.ForceClearQueue(onece3);
	AI.QueueLast(onece3, "move", { 6000, 600, -8000 }, 75, 100, false);
	AI.QueueLast(onece3, "hyperspaceout");
	AI.QueueLast(onece3, "setgamestateb", "Onece3Escaped", true);
	AI.QueueLast(onece3, "delete");
	

	AddEvent(4, "message_onece3_2");
	AddEvent(7, "message_onece3_3");
	//AddEvent(10, "announce_primaryComplete");
	AddEvent(11, "unally");
	AddEvent(5, "setmood0");
	AddEvent(5, "prewake_fighters");
	AddEvent(20, "spawn_enemySHU");
	
	AddEvent(44, "spawn_yander");
	AddEvent(48, "spawn_taloos");
	AddEvent(52, "message_group2");
	AddEvent(50, "unally2");

	AddEvent(91, "spawn_enemyES");
	AddEvent(110, "spawn_glich");
	
	if (GetDifficulty() != "easy")
		AddEvent(97, "spawn_enemyES2");

	
unally:
	Faction.MakeEnemy("Empire_Trans", "Neutral_Rebel");

	
unally2:
	Faction.MakeEnemy("Empire_Trans", "Rebels");


spawn_onece:
	spawn_faction = "Neutral_Inspect";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "CARGO_LG";
	spawn_name = "ONECE";
	spawn_target = -1;
	spawn_spacing = 350;
	spawn_pos = { 0, 600, 8000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn5");
	Audio.SetMood(-31);
	Script.Call("message_onece");
	AddEvent(7, "message_group1");
	AddEvent(10, "message_group1_2");
	AddEvent(14, "message_group1_3");

	onece1 = spawn_ids[0];
	onece2 = spawn_ids[1];
	onece3 = spawn_ids[2];
	onece4 = spawn_ids[3];
	onece5 = spawn_ids[4];

	AddEvent(0.5, "spawn_removechildren");
	foreach (int a in spawn_ids)
	{
		Actor.SetProperty(a, "Movement.MinSpeed", 0);
		Actor.SetProperty(a, "AI.CanEvade", false);
		Actor.SetProperty(a, "AI.CanRetaliate", false);
		AI.QueueLast(a, "move", { 6000, 600, -8000 }, 75, 500, false);
		AI.QueueLast(a, "hyperspaceout");
		if (a == onece3)
			AI.QueueLast(a, "setgamestateb", "Onece3Escaped", true);

		AI.QueueLast(a, "delete");
	}
	SetGameStateB("OneceArrived", true);
	Script.Call("setmood4");
	
	actorp_id = onece3;
	actorp_value = 20;
	Script.Call("actorp_setHull");

	
spawn_dayta:
	spawn_faction = "Neutral_Inspect";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "CARGO_SM";
	spawn_name = "DAYTA";
	spawn_target = -1;
	spawn_spacing = 600;
	spawn_pos = { 0, 400, 9000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn2");
	//Audio.SetMood(-31);
	Script.Call("message_dayta");
	
	AddEvent(0.5, "spawn_removechildren");
	foreach (int a in spawn_ids)
	{
		Actor.SetProperty(a, "Movement.MinSpeed", 0);
		Actor.SetProperty(a, "AI.CanEvade", false);
		Actor.SetProperty(a, "AI.CanRetaliate", false);
		AI.QueueLast(a, "move", { 6000, 400, -8000 }, 75, 500, false);
		AI.QueueLast(a, "hyperspaceout");
		AI.QueueLast(a, "delete");
	}	
	dayta1 = spawn_ids[0];
	dayta2 = spawn_ids[1];


spawn_yander:
	spawn_faction = "Neutral_Inspect";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "YT1300";
	spawn_name = "YANDER";
	spawn_target = -1;
	spawn_spacing = 600;
	spawn_pos = { 0, 500, 8000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn3");
	Audio.SetMood(-31);
	Script.Call("message_yander");
	AddEvent(10, "message_group2");

	AddEvent(0.5, "spawn_removechildren");
	foreach (int a in spawn_ids)
	{
		Actor.SetProperty(a, "Movement.MinSpeed", 0);
		Actor.SetProperty(a, "AI.CanEvade", false);
		Actor.SetProperty(a, "AI.CanRetaliate", false);
		AI.QueueLast(a, "move", { 6000, 600, -8000 }, 75, 500, false);
		AI.QueueLast(a, "hyperspaceout");
		AI.QueueLast(a, "delete");
	}	
	yander1 = spawn_ids[0];
	yander2 = spawn_ids[1];
	yander3 = spawn_ids[2];


spawn_taloos:
	spawn_faction = "Neutral_Inspect";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "TRAN";
	spawn_name = "TALOOS";
	spawn_target = -1;
	spawn_spacing = 1200;
	spawn_pos = { 0, 200, 8000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn2");
	//Audio.SetMood(-31);
	Script.Call("message_taloos");
	
	AddEvent(0.5, "spawn_removechildren");
	foreach (int a in spawn_ids)
	{
		Actor.SetProperty(a, "Movement.MinSpeed", 0);
		Actor.SetProperty(a, "AI.CanEvade", false);
		Actor.SetProperty(a, "AI.CanRetaliate", false);
		AI.QueueLast(a, "move", { 6000, 200, -8000 }, 75, 500, false);
		AI.QueueLast(a, "hyperspaceout");
		AI.QueueLast(a, "delete");
	}	
	taloos1 = spawn_ids[0];
	taloos2 = spawn_ids[1];


spawn_glich:
	spawn_faction = "Neutral_Rebel";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "CARGO_LG";
	spawn_name = "GLICH 1";
	spawn_target = -1;
	spawn_pos = { 0, -200, 9000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn1");
	//Audio.SetMood(-21);
	Script.Call("message_glich");
	AddEvent(4, "message_glich_2");

	if (GetDifficulty() == "easy")
		AddEvent(0.5, "spawn_removechildren");
	foreach (int a in spawn_ids)
	{
		Actor.SetProperty(a, "Movement.MinSpeed", 0);
		AI.QueueLast(a, "move", { 9000, -200, 3000 }, 75, 500, false);
		AI.QueueLast(a, "move", { 6000, -200, -8000 }, 75, 500, false);
		AI.QueueLast(a, "hyperspaceout");
		AI.QueueLast(a, "setgamestateb", "GlichEscaped", true);
		AI.QueueLast(a, "delete");
	}
	glich = spawn_ids[0];
	glich_arrived = true;


spawn_enemySHU:
	spawn_faction = "Rebels";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "T4A";
	spawn_name = "RAVTIN";
	spawn_target = -1;
	spawn_spacing = 500;
	spawn_pos = { 0, 0, 9000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn3");
	Audio.SetMood(-21);
	Script.Call("message_enemySHU");
	AddEvent(1, "spawn_ally_support");
	AddEvent(2, "wake_fighters");
	AddEvent(11, "message_rebels");
	AddEvent(15, "message_rebels_2");
	AddEvent(19, "message_protect");
	Script.Call("setmood4");
	ravtin_group = spawn_ids;


spawn_enemyES:
	spawn_faction = "Rebels";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = (GetDifficulty() == "hard") ? "JV7B" : "JV7";
	spawn_name = "TOUGH";
	spawn_target = outpost;
	spawn_spacing = 750;
	spawn_pos = { 0, 0, 9500 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn3");
	Audio.SetMood(-21);
	Script.Call("message_enemyES");
	AddEvent(10, "spawn_ally_reinf");
	Script.Call("setmood4");
	tough_group = spawn_ids;


spawn_enemyES2:
	spawn_faction = "Rebels";
	spawn_hyperspace = true;
	spawn_wait = 3;
	spawn_type = "JV7B";
	spawn_name = "STRESS";
	spawn_target = outpost;
	spawn_spacing = 750;
	spawn_pos = { 0, -200, 9800 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn3");
	Audio.SetMood(-21);
	stress_group = spawn_ids;


spawn_removechildren:
	foreach (int a in spawn_ids)
	{
		foreach (int chd in Actor.GetChildren(a))
		{
			AI.QueueLast(chd, "delete");
		}
	}


announce_primaryComplete:
	AddEvent(3, "message_primaryobj_completed");
	AddEvent(7, "message_returntobase");
	AddEvent(7.1, "setComplete");

	
announce_allComplete:
	primary_completed = true;
	AddEvent(3, "message_secondaryobj_completed");
	AddEvent(7, "message_returntobase");
	AddEvent(7.1, "setComplete");


setComplete:
	primary_completed = true;
