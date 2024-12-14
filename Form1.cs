using SprayerControl;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ArduinoControlApp
{

    public partial class Form1 : Form
    {
        private Keypad.NumKeypad numKeypad; // Declare an instance of NumKeypad
        private TextBox activeTextBox;      // Tracks which text box is currently active for data entry
        private readonly UdpClient udpClient;
        private IPEndPoint arduinoEndPoint;

        public Form1()
        {
            InitializeComponent();

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

            // Subscribe to the ButtonPressed event
            numKeypad.ButtonPressed += NumKeypad_ButtonPressed;

            // All communicatiion through port 7777
            udpClient = new UdpClient(7777);
            arduinoEndPoint = new IPEndPoint(IPAddress.Parse("192.168.5.123"), 7777);

            StartListening();
            PopulateDebugLevelComboBox();
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

                    if (textBox == txtGPATarget)
                    {
                        if (string.IsNullOrWhiteSpace(textBox.Text) || !decimal.TryParse(textBox.Text, out decimal gpa) || gpa < 0 || gpa > 30)
                        {
                            isValid = false;
                            textBox.BackColor = System.Drawing.Color.LightCoral;
                            errorMessages.AppendLine($"GPA Target: Enter a number between 0 and 30. (You entered: '{textBox.Text}')");
                        }
                        else
                        {
                            textBox.BackColor = System.Drawing.Color.White;
                        }
                    }
                    else if (textBox == txtPressureTarget)
                    {
                        if (string.IsNullOrWhiteSpace(textBox.Text) || !decimal.TryParse(textBox.Text, out decimal pressure) || pressure < 0 || pressure > 100)
                        {
                            isValid = false;
                            textBox.BackColor = System.Drawing.Color.LightCoral;
                            errorMessages.AppendLine($"Pressure Target: Enter a number between 0 and 100. (You entered: '{textBox.Text}')");
                        }
                        else
                        {
                            textBox.BackColor = System.Drawing.Color.White;
                        }
                    }
                    // Continue adding validations for other fields similarly...
                    else if (textBox == txtBall_Hyd)
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
                    if (!decimal.TryParse(input, out decimal gpa) || gpa < 0 || gpa > 30)
                    {
                        ShowValidationError("GPA Target must be a number between 0 and 30.");
                        return false;
                    }
                    break;

                case "txtPressureTarget":
                    if (!decimal.TryParse(input, out decimal pressure) || pressure < 0 || pressure > 100)
                    {
                        ShowValidationError("Pressure Target must be a number between 0 and 100.");
                        return false;
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
                    if (!decimal.TryParse(input, out decimal dca) || dca < 0 || dca > 5)
                    {
                        ShowValidationError("Duty Cycle Adjustment must be a number between 0 and 5.");
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
                    if (!decimal.TryParse(input, out decimal kp) || kp < 0 || kp > 5)
                    {
                        ShowValidationError("Kp must be a number between 0 and 5.");
                        return false;
                    }
                    break;

                case "txtKi":
                    if (!decimal.TryParse(input, out decimal ki) || ki < 0 || ki > 5)
                    {
                        ShowValidationError("Ki must be a number between 0 and 5.");
                        return false;
                    }
                    break;

                case "txtKd":
                    if (!decimal.TryParse(input, out decimal kd) || kd < 0 || kd > 5)
                    {
                        ShowValidationError("Kd must be a number between 0 and 5.");
                        return false;
                    }
                    break;

                case "txtSprayWidth":
                    if (!decimal.TryParse(input, out decimal SW) || SW < 0 || SW > 45)
                    {
                        ShowValidationError("Spray Width must be a number between 0 and 45.");
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
                11 => "ActualSteerAngle   - extractActualSteerAngle",
                12 => "PrintUserVariables - Debug UDP",
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

                Debug.WriteLine("Received Data: " + receivedData);

                // Determine if the data contains user settings or sensor data
                if (receivedData.StartsWith("USER_SETTINGS:"))
                {
                    UpdateUserSettings(receivedData.Replace("USER_SETTINGS:", ""));
                }
                else if (receivedData.StartsWith("SENSOR_DATA:"))
                {
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
                        default: Debug.WriteLine($"Unrecognized field: {name}"); break;
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

                Invoke((MethodInvoker)delegate {
                    switch (name)
                    {
                        case "pressure":
                            txtpressure.Text = value;
                            break;
                        case "onTime":
                            txtonTime.Text = value;
                            break;
                        case "actualGPAave":
                            txtactualGPAave.Text = value;
                            break;
                        case "gpsSpeed":
                            txtgpsSpeed.Text = value;
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
            // Construct the settings message
            string settingsMessage = $"UPDATE_SETTINGS:GPATarget:{txtGPATarget.Text}," +
                                     $"SprayWidth:{txtSprayWidth.Text},FlowCalibration:{txtFlowCalibration.Text}," +
                                     $"PSICalibration:{txtPSICalibration.Text},DutyCycleAdjustment:{txtDutyCycleAdjustment.Text}," +
                                     $"PressureTarget:{txtPressureTarget.Text},numberNozzles:{txtnumberNozzles.Text}," +
                                     $"currentDutyCycle:{txtcurrentDutyCycle.Text},Hz:{txtHz.Text},LowBallValve:{txtLowBallValve.Text}," +
                                     $"Ball_Hyd:{txtBall_Hyd.Text},WheelAngle:{txtWheelAngle.Text},Kp:{txtKp.Text}," +
                                     $"Ki:{txtKi.Text},Kd:{txtKd.Text}";

            // Construct the switches message
            string switchesMessage = $"SET_SWITCHES:pwm:{(chkPWM_Conventional.Checked ? 1 : 0)}," +
                                     $"stagger:{(chkStagger.Checked ? 1 : 0)}";

            // Construct the debug message
            int debugLevel = cmbDebugPwmLevel.SelectedIndex; // Debug level based on ComboBox selection
            string debugMessage = $"SET_DEBUG:debug:{debugLevel}";

            // Combine all messages into a single string, separated by newlines
            return $"{settingsMessage}\n{switchesMessage}\n{debugMessage}";
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000); // Wait 1 second before sending requests

            // Request user settings from the Teensy
            RequestDataFromTeensy("REQUEST_USER_SETTINGS");
            // Request sensor data from the Teensy
            RequestDataFromTeensy("REQUEST_SENSOR_DATA");

            this.Size = new System.Drawing.Size(400, 450); // Width = 400, Height = 450
            BtnToggleView.Image = SprayerControl_2._4.Properties.Resources.Next; // Set initial button image to "Next"

            // Initialize the sprayer control button
            BtnSprayerControl.Text = "Start"; // Initial text
            BtnSprayerControl.BackColor = System.Drawing.Color.Red; // Initial color
            isSprayerOn = false; // Ensure the sprayer is off at startup

            // Initialize chkStagger text based on its Checked state
            chkStagger.Text = chkStagger.Checked ? "Staggered" : "Not Staggered";

            // Similarly, you can add the logic for chkPWM_Conventional
            chkPWM_Conventional.Text = chkPWM_Conventional.Checked ? "PWM Mode" : "Conventional Mode";

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



        private void SendSettings_Click(object sender, EventArgs e)
        {
            // Validate all fields before submission
            if (ValidateAllFields())
            {
                try
                {
                    string message = GenerateSettingsMessage();
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    udpClient.Send(bytes, bytes.Length, arduinoEndPoint);

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

        private bool isExpanded = false; // Track the current state of the form

        private void BtnToggleView_Click(object sender, EventArgs e)
        {
            if (isExpanded)
            {
                // Switch to compact view
                this.Size = new System.Drawing.Size(400, 450); // Compact size W, H
                BtnToggleView.Image = SprayerControl_2._4.Properties.Resources.Next; // Change button image to "Next"
            }
            else
            {
                // Switch to expanded view
                this.Size = new System.Drawing.Size(900, 450); // Expanded size W, H
                BtnToggleView.Image = SprayerControl_2._4.Properties.Resources.Previous; // Change button image to "Previous"
            }

            // Toggle the state
            isExpanded = !isExpanded;
        }

        private bool isSprayerOn = false; // Track the current state of the sprayer

        private void BtnSprayerControl_Click(object sender, EventArgs e)
        {
            // Toggle the sprayer state
            isSprayerOn = !isSprayerOn;

            // Update the button text to reflect the current state
            BtnSprayerControl.Text = isSprayerOn ? "Stop" : "Start";
            BtnSprayerControl.BackColor = isSprayerOn ? Color.Green : Color.Red;

            // Construct the command to send
            string command = isSprayerOn ? "START_SPRAYER" : "STOP_SPRAYER";

            try
            {
                // Send the command via UDP
                byte[] commandBytes = Encoding.UTF8.GetBytes(command);
                udpClient.Send(commandBytes, commandBytes.Length, arduinoEndPoint);

                // Provide feedback to the user
                //MessageBox.Show($"Sprayer has been {(isSprayerOn ? "started" : "stopped")}.", "Sprayer Control", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Handle any errors during communication
                MessageBox.Show($"Error sending sprayer command: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkPWM_Conventional_CheckedChanged(object sender, EventArgs e)
        {
            // Change the text based on the Checked state
            chkPWM_Conventional.Text = chkPWM_Conventional.Checked ? "PWM Mode" : "Conventional Mode";
        }

        private void ChkStagger_CheckedChanged(object sender, EventArgs e)
        {
            // Change the text based on the Checked state
            chkStagger.Text = chkStagger.Checked ? "Staggered" : "Not Staggered";
        }

    }
}


