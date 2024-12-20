﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keypad
{
    public partial class NumKeypad : Form
    {
        public NumKeypad()
        {
            InitializeComponent(); // This is required for the Designer to work
        }

        // Declare the ButtonPressed event
        public event KeyPressEventHandler ButtonPressed;

        // Method to raise the ButtonPressed event
        private void RaiseButtonPressed(char whatToSend)
        {
            ButtonPressed?.Invoke(this, new KeyPressEventArgs(whatToSend));
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Hide(); // Simply hides the keypad
        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('1');
        }

        private void Btn2_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('2');

        }

        private void Btn3_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('3');

        }

        private void Btn4_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('4');

        }

        private void Btn5_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('5');

        }

        private void Btn6_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('6');

        }

        private void Btn7_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('7');

        }

        private void Btn8_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('8');

        }

        private void Btn9_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('9');

        }

        private void Btn0_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('0');

        }

        private void BtnPlusMinus_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('-');

        }

        private void BtnDecimal_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('.');

        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('C');

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('X');

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('K');

        }

        private void BtnBackSpace_Click(object sender, EventArgs e)
        {
            RaiseButtonPressed('B');
        }

        private void NumKeypad_Load(object sender, EventArgs e)
        {

        }

        private void NumKeypad_Load_1(object sender, EventArgs e)
        {

        }
    }
}
