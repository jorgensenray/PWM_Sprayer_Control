void setupEth_udp() {
  if (Ethernet.hardwareStatus() == EthernetNoHardware) {
    Serial.println("\r\n\n*** Ethernet was not found. ***");  // maybe using non Ethernet Teensy?
    return;
  }

  // Initialize Ethernet with the MAC address
  Ethernet.begin(mac, myIP, mydnsServer, myGW, myNetmask);

  // Check if the IP address is correctly assigned
  Serial.print("Ethernet connection set with static IP address: ");
  Serial.println(Ethernet.localIP());
}

void setupUDP() {
  // Set up UDP listener for PORT2    7777  -  Sprayer
  if (udp2.listen(PORT2)) {
    Serial.print("Listening on port ");
    Serial.println(PORT2);
    udp2.onPacket([](AsyncUDPPacket& packet) {
      if (debugPwmLevel == 12) {
        Serial.print("Received packet on port ");
        Serial.println(PORT2);
      }
      handleUDPMessage(packet.data(), packet.length());
    });  // all the brackets and ending ; are necessary!
  } else {
    Serial.print("Failed to start listener on port ");
    Serial.println(PORT2);
  }

  if (udpServer.listen(udpListenPort)) {  // 8888   Machine data
    Serial.print("UDP Listening on: ");
    Serial.println(udpListenPort);
    udpServer.onPacket([](AsyncUDPPacket packet) {
      if (debugPwmLevel == 12) {
        //Serial.print(" Received packet on port ");
        //Serial.println(udpListenPort);
      }
      checkForPGNs(packet);
    });  // all the brackets and ending ; are necessary!
  }

  // Set up UDP listener for PORT3   9999  Steer module 253 PGN
  if (udp3.listen(PORT3)) {
    Serial.print("Listening on port ");
    Serial.println(PORT3);
    udp3.onPacket([](AsyncUDPPacket& packet) {
      if (debugPwmLevel == 12) {
        Serial.print("Received packet on port ");
        Serial.println(PORT3);
      }
      PGN253(packet);
    });  // all the brackets and ending ; are necessary!
  } else {
    Serial.print("Failed to start listener on port ");
    Serial.println(PORT3);
  }
}

void PGN253(AsyncUDPPacket packet) {
  if (packet.data()[3] == 253) {  // Check if this is PGN 253
    //Serial.println(" PGN 253 received!");
    if (packet.length() >= 7) {                                                 // Check if length is valid for ActualSteerAngle
      int16_t actualSteerAngle = (packet.data()[5] | (packet.data()[6] << 8));  // Extract ActualSteerAngle
      float steerAngle = actualSteerAngle / 100.0;                              // Convert to the actual angle
      if (debugPwmLevel == 11) {
        Serial.print(" Actual Steer Angle: ");
        Serial.println(steerAngle);
      }
    }
  }
}

