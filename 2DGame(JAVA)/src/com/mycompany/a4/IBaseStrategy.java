package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.util.MathUtil;
import java.lang.Math;

public class IBaseStrategy implements IStrategy{
	private NonPlayerCyborg npc;
	private Base nextBase;
	
	IBaseStrategy(NonPlayerCyborg n, Base b) {
		this.npc = n;
		this.nextBase = b;
	}
	
	public void apply() {
		Point currentLocation = npc.getLocation();
		float x0 = currentLocation.getX();
		float y0 = currentLocation.getY();
		Point targetLocation = nextBase.getLocation();
		float x1 = targetLocation.getX();
		float y1 = targetLocation.getY();
		
		double deltaX = (double)(x1 - x0);
		double deltaY = (double)(y1 - y0);
		double yOverX = deltaY/deltaX;
		
		int idealHeading = 90 - ((int)Math.toDegrees(MathUtil.atan(yOverX)));
		if (deltaX < 0) {
			idealHeading = idealHeading + 180;
		}
		
		int idealSpeed = (int)Math.sqrt((deltaX * deltaX)+(deltaY * deltaY));
		if (idealSpeed > npc.getMaximumSpeed()) {
			idealSpeed = npc.getMaximumSpeed();
		}
		npc.setSteeringDirection(idealHeading);
		npc.setSpeed(idealSpeed);	
	}
	
	public void adjust() {
		/*int nextBaseNumber = npc.getLastBaseReached() + 1;
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof Cyborg) {
		*/
		Point currentLocation = npc.getLocation();
		float x0 = currentLocation.getX();
		float y0 = currentLocation.getY();
		Point targetLocation = nextBase.getLocation();
		float x1 = targetLocation.getX();
		float y1 = targetLocation.getY();
		
		double deltaX = (double)(x1 - x0);
		double deltaY = (double)(y1 - y0);
		double yOverX = deltaY/deltaX;
		
		int idealHeading = 90 - ((int)Math.toDegrees(MathUtil.atan(yOverX)));
		if (deltaX < 0) {
			idealHeading = idealHeading + 180;
		}
		idealHeading = idealHeading - npc.getHeading();
		
		double idealSpeed = Math.sqrt((deltaX * deltaX)+(deltaY * deltaY));
		if (idealSpeed > npc.getMaximumSpeed()) {
			idealSpeed = npc.getMaximumSpeed();
		}
		if (deltaX < 5 && deltaY < 5) {
			idealSpeed = 0;
		}
		
		npc.setSteeringDirection(idealHeading);
		npc.setSpeed((int)Math.round(idealSpeed));
	}
}
