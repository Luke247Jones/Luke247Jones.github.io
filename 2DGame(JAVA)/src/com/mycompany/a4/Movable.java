package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.ui.Graphics;
import com.codename1.ui.Transform;

import java.lang.Math;

public abstract class Movable extends GameObject{
	private int heading;
	private int speed;
	
	
	
	public Movable(int size, int color, Point location, int heading, int speed) {
		super(size, color, location);
		this.heading = heading;
		this.speed = speed;
		
		
	}
	public int getHeading() {
		return this.heading;
	}
	public int getSpeed() {
		return this.speed;
	}
	public void setHeading(int heading) {
		this.heading = heading;
	}
	public void setSpeed(int speed) {
		this.speed = speed;
	}
	
	public void move(int elapsedTime, int boundaryX, int boundaryY) {
		Point oldLocation = getLocation();
		float x = oldLocation.getX();
		float y = oldLocation.getY();
		double degree = 90 - getHeading();
		double radianAngle = Math.toRadians(degree);
		int speed = getSpeed(); /// (1000 / elapsedTime);
		int adjustedTime = 1000 / elapsedTime;
		float deltaX = (float)((Math.cos(radianAngle) * speed) / adjustedTime);
		float deltaY = (float)((Math.sin(radianAngle) * speed) / adjustedTime);
			
		Point newLocation = new Point((x + deltaX), (y + deltaY));
		this.translate(deltaX, deltaY);
		
		if(newLocation.getX() > boundaryX || newLocation.getY() > boundaryY || newLocation.getX() < 0 || newLocation.getY() < 0) {
			int oldHeading = getHeading();
			if(oldHeading < 180) {
				int newHeading = oldHeading + 180;
				setHeading(newHeading);
			}
			else {
				int newHeading = oldHeading - 180;
				setHeading(newHeading);
			}
			newLocation = oldLocation;
		}
		//super.setLocation(newLocation);
	}
	public void draw(Graphics g, Point parentOrigin) {}
}