void handleUDPMessage(const uint8_t* data, size_t len) {

  if (len > 512) {  // Prevent buffer overflow
    //Serial.println("Error: UDP message too large!");
    return;
  }

  // Create a null-terminated buffer for the message
  char message[len + 1];
  memcpy(message, data, len);
  message[len] = '\0';  // Null-terminate the message

  //Serial.print("Message received: ");
  //Serial.println(message);  // Log received message

  // Process REQUEST_USER_SETTINGS
  if (strncmp(message, "REQUEST_USER_SETTINGS", strlen("REQUEST_USER_SETTINGS")) == 0) {
    //Serial.println("Processing REQUEST_USER_SETTINGS...");
    loadUserSettingsFromEEPROM();
    sendUserSettingsToCSharp();
    return;
  }

  // Process REQUEST_SENSOR_DATA
  if (strncmp(message, "REQUEST_SENSOR_DATA", strlen("REQUEST_SENSOR_DATA")) == 0) {
    //Serial.println("Processing REQUEST_SENSOR_DATA...");
    sendSensorData();
    return;
  }

  // Process UPDATE_SETTINGS
  if (strncmp(message, "UPDATE_SETTINGS", strlen("UPDATE_SETTINGS")) == 0) {
    //Serial.println("Parsing UPDATE_SETTINGS...");

    float GPATarget, SprayWidth, FlowCalibration, PSICalibration, DutyCycleAdjustment, PressureTarget, currentDutyCycle;
    unsigned int Hz;
    byte numberNozzles, LowBallValve, Ball_Hyd, WheelAngle;
    double Kp, Ki, Kd;
    char Unit[9];              // 8 characters + 1 for null terminator
    uint8_t PWM_Conventional;  // Variable for PWM_Conventional
    uint8_t Stagger;           // Variable for Stagger
    uint8_t debugPwmLevel;     // Variable for debug level
    unsigned long KH_kickDurationMs;
    float KH_holdDutyCycle;
    unsigned long KH_holdPWMFrequency;
    float KH_holdRefV;


    int parsedFields = sscanf(
      message,
      "UPDATE_SETTINGS:GPATarget:%f,SprayWidth:%f,FlowCalibration:%f,"
      "PSICalibration:%f,DutyCycleAdjustment:%f,PressureTarget:%f,numberNozzles:%hhu,currentDutyCycle:%f,"
      "Hz:%u,LowBallValve:%hhu,Ball_Hyd:%hhu,WheelAngle:%hhu,Kp:%lf,Ki:%lf,Kd:%lf,Unit:%8[^,],"
      "PWM_Conventional:%hhu,Stagger:%hhu,debug:%hhu,KH_kickDurationMs:%lu,KH_holdDutyCycle:%f,"
      "KH_holdPWMFrequency:%lu,KH_holdRefV:%f",
      &GPATarget, &SprayWidth, &FlowCalibration, &PSICalibration, &DutyCycleAdjustment, &PressureTarget,
      &numberNozzles, &currentDutyCycle, &Hz, &LowBallValve, &Ball_Hyd, &WheelAngle,
      &Kp, &Ki, &Kd, Unit, &PWM_Conventional, &Stagger, &debugPwmLevel,
      &KH_kickDurationMs, &KH_holdDutyCycle, &KH_holdPWMFrequency, &KH_holdRefV);


    // Ensure the Unit string is null-terminated
    Unit[8] = '\0';

    // Save values to userSettings struct// Save the parsed Unit
    userSettings.GPATarget = GPATarget;
    userSettings.SprayWidth = SprayWidth;
    userSettings.FlowCalibration = FlowCalibration;
    userSettings.PSICalibration = PSICalibration;
    userSettings.DutyCycleAdjustment = DutyCycleAdjustment;
    userSettings.PressureTarget = PressureTarget;
    userSettings.numberNozzles = numberNozzles;
    userSettings.currentDutyCycle = currentDutyCycle;
    userSettings.Hz = Hz;
    userSettings.LowBallValve = LowBallValve;
    userSettings.Ball_Hyd = Ball_Hyd;
    userSettings.WheelAngle = WheelAngle;
    userSettings.Kp = Kp;
    userSettings.Ki = Ki;
    userSettings.Kd = Kd;
    strncpy(userSettings.unit, Unit, sizeof(userSettings.unit));
    userSettings.PWM_Conventional = PWM_Conventional;
    userSettings.Stagger = Stagger;
    userSettings.debugPwmLevel = debugPwmLevel;
    userSettings.KH_kickDurationMs = KH_kickDurationMs;
    userSettings.KH_holdDutyCycle = KH_holdDutyCycle;
    userSettings.KH_holdPWMFrequency = KH_holdPWMFrequency;
    userSettings.KH_holdRefV = KH_holdRefV;

    /*
    Serial.print("Size of UserSettings: ");
    Serial.println(sizeof(UserSettings));  // Debug size
    Serial.println(" ");
    Serial.print("Parsed fields: ");
    Serial.println(parsedFields);
    Serial.println(" ");
    Serial.print("Updateding Parsed fields: ");
    Serial.println(" ");
*/

    PrintUserVariables();

    saveuserSettingsToEEPROM();

    loadUserSettingsFromEEPROM();

    //printStructDetails();

    if (parsedFields <= 0) {
      return;  // or handle parse failure
    }
  }
}

void saveuserSettingsToEEPROM() {
  Serial.println("🔧 Saving settings to EEPROM...");
  Serial.print("Saving unit string: '");
  Serial.println(userSettings.unit);
  Serial.println("'");

  EEPROM.put(USER_SETTINGS_ADDR, userSettings);
  EEPROM.update(EEPROM_MAGIC_ADDR, EEPROM_MAGIC_VALUE);

  if (debugPwmLevel == 12) {
    //Serial.println(" ");
    //Serial.println("User settings saved to EEPROM.");
    PrintUserVariables();
  }
  //Serial.println(" ");
  //Serial.println("Settings After saving:");
  //Serial.println(" ");
  PrintUserVariables();
}

