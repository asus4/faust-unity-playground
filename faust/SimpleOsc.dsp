declare name "Simple OSC";
declare author "Koki Ibukuro";
declare copyright "(c)Koki Ibukuro 2019";

import("stdfaust.lib");

mono = no.noise * hslider("Gain", 0, 0, 1, 0.1);
process = mono,mono;

