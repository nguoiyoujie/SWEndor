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

float damagemod;

spawnreset:
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
	damagemod = 1;


spawn1:
	int spawnunit = Actor.Spawn(spawn_type, spawn_name, spawn_faction, "", 0, spawn_pos, spawn_rot);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	AI.QueueLast(spawnunit, "wait", spawn_wait);
	if (spawn_target >= 0)
		AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);

spawn2:
	string formation = (spawn_formation == "") ? "LINE" : spawn_formation;
	int[] spawnsquad = Actor.Squadron_Spawn(spawn_type, spawn_name, spawn_faction, 2, 0, spawn_hyperspace, spawn_pos, spawn_rot, formation, spawn_spacing, spawn_wait, "ANY");
	foreach(int spawnunit in spawnsquad)
	{
		Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
		if (spawn_target >= 0)
			AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);
	}

spawn3:
	string formation = (spawn_formation == "") ? "VSHAPE" : spawn_formation;
	int[] spawnsquad = Actor.Squadron_Spawn(spawn_type, spawn_name, spawn_faction, 3, 0, spawn_hyperspace, spawn_pos, spawn_rot, formation, spawn_spacing, spawn_wait, "ANY");
	foreach(int spawnunit in spawnsquad)
	{
		Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
		if (spawn_target >= 0)
			AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);
	}

spawn4:
	string formation = (spawn_formation == "") ? "VERTICAL_SQUARE" : spawn_formation;
	int[] spawnsquad = Actor.Squadron_Spawn(spawn_type, spawn_name, spawn_faction, 4, 0, spawn_hyperspace, spawn_pos, spawn_rot, formation, spawn_spacing, spawn_wait, "ANY");
	foreach(int spawnunit in spawnsquad)
	{
		Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
		if (spawn_target >= 0)
			AI.QueueLast(spawnunit, "attackactor", spawn_target, -1, -1, false);
	}

