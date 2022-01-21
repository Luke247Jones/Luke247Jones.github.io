package com.mycompany.a4;

import com.codename1.charts.util.ColorUtil;
import com.codename1.charts.models.Point;

public abstract class Cyborg extends Movable implements ISteerable, IChangeSpeed, IChangeColor{
	private int steeringDirection;
	private int maximumSpeed;
	private int energyLevel;
	private int energyConsumptionRate;
	private int damageLevel;
	private int lastBaseReached;

	public Cyborg(int size, int color, Point location, int heading, int speed, int steeringDirection, int maximumSpeed, int energyLevel, int energyConsumptionRate, int damageLevel, int lastBaseReached) {
		super(size, color, location, heading, speed);
		this.steeringDirection = steeringDirection;
		this.maximumSpeed = maximumSpeed;
		this.energyLevel = energyLevel;
		this.energyConsumptionRate = energyConsumptionRate;
		this.damageLevel = damageLevel;
		this.lastBaseReached = lastBaseReached;
	}
	public void steeringDirection(int amount) {
		int oldSteeringDirection = getSteeringDirection();
		int newSteeringDirection = oldSteeringDirection + amount;
		setSteeringDirection(newSteeringDirection);
		this.rotate(-amount);
	}
	public void changeSpeed(int amount) {
		int oldSpeed = super.getSpeed();
		int newSpeed = oldSpeed + amount;
		super.setSpeed(newSpeed);
	}
	public void changeColor(int amount) {
		int oldColor = super.getColor();
		int red = ColorUtil.red(oldColor);
		int newColor = ColorUtil.rgb(red - amount, 0, 0);
		super.setColor(newColor);
	}
	public int getSteeringDirection() {
		return this.steeringDirection;
	}
	public int getMaximumSpeed() {
		return this.maximumSpeed;
	}
	public int getEnergyLevel() {
		return this.energyLevel;
	}
	public int getEnergyConsumptionRate() {
		return this.energyConsumptionRate;
	}
	public int getDamageLevel() {
		return this.damageLevel;
	}
	public int getLastBaseReached() {
		return this.lastBaseReached;
	}
	public void setSteeringDirection(int steeringDirection) {
		this.steeringDirection = steeringDirection;
	}
	public void setMaximumSpeed(int maximumSpeed) {
		this.maximumSpeed = maximumSpeed;
	}
	public void setEnergyLevel(int energyLevel) {
		this.energyLevel = energyLevel;
	}
	public void setDamageLevel(int damageLevel) {
		this.damageLevel = damageLevel;
	}
	public void setLastBaseReached(int lastBaseReached) {
		this.lastBaseReached = lastBaseReached;
	}
	
	public void handleCollision(GameObject otherObject, GameWorld gw) {
		if (otherObject instanceof Base) {
			Base bObj = (Base)otherObject;
			int baseNum = bObj.getSequenceNumber();
			gw.baseReached(baseNum, this);
			//gw.playCrashSound();
		}
		if (otherObject instanceof Cyborg) {
			Cyborg cObj = (Cyborg)otherObject;
			gw.collisionCyborg(this, cObj);
			float collisionX = otherObject.getLocation().getX();
			float collisionY = otherObject.getLocation().getY();
			Point collisionLocation = new Point(collisionX, collisionY);
			gw.addShockwave(collisionLocation);
			gw.playCrashSound();
		}
		if (otherObject instanceof EnergyStation) {
			EnergyStation eObj = (EnergyStation)otherObject;
			Point energyLocation = eObj.getLocation();
			//System.out.println("Energy Collision at " + eObj.getLocation().getX() + ", " + eObj.getLocation().getY());
			gw.energyStationReached(this, energyLocation);
			gw.playEnergySound();
		}
		if (otherObject instanceof Drone) {
			gw.collisionDrone(this);
		}
	}
}
