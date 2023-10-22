// Globals

bool combat;
bool respawn;
bool triggerwinlose;
bool primary_completed;
bool scutz_arrived;
bool hammer_arrived;

float msg_time = 32;
float hammer_time = 240;

int alpha1;
int outpost;
string outpost_sidebarname;
int hammer;
int hammer_hangar;
int alpha2;
int alpha3;
int ubote1;
int ubote2;
int ubote3;

int[] tie_beta_group;
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

int wing_reb;
int wing_emp;

float3 _d = { 0, 0, 0 };
int _a = -1;
float _f = 0;

// message colors
float3 msg_outpost = { 0.6, 0.8, 1 };
float3 msg_delta = { 0.2, 0.6, 0.1 };
float3 msg_warn = { 0.8, 0.8, 0 };
float3 msg_bad = { 1, 0.3, 0.2 };
float3 msg_fail = { 0.8, 0, 0 };


Load:
    Scene.SetMaxBounds({15000, 4500, 20000});
    Scene.SetMinBounds({-15000, -4500, -15000});
    Scene.SetMaxAIBounds({15000, 4500, 20000});
    Scene.SetMinAIBounds({-15000, -4500, -15000});

    Player.SetLives(5);
    Score.SetScorePerLife(1000000);
    Score.SetScoreForNextLife(1000000);
    Score.Reset();

    UI.SetLine1Color(faction_empire_hammer_color);
    UI.SetLine2Color(faction_empire_color);
    UI.SetLine3Color(faction_rebel_color);
    
    Script.Call("spawn_reset");
    Script.Call("actorp_reset");
    
    InitMusic(); 
    InitShips();
    InitFighters();
    MakePlayer();
    AddEvent(1.5, "GivePlayerControl"); 

    AddEvent(3, "Spawn.EnemyYWing1");
    AddEvent(7, "Message.SectorAlert");
    AddEvent(10, "Message.AddressBetaGamma");
    AddEvent(14, "Message.AddressAlpha");
    AddEvent(17.5, "Message.BetaRespond");
    AddEvent(20, "Spawn.EnemyYWing2");
    AddEvent(28, "Message.AttackYWings");
    AddEvent(31, "Message.GammaRespond");
    AddEvent(msg_time, "Message.PriObj");
    AddEvent(50, "Spawn.EnemyXWing1");
    AddEvent(67, "Spawn.EnemyXWing2");
    AddEvent(71, "Spawn.AllyEta");
    AddEvent(115, "Spawn.Scutz");
    AddEvent(90, "Spawn.EnemyYWing3");
    AddEvent(130, "Spawn.EnemyYWing4");
    AddEvent(180, "Spawn.EnemyUbote");

    if (GetDifficulty() == "hard")
        AddEvent(195, "Spawn.EnemyXWing3");

    AddEvent(hammer_time, "Spawn.AllyHammer");


LoadFaction:
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


InitMusic:
    Audio.Mood.Ambient2();
    Audio.SetMusicDyn("TRO-IN");

    
MakePlayer:
    Player.DecreaseLives();
    if (respawn) 
    {
        Player.RequestSpawn(); 
        AddEvent(0.5, "PrepPlayer");
    }
    else 
    {
        Player.AssignActor(Actor.Spawn(GetPlayerActorType(), GetPlayerName(), "Empire", "", 0, { 1200, 500, -3800 }, { 0, 0, 0 }));
        Player.SetMovementEnabled(false);
        Actor.SetProperty(Player.GetActor(), "Movement.Speed", 250);
        PrepPlayer();
        respawn = true;
    }


PrepPlayer:
    alpha1 = Player.GetActor();
    if (alpha1 != -1)
    {
        Actor.CallOnRegisterKill(alpha1, "Speech.ScoreKillStandardLeader");
        ReformPlayerSquad();
    }


GivePlayerControl:
	Player.SetMovementEnabled(true);

    
ReformPlayerSquad:
    Squad.JoinSquad(alpha1, alpha2);
    Squad.JoinSquad(alpha1, alpha3);


