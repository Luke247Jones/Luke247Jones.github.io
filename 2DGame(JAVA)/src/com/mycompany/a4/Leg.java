package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Graphics;
import com.codename1.ui.Transform;

public class Leg {
	private Point bottomLeft;
	private int width, height;
	private int myColor;
	private Transform myTranslation;
	private Transform myRotation;
	private Transform myScale;
	
	private int parentX;
	private int parentY;
	private int screenX;
	private int screenY;
	
	public Leg(String side) {
		if (side == "right") {
			bottomLeft = new Point (5, -20);
			width = 5;
			height = 10;
		}
		else {
			bottomLeft = new Point (-10 , -20);
			width = 5;
			height = 10;
		}
		
		
		myTranslation = Transform.makeIdentity();
		myRotation = Transform.makeIdentity();
		myScale = Transform.makeIdentity();
	}
	
	public void setColor(int color){
		myColor = color;
	}
	public void rotate (double degrees) {
		myRotation.rotate((float)Math.toRadians(degrees), 0, 0);
	}
	public void scale (double sx, double sy) {
		myScale.scale ((float)sx, (float)sy);
	}
	public void translate (double tx, double ty) {
		myTranslation.translate ((float)tx, (float)ty);
	}
	public void draw (Graphics g, Point parentOrigin, Point screenOrigin) {
		parentX = (int)parentOrigin.getX();
		parentY = (int)parentOrigin.getY();
		System.out.println("Parent origin: " + parentOrigin.getX() + "," + parentOrigin.getY());
		screenX = (int)screenOrigin.getX();
		screenY = (int)screenOrigin.getY();
		
		Transform gXform = Transform.makeIdentity();
		g.getTransform(gXform);
		Transform gOrigXform = gXform.copy(); //save the original xform
		
		gXform.translate(screenOrigin.getX(), screenOrigin.getY());
		System.out.println("Screen origin: " + screenOrigin.getX() + "," + screenOrigin.getY());
		
		gXform.translate(myTranslation.getTranslateX(), myTranslation.getTranslateY());
		gXform.scale(myScale.getScaleX(), myScale.getScaleY());
		gXform.concatenate(myRotation);
		gXform.translate(-screenOrigin.getX(),-screenOrigin.getY());
		System.out.println("Screen origin: " + -(screenOrigin.getX()) + "," + (-screenOrigin.getY()));
		g.setTransform(gXform);
		//draw the lines as before
		g.setColor(myColor);
		g.drawRect(parentX + (int)bottomLeft.getX(), parentY + (int)bottomLeft.getY(), width, height);

		g.setTransform(gOrigXform); //restore the original xform (remove LTs)
		//do not use resetAffine() in draw()! Instead use getTransform()/setTransform(gOrigForm)
	}
}
