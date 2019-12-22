
float3 msg_thrawn = { 0.3, 0.5, 1 };
float3 msg_corvus = { 0.6, 0.8, 1 };
float3 msg_bad = { 1, 0.3, 0.2 };

float3 msg_warning_yellow = { 0.8, 0.8, 0 };
float3 msg_warning_orange = { 1, 0.5, 0 };
float3 msg_fail_red = { 0.8, 0, 0 };

message01:
	Message("INT CORVUS: This is Interdictor cruiser CORVUS. Interdiction field has been activated.", 5, msg_corvus, 1);

message02:
	Message("INT CORVUS: Enemy bombers en-route. Clear them out before they deal damage.", 5, msg_corvus, 1);
	
message03:
	Message("GRAND ADM. THRAWN: Strike Cruisers Ebolo and Daring will engage the enemy cruiser screen.", 5, msg_thrawn, 1);

message04:
	Message("GRAND ADM. THRAWN: The traitors will attempt to destroy the Interdictor CORVUS to facilitate their escape.", 5, msg_thrawn, 1);
	
message05:
	Message("GRAND ADM. THRAWN: ALPHA squadron, protect the Interdictor CORVUS from enemy attack.", 5, msg_thrawn, 1);
	
message06:
	Message("GRAND ADM. THRAWN: Then destroy the cowering traitor in his Star Destroyer.", 5, msg_thrawn, 1);
	

messageescape:
	Message("Warning: The traitor is trying to escape aboard the VORKNKX. Do not let him escape!", 5, faction_empire_color, 1);

messageescape2:
	Message("GRAND ADM. THRAWN: Interesting. I see Zaarin has refused to activate VORKNKX's stealth generators.", 5, msg_thrawn, 1);
	
messageescape3:
	Message("GRAND ADM. THRAWN: He may be able to escape if he leaves the range of the CORVUS. Destroy the VORKNKX.", 5, msg_thrawn, 1);

messagewin:
	Message("GRAND ADM. THRAWN: The traitor Zaarin has been dealt with.", 5, msg_thrawn, 1);
	
messagewin2:
	Message("GRAND ADM. THRAWN: Such is the fate of the enemies of the Empire.", 5, msg_thrawn, 1);


messagebombercorvus:
	Message("Enemy bombers heading towards INT Corvus", 5, msg_warning_yellow, 1);

messagebombergreywolf:
	Message("Enemy bombers heading towards our flagship", 5, msg_warning_yellow, 1);

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

messageloseescaped:
	Message("MISSION FAILED: The traitor has escaped!", 5, msg_fail_red, 99);
	
message_allyEta:
	Message("TIE/LN ETA squadron has been launched from ISD GREY WOLF.", 5, faction_empire_color, 1);
	
message_allyBeta:
	Message("TIE/SA BETA squadron has been launched from ISD GREY WOLF, targeting ISD GLORY.", 5, faction_empire_color, 1);

message_allyBeta2:
	Message("TIE/SA BETA-1: Heads up, BETA group engaging ISD GLORY.", 5, faction_empire_color, 1);

message_allyEpsilon:
	Message("TIE/SA EPSILON squadron has been launched from ISD GREY WOLF, targeting CRV VORKNKX.", 5, faction_empire_color, 1);

message_allyGamma:
	Message("TIE/I GAMMA squadron has been launched from ISD GREY WOLF, targeting CRV VORKNKX.", 5, faction_empire_color, 1);
	
message_allyGUN:
	Message("ASSAULT GUNSHIPS have arrived to reinforce INT CORVUS.", 5, faction_empire_color, 1);

message_allyTIEA:
	Message("Additional TIE-ADVANCED have arrived to reinforce INT CORVUS.", 5, faction_empire_color, 1);

message_EboloShieldsDown:
	Message("STRIKE CRUISER EBOLO's shields are down, but it will continue fighting!", 5, msg_warning_yellow, 1);

message_DaringShieldsDown:
	Message("STRIKE CRUISER DARING's shields are down, but it will continue fighting!", 5, msg_warning_yellow, 1);
	
message_EboloDestroyed:
	Message("STRIKE CRUISER EBOLO has been destroyed.", 5, msg_bad, 1);

message_DaringDestroyed:
	Message("STRIKE CRUISER DARING has been destroyed.", 5, msg_bad, 1);
	
message_GloryShieldsDown:
	Message("ISD GLORY's shields are down, the traitors will have no hope of escape.", 5, faction_empire_color, 1);

message_VorknkxShieldsDown:
	Message("CORVETTE VORKNKX's shields are down, finish it!", 5, faction_empire_color, 1);
	
message_GloryDestroyed:
	Message("ISD GLORY has been destroyed.", 5, faction_empire_color, 1);

message_VorknkxDestroyed:
	Message("CORVETTE VORKNKX has been destroyed.", 5, faction_empire_color, 1);
	

message_traitorZDelta:
	Message("New craft alert: TIE/D Z-DELTA squadron has been launched from ISD GLORY.", 5, faction_traitor_color);

message_traitorZIota:
	Message("New craft alert: TIE/D Z-IOTA squadron has been launched from ISD GLORY.", 5, faction_traitor_color);

message_traitorZTheta:
	Message("New craft alert: TIE/D Z-THETA squadron has been launched from ISD GLORY.", 5, faction_traitor_color);

