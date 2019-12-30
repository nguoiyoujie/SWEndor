
float3 msg_outpost = { 0.6, 0.8, 1 };
float3 msg_delta = { 0.2, 0.6, 0.1 };
float3 msg_bad = { 1, 0.3, 0.2 };

float3 msg_warning_yellow = { 0.8, 0.8, 0 };
float3 msg_warning_orange = { 1, 0.5, 0 };
float3 msg_fail_red = { 0.8, 0, 0 };

message1:
	Message("OUTPOST D-34: Sector alert! There are unknown craft entering our area.", 5, msg_outpost);

message2:
	Message("DELTA 1: BETA and GAMMA squadrons, engaging enemy forces.", 5, msg_delta);

message3:
	Message("OUTPOST D-34: ALPHA squadron, join the attack and engage the enemy.", 5, msg_outpost);

message4:
	Message("OUTPOST D-34: Stop all Y-WINGs from making torpedo runs on D-34.", 5, msg_outpost);

message_protect:
	Message("OUTPOST D-34: Hold all attacks until reinforcements arrive to relieve defenses.", 5, msg_outpost);

	
messagewarn_outpost_1:
	Message("WARNING: OUTPOST D-34 is under attack!", 5, msg_warning_yellow, 1);

messagewarn_outpost_y:
	Message("WARNING: OUTPOST D-34 is under heavy fire!", 5, msg_warning_yellow, 1);

messagewarn_outpost_o:
	Message("WARNING: OUTPOST D-34 is under heavy fire!", 5, msg_warning_orange, 1);


messagewin:
	Message("Docking...", 10, faction_empire_color, 99);
	
messagelose_outpostlost:
	Message("MISSION FAILED: Outpost D-34 has been lost.", 5, msg_fail_red, 99);

messagelose_hammerlost:
	Message("MISSION FAILED: The ISD HAMMER had enough of your antics. You will be dealt with switfly.", 5, msg_fail_red, 99);


message_primaryobj_completed:
	Message("ISD HAMMER: The ISD HAMMER has arrived to recover station defense forces.", 5, msg_outpost);

message_returntobase:
	Message("ISD HAMMER: ALPA 1, return to the HAMMER's hangar immediately.", 8, faction_empire_hammer_color);


message_scutz:
	Message("New craft alert: An unidentified shuttle has entered the area.", 5, faction_neutral_rebel_color);
	
message_inspected_scutz:
	Message("Shuttle SHUTZ 1 has been identified to be carrying Rebel officers.", 5, msg_warning_yellow);

message_located:
	Message("OUTPOST D-34: We have located the shuttle with the rebel officers fleeing from Hoth.", 5, faction_empire_hammer_color);

message_muapproach:
	Message("OUTPOST D-34: Reinforcements will arrive shortly to disable and capture the shuttle.", 5, faction_empire_hammer_color);
	
message_scutz_disabled:
	Message("MU 1: This is MU 1. Target disabled.", 5, faction_empire_hammer_color);


message_muboarding:
	Message("MU 1: Beginning boarding operation.", 5, faction_empire_hammer_color);

message_muboardingcomplete:
	Message("MU 1: Boarding operation complete. Proceeding to enter hyperspace.", 5, faction_empire_hammer_color);

message_rebelscaptured:
	Message("OUTPOST D-34: The rebel offices have been taken into Empire custody.", 5, msg_outpost);

message_rebelscaptured_2:
	Message("OUTPOST D-34: The shuttle SCUTZ may be destroyed.", 5, msg_outpost);


message_enemyYWING1:
	Message("New craft alert: Mugaari Y-WING PETDUR group entering the area, targeting OUTPOST D-34.", 5, faction_mugaari_color);

message_enemyYWING2:
	Message("New craft alert: Rebel X-WING GOLD group entering the area, targeting OUTPOST D-34.", 5, faction_rebel_color);

message_enemyYWING3:
	Message("New craft alert: Mugaari Y-WING LAIRE group entering the area, targeting OUTPOST D-34.", 5, faction_mugaari_color);

message_enemyYWING4:
	Message("New craft alert: Rebel Y-WING RED group entering the area, targeting OUTPOST D-34.", 5, faction_rebel_color);

message_enemyZ951:
	Message("New craft alert: Mugaari Z-95 DERK group entering the area, targeting OUTPOST D-34.", 5, faction_mugaari_color);
	
message_enemyZ952:
	Message("New craft alert: Rebel Z-95 BLUE group entering the area, targeting TIE-Fighters.", 5, faction_rebel_color);

message_enemyXWING1:
	Message("New craft alert: Mugaari X-WING DERK group entering the area, targeting OUTPOST D-34.", 5, faction_mugaari_color);
	
message_enemyXWING2:
	Message("New craft alert: Rebel X-WING BLUE group entering the area, targeting TIE-Fighters.", 5, faction_rebel_color);

message_enemyXWING3:
	Message("New craft alert: Rebel X-WING GRAY group entering the area, targeting TIE-Fighters.", 5, faction_rebel_color);
	
message_enemyUbote:
	Message("New craft alert: Rebel CORVETTE UBOTE group entering the area. Proceed with caution!", 5, faction_rebel_color);

message_hammer:
	Message("ISD HAMMER: The ISD HAMMER has arrived to take command of the battle.", 5, msg_outpost);

message_hammer_2:
	Message("ISD HAMMER: Good work. You have fought well in defending D-34 Outpost.", 5, msg_outpost);


	

message_reinf:
	Message("OUTPOST D-34: We are dispatching additional TIEs to engage the threats.", 5, msg_outpost);
