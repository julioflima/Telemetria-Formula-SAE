//Program : Comunicacao Xbee utilizando Arduino Xbee Shield
//Autor : Julio Cesar Ferreira Lima

#include <TimerOne.h>


//Put the values on serial
int received = '0';

//Define the number of samples of each input
int  n_samples = 25;

//Curses variables
int curse_brake = 0, curse_accelerator, curse_steering_wheel = 0;

//General variables
int gear_position = 0;
bool brake_light = 0;

void setup()
{
  //Set the standard of the timer one
  Timer1.initialize(100000);  //Set a timer of length 100000 microseconds (or 0.1 sec - or 10Hz => the led will blink 5 times, 5 cycles of on-and-off, per second)
  Timer1.attachInterrupt(Happen);  //Attach the service routine here


  //Define pin 13 to output
  pinMode(13, OUTPUT);
  Serial.begin(115200);
     
}

/*
List of parameters the will sending of car to supervisory, by ordem

1 - Air Temperature
2 - Oil Temperature
3 - Gear
4 - Speed
5 - RPM
6 - Brake Position
7 - Accelerator Position
8 - Steering Wheel Angle
9 - Brake Light
10 - Dead Point 
11 - Oil Level
12 - Gas Level
13 - Power Good
*/

void loop()
{
  //Wait  data of serial
  if (Serial.available() > 0)
  {
    //Read serial
    received = Serial.read();
    //Case receive 1, enables seding
    if (received == '1')
    {
      //Character that represent the begining of string  ?
      Serial.print("?");
      
      //1 - Air Temperature
      Turn(random(0, 501));
      //Character that represent the separation of variable  &
      Serial.print("&");

      //2 - Oil Temperature
      Turn(random(0, 2001));
      //Character that represent the separation of variable  &
      Serial.print("&");

      //3 - Gear
      Turn(gear_position);
      //Character that represent the separation of variable  &
      Serial.print("&");

      //4 - Speed
      Turn(random(0, 1700));
      //Character that represent the separation of variable  &
      Serial.print("&");
      
      //5 - RPM
      Turn(random(0, 8001));
      //Character that represent the separation of variable  &
      Serial.print("&");

      //6 - Brake Position
      Turn(curse_brake);
      //Character that represent the separation of variable  &
      Serial.print("&");

      //7 - Accelerator Position
      Turn(curse_accelerator);
      //Character that represent the separation of variable  &
      Serial.print("&");

      //8 - Steering Wheel Angle
      Turn(curse_steering_wheel);
      //Character that represent the separation of variable  &
      Serial.print("&");
      
      //9 - Brake Light
      Turn(brake_light);
      //Character that represent the separation of variable  &
      Serial.print("&");
      
      //10 - Dead Point 
      Turn(random(0, 2));
      //Character that represent the separation of variable  &
      Serial.print("&");
      
      //11 - Oil Level
      Turn(random(0, 2));
      //Character that represent the separation of variable  &
      Serial.print("&");

      //12 - Gas Level
      Turn(random(0, 2));
      //Character that represent the separation of variable  &
      Serial.print("&");

      //13 - Power Good
      Turn(random(0, 2));
      //Character that represent the end of string  !
      Serial.println("!");

      //Turn on the indicative led of comunication
      digitalWrite(13, HIGH);
    }
    //Case receive 0, disables seding
    else if (received == '0')
    {
      //Turn off the indicative ledof comunication
      digitalWrite(13, LOW);
    }
  }
}

void Turn(int a)
{
  if(a < -9)
  {
    Serial.print("0");
    Serial.print(a,DEC);
  }
  
  if(a >= -9 && a < 0)
  {
    Serial.print("00");
    Serial.print(a,DEC);
  }

  if(a >= 0 && a <= 9)
  {
    Serial.print("000");
    Serial.print(a,DEC);
  }
  if(a >= 10 && a <= 99)
  {
    Serial.print("00");
    Serial.print(a,DEC);
  }
  if(a >= 100 && a<= 999)
  {
    Serial.print("0");
    Serial.print(a,DEC);
  }
    if(a >= 1000)
  {
    Serial.print(a,DEC);
  }
}

void Change_Gear()
{
  if(analogRead(4) < 900 && gear_position > 0)
  {
    gear_position--;
  }
  
  if(analogRead(3) < 900 && gear_position < 4)
  {
    gear_position++;
  }
  
}

void Brake_Light()
{
  if(curse_brake>0)
  {
    brake_light = HIGH;
  }
  else
  {
    brake_light = LOW;
  }
  
}



void Happen()
{
  unsigned int long i=0 ,v0=0, v1=0, v2=0; 

  //feed of the random table with time that microcontroller was connected 
  randomSeed(millis()^(millis()*1));

  //verify if the gear was changed
  Change_Gear();
  Brake_Light();
  while(i<n_samples)
  {
    v0 += int(map(analogRead(1), 0, 885, 0, 100));
    v1 += int(map(analogRead(0), 0, 720, 0, 100));
    v2 +=analogRead(2);
    i++;
  }

  int aux = v2 / n_samples;
  if (aux > 511)
  {
    
    curse_steering_wheel =int(map(aux, 511, 1023, 0, -90));
  }
  else if (aux <= 511)
  {
    curse_steering_wheel =int(map(aux, 511, 0, 0, 90));
  }
  
  curse_brake = v0 / n_samples;
  curse_accelerator = v1 / n_samples;
  //curse_accelerator = analogRead(0);
}
