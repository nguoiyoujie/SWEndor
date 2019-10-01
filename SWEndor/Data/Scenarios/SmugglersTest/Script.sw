load:
	starttime = 3;
	triggerwinlose = false;
	started = false;
	time = 34;
	interval = 35;
	time2 = 70;
	interval2 = 75;
	
	SetMaxBounds(10000, 1500, 8000);
	SetMinBounds(-10000, -1500, -8000);
	SetMaxAIBounds(10000, 1500, 8000);
	SetMinAIBounds(-10000, -1500, -8000);
	
	SetMusic("battle_2_1");
	SetMusicLoop("battle_2_2");
	
	Player.SetLives(3);
	Player.SetScorePerLife(1000000);
	Player.SetScoreForNextLife(1000000);
	Player.ResetScore();
	
	SetUILine1Color(0.6, 0.6, 0);
	SetUILine2Color(0.6, 0.6, 0);

	CallScript("spawnreset");	
	CallScript("makesmugglers");
	CallScript("makeplayer");
	AddEvent(7, "stage1");
	AddEvent(12, "message01");
	AddEvent(17, "message02");
	AddEvent(22, "message03");
	AddEvent(28, "message04");
	AddEvent(35, "message05");
	AddEvent(154, "message06");
	AddEvent(157, "stage2");

loadfaction:
	AddFaction("Smugglers", 0.6, 0.6, 0);
	Faction.SetAsMainAllyFaction("Smugglers");
	AddFaction("Empire", 0, 0.8, 0);
	AddFaction("Rebels", 0.8, 0, 0);
	Faction.SetWingSpawnLimit("Empire", 24);
	Faction.SetWingSpawnLimit("Rebels", 10);

loadscene:
	planet = Actor.Spawn("HOTH", "", "", "", 0, "", 0, -20000, 0, 0, 180, 0);
	Actor.SetProperty(planet, "Scale", 10);
	
	impSD1 = Actor.Spawn("IMPL", "", "", "", 0, "Empire", 0, 500, -15000, 0, 25, 0);
	Actor.QueueLast(impSD1, "move", 0, 500, -12000, 2);
	Actor.QueueLast(impSD1, "rotate", 0, 500, 12000, 0);
	Actor.QueueLast(impSD1, "lock");

makeplayer:
	Player.DecreaseLives();
	Player.RequestSpawn();
	AddEvent(5, "setupplayer");

setupplayer:
	Actor.SetProperty(Player.GetActor(), "DamageModifier", 0.25);
	Actor.RegisterEvents(Player.GetActor());

makesmugglers:
	pspawner = Actor.Spawn("SPAWN", "", "", "", 0, "Smugglers", 9500, -900, 0, 0, -90, 45);
	Actor.SetProperty(pspawner, "SetSpawnerEnable", true);

	falcon1 = Actor.Spawn("FALC", "YT-1300", "", "", 0, "Smugglers", 8600, -1020, -500, 0, -90, 45, "CriticalAllies");
	Actor.SetProperty(falcon1, "DamageModifier", 0.15);
	Actor.QueueLast(falcon1, "wait", 5);
	Actor.QueueLast(falcon1, "rotate", 0, 0, 0, 0);
	Actor.QueueLast(falcon1, "wait", 5);
	
	falcon2 = Actor.Spawn("FALC", "YT-1300", "", "", 0, "Smugglers", 8650, -960, 500, 0, -90, 45, "CriticalAllies");
	Actor.SetProperty(falcon2, "DamageModifier", 0.15);
	Actor.QueueLast(falcon2, "wait", 5);
	Actor.QueueLast(falcon2, "rotate", 0, 0, 0, 0);
	Actor.QueueLast(falcon2, "wait", 5);

	falcon3 = Actor.Spawn("FALC", "YT-1300", "", "", 0, "Smugglers", 10250, -1040, 750, 0, -90, 45, "CriticalAllies");
	Actor.SetProperty(falcon3, "DamageModifier", 0.15);
	Actor.QueueLast(falcon3, "wait", 5);
	Actor.QueueLast(falcon3, "rotate", 0, 0, 0, 0);
	Actor.QueueLast(falcon3, "wait", 5);

	falcon4 = Actor.Spawn("FALC", "YT-1300", "", "", 0, "Smugglers", 10050, -940, -750, 0, -90, 45, "CriticalAllies");
	Actor.SetProperty(falcon4, "DamageModifier", 0.15);
	Actor.QueueLast(falcon4, "wait", 5);
	Actor.QueueLast(falcon4, "rotate", 0, 0, 0, 0);
	Actor.QueueLast(falcon4, "wait", 5);

	trn0 = Actor.Spawn("TRAN", "", "trn0", "TRAN", 0, "Smugglers", 8750, -1200, 0, 0, -90, 45, "CriticalAllies");
	Actor.QueueLast(trn0, "move", -8750, -200, 0, 50);
	Actor.QueueLast(trn0, "hyperspaceout");
	Actor.QueueLast(trn0, "setgamestateb","TransportExit",true);
	Actor.QueueLast(trn0, "delete");
	
	trn1 = Actor.Spawn("TRAN", "", "trn1", "TRAN", 0, "Smugglers", 9250, -1350, -350, 0, -90, 45, "CriticalAllies");
	Actor.QueueLast(trn1, "move", -8260, -350, -350, 50);
	Actor.QueueLast(trn1, "hyperspaceout");
	Actor.QueueLast(trn1, "setgamestateb","TransportExit",true);
	Actor.QueueLast(trn1, "delete");
	
	trn2 = Actor.Spawn("TRAN", "", "trn2", "TRAN", 0, "Smugglers", 9050, -1300, 350, 0, -90, 45, "CriticalAllies");
	Actor.QueueLast(trn2, "move", -8470, -300, 350, 50);
	Actor.QueueLast(trn2, "hyperspaceout");
	Actor.QueueLast(trn2, "setgamestateb","TransportExit",true);
	Actor.QueueLast(trn2, "delete");
	
	trn3 = Actor.Spawn("TRAN", "", "trn3", "TRAN", 0, "Smugglers", 9750, -1450, -250, 0, -90, 45, "CriticalAllies");
	Actor.QueueLast(trn3, "move", -7780, -450, -450, 50);
	Actor.QueueLast(trn3, "hyperspaceout");
	Actor.QueueLast(trn3, "setgamestateb","TransportExit",true);
	Actor.QueueLast(trn3, "delete");

	trn4 = Actor.Spawn("TRAN", "", "trn4", "TRAN", 0, "Smugglers", 9850, -1400, 150, 0, -90, 45, "CriticalAllies");
	Actor.QueueLast(trn4, "move", -7690, -400, 150, 50);
	Actor.QueueLast(trn4, "hyperspaceout");
	Actor.QueueLast(trn4, "setgamestateb","TransportExit",true);
	Actor.QueueLast(trn4, "delete");
	
	trn5 = Actor.Spawn("TRAN", "", "trn5", "TRAN", 0, "Smugglers", 10650, -1350, -50, 0, -90, 45, "CriticalAllies");
	Actor.QueueLast(trn5, "move", -6900, -350, -50, 50);
	Actor.QueueLast(trn5, "hyperspaceout");
	Actor.QueueLast(trn5, "setgamestateb","TransportExit",true);
	Actor.QueueLast(trn5, "delete");

