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

load:

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


	//AddEvent(157, "stage2");

loadfaction:
	AddFaction("Empire", 0, 0.8, 0);
	AddFaction("Traitors", 0.4, 0.5, 0.9);
	AddFaction("Rebels", 0.8, 0, 0);
	Faction.MakeAlly("Traitors", "Rebels");
	Faction.SetWingSpawnLimit("Empire", 30);
	Faction.SetWingSpawnLimit("Traitors", 24);

loadscene:
	greywolf = Actor.Spawn("IMPL", "ISD GREY WOLF (Thrawn)", "", "GREY WOLF", 0, "Empire", 1000, 400, 12000, 0, -180, 0, "CriticalAllies");
	Actor.SetProperty(greywolf, "DamageModifier", 0.8);
	Actor.SetProperty(greywolf, "SetSpawnerEnable", true);
	Actor.QueueLast(greywolf, "move", -1000, 400, -3000, 25);
	Actor.QueueLast(greywolf, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(greywolf, "lock");

	corvus = Actor.Spawn("INTD", "INT CORVUS", "", "CORVUS", 0, "Empire", 3500, -500, 500, 0, -130, 0, "CriticalAllies");
	Actor.SetProperty(corvus, "DamageModifier", 0.6);
	Actor.QueueLast(corvus, "move", 2400, -500, -1000, 12);
	Actor.QueueLast(corvus, "move", 0, -500, 4000, 20);
	Actor.QueueLast(corvus, "rotate", -2400, -500, 7000, 0);
	Actor.QueueLast(corvus, "lock");

	ebolo = Actor.Spawn("ARQT", "EBOLO", "", "EBOLO", 0, "Empire", 3000, -120, 350, 0, -210, 25);
	Actor.SetProperty(ebolo, "DamageModifier", 0.4);
	Actor.QueueLast(ebolo, "move", -400, 200, -350, 50);
	Actor.QueueLast(ebolo, "move", 1000, 150, -3000, 25);
	Actor.QueueLast(ebolo, "move", -8470, -300, -350, 100);
	Actor.QueueLast(ebolo, "rotate", -2000, 210, -20000, 0);
	Actor.QueueLast(ebolo, "lock");
	
	daring = Actor.Spawn("ARQT", "DARING", "", "DARING", 0, "Empire", 3200, -300, -1450, 0, -150, 25);
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
	Actor.QueueLast(corv2, "move", 1550, 200, -2200, 100);
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
			Player.AssignActor(Actor.Spawn(GetPlayerActorType(), "(Player)", "", "(Player)", 0, "Empire", 7000, -300, 0, 0, -120, 0));
	else 
		CallScript("firstspawn");
	
	AddEvent(5, "setupplayer");

setupplayer:
	Actor.RegisterEvents(Player.GetActor());
	playerisship = Actor.IsLargeShip(Player.GetActor());
	if (respawn) 
		CallScript("respawn");

firstspawn:
	Player.AssignActor(Actor.Spawn(GetPlayerActorType(), "(Player)", "", "(Player)", 0, "Empire", 500, -300, 12500, 0, -180, 0));
	respawn = true;

respawn:
	Actor.AddToSquad(Player.GetActor(), tiea2);
	Actor.AddToSquad(Player.GetActor(), tiea3);
	Actor.AddToSquad(Player.GetActor(), tiea4);

makeimperials:
	tiea2 = Actor.Spawn("TIEA", "Alpha-2", "", "", 0, "Empire", 700, -620, 10500, 0, -180, 0);
	Actor.SetProperty(tiea2, "DamageModifier", 0.25);
	Actor.QueueLast(tiea2, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea2);

	tiea3 = Actor.Spawn("TIEA", "Alpha-3", "", "", 4, "Empire", 6000, 300, -500, 0, -90, 0);
	Actor.SetProperty(tiea3, "DamageModifier", 0.25);
	Actor.QueueLast(tiea3, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea3);

	tiea4 = Actor.Spawn("TIEA", "Alpha-4", "", "", 4, "Empire", 6500, 600, -750, 0, -90, 0);
	Actor.SetProperty(tiea4, "DamageModifier", 0.25);
	Actor.QueueLast(tiea4, "wait", 2.5);
	Actor.AddToSquad(Player.GetActor(), tiea4);

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
	Message("Such is the fate of the enemies of the Empire.", 5, 0, 0.8, 0, 1);
	SetGameStateB("GameWon",true);
	AddEvent(3, "Common.FadeOut");

dangergreywolf:
	triggerdangergreywolf = true;
	for (float time = 0; time < 3; time += 0.4)
	{
		AddEvent(time, "dangergreywolfmsgG");
		AddEvent(time + 0.2, "dangergreywolfmsgY");
	}

losegreywolf:
	triggerwinlose = true;
	CallScript("messagelosegreywolf");
	SetGameStateB("GameOver",true);
	AddEvent(3, "Common.FadeOut");

dangercorvus:
	triggerdangercorvus = true;
	float time = 0;
	while (time < 3)
	{
		AddEvent(time, "dangercorvusmsgG");
		AddEvent(time + 0.2, "dangercorvusmsgY");
		time += 0.4;
	}



losecorvus:
	triggerwinlose = true;
	CallScript("messagelosecorvus");
	SetGameStateB("GameOver",true);
	AddEvent(3, "Common.FadeOut");

start:
	Player.SetMovementEnabled(true);

corvusbeginspawn:
	Faction.SetWingSpawnLimit("Empire", 36);
	Actor.SetProperty(corvus, "SetSpawnerEnable", true);

enemybeginspawn:
	Faction.SetWingSpawnLimit("Traitors", 28);
	Actor.SetProperty(nebu, "SetSpawnerEnable", true);

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
	spawntype = "TIEI";
	spawntarget = corvusshd1;
	CallScript("spawn1");

	spawnX = -24000;
	spawnY = 200;
	spawnZ = -24500;
	spawntype = "TIE";
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

spawn_allyGUN:
	spawnfaction = "Empire";
	spawnhyperspace = true;
	damagemod = 1;
	spawnwait = 0;
	spawntype = "GUN";
	spawntarget = corv2;
	spawnX = 2400;
	spawnY = 500;
	spawnZ = 10000;
	spawnRotX = 0;
	spawnRotY = 180;
	spawnRotZ = 0;
	CallScript("spawn4");
	SetMood(-11);
	CallScript("message_allyGUN");


spawn_allyTIEA:
	spawnfaction = "Empire";
	spawnhyperspace = true;
	damagemod = 1;
	spawnwait = 0;
	spawntype = "TIEA";
	spawntarget = corv4;
	spawnX = 2400;
	spawnY = 500;
	spawnZ = 10000;
	spawnRotX = 0;
	spawnRotY = 180;
	spawnRotZ = 0;
	CallScript("spawn4");
	SetMood(-11);
	CallScript("message_allyTIEA");


spawn_traitorZ95:
	spawnfaction = "Traitors";
	spawnhyperspace = true;
	damagemod = 1;
	spawnwait = 0;
	spawntarget = -1;

	spawntype = "Z95";
	spawnX = 300;
	spawnY = 0;
	spawnZ = -15000;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
		CallScript("spawn4");
	SetMood(-21);
	CallScript("message_traitorZ95");


spawn_traitorXWING:
	spawnfaction = "Traitors";
	spawnhyperspace = true;
	damagemod = 1;
	spawnwait = 0;
	spawntarget = -1;

	spawntype = "XWING";
	spawnX = -1000;
	spawnY = 0;
	spawnZ = -12500;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	CallScript("spawn3");
	SetMood(-21);
	CallScript("message_traitorXWING");


	spawn_traitorYWING:
	spawnfaction = "Traitors";
	spawnhyperspace = true;
	damagemod = 1;
	spawnwait = 0;
	spawntarget = -1;

	spawntype = "YWING";
	spawnX = -3000;
	spawnY = 0;
	spawnZ = -16000;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	CallScript("spawn3");

	SetMood(-21);
	CallScript("message_traitorYWING");


spawn_traitorAWING:
	spawnfaction = "Traitors";
	spawnhyperspace = true;
	damagemod = 1;
	spawnwait = 0;
	spawntarget = -1;

	spawntype = "AWING";
	spawnX = -2500;
	spawnY = 0;
	spawnZ = -14000;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	CallScript("spawn3");

	SetMood(-21);
	CallScript("message_traitorAWING");


spawn_traitorTIED:
	spawnfaction = "Traitors";
	spawnhyperspace = true;
	damagemod = 1;
	spawnwait = 0;
	spawntarget = -1;

	spawntype = "TIED";
	spawnX = -1600;
	spawnY = 0;
	spawnZ = -17000;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	CallScript("spawn2");

	SetMood(-21);
	CallScript("message_traitorTIED");


spawn_traitorTIEA:
	spawnfaction = "Traitors";
	spawnhyperspace = true;
	damagemod = 1;
	spawnwait = 0;
	spawntarget = -1;

	spawntype = "TIEA";
	spawnX = -4200;
	spawnY = 0;
	spawnZ = -17000;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;
	CallScript("spawn2");

	SetMood(-21);
	CallScript("message_traitorTIEA");



stage1:
	SetStageNumber(1);


stage2:
	SetStageNumber(2);
		
	