InitShips:
    // Empire
    outpost = Actor.Spawn("XQ1", "D-34", "Empire", "PLT/1 D-34", 0, { 0, 0, 0 }, { 0, -20, 0 }, { "CriticalAllies" });
    outpost_sidebarname = "PLT/1 D-34";
    Actor.SetProperty(outpost, "Spawner.Enabled", true);
    Actor.SetProperty(outpost, "Spawner.SpawnTypes", {"TIE"});
    Speech.RegisterCriticalInstallation(outpost);
    AI.QueueLast(outpost, "lock");

    Actor.Spawn("MINE1", "", "Empire", "", 0, { 2200, 150, 2600 }, { Random(360), Random(360), Random(360) });
    Actor.Spawn("MINE2", "", "Empire", "", 0, { -2100, -200, 2600 }, { Random(360), Random(360), Random(360) });
    Actor.Spawn("MINE2", "", "Empire", "", 0, { 3400, -100, 3400 }, { Random(360), Random(360), Random(360) });
    Actor.Spawn("MINE1", "", "Empire", "", 0, { -3200, 50, 3400 }, { Random(360), Random(360), Random(360) });
    Actor.Spawn("MINE3", "", "Empire", "", 0, { -400, 850, -1400 }, { Random(360), Random(360), Random(360) });

    
InitFighters:
    // Empire
    alpha2 = Actor.Spawn("TIE", "ALPHA 2", "Empire", "", 0, { 600, 400, -4100 }, { 0, 5, 0 });
    Actor.CallOnRegisterKill(alpha2, "Speech.ScoreKillStandardSquadmate");

    alpha3 = Actor.Spawn("TIE", "ALPHA 3", "Empire", "", 0, { 1800, 350, -4000 }, { 0, -5, 0 });
    Actor.CallOnRegisterKill(alpha3, "Speech.ScoreKillStandardSquadmate");
        
    spawn_faction = "Empire";
    spawn_hyperspace = true;
    spawn_wait = 5;
    spawn_type = "TIE";
    spawn_name = "BETA";
    spawn_spacing = 500;
    spawn_pos = { 4500, -300, 1500 };
    spawn_rot = { 20, -120 ,0 };
    Script.Call("spawn3");
    tie_beta_group = spawn_ids;
    Actor.CallOnRegisterKill(tie_beta_group[0], "ScoreKill.beta1");
    Actor.CallOnRegisterKill(tie_beta_group[1], "ScoreKill.beta");
    Actor.CallOnRegisterKill(tie_beta_group[2], "ScoreKill.beta");
    
    spawn_wait = 8;
    spawn_name = "GAMMA";
    spawn_spacing = 500;
    spawn_pos = { -6500, -200, -2500 };
    spawn_rot = { -40, 120 ,0 };
    Script.Call("spawn3");
    tie_gamma_group = spawn_ids;
    Actor.CallOnRegisterKill(tie_gamma_group[0], "ScoreKill.gamma1");
    Actor.CallOnRegisterKill(tie_gamma_group[1], "ScoreKill.gamma");
    Actor.CallOnRegisterKill(tie_gamma_group[2], "ScoreKill.gamma");


Tick:
    // Set UI
    wing_reb = Faction.GetWingCount("Rebels") + Faction.GetShipCount("Rebels") + Faction.GetWingCount("Mugaari");
    wing_emp = Faction.GetWingCount("Empire") + Faction.GetWingCount("Empire_Hammer");
    float tm = hammer_time - GetGameTime();
    
    if (tm >= 0 && GetGameTime() > msg_time)
    {
        UI.SetLine1Color(faction_empire_hammer_color);
        UI.SetLine2Color(faction_empire_color);
        UI.SetLine3Color(faction_rebel_color);
        UI.SetLine1Text("HAMMER: " + Math.FormatAsTime(tm));
        UI.SetLine2Text("WINGS: " + wing_emp);
        UI.SetLine3Text((wing_reb == 0) ? "" : "ENEMY: " + wing_reb);   
    }
    else
    {
        UI.SetLine1Color(faction_empire_color);
        UI.SetLine2Color(faction_rebel_color);
        UI.SetLine1Text("WINGS: " + wing_emp);
        UI.SetLine2Text((wing_reb == 0) ? "" : "ENEMY: " + wing_reb);
        UI.SetLine3Text("");
    }
    
    if (!triggerwinlose)
    {
        // Music Ambience
        if (combat && (wing_reb > 0) && Audio.GetMood() != 4)
            Audio.Mood.Combat();
        else if (combat && (wing_reb == 0) && Audio.GetMood() != 0 && Audio.GetMood() != 5)
            Audio.Mood.Ambient0();
    
        Check.d34();
        Check.hammer();
        Check.scutz();
        Check.ties();
    }


