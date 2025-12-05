
// I'm not smart enough to figure this out by myself.  ChatGPT is my friend.  Ray Jorgensen  11/7/2024
// https://github.com/jorgensenray


// Declared because function uses a default parameter:
void BasicPWM(size_t nozzleIndex, uint8_t currentPin, uint8_t dutyPercent = 50);

void PWM_Controls() {

  // Recalculate period based on the current frequency.
  period = static_cast<uint32_t>(1000.0 / userSettings.Hz);
  unsigned long cycleTime = millis() % period;
  bool globalEvenNozzlesActive = (cycleTime < (period / 2));

  for (size_t i = 0; i < pinStates.size(); i++) {

    isPinOn = pinStates[i].state;
    boomLength = (numActiveNozzles * userSettings.SprayWidth);
    uint8_t currentPin = pinStates[i].pinNumber;  // actual Teensy pin from mapping physical pin number (pins start at 1)
    pinNum = currentPin;                          // Update global pinNum for later use

    if (fabs(steerAngle) > userSettings.WheelAngle) {
      TurnComp = true;
    } else {
      TurnComp = false;
    }

    PrintAOGstuff();
    Pressure();
    Flow();

    if (isPinOn == 1) {


      // Conventional PWM (no stagger): just set timing normally.
      if ((userSettings.PWM_Conventional == 1) && (i < numActiveNozzles) && (userSettings.Stagger == 0) && (TurnComp == false)) {
        if (printTimer == 5000) {
          //Serial.println(" Conventional PWM (no stagger): just set timing normally. ");
          printTimer = 0;
        }
        setPWMTiming(userSettings.Hz, newDutyCycle, i);
        ControlNozzles(i, currentPin);
      }

      // Even/Odd (stagger) mode.
      if ((userSettings.PWM_Conventional == 1) && (i < numActiveNozzles) && (userSettings.Stagger == 1) && (TurnComp == false)) {
        if (printTimer == 5000) {
          //Serial.println(" Even/Odd (stagger) mode. ");
          printTimer = 0;
        }
        // Pass the global toggle state to EvenOdd.
        EvenOdd(i, currentPin, globalEvenNozzlesActive);
        ControlNozzles(i, currentPin);
      }

      // Even/Odd (stagger) with turn compensation.
      if ((userSettings.PWM_Conventional == 1) && (i < numActiveNozzles) && (userSettings.Stagger == 1) && (TurnComp == true)) {
        if (printTimer == 5000) {
          //Serial.println(" Even/Odd (stagger) with turn compensation. ");
          printTimer = 0;
        }
        EvenOdd(i, currentPin, globalEvenNozzlesActive);
        // Only apply turn compensation if the nozzle is active (nonzero duty cycle)
        if (OnTime[i] > 0) {
          NozzleSpeed(i, currentPin);
        }
        ControlNozzles(i, currentPin);  // control nozzles with PWM;
      }

      // Conventional with turn compensation (even/odd off).
      if ((userSettings.PWM_Conventional == 1) && (i < numActiveNozzles) && (userSettings.Stagger == 0) && (TurnComp == true)) {
        if (printTimer == 5000) {
          //Serial.println(" Conventional with turn compensation (even/odd off). ");
          printTimer = 0;
        }
        NozzleSpeed(i, currentPin);
        ControlNozzles(i, currentPin);
      }

      // Conventional on/off (non-adjusting - fixed PWM to keep the solenoids cool)
      if (userSettings.PWM_Conventional == 0) {
        digitalWrite(pinNum, HIGH);
        //BasicPWM(i, currentPin);  // Use default 4.5Hz-50% duty cycle
      }
    }

    // isPinOn is false
    else {
      if (printTimer == 5000) {
        //Serial.println(" isPinOn is false ");
        printTimer = 0;
      }
      digitalWrite(pinNum, LOW);
    }
    if (debugPwmLevel == 0) {
      DebugRawPulses();
    }
  }
}

// ----- Fast GPA display filter -----
// float gpaDisplay = 0.0f;           // fast-smoothed GPA for the user - made global
const float gpaDisplayAlpha = 0.45f;  // 0..1, higher = more responsive, more noisy