gametick:
	starttime = starttime - GetLastFrameTime();
	time = time + GetLastFrameTime();
	time2 = time2 + GetLastFrameTime();
	
	if (starttime < 0 && !started) 
		CallScript("start");
	
	if (time > interval) 
		CallScript("spawntie"); 
	
	if (time2 > interval2) 
		CallScript("spawnwing"); 
	
	if (GetTimeSinceLostWing() < GetGameTime() || GetGameTime() % 0.2 > 0.1) 
		SetUILine1Text("WINGS: " + Faction.GetWingCount("Smugglers")); 
	else 
		SetUILine1Text("");
	
	if (GetTimeSinceLostShip() < GetGameTime() || GetGameTime() % 0.2 > 0.1) 
		SetUILine2Text("SHIPS: " + Faction.GetShipCount("Smugglers")); 
	else 
		SetUILine2Text("");
	
	if (GetGameStateB("TransportExit") && !triggerwinlose) 
		CallScript("win");
	
	if (Faction.GetShipCount("Smugglers") < 1 && !triggerwinlose) 
		SetGameStateB("TransportDead",true);
	
	if (GetGameStateB("TransportDead") && !triggerwinlose) 
		CallScript("lose");

win:
	triggerwinlose = true;
	Message("Our Transports have entered hyperspace. Let us leave this area.", 5, 0.6, 0.6, 0, 1);
	SetGameStateB("GameWon",true);
	AddEvent(3, "Common.FadeOut");

lose:
	triggerwinlose = true;
	Message("All our Transports has been destroyed!", 5, 0.6, 0.6, 0, 1);
	SetGameStateB("GameOver",true);
	AddEvent(3, "Common.FadeOut");

start:
	started = true;
	Player.SetMovementEnabled(true);
	Message("We are entering Imperial Territory.", 5, 0.6, 0.6, 0, 1);

spawntie:
	time = time - interval;
	spawnfaction = "Empire";
	spawnwait = 2.5;
	spawntype = "TIE";
	spawntarget = -1;
	spawnX = 400;
	spawnY = 0;
	spawnZ = -10000;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;

	CallScript("spawn4");
	spawnX = -400;
	CallScript("spawn4");

	spawnX = 0;
	spawnZ = -10200;
	spawntype = "TIEI";
	CallScript("spawn2");

