import PsychComponentBase;

class ConstantStimulusPsychComponent (PsychComponentBase):

	def __init__(self, NumAFC = 1, GiveFeedback = False, Conditions = None, OutputFile = ''):

		super.__init__(NumAFC = 1, GiveFeedback = False, Conditions = None, OutputFile = '');

		self._Rep;
		self._StimVals;
		self._NumTrials;
		self._FeedbackFrequency;
		
	def Initialise():
		pass

	def SetNextTrail(): 
		pass

	def EvaluateResponse (trial):
		pass

	def Start():
		pass

	def Stop():
		pass


