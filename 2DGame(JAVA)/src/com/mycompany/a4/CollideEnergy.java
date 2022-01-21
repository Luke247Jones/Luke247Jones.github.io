package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;

public class CollideEnergy extends Command{
	private GameWorld gw;
	
	public CollideEnergy(GameWorld ref) {
		super("Collide with Energy");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		int energyStationNum = gw.pickEnergyStation();
		//gw.energyStationReached(energyStationNum);
	}
}

