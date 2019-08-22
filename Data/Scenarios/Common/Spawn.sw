spawnreset:
	spawnfaction = "Empire";
	damagemod = 1;
	spawnhyperspace = true;
	spawntype = ""; 
	spawntarget = -1;
	spawnwait = 0;
	spawnX = 0;
	spawnY = 0;
	spawnZ = 0;
	spawnRotX = 0;
	spawnRotY = 0;
	spawnRotZ = 0;

spawn1:
	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX, spawnY, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	if (spawntarget >= 0)
		Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

spawn2:
	spawnsquad = Actor.Squadron_Spawn("", spawntype, spawnfaction, 3, spawnwait, "ANY", spawnhyperspace, "LINE", spawnRotX, spawnRotY, spawnRotZ, 200, 0, spawnX, spawnY, spawnZ);
	foreach(spawnunit in spawnsquad)
	{
		Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
		if (spawntarget >= 0)
			Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);
	}

spawn3:
	spawnsquad = Actor.Squadron_Spawn("", spawntype, spawnfaction, 3, spawnwait, "ANY", spawnhyperspace, "VSHAPE", spawnRotX, spawnRotY, spawnRotZ, 200, 0, spawnX, spawnY, spawnZ);
	foreach(spawnunit in spawnsquad)
	{
		Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
		if (spawntarget >= 0)
			Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);
	}

spawn4:
	spawnsquad = Actor.Squadron_Spawn("", spawntype, spawnfaction, 4, spawnwait, "ANY", spawnhyperspace, "VERTICAL_SQUARE", spawnRotX, spawnRotY, spawnRotZ, 200, 0, spawnX, spawnY, spawnZ);
	foreach(spawnunit in spawnsquad)
	{
		Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
		if (spawntarget >= 0)
			Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);
	}

