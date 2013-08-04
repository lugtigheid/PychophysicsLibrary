from Trial import *

class ConstantStimulusPsychComponent(object):

    def __init__(self, *args, **kwargs):
        ''' Constructor: sets up the variables '''

        self._StimulusValues = list();
        self._Conditions = None;


    def Initialise(self):
        pass

    def GetNextTrial(self):
        trial = Trial();
        return trial;

    def AddStimval(self, stimval):
        
        # if this is a number (float, long, int, complex) add it. 
        if isinstance(stimval, (int, long, float)) : 
            self._StimulusValues.append(stimval)

        # otherwise if this is a list we add all items to the list
        elif type(stimval) is list:

            # loop through the list
            for item in stimval:

                '''but check if numeric before adding (otherwise 
                    I would have just used .extend() here) '''
                if isinstance(item, (int, long, float)): 
                    self._StimulusValues.append(item)

        else:
            print "ERROR: can only add numbers of lists of numbers to stimulus values!"

    def Evaluate(self, trial):
        pass

