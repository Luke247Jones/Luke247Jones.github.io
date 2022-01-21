package com.mycompany.a4;


import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;

public class CollideNPC extends Command{
	private GameWorld gw;
	
	public CollideNPC(GameWorld ref) {
		super("Collide with NPC");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		//gw.collisionCyborg();
	}
}

