using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        private SerialPort serialPortArduino = new SerialPort() { ReadTimeout = 1000, WriteTimeout = 1000 };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino == null)
                    serialPortArduino = new SerialPort() { ReadTimeout = 1000, WriteTimeout = 1000 };

                if (serialPortArduino.IsOpen)
                {
                    // Disconnect
                    try { serialPortArduino.Close(); } catch { }
                    radioButtonVerbonden.Checked = false;
                    buttonConnect.Text = "Connect";
                    labelStatus.Text = "Disconnected";
                }
                else
                {
                    // Set properties from UI
                    serialPortArduino.PortName = comboBoxPoort.SelectedItem != null ? comboBoxPoort.SelectedItem.ToString() : comboBoxPoort.Text;

                    int baud = 115200;
                    if (comboBoxBaudrate.SelectedItem != null) int.TryParse(comboBoxBaudrate.SelectedItem.ToString(), out baud);
                    else int.TryParse(comboBoxBaudrate.Text, out baud);
                    serialPortArduino.BaudRate = baud;

                    serialPortArduino.DataBits = (int)numericUpDownDatabits.Value;

                    // Parity
                    if (radioButtonParityEven.Checked) serialPortArduino.Parity = Parity.Even;
                    else if (radioButtonParityOdd.Checked) serialPortArduino.Parity = Parity.Odd;
                    else if (radioButtonParityMark.Checked) serialPortArduino.Parity = Parity.Mark;
                    else if (radioButtonParitySpace.Checked) serialPortArduino.Parity = Parity.Space;
                    else serialPortArduino.Parity = Parity.None;

                    // StopBits
                    if (radioButtonStopbitsTwo.Checked) serialPortArduino.StopBits = StopBits.Two;
                    else if (radioButtonStopbitsOnePointFive.Checked) serialPortArduino.StopBits = StopBits.OnePointFive;
                    else if (radioButtonStopbitsNone.Checked) serialPortArduino.StopBits = StopBits.None;
                    else serialPortArduino.StopBits = StopBits.One;

                    // Handshake
                    if (radioButtonHandshakeRTS.Checked) serialPortArduino.Handshake = Handshake.RequestToSend;
                    else if (radioButtonHandshakeRTSXonXoff.Checked) serialPortArduino.Handshake = Handshake.RequestToSendXOnXOff;
                    else if (radioButtonHandshakeXonXoff.Checked) serialPortArduino.Handshake = Handshake.XOnXOff;
                    else serialPortArduino.Handshake = Handshake.None;

                    serialPortArduino.RtsEnable = checkBoxRtsEnable.Checked;
                    serialPortArduino.DtrEnable = checkBoxDtrEnable.Checked;

                    // Open
                    serialPortArduino.Open();

                    radioButtonVerbonden.Checked = true;
                    buttonConnect.Text = "Disconnect";
                    labelStatus.Text = $"Connected: {serialPortArduino.PortName} @ {serialPortArduino.BaudRate}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening/closing serial port: {ex.Message}", "Serial error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error: " + ex.Message;
            }
        }
    }
}
