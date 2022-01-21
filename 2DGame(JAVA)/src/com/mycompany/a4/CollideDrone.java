package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;

public class CollideDrone extends Command{
	private GameWorld gw;
	
	public CollideDrone(GameWorld ref) {
		super("Collide with Drone");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		//gw.collisionDrone();
	}
}
