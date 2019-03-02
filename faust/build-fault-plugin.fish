#!/usr/local/bin/fish

set -x ANDROID_HOME $HOME/Library/Android/sdk

# faust2unity -android -ios -osx -nvoices 16 SimpleOsc.dsp
faust2unity -android -ios -osx SimpleOsc.dsp
