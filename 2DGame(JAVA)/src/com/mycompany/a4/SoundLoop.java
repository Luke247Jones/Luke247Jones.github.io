package com.mycompany.a4;

import java.io.InputStream;

import com.codename1.media.Media;
import com.codename1.media.MediaManager;
import com.codename1.ui.Display;

public class SoundLoop implements Runnable{
	private Media m;
	public SoundLoop(String fileName) {
		if (Display.getInstance().getCurrent() == null) {
			System.out.println("Error: Create sound objects after calling show()!");
			System.exit(0);
		}
		while (m == null) {
			try {
				InputStream is = Display.getInstance().getResourceAsStream(getClass(), "/"+fileName);
				m = MediaManager.createMedia(is, "audio/wav", this);
			}
			catch(Exception e) {
				e.printStackTrace();
			}
		}
	}
	public void run() {
		//start playing the sound from time zero (beginning of the sound file)
		m.setTime(0);
		m.play();
	}
	public void pause() { 
		m.pause();
	}
	public void play() {
		m.play();
	} 
}
