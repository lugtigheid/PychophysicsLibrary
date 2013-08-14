from StaircasePsychComponent import *

stair = StaircasePsychComponent();

stair.Initialise()

while not stair.isFinished():

    trial = stair.GetNextTrial()

    stair.Update();