bool isEEPROMInitialized() {
  return EEPROM.read(EEPROM_MAGIC_ADDR) == EEPROM_MAGIC_VALUE;
}

bool isValidUnit(const char* unit) {
  return (strncmp(unit, "Imperial", sizeof(userSettings.unit)) == 0 || strncmp(unit, "Metric", sizeof(userSettings.unit)) == 0);
}


void loadUserSettingsFromEEPROM() {
  if (isEEPROMInitialized()) {
    EEPROM.get(USER_SETTINGS_ADDR, userSettings);

    if (!isValidUnit(userSettings.unit)) {
      //Serial.println("Invalid unit string, resetting.");
      strncpy(userSettings.unit, "Imperial", sizeof(userSettings.unit));
      userSettings.unit[sizeof(userSettings.unit) - 1] = '\0';
    }

    Serial.println("User settings loaded from EEPROM.");
    Serial.println();
  } else {
    Serial.println("EEPROM not initialized, using defaults.");
    // set default values here...
  }

  debugPwmLevel = userSettings.debugPwmLevel;
  PrintUserVariables();
}

void sendSensorData() {
  udp2.flush();        // clear before send
  char buffer[192];    // a bit larger to hold extra fields

  // Duty for nozzle 1 (same as before)
  float dutyPercent = (period > 0) ? (OnTime[1] * 100.0f / period) : 0.0f;

  int len = snprintf(
      buffer,
      sizeof(buffer),
      "SENSOR_DATA:"
      "pressure:%.2f,"
      "onTime:%.2f,"
      "gpaDisplay:%.2f,"
      "gpsSpeed:%.2f,"
      "acresTotal:%.3f,"
      "acresPerHour:%.2f",
      pressure,
      dutyPercent,
      gpaDisplay,
      gpsSpeed,
      acresTotal,      // <- make sure these are defined & updated
      acresPerHour     // <- in your Flow()/acres logic
  );

  if (len > 0 && len < (int)sizeof(buffer)) {  // Check if formatting was successful and fits within buffer
    if (!udpServer.broadcastTo(buffer, udpSprayerSendPort)) {
      Serial.println("Failed to send sensor data.");  // Log failure
    } else {
      if (debugPwmLevel == 12) {
        Serial.println("Sensor data sent successfully.");  // Log success
      }
    }
  } else {
    Serial.println("Error formatting sensor data.");  // Formatting error
  }
}


void sendUserSettingsToCSharp() {
  char buffer[800];
  int len = snprintf(
    buffer, sizeof(buffer),
    "USER_SETTINGS:GPATarget:%.2f,SprayWidth:%.2f,FlowCalibration:%.2f,"
    "PSICalibration:%.2f,DutyCycleAdjustment:%.2f,PressureTarget:%.2f,"
    "numberNozzles:%hhu,currentDutyCycle:%.2f,Hz:%u,LowBallValve:%hhu,"
    "Ball_Hyd:%hhu,WheelAngle:%hhu,Kp:%.2f,Ki:%.2f,Kd:%.2f,Unit:%s,"
    "PWM_Conventional:%hhu,Stagger:%hhu,debug:%hhu,KH_kickDurationMs:%lu,"
    "KH_holdDutyCycle:%.4f,KH_holdPWMFrequency:%lu,KH_holdRefV:%.2f",
    userSettings.GPATarget, userSettings.SprayWidth, userSettings.FlowCalibration,
    userSettings.PSICalibration, userSettings.DutyCycleAdjustment, userSettings.PressureTarget,
    userSettings.numberNozzles, userSettings.currentDutyCycle, userSettings.Hz,
    userSettings.LowBallValve, userSettings.Ball_Hyd, userSettings.WheelAngle,
    userSettings.Kp, userSettings.Ki, userSettings.Kd, userSettings.unit,
    userSettings.PWM_Conventional, userSettings.Stagger, userSettings.debugPwmLevel,
    userSettings.KH_kickDurationMs, userSettings.KH_holdDutyCycle,
    userSettings.KH_holdPWMFrequency, userSettings.KH_holdRefV);

  udp2.flush();  // clear before send

  // Check if the formatted string fits in the buffer
  if (len < 0 || len >= (int)sizeof(buffer)) {
    Serial.println("Error: Buffer too small for user settings.");
    return;
  }

  // Send the formatted data over UDP
  if (!udp2.broadcastTo(buffer, udpSprayerSendPort)) {
    Serial.println("Failed to send user settings.");  // Log failure
  } else {

    if (debugPwmLevel == 12) {
      Serial.println("User settings sent successfully.");  // Log success
      PrintUserVariables();
    }
  }
  udp2.broadcastTo(buffer, udpSprayerSendPort);
  Serial.println("User settings sent to C#:");
  Serial.println(buffer);  // Debug transmission
}

