spawnreset:
	spawnfaction = "Empire";
	damagemod = 1;
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
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

spawn2:
	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX - 100, spawnY, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX + 100, spawnY, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

spawn3:
	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX - 150, spawnY, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX, spawnY, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX + 150, spawnY, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

spawn4:
	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX - 100, spawnY - 100, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX + 100, spawnY - 100, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX - 100, spawnY + 100, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);

	spawnunit = Actor.Spawn(spawntype, "", "", "", 0, spawnfaction, spawnX + 100, spawnY + 100, spawnZ, spawnRotX, spawnRotY, spawnRotZ);
	Actor.SetProperty(spawnunit, "DamageModifier", damagemod);
	Actor.QueueLast(spawnunit, "wait", spawnwait);
	Actor.QueueLast(spawnunit, "attackactor", spawntarget, -1, -1, false);
