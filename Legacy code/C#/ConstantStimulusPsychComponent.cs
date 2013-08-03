using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using Troschuetz.Random;

//TODO: Categorise and describe published properties etc.

namespace PsychComponent
{
    [DesignTimeVisible(true)]
    public class ConstantStimulusPsychComponent : PsychComponentBase
    {
        private uint _Repetitions;
        private List<string> _StimulusLevels;
        private List<double> _StimulusValues;
        private int _NumberofTrials;
        private int _FeedbackFrequency;
        private ProbitTypeEnum _PerformProbit;
        private int[,]_NumberCorrect;
        private int[,] _NumberWrong;
        private int[,,] _CountByChoiceConditionandStimVal; // originally FBPCByConditionandStimVal
        private bool[] _CorrectByTrial;
        private bool _OutputCountByInterval;

        [Browsable(false)]
        public bool OutputCountByInterval
        {
            get
            {
                return _OutputCountByInterval;
            }
            set
            {
                _OutputCountByInterval = value;
            }
        }


        public int FeedbackFrequency
        {
            get
            {
                return _FeedbackFrequency;
            }

            set
            {
                _FeedbackFrequency = value;
            }
        }


        public uint Repetitions
        {
            get
            {
                return _Repetitions;
            }
            set
            {
                _Repetitions = value;
            }
        }

        public ProbitTypeEnum PerformProbit
        {
            get
            {
                return _PerformProbit;
            }
            set
            {
                _PerformProbit = value;
            }
        }


        public ConstantStimulusPsychComponent()
            : base()
        {

            _StimulusLevels = new List<string>();
            _StimulusValues = new List<double>();
            _Repetitions = 0;
            _FeedbackFrequency = 0;
            _OutputCountByInterval = false;

        }

        protected override void InvokeFeedbackEvent(object sender, FeedbackEventArgs feedbackevent)
        {
            base.InvokeFeedbackEvent(sender, feedbackevent);
        }

        

        public override void SetNextTrial()
        {
            ActiveTrialIndex++;
            if (ActiveTrialIndex==TotalTrialCount) //TODO should this be -1 for 0 based array?
            {
                Status=PsychComponentStatusEnum.Done;
            }
        }
       
        public override void EvaluateResponse(UserResponse response, TrialInstance _trial)
        {

            ConstantTrialInstance trial = (ConstantTrialInstance)_trial; // cast to the correct type for this implementation
            
            StringBuilder sb = new StringBuilder();

            // there is a potential source of confusion here because interval is 1 based whilst all our arrays are zero-based
            // need to make sure the right values are compared etc.

            // log response info
            int ActualChoice = response.SelectedChoice; // this will be a 1-based value because interval is 1 based

            int choiceindex = ActualChoice - 1; // this is the real 0 based index into the Choice arrays 

            this.ChoiceSelectCount[choiceindex]++;
            _CountByChoiceConditionandStimVal[choiceindex,trial.Condition,trial.StimLevel]++; // condition is 0 based


            string ActualName = ChoiceAnswers[choiceindex];

            //sb.AppendLine("Trial\tCondition\tStimLevel\tInterval");
            sb.Append((ActiveTrialIndex+1) + "\t" + (trial.Condition+1) + "\t"  + _StimulusValues[trial.StimLevel] + "\t" + trial.Interval + "\t"); // interval here is 1-based

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

            string choice;
            if (choiceindex < ChoiceAnswers.Count)
                choice = ChoiceAnswers[choiceindex];
            else
                choice = response.SelectedChoice.ToString(); // 1 based

            sb.Append(choice + "\t");
            
            if (this.HasCorrectAnswer == true)
            {
                int CorrectChoice = trial.Interval;  // 1 based
                this.ChoicePresentCount[CorrectChoice-1]++; // 0 based so -1
                string CorrectName = ChoiceAnswers[CorrectChoice-1]; // 0 based
                sb.Append(GetResponseInfo(CorrectChoice, ActualChoice, ActualName, CorrectName)); // write out 1-based values to log file
                bool iscorrect = false;
                if (ActualChoice == CorrectChoice) // Actual choice was correct
                {
                    ProcessCorrectAnswer(trial);
                    iscorrect = true;
                }
                else
                {
                    ProcessWrongAnswer(trial);
                    iscorrect = false;
                }

                // allow feedback to client code if event has been defined
                {
                    if (_FeedbackFrequency > 0 && ActiveTrialIndex >= 0 && (ActiveTrialIndex % _FeedbackFrequency == (_FeedbackFrequency - 1))) // presumably this is because ActiveTrialIndex is 0 based
                    {
                        if (GiveFeedback == true)
                        {
                            FeedbackEventArgs feedbackevent = new FeedbackEventArgs(trial, iscorrect);
                            InvokeFeedbackEvent(this, feedbackevent);
                        }
                    }
                }

            }
            else
            {
                //sb.Append(GetResponseInfo(ActualChoice, ActualName)); // write out 1-based values
                sb.Append("---\t");
            }

            sb.Append("\t" + response.ResponseLatency.ToString("N4") + "\t");
            if (response.UserInput != "")
                sb.Append(response.UserInput);
            sb.AppendLine();
            
            WriteToFile(sb.ToString(),false);

        }

