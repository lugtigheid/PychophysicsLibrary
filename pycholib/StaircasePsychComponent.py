# -*- coding: utf-8 -*-

from Trial import * # not quite how it should be
import numpy as np
import scipy.stats as sp
import matplotlib.pyplot as plt

class StaircasePsychComponent ( object ):

    def __init__(self):

        ''' -- the internal variables -- '''
        self._TrialList = list()            # contains all trials
        self._ActiveStairsList = list()     # contains active staircases
        self._FinishedStairList = list()    # contains 'terminated' staircases
        self._CurrentStaircaseID = 0;       # this refers to the _ActiveStairsList index
        self._TotalTrials = 0;              # we won't really need this
        
        ''' -- these are simulation variables -- '''
        self._mu = 50;
        self._sg = 10;

    # empty for now
    def Start(self):
        pass

    def Stop(self):
        pass

    def AddStair(self, stair):

        # set the correct stair id
        stair._StaircaseIndex = len(self._ActiveStairsList)

        # now add it to the list
        self._ActiveStairsList.append(stair)

    ''' --- Properties --- '''

    @property
    def nActiveStairs(self):
        return len(self._ActiveStairsList)

    ''' --- Methods ---'''

    def GetNextTrial(self):

        ''' Gets a random staircase from the active staircases and gets 
            new trial parameters. Returns a trial and sets current stair id. '''

        # select one staircase from the list
        self._CurrentStair = self.SelectRandomStaircase();

        # increment the total trial count
        self._TotalTrials += 1;

        # new trial instance and return the fucker
        return self._CurrentStair.NewTrial()

    def ShowResults(self):

        ''' -- Plots the data ''' 

        # pre-define the list that will hold means
        stairMeans = []; 

        for iStairs in range(len(self._FinishedStairList)):

            # only get the values for a single staircase (iStairs.ID)
            data = [trial for trial in self._TrialList if trial._StaircaseID == iStairs]

            print iStairs;

            # get the stats for each staircase
            mu,sg = self._FinishedStairList[iStairs].GetStats()

            # save the mu for later reference
            stairMeans.append(mu)

            # create a subplot (left)
            plt.subplot(1,2,1)

            # plot the mu value
            plt.axhline(y=mu, color='r', ls='--')

            # extract the x and y values for the plot
            x = [t._TrialID for t in data]
            y = [t._Stimval for t in data]

            # plot the actual staircase
            plt.plot(x,y, color='k', marker='.', ls='-')


        # show the actual mean of the sampling distribution
        plt.axhline(y=self._mu,color='g',ls='dashed')
        plt.axhline(y=np.mean(stairMeans), color='r')

        # this provides and extra plot for reference
        plt.subplot(1,2,2)

        # plot the psychometric function used in the simulation
        x = np.linspace(0, 100, 1000)
        y = sp.norm.cdf(x, loc=self._mu, scale=self._sg)
        plt.plot(x,y, color='k')

        # plot some lines to show the thresholds
        [plt.axvline(x=xvar, color='r', ls='--') for xvar in stairMeans]

        # and the mean
        plt.axvline(x=np.mean(stairMeans), color='r')

        # and now plot the position on the y-axis
        plt.axhline(y=sp.norm.cdf(np.mean(stairMeans), 
            loc=self._mu, scale=self._sg), ls='--', color='0.75')

         # set the boundaries of the y-axis
        # plt.ylim([self._MinBoundary,self._MaxBoundary])

        # show the plot
        plt.show()

        # save these results to a pdf
        # plt.savefig('plot.pdf')


    def EvaluateTrial(self, trial):

        # this saves the trial into the trial list
        self._TrialList.append(trial);

        # store the current staircase
        cs = self.GetCurrentStaircase()

        # evaluate the response?
        cs.EvaluateTrial(trial)

        # this is the staircase termination rule
        if cs.Terminated():

            # we do this by ID, otherwise it's too tricky
            self.DeactivateStaircase(self._CurrentStaircaseID);

    def DeactivateStaircase(self, id):

        # obtain the staircase to be deleted
        tmpStair = self._ActiveStairsList[id];

        # put this in the finished list
        self._FinishedStairList.append(tmpStair);

        # remove it from the active list
        self._ActiveStairsList.pop(id);

    def GetCurrentStaircase(self):

        # this returns the current staircase object
        return self._ActiveStairsList[self._CurrentStaircaseID];


    def SelectRandomStaircase(self):

        # how many are left in the active list?
        if self.nActiveStairs > 1:

            # we don't use 'self.nActiveStairs - 1' here as np.random is exclusive
            self._CurrentStaircaseID = np.random.randint(0, self.nActiveStairs)
        else: 
            self._CurrentStaircaseID = 0

        # return a random staircase from the active stairslist
        return self._ActiveStairsList[self._CurrentStaircaseID];


    ''' MC: A staircase doesn't normally have a set number of trials - do we need this? ''' 

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
        return self.nActiveStairs == 0

    def GetRandomResponse(self):

        # just returns a random response (0|1)
        return np.random.randint(0,1)

    def SimulateResponse(self):

        # It's like magic!
        p = np.random.normal(self._mu,self._sg)
        v = self._CurrentStair._CurrentStimval
        return int(p<v)





''' The main staircase class: all variables pertaining to a single staircase
    are contained within this class and it has all the functions to run '''

