package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Graphics;

public abstract class Fixed extends GameObject {
	private boolean selected = false;
	
	public Fixed(int size, int color, Point location) {
		super(size, color, location);
	}
	//public void setLocation(Point location) {} //override parent method with empty body method
	
	public void draw(Graphics g, Point parentOrigin) {}
	
	public boolean contains(float ptrX, float ptrY) {
		boolean result = false;
		float thisCenterX = getLocation().getX(); // find centers
		float thisCenterY = getLocation().getY();

		float boundaryX1 = thisCenterX - (getSize()/2);
		float boundaryX2 = thisCenterX + (getSize()/2);
		float boundaryY1 = thisCenterY - (getSize()/2);
		float boundaryY2 = thisCenterY + (getSize()/2);

		if (ptrX >= boundaryX1 && ptrX <= boundaryX2 && ptrY >= boundaryY1 && ptrY <= boundaryY2) {
			result = true;
		}
		return result;
	}
	public void setSelected(boolean b) {
		this.selected = b;
	}
	public boolean getSelected() {
		return this.selected;
	}
}
