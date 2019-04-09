load:
	starttime := 3
	triggerwinlose := false
	started := false
	time := 19
	interval := 20
	time2 := 70
	interval2 := 75
	lives := 3
	
	SetMaxBounds(10000; 1500; 8000)
	SetMinBounds(-10000; -1500; -8000)
	SetMaxAIBounds(10000; 1500; 8000)
	SetMinAIBounds(-10000; -1500; -8000)
	
	SetMusic("battle_2_1")
	SetMusicLoop("battle_2_2")
	
	Player_SetLives(lives)
	Player_SetScorePerLife(1000000)
	Player_SetScoreForNextLife(1000000)
	Player_ResetScore()
	
	SetUILine1Color(0.6; 0.6; 0)
	SetUILine2Color(0.6; 0.6; 0)
	
	CallScript("makeplayer")
	CallScript("makesmugglers")
	AddEvent(7; "stage1")
	AddEvent(12; "message01")
	AddEvent(17; "message02")
	AddEvent(22; "message03")
	AddEvent(28; "message04")
	AddEvent(35; "message05")
	AddEvent(154; "message06")
	AddEvent(157; "stage2")

loadfaction:
	AddFaction("Smugglers"; 0.6; 0.6; 0)
	Faction_SetAsMainAllyFaction("Smugglers")
	AddFaction("Empire"; 0; 0.8; 0)
	AddFaction("Rebels"; 0.8; 0; 0)

loadscene:
	planet := Actor_Spawn("Hoth"; ""; ""; ""; 0; ""; 0; -2000; 0; 0; 180; 0)
	
	Actor_SetActive(Actor_Spawn("Imperial-I Star Destroyer"; ""; ""; ""; 0; "Empire"; 0; 500; -15000; 0; 25; 0))
	Actor_QueueLast("move"; 0; 500; -12000; 2)
	Actor_QueueLast("rotate"; 0; 500; 12000; 0)
	Actor_QueueLast("lock")

makeplayer:
	lives := lives - 1
	Player_SetLives(lives)
	player := Actor_Spawn(GetPlayerActorType(); "(Player)"; ""; "(Player)"; 0; "Smugglers"; 9500; 100; 0; 0; -90; 0)
	Actor_SetActive(player)
	Actor_SetProperty("DamageModifier"; 0.25)
	Player_AssignActor()
	Actor_RegisterEvents()

