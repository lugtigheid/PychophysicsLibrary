class StaircasePsychComponent ( object ):

    def __init__(self):

        self._ActiveStairsList = list()
        self._FinishedStairList = list()

    def Initialise(self):

        self._ActiveStairsList = [Staircase() for x in range(2)]

class Staircase ( object ):

    def __init__(self):

        self._StaircaseType;
        self._StepSizeType;
        self._MinBoundary;
        self._MaxBoundary;
        self._FixedStepsizes;
        self._InitialStimval;
        self._CurrentStimval;
        self._nUp;
        self._nDown;

        self._Reversals; # don't need reversal count - just return len(self._Reversals)
        self._OutOfBoundaryCount;

        self._RightSinceWrong;
        self._WrongSinceRight;
        self._direction;
        self._TrialNum;
        self._Status;
        self._StaircaseIndex;

        # these two determine the termination rule
        self._MaxTrials;
        self._MaxReversals;

        def isValid(self):
            # this should check that the values here are all valid for use
            pass