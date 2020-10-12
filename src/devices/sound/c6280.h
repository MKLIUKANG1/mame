// license:BSD-3-Clause
// copyright-holders:Charles MacDonald
#ifndef MAME_SOUND_C6280_H
#define MAME_SOUND_C6280_H

#pragma once

#include "vgmwrite.h"

typedef int8_t(*C6280_PCM_CALLBACK)();

class c6280_device : public device_t, public device_sound_interface
{
public:
	static constexpr feature_type imperfect_features() { return feature::SOUND; } // Incorrect / Not verified noise / LFO output

	c6280_device(const machine_config &mconfig, const char *tag, device_t *owner, u32 clock);

	// write only
	void c6280_w(offs_t offset, uint8_t data);

	void vgm_start(char *name);
	void vgm_stop(void);

	void set_pcm_callback(C6280_PCM_CALLBACK callback) { m_callback = callback; };

protected:
	// device-level overrides
	virtual void device_start() override;
	virtual void device_reset() override;
	virtual void device_clock_changed() override;

	// sound stream update overrides
	virtual void sound_stream_update(sound_stream &stream, stream_sample_t **inputs, stream_sample_t **outputs, int samples) override;

private:
	struct channel {
		u16 frequency;
		u8 control;
		u8 balance;
		u8 waveform[32];
		u8 index;
		s16 dda;
		u8 noise_control;
		s32 noise_counter;
		u32 noise_frequency;
		u32 noise_seed;
		s32 tick;
	};

	// internal state
	sound_stream *m_stream;
	u8 m_select;
	u8 m_balance;
	u8 m_lfo_frequency;
	u8 m_lfo_control;
	channel m_channel[8];
	s16 m_volume_table[32];

	vgm_writer *m_vgm_writer;

	emu_timer *m_timer;
	TIMER_CALLBACK_MEMBER(timer_callback);

	C6280_PCM_CALLBACK m_callback;
};

DECLARE_DEVICE_TYPE(C6280, c6280_device)

#endif // MAME_SOUND_C6280_H
