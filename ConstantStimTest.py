# -*- coding: utf-8 -*-
"""
Created on Sun Aug  4 10:03:16 2013

@author: Arthur
"""

from ConstantStimulusPsychComponent import * 

# new instance of the Const. Stim. Comp.
Exp = ConstantStimulusPsychComponent()

# this works
Exp.AddStimval(-10)
Exp.AddStimval(-5)
Exp.AddStimval(0)

# this works as well
Exp.AddStimval([5, 10])

# bit of debugging
print Exp._StimulusValues

Exp.AddCondition('Approaching')
Exp.AddCondition('Receding')

print Exp._Conditions

# this works through the setters, only takes integers now
Exp.nIntervals = 1
Exp.nRepetitions = 2

# do the initialisation here
Exp.Initialise()