makesmugglers:
	Actor_SetActive(Actor_Spawn("Millennium Falcon"; "YT-1300"; ""; ""; 0; "Smugglers"; 8600; -20; -500; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("wait"; 5)
	
	Actor_SetActive(Actor_Spawn("Millennium Falcon"; "YT-1300"; ""; ""; 0; "Smugglers"; 8650; 40; 500; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("wait"; 5)
	
	Actor_SetActive(Actor_Spawn("Millennium Falcon"; "YT-1300"; ""; ""; 0; "Smugglers"; 10250; -40; 750; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("wait"; 5)
	
	Actor_SetActive(Actor_Spawn("Millennium Falcon"; "YT-1300"; ""; ""; 0; "Smugglers"; 10050; 60; -750; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("wait"; 5)
	
	Actor_SetActive(Actor_Spawn("Transport"; ""; "trn0"; "TRANSPORT"; 0; "Smugglers"; 8750; -200; 0; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("move"; -8750; -200; 0; 50)
	Actor_QueueLast("hyperspaceout")
	Actor_QueueLast("setgamestateb";"TransportExit";true)
	Actor_QueueLast("delete")
	
	Actor_SetActive(Actor_Spawn("Transport"; ""; "trn1"; "TRANSPORT"; 0; "Smugglers"; 9250; -350; -350; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("move"; -8260; -350; -350; 50)
	Actor_QueueLast("hyperspaceout")
	Actor_QueueLast("setgamestateb";"TransportExit";true)
	Actor_QueueLast("delete")
	
	Actor_SetActive(Actor_Spawn("Transport"; ""; "trn2"; "TRANSPORT"; 0; "Smugglers"; 9050; -300; 350; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("move"; -8470; -300; 350; 50)
	Actor_QueueLast("hyperspaceout")
	Actor_QueueLast("setgamestateb";"TransportExit";true)
	Actor_QueueLast("delete")
	
	Actor_SetActive(Actor_Spawn("Transport"; ""; "trn3"; "TRANSPORT"; 0; "Smugglers"; 9750; -450; -250; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("move"; -7780; -450; -450; 50)
	Actor_QueueLast("hyperspaceout")
	Actor_QueueLast("setgamestateb";"TransportExit";true)
	Actor_QueueLast("delete")
	
	Actor_SetActive(Actor_Spawn("Transport"; ""; "trn4"; "TRANSPORT"; 0; "Smugglers"; 9850; -400; 150; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("move"; -7690; -400; 150; 50)
	Actor_QueueLast("hyperspaceout")
	Actor_QueueLast("setgamestateb";"TransportExit";true)
	Actor_QueueLast("delete")
	
	Actor_SetActive(Actor_Spawn("Transport"; ""; "trn5"; "TRANSPORT"; 0; "Smugglers"; 10650; -250; -50; 0; -90; 0; "CriticalAllies"))
	Actor_QueueLast("move"; -6900; -250; -50; 50)
	Actor_QueueLast("hyperspaceout")
	Actor_QueueLast("setgamestateb";"TransportExit";true)
	Actor_QueueLast("delete")

gametick:
	starttime := starttime - GetLastFrameTime()
	time := time + GetLastFrameTime()
	time2 := time2 + GetLastFrameTime()
	
	if (starttime < 0 && not(started)) then CallScript("start") 
	if (time > interval) then CallScript("spawntie") 
	if (time2 > interval2) then CallScript("spawnwing") 
	
	if (GetTimeSinceLostWing() < GetGameTime() || GetGameTime() % 0.2 > 0.1) then SetUILine1Text("WINGS: " & Faction_GetWingCount("Smugglers")) else SetUILine1Text("")
	if (GetTimeSinceLostShip() < GetGameTime() || GetGameTime() % 0.2 > 0.1) then SetUILine2Text("SHIPS: " & Faction_GetShipCount("Smugglers")) else SetUILine2Text("")
	
	if (GetGameStateB("TransportExit") && not(triggerwinlose)) then CallScript("win")
	if (Faction_GetShipCount("Smugglers") < 1 && not(triggerwinlose)) then SetGameStateB("TransportDead";true)
	if (GetGameStateB("TransportDead") && not(triggerwinlose)) then CallScript("lose")

win:
	triggerwinlose := true
	Message("Our Transports have entered hyperspace. Let us leave this area."; 5; 0.6; 0.6; 0; 1)
	SetGameStateB("GameWon";true)
	AddEvent(3; "Common_FadeOut")

lose:
	triggerwinlose := true
	Message("All our Transports has been destroyed!"; 5; 0.6; 0.6; 0; 1)
	SetGameStateB("GameOver";true)
	AddEvent(3; "Common_FadeOut")

calibratescene:
	Actor_SetActive(player)
	if (Actor_IsAlive()) then CallScript("calibratescene2") 

calibratescene2:
	player_pos := Actor_GetLocalPosition()
	Actor_SetActive(planet)
	Actor_SetLocalPosition(GetArrayElement(player_pos;0) / 1.25;GetArrayElement(player_pos;1) / 2.5 - 2000;GetArrayElement(player_pos;2) / 1.25)

start:
	started := true
	Player_SetMovementEnabled(true)
	Message("We are entering Imperial Territory."; 5; 0.6; 0.6; 0; 1)

spawntie:
	time := time - interval
	Actor_SetActive(Actor_Spawn("TIE"; ""; ""; ""; 0; "Empire"; 500; 100; -10000; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE"; ""; ""; ""; 0; "Empire"; 500; -100; -10000; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE"; ""; ""; ""; 0; "Empire"; 300; 100; -10000; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE"; ""; ""; ""; 0; "Empire"; 300; -100; -10000; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE"; ""; ""; ""; 0; "Empire"; -500; 100; -10000; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE"; ""; ""; ""; 0; "Empire"; -500; -100; -10000; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE"; ""; ""; ""; 0; "Empire"; -300; 100; -10000; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE"; ""; ""; ""; 0; "Empire"; -300; -100; -10000; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE Interceptor"; ""; ""; ""; 0; "Empire"; -100; 0; -10200; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE Interceptor"; ""; ""; ""; 0; "Empire"; -100; 0; -10200; 0; 0; 0))
	Actor_QueueLast("hunt")
	

spawnwing:
	time2 := time2 - interval2
	Actor_SetActive(Actor_Spawn("X-Wing"; ""; ""; ""; 0; "Rebels"; 7300; 0; 46000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; 300; 0; 3000)
	
	Actor_SetActive(Actor_Spawn("X-Wing"; ""; ""; ""; 0; "Rebels"; 6700; 0; 46000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; -300; 0; 3000)
	
	Actor_SetActive(Actor_Spawn("A-Wing"; ""; ""; ""; 0; "Rebels"; 7000; 0; 47000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; 0; -50; 3500)

stage1:
	SetStageNumber(1)

	Actor_SetActive(Actor_Spawn("Corellian Corvette"; ""; ""; ""; 0; "Rebels"; 8750; -400; 45000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; 750; -400; 4000)
	Actor_QueueLast("move"; -750; -400; -5500; 10)
	Actor_QueueLast("hyperspaceout")
	
	Actor_SetActive(Actor_Spawn("Corellian Corvette"; ""; ""; ""; 0; "Rebels"; 6750; -200; 45000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; 1750; 200; 4000)
	Actor_QueueLast("move"; 750; -200; -5500; 10)
	Actor_QueueLast("hyperspaceout")
	
	Actor_SetActive(Actor_Spawn("Acclamator Assault Ship"; ""; ""; ""; 0; "Empire"; -8750; -400; -45000; 0; 0; 0))
	Actor_QueueLast("hyperspacein"; -2750; 100; -5000)
	Actor_QueueLast("move"; 350; 100; 5500; 10)
	Actor_QueueLast("hyperspaceout")
	
	Actor_SetActive(Actor_Spawn("Acclamator Assault Ship"; ""; ""; ""; 0; "Empire"; -4750; -400; -45000; 0; 0; 0))
	Actor_QueueLast("hyperspacein"; 1750; -50; -4800)
	Actor_QueueLast("move"; 2250; 100; 5500; 10)
	Actor_QueueLast("hyperspaceout")

stage2:
	SetStageNumber(2)

	Actor_SetActive(Actor_Spawn("Corellian Corvette"; ""; ""; ""; 0; "Rebels"; 8750; -400; 45000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; -2250; -350; 4000)
	Actor_QueueLast("move"; -1750; -400; -9500; 20)
	Actor_QueueLast("hyperspaceout")
	
	Actor_SetActive(Actor_Spawn("Corellian Corvette"; ""; ""; ""; 0; "Rebels"; 6750; -20; 45000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; 2250; -20; 4000)
	Actor_QueueLast("move"; 1750; -200; -9500; 20)
	Actor_QueueLast("hyperspaceout")
	
	Actor_SetActive(Actor_Spawn("Mon Calamari 80B Capital Ship"; ""; ""; ""; 0; "Rebels"; 7750; -600; 45000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; -1750; -600; 6000)
	Actor_QueueLast("move"; -750; -200; -9500; 12)
	Actor_QueueLast("hyperspaceout")
	
	Actor_SetActive(Actor_Spawn("Arquitens Light Cruiser"; ""; ""; ""; 0; "Empire"; -4750; -400; -45000; 0; 0; 0))
	Actor_QueueLast("hyperspacein"; 3750; 100; -5000)
	Actor_QueueLast("move"; 1350; 100; 9500; 20)
	Actor_QueueLast("hyperspaceout")
	
	Actor_SetActive(Actor_Spawn("Imperial-I Star Destroyer"; ""; ""; ""; 0; "Empire"; -8750; -400; -45000; 0; 0; 0))
	Actor_QueueLast("hyperspacein"; -3750; -50; -4800)
	Actor_QueueLast("move"; 1750; 100; 9500; 12)
	Actor_QueueLast("hyperspaceout")
	
	Actor_SetActive(Actor_Spawn("TIE Defender"; ""; ""; ""; 0; "Empire"; -500; 0; -10200; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE Defender"; ""; ""; ""; 0; "Empire"; -300; 0; -10200; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE Defender"; ""; ""; ""; 0; "Empire"; -100; 0; -10200; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE Defender"; ""; ""; ""; 0; "Empire"; 100; 0; -10200; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE Defender"; ""; ""; ""; 0; "Empire"; 300; 0; -10200; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("TIE Defender"; ""; ""; ""; 0; "Empire"; 500; 0; -10200; 0; 0; 0))
	Actor_QueueLast("hunt")
	
	Actor_SetActive(Actor_Spawn("A-Wing"; ""; ""; ""; 0; "Rebels"; 7000; 0; 47000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; 0; 150; 5500)
	
	Actor_SetActive(Actor_Spawn("A-Wing"; ""; ""; ""; 0; "Rebels"; 7000; 0; 47000; 0; 180; 0))
	Actor_QueueLast("hyperspacein"; 0; -150; 5500)
	