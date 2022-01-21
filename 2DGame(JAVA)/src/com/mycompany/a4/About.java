package com.mycompany.a4;

import com.codename1.components.SpanLabel;
import com.codename1.ui.Button;
import com.codename1.ui.Command;
import com.codename1.ui.Dialog;
import com.codename1.ui.events.ActionEvent;

public class About extends Command{
	public About() {
		super("About");
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		Dialog d = new Dialog();
		SpanLabel sl = new SpanLabel("Lucas Jones \n" + "CSC 133 \n" + "Assignment 2 \n" + "- GUI implementation \n" + "- Design Patterns");
		Button b = new Button(new Command("Cancel"));
		d.add(sl);
		d.add(b);
		d.show();
	}
}
