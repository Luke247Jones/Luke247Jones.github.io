package com.mycompany.a4;

import com.codename1.ui.Command;
import com.codename1.ui.Dialog;
import com.codename1.ui.Label;
import com.codename1.ui.TextField;
import com.codename1.ui.events.ActionEvent;

public class CollideBase extends Command{
	private GameWorld gw;
	
	public CollideBase(GameWorld ref) {
		super("Collide with Base");
		this.gw = ref;
	}
	
	@Override
	public void actionPerformed(ActionEvent evt) {
		Command Enter = new Command("Enter");
		Command Cancel = new Command("Cancel");
		Command Ok = new Command("Ok");
		Command Close = new Close("Ok");
		TextField myTextField = new TextField();
		Command c = Dialog.show("Please enter base number (1-9): ", myTextField, Enter, Cancel);
		if (c == Enter && myTextField.getText() != "") {
			String input = myTextField.getText();
			int baseNum = Integer.parseInt(input);
			if (baseNum >= 1 && baseNum <= 9) {
				if (baseNum == gw.getFinalBase()) {
					Label win = new Label("Total time: " + gw.getClock());
					Dialog.show("Game Over. You Win!", win, Close);
				}
				else if (baseNum <= gw.getHighestBaseReached()) {
					Label warning = new Label("Please enter the next sequential Base number");
					Dialog.show("Base Already Reached", warning, Ok);
				}
				else if (baseNum > (gw.getHighestBaseReached() + 1)) {
					Label warning = new Label("Please enter the next sequential Base number");
					Dialog.show("Base Not Reachable", warning, Ok);
				}
				else {
					//gw.baseReached(baseNum);
				}
			}
			else {
				Label invalid = new Label("Invalid command. Base Number must be between 1-9.");
				Dialog.show("Error", invalid, Ok);
			}
		}
	}
}
