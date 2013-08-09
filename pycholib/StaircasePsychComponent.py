import random

class StaircasePsychComponent ( object ):

    def __init__(self):

        self._ActiveStairsList = list()
        self._FinishedStairList = list()
        self._CurrentStaircaseID = 0;   # this refers to the _ActiveStairsList index

    def Initialise(self):

        # just set some values
        start = [0,100];
        stepsize = 2;
        minboundary = 0;
        maxboundary = 100;

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

        # select one staircase from the list
        self._CurrentStair = self.SelectRandomStaircase();

        # increment the trial counter
        self._CurrentStair.IncrementTrial()

        # bit of debugging
        # print 'A:', self.GetCurrentStaircase()._StaircaseIndex, 'T:', self.GetCurrentStaircase()._TrialNum;

    def EvaluateTrial(self, trial):

        # TODO: I don't actually think the trial parameter is needed.

        # store the current staircase
        cs = self.GetCurrentStaircase()

        # bit of debugging
        # print cs._TrialNum, cs._MaxTrials, len(self._ActiveStairsList);

        # this is staircase termination rule #1
        if cs._TrialNum < cs._MaxTrials:
            pass;
        else:
            self.DeactivateStaircase(self._CurrentStaircaseID);

    def GetTotalTrials(self):
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

    def __init__(self, staircaseID=0, steptype='fixed', initial=0, minboundary=0, maxboundary=100, stepsize=1):

        self._StepType = steptype;
        self._MinBoundary = minboundary;
        self._MaxBoundary = maxboundary;
        self._FixedStepsizes = stepsize;
        self._InitialStimval = initial;
        self._CurrentStimval = initial;
        self._nUp = 1;
        self._nDown = 1;

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