void Flow() {
  // Only bother if at least one nozzle is on
  if (numActiveNozzles == 0) {
    return;
  }

  // Wait until our flow sample window has elapsed
  if (Flow_Timer > FLOW_SAMPLE_MS) {

    // --- Snapshot pulses atomically ---
    noInterrupts();
    uint32_t pulses = pulseCount;
    pulseCount = 0;  // reset for next window
    interrupts();

    // Sample period in seconds
    const float sec = FLOW_SAMPLE_MS / 1000.0f;

    // --- Compute flow in LPM from pulses over this window ---
    float flowLpm = 0.0f;
    if (sec > 0.0f && PSSL > 0.0f) {
      // pulses/sec
      float pulsesPerSec = (float)pulses / sec;

      // LPM = (pulses/sec) / PSSL * 60
      flowLpm = (pulsesPerSec / PSSL) * 60.0f;
    }

    // --- Smooth LPM with moving average ---
    pulseAvg.addValue(flowLpm);
    float flowLpmAvg = pulseAvg.getAverage();

    // --- Convert to GPM (for display & control) ---
    GPM = flowLpmAvg / 3.78541f;

    // --- Compute GPA (for information) ---
    // --- GPA calculations ---
    // Instant GPA from current (smoothed) GPM
    float gpaInstant = 0.0f;
    if (gpsSpeed > 0.01f && boomLength > 0.0f) {
      gpaInstant = (GPM * 5940.0f) / (gpsSpeed * boomLength);
    }

    // 1) Fast display GPA (EMA) for the user
    //    More responsive than the moving average, but still smoothed.
    if (gpaDisplay == 0.0f) {
      // First-time init to avoid a big jump from 0 → real value
      gpaDisplay = gpaInstant;
    } else {
      gpaDisplay = gpaDisplayAlpha * gpaInstant + (1.0f - gpaDisplayAlpha) * gpaDisplay;
    }

    // 2) Slower moving-average GPA (if you still want it for logs)
    GPA_Avg.addValue(gpaInstant);
    actualGPAave = GPA_Avg.getAverage();

    // Keep actualGPA for compatibility if you use it elsewhere
    actualGPA = gpaInstant;


    // ==========================================================
    //   FLOW-BASED CLOSED-LOOP CONTROL (tames the runaway)
    // ==========================================================
    if (gpsSpeed > 0.01f && boomLength > 0.0f) {
      // How much flow we *should* have at this speed & boom size
      float targetGPM = (userSettings.GPATarget * gpsSpeed * boomLength) / 5940.0f;

      // Error in GPM (positive => we're low, need more duty)
      float errGPM = targetGPM - GPM;

      // User-tunable gain
      float k = userSettings.DutyCycleAdjustment;  // e.g. start with 0.5

      // Proposed duty step this sample
      float deltaDuty = errGPM * k;  // % per sample

      // Clamp how fast duty can move per sample
      if (deltaDuty > maxStep) deltaDuty = maxStep;
      if (deltaDuty < -maxStep) deltaDuty = -maxStep;

      userSettings.currentDutyCycle += deltaDuty;
      userSettings.currentDutyCycle = constrain(userSettings.currentDutyCycle, 0.0f, 100.0f);
    }

    // Use the adjusted duty for PWM / K&H
    newDutyCycle = userSettings.currentDutyCycle;

  // --- Acres calculation (rate + cumulative) ---
  // Only accumulate when we're actually spraying
  if (numActiveNozzles > 0 && gpsSpeed > 0.01f) {
    // boomLength is in inches; convert to feet
    float widthFt = boomLength / 12.0f;

    // Instantaneous acres/hour
    acresPerHour = (gpsSpeed * widthFt) / 8.25f;

    // Time step in hours, based on FLOW_SAMPLE_MS
    float dtHours = (float)FLOW_SAMPLE_MS / 3600000.0f; // ms -> hours

    // Acres sprayed during this sample interval
    float deltaAcres = acresPerHour * dtHours;

    // Accumulate total
    acresTotal += deltaAcres;
  } else {
    // Not spraying – still keep a valid "zero" rate for UI
    acresPerHour = 0.0f;
  }

    // --- Debug output ---
    if (debugPwmLevel == 6) {
      Serial.print(F("Flow/GPA | pulses: "));
      Serial.print(pulses);

      Serial.print(F(" Pressure: "));
      Serial.print(pressure);

      Serial.print(F("  LPM(avg): "));
      Serial.print(flowLpmAvg, 2);
      Serial.print(F("  GPM: "));
      Serial.print(GPM, 3);
      Serial.print(F("  targetGPM: "));
      if (gpsSpeed > 0.01f && boomLength > 0.0f) {
        float targetGPM = (userSettings.GPATarget * gpsSpeed * boomLength) / 5940.0f;
        Serial.print(targetGPM, 3);
      } else {
        Serial.print(F("N/A"));
      }
      Serial.print(F("  GPA target: "));
      Serial.print(userSettings.GPATarget, 2);
      Serial.print(F("  gpaDisplay: "));
      Serial.print(gpaDisplay, 2);

      Serial.print(F("  GPA actual(avg): "));
      Serial.print(actualGPAave, 2);

      Serial.print(F("  duty: "));
      Serial.println(newDutyCycle, 2);
    }

    // Reset the timer for the next sample window
    Flow_Timer = 0;
  }
}


