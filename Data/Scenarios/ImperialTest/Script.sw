load:
	respawn = false;
	triggerwinlose = false;
	triggerdangergreywolf = false;
	triggerdangercorvus = false;

	SetMaxBounds(10000, 1500, 15000);
	SetMinBounds(-10000, -1500, -20000);
	SetMaxAIBounds(10000, 1500, 15000);
	SetMinAIBounds(-10000, -1500, -20000);

	Player.SetLives(5);
	Player.SetScorePerLife(1000000);
	Player.SetScoreForNextLife(1000000);
	Player.ResetScore();
	
	SetUILine1Color(0, 0.8, 0);
	SetUILine2Color(0.4, 0.5, 0.9);
	
  CallScript("engagemusic");	
	CallScript("spawnreset");	
	CallScript("makeimperials");
	CallScript("makeplayer");
	AddEvent(2, "getchildren");
	AddEvent(3, "start");
	AddEvent(5, "spawnenemybombers");
	AddEvent(40, "spawnenemy");
	AddEvent(80, "spawnenemy");
	AddEvent(120, "spawnenemy");
	AddEvent(135, "spawnenemy2");
	AddEvent(170, "spawnenemy");
	AddEvent(200, "spawnenemy");
	AddEvent(205, "spawnenemy2");
	AddEvent(240, "spawnenemybombers2");
	AddEvent(270, "messagebombercorvus");
	AddEvent(300, "spawnenemy");
	AddEvent(325, "spawnenemy2");
	AddEvent(360, "messagebombergreywolf");
	AddEvent(380, "spawnenemybombers3");

	AddEvent(17, "message01");
	AddEvent(22, "message02");
	AddEvent(27, "message03");
	AddEvent(34, "message04");
	AddEvent(38, "message05");
	AddEvent(45, "message06");

	AddEvent(50, "corvusbeginspawn");


	//AddEvent(157, "stage2");

loadfaction:
	AddFaction("Empire", 0, 0.8, 0);
	AddFaction("Traitors", 0.4, 0.5, 0.9);
	AddFaction("Rebels", 0.8, 0, 0);
	Faction.MakeAlly("Traitors", "Rebels");
	Faction.SetWingSpawnLimit("Empire", 26);
	Faction.SetWingSpawnLimit("Traitors", 32);