ScoreKill.beta1(int actor, int victim):
    Speech.ScoreKillStandardLeaderOther(actor, victim, "a-beta", "a-one");
    
    
ScoreKill.beta(int actor, int victim):
    Speech.ScoreKillStandardSquadmate(actor, victim, "a-beta", "a-one");


ScoreKill.gamma1(int actor, int victim):
    Speech.ScoreKillStandardLeaderOther(actor, victim, "a-gamma", "a-one");


ScoreKill.gamma(int actor, int victim):
    Speech.ScoreKillStandardSquadmate(actor, victim, "a-gamma", "a-one");
    

Check.d34:
    if (!GetGameStateB("OutpostDestroyed"))
    {
        float hpfrac = Actor.GetHPFrac(outpost);
        float hullfrac = Actor.GetHullFrac(outpost);
        float shdfrac = Actor.GetShdFrac(outpost);
        if (hpfrac <= 0) 
        {
            SetGameStateB("OutpostDestroyed", true);
            Lose.d34();
        }
        
        if (!GetGameStateB("OutpostWeakened"))
        {
            if (shdfrac <= 0.5) 
            {
                SetGameStateB("OutpostWeakened", true);
                Warn.d34();
            }
        }
        
        if (!GetGameStateB("OutpostShieldsDown"))
        {
            if (shdfrac <= 0) 
            {
                SetGameStateB("OutpostShieldsDown", true);
                Warn.d34_shd();
            }
        }
        
        if (!GetGameStateB("OutpostDanger"))
        {
            if (hullfrac <= 0.5) 
            {
                SetGameStateB("OutpostDanger", true);
                Warn.d34_critical();
            }
        }
    }


Warn.d34:
    Common.MessageCritical("WARNING: PLT/1 D-34 is under attack!", msg_warn);
    Audio.QueueSpeech("1M2\1m2r5");


Warn.d34_shd:
    Audio.QueueSpeech("a-critic", "a-instal", "a-shield");


Warn.d34_critical:
    Audio.QueueSpeech("a-critic", "a-instal", "a-hull");
    Common.MessageFlashDanger("WARNING: PLT/1 D-34 is under heavy fire!");


Lose.d34:
    triggerwinlose = true;
	Common.MessageCritical("MISSION FAILED: PLT/1 D-34 has been lost.", msg_fail);
    SetGameStateB("GameOver",true);
    AddEvent(3, "CurtainCall");
    Audio.StopSpeech();
    Audio.QueueSpeech("1M2\1m2r6");
	Audio.Mood.PriObjFail();


Check.hammer:
    if (hammer_arrived && !GetGameStateB("HammerWarn"))
    {
        float shdfrac = Actor.GetShdFrac(hammer);
        if (shdfrac <= 0.75) 
        {
            SetGameStateB("HammerWarn", true);
            Warn.hammer();
        }
    }
    
    if (hammer_arrived && !GetGameStateB("HammerAngry"))
    {
        float shdfrac = Actor.GetShdFrac(hammer);
        if (shdfrac <= 0.5) 
        {
            SetGameStateB("HammerAngry", true);
            Lose.hammer();
        }
    }
    
    if (!GetGameStateB("PriComplete"))
    {
        if (hammer_arrived)
        {
            AddEvent(10, "Sequence.primaryComplete");
            SetGameStateB("PriComplete", true);
            Audio.Mood.PriObjComplete();
        }
    }

    if (primary_completed)
    {
        if (Math.GetActorDistance(alpha1, hammer_hangar) < 400)
        {
            Commit.win();
        }
    }


