/*************************************************************************

    Atari Canyon Bomber hardware

*************************************************************************/

#include "sound/discrete.h"

/* Discrete Sound Input Nodes */
#define CANYON_MOTOR1_DATA		NODE_01
#define CANYON_MOTOR2_DATA		NODE_02
#define CANYON_EXPLODE_DATA		NODE_03
#define CANYON_WHISTLE1_EN		NODE_04
#define CANYON_WHISTLE2_EN		NODE_05
#define CANYON_ATTRACT1_EN		NODE_06
#define CANYON_ATTRACT2_EN		NODE_07



class canyon_state : public driver_device
{
public:
	canyon_state(const machine_config &mconfig, device_type type, const char *tag)
		: driver_device(mconfig, type, tag) ,
		m_videoram(*this, "videoram"){ }

	/* memory pointers */
	required_shared_ptr<UINT8> m_videoram;

	/* video-related */
	tilemap_t  *m_bg_tilemap;
	DECLARE_READ8_MEMBER(canyon_switches_r);
	DECLARE_READ8_MEMBER(canyon_options_r);
	DECLARE_WRITE8_MEMBER(canyon_led_w);
	DECLARE_WRITE8_MEMBER(canyon_videoram_w);
	TILE_GET_INFO_MEMBER(get_bg_tile_info);
	virtual void video_start();
	virtual void palette_init();
	UINT32 screen_update_canyon(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
};


/*----------- defined in audio/canyon.c -----------*/

DECLARE_WRITE8_DEVICE_HANDLER( canyon_motor_w );
DECLARE_WRITE8_DEVICE_HANDLER( canyon_explode_w );
DECLARE_WRITE8_DEVICE_HANDLER( canyon_attract_w );
DECLARE_WRITE8_DEVICE_HANDLER( canyon_whistle_w );

DISCRETE_SOUND_EXTERN( canyon );


/*----------- defined in video/canyon.c -----------*/




