package com.mycompany.a4;

import com.codename1.components.SpanLabel;
import com.codename1.ui.Button;
import com.codename1.ui.Command;
import com.codename1.ui.Dialog;
import com.codename1.ui.events.ActionEvent;

public class Help extends Command{
	public Help() {
		super("Help");
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		Dialog d = new Dialog();
		SpanLabel sl = new SpanLabel("Keyboard Commands: \n" 
									+ "'a' = accelerate \n" 
									+ "'b' = brake \n" 
									+ "'l' = left \n"
									+ "'r' = right \n" 
									+ "'e' = reach energy station \n"
									+ "'g' = collide with drone \n"
									+ "'t' = tick clock");
		Button b = new Button(new Command("Cancel"));
		d.add(sl);
		d.add(b);
		d.show();
	}
}