loadscene:
	greywolf = Actor.Spawn("IMPL", "ISD GREY WOLF (Thrawn)", "", "GREY WOLF", 0, "Empire", 1000, 400, 12000, 0, -180, 0, "CriticalAllies");
	Actor.SetProperty(greywolf, "DamageModifier", 0.8);
	Actor.SetProperty(greywolf, "SetSpawnerEnable", true);
	Actor.QueueLast(greywolf, "move", -1000, 400, -3000, 25);
	Actor.QueueLast(greywolf, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(greywolf, "lock");

	corvus = Actor.Spawn("INTD", "INT CORVUS", "", "CORVUS", 0, "Empire", 3500, -500, 500, 0, -130, 0, "CriticalAllies");
	Actor.SetProperty(corvus, "DamageModifier", 0.6);
	Actor.QueueLast(corvus, "move", 2000, -500, -1500, 8);
	Actor.QueueLast(corvus, "rotate", 0, -500, 4000, 0);
	Actor.QueueLast(corvus, "lock");

	ebolo = Actor.Spawn("ARQT", "EBOLO", "", "EBOLO", 0, "Empire", 3000, -120, 350, 0, -130, 45);
	Actor.SetProperty(ebolo, "DamageModifier", 0.4);
	Actor.QueueLast(ebolo, "move", -400, 200, -350, 100);
	Actor.QueueLast(ebolo, "move", 1000, 150, -3000, 100);
	Actor.QueueLast(ebolo, "move", -8470, -300, -350, 100);
	Actor.QueueLast(ebolo, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(ebolo, "lock");
	
	daring = Actor.Spawn("ARQT", "DARING", "", "DARING", 0, "Empire", 3200, -300, -1450, 0, -130, 45);
	Actor.SetProperty(daring, "DamageModifier", 0.4);
	Actor.QueueLast(daring, "move", -7780, -450, 450, 15);
	Actor.QueueLast(daring, "move", -8470, -300, -350, 100);
	Actor.QueueLast(daring, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(daring, "lock");

	glory = Actor.Spawn("DEVA", "ISD GLORY", "", "GLORY", 0, "Traitors", -6750, 0, -22000, 0, 40, 0, "CriticalEnemies");
	Actor.SetProperty(glory, "SetSpawnerEnable", true);
	Actor.SetProperty(glory, "DamageModifier", 0.75);
	Actor.QueueLast(glory, "move", -1000, 100, -6000, 70);
	Actor.QueueLast(glory, "move", 4000, 200, -10000, 10);
	Actor.QueueLast(glory, "rotate", 2000, 210, -20000, 0);
	Actor.QueueLast(glory, "lock");

	corv1 = Actor.Spawn("CORV", "", "", "", 0, "Traitors", -7750, 250, -20000, 0, 20, 0);
	Actor.QueueLast(corv1, "move", 0, 250, -3000, 70);
	Actor.QueueLast(corv1, "move", 3000, 250, -5000, 10);
	Actor.QueueLast(corv1, "rotate", 0, 500, 4000, 0);
	Actor.QueueLast(corv1, "lock");

	nebu = Actor.Spawn("NEBL", "", "", "", 0, "Traitors", -3750, -450, -16000, 0, 30, 0);
	Actor.QueueLast(nebu, "move", 6250, -450, -3200, 100);
	Actor.QueueLast(nebu, "hyperspaceout");
	Actor.QueueLast(nebu, "delete");

	corv2 = Actor.Spawn("CORV", "", "", "", 0, "Traitors", -8750, -100, -11000, 0, 75, 0);
	Actor.QueueLast(corv2, "move", 1550, -200, -2200, 100);
	Actor.QueueLast(corv2, "rotate", 0, 500, 4000, 0);
	Actor.QueueLast(corv2, "lock");

	corv3 = Actor.Spawn("CORV", "", "", "", 0, "Traitors", -3750, 300, -12500, 0, 45, 0);
	Actor.QueueLast(corv3, "move", 2750, -100, -800, 100);
	Actor.QueueLast(corv3, "rotate", 0, 500, 4000, 0);
	Actor.QueueLast(corv3, "lock");

	corv4 = Actor.Spawn("CORV", "", "", "", 0, "Traitors", -1750, 50, -14500, 0, 20, 0);
	Actor.QueueLast(corv4, "move", -750, 100, -1400, 100);
	Actor.QueueLast(corv4, "rotate", 0, 500, 4000, 0);
	Actor.QueueLast(corv4, "lock");

getchildren:
	greywolfc = Actor.GetChildren(greywolf);
	greywolfshd1 = GetArrayElement(greywolfc, 30);
	greywolfshd2 = GetArrayElement(greywolfc, 31);
	corvusc = Actor.GetChildren(corvus);
	corvusshd1 = GetArrayElement(corvusc, 18);
	corvusshd2 = GetArrayElement(corvusc, 19);

engagemusic:
	SetMood(4);
	SetMusicDyn("PANIC-06");

makeplayer:
	Player.DecreaseLives();
	if (respawn) 
		if (!playerisship)
			Player.RequestSpawn(); 
		else
			Player.AssignActor(Actor.Spawn(GetPlayerActorType(), "(Player)", "", "(Player)", 0, "Empire", 7000, -300, 0, 0, -120, 0));
	else 
		CallScript("firstspawn");
	
	AddEvent(5, "setupplayer");

setupplayer:
	//Actor.SetProperty(Player.GetActor(), "DamageModifier", 0.25);
	Actor.RegisterEvents(Player.GetActor());
	playerisship = Actor.IsLargeShip(Player.GetActor());
	if (respawn) 
		CallScript("respawn");

firstspawn:
	Player.AssignActor(Actor.Spawn(GetPlayerActorType(), "(Player)", "", "(Player)", 0, "Empire", 500, -300, 12500, 0, -180, 0));
	respawn = true;

respawn:
	Actor.AddToSquad(Player.GetActor(), tiea1);
	Actor.AddToSquad(Player.GetActor(), tiea2);
	Actor.AddToSquad(Player.GetActor(), tiea3);

makeimperials:
	tiea1 = Actor.Spawn(GetPlayerActorType(), "Alpha-2", "", "", 0, "Empire", 700, -620, 10500, 0, -180, 0);
	Actor.SetProperty(tiea1, "DamageModifier", 0.25);
	Actor.QueueLast(tiea1, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea1);

	tiea2 = Actor.Spawn("TIEA", "Alpha-3", "", "", 4, "Empire", 6000, 300, -500, 0, -90, 0);
	Actor.SetProperty(tiea2, "DamageModifier", 0.25);
	Actor.QueueLast(tiea2, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea2);

	tiea3 = Actor.Spawn("TIEA", "Alpha-4", "", "", 4, "Empire", 6500, 600, -750, 0, -90, 0);
	Actor.SetProperty(tiea3, "DamageModifier", 0.25);
	Actor.QueueLast(tiea3, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea3);

	tied1 = Actor.Spawn("TIED", "Delta-1", "", "", 4, "Empire", 7000, 300, -500, 0, -90, 0);
	Actor.SetProperty(tied1, "DamageModifier", 0.25);
	Actor.QueueLast(tied1, "wait", 2.5);

	tied2 = Actor.Spawn("TIED", "Delta-2", "", "", 4, "Empire", 7500, 600, -750, 0, -90, 0);
	Actor.SetProperty(tied2, "DamageModifier", 0.25);
	Actor.QueueLast(tied2, "wait", 2.5);
	Actor.AddToSquad(tied1, tied2);

	spawnhyperspace = false;
	spawnfaction = "Empire";
	damagemod = 0.25;
	spawnwait = 3.5;
	spawntype = "TIEI";
	spawntarget = -1;
	spawnX = 200;
	spawnY = 0;
	spawnZ = 8000;
	spawnRotX = 0;
	spawnRotY = -180;
	spawnRotZ = 0;
	CallScript("spawn4");

	spawnX = 1300;
	spawnY = 150;
	spawnZ = 11000;
	spawnRotX = 0;
	spawnRotY = -180;
	spawnRotZ = 0;
	CallScript("spawn4");

	spawnfaction = "Traitors";
	damagemod = 0.5;
	spawnwait = 0;
	spawnX = 400;
	spawnY = 0;
	spawnZ = -2000;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	spawntype = "TIE";
	CallScript("spawn4");

	spawnX = -400;
	spawnY = 0;
	spawnZ = -2000;
	CallScript("spawn4");

	spawnX = -2000;
	spawnY = 0;
	spawnZ = -3500;
	spawntarget = corvus;
	spawntype = "TIESA";
	CallScript("spawn4");

	spawnX = -4600;
	spawnY = 0;
	spawnZ = -3000;
	spawntarget = corvus;
	spawntype = "TIE";
	CallScript("spawn4");

	spawnX = -5400;
	spawnY = 0;
	spawnZ = -3000;
	spawntarget = -1;
	CallScript("spawn4");


gametick:
	SetUILine1Text("WINGS: " + Faction.GetWingCount("Empire"));
	SetUILine2Text("ENEMY: " + Faction.GetWingCount("Traitors"));
	
	hp = Actor.GetProperty(greywolf, "Strength");
	if ((hp == null || hp <= 0) && !triggerwinlose) 
		CallScript("losegreywolf");
	
	if (!triggerwinlose && !triggerdangergreywolf && hp < 450) 
		CallScript("dangergreywolf");

	hp = Actor.GetProperty(corvus, "Strength");
	if ((hp == null || hp <= 0) && !triggerwinlose) 
		CallScript("losecorvus");
	
	if (!triggerwinlose && !triggerdangercorvus && hp < 300) 
		CallScript("dangercorvus");

	if (Faction.GetShipCount("Traitors") < 1 && !triggerwinlose) 
		CallScript("win");

win:
	triggerwinlose = true;
	Message("Such is the fate of the enemies of the Empire.", 5, 0, 0.8, 0, 1);
	SetGameStateB("GameWon",true);
	AddEvent(3, "Common.FadeOut");

dangergreywolf:
	triggerdangergreywolf = true;
	AddEvent(0.0, "dangergreywolfmsgG");
	AddEvent(0.2, "dangergreywolfmsgY");
	AddEvent(0.4, "dangergreywolfmsgG");
	AddEvent(0.6, "dangergreywolfmsgY");
	AddEvent(0.8, "dangergreywolfmsgG");
	AddEvent(1.0, "dangergreywolfmsgY");
	AddEvent(1.2, "dangergreywolfmsgG");
	AddEvent(1.4, "dangergreywolfmsgY");
	AddEvent(1.6, "dangergreywolfmsgG");
	AddEvent(1.8, "dangergreywolfmsgY");
	AddEvent(2.0, "dangergreywolfmsgG");
	AddEvent(2.2, "dangergreywolfmsgY");
	AddEvent(2.4, "dangergreywolfmsgG");
	AddEvent(2.6, "dangergreywolfmsgY");
	AddEvent(2.8, "dangergreywolfmsgG");
	AddEvent(3.0, "dangergreywolfmsgY");

dangergreywolfmsgY:
	Message("WARNING! Our flagship is under heavy fire!", 3, 0.8, 0.8, 0, 1);

dangergreywolfmsgG:
	Message("WARNING! Our flagship is under heavy fire!", 3, 0, 0.8, 0, 1);


losegreywolf:
	triggerwinlose = true;
	Message("MISSION FAILED: Our flagship has been destroyed!", 5, 0.8, 0, 0, 1);
	SetGameStateB("GameOver",true);
	AddEvent(3, "Common.FadeOut");

dangercorvus:
	triggerdangercorvus = true;
	AddEvent(0.0, "dangercorvusmsgG");
	AddEvent(0.2, "dangercorvusmsgY");
	AddEvent(0.4, "dangercorvusmsgG");
	AddEvent(0.6, "dangercorvusmsgY");
	AddEvent(0.8, "dangercorvusmsgG");
	AddEvent(1.0, "dangercorvusmsgY");
	AddEvent(1.2, "dangercorvusmsgG");
	AddEvent(1.4, "dangercorvusmsgY");
	AddEvent(1.6, "dangercorvusmsgG");
	AddEvent(1.8, "dangercorvusmsgY");
	AddEvent(2.0, "dangercorvusmsgG");
	AddEvent(2.2, "dangercorvusmsgY");
	AddEvent(2.4, "dangercorvusmsgG");
	AddEvent(2.6, "dangercorvusmsgY");
	AddEvent(2.8, "dangercorvusmsgG");
	AddEvent(3.0, "dangercorvusmsgY");

dangercorvusmsgY:
	Message("WARNING! INT Corvus is under heavy fire!", 3, 0.8, 0.8, 0, 1);

dangercorvusmsgG:
	Message("WARNING! INT Corvus is under heavy fire!", 3, 0, 0.8, 0, 1);

losecorvus:
	triggerwinlose = true;
	Message("MISSION FAILED: INT CORVUS has been destroyed!", 5, 0.8, 0, 0, 1);
	SetGameStateB("GameOver",true);
	AddEvent(3, "Common.FadeOut");

start:
	Player.SetMovementEnabled(true);

corvusbeginspawn:
	Actor.SetProperty(corvus, "SetSpawnerEnable", true);

spawnenemybombers:
	spawnfaction = "Traitors";
	damagemod = 1;
	spawnwait = 0;
	spawnX = -2000;
	spawnY = 250;
	spawnZ = -22500;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	spawntype = "TIESA";
	spawntarget = corvusshd1;
	CallScript("spawn1");

	spawnX = -4000;
	spawnY = 50;
	spawnZ = -12000;
	spawntarget = corvusshd2;
	CallScript("spawn2");

	spawnX = -5000;
	spawnY = -150;
	spawnZ = -24500;
	spawntarget = corvus;
	CallScript("spawn4");

	spawnX = -25000;
	spawnY = 300;
	spawnZ = -23500;
	spawntype = "TIEA";
	spawntarget = corvusshd1;
	CallScript("spawn1");

	spawnX = -24000;
	spawnY = 200;
	spawnZ = -24500;
	spawntype = "TIED";
	spawntarget = corvus;
	CallScript("spawn2");

spawnenemybombers2:
	damagemod = 1;
	spawnwait = 0;
	spawnX = -10000;
	spawnY = 250;
	spawnZ = -12500;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	spawntype = "TIE";
	spawntarget = corvusshd1;
	CallScript("spawn2");

	spawnX = -11000;
	spawnY = 150;
	spawnZ = -13500;
	spawntarget = corvusshd2;
	CallScript("spawn2");

	spawnX = -11000;
	spawnY = 50;
	spawnZ = -13500;
	spawntype = "TIESA";
	spawntarget = corvus;
	CallScript("spawn2");

spawnenemybombers3:
	//spawnfaction = "Traitors";
	damagemod = 1;
	spawnwait = 0;
	spawnX = -10000;
	spawnY = 250;
	spawnZ = -12500;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	spawntype = "TIESA";
	spawntarget = greywolfshd1;
	CallScript("spawn4");

	spawnX = -11000;
	spawnY = 450;
	spawnZ = -13500;
	spawntarget = greywolfshd2;
	CallScript("spawn4");

	spawnX = -11000;
	spawnY = 650;
	spawnZ = -13500;
	spawntype = "GUN";
	spawntarget = greywolf;
	CallScript("spawn3");

	spawnX = -12000;
	spawnY = 150;
	spawnZ = -13500;
	spawntype = "TIE";
	CallScript("spawn4");

	spawnX = -10000;
	spawnY = 150;
	spawnZ = -13500;
	CallScript("spawn4");

spawnenemy:
	spawnfaction = "Traitors";
	damagemod = 1;
	spawnwait = 0;
	spawntype = "TIE";
	spawntarget = -1;
	spawnX = 400;
	spawnY = 0;
	spawnZ = -20000;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	CallScript("spawn4");

	spawnX = -400;
	CallScript("spawn4");

	spawntype = "Z95";
	spawnX = 300;
	spawnZ = -21000;
	CallScript("spawn3");

	spawnX = -300;
	CallScript("spawn3");

spawnenemy2:
	spawnfaction = "Traitors";
	damagemod = 1;
	spawnwait = 0;
	spawntype = "TIED";
	spawntarget = -1;
	spawnX = -5500;
	spawnY = 0;
	spawnZ = -20200;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	CallScript("spawn2");

	spawnX = -12500;
	spawnY = -50;
	spawnZ = -25000;
	spawntype = "TIEA";
	CallScript("spawn1");

	spawnX = -5000;
	spawnY = -50;
	spawnZ = -21200;
	spawntype = "XWING";
	CallScript("spawn2");

	spawnX = -4800;
	spawnY = 50;
	spawnZ = -21100;
	spawntype = "AWING";
	CallScript("spawn1");

	spawnX = -5000;
	spawnY = -50;
	spawnZ = -22200;
	spawntype = "YWING";
	CallScript("spawn3");


stage1:
	SetStageNumber(1);


stage2:
	SetStageNumber(2);
		
	