﻿/*!
Apache License
Version 2.0, January 2004

Copyright (c) 2018 Yiannis Menexes <johnmenex@gmail.com>, Dimitris Katikaridis <dkatikaridis@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace MCT {
    public partial class RealTimeValues : Form {
        public RealTimeValues() {
            InitializeComponent();
        }

        public RealTimeValues(List<int> _active_sensors, double[] _current_Values, int _sampling_rate) {
            if (_active_sensors == null || _active_sensors.Count == 0)
                return;

            InitializeComponent();
            CenterToScreen();

            Sampling_rate = _sampling_rate;
            ActiveSensors = _active_sensors;
            NumberOfSensors = _active_sensors.Count;

            InitGraphs(NumberOfSensors, _current_Values);
            InitUI();
            
            timer_visualiser.Start();
        }
        public void ReceiveData(double[] _sensor_values) {
            SensorValues = _sensor_values;
        }


        private bool barInitialized;
        private protected int sampling_rate;
        private protected int _number_of_sensors;
        private List<int> _activeSensors;
        private protected GraphPane z;
        private protected List<BarItem> _bar;
        private protected double[] _sensorValues;
        private List<GroupBox> gb_threshold;
        private List<string> labelNames;


        private protected int NumberOfSensors { get => _number_of_sensors; set => _number_of_sensors = value; }
        private protected List<BarItem> Bar { get => _bar; set => _bar = value; }
        private protected int Sampling_rate { get => sampling_rate; set => sampling_rate = value; }
        private protected bool BarInitialized { get => barInitialized; set => barInitialized = value; }
        private protected double[] SensorValues { get => _sensorValues; set => _sensorValues = value; }
        private protected List<GroupBox> Gb_threshold { get => gb_threshold; set => gb_threshold = value; }
        private List<int> ActiveSensors { get => _activeSensors; set => _activeSensors = value; }
        private List<string> LabelNames { get => labelNames; set => labelNames = value; }
        
        public void Stop() { timer_visualiser.Stop(); }
        public void Start() { timer_visualiser.Start(); }
        
        private protected void InitUI() {
            int column = 0;
            int row = 0;
            Gb_threshold = new List<GroupBox>();
            for (int i = 0; i < NumberOfSensors; i++) {

                #region Parent groupbox
                Gb_threshold.Add(new GroupBox());
                Gb_threshold[i].Text = "Sensor " + ActiveSensors[i];
                #endregion

                #region create cb_sensor -> Gb_threshold.Controls[0]
                CheckBox cb_threshold = new CheckBox();
                cb_threshold.Location = new Point(4, 15);
                cb_threshold.AutoSize = true;
                cb_threshold.Name = "cb_sensor" + ActiveSensors[i];
                cb_threshold.Text = "Threshold";
                cb_threshold.Checked = false;
                cb_threshold.Show();
                Gb_threshold[i].Controls.Add(cb_threshold);
                #endregion

                #region create min_label -> Gb_threshold.Controls[1]
                System.Windows.Forms.Label lb_min = new System.Windows.Forms.Label();
                lb_min.AutoSize = true;
                lb_min.Name = "lb_min";
                lb_min.Text = "Min:";
                lb_min.Location = new Point(2, cb_threshold.Location.Y + cb_threshold.Height + 5);
                lb_min.Show();
                Gb_threshold[i].Controls.Add(lb_min);
                #endregion

                #region create min_nuD -> Gb_threshold.Controls[2]
                NumericUpDown nUD_min = new NumericUpDown();
                nUD_min.Minimum = -999;
                nUD_min.Maximum = 999;
                nUD_min.Width = 40;
                nUD_min.Location = new Point(lb_min.Location.X + lb_min.Width + 3, lb_min.Location.Y - 5);
                nUD_min.BackColor = Color.FromKnownColor(KnownColor.Control);
                nUD_min.Show();
                Gb_threshold[i].Controls.Add(nUD_min);
                #endregion

                #region create max_label -> Gb_threshold.Controls[3]
                System.Windows.Forms.Label lb_max = new System.Windows.Forms.Label();
                lb_max.AutoSize = true;
                lb_max.Name = "lb_max";
                lb_max.Text = "Max:";
                lb_max.Location = new Point(2, lb_min.Location.Y + lb_min.Height + 10);
                lb_max.Show();
                Gb_threshold[i].Controls.Add(lb_max);
                #endregion

                #region create max_nUD -> Gb_threshold.Controls[4]
                NumericUpDown nUD_max = new NumericUpDown();
                nUD_max.Minimum = -999;
                nUD_max.Maximum = 999;
                nUD_max.Width = 40;
                nUD_max.Location = new Point(lb_max.Location.X + lb_max.Width, lb_max.Location.Y - 5);
                nUD_max.BackColor = Color.FromKnownColor(KnownColor.Control);
                nUD_max.Show();
                Gb_threshold[i].Controls.Add(nUD_max);
                #endregion

                #region Set the dynamic location of each groupbox
                Gb_threshold[i].AutoSize = true;
                Gb_threshold[i].Width = cb_threshold.Width;

                if (row <= 0)
                    Gb_threshold[i].Location = new Point(
                        5 + (column * (Gb_threshold[i].Width + 30)),
                        15 + (row * Gb_threshold[i].Height)
                        );
                else
                    Gb_threshold[i].Location = new Point(
                    5 + (column * (Gb_threshold[i].Width + 30)),
                    15 + (row * Gb_threshold[i].Height + 5)
                    );
                column++;
                if ((5 + (column * (Gb_threshold[i].Width + 30))) > gb_parent.Width) {
                    if (column != NumberOfSensors) {
                        gb_parent.Height += Gb_threshold[i].Height;
                        Height += Gb_threshold[i].Height;
                    }
                    column = 0;
                    row++;
                }
                #endregion

                Gb_threshold[i].Show();
                gb_parent.Controls.Add(Gb_threshold[i]);
            }
        }
        private protected void InitGraphs(int _nSensors, double[] _curValues) {
            InitGraphPane();
            InitBar(_nSensors, _curValues);

            timer_visualiser.Interval = Sampling_rate;
        }
        private protected void InitGraphPane() {
            z = zedGraphControl1.GraphPane;

            z.YAxis.Scale.MaxAuto = true;
            z.YAxis.Scale.MinAuto = true;

            z.XAxis.MajorGrid.DashOff = 0;
            z.YAxis.MajorGrid.DashOff = 0;

            z.XAxis.MajorGrid.Color = Color.DarkSlateGray;
            z.XAxis.MinorGrid.IsVisible = true;
            z.XAxis.MinorGrid.Color = Color.Gray;
            z.XAxis.MajorGrid.IsVisible = true;

            z.YAxis.MajorGrid.Color = Color.DarkSlateGray;
            z.YAxis.MajorGrid.IsVisible = true;
            z.YAxis.MinorGrid.Color = Color.Gray;
            z.YAxis.MinorGrid.IsVisible = true;

            z.XAxis.IsAxisSegmentVisible = false;
            z.XAxis.IsVisible = false;

            z.Title.Text = "Realtime Values";

            z.XAxis.Title.Text = "Sensors";
            z.XAxis.IsVisible = true;

            z.YAxis.Title.Text = "Temperature";
            z.YAxis.IsVisible = true;

            zedGraphControl1.ContextMenuBuilder +=
                (ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
                    => Zed_CustomRightClickMenu(sender, menuStrip, mousePt, objState);
        }
        private protected void InitBar(int _nSensors, double[] _curValues) {
            List<Color> Colors = new List<Color>() {
                Color.Green,
                Color.Red,
                Color.Black,
                Color.Yellow,
                Color.Blue,
                Color.Brown,
                Color.Purple,
                Color.Orange,
                Color.Gray,
                Color.LightBlue,
                Color.DarkGreen,
                Color.Olive
                };
            Bar = new List<BarItem>();
            LabelNames = new List<string>();
            for (int i = 0; i < _nSensors; i++) {
                double[] x_value = new double[1] { i + 1 };
                double[] y_value = new double[1] { _curValues[i] };
                Bar.Add(zedGraphControl1.GraphPane.AddBar(("Sensor" + ActiveSensors[i]), x_value, y_value, Colors[i]));
                LabelNames.Add(Bar[i].Label.Text);
            }
            z.XAxis.Scale.Min = Bar[0].Points[0].X - 1;
            z.XAxis.Scale.Max = Bar[Bar.Count - 1].Points[0].X + 1;
            z.AxisChange();

            BarInitialized = true;
        }

        private void Zed_CustomRightClickMenu(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState) {
            foreach (ToolStripMenuItem _item in menuStrip.Items) {
                if ((string)_item.Tag == "show_val") {
                    menuStrip.Items.Remove(_item);
                    break;
                }
            }
        }

        private protected void RefreshBars() {
            double[] _sensor_values = SensorValues;

            int _index = 0;
            foreach (BarItem _b in Bar) {
                _b.Clear();
                _b.AddPoint(_index + 1, _sensor_values[_index]);
                _b.Label.Text = LabelNames[_index] + " - " + _sensorValues[_index];
                _index++;
            }

            zedGraphControl1.Refresh();
        }

        private protected void CheckThresholds() {
            int _index = 0;
            foreach (GroupBox _gb in gb_parent.Controls) {
                if (((CheckBox)_gb.Controls[0]).Checked) {
                    if ((double)((NumericUpDown)_gb.Controls[2]).Value > _sensorValues[_index])
                        ((NumericUpDown)_gb.Controls[2]).BackColor = Color.Red;
                    else
                        ((NumericUpDown)_gb.Controls[2]).BackColor = Color.FromKnownColor((KnownColor.Control));

                    if ((double)((NumericUpDown)_gb.Controls[4]).Value < _sensorValues[_index])
                        ((NumericUpDown)_gb.Controls[4]).BackColor = Color.Red;
                    else
                        ((NumericUpDown)_gb.Controls[4]).BackColor = Color.FromKnownColor((KnownColor.Control));
                }
                else {
                    ((NumericUpDown)_gb.Controls[2]).BackColor = Color.FromKnownColor((KnownColor.Control));
                    ((NumericUpDown)_gb.Controls[4]).BackColor = Color.FromKnownColor((KnownColor.Control));
                }
                _index++;
            }
        }

        private void timer_visualiser_Tick(object sender, EventArgs e) {
            if (BarInitialized)
                RefreshBars();


            CheckThresholds();
        }
    }
}