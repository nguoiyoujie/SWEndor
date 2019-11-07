
float3 msg_warning_yellow = { 0.8, 0.8, 0 };
float3 msg_warning_orange = { 1, 0.5, 0 };
float3 msg_fail_red = { 0.8, 0, 0 };

message01:
	Message("GRAND ADM. THRAWN: Traitors will die today.", 5, faction_empire_color, 1);

message02:
	Message("GRAND ADM. THRAWN: The Interdictor CORVUS has advanced far ahead and is in a vulnerable position.", 5, faction_empire_color, 1);

message03:
	Message("GRAND ADM. THRAWN: The traitors are attempting to destroy it to facilitate their escape.", 5, faction_empire_color, 1);

message04:
	Message("GRAND ADM. THRAWN: The Interdictor CORVUS  must survive.", 5, faction_empire_color, 1);

message05:
	Message("GRAND ADM. THRAWN: Then destroy the cowering traitor in his Star Destroyer.", 5, faction_empire_color, 1);

message06:
	Message("GRAND ADM. THRAWN: The Empire's future lies in your performance.", 5, faction_empire_color, 1);

messagewin:
	Message("GRAND ADM. THRAWN: Such is the fate of the enemies of the Empire.", 5, faction_empire_color, 1);


messagebombercorvus:
	Message("Warning: Enemy bombers heading towards INT Corvus", 5, faction_empire_color, 1);

messagebombergreywolf:
	Message("Warning: Enemy bombers heading towards our flagship", 5, faction_empire_color, 1);

dangergreywolfmsgY:
	Message("WARNING! Our flagship is under heavy fire!", 3, msg_warning_yellow, 1);

dangergreywolfmsgO:
	Message("WARNING! Our flagship is under heavy fire!", 3, msg_warning_orange, 1);

dangercorvusmsgY:
	Message("WARNING! INT Corvus is under heavy fire!", 3, msg_warning_yellow, 1);

dangercorvusmsgO:
	Message("WARNING! INT Corvus is under heavy fire!", 3, msg_warning_orange, 1);

messagelosegreywolf:
	Message("MISSION FAILED: Our flagship has been destroyed!", 5, msg_fail_red, 99);

messagelosecorvus:
	Message("MISSION FAILED: INT CORVUS has been destroyed!", 5, msg_fail_red, 99);

message_allyGUN:
	Message("ASSAULT GUNSHIPS have arrived to reinforce the CORVUS.", 5, faction_empire_color);

message_allyTIEA:
	Message("Additional TIE-ADVANCED have arrived to reinforce the CORVUS.", 5, faction_empire_color);


message_traitorZ95:
	Message("4x Z95 has entered the battlefield.", 5, faction_traitor_color);

message_traitorXWING:
	Message("3x X-WING has entered the battlefield.", 5, faction_traitor_color);

message_traitorYWING:
	Message("3x Y-WING has entered the battlefield.", 5, faction_traitor_color);

message_traitorAWING:
	Message("3x A-WING has entered the battlefield.", 5, faction_traitor_color);

message_traitorTIED:
	Message("2x TIE-DEFENDER has entered the battlefield.", 5, faction_traitor_color);

message_traitorTIEA:
	Message("2x TIE-ADVANCED has entered the battlefield.", 5, faction_traitor_color);