//-------------------------------------------------------------------
// setPWMTiming: Calculate OnTime per nozzle based on duty cycle
//-------------------------------------------------------------------
void setPWMTiming(unsigned int freq, float dutyCycle, size_t nozzleIndex) {
  freq = userSettings.Hz;
  period = static_cast<uint32_t>(1000.0 / freq);
  userSettings.currentDutyCycle = dutyCycle;  // Use the passed dutyCycle

  if (nozzleIndex < numActiveNozzles) {
    OnTime[nozzleIndex] = static_cast<uint32_t>((period * dutyCycle) / 100.0);
  }

  if (debugPwmLevel == 2) {
    if (printTimer > PrintFrequency) {
      Serial.print("newDutyCycle (from parameter) ");
      Serial.print(dutyCycle);
      Serial.print(" OnTime for nozzle ");
      Serial.print(nozzleIndex);
      Serial.print(" = ");
      Serial.println(OnTime[nozzleIndex]);
      Serial.print(" userSettings.Hz ");
      Serial.println(userSettings.Hz);
      printTimer = 0;
    }
  }
}

void EvenOdd(size_t nozzleIndex, uint8_t currentPin, bool globalEvenNozzlesActive) {
  bool isEven = (currentPin % 2 == 0);

  // Decide whether this nozzle is in the active group this half-cycle
  bool isActiveGroup =
    (globalEvenNozzlesActive && isEven) ||  // even pins active when flag is true
    (!globalEvenNozzlesActive && !isEven);  // odd pins active when flag is false

  // Start from the global newDutyCycle computed by your flow/GPA logic
  float appliedDuty = 0.0f;

  if (isActiveGroup) {
    appliedDuty = newDutyCycle;

    // ---- bench-friendly minimum duty ----
    // If GPA logic says "tiny but non-zero", we bump it up so valves actually move.
    const float minActiveDuty = 30.0f;  // <-- try 30% to start
    if (appliedDuty > 0.0f && appliedDuty < minActiveDuty) {
      appliedDuty = minActiveDuty;
    }

  } else {
    appliedDuty = 0.0f;  // inactive group is fully off
  }

  // Clamp to valid range
  appliedDuty = constrain(appliedDuty, 0.0f, 100.0f);

  // Apply timing for this nozzle index
  setPWMTiming(userSettings.Hz, appliedDuty, nozzleIndex);

  // Optional debug
  if (debugPwmLevel == 5) {
    Serial.print("EvenOdd Debug - Nozzle: ");
    Serial.print(nozzleIndex);
    Serial.print(" | Pin: ");
    Serial.print(currentPin);
    Serial.print(" | isEven: ");
    Serial.print(isEven ? "true" : "false");
    Serial.print(" | globalEvenNozzlesActive: ");
    Serial.print(globalEvenNozzlesActive ? "true" : "false");
    Serial.print(" | isActiveGroup: ");
    Serial.print(isActiveGroup ? "true" : "false");
    Serial.print(" | appliedDuty: ");
    Serial.println(appliedDuty);
  }
}
/*
void EvenOdd(size_t nozzleIndex, uint8_t currentPin, bool globalEvenNozzlesActive) {
  bool isEven = (currentPin % 2 == 0);

  // Debug prints showing the decision for this nozzle.
  if (debugPwmLevel == 5) {
    Serial.print("EvenOdd Debug - Nozzle: ");
    Serial.print(nozzleIndex);
    Serial.print(" | Pin: ");
    Serial.print(currentPin);
    Serial.print(" | isEven: ");
    Serial.print(isEven ? "true" : "false");
    Serial.print(" | globalEvenNozzlesActive: ");
    Serial.print(globalEvenNozzlesActive ? "true" : "false");
  }

  // Apply full duty cycle to one group and 0 to the other.
  if (globalEvenNozzlesActive && isEven) {
    if (debugPwmLevel == 5) {
      Serial.println(" => Even branch: setting full duty cycle");
    }
    setPWMTiming(userSettings.Hz, newDutyCycle, nozzleIndex);
  } else if (!globalEvenNozzlesActive && !isEven) {
    if (debugPwmLevel == 5) {
      Serial.println(" => Odd branch: setting full duty cycle");
    }
    setPWMTiming(userSettings.Hz, newDutyCycle, nozzleIndex);
  } else {
    if (debugPwmLevel == 5) {
      Serial.println(" => Inactive branch: setting 0 duty cycle");
    }
    setPWMTiming(userSettings.Hz, 0, nozzleIndex);
  }
}
*/

