package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Graphics;
import com.codename1.ui.Transform;

import java.util.Random;

public class Drone extends Movable {
	public Graphics droneGraphics;
	
	public Drone(int size, int color, Point location, int heading, int speed) {
		super(size, color, location, heading, speed);
	}
	public void updateHeading() {
		int newHeading = super.getHeading();
		Random ranObj = new Random();
		int randomInt = ranObj.nextInt(100);
		if(randomInt % 2 == 0) {
			newHeading = newHeading + 5;
			this.rotate(-5);
			super.setHeading(newHeading);
		}
		else {
			newHeading = newHeading - 5;
			this.rotate(5);
			super.setHeading(newHeading);
		}
	}
	public void setColor(int color) {
		super.setColor(color);
	}
	public void draw(Graphics g, Point parentOrigin, Point screenOrigin) {
		int parentX = (int)parentOrigin.getX();
		int parentY = (int)parentOrigin.getY();
		int playerX = (int)getLocation().getX();
		int playerY = (int)getLocation().getY();
		int centerX = parentX;
		int centerY = parentY;
		int[] triangleX = new int[] {centerX - (getSize()/2), centerX, centerX + (getSize()/2)};
		int[] triangleY = new int[] {centerY - (getSize()/2), centerY + (getSize()/2), centerY - (getSize()/2)};
		
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
		droneGraphics = g;
		droneGraphics.setColor(getColor());
		droneGraphics.drawPolygon(triangleX, triangleY, 3);
		
		g.setTransform(gOrigXform);
	}
	
	public void handleCollision(GameObject otherObject, GameWorld gw) {}
}
