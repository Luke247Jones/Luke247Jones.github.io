package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.util.MathUtil;
import java.lang.Math;

public class IAttackStrategy implements IStrategy{
	private NonPlayerCyborg npc;
	private PlayerCyborg player;
	
	IAttackStrategy(NonPlayerCyborg n, PlayerCyborg p) {
		this.npc = n;
		this.player = p;
	}
	
	public void apply() {
		Point currentLocation = npc.getLocation();
		float x0 = currentLocation.getX();
		float y0 = currentLocation.getY();
		Point targetLocation = player.getLocation();
		float x1 = targetLocation.getX();
		float y1 = targetLocation.getY();
		
		double deltaX = (double)(x1 - x0);
		double deltaY = (double)(y1 - y0);
		double yOverX = deltaY/deltaX;
		
		int idealHeading = 90 - ((int)Math.toDegrees(MathUtil.atan(yOverX)));	
		if (deltaX < 0) {
			idealHeading = idealHeading + 180;
		}
		//}
		double idealSpeed = Math.sqrt((deltaX * deltaX)+(deltaY * deltaY));
		if (idealSpeed > npc.getMaximumSpeed()) {
			idealSpeed = npc.getMaximumSpeed();
		}
		npc.setSteeringDirection(idealHeading);
		npc.setSpeed((int)Math.round(idealSpeed));	
	}
	
	public void adjust() {
		Point currentLocation = npc.getLocation();
		float x0 = currentLocation.getX();
		float y0 = currentLocation.getY();
		Point targetLocation = player.getLocation();
		float x1 = targetLocation.getX();
		float y1 = targetLocation.getY();
		
		double deltaX = (double)(x1 - x0);
		double deltaY = (double)(y1 - y0);
		double yOverX = deltaY/deltaX;
		
		int idealHeading = 90 - ((int)Math.toDegrees(MathUtil.atan(yOverX)));
		
		if (deltaX < 0) {
			idealHeading = 90 - ((int)Math.toDegrees(MathUtil.atan(yOverX)));
			idealHeading = idealHeading + 180;
		}
		
		idealHeading = idealHeading - npc.getHeading();
		
		double idealSpeed = Math.sqrt((deltaX * deltaX)+(deltaY * deltaY));
		
		if (idealSpeed > npc.getMaximumSpeed()) {
			idealSpeed = npc.getMaximumSpeed();
		}
		
		npc.setSteeringDirection(idealHeading);
		npc.setSpeed((int)Math.round(idealSpeed));
	}
}
