from numpy import *

# ----------------------------------------------------------------------------------------
class staircase:
	
	def __init__(self):
		self.activestairslist = []
		self.finishedstairslist = []
		self.currentsc = 0
		self.finished = False
		
	def __str__(self):
		pass
		
	def Initialise(self):
		''' Initialises the staircases '''
		
		# generate all the staircases - this should be dynamic
		self.activestairslist = [stair(0, 0,100,16,x) for x in range(4)]
	
	def GetCurrentTrial(self):
		pass
		
	def SetNextTrial(self):
		''' Sets the next trial: selects random staircase and initialises it '''
		
		# get the length of active staircases
		s = len(self.activestairslist)
		
		# select a random staircase from this list
		self.currentsc = random.randint(0,s)
		
		
	def EvaluateTrial(self, trial):
		''' Evaluates the staircase response '''
		
		pass
		
# ----------------------------------------------------------------------------------------
class stair:
	''' The stair class holds single staircases '''
	
	def __init__(self, id=0, condition=0, start=0, min=0, max=0, maxreversals=0):
		''' These are set values '''
		
		# x-up/y-down staircases
		self.up = 0
		self.down = 0
		
		# upper and lower boundaries
		self.min = min
		self.max = max
		
		# starting value
		self.start = start
		
		# stop after x reversals
		self.maxreversals = maxreversals
		
		# fixed, scaled, random 
		self.steptype = ''
		
		# the condition this staircase belongs to
		self.condition = condition
		
		''' These are internal '''
		
		self.id = id
		self.reversalcount = 0
		self.direction = 0
		self.stimval = 0
		self.trialnum = 0
		
	def __str__(self):
		''' Prints the class '''
		return 'nothing'


# ----------------------------------------------------------------------------------------
class trial:

	def __init__(self):
		pass
		
	def __str__(self):
		pass

# run this crap
p = staircase()

p.Initialise()

p.SetNextTrial()

print p.currentsc