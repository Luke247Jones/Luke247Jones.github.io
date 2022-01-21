package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.charts.util.ColorUtil;
import java.util.Vector;
import java.util.Observable;
import java.util.Random;

public class GameWorld extends Observable{
	//Initial game world values
	private GameObjectCollection theGameCollection;
	private Vector<SoundLoop> theSoundCollection;
	private SoundLoop bgSound; 
	private Sound crashSound, energySound, dieSound;
	private int mapWidth =2;
	private int mapHeight =1;
	private int clock;
	private int livesRemaining;
	private int finalBase;
	private boolean sound = true;
	private boolean playMode = true;
	private boolean positionMode = false;
	private boolean won = false;
	private boolean npcWon = false;
	private int highestBaseReached = 1;
	public int primaryColor = ColorUtil.rgb(50, 10, 90);
	public int elapsedTime = 20;
	
	
	//Initial base values
	final private int baseSize = 70;
	final private int baseColor = ColorUtil.rgb(0,0,255);
	Vector<Point> baseLocations = new Vector<Point>();
	
	//Initial cyborg values
	final private int cyborgSize = 20;
	final private int cyborgColor = ColorUtil.rgb(255,0,0); //Red
	Vector<Point> playerCyborgLocations = new Vector<Point>();
	Vector<Point> npcCyborgLocations = new Vector<Point>();
	final private int cyborgHeading = 0;
	final private int cyborgSpeed = 10;
	final private int cyborgSteeringDirection = 0;
	final private int cyborgMaxSpeed = 50;
	final private int cyborgEnergyLevel = 100;
	final private int cyborgDamageLevel = 0;
	final private int cyborgEnergyRate = 5;
	final private int cyborgLastBase = 1;
	final private int npcCyborgEnergyLevel = 300;
	final static int MAX_SPEED = 50;
	final static int MAX_DAMAGE = 100;
	final static int ARMOURED_MAX_DAMAGE = 200;
	final static int COLLISION_DAMAGE = 20;
	final static int ARMORED_COLLISION_DAMAGE = COLLISION_DAMAGE / 2;
	final static int ACCEL_AMOUNT = 10;
	final static int BRAKE_AMOUNT = -10;
	
	//Initial drone variables and values
	private int droneSize;
	final private int droneColor = ColorUtil.rgb(90,90,90); //Grey
	Vector<Point> droneLocations = new Vector<Point>();
	private int droneHeading;
	private int droneSpeed;
	
	//Initial energy station variables and values
	private int energyStationSize;
	final private int energyStationColor = ColorUtil.rgb(0,255,0); //Green
	Vector<Point> energyStationLocations = new Vector<Point>();
	private int energyStationCapacity;
	
