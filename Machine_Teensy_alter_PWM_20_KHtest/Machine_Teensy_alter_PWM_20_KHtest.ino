
/*

    Matt Elias Feb 2024     https://github.com/m-elias/AOG-Machine
    Adapted from Machine_UDP_v5 in official AOG repo

*/

#include <EEPROM.h>

#include <QNEthernet.h>   // must be first

#ifdef LWIP_IGMP
#undef LWIP_IGMP
#endif

#include <AsyncUDP_Teensy41.h>



using namespace qindesign::network;

IPAddress myIP = { 192, 168, 5, 123 };                             // IP of AGIO default
IPAddress myNetmask = { 255, 255, 255, 0 };                        //QNEthernet requires this to be set.
IPAddress myGW = { 192, 168, 5, 1 };                               //QNEthernet requires this to be set.
IPAddress mydnsServer = { 192, 168, 5, 1 };                        //QNEthernet requires this to be set.
byte mac[6] = { 0x0A, 0x0F, myIP[0], myIP[1], myIP[2], myIP[3] };  // create unique MAC from IP as IP should already be unique

AsyncUDP udpServer;  // AOG server
AsyncUDP udp2;       // Sprayer sever
AsyncUDP udp3;       // Steer server

// Ports to listen on
const uint16_t PORT2 = 7777;    // Sprayer
const uint16_t PORT3 = 9999;    // Steer - listen for PGN 253 Steer Angle
uint16_t udpListenPort = 8888;  // listen for AOG PGNs

// Ports to send on
uint16_t udpSendPort = 9999;         // UDP port to send PGN data back to AgIO/AOG
uint16_t udpSprayerSendPort = 7777;  // UDP port to send PGN data Sprayer

#include "PWMdefinitions.h"
#include "machine.h"
MACHINE machine;
MACHINE::States machineStates;

//const byte numMachineOutputs = 8;
//byte machineOutputPins[numMachineOutputs] = { 12, 13, 5, 23, 19, 18, 21, 22 };
//byte machineOutputPins[numMachineOutputs] = { 12, 13, 5, 23, 19, 18, 21, 22, 14, 27, 16, 17, 25, 26, 4 };

//const byte numMachineOutputs = 17;
//byte machineOutputPins[numMachineOutputs] = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};

const byte numMachineOutputs = 16;
byte machineOutputPins[numMachineOutputs] = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};

void setup() {
  delay(250);
  Serial.begin(115200);

  setupEth_udp();
  setupUDP();
  PWM_setup();
  //loadUserSettingsFromEEPROM();

  EEPROM.begin();  // Initialize EEPROM

  machine.init(100);  // 100 is address for machine EEPROM storage (uses 33 bytes)
  machine.setSectionOutputsHandler(updateSectionOutputs);
  machine.setMachineOutputsHandler(updateMachineOutputs);
  machine.setUdpReplyHandler(pgnReplies);

  setOutputPinModes();
  Serial.print("\r\n\nSetup complete\r\n*******************************************\r\n");
}

void loop() {
  //delay(10);
  yield();

  machine.watchdogCheck();  // Check communication status continuously
  machine.updateStates();   // Continuously update states

  // need for PWM
  static unsigned long lastUpdate = 0;  // Static ensures it retains its value across calls
  for (auto& pinState : pinStates) {
    PwmTimer[pinState.pinNumber] += millis() - lastUpdate;  // Update the timer for the pin
  }
  lastUpdate = millis();  // Update the last timestamp

  PWM_Controls();  // PWN control of system
  //printMemoryUsage();

  //if (Serial.available()) parseSerial();

  if (Serial.available() > 0) {
    int input = Serial.parseInt();
    if (input >= 0 && input <= 12) {
      setDebugPwmLevel((uint8_t)input);
      Serial.print("Debug level set to: ");
      Serial.println(debugPwmLevel);
    } else {
      Serial.println("Invalid level. Enter 0-12.");
    }
    // clear leftover characters
    while (Serial.available()) Serial.read();
  }
}

void parseSerial() {
  if (Serial.read() == 'm' && Serial.available()) {
    machine.debugLevel = Serial.read() - '0';
  }
}