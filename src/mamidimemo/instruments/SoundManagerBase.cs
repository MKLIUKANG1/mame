﻿// copyright-holders:K.Ito
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Smf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zanac.MAmidiMEmo.ComponentModel;

namespace zanac.MAmidiMEmo.Instruments
{

    /// <summary>
    /// 
    /// </summary>
    public class SoundManagerBase : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected List<SoundBase> AllOnSounds
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public SoundManagerBase()
        {
            AllOnSounds = new List<SoundBase>();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            for (int i = AllOnSounds.Count - 1; i >= 0; i--)
            {
                var removed = AllOnSounds[i];
                AllOnSounds.RemoveAt(i);
                removed.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="midiEvent"></param>
        public virtual void PitchBend(PitchBendEvent midiEvent)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        public virtual void ControlChange(ControlChangeEvent midiEvent)
        {
            switch (midiEvent.ControlNumber)
            {
                case 120:   //All Sounds Off
                case 123:   //All Note Off
                    {
                        foreach (var snd in new List<SoundBase>(AllOnSounds))
                        {
                            var noff = new NoteOffEvent(snd.NoteOnEvent.NoteNumber, (SevenBitNumber)0) { Channel = snd.NoteOnEvent.Channel };
                            NoteOff(noff);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        public virtual void NoteOn(NoteOnEvent note)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        public virtual SoundBase NoteOff(NoteOffEvent note)
        {
            return SearchAndRemoveOnSound(note, AllOnSounds);
        }

        protected virtual int SearchEmptySlot(List<SoundBase> onSounds, NoteOnEvent newNote, int maxSlot)
        {
            return SearchEmptySlot(onSounds, newNote, maxSlot, false);
        }

        /// <summary>
        /// 未使用のスロットを検索する
        /// 空が無い場合は最初に鳴った音を消してそこを再利用する
        /// </summary>
        /// <param name="onSounds"></param>
        /// <param name="maxSlot"></param>
        /// <returns></returns>
        protected virtual int SearchEmptySlot(List<SoundBase> onSounds, NoteOnEvent newNote, int maxSlot, bool reversSlot)
        {
            int emptySlot = -1;

            //未使用のスロットを検索する
            if (!reversSlot)
            {
                for (int i = 0; i < maxSlot; i++)
                {
                    bool found = false;
                    foreach (var snd in onSounds)
                    {
                        if (snd.NoteOnEvent.NoteNumber == newNote.NoteNumber)
                            return -1;

                        if (snd.Slot == i)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        emptySlot = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = maxSlot - 1; i >= 0; i--)
                {
                    bool found = false;
                    foreach (var snd in onSounds)
                    {
                        if (snd.Slot == i)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        emptySlot = i;
                        break;
                    }
                }
            }

            //空が無いので最初に鳴った音を消してそこを再利用する
            if (emptySlot < 0)
            {
                var snd = onSounds[0];
                emptySlot = snd.Slot;

                var noff = new NoteOffEvent(snd.NoteOnEvent.NoteNumber, (SevenBitNumber)0) { Channel = snd.NoteOnEvent.Channel };
                NoteOff(noff);
            }
            return emptySlot;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        /// <param name="onOnSounds"></param>
        /// <returns></returns>
        protected static T SearchAndRemoveOnSound<T>(NoteOffEvent note, List<T> onOnSounds) where T : SoundBase
        {
            T removed = null;
            for (int i = 0; i < onOnSounds.Count; i++)
            {
                if (onOnSounds[i].NoteOnEvent.Channel == note.Channel)
                {
                    if (onOnSounds[i].NoteOnEvent.NoteNumber == note.NoteNumber)
                    {
                        removed = onOnSounds[i];
                        onOnSounds.RemoveAt(i);
                        removed.Dispose();
                        break;
                    }
                }
            }

            return removed;
        }
    }
}
