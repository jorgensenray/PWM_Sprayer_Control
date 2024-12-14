namespace ArduinoControlApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtpressure = new System.Windows.Forms.TextBox();
            this.txtonTime = new System.Windows.Forms.TextBox();
            this.txtactualGPAave = new System.Windows.Forms.TextBox();
            this.txtgpsSpeed = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGPATarget = new System.Windows.Forms.TextBox();
            this.txtSprayWidth = new System.Windows.Forms.TextBox();
            this.txtFlowCalibration = new System.Windows.Forms.TextBox();
            this.txtPSICalibration = new System.Windows.Forms.TextBox();
            this.txtDutyCycleAdjustment = new System.Windows.Forms.TextBox();
            this.txtPressureTarget = new System.Windows.Forms.TextBox();
            this.txtnumberNozzles = new System.Windows.Forms.TextBox();
            this.txtcurrentDutyCycle = new System.Windows.Forms.TextBox();
            this.txtHz = new System.Windows.Forms.TextBox();
            this.txtLowBallValve = new System.Windows.Forms.TextBox();
            this.txtBall_Hyd = new System.Windows.Forms.TextBox();
            this.txtWheelAngle = new System.Windows.Forms.TextBox();
            this.txtKp = new System.Windows.Forms.TextBox();
            this.txtKi = new System.Windows.Forms.TextBox();
            this.txtKd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.cmbDebugPwmLevel = new System.Windows.Forms.ComboBox();
            this.chkPWM_Conventional = new System.Windows.Forms.CheckBox();
            this.chkStagger = new System.Windows.Forms.CheckBox();
            this.De = new System.Windows.Forms.Label();
            this.SendSettings = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.HelpButton = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.BtnSprayerControl = new System.Windows.Forms.Button();
            this.BtnToggleView = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtpressure
            // 
            this.txtpressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpressure.Location = new System.Drawing.Point(414, 76);
            this.txtpressure.Name = "txtpressure";
            this.txtpressure.ReadOnly = true;
            this.txtpressure.Size = new System.Drawing.Size(116, 53);
            this.txtpressure.TabIndex = 0;
            this.txtpressure.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtonTime
            // 
            this.txtonTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtonTime.Location = new System.Drawing.Point(415, 137);
            this.txtonTime.Name = "txtonTime";
            this.txtonTime.ReadOnly = true;
            this.txtonTime.Size = new System.Drawing.Size(115, 53);
            this.txtonTime.TabIndex = 1;
            this.txtonTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtactualGPAave
            // 
            this.txtactualGPAave.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtactualGPAave.Location = new System.Drawing.Point(414, 207);
            this.txtactualGPAave.Name = "txtactualGPAave";
            this.txtactualGPAave.ReadOnly = true;
            this.txtactualGPAave.Size = new System.Drawing.Size(116, 53);
            this.txtactualGPAave.TabIndex = 2;
            this.txtactualGPAave.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtgpsSpeed
            // 
            this.txtgpsSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtgpsSpeed.Location = new System.Drawing.Point(415, 274);
            this.txtgpsSpeed.Name = "txtgpsSpeed";
            this.txtgpsSpeed.ReadOnly = true;
            this.txtgpsSpeed.Size = new System.Drawing.Size(115, 53);
            this.txtgpsSpeed.TabIndex = 3;
            this.txtgpsSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(215, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 46);
            this.label1.TabIndex = 4;
            this.label1.Text = "Pressure";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(222, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 46);
            this.label2.TabIndex = 5;
            this.label2.Text = "On Time";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(265, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 46);
            this.label3.TabIndex = 6;
            this.label3.Text = "GPA";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(233, 274);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 46);
            this.label4.TabIndex = 7;
            this.label4.Text = "Speed";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtGPATarget
            // 
            this.txtGPATarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGPATarget.Location = new System.Drawing.Point(97, 211);
            this.txtGPATarget.Name = "txtGPATarget";
            this.txtGPATarget.Size = new System.Drawing.Size(109, 53);
            this.txtGPATarget.TabIndex = 8;
            this.txtGPATarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSprayWidth
            // 
            this.txtSprayWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSprayWidth.Location = new System.Drawing.Point(859, 62);
            this.txtSprayWidth.Name = "txtSprayWidth";
            this.txtSprayWidth.Size = new System.Drawing.Size(100, 48);
            this.txtSprayWidth.TabIndex = 10;
            this.txtSprayWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtFlowCalibration
            // 
            this.txtFlowCalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFlowCalibration.Location = new System.Drawing.Point(1079, 326);
            this.txtFlowCalibration.Name = "txtFlowCalibration";
            this.txtFlowCalibration.Size = new System.Drawing.Size(100, 53);
            this.txtFlowCalibration.TabIndex = 11;
            this.txtFlowCalibration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPSICalibration
            // 
            this.txtPSICalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPSICalibration.Location = new System.Drawing.Point(1079, 110);
            this.txtPSICalibration.Name = "txtPSICalibration";
            this.txtPSICalibration.Size = new System.Drawing.Size(100, 53);
            this.txtPSICalibration.TabIndex = 12;
            this.txtPSICalibration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtDutyCycleAdjustment
            // 
            this.txtDutyCycleAdjustment.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDutyCycleAdjustment.Location = new System.Drawing.Point(859, 277);
            this.txtDutyCycleAdjustment.Name = "txtDutyCycleAdjustment";
            this.txtDutyCycleAdjustment.Size = new System.Drawing.Size(100, 48);
            this.txtDutyCycleAdjustment.TabIndex = 13;
            this.txtDutyCycleAdjustment.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPressureTarget
            // 
            this.txtPressureTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPressureTarget.Location = new System.Drawing.Point(97, 79);
            this.txtPressureTarget.Name = "txtPressureTarget";
            this.txtPressureTarget.Size = new System.Drawing.Size(109, 53);
            this.txtPressureTarget.TabIndex = 14;
            this.txtPressureTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPressureTarget.Click += new System.EventHandler(this.TextBox_Click);
            // 
            // txtnumberNozzles
            // 
            this.txtnumberNozzles.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtnumberNozzles.Location = new System.Drawing.Point(859, 116);
            this.txtnumberNozzles.Name = "txtnumberNozzles";
            this.txtnumberNozzles.Size = new System.Drawing.Size(100, 48);
            this.txtnumberNozzles.TabIndex = 15;
            this.txtnumberNozzles.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtcurrentDutyCycle
            // 
            this.txtcurrentDutyCycle.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtcurrentDutyCycle.Location = new System.Drawing.Point(97, 140);
            this.txtcurrentDutyCycle.Name = "txtcurrentDutyCycle";
            this.txtcurrentDutyCycle.Size = new System.Drawing.Size(109, 53);
            this.txtcurrentDutyCycle.TabIndex = 16;
            this.txtcurrentDutyCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtHz
            // 
            this.txtHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHz.Location = new System.Drawing.Point(859, 333);
            this.txtHz.Name = "txtHz";
            this.txtHz.Size = new System.Drawing.Size(100, 48);
            this.txtHz.TabIndex = 17;
            this.txtHz.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtLowBallValve
            // 
            this.txtLowBallValve.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLowBallValve.Location = new System.Drawing.Point(1079, 217);
            this.txtLowBallValve.Name = "txtLowBallValve";
            this.txtLowBallValve.Size = new System.Drawing.Size(100, 53);
            this.txtLowBallValve.TabIndex = 18;
            this.txtLowBallValve.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBall_Hyd
            // 
            this.txtBall_Hyd.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBall_Hyd.Location = new System.Drawing.Point(859, 170);
            this.txtBall_Hyd.Name = "txtBall_Hyd";
            this.txtBall_Hyd.Size = new System.Drawing.Size(100, 48);
            this.txtBall_Hyd.TabIndex = 19;
            this.txtBall_Hyd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtWheelAngle
            // 
            this.txtWheelAngle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWheelAngle.Location = new System.Drawing.Point(859, 223);
            this.txtWheelAngle.Name = "txtWheelAngle";
            this.txtWheelAngle.Size = new System.Drawing.Size(100, 48);
            this.txtWheelAngle.TabIndex = 20;
            this.txtWheelAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtKp
            // 
            this.txtKp.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKp.Location = new System.Drawing.Point(737, 434);
            this.txtKp.Name = "txtKp";
            this.txtKp.Size = new System.Drawing.Size(98, 53);
            this.txtKp.TabIndex = 21;
            this.txtKp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtKi
            // 
            this.txtKi.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKi.Location = new System.Drawing.Point(737, 500);
            this.txtKi.Name = "txtKi";
            this.txtKi.Size = new System.Drawing.Size(98, 53);
            this.txtKi.TabIndex = 22;
            this.txtKi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtKd
            // 
            this.txtKd.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKd.Location = new System.Drawing.Point(737, 565);
            this.txtKd.Name = "txtKd";
            this.txtKd.Size = new System.Drawing.Size(98, 53);
            this.txtKd.TabIndex = 23;
            this.txtKd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(685, 437);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 46);
            this.label6.TabIndex = 25;
            this.label6.Text = "p";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(685, 500);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 46);
            this.label7.TabIndex = 26;
            this.label7.Text = "i";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(672, 565);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 46);
            this.label8.TabIndex = 27;
            this.label8.Text = "d";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(705, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(209, 40);
            this.label9.TabIndex = 28;
            this.label9.Text = "Enter Once";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(407, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(138, 46);
            this.label10.TabIndex = 29;
            this.label10.Text = "Actual";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(1043, 277);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(254, 46);
            this.label11.TabIndex = 30;
            this.label11.Text = "Flow Sensor";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(630, 65);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(224, 40);
            this.label13.TabIndex = 32;
            this.label13.Text = "Spray Width";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(663, 116);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(184, 40);
            this.label14.TabIndex = 33;
            this.label14.Text = "# Nozzles";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(672, 170);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(181, 40);
            this.label15.TabIndex = 34;
            this.label15.Text = "Ball / Hyd";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(615, 227);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(232, 40);
            this.label16.TabIndex = 35;
            this.label16.Text = "Wheel Angle";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(580, 281);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(267, 40);
            this.label17.TabIndex = 36;
            this.label17.Text = "Duty Cycle Adj";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(791, 334);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(63, 40);
            this.label18.TabIndex = 37;
            this.label18.Text = "Hz";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(1043, 10);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(222, 46);
            this.label20.TabIndex = 39;
            this.label20.Text = "Calibration";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label21.Location = new System.Drawing.Point(987, 56);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(332, 46);
            this.label21.TabIndex = 40;
            this.label21.Text = "Pressure Sensor";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(1025, 170);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(286, 46);
            this.label22.TabIndex = 41;
            this.label22.Text = "Min Ball Valve";
            // 
            // cmbDebugPwmLevel
            // 
            this.cmbDebugPwmLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDebugPwmLevel.FormattingEnabled = true;
            this.cmbDebugPwmLevel.Location = new System.Drawing.Point(29, 353);
            this.cmbDebugPwmLevel.Name = "cmbDebugPwmLevel";
            this.cmbDebugPwmLevel.Size = new System.Drawing.Size(510, 33);
            this.cmbDebugPwmLevel.TabIndex = 42;
            this.cmbDebugPwmLevel.Text = "debugPwmLevel";
            // 
            // chkPWM_Conventional
            // 
            this.chkPWM_Conventional.AutoSize = true;
            this.chkPWM_Conventional.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPWM_Conventional.Location = new System.Drawing.Point(882, 503);
            this.chkPWM_Conventional.Name = "chkPWM_Conventional";
            this.chkPWM_Conventional.Size = new System.Drawing.Size(415, 50);
            this.chkPWM_Conventional.TabIndex = 44;
            this.chkPWM_Conventional.Text = "PWM_Conventional";
            this.chkPWM_Conventional.UseVisualStyleBackColor = true;
            this.chkPWM_Conventional.CheckedChanged += new System.EventHandler(this.ChkPWM_Conventional_CheckedChanged);
            // 
            // chkStagger
            // 
            this.chkStagger.AutoSize = true;
            this.chkStagger.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkStagger.Location = new System.Drawing.Point(880, 559);
            this.chkStagger.Name = "chkStagger";
            this.chkStagger.Size = new System.Drawing.Size(192, 50);
            this.chkStagger.TabIndex = 45;
            this.chkStagger.Text = "Stagger";
            this.chkStagger.UseVisualStyleBackColor = true;
            this.chkStagger.CheckedChanged += new System.EventHandler(this.ChkStagger_CheckedChanged);
            // 
            // De
            // 
            this.De.AutoSize = true;
            this.De.Location = new System.Drawing.Point(237, 330);
            this.De.Name = "De";
            this.De.Size = new System.Drawing.Size(93, 20);
            this.De.TabIndex = 46;
            this.De.Text = "Debug Print";
            // 
            // SendSettings
            // 
            this.SendSettings.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.SendSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendSettings.Location = new System.Drawing.Point(981, 407);
            this.SendSettings.Name = "SendSettings";
            this.SendSettings.Size = new System.Drawing.Size(316, 84);
            this.SendSettings.TabIndex = 48;
            this.SendSettings.Text = "Send Settings";
            this.SendSettings.UseVisualStyleBackColor = false;
            this.SendSettings.Click += new System.EventHandler(this.SendSettings_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(882, 430);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(62, 61);
            this.pictureBox1.TabIndex = 52;
            this.pictureBox1.TabStop = false;
            // 
            // HelpButton
            // 
            this.HelpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelpButton.Image = ((System.Drawing.Image)(resources.GetObject("HelpButton.Image")));
            this.HelpButton.Location = new System.Drawing.Point(283, 0);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(64, 70);
            this.HelpButton.TabIndex = 53;
            this.HelpButton.Text = " ";
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(79, 13);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(140, 46);
            this.label19.TabIndex = 38;
            this.label19.Text = "Target";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1183, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 46);
            this.label5.TabIndex = 54;
            this.label5.Text = "#1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(1185, 223);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 46);
            this.label12.TabIndex = 55;
            this.label12.Text = "#2";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(1183, 329);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(66, 46);
            this.label23.TabIndex = 56;
            this.label23.Text = "#3";
            // 
            // BtnSprayerControl
            // 
            this.BtnSprayerControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSprayerControl.Location = new System.Drawing.Point(44, 419);
            this.BtnSprayerControl.Name = "BtnSprayerControl";
            this.BtnSprayerControl.Size = new System.Drawing.Size(245, 126);
            this.BtnSprayerControl.TabIndex = 57;
            this.BtnSprayerControl.UseVisualStyleBackColor = true;
            this.BtnSprayerControl.Click += new System.EventHandler(this.BtnSprayerControl_Click);
            // 
            // BtnToggleView
            // 
            this.BtnToggleView.Location = new System.Drawing.Point(394, 424);
            this.BtnToggleView.Name = "BtnToggleView";
            this.BtnToggleView.Size = new System.Drawing.Size(136, 115);
            this.BtnToggleView.TabIndex = 58;
            this.BtnToggleView.UseVisualStyleBackColor = true;
            this.BtnToggleView.Click += new System.EventHandler(this.BtnToggleView_Click);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(130, 573);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(409, 46);
            this.textBox2.TabIndex = 61;
            this.textBox2.Text = "Remember to set Switches! ->>\r\n";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1339, 647);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.BtnToggleView);
            this.Controls.Add(this.BtnSprayerControl);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.SendSettings);
            this.Controls.Add(this.De);
            this.Controls.Add(this.chkStagger);
            this.Controls.Add(this.chkPWM_Conventional);
            this.Controls.Add(this.cmbDebugPwmLevel);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtKd);
            this.Controls.Add(this.txtKi);
            this.Controls.Add(this.txtKp);
            this.Controls.Add(this.txtWheelAngle);
            this.Controls.Add(this.txtBall_Hyd);
            this.Controls.Add(this.txtLowBallValve);
            this.Controls.Add(this.txtHz);
            this.Controls.Add(this.txtcurrentDutyCycle);
            this.Controls.Add(this.txtnumberNozzles);
            this.Controls.Add(this.txtPressureTarget);
            this.Controls.Add(this.txtDutyCycleAdjustment);
            this.Controls.Add(this.txtPSICalibration);
            this.Controls.Add(this.txtFlowCalibration);
            this.Controls.Add(this.txtSprayWidth);
            this.Controls.Add(this.txtGPATarget);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtgpsSpeed);
            this.Controls.Add(this.txtactualGPAave);
            this.Controls.Add(this.txtonTime);
            this.Controls.Add(this.txtpressure);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtpressure;
        private System.Windows.Forms.TextBox txtonTime;
        private System.Windows.Forms.TextBox txtactualGPAave;
        private System.Windows.Forms.TextBox txtgpsSpeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtGPATarget;
        private System.Windows.Forms.TextBox txtSprayWidth;
        private System.Windows.Forms.TextBox txtFlowCalibration;
        private System.Windows.Forms.TextBox txtPSICalibration;
        private System.Windows.Forms.TextBox txtDutyCycleAdjustment;
        private System.Windows.Forms.TextBox txtPressureTarget;
        private System.Windows.Forms.TextBox txtnumberNozzles;
        private System.Windows.Forms.TextBox txtcurrentDutyCycle;
        private System.Windows.Forms.TextBox txtHz;
        private System.Windows.Forms.TextBox txtLowBallValve;
        private System.Windows.Forms.TextBox txtBall_Hyd;
        private System.Windows.Forms.TextBox txtWheelAngle;
        private System.Windows.Forms.TextBox txtKp;
        private System.Windows.Forms.TextBox txtKi;
        private System.Windows.Forms.TextBox txtKd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox cmbDebugPwmLevel;
        private System.Windows.Forms.CheckBox chkPWM_Conventional;
        private System.Windows.Forms.CheckBox chkStagger;
        private System.Windows.Forms.Label De;
        private System.Windows.Forms.Button SendSettings;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button BtnSprayerControl;
        private System.Windows.Forms.Button BtnToggleView;
        private System.Windows.Forms.TextBox textBox2;
    }
}

