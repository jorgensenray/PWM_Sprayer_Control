using SprayerControl;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace ArduinoControlApp
{
    public enum Units
    {
        Imperial,
        Metric
    }

    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer sensorDataTimer;
        private Keypad.NumKeypad numKeypad; // Declare an instance of NumKeypad
        private TextBox activeTextBox;      // Tracks which text box is currently active for data entry
        private readonly UdpClient udpClient;
        private IPEndPoint arduinoEndPoint;

        private bool settingsSent = false;
        private bool isExpanded = false; // Track the current state of the form
        private bool isSprayerOn = false; // Track the current state of the sprayer

        private Units currentUnit = Units.Imperial;  // Default to Imperial

        public Form1()
        {
            InitializeComponent();

            // Initial unit selection; items will be reconfigured in Form1_Load
            CmbUnits.SelectedIndex = 0; // Default to first item (expected Imperial)
            CmbUnits.SelectedIndexChanged += CmbUnits_SelectedIndexChanged;

            numKeypad = new Keypad.NumKeypad
            {
                StartPosition = FormStartPosition.Manual, // Allows us to position the keypad manually
                TopMost = true, // Keeps the keypad on top of Form1
                ShowInTaskbar = false // Prevents it from appearing in the taskbar
            };

            // Subscribe to Click events data entry text boxes
            txtGPATarget.Click += TextBox_Click;
            txtSprayWidth.Click += TextBox_Click;
            txtFlowCalibration.Click += TextBox_Click;
            txtPSICalibration.Click += TextBox_Click;
            txtDutyCycleAdjustment.Click += TextBox_Click;
            txtPressureTarget.Click += TextBox_Click;
            txtnumberNozzles.Click += TextBox_Click;
            txtcurrentDutyCycle.Click += TextBox_Click;
            txtHz.Click += TextBox_Click;
            txtLowBallValve.Click += TextBox_Click;
            txtBall_Hyd.Click += TextBox_Click;
            txtWheelAngle.Click += TextBox_Click;
            txtKp.Click += TextBox_Click;
            txtKi.Click += TextBox_Click;
            txtKd.Click += TextBox_Click;
            txtKH_kickDurationMs.Click += TextBox_Click;
            txtKH_holdDutyCycle.Click += TextBox_Click;
            txtKH_holdPWMFrequency.Click += TextBox_Click;
            txtKH_holdRefV.Click += TextBox_Click;

            // Subscribe to the ButtonPressed event
            numKeypad.ButtonPressed += NumKeypad_ButtonPressed;

            PopulateDebugLevelComboBox();
            Debug.WriteLine("Populated Debug Level ComboBox.");

            // All communicatiion through port 7777
            udpClient = new UdpClient(7777);
            arduinoEndPoint = new IPEndPoint(IPAddress.Parse("192.168.5.123"), 7777);
            Debug.WriteLine($"Initialized arduinoEndPoint: {arduinoEndPoint.Address}:{arduinoEndPoint.Port}");

            StartListening();

            sensorDataTimer = new System.Windows.Forms.Timer
            {
                Interval = 2000 // 2 seconds (2000 milliseconds)
            };
            sensorDataTimer.Tick += SensorDataTimer_Tick; // Attach event handler
            sensorDataTimer.Start(); // Start the timer
        }

        private void SensorDataTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Sending REQUEST_SENSOR_DATA...");

                // Explicitly set the correct endpoint
                arduinoEndPoint = new IPEndPoint(IPAddress.Parse("192.168.5.123"), 7777);

                // Prepare the request message
                string requestMessage = "REQUEST_SENSOR_DATA";
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestMessage);

                // Send the request over UDP
                udpClient.Send(requestBytes, requestBytes.Length, arduinoEndPoint);

                Debug.WriteLine("REQUEST_SENSOR_DATA sent successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending REQUEST_SENSOR_DATA: {ex.Message}");
            }
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.ReadOnly || !textBox.Enabled)
                {
                    return; // Ignore non-editable fields
                }

                // Clear the text in the clicked field
                textBox.Clear();

                // Set the active text box
                activeTextBox = textBox;

                // Reset the background color of all text boxes
                foreach (Control control in this.Controls)
                {
                    if (control is TextBox tb)
                    {
                        tb.BackColor = System.Drawing.Color.White;
                    }
                }

                // Highlight the active text box
                activeTextBox.BackColor = System.Drawing.Color.SkyBlue;

                // Position the keypad relative to the text box
                var textBoxPosition = this.PointToScreen(textBox.Location);
                int x = textBoxPosition.X + textBox.Width + 10; // Place keypad to the right of the text box
                int y = textBoxPosition.Y - 150; // Slightly above the field

                numKeypad.Location = new System.Drawing.Point(x, y);

                // Show the keypad
                numKeypad.Show();
            }
        }

        private bool ValidateAllFields()
        {
            bool isValid = true;
            StringBuilder errorMessages = new StringBuilder();

            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    // Log for debugging purposes
                    Debug.WriteLine($"Validating: {textBox.Name}, Value: '{textBox.Text}'");

                    string value = textBox.Text;

                    if (textBox == txtGPATarget)
                    {
                        if (currentUnit == Units.Imperial)
                        {
                            // Validate Imperial
                            if (!decimal.TryParse(value, out decimal gpa) || gpa < 0 || gpa > 30)
                            {
                                isValid = false;
                                textBox.BackColor = Color.LightCoral;
                                errorMessages.AppendLine("GPA Target must be a number between 0 and 30 GPA.");
                            }
                            else
                            {
                                textBox.BackColor = Color.White;
                            }
                        }
                        else
                        {
                            // Validate Metric
                            if (!decimal.TryParse(value, out decimal gpaMetric) || gpaMetric < 0 || gpaMetric > (30 * 9.35m))
                            {
                                isValid = false;
                                textBox.BackColor = Color.LightCoral;
                                errorMessages.AppendLine($"GPA Target must be a number between 0 and {(30 * 9.35m):F2} Liters/Ha.");
                            }
                            else
                            {
                                textBox.BackColor = Color.White;
                            }
                        }
                    }

                    if (textBox == txtPressureTarget)
                    {
                        if (currentUnit == Units.Imperial)
                        {
                            // Validate Imperial
                            if (!decimal.TryParse(value, out decimal pressure) || pressure < 0 || pressure > 100)
                            {
                                isValid = false;
                                textBox.BackColor = Color.LightCoral;
                                errorMessages.AppendLine("Pressure Target must be a number between 0 and 100 PSI.");
                            }
                            else
                            {
                                textBox.BackColor = Color.White;
                            }
                        }
                        else
                        {
                            // Validate Metric
                            if (!decimal.TryParse(value, out decimal pressureMetric) || pressureMetric < 0 || pressureMetric > (100 * 6.895m))
                            {
                                isValid = false;
                                textBox.BackColor = Color.LightCoral;
                                errorMessages.AppendLine($"Pressure Target must be a number between 0 and {(100 * 6.895m):F2} kPa.");
                            }
                            else
                            {
                                textBox.BackColor = Color.White;
                            }
                        }
                    }

                    if (textBox == txtBall_Hyd)
                    {
                        if (string.IsNullOrWhiteSpace(textBox.Text) || !decimal.TryParse(textBox.Text, out decimal BH) || BH < 0 || BH > 1)
                        {
                            isValid = false;
                            textBox.BackColor = System.Drawing.Color.LightCoral;
                            errorMessages.AppendLine($"Ball Hydraulics: Enter a number between 0 and 1. (You entered: '{textBox.Text}')");
                        }
                        else
                        {
                            textBox.BackColor = System.Drawing.Color.White;
                        }
                    }

                    if (textBox == txtSprayWidth)
                    {
                        if (currentUnit == Units.Imperial)
                        {
                            // Validate Imperial
                            if (!decimal.TryParse(value, out decimal sprayWidth) || sprayWidth < 0 || sprayWidth > 45)
                            {
                                isValid = false;
                                textBox.BackColor = Color.LightCoral;
                                errorMessages.AppendLine("Spray Width must be a number between 0 and 45 feet.");
                            }
                            else
                            {
                                textBox.BackColor = Color.White;
                            }
                        }
                        else
                        {
                            // Validate Metric
                            if (!decimal.TryParse(value, out decimal sprayWidthMetric) || sprayWidthMetric < 0 || sprayWidthMetric > (45 * 0.3048m))
                            {
                                isValid = false;
                                textBox.BackColor = Color.LightCoral;
                                errorMessages.AppendLine($"Spray Width must be a number between 0 and {(45 * 0.3048m):F2} meters.");
                            }
                            else
                            {
                                textBox.BackColor = Color.White;
                            }
                        }
                    }

                    if (textBox == txtKH_kickDurationMs)
                    {
                        if (!decimal.TryParse(value, out decimal kickMs) || kickMs <= 0 || kickMs > 20)
                        {
                            isValid = false;
                            textBox.BackColor = Color.LightCoral;
                            errorMessages.AppendLine("Kick Duration must be a number between 0 and 20 ms (2–4 ms typical).");
                        }
                        else
                        {
                            textBox.BackColor = Color.White;
                        }
                    }

                    if (textBox == txtKH_holdDutyCycle)
                    {
                        if (!decimal.TryParse(value, out decimal holdDuty) || holdDuty < 0.01m || holdDuty > 0.30m)
                        {
                            isValid = false;
                            textBox.BackColor = Color.LightCoral;
                            errorMessages.AppendLine("Hold Duty Cycle must be between 0.01 and 0.30.");
                        }
                        else
                        {
                            textBox.BackColor = Color.White;
                        }
                    }

                    if (textBox == txtKH_holdPWMFrequency)
                    {
                        if (!decimal.TryParse(value, out decimal holdFreq) || holdFreq < 5 || holdFreq > 200)
                        {
                            isValid = false;
                            textBox.BackColor = Color.LightCoral;
                            errorMessages.AppendLine("Hold PWM Frequency must be between 5 and 200 Hz");
                        }
                        else
                        {
                            textBox.BackColor = Color.White;
                        }
                    }

                    if (textBox == txtKH_holdRefV)
                    {
                        if (!decimal.TryParse(value, out decimal holdRefV) || holdRefV < 6 || holdRefV > 16)
                        {
                            isValid = false;
                            textBox.BackColor = Color.LightCoral;
                            errorMessages.AppendLine("Hold Reference Voltage must be between 6.0 and 16.0 V");
                        }
                        else
                        {
                            textBox.BackColor = Color.White;
                        }
                    }

                }
            }

            if (!isValid)
            {
                MessageBox.Show(errorMessages.ToString(), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return isValid;
        }

        private void NumKeypad_ButtonPressed(object sender, KeyPressEventArgs e)
        {
            if (activeTextBox == null) return;

            char key = e.KeyChar;

            switch (key)
            {
                case 'C': // Clear the input
                    activeTextBox.Text = "";
                    break;

                case 'B': // Backspace
                    if (activeTextBox.Text.Length > 0)
                    {
                        activeTextBox.Text = activeTextBox.Text.Substring(0, activeTextBox.Text.Length - 1);
                    }
                    break;

                case 'K': // OK - Validate and finalize input
                    if (ValidateInput(activeTextBox.Text))
                    {
                        numKeypad.Hide();
                        activeTextBox = null; // Clear the active text box
                    }
                    break;

                case 'X': // Cancel
                    numKeypad.Hide();
                    activeTextBox = null;
                    break;

                case '-': // Toggle positive/negative sign
                    if (!string.IsNullOrEmpty(activeTextBox.Text))
                    {
                        // If the value is negative, remove the '-' sign
                        if (activeTextBox.Text.StartsWith("-"))
                        {
                            activeTextBox.Text = activeTextBox.Text.Substring(1);
                        }
                        else
                        {
                            // Add the '-' sign if not already negative
                            activeTextBox.Text = "-" + activeTextBox.Text;
                        }
                    }
                    else
                    {
                        // If the field is empty, start with a negative sign
                        activeTextBox.Text = "-";
                    }
                    break;

                default: // Append numeric input
                    if (char.IsDigit(key) || key == '.')
                    {
                        if (key == '.' && activeTextBox.Text.Contains(".")) return; // Prevent multiple decimals
                        activeTextBox.Text += key;
                    }
                    break;
            }
        }

        private bool ValidateInput(string input)
        {
            if (activeTextBox == null) return true; // No active field to validate

            switch (activeTextBox.Name)
            {
                case "txtGPATarget":
                    if (currentUnit == Units.Imperial)
                    {
                        // Validate Imperial
                        if (!decimal.TryParse(input, out decimal gpa) || gpa < 0 || gpa > 30)
                        {
                            ShowValidationError("GPA Target must be a number between 0 and 30 GPA.");
                            return false;
                        }
                    }
                    else
                    {
                        // Validate Metric
                        if (!decimal.TryParse(input, out decimal gpaMetric) || gpaMetric < 0 || gpaMetric > (30 * 9.35m))
                        {
                            ShowValidationError($"GPA Target must be a number between 0 and {(30 * 9.35m):F2} Liters/Ha.");
                            return false;
                        }
                    }
                    break;

                case "txtPressureTarget":
                    if (currentUnit == Units.Imperial)
                    {
                        // Validate Imperial
                        if (!decimal.TryParse(input, out decimal pressure) || pressure < 0 || pressure > 100)
                        {
                            ShowValidationError("Pressure Target must be a number between 0 and 100 PSI.");
                            return false;
                        }
                    }
                    else
                    {
                        // Validate Metric
                        if (!decimal.TryParse(input, out decimal pressureMetric) || pressureMetric < 0 || pressureMetric > (100 * 6.895m))
                        {
                            ShowValidationError($"Pressure Target must be a number between 0 and {(100 * 6.895m):F2} kPa.");
                            return false;
                        }
                    }
                    break;

                case "txtFlowCalibration":
                    if (!decimal.TryParse(input, out decimal flow) || flow <= 0)
                    {
                        ShowValidationError("Flow Calibration must be a positive number greater than 0.");
                        return false;
                    }
                    break;

                case "txtPSICalibration":
                    if (!decimal.TryParse(input, out decimal psi) || psi < 0 || psi > 10)
                    {
                        ShowValidationError("PSI Calibration must be a number between 0 and 10.");
                        return false;
                    }
                    break;

                case "txtDutyCycleAdjustment":
                    if (!decimal.TryParse(input, out decimal dca) || dca < 0 || dca > 50)
                    {
                        ShowValidationError("Duty Cycle Adjustment must be a number between 0 and 50.");
                        return false;
                    }
                    break;

                case "txtnumberNozzles":
                    if (!decimal.TryParse(input, out decimal nozzles) || nozzles < 0 || nozzles > 100)
                    {
                        ShowValidationError("Number of Nozzles must be a number between 0 and 100.");
                        return false;
                    }
                    break;

                case "txtcurrentDutyCycle":
                    if (!decimal.TryParse(input, out decimal dutyCycle) || dutyCycle < 0 || dutyCycle > 100)
                    {
                        ShowValidationError("Current Duty Cycle must be a number between 0 and 100.");
                        return false;
                    }
                    break;

                case "txtHz":
                    if (!decimal.TryParse(input, out decimal hz) || hz < 1 || hz > 30)
                    {
                        ShowValidationError("Hz must be a number between 1 and 30.");
                        return false;
                    }
                    break;

                case "txtLowBallValve":
                    if (!decimal.TryParse(input, out decimal lowValve) || lowValve < 0 || lowValve > 40)
                    {
                        ShowValidationError("Low Ball Valve must be a number between 0 and 40.");
                        return false;
                    }
                    break;

                case "txtBall_Hyd":
                    if (!decimal.TryParse(input, out decimal ballHyd) || ballHyd < 0 || ballHyd > 1)
                    {
                        ShowValidationError("Ball Hydraulics must be 0 for Hyd and 1 for ball valve.");
                        return false;
                    }
                    break;

                case "txtWheelAngle":
                    if (!decimal.TryParse(input, out decimal wheelAngle) || wheelAngle < 0 || wheelAngle > 10)
                    {
                        ShowValidationError("Wheel Angle must be a number between 0 and 10.");
                        return false;
                    }
                    break;

                case "txtKp":
                    if (!decimal.TryParse(input, out decimal kp) || kp < 0 || kp > 50)
                    {
                        ShowValidationError("Kp must be a number between 0 and 50.");
                        return false;
                    }
                    break;

                case "txtKi":
                    if (!decimal.TryParse(input, out decimal ki) || ki < 0 || ki > 50)
                    {
                        ShowValidationError("Ki must be a number between 0 and 50.");
                        return false;
                    }
                    break;

                case "txtKd":
                    if (!decimal.TryParse(input, out decimal kd) || kd < 0 || kd > 50)
                    {
                        ShowValidationError("Kd must be a number between 0 and 50.");
                        return false;
                    }
                    break;

                case "txtSprayWidth":
                    if (currentUnit == Units.Imperial)
                    {
                        // Validate Imperial
                        if (!decimal.TryParse(input, out decimal sprayWidth) || sprayWidth < 0 || sprayWidth > 45)
                        {
                            ShowValidationError("Spray Width must be a number between 0 and 45 feet.");
                            return false;
                        }
                    }
                    else
                    {
                        // Validate Metric
                        if (!decimal.TryParse(input, out decimal sprayWidthMetric) || sprayWidthMetric < 0 || sprayWidthMetric > (45 * 0.3048m))
                        {
                            ShowValidationError($"Spray Width must be a number between 0 and {(45 * 0.3048m):F2} meters.");
                            return false;
                        }
                    }
                    break;

                case "txtKH_kickDurationMs":
                    if (!decimal.TryParse(input, out decimal kickMs) || kickMs <= 0 || kickMs > 20)
                    {
                        ShowValidationError("Kick Duration must be a number between 0 and 20 ms (2–4 ms typical).");
                        return false;
                    }
                    break;

                case "txtKH_holdDutyCycle":
                    // 0.01–0.30 matches KH_holdDutyMin / KH_holdDutyMax on the Teensy
                    if (!decimal.TryParse(input, out decimal holdDuty) || holdDuty < 0.01m || holdDuty > 0.30m)
                    {
                        ShowValidationError("Hold Duty Cycle must be between 0.01 and 0.30 (1–30% duty, 0.02–0.10 typical).");
                        return false;

                        // Format consistently: always show 0.XX
                        activeTextBox.Text = holdDuty.ToString("0.00");
                    }
                    break;

                case "txtKH_holdPWMFrequency":
                    // Allow a safe range; 200–1000 Hz is your typical working band
                    if (!decimal.TryParse(input, out decimal holdFreq) || holdFreq < 5 || holdFreq > 200)
                    {
                        ShowValidationError("Hold PWM Frequency must be between 5 and 200 Hz.");
                        return false;
                    }
                    break;

                case "txtKH_holdRefV":
                    // Typical 12.6 V; allow a reasonable automotive range
                    if (!decimal.TryParse(input, out decimal holdRefV) || holdRefV < 6 || holdRefV > 16)
                    {
                        ShowValidationError("Hold Reference Voltage must be between 6.0 and 16.0 V (12.6 V typical).");
                        return false;
                    }
                    break;

                default:
                    // No validation rule defined for this field
                    return true;
            }

            return true; // Input is valid
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            activeTextBox.BackColor = System.Drawing.Color.LightCoral; // Highlight field
            activeTextBox.Focus(); // Keep focus on the invalid field
        }

        private void PopulateDebugLevelComboBox()
        {
            // Populate ComboBox with debug level options (0 to 12)
            for (int i = 0; i <= 12; i++)
            {
                cmbDebugPwmLevel.Items.Add($"{i} - {GetDebugLevelDescription(i)}");
            }
            cmbDebugPwmLevel.SelectedIndex = 0; // Default selection
        }

        private string GetDebugLevelDescription(int level)
        {
            return level switch
            {
                0 => "debug - Turns OFF all reporting",
                1 => "dutycycleTurncomp - Individual nozzle reporting",
                2 => "setPWMTiming - Sets timing of nozzle on cycle",
                3 => "ControlNozzle - Controls nozzle (not used in a turn)",
                4 => "Pressure - System pressure",
                5 => "EvenOdd - Toggle firing of even/odd nozzles",
                6 => "Flow - Flow rate control",
                7 => "NozzleSpeed - Individual nozzle speed in a turn",
                8 => "PrintDebug - Overall system reporting",
                9 => "PrintAOG - Reports variables passed from AOG",
                10 => "Calibrate_PSI_Flow - Calibration function",
                11 => "ActualSteerAngle - extractActualSteerAngle",
                12 => "Communication - Debug UDP",
                _ => "Unknown"
            };
        }

        private void StartListening()
        {
            udpClient.BeginReceive(OnDataReceived, null);
        }

        private void OnDataReceived(IAsyncResult ar)
        {
            try
            {
                var data = udpClient.EndReceive(ar, ref arduinoEndPoint);
                string receivedData = Encoding.UTF8.GetString(data);

                Debug.WriteLine("Received Data: " + receivedData); // Log the raw received data

                if (receivedData.StartsWith("USER_SETTINGS:"))
                {
                    Debug.WriteLine("Processing USER_SETTINGS data.");
                    UpdateUserSettings(receivedData.Replace("USER_SETTINGS:", ""));
                }
                else if (receivedData.StartsWith("SENSOR_DATA:"))
                {
                    Debug.WriteLine("Processing SENSOR_DATA.");
                    UpdateUI(receivedData.Replace("SENSOR_DATA:", ""));
                }
                else
                {
                    Debug.WriteLine("Unrecognized data format.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error receiving data: " + ex.Message);
            }
            finally
            {
                StartListening(); // Restart listening
            }
        }

        private void UpdateUserSettings(string data)
        {
            string[] variables = data.Split(',');
            foreach (var variable in variables)
            {
                var parts = variable.Split(':');
                if (parts.Length != 2) continue;

                string name = parts[0].Trim();
                string value = parts[1].Trim();

                Debug.WriteLine($"Updating setting: {name} = {value}"); // Log each setting update

                Invoke((MethodInvoker)delegate
                {
                    switch (name)
                    {
                        case "GPATarget": txtGPATarget.Text = value; break;
                        case "SprayWidth": txtSprayWidth.Text = value; break;
                        case "FlowCalibration": txtFlowCalibration.Text = value; break;
                        case "PSICalibration": txtPSICalibration.Text = value; break;
                        case "DutyCycleAdjustment": txtDutyCycleAdjustment.Text = value; break;
                        case "PressureTarget": txtPressureTarget.Text = value; break;
                        case "numberNozzles": txtnumberNozzles.Text = value; break;
                        case "currentDutyCycle": txtcurrentDutyCycle.Text = value; break;
                        case "Hz": txtHz.Text = value; break;
                        case "LowBallValve": txtLowBallValve.Text = value; break;
                        case "Ball_Hyd": txtBall_Hyd.Text = value; break;
                        case "WheelAngle": txtWheelAngle.Text = value; break;
                        case "Kp": txtKp.Text = value; break;
                        case "Ki": txtKi.Text = value; break;
                        case "Kd": txtKd.Text = value; break;

                        // ---- Kick & Hold coming back from Teensy ----
                        case "KH_kickDurationMs":
                            txtKH_kickDurationMs.Text = value;
                            break;

                        case "KH_holdDutyCycle":
                            if (decimal.TryParse(value, out decimal holdDuty))
                                txtKH_holdDutyCycle.Text = holdDuty.ToString("0.00");
                            else
                                txtKH_holdDutyCycle.Text = value;  // fallback
                            break;


                        case "KH_holdPWMFrequency":
                            txtKH_holdPWMFrequency.Text = value;
                            break;

                        case "KH_holdRefV":
                            txtKH_holdRefV.Text = value;
                            break;

                        case "Unit":
                            {
                                Units newUnitEnum;
                                if (value.Equals("Imperial", StringComparison.OrdinalIgnoreCase) || value == "I")
                                    newUnitEnum = Units.Imperial;
                                else if (value.Equals("Metric", StringComparison.OrdinalIgnoreCase) || value == "M")
                                    newUnitEnum = Units.Metric;
                                else
                                    newUnitEnum = Units.Imperial; // Fallback

                                string comboText = newUnitEnum == Units.Imperial ? "I - Imperial" : "M - Metric";

                                Debug.WriteLine($"[DEBUG] Received Unit: {value}, Combo Text: {comboText}, Enum: {newUnitEnum}");

                                // Prevent triggering SelectedIndexChanged unnecessarily
                                CmbUnits.SelectedIndexChanged -= CmbUnits_SelectedIndexChanged;
                                if (CmbUnits.Items.Contains(comboText))
                                {
                                    CmbUnits.SelectedItem = comboText;
                                }
                                else
                                {
                                    // Fallback if items not yet set as expected
                                    CmbUnits.SelectedIndex = newUnitEnum == Units.Imperial ? 0 : 1;
                                }
                                CmbUnits.SelectedIndexChanged += CmbUnits_SelectedIndexChanged;

                                currentUnit = newUnitEnum;
                                break;
                            }

                        case "PWM_Conventional":
                            chkPWM_Conventional.Checked = value == "1";
                            break;

                        case "Stagger":
                            chkStagger.Checked = value == "1";
                            break;

                        case "debug":
                            cmbDebugPwmLevel.SelectedIndex = int.TryParse(value, out int debugLevel)
                                ? debugLevel
                                : 0;
                            break;

                        default:
                            Debug.WriteLine($"Unrecognized field: {name}");
                            break;
                    }
                });
            }
        }

        private void UpdateUI(string data)
        {
            string[] variables = data.Split(',');
            foreach (var variable in variables)
            {
                var parts = variable.Split(':');
                if (parts.Length != 2) continue;

                string name = parts[0].Trim();
                string value = parts[1].Trim();

                Invoke((MethodInvoker)delegate
                {
                    switch (name)
                    {
                        case "pressure":
                            if (decimal.TryParse(value, out decimal pressure))
                            {
                                if (currentUnit == Units.Metric)
                                {
                                    // PSI -> kPa
                                    value = (pressure * 6.895m).ToString("F2");
                                }
                                txtpressure.Text = value;
                            }
                            break;

                        case "gpsSpeed":
                            if (decimal.TryParse(value, out decimal speed))
                            {
                                if (currentUnit == Units.Metric)
                                {
                                    // MPH -> KPH
                                    value = (speed * 1.609m).ToString("F2");
                                }
                                txtgpsSpeed.Text = value;
                            }
                            break;

                        case "gpaDisplay":   // was actualGPAave on Teensy before
                        case "actualGPAave": // keep this for backward compatibility, just in case
                            if (decimal.TryParse(value, out decimal gpa))
                            {
                                if (currentUnit == Units.Metric)
                                {
                                    // GPA -> L/ha
                                    value = (gpa * 9.35m).ToString("F2");
                                }
                                txtactualGPAave.Text = value;
                            }
                            break;

                        case "onTime":
                            // onTime is unitless (%), no conversion needed
                            txtonTime.Text = value;
                            break;

                        case "acresTotal":
                            if (decimal.TryParse(value, out decimal acresTotal))
                            {
                                if (currentUnit == Units.Metric)
                                {
                                    // acres -> hectares
                                    value = (acresTotal * 0.4047m).ToString("F2");
                                }
                                txtAcresTotal.Text = value;
                            }
                            break;

                        case "acresPerHour":
                            if (decimal.TryParse(value, out decimal acresPerHour))
                            {
                                if (currentUnit == Units.Metric)
                                {
                                    // acres/hour -> hectares/hour
                                    value = (acresPerHour * 0.4047m).ToString("F2");
                                }
                                txtAcresPerHour.Text = value;
                            }
                            break;

                        default:
                            Debug.WriteLine($"Unrecognized sensor data: {name} = {value}");
                            break;
                    }
                });
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string message = GenerateSettingsMessage();
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(bytes, bytes.Length, arduinoEndPoint);
        }

        private string GenerateSettingsMessage()
        {
            Debug.WriteLine($"[DEBUG] Current Unit before Sending Settings: {currentUnit}");

            string gpa = string.IsNullOrWhiteSpace(txtGPATarget.Text) ? "0" : txtGPATarget.Text;
            string sprayWidth = string.IsNullOrWhiteSpace(txtSprayWidth.Text) ? "0" : txtSprayWidth.Text;
            string flowCalibration = string.IsNullOrWhiteSpace(txtFlowCalibration.Text) ? "0" : txtFlowCalibration.Text;
            string psiCalibration = string.IsNullOrWhiteSpace(txtPSICalibration.Text) ? "0" : txtPSICalibration.Text;
            string dutyCycleAdjustment = string.IsNullOrWhiteSpace(txtDutyCycleAdjustment.Text) ? "0" : txtDutyCycleAdjustment.Text;
            string pressureTarget = string.IsNullOrWhiteSpace(txtPressureTarget.Text) ? "0" : txtPressureTarget.Text;
            string numberNozzles = string.IsNullOrWhiteSpace(txtnumberNozzles.Text) ? "0" : txtnumberNozzles.Text;
            string currentDutyCycle = string.IsNullOrWhiteSpace(txtcurrentDutyCycle.Text) ? "0" : txtcurrentDutyCycle.Text;
            string hz = string.IsNullOrWhiteSpace(txtHz.Text) ? "0" : txtHz.Text;
            string lowBallValve = string.IsNullOrWhiteSpace(txtLowBallValve.Text) ? "0" : txtLowBallValve.Text;
            string ballHyd = string.IsNullOrWhiteSpace(txtBall_Hyd.Text) ? "0" : txtBall_Hyd.Text;
            string wheelAngle = string.IsNullOrWhiteSpace(txtWheelAngle.Text) ? "0" : txtWheelAngle.Text;
            string kp = string.IsNullOrWhiteSpace(txtKp.Text) ? "0" : txtKp.Text;
            string ki = string.IsNullOrWhiteSpace(txtKi.Text) ? "0" : txtKi.Text;
            string kd = string.IsNullOrWhiteSpace(txtKd.Text) ? "0" : txtKd.Text;

            // Use enum as single source of truth for unit sent to Teensy
            string unit = currentUnit == Units.Imperial ? "Imperial" : "Metric";

            string pwmConventional = chkPWM_Conventional.Checked ? "1" : "0";
            string stagger = chkStagger.Checked ? "1" : "0";
            string debugPwmLevel = cmbDebugPwmLevel.SelectedIndex.ToString();

            // ---- Kick & Hold fields from UI ----
            string khKickDurationMs = string.IsNullOrWhiteSpace(txtKH_kickDurationMs.Text) ? "0" : txtKH_kickDurationMs.Text;
            string khHoldDutyCycle =
                decimal.TryParse(txtKH_holdDutyCycle.Text, out decimal holdDutyValue)
                    ? holdDutyValue.ToString("0.00")
                    : "0.00";

            string khHoldPWMFrequency = string.IsNullOrWhiteSpace(txtKH_holdPWMFrequency.Text) ? "0" : txtKH_holdPWMFrequency.Text;
            string khHoldRefV = string.IsNullOrWhiteSpace(txtKH_holdRefV.Text) ? "0" : txtKH_holdRefV.Text;

            string settingsMessage = $"UPDATE_SETTINGS:GPATarget:{gpa}," +
                                     $"SprayWidth:{sprayWidth},FlowCalibration:{flowCalibration}," +
                                     $"PSICalibration:{psiCalibration},DutyCycleAdjustment:{dutyCycleAdjustment}," +
                                     $"PressureTarget:{pressureTarget},numberNozzles:{numberNozzles}," +
                                     $"currentDutyCycle:{currentDutyCycle},Hz:{hz},LowBallValve:{lowBallValve}," +
                                     $"Ball_Hyd:{ballHyd},WheelAngle:{wheelAngle},Kp:{kp}," +
                                     $"Ki:{ki},Kd:{kd},Unit:{unit},PWM_Conventional:{pwmConventional}," +
                                     $"Stagger:{stagger},debug:{debugPwmLevel}," +
                                     $"KH_kickDurationMs:{khKickDurationMs},KH_holdDutyCycle:{khHoldDutyCycle}," +
                                     $"KH_holdPWMFrequency:{khHoldPWMFrequency},KH_holdRefV:{khHoldRefV}";

            Debug.WriteLine($"Generated Settings Message: {settingsMessage}");
            return settingsMessage;
        }


        private void SendDefaultSettings()
        {
            try
            {
                // Use the GenerateSettingsMessage method to construct default settings
                string defaultSettings = GenerateSettingsMessage();
                byte[] bytes = Encoding.UTF8.GetBytes(defaultSettings);

                udpClient.Send(bytes, bytes.Length, arduinoEndPoint);
                Debug.WriteLine("Default settings have been sent.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error sending default settings: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000); // Wait 1 second before sending requests

            // Request user settings from the Teensy
            Debug.WriteLine("Sending REQUEST_USER_SETTINGS...");
            RequestDataFromTeensy("REQUEST_USER_SETTINGS");

            System.Threading.Thread.Sleep(500); // Wait 1/2 second before sending requests

            // Request sensor data from the Teensy
            Debug.WriteLine("Sending REQUEST_SENSOR_DATA...");
            RequestDataFromTeensy("REQUEST_SENSOR_DATA");

            this.Size = new System.Drawing.Size(300, 620); // Width = 290, Height = 600
            BtnToggleView.Image = SprayerControl_2._4.Properties.Resources.Next; // Set initial button image to "Next"

            // Initialize the sprayer control button
            BtnSprayerControl.Text = "Start"; // Initial text
            BtnSprayerControl.BackColor = System.Drawing.Color.Red; // Initial color
            isSprayerOn = false; // Ensure the sprayer is off at startup

            // Initialize chkStagger text based on its Checked state
            chkStagger.Text = chkStagger.Checked ? "Staggered" : "Not Staggered";

            // Similarly, initialize chkPWM_Conventional text
            chkPWM_Conventional.Text = chkPWM_Conventional.Checked ? "PWM Mode" : "Conventional Mode";

            // Clear and populate ComboBox with "Imperial" and "Metric"
            Debug.WriteLine("[DEBUG] Initializing Form...");
            CmbUnits.SelectedIndexChanged -= CmbUnits_SelectedIndexChanged;
            CmbUnits.Items.Clear();
            CmbUnits.Items.Add("I - Imperial");
            CmbUnits.Items.Add("M - Metric");

            if (CmbUnits.Items.Count > 0)
            {
                // Sync dropdown with currentUnit enum
                CmbUnits.SelectedIndex = currentUnit == Units.Imperial ? 0 : 1;
                Debug.WriteLine($"[DEBUG] Default Unit Set to: {CmbUnits.SelectedItem}");
            }

            CmbUnits.SelectedIndexChanged += CmbUnits_SelectedIndexChanged;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0); // Top-left corner
        }

        private void RequestDataFromTeensy(string requestType)
        {
            try
            {
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestType);
                udpClient.Send(requestBytes, requestBytes.Length, arduinoEndPoint);
                Debug.WriteLine($"Sent request: {requestType}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error sending request to Teensy: " + ex.Message);
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            // Create an instance of HelpForm
            HelpForm helpForm = new HelpForm();
            // Show the HelpForm as a modal dialog
            helpForm.ShowDialog();
        }

        private void txtPressureTarget_TextChanged(object sender, EventArgs e)
        {
            // Placeholder to satisfy designer wiring.
            // Add logic here if you ever need to react when PressureTarget changes.
        }


        private void SendSettings_Click(object sender, EventArgs e)
        {
            // Validate all fields before submission
            if (ValidateAllFields())
            {
                try
                {
                    string message = GenerateSettingsMessage();
                    Debug.WriteLine("Generated Settings Message: " + message); // Log the generated message
                    byte[] bytes = Encoding.UTF8.GetBytes(message);

                    // I know I shouldn't have to do this, but I can't find where the port is getting changed to 8888
                    arduinoEndPoint = new IPEndPoint(IPAddress.Parse("192.168.5.123"), 7777);
                    Debug.WriteLine($"Enforcing arduinoEndPoint: {arduinoEndPoint.Address}:{arduinoEndPoint.Port}");
                    udpClient.Send(bytes, bytes.Length, arduinoEndPoint);
                    settingsSent = true; // Mark settings as sent

                    Debug.WriteLine("Settings have been sent successfully.");
                    MessageBox.Show("Settings have been sent successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error sending settings: " + ex.Message);
                    MessageBox.Show($"Error sending settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Validation failed; do not proceed
                MessageBox.Show("Please correct the highlighted fields and try again.", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnToggleView_Click(object sender, EventArgs e)
        {
            if (isExpanded)
            {
                // Switch to compact view
                this.Size = new System.Drawing.Size(300, 620); // Compact size W, H
                BtnToggleView.Image = SprayerControl_2._4.Properties.Resources.Next; // Change button image to "Next"
            }
            else
            {
                // Switch to expanded view
                this.Size = new System.Drawing.Size(900, 620); // Expanded size W, H
                BtnToggleView.Image = SprayerControl_2._4.Properties.Resources.Previous; // Change button image to "Previous"
            }

            // Toggle the state
            isExpanded = !isExpanded;
        }

        private void BtnSprayerControl_Click(object sender, EventArgs e)
        {
            // Ensure settings are sent before toggling the sprayer
            if (!settingsSent) // Use a flag to track if settings were sent
            {
                Debug.WriteLine("Settings not sent. Sending default settings...");
                SendDefaultSettings();
                settingsSent = true;
            }

            // Toggle the sprayer state
            isSprayerOn = !isSprayerOn;

            // Update the button text to reflect the current state
            BtnSprayerControl.Text = isSprayerOn ? "Stop" : "Start";
            BtnSprayerControl.BackColor = isSprayerOn ? Color.Green : Color.Red;

            // Construct the command to send
            string command = isSprayerOn ? "START_SPRAYER" : "STOP_SPRAYER";

            try
            {
                arduinoEndPoint = new IPEndPoint(IPAddress.Parse("192.168.5.123"), 7777);
                Debug.WriteLine($"Enforcing arduinoEndPoint: {arduinoEndPoint.Address}:{arduinoEndPoint.Port}");

                // Send the command via UDP
                byte[] commandBytes = Encoding.UTF8.GetBytes(command);
                udpClient.Send(commandBytes, commandBytes.Length, arduinoEndPoint);

                Debug.WriteLine($"Sprayer command sent: {command}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error sending sprayer command: " + ex.Message);
            }
        }

        private void ChkPWM_Conventional_CheckedChanged(object sender, EventArgs e)
        {
            // Change the text based on the Checked state
            chkPWM_Conventional.Text = chkPWM_Conventional.Checked ? "PWM Mode" : "Conventional";
        }

        private void ChkStagger_CheckedChanged(object sender, EventArgs e)
        {
            // Change the text based on the Checked state
            chkStagger.Text = chkStagger.Checked ? "Staggered" : "Not Staggered";
        }

        private void ConvertToMetric()
        {
            if (decimal.TryParse(txtGPATarget.Text, out decimal gpa))
            {
                txtGPATarget.Text = (gpa * 9.35m).ToString("F2"); // Example: GPA to Liters/Ha
            }

            if (decimal.TryParse(txtPressureTarget.Text, out decimal pressure))
            {
                txtPressureTarget.Text = (pressure * 6.895m).ToString("F2"); // PSI to kPa
            }

            // Add similar conversions for other fields if needed
        }

        private void ConvertToImperial()
        {
            if (decimal.TryParse(txtGPATarget.Text, out decimal gpa))
            {
                txtGPATarget.Text = (gpa / 9.35m).ToString("F2"); // Liters/Ha to GPA
            }

            if (decimal.TryParse(txtPressureTarget.Text, out decimal pressure))
            {
                txtPressureTarget.Text = (pressure / 6.895m).ToString("F2"); // kPa to PSI
            }

            // Add similar conversions for other fields if needed
        }

        private void CmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbUnits.SelectedItem == null) return;

            string selectedUnit = CmbUnits.SelectedItem.ToString();
            Units newUnit = selectedUnit.StartsWith("M") ? Units.Metric : Units.Imperial;

            Debug.WriteLine($"[DEBUG] Dropdown Selection Changed: {selectedUnit}, Interpreted as: {newUnit}");

            // Only proceed if the unit has truly changed
            if (newUnit == currentUnit)
            {
                Debug.WriteLine($"[DEBUG] No Unit Change Detected: Current Unit = {currentUnit}");
                return;
            }

            // Convert existing values based on direction of change
            if (currentUnit == Units.Imperial && newUnit == Units.Metric)
            {
                ConvertToMetric();
            }
            else if (currentUnit == Units.Metric && newUnit == Units.Imperial)
            {
                ConvertToImperial();
            }

            currentUnit = newUnit;
            Debug.WriteLine($"[DEBUG] Unit Change Detected: New Unit = {currentUnit}");
        }

        private void txtKp_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtpressure.Clear();
            txtactualGPAave.Clear();
            txtonTime.Clear();
            txtAcresTotal.Clear();
            txtAcresPerHour.Clear();
        }
    }
}



