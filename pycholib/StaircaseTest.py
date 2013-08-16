from StaircasePsychComponent import *

# new instance of the StaircasePsychComponent
Exp = StaircasePsychComponent();

# Initialise (this is now an internal initialisation but should allow for 
# all the initial staircases to be passed as a variable here, batch?)
Exp.Initialise()

# we continue until finished
while not Exp.isFinished():

    # get the next trial
    trial = Exp.GetNextTrial()

    # get a response from the simulator
    trial.Response = Exp.SimulateResponse()

    # this updates the results and saves the trial
    Exp.EvaluateTrial(trial);

# plot the results
# Exp._CurrentStair.PlotResults()

# gets the standard deviation and mean for staircase
# Exp._CurrentStair.Stats()

Exp.ShowResults();