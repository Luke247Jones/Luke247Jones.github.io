package com.mycompany.a4;

import java.util.Vector;

import com.codename1.charts.models.Point;
import com.codename1.charts.util.ColorUtil;
import com.codename1.ui.Graphics;
import com.codename1.ui.Transform;

public class PlayerCyborg extends Cyborg {
	private static PlayerCyborg player;
	private Graphics playerGraphics;
	private Body body;
	private Head head;
	private Arm rightArm, leftArm;
	private Leg rightLeg, leftLeg;
	
	private int walkCadence = 0;
	private Point objectCenter;
	


	public PlayerCyborg() {
		super(40, ColorUtil.rgb(255,0,0), new Point(20,20), 0, 10, 0, 65, 100, 5, 0, 1);
		this.translate(20, 20);
		head = new Head();
		body = new Body();
		rightArm = new Arm("right");
		leftArm = new Arm("left");
		rightLeg = new Leg("right");
		leftLeg = new Leg("left");
		//head.scale(2.5, 1.5);
		//head.translate(40, 40);
	}
	
	public static PlayerCyborg getPlayerCyborg() {
		if (player == null)
			player = new PlayerCyborg();
			return player;
	}
	public void reset() {
		player = null;
		playerGraphics = null;
	}
	
	public void draw(Graphics g, Point parentOrigin, Point screenOrigin) {
		float parentX = parentOrigin.getX();
		float parentY = parentOrigin.getY();
		float playerX = myTranslation.getTranslateX();
		float playerY = myTranslation.getTranslateY();
		float centerX = parentX;
		float centerY = parentY;
		objectCenter = new Point(centerX, centerY);
		
		Transform gXform = Transform.makeIdentity();
		g.getTransform(gXform);
		Transform gOrigXform = gXform.copy(); //save the original xform
		//move the drawing coordinates back
		gXform.translate(screenOrigin.getX(), screenOrigin.getY());
		gXform.translate(myTranslation.getTranslateX(), myTranslation.getTranslateY());
		gXform.concatenate(myRotation);
		gXform.scale(myScale.getScaleX(), myScale.getScaleY());
		gXform.translate(-screenOrigin.getX(), -screenOrigin.getY());
		g.setTransform(gXform);
		//draw sub-shapes
		head.setColor(getColor());
		body.setColor(getColor());
		rightArm.setColor(getColor());
		leftArm.setColor(getColor());
		rightLeg.setColor(getColor());
		leftLeg.setColor(getColor());
		head.draw(g, objectCenter, screenOrigin);
		body.draw(g, objectCenter, screenOrigin);
		rightArm.draw(g, objectCenter, screenOrigin);
		leftArm.draw(g, objectCenter, screenOrigin);
		rightLeg.draw(g, objectCenter, screenOrigin);
		leftLeg.draw(g, objectCenter, screenOrigin);
		
		g.setTransform(gOrigXform);
	}
	
	public void updateLTs() {
		//leftLeg.translate(objectCenter.getX(), objectCenter.getY());
		//rightLeg.translate(objectCenter.getX(), objectCenter.getY());
		//this.translate(1, 1);
		//this.rotate(1);
		
		if (walkCadence > 0) {
			leftLeg.translate(-5, 0);
			rightLeg.translate(5, 0);
			leftLeg.rotate(20);
			rightLeg.rotate(20);
			leftArm.rotate(-10);
			rightArm.rotate(-10);
		}
		else if (walkCadence < 0) {
			leftLeg.translate(5, 0);
			rightLeg.translate(-5, 0);
			leftLeg.rotate(-20);
			rightLeg.rotate(-20);
			leftArm.rotate(10);
			rightArm.rotate(10);
		}
		else {
			walkCadence = 1;
			leftLeg.translate(-3, 0);
			rightLeg.translate(3, 0);
			leftLeg.rotate(10);
			rightLeg.rotate(10);
			leftArm.rotate(-5);
			rightArm.rotate(-5);
		}
		walkCadence = walkCadence * (-1);
	}
	
}
