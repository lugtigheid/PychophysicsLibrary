class Trial(object):

    ''' 
        Created         Fri 2 Aug 2013 - ajl
        Last edited:    Sun 4 Aug 2013 - ajl

        This class contains all variables and methods associated with a single trial 
    '''

    def __init__(self, trialid = 0, staircaseid = 0, condition = 0, stimval = 0, interval = 1):
        ''' Constructor '''

        self._TrialID = trialid;
        self._StaircaseID = staircaseid;
        self._Condition = condition;
        self._Stimval = stimval;
        self._Interval = interval;
        self._Response = None;
        self._ReactionTime = None;

    def GetTrialData(self):
        ''' Return a list of data from this trial '''
        print self._TrialID, self._Condition, self._Stimval, self._Interval
