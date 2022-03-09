/*
 Name:		mqtt_client.ino
 Created:	2/13/2022 7:27:22 PM
 Author:	Michael
*/

// the setup function runs once when you press reset or power the board
//#include <Key.h>
#include <PubSubClient.h>
#include <WiFiNINA.h>
#include <Keypad.h>
#include "credentials.h"

#define LED_PIN 10

const char* ssid = networkSSID;
const char* password = networkPASSWORD;
const char* mqttServer = mqttSERVER;
const char* mqttUsername = mqttUSERNAME;
const char* mqttPassword = mqttPASSWORD;

char subTopic[] = "arduino/ledControl";
char pubTopic[] = "arduino/ledState";

WiFiClient wifiClient;
PubSubClient client(wifiClient);

long lastMsg = 0;
char msg[50];
int value = 0;
int ledState = 0;


/* Keypad */
const byte ROWS = 4;
const byte COLS = 4;

char hexaKeys[ROWS][COLS] = {
  {'1', '2', '3', 'A'},
  {'4', '5', '6', 'B'},
  {'7', '8', '9', 'C'},
  {'*', '0', '#', 'D'}
};

byte rowPins[ROWS] = { 9, 8, 7, 6 };
byte colPins[COLS] = { 5, 4, 3, 2 };

Keypad customKeypad = Keypad(makeKeymap(hexaKeys), rowPins, colPins, ROWS, COLS);

// Track Dataset/Series mapping
//const char* mapping[5] = { "1", "1", "2", "3", "4" };
const char* mapping[5] = { "3", "9", "10", "11", "12" };

void setup()
{
    pinMode(LED_PIN, OUTPUT);
    Serial.begin(115200);
    setup_wifi();
    client.setServer(mqttServer, 1883);
    client.setCallback(callback);
}

// the loop function runs over and over again until power down or reset
void loop()
{
  if (!client.connected())
  {
      reconnect();
  }

  client.loop();

  long now = millis();
  if (now - lastMsg > 5000) {
      lastMsg = now;
      char payLoad[1];
      itoa(ledState, payLoad, 10);
      client.publish(pubTopic, payLoad);
  }


  char customKey = customKeypad.getKey();

  if (customKey) {
      Serial.println(customKey);

      char seriesId[2];

      if (customKey == 'A')
      {
          strcpy(seriesId, mapping[1]);
      }
      else if (customKey == 'B')
      {
          strcpy(seriesId, mapping[2]);
      }
      else if (customKey == 'C')
      {
          strcpy(seriesId, mapping[3]);
      }
      else if (customKey == 'D')
      {
          strcpy(seriesId, mapping[4]);
      }

      // Form payload
      char url[19] = "app/track/record/";
      strcat(url, mapping[0]);

      char prepend[3] = "{\"";
      char append[6] = "\": 1}";

      char json[10];
      strcpy(json, prepend);
      strcat(json, seriesId);
      strcat(json, append);

      // Send track request
      client.publish(url, json);

      // Set status to off
      digitalWrite(LED_PIN, LOW);
      ledState = 0;
  }
}

void callback(char* topic, byte* payload, unsigned int length)
{
    Serial.print("Message arrived [");
    Serial.print(topic);
    Serial.print("] ");
    for (int i = 0; i < length; i++)
    {
        Serial.print((char)payload[i]);
    }
    Serial.println();

    // Switch on the LED if 1 was received as first character
    if ((char)payload[0] == '1')
    {
        digitalWrite(LED_PIN, HIGH);
        ledState = 1;
        char payLoad[1];
        itoa(ledState, payLoad, 10);
        client.publish(pubTopic, payLoad);
    }
    else
    {
        digitalWrite(LED_PIN, LOW);
        ledState = 0;
        char payLoad[1];
        itoa(ledState, payLoad, 10);
        client.publish(pubTopic, payLoad);
    }
}

void reconnect()
{
    // Loop until we're reconnected
    while (!client.connected())
    {
        Serial.print("Attempting MQTT connection...");

        // Create a random client ID
        String clientId = "ArduinoClient-";
        clientId += String(random(0xffff), HEX);

        // Attempt to connect
        if (client.connect(clientId.c_str(), mqttUsername, mqttPassword))
        {
            Serial.println("connected");

            // ... and resubscribe
            client.subscribe(subTopic);
        }
        else
        {
            Serial.print("failed, rc=");
            Serial.print(client.state());
            Serial.println(" try again in 5 seconds");
            // Wait 5 seconds before retrying
            delay(5000);
        }
    }
}

void setup_wifi()
{
    delay(10);

    // We start by connecting to a WiFi network
    Serial.println();
    Serial.print("Connecting to ");
    Serial.println(ssid);

    WiFi.begin(ssid, password);

    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        Serial.print(".");
    }

    randomSeed(micros());

    Serial.println("");
    Serial.println("WiFi connected");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());
}