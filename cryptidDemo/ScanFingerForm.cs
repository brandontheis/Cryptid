﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using cryptid.Scanners;
using Cryptid.Utils;

namespace cryptidDemo {
    public partial class ScanFingerForm : Form {
        private enum State {
            FINGER_NOT_PRESSED,
            FINGER_PRESSED,
            TRANSFERING_DATA,
            TRANSFER_COMPLETE
        }

        private State _currState; 
        private State CurrentState {
            get { return _currState; }
            set {
                _currState = value;
                OnStateChange(value);
            }
        }

        public Bitmap Fingerprint { get; set; }

        public ScanFingerForm() {
            InitializeComponent();
        }

        private void ScanFingerForm_Load(object sender, EventArgs e) {
            Task t = Task.Factory.StartNew(() => {
                CurrentState = State.FINGER_NOT_PRESSED;
                FPS_GT511C3.SetCmosLed(true);
                while (FPS_GT511C3.IsPressingFinger() != 0) Task.Delay(1000);
                CurrentState = State.FINGER_PRESSED;
                CurrentState = State.TRANSFERING_DATA;
                Fingerprint = FPS_GT511C3.GetImage();
                FPS_GT511C3.SetCmosLed(false);
                CurrentState = State.TRANSFER_COMPLETE;
                DialogResult = DialogResult.OK;
                SafeClose();
            });
            t.Start();

            
        }

        private void OnStateChange(State s) {
            switch (CurrentState) {
                case State.FINGER_NOT_PRESSED:
                    BackColor = Color.Red;
                    stateText.SetPropertyThreadSafe(() => stateText.Text, "Place Finger on Sensor");
                    break;
                case State.FINGER_PRESSED:
                    BackColor = Color.Blue;
                    stateText.SetPropertyThreadSafe(() => stateText.Text, "Begining Transfer");
                    break;
                case State.TRANSFERING_DATA:
                    BackColor = Color.Green;
                    stateText.SetPropertyThreadSafe(() => stateText.Text, "Transfering Data...");
                    break;
                case State.TRANSFER_COMPLETE:
                    BackColor = Color.Green;
                    stateText.SetPropertyThreadSafe(() => stateText.Text, "Transfering Complete");
                    break;
            }
        }

        public void SafeClose() {
            if (InvokeRequired) {
                BeginInvoke(new Action(SafeClose));
                return;
            }
            Close();
        }
    }
}