	//Constructor
	public GameWorld() {
		theGameCollection = new GameObjectCollection();
		this.clock = 0;
		this.livesRemaining = 3;
		this.sound = false;
	}
	//Create all sounds for GameWorld 
	public void createSounds(SoundOnOff s) {
		//bgSound = new SoundLoop("alarm.wav");
		bgSound = new SoundLoop("Assignment4MaskOff_Trim.wav");
		System.out.println("Background sound created. Crash sound, Energy sound, Die sound still need to be created");
		
		//crashSound = new Sound("Assignment4CrashSound_Trim.wav");
		System.out.println("Background sound, Crash sound created. Energy sound, Die sound still need to be created");
		
		//energySound = new Sound("Assignment4EnergySound_Trim.wav");
		System.out.println("Background sound, Crash sound, Energy sound created. Die sound still needs to be created");
		
		//dieSound = new Sound("Assignment4DieSound_Trim.wav");
		System.out.println("Background sound, Crash sound, Energy sound, Die sound all have been created");
		//setSounds(bgSound, crashSound);
	}
	public void playBackgroundMusic() {
		if (this.getSound() == "ON") {
			bgSound.play();
		}
		else {
			bgSound.pause();
		}
		
	}
	public void playCrashSound() {
		if (this.getSound() == "ON") {
			crashSound.play2();
		}
	}
	public void playEnergySound() {
		if (this.getSound() == "ON") {
			energySound.play1();
		}
	}
	public void playDieSound() {
		if (this.getSound() == "ON") {
			dieSound.play2();
		}
	}
	public void setPlayMode(boolean b) {
		this.playMode = b;
		setChanged();
		notifyObservers();
	}
	public boolean getPlayMode() {
		return this.playMode;
	}
	public void setPositionMode(boolean b) {
		this.positionMode = b;
		setChanged();
		notifyObservers();
	}
	public boolean getPositionMode() {
		return this.positionMode;
	}
	//Game world initializer
	public void init(){
		//Add all Base, Cyborg, Drone, and Energy Station locations to Locations Vector
		baseLocations.addElement(new Point(0, 0));
		baseLocations.addElement(new Point(100, 200));
		baseLocations.addElement(new Point(200, 400));
		baseLocations.addElement(new Point(300, 500));
		setFinalBase(baseLocations.size());
		playerCyborgLocations.addElement(new Point(0, 0));
		npcCyborgLocations.addElement(new Point(0, 100));
		npcCyborgLocations.addElement(new Point(50, 50));
		npcCyborgLocations.addElement(new Point(100, 0));
		droneLocations.addElement(randomLocation());
		droneLocations.addElement(randomLocation());
		energyStationLocations.addElement(randomLocation());
		energyStationLocations.addElement(randomLocation());
		
		//Create all game objects
		//Add all Base objects to World vector from location vector 
		for(int i = 0; i<baseLocations.size(); i++) { 
			Point location = baseLocations.elementAt(i);
			Point baseLocation = adjustedLocation(location, baseSize);
			int baseSequenceNumber = i + 1;
			Base baseObj = new Base(baseSize, baseColor, baseLocation, baseSequenceNumber);
			baseObj.translate(baseLocation.getX(), baseLocation.getY());
			theGameCollection.add(baseObj);			
		}
		//Add all Player Cyborg objects to World vector from location vector
		for(int i = 0; i<playerCyborgLocations.size(); i++) { 
			PlayerCyborg playerCyborgObj = PlayerCyborg.getPlayerCyborg();
			theGameCollection.add(playerCyborgObj);
		}
		//Add all NPC Cyborg objects to World vector from location vector
		for(int i = 0; i<npcCyborgLocations.size(); i++) { 
			Point location = npcCyborgLocations.elementAt(i);
			Point npcCyborgLocation = adjustedLocation(location, cyborgSize);
			NonPlayerCyborg npcCyborgObj = new NonPlayerCyborg(cyborgSize, cyborgColor, npcCyborgLocation, cyborgHeading, cyborgSpeed, cyborgSteeringDirection, cyborgMaxSpeed, npcCyborgEnergyLevel, cyborgEnergyRate, cyborgDamageLevel, cyborgLastBase);
			IIterator theElements = theGameCollection.getIterator();
			while(theElements.hasNext()) {
				GameObject nextGameObject = theElements.getNext();
				//Base Strategy
				if (i % 2 == 0) { 
					//IIterator theBaseElements = theGameCollection.getIterator();
					if(nextGameObject instanceof Base) {
						Base nextBase = (Base)nextGameObject;
						int nextBaseNum = nextBase.getSequenceNumber();
						if(nextBaseNum == (npcCyborgObj.getLastBaseReached() + 1)) {
							IBaseStrategy baseStrategy = new IBaseStrategy(npcCyborgObj, nextBase);
							npcCyborgObj.setStrategy(baseStrategy);
						}
					}
				}
				//Attack Strategy
				else {
					//IIterator thePlayerElements = theGameCollection.getIterator();
					if(nextGameObject instanceof PlayerCyborg) {
						PlayerCyborg player = PlayerCyborg.getPlayerCyborg();
						IAttackStrategy attackStrategy = new IAttackStrategy(npcCyborgObj, player);
						npcCyborgObj.setStrategy(attackStrategy);
					}
				}
			}
			npcCyborgObj.translate(npcCyborgLocation.getX(), npcCyborgLocation.getY());
			npcCyborgObj.invokeStrategy();
			theGameCollection.add(npcCyborgObj);
		}
		//Add all Drone objects to World vector from location vector 
		for(int i = 0; i<droneLocations.size(); i++) {
			Random ranObj = new Random(); //create random object for size
			droneSize = 10 + ranObj.nextInt(10); //set size to 10-20 of random value
			Point location = droneLocations.elementAt(i);
			Point droneLocation = adjustedLocation(location, droneSize);
			droneHeading = ranObj.nextInt(359); //set heading to 0-359 of random value
			droneSpeed = 20 + ranObj.nextInt(10); //set speed to 20-30 of random value
			Drone droneObj = new Drone(droneSize, droneColor, droneLocation, droneHeading, droneSpeed);
			droneObj.translate(droneLocation.getX(), droneLocation.getY());
			theGameCollection.add(droneObj);	
		}
		//Add all Energy Station objects to World vector from location vector
		for(int i = 0; i<energyStationLocations.size(); i++) {  
			Random ranObj = new Random(); //create random object for size
			energyStationSize = 50 + ranObj.nextInt(20); //set size to 50-70 of random value
			Point location = energyStationLocations.elementAt(i);
			Point energyStationLocation = adjustedLocation(location, energyStationSize);
			energyStationCapacity = (int)(energyStationSize * 0.5);
			EnergyStation energyStationObj = new EnergyStation(energyStationSize, energyStationColor, energyStationLocation, energyStationCapacity);
			energyStationObj.translate(energyStationLocation.getX(), energyStationLocation.getY());
			theGameCollection.add(energyStationObj);	
		}
		setChanged();
		notifyObservers();
	}
	protected Point adjustedLocation(Point location, int size) {
		float x = location.getX(); //adjust X location with object size padding
		float y = location.getY(); //adjust Y location with object size padding
		if ((x - (size/2)) < 0) {
			x = x + (size/2);
		}
		if ((x + (size/2)) > 1000) {
			x = x - (size/2);
		}
		if ((y - (size/2)) < 0) {
			y = y + (size/2);
		}
		if ((y + (size/2)) > 1000) {
			y = y - (size/2);
		}
		return new Point(x,y);
	}
	
