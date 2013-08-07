class StaircasePsychComponent ( object ):

    def __init__(self):

        self._ActiveStairsList = list()
        self._FinishedStairList = list()

    def Initialise(self):

        self._ActiveStairsList = [Staircase() for x in range(2)]

class Staircase ( object ):

    def __init__(self):

        self._StaircaseType = 0;
        self._StepSizeType = '';
        self._MinBoundary = 0;
        self._MaxBoundary = 0;
        self._FixedStepsizes = 0;
        self._InitialStimval = 0;
        self._CurrentStimval = 0;
        self._nUp = 1;
        self._nDown = 3;

        self._Reversals = 0; # don't need reversal count - just return len(self._Reversals)
        self._OutOfBoundaryCount = 0;

        self._RightSinceWrong = 0;
        self._WrongSinceRight = 0;
        self._direction = 0;
        self._TrialNum = 0;
        self._Status = 0;
        self._StaircaseIndex = 0;

        # these two determine the termination rule
        self._MaxTrials = 0;
        self._MaxReversals = 0;

    def isValid(self):
    
        # this should check that the values here are all valid for use
        pass