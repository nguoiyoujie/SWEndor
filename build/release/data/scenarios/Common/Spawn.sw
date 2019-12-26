// Global controls
string spawn_type; 
string spawn_name;
string spawn_faction;

// positioning
bool spawn_hyperspace;
float3 spawn_pos;
float3 spawn_rot;
string spawn_formation;
float spawn_spacing;

// ai
float spawn_wait;
int spawn_target;

// calbacks
int[] spawn_ids;

float spawn_dmgmod;

spawn_reset:
	spawn_name = "";
	spawn_type = "";
	spawn_faction = "Empire";
	spawn_hyperspace = true;
	spawn_pos = { 0, 0, 0 };
	spawn_rot = { 0, 0, 0 };
	spawn_formation = "";
	spawn_spacing = 300;
	spawn_wait = 0;
	spawn_target = -1;
	spawn_dmgmod = 1;


spawn1:
	int spawnunit = Actor.Spawn(spawn_type, spawn_name, spawn_faction, "", 0, spawn_pos, spawn_rot);
	if (spawn_dmgmod >= 0)
		Actor.SetArmorAll(spawnunit, spawn_dmgmod);
	AI.QueueLast(spawnunit, "wait", spawn_wait);
	if (spawn_target >= 0)
		AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);
	spawn_ids = { spawnunit };

spawn2:
	string formation = (spawn_formation == "") ? "LINE" : spawn_formation;
	int[] spawnsquad = Squad.Spawn(spawn_type, spawn_name, spawn_faction, 2, 0, spawn_hyperspace, spawn_pos, spawn_rot, formation, spawn_spacing, spawn_wait, "ANY");
	foreach(int spawnunit in spawnsquad)
	{
		if (spawn_dmgmod >= 0)
			Actor.SetArmorAll(spawnunit, spawn_dmgmod);
		if (spawn_target >= 0)
			AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);
	}
	spawn_ids = spawnsquad;

spawn3:
	string formation = (spawn_formation == "") ? "VSHAPE" : spawn_formation;
	int[] spawnsquad = Squad.Spawn(spawn_type, spawn_name, spawn_faction, 3, 0, spawn_hyperspace, spawn_pos, spawn_rot, formation, spawn_spacing, spawn_wait, "ANY");
	foreach(int spawnunit in spawnsquad)
	{
		if (spawn_dmgmod >= 0)
			Actor.SetArmorAll(spawnunit, spawn_dmgmod);
		if (spawn_target >= 0)
			AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);
	}
	spawn_ids = spawnsquad;

spawn4:
	string formation = (spawn_formation == "") ? "VERTICAL_SQUARE" : spawn_formation;
	int[] spawnsquad = Squad.Spawn(spawn_type, spawn_name, spawn_faction, 4, 0, spawn_hyperspace, spawn_pos, spawn_rot, formation, spawn_spacing, spawn_wait, "ANY");
	foreach(int spawnunit in spawnsquad)
	{
		if (spawn_dmgmod >= 0)
			Actor.SetArmorAll(spawnunit, spawn_dmgmod);
		if (spawn_target >= 0)
			AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);
	}
	spawn_ids = spawnsquad;

spawn5:
	string formation = (spawn_formation == "") ? "VSHAPE" : spawn_formation;
	int[] spawnsquad = Squad.Spawn(spawn_type, spawn_name, spawn_faction, 5, 0, spawn_hyperspace, spawn_pos, spawn_rot, formation, spawn_spacing, spawn_wait, "ANY");
	foreach(int spawnunit in spawnsquad)
	{
		if (spawn_dmgmod >= 0)
			Actor.SetArmorAll(spawnunit, spawn_dmgmod);
		if (spawn_target >= 0)
			AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);
	}
	spawn_ids = spawnsquad;

