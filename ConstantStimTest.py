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

# this just adds two conditions, by name!!!
Exp.AddCondition('Approaching')
Exp.AddCondition('Receding')

# this works through the setters, only takes integers now
Exp.nIntervals = 1
Exp.nRepetitions = 2

# do the initialisation here
Exp.Initialise()

# -- this is where the actual experimental loop starts

# cycle though until finished
while not Exp.isFinished():

    # get the parameters for this trial
    trial = Exp.GetNextTrial()

    # just print the parameters (needs to be a bit nicer)
    trial.GetTrialData()

    # set the response
    trial.Response = 1

    # evaluate the trial (i.e. increment trial number)
    Exp.Evaluate(trial)

