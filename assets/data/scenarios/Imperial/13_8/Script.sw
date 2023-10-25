// Globals
// global toggles
bool respawn;
bool triggerwinlose;
bool triggerescapecorv;
bool playerisship;

// you
int alpha1;

// your partner (used to re-invoke squad formation)
int alpha2;

// important actors
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
int corv5;
int vorknkx;

// important subactors
int corvusshd1;
int corvusshd2;
int gloryhangar;

// minelaying code
int corv1_mines = 12;
int corv2_mines = 12;
int corv3_mines = 12;
int corv4_mines = 12;
int corv5_mines = 12;
int vork_mines = 16;

float playerkillcount = 0;
float playerkillcountawe = 32;
int tiedcount = 0;
int tieDdowncount = 0;

// escape position
float3 shuttle1_escape = {-3750, -500, -28000};
float3 shuttle2_escape = {4750, 350, -28000};
float3 vorknkx_escape = {-13750, 0, -27000};

// faction colors
float3 faction_empire_color = { 0, 0.8, 0 };
float3 faction_traitor_color = { 0.4, 0.5, 0.9 };
float3 faction_rebel_color = { 0.8, 0, 0 };
float3 faction_empire_laser_color = { 0.1, 1, 0.12 };
float3 faction_rebel_laser_color = { 1, 0, 0 };

// message colors
float3 msg_thrawn = { 0.3, 0.5, 1 };
float3 msg_corvus = { 0.6, 0.8, 1 };
float3 msg_warn = { 0.8, 0.8, 0 };
float3 msg_bad = { 1, 0.3, 0.2 };
float3 msg_fail = { 0.8, 0, 0 };

// placeholder
float3 _d = { 0, 0, 0 };
//int _a = -1;

Load:
    // Scene bounds
	Scene.SetMaxBounds({10000, 4500, 15000});
	Scene.SetMinBounds({-15000, -4500, -30000});
	Scene.SetMaxAIBounds({10000, 4500, 15000});
	Scene.SetMinAIBounds({-15000, -4500, -30000});

    // Player Info
	Player.SetLives(5);
	Score.SetScorePerLife(1000000);
	Score.SetScoreForNextLife(1000000);
	Score.Reset();

    // Prep UI
	UI.SetLine1Color(faction_empire_color);
	UI.SetLine2Color(faction_traitor_color);
	UI.SetLine3Color(faction_traitor_color);
	
    // Reset common functions
	spawn_reset();
	actorp_reset();
	
    // Init functions
	InitMusic();	
	InitShips();
	InitFighters();
	MakePlayer();

    // Queued events
	AddEvent(1, "InitChildren");
	AddEvent(5, "Spawn.TraitorBombers");    // Enemy 7xT/B, 1xT/I attack
	AddEvent(42, "Spawn.TraitorZDelta");    // Enemy 6xT/D attack
	AddEvent(62, "Spawn.AllyUpsilon");      // Ally 4xGUN defense
	AddEvent(80, "Spawn.AllyEta");          // Ally 4xT/F counterattack
	AddEvent(86, "Spawn.AllyBeta");         // Ally 6xT/B, 2xT/F counterattack
	AddEvent(90, "Spawn.Configure1");       // Ally spawn defense
	AddEvent(120, "Spawn.TraitorZIota");    // Enemy 6xT/D attack
	AddEvent(145, "Spawn.AllyIota");        // Ally 4xT/A counterattack
	
	AddEvent(200, "Spawn.TraitorZTheta");   // Enemy 6xT/D attack
	AddEvent(210, "Spawn.Configure2");      // Enemy spawn defense
	
	AddEvent(7, "Message.Brief01");
	AddEvent(12, "Message.Brief02");
	AddEvent(17, "Message.Brief03");
	AddEvent(24, "Message.Brief04");
	AddEvent(28, "Message.Brief05");
	AddEvent(35, "Message.Brief06");


LoadFaction:
    // Note: "Rebels" are not used in this mission
	Faction.Add("Empire", faction_empire_color, faction_empire_laser_color);
	Faction.Add("Traitors", faction_traitor_color, faction_empire_laser_color);
	Faction.Add("Rebels", faction_rebel_color, faction_rebel_laser_color);
	Faction.MakeAlly("Traitors", "Rebels");
	Faction.SetWingSpawnLimit("Empire", 14);
	Faction.SetWingSpawnLimit("Traitors", 12);

	
InitChildren:
	int[] corvusc = Actor.GetChildrenByType(corvus, "SHD");
	corvusshd1 = corvusc[0]; // T/B targets
	corvusshd2 = corvusc[1]; // T/B targets
	int[] gloryc = Actor.GetChildrenByType(glory, "HANGAR");	
	gloryhangar = gloryc[0]; // for Vorknkx spawn

	
InitMusic:
	Audio.Mood.Combat();
    Audio.SetMusicDyn("PANIC-06");

	
