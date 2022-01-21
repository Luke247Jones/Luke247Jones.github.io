package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Graphics;
import com.codename1.ui.Transform;

import java.util.Random;

public class Shockwave extends Movable {
	public Graphics droneGraphics;
	
	public Shockwave(int size, int color, Point location, int heading, int speed) {
		super(size, color, location, heading, speed);
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
		g.setColor(getColor());
		float startX = getLocation().getX();
		float startY = getLocation().getY();
		Point control1 = new Point(startX, startY);
		Point control2 = new Point(startX + 10, startY + 20);
		Point control3 = new Point(startX + 20, startY + 40);
		Point control4 = new Point(startX + 10, startY + 10);
		Point[] controlArray = {control1, control2, control3, control4};
		drawBezierCurve(g, controlArray);
		//g.drawPolygon(triangleX, triangleY, 3);
		
		g.setTransform(gOrigXform);
	}
	
	private void drawBezierCurve(Graphics g, Point[] controlPointArray) {
		Point currentPoint = controlPointArray[0]; // start drawing at first control point
		double t = 0; // vary the parametric value "t" over the length of the curve
		while ( t <= 1 ) {
			// compute next point on the curve as the sum of the Control Points, each
			// weighted by the appropriate polynomial evaluated at t.
			Point nextPoint = new Point(getLocation().getX(), getLocation().getY());
			
			for (int i=0; i<=3; i++) {
				float nextPointX = nextPoint.getX() + ((float)blendingFunction(i,t) * controlPointArray[i].getX());
				float nextPointY = nextPoint.getY() + ((float)blendingFunction(i,t) * controlPointArray[i].getY());
				nextPoint = new Point(nextPointX, nextPointY);
			}
			g.drawLine((int)currentPoint.getX(), (int)currentPoint.getY(), (int)nextPoint.getX(), (int)nextPoint.getY());
			currentPoint = nextPoint;
			t = t + 0.2;
			
		}
	}
	private double blendingFunction(int i, double t) {
		double blended = 0;
		switch (i) {
			case 0: 
				blended = ( (1-t) * (1-t) * (1-t) ) ; // (1-t)3
			case 1: 
				blended = ( 3 * t * (1-t) * (1-t) ) ; // 3t(1-t)2
			case 2: 
				blended = ( 3 * t * t * (1-t) ) ; // 3t2(1-t)
			case 3: 
				blended = ( t * t * t ) ; // t3
		}
		return blended;
	}
	
	public void handleCollision(GameObject otherObject, GameWorld gw) {}
}
