package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.events.ActionEvent;

public class Close extends Command{
	
	public Close(String title) {
		super(title);
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		System.exit(0);
	}
}
