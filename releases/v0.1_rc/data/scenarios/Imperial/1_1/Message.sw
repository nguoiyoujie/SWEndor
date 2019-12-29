
float3 msg_outpost = { 0.6, 0.8, 1 };
float3 msg_delta = { 0.2, 0.6, 0.1 };
float3 msg_bad = { 1, 0.3, 0.2 };

float3 msg_warning_yellow = { 0.8, 0.8, 0 };
float3 msg_warning_orange = { 1, 0.5, 0 };
float3 msg_fail_red = { 0.8, 0, 0 };

message_group1:
	Message("OUTPOST D-34: ALPHA 1, we have freighters to be inspected.", 5, msg_outpost);

message_group1_2:
	Message("OUTPOST D-34: Fly close to the freighters to inspect them.", 5, msg_outpost);

message_group1_3:
	Message("OUTPOST D-34: The inspection should be automatic. If there are any Rebels hiding in the ships, we will find them.", 5, msg_outpost);

message_group2:
	Message("OUTPOST D-34: ALPHA 1, additional freighters have arrived for inspection.", 5, msg_outpost);

messagewarn_outpost_y:
	Message("WARNING: OUTPOST D-34 is under heavy fire!", 5, msg_warning_yellow, 1);

messagewarn_outpost_o:
	Message("WARNING: OUTPOST D-34 is under heavy fire!", 5, msg_warning_orange, 1);

messagewarn_sigma_y:
	Message("WARNING: Transport SIGMA, shields down!", 5, msg_warning_yellow, 1);

messagewarn_sigma_o:
	Message("WARNING: Transport SIGMA, shields down!", 5, msg_warning_orange, 1);

messagewarn_onece3_y:
	Message("WARNING: ONECE 3 is in a critical state, we need the rebels alive!", 5, msg_warning_yellow, 1);

messagewarn_onece3_o:
	Message("WARNING: ONECE 3 is in a critical state, we need the rebels alive!", 5, msg_warning_orange, 1);


messagewin:
	Message("Docking...", 10, faction_empire_color, 99);
	
messagelose_outpostlost:
	Message("MISSION FAILED: Somehow, the unthinkable happened - Outpost D-34 is lost.", 5, msg_fail_red, 99);
	
messagelose_onece3lost:
	Message("MISSION FAILED: We sought the capture of escaping rebels, but the freighter has been lost.", 5, msg_fail_red, 99);

messagelose_sigmalost:
	Message("MISSION FAILED: We sought the capture of escaping rebels, but transport shuttle SIGMA has been lost.", 5, msg_fail_red, 99);
	
messagelose_onece3escaped:
	Message("MISSION FAILED: We sought the capture of escaping rebels, but the freighter has escaped.", 5, msg_fail_red, 99);

message_primaryobj_completed:
	Message("OUTPOST D-34: Good work, Alpha 1. You have completed your primary objectives.", 5, msg_outpost);

message_secondaryobj_completed:
	Message("OUTPOST D-34: Outstanding work, ALPHA 1! You have taken the extra steps to ensure the security of this region.", 5, msg_outpost);

message_returntobase:
	Message("OUTPOST D-34: Enter the outpost hangar to end your mission.", 8, msg_outpost);


message_inspected_onece1:
	Message("Freighter ONECE 1 has been identified to be carrying foodstuff.", 5, msg_outpost);

message_inspected_onece2:
	Message("Freighter ONECE 2 has been identified to be carrying foodstuff.", 5, msg_outpost);
	
message_inspected_onece3:
	Message("Freighter ONECE 3 has been identified to be carrying Rebels.", 5, msg_warning_yellow);
	
message_inspected_onece4:
	Message("Freighter ONECE 4 has been identified to be carrying foodstuff.", 5, msg_outpost);
	
message_inspected_onece5:
	Message("Freighter ONECE 5 has been identified to be carrying foodstuff.", 5, msg_outpost);
	
message_inspected_dayta1:
	Message("Transport DAYTA 1 has been identified to be carrying workers.", 5, msg_outpost);
	
