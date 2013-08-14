from StaircasePsychComponent import *

Exp = StaircasePsychComponent();

Exp.Initialise()

while not Exp.isFinished():

    trial = Exp.GetNextTrial()

    trial.Response = Exp.SimulateResponse()

    print '----->', trial.Response

    Exp.Update(trial);

print Exp._CurrentStair._Reversals