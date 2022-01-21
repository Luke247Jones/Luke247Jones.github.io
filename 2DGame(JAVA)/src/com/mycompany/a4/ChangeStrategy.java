package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.Dialog;
import com.codename1.ui.Label;
import com.codename1.ui.events.ActionEvent;

public class ChangeStrategy extends Command {
	private GameWorld gw;
	
	public ChangeStrategy(GameWorld ref) {
		super("Change Strategy");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		gw.changeStrategies();
		if (gw.getGameOverNPC() == true) {
			Label lose = new Label("Non Player Cyborg reached the final Base");
			Command Close = new Close("Ok");
			Dialog.show("Game Over. You Lose!", lose, Close);
		}
	}
}