MakePlayer:
	Player.DecreaseLives();
	if (respawn) 
    {
        if (!playerisship)
            Player.RequestSpawn(); // Request respawn from a friendly HANGAR (ISD Greywolf)
        else
            Player.AssignActor(Actor.Spawn(GetPlayerActorType(), GetPlayerName(), "Empire", "", 0, { 4700, -500, -400 }, { 0, -120, 0 }));

        AddEvent(1, "PrepPlayer"); // Allow request spawn to conduct its job
    }
	else 
    {
        // spawning the first time. 
        Player.AssignActor(Actor.Spawn(GetPlayerActorType(), GetPlayerName(), "Empire", "", 0, { 4200, -950, 450 }, { 0, -130, 0 }));
        Player.SetMovementEnabled(false);
        PrepPlayer();
        Actor.SetProperty(alpha1, "Movement.Speed", 650);
        if (Actor.IsLargeShip(alpha1))
        {
            Scene.SetMaxBounds({10000, 4500, 105000});
            Actor.SetLocalPosition(alpha1, { 8000, -300, 100000 });
            Player.SetAI(true);
            AI.ForceClearQueue(alpha1);
            AI.QueueNext(alpha1, "wait", 0.5);
            AI.QueueNext(alpha1, "hyperspacein", { 4500, -300, 0 });
            AI.QueueLast(alpha1, "setgamestateb", "TurnPlayerAIOff", true);
        }
        respawn = true;    
        AddEvent(1.5, "GivePlayerControl");
    }

	
PrepPlayer:
    alpha1 = Player.GetActor();
    if (alpha1 != -1)
    {
        playerisship = Actor.IsLargeShip(alpha1);
        Actor.CallOnRegisterKill(alpha1, "ScoreKill.player");
        ReformPlayerSquad();
    }


GivePlayerControl:
	Player.SetMovementEnabled(true);


ReformPlayerSquad:
	Squad.JoinSquad(alpha1, alpha2);

	
