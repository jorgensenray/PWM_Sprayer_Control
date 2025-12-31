# PWM_Sprayer_Teensy_code

Thanks to Matt Elias for the class structured AOG machine code  https://github.com/m-elias/AOG-Machine.  I just added the PWM to it.  Also thanks to whiterose on the AOG forum for the help with curcuits for the PWM driver board.

A working, cost effective PWM sprayer.  This is a companion app to Ag Open GPS.  To learn more https://discourse.agopengps.com/  Communication is via UDP between this app and the teensy 4.1.

Features include; -  On/Off valve control  -  PWM valve control  -  Turn compensation  -  Staggered valve control  -  User definable Hz up to 15 and possibly higher.

The solenoid: If you read far enough it’s rated for 50 million cycles. At 10 Hz running 24 hrs a day that is just under 58 days.  It only draws .4 amps in my testing. I can measure 15 Hz and I’m sure it will cycle faster.

Optional but recommended is an INA219 current sensor.

This AOG compatible controller will cost $40 for a Teensy 4.1 W/Ethernet that will get you 34 individually controlled nozzles. (AgOpenGPS is limetted to 24 at this time, but I see some activity with more than that) If you need more than that, let’s talk.  You'll need a PCB or just make one with perfboard.

So you’re just shy of $100 in these parts.

You need a PWM driver for each valve at a cost of around $10.  (Digikey thru hole parts). or JLC SMD components - assembled for $10.96 (I just ordered some and that's my cost w/shipping). 

The negatives would be that the components aren’t designed for ag chemicals and won’t last.  At $6.95 you can replace a valve a bunch of times before you’re even money with a Raven/Capstan or your favorite flavor of valve.

Links to hardware

Solenoid  -  https://www.adafruit.com/product/997?gad_source=1&gad_campaignid=21079227318&gbraid=0AAAAADx9JvREtZbzN7VxtLYFz7-A3xMAd&gclid=CjwKCAiAz_DIBhBJEiwAVH2XwC8ni-AIbHYb4CeiB6gdH6J2SGWmQ1-fPDBJdPGucnD4ahceZ4B41BoCmG8QAvD_BwE

Flow meter  -  https://www.amazon.com/gp/product/B0CC949527/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1

Pressure Sensor  -  https://www.amazon.com/Pressure-Transducer-Sender-Sensor-Stainless/dp/B0748BHLQL?pd_rd_w=rgG6A&content-id=amzn1.sym.d7d5d8dd-56a7-4d54-9c0f-9d874f0a0a14&pf_rd_p=d7d5d8dd-56a7-4d54-9c0f-9d874f0a0a14&pf_rd_r=XZ2K3VSHET9GX73V3D8Z&pd_rd_wg=x8E3r&pd_rd_r=d09c411c-8908-4896-a73f-ec0b8fd7a650&pd_rd_i=B0748BHLQL&psc=1&ref_=pd_bap_d_grid_rp_0_1_ec_cp_pd_hp_d_atf_rp_3_i

Current Sensor  -  https://www.amazon.com/dp/B07VL8NY32?ref_=ppx_hzsearch_conn_dt_b_fed_asin_title_5

Teensy 4.1 W/Ethernet  -  https://www.amazon.com/dp/B08RSCFBNF?ref_=ppx_hzsearch_conn_dt_b_fed_asin_title_1








