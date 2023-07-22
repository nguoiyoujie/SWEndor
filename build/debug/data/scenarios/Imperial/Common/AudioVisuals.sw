// Global controls
float Speech_message_critical_next = 0;
float Speech_message_critical_delay = 60;


Audio.Mood.Ambient0:
	Audio.SetMood(0);


Audio.Mood.Ambient1:
	Audio.SetMood(1);
    

Audio.Mood.Ambient2:
	Audio.SetMood(2);
    
    
Audio.Mood.Ambient3:
	Audio.SetMood(3);


Audio.Mood.Combat:
	Audio.SetMood(4);


Audio.Mood.AllyReinforce:
	Audio.SetMood(-11);


Audio.Mood.AllyBigReinforce:
	Audio.SetMood(-12);


Audio.Mood.EnemyReinforce:
	Audio.SetMood(-21);


Audio.Mood.EnemyBigReinforce:
	Audio.SetMood(-22);


Audio.Mood.NeutralReinforce:
	Audio.SetMood(-31);


Audio.Mood.NeutralBigReinforce:
	Audio.SetMood(-32);    


Audio.Mood.AllyLoss:
	Audio.SetMood(-13);


Audio.Mood.AllyBigLoss:
	Audio.SetMood(-14);


Audio.Mood.PlayerScoreKill:
	Audio.SetMood(-1); // for now it is automatic in the engine


Audio.Mood.PlayerEject:
	Audio.SetMood(-2);


Audio.Mood.PlayerScoreBigKill:
	Audio.SetMood(-3); // for now it is automatic in the engine


Audio.Mood.PriObjComplete:
	Audio.SetMood(-4);


Audio.Mood.SecObjComplete:
	Audio.SetMood(-5);


Audio.Mood.BonusObjComplete:
	Audio.SetMood(-6);


Audio.Mood.PriObjFail:
	Audio.SetMood(-7);


Audio.Mood.SecObjFail:
	Audio.SetMood(-8);


Speech.AnnounceAllyKill(int actor, int victim):
    if (!Faction.IsAlly(Actor.GetFaction(actor), Actor.GetFaction(victim)) && (Actor.IsFighter(victim) || Actor.IsLargeShip(victim)))
        if (Random() < 0.6) 
        {
            Audio.QueueSpeech("a-target", "a-destr");
        }



Speech.RegisterCriticalInstallation(int actorid):
    Actor.CallOnHit(actorid, "Speech.CriticalInstallationUnderAttackHandler");


Speech.CriticalInstallationUnderAttackHandler(int actorid):
    if (!GetGameStateB("Actor" + actorid + "_ShieldsDown"))
    {
        float shd = Actor.GetShd(actorid);
        if (shd <= 0) 
        {
            SetGameStateB("Actor" + actorid + "_ShieldsDown", true);
            Audio.QueueSpeech("a-critic", "a-instal", "a-shield");
            return 0;
        }
    }
    if (!GetGameStateB("Actor" + actorid + "_HullCritical"))
    {
        float hpfrac = Actor.GetHPFrac(actorid);
        if (hpfrac <= 0.5) 
        {
            SetGameStateB("Actor" + actorid + "_HullCritical", true);
            Audio.QueueSpeech("a-critic", "a-instal", "a-hull");
            return 0;
        }
    }
    if (GetGameTime() > Speech_message_critical_next)
    {
        Audio.QueueSpeech("a-critic", "a-instal", "a-under");
        Speech_message_critical_next = GetGameTime() + Speech_message_critical_delay;
    }


Speech.CriticalInstallationUnderAttack(int actorid):
    if (GetGameTime() > Speech_message_critical_next)
    {
        Audio.QueueSpeech("a-critic", "a-instal", "a-under");
        Speech_message_critical_next = GetGameTime() + Speech_message_critical_delay;
    }


Speech.CriticalInstallationShieldsDown(int actorid):
    if (!GetGameStateB("Actor" + actorid + "_ShieldsDown"))
    {
        float shd = Actor.GetShd(actorid);
        if (shd <= 0) 
        {
            SetGameStateB("Actor" + actorid + "_ShieldsDown", true);
            Audio.QueueSpeech("a-critic", "a-instal", "a-shield");
        }
    }
    

Speech.CriticalInstallationHullCritical(int actorid):
    if (!GetGameStateB("Actor" + actorid + "_HullCritical"))
    {
        float hpfrac = Actor.GetHPFrac(actorid);
        if (hpfrac <= 0.5) 
        {
            SetGameStateB("Actor" + actorid + "_HullCritical", true);
            Audio.QueueSpeech("a-critic", "a-instal", "a-hull");
        }
    }


Speech.RegisterCriticalCraft(int actorid):
    Actor.CallOnHit(actorid, "Speech.CriticalCraftUnderAttackHandler");


Speech.CriticalCraftUnderAttackHandler(int actorid):
    if (!GetGameStateB("Actor" + actorid + "_ShieldsDown"))
    {
        float shd = Actor.GetShd(actorid);
        if (shd <= 0) 
        {
            SetGameStateB("Actor" + actorid + "_ShieldsDown", true);
            Audio.QueueSpeech("a-critic", "a-craft", "a-shield");
            return 0;
        }
    }
    if (!GetGameStateB("Actor" + actorid + "_HullCritical"))
    {
        float hpfrac = Actor.GetHPFrac(actorid);
        if (hpfrac <= 0.5) 
        {
            SetGameStateB("Actor" + actorid + "_HullCritical", true);
            Audio.QueueSpeech("a-critic", "a-craft", "a-hull");
            return 0;
        }
    }
    if (GetGameTime() > Speech_message_critical_next)
    {
        Audio.QueueSpeech("a-critic", "a-craft", "a-under");
        Speech_message_critical_next = GetGameTime() + Speech_message_critical_delay;
    }


Speech.CriticalCraftUnderAttack(int actorid):
    if (GetGameTime() > Speech_message_critical_next)
    {
        Audio.QueueSpeech("a-critic", "a-craft", "a-under");
        Speech_message_critical_next = GetGameTime() + Speech_message_critical_delay;
    }


Speech.CriticalCraftShieldsDown(int actorid):
    if (!GetGameStateB("Actor" + actorid + "_ShieldsDown"))
    {
        float shd = Actor.GetShd(actorid);
        if (shd <= 0) 
        {
            SetGameStateB("Actor" + actorid + "_ShieldsDown", true);
            Audio.QueueSpeech("a-critic", "a-craft", "a-shield");
        }
    }
    

Speech.CriticalCraftHullCritical(int actorid):
    if (!GetGameStateB("Actor" + actorid + "_HullCritical"))
    {
        float hpfrac = Actor.GetHPFrac(actorid);
        if (hpfrac <= 0.5) 
        {
            SetGameStateB("Actor" + actorid + "_HullCritical", true);
            Audio.QueueSpeech("a-critic", "a-craft", "a-hull");
        }
    }