Prep.hammer:
    int[] list = Actor.GetChildrenByType(hammer, "HANGAR");
    hammer_hangar = list[0];


Message1.hammer:
	Common.MessageImportant("ISD HAMMER: The ISD HAMMER has arrived to take command of the battle.", msg_outpost);
    Audio.QueueSpeech("1M2\1m2w1");

    
Message2.hammer:
	Common.MessageImportant("ISD HAMMER: Good work. You have fought well in defending D-34 Outpost.", msg_outpost);


Warn.hammer:
    Actor.SetFaction(alpha1, "Traitor");
	Common.MessageCritical("ISD HAMMER: What are you doing, pilot?", msg_warn);


Lose.hammer:
    triggerwinlose = true;
	Common.MessageCritical("MISSION FAILED: The ISD HAMMER had enough of your antics. You will be dealt with switfly.", msg_fail);
    Actor.SetFaction(alpha1, "Traitor");
    SetGameStateB("GameOver", true);
    AddEvent(3, "CurtainCall");
	Audio.Mood.PriObjFail();


Check.scutz:
    if (!GetGameStateB("ScutzInspected"))
    {

    }
    else
    {
        if (!GetGameStateB("ScutzDisabled"))
        {
            float shd = Actor.GetShd(scutz);
            if (shd <= 0) 
            {
                //float3 pos = Actor.GetGlobalPosition(scutz);
                //float3 fac = Actor.GetGlobalDirection(scutz);
                //pos += fac * 5000;
                AI.ForceClearQueue(scutz);
                //AI.QueueLast(scutz, "rotate", pos, 0, 1, false);
                AI.QueueLast(scutz, "lock");
                Actor.DisableSubsystem(scutz, "ENGINE");
                Actor.DisableSubsystem(scutz, "SIDE_THRUSTERS");
                Actor.DisableSubsystem(scutz, "SHIELD_GENERATOR");

                Actor.SetFaction(scutz, "Neutral_Inspect");
                //Actor.SetProperty(scutz, "Regen.Self", 0);
	            Common.MessageStandard("MU 1: This is MU 1. Target disabled.", faction_empire_hammer_color);
                Audio.QueueSpeech("a-this", "a-mu", "a-one", "a-target", "a-disabl");
                SetGameStateB("ScutzDisabled", true);
            }
        }
    }
    
    if (GetGameStateB("MuDispatched") && GetGameStateB("ScutzDisabled"))
    {
        if (!GetGameStateB("MuBoarding"))
        {
            if (Math.GetActorDistance(mu, scutz) < 400)
            {
                SetGameStateB("MuBoarding", true);
                Boarding.mu();
            }
        }
    }
    
    if (GetGameStateB("RebelsCaptured") && !GetGameStateB("RebelsCaptureAnnounced"))
    {
        SetGameStateB("RebelsCaptureAnnounced", true);
        Message1.rebelscaptured();
        AddEvent(4, "Message2.rebelscaptured");
    }


Inspected.scutz:
    //if (Math.GetActorDistance(alpha1, scutz) < 200)
    //{
        SetGameStateB("ScutzInspected", true);
        Common.MessageStandard("Shuttle SHUTZ 1 has been identified to be carrying Rebel officers.", msg_warn);
        
        Actor.SetFaction(scutz, "Neutral_Rebel");
        AddEvent(6, "Message1.scutz");
        AddEvent(9, "Spawn.AllyMu");
        AddEvent(10, "Message2.scutz");
        Audio.Mood.SecObjComplete();
    //}

Message1.scutz:
	Common.MessageStandard("PLT/1 D-34: We have located the shuttle with the rebel officers fleeing from Hoth.", faction_empire_hammer_color);
    Audio.QueueSpeech("1M2\1m2w2");


Message2.scutz:
	Common.MessageStandard("PLT/1 D-34: Reinforcements will arrive shortly to disable and capture the shuttle.", faction_empire_hammer_color);


