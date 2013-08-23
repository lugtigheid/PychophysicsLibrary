# import the staircase component
from StaircasePsychComponent import *

# new instance of the StaircasePsychComponent
Exp = StaircasePsychComponent();

# define the stepsizes - generic to every staircase
stepsize = [10, 10, 8, 6, 4, 2, 1]

# add two staircases that target the ~80% point
Exp.AddStair(Staircase(up=1, down=3, initial=0, fixedstepsize=stepsize))
Exp.AddStair(Staircase(up=1, down=3, initial=100, fixedstepsize=stepsize))

# add two staircases that target the ~20% point
Exp.AddStair(Staircase(up=3, down=1, initial=0, fixedstepsize=stepsize))
Exp.AddStair(Staircase(up=3, down=1, initial=100, fixedstepsize=stepsize))

# this should validate all the staircases and start logging
# Exp.Start()

# we continue until finished
while not Exp.isFinished():

    # get the next trial
    trial = Exp.GetNextTrial()

    # get a response from the simulator
    trial.Response = Exp.SimulateResponse()

    # this updates the results and saves the trial
    Exp.EvaluateTrial(trial);

# plot the results
Exp.ShowResults();

# this should end the logging, mainly
# Exp.Stop()