float EMA(float newValue) {
  filteredADC = alpha * newValue + (1 - alpha) * filteredADC;
  return filteredADC;
}

void Pressure() {
  //Serial.println("in pressure ");
  if (numActiveNozzles > 0) {    // Change pressure only if 1, or more, nozzles are on
    if (read_pressure >= 100) {  // Take readings every 100 milliseconds

      float rawADC = analogRead(PressurePin);                                                     // Read sensor
      float smoothedADC = EMA(rawADC);                                                            // Apply EMA smoothing
      float voltage = smoothedADC * (3.3 / 1023.0);                                               // Convert to voltage
      voltage += userSettings.PSICalibration;                                                     // Apply calibration offset
      pressure = ((voltage - 0.33) * (maxPressure - minPressure)) / (2.97 - 0.33) + minPressure;  // Convert to pressure (psi)

      //float sensorvalue = analogRead(PressurePin);  // Read the pressure pin
      //voltage = sensorvalue * (3.3 / 1023.0);       // Convert the sensor value to voltage
      //voltage += userSettings.PSICalibration;       // Apply an adjustment factor (for calibration)
      //pressure = ((voltage - 0.33) * (maxPressure - minPressure)) / (2.97 - 0.33) + minPressure;

      read_pressure = 0;  // Reset the timer for the next reading
/*
      if (userSettings.Ball_Hyd == 1) {
        Output = userSettings.PressureTarget - pressure;
        if ((Output <= userSettings.LowBallValve) && (Output > 2)) {
          Output = userSettings.LowBallValve;
        }
        if ((Output >= -userSettings.LowBallValve) && (Output < -2)) {
          Output = -userSettings.LowBallValve;
        }
        if ((Output <= 1) && (Output >= -1)) {
          (Output = 0);
        }
      } else {
        Input = pressure;
        myPID.Compute();
      }

      motor.setSpeed(Output);
*/
      if (debugPwmLevel == 4) {
        // if (printTimer == PrintFrequency) {
        Serial.print("smoothedADC ");
        Serial.print(smoothedADC);
        Serial.print(" voltage ");
        Serial.print(voltage);
        Serial.print(" pressure ");
        Serial.println(pressure);
        printTimer = 0;
        //}
      }
    }
  }
}

void BasicPWM(size_t nozzleIndex, uint8_t currentPin, uint8_t dutyPercent) {
  uint32_t basicPeriod = (1000 / userSettings.Hz);  // 222.22ms = 4.5Hz (adjust if needed)
  uint32_t onTime = (basicPeriod * dutyPercent) / 100;

  if (nozzleState[nozzleIndex] == HIGH && PwmTimer[nozzleIndex] >= onTime) {
    digitalWrite(currentPin, LOW);
    nozzleState[nozzleIndex] = LOW;
    PwmTimer[nozzleIndex] = 0;
  } else if (nozzleState[nozzleIndex] == LOW && PwmTimer[nozzleIndex] >= (basicPeriod - onTime)) {
    digitalWrite(currentPin, HIGH);
    nozzleState[nozzleIndex] = HIGH;
    PwmTimer[nozzleIndex] = 0;
  }

  if (pwmCycleTimer >= 1) {
    PwmTimer[nozzleIndex]++;
    pwmCycleTimer = 0;
  }
}