InitShips:
	// Empire
	greywolf = Actor.Spawn("ISD", "GREY WOLF (Thrawn)", "Empire", "GREY WOLF", 0, { 1000, 400, 12000 }, { 0, -180, 0 }, { "CriticalAllies" });
	Actor.SetProperty(greywolf, "Spawner.Enabled", true);
	Actor.SetProperty(greywolf, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIEI"});
	AI.QueueLast(greywolf, "move", {-1000, 400, 0}, 15);
	AI.QueueLast(greywolf, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(greywolf, "lock");

	corvus = Actor.Spawn("INTD", "CORVUS", "Empire", "CORVUS", 0, { 3500, -575, 500 }, { 0, -130, 0 }, { "CriticalAllies" });
	AI.QueueLast(corvus, "rotate", {2400, -575, -1000}, 0);
	AI.QueueLast(corvus, "lock");
    Speech.RegisterCriticalCraft(corvus);
    
	ebolo = Actor.Spawn("STRKC", "EBOLO", "Empire", "EBOLO", 0, { 3000, -20, 350 }, { 0, -110, 25 }, { "CriticalAllies" });
	AI.QueueLast(ebolo, "move", {200, 450, -3500}, 5);
	AI.QueueLast(ebolo, "move", {-2470, 500, -7350}, 3);
	AI.QueueLast(ebolo, "move", {3100, 500, -11350}, 2);
	AI.QueueLast(ebolo, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(ebolo, "lock");
	
	daring = Actor.Spawn("STRKC", "DARING", "Empire", "DARING", 0, { 3200, -300, -1450 }, { 0, -150, 25 }, { "CriticalAllies" });
	AI.QueueLast(daring, "move", {-140, 350, -7250}, 5);
	AI.QueueLast(daring, "move", {1470, 350, -10050}, 3);
	AI.QueueLast(daring, "move", {5100, 300, -12350}, 2);
	AI.QueueLast(daring, "rotate", {-2000, 210, -20000}, 0);
	AI.QueueLast(daring, "lock");
	
	// Traitors
	glory = Actor.Spawn("ISD", "GLORY", "Traitors", "GLORY", 0, { -9750, 100, -32000 }, { 0, 40, 0 }, { "CriticalEnemies" });
	Actor.SetProperty(glory, "Spawner.Enabled", true);
	Actor.SetProperty(glory, "Spawner.SpawnTypes", {"TIE_S","TIEI_S","TIEI_S","TIEI_S"});
	AI.QueueLast(glory, "move", {-1000, 200, -6000}, 35);
	AI.QueueLast(glory, "move", {5000, 300, -10000}, 10);
	AI.QueueLast(glory, "rotate", {2000, 410, -20000}, 0);
	AI.QueueLast(glory, "lock");

	corv1 = Actor.Spawn("CORV", "Z-SHOOTING STAR", "Traitors", "", 0, { -10750, 350, -30000 }, { 0, 20, 0 });
	AI.QueueLast(corv1, "move", {0, 350, -3000}, 35);
	AI.QueueLast(corv1, "move", {4000, 350, -5000}, 10);
	AI.QueueLast(corv1, "rotate", {0, 600, 4000}, 0);
	AI.QueueLast(corv1, "lock");
	
	corv2 = Actor.Spawn("CORV", "Z-RUSH", "Traitors", "", 0, { -12750, -100, -16000 }, { 0, 75, 0 });
	AI.QueueLast(corv2, "move", {1550, 200, -2200}, 100);
	AI.QueueLast(corv2, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv2, "lock");

	corv3 = Actor.Spawn("CORV", "Z-DIVINE WIND", "Traitors", "", 0, { -6750, 300, -17500 }, { 0, 45, 0 });
	AI.QueueLast(corv3, "move", {750, -100, -800}, 100);
	AI.QueueLast(corv3, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv3, "lock");

	corv4 = Actor.Spawn("STRKC", "Z-PINCER", "Traitors", "", 0, { -2750, 50, -19500 }, { 0, 20, 0 });
	AI.QueueLast(corv4, "move", {-750, 100, -1400}, 100);
	AI.QueueLast(corv4, "rotate", {0, 500, 4000}, 0);
	AI.QueueLast(corv4, "lock");
    
    corv5 = Actor.Spawn("CORV", "Z-FLAME", "Traitors", "", 0, { -7250, 310, -32500 }, { 0, 30, 0 });
	AI.QueueLast(corv5, "move", {-400, 280, -2000}, 35);
	AI.QueueLast(corv5, "move", {5500, 250, -4000}, 10);
	AI.QueueLast(corv5, "rotate", {0, 200, 4000}, 0);
	AI.QueueLast(corv5, "lock");
	
	nebu = Actor.Spawn("NEB2", "Z-EBONY", "Traitors", "", 0, { -6750, -450, -21000 }, { 0, 30, 0 });
	AI.QueueLast(nebu, "move", {1750, -450, -3200}, 50);
	AI.QueueLast(nebu, "lock");
	
	AddEvent(65, "Minelayer.corv1");
	AddEvent(3, "Minelayer.corv2");
	AddEvent(4, "Minelayer.corv3");
	AddEvent(5, "Minelayer.corv4");
	AddEvent(95, "Minelayer.corv5");

	
InitFighters:

	// Empire initial spawns
	
	alpha2 = Actor.Spawn("TIEA", "ALPHA 2", "Empire", "", 0, { 700, -620, 10500 }, { 0, -180, 0 });
    Actor.CallOnRegisterKill(alpha2, "ScoreKill.alpha2");
    Actor.CallOnDead(alpha2, "Lost.alpha2");

	Actor.Spawn("TIEI", "", "Empire", "", 0, { 6000, 300, -500 }, { 0, -90, 0 });
	Actor.Spawn("TIEI", "", "Empire", "", 0, { 6500, 600, -750 }, { 0, -90, 0 });
	Actor.Spawn("TIEI", "", "Empire", "", 0, { 7000, 300, -500 }, { 0, -90, 0 });
	Actor.Spawn("TIEI", "", "Empire", "", 0, { 7500, 600, -750 }, { 0, -90, 0 });

	foreach (int a in {alpha2})
	{
		Common.MultiplyShields(a, 4);
		AI.QueueLast(a, "wait", 2.5);
	}
	
	spawn_hyperspace = false;
	spawn_faction = "Empire";
	spawn_dmgmod = 1;
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
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_pos = { 400, -100, -5000 };
	spawn_rot = { 0, 0, 0 };
	spawn_type = "TIE_S";
	Script.Call("spawn4");
	
	spawn_pos = { -400, -400, -5200 };
	Script.Call("spawn4");
	
	spawn_pos = { -2000, 0, -9500 };
	spawn_target = corvus;
	spawn_type = "TIESA_S";
	Script.Call("spawn2");
	
	spawn_pos = { -4600, 200, -7200 };
	spawn_target = corvus;
	spawn_type = "TIE_S";
	Script.Call("spawn4");
	
	spawn_pos = { -5400, -150, -6600 };
	spawn_target = -1;
	Script.Call("spawn4");

	// Empire spawn reserves
	
	for (int i = 1; i <= 8; i++)
		Actor.QueueAtSpawner(Actor.Spawn("TIE", "", "Empire", "", 0, _d, _d), greywolf);
	
	// Traitor spawn reserves
	
	for (int i = 1; i <= 8; i++)
		Actor.QueueAtSpawner(Actor.Spawn("TIEI_S", "", "Traitors", "", 0, _d, _d), glory);


Tick:
    // Set UI
	UI.SetLine1Text("WINGS: " + Faction.GetWingCount("Empire"));
	UI.SetLine2Text("ENEMY: " + Faction.GetWingCount("Traitors"));
    if (triggerescapecorv && !GetGameStateB("VorknkxDestroyed"))
    {
        float vorknkx_escape_dist = Math.GetDistance(vorknkx_escape, Actor.GetGlobalPosition(vorknkx));
        UI.SetLine3Text("VORKN: " + vorknkx_escape_dist);
    }
    else 
    {
        tiedcount = 0;
        foreach (int t in Faction.GetWings("Traitors"))
        {
            if (Actor.GetActorType(t) == "TIED")
            {
                tiedcount = tiedcount + 1;
            }
        }
        if (tiedcount > 0)
        {
            UI.SetLine3Text("T/D:   " + tiedcount);
        }
        else
        {
            UI.SetLine3Text("");
        }
    }
    
	
	if (!triggerwinlose)
	{
        Check.greywolf();
        Check.corvus();
        Check.ebolo();
        Check.daring();
        Check.glory();
		
        if (triggerescapecorv)
		{
            Check.vorknkx();
            Check.win();
		}

        Check.flightGroups();
        if (GetGameStateB("TurnPlayerAIOff"))
        {
            Player.SetAI(false);
            SetGameStateB("TurnPlayerAIOff", false);
        }
	}


// Nice one, Player!
ScoreKill.player(int player, int victim):
    if (!Faction.IsAlly(Actor.GetFaction(player) , Actor.GetFaction(victim)) && (Actor.IsFighter(victim) || Actor.IsLargeShip(victim)))
    {
        playerkillcount = playerkillcount + 1;
        if (playerkillcount == playerkillcountawe)
        {
            Audio.QueueSpeech("13M8\13m8r12");
        }
        else
        {
            if (Random() < 0.4) 
            {
                Audio.QueueSpeech("a-target", "a-destr");
            }
            
            float rnd = Random();
            if (rnd < 0.15) 
            {
                Audio.QueueSpeech("a-good", "a-shoot", "a-alpha", "a-one");
            }
            else if (rnd < 0.3) 
            {
                Audio.QueueSpeech("a-good", "a-hunt", "a-alpha", "a-one");
            }
            else if (rnd < 0.45) 
            {
                Audio.QueueSpeech("a-excel", "a-shoot", "a-alpha", "a-one");
            }
            else if (rnd < 0.6) 
            {
                Audio.QueueSpeech("a-super", "a-shoot", "a-alpha", "a-one");
            }
        }
    }


// Alpha 2
ScoreKill.alpha2(int actor, int victim):
    if (!Faction.IsAlly(Actor.GetFaction(actor), Actor.GetFaction(victim)) && (Actor.IsFighter(victim) || Actor.IsLargeShip(victim)))
    {
        if (Random() < 0.3) 
        {
            Audio.QueueSpeech("a-alpha", "a-one");
        }
        if (Random() < 0.3) 
        {
            Audio.QueueSpeech("a-this", "a-alpha", "a-two");
        }
        Audio.QueueSpeech("a-target", "a-destr");
    }


Lost.alpha2:
    Audio.QueueSpeech("13M8\13m8r10");



// ISD Greywolf
Check.greywolf:
    float hullfrac = Actor.GetHullFrac(greywolf);
    if (hullfrac <= 0) 
    {
        Lose.greywolf();
    }
    else if (!GetGameStateB("GreyWolfInDanger") && hullfrac < 0.5) 
    {
        SetGameStateB("GreyWolfInDanger", true);
        Warn.greywolf();
    }
    

Warn.greywolf:
    Common.MessageFlashDanger("WARNING! Our flagship is under heavy fire!");

	
Lose.greywolf:
	triggerwinlose = true;
    Common.MessageCritical("MISSION FAILED: Our flagship has been destroyed!", msg_fail);
	SetGameStateB("GameOver",true);
	AddEvent(3, "CurtainCall");
	Audio.Mood.PriObjFail();


// INT Corvus
Check.corvus:
    float hullfrac = Actor.GetHullFrac(corvus);
    float shd = Actor.GetShd(corvus);
    if (hullfrac <= 0) 
    {
        Lose.corvus();
    }
    else if (!GetGameStateB("CorvusInDanger") && hullfrac < 0.5) 
    {
        SetGameStateB("CorvusInDanger", true);
        Warn.corvus();
    }
    else if (!GetGameStateB("CorvusShieldsDown")  && shd <= 0) 
    {
        SetGameStateB("CorvusShieldsDown", true);
        Warn.corvus_shd();
    }

	
Warn.corvus:
    Audio.QueueSpeech("13M8\13m8r4");
    Audio.QueueSpeech("13M8\13m8r5");
    Common.MessageFlashDanger("WARNING! INT Corvus might not hold any longer!");


Warn.corvus_shd:
    Audio.QueueSpeech("13M8\13m8r2");
    Audio.QueueSpeech("13M8\13m8r3");
    Common.MessageFlashDanger("WARNING! INT Corvus shields have been depleted!");

	
Lose.corvus:
    // enemy hyperspace out
    foreach (int ship in Faction.GetShips("Traitors"))
    {
        AI.QueueFirst(ship, "hyperspaceout");
        AI.QueueFirst(ship, "wait", 1.5);
    }
	triggerwinlose = true;
    Common.MessageCritical("MISSION FAILED: INT CORVUS has been destroyed! Zaarin has made his escape!", msg_fail);
	SetGameStateB("GameOver",true);
	AddEvent(3, "CurtainCall");
	Audio.Mood.PriObjFail();


// STRKC Ebolo
Check.ebolo:
	if (!GetGameStateB("EboloDestroyed"))
	{
		if (Actor.GetHP(ebolo) <= 0) 
		{
            SetGameStateB("EboloDestroyed", true);
            Lose.ebolo();
		}			
		else if (!GetGameStateB("EboloShieldsDown"))
		{
			if (Actor.GetShd(ebolo) <= 0) 
			{
                Warn.ebolo();
				SetGameStateB("EboloShieldsDown", true);
			}
		}
	}


Warn.ebolo:
    Audio.QueueSpeech("13M8\13m8r13");
	Common.MessageStandard("STRIKE CRUISER EBOLO's shields are down, but it will continue fighting!", msg_warn);


Lose.ebolo:
    Common.MessageStandard("STRIKE CRUISER EBOLO has been destroyed.", msg_bad);            
	Audio.Mood.AllyBigLoss();


// STRKC Daring
Check.daring:
	if (!GetGameStateB("DaringDestroyed"))
	{
        float hullfrac = Actor.GetHullFrac(daring);
		if (hullfrac <= 0) 
		{
            SetGameStateB("DaringDestroyed", true);
            Lose.daring();
		}			
		else if (!GetGameStateB("DaringInDanger"))
		{
			if (hullfrac <= 0.5) 
			{
                SetGameStateB("DaringInDanger", true);
                Warn.daring();
			}
		}
	}



Warn.daring:
    Audio.QueueSpeech("13M8\13m8r14");
    Common.MessageStandard("STRIKE CRUISER DARING probably won't make it, we'll fight to the death anyway!", msg_warn);
   

Lose.daring:
    Common.MessageStandard("STRIKE CRUISER DARING has been destroyed.", msg_bad);
	Audio.Mood.AllyBigLoss();


// ISD Glory
Check.glory:
    float hullfrac = Actor.GetHullFrac(glory);
    if (!triggerescapecorv)
	{
		if (hullfrac < 0.05) 
        {
        	triggerescapecorv = true;
            Spawn.vorknkx();
        }	
    }
	if (!GetGameStateB("GloryDestroyed"))
	{
		if (hullfrac <= 0) 
		{
			SetGameStateB("GloryDestroyed", true);
			Destroyed.glory();
			Audio.Mood.SecObjComplete();
			AddEvent(15, "Audio.Mood.Combat"); // return to combat music
		}			
		else if (!GetGameStateB("GloryShieldsDown"))
		{
			if (Actor.GetShd(glory) <= 0) 
			{
				SetGameStateB("GloryShieldsDown", true);
                ShieldsDown.glory();
			}
		}
        else if (!GetGameStateB("GloryShuttlesEscape"))
		{
			if (hullfrac <= 0.6 && tiedcount == 0) 
			{
				SetGameStateB("GloryShuttlesEscape", true);
                Spawn.TraitorShuttles();
			}
		}
        else if (!GetGameStateB("GloryHullCritical"))
		{
			if (hullfrac <= 0.25) 
			{
				SetGameStateB("GloryHullCritical", true);
                Critical.glory();
			}
		}
	}


Destroyed.glory:
	Common.MessageStandard("ISD GLORY has been destroyed.", faction_empire_color);
	Audio.Mood.SecObjComplete();


Critical.glory:
    Audio.QueueSpeech("13M8\13m8r9");
	Common.MessageStandard("ISD GLORY's hull is critical. Go for the kill, ALHPA 1.", faction_empire_color);


ShieldsDown.glory:
    Audio.QueueSpeech("13M8\13m8r7");
	Common.MessageStandard("ISD GLORY's shields are down, the traitors will have no hope of escape.", faction_empire_color);



// CRV/M Vorknkx
Check.vorknkx:
	if (GetGameStateB("Escaped"))
		Escaped.vorknkx();
	
	if (!GetGameStateB("VorknkxDestroyed"))
	{
		if (Actor.GetHP(vorknkx) <= 0) 
		{
			Destroyed.vorknkx();
			SetGameStateB("VorknkxDestroyed", true);
		}			
		else if (!GetGameStateB("VorknkxShieldsDown"))
		{
			if (Actor.GetShd(vorknkx) <= 0) 
			{
				ShieldsDown.vorknkx();
				SetGameStateB("VorknkxShieldsDown", true);
			}
		}
	}


Spawn.vorknkx:
	float3 pos = Actor.GetGlobalPosition(gloryhangar);
	float3 rot = Actor.GetGlobalRotation(glory);
	rot = {rot[0] + 45, rot[1], rot[2]}; // point downwards out of hangar
	vorknkx = Actor.Spawn("CORV", "VORKNKX", "Traitors", "VORKNKX", 0, pos, rot, { "CriticalEnemies" });
	Common.MultiplyShields(vorknkx, 2);
	AI.QueueLast(vorknkx, "wait", 1.5);
	AI.QueueLast(vorknkx, "move", vorknkx_escape, 250);
	AI.QueueLast(vorknkx, "wait", 2.5);
	AI.QueueLast(vorknkx, "hyperspaceout");
	AI.QueueLast(vorknkx, "setgamestateb", "Escaped", true);
	AI.QueueLast(vorknkx, "delete");
	AddEvent(1, "Message1.vorknkx");
	AddEvent(6, "Minelayer.vorknkx");
	AddEvent(10, "Message2.vorknkx");
	AddEvent(17, "Spawn.AllyEpsilon");
	AddEvent(29, "Message3.vorknkx");


Message1.vorknkx:
	Common.MessageImportant("Warning: The traitor is trying to escape aboard the VORKNKX. Do not let him escape!", msg_warn);


Message2.vorknkx:
	Common.MessageStandard("GRAND ADM. THRAWN: Interesting. I see Zaarin has not yet activated VORKNKX's stealth generators.", msg_thrawn);


Message3.vorknkx:
	Common.MessageStandard("GRAND ADM. THRAWN: He may be able to escape if he leaves the range of the CORVUS. Destroy the VORKNKX.", msg_thrawn);


Escaped.vorknkx:
	triggerwinlose = true;
	Common.MessageCritical("MISSION FAILED: The traitor has escaped!", msg_fail);
	SetGameStateB("GameOver",true);
	AddEvent(3, "CurtainCall");
	Audio.Mood.PriObjFail();


Destroyed.vorknkx:
	Common.MessageImportant("M/CRV VORKNKX has been destroyed.", faction_empire_color);
	Audio.Mood.PriObjComplete();


ShieldsDown.vorknkx:
	Common.MessageImportant("M/CRV VORKNKX's shields are down, finish it!", faction_empire_color);

	
Spawn.Configure1:
    // Empire preps defense
	Faction.SetWingSpawnLimit("Empire", 20);
	Actor.SetProperty(corvus, "Spawner.Enabled", true);
	Actor.SetProperty(corvus, "Spawner.SpawnTypes", {"TIE","TIE","TIE","TIEI"});


Spawn.Configure2:
    // Traitors approach
	Faction.SetWingSpawnLimit("Traitors", 16);
	Actor.SetProperty(nebu, "Spawner.Enabled", true);
	Actor.SetProperty(nebu, "Spawner.SpawnTypes", {"TIE_S","TIEI_S"});
	AI.QueueFirst(ebolo, "move", {-3470, -100, -10350}, 50);
	AI.QueueFirst(daring, "move", {-3470, -300, -7350}, 50);
	
	
Spawn.TraitorShuttles:
 // spawn JV7
	int a = Actor.Spawn("T4A", "Z-SIGMA 1", "Traitors", "", 0, _d, _d);
	Actor.QueueAtSpawner(a, glory);
	Actor.SetProperty(a, "AI.HuntWeight", 0);
	Actor.SetProperty(a, "AI.CanEvade", false);
	Actor.SetProperty(a, "AI.CanRetaliate", false);
    AI.QueueLast(a, "move", shuttle1_escape, 300, false);
    AI.QueueLast(a, "move", shuttle1_escape, 300, false);
    AI.QueueLast(a, "hyperspaceout");
    AI.QueueLast(a, "delete");
    

	Audio.Mood.EnemyReinforce();
    Audio.QueueSpeech("13M8\13m8r8");
	Common.MessageStandard("New craft alert: SHU Z-SIGMA shuttles has been launched from ISD GLORY, attempting to escape.", faction_traitor_color);
	AddEvent(25, "Spawn.TraitorShuttles2");

 // glory stop
	AI.QueueLast(glory, "rotate", {3000, 360, -20000}, 0);
	AI.QueueLast(glory, "lock");


Spawn.TraitorShuttles2:
 // spawn 2nd JV7	
	int a = Actor.Spawn("T4A", "Z-SIGMA 2", "Traitors", "", 0, _d, _d);
	Actor.QueueAtSpawner(a, glory);
	Actor.SetProperty(a, "AI.HuntWeight", 0);
	Actor.SetProperty(a, "AI.CanEvade", false);
	Actor.SetProperty(a, "AI.CanRetaliate", false);
    AI.QueueLast(a, "move", shuttle2_escape, 300, false);
    AI.QueueLast(a, "move", shuttle2_escape, 300, false);
    AI.QueueLast(a, "hyperspaceout");
    AI.QueueLast(a, "delete");
    
    
Spawn.TraitorBombers:
	spawn_faction = "Traitors";
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_pos = { -2000, 250, -22500 };
	spawn_rot = { 0, 0 ,0 };
	spawn_type = "TIESA_S";
	spawn_target = corvusshd1;
	Script.Call("spawn1");
	
	spawn_pos = { -4500, 50, -16000 };
	spawn_target = corvusshd2;
	Script.Call("spawn2");
	
	spawn_pos = { -5000,-150,-24500 };
	spawn_target = corvus;
	Script.Call("spawn2");
	
	spawn_pos = { -25000, 300, -23500 };
	spawn_type = "TIEI_S";
	spawn_target = corvusshd1;
	Script.Call("spawn1");
	
	spawn_pos = { -24000, 200, 24500 };
	spawn_type = "TIE_S";
	spawn_target = corvus;
	Script.Call("spawn2");


Spawn.AllyEpsilon:
	for (int k = 0; k <= 1; k++)
	{
		for (int i = 1; i <= 4; i++)
		{
			if (i == 4)
			{
				int a = Actor.Spawn("TIEI", "EPSILON " + (k + 1), "Empire", "", 0, _d, _d);
				Actor.QueueAtSpawner(a, greywolf);
                AI.QueueLast(a, "attackactor", vorknkx, -1, -1, false, 40);
				if (k == 0)
				{
					AI.QueueLast(a, "setgamestateb", "EpsilonSpawned", true);
				}			
            }
			else
			{
				int a = Actor.Spawn("TIESA", "GAMMA " + (k * 3 + i), "Empire", "", 0, _d, _d);
				Actor.QueueAtSpawner(a, greywolf);
                AI.QueueLast(a, "attackactor", vorknkx, -1, -1, false, 40);
			}	
		}
	}
	

Spawn.AllyEta:
	for (int i = 1; i <= 4; i++)
	{
        // ETA 1-4
		int a = Actor.Spawn("TIE", "ETA " + i, "Empire", "", 0, _d, _d);
		Actor.QueueAtSpawner(a, greywolf);
		if (i == 1)
			AI.QueueLast(a, "setgamestateb", "EtaSpawned", true);
	}
	
	
Spawn.AllyBeta:
	for (int k = 0; k <= 1; k++)
	{
		for (int i = 1; i <= 4; i++)
		{
			if (i == 4)
			{
                // ETA 5-6 
				int a = Actor.Spawn("TIE", "ETA " + (k + 5), "Empire", "", 0, _d, _d);
				Actor.QueueAtSpawner(a, greywolf);
				AI.QueueLast(a, "attackactor", glory, -1, -1, false, 120);
			}
			else
			{
				int a = Actor.Spawn("TIESA", "BETA " + (k * 3 + i), "Empire", "", 0, _d, _d);
				Actor.QueueAtSpawner(a, greywolf);
				if (i == 1 && k == 0)
				{
					AI.QueueLast(a, "setgamestateb", "BetaSpawned", true);
				}
				AI.QueueLast(a, "attackactor", glory, -1, -1, false, 125);
			}	
		}
	}
    
	
Spawn.AllyUpsilon:
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_type = "GUN";
	spawn_name = "UPSILON";
  	spawn_formation = "VSHAPE";
	spawn_target = corv2;
	spawn_pos = { 2400,500,8000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn4");
	Audio.Mood.AllyReinforce();
	Common.MessageStandard("GUN UPSILON squadron has arrived to reinforce INT CORVUS.", faction_empire_color);


Spawn.AllyIota:
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	spawn_dmgmod = 1;
	spawn_wait = 0;
	spawn_type = "TIEA";
	spawn_name = "IOTA";
  	spawn_formation = "VSHAPE";
	spawn_target = corv4;
	spawn_pos = { 2400,500,8000 };
	spawn_rot = { 0, 180 ,0 };
	Script.Call("spawn4");
	Audio.Mood.AllyReinforce();
	Common.MessageStandard("Additional T/A IOTA squadron has arrived to reinforce INT CORVUS.", faction_empire_color);


Spawn.TraitorZDelta:
	for (int i = 1; i <= 6; i++)
	{
		int a = Actor.Spawn("TIED", "Z-DELTA " + i, "Traitors", "", 0, _d, _d);
        Actor.CallOnDead(a, "OneDown.tieD");
		if (i <= 4)
			AI.QueueLast(a, "attackactor", daring, -1, -1, false);
		else
			AI.QueueLast(a, "attackactor", corvus, -1, -1, false);
		
		Actor.QueueAtSpawner(a, glory);
	}
	
	for (int i = 1; i <= 2; i++)
	{
		int a = Actor.Spawn("TIESA_S", "", "Traitors", "", 0, _d, _d);
		AI.QueueLast(a, "attackactor", corvus, -1, -1, false, 40);
		Actor.QueueAtSpawner(a, glory);
	}
	Audio.Mood.EnemyReinforce();
	Common.MessageStandard("New craft alert: T/D Z-DELTA squadron has been launched from ISD GLORY.", faction_traitor_color);

	
Spawn.TraitorZIota:
	
	for (int k = 0; k <= 2; k++)
	{
		for (int i = 1; i <= 2; i++)
		{
			int a = Actor.Spawn("TIED", "Z-IOTA " + (k * 2 + i), "Traitors", "", 0, _d, _d);
            Actor.CallOnDead(a, "OneDown.tieD");
			Actor.QueueAtSpawner(a, glory);
		}
		
		for (int i = 1; i <= 2; i++)
		{
            int a = -1;
			if (k == 2)
				a = Actor.Spawn("TIEI_S", "", "Traitors", "", 0, _d, _d);
			else
				a = Actor.Spawn("TIESA_S", "", "Traitors", "", 0, _d, _d);
			AI.QueueLast(a, "attackactor", greywolf, -1, -1, false, 30);
			Actor.QueueAtSpawner(a, glory);
		}
	}
	Audio.Mood.EnemyReinforce();
	Common.MessageStandard("New craft alert: T/D Z-IOTA squadron has been launched from ISD GLORY.", faction_traitor_color);


Spawn.TraitorZTheta:
	for (int k = 0; k <= 2; k++)
	{
		for (int i = 1; i <= 2; i++)
		{
			int a = Actor.Spawn("TIED", "Z-THETA " + (k * 2 + i), "Traitors", "", 0, _d, _d);
            Actor.CallOnDead(a, "OneDown.tieD");
			Actor.QueueAtSpawner(a, glory);
			if (i == 2)
				Squad.JoinSquad(a, glory);
		}
		
		for (int i = 1; i <= 2; i++)
		{
			int a = Actor.Spawn("TIEI", "", "Traitors", "", 0, _d, _d);
			Actor.QueueAtSpawner(a, glory);
		}
	}
	Audio.Mood.EnemyReinforce();
    Audio.QueueSpeech("13M8\13m8r15");
	Common.MessageStandard("New craft alert: T/D Z-THETA squadron has been launched from ISD GLORY.", faction_traitor_color);



Minelayer.corv1:
	if (Actor.IsAlive(corv1) && corv1_mines > 0)
	{
		float3 pos = Actor.GetGlobalPosition(corv1);
		float3 fac = Actor.GetGlobalDirection(corv1);
		pos -= fac * 57;
		pos -= {Random(-20, 20), 55, Random(-20, 20)};
		float3 rot = { Random(360), Random(360), Random(360)};
		
		Actor.Spawn("MINE2", "", "Traitors", "", 0, pos, rot);
		corv1_mines -= 1;
		AddEvent(5 + 25 * Random(), "Minelayer.corv1");
	}


Minelayer.corv2:
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
		AddEvent(5 + 10 * Random(), "Minelayer.corv2");
	}


Minelayer.corv3:
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
		AddEvent(5 + 10 * Random(), "Minelayer.corv3");
	}


Minelayer.corv4:
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
		AddEvent(5 + 10 * Random(), "Minelayer.corv4");
	}

Minelayer.corv5:
	if (Actor.IsAlive(corv5) && corv5_mines > 0)
	{
		float3 pos = Actor.GetGlobalPosition(corv5);
		float3 fac = Actor.GetGlobalDirection(corv5);
		pos -= fac * 57;
		pos -= {Random(-20, 20), 55, Random(-20, 20)};
		float3 rot = { Random(360), Random(360), Random(360)};
		
		string type = (corv5_mines == 2 || corv5_mines == 6) ? "MINE3" : "MINE2";
		Actor.Spawn(type, "", "Traitors", "", 0, pos, rot);
		corv5_mines -= 1;
		AddEvent(5 + 25 * Random(), "Minelayer.corv5");
	}

Minelayer.vorknkx:
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
		AddEvent(1.5 + 4.5 * Random(), "Minelayer.vorknkx");
	}

	
