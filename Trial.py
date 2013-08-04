class Trial(object):

    ''' 
        Created         Fri 2 Aug 2013 - ajl
        Last edited:    Sun 4 Aug 2013 - ajl

        This class contains all variables and methods associated with a single trial 
    '''

    def __init__(self):
        ''' Constructor '''

        self._TrialID;
        self._StaircaseID;
        self._Condition;
        self._Stimval;
        self._Interval;
        self._Response;
        self._ReactionTime;

    def GetTrialData(self):
        ''' Return a list of data from this trial '''
        pass
