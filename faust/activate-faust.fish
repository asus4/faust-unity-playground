#!/usr/local/bin/fish

# set path each session
# set -x PATH (echo $HOME)/Applications/Faust-2.14.4/bin $PATH

# set user path
set -U fish_user_paths (echo $HOME)/Applications/Faust-2.14.4/bin $fish_user_paths

# remove path from index --- NOTE: fish array start from 1
# set -e -U fish_user_paths[1]