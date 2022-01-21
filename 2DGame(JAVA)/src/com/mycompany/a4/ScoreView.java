package com.mycompany.a4;

import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Component;
import com.codename1.ui.Container;
import com.codename1.ui.Label;
import com.codename1.ui.layouts.FlowLayout;

import java.util.Observable;
import java.util.Observer;

public class ScoreView extends Container implements Observer{
	private GameWorld gw = null;
	private Label scoreLabel;
	
	public ScoreView(GameWorld obs) {
		this.gw = obs;
		this.setLayout(new FlowLayout(Component.CENTER));
		scoreLabel = new Label(gw.score());
		scoreLabel.getAllStyles().setFgColor(ColorUtil.rgb(50, 10, 90));
		this.add(scoreLabel);
		//revalidate();
	}
	
	@Override
	public void update(Observable obs, Object args) {
		//this.gw = obs;
		scoreLabel.setText(gw.score());
		//revalidate();
		//repaint();
	}
}
