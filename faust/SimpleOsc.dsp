import("stdfaust.lib");


osc(index) = os.osc(freq) * gain
with {
    freq = hslider("[%index]Freq %index", 1000, 50, 20000, 0.1) : si.smoo;
    gain = hslider("[%index]Gain %index", 0.1, 0, 1, 0.01) : si.smoo;
};

channels = 16;
mono = par(i, channels, osc(i)) :> / (channels);
process = mono,mono;

