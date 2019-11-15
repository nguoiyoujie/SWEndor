float starttime = 3;
bool triggerwinlose = false;
bool started = false;
float time = 34;
float interval = 35;
float time2 = 70;
float interval2 = 75;

load:
	Scene.SetMaxBounds({ 10000, 1500, 8000 });
	Scene.SetMinBounds({ -10000, -1500, -8000 });
	Scene.SetMaxAIBounds({ 10000, 1500, 8000 });
	Scene.SetMinAIBounds({ -10000, -1500, -8000 });
	
	Audio.SetMusic("battle_2_1");
	Audio.SetMusicLoop("battle_2_2");
	
	Player.SetLives(3);
	Player.SetScorePerLife(1000000);
	Player.SetScoreForNextLife(1000000);
	Player.ResetScore();
	
	UI.SetLine1Color({ 0.6, 0.6, 0 });
	UI.SetLine2Color({ 0.6, 0.6, 0 });

	Script.CallX("spawnreset");	
	Script.CallX("makesmugglers");
	Script.CallX("makeplayer");
	AddEvent(7, "stage1");
	AddEvent(12, "message01");
	AddEvent(17, "message02");
	AddEvent(22, "message03");
	AddEvent(28, "message04");
	AddEvent(35, "message05");
	AddEvent(154, "message06");
	AddEvent(157, "stage2");

loadfaction:
	Faction.Add("Smugglers", { 0.6, 0.6, 0 });
	Faction.Add("Empire", { 0, 0.8, 0 });
	Faction.Add("Rebels", { 0.8, 0, 0 });
	Faction.SetWingSpawnLimit("Empire", 24);
	Faction.SetWingSpawnLimit("Rebels", 10);

loadscene:
	int planet = Actor.Spawn("HOTH", "", "", "", 0, { 0, -20000, 0 }, { 0, 180, 0 });
	Actor.SetProperty(planet, "Scale", 10);
	
	int impSD1 = Actor.Spawn("IMPL", "", "Empire", "", 0, { 0, 500, -15000 }, { 0, 25, 0 });
	AI.QueueLast(impSD1, "move", { 0, 500, -12000 }, 2);
	AI.QueueLast(impSD1, "rotate", { 0, 500, 12000 }, 0);
	AI.QueueLast(impSD1, "lock");

makeplayer:
	Player.DecreaseLives();
	Player.RequestSpawn();
	AddEvent(5, "setupplayer");

setupplayer:
	Actor.SetProperty(Player.GetActor(), "DamageModifier", 0.25);

makesmugglers:
	int pspawner = Actor.Spawn("SPAWN", "", "Smugglers", "", 0, { 9500, -900, 0 }, { -15, -90, 0 });
	Actor.SetProperty(pspawner, "Spawner.Enabled", true);

	int falcon1 = Actor.Spawn("FALC", "YT-1300", "Smugglers", "", 0, { 8600, -1020, -500 }, { -5, -90, 0 }, "CriticalAllies");
	Actor.SetProperty(falcon1, "DamageModifier", 0.15);
	AI.QueueLast(falcon1, "wait", 5);
	AI.QueueLast(falcon1, "rotate", { 0, 0, 0 }, 0);
	AI.QueueLast(falcon1, "wait", 5);
	
	int falcon2 = Actor.Spawn("FALC", "YT-1300", "Smugglers", "", 0, { 8650, -960, 500 }, { -5, -90, 0 }, "CriticalAllies");
	Actor.SetProperty(falcon2, "DamageModifier", 0.15);
	AI.QueueLast(falcon2, "wait", 5);
	AI.QueueLast(falcon2, "rotate", { 0, 0, 0 }, 0);
	AI.QueueLast(falcon2, "wait", 5);

	int falcon3 = Actor.Spawn("FALC", "YT-1300", "Smugglers", "", 0, { 10250, -1040, 750 }, { -5, -90, 0 }, "CriticalAllies");
	Actor.SetProperty(falcon3, "DamageModifier", 0.15);
	AI.QueueLast(falcon3, "wait", 5);
	AI.QueueLast(falcon3, "rotate", { 0, 0, 0 }, 0);
	AI.QueueLast(falcon3, "wait", 5);

	int falcon4 = Actor.Spawn("FALC", "YT-1300", "Smugglers", "", 0, { 10050, -940, -750 }, { -5, -90, 0 }, "CriticalAllies");
	Actor.SetProperty(falcon4, "DamageModifier", 0.15);
	AI.QueueLast(falcon4, "wait", 5);
	AI.QueueLast(falcon4, "rotate", { 0, 0, 0 }, 0);
	AI.QueueLast(falcon4, "wait", 5);

	int trn0 = Actor.Spawn("TRAN", "", "Smugglers", "TRANSPORT", 0, { 8750, -1200, 0 }, { -5, -90, 0 }, "CriticalAllies");
	AI.QueueLast(trn0, "move", { -8750, -200, 0 }, 50);
	AI.QueueLast(trn0, "hyperspaceout");
	AI.QueueLast(trn0, "setgamestateb","TransportExit",true);
	AI.QueueLast(trn0, "delete");
	
	int trn1 = Actor.Spawn("TRAN", "", "Smugglers", "TRANSPORT", 0, { 9250, -1350, -350 }, { -5, -90, 0 }, "CriticalAllies");
	AI.QueueLast(trn1, "move", { -8260, -350, -350 }, 50);
	AI.QueueLast(trn1, "hyperspaceout");
	AI.QueueLast(trn1, "setgamestateb","TransportExit",true);
	AI.QueueLast(trn1, "delete");
	
	int trn2 = Actor.Spawn("TRAN", "", "Smugglers", "TRANSPORT", 0, { 9050, -1300, 350 }, { -5, -90, 0 }, "CriticalAllies");
	AI.QueueLast(trn2, "move", { -8470, -300, 350 }, 50);
	AI.QueueLast(trn2, "hyperspaceout");
	AI.QueueLast(trn2, "setgamestateb","TransportExit",true);
	AI.QueueLast(trn2, "delete");
	
	int trn3 = Actor.Spawn("TRAN", "", "Smugglers", "TRANSPORT", 0, { 9750, -1450, -250 }, { -5, -90, 0 }, "CriticalAllies");
	AI.QueueLast(trn3, "move", { -7780, -450, -450 }, 50);
	AI.QueueLast(trn3, "hyperspaceout");
	AI.QueueLast(trn3, "setgamestateb","TransportExit",true);
	AI.QueueLast(trn3, "delete");

	int trn4 = Actor.Spawn("TRAN", "", "Smugglers", "TRANSPORT", 0, { 9850, -1400, 150 }, { -5, -90, 0 }, "CriticalAllies");
	AI.QueueLast(trn4, "move", { -7690, -400, 150 }, 50);
	AI.QueueLast(trn4, "hyperspaceout");
	AI.QueueLast(trn4, "setgamestateb","TransportExit",true);
	AI.QueueLast(trn4, "delete");
	
	int trn5 = Actor.Spawn("TRAN", "", "Smugglers", "TRANSPORT", 0, { 10650, -1350, -50 }, { -5, -90, 0 }, "CriticalAllies");
	AI.QueueLast(trn5, "move", { -6900, -350, -50 }, 50);
	AI.QueueLast(trn5, "hyperspaceout");
	AI.QueueLast(trn5, "setgamestateb","TransportExit",true);
	AI.QueueLast(trn5, "delete");

