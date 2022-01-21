package com.mycompany.a4;

import java.util.Vector;

public class GameObjectCollection implements ICollection{
	private Vector<GameObject> theCollection;
	
	public GameObjectCollection() {
		theCollection = new Vector<GameObject>();
	}
	
	public void add(GameObject gObj) {
		theCollection.addElement(gObj);
	}
	
	public IIterator getIterator() {
		return new GameVectorIterator() ;
	}
	public void clear() {
		theCollection.clear();
	}
	private class GameVectorIterator implements IIterator {
		private int currentIndex;
		
		public GameVectorIterator() {
			currentIndex = -1;
		}
		public boolean hasNext() {
			if (theCollection.size() <= 0) { 
				return false;
			}
			if (currentIndex == (theCollection.size() - 1)) {
				return false;
			}
			
			return true;
		}
		public GameObject getNext() {
			currentIndex ++ ;
			return(theCollection.elementAt(currentIndex));
		}
	} 
}