	//Re-initialize game
	public void reset() {
		baseLocations.clear();
		playerCyborgLocations.clear();
		npcCyborgLocations.clear();
		droneLocations.clear();
		energyStationLocations.clear();
		theGameCollection.clear();
		PlayerCyborg pObj = PlayerCyborg.getPlayerCyborg();
		pObj.reset();
		init();
	}
	
	//Get current Game Collection
	public GameObjectCollection getTheGameCollection() {
		return this.theGameCollection;
	}
	//String format for color 
	protected String colorToString(int color) {
		String colorString = "[" + ColorUtil.red(color) + "," + ColorUtil.green(color) + "," + ColorUtil.blue(color) + "]";
		return colorString;
	}
	//Random locations
	protected Point randomLocation() {
		Random ranObj = new Random(); //create random object for location x,y
		int x = ranObj.nextInt(500); //set x to 0-500 of random value
		int y = ranObj.nextInt(500); //set y to 0-500 of random value
		Point location = new Point(x,y);
		return location;
	}
	//Set and get Map size
	public void setMapSize(int x, int y) {
		this.mapWidth = x;
		this.mapHeight = y;
		System.out.println("Map Size: " + mapWidth + ", " + mapHeight);
		setChanged();
		notifyObservers();
	}
	public int getMapWidth() {
		return this.mapWidth;
	}
	public int getMapHeight() {
		return this.mapHeight;
	}
	protected void gameOverWon() {
		this.won = true;
	}
	public boolean getGameOverWon() {
		return this.won;
	}
	///Set and get Non Player Cyborg won flag
	protected void gameOverNPC() {
		this.npcWon = true;
	}
	public boolean getGameOverNPC() {
		return this.npcWon;
	}
	//Set and get clock
	protected void clockTick() {
		this.clock++;
	}
	public int getClock() {
		return this.clock;
	}
	public int getTimeInSecs() {
		int timeSecs = getClock() / 50; //Clock ticks every 20 millisecs (20*50 = 1 sec)
		return timeSecs;
	}
	//Set and get highest base reached
	protected void setHighestBaseReached(int baseNum) {
		this.highestBaseReached = baseNum;
	}
	public int getHighestBaseReached() {
		return this.highestBaseReached;
	}
	//Set and get final base
	protected void setFinalBase(int baseNum) {
		this.finalBase = baseNum;
	}
	public int getFinalBase() {
		return this.finalBase;
	}
	//Set and get lives left
	protected void loseLife() {
		this.playDieSound();
		this.livesRemaining--;
	}
	public int getLivesRemaining() {
		return this.livesRemaining;
	}
	//Set and get sound
	public void setSound(boolean b) {
		this.sound = b;	
		setChanged();
		notifyObservers();
	}
	public String getSound() {
		if (this.sound == false)
			return "OFF";
		else
			return "ON";
	}
	
