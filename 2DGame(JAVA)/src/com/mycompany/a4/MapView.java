package com.mycompany.a4;

import com.codename1.ui.Button;
import com.codename1.ui.Command;
import com.codename1.ui.Component;
import com.codename1.ui.Container;
import com.codename1.ui.Dialog;
import com.codename1.ui.Graphics;
import com.codename1.ui.Label;
import com.codename1.ui.Transform;
import com.codename1.ui.Transform.NotInvertibleException;
import com.codename1.ui.geom.Dimension;
import com.codename1.ui.layouts.BorderLayout;
import com.codename1.ui.layouts.BoxLayout;
import com.codename1.ui.layouts.FlowLayout;
import com.codename1.ui.layouts.GridLayout;
import com.codename1.ui.plaf.Border;

import java.util.Observable;
import java.util.Observer;

import com.codename1.charts.models.Point;
import com.codename1.charts.util.ColorUtil;

public class MapView extends Container implements Observer {
	private GameWorld gw = null;
	private PlayerCyborg pc;
	private GameObjectCollection theGameCollection;
	
	private Transform worldToND, ndToDisplay, theVTM;
	private float winLeft, winBottom, winRight, winTop, winWidth, winHeight;
	private Point pPrevDragLoc = new Point(-1, -1);
	
	public MapView(GameWorld obs) {
		this.gw = obs;
		this.theGameCollection = gw.getTheGameCollection();
		this.setLayout(new FlowLayout(Component.CENTER, Component.CENTER));
		this.getAllStyles().setBorder(Border.createLineBorder(1, ColorUtil.rgb(250, 180, 0)));
		System.out.println(gw.map());
		
		winLeft = 0;
		winBottom = 0;
		winRight = 700;
		winTop = 700;
		winWidth = winRight - winLeft;
		winHeight = winTop - winBottom;
		// create a FireOval to display
		// rotate, scale, and translate this FireOval on the container
		//pc.scale(2,2);
		//pc.rotate (45);
		//pc.translate (400, 200);
	}
	
