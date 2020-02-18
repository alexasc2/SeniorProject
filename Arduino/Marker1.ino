#include <Uduino.h>
#include <Adafruit_NeoPixel.h>

#define PIN 3
#define NUMPIXELS 8

Adafruit_NeoPixel pixels(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);

// Name of the board
Uduino uduino("Marker #1");
int active = 0;
int brightnessSet = 0;
void setup(){
  Serial.begin(9600);
  pixels.begin();
  pixels.show();
  uduino.addCommand("activate", activate);
  uduino.addCommand("deactivate",deactivate);
  uduino.addCommand("brightness",brightness);
}

void loop(){
  uduino.update();
  pixels.setBrightness(brightnessSet);
  if(active == 1){
    for(int i = 0; i < NUMPIXELS; i++){
      pixels.setPixelColor(i, pixels.Color(10, 0 ,2));
      pixels.show();
      Serial.print("Activate\n");
      delay(100);
    }
  }else{
    for(int i = 0; i < NUMPIXELS; i++){
      pixels.setPixelColor(i, pixels.Color(50, 0 ,2));
      pixels.show();
      Serial.print("Deactivate\n");
      delay(100);
      pixels.setPixelColor(i, pixels.Color(0, 0 ,0));
      pixels.show();
    }
  }
}

void brightness(){
  char *arg;
  arg = uduino.next();
  brightnessSet = int(200*atoi(arg)/100);
}

void activate(){
  active = 1;
}

void deactivate(){
  active = 0;
}