void ControlNozzles(size_t nozzleIndex, uint8_t currentPin) {
  // === Scheduler timing (unchanged) ===
  uint32_t perMs = period;
  if (perMs == 0) {
    digitalWrite(currentPin, LOW);
    return;
  }

  uint32_t onMs = OnTime[nozzleIndex];
  uint32_t phaseMs = PwmTimer[nozzleIndex];

  // If this nozzle is "off" in this PWM cycle, keep it off.
  if (onMs == 0) {
    digitalWrite(currentPin, LOW);
  }

  // === Simple Kick + Hold using millis() ===
  // One state per nozzle index
  struct SimpleKHState {
    bool active;
    bool inKick;
    uint32_t startMs;  // start of this ON window
  };
  static SimpleKHState s[64];

  SimpleKHState& st = s[nozzleIndex];

  // Are we in the ON window this cycle?
  if (phaseMs < onMs && onMs > 0) {

    // Start of ON window: initialize Kick & Hold state
    if (!st.active) {
      st.active = true;
      st.inKick = true;
      st.startMs = millis();
    }

    uint32_t now = millis();
    uint32_t elapsed = now - st.startMs;

    uint32_t kickMs = userSettings.KH_kickDurationMs;  // from PWMdefinitions.h
    if (kickMs > onMs) kickMs = onMs;                  // clamp

    // KICK phase: full ON
    if (elapsed < kickMs) {
      st.inKick = true;
      digitalWrite(currentPin, HIGH);
    } else {
      // HOLD phase: simple software PWM at modest frequency (to match loop rate)
      st.inKick = false;

      // Clamp frequency to something sane for millis timing
      uint32_t freq = userSettings.KH_holdPWMFrequency;
      if (freq < 5) freq = 5;
      if (freq > 50) freq = 50;  // we can't reliably do high kHz in a big sketch

      uint32_t holdPeriodMs = 1000UL / freq;
      float duty = userSettings.KH_holdDutyCycle;  // 0.0–1.0
      if (duty < 0.01f) duty = 0.01f;
      if (duty > 0.9f) duty = 0.9f;

      uint32_t onHoldMs = (uint32_t)(holdPeriodMs * duty);
      uint32_t phaseHold = (now - (st.startMs + kickMs)) % holdPeriodMs;

      if (phaseHold < onHoldMs) {
        digitalWrite(currentPin, HIGH);
      } else {
        digitalWrite(currentPin, LOW);
      }
    }

  } else {
    // OFF window: ensure LOW and reset state for next period
    digitalWrite(currentPin, LOW);
    st.active = false;
    st.inKick = false;
  }

  // === Your existing per-nozzle timing (unchanged) ===
  if (pwmCycleTimer >= 1) {
    PwmTimer[nozzleIndex]++;
    pwmCycleTimer = 0;
  }
  if (PwmTimer[nozzleIndex] >= perMs) {
    PwmTimer[nozzleIndex] = 0;
  }

  // === Old direct-PWM code for reference (now disabled) ===
  /*
  if (nozzleState[nozzleIndex] == HIGH) {
    if (PwmTimer[nozzleIndex] >= OnTime[nozzleIndex]) {
      digitalWrite(currentPin, LOW);
      nozzleState[nozzleIndex] = LOW;
    } else {
      digitalWrite(currentPin, HIGH);
    }
  } else if (nozzleState[nozzleIndex] == LOW) {
    if (PwmTimer[nozzleIndex] == 0) {
      digitalWrite(currentPin, HIGH);
      nozzleState[nozzleIndex] = HIGH;
    } else {
      digitalWrite(currentPin, LOW);
    }
  }
  if (pwmCycleTimer >= 1) {
    PwmTimer[nozzleIndex]++;
    pwmCycleTimer = 0;
  }
  if (PwmTimer[nozzleIndex] >= period) {
    PwmTimer[nozzleIndex] = 0;
  }
  */
}