        protected override bool ProcessCorrectAnswer(TrialInstance t)
        {
            _NumberCorrect[t.Condition, t.StimLevel]++; // condition and stimlevel are 0 based
            _CorrectByTrial[ActiveTrialIndex] = true; // this trial was answered correctly
            return true;
        }

        protected override bool ProcessWrongAnswer(TrialInstance t)
        {
            _NumberWrong[t.Condition, t.StimLevel]++; // condition and stimlevel are 0 based
            _CorrectByTrial[ActiveTrialIndex] = false; // this trial was answered wrongly
            return true;
        }

        public override bool Initialise()
        {
            if (!base.Initialise())
                return false;

            if (this._StimulusValues.Count < 1)
            {
                MessageBox.Show("Must have 1 or more stimulus levels to continue", "Not enough stimulus levels", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (this._Repetitions<=0)
            {
                MessageBox.Show("Must have 1 or more repetitions to continue", "Not enough repetitions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // reset Stimulus levels so we don't add too many if we repeat the run
            _StimulusLevels = new List<string>();

            for (int i = 0; i < _StimulusValues.Count; i++)
            {
                _StimulusLevels.Add("Level " + (i + 1));
            }

            _NumberofTrials = ConditionCount * (int)_Repetitions * _StimulusLevels.Count;
            Trials = new List<TrialInstance>(_NumberofTrials);
            _NumberCorrect = new int[ConditionCount,_StimulusLevels.Count];
            _NumberWrong = new int[ConditionCount,_StimulusLevels.Count];
            _CorrectByTrial = new bool[_NumberofTrials];
            
            _CountByChoiceConditionandStimVal=new int[NumberAFC,ConditionCount,_StimulusLevels.Count];
            // initialise all trials at once
            for (int cond=0;cond<Conditions.Count;cond++)
            {
                for (int level=0;level<_StimulusLevels.Count;level++)
                {
                    for (int rep=0;rep<_Repetitions;rep++)
                    {
                        ConstantTrialInstance trial = new ConstantTrialInstance();
                        trial.StimLevel = level;
                        trial.StimVal = StimulusValues[level];
                        trial.Condition = cond;
                        trial.Interval = GetRandomInterval();

                        SetFixedParameters(trial);
                        SetRandomParameters(trial);
                       
                        Trials.Add(trial);
                        trial.InitialIndex = Trials.IndexOf(trial);

                    }

                }
            }

            // randomize order of trials

            for (int i = 0; i < Trials.Count; i++)
            {
                // randomly swap current trial with another trial in the list
                int index = sg.Next(0, Trials.Count); // Next method returns random value upto but not including upper limit
                ConstantTrialInstance trial = (ConstantTrialInstance)Trials[index];
                Trials[index] = Trials[i];
                Trials[i] = trial;
            }

			// set trial number for re-ordered trials
			for (int i = 0; i < Trials.Count; i++)
				Trials[i].TrialNumber = i + 1;

            // reset active trial pointer
            ActiveTrialIndex = 0;


            return true;
        }
        [Browsable(false)]
        public List<string> StimulusLevels
        {
            get
            {
                return _StimulusLevels;
            }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<double> StimulusValues
        {
            get
            {
                return _StimulusValues;
            }
        }

        private string GetStimulusInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.Append("Stimulus values: ");

            for (int i = 0; i < StimulusLevels.Count; i++)
            {
                sb.Append( (i+1) + ": " + _StimulusValues[i] + ";\t");
            }
            sb.AppendLine();
            sb.AppendLine(NumberAFC + "-interval forced choice; " + _Repetitions + " repetitions; total trials = " + _NumberofTrials);

            return sb.ToString();

        }

        
        public override bool Start()
        {
            StringBuilder sb = new StringBuilder();

            // construct logfile header
            sb.Append(GetExperimentHeader());
            sb.Append(GetConditionsDetails());
            sb.Append(GetStimulusInfo());
            sb.Append(GetParametersDetails());
            sb.Append(AddSectionBreak());
            sb.Append(GetTrialHeader());

            WriteToFile(sb.ToString(),true,true);

            // set status to active
            this.Status = PsychComponentStatusEnum.Active;
            return true;

        }

        public override void Stop()
        {

            // stop the experiment and write out the remaining parts of the log file
            string ChoiceName;
            StringBuilder sb=new StringBuilder();
            sb.Append(AddSectionBreak());
            sb.Append(GetChoiceStats());
            sb.Append(AddSectionBreak());

            if (_PerformProbit==ProbitTypeEnum.None)
            {
                for (int i=0;i<ConditionCount;i++)
                {
                    sb.AppendLine("*Condition: " + (i+1) + " : " + Conditions[i]);

                    if (HasCorrectAnswer==true)
                    {
                        sb.AppendLine("StimVal\tN Corr\tTotal");

                        for (int j=0;j<_StimulusLevels.Count;j++)
                        {
                            sb.AppendLine(StimulusValues[j]+"\t"+_NumberCorrect[i,j]+"\t"+(_NumberCorrect[i,j] + _NumberWrong[i,j]));
                        }
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.Append("StimVal\t");
                        for (int j=1;j<=NumberAFC;j++) // NumberAFC is 1 based
                        {
                            int k=j-1; // real index into ChoiceAnswers

                            if (k<ChoiceAnswers.Count)
                                ChoiceName=ChoiceAnswers[k];
                            else
                                ChoiceName="Choice " + j.ToString();
                                sb.Append(ChoiceName);

                            if (j<NumberAFC)
                                sb.Append("\t");
                        }
                        sb.AppendLine();

                    }
                    if (_OutputCountByInterval == true)
                    {

                        sb.Append(AddSectionBreak());
                        sb.AppendLine("Count by Choice and StimVal");
                        sb.Append("Stimval\t");
                        for (int k = 1; k <= NumberAFC; k++)
                            sb.Append("Interval" + k + "\t");
                        sb.AppendLine();
                        
                        for (int j = 0; j < StimulusValues.Count; j++)
                        {
                            sb.Append(StimulusValues[j] + "\t");
                            for (int k = 1; k <= NumberAFC; k++)
                            {
                                sb.Append(_CountByChoiceConditionandStimVal[k - 1, i, j]); // k is 0 based
                                if (k < NumberAFC)
                                    sb.Append("\t");
                                else
                                    sb.AppendLine();
                            }

                        }
                    }
                    sb.Append(AddSectionBreak());

                }

            }
            else // PerformProbit
            {
                DataInput Input=new DataInput(StimulusLevels.Count);

                DataOutput Output=new DataOutput(StimulusLevels.Count);

                for (int i=0;i<ConditionCount;i++)
                {
                    Input.N_points=StimulusLevels.Count;
                    for (int j=0;j<StimulusLevels.Count;j++)
                    {
                        Input.StimLevelArray[j]=StimulusValues[j];
                        Input.ResponseArray[j]=_NumberCorrect[i,j];
                        Input.TotalObsArray[j]=_NumberCorrect[i,j] + _NumberWrong[i,j];
                    }

                    if (_PerformProbit==ProbitTypeEnum.Fifty_100)
                        Input.Fit50_100=true;
                    else
                        Input.Fit50_100=false;

                    Output=new Probit().DoProbit(Input);
                    sb.AppendLine("**\tCondition: " + Conditions[i]);
                    sb.AppendLine("\tStimVal\tN corr\tTotal\tObs%\tPred%");
                    for (int j=0;j<StimulusLevels.Count;j++)
                    {
                        sb.AppendLine("**\t"+StimulusValues[j] + "\t" + _NumberCorrect[i,j] + "\t" + (_NumberCorrect[i, j] + _NumberWrong[i, j]) + "\t" + Output.ObservedArray[j].ToString("N4") + "\t" + Output.PredictArray[j].ToString("N4"));
                    }
                    sb.AppendLine("_______________________________________________________");

                    sb.AppendLine("Threshold\tSE\tChiSq\tChi_Critical");
                    sb.AppendLine(Output.Threshold.ToString("N4") +"\t" + Output.SE.ToString("N4") + "\t" + Output.Chi.ToString("N4") + "\t" + Output.ChiSqrCritical.ToString("N4"));
                    sb.AppendLine("_______________________________________________________");
                    sb.AppendLine("Probit Notes");
                    sb.AppendLine(Output.OutPutString);
                    

                }
                sb.Append(AddSectionBreak());

            }
            sb.AppendLine("*** Experiment finished at " + DateTime.Now.ToString());
            WriteToFile(sb.ToString(),true);

           
        }



    }
}
