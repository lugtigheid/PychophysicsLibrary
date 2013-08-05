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

    ''' Fields or properties '''

    @property
    def Response(self):
        return self._Response;

    @Response.setter
    def Response(self, value):
        self._Response = value;

    ''' Methods '''

    # returns the name of the current condition
    def GetConditionName(self):

        # this is really quite awkward
        return 

    # show the values of this specific trial
    def __str__(self):
        
        # print self._TrialID, self._Stimval, self._Interval, self._Response

        return '[ #%2.f ] \t (%s:%.f) \t\t Stim(%4.f) \t Interval(%.f) \t Resp(%.f)' % \
        (self._TrialID+1, self._Condition.Name, self._Condition.Index, self._Stimval, self._Interval, self._Response)
