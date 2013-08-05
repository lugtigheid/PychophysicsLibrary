from Trial import *
from random import *
from numpy import *

class ConstantStimulusPsychComponent(object):

    def __init__(self, *args, **kwargs):
        ''' Constructor: sets up the variables '''

        # for now, it seems like there's a problem with the random seed:
        # doesn't look like it changes at all across instantiations. 
        random.seed()

        self._StimulusValues = list();
        self._Conditions = list();
        self._TrialList = list();
        self._nIntervals = 0;
        self._nRepetitions = 0;
        self._nTrials = 0;
        self._ActiveTrial = 0;

    ''' All the properties that need validation on setting '''

    @property
    def nIntervals(self):
        return self._nIntervals

    @nIntervals.setter
    def nIntervals(self, value):
        if not isinstance(value, int) or value < 1:
            raise ValueError('Invalid input for number of intervals!')
        self._nIntervals = value;

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

    def AddCondition(self, condition):

        ''' This function adds a condition to the conditions list. I implemented 
            this so that it fills a tuple, but I am not yet sure that it needs a 
            method like this - it could just be the index of the list that could 
            be used as the index - maybe convert to a getter / setter construct'''

        # get the current number of conditions
        size = len(self._Conditions)

        # we're going to add it as a tuple, with an index and value
        newItem = {size+1: condition};

        # add the new item to the list
        self._Conditions.append(newItem)

    ''' Class methods '''

    def Initialise(self):
        ''' Initialise the class - creates list of trials '''

        # determine the total number of trials here first
        self._nTrials = len(self._StimulusValues) * len(self._Conditions) * self._nRepetitions;

        # this won't work at the moment - the conditions are in a dictionary

        # create a temporary factorial design here
        tmpCond, tmpStim = meshgrid(self._Conditions, self._StimulusValues)

        # these vectorise the 2D array and replicates them nRepetitions times
        tmpStim = tile(tmpStim.reshape(-1), self._nRepetitions)
        tmpCond = tile(tmpCond.reshape(-1), self._nRepetitions)
        tmpInte = [random.randint(self._nIntervals) for x in range(self._nTrials)]

        # this creates a random permutation for the trial order
        r = random.permutation(self._nTrials)

        # create the list of trials
        for iTrial in range(self._nTrials):

            NewTrial = Trial(trialid = iTrial, condition = tmpCond[iTrial], 
                            stimval = tmpStim[iTrial], interval = tmpInte[iTrial])

            self._TrialList.append(NewTrial);

        print self._TrialList[9]._Condition

    def GetNextTrial(self):
        trial = Trial();
        return trial;

    def Evaluate(self, trial):
        pass

    def GetRandomInterval(self):
        if self._nIntervals > 0:
            return random.randint(1,self._nIntervals)

    def isFinished(self):
        pass