class Staircase ( object ):

    def __init__(self, staircaseID=0, steptype='fixed', condition=0, 
                 interval=0, initial=0, minboundary=0, maxboundary=100, 
                 up=1, down=3, fixedstepsize=0, maxtrials=100, 
                 maxreversals=13, maxboundaryhit=3, ignorereversals=3):

        self._MinBoundary = minboundary;
        self._MaxBoundary = maxboundary;
        self._OutOfBoundaryCount = 0;
        self._MaxBoundaryHit = maxboundaryhit

        self._StepType = steptype;
        self._FixedStepsizes = fixedstepsize;
        self._nUp = up;
        self._nDown = down;

        self._Condition = condition;
        self._nIntervals = interval;
        self._InitialStimval = initial;
        self._CurrentStimval = initial;

        self._nRight = 0;
        self._nWrong = 0;
        self._Reversals = list();
        self._direction = 0;
        self._TrialNum = 0;
        
        self._StaircaseIndex = staircaseID;

        # these two determine the termination rule
        self._MaxTrials = maxtrials;
        self._MaxReversals = maxreversals;
        self._IgnoreReversals = ignorereversals;
        
        # keep track of the data
        self._TrialList = list();

    ''' -- Properties, some of which are "dynamic" -- '''

    @property
    def NumReversals(self):
        return len(self._Reversals)

    @property
    def NumStepSizes(self):
        return len(self._FixedStepsizes)

    @property
    def NumTrials(self):
        return self._TrialNum;

    @property
    def ID(self):
        return self._StaircaseIndex


    ''' -- The meat of this class -- '''

    def NewTrial(self):

        ''' Gets a new trial based on previous parameters '''

        # increment the trial counter
        self.IncrementTrial()

        # set the stimulus value here
        self.SetStimval()

        # new trial instance
        return Trial(trialid = self.NumTrials, 
                  staircaseid = self._StaircaseIndex,
                  condition = self._Condition, 
                  stimval = self._CurrentStimval,
                  interval = self._nIntervals);


    def SetStimval(self):
    
        ''' sets a new stimulus value in _CurrentStimval '''
  
        # only do this if we're past trial one
        if self._TrialNum > 1:

            # make sure the steptype is a lowercase string
            steptype = self._StepType.lower()

            # first determine the 
            if steptype == 'fixed':

                # if we're not at the end of the list, otherwise we just
                # choose the last item of the _FixedStepsizes list.
                if self.NumReversals < self.NumStepSizes:
                    stepindex = self.NumReversals;
                else:
                    stepindex = self.NumStepSizes-1;

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

        # print 'stimval:', stimval, '(', self._StaircaseIndex, ')'

    def EvaluateTrial(self, trial):
    
        '''
        Evaluates the response
        '''

        # saves trial data to the actual staircase
        self._TrialList.append(trial)

        # set a default value for the direction
        self._direction = 0;

        # prepare a tuple to save to the _Reversals list
        revItem = (self._TrialNum, self._CurrentStimval, trial.Response)

        # "wrong" answer
        if trial.Response == 0:
            
            self._nWrong += 1

            # everything % 1 evaluates to true, so we need this
            if self._nUp == 1 or self._nWrong % self._nUp == 0:

                if self._nRight >= self._nDown:
                    
                    # add this to the reversal list
                    self._Reversals.append(revItem)

                # reset the counter
                self._nRight = 0;

                # set the step direction to up (+1)
                self._direction = 1
       
        # "correct" answer
        elif trial.Response == 1:

            self._nRight += 1

             # everything % 1 evaluates to true, so we need this
            if self._nDown == 1 or self._nRight % self._nDown == 0:

                if self._nWrong >= self._nUp:
                    
                    # add this to the reversal list
                    self._Reversals.append(revItem)

                # reset the counter
                self._nWrong = 0;

                # set the step direction to up (+1)
                self._direction = -1

        else:
            raise ValueError('Incorrect response: needs to be 0 or 1')

    def Terminated(self):

        ''' Determines if this staircase should terminate '''

        # maximum number of trials has been reached
        if self._MaxTrials <= self.NumTrials:
            return True

        # maximum number of reversals has been reached
        if self._MaxReversals <= self.NumReversals:
            return True

        # maximum number of boundary hits has been reached
        if self._MaxBoundaryHit <= self._OutOfBoundaryCount:
            return True

        # this is the default return: do not terminate
        return False


    def PlotResults(self):

        ''' plots the results - now depreciated in favor of the plotting routine in 
            the main component (i.e. ShowResults())'''

        print '- Now plotting results -'

        # define two lists to contain the x-y values
        x = list(); y = list()

        # loop through and create a list of values
        for idx, val in enumerate(self._TrialList):
            
            # creates list of x and y valeus
            x.append(val._TrialID); y.append(val._Stimval)

        # plot the stimulus values
        plt.plot(x,y, color='k', marker='o', ls='-')

        # reset these
        x = list(); y = list();

        # this can probably be done using a list comprehension
        for idx, val in enumerate(self._Reversals):

            # this creates a list of x and y values
            # x.append(val[0]); y.append(val[1])

            if val[2] == 0: 
                plt.plot(val[0], val[1], marker='o', color='r')
            else:
                plt.plot(val[0], val[1], marker='o', color='g')

        # plot the line that shows the mu
        plt.axhline(y=50,color='k',ls='dashed')

        # set limits
        plt.ylim([self._MinBoundary,self._MaxBoundary])
        plt.xlim([1, self.NumTrials])

        # actually show these results
        plt.savefig('plot.pdf')

    def GetStats(self):

        ''' Provides simple stats for the current staircase '''

        # extract the relevant values from the reversals
        vals = [x[1] for x in self._Reversals]

        # get just the last (i.e. ignore the first couple)
        reversals = vals[self._IgnoreReversals:]

        # return the mean and standard deviation
        return (np.mean(reversals), np.std(reversals))

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
