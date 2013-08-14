import random
from Trial import * # not quite how it should be

class StaircasePsychComponent ( object ):

    def __init__(self):

        self._ActiveStairsList = list()
        self._FinishedStairList = list()
        self._CurrentStaircaseID = 0;   # this refers to the _ActiveStairsList index
        self._TotalTrials = 0;

    def Initialise(self):

        # just set some values
        start = [0,100];
        fixedstepsize = [5,4,3,2,1];
        minboundary = 0;
        maxboundary = 100;

        '''
            def init()
        --------------------------
        
        - global trial counter = 0
        - get the number of staircases we're initialising
        - set active staircases
        - for each staircase, set the parameters for each
     
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


    def GetNextTrial(self):

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
                  interval = self._CurrentStair._nIntervals);

        # print all trial parameters
        print t;

        # return the trial
        return t;

    def Update(self):

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

    def __init__(self, staircaseID=0, steptype='fixed', condition=0, 
                 interval=0, initial=0, minboundary=0, maxboundary=100, 
                 nUp=1, nDown=1, fixedstepsize=0):

        self._MinBoundary = minboundary;
        self._MaxBoundary = maxboundary;
        self._OutOfBoundaryCount = 0;

        self._StepType = steptype;
        self._FixedStepsizes = fixedstepsize;
        self._nUp = 1;
        self._nDown = 1;

        self._Condition = condition;
        self._nIntervals = interval;
        self._InitialStimval = initial;
        self._CurrentStimval = initial;

        self._Right = 0;
        self._Wrong = 0;
        self._Reversals = list();
        self._direction = 0;
        self._TrialNum = 0;
        
        self._StaircaseIndex = staircaseID;

        # these two determine the termination rule
        self._MaxTrials = 50;
        self._MaxReversals = 13;
    

    ''' -- Properties, some of which are "dynamic" -- '''

    @property
    def NumReversals(self):
        return len(self._Reversals)

    @property
    def NumStepSizes(self):
        return len(self._FixedStepsizes)


    ''' -- The meat of this class -- '''

    def NewTrial(self):

        ''' Gets a new trial based on previous parameters '''

        # increment the trial counter
        self.IncrementTrial()

        # self.SetStimval()

        # new trial instance
        return Trial(trialid = self._TotalTrials, 
                  staircaseid = self._StaircaseIndex,
                  condition = self._Condition, 
                  stimval = self._CurrentStimval,
                  interval = self._Interval);


    def SetStimval(self):
    
        '''         
        sets a new stimulus value in _CurrentStimval
        '''
  
        # only do this if we're past trial one
        if self._TrialNum > 1:

            # make sure the steptype is a lowercase string
            steptype = self._StepType.lowercase()

            # first determine the 
            if steptype == 'fixed':

                # if we're not at the end of the list, otherwise we just
                # choose the last item of the _FixedStepsizes list.
                if self.NumReversals < self.NumStepSizes:
                    stepindex = self.NumReversals;
                else:
                    stepindex = self.NumStepSizes;

                # get the stepsize from the _FixedStepsizes list
                stepsize = self._FixedStepsizes[stepindex];

            elif steptype == 'random':

                # choose a random step number
                stepsize = self.GetRandomStepsize();

            else:
                raise ValueError('Invalid step step type (Fixed|Random)')

            # now calculate the stimulus value 
            stimval = self._CurrentStimval + (self._direction * stepsize)

        else:

            # otherwise just return the initial stimulus value
            stimval = self._InitialStimval;

        # this checks this staircase against the _Min/_MaxBoundary
        # if either of these are true, increment the _OutOfBoundaryCount
        # and set the stimulus value to the _Min/_MaxBoundary
        if stimval < self._MinBoundary:

            # increment the _OutOfBoundaryCount
            self._OutOfBoundaryCount += 1;

            # set the stimval to the _MinBoundary
            stimval = self._MinBoundary;

        # here we hit the upper limit
        if stimval > self._MaxBoundary:

            # increment the _OutOfBoundaryCount
            self._OutOfBoundaryCount += 1;

            # set the stimval to the _MaxBoundary
            stimval = self._MaxBoundary;

        # set the current stimulus value here
        self._CurrentStimval = stimval;



    def EvaluateTrial(self, response):
    
        '''
        Evaluates the response
        '''

        # set a default value for the direction
        self._direction = 0;

        # "wrong" answer
        if response == 0:
            
            self._nWrong += 1

            # this demarcates a reversal, so we save it and reset
            # everything % 1 evaluates to true, so we need this
            if self._nUp == 1 or self._nWrong % self._nUp == 0:

                if self._nRight >= self._nDown:
                    
                        self._Reversals.append(self._CurrentStimval)

                # reset the counter
                self._Right = 0;

                # set the step direction to up (+1)
                self._direction = 1
       
        # "correct" answer
        elif response == 1:

            self._nRight += 1

            # this demarcates a reversal, so we save it and reset
            # everything % 1 evaluates to true, so we need this
            if self._nDown == 1 or self._nRight % self._nDown == 0:

                if self._nWrong >= self._nUp:
                    
                    self._Reversals.append(self._CurrentStimval)

                # reset the counter
                self._nWrong = 0;

                # set the step direction to up (+1)
                self._direction = -1

        else:
            raise ValueError('Incorrect response: needs to be 0 or 1')


    def Update(self):


        '''
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




    ''' -- Utility functions --- '''

    def GetRandomStepsize(self):
        '''Returns a random stepsize from the _FixedStepsizes list'''
        return self._FixedStepsizes[random.randint(1,self.NumStepSizes)];

    def GetTrialCount(self):
        return self._MaxTrials
    
    def IncrementTrial(self):
        self._TrialNum += 1

    def isValid(self):
    
        # this should check that the values here are all valid for use
        pass