gametick:
	float gframe = GetLastFrameTime();
	float gtime = GetGameTime();

	starttime = starttime - gframe;
	time = time + gframe;
	time2 = time2 + gframe;
	
	if (starttime < 0 && !started) 
		Script.CallX("start");
	
	if (time > interval) 
		Script.CallX("spawntie"); 
	
	if (time2 > interval2) 
		Script.CallX("spawnwing"); 
	
	if (GetTimeSinceLostWing() < gtime || gtime % 0.2 > 0.1)
		UI.SetLine1Text("WINGS: " + Faction.GetWingCount("Smugglers")); 
	else 
		UI.SetLine1Text("");
	
	if (GetTimeSinceLostShip() < gtime || gtime % 0.2 > 0.1)
		UI.SetLine2Text("SHIPS: " + Faction.GetShipCount("Smugglers")); 
	else 
		UI.SetLine2Text("");
	
	if (GetGameStateB("TransportExit") && !triggerwinlose) 
		Script.CallX("win");
	
	if (Faction.GetShipCount("Smugglers") < 1 && !triggerwinlose) 
		SetGameStateB("TransportDead",true);
	
	if (GetGameStateB("TransportDead") && !triggerwinlose) 
		Script.CallX("lose");

win:
	triggerwinlose = true;
	Script.CallX("messagewin");
	SetGameStateB("GameWon",true);
	AddEvent(3, "fadeout");

lose:
	triggerwinlose = true;
	Script.CallX("messagelose");
	SetGameStateB("GameOver",true);
	AddEvent(3, "fadeout");

fadeout:
	Scene.FadeOut();


start:
	started = true;
	Player.SetMovementEnabled(true);
	Script.CallX("message00");

spawntie:
	time = time - interval;
	spawn_faction = "Empire";
	spawn_wait = 2.5;
	spawn_type = "TIE";
	spawn_target = -1;
	spawn_pos = { 400, 0, -10000 };
	spawn_rot = { 0, 0, 0 };

	Script.CallX("spawn4");
	spawn_pos = { -400, 0, -10000 };
	Script.CallX("spawn4");

	spawn_pos = { 0, 0, -10200 };
	spawn_type = "TIEI";
	Script.CallX("spawn2");

