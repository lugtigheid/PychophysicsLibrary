# -*- coding: utf-8 -*-

class WorkFlow(object):
	''' Create a workflow for experimental control '''
	''' Essentially just a list of display functions '''
	
	
	def __init__(self, workitemlist = []):
		''' Constructor '''		
		
		# set the basic variables here
		self.workitemlist = workitemlist
		self.current = 0
		
	def AddWorkItem(self, workitem):
		''' Just a simple setter function for workitems '''
		
		# add the workitem to the list
		self.workitemlist.append(workitem)
		
	def Next(self):
		''' Sets the next workitem and executes it'''
		
		# set the next or if last loop back to first
		if self.current == len(self.workitemlist)-1:
			
			# reached the end of the list, so back to zero
			self.current = 0
		else:
			
			# go to the next workitem
			self.current += 1

		# execute that shit
		self.Execute()

	def Execute(self):
		''' Execute the current workitem '''
		
		# execute the current flow item 
		self.workitemlist[self.current]()
		
	def Start(self):
		''' Function to execute when starting the workflow '''
		
		# just execute the current workitem
		self.Execute()
	
	def Quit(self):
		''' Function to execute when ending the workflow '''
		
		# nothing implemented just yet
		pass
		
		
''' Test out the workflow here '''

# instantiate new workflow class
trialflow = WorkFlow()

def Start():
	print '---------'
	print 'Starting!'
	trialflow.Next()
	
def Interval(num=1):
	print 'Interval %.f' % num
	trialflow.Next()
	
def End():
	print 'Ending!\n'
	
# add the relevant items to the list
trialflow.AddWorkItem(Start)
trialflow.AddWorkItem(Interval)
trialflow.AddWorkItem(End)

# start the list off with the first one
trialflow.Start()
