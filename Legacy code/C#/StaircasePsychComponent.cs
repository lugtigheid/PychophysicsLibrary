using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;


namespace PsychComponent
{
    [DesignTimeVisible(true)]
    public class StaircasePsychComponent : PsychComponentBase
    {
        
        private bool _ShowStaircaseEnding;
        private List<Staircase> _Staircases;
        private List<RuntimeStaircase> _RuntimeStaircases;
        private List<RuntimeStaircase> _EndedStaircases;
        


        public StaircasePsychComponent() : base()
        {
            _Staircases = new List<Staircase>();
            Trials=new List<TrialInstance>();
            _ShowStaircaseEnding = false;

        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<Staircase> Staircases
        {
            get
            {
                return _Staircases;
            }
        }


        [Category("Counters")]
        public int StaircaseCount
        {
            get
            {
                return _Staircases.Count;
            }
        }

        [Category("Counters")]
        public int RunTimeStaircaseCount
        {
            get
            {
                return _Staircases.Count * Conditions.Count;
            }
        }


        [Browsable(false)]
        public List<RuntimeStaircase> ActiveStairCases
        {
            get
            {
                return _RuntimeStaircases;
            }
        }

        public bool ShowStaircaseEnding
        {
            get
            {
                return _ShowStaircaseEnding;
            }
            set
            {
                _ShowStaircaseEnding = value;
            }
        }
        
        public override bool Initialise()
        {
            if (!base.Initialise())
                return false;

            // validate that is worth starting an experiment

            if (_Staircases.Count == 0)
            {
                MessageBox.Show("Must have 1 or more staircases to continue", "No staircases specified", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return false;
            }

            //TODO: validate base staircase properties
            //TODO:  validate stimval,maxstimval,minstimval for each staircase
            bool AllStaircasesValid = true;
            for (int i = 0; i < _Staircases.Count; i++) 
            {
                _Staircases[i].Index=i;
                if (_Staircases[i].Validate() == false)
                {
                    //TODO: notify that a staircase is invalid some reason
                    //TODO: design a form which can display errors in all staircases maybe?
                    AllStaircasesValid = false;
                }
                
            }
            if (AllStaircasesValid == false)
                return false;

            // construct runtime staircase list - copy details from staircase template via copy constructor
            _RuntimeStaircases = new List<RuntimeStaircase>(Conditions.Count * _Staircases.Count);
            _EndedStaircases = new List<RuntimeStaircase>();

            int index = 0;
            for (int i = 0; i < _Staircases.Count; i++)
            {
                _Staircases[i].Index = i;
                for (int j = 0; j < Conditions.Count; j++)
                {
                    RuntimeStaircase rs=new RuntimeStaircase(_Staircases[i],j);
                    rs.RuntimeIndex = index;
                    _RuntimeStaircases.Add(rs);
                    index++;
                    
                }
            }

            Trials = new List<TrialInstance>(); // instantiate list of trials for this staircase
            ActiveTrialIndex = -1; // _Trials would be zero-based
            return true;
            
        }

        private RuntimeStaircase SelectRandomStaircase()
        {
            int index = sg.Next(0, _RuntimeStaircases.Count);
            return _RuntimeStaircases[index];
            
        }

        private StairCaseTrialInstance CreateTrial()
        {
            StairCaseTrialInstance trial=new StairCaseTrialInstance();
            RuntimeStaircase rs = SelectRandomStaircase();
            if (rs.Status == StaircaseStatusEnum.NotStarted)
                rs.Status = StaircaseStatusEnum.Active;

            SetStimVal(rs);

            trial.RuntimeStaircase = rs;
            // choose random interval 
            trial.Interval = GetRandomInterval();
            trial.Condition = rs.Condition;
            trial.StimVal = rs.StimVal;
            SetFixedParameters(trial);
            SetRandomParameters(trial);
			rs.TrialCount = rs.TrialCount + 1;
            return trial;

        }

        private void SetStimVal(RuntimeStaircase rs)
        {
            int stepindex;
            if (rs.StepType == StepTypeEnum.NoChange) // don't change the stim val
                return;

            if (rs.StepType != StepTypeEnum.Start)
            {
                switch (rs.StepSizeType)
                {
                    case StepSizeTypeEnum.ScaledStimVal:
                            double scaleval = rs.StimVal * (rs.ScaleFactorPercentage/100);

                            if (rs.StepType == StepTypeEnum.StepDown)
                                rs.StimVal = rs.StimVal - scaleval;
                            else if (rs.StepType == StepTypeEnum.StepUp)
                                rs.StimVal = rs.StimVal +scaleval; 
                        break;
                    case  StepSizeTypeEnum.ScaledStepSize:
                        //TODO: check the logic here
                        if (rs.ReversalCount < rs.FixedStepSizes.Count)
                        {
                            stepindex = rs.ReversalCount;

                            double step = rs.FixedStepSizes[stepindex];
                            scaleval = step * (rs.ScaleFactorPercentage / 100);

                            if (rs.StepType == StepTypeEnum.StepDown)
                                step = step - scaleval;
                            else if (rs.StepType == StepTypeEnum.StepUp)
                                step = step + scaleval;

                        }
                        else
                        {
                            stepindex = rs.FixedStepSizes.Count - 1;
                            double step = rs.FixedStepSizes[stepindex];
                            scaleval = step * (rs.ScaleFactorPercentage / 100);

                            if (rs.StepType == StepTypeEnum.StepDown)
                                rs.StimVal = rs.StimVal - scaleval;
                            else if (rs.StepType == StepTypeEnum.StepUp)
                                rs.StimVal = rs.StimVal + scaleval;
                        }

                        break;
                    case StepSizeTypeEnum.FixedStepSize:
                            if (rs.ReversalCount < rs.FixedStepSizes.Count) // no. of reversals is less than fixed step sizes
                            {
                                stepindex=rs.ReversalCount;
                                if (rs.StepType==StepTypeEnum.StepDown)
                                {
                                   rs.StimVal = rs.StimVal - (rs.FixedStepSizes[stepindex]);
                                }
                                else
                                {
                                    rs.StimVal = rs.StimVal + (rs.FixedStepSizes[stepindex]);
                                }
                            }
                            else // no of reversals has exceeded fixed step sizes so just use last step size
                            {
                                if (rs.StepType==StepTypeEnum.StepDown)
                                {
                                    rs.StimVal=rs.StimVal - (rs.FixedStepSizes[rs.FixedStepSizes.Count-1]);
                                }
                                else
                                {
                                    rs.StimVal=rs.StimVal + (rs.FixedStepSizes[rs.FixedStepSizes.Count-1]);
                                }
                            }
                        break;
                }
 

            }
            else
            {
                rs.StimVal = rs.InitialStimVal;
                if (rs.RandomizeStimVal == true)
                {
                    rs.StimVal = FlatRandom(rs.StimVal, rs.StimValRandPercentage);
                }
            }

        }

        protected override string GetTrialHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#\tCond\tStaircase\tStimVal\tInterv\t");

            if (FixedParameters.Count > 0)
            {
                for (int i = 0; i < FixedParameters.Count; i++)
                {
                    sb.Append(FixedParameters[i].Name + "\t");
                }
            }
            
            if (RandomParameters.Count > 0)
            {
                for (int i = 0; i < RandomParameters.Count; i++)
                {
                    sb.Append(RandomParameters[i].Name + "\t");
                }
            }

            sb.Append("Response\tCorrect?\tLatency");
            sb.AppendLine();

            return sb.ToString();
        }


        public override bool Start()
        {
            StringBuilder sb = new StringBuilder();

            // construct logfile header
            
            sb.Append(GetExperimentHeader());
            sb.Append(GetConditionsDetails());
            sb.Append(GetParametersDetails());
            sb.Append(AddSectionBreak());
            sb.Append(GetStaircasesDetails());
            sb.Append(GetTrialHeader());
            WriteToFile(sb.ToString(),true,true);
            // setup the first trial
            StairCaseTrialInstance trial=CreateTrial();

            this.Trials.Add(trial);
            this.Status = PsychComponentStatusEnum.Active; // we've started
            ActiveTrialIndex = 0; //TODO: is this per staircase or for the whole component

            return true;

        }

        protected override void InvokeFeedbackEvent(object sender, FeedbackEventArgs feedbackevent)
        {
            base.InvokeFeedbackEvent(sender, feedbackevent);
        }


        public override void Stop()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(AddSectionBreak());
            sb.Append(GetChoiceStats());
            sb.Append(AddSectionBreak());
            sb.AppendLine(GetStaircaseStats());
            sb.Append(AddSectionBreak());
            sb.AppendLine("*** Experiment finished at " + DateTime.Now.ToString());
            WriteToFile(sb.ToString(), true);
        }

