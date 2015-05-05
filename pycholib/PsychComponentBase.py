# -*- coding: utf-8 -*-

import random;

class PsychComponentBase:

	def __init__(self, 
		NumAFC = 1, 
		GiveFeedback = False, 
		Conditions = None, 
		OutputFile = ''):
		
		# start the random number generator 
		random.seed();

		# These are all the parameters that are common to all procedures
		self._NumAFC = NumAFC;
		self._GiveFeedback = GiveFeedback;
		self._Conditions = Conditions;
		self._ChoiceAnswers;
		self._Status; 
		self._OutputFile = OutputFile;

	def GetRandomInterval(self):
		return random.randint(1,self.NumberAFC)

	def GetTrial(self, index):
		pass

	def Initialise():
		pass
	
	def InvokeFeedbackEvent(): 
		pass 

	#all logging should also be in here

	def GetConditionsDetails(): pass
	def GetExperimentHeader(): pass
	def GetParameterDetails(): pass
	def GetTrialHeader(): pass
	def GetResponseInfo(): pass
	def WriteLogFile(): pass

class ConstantStimulusPsychComponent (PsychComponentBase):

	def __init__(self, *args, **kwargs):

		super(ConstantStimulusPsychComponent, self).__init__(*args, **kwargs)

		self._Rep;
		self._StimVals = list();
		self._NumTrials;
		self._FeedbackFrequency;
		self._CurrentTrial;
		
	def Initialise():
		pass

	def AddStimval(self, stimval):
     
		''' Adds stimulus values to the list of stimulus values - checks that
              input is not a string, but should also not be a logical etc: allowed
              types should be float, double and integer'''
      
		if not isinstance(stimval, str):
			self._StimVals.append(stimval);
		else:
			# should perhaps be some kind of try/catch statement
			print "Error: stimulus values cannot be strings!"


	def SetNextTrail(): 
		pass

	def EvaluateResponse (trial):
		pass

	def Start():
		pass

	def Stop():
		pass
