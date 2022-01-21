package com.mycompany.a4;

import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Font;
import com.codename1.ui.Graphics;
import com.codename1.ui.Transform;
import com.codename1.charts.models.Point;

public class EnergyStation extends Fixed implements IChangeColor{
	private int capacity;
	private Graphics energyGraphics;
	private Text text;
	
	public EnergyStation(int size, int color, Point location, int capacity) {
		super(size, color, location); // 
		this.capacity = capacity;
		text = new Text(Integer.toString(capacity));
	}
	
	public void changeColor(int amount) {
		int oldColor = super.getColor();
		int green = ColorUtil.green(oldColor);
		int newColor = ColorUtil.rgb(0, green - amount, 0);
		super.setColor(newColor);
	}
	public int getCapacity() {
		return this.capacity;
	}
	public void setCapacity(int capacity) {
		this.capacity = capacity;
	}
	public void draw(Graphics g, Point parentOrigin, Point screenOrigin) {
		int parentX = (int)parentOrigin.getX();
		int parentY = (int)parentOrigin.getY();
		int energyX = (int)getLocation().getX();
		int energyY = (int)getLocation().getY();
		int centerX = parentX;
		int centerY = parentY;
		Point objectCenter = new Point(centerX, centerY);
		
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
		energyGraphics = g;
		energyGraphics.setColor(getColor());
		if (getSelected() == true) {
			energyGraphics.drawArc(centerX - (getSize()/2), centerY - (getSize()/2), getSize(), getSize(), 0, 360);
			energyGraphics.setColor(getColor());
		}
		else {
			energyGraphics.fillArc(centerX - (getSize()/2), centerY - (getSize()/2), getSize(), getSize(), 0, 360);
			energyGraphics.setColor(ColorUtil.WHITE);
		}
		text.setColor(ColorUtil.WHITE);
		text.draw(g, objectCenter, screenOrigin);
		
		g.setTransform(gOrigXform);
	}
	
	public void handleCollision(GameObject otherObject, GameWorld gw) {}
	
}