void clearAndResetEEPROM() {
  // Clear memory first to avoid garbage
  userSettings = UserSettings{};  // zero-initialize all fields

  // Wipe EEPROM
  for (int i = 0; i < EEPROM.length(); i++) {
    EEPROM.write(i, 0xFF);
  }

  // Assign default values
  userSettings.GPATarget = 7.0;
  userSettings.SprayWidth = 36.0;
  userSettings.FlowCalibration = 7.71;
  userSettings.PSICalibration = 0.08;
  userSettings.DutyCycleAdjustment = 0.10;
  userSettings.PressureTarget = 20.0;
  userSettings.numberNozzles = 1;
  userSettings.currentDutyCycle = 50.0;
  userSettings.Hz = 4;
  userSettings.LowBallValve = 28;
  userSettings.Ball_Hyd = 1;
  userSettings.WheelAngle = 5;
  userSettings.Kp = 1.0;
  userSettings.Ki = 0.1;
  userSettings.Kd = 0.01;
  strncpy(userSettings.unit, "Imperial", sizeof(userSettings.unit));
  userSettings.unit[sizeof(userSettings.unit) - 1] = '\0';  // ensure null-termination
  userSettings.PWM_Conventional = 0;
  userSettings.Stagger = 0;
  userSettings.debugPwmLevel = 0;

  // Save to EEPROM
  saveuserSettingsToEEPROM();

  Serial.println("EEPROM cleared and reset with default values.");
}



void saveDebugLevelToEEPROM() {
  EEPROM.update(400, userSettings.debugPwmLevel);  // Use an arbitrary offset like 100
}

void loadDebugLevelFromEEPROM() {
  userSettings.debugPwmLevel = EEPROM.read(400);
  Serial.println(" ");
  Serial.print("Loaded debugPwmLevel: ");
  Serial.println(userSettings.debugPwmLevel);
}

void printStructDetails() {
  Serial.println(" ");
  Serial.print("Size of UserSettings struct: ");
  Serial.println(sizeof(UserSettings));
  Serial.println(" ");
  Serial.print("Address of debugPwmLevel: ");
  Serial.println((uintptr_t)&userSettings.debugPwmLevel, HEX);
  Serial.println(" ");
}

// callback function for Machine class to send data back to AgIO/AOG
void pgnReplies(const uint8_t* pgnData, uint8_t len, IPAddress destIP) {
  udpServer.writeTo(pgnData, len, destIP, udpSendPort);
}

