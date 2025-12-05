// callback function triggered by Machine class to update 1-64 "section" outputs only
void updateSectionOutputs() {
  if (debugPwmLevel == 8) {
    Serial.print("\r\n*** Section Outputs update! *** ");
  }
}

// callback function triggered by Machine class to update "machine" outputs
// this updates the Machine Module Pin Configuration outputs
// - sections 1-16, Hyd Up/Down, Tramline Right/Left, Geo Stop
void updateMachineOutputs() {
  //Serial.print("\r\nMachine Outputs update! ");

  updatePinStates();
  //setupNozzles();

  for (uint8_t i = 1; i < numMachineOutputs; i++) {
    /*
    Serial.print("\r\n- Pin ");
    Serial.print((machineOutputPins[i] < 10 ? " " : ""));
    Serial.print(machineOutputPins[i]); Serial.print(": ");
    Serial.print(machine.state.functions[machine.config.pinFunction[i - 1]]);
    Serial.print(" ");
    Serial.print(machine.functionNames[machine.config.pinFunction[i - 1]]);
    */

    //digitalWrite(machineOutputPins[i], machine.state.functions[machine.config.pinFunction[i]] == machine.config.isPinActiveHigh);  // ==, XOR
  }
}
/*
void updatePinStates() {   // “send me the full updatePinStates() with reset included”
  pinStates.clear();     // Clear previous states
  numActiveNozzles = 0;  // Reset the count

  for (uint8_t i = 0; i < numMachineOutputs; i++) {
    uint8_t pinNum = machineOutputPins[i];  // pin 1–16
    bool isPinOn = static_cast<bool>(
      machine.state.functions[machine.config.pinFunction[i]]);

    bool found = false;
    for (auto& pinState : pinStates) {
      if (pinState.pinNumber == pinNum) {
        // Update state if needed
        if (pinState.state != isPinOn) {
          pinState.state = isPinOn;
        }
        if (isPinOn) {
          numActiveNozzles++;
        }
        found = true;
        break;
      }
    }

    if (!found) {
      pinStates.push_back({ pinNum, isPinOn });
      if (isPinOn) {
        numActiveNozzles++;
      }
    }
  }

  setupNozzles();  // adjust K&H memory to match active nozzles
}
*/

void updatePinStates() {
  // Clear previous states to avoid duplicates
  pinStates.clear();
  numActiveNozzles = 0;

  // Walk all configured machine outputs (pins 1..16)
  for (uint8_t i = 0; i < numMachineOutputs; i++) {
    uint8_t pinNumLocal = machineOutputPins[i];

    // Map this output index -> machine function -> ON/OFF state
    bool isOnLocal = static_cast<bool>(
      machine.state.functions[machine.config.pinFunction[i]]
    );

    // Look for an existing entry for this pin
    bool found = false;
    for (auto& pinState : pinStates) {
      if (pinState.pinNumber == pinNumLocal) {
        if (pinState.state != isOnLocal) {
          pinState.state = isOnLocal;
        }
        if (isOnLocal) {
          numActiveNozzles++;
        }
        found = true;
        break;
      }
    }

    // If pin not yet present, add it
    if (!found) {
      pinStates.push_back({ pinNumLocal, isOnLocal });
      if (isOnLocal) {
        numActiveNozzles++;
      }
    }
  }

  // Keep nozzle timing arrays aligned
  setupNozzles();

  // ---- NEW: detect OFF→ON transition & reset flow filters ----
  static bool spraying = false;
  bool nowSpraying = (numActiveNozzles > 0);

  if (nowSpraying && !spraying) {
    resetFlowAverages();   // fresh GPA/flow for this spray run
  }

  spraying = nowSpraying;
}

void setOutputPinModes() {
  if (numMachineOutputs > 0) {
    for (uint8_t i = 0; i < numMachineOutputs; i++) {
      pinMode(machineOutputPins[i], OUTPUT);
      digitalWrite(machineOutputPins[i], LOW);  // set OFF
    }
  }
}
