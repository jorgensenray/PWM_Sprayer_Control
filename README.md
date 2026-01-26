# PWM_Sprayer_C#_code

Thanks to Matt Elias for the class structured AOG machine code  https://github.com/m-elias/AOG-Machine.  I just added the PWM to it.  Also thanks to whiterose on the AOG forum for the help with curcuits for the PWM driver board.  I accidently found some that work better than what I made, thanks anyway whiterose!

A working, cost effective PWM sprayer.  This is a companion app to Ag Open GPS.  To learn more https://discourse.agopengps.com/  Communication is via UDP between this app and the teensy 4.1.

Features include; -  On/Off valve control  -  PWM valve control  -  Turn compensation  -  Staggered valve control  -  User definable Hz.

The solenoid: If you read far enough it’s rated for 50 million cycles. At 10 Hz running 24 hrs a day that is just under 58 days.  It only draws .4 amps in my testing. I can measure 15 Hz and my non scientific tester is, blow thru it and if you feel wind on the other side it must work at that Hz rate, I quit at 50 Hz.

This AOG compatible controller a Teensy 4.1 W/Ethernet that will get you 24 individually controlled nozzles. (AgOpenGPS is limited to 24 at this time, but I see some activity with more than that) If you need more than that, let’s talk.  You'll need a PWM driver for each valve and a diode.

The negatives would be that the components aren’t designed for ag chemicals and won’t last.  At $6.95 you can replace a valve a bunch of times before you’re even money with a Raven/Capstan or your favorite flavor of valve.

Links to hardware

Solenoid  $6.95 -  https://www.adafruit.com/product/997?gad_source=1&gad_campaignid=21079227318&gbraid=0AAAAADx9JvREtZbzN7VxtLYFz7-A3xMAd&gclid=CjwKCAiAz_DIBhBJEiwAVH2XwC8ni-AIbHYb4CeiB6gdH6J2SGWmQ1-fPDBJdPGucnD4ahceZ4B41BoCmG8QAvD_BwE

Flow meter  $8.99 -  https://www.amazon.com/gp/product/B0CC949527/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1

PWM drver $.83 - https://www.amazon.com/dp/B0FMJH3DML?ref=ppx_yo2ov_dt_b_fed_asin_title

Diode $.20 - suppression at the inductive load - stripe to 12V out + other end to 12V -.    https://www.amazon.com/dp/B0CKSGFQ5T?ref=ppx_yo2ov_dt_b_fed_asin_title&th=1

Teensy 4.1 W/Ethernet  $48.42 -  [https://www.amazon.com/dp/B08RSCFBNF?ref_=ppx_hzsearch_conn_dt_b_fed_asin_title_1](https://www.amazon.com/dp/B08RSCFBNF?ref_=ppx_hzsearch_conn_dt_b_fed_asin_title_3)

Optional - Or add it later - Pressure adjustment from inside cab.

Ball Valve $29.00 -  https://www.amazon.com/dp/B09QC9YBKQ?ref_=ppx_hzsearch_conn_dt_b_fed_asin_title_12&th=1

Cytron Motor driver for Ball valve - $15.50  - https://www.amazon.com/dp/B07CW3GRL6?ref_=ppx_hzsearch_conn_dt_b_fed_asin_title_1

Pressure Sensor  $15.20 -  https://www.amazon.com/Pressure-Transducer-Sender-Sensor-Stainless/dp/B0748BHLQL?pd_rd_w=rgG6A&content-id=amzn1.sym.d7d5d8dd-56a7-4d54-9c0f-9d874f0a0a14&pf_rd_p=d7d5d8dd-56a7-4d54-9c0f-9d874f0a0a14&pf_rd_r=XZ2K3VSHET9GX73V3D8Z&pd_rd_wg=x8E3r&pd_rd_r=d09c411c-8908-4896-a73f-ec0b8fd7a650&pd_rd_i=B0748BHLQL&psc=1&ref_=pd_bap_d_grid_rp_0_1_ec_cp_pd_hp_d_atf_rp_3_i










