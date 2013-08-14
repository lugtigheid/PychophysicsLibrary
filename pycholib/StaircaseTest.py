from StaircasePsychComponent import *

Exp = StaircasePsychComponent();

Exp.Initialise()

while not Exp.isFinished():

    trial = Exp.GetNextTrial()

    trial.Response = Exp.GetRandomResponse()

    Exp.Update(trial);