spawnwing:
	time2 = time2 - interval2;
	int wing = Actor.Spawn("XWING", "", "Rebels", "", 0, { 7300, 0, 46000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 300, 0, 3000 });
	
	wing = Actor.Spawn("XWING", "", "Rebels", "", 0, { 6700, 0, 46000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { -300, 0, 3000 });
	
	wing = Actor.Spawn("AWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, -50, 3500 });

stage1:
	SetStageNumber(1);

	int corv1 = Actor.Spawn("CORV", "", "Rebels", "", 0, { 8750, -100, 45000 }, { 0, 180, 0 });
	AI.QueueLast(corv1, "hyperspacein", { 750, -100, 1500 });
	AI.QueueLast(corv1, "move", { -750, -100, -5500 }, 15);
	AI.QueueLast(corv1, "hyperspaceout");
	
	int corv2 = Actor.Spawn("CORV", "", "Rebels", "", 0, { 6750, 400, 45000 }, { 0, 180, 0 });
	AI.QueueLast(corv2, "hyperspacein", { 1750, 400, 1500 });
	AI.QueueLast(corv2, "move", { 750, 400, -5500 }, 15);
	AI.QueueLast(corv2, "hyperspaceout");

	int corv3 = Actor.Spawn("NEBL", "", "Rebels", "", 0, { 7750, 200, 45000 }, { 0, 180, 0 });
	AI.QueueLast(corv3, "hyperspacein", { -1250, 200, 3000 });
	AI.QueueLast(corv3, "move", { -250, 400, -5500 }, 10);
	AI.QueueLast(corv3, "hyperspaceout");
	
	int acc1 = Actor.Spawn("ACCL", "", "Empire", "", 0, { -8750, -400, -45000 }, { 0, 0, 0 });
	AI.QueueLast(acc1, "hyperspacein", { -2750, 100, -5000 });
	AI.QueueLast(acc1, "move", { 350, 100, 5500 }, 10);
	AI.QueueLast(acc1, "hyperspaceout");
	
	int acc2 = Actor.Spawn("ACCL", "", "Empire", "", 0, { -4750, -400, -45000 }, { 0, 0, 0 });
	AI.QueueLast(acc2, "hyperspacein", { 1750, -50, -4800 });
	AI.QueueLast(acc2, "move", { 2250, 100, 5500 }, 10);
	AI.QueueLast(acc2, "hyperspaceout");

stage2:
	SetStageNumber(2);

	interval = 50;
	interval2 = 100;

	int corv4 = Actor.Spawn("CORV", "", "Rebels", "", 0, { 8750, -400, 45000 }, { 0, 180, 0 });
	AI.QueueLast(corv4, "hyperspacein", { -2250, -350, 2000 });
	AI.QueueLast(corv4, "move", { -1750, -400, -9500 }, 20);
	AI.QueueLast(corv4, "hyperspaceout");
	
	int corv5 = Actor.Spawn("CORV", "", "Rebels", "", 0, { 6750, -20, 45000 }, { 0, 180, 0 });
	AI.QueueLast(corv5, "hyperspacein", { 2250, -20, 2000 });
	AI.QueueLast(corv5, "move", { 1750, -200, -9500 }, 20);
	AI.QueueLast(corv5, "hyperspaceout");
	
	int monc = Actor.Spawn("MC90", "", "Rebels", "", 0, { 7750, -600, 45000 }, { 0, 180, 0 });
	Actor.SetProperty(monc, "Spawner.Enabled", true);
	AI.QueueLast(monc, "hyperspacein", { -1750, -600, 4000 });
	AI.QueueLast(monc, "move", { -750, -200, -9500 }, 12);
	AI.QueueLast(monc, "hyperspaceout");
	
	int arqt = Actor.Spawn("ARQT", "", "Empire", "", 0, { -4750, -400, -45000 }, { 0, 0, 0 });
	AI.QueueLast(arqt, "hyperspacein", { 3750, 100, -5000 });
	AI.QueueLast(arqt, "move", { 1350, 100, 9500 }, 20);
	AI.QueueLast(arqt, "hyperspaceout");
	
	int vict = Actor.Spawn("VICT", "", "Empire", "", 0, { -8750, -400, -45000 }, { 0, 0, 0 });
	Actor.SetProperty(vict, "Spawner.Enabled", true);
	AI.QueueLast(vict, "hyperspacein", { -3750, -50, -4800 });
	AI.QueueLast(vict, "move", { 1750, 100, 9500 }, 12);
	AI.QueueLast(vict, "hyperspaceout");

	spawn_faction = "Empire";
	spawn_wait = 2.5;
	spawn_target = -1;
	spawn_pos = {-300, 0, -10200 };
	spawn_rot = { 0, 0, 0 };

	spawn_type = "TIED";
	Script.CallX("spawn3");
	spawn_pos = { 300, 0, -10200 };
	Script.CallX("spawn3");

	int wing = Actor.Spawn("AWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, 150, 5500 });
	
	wing = Actor.Spawn("AWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, -150, 5500 });
	
	wing = Actor.Spawn("XWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, 50, 6500 });
	
	wing = Actor.Spawn("XWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, -50, 6500 });

	wing = Actor.Spawn("YWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, 150, 7500 });
	
	wing = Actor.Spawn("YWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, 50, 7500 });

	wing = Actor.Spawn("YWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, -50, 7500 });
	
	wing = Actor.Spawn("YWING", "", "Rebels", "", 0, { 7000, 0, 47000 }, { 0, 180, 0 });
	AI.QueueLast(wing, "hyperspacein", { 0, -150, 7500 });