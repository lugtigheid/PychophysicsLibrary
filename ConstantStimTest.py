# -*- coding: utf-8 -*-
"""
Created on Sun Aug  4 10:03:16 2013
@author: Arthur
"""

from ConstantStimulusPsychComponent import * 

# new instance of the Const. Stim. Comp.
Exp = ConstantStimulusPsychComponent()

Exp.ExperimentName = 'Simple test experiment for ConstantStimulusPsychComponent'

# this works
Exp.AddStimval(-10)
Exp.AddStimval(-5)
Exp.AddStimval(0)

# this works as well
Exp.AddStimval([5, 10])

# this just adds two conditions, by name!!!
Exp.AddCondition('Approach')
Exp.AddCondition('Recede')

# this works through the setters, only takes integers now
Exp.nIntervals = 2
Exp.nRepetitions = 2

# do the initialisation here
Exp.Initialise()

# -- this is where the actual experimental loop starts

# cycle though until finished
while not Exp.isFinished():

    # get the parameters for this trial
    trial = Exp.GetNextTrial()

    ''' This is where you would normally put the code to show your stimuli. 
        It would be awesome if we could do this part using a workflow idea. '''

    # set the response - this should be either a choice answer (e.g. for nAFC) or 
    # it should be a float/double if it's a metric depth estimate (e.g. with ruler)
    
    # at the moment, this sets the response DIRECTLY in the Exp._TrialList items
    # I am not entirely sure whether or not this is a good implementation for Python

    trial.Response = Exp.GetRandomResponse()+1
    trial.ReactionTime = 0.01

    # just print the parameters (needs to be a bit nicer).
    # does not yet include the condition, due to me being a tard.
    print trial

    # evaluate the trial (i.e. increment trial number)
    Exp.Evaluate()

