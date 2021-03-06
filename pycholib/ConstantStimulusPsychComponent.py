# -*- coding: utf-8 -*-
from Trial import *
import numpy as np

class ConstantStimulusPsychComponent(object):

    def __init__(self, *args, **kwargs):
        ''' Constructor: sets up the variables '''

        # seed the random number generator
        np.random.seed()

        # set up the fields / properties
        self._ExperimentName = ''
        self._StimulusValues = list();
        self._Conditions = list();
        self._TrialList = list();
        self._nIntervals = 0;
        self._nRepetitions = 0;
        self._nTrials = 0;
        self._ActiveTrial = 0;
        self._StartDate = ''
        self._StopDate = ''

        # this is for the logging
        self._WriteOutputFile = ''

    ''' All the properties that need validation on setting '''

    # number of intervals

    @property
    def nIntervals(self):
        return self._nIntervals

    @nIntervals.setter
    def nIntervals(self, value):
        if not isinstance(value, int) or value < 1:
            raise ValueError('Invalid input for number of intervals!')
        self._nIntervals = value;

    # number of repetitions

    @property
    def nRepetitions(self):
        return self._nRepetitions;

    @nRepetitions.setter
    def nRepetitions(self, value):
        if not isinstance(value, int) or value < 1:
            raise ValueError('Invalid input for number of repetitions!')
        self._nRepetitions = value

    ''' Functions to set the conditions and the stimulus values '''
    
    def AddStimval(self, stimval):
        
        # if this is a number (float, long, int, complex) add it. 
        if isinstance(stimval, (int, long, float)) : 
            self._StimulusValues.append(stimval)

        # otherwise if this is a list we add all items to the list
        elif type(stimval) is list:

            # loop through the list
            for item in stimval:

                '''but check if numeric before adding (otherwise 
                    I would have just used .extend() here) '''
                if isinstance(item, (int, long, float)): 
                    self._StimulusValues.append(item)

        else:
            print "ERROR: can only add numbers of lists of numbers to stimulus values!"

    def AddCondition(self, conditionname):

        ''' Now uses a Condition object instead of dict '''

        # get the current number of conditions
        size = len(self._Conditions)

        # we're going to add it as a tuple, with an index and value
        newItem = Condition(conditionname, size+1)

        # add the new item to the list
        self._Conditions.append(newItem)

    ''' Class methods '''

    def Initialise(self):
        ''' Initialise the class - creates list of trials '''

        # determine the total number of trials here first
        self._nTrials = len(self._StimulusValues) * len(self._Conditions) * self._nRepetitions;

        # create a temporary factorial design here
        tmpCond, tmpStim = np.meshgrid(self._Conditions, self._StimulusValues)

        # these vectorise the 2D array and replicates them nRepetitions times
        tmpStim = np.tile(tmpStim.reshape(-1), self._nRepetitions)
        tmpCond = np.tile(tmpCond.reshape(-1), self._nRepetitions)

        # generates random intervals for all trials
        tmpInte = [self.GetRandomInterval() for x in range(self._nTrials)]

        # create the list of trials
        for iTrial in range(self._nTrials):

            # create a new trial using the parameters from the tmp lists
            NewTrial = Trial(trialid = iTrial, condition = tmpCond[iTrial], 
                            stimval = tmpStim[iTrial], interval = tmpInte[iTrial])

            # add this new trial to the trial list
            self._TrialList.append(NewTrial);

        # shuffle the order of the trials pseudo-randomly        
        np.random.shuffle(self._TrialList)

        self.Start()

    def Start(self):

        # this sets the current time
        self._StartDate = self.GetCurrentTime();

    def Stop(self):

        # this sets the current time
        self._StopDate = self.GetCurrentTime();

    def GetNextTrial(self):
    
        # this just returns the next trial
        return self._TrialList[self._ActiveTrial]
        
    def Evaluate(self):
        
        # set the response directly into the trial list
        # self._TrialList[self._ActiveTrial].Response = response;

        # this would normally evaluate a response, here just increments counter
        self._ActiveTrial += 1

    def GetRandomResponse(self):

        # just return a random 
        return np.random.randint(2)

    def GetRandomInterval(self):

        # generates a random interval
        if self._nIntervals > 0:
            return np.random.randint(self._nIntervals, size = 1)

    def isFinished(self):

        # this just checks if we're at the end
        if self._ActiveTrial == self._nTrials:

            # stop logging and set end time
            self._StopDate = self.GetCurrentTime()

            # yes, we are finished
            return True

        else:

            # nope, not finished yet
            return False

    def GetCurrentTime(self):

        # import this just for this function
        import datetime 

        # get the current time
        CurrentTime = datetime.datetime.now().strftime("%a, %b %d %Y %T")

        # print it to the console, just for debugging
        print 'Current time is: ', CurrentTime;

        # return it
        return CurrentTime;



    ''' Below are the functions for the logging '''


class Condition(object):
    def __init__(self, name=None, index=None):
        self._Name = name
        self._Index = index

    @property 
    def Name(self):
        return self._Name

    @Name.setter
    def Name(self, value):
        self._Name = value;

    @property 
    def Index(self):
        return self._Index

    @Index.setter
    def Index(self, value):
        self._Index = value;
