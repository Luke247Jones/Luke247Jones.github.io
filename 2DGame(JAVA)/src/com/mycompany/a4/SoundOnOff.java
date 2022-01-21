package com.mycompany.a4;

import com.codename1.ui.CheckBox;
import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;


public class SoundOnOff extends Command {
	private GameWorld gw;
	private Game g;
	//private SoundLoop bgSound = null;
	
	public SoundOnOff(GameWorld ref, Game obj) {
		super("Sound");
		gw = ref;
		g = obj;
	}
	public void update(SoundLoop bg) {
		//bgSound = bg;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		if (gw.getClock() > 0) {
		if (((CheckBox)evt.getComponent()).isSelected()) {
			gw.setSound(true);
			g.titleBar.closeSideMenu();
			//bgSound.play();
			gw.playBackgroundMusic();
		}
		else {
			gw.setSound(false);
			g.titleBar.closeSideMenu();
			gw.playBackgroundMusic();
		}

		}
	}
}
