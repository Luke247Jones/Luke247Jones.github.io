package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;

public class Accelerate extends Command{
	private GameWorld gw;
	
	public Accelerate(GameWorld ref) {
		super("Accelerate");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		gw.accelerate();
	}
}
