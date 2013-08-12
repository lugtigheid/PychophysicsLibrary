import random
from Trial import *

class StaircasePsychComponent ( object ):

    def __init__(self):

        self._ActiveStairsList = list()
        self._FinishedStairList = list()
        self._CurrentStaircaseID = 0;   # this refers to the _ActiveStairsList index
        self._TotalTrials = 0;

    def Initialise(self):

        # just set some values
        start = [0,100];
        stepsize = 2;
        minboundary = 0;
        maxboundary = 100;

    '''
        def init()
    --------------------------
    
    - global trial counter = 0
    - get the number of staircases we're initialising
    - set active staircases
    - for each staircase, set the parameters for each
 
    def Update()
    --------------------------
    
    - create a new line with the data
    - add those data to the data within the staircase
        - However, I think the data should be saved centrally in a trial list
    - Do the staircase tarmination rule here:
        1. current trial is larger than max trials (is this total or just for this staircase?)
        2. max reversals has been reached
        3. max boundaries was hit
    -- check global termination rule:
        1. done = ~numel(sc.active)
        there's a function to remove terminated staircases





    '''




        # set up the staircases for now
        self._ActiveStairsList = [Staircase(staircaseID=x, initial=start[x]) for x in range(2)]

    def SelectRandomStaircase(self):

        # how many are left in the active list?
        tmpRange = len(self._ActiveStairsList)

        # get a random number within that range
        self._CurrentStaircaseID = random.randint(0, tmpRange-1)

        # return a random staircase from the active stairslist
        return self._ActiveStairsList[self._CurrentStaircaseID];

    def GetCurrentStaircase(self):
        return self._ActiveStairsList[self._CurrentStaircaseID];

    def DeactivateStaircase(self, id):

        # obtain the staircase to be deleted
        tmpStair = self._ActiveStairsList[id];

        # put this in the finished list
        self._FinishedStairList.append(tmpStair);

        print 'Removing #', self._ActiveStairsList[id]._StaircaseIndex

        # remove it from the active list
        self._ActiveStairsList.pop(id);


    def GetStimval(self):
    
    '''         
    returns a new stimulus value on the basis of the previous trial

    - get current stimulus value and direction (+ or -)
    - determine the number of reversals for this staircase
    - if number of trials is larger than one, do this, otherwise _InitialStimval

    - determine steptype:
        FIXED:
            if we're not on the last item of the _FixedStepsizes array:
                the index is equal to the number of reversals
            otherwise just use the last item in the list (-1)
            extract the stepsize from _FixedStepsizes [index]
            calculate stimval: stimval+direction*stepsize;

        RANDOM: 
            stepindex is random number in the list
            do the same as above ^-

    - code that checks the out of _OutOfBoundaryCount:
        if stimval smaller than mimimum stimval: 
            increment the hit boundaries counter for this staircase
            set the stimulus value to the minimum value (no change)
        if larger than the maximum stimval:
            increment the hit boundaries counter for this staircase:
            set the stimulus value to the maximum value (no change)
    
    return stimval
    '''


        pass





    def GetNextTrial(self):
        
    '''
    def NewTrial()
    --------------------------

        - select a random trial from the active staircases -> becomes current
        - increment the trial count for current staircase
        - increment the total trial count
        - get the stimulus value ( sc = get stimulusval(sc) )
        - set the trial stimval and trial number. 
        - return these. 
        '''


        ''' Gets a random staircase from the active staircases and gets 
            new trial parameters. Returns a trial and sets current stair id. '''

        # select one staircase from the list
        self._CurrentStair = self.SelectRandomStaircase();

        # increment the trial counter
        self._CurrentStair.IncrementTrial()

        # increment the total trial count
        self._TotalTrials += 1;

        # new trial instance
        t = Trial(trialid = self._TotalTrials, 
                  staircaseid = self._CurrentStair._StaircaseIndex,
                  condition = self._CurrentStair._Condition, 
                  stimval = self._CurrentStair._CurrentStimval,
                  interval = self._CurrentStair._Interval);

        # print all trial parameters
        print t;

        # return the trial
        return t;

    def GetStimVal(self):
        pass

    def Update(self):
        pass

    def EvaluateTrial(self, trial):
    
    '''
    --------------------------
    Evaluates the response and preps the stimval of the stiarcase

    sets the direction to 0 as a default
    check the response (coded as either 0 or 1)

    case 0:
        increase the number of incorrect answer count
        if staircase up count is one (not sure why this is) OR mod
        (_CurrentStair.wrong, _CurrentStair.up) == 0:

            This means there's a reversal, so save that stimval (if right >= down) 
            reset the correct counter
            set the direction to 1

    case 1:
        increase the number of correct answer count
        if staircase down count is one (not sure why this is) or mod
        (_CurrentStair.right, _CurrentStair.down) == 0:

            That's a reversal! Save it (if wrong >= up)
            reset the wrong counter
            set the direction to -1

    otherwise Raise exception.

    update() 

    '''

        # TODO: I don't actually think the trial parameter is needed.

        # store the current staircase
        cs = self.GetCurrentStaircase()

        # bit of debugging
        print cs._TrialNum, cs._MaxTrials, len(self._ActiveStairsList);

        # this is staircase termination rule #1
        if cs._TrialNum < cs._MaxTrials:
            pass;
        else:
            self.DeactivateStaircase(self._CurrentStaircaseID);

    def GetRemainingTrials(self):
        # return a count of the total number of trials for logging
        # if _ActiveStairsList's length is > 0, this module has been initalized
        trial_sum = 0
        if len(self._ActiveStairsList) > 0:
            for staircase in self._ActiveStairsList:
                trial_sum += staircase.GetTrialCount()
            return trial_sum
        else:
            print("WARNING: StaircasePsychComponent contains no active staircases. Has it been initialised?")
            return None

    def isFinished(self):

        # this is the termination rule: no more active staircases
        return len(self._ActiveStairsList) == 0


class Staircase ( object ):

    def __init__(self, staircaseID=0, steptype='fixed', condition=0, interval=0, 
                 initial=0, minboundary=0, maxboundary=100, stepsize=1):

        self._StepType = steptype;
        self._MinBoundary = minboundary;
        self._MaxBoundary = maxboundary;
        self._FixedStepsizes = stepsize;
        self._nUp = 1;
        self._nDown = 1;

        self._Condition = condition;
        self._Interval = interval;
        self._InitialStimval = initial;
        self._CurrentStimval = initial;
 
        self._Reversals = 0; # don't need reversal count - just return len(self._Reversals)
        self._OutOfBoundaryCount = 0;

        self._RightSinceWrong = 0;
        self._WrongSinceRight = 0;
        self._direction = 0;
        self._TrialNum = 0;
        self._Status = 0;
        
        self._StaircaseIndex = staircaseID;

        # these two determine the termination rule
        self._MaxTrials = 50;
        self._MaxReversals = 13;
    
    def GetTrialCount(self):
        return self._MaxTrials
    
    def IncrementTrial(self):
        self._TrialNum += 1

    def isValid(self):
    
        # this should check that the values here are all valid for use
        pass