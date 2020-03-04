#include <Uduino.h>
#include <Adafruit_NeoPixel.h>

#define PIN 3
#define NUMPIXELS 8

Adafruit_NeoPixel pixels(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);

// Name of the board
Uduino uduino("Marker#1");
int active = 0,
    patterns = 0;
int brightnessSet = 0;
int delayValue = 100;
int r = 50,
    g = 0,
    b = 0;

void setup(){
  Serial.begin(9600);
  pixels.begin();
  pixels.show();
  uduino.addCommand("activate", activate);
  uduino.addCommand("brightness",brightness);
  uduino.addCommand("delayControl", delayControl);
  uduino.addCommand("color",color);
  uduino.addCommand("pattern",pattern);
  uduino.addDisconnectedFunction(Disabled);
}

void loop(){
  uduino.update();
  pixels.setBrightness(brightnessSet);
  if(uduino.isConnected()){
    if(active == 1){
      switch(patterns){
        case 0:
          On();
          break;
        case 1:
          Chase();
          break;
        case 2:
          alternate();
          break;
        case 3:
          breathe();
          break;
        default:
          On();
          break;
      }
    }else{
      Off();
    }
  }else{
    Disabled();
  }
}

void On(){
  delay(delayValue);
  for(int i = 0; i < NUMPIXELS; i++){
        pixels.setPixelColor(i, pixels.Color(r, g ,b));
        pixels.show();
      }
}

void Chase(){
  delay(delayValue);
  for(int i = 0; i < NUMPIXELS; i++){
        pixels.setPixelColor(i, pixels.Color(r, g ,b));
        pixels.show();
        delay(delayValue);
        pixels.setPixelColor(i, pixels.Color(0, 0 ,0));
        pixels.show();
      }
}

void Off(){
  delay(delayValue);
  for(int i = 0; i < NUMPIXELS; i++){
        pixels.setPixelColor(i, pixels.Color(r, g ,b));
        pixels.show();
        delay(delayValue);
        pixels.setPixelColor(i, pixels.Color(0, 0 ,0));
        pixels.show();
      }
}

void breathe(){
  int i = 0;
  for(i; i < int(brightnessSet/2); i++){
    for(int j = 0; j < NUMPIXELS; j++){
      pixels.setPixelColor(j, pixels.Color(r, g ,b));
    }
    pixels.setBrightness(i);
    pixels.show();
    delay(10);
  }
  for(i; i > 0; i--){
    for(int j = 0; j < NUMPIXELS; j++){
      pixels.setPixelColor(j, pixels.Color(r, g ,b));
    }
    pixels.setBrightness(i);
    pixels.show();
    delay(10);
  }
}

void alternate(){
    for(int i = 0; i < NUMPIXELS; i++){
      if(i%2 == 0){
        pixels.setPixelColor(i, pixels.Color(r, g ,b));
      }else{
        pixels.setPixelColor(i, pixels.Color(0, 0 ,0));
      }
      pixels.show();
    }
    delay(delayValue);
    for(int i = 0; i < NUMPIXELS; i++){
      if(i%2 == 1){
        pixels.setPixelColor(i, pixels.Color(r, g ,b));
      }else{
        pixels.setPixelColor(i, pixels.Color(0, 0 ,0));
      }
      pixels.show();
    }
    delay(delayValue);
}

void Disabled(){
  for(int i = 0; i < NUMPIXELS; i++){
        pixels.setPixelColor(i, pixels.Color(0, 0 ,0));
        pixels.show();
      }
}

void color(){
  int parameters = uduino.getNumberOfParameters();

  if(parameters > 0){
    r = uduino.charToInt(uduino.getParameter(0));
    g = uduino.charToInt(uduino.getParameter(1));
    b = uduino.charToInt(uduino.getParameter(2));
  }
}

void pattern(){
  char *arg;
  arg = uduino.next();
  patterns = atoi(arg);
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
