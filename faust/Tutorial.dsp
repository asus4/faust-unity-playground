import("stdfaust.lib");

freq = hslider("[1]freq",1000,100,5000,0.1) : si.smoo;
res = hslider("[2]res",1,0,30,0.01) : si.smoo;
gate = button("[3]gate") : si.smoo;

// freq, res and gate definitions go here
string(frequency,resonance,trigger) = trigger : ba.impulsify : fi.fb_fcomb(1024,del,1,resonance)
with{
    del = ma.SR/frequency;
};
process = string(freq,res,gate);