void handleSerialInput() {
  if (Serial.available() > 0) {
    String message = Serial.readStringUntil('\n');   // Read a full line from Serial
    Serial.println("Message received: " + message);  // Log received messages

    if (message.startsWith("REQUEST_USER_SETTINGS")) {
      Serial.println("REQUEST_USER_SETTINGS");
      loadUserSettingsFromEEPROM();
      sendUserSettingsToCSharp();
    }
    else if (message.startsWith("REQUEST_SENSOR_DATA")) {
      sendSensorData();
    }
    // Handle Start/Stop Sprayer Commands
    else if (message == "START_SPRAYER") {
      if (!sprayerOn) {
        sprayerOn = true;
        Serial.println("Sprayer started.");
      } else {
        Serial.println("Sprayer already running.");
      }
    }
    else if (message == "STOP_SPRAYER") {
      if (sprayerOn) {
        sprayerOn = false;
        Serial.println("Sprayer stopped.");
      } else {
        Serial.println("Sprayer already stopped.");
      }
    }

    // Process command strings for switches and settings
    if (message.startsWith("SET_SWITCHES")) {
      PWM_Conventional = message.indexOf("PWM_Conventional:1") != -1;
      Stagger          = message.indexOf("Stagger:1") != -1;
    }
    else if (message.startsWith("SET_DEBUG")) {
      // debugPwmLevel = message.substring(message.indexOf("debug:") + 6).toInt();
    }
    else if (message.startsWith("UPDATE_SETTINGS")) {
      // Local variables for parsing
      float        GPATarget, SprayWidth, FlowCalibration, PSICalibration;
      float        DutyCycleAdjustment, PressureTarget, currentDutyCycle;
      unsigned int Hz;
      byte         numberNozzles, LowBallValve, Ball_Hyd, WheelAngle;
      double       Kp, Ki, Kd;
      char         Unit[9];              // 8 chars + null
      uint8_t      PWM_Conventional_local;
      uint8_t      Stagger_local;
      uint8_t      debugPwmLevel_local;
      unsigned long KH_kickDurationMs;
      float         KH_holdDutyCycle;
      unsigned long KH_holdPWMFrequency;
      float         KH_holdRefV;

      int parsedFields = sscanf(
        message.c_str(),
        "UPDATE_SETTINGS:GPATarget:%f,SprayWidth:%f,FlowCalibration:%f,"
        "PSICalibration:%f,DutyCycleAdjustment:%f,PressureTarget:%f,numberNozzles:%hhu,currentDutyCycle:%f,"
        "Hz:%u,LowBallValve:%hhu,Ball_Hyd:%hhu,WheelAngle:%hhu,Kp:%lf,Ki:%lf,Kd:%lf,Unit:%8[^,],"
        "PWM_Conventional:%hhu,Stagger:%hhu,debug:%hhu,KH_kickDurationMs:%lu,KH_holdDutyCycle:%f,"
        "KH_holdPWMFrequency:%lu,KH_holdRefV:%f",
        &GPATarget, &SprayWidth, &FlowCalibration, &PSICalibration,
        &DutyCycleAdjustment, &PressureTarget, &numberNozzles, &currentDutyCycle,
        &Hz, &LowBallValve, &Ball_Hyd, &WheelAngle,
        &Kp, &Ki, &Kd, Unit,
        &PWM_Conventional_local, &Stagger_local, &debugPwmLevel_local,
        &KH_kickDurationMs, &KH_holdDutyCycle, &KH_holdPWMFrequency, &KH_holdRefV
      );

      if (parsedFields >= 24) {  // sanity check
        // Copy into userSettings
        userSettings.GPATarget        = GPATarget;
        userSettings.SprayWidth       = SprayWidth;
        userSettings.FlowCalibration  = FlowCalibration;
        userSettings.PSICalibration   = PSICalibration;
        userSettings.DutyCycleAdjustment = DutyCycleAdjustment;
        userSettings.PressureTarget   = PressureTarget;
        userSettings.numberNozzles    = numberNozzles;
        userSettings.currentDutyCycle = currentDutyCycle;
        userSettings.Hz               = Hz;
        userSettings.LowBallValve     = LowBallValve;
        userSettings.Ball_Hyd         = Ball_Hyd;
        userSettings.WheelAngle       = WheelAngle;
        userSettings.Kp               = Kp;
        userSettings.Ki               = Ki;
        userSettings.Kd               = Kd;

        // Unit string (ensure null-terminated)
        strncpy(userSettings.unit, Unit, sizeof(userSettings.unit));
        userSettings.unit[sizeof(userSettings.unit) - 1] = '\0';

        userSettings.PWM_Conventional = PWM_Conventional_local;
        userSettings.Stagger          = Stagger_local;
        userSettings.debugPwmLevel    = debugPwmLevel_local;

        userSettings.KH_kickDurationMs   = KH_kickDurationMs;
        userSettings.KH_holdDutyCycle    = KH_holdDutyCycle;
        userSettings.KH_holdPWMFrequency = KH_holdPWMFrequency;
        userSettings.KH_holdRefV         = KH_holdRefV;

        // Also update global flags used elsewhere
        PWM_Conventional = PWM_Conventional_local;
        Stagger          = Stagger_local;
        debugPwmLevel    = debugPwmLevel_local;

        // Save & reload to verify
        saveuserSettingsToEEPROM();
        loadUserSettingsFromEEPROM();
        PrintUserVariables();
      } else {
        Serial.print(F("UPDATE_SETTINGS parse error, fields parsed: "));
        Serial.println(parsedFields);
      }
    }
  }
}