Boarding.mu:
	Common.MessageStandard("MU 1: Beginning boarding operation.", faction_empire_hammer_color);
    Audio.QueueSpeech("a-this", "a-mu", "a-one", "a-commen", "a-board", "a-missio");
    AddEvent(60, "BoardingComplete.mu");
    float3 pos = Actor.GetGlobalPosition(scutz);
    float3 fac = Actor.GetGlobalDirection(scutz);
    float3 pos1 = pos + fac * 1000;
    AI.ForceClearQueue(mu);
    AI.QueueLast(mu, "move", pos, 100, 600, false);
    AI.QueueLast(mu, "move", pos, 25, 200, false);
    AI.QueueLast(mu, "rotate", pos1, 0, 1, false);
    AI.QueueLast(mu, "lock");


BoardingComplete.mu:
    if (!triggerwinlose && Actor.IsAlive(scutz))
    {
        SetGameStateB("MuBoardingComplete", true);
        Common.MessageStandard("MU 1: Boarding operation complete. Proceeding to enter hyperspace.", faction_empire_hammer_color);
        Audio.QueueSpeech("a-this", "a-mu", "a-one", "a-board", "a-compl");
        AI.ForceClearQueue(mu);
        AI.QueueLast(mu, "rotate", {2300, 100, 5000}, 200, 1, false);
        AI.QueueLast(mu, "wait", 3);
        AI.QueueLast(mu, "hyperspaceout");
        AI.QueueLast(mu, "setgamestateb", "MuDispatched", false);
        AI.QueueLast(mu, "setgamestateb", "RebelsCaptured", true);
        AI.QueueLast(mu, "delete");
    }


Message1.rebelscaptured:
	Common.MessageStandard("PLT/1 D-34: The rebel offices have been taken into Empire custody.", msg_outpost);


Message2.rebelscaptured:
	Common.MessageStandard("PLT/1 D-34: The shuttle SCUTZ may now be destroyed.", msg_outpost);


Check.ties:
    if (!GetGameStateB("ZetaSpawned"))
    {
        if (wing_emp < 5)
        {
            SetGameStateB("ZetaSpawned", true);
            Spawn.AllyZeta();
        }
    }
    else
    {
        if (!GetGameStateB("LambdaSpawned"))
        {
            if (wing_emp < 6)
            {
                SetGameStateB("LambdaSpawned", true);
                Spawn.AllyLambda();
            }
        }
    }



// Congrats
Commit.win:
    triggerwinlose = true;
    SetGameStateB("GameWon",true);
    Message.Docking();
    AddEvent(1, "CurtainCall");
    AddEvent(0.001, "Player.slow");


Player.slow:
	if (Actor.IsAlive(alpha1))
	{
        Actor.DisableSubsystem(alpha1, "ENGINE");
        Actor.SetProperty(alpha1, "Movement.Speed", Actor.GetProperty(alpha1, "Movement.Speed") - GetLastFrameTime() * 100);
        AddEvent(0.01, "Player.slow");
	}    


Spawn.AllyEta:
    for (int i = 0; i < 4; i += 1)
    {   
        _a = Actor.Spawn("TIE", "ETA " + (i + 1), "Empire", "", 0, _d, _d);
        Actor.QueueAtSpawner(_a, outpost);
    }

    Audio.Mood.AllyReinforce();
	Common.MessageStandard("PLT/1 D-34: We are dispatching additional TIEs to engage the threats.", msg_outpost);
    
    
Spawn.AllyZeta:
    for (int i = 0; i < 4; i += 1)
    {   
        _a = Actor.Spawn("TIE", "ZETA " + (i + 1), "Empire", "", 0, _d, _d);
        Actor.QueueAtSpawner(_a, outpost);
    }

    Audio.Mood.AllyReinforce();
	Common.MessageStandard("PLT/1 D-34: We are dispatching additional TIEs to engage the threats.", msg_outpost);


Spawn.AllyLambda:
    for (int i = 0; i < 4; i += 1)
    {   
        _a = Actor.Spawn("TIE", "LAMBDA " + (i + 1), "Empire", "", 0, _d, _d);
        Actor.QueueAtSpawner(_a, outpost);
    }

    Audio.Mood.AllyReinforce();
	Common.MessageStandard("PLT/1 D-34: We are dispatching additional TIEs to engage the threats.", msg_outpost);


