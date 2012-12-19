PLCNetLib
===========

This repository consists of three projects.

1) ENDAPLCNetLib
-----------------

This is the library that you can use to discover ENDA PLC devices on the network and talk to them in a very straight forward manner.

	PLC plc = new PLC(ip, pass);
	if(plc.IP[0])
		plc.MI[0] = 42;

2) PLCNetLibDemo
----------------
Demonstration of almost all features of the PLCNetLib library.

3) plcctrl
-------------
A handy tool I use in my Makefiles to command firmware of the devices on the network. Could be used in automations.

