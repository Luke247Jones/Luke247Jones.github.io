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

public class WestView extends Container implements Observer{
	private GameWorld gw = null;
	private Accelerate accelerate;
	private Left left;
	private ChangeStrategy changeStrategy;
	private Button bAccelerate;
	private Button bLeft;
	private Button bChangeStrategies;
	private Button[] buttonArr;
	
	public WestView(GameWorld obs, Accelerate a, Left l, ChangeStrategy c) {
		this.gw = obs;
		this.accelerate = a;
		this.left = l;
		this.changeStrategy = c;
		
		this.setLayout(new FlowLayout(CENTER));
		Container buttonContainer = new Container(new BoxLayout(BoxLayout.Y_AXIS));
		this.add(buttonContainer);
		buttonContainer.getUnselectedStyle().setBorder(Border.createLineBorder(2, ColorUtil.WHITE));
		bAccelerate = new Button("Accelerate");
		bLeft = new Button("Left");
		bChangeStrategies = new Button("Change Strategies");
		bAccelerate.setCommand(accelerate);
		bLeft.setCommand(left);
		bChangeStrategies.setCommand(changeStrategy);
		buttonArr = new Button[] {bAccelerate, bLeft, bChangeStrategies};
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
			for(int i=0; i<buttonArr.length; i++) {
				buttonArr[i].setEnabled(false);
				buttonArr[i].getDisabledStyle().setBgColor(ColorUtil.WHITE);
				buttonArr[i].getDisabledStyle().setFgColor(gw.primaryColor);
				//buttonArr[i].getDisabledStyle().setBorder(Border.createLineBorder(2, gw.primaryColor));
			}
		}
		else {
			for(int i=0; i<buttonArr.length; i++) {
				buttonArr[i].setEnabled(true);
				//buttonArr[i].getUnselectedStyle().setBorder(Border.createLineBorder(2,ColorUtil.WHITE));
				buttonArr[i].getAllStyles().setBgColor(gw.primaryColor);
				buttonArr[i].getAllStyles().setFgColor(ColorUtil.WHITE);
			}
		}
		revalidate();
		//repaint();
	}
}