Check.flightGroups:
    // spawned flight groups
    // states are triggered by flight leaders
	if (GetGameStateB("EtaSpawned")) 
	{
		SetGameStateB("EtaSpawned", false);
		Message.Eta();
		Audio.Mood.AllyReinforce();
	}
		
	if (GetGameStateB("BetaSpawned"))
	{
		SetGameStateB("BetaSpawned", false);
		Message.Beta();
		Audio.Mood.AllyReinforce();
	}
    
    if (GetGameStateB("EpsilonSpawned")) // Epsilon & Gamma
    {
        SetGameStateB("EpsilonSpawned", false);
		Message.Epsilon();
        Audio.Mood.AllyReinforce();
    }


Message.Eta:
	Common.MessageStandard("T/F ETA squadron has been launched from ISD GREY WOLF.", faction_empire_color);

	
Message.Beta:
	Common.MessageStandard("T/B BETA squadron has been launched from ISD GREY WOLF, targeting ISD GLORY.", faction_empire_color);
	AddEvent(4, "Message2.Beta");


Message2.Beta:
    Audio.QueueSpeech("13M8\13m8r1");
	Common.MessageStandard("T/B BETA 1: Heads up, BETA group engaging ISD GLORY.", faction_empire_color);


Message.Epsilon:
	Common.MessageStandard("T/B EPSILON squadron has been launched from ISD GREY WOLF, targeting M/CRV VORKNKX.", faction_empire_color);
	AddEvent(3, "Message.Gamma");


