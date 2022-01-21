package com.mycompany.a4;

import com.codename1.ui.Button;
import com.codename1.ui.Command;
import com.codename1.ui.Dialog;
import com.codename1.ui.Label;
import com.codename1.ui.events.ActionEvent;

public class Exit extends Command{
	public Exit() {
		super("Exit");
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		Dialog d = new Dialog();
		Label l = new Label("Are you sure you want to exit?");
		Button y = new Button(new Close("Yes"));
		Button n = new Button(new Command("No"));
		d.add(l);
		d.add(y);
		d.add(n);
		d.show();
	}
}