        private string GetTrialDetails(StairCaseTrialInstance trial)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append((ActiveTrialIndex+1) + "\t" + (trial.Condition+1) + "\t" + (trial.RuntimeStaircase.RuntimeIndex+1) + "\t" + trial.StimVal+ "\t" + trial.Interval +"\t");
            if (trial.FixedParams.Count > 0)
            {
                for (int i = 0; i < trial.FixedParams.Count; i++)
                {
                    sb.Append(trial.FixedParams[i] + "\t");
                }
            }

            if (trial.RandomParams.Count > 0)
            {
                for (int i = 0; i < trial.RandomParams.Count; i++)
                {
                    sb.Append(trial.RandomParams[i].ToString("N4") + "\t");
                }

            }
            return sb.ToString();

        }


        public override void EvaluateResponse(UserResponse response,TrialInstance _trial)
        {

            StairCaseTrialInstance trial=(StairCaseTrialInstance)_trial;
            RuntimeStaircase rs=trial.RuntimeStaircase;
            StringBuilder sb = new StringBuilder();
            bool reverse=false;

            // log response info
            int ActualChoice=response.SelectedChoice;

            int choiceindex = ActualChoice - 1;

            this.ChoiceSelectCount[choiceindex]++;

            string ActualName=ChoiceAnswers[choiceindex];

            sb.Append(GetTrialDetails(trial));

			string choice;
			if (choiceindex < ChoiceAnswers.Count)
				choice = ChoiceAnswers[choiceindex];
			else
				choice = response.SelectedChoice.ToString(); // 1 based

			sb.Append(choice + "\t");

            if (this.HasCorrectAnswer == true)
            {
                int CorrectChoice = trial.Interval;  
                this.ChoicePresentCount[CorrectChoice-1]++;
                string CorrectName=ChoiceAnswers[CorrectChoice-1];
                sb.Append(GetResponseInfo(CorrectChoice,ActualChoice,ActualName,CorrectName));
                
                if (ActualChoice==CorrectChoice)
                {
                    reverse=ProcessCorrectAnswer(trial);
                    if (this.GiveFeedback==true) //TODO: check this
                    {
                        FeedbackEventArgs feedbackevent = new FeedbackEventArgs(trial,true);
                        InvokeFeedbackEvent(this, feedbackevent);
                    }

                }
                else
                {
                    reverse=ProcessWrongAnswer(trial);
                    if (this.GiveFeedback==true)
                    {
                        FeedbackEventArgs feedbackevent = new FeedbackEventArgs(trial,false);
                        InvokeFeedbackEvent(this, feedbackevent);
                    }

                }

            }
            else
            {
                sb.Append(GetResponseInfo(ActualChoice,ActualName));
            }

            sb.Append("\t" + response.ResponseLatency.ToString("N4") + "\t");

            if (reverse == true)
            {
                sb.Append(" - Reversal ");
                sb.Append(trial.RuntimeStaircase.ReversalValues.Count);
                sb.Append(": Staircase " + (trial.RuntimeStaircase.RuntimeIndex+1));
              /*  for (int i = 0; i < trial.RuntimeStaircase.ReversalTrials.Count; i++)
                {
                    sb.Append(trial.RuntimeStaircase.ReversalTrials[i]);
                    if (i < trial.RuntimeStaircase.ReversalTrials.Count - 1)
                        sb.Append(",");
                }*/
            }

            if (response.UserInput != "")
            {
                sb.Append(response.UserInput);
            }

            // determine if this staircase should end

            bool ended=false;
            string StaircaseEnding = "";
            if (rs.ReversalCount >=rs.MaxReversals)
            {
                rs.Status=StaircaseStatusEnum.EndedMaxReversals;
                ended=true;
                StaircaseEnding = " the staircase exceeded the maximum number of reversals";
            }
            else if (rs.MaxTrials>0 && rs.TrialCount > rs.MaxTrials)
            {
                rs.Status=StaircaseStatusEnum.EndedMaxTrials;
                ended=true;
                StaircaseEnding = " the maximum number of trials was reached";
            }
            else if (rs.StimVal < rs.MinStimVal)
            {
                rs.StimVal = rs.MinStimVal;
                rs.OutOfBoundariesCount++;
                if (rs.OutOfBoundariesCount > rs.MaxBoundaries)
                {
                    rs.Status = StaircaseStatusEnum.EndedMaxLowerBoundary;
                    if (ShowStaircaseEnding == true)
                    {
                        StaircaseEnding=" the subject tried passing the min or max stim val too many times";
                    }
                    ended = true;
                }
            }
            else if (rs.StimVal > rs.MaxStimVal)
            {
                rs.StimVal = rs.MaxStimVal;
                rs.OutOfBoundariesCount++;
                if (rs.OutOfBoundariesCount > rs.MaxBoundaries)
                {
                    rs.Status = StaircaseStatusEnum.EndedMaxUpperBoundary;
                    if (ShowStaircaseEnding == true)
                    {
                        StaircaseEnding=" the subject tried passing the min or max stim val too many times";
                    }
                    ended = true;
                }
            }

            if (ended==true)
            {
                // remove staircase from runtime staircases and add it to ended staircases
                sb.Append(GetEndedDetails(rs.Status,rs.Index));
                this._EndedStaircases.Add(rs);
                this._RuntimeStaircases.Remove(rs);
                if (_ShowStaircaseEnding==true)
                    MessageBox.Show("Staircase " + (rs.RuntimeIndex+1) + " was ended because " + StaircaseEnding);
            }

            if (this._RuntimeStaircases.Count==0) // there are no more staircases left to run
            {
                this.Status = PsychComponentStatusEnum.Done;
            }
            sb.AppendLine();

            WriteToFile(sb.ToString(),false);



        }

