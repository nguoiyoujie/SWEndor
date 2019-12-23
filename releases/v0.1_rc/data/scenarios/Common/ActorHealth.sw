// Global controls
int actorp_id; 
float actorp_value;
float actorp_multiplier;


actorp_reset:
	actorp_id = 0;
	actorp_value = 0;
	actorp_multiplier = 1;

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
