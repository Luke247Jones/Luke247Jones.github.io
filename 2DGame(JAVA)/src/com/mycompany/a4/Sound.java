package com.mycompany.a4;

import java.io.InputStream;

import com.codename1.media.Media;
import com.codename1.media.MediaManager;
import com.codename1.ui.Display;

public class Sound {
	private Media m;
	public Sound(String fileName) {
		if (Display.getInstance().getCurrent() == null) {
			System.out.println("Error: Create sound objects after calling show()!");
			System.exit(0);
		}
		while (m == null) {
			try {
				InputStream is = Display.getInstance().getResourceAsStream(getClass(), "/"+fileName);
				m = MediaManager.createMedia(is, "audio/wav");
			}
			catch(Exception e) {
				e.printStackTrace();
			}
		}
	}
	public void pause() { 
		m.pause();
	}
	public void play1() {
		m.setTime(0);
		m.play();
	}
	public void play2() {
		m.setTime(400);
		m.play();
	} 
}