        public override void SetNextTrial()
        {
            StairCaseTrialInstance trial = CreateTrial();
            Trials.Add(trial);
			trial.TrialNumber = ActiveTrialIndex;
            ActiveTrialIndex++;
        }

        

        private String GetStaircasesDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(_Staircases.Count + " staircases per condition:" );
            for (int i=0;i<_Staircases.Count;i++)
            {
                Staircase st=_Staircases[i];
                sb.Append ("Staircase " + (i+1) + ": ");
                sb.Append ("type " + st.STCUp + " up , " + st.STCDown + " down" );
                sb.AppendLine();
                if (st.RandomizeStimVal)
                    sb.AppendLine ("Stimval randomization +/-" + st.StimValRandPercentage + "%" );
                sb.Append("Initial Stimval: " + st.InitialStimVal + " ");
                if (!st.RandomizeStimVal)
                    sb.Append("(fixed) ");
                sb.Append ("Min: " + st.MinStimVal + " ");
                sb.Append ("Max: " + st.MaxStimVal );
                sb.AppendLine();
                if (st.StepSizeType==StepSizeTypeEnum.ScaledStimVal)
                    sb.AppendLine ("Stim val scaled by " + st.ScaleFactorPercentage );

                if (st.StepSizeType==StepSizeTypeEnum.ScaledStepSize)
                {
                    sb.Append ("Scaled step sizes - scale factor: " + st.ScaleFactorPercentage + "%" );
                }
                else
                {
                    sb.Append("Fixed step sizes: ");
                }
                
                for (int j=0;j<st.FixedStepSizes.Count;j++)
                {
                    sb.Append (st.FixedStepSizes[j]);
                    if (j < st.FixedStepSizes.Count-1)
                        sb.Append(" ");
                }
                sb.AppendLine();
                sb.Append("Max reversals : " + st.MaxReversals + " ");
                sb.Append("Useful reversals : " + st.UsefulReversals + " ");
                sb.AppendLine();
                sb.AppendLine("Maximum trials : " + st.MaxTrials);
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.Append(AddSectionBreak());
            return sb.ToString();

        }