spawnwing:
	time2 = time2 - interval2;
	wing = Actor.Spawn("XWING", "", "", "", 0, "Rebels", 7300, 0, 46000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 300, 0, 3000);
	
	wing = Actor.Spawn("XWING", "", "", "", 0, "Rebels", 6700, 0, 46000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", -300, 0, 3000);
	
	wing = Actor.Spawn("AWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, -50, 3500);

stage1:
	SetStageNumber(1);

	corv1 = Actor.Spawn("CORV", "", "", "", 0, "Rebels", 8750, -100, 45000, 0, 180, 0);
	Actor.QueueLast(corv1, "hyperspacein", 750, -100, 1500);
	Actor.QueueLast(corv1, "move", -750, -100, -5500, 15);
	Actor.QueueLast(corv1, "hyperspaceout");
	
	corv2 = Actor.Spawn("CORV", "", "", "", 0, "Rebels", 6750, 400, 45000, 0, 180, 0);
	Actor.QueueLast(corv2, "hyperspacein", 1750, 400, 1500);
	Actor.QueueLast(corv2, "move", 750, 400, -5500, 15);
	Actor.QueueLast(corv2, "hyperspaceout");

	corv3 = Actor.Spawn("NEBL", "", "", "", 0, "Rebels", 7750, 200, 45000, 0, 180, 0);
	Actor.QueueLast(corv3, "hyperspacein", -1250, 200, 3000);
	Actor.QueueLast(corv3, "move", -250, 400, -5500, 10);
	Actor.QueueLast(corv3, "hyperspaceout");
	
	acc1 = Actor.Spawn("ACCL", "", "", "", 0, "Empire", -8750, -400, -45000, 0, 0, 0);
	Actor.QueueLast(acc1, "hyperspacein", -2750, 100, -5000);
	Actor.QueueLast(acc1, "move", 350, 100, 5500, 10);
	Actor.QueueLast(acc1, "hyperspaceout");
	
	acc2 = Actor.Spawn("ACCL", "", "", "", 0, "Empire", -4750, -400, -45000, 0, 0, 0);
	Actor.QueueLast(acc2, "hyperspacein", 1750, -50, -4800);
	Actor.QueueLast(acc2, "move", 2250, 100, 5500, 10);
	Actor.QueueLast(acc2, "hyperspaceout");

stage2:
	SetStageNumber(2);

	interval = 50;
	interval2 = 100;

	corv4 = Actor.Spawn("CORV", "", "", "", 0, "Rebels", 8750, -400, 45000, 0, 180, 0);
	Actor.QueueLast(corv4, "hyperspacein", -2250, -350, 2000);
	Actor.QueueLast(corv4, "move", -1750, -400, -9500, 20);
	Actor.QueueLast(corv4, "hyperspaceout");
	
	corv5 = Actor.Spawn("CORV", "", "", "", 0, "Rebels", 6750, -20, 45000, 0, 180, 0);
	Actor.QueueLast(corv5, "hyperspacein", 2250, -20, 2000);
	Actor.QueueLast(corv5, "move", 1750, -200, -9500, 20);
	Actor.QueueLast(corv5, "hyperspaceout");
	
	monc = Actor.Spawn("MC90", "", "", "", 0, "Rebels", 7750, -600, 45000, 0, 180, 0);
	Actor.SetProperty(monc, "SetSpawnerEnable", true);
	Actor.QueueLast(monc, "hyperspacein", -1750, -600, 4000);
	Actor.QueueLast(monc, "move", -750, -200, -9500, 12);
	Actor.QueueLast(monc, "hyperspaceout");
	
	arqt = Actor.Spawn("ARQT", "", "", "", 0, "Empire", -4750, -400, -45000, 0, 0, 0);
	Actor.QueueLast(arqt, "hyperspacein", 3750, 100, -5000);
	Actor.QueueLast(arqt, "move", 1350, 100, 9500, 20);
	Actor.QueueLast(arqt, "hyperspaceout");
	
	vict = Actor.Spawn("VICT", "", "", "", 0, "Empire", -8750, -400, -45000, 0, 0, 0);
	Actor.SetProperty(vict, "SetSpawnerEnable", true);
	Actor.QueueLast(vict, "hyperspacein", -3750, -50, -4800);
	Actor.QueueLast(vict, "move", 1750, 100, 9500, 12);
	Actor.QueueLast(vict, "hyperspaceout");

	spawnfaction = "Empire";
	spawnwait = 2.5;
	spawntarget = -1;
	spawnX = -300;
	spawnY = 0;
	spawnZ = -10200;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	spawntype = "TIED";
	CallScript("spawn3");
	spawnX = 300;
	CallScript("spawn3");

	wing = Actor.Spawn("AWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, 150, 5500);
	
	wing = Actor.Spawn("AWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, -150, 5500);
	
	wing = Actor.Spawn("XWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, 50, 6500);
	
	wing = Actor.Spawn("XWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, -50, 6500);

	wing = Actor.Spawn("YWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, 150, 7500);
	
	wing = Actor.Spawn("YWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, 50, 7500);

	wing = Actor.Spawn("YWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, -50, 7500);
	
	wing = Actor.Spawn("YWING", "", "", "", 0, "Rebels", 7000, 0, 47000, 0, 180, 0);
	Actor.QueueLast(wing, "hyperspacein", 0, -150, 7500);