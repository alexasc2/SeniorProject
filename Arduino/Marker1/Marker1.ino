#include <Uduino.h>
#include <Adafruit_NeoPixel.h>

#define PIN 3
#define NUMPIXELS 8

Adafruit_NeoPixel pixels(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);

// Name of the board
Uduino uduino("Marker#1");
int active = 0;
int brightnessSet = 0;
int delayValue = 100;

void setup(){
  Serial.begin(9600);
  pixels.begin();
  pixels.show();
  uduino.addCommand("activate", activate);
  uduino.addCommand("brightness",brightness);
  uduino.addCommand("delayControl", delayControl);
  uduino.addDisconnectedFunction(Disabled);
}

void loop(){
  uduino.update();
  pixels.setBrightness(brightnessSet);
  if(uduino.isConnected()){
    if(active == 1){
      On();
    }else{
      Off();
    }
  }else{
    Disabled();
  }
}

void On(){
  for(int i = 0; i < NUMPIXELS; i++){
        pixels.setPixelColor(i, pixels.Color(100, 100 ,2));
        delay(delayValue);
        pixels.show();
      }
}

void Off(){
  for(int i = 0; i < NUMPIXELS; i++){
        pixels.setPixelColor(i, pixels.Color(50, 0 ,2));
        pixels.show();
        delay(delayValue);
        pixels.setPixelColor(i, pixels.Color(0, 0 ,0));
        pixels.show();
      }
}

void Disabled(){
  for(int i = 0; i < NUMPIXELS; i++){
        pixels.setPixelColor(i, pixels.Color(0, 0 ,0));
        pixels.show();
      }
}

void brightness(){
  char *arg;
  arg = uduino.next();
  brightnessSet = int(255*atoi(arg)/100);
}

void delayControl(){
  char *arg;
  arg = uduino.next();
  delayValue = atoi(arg);
}

void activate(){
  char *arg;
  arg = uduino.next();
  active = atoi(arg);
  brightness();
  delayControl();
  if(active == 1){
    Serial.print("Activate\n");
  }else{
    Serial.print("Deactivate\n");
  }
}
