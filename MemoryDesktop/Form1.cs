using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MemoryDesktop.Properties;
using MemoryLibrary;

namespace MemoryDesktop
{
    public partial class Form1 : Form
    {
        private ToolStripMenuItem previousSpeedMenuItem;
        private const string appPrefix = "Desktop";

        public Form1()
        {
            InitializeComponent();

            switch (Settings.Default.TimeInterval)
            {
                case "5sec":
                    this.timer1.Interval = 1000 * 5; //5 sec
                    previousSpeedMenuItem = per15SecToolStripMenuItem;
                    this.timer1.Enabled = true;
                    break;
                case "15sec":
                    this.timer1.Interval = 1000 * 15; //15 sec
                    previousSpeedMenuItem = perMinuteToolStripMenuItem;
                    this.timer1.Enabled = true;
                    break;
                case "1min":
                    this.timer1.Interval = 1000 * 60; //1 minute
                    previousSpeedMenuItem = toolStripMenuItem7;
                    this.timer1.Enabled = true;
                    break;
                case "5min":
                    this.timer1.Interval = 1000 * 60 * 5; //5 min
                    previousSpeedMenuItem = minutesToolStripMenuItem;
                    this.timer1.Enabled = true;
                    break;
                case "15min":
                    this.timer1.Interval = 1000 * 60 * 15; //15 min
                    previousSpeedMenuItem = minutesToolStripMenuItem1;
                    this.timer1.Enabled = true;
                    break;
                case "1hour":
                    this.timer1.Interval = 1000 * 60 * 60; //1 hour
                    previousSpeedMenuItem = hourToolStripMenuItem;
                    this.timer1.Enabled = true;
                    break;
                default:
                    this.timer1.Interval = 1000 * 60; //1 minute
                    previousSpeedMenuItem = toolStripMenuItem7;
                    this.timer1.Enabled = true;
                    break;
            }

            previousSpeedMenuItem.Checked = true;

            //Display first memory string
            GetNextMemoryString(this.label1);

        }

        private void GetNextMemoryString(Label label)
        {
            label.Text = MemoryInterface.GetMemoryString(appPrefix);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Show/Hide MemoryDesktop control
            //this.Visible = !this.Visible;

            //Toggle next one
            this.timer1.Stop();
            this.label1.Text = MemoryInterface.GetMemoryString(appPrefix);
            this.timer1.Start();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //toggle next memory string
            this.label1.Text = MemoryInterface.GetMemoryString(appPrefix);
        }

        #region SpeedEventHandling
        private void hourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000 * 60 * 60;
            previousSpeedMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            previousSpeedMenuItem = (ToolStripMenuItem)sender;
            Settings.Default.TimeInterval = "1hour";
        }

        private void minutesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000 * 60 * 15; //15 min
            previousSpeedMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            previousSpeedMenuItem = (ToolStripMenuItem)sender;
            Settings.Default.TimeInterval = "15min";
        }

        private void minutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000 * 60 * 5; //5 min
            previousSpeedMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            previousSpeedMenuItem = (ToolStripMenuItem)sender;
            Settings.Default.TimeInterval = "5min";
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000 * 60 * 1; //1 min
            previousSpeedMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            previousSpeedMenuItem = (ToolStripMenuItem)sender;
            Settings.Default.TimeInterval = "1min";
        }

        private void perMinuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000 * 15 * 1; //15 sec
            previousSpeedMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            previousSpeedMenuItem = (ToolStripMenuItem)sender;
            Settings.Default.TimeInterval = "15sec";
        }

        private void per15SecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000 * 5 * 1; //5 sec
            previousSpeedMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            previousSpeedMenuItem = (ToolStripMenuItem)sender;
            Settings.Default.TimeInterval = "5sec";
        }
        #endregion SpeedEventHandling

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.timer1.Enabled = false;
            Settings.Default.Save();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            this.timer1.Stop();
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            this.timer1.Start();
        }

        #region GradeEventHandling
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //5
            int gradeValue = 5;
            float prevGradeValue = 0.0f, currentGradeValue = 0.0f;
            ConfigurationFile.GradeLatestMemoryString(gradeValue, out prevGradeValue, out currentGradeValue, appPrefix);

            this.label1.Text = MemoryInterface.GetMemoryString(appPrefix);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //4
            int gradeValue = 4;
            float prevGradeValue = 0.0f, currentGradeValue = 0.0f;
            ConfigurationFile.GradeLatestMemoryString(gradeValue, out prevGradeValue, out currentGradeValue, appPrefix);

            this.label1.Text = MemoryInterface.GetMemoryString(appPrefix);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //3
            int gradeValue = 3;
            float prevGradeValue = 0.0f, currentGradeValue = 0.0f;
            ConfigurationFile.GradeLatestMemoryString(gradeValue, out prevGradeValue, out currentGradeValue, appPrefix);

            this.label1.Text = MemoryInterface.GetMemoryString(appPrefix);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            //2
            int gradeValue = 2;
            float prevGradeValue = 0.0f, currentGradeValue = 0.0f;
            ConfigurationFile.GradeLatestMemoryString(gradeValue, out prevGradeValue, out currentGradeValue, appPrefix);
            MessageBox.Show("previous grade: "+ prevGradeValue + " current grade:" + currentGradeValue);

            this.label1.Text = MemoryInterface.GetMemoryString(appPrefix);
        }
        #endregion

    }
}
