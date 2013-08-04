# -*- coding: utf-8 -*-
"""
Created on Sun Aug  4 10:03:16 2013

@author: Arthur
"""

from ConstantStimulusPsychComponent import * 

Exp = ConstantStimulusPsychComponent()

# this works
Exp.AddStimval(-10)
Exp.AddStimval(-5)
Exp.AddStimval(0)
Exp.AddStimval(5)
Exp.AddStimval(10)

# this works as well
Exp.AddStimval([3, 4, 9])

# bit of debugging
print Exp._StimulusValues