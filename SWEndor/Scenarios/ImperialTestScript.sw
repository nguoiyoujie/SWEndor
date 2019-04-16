load:
	respawnX = 12000;
	triggerwinlose = false;
	lives = 5;

	SetMaxBounds(10000, 1500, 15000);
	SetMinBounds(-10000, -1500, -20000);
	SetMaxAIBounds(10000, 1500, 15000);
	SetMinAIBounds(-10000, -1500, -20000);
	
	SetMusic("battle_3_2");
	SetMusicLoop("battle_3_3");
	
	Player_SetLives(lives);
	Player_SetScorePerLife(1000000);
	Player_SetScoreForNextLife(1000000);
	Player_ResetScore();
	
	SetUILine1Color(0.6, 0.6, 0);
	SetUILine2Color(0.6, 0.6, 0);
	
	CallScript("makeplayer");
	CallScript("makeimperials");
	AddEvent(3, "start");
	AddEvent(40, "spawnenemy");
	AddEvent(80, "spawnenemy");
	AddEvent(120, "spawnenemy");
	AddEvent(135, "spawnenemy2");
	AddEvent(170, "spawnenemy");
	AddEvent(200, "spawnenemy");
	AddEvent(205, "spawnenemy2");
	AddEvent(300, "spawnenemy");
	
	AddEvent(12, "message01");
	AddEvent(17, "message02");
	AddEvent(22, "message03");
	AddEvent(28, "message04");
	AddEvent(33, "message05");
	AddEvent(40, "message06");

	AddEvent(50, "corvusbeginspawn");


	//AddEvent(157, "stage2");

loadfaction:
	AddFaction("Empire", 0, 0.8, 0);
	AddFaction("Traitors", 0.4, 0.5, 0.9);
	Faction_SetWingSpawnLimit("Empire", 25);
	Faction_SetWingSpawnLimit("Traitors", 25);

loadscene:
	greywolf = Actor_Spawn("Imperial-I Star Destroyer", "ISD GREY WOLF (Thrawn)", "", "GREY WOLF", 0, "Empire", 1000, 400, 12000, 0, -180, 0, "CriticalAllies");
	Actor_SetActive(greywolf);
	Actor_SetProperty("SetSpawnerEnable", true);
	Actor_QueueLast("move", -1000, 400, -3000, 25);
	Actor_QueueLast("rotate", -2000, 210, -20000, 0);
	Actor_QueueLast("lock");
	
	corvus = Actor_Spawn("Interdictor Star Destroyer", "INT CORVUS", "", "CORVUS", 0, "Empire", 3500, -500, 500, 0, -130, 0, "CriticalAllies");
	Actor_SetActive(corvus);
	Actor_QueueLast("move", 2000, -500, -1500, 9);
	Actor_QueueLast("rotate", 0, -500, 4000, 0);
	Actor_QueueLast("lock");
	
	Actor_SetActive(Actor_Spawn("Arquitens Light Cruiser", "EBOLO", "", "EBOLO", 0, "Empire", 3000, -120, 350, 0, -130, 45));
	Actor_SetProperty("DamageModifier", 0.4);
	Actor_QueueLast("move", -400, 200, -350, 100);
	Actor_QueueLast("move", 1000, 150, -3000, 100);
	Actor_QueueLast("move", -8470, -300, -350, 100);
	Actor_QueueLast("rotate", -2000, 210, -20000, 0);
	Actor_QueueLast("lock");
	
	Actor_SetActive(Actor_Spawn("Arquitens Light Cruiser", "DARING", "", "DARING", 0, "Empire", 3200, -300, -1450, 0, -130, 45));
	Actor_SetProperty("DamageModifier", 0.4);
	Actor_QueueLast("move", -7780, -450, 450, 15);
	Actor_QueueLast("move", -8470, -300, -350, 100);
	Actor_QueueLast("rotate", -2000, 210, -20000, 0);
	Actor_QueueLast("lock");

	Actor_SetActive(Actor_Spawn("Devastator Imperial-I Star Destroyer", "ISD GLORY", "", "ISD GLORY", 0, "Traitors", -6750, 0, -22000, 0, 40, 0, "CriticalEnemies"));
	Actor_SetProperty("SetSpawnerEnable", true);
	Actor_QueueLast("move", -1000, 100, -6000, 100);
	Actor_QueueLast("move", -4000, 200, -10000, 10);
	Actor_QueueLast("rotate", -2000, 210, -20000, 0);
	Actor_QueueLast("lock");

	Actor_SetActive(Actor_Spawn("Nebulon-B Frigate", "", "", "", 0, "Traitors", -3750, -450, -16000, 0, 30, 0));
	Actor_QueueLast("move", 6250, -450, -3200, 100);
	Actor_QueueLast("hyperspaceout");

	Actor_SetActive(Actor_Spawn("Corellian Corvette", "", "", "", 0, "Traitors", -8750, -100, -11000, 0, 75, 0));
	Actor_QueueLast("move", 1550, -200, -2200, 100);
	Actor_QueueLast("rotate", 0, 500, 4000, 0);
	Actor_QueueLast("lock");

	Actor_SetActive(Actor_Spawn("Corellian Corvette", "", "", "", 0, "Traitors", -3750, 300, -12500, 0, 45, 0));
	Actor_QueueLast("move", 2750, -100, -800, 100);
	Actor_QueueLast("rotate", 0, 500, 4000, 0);
	Actor_QueueLast("lock");

	Actor_SetActive(Actor_Spawn("Corellian Corvette", "", "", "", 0, "Traitors", -1750, 50, -14500, 0, 20, 0));
	Actor_QueueLast("move", -750, 100, -1400, 100);
	Actor_QueueLast("rotate", 0, 500, 4000, 0);
	Actor_QueueLast("lock");

