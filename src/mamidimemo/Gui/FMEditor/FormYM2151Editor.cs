﻿// copyright-holders:K.Ito
using FM_SoundConvertor;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using zanac.MAmidiMEmo.Instruments;
using zanac.MAmidiMEmo.Instruments.Chips;
using zanac.MAmidiMEmo.Midi;
using zanac.MAmidiMEmo.Properties;
using static zanac.MAmidiMEmo.Instruments.Chips.YM2151;

namespace zanac.MAmidiMEmo.Gui.FMEditor
{
    public partial class FormYM2151Editor : FormFmEditor
    {
        private YM2151Timbre timbre;

        /// <summary>
        /// 
        /// </summary>
        public string MmlValueGeneral
        {
            get
            {
                return GetControl("General").SerializeData;
            }
            set
            {
                GetControl("General").SerializeData = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] MmlValueOps
        {
            get
            {
                List<string> list = new List<string>();
                for (int i = 0; i < timbre.Ops.Length; i++)
                {
                    string opt = GetControl("Operator " + (i + 1)).SerializeData;
                    list.Add(opt);
                }
                return list.ToArray();
            }
            set
            {
                for (int i = 0; i < timbre.Ops.Length; i++)
                {
                    GetControl("Operator " + (i + 1)).SerializeData = value[i];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FormYM2151Editor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public FormYM2151Editor(YM2151 inst, YM2151Timbre timbre, bool singleSelect) : base(inst, timbre, singleSelect)
        {
            this.timbre = timbre;
            InitializeComponent();

            Size = Settings.Default.YM2151EdSize;

            AddControl(new YM2151GeneralContainer(inst, timbre, "General"));

            AddControl(new YM2151OperatorContainer(timbre.Ops[0], "Operator 1"));
            AddControl(new YM2151OperatorContainer(timbre.Ops[1], "Operator 2"));
            AddControl(new YM2151OperatorContainer(timbre.Ops[2], "Operator 3"));
            AddControl(new YM2151OperatorContainer(timbre.Ops[3], "Operator 4"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Settings.Default.YM2151EdSize = Size;
        }

        protected override void ApplyTone(Tone tone)
        {
            ((RegisterValue)this["General"]["ALG"]).Value = tone.AL;
            ((RegisterValue)this["General"]["FB"]).Value = tone.FB;
            ((RegisterValue)this["General"]["AMS"]).Value = 0;
            ((RegisterValue)this["General"]["PMS"]).Value = 0;
            ((RegisterFlag)this["General"]["GlobalSettings.EN"]).Value = false;
            ((RegisterValue)this["General"]["GlobalSettings.LFRQ"]).NullableValue = null;
            ((RegisterValue)this["General"]["GlobalSettings.LFOF"]).NullableValue = null;
            ((RegisterValue)this["General"]["GlobalSettings.LFOD"]).NullableValue = null;
            ((RegisterValue)this["General"]["GlobalSettings.LFOW"]).NullableValue = null;

            for (int i = 0; i < 4; i++)
            {
                ((RegisterFlag)this["Operator " + (i + 1)]["EN"]).Value = true;
                ((RegisterValue)this["Operator " + (i + 1)]["AR"]).Value = tone.aOp[i].AR;
                ((RegisterValue)this["Operator " + (i + 1)]["D1R"]).Value = tone.aOp[i].DR;
                ((RegisterValue)this["Operator " + (i + 1)]["D2R"]).Value = tone.aOp[i].SR;
                ((RegisterValue)this["Operator " + (i + 1)]["RR"]).Value = tone.aOp[i].RR;
                ((RegisterValue)this["Operator " + (i + 1)]["SL"]).Value = tone.aOp[i].SL;
                ((RegisterValue)this["Operator " + (i + 1)]["TL"]).Value = tone.aOp[i].TL;
                ((RegisterValue)this["Operator " + (i + 1)]["RS"]).Value = tone.aOp[i].KS;
                ((RegisterValue)this["Operator " + (i + 1)]["MUL"]).Value = tone.aOp[i].ML;
                ((RegisterValue)this["Operator " + (i + 1)]["DT1"]).Value = tone.aOp[i].DT;
                ((RegisterValue)this["Operator " + (i + 1)]["AM"]).Value = tone.aOp[i].AM;
                ((RegisterValue)this["Operator " + (i + 1)]["DT2"]).Value = tone.aOp[i].DT2;
            }
            timbre.Memo = tone.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tone"></param>
        protected override void ApplyTone(TimbreBase timbre, Tone tone)
        {
            YM2151Timbre tim = (YM2151Timbre)timbre;

            tim.ALG = (byte)tone.AL;
            tim.FB = (byte)tone.FB;
            tim.AMS = (byte)0;
            tim.PMS = (byte)0;
            tim.GlobalSettings.Enable = false;
            tim.GlobalSettings.LFRQ = null;
            tim.GlobalSettings.LFOF = null;
            tim.GlobalSettings.LFOD = null;
            tim.GlobalSettings.LFOW = null;

            for (int i = 0; i < 4; i++)
            {
                tim.Ops[i].Enable = 1;
                tim.Ops[i].AR = (byte)tone.aOp[i].AR;
                tim.Ops[i].D1R = (byte)tone.aOp[i].DR;
                tim.Ops[i].D2R = (byte)tone.aOp[i].SR;
                tim.Ops[i].RR = (byte)tone.aOp[i].RR;
                tim.Ops[i].SL = (byte)tone.aOp[i].SL;
                tim.Ops[i].TL = (byte)tone.aOp[i].TL;
                tim.Ops[i].RS = (byte)tone.aOp[i].KS;
                tim.Ops[i].MUL = (byte)tone.aOp[i].ML;
                tim.Ops[i].DT1 = (byte)tone.aOp[i].DT;
                tim.Ops[i].AM = (byte)tone.aOp[i].AM;
                tim.Ops[i].DT2 = (byte)tone.aOp[i].DT2;
            }
            timbre.Memo = tone.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tone"></param>
        protected override void ApplyTimbre(TimbreBase timbre)
        {
            YM2151Timbre tim = (YM2151Timbre)timbre;
            this.timbre = tim;

            this["General"].Target = tim;
            ((RegisterValue)this["General"]["ALG"]).Value = tim.ALG;
            ((RegisterValue)this["General"]["FB"]).Value = tim.FB;
            ((RegisterValue)this["General"]["AMS"]).Value = tim.AMS;
            ((RegisterValue)this["General"]["PMS"]).Value = tim.PMS;
            ((RegisterFlag)this["General"]["GlobalSettings.EN"]).Value = tim.GlobalSettings.Enable;
            ((RegisterValue)this["General"]["GlobalSettings.LFRQ"]).NullableValue = tim.GlobalSettings.LFRQ;
            ((RegisterValue)this["General"]["GlobalSettings.LFOF"]).NullableValue = tim.GlobalSettings.LFOF;
            ((RegisterValue)this["General"]["GlobalSettings.LFOD"]).NullableValue = tim.GlobalSettings.LFOD;
            ((RegisterValue)this["General"]["GlobalSettings.LFOW"]).NullableValue = tim.GlobalSettings.LFOW;
            for (int i = 0; i < 4; i++)
            {
                this["Operator " + (i + 1)].Target = tim.Ops[i];
                ((RegisterFlag)this["Operator " + (i + 1)]["EN"]).Value = tim.Ops[i].Enable == 0 ? false : true;
                ((RegisterValue)this["Operator " + (i + 1)]["AR"]).Value = tim.Ops[i].AR;
                ((RegisterValue)this["Operator " + (i + 1)]["D1R"]).Value = tim.Ops[i].D1R;
                ((RegisterValue)this["Operator " + (i + 1)]["D2R"]).Value = tim.Ops[i].D2R;
                ((RegisterValue)this["Operator " + (i + 1)]["RR"]).Value = tim.Ops[i].RR;
                ((RegisterValue)this["Operator " + (i + 1)]["SL"]).Value = tim.Ops[i].SL;
                ((RegisterValue)this["Operator " + (i + 1)]["TL"]).Value = tim.Ops[i].TL;
                ((RegisterValue)this["Operator " + (i + 1)]["RS"]).Value = tim.Ops[i].RS;
                ((RegisterValue)this["Operator " + (i + 1)]["MUL"]).Value = tim.Ops[i].MUL;
                ((RegisterValue)this["Operator " + (i + 1)]["DT1"]).Value = tim.Ops[i].DT1;
                ((RegisterValue)this["Operator " + (i + 1)]["AM"]).Value = tim.Ops[i].AM;
                ((RegisterValue)this["Operator " + (i + 1)]["DT2"]).Value = tim.Ops[i].DT2;
            }
        }
    }

}