	public void paint(Graphics g) {
		super.paint(g);
		
		winRight = this.getWidth() / 2;
		winTop = this.getHeight() / 2;
		winWidth = winRight - winLeft;
		winHeight = winTop - winBottom;
		
		worldToND = buildWorldToNDXform(winWidth, winHeight, winLeft, winBottom);
		ndToDisplay = buildNDToDisplayXform(this.getWidth(), this.getHeight());
		theVTM = ndToDisplay.copy();
		theVTM.concatenate(worldToND);
			
		Transform gXform = Transform.makeIdentity();
		g.getTransform(gXform);
		//move the drawing coordinates back
		gXform.translate(getAbsoluteX(), getAbsoluteY());
		gXform.concatenate(theVTM);
		//move the drawing coordinates as part of the local origin transformations
		gXform.translate(-getAbsoluteX(), -getAbsoluteY());
		g.setTransform(gXform);
		// tell objects to draw itself
		Point parentOrigin = new Point(this.getX(), this.getY());
		Point screenOrigin = new Point(getAbsoluteX(), getAbsoluteY());
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			nextGameObject.draw(g, parentOrigin, screenOrigin);
		}
		g.resetAffine();
	}
	private Transform buildWorldToNDXform(float winWidth, float winHeight, float winLeft, float winBottom) {	
		Transform tmpXfrom = Transform.makeIdentity();
		tmpXfrom.scale( (1/winWidth) , (1/winHeight) );
		tmpXfrom.translate(-winLeft,-winBottom);
		return tmpXfrom;
	}
	private Transform buildNDToDisplayXform (float displayWidth, float displayHeight) {
		Transform tmpXfrom = Transform.makeIdentity();
		tmpXfrom.translate(0, displayHeight);
		tmpXfrom.scale(displayWidth, -displayHeight);
		return tmpXfrom;
	}
	public void zoom(float factor) {
		//positive factor would zoom in (make the worldWin smaller), suggested value is 0.05f
		//negative factor would zoom out (make the worldWin larger), suggested value is -0.05f
		//...[calculate winWidth and winHeight]
		float newWinLeft = winLeft + winWidth*factor;
		float newWinRight = winRight - winWidth*factor;
		float newWinTop = winTop - winHeight*factor;
		float newWinBottom = winBottom + winHeight*factor;
		float newWinHeight = newWinTop - newWinBottom;
		float newWinWidth = newWinRight - newWinLeft;
		//in CN1 do not use world window dimensions greater than 1000!!!
		if (newWinWidth <= 1000 && newWinHeight <= 1000 && newWinWidth > 200 && newWinHeight > 200 ){
			winLeft = newWinLeft;
			winRight = newWinRight;
			winTop = newWinTop;
			winBottom = newWinBottom;
		}
		else {
			System.out.println("Cannot zoom further!");
		}
		this.repaint();
	}
	public void panHorizontal(double delta) {
		//positive delta would pan right (image would shift left), suggested value is 5
		//negative delta would pan left (image would shift right), suggested value is -5
		winLeft += delta;
		winRight += delta;
		this.repaint();
	}
	public void panVertical(double delta) {
		//positive delta would pan up (image would shift down), suggested value is 5
		//negative delta would pan down (image would shift up), suggested value is -5
		winBottom += delta;
		winTop += delta;
		this.repaint();
	}
	@Override
	public boolean pinch(float scale){
		if (scale < 1.0) {
			//Zooming Out: two fingers come closer together (on actual device), right mouse
			//click + drag towards the top left corner of screen (on simulator)
			zoom(-0.05f);
		}
		else if (scale > 1.0) {
			//Zooming In: two fingers go away from each other (on actual device), right mouse
			//click + drag away from the top left corner of screen (on simulator)
			zoom(0.05f);
		}
		return true;
	}
	@Override
	public void pointerDragged(int x, int y) {
	if (pPrevDragLoc.getX() != -1) {
		if (pPrevDragLoc.getX() < x)
			panHorizontal(5);
		else if (pPrevDragLoc.getX() > x)
			panHorizontal(-5);
		if (pPrevDragLoc.getY() < y)
			panVertical(-5);
		else if (pPrevDragLoc.getY() > y)
			panVertical(5);
	}
	pPrevDragLoc.setX(x);
	pPrevDragLoc.setY(y);
	}
	
	@Override
	public void update(Observable obs, Object args) {
		//winRight = this.getWidth() / 2;
		//winTop = this.getHeight() / 2;
		
		if (gw.getLivesRemaining() <= 0) {
			Dialog d = new Dialog();
			Label l = new Label("Game over, you failed! \n");
			Button b = new Button(new Close("Close"));
			d.add(l);
			d.add(b);
			d.show();
		}
		if (gw.getGameOverNPC() == true) {
			Label lose = new Label("Non Player Cyborg reached the final Base");
			Command Close = new Close("Ok");
			Dialog.show("Game Over. You Lose!", lose, Close);
		}
		if (gw.getGameOverWon() == true) {
			Label lose = new Label("Player Cyborg reached the Final Base in " + gw.getTimeInSecs() + " seconds");
			Command Close = new Close("Ok");
			Dialog.show("Game Over. You Won!", lose, Close);
		}
		System.out.println(gw.map());
		revalidate();
		repaint();
	}
	
	@Override
	public void pointerPressed(int x, int y){
		//(x, y) is the pointer location relative to screen origin
		//make it relative to display origin
		//float [] fPtr = new float [] {x - getAbsoluteX(), y - getAbsoluteY()};
		float ptrX = (float)(x - getAbsoluteX());
		float ptrY = (float)(y - getAbsoluteY());
		/*Transform inverseVTM = Transform.makeIdentity();
		try {
			theVTM.getInverse(inverseVTM);
		} 
		catch (NotInvertibleException e) {
			System.out.println("Non invertible xform!");
		}*/
		//calculate the location of fPtr the in world space
		//inverseVTM.transformPoint(fPtr, fPtr);
		IIterator theElements = theGameCollection.getIterator();
		while(theElements.hasNext()) {
			GameObject nextGameObject = theElements.getNext();
			if (nextGameObject instanceof Fixed) {
				Fixed fObj = (Fixed)nextGameObject;
				if (gw.getPlayMode() == false) {
					if ((fObj.getSelected() == true) && (gw.getPositionMode() == true)) {
						Point newLocation = new Point(ptrX, ptrY);
						fObj.setLocation(newLocation);
						fObj.setSelected(false);
						gw.setPositionMode(false);
						break;
					}
					else if (fObj.contains(ptrX, ptrY) && gw.getPlayMode() == false) {
						fObj.setSelected(true);
						//gw.setPositionMode(false);
					}
					else {
						fObj.setSelected(false);
						//gw.setPositionMode(false);
					}
				}
				
			}
		}
		repaint();
	}

}
