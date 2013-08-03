using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Design;
using System.Diagnostics;

using Troschuetz.Random;

namespace PsychComponent
{

    [DesignTimeVisible(false)]
    public class PsychComponentBase : Component
    {


        // declare function prototype for feedback event
        public delegate void FeedbackDelegate(object sender, FeedbackEventArgs e);

        // declare event  of type FeedbackDelegate
        // events can only be invoked from the base class - we need to provide access methods to allow them to be invoked from a derived class
        public event FeedbackDelegate OnFeedback;

        private string _ExperimentDescription;

        private List<TrialInstance> _Trials;

        private bool _GiveFeedback;
        private string _OutputFile;
        private bool _WriteToOutputFile;
        private List<FixedParameter> _FixedParameters; //MPD: I guess these should really be dictionary objects indexed by the name property etc. for clarity
        private List<RandomParameter> _RandomParameters; //MPD: I guess these should really be dictionary objects indexed by the name property etc.
        private List<string> _Conditions;
        private List<string> _ChoiceAnswers;
        private bool _HasCorrectAnswer;
        private PsychComponentStatusEnum _Status;
        private IntervalTypeEnum _IntervalType;
        private int _NumberAFC;
        private List<int> _ChoicePresentCount;
        private List<int> _ChoiceSelectCount;
        private static StandardGenerator _sg;
        private int _ActiveTrialIndex;
        private string _InputData;
        protected const int GaussDistributionCrop = 2;

        public static ContinuousUniformDistribution d = new ContinuousUniformDistribution();
        public static NormalDistribution n = new NormalDistribution();

        // declare protected method which we can use to invoke the feedback event
        protected virtual void InvokeFeedbackEvent(object sender, FeedbackEventArgs feedbackevent)
        {

            if (OnFeedback != null)
            {
                OnFeedback(sender, feedbackevent);
            }
        }

        protected int GetRandomInterval()
        {
            //            byte interval=(byte)Enum.Parse(typeof(IntervalTypeEnum),IntervalType.ToString(),true);
            int i = sg.Next(1, NumberAFC + 1); // sg.Next returns a value less than the MaxValue but does not include MaxValue hence NumberAFC+1 here
            return i;
        }

        protected bool IsFeedbackEventSubscribed()
        {
            if (OnFeedback != null)
                return true;
            else
                return false;
        }

        [Browsable(false)]
        public int ActiveTrialIndex
        {
            get
            {
                return _ActiveTrialIndex;
            }
            set
            {
                _ActiveTrialIndex = value;
            }
        }

        protected List<TrialInstance> Trials
        {
            get
            {
                return _Trials;
            }

            set
            {
                _Trials = value;
            }
        }

        [Browsable(false)]
        public PsychComponentStatusEnum Status
        {
            get
            {
                return _Status;
            }
            protected set
            {
                _Status = value;
            }
        }

