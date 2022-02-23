import java.*;
import java.io.*;

RandomAccessFile pipe;
byte[] readBytes = new byte[256];

void setup()
{
  size(128, 128);
  surface.setTitle("PWDTEST");
  noSmooth();
}

void draw()
{
  background(0, 255, 0);
  
  fill(255, 0, 255);
  stroke(0, 0, 255);
  strokeWeight(3);
  rect(10, 10, 50, 50);
  
  fill(0, 0, 255);
  textSize(16);
  textAlign(RIGHT, BOTTOM);
  text("pwdtest", 120, 120);
  text(frameCount, 120, 100);
  
  set(0, 0, color(0, 0, 255));
  set(width - 1, height - 1, color(0, 0, 255));
}
