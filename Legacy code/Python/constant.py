from numpy import *

class workflow:

	def __init__(self, workflowitems=[]):
		self.workflowitems = workflowitems
		self.current = 0
		
	def execute(self):
		self.workflowitems[self.current]()

	def additem(self, item):
		self.workflowitems.append(item)

class constant:

	def __init__(self, conditions=0, stimvals=0, nrep=0, ninterval = 1):
	
		# constructor
		self.conditions = conditions
		self.stimvals = stimvals
		self.nrep = nrep
		self.trialnum = 0
		self.ninterval = ninterval
		self.finished = False
		self.numtrials = 0
		self.workflow = workflow()
		
	def Initialise(self):
		
		''' Initialises the object and creates a randomised factorial design '''
	
		# calculate the total number of trials for the experiment
		self.numtrials = self.conditions.size * self.stimvals.size * self.nrep
		
		# create the factorial design (conditions x stimvals)
		tmpCond, tmpStim = meshgrid(self.conditions, self.stimvals)
		
		# these lines vectorise the 2D array of values and replicates them nrep times
		tmpCond = tile(tmpCond.reshape(-1), self.nrep)
		tmpStim = tile(tmpStim.reshape(-1), self.nrep)
		
		# creates a random order for the trials
		r = random.permutation(self.numtrials)

		# new experimental parameters		
		self.experiment = experiment()
		self.experiment.conditions = tmpCond[r]
		self.experiment.stimvals = tmpStim[r]
		self.experiment.intervals = [int(x) for x in random.rand(self.numtrials,1)>0.5]
	
			
	def SetNextTrial(self):
		
		'''gets the parameters of the next trial'''
		
		# if we've reached the total number of trials we can stop
		if self.trialnum + 1 == self.numtrials: self.finished = True
		
		# otherwise we can continue to the next trial
		else: self.trialnum = self.trialnum + 1
			
			
	def GetTrial(self):
	
		''' gets the parameters of the current trial '''
			
		return trial(self.trialnum, self.experiment)
			
# ends class constant


class experiment:

	''' 
	At the moment I'm using this class to store values that are used in the experiment
	This should probably be some kind of list, but not sure which data type to use
	'''	
	
	# constructor
	def __init__(self, stimvals=0, conditions=0, intervals=0):
		self.stimvals = stimvals
		self.conditions = conditions
		self.intervals = intervals

# ends class exp	


class trial:

	# constructor
	def __init__(self, trialnum, experiment):
	
		self.trialnum = trialnum
		self.stimval = experiment.stimvals[trialnum]
		self.condition = experiment.conditions[trialnum]
		self.interval = experiment.intervals[trialnum]
		self.response = None
	
	# show the values of this specific trial
	def __str__(self):
		
		return 'Parameters: [#%.f] \t C(%.f) \t S(%.f) \t I(%.f) \t R(%.f)' % \
		(self.trialnum+1, self.condition, self.stimval, self.interval, self.response)

# end class trial

# ====================

def this_is_one():
	print 'one one one'

def get_response():
	return int(random.rand(1)>0.5)

# --------------------
# a little test script
# --------------------

# set the values for this experiment
conditions = arange(2)			# two conditions
stimulusvalues = r_[-5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5]		# eleven stimulus values
nintervals = 2					# two intervals
nrepetitions = 1				# ten repetitions per unique (condition x stimulusvalue)

# instantiate a new class of 'constant'
Exp = constant(conditions, stimulusvalues, nrepetitions, nintervals)

Exp.workflow.additem(this_is_one)
Exp.workflow.additem(get_response)

# initialise the class
Exp.Initialise()

# loop through the experiment
while not Exp.finished:

	# get the values for the current trial
	t = Exp.GetTrial()

	Exp.workflow.execute()
	Exp.workflow.execute()

	# this is normally where the drawing happens
	
	# set the observer response (0 or 1)
	
	# show the values in the trial class
	print t
		
	# set the next trial and save the response
	# this action should save the response and trials to the class (not yet done)
	Exp.SetNextTrial()