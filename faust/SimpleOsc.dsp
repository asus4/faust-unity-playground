import("stdfaust.lib");

masterGain = hslider("[-1]Master Gain", 1, 0.1, 100, 0.1);

osc(index) = os.osc(freq) * gain
with {
    freq = hslider("[%index]Freq %index", 1000, 50, 20000, 0.1) : si.smoo;
    gain = hslider("[%index]Gain %index", 0.1, 0, 1, 0.001) : si.smoo;
};

channels = 32;
mono = par(i, channels, osc(i)) :> / (channels) * masterGain;
process = mono,mono;

