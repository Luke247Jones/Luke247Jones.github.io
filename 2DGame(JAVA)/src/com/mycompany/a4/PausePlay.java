package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;
import com.codename1.ui.util.UITimer;

public class PausePlay extends Command{
	private UITimer timer;
	private Game g;
	private GameWorld gw;
	private boolean pause = false;
	private boolean soundWasOn = false;
	
	public PausePlay(Game ref1, GameWorld ref2) {
		super("Pause");
		this.g = ref1;
		this.gw = ref2;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		pause = !pause;
		if (pause) {
			g.pauseTimer();
			if (gw.getSound() == "ON") {
				soundWasOn = true;
				gw.setSound(false);
			}
			else {
				soundWasOn = false;
			}
			//gw.setSound(false);
			gw.playBackgroundMusic();
			gw.setPlayMode(false);
			g.disableTitleBar();
			g.disableKeyBindings();
			//super.setCommandName("Play");
		}
		else {
			g.startTimer();
			if (soundWasOn == true) {
				gw.setSound(true);
			}
			//gw.setSound(true);
			gw.playBackgroundMusic();
			gw.setPlayMode(true);
			g.enableTitleBar();
			g.enableKeyBindings();
			//super.setCommandName("Pause");
		}
	}
}