/*
void ControlNozzles(size_t nozzleIndex, uint8_t currentPin) {
  if (debugPwmLevel == 3) {
    if (printTimer > PrintFrequency) {
      Serial.print("🔍 ControlNozzles - Nozzle: ");
      Serial.print(nozzleIndex);
      Serial.print(" | Pin: ");
      Serial.print(currentPin);
      Serial.print(" | isPinOn: ");
      Serial.print(nozzleState[nozzleIndex]);  // ✅ Use per-nozzle state
      Serial.print(" | PwmTimer[");
      Serial.print(nozzleIndex);
      Serial.print("]: ");
      Serial.print(PwmTimer[nozzleIndex]);
      Serial.print(" | OnTime[");
      Serial.print(nozzleIndex);
      Serial.print("]: ");
      Serial.print(OnTime[nozzleIndex]);
      Serial.print(" | (period - OnTime[");
      Serial.print(nozzleIndex);
      Serial.print("]) = ");
      Serial.println(period - OnTime[nozzleIndex]);
      printTimer = 0;
    }
  }

  // Hardened K&H-driven ControlNozzles: keep your timing, replace only the drive
  //void ControlNozzles(size_t nozzleIndex, uint8_t currentPin) {
  // --- Required timing from your scheduler (unchanged) ---
  // PwmTimer[nozzleIndex] : ms elapsed in this nozzle's current period
  // OnTime[nozzleIndex]   : ms ON window for this nozzle
  // period                : total ms in one PWM cycle (all nozzles share)
  // pwmCycleTimer         : your 1ms heartbeat (elapsedMillis)

  // Safety guards
  uint32_t perMs = period;
  if (perMs == 0) {
    // If something upstream didn't set period yet, fail-safe to LOW and exit.
    digitalWrite(currentPin, LOW);
    return;
  }

  uint32_t onMs = OnTime[nozzleIndex];
  uint32_t phaseMs = PwmTimer[nozzleIndex];

  // If ON window is zero, force LOW and ensure we start fresh next time
  if (onMs == 0) {
    digitalWrite(currentPin, LOW);
    // K&H entry flag will reset below in OFF path
  }

  // One flag per NOZZLE index (safer than pin-number indexing)
  static bool khStartedPerNozzle[64] = { false };

  // ===== K&H waveform over your ON window =====
  if (phaseMs < onMs && onMs > 0) {
    // We are inside the ON window → ensure KH_Begin runs once per window
    if (!khStartedPerNozzle[nozzleIndex]) {
      KH_Begin(currentPin);  // primes Kick phase & resets K&H per-pin state
      khStartedPerNozzle[nozzleIndex] = true;
    }

    // Non-blocking service; it will drive: Kick then Hold PWM, then force LOW at end of onMs
    (void)KH_Service(currentPin, onMs);

  } else {
    // OFF window → ensure pin LOW and arm for the next period's ON window
    digitalWrite(currentPin, LOW);
    khStartedPerNozzle[nozzleIndex] = false;
  }

  // ===== Period/phase accounting (unchanged) =====
  if (pwmCycleTimer >= 1) {
    PwmTimer[nozzleIndex]++;
    pwmCycleTimer = 0;
  }
  if (PwmTimer[nozzleIndex] >= perMs) {
    PwmTimer[nozzleIndex] = 0;
  }

  if (PwmTimer[nozzleIndex] == 0) {
    Serial.printf("K&H begin on pin %u, onMs=%lu, perMs=%lu\r\n", currentPin, (unsigned long)onMs, (unsigned long)perMs);
  }
  //}

  // ===== Original direct-PWM/toggle code (disabled; keep for reference) =====
  // if (nozzleState[nozzleIndex] == HIGH) {
  //   if (PwmTimer[nozzleIndex] >= OnTime[nozzleIndex]) {
  //     digitalWrite(currentPin, LOW);
  //     nozzleState[nozzleIndex] = LOW;
  //   } else {
  //     digitalWrite(currentPin, HIGH);
  //   }
  // } else if (nozzleState[nozzleIndex] == LOW) {
  //   if (PwmTimer[nozzleIndex] == 0) {
  //     digitalWrite(currentPin, HIGH);
  //     nozzleState[nozzleIndex] = HIGH;
  //   } else {
  //     digitalWrite(currentPin, LOW);
  //   }
  // }
  // if (pwmCycleTimer >= 1) {
  //   PwmTimer[nozzleIndex]++;
  //   pwmCycleTimer = 0;
  // }
  // if (PwmTimer[nozzleIndex] >= period) {
  //   PwmTimer[nozzleIndex] = 0;
  // }
}
*/

void NozzleSpeed(size_t nozzleIndex, uint8_t currentPin) {
  // If EvenOdd already set this nozzle off, skip turn compensation.
  if (OnTime[nozzleIndex] == 0) {
    return;
  }

  float tractorSpeedIps = gpsSpeed * 17.6;                 // Convert tractor speed to inches per second
  float turnRadius = 360 / (2 * M_PI * steerAngle / 360);  // Calculate turn radius
  // Calculate the distance from the boom's center for this nozzle.
  float distanceFromCenter = (currentPin - (boomLength / 2.0 / userSettings.SprayWidth)) * userSettings.SprayWidth;
  float nozzleRadius = turnRadius + distanceFromCenter;
  float nozzleSpeedIps = (nozzleRadius / turnRadius) * tractorSpeedIps;
  float nozzleSpeedMph = nozzleSpeedIps / 17.6;

  if (debugPwmLevel == 7) {
    Serial.print("currentPin ");
    Serial.print(currentPin);
    Serial.print(": Speed = ");
    Serial.print(nozzleSpeedMph);
    Serial.println(" MPH");
    printTimer = 0;
  }

  // Now call dutycycleTurncomp with the actual currentPin.
  dutycycleTurncomp(nozzleIndex, currentPin, nozzleSpeedMph);
}


