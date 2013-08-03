import random;

class PsychComponentBase:

	def __init__(self, NumAFC = 1, GiveFeedback = False, Conditions = None, OutputFile = ''):
		
		# start the random number generator 
		random.seed();

		# These are all the parameters that are common to all procedures
		self._NumAFC = NumAFC;
		self._GiveFeedback = GiveFeedback;
		self._Conditions = Conditions;
		self._ChoiceAnswers;
		self._Status; 
		self._ActiveTrialIndex;
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