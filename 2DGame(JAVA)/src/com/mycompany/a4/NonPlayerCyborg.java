package com.mycompany.a4;

import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Graphics;
import com.codename1.ui.Transform;
import com.codename1.charts.models.Point;

public class NonPlayerCyborg extends Cyborg {
	private IStrategy currentStrategy;
	private Graphics npcGraphics;
	
	public NonPlayerCyborg(int size, int color, Point location, int heading, int speed, int steeringDirection, int maximumSpeed, int energyLevel, int energyConsumptionRate, int damageLevel, int lastBaseReached) {
		super(size, color, location, heading, speed, steeringDirection, maximumSpeed, energyLevel, energyConsumptionRate, damageLevel, lastBaseReached);
	}
	public void setStrategy(IStrategy strategy) {
		this.currentStrategy = strategy;
	}
	public IStrategy getStrategy() {
		return this.currentStrategy;
	}
	public void invokeStrategy() {
		this.currentStrategy.apply();
	}
	public void reachTarget() {
		this.currentStrategy.adjust();
	}
	public String toString() {
		int size = super.getSize();
		String color = "[" + ColorUtil.red(super.getColor()) + "," + ColorUtil.green(super.getColor()) + "," + ColorUtil.blue(super.getColor()) + "]";
		String locationXY = Float.toString(Math.round((10.0*super.getLocation().getX())/10.0)) + "," + Float.toString(Math.round((10.0*super.getLocation().getY())/10.0));
		int heading = super.getHeading();
		int speed = super.getSpeed();
		int steeringDirection = super.getSteeringDirection();
		int maximumSpeed = super.getMaximumSpeed();
		int energyLevel = super.getEnergyLevel();
		int damageLevel = super.getDamageLevel();
		String strategyName;
		if(currentStrategy instanceof IBaseStrategy) {
			strategyName = "Next Base";
		}
		else {
			strategyName = "Attack Player";
		}
		String output = "Non-Player Cyborg: " + "loc=" + locationXY + " color=" + color + " size=" + size + " heading=" + heading + " speed=" + speed + "\n"
				+ " maxSpeed=" + maximumSpeed + " steeringDirection=" + steeringDirection + " energyLevel=" + energyLevel + " damageLevel=" + damageLevel + "\n" + " currentStrategy=" + strategyName + "\n";
		return output;
	}
	public void draw(Graphics g, Point parentOrigin, Point screenOrigin) {
		int parentX = (int)parentOrigin.getX();
		int parentY = (int)parentOrigin.getY();
		int playerX = (int)getLocation().getX();
		int playerY = (int)getLocation().getY();
		int centerX = parentX;
		int centerY = parentY;
		
		Transform gXform = Transform.makeIdentity();
		g.getTransform(gXform);
		Transform gOrigXform = gXform.copy(); //save the original xform

		gXform.translate(screenOrigin.getX(), screenOrigin.getY());
		gXform.translate(myTranslation.getTranslateX(), myTranslation.getTranslateY());
		gXform.concatenate(myRotation);
		gXform.scale(myScale.getScaleX(), myScale.getScaleY());
		gXform.translate(-screenOrigin.getX(), -screenOrigin.getY());
		g.setTransform(gXform);
		//draw sub-shapes of FireOval
		g.drawRect((int)centerX - (getSize()/2), (int)centerY - (getSize()/2), getSize(), getSize());
		
		System.out.println("Non Player Cyborg origin: " + myTranslation.getTranslateX() + "," + myTranslation.getTranslateY());
		npcGraphics = g;
		npcGraphics.setColor(getColor());
		npcGraphics.drawRect((int)centerX - (getSize()/2), (int)centerY - (getSize()/2), getSize(), getSize());
		//npcGraphics.drawRect(centerX - (getSize()/2), centerY - (getSize()/2), getSize(), getSize());
		g.setTransform(gOrigXform);
	}
}