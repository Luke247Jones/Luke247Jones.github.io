package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;

public class Right extends Command{
	private GameWorld gw;
	
	public Right(GameWorld ref) {
		super("Right");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		gw.right();
	}
}