Spawn.AllyMu:
    mu = Actor.Spawn("GUN", "MU 1", "Empire_Mu", "", 0, {15000,0,-100000}, {0,0,0});

    Actor.SetProperty(mu, "Movement.MinSpeed", 0);
    Actor.SetProperty(mu, "AI.CanEvade", false);
    Actor.SetProperty(mu, "AI.CanRetaliate", false);

    AI.QueueLast(mu, "hyperspacein", { 3600, 500, 7600 });
    AI.QueueLast(mu, "setgamestateb", "MuDispatched", true);
    AI.QueueLast(mu, "attackactor", scutz, 600, 200, false, 30);
    AI.QueueLast(mu, "followactor", scutz, 400, false);
    
    AI.ForceClearQueue(scutz);
    AI.QueueLast(scutz, "move", { -6000, -200, -8000 }, 500, 500, false);
    AI.QueueLast(scutz, "hyperspaceout");
    AI.QueueLast(scutz, "setgamestateb", "ScutzEscaped", true);
    AI.QueueLast(scutz, "delete");


Spawn.EnemyYWing1:
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
    
    Audio.Mood.NeutralReinforce();
    Common.MessageNewCraftAlertTarget(spawn_faction, "Y-WING", spawn_name, outpost_sidebarname, faction_mugaari_color);
    Audio.Mood.Combat();
    combat = true;


Spawn.EnemyYWing2:
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

    Audio.Mood.EnemyReinforce();
    Common.MessageNewCraftAlertTarget(spawn_faction, "Y-WING", spawn_name, outpost_sidebarname, faction_rebel_color);


Spawn.EnemyYWing3:
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
    
    Audio.Mood.NeutralReinforce();
    Common.MessageNewCraftAlertTarget(spawn_faction, "Y-WING", spawn_name, outpost_sidebarname, faction_mugaari_color);
    
    
Spawn.EnemyXWing1:
    spawn_faction = "Mugaari";
    spawn_hyperspace = true;
    spawn_wait = 6;
    spawn_type = "Z95";
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
    
    Audio.Mood.NeutralReinforce();
    Common.MessageNewCraftAlertTarget(spawn_faction, "Z-95", spawn_name, outpost_sidebarname, faction_mugaari_color);
    
    
Spawn.EnemyXWing2:
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
    
    Audio.Mood.EnemyReinforce();
    Common.MessageNewCraftAlertTarget(spawn_faction, (GetDifficulty() != "easy") ? "X-WING" : "Z-95", spawn_name, "TIE-Fighters", faction_rebel_color);


Spawn.EnemyXWing3:
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
    
    Audio.Mood.EnemyReinforce();
    Common.MessageNewCraftAlertTarget(spawn_faction, "X-WING", spawn_name, "TIE-Fighters", faction_rebel_color);


Spawn.EnemyYWing4:
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
    
    Audio.Mood.EnemyReinforce();
    Common.MessageNewCraftAlertTarget(spawn_faction, "Y-WING", spawn_name, outpost_sidebarname, faction_rebel_color);


Spawn.Scutz:
    spawn_faction = "Neutral_Inspect";
    spawn_hyperspace = true;
    spawn_wait = 3;
    spawn_type = "T4A";
    spawn_name = "SCUTZ 1";
    spawn_target = -1;
    spawn_pos = { 0, -200, 19000 };
    spawn_rot = { 0, 180 ,0 };
    Script.Call("spawn1");
    Audio.Mood.NeutralReinforce();
	Common.MessageStandard("New craft alert: An unidentified shuttle has entered the area.", faction_neutral_rebel_color);

    AddEvent(0.5, "spawn_removechildren");
    foreach (int a in spawn_ids)
    {
        Actor.SetProperty(a, "Movement.MinSpeed", 0);
        Actor.SetArmor(a, "MISSILE", 0.1);
        Common.SetHull(a, 40);
        
        AI.QueueLast(a, "move", { -9000, -200, 3000 }, 250, 500, false);
        AI.QueueLast(a, "move", { -6000, -200, -8000 }, 250, 500, false);
        AI.QueueLast(a, "hyperspaceout");
        AI.QueueLast(a, "setgamestateb", "ScutzEscaped", true);
        AI.QueueLast(a, "delete");

        Actor.CallOnCargoScanned(a, "Inspected.scutz");
        Actor.SetProperty(a, "Cargo", "OFFICERS");
    }
    scutz = spawn_ids[0];
    scutz_arrived = true;