Message.Gamma:
	Common.MessageStandard("T/I GAMMA squadron has been launched from ISD GREY WOLF, targeting M/CRV VORKNKX.", faction_empire_color);


// Mission dialogue
Message.Brief01:
	Common.MessageStandard("INT CORVUS: This is Interdictor cruiser CORVUS. Interdiction field has been activated.", msg_corvus);


Message.Brief02:
	Common.MessageStandard("INT CORVUS: Enemy bombers en-route. Clear them out before they deal damage.", msg_corvus);

	
Message.Brief03:
	Common.MessageStandard("GRAND ADM. THRAWN: Strike Cruisers Ebolo and Daring will engage the enemy cruiser screen.", msg_thrawn);


Message.Brief04:
	Common.MessageStandard("GRAND ADM. THRAWN: The traitors will attempt to destroy the Interdictor CORVUS to facilitate their escape.", msg_thrawn);

	
Message.Brief05:
	Common.MessageStandard("GRAND ADM. THRAWN: ALPHA squadron, protect the Interdictor CORVUS from enemy attack.", msg_thrawn);

	
Message.Brief06:
	Common.MessageStandard("GRAND ADM. THRAWN: Then destroy the cowering traitor in his Star Destroyer.", msg_thrawn);


// Interjection
OneDown.tieD:
    tieDdowncount = tieDdowncount + 1;
    if (tieDdowncount == 1)
    {
        Audio.QueueSpeech("13M8\13m8r6");
    }

// Mission progression
Check.win:
    if (Faction.GetShipCount("Traitors") < 1 && !triggerwinlose) 
        Commit.win();


// Congrats
Commit.win:
	triggerwinlose = true;
    Message1.win();
	AddEvent(4, "Message2.win");
	SetGameStateB("GameWon", true);
	AddEvent(10, "CurtainCall");


Message1.win:
    Audio.QueueSpeech("13M8\13m8w1");
	Common.MessageImportant("GRAND ADM. THRAWN: The traitor Zaarin has been dealt with.", msg_thrawn);


Message2.win:
	Common.MessageImportant("GRAND ADM. THRAWN: Such is the fate of the enemies of the Empire.", msg_thrawn);


// End the mission
CurtainCall:
	Scene.FadeOut();