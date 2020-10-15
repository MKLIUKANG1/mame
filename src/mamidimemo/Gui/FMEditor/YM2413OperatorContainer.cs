﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using zanac.MAmidiMEmo.Instruments.Chips;
using zanac.MAmidiMEmo.Instruments;

namespace zanac.MAmidiMEmo.Gui.FMEditor
{
    public partial class YM2413OperatorContainer : RegisterContainerBase
    {
        private YM2413.YM2413Modulator mod;

        private YM2413.YM2413Career ca;

        /// <summary>
        /// 
        /// </summary>
        public YM2413OperatorContainer(YM2413.YM2413Modulator mod, string name) : base(mod, name)
        {
            InitializeComponent();

            this.mod = mod;

            AddControl(new RegisterValue("AR", mod.AR, 0, 15));
            AddControl(new RegisterValue("DR", mod.DR, 0, 15));
            AddControl(new RegisterValue("RR", mod.RR, 0, 15));
            AddControl(new RegisterValue("SL", mod.SL, 0, 15));
            AddControl(new RegisterValue("SR", mod.SR == null ? -1 : mod.SR.Value, 0, 15, true));
            AddControl(new RegisterValue("TL", mod.TL, 0, 63));
            AddControl(new RegisterValue("KSL", mod.KSL, 0, 3));
            AddControl(new RegisterValue("KSR", mod.KSR, 0, 1));
            AddControl(new RegisterValue("MUL", mod.MUL, 0, 15));
            AddControl(new RegisterValue("AM", mod.AM, 0, 1));
            AddControl(new RegisterValue("VIB", mod.VIB, 0, 1));
            AddControl(new RegisterValue("EG", mod.EG, 0, 1));
            AddControl(new RegisterValue("DIST", mod.DIST, 0, 1));

            AddControl(new RegisterEnvForm(
                (RegisterValue)GetControl("AR"),
                (RegisterValue)GetControl("TL"),
                (RegisterValue)GetControl("DR"), true,
                (RegisterValue)GetControl("SL"),
                (RegisterValue)GetControl("SR"),
                (RegisterValue)GetControl("RR")
                ));
        }


        /// <summary>
        /// 
        /// </summary>
        public YM2413OperatorContainer(YM2413.YM2413Career ca, string name) : base(ca, name)
        {
            InitializeComponent();

            this.ca = ca;

            AddControl(new RegisterValue("AR", ca.AR, 0, 15));
            AddControl(new RegisterValue("DR", ca.DR, 0, 15));
            AddControl(new RegisterValue("RR", ca.RR, 0, 15));
            AddControl(new RegisterValue("SL", ca.SL, 0, 15));
            AddControl(new RegisterValue("SR", ca.SR == null ? -1 : ca.SR.Value, 0, 15, true));
            AddControl(new RegisterValue("KSL", ca.KSL, 0, 3));
            AddControl(new RegisterValue("KSR", ca.KSR, 0, 1));
            AddControl(new RegisterValue("MUL", ca.MUL, 0, 15));
            AddControl(new RegisterValue("AM", ca.AM, 0, 1));
            AddControl(new RegisterValue("VIB", ca.VIB, 0, 1));
            AddControl(new RegisterValue("EG", ca.EG, 0, 1));
            AddControl(new RegisterValue("DIST", ca.DIST, 0, 1));

            AddControl(new RegisterEnvForm(
                (RegisterValue)GetControl("AR"),
                null,
                (RegisterValue)GetControl("DR"), true,
                (RegisterValue)GetControl("SL"),
                (RegisterValue)GetControl("SR"),
                (RegisterValue)GetControl("RR")
                ));
        }
    }
}
