class StaircasePsychComponent ( object ):

    def __init__(self):

        self._Staircases = list()
        self._RuntimeStaircases = list()
        self._EndedStaircases = list()


class Staircase ( object ):

    def __init__(self):

        self._StaircaseType;
        self._StepSizeType;
        self._MinBoundary;
        self._MaxBoundary;
        self._FixedStepsizes;
        self._InitialStimval;
        self._nDown;
        self._nUp;

        self._Reversals; # don't need reversal count - just return len(self._Reversals)
        self._OutOfBoundaryCount;
        self._RightSinceWrong;
        self._WrongSinceRight;
        self._TrialCount;
        self._Status;
        self._StaircaseIndex;

        # these two determine the termination rule
        self._MaxTrials;
        self._MaxReversals;

        def isValid(self):
            # this should check that the values here are all valid for use
            pass