package com.mycompany.a4;

import com.codename1.ui.CheckBox;
import com.codename1.ui.Command;
import com.codename1.ui.Form;
import com.codename1.ui.events.ActionListener;
import com.codename1.ui.layouts.BorderLayout;
import com.codename1.ui.util.UITimer;
import com.codename1.ui.Label;
import com.codename1.charts.util.ColorUtil;
import com.codename1.components.*;
import com.codename1.ui.TextField;
import com.codename1.ui.Toolbar;
import com.codename1.ui.events.ActionEvent;
import java.lang.String;

public class Game extends Form implements Runnable{
	private GameWorld gw;
	private MapView mv;
	private ScoreView sv;
	private SouthView sov;
	private WestView wv;
	private EastView ev;
	private Accelerate accelerate;
	private SoundOnOff soundOnOff;
	private About about;
	private Exit exit;
	private Help help;
	private Left left;
	private Brake brake;
	private ChangeStrategy changeStrategy;
	private Right right;
	
	private PausePlay pausePlay;
	private Position position;
	private UITimer timer; 
	 
	
	public Toolbar titleBar = new Toolbar();
	public Label checkStatusVal = new Label("OFF");
	public CheckBox soundCheck = new CheckBox("Sound");
	
	public Game() {
		gw = new GameWorld();
		
		
		pausePlay = new PausePlay(this, gw);
		position = new Position(gw);
		
		accelerate = new Accelerate(gw);
		soundOnOff = new SoundOnOff(gw, this);
		soundCheck.setCommand(soundOnOff);
		about = new About();
		exit = new Exit();
		help = new Help();
		left = new Left(gw);
		brake = new Brake(gw);
		changeStrategy = new ChangeStrategy(gw);
		right = new Right(gw);
		
		mv = new MapView(gw);
		sv = new ScoreView(gw);
		sov = new SouthView(gw, pausePlay, position);
		wv = new WestView(gw, accelerate, left, changeStrategy);
		ev = new EastView(gw, brake, right);
		gw.addObserver(mv);
		gw.addObserver(sv);
		gw.addObserver(sov);
		gw.addObserver(wv);
		gw.addObserver(ev);
		
		this.setLayout(new BorderLayout());
		this.add(BorderLayout.CENTER, mv);
		this.add(BorderLayout.NORTH, sv);
		this.add(BorderLayout.SOUTH, sov);
		this.add(BorderLayout.WEST, wv);
		this.add(BorderLayout.EAST, ev);
		buildTitleBar();
		buildKeyBindings();
		
		
		//revalidate();
		
		this.show();
		gw.init();
		gw.setMapSize((mv.getWidth() / 2), (mv.getHeight() / 2));
		gw.createSounds(soundOnOff);
		//soundOnOff.update(gw);
		revalidate();
		
		startTimer();
		//UITimer timer = new UITimer(this);
		//timer.schedule(20, true, this);
		//revalidate();
		
		
		//gw.setMapSize(mv.getWidth(), mv.getHeight());
		
	}
	private void buildKeyBindings() {
		addKeyListener('a', accelerate);
		addKeyListener('b', brake);
		addKeyListener('l', left);
		addKeyListener('r', right);
		//addKeyListener('t', tick);
	}
	public void enableKeyBindings() {
		buildKeyBindings();
	}
	public void disableKeyBindings() {
		removeKeyListener('a', accelerate);
		removeKeyListener('b', brake);
		removeKeyListener('l', left);
		removeKeyListener('r', right);
	}
	private void buildTitleBar() {
		setToolbar(titleBar);
		titleBar.setTitle("Keep on Track");
		
		//soundCheck.setCommand(soundOnOff);
		
		titleBar.addCommandToSideMenu(accelerate);
		titleBar.addComponentToSideMenu(soundCheck); 
		titleBar.addCommandToSideMenu(about);
		titleBar.addCommandToSideMenu(exit);
		titleBar.addCommandToRightBar(help);
		
	}
	public void enableTitleBar() {
		accelerate.setEnabled(true);
		soundCheck.setEnabled(true);
		titleBar.removeCommand(accelerate);
		titleBar.removeComponent(soundCheck);
		titleBar.removeCommand(about);
		titleBar.removeCommand(exit);
		titleBar.removeCommand(help);
		//revalidate();
		//repaint();
		buildTitleBar();
		//revalidate();
		//repaint();
	}
	public void disableTitleBar() {
		accelerate.setEnabled(false);
		soundCheck.setEnabled(false);
		titleBar.removeCommand(accelerate);
		titleBar.removeComponent(soundCheck);
		titleBar.removeCommand(about);
		titleBar.removeCommand(exit);
		titleBar.removeCommand(help);
		//revalidate();
		//repaint();
		buildTitleBar();
		//revalidate();
		//repaint();
	}
	public void startTimer() {
		timer = new UITimer(this);
		timer.schedule(20, true, this);
		repaint();
	}
	public void pauseTimer() {
		timer.cancel();
		repaint();
	}
	public void run() {
		gw.tick();
		//mv.repaint();
	}
	
	private void play() {
		Label myLabel=new Label("Enter a Command:");
		this.addComponent(myLabel);
		final TextField myTextField=new TextField();
		this.addComponent(myTextField);
		final SpanLabel responseLabel= new SpanLabel("");
		this.addComponent(responseLabel);
		this.show();
		myTextField.addActionListener(new ActionListener(){
			public void actionPerformed(ActionEvent evt) {
				String sCommand=myTextField.getText().toString();
				myTextField.clear();
				if(sCommand.length() != 0)
					switch (sCommand.charAt(0)) {
						case 'x':			
							responseLabel.setText("Are you sure you want exit? (y/n)");
							//gw.setExitIntent(true);
							break;
						case 'y':
							//if(gw.getExitIntent() == true)
								System.exit(0);
						case 'n':
							responseLabel.setText("");
							break;
						case 'd':
							responseLabel.setText(gw.score());
							break;
						case 'm':
							responseLabel.setText(gw.map());
							break;
						case 't':
							//responseLabel.setText(gw.tick());
							break;
						case 'r':
							//responseLabel.setText(gw.right());
							break;
						case 'l':
							//responseLabel.setText(gw.left());
							break;
						case 'a':
							//responseLabel.setText(gw.accelerate());
							break;
						case 'b':
							//responseLabel.setText(gw.brake());
							break;
						case 'c':
							//responseLabel.setText(gw.collisionCyborg());
							break;
						case '1':
							//responseLabel.setText(gw.baseReached(1));
							break;
						case '2':
							//responseLabel.setText(gw.baseReached(2));
							break;
						case '3':
							//responseLabel.setText(gw.baseReached(3));
							break;
						case '4':
							//responseLabel.setText(gw.baseReached(4));
							break;
						case '5':
							//responseLabel.setText(gw.baseReached(5));
							break;
						case '6':
							//responseLabel.setText(gw.baseReached(6));
							break;
						case '7':
							//responseLabel.setText(gw.baseReached(7));
							break;
						case '8':
							//responseLabel.setText(gw.baseReached(8));
							break;
						case '9':
							//responseLabel.setText(gw.baseReached(9));
							break;
						case 'g':
							//responseLabel.setText(gw.collisionDrone());
							break;
						case 'e':
							//int energyNumber = gw.pickEnergyStation();
							//responseLabel.setText(gw.energyStationReached(energyNumber));
							break;
						default:
							responseLabel.setText("Error: invalid command");
							break;
							//map
						//add code to handle rest of the commands
					} //switch
			} //actionPerformed
		} //new ActionListener()
		); //addActionListener
	}
}