Spawn.EnemyUbote:
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
    
    Audio.Mood.EnemyBigReinforce();
	Common.MessageStandard("New craft alert: Rebel CORVETTE UBOTE group entering the area. Proceed with caution!", faction_rebel_color);


Spawn.AllyHammer:
    hammer_arrived = true;
    hammer = Actor.Spawn("ISD", "HAMMER", "Empire_Hammer", "", 0, {10000,0,-100000}, {0,0,0}, {"CriticalAllies"});
    
    Actor.SetProperty(hammer, "Spawner.Enabled", true);
    Actor.SetProperty(hammer, "Spawner.SpawnTypes", {"TIE"});
    AI.QueueLast(hammer, "hyperspacein", { 5600, 700, 4600 });
    AI.QueueLast(hammer, "rotate", {0, 600, 13000}, 0);
    AI.QueueLast(hammer, "lock");
    Audio.Mood.AllyBigReinforce();
    Message1.hammer();
    AddEvent(0.1, "Prep.hammer");
    AddEvent(4, "Message2.hammer");
    AddEvent(15, "Retreat.rebelShips");
    AddEvent(18, "Retreat.rebelWings");


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


Retreat.rebelShips:
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


Retreat.rebelWings:
    int[] rebelwings = Faction.GetWings("Rebels");
    foreach (int a in rebelwings)
    {
        Actor.SetProperty(a, "AI.CanEvade", false);
        Actor.SetProperty(a, "AI.CanRetaliate", false);
        AI.ForceClearQueue(a);
        AI.QueueLast(a, "rotate", { -1600, 0, 13600 }, 50, false);
        AI.QueueLast(a, "hyperspaceout");
        AI.QueueLast(a, "delete");
    }


Message.SectorAlert:
	Common.MessageStandard("PLT\1 D-34: Sector alert! There are unknown craft entering our area.", msg_outpost);
    Audio.StopSpeech();
    Audio.QueueSpeech("1M2\1m2r1");


Message.AddressBetaGamma:
	Common.MessageStandard("PLT\1 D-34: BETA and GAMMA squadrons, engage enemy forces.", msg_outpost);
    Audio.QueueSpeech("1M2\1m2r2");


Message.BetaRespond:
    Audio.QueueSpeech("a-this", "a-beta", "a-one", "a-onway");


Message.AddressAlpha:
	Common.MessageStandard("PLT\1 D-34: ALPHA squadron, join the attack and engage the enemy.", msg_outpost);
    Audio.StopSpeech();
    Audio.QueueSpeech("1M2\1m2r3");


Message.AttackYWings:
	Common.MessageStandard("PLT\1 D-34: Stop all Y-WINGs from making torpedo runs on D-34.", msg_outpost);
    Audio.QueueSpeech("1M2\1m2r4");


Message.GammaRespond:
    Audio.QueueSpeech("a-this", "a-gamma", "a-one", "a-proc");


Message.PriObj:
	Common.MessageStandard("PLT/1 D-34: Hold all attacks until reinforcements arrive to relieve defenses.", msg_outpost);



Sequence.primaryComplete:
    AddEvent(3, "Message.PriObjCompleted");
    AddEvent(7, "Message.GotoHangar");


Message.PriObjCompleted:
	Common.MessageStandard("ISD HAMMER: The ISD HAMMER has arrived to recover station defense forces.", msg_outpost);
    Audio.StopSpeech();
    Audio.QueueSpeech("1M2\1m2r7");


Message.GotoHangar:
    primary_completed = true;
	Common.MessageStandard("ISD HAMMER: ALPHA 1, return to the HAMMER's hangar immediately.", faction_empire_hammer_color);
    Audio.QueueSpeech("1M2\1m2r8");


Message.Docking:
	Common.MessageStandard("Docking...", faction_empire_color);


// End the mission
CurtainCall:
	Scene.FadeOut();