        public string ExperimentDescription
        {
            get
            {
                return _ExperimentDescription;
            }
            set
            {
                _ExperimentDescription = value;
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<FixedParameter> FixedParameters
        {
            get
            {
                return _FixedParameters;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor(typeof(ParameterCollectionEditor), typeof(UITypeEditor))]
        public List<RandomParameter> RandomParameters
        {
            get
            {
                return _RandomParameters;
            }
        }

        [Browsable(false)]
        public List<int> ChoiceSelectCount
        {
            get
            {
                return _ChoiceSelectCount;
            }
        }

        [Browsable(false)]
        public List<int> ChoicePresentCount
        {
            get
            {
                return _ChoicePresentCount;
            }
        }


        [Browsable(false)]
        public int TotalTrialCount
        {
            get
            {
                return _Trials.Count;
            }

        }

        public bool GiveFeedback
        {
            get
            {
                return _GiveFeedback;
            }
            set
            {
                _GiveFeedback = value;
            }
        }

        public bool WriteToOutputFile
        {
            get
            {
                return _WriteToOutputFile;
            }
            set
            {
                _WriteToOutputFile = value;
            }
        }


        [Browsable(false)]
        public string OutputFile
        {
            get
            {
                return _OutputFile;
            }
            set
            {
                _OutputFile = value;
            }
        }

        [Category("Counters")]
        public int ConditionCount
        {
            get
            {
                return _Conditions.Count;
            }
        }

        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
"System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> Conditions
        {
            get
            {
                return _Conditions;
            }
        }

        public bool HasCorrectAnswer
        {
            get
            {
                return _HasCorrectAnswer;
            }
            set
            {
                _HasCorrectAnswer = value;
            }
        }

        public IntervalTypeEnum IntervalType
        {
            get
            {
                return _IntervalType;
            }
            set
            {
                _IntervalType = value;
                string iname = Enum.GetName(typeof(IntervalTypeEnum), value);
                _NumberAFC = (int)Enum.Parse(typeof(IntervalTypeEnum), iname); 

            }
        }


        [Category("Counters")]
        public int NumberAFC
        {
            get
            {
                return _NumberAFC; // this value starts at 1 unlike choices which start at 0 - need to be careful of this in the code
            }
        }


        public string InputData
        {
            get
            {
                return _InputData;
            }
            set
            {
                _InputData = value;
            }
        }

        static PsychComponentBase()
        {
            _sg = new StandardGenerator();
        }

        public static StandardGenerator sg
        {
            get
            {
                return _sg;
            }
        }

        public PsychComponentBase()
        {

            _ExperimentDescription = "Description of Experiment";

            _GiveFeedback = false;
            _OutputFile = "";
            _WriteToOutputFile = false;
            _Conditions = new List<string>();
            _FixedParameters = new List<FixedParameter>();
            _RandomParameters = new List<RandomParameter>();
            _ChoiceAnswers = new List<string>();
            IntervalType = IntervalTypeEnum.i2AFC; // set this via the property to set NumberAFC correctly
            _HasCorrectAnswer = true;
            _ActiveTrialIndex = 0;
            _Status = PsychComponentStatusEnum.NotStarted;
            _InputData = "";

        }

        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
"System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> ChoiceAnswers
        {
            get
            {
                return _ChoiceAnswers;
            }
        }

        public virtual void EvaluateResponse(UserResponse response, TrialInstance _trial)
        {
            throw new NotImplementedException("Do not call the base class version of EvaluateResponse - this must be implemented in derived classes");
            //This must be  implemented in derived classes
        }

        public virtual bool Initialise()
        {
            if (this._Conditions.Count == 0)
            {
                MessageBox.Show("Must have 1 or more conditions to continue", "No conditions specified", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (this._ChoiceAnswers.Count < 2)
            {
                MessageBox.Show("Must have 2 or more choices for the user to select from to continue", "No choices (must be more than 2) specified", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (this._WriteToOutputFile == true)
            {
                if (this.OutputFile == "")
                {
                    MessageBox.Show("No output file specified", "No output file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                FileInfo fi = new FileInfo(this._OutputFile);
                if (fi.Exists == true)
                {
                    if (MessageBox.Show("Selected log file already exists, if you continue this file will be overwritten. Do you still wish to continue?", "Logfile exists", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        return false;
                }
                else
                {
                    // validate parent folder exists
                    DirectoryInfo di = new DirectoryInfo(fi.DirectoryName);
                    if (di.Exists == false)
                    {
                        MessageBox.Show("Directory for output file does not exist", "Directory does not exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }


            // Initialise counters for presented choices and selected choices
            _ChoicePresentCount = new List<int>();
            _ChoiceSelectCount = new List<int>();
            for (int i = 0; i < _ChoiceAnswers.Count; i++)
            {
                _ChoicePresentCount.Add(0);
                _ChoiceSelectCount.Add(0);
            }

            return true;

        }

        public virtual bool Start()
        {
            return false;

        }

        public virtual void Stop()
        {

        }

        protected string AddSectionBreak()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("_______________________________________________________");
            return sb.ToString();
        }

        public static double FlatRandom(double UpperLimit,double LowerLimit)
        {
            if (LowerLimit > UpperLimit)
                throw new ArgumentException("Limit values are invalid - lower : " + LowerLimit + " uppper : " + UpperLimit);

            // set valid alpha and beta - the code to set these checks if Alpha <=Beta so this ensures that the function works correctly for any valid alpha and beta
            
			// alpha = lower limit
			// beter = upper limit

            d.Alpha = Math.Min(LowerLimit, d.Alpha);
            d.Beta = Math.Max(UpperLimit, d.Beta);

            d.Alpha = LowerLimit;
            d.Beta = UpperLimit;

			Debug.Assert(d.Alpha == LowerLimit);
			Debug.Assert(d.Beta == UpperLimit);


            return d.NextDouble();

        }

        public static double CroppedRandG(double mean, double SD, double SDRange)
        {
            //TODO: AEW is this correct
            bool valid = false;
            int MaxIterations = 1000; // bail out if we hit this and just return the mean
            int i = 0;
            double result = 0;
            n.Mu = mean;
            n.Sigma = SD;

            while (valid == false)
            {
                i++;
                result = n.NextDouble();
                // check if result is within range
                if (result < (mean - (SDRange * SD)) || result > (mean + (SDRange * SD)))
                    valid = false;
                else
                    valid = true;

                if (i > MaxIterations)
                {
                    result = mean;
                    valid = true;
                }
            }

            return result;

        }

        protected void SetFixedParameters(TrialInstance trial)
        {
            if (_FixedParameters.Count > 0)
            {
                trial.FixedParams = new List<string>();
                for (int i = 0; i < _FixedParameters.Count; i++)
                {
                    if (_FixedParameters[i].Levels.Count > 0)
                    {
                        // set fixed parameter by choosing a level value at random and getting the value from it
                        int paramlevel = sg.Next(0, _FixedParameters[i].Levels.Count);
                        trial.FixedParams.Add(_FixedParameters[i].Levels[paramlevel]);
                    }
                }
            }
        }

        protected void SetRandomParameters(TrialInstance trial)
        {
            // set random parameters
            if (RandomParameters.Count > 0)
            {
                trial.RandomParams = new List<double>();
                for (int i = 0; i < this.RandomParameters.Count; i++)
                {
                    if (this.RandomParameters[i].DistributionType == DistributionTypeEnum.Flat)
                    {
                        RandomFlatParameter r = (RandomFlatParameter)RandomParameters[i];
                        trial.RandomParams.Add(FlatRandom(r.UpperLimit, r.LowerLimit));
                    }
                    else if (this.RandomParameters[i].DistributionType == DistributionTypeEnum.Gaussian)
                    {
                        RandomGaussParameter r = (RandomGaussParameter)RandomParameters[i];
                        trial.RandomParams.Add(CroppedRandG(r.Mean, r.StdDev, GaussDistributionCrop));
                    }
                }

            }

        }

        public virtual void SetNextTrial()
        {

        }

        protected String GetExperimentHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(_ExperimentDescription + ": started at " + DateTime.Now.ToString());
            if (_InputData != "")
                sb.AppendLine(_InputData);
            return sb.ToString();

        }

        protected String GetConditionsDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_Conditions.Count + " conditions: ");
            for (int i=0;i <_Conditions.Count;i ++)
            {
                sb.Append((i+1) + ". " + _Conditions[i] );
                if (i < _Conditions.Count)
                    sb.Append(" ; ");
                
            }
            sb.AppendLine();
            return sb.ToString();

        }

        protected String GetParametersDetails()
        {
            StringBuilder sb = new StringBuilder();
            if (_FixedParameters.Count > 0)
            {
                sb.AppendLine("Fixed parameters:");
                foreach (FixedParameter fp in _FixedParameters)
                {
                    sb.Append(fp.Name);
                    if (fp.Levels.Count > 0)
                    {
                        sb.Append(" with " + fp.Levels.Count + " randomly assigned levels");
                        sb.Append("(");
                        for (int i=0;i<fp.Levels.Count;i++)
                        {
                            string level=fp.Levels[i];
                            sb.Append(level.ToString());
                            if (fp.Levels.IndexOf(level) != fp.Levels.Count - 1)
                                sb.Append(", ");
                        }
                        sb.Append(")");
                        sb.AppendLine();
                    }
                    else
                        sb.AppendLine();
                }

            }

            if (_RandomParameters.Count > 0)
            {
                sb.AppendLine("Random parameters:");
                foreach (RandomParameter rp in _RandomParameters)
                {
                    sb.Append(rp.Name + " : ");
                    if (rp.DistributionType == DistributionTypeEnum.Flat)
                    {
                        RandomFlatParameter r = (RandomFlatParameter)rp;
                        sb.Append(" (flat distribution with range " + r.LowerLimit + " to " + r.UpperLimit + ")");
                    }
                    else
                    {
                        RandomGaussParameter r = (RandomGaussParameter)rp;
                        //TODO: change this when we separate flat from gaussian random parameters
                        sb.Append(" (gaussian distribution with mean " + r.Mean + " and SD " + r.StdDev + " cropped to +/- " + GaussDistributionCrop + " SDs.)");
                    }
                    sb.AppendLine();
                }
            }

            return sb.ToString();

        }

        protected virtual string GetTrialHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#\tCond\tStimLevel\tInterv\t");

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

        protected TrialInstance GetTrial(uint index)
        {
            if (index > _Trials.Count)
                throw new System.ArgumentOutOfRangeException("Trial index out of range");
            else
                return _Trials[(int)index];
        }

        public TrialInstance GetActiveTrial()
        {
            return _Trials[ActiveTrialIndex];
        }

        protected string GetResponseInfo(int Actual, string ActualName)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(ActualName + "(" + Actual.ToString() + ")" + "\t");

            return sb.ToString();
        }

        protected string GetResponseInfo(int Correct, int Actual, string ActualName, string CorrectName)
        {
            StringBuilder sb = new StringBuilder();

//            sb.AppendFormat("Expected - " + CorrectName + "(" + Correct.ToString() + ")" + "\t");
//            sb.AppendFormat(GetResponseInfo(Actual, ActualName));


            if (Actual == Correct)
                sb.Append("Correct");
            else
                sb.Append("Wrong");
            return sb.ToString();

        }

        protected virtual bool ProcessCorrectAnswer(TrialInstance t)
        {
            return false;

        }

        protected virtual bool ProcessWrongAnswer(TrialInstance t)
        {
            return false;
        }

        protected string GetChoiceStats()
        {
            string ChoiceName;
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i <_NumberAFC; i++) // since _NumberAFC starts at 1 but ChoiceSelectCount etc. are zero based arrays we need to make sure i starts at 0
            {
                

                if (i < ChoiceAnswers.Count)
                    ChoiceName = ChoiceAnswers[i];
                else
                    ChoiceName = "Choice " + i.ToString();

                float Percent;

                sb.Append("Choice : " + ChoiceName + " - Chosen " + ChoiceSelectCount[i] + " times ; ");
                if (ChoicePresentCount[i] > 0)
                {
                    Percent = ((float)ChoiceSelectCount[i] / (float)ChoicePresentCount[i]) * (float)100;
                    sb.Append("Presented " + ChoicePresentCount[i] + " times ; ");
                    sb.Append("Percentage selected = " + Percent);
                }
                else
                    sb.Append("Never presented ");

                sb.AppendLine();

            }
            return sb.ToString();

        }
        protected void WriteToFile(string data, bool newline)
        {
            WriteToFile(data, false,newline);
        }
        protected void WriteToFile(string data, bool overwrite,bool newline)
        {
            if (this._WriteToOutputFile == true)
            {
                StreamWriter sw=null;
                try
                {

                    if (File.Exists(_OutputFile) && overwrite == false)
                        sw = File.AppendText(_OutputFile);
                    else
                        sw = File.CreateText(_OutputFile);
                    sw.Write(data);
                    if (newline)
                        sw.WriteLine();
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File error: " + Environment.NewLine + ex.Message + Environment.NewLine + "Writing to output file: " + this.OutputFile, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
        }
    }


    public class FeedbackEventArgs : EventArgs
    {
        private TrialInstance _trial;
        private bool _iscorrect;

        public TrialInstance Trial
        {
            get
            {
                return _trial;
            }
            internal set
            {
                _trial = value;
            }
        }

        public bool IsCorrect
        {
            get
            {
                return _iscorrect;
            }
        }

        private FeedbackEventArgs()
        {

        }
        public FeedbackEventArgs(TrialInstance trial, bool IsCorrect)
        {
            _trial = trial;
            _iscorrect = IsCorrect;
        }
    }

    public class Stats
    {

        public static double StandardDeviation(double[] data)
        {

            double ret = 0;
            double DataAverage = 0;
            double TotalVariance = 0;
            int Max = 0;

            try
            {

                Max = data.Length;

                if (Max == 0)
                {
                    return ret;
                }

                DataAverage = Average(data);

                for (int i = 0; i < Max; i++)
                {
                    TotalVariance += Math.Pow(data[i] - DataAverage, 2);
                }

                ret = Math.Sqrt(SafeDivide(TotalVariance, Max));

            }
            catch (Exception)
            {
                throw;
            }
            return ret;
        }

        public static double Average(double[] data)
        {

            double DataTotal = 0;

            try
            {

                for (int i = 0; i < data.Length; i++)
                {
                    DataTotal += data[i];
                }

                return SafeDivide(DataTotal, data.Length);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static double SafeDivide(double value1, double value2)
        {

            double ret = 0;

            try
            {

                if ((value1 == 0) || (value2 == 0))
                {
                    return ret;
                }

                ret = value1 / value2;

            }
            catch
            {
            }
            return ret;
        }



    }

}
