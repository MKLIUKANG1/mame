﻿// copyright-holders:K.Ito
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.MusicTheory;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;
using zanac.MAmidiMEmo.ComponentModel;
using zanac.MAmidiMEmo.Gui;
using zanac.MAmidiMEmo.Instruments.Envelopes;
using zanac.MAmidiMEmo.Mame;
using zanac.MAmidiMEmo.Midi;


namespace zanac.MAmidiMEmo.Instruments.Chips
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class CM32P : InstrumentBase
    {

        public override string Name => "CM32P";

        public override string Group => "LA";

        public override InstrumentType InstrumentType => InstrumentType.CM32P;

        [Browsable(false)]
        public override string ImageKey => "CM32P";

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        protected override string SoundInterfaceTagNamePrefix => "cm32p_";

        /// <summary>
        /// 
        /// </summary>
        [Category("MIDI")]
        [Description("MIDI Device ID")]
        [IgnoreDataMember]
        [JsonIgnore]
        public override uint DeviceID
        {
            get
            {
                return 21;
            }
        }

        private static string[] cardToneTableFileNames;

        private static string[] CardToneTableFilesNames
        {
            get
            {
                if (cardToneTableFileNames == null)
                {
                    List<string> cn = new List<string>();
                    {
                        string tblp = Path.Combine(Environment.CurrentDirectory, "Data", "cm32p_internal_tone.tbl");
                        cn.Add(tblp);
                    }
                    for (byte ci = 1; ci <= 15; ci++)
                    {
                        string tn = "cm32p_card" + ci.ToString("00") + "_tone.tbl";
                        string tblp = Path.Combine(Environment.CurrentDirectory, "Data", tn);
                        cn.Add(tblp);
                    }
                    cardToneTableFileNames = cn.ToArray();
                }
                return cardToneTableFileNames;
            }
        }

        private SN_U110_Cards card;

        [DataMember]
        [Category("Chip")]
        [Description("Set inserted SN-U110 card No.")]
        [DefaultValue(SN_U110_Cards.None)]
        [TypeConverter(typeof(SN_U110_CardsConverter))]
        public SN_U110_Cards Card
        {
            get
            {
                return card;
            }
            set
            {
                if (card != value)
                {
                    card = value;

                    CM32P_set_card(UnitNumber, (byte)card);
                }
            }
        }

        private class SN_U110_CardsConverter : TypeConverter
        {

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
            {
                return srcType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return Enum.Parse(typeof(SN_U110_Cards), (string)value);
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                CM32P cm32p = context.Instance as CM32P;

                if (cm32p != null)
                    return true;
                else
                    return base.GetStandardValuesSupported();
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                List<string> names = new List<string>();
                names.Add(Enum.GetName(typeof(SN_U110_Cards), SN_U110_Cards.None));
                for (byte ci = 1; ci < CardToneTableFilesNames.Length; ci++)
                {
                    if (File.Exists(CardToneTableFilesNames[ci]))
                        names.Add(Enum.GetName(typeof(SN_U110_Cards), ci));
                }
                return new StandardValuesCollection(names);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public override TimbreBase[] BaseTimbres
        {
            get
            {
                return Timbres;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public CM32PTimbre[] Timbres
        {
            get;
            private set;
        }

        [Browsable(false)]
        public override bool[] Channels
        {
            get
            {
                return base.Channels;
            }
        }

        [Browsable(false)]
        public override ushort[] Pitchs
        {
            get
            {
                return base.Pitchs;
            }
        }

        [Browsable(false)]
        public override byte[] PitchBendRanges
        {
            get
            {
                return base.PitchBendRanges;
            }
        }

        [Browsable(false)]
        public override byte[] ProgramNumbers
        {
            get
            {
                return base.ProgramNumbers;
            }
        }

        [Browsable(false)]
        public override byte[] Volumes
        {
            get
            {
                return base.Volumes;
            }
        }

        [Browsable(false)]
        public override byte[] Expressions
        {
            get
            {
                return base.Expressions;
            }
        }


        [Browsable(false)]
        public override byte[] Panpots
        {
            get
            {
                return base.Panpots;
            }
        }


        [Browsable(false)]
        public override byte[] Modulations
        {
            get
            {
                return base.Modulations;
            }
        }

        [Browsable(false)]
        public override byte[] ModulationRates
        {
            get
            {
                return base.ModulationRates;
            }
        }

        [Browsable(false)]
        public override byte[] ModulationDepthes
        {
            get
            {
                return base.ModulationDepthes;
            }
        }


        [Browsable(false)]
        public override byte[] ModulationDelays
        {
            get
            {
                return base.ModulationDelays;
            }
        }


        [Browsable(false)]
        public override byte[] ModulationDepthRangesNote
        {
            get
            {
                return base.ModulationDepthRangesNote;
            }
        }

        [Browsable(false)]
        public override byte[] ModulationDepthRangesCent
        {
            get
            {
                return base.ModulationDepthRangesCent;
            }
        }

        [Browsable(false)]
        public override byte[] Portamentos
        {
            get
            {
                return base.Portamentos;
            }
        }

        [Browsable(false)]
        public override byte[] PortamentoTimes
        {
            get
            {
                return base.PortamentoTimes;
            }
        }

        [Browsable(false)]
        public override byte[] MonoMode
        {
            get
            {
                return base.MonoMode;
            }
        }


        private const float DEFAULT_GAIN = 1.0f;

        public override bool ShouldSerializeGainLeft()
        {
            return GainLeft != DEFAULT_GAIN;
        }

        public override void ResetGainLeft()
        {
            GainLeft = DEFAULT_GAIN;
        }

        public override bool ShouldSerializeGainRight()
        {
            return GainRight != DEFAULT_GAIN;
        }

        public override void ResetGainRight()
        {
            GainRight = DEFAULT_GAIN;
        }

        public override bool ShouldSerializeFilterMode()
        {
            return FilterMode != FilterMode.None;
        }

        public override void ResetFilterMode()
        {
            FilterMode = FilterMode.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TimbreBase GetTimbre(int channel)
        {
            var pn = (SevenBitNumber)ProgramNumbers[channel];
            return Timbres[pn];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializeData"></param>
        public override void RestoreFrom(string serializeData)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<CM32P>(serializeData);
                this.InjectFrom(new LoopInjection(new[] { "SerializeData" }), obj);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(Exception))
                    throw;
                else if (ex.GetType() == typeof(SystemException))
                    throw;


                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void delegate_CM32P_play_msg(uint unitNumber, byte type, byte channel, byte param1, byte param2);

        /// <summary>
        /// 
        /// </summary>
        private static delegate_CM32P_play_msg CM32P_play_msg
        {
            get;
            set;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void delegate_CM32P_set_tone(uint unitNumber, byte card_id, byte tone_no, ushort sf_preset_no);

        /// <summary>
        /// 
        /// </summary>
        private static delegate_CM32P_set_tone CM32P_set_tone
        {
            get;
            set;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void delegate_CM32P_initlaize_meory(uint unitNumber);

        /// <summary>
        /// 
        /// </summary>
        private static delegate_CM32P_initlaize_meory CM32P_initlaize_meory
        {
            get;
            set;
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void delegate_CM32P_play_sysex(uint unitNumber, byte[] sysex, int len);

        /// <summary>
        /// 
        /// </summary>
        private static delegate_CM32P_play_sysex CM32P_play_sysex
        {
            get;
            set;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void delegate_CM32P_load_sf(uint unitNumber, byte cardId, string fileName);

        /// <summary>
        /// 
        /// </summary>
        private static delegate_CM32P_load_sf CM32P_load_sf
        {
            get;
            set;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void delegate_CM32P_set_card(uint unitNumber, byte cardId);

        /// <summary>
        /// 
        /// </summary>
        private static delegate_CM32P_set_card CM32P_set_card
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CM32PPlayMsgNow(uint unitNumber, byte type, byte channel, byte param1, byte param2)
        {
            try
            {
                Program.SoundUpdating();
                CM32P_play_msg(unitNumber, type, channel, param1, param2);
            }
            finally
            {
                Program.SoundUpdated();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void CM32PPlaySysExNow(uint unitNumber, byte[] sysex)
        {
            try
            {
                Program.SoundUpdating();
                CM32P_play_sysex(unitNumber, sysex, sysex.Length);
            }
            finally
            {
                Program.SoundUpdated();
            }
        }

        private static FieldInfo channelEventParameters;

        /// <summary>
        /// 
        /// </summary>
        static CM32P()
        {
            IntPtr funcPtr = MameIF.GetProcAddress("cm32p_play_msg");
            if (funcPtr != IntPtr.Zero)
            {
                CM32P_play_msg = (delegate_CM32P_play_msg)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(delegate_CM32P_play_msg));
            }
            funcPtr = MameIF.GetProcAddress("cm32p_play_sysex");
            if (funcPtr != IntPtr.Zero)
            {
                CM32P_play_sysex = (delegate_CM32P_play_sysex)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(delegate_CM32P_play_sysex));
            }
            funcPtr = MameIF.GetProcAddress("cm32p_load_sf");
            if (funcPtr != IntPtr.Zero)
            {
                CM32P_load_sf = (delegate_CM32P_load_sf)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(delegate_CM32P_load_sf));
            }
            funcPtr = MameIF.GetProcAddress("cm32p_set_tone");
            if (funcPtr != IntPtr.Zero)
            {
                CM32P_set_tone = (delegate_CM32P_set_tone)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(delegate_CM32P_set_tone));
            }
            funcPtr = MameIF.GetProcAddress("cm32p_initialize_memory");
            if (funcPtr != IntPtr.Zero)
            {
                CM32P_initlaize_meory = (delegate_CM32P_initlaize_meory)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(delegate_CM32P_initlaize_meory));
            }
            funcPtr = MameIF.GetProcAddress("cm32p_set_card");
            if (funcPtr != IntPtr.Zero)
            {
                CM32P_set_card = (delegate_CM32P_set_card)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(delegate_CM32P_set_card));
            }

            channelEventParameters = typeof(ChannelEvent).GetField("_parameters", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        private CM32PSoundManager soundManager;

        /// <summary>
        /// 
        /// </summary>
        public CM32P(uint unitNumber) : base(unitNumber)
        {
            GainLeft = DEFAULT_GAIN;
            GainRight = DEFAULT_GAIN;
            FilterMode = FilterMode.None;

            this.soundManager = new CM32PSoundManager(this);

            Timbres = new CM32PTimbre[128];
            for (int i = 0; i < 128; i++)
                Timbres[i] = new CM32PTimbre();
        }

        internal override void PrepareSound()
        {
            base.PrepareSound();

            loadSfTable();
            CM32P_initlaize_meory(UnitNumber);
        }

        private void loadSfTable()
        {
            for (byte cid = 0; cid < CardToneTableFilesNames.Length; cid++)
            {
                string cfn = CardToneTableFilesNames[cid];
                if (File.Exists(CardToneTableFilesNames[cid]))
                {
                    using (var s = File.OpenText(cfn))
                    {
                        string fn = s.ReadLine();
                        CM32P_load_sf(UnitNumber, cid, Path.Combine(Environment.CurrentDirectory, "Data", fn));
                        while (!s.EndOfStream)
                        {
                            var line = s.ReadLine();
                            string[] ns = line.Split(',');
                            string tone_no_t = ns[0].Split(':')[1];
                            byte tone_no = byte.Parse(tone_no_t);
                            string[] preset_no_t = ns[1].Split(':');
                            ushort preset_no = (ushort)(ushort.Parse(preset_no_t[0]) << 8 | ushort.Parse(preset_no_t[1]));
                            CM32P_set_tone(UnitNumber, cid, tone_no, preset_no);
                        }
                    }
                }
            }
        }

        protected override void OnMidiEvent(MidiEvent midiEvent)
        {
            byte type = 0;
            byte ch = 0;
            byte param1 = 0;
            byte param2 = 0;
            switch (midiEvent)
            {
                case SysExEvent sysex:
                    {
                        List<byte> data = new List<byte>();
                        data.Add(0xf0);
                        data.AddRange(sysex.Data);
                        CM32PPlaySysExNow(UnitNumber, data.ToArray());
                        return;
                    }
                case NoteOffEvent noff:
                    {
                        type = 0x80;
                        ch = noff.Channel;
                        param1 = noff.NoteNumber;
                        param2 = noff.Velocity;
                        break;
                    }
                case NoteOnEvent non:
                    {
                        if (non.Velocity == 0)
                            type = 0x80;
                        else
                            type = 0x90;
                        ch = non.Channel;
                        param1 = non.NoteNumber;
                        param2 = non.Velocity;
                        break;
                    }
                case NoteAftertouchEvent na:
                    {
                        type = 0xa0;
                        ch = na.Channel;
                        param1 = na.NoteNumber;
                        param2 = na.AftertouchValue;
                        break;
                    }
                case ControlChangeEvent cc:
                    {
                        type = 0xb0;
                        ch = cc.Channel;
                        param1 = cc.ControlNumber;
                        param2 = cc.ControlValue;
                        break;
                    }
                case ProgramChangeEvent pc:
                    {
                        type = 0xc0;
                        ch = pc.Channel;
                        param1 = pc.ProgramNumber;
                        break;
                    }
                case ChannelAftertouchEvent ca:
                    {
                        type = 0xd0;
                        ch = ca.Channel;
                        param1 = ca.AftertouchValue;
                        break;
                    }
                case PitchBendEvent pb:
                    {
                        type = 0xe0;
                        ch = pb.Channel;
                        param1 = (byte)(pb.PitchValue & 0x7f);
                        param2 = (byte)(pb.PitchValue >> 7);
                        break;
                    }
                case TimingClockEvent tc:
                    {
                        type = 0xf8;
                        break;
                    }
                case ActiveSensingEvent ase:
                    {
                        type = 0xfe;
                        break;
                    }
                case ResetEvent re:
                    {
                        type = 0xff;
                        break;
                    }
            }

            CM32PPlayMsgNow(UnitNumber, type, ch, param1, param2);

            switch (midiEvent)
            {
                case NoteOffEvent noff:
                    {
                        OnNoteOffEvent(noff);
                        break;
                    }
                case NoteOnEvent non:
                    {
                        if (non.Velocity == 0)
                            OnNoteOffEvent(new NoteOffEvent(non.NoteNumber, (SevenBitNumber)0) { Channel = non.Channel, DeltaTime = non.DeltaTime });
                        else
                            OnNoteOnEvent(non);
                        break;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="midiEvent"></param>
        protected override void OnNoteOnEvent(NoteOnEvent midiEvent)
        {
            soundManager.KeyOn(midiEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="midiEvent"></param>
        protected override void OnNoteOffEvent(NoteOffEvent midiEvent)
        {
            soundManager.KeyOff(midiEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        private class CM32PSoundManager : SoundManagerBase
        {
            private SoundList<CM32PSound> instOnSounds = new SoundList<CM32PSound>(24);

            private CM32P parentModule;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="parent"></param>
            public CM32PSoundManager(CM32P parent) : base(parent)
            {
                this.parentModule = parent;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="note"></param>
            public override SoundBase SoundOn(NoteOnEvent note)
            {
                int emptySlot = searchEmptySlot(note);
                if (emptySlot < 0)
                    return null;

                var programNumber = (SevenBitNumber)parentModule.ProgramNumbers[note.Channel];
                var timbre = parentModule.Timbres[programNumber];
                CM32PSound snd = new CM32PSound(parentModule, this, timbre, note, emptySlot);
                instOnSounds.Add(snd);

                FormMain.OutputDebugLog("KeyOn ch" + emptySlot + " " + note.ToString());
                snd.KeyOn();

                return snd;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private int searchEmptySlot(NoteOnEvent note)
            {
                int emptySlot = -1;

                var programNumber = (SevenBitNumber)parentModule.ProgramNumbers[note.Channel];
                var timbre = parentModule.Timbres[programNumber];
                emptySlot = SearchEmptySlotAndOff(instOnSounds, note, parentModule.CalcMaxVoiceNumber(note.Channel, 24));
                return emptySlot;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        private class CM32PSound : SoundBase
        {
            private CM32P parentModule;

            private SevenBitNumber programNumber;

            private CM32PTimbre timbre;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="parentModule"></param>
            /// <param name="noteOnEvent"></param>
            /// <param name="programNumber"></param>
            /// <param name="slot"></param>
            public CM32PSound(CM32P parentModule, CM32PSoundManager manager, TimbreBase timbre, NoteOnEvent noteOnEvent, int slot) : base(parentModule, manager, timbre, noteOnEvent, slot)
            {
                this.parentModule = parentModule;
                this.programNumber = (SevenBitNumber)parentModule.ProgramNumbers[noteOnEvent.Channel];
                this.timbre = (CM32PTimbre)timbre;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(NoTypeConverterJsonConverter<CM32PTimbre>))]
        [DataContract]
        public class CM32PTimbre : TimbreBase
        {
            /// <summary>
            /// 
            /// </summary>
            public CM32PTimbre()
            {
                this.SDS.FxS = new BasicFxSettings();
            }

            public override void RestoreFrom(string serializeData)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CM32PTimbre>(serializeData);
                    this.InjectFrom(new LoopInjection(new[] { "SerializeData" }), obj);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(Exception))
                        throw;
                    else if (ex.GetType() == typeof(SystemException))
                        throw;


                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
        }

    }

    public enum SN_U110_Cards : byte
    {
        None,
        C01_PipeOrgan_Harpsichord,
        C02_Latin_FX_Percussions,
        C03_Ethnic,
        C04_Electric_Grand_Clavi,
        C05_Orchestral_Strings,
        C06_Orchestral_Winds,
        C07_Electric_Guitar,
        C08_Synthesizer,
        C09_Guitar_Keyboard,
        C10_Rock_Drums,
        C11_Sound_Effects,
        C12_Sax_Trombone,
        C13_Super_Strings,
        C14_Super_Ac_Guitar,
        C15_Super_Brass
    }


}