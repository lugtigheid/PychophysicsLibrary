from StaircasePsychComponent import *

# Version checking - I use 2.7.3 EPD
import platform
print(platform.python_version())

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

    # this updates the results
    Exp.Update(trial);

# plot the results
Exp._CurrentStair.PlotResults()

# gets the standard deviation and mean for staircase
Exp._CurrentStair.Stats()