        //private String GetColumnsDetails()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendFormat("Condition\tStaircase\tStimval ");
        //    if (FixedParameters.Count > 0)
        //    {
        //        for (int i = 0; i < FixedParameters.Count - 1; i++)
        //            sb.Append(FixedParameters[i].Name + "\t");
        //    }
        //    if (RandomParameters.Count > 0)
        //    {
        //        for (int i = 0; i < RandomParameters.Count - 1; i++)
        //            sb.Append(RandomParameters[i].Name + "\t");
        //    }
        //    sb.AppendFormat("Interval\tResponse\tCorrect Btn\tLatency");
        //    sb.AppendLine();
        //    return sb.ToString();
        //}

        private string GetEndedDetails(StaircaseStatusEnum s, int StaircaseIndex)
        {

            StringBuilder sb = new StringBuilder();
            StaircaseIndex++; // so staircase starts with 1 rather than 0

            switch (s)
            {
                case StaircaseStatusEnum.EndedMaxLowerBoundary | StaircaseStatusEnum.EndedMaxUpperBoundary:
                    sb.Append(" (Ended (" + StaircaseIndex + ") - Stimval") ;
                    break;
                case StaircaseStatusEnum.EndedMaxReversals:
                    sb.Append(" (Ended (" + StaircaseIndex + ") - Reversals");

                    break;
                case StaircaseStatusEnum.EndedMaxTrials:
                    sb.Append(" (Ended (" + StaircaseIndex + ") - Trials");
                    break;
            }
            return sb.ToString();
        }