message_inspected_dayta2:
	Message("Transport DAYTA 2 has been identified to be carrying workers.", 5, msg_outpost);
	
message_inspected_yander1:
	Message("Transport YANDER 1 has been identified to be carrying water.", 5, msg_outpost);
	
message_inspected_yander2:
	Message("Transport YANDER 2 has been identified to be carrying foodstuff.", 5, msg_outpost);
	
message_inspected_yander3:
	Message("Transport YANDER 3 has been identified to be carrying foodstuff.", 5, msg_outpost);

message_inspected_taloos1:
	Message("Freighter TALOOS 1 has been identified to be carrying machinery.", 5, msg_outpost);
	
message_inspected_taloos2:
	Message("Freighter TALOOS 2 has been identified to be carrying no cargo.", 5, msg_outpost);
	
message_inspected_onece100%:
	Message("All freighters from ONECE group has been inspected.", 5, msg_outpost);

message_inspected_dayta100%:
	Message("All transports from DAYTA group has been inspected.", 5, msg_outpost);

message_inspected_yander100%:
	Message("All transports from YANDER group has been inspected.", 5, msg_outpost);

message_inspected_taloos100%:
	Message("All freighters from TALOOS group has been inspected.", 5, msg_outpost);

message_inspected_all:
	Message("All traffic has been inspected. Good work, ALPHA 1.", 5, msg_outpost);

message_inspected_glich:
	Message("Transport GLICH 1 has been identified to be carrying Rebels.", 5, msg_warning_yellow);

	

message_sigmaboarding:
	Message("SIGMA 1: Beginning boarding operation.", 5, faction_empire_trans_color);

message_sigmaboardingcomplete:
	Message("SIGMA 1: Boarding operation complete. Returning to base.", 5, faction_empire_trans_color);

message_rebelscaptured:
	Message("OUTPOST D-34: The rebels in ONECE 3 have been captured and taken into the outpost.", 5, msg_outpost);

	
message_glich_destroyed:
	Message("OUTPOST D-34: Rebel transport GLICH 1 destroyed.", 5, msg_outpost);

	
message_onece:
	Message("New craft alert: Freighters ONECE has entered the area, ready for inspection.", 5, faction_neutral_color);

message_onece3_2:
	Message("OUTPOST D-34: A transport is being dispatched to disable the craft.", 5, msg_outpost);

message_onece3_3:
	Message("OUTPOST D-34: Assist our transport by lowering the rebel freighter's shields.", 5, msg_outpost);

message_onece3_disabled:
	Message("SIGMA 1: Freighter ONECE 3 has been disabled.", 5, faction_empire_trans_color);
	
message_dayta:
	Message("New craft alert: Transports DAYTA has entered the area, ready for inspection.", 5, faction_neutral_color);
	
message_yander:
	Message("New craft alert: Transports YANDER has entered the area, ready for inspection.", 5, faction_neutral_color);

message_taloos:
	Message("New craft alert: Freighters TALOOS has entered the area, ready for inspection.", 5, faction_neutral_color);

message_glich:
	Message("New craft alert: We have an unidentified transport entering the area.", 5, faction_neutral_rebel_color);

message_glich_2:
	Message("OUTPOST D-34: Transport GLICH 1 is trying to evade inspection. You are authorized to destroy it if it puts up any resistance.", 5, msg_outpost);


	
	
message_enemySHU:
	Message("New craft alert: We have unidentifed shuttles entering the area.", 5, faction_rebel_color);

message_rebels:
	Message("DELTA 1: The shuttles are operated by rebels!", 5, msg_delta);

message_rebels_2:
	Message("OUTPOST D-34: All craft, engage the enemy shuttles.", 5, msg_outpost);
	
message_enemyES:
	Message("New craft alert: Rebel escort shuttles entering the area. Proceed with caution.", 5, faction_rebel_color);

message_protect:
	Message("OUTPOST D-34: Protect shuttle SIGMA and freighter ONECE 3. We want the rebels alive.", 5, msg_outpost);

	
message_reinf:
	Message("OUTPOST D-34: We are dispatching additional TIEs to engage the threats.", 5, msg_outpost);
