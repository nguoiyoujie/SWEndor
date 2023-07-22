// Global controls
int actorp_id; 
float actorp_value;
float actorp_multiplier;


actorp_reset:
	actorp_id = 0;
	actorp_value = 0;
	actorp_multiplier = 1;

Common.SetShields(int actor, float shd, float regen):
	Actor.SetProperty(actor, "Health.MaxShd", shd);
	Actor.SetProperty(actor, "Health.Shd", shd);
	Actor.SetProperty(actor, "Regen.Self", z_tie_shdregen);

Common.MultiplyShields(int actor, float multiplier):
	Actor.SetProperty(actor, "Health.MaxShd", multiplier * Actor.GetProperty(actor, "Health.MaxShd"));
	Actor.SetProperty(actor, "Health.Shd", multiplier * Actor.GetProperty(actor, "Health.Shd"));

Common.SetHull(int actor, float hull):
	Actor.SetProperty(actor, "Health.MaxHull", hull);
	Actor.SetProperty(actor, "Health.Hull", hull);

Common.MultiplyHull(int actor, float hull):
	Actor.SetProperty(actor, "Health.MaxHull", multiplier * Actor.GetProperty(actor, "Health.MaxHull"));
	Actor.SetProperty(actor, "Health.Hull", multiplier * Actor.GetProperty(actor, "Health.Hull"));


actorp_setShd:
	Actor.SetProperty(actorp_id, "Health.MaxShd", actorp_value);
	Actor.SetProperty(actorp_id, "Health.Shd", actorp_value);

actorp_multShd:
	Actor.SetProperty(actorp_id, "Health.MaxShd", actorp_multiplier * Actor.GetProperty(actorp_id, "Health.MaxShd"));
	Actor.SetProperty(actorp_id, "Health.Shd", actorp_multiplier * Actor.GetProperty(actorp_id, "Health.Shd"));

actorp_setHull:
	Actor.SetProperty(actorp_id, "Health.MaxHull", actorp_value);
	Actor.SetProperty(actorp_id, "Health.Hull", actorp_value);

actorp_multHull:
	Actor.SetProperty(actorp_id, "Health.MaxHull", actorp_multiplier * Actor.GetProperty(actorp_id, "Health.MaxHull"));
	Actor.SetProperty(actorp_id, "Health.Hull", actorp_multiplier * Actor.GetProperty(actorp_id, "Health.Hull"));

actorp_multFullRecover:
	Actor.SetProperty(actorp_id, "Health.Shd", Actor.GetProperty(actorp_id, "Health.MaxShd"));
	Actor.SetProperty(actorp_id, "Health.Hull", Actor.GetProperty(actorp_id, "Health.MaxHull"));