        private string GetStaircaseStats()
        {
            StringBuilder sb=new StringBuilder();
            for (int i = 0; i < _EndedStaircases.Count; i++)
            {
                RuntimeStaircase sc = _EndedStaircases[i];
                sb.Append("Staircase " + (i+1) + " - (Condition " + (sc.Condition+1) + " - " + Conditions[sc.Condition] + ")");
                switch (sc.Status)
                {
                    case StaircaseStatusEnum.EndedMaxLowerBoundary | StaircaseStatusEnum.EndedMaxUpperBoundary:
                        sb.Append(" - Ended because of boundaries");
                        break;
                    case StaircaseStatusEnum.EndedMaxTrials:
                        sb.Append(" - Ended because of max trials");
                        break;
                    case StaircaseStatusEnum.EndedMaxReversals:
                        sb.Append(" - Ended because of max reversals");
                        break;
                }
                sb.AppendLine();

                if (sc.ReversalCount > 0)
                {
                    if (sc.ReversalCount - sc.IgnoreReversals <= 0)
                        sb.AppendLine("No useful reversals");
                    else
                    {
                        sb.AppendLine("Reversal values:");
                        sb.AppendLine("Ignored :");
                        for (int j = 0; j < sc.IgnoreReversals; j++)
                        {
                            sb.Append(sc.ReversalValues[j].ToString("N2"));
                            sb.Append("\t");
                        }
                        sb.AppendLine();
                        sb.AppendLine("Included :");
                        for (int j = (int)sc.IgnoreReversals; j < sc.ReversalValues.Count; j++)
                        {
                            sb.Append(sc.ReversalValues[j].ToString("N2"));
                            sb.Append("\t");
                        }

                        sb.AppendLine();

                        double[] rv;
                        rv = new double[sc.ReversalValues.Count - sc.IgnoreReversals];
                        sc.ReversalValues.CopyTo((int)sc.IgnoreReversals, rv,0,sc.ReversalValues.Count-(int)sc.IgnoreReversals);

                        double Avg = Stats.Average(rv);
                        double StdDev = Stats.StandardDeviation(rv);

                        sb.AppendLine("Average Stim Val : " + Avg.ToString("N3"));
                        sb.AppendLine("StDev: " + StdDev.ToString("N3"));
                        sb.AppendLine();

                    }
                }                

            }

            return sb.ToString();

        }

        protected override bool ProcessCorrectAnswer(TrialInstance t)
        {
            
            StairCaseTrialInstance st = (StairCaseTrialInstance)t;
            RuntimeStaircase rs = st.RuntimeStaircase;
            // answer is correct
            rs.RightSinceWrong++;  // increment right since wrong
            rs.StepType = StepTypeEnum.NoChange;
            bool reverse=false;

            if (rs.STCDown == 1 || (rs.RightSinceWrong % rs.STCDown) == 0)
            {
                if (rs.WrongSinceRight >= rs.STCUp) // 
                {
                    reverse = true;
                    rs.ReversalCount++;
                    rs.ReversalValues.Add(st.StimVal); // store all reversals even if not useful
                    rs.ReversalTrials.Add(Trials.IndexOf(t)+1);
                }
                else
                {
                    reverse = false;
                }
                rs.WrongSinceRight = 0;
                //TODO: AEW check the logic here
                rs.StepType = StepTypeEnum.StepDown; // set this so when we set next stim val we know if we are going up or down in value
            }

            return reverse;

        }


        protected override bool ProcessWrongAnswer(TrialInstance t)
        {
            StairCaseTrialInstance st = (StairCaseTrialInstance)t;
            RuntimeStaircase rs=st.RuntimeStaircase;
            rs.WrongSinceRight++;
            rs.StepType = StepTypeEnum.NoChange;
            bool reverse = false;

            if (rs.STCUp==1 || (rs.WrongSinceRight %  rs.STCUp==0))
            {
                if (rs.RightSinceWrong >= rs.STCDown)
                {
                    reverse = true;
                    rs.ReversalCount++;
                    rs.ReversalValues.Add(st.StimVal); // store all reversals
                    rs.ReversalTrials.Add(Trials.IndexOf(t)+1);

                }
                else
                {
                    reverse = false;
                }

                rs.RightSinceWrong=0; // reset RightSinceWrong as we got a wrong answer
                rs.StepType=StepTypeEnum.StepUp;
            }
            return reverse;
        }


    }
    
}
