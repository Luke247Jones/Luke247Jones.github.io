package com.mycompany.a4;

import java.util.Vector;

import com.codename1.charts.models.Point;
import com.codename1.ui.Transform;

public abstract class GameObject implements IDrawable, ICollider {
	private int size;
	private int color;
	private Point location;
	private Vector<GameObject> collisionElements;
	
	protected Transform myTranslation;
	protected Transform myRotation;
	protected Transform myScale;
	
	public GameObject(int size, int color, Point location) {
		this.size = size;
		this.color = color;
		this.location = location;
		collisionElements = new Vector<GameObject>();
		
		myTranslation = Transform.makeIdentity();
		myRotation = Transform.makeIdentity();
		myScale = Transform.makeIdentity();
	}
	public int getSize() {
		return this.size;
	}
	public int getColor() {
		return this.color;
	}
	public Point getLocation() {
		float pointX = this.myTranslation.getTranslateX();
		float pointY = this.myTranslation.getTranslateY();
		Point newLocation = new Point(pointX, pointY);
		return newLocation;
	}
	public void setColor(int color) {
		this.color = color;
	}
	public void setLocation(Point location) {
		//this.location = location;
	}
	public void rotate (double degrees) {
		myRotation.rotate ((float)Math.toRadians(degrees), 0, 0);
	}
	public void scale (double sx, double sy) {
		myScale.scale ((float)sx, (float)sy);
	}
	public void translate (double tx, double ty) {
		myTranslation.translate ((float)tx, (float)ty);
	}
	
	public boolean collidesWith(GameObject otherObject) {
		boolean result = false;
		float thisCenterX = getLocation().getX(); // find centers
		float thisCenterY = getLocation().getY();
		float otherCenterX = otherObject.getLocation().getX();
		float otherCenterY = otherObject.getLocation().getY();
		// find dist between centers (use square, to avoid taking roots)
		float boundaryX1 = thisCenterX - (getSize()/2);
		float boundaryX2 = thisCenterX + (getSize()/2);
		float boundaryY1 = thisCenterY - (getSize()/2);
		float boundaryY2 = thisCenterY + (getSize()/2);
		int dx1 = (int)Math.abs(otherCenterX - boundaryX1);
		int dx2 = (int)Math.abs(otherCenterX - boundaryX2);
		int dy1 = (int)Math.abs(otherCenterY - boundaryY1);
		int dy2 = (int)Math.abs(otherCenterY - boundaryY2);
		int otherObjectRadius = otherObject.getSize()/2;
		if ((dx1 <= otherObjectRadius || dx2 <= otherObjectRadius) && (dy1 <= otherObjectRadius || dy2 <= otherObjectRadius)) {
			result = true;
		}
		return result;
		/*int distBetweenCentersSqr = (dx*dx + dy*dy);
		// find square of sum of radii
		int thisRadius = objSize/2;
		int otherRadius = objSize/2;
		int radiiSqr = (thisRadius*thisRadius + 2*thisRadius*otherRadius
		+ otherRadius*otherRadius);
		if (distBetweenCentersSqr <= radiiSqr) { result = true ; }
		return result ;
		*/
	}
	public void addCollisionElement(GameObject otherObject) {
		this.collisionElements.addElement(otherObject);
	}
	public void updateCollisionElements(GameObject otherObject) {
		if (this.alreadyCollidesWith(otherObject) && (this.collidesWith(otherObject) == false)) {
			this.collisionElements.remove(otherObject);
		}
	}
	public boolean alreadyCollidesWith(GameObject otherObject) {
		if (this.collisionElements.isEmpty()) {
			return false;
		}
		else if (this.collisionElements.contains(otherObject)) {
			return true;
		}
		else {
			return false;
		}
	}
	abstract public void handleCollision(GameObject otherObject, GameWorld gw);
	
	
	
}