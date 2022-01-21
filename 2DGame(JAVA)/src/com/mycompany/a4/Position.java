package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;
import com.codename1.ui.util.UITimer;

public class Position extends Command {
	private GameWorld gw;
	private boolean position = false;
	
	public Position(GameWorld ref) {
		super("Position");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		position = !position;
		if (position) {
			gw.setPositionMode(true);
		}
		else {
			gw.setPositionMode(true);
		}
	}

}