	//Command 'd'
	protected String score() {
		String output = "";
		String time = Integer.toString(getClock());
		String sound = getSound();
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof PlayerCyborg) {
				PlayerCyborg cObj = PlayerCyborg.getPlayerCyborg();
				String lives = Integer.toString(this.livesRemaining);
				String lastBase = Integer.toString(cObj.getLastBaseReached());
				String energy = Integer.toString(cObj.getEnergyLevel());
				String damage = Integer.toString(cObj.getDamageLevel());
				output = "Time: " + getClock()/50 + "," + getMapHeight() + "     Lives left: " + lives + "     Highest base reached: " + lastBase + "     Energy level: " + energy + "     Damage level: " + damage + "    Sound: " + sound;
			}
		}
		return output;
	}
	//Command 'm'
	protected String map() {
		String output = "";
		//Base display
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof Base) {
				//Game object attributes
				Base bObj = (Base)nextGameObject;
				Point base = bObj.getLocation();
				String location = Float.toString(base.getX()) + "," + Float.toString(base.getY());
				String color = colorToString(bObj.getColor());
				String size = Integer.toString(bObj.getSize());
				//Base object attributes
				String seqNum = Integer.toString(bObj.getSequenceNumber());
				//Output all attributes
				output = output + "Base: " + "loc=" + location + " color=" + color + " size=" + size + " seqNum" + seqNum + "\n";
			}
			if (nextGameObject instanceof PlayerCyborg) {
				//Game object attributes
				PlayerCyborg pObj = PlayerCyborg.getPlayerCyborg();
				Point cyborg = pObj.getLocation();
				String location = Float.toString(Math.round((10.0*cyborg.getX())/10.0)) + "," + Float.toString(Math.round((10.0*cyborg.getY())/10.0));
				String color = colorToString(pObj.getColor());
				String size = Integer.toString(pObj.getSize());
				//Movable object attributes
				String heading = Integer.toString(pObj.getHeading());
				String speed = Integer.toString(pObj.getSpeed());
				//Cyborg object attributes
				String maxSpeed = Integer.toString(pObj.getMaximumSpeed());
				String steeringDirection = Integer.toString(pObj.getSteeringDirection());
				String energyLevel = Integer.toString(pObj.getEnergyLevel());
				String damageLevel = Integer.toString(pObj.getDamageLevel());
				//Output all attributes
				output = output + "Player Cyborg: " + "loc=" + location + " color=" + color + " size=" + size + " heading=" + heading + " speed=" + speed + "\n"
								+ " maxSpeed=" + maxSpeed + " steeringDirection=" + steeringDirection + " energyLevel=" + energyLevel + " damageLevel=" + damageLevel + "\n";
			}
			if (nextGameObject instanceof NonPlayerCyborg) {
				NonPlayerCyborg npcObj = (NonPlayerCyborg)nextGameObject;
				output = output + npcObj.toString();
			}
			if (nextGameObject instanceof Drone) {
				//Game object attributes
				Drone dObj = (Drone)nextGameObject;
				Point drone = dObj.getLocation();
				String location = Float.toString(Math.round((10.0*drone.getX())/10.0)) + "," + Float.toString(Math.round((10.0*drone.getY())/10.0));
				String color = colorToString(dObj.getColor());
				String size = Integer.toString(dObj.getSize());
				//Movable object attributes
				String heading = Integer.toString(dObj.getHeading());
				String speed = Integer.toString(dObj.getSpeed());
				//Output all attributes
				output = output + "Drone: " + "loc=" + location + " color=" + color + " size=" + size + " heading=" + heading + " speed=" + speed + "\n";
			}
			if (nextGameObject instanceof EnergyStation) {
				EnergyStation eObj = (EnergyStation)nextGameObject;
				//Game object attributes
				Point energyStation = eObj.getLocation();
				String location = Float.toString(Math.round((10.0*energyStation.getX())/10.0)) + "," + Float.toString(Math.round((10.0*energyStation.getY())/10.0));
				String color = colorToString(eObj.getColor());
				String size = Integer.toString(eObj.getSize());
				//EnergyStation object attributes
				String capacity = Integer.toString(eObj.getCapacity());
				//Output all attributes
				output = output + "EnergyStation: " + "loc=" + location + " color=" + color + " size=" + size + " capacity=" + capacity + "\n";
			}
		}
		return output;
	}
	
	//Command 't'
	protected void tick() {
		clockTick();
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof Cyborg) {
				Cyborg cObj = (Cyborg)nextGameObject;
				if ((getClock() % 50) == 0) {
					int oldEnergyLevel = cObj.getEnergyLevel();
					int energyConsumptionRate = cObj.getEnergyConsumptionRate();
					int newEnergyLevel = oldEnergyLevel - energyConsumptionRate;
					cObj.setEnergyLevel(newEnergyLevel);
				}
				//Approach target for NPC and update
				if (nextGameObject instanceof NonPlayerCyborg) {
					NonPlayerCyborg npcObj = (NonPlayerCyborg)nextGameObject;
					npcObj.reachTarget();
					int amountToSteer = npcObj.getSteeringDirection();
					int oldHeading = npcObj.getHeading();
					int newHeading = oldHeading + amountToSteer;
					if(newHeading > 360) {
						newHeading = newHeading - 360;
					}
					if(newHeading < 0) {
						newHeading = newHeading + 360;
					}
					npcObj.rotate(-amountToSteer);
					npcObj.setHeading(newHeading);
					npcObj.setSteeringDirection(0);
					npcObj.move(elapsedTime, mapWidth, mapHeight);
				}
				//Apply update to Player Cyborg
				else {
					PlayerCyborg pcObj = PlayerCyborg.getPlayerCyborg();
					//Apply new heading if damage, energy, and speed all at acceptable levels
					if(pcObj.getDamageLevel() < MAX_DAMAGE && pcObj.getEnergyLevel() > 0 && pcObj.getSpeed() > 0) {
						int amountToSteer = pcObj.getSteeringDirection();
						int oldHeading = pcObj.getHeading();
						int newHeading = oldHeading + amountToSteer;
						if(newHeading > 360) {
							newHeading = newHeading - 360;
						}
						if(newHeading < 0) {
							newHeading = newHeading + 360;
						}
						pcObj.setHeading(newHeading);
						pcObj.setSteeringDirection(0);
						pcObj.move(elapsedTime, mapWidth, mapHeight);
						if(getClock() % 50 == 0) {
							pcObj.updateLTs();
						}
					}
					else {
						loseLife();
						reset();
					}
				}
			}
			if (nextGameObject instanceof Drone) {
				//Update random heading
				Drone dObj = (Drone)nextGameObject;
				dObj.updateHeading();
				dObj.move(elapsedTime, mapWidth, mapHeight);
			}
			if (nextGameObject instanceof Cyborg) {
				IIterator theOtherElements = theGameCollection.getIterator();
				while(theOtherElements.hasNext()){
					GameObject otherObject = (GameObject) theOtherElements.getNext(); // get a collidable object
					if (nextGameObject.collidesWith(otherObject)) {
						collision(nextGameObject, otherObject);
					}
					nextGameObject.updateCollisionElements(otherObject);
				}
			}
			if (nextGameObject instanceof EnergyStation) {
				EnergyStation eObj = (EnergyStation)nextGameObject;
				eObj.setSelected(false);
			}
			if (nextGameObject instanceof Shockwave) {
				Shockwave sObj = (Shockwave)nextGameObject;
				sObj.move(elapsedTime, 2000, 2000);
			}
		}
		setChanged();
		notifyObservers();
	}
	
	protected void collision(GameObject obj1, GameObject obj2) {
		if (obj1 != obj2) {
			if (obj1 instanceof Cyborg) {
				if (obj1.alreadyCollidesWith(obj2) == false) {
					obj1.handleCollision(obj2, this);
					//obj2.handleCollision(obj1, this);
					obj1.addCollisionElement(obj2);
					obj2.addCollisionElement(obj1);
					//this.setCollision(true);
				}
				//obj1.updateCollisionElements(obj2);
				//obj2.updateCollisionElements(obj1);
			}
		}
		setChanged();
		notifyObservers();
	}
	
	protected void updateBaseStrategy(NonPlayerCyborg npc) {
		int nextBaseNum = npc.getLastBaseReached() + 1;
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextBaseObject = theElements.getNext();
			if (nextBaseObject instanceof Base) {
				Base nextBase = (Base)nextBaseObject;
				if (nextBaseNum == nextBase.getSequenceNumber()) {
					IBaseStrategy baseStrategy = new IBaseStrategy(npc, nextBase);
					npc.setStrategy(baseStrategy);
					npc.invokeStrategy();
				}
			}
		}
		setChanged();
		notifyObservers();
	}
	
	protected void baseReached(int baseNumber, Cyborg cObj) {
		String output = "";
		System.out.println("--Base Reached");
		int lastBase = cObj.getLastBaseReached();
		int nextBase =  lastBase + 1;
		int finalBase = baseLocations.size();
		if (baseNumber == nextBase && baseNumber == finalBase) {
			cObj.setLastBaseReached(baseNumber);
			if (cObj instanceof PlayerCyborg) {
				setHighestBaseReached(baseNumber);
				gameOverWon();
			}
			if (cObj instanceof NonPlayerCyborg) {
				gameOverNPC();
			}
			//output = gameOver();
					
		}
		else if (baseNumber == nextBase) {
			cObj.setLastBaseReached(baseNumber);
			if (cObj instanceof PlayerCyborg) {
				setHighestBaseReached(baseNumber);
			}
			if (cObj instanceof NonPlayerCyborg) {
				updateBaseStrategy((NonPlayerCyborg)cObj);
			}
		}
		else {
			output = "Base not next in sequence \n"
					+ "Next sequential base: " + nextBase;
		}
		setChanged();
		notifyObservers();
	}
	
	protected void collisionCyborg(Cyborg cObj1, Cyborg cObj2) {
		//Cyborg 1
		if (cObj1 instanceof PlayerCyborg) {
			//Calculate new damage of Player Cyborg
			int oldDamageLevel = cObj1.getDamageLevel();
			int newDamageLevel = oldDamageLevel + COLLISION_DAMAGE;
			if(newDamageLevel < MAX_DAMAGE) {
				//Apply new damage and calculate adjusted speed
				cObj1.setDamageLevel(newDamageLevel);
				double damagePercentage = (double)COLLISION_DAMAGE / (double)MAX_DAMAGE;
				double oldMaxSpeed = (double)cObj1.getMaximumSpeed();
				int newMaxSpeed = (int)Math.round(oldMaxSpeed - (MAX_SPEED * damagePercentage));
				cObj1.setMaximumSpeed(newMaxSpeed);
				if(cObj1.getSpeed() > newMaxSpeed) {
					//Apply new speed
					cObj1.setSpeed(newMaxSpeed);
				}
				cObj1.changeColor(50);
			}
			else {
				//Decrement lives remaining 
				loseLife();
				if(this.livesRemaining > 0) {
					reset();
				}
			}
		}
		else {		
			//Calculate new damage of Non Player Cyborg
			int oldDamageLevel = cObj1.getDamageLevel();
			int newDamageLevel = oldDamageLevel + ARMORED_COLLISION_DAMAGE;
			if (newDamageLevel > MAX_DAMAGE) {
				newDamageLevel = MAX_DAMAGE;
			}
			cObj1.setDamageLevel(newDamageLevel);
			double damagePercentage = (double)ARMORED_COLLISION_DAMAGE / (double)MAX_DAMAGE;
			double oldMaxSpeed = (double)cObj1.getMaximumSpeed();
			int newMaxSpeed = (int)Math.round(oldMaxSpeed - (MAX_SPEED * damagePercentage));
			cObj1.setMaximumSpeed(newMaxSpeed);
			if(cObj1.getSpeed() > newMaxSpeed) {
				//Apply new speed
				cObj1.setSpeed(newMaxSpeed);
			}
			cObj1.changeColor(50);
		}
		
		// Cyborg 2
		if (cObj2 instanceof PlayerCyborg) {
			//Calculate new damage of Cyborg 1
			int oldDamageLevel = cObj2.getDamageLevel();
			int newDamageLevel = oldDamageLevel + COLLISION_DAMAGE;
			if(newDamageLevel < MAX_DAMAGE) {
				//Apply new damage and calculate adjusted speed
				cObj2.setDamageLevel(newDamageLevel);
				double damagePercentage = (double)COLLISION_DAMAGE / (double)MAX_DAMAGE;
				double oldMaxSpeed = (double)cObj2.getMaximumSpeed();
				int newMaxSpeed = (int)Math.round(oldMaxSpeed - (MAX_SPEED * damagePercentage));
				cObj2.setMaximumSpeed(newMaxSpeed);
				if(cObj2.getSpeed() > newMaxSpeed) {
					//Apply new speed
					cObj2.setSpeed(newMaxSpeed);
				}
				cObj2.changeColor(50);
			}
			else {
				//Decrement lives remaining 
				loseLife();
				if(this.livesRemaining > 0) {
					reset();
				}
			}
		}
		else {		
			//Calculate new damage of Non Player Cyborg
			int oldDamageLevel = cObj2.getDamageLevel();
			int newDamageLevel = oldDamageLevel + ARMORED_COLLISION_DAMAGE;
			if (newDamageLevel > MAX_DAMAGE) {
				newDamageLevel = MAX_DAMAGE;
			}
			cObj2.setDamageLevel(newDamageLevel);
			double damagePercentage = (double)ARMORED_COLLISION_DAMAGE / (double)MAX_DAMAGE;
			double oldMaxSpeed = (double)cObj2.getMaximumSpeed();
			int newMaxSpeed = (int)Math.round(oldMaxSpeed - (MAX_SPEED * damagePercentage));
			cObj2.setMaximumSpeed(newMaxSpeed);
			if(cObj2.getSpeed() > newMaxSpeed) {
				//Apply new speed
				cObj2.setSpeed(newMaxSpeed);
			}
			cObj2.changeColor(50);
		}
		//playCrashSound();
		setChanged();
		notifyObservers();
	}
	
	protected void energyStationReached(Cyborg cObj, Point energyStationLocation) {
		System.out.println("*****Energy Reached");
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof EnergyStation) {
				EnergyStation eObj = (EnergyStation)nextGameObject;
				if (((int)energyStationLocation.getX() == (int)eObj.getLocation().getX()) && ((int)energyStationLocation.getY() == (int)eObj.getLocation().getY())) {
					//Get energy capacity if correct location	
					int energyCapacity = eObj.getCapacity();
					if (energyCapacity != 0) {
						//Adjust Cyborg energy level, clear Energy Station capacity, change color
						int oldEnergyLevel = cObj.getEnergyLevel();
						int newEnergyLevel = oldEnergyLevel + energyCapacity;
						cObj.setEnergyLevel(newEnergyLevel);
						eObj.setCapacity(0);
						eObj.changeColor(100);
						addEnergyStation();
					}
				}
			}
		}
		setChanged();
		notifyObservers();
	}
	
	protected void collisionDrone(Cyborg cObj) {
		//Player Cyborg 
		if (cObj instanceof PlayerCyborg) {
			//Calculate new damage of Player Cyborg
			int oldDamageLevel = cObj.getDamageLevel();
			int newDamageLevel = oldDamageLevel + COLLISION_DAMAGE;
			if(newDamageLevel < MAX_DAMAGE) {
				//Apply new damage and calculate adjusted speed
				cObj.setDamageLevel(newDamageLevel);
				double damagePercentage = (double)COLLISION_DAMAGE / (double)MAX_DAMAGE;
				double oldMaxSpeed = (double)cObj.getMaximumSpeed();
				int newMaxSpeed = (int)Math.round(oldMaxSpeed - (MAX_SPEED * damagePercentage));
				cObj.setMaximumSpeed(newMaxSpeed);
				if(cObj.getSpeed() > newMaxSpeed) {
					//Apply new speed
					cObj.setSpeed(newMaxSpeed);
				}
				cObj.changeColor(50);
			}
			else {
				//Decrement lives remaining
				loseLife();
				if (this.livesRemaining > 0) {
					reset();
				}
			}
		}
		// Non Player Cyborg
		if (cObj instanceof NonPlayerCyborg) {
			//Calculate new damage of Non Player Cyborg
			int oldDamageLevel = cObj.getDamageLevel();
			int newDamageLevel = oldDamageLevel + ARMORED_COLLISION_DAMAGE;
			if (newDamageLevel > MAX_DAMAGE) {
				newDamageLevel = MAX_DAMAGE;
			}
			cObj.setDamageLevel(newDamageLevel);
			double damagePercentage = (double)ARMORED_COLLISION_DAMAGE / (double)MAX_DAMAGE;
			double oldMaxSpeed = (double)cObj.getMaximumSpeed();
			int newMaxSpeed = (int)Math.round(oldMaxSpeed - (MAX_SPEED * damagePercentage));
			cObj.setMaximumSpeed(newMaxSpeed);
			if(cObj.getSpeed() > newMaxSpeed) {
				//Apply new speed
				cObj.setSpeed(newMaxSpeed);
			}
			cObj.changeColor(50);
		}
		setChanged();
		notifyObservers();
	}
	
	//Command 'l'
	protected void left() {
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof PlayerCyborg) {
				PlayerCyborg cObj = PlayerCyborg.getPlayerCyborg();
				if(cObj.getSteeringDirection() % 5 == 0 && cObj.getSteeringDirection() > -40) {
					cObj.steeringDirection(-5);
				}
			}
		}
		setChanged();
		notifyObservers();
	}
	
	//Command 'r'
	protected void right() {
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof PlayerCyborg) {
				PlayerCyborg cObj = PlayerCyborg.getPlayerCyborg();
				if(cObj.getSteeringDirection() % 5 == 0 && cObj.getSteeringDirection() < 40) {
					cObj.steeringDirection(5);
				}
			}
		}
		setChanged();
		notifyObservers();
	}
	
	//Command 'a'
	protected void accelerate() {
		String output = "";
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof PlayerCyborg) {
				PlayerCyborg cObj = PlayerCyborg.getPlayerCyborg();
				if(cObj.getSpeed() >= cObj.getMaximumSpeed()) {
					output = "Can't accelerate \n"
							+ "Already at max speed \n";
				}
				else if(cObj.getSpeed() < (cObj.getMaximumSpeed() - ACCEL_AMOUNT)) {
					cObj.changeSpeed(ACCEL_AMOUNT);
					output = "Accelerate " + ACCEL_AMOUNT + " units per tick \n";
				}
				else {
					cObj.setSpeed(cObj.getMaximumSpeed());
					output = "Accelerate to max speed! \n";
				}
			}
		}
		setChanged();
		notifyObservers();
	}
	
	//Command 'b'
	protected void brake() {
		String output = "";
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof PlayerCyborg) {
				PlayerCyborg cObj = PlayerCyborg.getPlayerCyborg();
				Movable mObj = (Movable)nextGameObject;
				if(mObj.getSpeed() <= 0) {
					output = "Can't brake \n"
							+ "Already stopped \n";
				}
				else if(mObj.getSpeed() > Math.abs(BRAKE_AMOUNT)) {
					cObj.changeSpeed(BRAKE_AMOUNT);
					output = "Brake " + BRAKE_AMOUNT + " units per tick \n";
				}
				else {
					mObj.setSpeed(0);
					output = "Cyborg stopped! \n";
				}
			}
		}
		setChanged();
		notifyObservers();
	}
	
	//Command 'c'
	
	
	//Command 'g'
	
	
	//Random energy station chosen for you
	protected int pickEnergyStation() {
		Random ranObj = new Random(); //create random object for size
		int size = energyStationLocations.size(); //get number of energy stations
		int randomEnergyStation = 1 + ranObj.nextInt(size); //set size to 1-NumOfEnergyStations of random value
		return randomEnergyStation;
	}
	
	//Command 'e'
	
	
	//Add new Energy Station
	protected void addEnergyStation() {
		Random ranObj = new Random(); //create random object for size
		energyStationSize = 50 + ranObj.nextInt(50); //set size to 50-100 of random value
		energyStationLocations.addElement(randomLocation()); //add energy station location to location vector
		Point energyStationLocation = energyStationLocations.lastElement();
		energyStationCapacity = (int)(energyStationSize * 0.5);
		EnergyStation energyStationObj = new EnergyStation(energyStationSize, energyStationColor, energyStationLocation, energyStationCapacity);
		energyStationObj.translate(energyStationLocation.getX(), energyStationLocation.getY());
		theGameCollection.add(energyStationObj); //add energy station to world vector
	}
	
	//Add new Shockwave
	protected void addShockwave(Point location) {
		Random ranObj = new Random(); //create random object for size
		int size = 50 + ranObj.nextInt(30); //set size to 50-80 of random value
		int color = ColorUtil.MAGENTA;
		int heading = 0 + ranObj.nextInt(359);
		int speed = 50 + ranObj.nextInt(50);
		Shockwave shockwaveObj = new Shockwave(size, color, location, heading, speed);
		shockwaveObj.translate(location.getX(), location.getY());
		theGameCollection.add(shockwaveObj);
	}
	
	//Command Change Strategies
	protected void changeStrategies() {
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof NonPlayerCyborg) {
				NonPlayerCyborg npcObj = (NonPlayerCyborg)nextGameObject;
				int nextBaseNum = npcObj.getLastBaseReached() + 1;
				//Change from Attack to Base strategy
				if (npcObj.getStrategy() instanceof IAttackStrategy) {
					IIterator theBaseElements = theGameCollection.getIterator();
					while(theBaseElements.hasNext()) {
						GameObject nextBaseObject = theBaseElements.getNext();
						if(nextBaseObject instanceof Base) {
							Base nextBase = (Base)nextBaseObject;
							if(nextBase.getSequenceNumber() == nextBaseNum) {
								IBaseStrategy baseStrategy = new IBaseStrategy(npcObj, nextBase);
								npcObj.setStrategy(baseStrategy);
							}
						}
					}
				}
				//Change from Base to Attack strategy
				else if (npcObj.getStrategy() instanceof IBaseStrategy) {
					IIterator thePlayerElements = theGameCollection.getIterator();
					while(thePlayerElements.hasNext()) {
						GameObject nextPlayerObject = thePlayerElements.getNext();
						if(nextPlayerObject instanceof PlayerCyborg) {
							PlayerCyborg player = PlayerCyborg.getPlayerCyborg();
							IAttackStrategy attackStrategy = new IAttackStrategy(npcObj, player);
							npcObj.setStrategy(attackStrategy);	
						}
					}
				}
				npcObj.invokeStrategy();
				//npcObj.setLastBaseReached(nextBaseNum);
				if (npcObj.getLastBaseReached() == getFinalBase()) {
					gameOverNPC();
				}
			}
		}	
		setChanged();
		notifyObservers();
	}
	
}