package com.mycompany.a4;

import com.codename1.charts.models.Point;
import com.codename1.ui.Graphics;

public interface IDrawable {
	public void draw(Graphics g, Point parentOrigin, Point screenOrigin);
}
