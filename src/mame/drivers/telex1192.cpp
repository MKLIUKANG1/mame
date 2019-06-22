// license:BSD-3-Clause
// copyright-holders:AJR
/************************************************************************************************************

    Skeleton driver for Memorex Telex 1192 IBM-compatible video display terminal.

************************************************************************************************************/

#include "emu.h"
#include "cpu/bcp/dp8344.h"
//#include "machine/nvram.h"
//#include "screen.h"

class telex1192_state : public driver_device
{
public:
	telex1192_state(const machine_config &mconfig, device_type type, const char *tag)
		: driver_device(mconfig, type, tag)
		, m_bcp(*this, "bcp")
		, m_chargen(*this, "chargen")
	{
	}

	void telex1192(machine_config &config);

private:
	void inst_map(address_map &map);
	void data_map(address_map &map);

	required_device<dp8344_device> m_bcp;
	required_region_ptr<u8> m_chargen;
};


void telex1192_state::inst_map(address_map &map)
{
	map(0x0000, 0x1fff).rom().region("program", 0);
}

void telex1192_state::data_map(address_map &map)
{
}


static INPUT_PORTS_START(telex1192)
INPUT_PORTS_END


void telex1192_state::telex1192(machine_config &config)
{
	DP8344(config, m_bcp, 18.8696_MHz_XTAL);
	m_bcp->set_addrmap(AS_PROGRAM, &telex1192_state::inst_map);
	m_bcp->set_addrmap(AS_DATA, &telex1192_state::data_map);
}


// XTALs: 18.8696 (Y1), 6.1440 MHz (Y2), 46.2 MHz (Y3), 24.8443 MHz (Y4), 28.3046 MHz (Y5), 40.993430 MHz (Y6)
// CPU: NS DP8344AV BCP
// Gate array: Toshiba TC110G17AT
// RAM: 3x Motorola MCM6264P45, Toshiba TC55257APL-10, Dallas DS1220Y-150
ROM_START(telex1192)
	ROM_REGION16_LE(0x4000, "program", 0)
	ROM_LOAD16_BYTE("206252-003.u7", 0x0000, 0x2000, CRC(3632b762) SHA1(a45dbcd96485ad87c60257e95dc58f31d1e4ddee))
	ROM_LOAD16_BYTE("206251-003.u8", 0x0001, 0x2000, CRC(2852d830) SHA1(947612957daa441596a547f23c526d20306e6e0d))

	ROM_REGION(0x10000, "chargen", 0)
	ROM_LOAD("206253-003.u6", 0x00000, 0x10000, CRC(8d7d9356) SHA1(0b2eda8b04b2a6651f4fbf4cd6cdd129a70110ee))
ROM_END

COMP(1989, telex1192, 0, 0, telex1192, telex1192, telex1192_state, empty_init, "Memorex Telex", "Telex 1192", MACHINE_IS_SKELETON)
