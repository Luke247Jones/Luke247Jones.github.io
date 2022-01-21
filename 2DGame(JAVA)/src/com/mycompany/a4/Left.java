package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;

public class Left extends Command{
	private GameWorld gw;
	
	public Left(GameWorld ref) {
		super("Left");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		gw.left();
	}
}
