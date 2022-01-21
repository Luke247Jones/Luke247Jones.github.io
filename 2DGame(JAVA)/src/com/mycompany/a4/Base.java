package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Graphics;
import com.codename1.ui.Label;
import com.codename1.ui.Transform;

public class Base extends Fixed {
	private int sequenceNumber;
	private Graphics baseGraphics;
	private Text text;
	
	public Base(int size, int color, Point location, int sequenceNumber) {
		super(size, color, location);
		this.sequenceNumber = sequenceNumber;
		
		text = new Text(Integer.toString(getSequenceNumber()));
	}
	public int getSequenceNumber() {
		return this.sequenceNumber;
	}
	public void setColor(int color) {
		super.setColor(color);
	}
	public void draw(Graphics g, Point parentOrigin, Point screenOrigin) {
		int parentX = (int)parentOrigin.getX();
		int parentY = (int)parentOrigin.getY();
		int baseX = (int)getLocation().getX();
		int baseY = (int)getLocation().getY();
		int centerX = parentX;
		int centerY = parentY;
		Point objectCenter = new Point(centerX, centerY);
		
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
		baseGraphics = g;
		baseGraphics.setColor(getColor());
		if (getSelected() == false) {
			baseGraphics.fillTriangle(centerX - (getSize()/2), centerY - (getSize()/2), centerX, centerY + (getSize()/2), centerX + (getSize()/2), centerY - (getSize()/2));
			baseGraphics.setColor(ColorUtil.WHITE);
		}
		else {
			baseGraphics.drawPolygon(triangleX, triangleY, 3);
			baseGraphics.setColor(getColor());
		}
		text.setColor(ColorUtil.WHITE);
		text.draw(g, objectCenter, screenOrigin);
		//text.rotate();
		
		g.setTransform(gOrigXform);
	}
	
	public void handleCollision(GameObject otherObject, GameWorld gw) {}
}
