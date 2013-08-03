class Tester:

	def __init__(self, conditions=None, stimvals=None):
		self.conditions = conditions
		self.stimvals = stimvals
		self.workflow = WorkFlow()
		
class WorkFlow:

	def __init__(self, workitemlist = []):
		self.current = 0
		self.workitemlist = workitemlist
		
	@get_parameters
	def here_we_go(self):
		print 'Here we go'
		
	def get_parameters(self, func):
		def wrap(func):
			print '---'
			func()
			print '+++'
		return wrap

		
	def execute(self):
	
		# execute the current flow item 
		self.workitemlist[self.current]()
		
class Trial:
	
	def __init__(self):
		pass
		
t = Tester()
t.workflow.here_we_go()