makeplayer:
	lives = lives - 1;
	Player_SetLives(lives);
	player = Actor_Spawn(GetPlayerActorType(), "(Player)", "", "(Player)", 0, "Empire", 500, -300, respawnX, 0, -180, 0);
	Actor_SetActive(player);
	Actor_SetProperty("DamageModifier", 0.25);
	Player_AssignActor();
	Actor_RegisterEvents();
	respawnX = 5000;

makeimperials:
	Actor_SetActive(Actor_Spawn("TIE Avenger", "Alpha-2", "", "", 0, "Empire", 700, -620, 10500, 0, -180, 0));
	Actor_SetProperty("DamageModifier", 0.25);
	Actor_QueueLast("wait", 2.5);
	
	Actor_SetActive(Actor_Spawn("TIE Avenger", "Alpha-3", "", "", 0, "Empire", 6000, 300, -500, 0, -90, 0));
	Actor_SetProperty("DamageModifier", 0.6);
	Actor_QueueLast("rotate", 0, 0, 0, 0);
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Avenger", "Alpha-4", "", "", 0, "Empire", 6500, 600, -750, 0, -90, 0));
	Actor_SetProperty("DamageModifier", 0.6);
	Actor_QueueLast("rotate", 0, 0, 0, 0);
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Defender", "Delta-1", "", "", 0, "Empire", 7000, 300, -500, 0, -90, 0));
	Actor_SetProperty("DamageModifier", 0.6);
	Actor_QueueLast("rotate", 0, 0, 0, 0);
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Defender", "Delta-2", "", "", 0, "Empire", 7500, 600, -750, 0, -90, 0));
	Actor_SetProperty("DamageModifier", 0.6);
	Actor_QueueLast("rotate", 0, 0, 0, 0);
	Actor_QueueLast("wait", 2.5);


	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Empire", 500, 90, 8000, 0, -180, 0));
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Empire", 480, -120, 8100, 0, -180, 0));
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Empire", -500, 100, 8050, 0, -180, 0));
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Empire", -515, -100, 8000, 0, -180, 0));
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Empire", 1500, 290, 11000, 0, -180, 0));
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Empire", 1480, -20, 11120, 0, -180, 0));
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Empire", 500, 100, 11000, 0, -180, 0));
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Empire", 515, -50, 10980, 0, -180, 0));
	Actor_QueueLast("wait", 2.5);

	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", 500, 100, -2000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", 500, -100, -2000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", 300, 100, -2000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", 300, -100, -2000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", -500, 100, -2000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE Bomber", "", "", "", 0, "Traitors", -500, -100, -3000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE Bomber", "", "", "", 0, "Traitors", -300, 100, -3000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE Bomber", "", "", "", 0, "Traitors", -300, -100, -3000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Traitors", -100, 0, -3000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE Interceptor", "", "", "", 0, "Traitors", -100, 0, -3000, 0, 0, 0));
	Actor_QueueLast("hunt");

gametick:
	SetUILine1Text("WINGS: " + Faction_GetWingCount("Empire"));
	SetUILine2Text("ENEMY: " + Faction_GetWingCount("Traitors"));
	
	if (Faction_GetShipCount("Traitors") < 1 && !triggerwinlose) then CallScript("win");
	if (Faction_GetShipCount("Empire") < 1 && !triggerwinlose) then CallScript("lose");

win:
	triggerwinlose = true;
	Message("Our Transports have entered hyperspace. Let us leave this area.", 5, 0, 0, 0.8, 1);
	SetGameStateB("GameWon",true);
	AddEvent(3, "Common_FadeOut");

lose:
	triggerwinlose = true;
	Message("All our Transports has been destroyed!", 5, 0.6, 0.6, 0, 1);
	SetGameStateB("GameOver",true);
	AddEvent(3, "Common_FadeOut");

start:
	Player_SetMovementEnabled(true);
	Message("Form up, squadron.", 5, 0, 0.8, 0, 1);

corvusbeginspawn:
	Actor_SetActive(corvus);
	Actor_SetProperty("SetSpawnerEnable", true);

spawnenemy:
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", 500, 100, -20000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", 500, -100, -20000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", 300, 100, -20000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", 300, -100, -20000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", -500, 100, -20000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", -500, -100, -20000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", -300, 100, -20000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("TIE", "", "", "", 0, "Traitors", -300, -100, -20000, 0, 0, 0));
	Actor_QueueLast("hunt");
	
spawnenemy2:
	Actor_SetActive(Actor_Spawn("TIE Defender", "", "", "", 0, "Traitors", 500, 0, -20200, 0, 0, 0));
	Actor_QueueLast("hunt");

	Actor_SetActive(Actor_Spawn("TIE Defender", "", "", "", 0, "Traitors", -500, 0, -20200, 0, 0, 0));
	Actor_QueueLast("hunt");

	Actor_SetActive(Actor_Spawn("X-Wing", "", "", "", 0, "Traitors", 100, -50, -21200, 0, 0, 0));
	Actor_QueueLast("hunt");
	
	Actor_SetActive(Actor_Spawn("A-Wing", "", "", "", 0, "Traitors", -100, 50, -21200, 0, 0, 0));
	Actor_QueueLast("hunt");

	Actor_SetActive(Actor_Spawn("Y-Wing", "", "", "", 0, "Traitors", 100, -50, -22200, 0, 0, 0));
	Actor_QueueLast("hunt");
	

stage1:
	SetStageNumber(1);


stage2:
	SetStageNumber(2);
		
	