void dutycycleTurncomp(uint8_t nozzleIndex, uint8_t currentPin, float nozzleSpeedMph) {
  // Calculate the actual GPA for the nozzle.
  actualGPA = (GPM * 5940) / (nozzleSpeedMph * userSettings.SprayWidth);
  // Start with the base duty cycle.
  float nozzleDutyCycle = userSettings.currentDutyCycle;
  if (actualGPA < userSettings.GPATarget) {
    nozzleDutyCycle += (userSettings.GPATarget - actualGPA) * userSettings.DutyCycleAdjustment;
  } else if (actualGPA > userSettings.GPATarget) {
    nozzleDutyCycle -= ((actualGPA - userSettings.GPATarget) * (1 + userSettings.DutyCycleAdjustment));
  }

  // Constrain and update the duty cycle.
  newDutyCycle = constrain(nozzleDutyCycle, 0.0, 100.0);
  OnTime[nozzleIndex] = static_cast<uint32_t>(period * (newDutyCycle / 100.0));

  if (debugPwmLevel == 1) {
    Serial.print("Nozzle ");
    Serial.print(nozzleIndex);
    Serial.print(" (Pin ");
    Serial.print(currentPin);
    Serial.print("): newDutyCycle = ");
    Serial.print(newDutyCycle);
    Serial.print(", OnTime = ");
    Serial.print(OnTime[nozzleIndex]);
    Serial.print(": Speed = ");
    Serial.print(nozzleSpeedMph);
    Serial.println(" MPH");
    printTimer = 0;
  }
}


void PrintDebug() {
  if (debugPwmLevel == 8) {
    if (printTimer == PrintFrequency) {
      Serial.print(" gpsSpeed ");
      Serial.print(gpsSpeed);
      Serial.print(" Actual GPA: ");
      Serial.print(actualGPA);
      Serial.print(" onTime: ");
      Serial.print(onTime);
      Serial.print(" period: ");
      Serial.print(period);
      Serial.print(" Pressure: ");
      Serial.println(pressure);
      printTimer = 0;
    }
  }
}


void PrintAOG() {
  if (debugPwmLevel == 9) {
    if (printTimer == PrintFrequency) {
      Serial.print(" steerAngle ");
      Serial.print(steerAngle);
      Serial.print(" gpsSpeed ");
      Serial.print(gpsSpeed);
      Serial.print(" isPinOn ");
      Serial.print(isPinOn);
      Serial.print(" currentPin ");
      Serial.print(currentPin);
      printTimer = 0;
    }
  }
}

void Calibrate_PSI_Flow() {

  digitalWrite(1, HIGH);

  Pressure();

  if (Flow_Timer > 100) {
    detachInterrupt(digitalPinToInterrupt(flowSensorPin));  // Suspend interrupt during calculations

    // Calculate the average pulse count and flow rate (LPM)
    pulseAvg.addValue(pulseCount);
    float pulseCountAve = pulseAvg.getAverage();
    flowRate = pulseCountAve / userSettings.FlowCalibration;  // Flow rate in LPM

    // Convert to GPM and calculate the actual GPA
    GPM = flowRate / 3.785;  // Convert LPM to GPM
    actualGPA = (GPM * 5940) / (gpsSpeed * (userSettings.SprayWidth));
    GPA_Avg.addValue(actualGPA);
    actualGPAave = GPA_Avg.getAverage();

    // Update FlowCalibration based on the difference between target and actual GPA
    float errorGPA = actualGPAave - userSettings.GPATarget;

    float adjustmentFactor = 0.01;

    if ((errorGPA < 0.02) && (errorGPA > 0.00)) {
      Serial.print("Calibration complete - value = ");
      Serial.print(userSettings.FlowCalibration);
      saveuserSettingsToEEPROM();
      Serial.print(" has been saved to EEPROM. ");
    }

    userSettings.FlowCalibration += adjustmentFactor * errorGPA;

    pulseCount = 0;  // Reset pulse counter
    Flow_Timer = 0;  // Reset timer

    attachInterrupt(digitalPinToInterrupt(flowSensorPin), pulseCounter, FALLING);

    // Print debug information
    Serial.print(" Output: ");
    Serial.print(Output);
    Serial.print(" | pressure: ");
    Serial.print(pressure);
    Serial.print(" | FlowCalibration: ");
    Serial.print(userSettings.FlowCalibration, 3);
    Serial.print(" | actualGPAave: ");
    Serial.print(actualGPAave);
    Serial.print(" | GPATarget: ");
    Serial.println(userSettings.GPATarget);
  }
}

