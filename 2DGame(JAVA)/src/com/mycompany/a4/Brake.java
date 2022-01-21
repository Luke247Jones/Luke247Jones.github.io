package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;

public class Brake extends Command{
	private GameWorld gw;
	
	public Brake(GameWorld ref) {
		super("Brake");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		gw.brake();
	}
}

