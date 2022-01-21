package com.mycompany.a4;

import java.util.Observable;
import java.util.Observer;

import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Button;
import com.codename1.ui.Component;
import com.codename1.ui.Container;
import com.codename1.ui.layouts.BoxLayout;
import com.codename1.ui.layouts.FlowLayout;
import com.codename1.ui.plaf.Border;

public class SouthView extends Container implements Observer{
	private GameWorld gw = null;
	private PausePlay pausePlay;
	private Position position;
	private Button bPause;
	private Button bPosition;
	
	public SouthView(GameWorld obs, PausePlay p, Position pos) {
		this.gw = obs;
		this.pausePlay = p;
		this.position = pos;
		
		
		this.setLayout(new FlowLayout(CENTER));
		//BoxLayout xCenter() box = new BoxLayout(BoxLayout.X_AXIS);
		Container buttonContainer = new Container(new BoxLayout(BoxLayout.X_AXIS));
		this.add(buttonContainer);
		bPause = new Button("Pause");
		bPosition = new Button("Position");
		bPause.setCommand(pausePlay);
		bPosition.setCommand(position);
		
		Button[] buttonArr = new Button[] {bPause, bPosition};
		for(int i=0; i<buttonArr.length; i++) {
			buttonArr[i].getAllStyles().setPadding(Component.TOP, 5);
			buttonArr[i].getAllStyles().setPadding(Component.BOTTOM, 5);
			buttonArr[i].getAllStyles().setPadding(Component.LEFT, 5);
			buttonArr[i].getAllStyles().setPadding(Component.RIGHT, 5);
			buttonArr[i].getUnselectedStyle().setBgTransparency(255);
			buttonArr[i].getUnselectedStyle().setBorder(Border.createLineBorder(2,ColorUtil.WHITE));
			buttonArr[i].getAllStyles().setBgColor(gw.primaryColor);
			buttonArr[i].getAllStyles().setFgColor(ColorUtil.WHITE);
			buttonContainer.add(buttonArr[i]);
		}
	}
	
	@Override
	public void update(Observable obs, Object args) {
		if (gw.getPlayMode() == false) {
			bPause.setText("Play");
		}
		else {
			bPause.setText("Pause");
		}
		if (gw.getPositionMode() == false) {
			bPosition.getAllStyles().setBgColor(gw.primaryColor);
			bPosition.getAllStyles().setFgColor(ColorUtil.WHITE);
		} 
		else {
			bPosition.getUnselectedStyle().setBgColor(ColorUtil.WHITE);
			bPosition.getUnselectedStyle().setFgColor(gw.primaryColor);
		}
		revalidate();
		repaint();
	}
}