void setupNozzles() {
  //if (numActiveNozzles < 1) numActiveNozzles = 1;  // Ensure the number of active nozzles is valid

  for (uint8_t i = 0; i <= numActiveNozzles; i++) {  // Initialize all values to 0
    OnTime[i] = 0;
    PwmTimer[i] = 0;
  }

  //Serial.print("Allocated OnTime & PwmTimer arrays for ");
  //Serial.print(numActiveNozzles);
  //Serial.println(" nozzles.");
}

void pwmtimerstates() {
  static unsigned long lastUpdate = 0;  // Static ensures it retains its value across calls
  for (auto& pinState : pinStates) {
    PwmTimer[pinState.pinNumber] += millis() - lastUpdate;  // Update the timer for the pin
  }
  lastUpdate = millis();  // Update the last timestamp
}

void setDebugPwmLevel(uint8_t level) {
  debugPwmLevel = level;
}

void PrintUserVariables() {
  // Debug: Print all loaded values
  Serial.println();
  Serial.println(F("===== User Settings ====="));

  Serial.print(F("GPATarget: "));
  Serial.println(userSettings.GPATarget);

  Serial.print(F("SprayWidth: "));
  Serial.println(userSettings.SprayWidth);

  Serial.print(F("FlowCalibration: "));
  Serial.println(userSettings.FlowCalibration);

  Serial.print(F("PSICalibration: "));
  Serial.println(userSettings.PSICalibration);

  Serial.print(F("DutyCycleAdjustment: "));
  Serial.println(userSettings.DutyCycleAdjustment);

  Serial.print(F("PressureTarget: "));
  Serial.println(userSettings.PressureTarget);

  Serial.print(F("numberNozzles: "));
  Serial.println(userSettings.numberNozzles);

  Serial.print(F("currentDutyCycle: "));
  Serial.println(userSettings.currentDutyCycle);

  Serial.print(F("Hz: "));
  Serial.println(userSettings.Hz);

  Serial.print(F("LowBallValve: "));
  Serial.println(userSettings.LowBallValve);

  Serial.print(F("Ball_Hyd: "));
  Serial.println(userSettings.Ball_Hyd);

  Serial.print(F("WheelAngle: "));
  Serial.println(userSettings.WheelAngle);

  Serial.print(F("Kp: "));
  Serial.println(userSettings.Kp);

  Serial.print(F("Ki: "));
  Serial.println(userSettings.Ki);

  Serial.print(F("Kd: "));
  Serial.println(userSettings.Kd);

  Serial.print(F("Parsed Unit: "));
  Serial.println(userSettings.unit);

  Serial.print(F("Parsed PWM_Conventional: "));
  Serial.println(userSettings.PWM_Conventional);

  Serial.print(F("Stagger: "));
  Serial.println(userSettings.Stagger);

  Serial.print(F("debugPwmLevel: "));
  Serial.println(userSettings.debugPwmLevel);

  // ---- Kick & Hold (KH) settings ----
  Serial.println(F("----- Kick & Hold (KH) -----"));

  Serial.print(F("KH_kickDurationMs: "));
  Serial.println(userSettings.KH_kickDurationMs);

  Serial.print(F("KH_holdDutyCycle: "));
  Serial.println(userSettings.KH_holdDutyCycle, 4);  // e.g. 0.0400

  Serial.print(F("KH_holdPWMFrequency: "));
  Serial.println(userSettings.KH_holdPWMFrequency);

  Serial.print(F("KH_holdRefV: "));
  Serial.println(userSettings.KH_holdRefV, 2);  // e.g. 12.60

  Serial.println();
}


void PrintAOGstuff() {
  if (debugPwmLevel == 9) {
    Serial.print(" boomLength = ");
    Serial.print(boomLength);
    Serial.print(" numActiveNozzles = ");
    Serial.print(numActiveNozzles);
    Serial.print(" gpsSpeed = ");
    Serial.print(gpsSpeed);
    Serial.print(" currentPin = ");
    Serial.print(currentPin);
    Serial.print(" isPinOn = ");
    Serial.println(isPinOn);
  }
}

float mapfloat(float x, float in_min, float in_max, float out_min, float out_max) {
  return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}

void DebugRawPulses() {
  static uint32_t lastPrintMs = 0;

  if (millis() - lastPrintMs >= 500) {  // print every 500 ms
    lastPrintMs = millis();

    noInterrupts();
    uint32_t total = pulseCountTotal;
    interrupts();

    Serial.print("RAW pulseCountTotal (ever-growing): ");
    Serial.println(total);
  }
}

void resetFlowAverages() {
  pulseAvg.clear();  // clear LPM moving average
  GPA_Avg.clear();   // clear GPA moving average
  actualGPAave = 0.0f;
}
